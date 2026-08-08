# ADR 0001: Implementação das Entidades e Value Objects Restantes do Domínio

Status: Implementado
Data: 2025-01-26
Implementado em: 2026-08-07

> **Nota:** A entidade `HistoryRecord` foi posteriormente renomeada para `TicketHistory` conforme [ADR 0002](0002-renomear-historyrecord-para-tickethistory.md) para maior clareza semântica.

## Decisão de Design: Implementação das Entidades e Value Objects Restantes do Domínio

## Contexto

O projeto ServiZone já possui a estrutura base (`Entity`, `TenantEntity`, `Organization`). Conforme o Modelo de Domínio (seção 7 do documento `03-modelo-de-dominio`), as seguintes entidades principais ainda precisam ser implementadas:

### Entidades Faltantes

1. **Ticket** — entidade central, representa a unidade de trabalho operacional
2. **Technician** — profissional que executa Tickets em campo
3. **Team** — conjunto de Técnicos que opera de forma coordenada
4. **Client** — destinatário do serviço
5. **Attendance** — representa a execução prática de um Ticket (Agregado separado)
6. **Integration** — configuração de comunicação entre Organização e sistema externo
7. **HistoryRecord** — registro imutável de eventos do ciclo de vida do Ticket

### Value Objects Faltantes

- `ServiceAddress` — endereço congelado do Local de Atendimento
- `GeoCoordinates` — latitude + longitude
- `TechnicianLocation` — coordenadas + timestamp (informação temporal)
- `ServiceRadius` — distância máxima para Distribuição Inteligente
- `ExternalId` — identificador no Sistema de Origem + tipo
- `Priority` — grau de urgência do Ticket
- `ServiceType` — classificação operacional da atividade

## Camadas afetadas

### Domain

- **Entidades** (`src/ServiZone.Domain/Entities/`):
  - `Ticket.cs` — herda de `TenantEntity`
  - `Technician.cs` — herda de `TenantEntity`
  - `Team.cs` — herda de `TenantEntity`
  - `Client.cs` — herda de `TenantEntity`
  - `Attendance.cs` — herda de `TenantEntity`
  - `Integration.cs` — herda de `TenantEntity`
  - `HistoryRecord.cs` — herda de `TenantEntity`

- **Value Objects** (`src/ServiZone.Domain/ValueObjects/`):
  - `ServiceAddress.cs`
  - `GeoCoordinates.cs`
  - `TechnicianLocation.cs`
  - `ServiceRadius.cs`
  - `ExternalId.cs`
  - `Priority.cs`
  - `ServiceType.cs`

- **Interfaces de Repositório** (`src/ServiZone.Domain/Interfaces/`):
  - `ITicketRepository.cs`
  - `ITechnicianRepository.cs`
  - `ITeamRepository.cs`
  - `IClientRepository.cs`
  - `IAttendanceRepository.cs`
  - `IIntegrationRepository.cs`

### Application

- DTOs de Request/Response para cada entidade (futura implementação incremental)
- Use Cases/Application Services (futura implementação incremental)

### Infrastructure

- Implementações de repositórios (futura implementação incremental)
- Configuração do `DbContext` com mapeamento EF Core (futura implementação incremental)

## Novas Entidades/VOs/Interfaces

### 1. Ticket

**Responsabilidade**: Representa uma unidade de trabalho operacional; controla seu ciclo de vida através de máquina de estados.

**Propriedades principais**:

- `Subject` (string): assunto do Ticket
- `Description` (string, nullable): descrição detalhada
- `Status` (string): estado atual (enum-like: `Recebido`, `Aberto`, `AguardandoDistribuicao`, `DisponibilizadoAoTecnico`, `Aceito`, `EmDeslocamento`, `EmAtendimento`, `Concluido`, `Recusado`, `Cancelado`)
- `Priority` (Priority VO): prioridade do Ticket
- `ServiceType` (ServiceType VO): tipo de serviço
- `ServiceAddress` (ServiceAddress VO): endereço congelado do Local de Atendimento
- `ClientId` (Guid, nullable): referência ao Cliente
- `IntegrationId` (Guid, nullable): referência à Integração de origem
- `ExternalId` (ExternalId VO, nullable): identificador no sistema externo
- `AssignedTechnicianId` (Guid, nullable): Técnico atribuído após Aceite
- `AssignedTeamId` (Guid, nullable): Equipe atribuída

