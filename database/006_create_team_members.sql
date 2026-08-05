-- ServiZone: 006_create_team_members
-- Relacionamento N:N entre Equipes e Técnicos.
-- Um Técnico pode pertencer a mais de uma Equipe.
-- Chave primária composta -- sem UUID próprio.

CREATE TABLE team_members (
    team_id         UUID        NOT NULL REFERENCES teams(id),           -- equipe à qual o técnico pertence
    technician_id   UUID        NOT NULL REFERENCES users(id),           -- técnico membro da equipe
    joined_at       TIMESTAMPTZ NOT NULL DEFAULT now(),                  -- data em que o técnico foi adicionado à equipe
    PRIMARY KEY (team_id, technician_id)
);

CREATE INDEX idx_team_members_technician ON team_members (technician_id);
