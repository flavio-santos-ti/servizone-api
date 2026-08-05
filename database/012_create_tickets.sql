-- ServiZone: 012_create_tickets
-- Entidade central da plataforma. Toda demanda operacional é representada como um Ticket.
-- Depende de: organizations, users, clients, teams, service_types, priorities, integration_credentials.
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE tickets (
    id                  UUID         PRIMARY KEY,                                    -- identificador único gerado pelo backend
    organization_id     UUID         NOT NULL REFERENCES organizations(id),          -- tenant: isola tickets entre organizações
    client_id           UUID         REFERENCES clients(id),                         -- cliente beneficiário do serviço (opcional conforme configuração da org)
    technician_id       UUID         REFERENCES users(id),                           -- técnico atualmente responsável pela execução
    team_id             UUID         REFERENCES teams(id),                           -- equipe atribuída ao ticket (alternativa ao técnico individual)
    service_type_id     UUID         NOT NULL REFERENCES service_types(id),          -- tipo de serviço que determina especialidade e raio de atendimento
    priority_id         UUID         NOT NULL REFERENCES priorities(id),             -- nível de urgência configurado pela organização
    created_by_id       UUID         NOT NULL REFERENCES users(id),                  -- usuário que registrou o ticket na plataforma
    status              VARCHAR(40)  NOT NULL DEFAULT 'received',                    -- estado atual: received | open | waiting_distribution | available_to_technician | accepted | in_transit | in_progress | completed | refused | cancelled
    origin              VARCHAR(20)  NOT NULL DEFAULT 'manual',                      -- origem da demanda: manual (operador) | integration (sistema externo)
    subject             VARCHAR(500) NOT NULL,                                       -- título resumido da demanda
    description         TEXT,                                                        -- detalhamento da demanda informado pelo solicitante
    geocoding_status    VARCHAR(20)  NOT NULL DEFAULT 'pending',                     -- estado da geocodificação: pending | resolved | failed | manual
    address_raw         TEXT,                                                        -- endereço textual conforme informado; base para geocodificação
    address_latitude    DECIMAL(10, 8),                                              -- latitude do local de atendimento (preenchida após geocodificação)
    address_longitude   DECIMAL(11, 8),                                              -- longitude do local de atendimento (preenchida após geocodificação)
    external_id         VARCHAR(200),                                                -- identificador da demanda no sistema de origem (ERP, CRM, etc.)
    external_system_id  UUID         REFERENCES integration_credentials(id),         -- integração que originou o ticket; nulo quando criado manualmente
    scheduled_at        TIMESTAMPTZ,                                                 -- data/hora desejada para execução do serviço
    accepted_at         TIMESTAMPTZ,                                                 -- momento em que o técnico aceitou o ticket
    started_travel_at   TIMESTAMPTZ,                                                 -- momento em que o técnico iniciou o deslocamento
    started_at          TIMESTAMPTZ,                                                 -- momento em que o atendimento foi iniciado em campo
    completed_at        TIMESTAMPTZ,                                                 -- momento em que o atendimento foi concluído
    cancelled_at        TIMESTAMPTZ,                                                 -- momento do cancelamento; nulo se não cancelado
    cancellation_reason TEXT,                                                        -- motivo informado ao cancelar o ticket
    conclusion_data     JSONB,                                                       -- campos de conclusão livres definidos pela organização por tipo de serviço
    created_at          TIMESTAMPTZ  NOT NULL DEFAULT now(),                         -- data de criação do registro
    updated_at          TIMESTAMPTZ  NOT NULL DEFAULT now()                          -- data da última atualização
);

CREATE INDEX idx_tickets_org_status   ON tickets (organization_id, status);
CREATE INDEX idx_tickets_org_created  ON tickets (organization_id, created_at DESC);
CREATE INDEX idx_tickets_technician   ON tickets (organization_id, technician_id)
    WHERE status NOT IN ('concluido', 'cancelado');
CREATE UNIQUE INDEX idx_tickets_external ON tickets (organization_id, external_system_id, external_id)
    WHERE external_id IS NOT NULL;