**Invariantes**:

- Status só pode evoluir por transições permitidas (máquina de estados)
- `ServiceAddress` é imutável após criação (congelamento do contexto)
- `OrganizationId` nunca pode ser alterado

**Métodos de domínio** (exemplos):

- `Open()` — transição de `Recebido` → `Aberto`
- `MakeAvailableForDistribution()` — transição → `AguardandoDistribuicao`
- `OfferToTechnician(Guid technicianId)` — transição → `DisponibilizadoAoTecnico`
- `Accept(Guid technicianId)` — transição → `Aceito`
- `Refuse(Guid technicianId, string reason)` — transição → `Recusado`
- `StartTravel()` — transição → `EmDeslocamento`
- `StartAttendance()` — transição → `EmAtendimento`
- `Complete()` — transição → `Concluido`
- `Cancel(string reason)` — transição → `Cancelado`

### 2. Technician

**Responsabilidade**: Representa profissional de campo; mantém disponibilidade e localização temporal.

**Propriedades principais**:

- `Name` (string): nome completo
- `Email` (string): e-mail de contato
- `Phone` (string, nullable): telefone
- `Status` (string): situação operacional (`active`, `inactive`, `unavailable`)
- `CurrentLocation` (TechnicianLocation VO, nullable): localização atual com timestamp
- `ServiceRadius` (ServiceRadius VO, nullable): raio máximo de atuação
- `Specialties` (string, JSON): lista de especialidades
- `WorkingArea` (string, nullable, JSON): área geográfica de atuação

**Métodos de domínio**:

- `UpdateLocation(GeoCoordinates coordinates)` — atualiza localização com timestamp atual
- `Activate()`, `Deactivate()`, `SetUnavailable()`

### 3. Team

**Responsabilidade**: Agrupa Técnicos que operam de forma coordenada.

**Propriedades principais**:

- `Name` (string): nome da Equipe
- `Status` (string): situação operacional (`active`, `inactive`)
- `Specialties` (string, JSON): especialidades da Equipe
- `WorkingArea` (string, nullable, JSON): área de atuação

**Relacionamentos**:

- Relacionamento N:N com `Technician` (tabela associativa `team_technician`)

### 4. Client

**Responsabilidade**: Representa destinatário do serviço.

**Propriedades principais**:

- `Name` (string): nome ou razão social
- `DocumentNumber` (string, nullable): CPF/CNPJ
- `Email` (string, nullable): e-mail de contato
- `Phone` (string, nullable): telefone
- `DefaultAddress` (ServiceAddress VO, nullable): endereço principal

### 5. Attendance

**Responsabilidade**: Representa execução prática de um Ticket (Agregado separado conforme decisão 19.9 do modelo de domínio).

**Propriedades principais**:

- `TicketId` (Guid): referência ao Ticket (1:1 no MVP)
- `TechnicianId` (Guid): Técnico executor
- `TeamId` (Guid, nullable): Equipe executora
- `StartedAt` (DateTime, nullable): início do Atendimento
- `CompletedAt` (DateTime, nullable): conclusão do Atendimento
- `Notes` (string, nullable): observações operacionais
- `Status` (string): situação (`in_progress`, `completed`, `cancelled`)

### 6. Integration

**Responsabilidade**: Configuração de comunicação entre Organização e sistema externo.

**Propriedades principais**:

- `Name` (string): nome da Integração
- `SystemType` (string): tipo do sistema externo (`erp`, `crm`, `itsm`, `custom`)
- `Status` (string): situação operacional (`active`, `inactive`)
- `ApiKeyHash` (string): hash da API Key para autenticação
- `Config` (string, JSON): configurações específicas da integração

