# ServiZone API — Instruções para GitHub Copilot

## 1. Visão do Projeto

O **ServiZone** é uma plataforma SaaS de **Gestão e Orquestração de Operações em Campo**.

A entidade central é o **Ticket** — unidade de trabalho que representa uma demanda operacional a ser executada em campo (instalação, manutenção, inspeção, assistência técnica, etc.).

Este repositório (`servizone-api`) contém o **backend** da plataforma: API REST consumida pelo frontend web (Angular) e pelo aplicativo mobile (Flutter), além de workers assíncronos para entrega de webhooks, notificações push e geocodificação.

O ServiZone **não é** um ERP, CRM, help desk ou sistema de manutenção. É uma camada complementar especializada na orquestração da execução em campo.

---

## 2. Stack de Tecnologias

| Pacote | Camada | Papel |
|---|---|---|
| ASP.NET Core (.NET 9) | Api | Framework principal |
| Entity Framework Core 9 | Infrastructure | ORM |
| Npgsql.EntityFrameworkCore.PostgreSQL 9 | Infrastructure | Provider PostgreSQL |
| StackExchange.Redis | Api / Application | Cliente Redis |
| System.IdentityModel.Tokens.Jwt | Api | Geração e validação de JWT |
| Microsoft.AspNetCore.Authentication.JwtBearer | Api | Middleware de autenticação JWT |
| BCrypt.Net-Next | Application | Hash de senhas |
| AWSSDK.S3 | Application / Infrastructure | Cloudflare R2 (fotos de Técnicos) |
| QuestPDF | Application | Geração de PDFs |
| Flavio.Santos.NetCore.ApiResponse | Api / Application | Envelope padronizado de resposta |
| Swashbuckle.AspNetCore | Api | OpenAPI / Swagger UI |
| SSH.NET (Renci.SshNet) | Api | Túnel SSH para banco remoto (Development only) |
| Docker / k3s | Infra | Containerização e orquestração |

---

## 3. Arquitetura — Clean Architecture

O projeto segue **Clean Architecture**. As dependências apontam sempre para dentro.

### Solution Folders (fluxo da informação)

```
📁 1 - Api            → ServiZone.Api
📁 2 - Application    → ServiZone.Application
📁 3 - Domain         → ServiZone.Domain
📁 4 - Infrastructure → ServiZone.Infrastructure
📁 5 - Workers        → ServiZone.Workers
📁 Tests
```

### Responsabilidades por Camada

**ServiZone.Api**
- **Minimal API** (sem Controllers/MVC): Endpoints organizados por feature em classes de extensão de `IEndpointRouteBuilder` (ex.: `TicketEndpoints`, `TechnicianEndpoints`), cada uma expondo um método `Map*Endpoints(this IEndpointRouteBuilder app)`
- Route Groups via `app.MapGroup(...)` por feature (prefixo, tags de OpenAPI, políticas de autorização)
- Middleware, filtros globais de exceção, `IEndpointFilter` para validação de request
- Configuração de DI (Program.cs)
- `SshTunnelService` (IHostedService — Development only)
- Registra `Flavio.Santos.NetCore.ApiResponse` nas respostas HTTP

**ServiZone.Application**
- DTOs de entrada (Request) e saída (Response) — pasta/namespace `Dtos`
- Use Cases / Application Services
- Interfaces de serviços externos (`IGeocoder`, `IPushNotificationService`, `IOutboxPublisher`, `IFileStorageService`)
- Hash de senha com `BCrypt.Net-Next`
- Geração de PDF com `QuestPDF`
- Upload de arquivos com `AWSSDK.S3`

**ServiZone.Domain**
- Entidades: `Ticket`, `Organization`, `Technician`, `Team`, `Client`, `Attendance`, `Integration`, `HistoryRecord`
- Value Objects: `Location`, `ServiceAddress`, `Priority`, `ServiceType`, `ExternalId`, `ServiceRadius`, `TechnicianAvailability`
- Interfaces de repositório: `ITicketRepository`, `ITechnicianRepository`, etc.
- Domain Events: `TicketCreated`, `TicketStatusChanged`, `LocationUpdated`
- Nenhuma dependência de EF Core, Redis ou qualquer framework externo

**ServiZone.Infrastructure**
- `ServiZoneDbContext` (EF Core 9 + Npgsql)
- Implementações de repositório
- Redis cache (`StackExchange.Redis`)
- Adapters externos: `GoogleGeocoder`, `FcmPushService`, `CloudflareR2FileStorage`
- Tabela `outbox_events` + publisher atômico (Outbox Pattern)

**ServiZone.Workers**
- `WebhookDeliveryWorker` — polling do outbox, entrega com retentativa exponencial
- `PushNotificationWorker` — entrega de notificações push
- `GeocodingWorker` — geocodificação assíncrona de endereços pendentes

