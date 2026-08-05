-- ServiZone: 015_create_outbox_events
-- Tabela do Outbox Pattern para entrega assíncrona de webhooks e notificações push.
-- Registros são gravados atomicamente com a operação principal (mesma transação).
-- Workers fazem polling com SELECT ... FOR UPDATE SKIP LOCKED.
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE outbox_events (
    id              UUID         PRIMARY KEY,                            -- identificador único gerado pelo backend
    organization_id UUID         NOT NULL,                               -- tenant: usado para rastreabilidade; sem FK para evitar lock na tabela organizations
    event_type      VARCHAR(100) NOT NULL,                               -- tipo do evento a entregar: webhook.ticket.created | push.ticket.available | etc.
    payload         JSONB        NOT NULL,                               -- corpo completo do evento serializado para entrega
    status          VARCHAR(20)  NOT NULL DEFAULT 'pending',             -- estado de entrega: pending | processing | delivered | failed
    attempts        INT          NOT NULL DEFAULT 0,                     -- número de tentativas de entrega realizadas até o momento
    next_attempt_at TIMESTAMPTZ  NOT NULL DEFAULT now(),                 -- próximo momento permitido para tentativa de entrega (backoff exponencial)
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),                 -- momento em que o evento foi gravado (mesma transação da operação principal)
    processed_at    TIMESTAMPTZ                                          -- momento da entrega bem-sucedida; nulo enquanto pendente ou com falha
);

-- Índice para polling eficiente pelos Workers (evita full scan)
CREATE INDEX idx_outbox_pending ON outbox_events (next_attempt_at)
    WHERE status IN ('pending', 'processing');