**Métodos de domínio**:

- `GenerateApiKey()` — gera nova API Key e armazena seu hash (BCrypt)
- `ValidateApiKey(string apiKey)` — valida API Key fornecida

### 7. HistoryRecord

**Responsabilidade**: Registro imutável de eventos do ciclo de vida do Ticket.

**Propriedades principais**:

- `TicketId` (Guid): referência ao Ticket
- `EventType` (string): tipo do evento (`status_changed`, `assigned`, `location_updated`, `note_added`, etc.)
- `OldValue` (string, nullable, JSON): valor anterior
- `NewValue` (string, nullable, JSON): novo valor
- `PerformedBy` (string): identificador de quem realizou a ação
- `PerformedAt` (DateTime): timestamp do evento
- `Notes` (string, nullable): observações adicionais

**Invariante**:

- Uma vez criado, nunca pode ser alterado ou deletado

## Value Objects

### ServiceAddress

```csharp
public record ServiceAddress(
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    string PostalCode,
    string Country,
    GeoCoordinates? Coordinates
);
```

### GeoCoordinates

```csharp
public record GeoCoordinates(double Latitude, double Longitude)
{
    // Validação: Latitude [-90, 90], Longitude [-180, 180]
}
```

### TechnicianLocation

```csharp
public record TechnicianLocation(GeoCoordinates Coordinates, DateTime CapturedAt)
{
    public bool IsStale(TimeSpan threshold) => DateTime.UtcNow - CapturedAt > threshold;
}
```

### ServiceRadius

```csharp
public record ServiceRadius(double RadiusInKilometers)
{
    // Validação: RadiusInKilometers > 0
}
```

### ExternalId

```csharp
public record ExternalId(string SystemType, string Value);
```

### Priority

```csharp
public record Priority(string Value)
{
    // Validação: Value in ["low", "normal", "high", "urgent"]
    public static Priority Low => new("low");
    public static Priority Normal => new("normal");
    public static Priority High => new("high");
    public static Priority Urgent => new("urgent");
}
```

### ServiceType

```csharp
public record ServiceType(string Value)
{
    // Validação: Value não-vazio
    // Ex: "installation", "maintenance", "inspection", "support"
}
```

## Impacto em invariantes existentes

### Invariante 1: Isolamento Multi-tenant

✅ **Preservada**: Todas as entidades herdam de `TenantEntity`, garantindo o filtro global por `OrganizationId`.

### Invariante 3: Status do Ticket — Máquina de Estados

✅ **Garantida pelo design**: `Ticket` expõe apenas métodos de domínio para transições (ex.: `Open()`, `Accept()`, `Complete()`). A propriedade `Status` tem setter privado.

### Invariante 4: Histórico Imutável

✅ **Garantida**: `HistoryRecord` não expõe métodos de mutação. Após criação via construtor, nenhuma propriedade pode ser alterada.

### Invariante 5: Local de Atendimento Congelado

✅ **Garantida**: `ServiceAddress` é um `record` imutável e é armazenado como propriedade do `Ticket`, não como referência ao `Client.DefaultAddress`.

### Invariante 6: Localização Temporal do Técnico

✅ **Garantida**: `TechnicianLocation` é um `record` que inclui `CapturedAt` e fornece método `IsStale()` para validação temporal.

### Invariante 7: Disponibilização ≠ Atribuição

✅ **Garantida pelo design**: `Ticket` possui métodos separados:

- `OfferToTechnician()` — apenas disponibiliza (status `DisponibilizadoAoTecnico`)
- `Accept()` — estabelece atribuição (status `Aceito` + preenche `AssignedTechnicianId`)

## Alternativas consideradas

### Opção A: Implementar todas as entidades de uma vez

**Prós**:

- Visão completa do modelo de domínio
- Reduz retrabalho futuro

**Contras**:

- Alto risco de sobrecarga cognitiva
- Dificulta validação incremental
- Atrasa entrega de funcionalidades prioritárias

### Opção B: Implementar entidades incrementalmente por feature

