-- ServiZone: 009_create_refusal_reasons
-- Motivos de recusa configuráveis por Organization.
-- Técnicos selecionam um code ao recusar um Ticket;
-- o código é registrado no histórico para análise operacional.
-- UUID gerado pelo backend (Application layer) -- sem DEFAULT gen_random_uuid().

CREATE TABLE refusal_reasons (
    id              UUID         PRIMARY KEY,                               -- identificador único gerado pelo backend
    organization_id UUID         NOT NULL REFERENCES organizations(id),     -- tenant: isola motivos de recusa entre organizações
    code            VARCHAR(50)  NOT NULL,                                  -- código interno único por org, ex: "SEM_ACESSO", "FORA_DA_AREA"
    label           VARCHAR(200) NOT NULL,                                  -- descrição exibida na interface para o técnico selecionar
    active          BOOLEAN      NOT NULL DEFAULT true,                     -- false oculta o motivo sem excluir; recusas existentes mantêm referência
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data de criação do registro
    updated_at      TIMESTAMPTZ  NOT NULL DEFAULT now(),                    -- data da última atualização
    UNIQUE (organization_id, code)
);

CREATE INDEX idx_refusal_reasons_org ON refusal_reasons (organization_id) WHERE active = true;