---

## 4. Multi-tenancy

Toda entidade operacional (`Ticket`, `Technician`, `Team`, `Client`, `Integration`) possui `OrganizationId`.

### Regra fundamental
**Nunca** filtre manualmente por `OrganizationId` nas queries. O `ServiZoneDbContext` aplica um **Global Query Filter** em todas as entidades multi-tenant:

```csharp
builder.Entity<Ticket>()
    .HasQueryFilter(t => t.OrganizationId == _currentTenant.OrganizationId);
```

O `OrganizationId` é extraído do JWT pelo `TenantMiddleware` e disponibilizado via `ICurrentTenant`. Nunca receba `OrganizationId` em payloads de request — ele vem sempre do token.

---

## 5. Autenticação

### API Interna (`/api/v1/`)
- **JWT Bearer Token** via `System.IdentityModel.Tokens.Jwt`
- Header: `Authorization: Bearer <token>`
- JWT contém: `sub` (userId), `org` (organizationId), `role` (perfil), `iat`, `exp`
- Refresh token armazenado no Redis com TTL de 30 dias

### API Externa (`/api/ext/v1/`)
- **API Key** via header `X-Api-Key`
- Lookup no banco: hash da chave → `integration_credential` → `organization_id`
- Resultado cacheado no Redis por 5 minutos

### Perfis (RBAC)
- `Gestor` ⊃ `Supervisor` ⊃ `Operador` ⊃ `Técnico`
- Chamadas via API Key representam o perfil `Sistema`

---

## 6. Padrão de Resposta

**Sempre** use `Flavio.Santos.NetCore.ApiResponse` para retornar respostas dos Controllers. Nunca retorne objetos brutos diretamente.

Envelope de sucesso paginado:
```json
{
  "data": [ ... ],
  "pagination": { "page": 1, "pageSize": 20, "totalItems": 150, "totalPages": 8 }
}
```

Envelope de erro:
```json
{
  "error": {
    "code": "TICKET_NOT_FOUND",
    "message": "O Ticket informado não existe ou não pertence à sua Organização.",
    "details": []
  }
}
```

Códigos HTTP padrão:
- `200` — recurso retornado
- `201` — recurso criado
- `204` — ação sem retorno de corpo
- `400` — request malformado
- `401` — token ausente ou inválido
- `403` — sem permissão
- `404` — recurso não encontrado
- `409` — conflito de regra de negócio
- `422` — falha de validação de domínio
- `500` — erro interno

---

## 7. Domínio — Entidades Principais

### Ticket (entidade central)
Unidade de trabalho operacional. Toda demanda, independente da origem, é representada como um Ticket.

**Estados do ciclo de vida:**
```
Recebido → Aberto → AguardandoDistribuicao → DisponibilizadoAoTecnico
    → Aceito → EmDeslocamento → EmAtendimento → Concluido
         ↘ Recusado → (redistribuição)
         ↘ Cancelado (qualquer estado)
```

Transições de status são controladas pela própria entidade `Ticket`. Nenhum código externo altera o status diretamente — use os métodos de domínio.

### Organization
Tenant da plataforma. Delimita o isolamento de todos os dados. Toda entidade operacional pertence a exatamente uma `Organization`.

### Technician
Profissional que executa Tickets em campo. Possui disponibilidade, localização atual (temporal), especialidades e área de atuação.

### Team
Conjunto de Técnicos que opera de forma coordenada. Um Ticket pode ser atribuído a uma Equipe.

### Client
Destinatário do serviço. Pertence a uma Organization. Relacionado ao Local de Atendimento do Ticket.

### Attendance
Representa a execução prática de um Ticket. É um Agregado separado do Ticket para permitir evoluções futuras (múltiplas visitas, atendimentos parciais).

### Integration
Configuração de comunicação entre uma Organization e um sistema externo. Possui API Key para autenticação na API Externa.

### HistoryRecord
Registro imutável de eventos relevantes do ciclo de vida do Ticket. Nunca altere um registro — crie um novo.

---

## 8. Value Objects Principais

- `ServiceAddress` — endereço do Local de Atendimento (preserva o estado no momento do Ticket)
- `GeoCoordinates` — latitude + longitude
- `TechnicianLocation` — coordenadas + timestamp da coleta (informação temporal)
- `ServiceRadius` — distância máxima para Distribuição Inteligente
- `ExternalId` — identificador no Sistema de Origem + referência ao sistema
- `Priority` — grau de urgência do Ticket
- `ServiceType` — classificação operacional da atividade

---

## 9. Invariantes Críticas do Domínio

