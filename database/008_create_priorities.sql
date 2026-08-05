-- ServiZone: 008_create_priorities
-- Níveis de prioridade configuráveis por Organization.
-- sort_order define a ordenação de exibição; is_default indica a prioridade padrão ao criar Tickets.
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE priorities (
    id              UUID        PRIMARY KEY,                                -- identificador único gerado pelo backend
    organization_id UUID        NOT NULL REFERENCES organizations(id),      -- tenant: isola níveis de prioridade entre organizações
    name            VARCHAR(100) NOT NULL,                                  -- nome exibido na interface, ex: "Alta", "Normal", "Baixa"
    sort_order      INT          NOT NULL,                                  -- ordem de exibição; menor valor = maior urgência
    is_default      BOOLEAN      NOT NULL DEFAULT false,                    -- prioridade aplicada automaticamente quando não informada na criação do ticket
    active          BOOLEAN      NOT NULL DEFAULT true,                     -- false oculta a prioridade sem excluir; tickets existentes não são afetados
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data de criação do registro
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data da última atualização
    UNIQUE (organization_id, name)
);

CREATE INDEX idx_priorities_org ON priorities (organization_id, sort_order) WHERE active = true;
