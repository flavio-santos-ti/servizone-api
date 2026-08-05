# Próximos Passos de Desenvolvimento

Este documento orienta a continuidade do desenvolvimento da API ServiZone após a criação da estrutura inicial do projeto.

---

## ✅ O que já foi implementado

### Estrutura de Projetos

- ✅ Solution `ServiZone.Api.sln` criada
- ✅ Projetos das 4 camadas (Domain, Application, Infrastructure, Api)
- ✅ Projetos de teste (Domain.Tests, Application.Tests, Integration.Tests)
- ✅ Referências entre projetos configuradas corretamente
- ✅ Pacotes NuGet essenciais instalados

### Arquivos Base Criados

**Domain:**

- `Entity.cs` — classe base para todas as entidades
- `TenantEntity.cs` — classe base para entidades multi-tenant
- `Organization.cs` — entidade Organization completa
- `IRepository<T>` — interface base de repositório
- `IOrganizationRepository` — interface específica para Organization
- `ICurrentTenant` — interface para resolução do tenant atual

**Application:**

- `RequestDto.cs` — DTO base para requisições
- `ResponseDto.cs` — DTO base para respostas
- `IGeocoder` — interface para serviço de geocodificação
- `IPushNotificationService` — interface para notificações push

**Infrastructure:**

- `ServiZoneDbContext` — contexto EF Core com Global Query Filter para multi-tenancy
- `OrganizationConfiguration` — configuração EF Core para Organization
- `Repository<T>` — implementação base de repositório
- `OrganizationRepository` — implementação específica para Organization

**Api:**

- `Program.cs` — configuração básica da aplicação
- `CurrentTenant` — implementação de ICurrentTenant
- `SshTunnelOptions` — configuração para túnel SSH (Development)
- `CorrelationIdMiddleware` — middleware para rastreamento de requisições
- `appsettings.json` — configurações gerais
- `appsettings.Development.json.template` — template de configurações de desenvolvimento

---

## 🚧 Próximas Implementações Necessárias

### 1. Completar Entidades do Domínio

Implementar as entidades restantes conforme o modelo de domínio:

**Prioridade Alta:**

- [ ] `Ticket` (entidade central do sistema)
- [ ] `Technician` (agregado de User + TechnicianProfile + TechnicianLocation)
- [ ] `User` (credenciais de autenticação)
- [ ] `Client` (destinatário do serviço)
- [ ] `Team` (grupo de técnicos)

**Prioridade Média:**

- [ ] `Attendance` (execução prática do Ticket)
- [ ] `Integration` (configuração de sistemas externos)
- [ ] `HistoryRecord` (histórico de eventos do Ticket)

**Configuráveis por Organização:**

- [ ] `ServiceType`
- [ ] `Priority`
- [ ] `RefusalReason`

### 2. Implementar Value Objects

Criar os value objects conforme documentação:

- [ ] `TicketStatus` (enum)
- [ ] `GeoCoordinates` (latitude, longitude)
- [ ] `TechnicianLocation` (coordenadas + timestamp)
- [ ] `ServiceAddress` (endereço congelado no momento da criação do Ticket)
- [ ] `ServiceRadius` (distância máxima para distribuição)
- [ ] `ExternalId` (identificador no sistema de origem)

### 3. Completar Repositórios

Para cada entidade, criar:

- Interface no `Domain/Interfaces`
- Implementação no `Infrastructure/Repositories`
- Configuração EF Core no `Infrastructure/Data/Configurations`

### 4. Implementar Autenticação e Autorização

**Autenticação JWT:**

- [ ] `JwtOptions` (POCO de configuração)
- [ ] `JwtService` (geração e validação de tokens)
- [ ] `AuthController` (login, refresh token)
- [ ] Configurar middleware JWT Bearer no `Program.cs`

**Autenticação API Key (sistemas externos):**

- [ ] `ApiKeyAuthenticationHandler`
- [ ] Middleware de validação de API Key
- [ ] Cache de API Keys no Redis

**RBAC:**

- [ ] Policies de autorização (Gestor, Supervisor, Operador, Técnico)
- [ ] Atributos de autorização nos Controllers

### 5. Implementar TenantMiddleware

Criar middleware para extração do `OrganizationId` do JWT e injeção no contexto:

- [ ] `TenantMiddleware.cs`
- [ ] Registrar no pipeline antes dos Controllers
- [ ] Validação de token contendo claim "org"

### 6. Implementar Controllers

Criar controllers RESTful para cada recurso:

**API Interna (`/api/v1/`):**

- [ ] `AuthController` (login, refresh)
- [ ] `OrganizationsController`
- [ ] `TicketsController`
- [ ] `TechniciansController`
- [ ] `TeamsController`
- [ ] `ClientsController`

**API Externa (`/api/ext/v1/`):**

- [ ] `ExternalTicketsController` (criar/atualizar tickets via integração)
- [ ] `ExternalWebhooksController` (receber eventos de sistemas externos)

### 7. Implementar Serviços Externos

**Geocodificação:**

- [ ] `GoogleGeocoder : IGeocoder` (Google Maps API)
- [ ] Registrar no DI

**Push Notifications:**

- [ ] `FcmPushService : IPushNotificationService` (Firebase Cloud Messaging)
- [ ] Registrar no DI

**File Storage:**

- [ ] `IFileStorageService` (interface)
- [ ] `CloudflareR2FileStorage : IFileStorageService` (implementação)

