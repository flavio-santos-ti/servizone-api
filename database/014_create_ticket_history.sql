-- ServiZone: 014_create_ticket_history
-- Registro imutável de eventos do ciclo de vida de um Ticket.
-- Tabela particionada por RANGE em occurred_at para isolar queries recentes de dados históricos.
-- NUNCA atualize um registro -- crie um novo.
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE ticket_history (
    id              UUID         PRIMARY KEY,                               -- identificador único gerado pelo backend
    organization_id UUID         NOT NULL REFERENCES organizations(id),     -- tenant: filtra histórico por organização
    ticket_id       UUID         NOT NULL REFERENCES tickets(id),           -- ticket ao qual este evento pertence
    event_type      VARCHAR(100) NOT NULL,                                  -- tipo do evento: StatusChanged | DistributedManually | TicketAccepted | AttendanceStarted | etc.
    actor_type      VARCHAR(20)  NOT NULL,                                  -- quem originou o evento: user | system | integration
    actor_id        UUID,                                                   -- id do usuário ou integração; nulo quando originado pelo sistema
    actor_label     VARCHAR(200),                                           -- nome legível do ator para exibição no histórico (preservado mesmo se o usuário for removido)
    previous_status VARCHAR(40),                                            -- status anterior ao evento; nulo para eventos que não alteram status
    new_status      VARCHAR(40),                                            -- status resultante do evento; nulo para eventos que não alteram status
    metadata        JSONB,                                                  -- dados adicionais do evento: campos alterados, motivo de recusa, dados de distribuição, etc.
    occurred_at     TIMESTAMPTZ  NOT NULL DEFAULT now()                     -- momento em que o evento ocorreu
) PARTITION BY RANGE (occurred_at);

-- Partições trimestrais -- novas partições criadas por CronJob ou manualmente antes de cada trimestre
CREATE TABLE ticket_history_2026_q3
    PARTITION OF ticket_history
    FOR VALUES FROM ('2026-07-01') TO ('2026-10-01');

CREATE TABLE ticket_history_2026_q4
    PARTITION OF ticket_history
    FOR VALUES FROM ('2026-10-01') TO ('2027-01-01');

CREATE TABLE ticket_history_2027_q1
    PARTITION OF ticket_history
    FOR VALUES FROM ('2027-01-01') TO ('2027-04-01');

CREATE INDEX idx_ticket_history_ticket  ON ticket_history (organization_id, ticket_id);
CREATE INDEX idx_ticket_history_created ON ticket_history (organization_id, occurred_at DESC);
