-- ServiZone: 016_create_attendances
-- Registro de execução prática de um Ticket.
-- No MVP cada Ticket possui no máximo um Atendimento (enforced pelo unique index).
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE attendances (
    id                    UUID        PRIMARY KEY,                               -- identificador único gerado pelo backend
    organization_id       UUID        NOT NULL REFERENCES organizations(id),     -- tenant: isola atendimentos entre organizações
    ticket_id             UUID        NOT NULL REFERENCES tickets(id),           -- ticket que originou este atendimento
    technician_id         UUID        NOT NULL REFERENCES users(id),             -- técnico responsável pela execução
    travel_started_at     TIMESTAMPTZ,                                           -- momento em que o técnico iniciou o deslocamento
    attendance_started_at TIMESTAMPTZ,                                           -- momento em que o técnico chegou ao local e iniciou o serviço
    completed_at          TIMESTAMPTZ,                                           -- momento em que o atendimento foi concluído ou encerrado
    notes                 TEXT,                                                  -- observações do técnico durante o atendimento
    completion_notes      TEXT,                                                  -- descrição do que foi realizado; exibida no comprovante
    created_at            TIMESTAMPTZ NOT NULL DEFAULT now(),                    -- data de criação do registro
    updated_at            TIMESTAMPTZ NOT NULL DEFAULT now()                     -- data da última atualização
);

-- MVP: um Atendimento por Ticket
CREATE UNIQUE INDEX idx_attendances_ticket ON attendances (ticket_id);
CREATE INDEX idx_attendances_org ON attendances (organization_id);