1. **Isolamento**: dados de uma Organization nunca são visíveis a outra
2. **OrganizationId**: nunca recebido em payload — sempre extraído do token
3. **Status do Ticket**: só evolui por transições permitidas pela máquina de estados
4. **Histórico**: imutável — correções geram novos registros, nunca alteram os existentes
5. **Local de Atendimento**: congelado no momento da criação do Ticket — alterações no cadastro do Cliente não afetam Tickets já criados
6. **Localização do Técnico**: informação temporal — localização desatualizada não deve ser usada em distribuição
7. **Disponibilização ≠ Atribuição**: disponibilizar é encaminhar para avaliação; atribuir é estabelecer responsabilidade (ocorre após Aceite)

---

## 10. Outbox Pattern

Processamento assíncrono de webhooks, push e notificações:

1. Application Service executa a operação principal
2. Adiciona registro em `outbox_events` **na mesma transação**
3. Worker faz polling com `SELECT ... FOR UPDATE SKIP LOCKED`
4. Retentativas exponenciais: imediato → 1min → 5min → 30min → 2h (máx. 5 tentativas)

---

## 11. Regras de Código

### O que SEMPRE fazer
- Defina endpoints com **Minimal API** (`MapGroup`/`MapGet`/`MapPost`/...), organizados por feature em classes de extensão de `IEndpointRouteBuilder`
- DTOs (Request/Response) ficam em `ServiZone.Application`, na pasta/namespace `Dtos` (nunca `DTOs`) — segue a convenção .NET de PascalCase para siglas com 3+ letras (ex.: `Dto`, `Uri`, `Xml`)
- Hash de senha com `BCrypt.Net-Next` na camada Application
- Respostas sempre via `Flavio.Santos.NetCore.ApiResponse`
- Global Query Filter do EF Core gerencia o isolamento multi-tenant
- `appsettings.Development.json` nunca versionado (contém credenciais)
- Túnel SSH (`SshTunnelService`) registrado **somente** em Development

### O que NUNCA fazer
- Não use Controllers baseados em `ControllerBase`/MVC — a API é implementada inteiramente com Minimal API
- Não filtre manualmente por `OrganizationId` em queries
- Não receba `OrganizationId` em payloads de request
- Não retorne objetos brutos nos Controllers — use o envelope de resposta
- Não altere registros de histórico — crie novos
- Não coloque lógica de negócio em Controllers — use Use Cases
- Não referencie EF Core, Redis ou frameworks externos no `Domain`
- Não versione `appsettings.Development.json`, `.env` ou chaves SSH
- Não use EF Core Migrations — o schema é gerenciado exclusivamente por scripts SQL versionados em `database/`
- Não declare `DEFAULT gen_random_uuid()` em colunas `id` — UUIDs são sempre gerados pelo backend (camada Application) antes da persistência

---

## 12. Configuração por Ambiente

**Development**: túnel SSH automático via `SshTunnelService`. Connection string aponta para `localhost:15432` (porta local do túnel).

**Production**: sem túnel. Configuração via variáveis de ambiente injetadas pelo k3s:
- `DATABASE__CONNECTIONSTRING`
- `REDIS__CONNECTIONSTRING`
- `JWT__SECRET`
- `CLOUDFLARE_R2__ACCESSKEY` / `SECRETKEY`

---

## 13. Documentação de Referência

A documentação completa está disponível **localmente** no repositório `servizone`, que deve estar clonado em `C:\workarea\projects\servizone`.

> **Pré-requisito**: clone o repositório de documentação antes de iniciar o desenvolvimento:
> ```bash
> cd C:\workarea\projects
> git clone https://github.com/flavio-santos-ti/servizone.git
> ```

| Documento | Caminho local |
|---|---|
| Visão do Produto | `C:\workarea\projects\servizone\docs\01-visao-do-produto\servizone-01-visao-do-produto.md` |
| Modelo de Domínio | `C:\workarea\projects\servizone\docs\03-modelo-de-dominio\servizone-03-modelo-de-dominio.md` |
| Requisitos Funcionais | `C:\workarea\projects\servizone\docs\04-requisitos-funcionais\servizone-04-00-requisitos-funcionais.md` |
| Regras de Negócio | `C:\workarea\projects\servizone\docs\05-regras-de-negocio\servizone-05-00-regras-de-negocio.md` |
| Fluxos de Negócio | `C:\workarea\projects\servizone\docs\06-fluxos-de-negocio\servizone-06-00-fluxos-de-negocio.md` |
| Arquitetura Backend | `C:\workarea\projects\servizone\docs\08-arquitetura\servizone-08-01-arquitetura-backend.md` |
| Contratos de API | `C:\workarea\projects\servizone\docs\09-contratos-de-api\servizone-09-00-contratos-de-api.md` |
