-- ServiZone: 002_create_users
-- Usuários da plataforma (Gestores, Supervisores, Operadores e Técnicos).
-- O papel 'tecnico' possui dados operacionais adicionais em technician_profiles.
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE users (
    id              UUID         PRIMARY KEY,                               -- identificador único gerado pelo backend
    organization_id UUID         NOT NULL REFERENCES organizations(id),     -- tenant: isola usuários entre organizações
    name            VARCHAR(200) NOT NULL,                                  -- nome completo do usuário
    email           VARCHAR(320) NOT NULL,                                  -- e-mail de acesso, único por organização
    password_hash   TEXT         NOT NULL,                                  -- hash bcrypt da senha; nunca armazenar senha em texto claro
    role            VARCHAR(30)  NOT NULL,                                  -- perfil de acesso: gestor | supervisor | operador | tecnico
    status          VARCHAR(20)  NOT NULL DEFAULT 'active',                 -- active | inactive: controla login na plataforma
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data de criação do registro
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data da última atualização
    UNIQUE (organization_id, email)
);

CREATE INDEX idx_users_org_created ON users (organization_id, created_at DESC);
