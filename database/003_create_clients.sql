-- ServiZone: 003_create_clients
-- Destinatários do serviço. Pertencem a uma Organization.
-- Um Cliente pode ter múltiplos locais de atendimento (ver client_service_locations).
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE clients (
    id              UUID         PRIMARY KEY,                               -- identificador único gerado pelo backend
    organization_id UUID         NOT NULL REFERENCES organizations(id),     -- tenant: isola clientes entre organizações
    type            VARCHAR(20)  NOT NULL,                                  -- tipo de pessoa: individual (CPF) | company (CNPJ)
    name            VARCHAR(200) NOT NULL,                                  -- nome completo ou razão social do cliente
    document        VARCHAR(30),                                            -- CPF ou CNPJ; nulo quando não informado
    email           VARCHAR(320),                                           -- e-mail de contato do cliente
    phone           VARCHAR(30),                                            -- telefone de contato do cliente
    external_id     VARCHAR(200),                                           -- id do cliente no sistema de origem quando sincronizado via integração
    active          BOOLEAN      NOT NULL DEFAULT true,                     -- false oculta o cliente de listagens sem excluir o registro
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data de criação do registro
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT now()                     -- data da última atualização
);

CREATE INDEX idx_clients_org ON clients (organization_id) WHERE active = true;
CREATE UNIQUE INDEX idx_clients_external ON clients (organization_id, external_id)
    WHERE external_id IS NOT NULL;
