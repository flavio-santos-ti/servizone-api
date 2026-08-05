-- ServiZone: 017_create_audit_log
-- Log de auditoria imutável de toda ação realizada sobre qualquer recurso da Organization.
-- Distinto do ticket_history (eventos do ciclo de vida de um Ticket específico):
-- o audit_log cobre todos os recursos e inclui rastreabilidade de segurança (ip_address).
-- Registros são IMUTÁVEIS -- nunca atualizados, apenas inseridos.
-- Sem updated_at: imutável por definição. Sem created_at: occurred_at já representa o evento.
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE audit_log (
    id              UUID         PRIMARY KEY,                               -- identificador único gerado pelo backend
    organization_id UUID         NOT NULL REFERENCES organizations(id),     -- tenant: isola registros de auditoria entre organizações
    occurred_at     TIMESTAMPTZ  NOT NULL,                                  -- momento exato em que o evento ocorreu (não confundir com instante de inserção)
    actor_type      VARCHAR(20)  NOT NULL,                                  -- quem agiu: user, system, integration
    actor_id        UUID,                                                   -- id do ator; NULL quando actor_type = system
    actor_name      VARCHAR(200),                                           -- nome do ator no momento do evento; desnormalizado para preservar histórico
    actor_profile   VARCHAR(30),                                            -- perfil do ator no momento do evento, ex: Admin, Manager, Technician
    resource_type   VARCHAR(50)  NOT NULL,                                  -- tipo do recurso afetado: Ticket, Technician, User, Team, Client, Integration, Organization
    resource_id     UUID         NOT NULL,                                  -- id do recurso afetado
    event_type      VARCHAR(100) NOT NULL,                                  -- ação realizada: Created, Updated, Deleted, StatusChanged, etc.
    summary         TEXT         NOT NULL,                                  -- descrição legível do evento, ex: "Status alterado de Em Aberto para Em Atendimento"
    ip_address      VARCHAR(45)                                             -- endereço IPv4 ou IPv6 de origem; coletado para rastreabilidade de segurança (LGPD)
);

CREATE INDEX idx_audit_log_org_occurred ON audit_log (organization_id, occurred_at DESC);
CREATE INDEX idx_audit_log_resource     ON audit_log (organization_id, resource_type, resource_id);
CREATE INDEX idx_audit_log_actor        ON audit_log (organization_id, actor_id) WHERE actor_id IS NOT NULL;
