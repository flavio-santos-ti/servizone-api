-- ServiZone: 010_create_integration_credentials
-- Credenciais de integração de sistemas externos com a Organization.
-- A API Key NUNCA é armazenada em texto claro -- apenas seu hash (bcrypt).
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE integration_credentials (
    id                  UUID         PRIMARY KEY,                               -- identificador único gerado pelo backend
    organization_id     UUID         NOT NULL REFERENCES organizations(id),     -- tenant: isola credenciais entre organizações
    name                VARCHAR(200) NOT NULL,                                  -- nome amigável da integração, ex: "ERP Totvs Produção"
    system_type         VARCHAR(100),                                           -- categoria do sistema externo: ERP, CRM, ITSM, outro
    api_key_hash        TEXT         NOT NULL,                                  -- hash bcrypt da API Key; a chave em texto claro nunca é armazenada
    permissions         JSONB        NOT NULL DEFAULT '[]',                     -- lista de escopos concedidos, ex: ["tickets:read","tickets:write"]
    webhook_url         TEXT,                                                   -- endpoint do sistema externo que receberá as notificações
    webhook_secret      TEXT,                                                   -- secret para assinatura HMAC-SHA256 do payload enviado
    subscribed_events   JSONB        NOT NULL DEFAULT '[]',                     -- eventos que disparam webhook, ex: ["ticket.created","ticket.closed"]
    active              BOOLEAN      NOT NULL DEFAULT true,                     -- false suspende a integração sem excluir as credenciais
    last_used_at        TIMESTAMPTZ,                                            -- última vez que a API Key foi usada; útil para auditoria de integrações ativas
    created_at          TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data de criação do registro
    updated_at          TIMESTAMPTZ  NOT NULL DEFAULT now()                     -- data da última atualização
);

CREATE INDEX idx_integration_credentials_org ON integration_credentials (organization_id) WHERE active = true;
