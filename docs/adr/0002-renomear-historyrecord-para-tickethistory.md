# ADR 0002: Renomear HistoryRecord para TicketHistory

Status: Implementado
Data: 2026-08-07
Implementado em: 2026-08-07

## Decisão de Design: Renomear a entidade HistoryRecord para TicketHistory

## Contexto

A entidade `HistoryRecord` foi implementada conforme ADR 0001 como registro imutável de eventos do ciclo de vida do Ticket. No entanto, o nome genérico `HistoryRecord` pode gerar ambiguidade conforme o sistema evolui:

- Não deixa claro que é específico para Tickets
- Futuramente pode haver histórico de outras entidades (Technician, Team, Attendance)
- Dificulta a identificação do agregado ao qual pertence

## Camadas afetadas

- **Domain** — renomear classe `HistoryRecord` → `TicketHistory`
- **Domain/Interfaces** — renomear interface `IHistoryRecordRepository` → `ITicketHistoryRepository`
- **Infrastructure** (futura) — renomear implementação de repositório quando criada
- **Application** (futura) — renomear DTOs relacionados quando criados

## Novas Entidades/VOs/Interfaces

Nenhuma entidade nova. Apenas renomeação de:

- `HistoryRecord` → `TicketHistory`
- `IHistoryRecordRepository` → `ITicketHistoryRepository`

## Impacto em invariantes existentes

✅ **Nenhum impacto** — a renomeação é puramente semântica. Todas as invariantes permanecem:

- **Invariante 4 (Histórico Imutável):** mantida — `TicketHistory` continua imutável
- **Isolamento multi-tenant:** mantido — continua herdando de `TenantEntity`
- **Relacionamento com Ticket:** mantido — continua com propriedade `TicketId`

## Alternativas consideradas

### Opção A: Manter HistoryRecord

**Prós:**

- Código já implementado
- Nome genérico permite reutilização futura

**Contras:**

- Ambíguo — não deixa claro que pertence ao agregado Ticket
- Pode causar confusão quando houver histórico de outras entidades
- Dificulta leitura do código

### Opção B (Recomendada): Renomear para TicketHistory

**Prós:**

- Explícito e específico
- Segue padrão de nomenclatura clara (`TechnicianLocation`, `ServiceAddress`)
- Facilita identificação do agregado ao qual pertence
- Permite criar futuramente `TechnicianHistory`, `AttendanceHistory`, etc.

**Contras:**

- Requer refatoração (baixo custo — apenas renomeação)

## Recomendação

**Opção B** — Renomear para `TicketHistory`.

**Justificativa:**

1. Clareza é mais importante que brevidade
2. Segue princípio de nomenclatura explícita do DDD
3. Evita ambiguidade futura
4. Baixo custo de refatoração (classe recém-criada, sem uso em Application/Infrastructure ainda)

## Checklist para o Dev .NET

- [ ] Renomear arquivo `src/ServiZone.Domain/Entities/HistoryRecord.cs` → `TicketHistory.cs`
- [ ] Renomear classe `HistoryRecord` → `TicketHistory` no arquivo
- [ ] Atualizar namespace e comentários XML
- [ ] Renomear arquivo `src/ServiZone.Domain/Interfaces/IHistoryRecordRepository.cs` → `ITicketHistoryRepository.cs`
- [ ] Renomear interface `IHistoryRecordRepository` → `ITicketHistoryRepository`
- [ ] Atualizar comentários XML da interface
- [ ] Executar `dotnet build` para garantir que não há erros de compilação
- [ ] Verificar se há referências à classe antiga em outros arquivos (não deve haver, pois Application/Infrastructure ainda não foram implementados)