**Prós**:

- Entrega valor funcional rapidamente
- Facilita testes e validação
- Reduz complexidade de cada iteração

**Contras**:

- Pode gerar dependências não previstas
- Requer coordenação cuidadosa entre iterações

### Opção C (Recomendada): Implementar estrutura base + iterações por Agregado

**Prós**:

- Estrutura completa do domínio disponível desde o início
- Implementações detalhadas (Application/Infrastructure) podem ser iterativas
- Reduz acoplamento temporal entre features
- Facilita trabalho paralelo de múltiplos desenvolvedores

**Contras**:

- Requer esforço inicial maior de design
- Pode criar classes "vazias" temporariamente

## Recomendação

**Opção C**: Implementar a estrutura completa das entidades e Value Objects no Domain, permitindo que a camada Application e Infrastructure evoluam incrementalmente.

**Justificativa**:

1. O Domain deve ser estável e completo — representa a linguagem ubíqua
2. Application Services (Use Cases) podem ser criados conforme prioridade de negócio
3. Reduz risco de refatorações estruturais futuras
4. Facilita evolução futura (ex.: múltiplas visitas em `Attendance`)

## Ordem de implementação sugerida

### Fase 1: Value Objects (sem dependências)

1. `GeoCoordinates`
2. `ServiceRadius`
3. `Priority`
4. `ServiceType`
5. `ExternalId`
6. `ServiceAddress` (depende de `GeoCoordinates`)
7. `TechnicianLocation` (depende de `GeoCoordinates`)

### Fase 2: Entidades base (dependências mínimas)

1. `Client`
2. `Integration`
3. `Team`
4. `Technician`

### Fase 3: Entidades centrais (dependem das anteriores)

1. `Ticket` (depende de VOs + `Client` + `Integration` + `Technician` + `Team`)
2. `Attendance` (depende de `Ticket` + `Technician` + `Team`)
3. `HistoryRecord` (depende de `Ticket`)

### Fase 4: Interfaces de Repositório

- Criar todas as interfaces de repositório em `ServiZone.Domain/Interfaces/`

## Checklist para o Dev .NET

- [ ] Criar pasta `src/ServiZone.Domain/ValueObjects/`
- [ ] Implementar todos os Value Objects (Fase 1) como `record` imutáveis
- [ ] Adicionar validação nos construtores dos VOs (ex.: `GeoCoordinates` valida lat/lng)
- [ ] Implementar entidades base (Fase 2) em `src/ServiZone.Domain/Entities/`
- [ ] Implementar `Ticket` com máquina de estados (Fase 3)
- [ ] Implementar `Attendance`, `HistoryRecord` (Fase 3)
- [ ] Criar interfaces de repositório em `src/ServiZone.Domain/Interfaces/`
- [ ] Garantir que todos os setters de propriedades críticas sejam `private set`
- [ ] Garantir que `Ticket.Status` só muda via métodos de domínio (ex.: `Accept()`, `Complete()`)
- [ ] Adicionar testes unitários de domínio em `tests/ServiZone.Domain.Tests/`
- [ ] Validar que `HistoryRecord` não expõe métodos de mutação
- [ ] Validar que `ServiceAddress` é imutável (record)
- [ ] Validar que `TechnicianLocation.IsStale()` funciona corretamente

## Observações

- **Não criar migrations do EF Core** — o schema será gerenciado por scripts SQL em `database/`
- **UUIDs gerados na Application** — nunca use `DEFAULT gen_random_uuid()` no banco
- **Global Query Filter** — o `DbContext` será responsável por aplicar o filtro de `OrganizationId`
- **Relacionamentos N:N** — tabela `team_technician` deve ser criada via script SQL
- **Histórico imutável** — `HistoryRecord` nunca deve ter métodos `Update()` ou `Delete()`

---

## Validação

**Status:** ✅ Aprovado  
**Data:** 2026-08-07  
**Validador:** Agente QA  
**Build:** Sucesso (0 erros, 0 warnings)

