-- ServiZone: 013_create_technician_locations
-- MVP: apenas a localização mais recente por Técnico (substituída a cada atualização).
-- Informação temporal -- localização desatualizada não deve ser usada em distribuição.
-- Chave primária é technician_id (um registro por Técnico).

CREATE TABLE technician_locations (
    technician_id       UUID           PRIMARY KEY REFERENCES users(id),         -- referência ao técnico; PK garante apenas uma localização por técnico
    organization_id     UUID           NOT NULL REFERENCES organizations(id),     -- tenant: necessário para queries de mapa operacional por organização
    latitude            DECIMAL(10, 8) NOT NULL,                                  -- latitude atual do técnico em graus decimais (precisão ~1mm)
    longitude           DECIMAL(11, 8) NOT NULL,                                  -- longitude atual do técnico em graus decimais (precisão ~1mm)
    accuracy_meters     DECIMAL(8, 2),                                            -- precisão do GPS em metros; nulo quando não fornecida pelo dispositivo
    sharing_active      BOOLEAN        NOT NULL DEFAULT true,                     -- indica se o técnico autorizou o compartilhamento de localização
    captured_at         TIMESTAMPTZ    NOT NULL,                                  -- momento da captura no dispositivo móvel (pode diferir de received_at)
    received_at         TIMESTAMPTZ    NOT NULL DEFAULT now()                     -- momento em que a API recebeu a atualização
);

-- Índice para busca por proximidade dentro de uma Organization
CREATE INDEX idx_locations_org ON technician_locations (organization_id)
    WHERE sharing_active = true;
