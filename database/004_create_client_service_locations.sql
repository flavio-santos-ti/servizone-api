-- ServiZone: 004_create_client_service_locations
-- Locais de atendimento cadastrados para um Cliente.
-- Um Cliente pode ter múltiplos locais. O Ticket preserva o endereço usado no
-- momento da criação (campos address_* em tickets); esta tabela é o catálogo de referência.
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE client_service_locations (
    id              UUID         PRIMARY KEY,                               -- identificador único gerado pelo backend
    organization_id UUID         NOT NULL REFERENCES organizations(id),     -- tenant: isola locais entre organizações
    client_id       UUID         NOT NULL REFERENCES clients(id),           -- cliente proprietário deste local de atendimento
    label           VARCHAR(100) NOT NULL,                                  -- nome identificador do local, ex: "Sede", "Filial Norte"
    street          VARCHAR(300),                                           -- logradouro do endereço
    number          VARCHAR(20),                                            -- número do imóvel
    complement      VARCHAR(100),                                           -- complemento, ex: "Sala 5", "Galpão B"
    neighborhood    VARCHAR(100),                                           -- bairro
    city            VARCHAR(100),                                           -- município
    state           VARCHAR(2),                                             -- UF em dois caracteres, ex: "SP"
    zip_code        VARCHAR(10),                                            -- CEP formatado
    country         VARCHAR(2)   NOT NULL DEFAULT 'BR',                     -- código ISO 3166-1 alpha-2 do país
    latitude        DECIMAL(10, 8),                                         -- latitude pré-geocodificada do local; nulo quando não disponível
    longitude       DECIMAL(11, 8),                                         -- longitude pré-geocodificada do local; nulo quando não disponível
    active          BOOLEAN      NOT NULL DEFAULT true,                     -- false oculta o local sem excluir o registro
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data de criação do registro
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT now()                     -- data da última atualização
);

CREATE INDEX idx_client_locations_client ON client_service_locations (client_id) WHERE active = true;
