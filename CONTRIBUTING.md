# Convenção de Mensagens de Commit

Todas as mensagens de commit devem seguir o padrão abaixo para manter o histórico legível e consistente.

## Formato

```
<emoji> <tipo>: <descrição curta no imperativo>
```

**Exemplos:**
```
✨ feat: adiciona endpoint de criação de ticket
🐛 fix: corrige validação de status no fluxo de aceite
🗃️ db: adiciona script de criação da tabela tickets
```

## Tipos

| Emoji | Tipo | Quando usar |
|---|---|---|
| ✨ | `feat:` | Nova funcionalidade |
| 🐛 | `fix:` | Correção de bug |
| ♻️ | `refactor:` | Refatoração sem mudança de comportamento |
| 📝 | `docs:` | Documentação |
| 🗃️ | `db:` | Scripts de banco de dados |
| ✅ | `test:` | Testes |
| 🔧 | `chore:` | Configuração, dependências, build |

## Regras

- Descrição em **português**, no **imperativo** ("adiciona", "corrige", "remove" — não "adicionado" ou "adicionando")
- Primeira letra minúscula após o tipo
- Sem ponto final
- Máximo de 72 caracteres na linha de título
