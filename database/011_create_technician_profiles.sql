-- ServiZone: 011_create_technician_profiles
-- Dados operacionais do Técnico.
-- O usuário de acesso é criado em users (que armazena credenciais);
-- esta tabela armazena os atributos operacionais específicos do Técnico.
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE technician_profiles (
    id              UUID        PRIMARY KEY,                                            -- identificador único gerado pelo backend
    organization_id UUID        NOT NULL REFERENCES organizations(id),                 -- tenant: isola perfis de técnicos entre organizações
    user_id         UUID        NOT NULL REFERENCES users(id),                         -- FK para o usuário de acesso; um técnico é sempre também um usuário
    phone           VARCHAR(30),                                                        -- telefone de contato operacional do técnico
    specialties     JSONB       NOT NULL DEFAULT '[]',                                  -- lista de especialidades técnicas, ex: ["elétrica","hidráulica"]
    service_area    VARCHAR(200),                                                       -- descrição textual da área de cobertura do técnico
    availability    VARCHAR(20) NOT NULL DEFAULT 'available',                           -- estado atual: available, unavailable, busy, off_duty
    active          BOOLEAN     NOT NULL DEFAULT true,                                  -- false desativa o técnico sem excluir; tickets existentes não são afetados
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now(),                                -- data de criação do registro
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT now(),                                -- data da última atualização
    UNIQUE (organization_id, user_id)
);

CREATE INDEX idx_technician_profiles_org ON technician_profiles (organization_id) WHERE active = true;
