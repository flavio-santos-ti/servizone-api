<p align="center">
  <img src="logo-servizone.png" alt="ServiZone" width="200"/>
</p>

# ServiZone API

Backend da plataforma **ServiZone** — sistema de gestão e orquestração de operações em campo.

Responsável por expor a API REST consumida pelo frontend web e pelo aplicativo mobile, além de processar tarefas assíncronas como entrega de webhooks, notificações push e geocodificação de endereços.

---

## Tecnologias

| Camada | Tecnologia |
|---|---|
| Framework | ASP.NET Core (.NET 9) |
| ORM | Entity Framework Core 9 + Npgsql.EntityFrameworkCore.PostgreSQL 9 |
| Banco de Dados | PostgreSQL |
| Cache | Redis via StackExchange.Redis |
| Autenticação | JWT Bearer Token via System.IdentityModel.Tokens.Jwt |
| Hash de Senha | BCrypt.Net-Next — hashing de senhas na camada Application |
| Armazenamento de Arquivos | Cloudflare R2 via AWS SDK S3 — fotos de perfil de Técnicos |
| Geração de PDF | QuestPDF — relatórios e comprovantes de atendimento |
| Padrão de Resposta | Flavio.Santos.NetCore.ApiResponse — envelope padronizado de retorno da API |
| Background Jobs | .NET BackgroundService (Outbox Pattern) |
| Documentação da API | Swashbuckle.AspNetCore (OpenAPI / Swagger UI) |
| Túnel de Desenvolvimento | SSH.NET (`Renci.SshNet`) — túnel SSH para banco remoto |
| Containerização | Docker / k3s |

---

## Arquitetura

O projeto segue os princípios da **Clean Architecture**, organizado em camadas com separação clara de responsabilidades.

Os projetos são organizados em **Solution Folders** no Visual Studio seguindo o fluxo real da informação — da entrada da requisição até a persistência:

```
ServiZone.sln
│
├── 📁 1 - Api            → ServiZone.Api        (Controllers, Middleware, DI)
├── 📁 2 - Application    → ServiZone.Application (DTOs, Use Cases, interfaces)
├── 📁 3 - Domain         → ServiZone.Domain      (Entidades, Value Objects, regras de negócio)
├── 📁 4 - Infrastructure → ServiZone.Infrastructure (EF Core, Redis, adapters externos)
├── 📁 5 - Workers        → ServiZone.Workers     (Background Services: webhooks, push, geocoding)
│
└── 📁 Tests
    ├── ServiZone.Domain.Tests
    ├── ServiZone.Application.Tests
    └── ServiZone.Integration.Tests
```

> As Solution Folders são virtuais — existem apenas no `.sln` para organização visual no Visual Studio. No disco, os projetos ficam em `src/` e `tests/`.

---

## Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)
- Ferramenta EF Core CLI (instalar uma vez globalmente):

```bash
dotnet tool install --global dotnet-ef --version 9.*
```

- Repositório de documentação clonado localmente em `C:\workarea\projects\servizone`:

```bash
cd C:\workarea\projects
git clone https://github.com/flavio-santos-ti/servizone.git
```

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

| Serviço | Porta local | Porta remota |
|---|---|---|
| PostgreSQL | `localhost:15432` | `<servidor>:5432` |
| Redis | `localhost:6379` | `<servidor>:6379` |

O `SshTunnelService` (implementado como `IHostedService`) faz isso usando a biblioteca **SSH.NET** (`Renci.SshNet`). Em produção o túnel não é ativado — a API conecta diretamente via variáveis de ambiente.

### 5. Execute as migrations

```bash
dotnet ef database update --project src/ServiZone.Infrastructure --startup-project src/ServiZone.Api
```

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

| Seção | Documento |
|---|---|
| Visão do Produto | [servizone-01-visao-do-produto.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/01-visao-do-produto/servizone-01-visao-do-produto.md) |
| Modelo de Domínio | [servizone-03-modelo-de-dominio.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/03-modelo-de-dominio/servizone-03-modelo-de-dominio.md) |
| Requisitos Funcionais | [servizone-04-00-requisitos-funcionais.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/04-requisitos-funcionais/servizone-04-00-requisitos-funcionais.md) |
| Regras de Negócio | [servizone-05-00-regras-de-negocio.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/05-regras-de-negocio/servizone-05-00-regras-de-negocio.md) |
| Fluxos de Negócio | [servizone-06-00-fluxos-de-negocio.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/06-fluxos-de-negocio/servizone-06-00-fluxos-de-negocio.md) |
| Requisitos Não Funcionais | [servizone-07-00-requisitos-nao-funcionais.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/07-requisitos-nao-funcionais/servizone-07-00-requisitos-nao-funcionais.md) |
| Arquitetura — Visão Geral | [servizone-08-00-arquitetura-visao-geral.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/08-arquitetura/servizone-08-00-arquitetura-visao-geral.md) |
| Arquitetura — Backend | [servizone-08-01-arquitetura-backend.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/08-arquitetura/servizone-08-01-arquitetura-backend.md) |
| Contratos de API | [servizone-09-00-contratos-de-api.md](https://github.com/flavio-santos-ti/servizone/blob/main/docs/09-contratos-de-api/servizone-09-00-contratos-de-api.md) |

---

## Repositórios relacionados

| Repositório | Descrição |
|---|---|
| [servizone](https://github.com/flavio-santos-ti/servizone) | Documentação do produto |
| [servizone-web](https://github.com/flavio-santos-ti/servizone-web) | Frontend Web (Angular) |
| [servizone-mobile](https://github.com/flavio-santos-ti/servizone-mobile) | Aplicativo Mobile (Flutter) |
