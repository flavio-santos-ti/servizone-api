-- ServiZone: 001_create_organizations
-- Tabela central de tenants da plataforma.
-- Cada Organization representa um tenant isolado.
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE organizations (
    id              UUID PRIMARY KEY,                        -- identificador único gerado pelo backend
    name            VARCHAR(200) NOT NULL,                   -- razão social ou nome fantasia da organização
    slug            VARCHAR(100) NOT NULL UNIQUE,            -- identificador legível único para URLs e logs
    status          VARCHAR(20)  NOT NULL DEFAULT 'active',  -- active | inactive: controla acesso à plataforma
    config          JSONB        NOT NULL DEFAULT '{}',       -- configurações operacionais da org: raio padrão, campos obrigatórios, etc.
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),     -- data de criação do registro
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT now()      -- data da última atualização
);
