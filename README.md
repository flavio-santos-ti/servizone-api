<p align="center">
  <img src="logo-servizone.png" alt="ServiZone" width="200"/>
</p>

# ServiZone API

Backend da plataforma **ServiZone** — sistema de gestão e orquestração de operações em campo.

Responsável por expor a API REST consumida pelo frontend web e pelo aplicativo mobile, além de processar tarefas assíncronas como entrega de webhooks, notificações push e geocodificação de endereços.

---

## 📋 Índice

- [Tecnologias](#-tecnologias)
- [Arquitetura](#-arquitetura)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Pré-requisitos](#-pré-requisitos)
- [Configuração](#-configuração)
- [Executando o Projeto](#-executando-o-projeto)
- [Scripts SQL](#-scripts-sql)
- [Multi-tenancy](#-multi-tenancy)
- [Convenções](#-convenções)
- [Documentação Completa](#-documentação-completa)

---

## 🚀 Tecnologias

| Camada           | Tecnologia                              | Versão   |
| ---------------- | --------------------------------------- | -------- |
| Framework        | ASP.NET Core                            | .NET 9   |
| ORM              | Entity Framework Core + Npgsql          | 9.0      |
| Banco de Dados   | PostgreSQL                              | 16       |
| Cache            | Redis via StackExchange.Redis           | 7        |
| Autenticação     | JWT Bearer Token                        | -        |
| Hash de Senha    | BCrypt.Net-Next                         | 4.0.3    |
| Storage          | Cloudflare R2 via AWS SDK S3            | -        |
| Geração de PDF   | QuestPDF                                | 2025.3+  |
| Logging          | Serilog.AspNetCore                      | 9.0      |
| Background Jobs  | .NET BackgroundService (Outbox Pattern) | -        |
| Documentação API | Swashbuckle.AspNetCore (Swagger)        | -        |
| Túnel SSH (Dev)  | SSH.NET (Renci.SshNet)                  | 2024.2.0 |
| Containerização  | Docker / k3s                            | -        |

---

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, com separação clara de responsabilidades em camadas. As dependências apontam sempre para dentro (das camadas externas para o núcleo de domínio).

### Camadas

```
┌─────────────────────────────────────────────────────┐
│              1 - Api (HTTP Entry Point)             │
│  Controllers, Middleware, Filters, DI, Program.cs   │
└─────────────────────┬───────────────────────────────┘
                      │ depende de
┌─────────────────────▼───────────────────────────────┐
│         2 - Application (Use Cases & DTOs)          │
│   Request/Response DTOs, Application Services,      │
│   Interfaces de serviços externos                   │
└─────────────────────┬───────────────────────────────┘
                      │ depende de
┌─────────────────────▼───────────────────────────────┐
│           3 - Domain (Business Rules)               │
│   Entidades, Value Objects, Interfaces, Events      │
│   SEM dependências externas (puro C#)               │
└─────────────────────△───────────────────────────────┘
                      │ implementa
┌─────────────────────┴───────────────────────────────┐
│     4 - Infrastructure (Data & External Services)   │
│  EF Core, Repositórios, Redis, Geocoder, S3, etc.   │
└─────────────────────────────────────────────────────┘
```

---

## 📁 Estrutura do Projeto

```
ServiZone.Api.sln
│
├── src/
│   ├── ServiZone.Api/
│   │   ├── Controllers/          # Endpoints HTTP
│   │   ├── Middleware/           # Middlewares customizados
│   │   ├── Configuration/        # POCOs de configuração
│   │   ├── Services/             # Serviços da camada Api (CurrentTenant, etc.)
│   │   ├── Program.cs            # Entry point e configuração de DI
│   │   └── appsettings.json      # Configurações gerais
│   │
│   ├── ServiZone.Application/
│   │   ├── UseCases/             # Application Services / Use Cases
│   │   ├── DTOs/                 # Request e Response DTOs
│   │   └── Interfaces/           # Interfaces de serviços externos
│   │
│   ├── ServiZone.Domain/
│   │   ├── Entities/             # Entidades de negócio
│   │   ├── ValueObjects/         # Value Objects
│   │   ├── Interfaces/           # Interfaces de repositórios
│   │   └── Events/               # Domain Events
│   │
│   └── ServiZone.Infrastructure/
│       ├── Data/                 # DbContext e Configurations (EF Core)
│       ├── Repositories/         # Implementações de repositórios
│       ├── Cache/                # Implementação de cache (Redis)
│       └── Services/             # Adapters externos (Geocoder, Push, S3)
│
├── tests/
│   ├── ServiZone.Domain.Tests/
│   ├── ServiZone.Application.Tests/
│   └── ServiZone.Integration.Tests/
│
└── database/
    ├── 000_create_database.sql
    ├── 001_create_organizations.sql
    └── ... (scripts SQL versionados)
```

---

## ⚙️ Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL 16](https://www.postgresql.org/download/)
- [Redis 7](https://redis.io/download/)
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

---

## 🔧 Configuração

### 1. Clone o repositório

```bash
git clone https://github.com/seu-usuario/servizone-api.git
cd servizone-api
```

### 2. Configure o banco de dados

Execute os scripts SQL na ordem numérica em `database/`:

```bash
psql -h localhost -U postgres -d servizone -f database/000_create_database.sql
psql -h localhost -U postgres -d servizone -f database/001_create_organizations.sql
# ... e assim por diante
```

> ⚠️ **Importante**: Este projeto **não usa EF Core Migrations**. O schema é gerenciado exclusivamente via scripts SQL versionados.

### 3. Configure as credenciais

Copie o template e preencha com suas credenciais:

```bash
cd src/ServiZone.Api
cp appsettings.Development.json.template appsettings.Development.json
```

Edite `appsettings.Development.json` com suas credenciais reais (PostgreSQL, Redis, JWT Secret, Google Maps API Key, etc.).

> 🔒 **Segurança**: `appsettings.Development.json` está no `.gitignore`. **NUNCA** versione este arquivo.

### 4. Restaure os pacotes

```bash
dotnet restore
```

---

## 🏃 Executando o Projeto

### Modo Development (com túnel SSH)

```bash
cd src/ServiZone.Api
dotnet run
```

A API estará disponível em:

- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5001`
- Swagger UI: `https://localhost:7001/swagger`

### Modo Production (Docker/k3s)

Consulte a [documentação de infraestrutura](../servizone/docs/08-arquitetura/servizone-08-02-arquitetura-infraestrutura.md).

---

## 📊 Scripts SQL

⚠️ **IMPORTANTE**: Este projeto **não utiliza EF Core Migrations**.

Todo o schema do banco de dados é gerenciado exclusivamente via **scripts SQL versionados** localizados em `database/`.

### Por que não usar Migrations?

- **Controle total**: scripts SQL permitem controle total sobre o schema, índices, tipos de dados PostgreSQL específicos (JSONB, TIMESTAMPTZ, UUID).
- **Versionamento explícito**: cada alteração é um arquivo SQL numerado, facilitando auditoria e rollback.
- **Sem dependência do ORM**: o schema não está acoplado ao EF Core ou às classes C#.
- **Colaboração**: DBAs podem revisar e ajustar scripts sem conhecer C# ou EF Core.

### Como aplicar mudanças no schema

1. Crie um novo script SQL em `database/` seguindo a numeração sequencial (ex: `018_create_feedback_table.sql`)
2. Execute o script diretamente no PostgreSQL:
   ```bash
   psql -h localhost -U servizone_dba -d servizone -f database/018_create_feedback_table.sql
   ```
3. Atualize o EF Core Configuration correspondente em `Infrastructure/Data/Configurations/`

---

## 🏢 Multi-tenancy

O ServiZone implementa **multi-tenancy via Schema Compartilhado**:

- Todas as entidades operacionais possuem `OrganizationId`
- O **Global Query Filter** do EF Core aplica isolamento automático
- O `OrganizationId` é extraído do JWT pelo `TenantMiddleware`
- **Nunca** receba `OrganizationId` em payloads — ele vem sempre do token

### Regra de Ouro

**Nunca filtre manualmente por `OrganizationId` em queries LINQ**. O Global Query Filter já faz isso automaticamente.

---

## 📐 Convenções

### Geração de UUIDs

✅ **CORRETO**: UUIDs são gerados pela **camada Application** antes da persistência.

```csharp
var organization = new Organization(Guid.NewGuid(), "Acme Corp", "acme");
```

❌ **INCORRETO**: Nunca use `DEFAULT gen_random_uuid()` no PostgreSQL.

### Entidades

- Toda entidade herda de `Entity` (base) ou `TenantEntity` (multi-tenant)
- Propriedades são `private set` — mutação via métodos de domínio
- Construtor privado sem parâmetros para o ORM
- Construtor público com validações para criação de novas instâncias

### Repositórios

- Interface no `Domain`, implementação na `Infrastructure`
- Métodos assíncronos com `CancellationToken`
- Queries LINQ filtradas automaticamente pelo Global Query Filter

### DTOs

- Request/Response DTOs ficam na `Application`
- Naming: `CreateTicketRequest`, `TicketResponse`
- Validação com Data Annotations ou FluentValidation

---

## 📚 Documentação Completa

A documentação detalhada do projeto está disponível no repositório principal:

📖 [C:\workarea\projects\servizone\docs\](C:\workarea\projects\servizone\docs\)

Principais documentos:

- [Visão do Produto](C:\workarea\projects\servizone\docs\01-visao-do-produto\servizone-01-visao-do-produto.md)
- [Modelo de Domínio](C:\workarea\projects\servizone\docs\03-modelo-de-dominio\servizone-03-modelo-de-dominio.md)
- [Arquitetura de Dados](C:\workarea\projects\servizone\docs\08-arquitetura\servizone-08-01-arquitetura-dados.md)
- [Arquitetura de Infraestrutura](C:\workarea\projects\servizone\docs\08-arquitetura\servizone-08-02-arquitetura-infraestrutura.md)
- [Arquitetura Backend](C:\workarea\projects\servizone\docs\08-arquitetura\servizone-08-03-arquitetura-backend.md)

---

## 📝 Licença

[Definir licença]

---

## 👥 Contribuindo

Consulte [CONTRIBUTING.md](CONTRIBUTING.md) para diretrizes de contribuição.
└── ServiZone.Integration.Tests

````

> As Solution Folders são virtuais — existem apenas no `.sln` para organização visual no Visual Studio. No disco, os projetos ficam em `src/` e `tests/`.

---

## Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Git](https://git-scm.com/)

> Docker **não é necessário** no ambiente local. O banco de dados e o Redis rodam remotamente no servidor e são acessados via túnel SSH. Docker é utilizado apenas no servidor de produção (k3s).

> **EF Core Migrations não são utilizadas neste projeto.** O schema do banco de dados é gerenciado exclusivamente por scripts SQL versionados em `database/`, executados diretamente no PostgreSQL. O EF Core é usado apenas para mapeamento de entidades e consultas.

- Repositório de documentação clonado localmente em `C:\workarea\projects\servizone`:

```bash
cd C:\workarea\projects
git clone https://github.com/flavio-santos-ti/servizone.git
````

> O repositório de documentação é lido diretamente pelo GitHub Copilot a partir do disco local. Mantê-lo atualizado garante que as sugestões do Copilot reflitam sempre a documentação vigente.

---

## Configuração do ambiente local

O ambiente de desenvolvimento conecta ao banco de dados e Redis **remotos** via **túnel SSH**, eliminando a necessidade de rodar PostgreSQL e Redis localmente.

### 1. Clone o repositório

```bash
git clone https://github.com/flavio-santos-ti/servizone-api.git
cd servizone-api
```

### 2. Instale a chave SSH de acesso ao servidor remoto

Obtenha a chave privada `id_ed25519` com o responsável pelo ambiente e salve em:

```
C:\workarea\chaves\api-dev\id_ed25519
```

> A chave deve ter permissão de leitura restrita ao seu usuário. No Windows, basta manter o arquivo fora de pastas públicas.

### 3. Configure o ambiente de desenvolvimento

O arquivo `appsettings.Development.json` (não versionado) deve ser criado em `src/ServiZone.Api/` com a seguinte estrutura:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=127.0.0.1;Port=15432;Database=servizone_dev;Username=servizone;Password=<senha>;Timeout=30;Command Timeout=30"
  },
  "SshTunnel": {
    "Enabled": true,
    "SshHost": "<ip-do-servidor-remoto>",
    "SshPort": 22,
    "SshUsername": "root",
    "SshPrivateKeyPath": "C:\\workarea\\chaves\\api-dev\\id_ed25519",
    "SshPassphrase": "<passphrase-se-houver>",
    "RemoteHost": "127.0.0.1",
    "RemotePort": 5432,
    "LocalPort": 15432,
    "RedisRemotePort": 6379,
    "RedisLocalPort": 6379
  },
  "Redis": {
    "ConnectionString": "127.0.0.1:6379,password=<redis-password>,abortConnect=false,connectTimeout=10000,syncTimeout=10000"
  },
  "Jwt": {
    "SecretKey": "<chave-secreta-minimo-32-chars>",
    "Issuer": "ServiZone.Api",
    "Audience": "ServiZone.Client",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "CloudflareR2": {
    "AccessKey": "<r2-access-key>",
    "SecretKey": "<r2-secret-key>",
    "Endpoint": "https://<account-id>.r2.cloudflarestorage.com",
    "BucketName": "avatars",
    "AccountId": "<account-id>"
  }
}
```

Substitua os valores entre `<>` com as credenciais fornecidas pelo responsável pelo ambiente.

> `appsettings.Development.json` está no `.gitignore` — nunca o versione, pois contém credenciais de acesso.

### 4. Como funciona o túnel SSH

Ao iniciar em modo Development, a API abre automaticamente um túnel SSH que redireciona:

| Serviço    | Porta local       | Porta remota      |
| ---------- | ----------------- | ----------------- |
| PostgreSQL | `localhost:15432` | `<servidor>:5432` |
| Redis      | `localhost:6379`  | `<servidor>:6379` |

O `SshTunnelService` (implementado como `IHostedService`) faz isso usando a biblioteca **SSH.NET** (`Renci.SshNet`). Em produção o túnel não é ativado — a API conecta diretamente via variáveis de ambiente.

### 5. Execute os scripts de banco de dados

Os scripts SQL estão em `database/` e devem ser executados diretamente no PostgreSQL em ordem numérica:

```bash
psql -h 127.0.0.1 -p 15432 -U servizone -d servizone_dev -f database/001_create_organizations.sql
# repetir para cada script na sequência
```

> Execute os scripts com o túnel SSH ativo (passo 4).

### 6. Inicie a API

```bash
dotnet run --project src/ServiZone.Api
```

A API estará disponível em `https://localhost:5001`.

---

## Executando os testes

```bash
# Todos os testes
dotnet test

# Por projeto
dotnet test tests/ServiZone.Domain.Tests
dotnet test tests/ServiZone.Application.Tests
dotnet test tests/ServiZone.Integration.Tests
```

---

## Documentação

A documentação completa do produto está no repositório **[servizone](https://github.com/flavio-santos-ti/servizone)**.

| Seção                     | Documento                                                                                                                                                                              |
| ------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Visão do Produto          | [servizone-01-visao-do-produto.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/01-visao-do-produto/servizone-01-visao-do-produto.md)                                  |
| Modelo de Domínio         | [servizone-03-modelo-de-dominio.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/03-modelo-de-dominio/servizone-03-modelo-de-dominio.md)                               |
| Requisitos Funcionais     | [servizone-04-00-requisitos-funcionais.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/04-requisitos-funcionais/servizone-04-00-requisitos-funcionais.md)             |
| Regras de Negócio         | [servizone-05-00-regras-de-negocio.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/05-regras-de-negocio/servizone-05-00-regras-de-negocio.md)                         |
| Fluxos de Negócio         | [servizone-06-00-fluxos-de-negocio.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/06-fluxos-de-negocio/servizone-06-00-fluxos-de-negocio.md)                         |
| Requisitos Não Funcionais | [servizone-07-00-requisitos-nao-funcionais.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/07-requisitos-nao-funcionais/servizone-07-00-requisitos-nao-funcionais.md) |
| Arquitetura — Visão Geral | [servizone-08-00-arquitetura-visao-geral.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/08-arquitetura/servizone-08-00-arquitetura-visao-geral.md)                   |
| Arquitetura — Backend     | [servizone-08-01-arquitetura-backend.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/08-arquitetura/servizone-08-01-arquitetura-backend.md)                           |
| Contratos de API          | [servizone-09-00-contratos-de-api.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/09-contratos-de-api/servizone-09-00-contratos-de-api.md)                            |

---

## Repositórios relacionados

| Repositório                                                              | Descrição                   |
| ------------------------------------------------------------------------ | --------------------------- |
| [servizone](https://github.com/flavio-santos-ti/servizone)               | Documentação do produto     |
| [servizone-web](https://github.com/flavio-santos-ti/servizone-web)       | Frontend Web (Angular)      |
| [servizone-mobile](https://github.com/flavio-santos-ti/servizone-mobile) | Aplicativo Mobile (Flutter) |
