-- ServiZone: 005_create_teams
-- Equipes operacionais de uma Organization.
-- Um Técnico pode pertencer a mais de uma Equipe (ver team_members).
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE teams (
    id              UUID         PRIMARY KEY,                               -- identificador único gerado pelo backend
    organization_id UUID         NOT NULL REFERENCES organizations(id),     -- tenant: isola equipes entre organizações
    name            VARCHAR(200) NOT NULL,                                  -- nome da equipe, único por organização
    specialties     JSONB        NOT NULL DEFAULT '[]',                     -- lista de especialidades da equipe usada nos critérios de elegibilidade
    service_area    VARCHAR(200),                                           -- descrição textual da área geográfica de atuação
    active          BOOLEAN      NOT NULL DEFAULT true,                     -- false inativa a equipe sem excluir o registro
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data de criação do registro
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data da última atualização
    UNIQUE (organization_id, name)
);

CREATE INDEX idx_teams_org ON teams (organization_id) WHERE active = true;