### 8. Implementar Outbox Pattern

**Tabela outbox_events:**

- [ ] Adicionar script SQL `015_create_outbox_events.sql` se ainda não criado
- [ ] Entidade `OutboxEvent` no Domain
- [ ] `IOutboxPublisher` (interface)
- [ ] `OutboxPublisher` (implementação que grava na mesma transação)

**Worker de entrega:**

- [ ] Criar projeto `ServiZone.Workers` (solution separada conforme arquitetura)
- [ ] `WebhookDeliveryWorker : BackgroundService`
- [ ] Polling com `SELECT ... FOR UPDATE SKIP LOCKED`
- [ ] Retentativas com backoff exponencial

### 9. Implementar Cache (Redis)

- [ ] `IRedisCacheService` (interface)
- [ ] `RedisCacheService` (implementação com StackExchange.Redis)
- [ ] Cache de API Keys (5 minutos)
- [ ] Cache de Refresh Tokens (30 dias)
- [ ] Registrar no DI

### 10. Implementar Logging (Serilog)

- [ ] Configurar Serilog no `Program.cs`
- [ ] Sink para console estruturado (JSON)
- [ ] Enrichers: CorrelationId, OrganizationId, ActorType
- [ ] Configuração de níveis por ambiente

### 11. Implementar Túnel SSH (Development)

- [ ] `SshTunnelService : IHostedService`
- [ ] Ler configurações de `SshTunnelOptions`
- [ ] Abrir túneis para PostgreSQL (porta 15432) e Redis (porta 6379)
- [ ] Registrar apenas em ambiente Development

### 12. Implementar Exception Handling

- [ ] `GlobalExceptionFilter : IExceptionFilter`
- [ ] Mapear exceções de domínio para status HTTP apropriados
- [ ] Formato de erro padronizado conforme RNF-INT-009
- [ ] Logging de exceções não tratadas

### 13. Implementar Health Checks

- [ ] Health check para PostgreSQL
- [ ] Health check para Redis
- [ ] Endpoints `/health/live` e `/health/ready`

### 14. Implementar Testes

**Testes de Unidade (Domain):**

- [ ] Testes de entidades
- [ ] Testes de value objects
- [ ] Testes de regras de negócio

**Testes de Unidade (Application):**

- [ ] Testes de use cases
- [ ] Mocks de repositórios e serviços externos

**Testes de Integração:**

- [ ] Testes de controllers
- [ ] Testes de repositórios com banco real (TestContainers)
- [ ] Testes de fluxo completo (E2E)

### 15. Documentação da API

- [ ] Configurar Swagger/OpenAPI com exemplos de requisição/resposta
- [ ] Adicionar comentários XML nos controllers
- [ ] Documentar autenticação JWT e API Key
- [ ] Documentar códigos de erro padrão

---

## 📋 Ordem Recomendada de Implementação

### Sprint 1: Fundação (MVP mínimo funcional)

1. Completar entidades principais (Organization, User, Ticket)
2. Implementar autenticação JWT
3. Implementar TenantMiddleware
4. Criar OrganizationsController e AuthController básicos
5. Implementar exception handling global

### Sprint 2: Tickets (funcionalidade central)

1. Completar entidade Ticket com value objects
2. Implementar TicketsController
3. Criar use cases: CreateTicket, GetTicket, ListTickets
4. Implementar validações de negócio
5. Testes de unidade de Ticket

### Sprint 3: Técnicos e Equipes

1. Implementar entidades Technician, Team, TechnicianProfile
2. Implementar TechniciansController e TeamsController
3. Criar use cases de gestão de técnicos
4. Implementar geocodificação (GoogleGeocoder)

### Sprint 4: Distribuição

1. Implementar lógica de distribuição inteligente
2. Criar use cases de disponibilização e aceite
3. Implementar notificações push (FcmPushService)
4. Testes de fluxo de distribuição

### Sprint 5: Integrações

1. Implementar API externa (/api/ext/v1/)
2. Implementar entidade Integration e credenciais
3. Criar OutboxEvent e OutboxPublisher
4. Implementar WebhookDeliveryWorker (projeto separado)

### Sprint 6: Infraestrutura

1. Implementar cache Redis
2. Implementar file storage (Cloudflare R2)
3. Configurar Serilog completo
4. Implementar health checks
5. Configurar túnel SSH para Development

---

## 🎯 Checklist de Qualidade

Antes de considerar cada sprint concluído:

- [ ] Código compilando sem warnings
- [ ] Testes de unidade passando (coverage mínimo: 80%)
- [ ] Testes de integração passando
- [ ] Documentação XML nos métodos públicos
- [ ] Swagger atualizado com exemplos
- [ ] Código revisado por pelo menos um peer
- [ ] Migrations aplicadas (scripts SQL versionados)
- [ ] Variáveis de ambiente documentadas
- [ ] README atualizado com novos endpoints

---

## 📚 Referências

- [Documentação Completa](C:\workarea\projects\servizone\docs\)
- [Arquitetura Backend](C:\workarea\projects\servizone\docs\08-arquitetura\servizone-08-03-arquitetura-backend.md)
- [Modelo de Domínio](C:\workarea\projects\servizone\docs\03-modelo-de-dominio\servizone-03-modelo-de-dominio.md)
- [Regras de Negócio](C:\workarea\projects\servizone\docs\05-regras-de-negocio\)
- [Scripts SQL](./database/)