### Critérios de Aceite Validados

✅ **1. Isolamento multi-tenant**

- Todas as 7 entidades operacionais herdam de `TenantEntity` com `OrganizationId protected`
- Global Query Filter será aplicado pelo `DbContext` (verificado em design)

✅ **2. Máquina de estados do Ticket**

- Propriedade `Status` com setter `private`
- 9 métodos de transição implementados: `Open()`, `MakeAvailableForDistribution()`, `OfferToTechnician()`, `Accept()`, `Refuse()`, `StartTravel()`, `StartAttendance()`, `Complete()`, `Cancel()`
- Cada transição valida estado atual (throw `InvalidOperationException` se inválida)

✅ **3. Histórico imutável**

- `TicketHistory` sem métodos de mutação
- Comentário explícito no código confirmando imutabilidade
- Propriedades com setter `private`

✅ **4. Local de Atendimento congelado**

- `ServiceAddress` implementado como `record` imutável
- Propriedades com `init` (imutáveis após inicialização)
- `Ticket.ServiceAddress` com setter `private`

✅ **5. Localização temporal**

- `TechnicianLocation` como `record` com propriedade `CapturedAt`
- Método `IsStale(TimeSpan threshold)` implementado

✅ **6. Disponibilização ≠ Atribuição**

- Métodos separados com semântica correta:
  - `OfferToTechnician()` → status `DisponibilizadoAoTecnico`, **sem** preencher `AssignedTechnicianId`
  - `Accept()` → status `Aceito`, **e** preenche `AssignedTechnicianId`

✅ **7. Value Objects imutáveis**

- Todos os 7 VOs declarados como `record`: `GeoCoordinates`, `ServiceRadius`, `Priority`, `ServiceType`, `ExternalId`, `ServiceAddress`, `TechnicianLocation`
- Propriedades com `init`

✅ **8. Validações nos construtores**

- Todos VOs e entidades validam invariantes nos construtores
- Lançam `ArgumentException` com mensagens descritivas
- Verificado: `GeoCoordinates`, `ServiceRadius`, `Priority`, `ServiceType`, `ExternalId`, `ServiceAddress`, `TechnicianLocation`

✅ **9. Setters privados**

- Propriedades críticas em todas entidades possuem `private set`
- Alterações apenas via métodos de negócio

✅ **10. Domain sem dependências externas**

- `.csproj` não possui referências a EF Core, Redis, BCrypt
- Grep confirmou: zero imports de frameworks externos em arquivos `.cs` do Domain

### Invariantes Críticas Verificadas

| Invariante                              | Status | Evidência                              |
| --------------------------------------- | ------ | -------------------------------------- |
| Isolamento multi-tenant                 | ✅     | Herança de `TenantEntity`              |
| Status do Ticket por métodos de domínio | ✅     | Setter `private` + métodos validados   |
| Histórico imutável                      | ✅     | `TicketHistory` sem métodos de mutação |
| Local de Atendimento congelado          | ✅     | `ServiceAddress` `record` imutável     |
| Localização temporal                    | ✅     | `TechnicianLocation.IsStale()`         |
| Disponibilização ≠ Atribuição           | ✅     | Métodos separados                      |

### Evidências

- Build log: `dotnet build src/ServiZone.Domain/ServiZone.Domain.csproj` — sucesso em 1,4s
- 21 arquivos implementados conforme especificação ADR 0001
- Zero violações de invariantes críticas
- Código idiomático, bem documentado, seguindo padrões .NET

### Conclusão

A implementação da camada Domain está **completa, correta e em conformidade total com o ADR 0001**. Todos os Critérios de Aceite foram atendidos integralmente. A feature está pronta para prosseguir para a próxima fase (camada Application/Infrastructure).

---

## Referências

- [Modelo de Domínio](C:\workarea\projects\servizone\docs\03-modelo-de-dominio\servizone-03-modelo-de-dominio.md)
- [Copilot Instructions](.....github\copilot-instructions.md) — Seção 9 (Invariantes Críticas do Domínio)
