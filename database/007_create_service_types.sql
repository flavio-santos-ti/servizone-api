-- ServiZone: 007_create_service_types
-- Tipos de serviço configuráveis por Organization.
-- Definem especialidades requeridas e raio de atendimento padrão.
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE service_types (
    id                   UUID         PRIMARY KEY,                               -- identificador único gerado pelo backend
    organization_id      UUID         NOT NULL REFERENCES organizations(id),     -- tenant: isola tipos de serviço entre organizações
    name                 VARCHAR(200) NOT NULL,                                  -- nome do tipo de serviço, único por organização
    description          TEXT,                                                   -- descrição detalhada da atividade a ser executada
    required_specialties JSONB        NOT NULL DEFAULT '[]',                     -- especialidades que o técnico precisa ter para ser elegível
    attendance_radius_km DECIMAL(8, 2),                                          -- raio máximo de atendimento em km para este tipo de serviço; sobrescreve o padrão da org
    active               BOOLEAN      NOT NULL DEFAULT true,                     -- false inativa o tipo sem excluir; tickets existentes não são afetados
    created_at           TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data de criação do registro
    updated_at           TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data da última atualização
    UNIQUE (organization_id, name)
);

CREATE INDEX idx_service_types_org ON service_types (organization_id) WHERE active = true;
