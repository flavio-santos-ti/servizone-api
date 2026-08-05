-- ServiZone: 000_create_database
-- Script executado uma única vez no servidor antes de qualquer outra operação.
-- Deve ser executado pelo administrador do PostgreSQL com permissões de superusuário.
-- Versão do PostgreSQL: 16

CREATE DATABASE servizone
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'pt_BR.utf8'
    LC_CTYPE = 'pt_BR.utf8'
    LOCALE_PROVIDER = libc
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    TEMPLATE = template0;

-- Verificar se o banco foi criado corretamente:
-- SELECT * FROM pg_database WHERE datname = 'servizone';

-- Criar o usuário da aplicação com acesso restrito ao banco.
-- O usuário servizone não deve ter permissão de CREATE, DROP ou ALTER --
-- essas operações são executadas apenas pelo administrador via scripts versionados.
CREATE USER servizone WITH PASSWORD '<senha-forte>';
GRANT CONNECT ON DATABASE servizone TO servizone;
GRANT USAGE ON SCHEMA public TO servizone;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO servizone;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO servizone;
