# FIAP Cloud Games (FCG) — Fase 1

API REST em .NET 8 para cadastro de usuários, autenticação JWT, catálogo de jogos, biblioteca e promoções.

## Objetivos

- Cadastro de usuários (nome, e-mail, senha segura)
- Autenticação JWT com papéis **User** e **Admin**
- Persistência com EF Core + PostgreSQL (migrations)
- Logs estruturados com Serilog → Seq
- Swagger, middleware de erros e testes (TDD no domínio)

## Stack

- .NET 8 / ASP.NET Core Controllers
- Entity Framework Core + PostgreSQL (Npgsql)
- JWT Bearer + BCrypt
- Serilog + Seq
- xUnit, FluentAssertions, NSubstitute
- FluentValidation

## Estrutura

```
src/
  Fcg.Domain
  Fcg.Application
  Fcg.Infrastructure
  Fcg.Api
tests/
  Fcg.Domain.Tests
  Fcg.Application.Tests
docs/
  ddd/
```

## Pré-requisitos

- SDK .NET 8+
- PostgreSQL em execução
- Seq em execução (opcional; logs também vão para o console)

Este repositório **não** provisiona Docker, Postgres nem Seq.

## Configuração

Os arquivos `appsettings.json` e `appsettings.Development.json` **não** contêm senhas nem chaves reais. Configure os valores sensíveis de uma das formas abaixo.

### Opção 1 — User Secrets (recomendado para desenvolvimento local)

Na pasta `src/Fcg.Api`:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=fcg;Username=postgres;Password=SUA_SENHA_POSTGRES"

dotnet user-secrets set "Jwt:SecretKey" "SUA_CHAVE_SECRETA_COM_PELO_MENOS_32_CARACTERES"

dotnet user-secrets set "Seed:AdminPassword" "SUA_SENHA_ADMIN_SEGURA"

# Opcional — apenas se o Seq exigir autenticação
dotnet user-secrets set "Seq:ApiKey" "SUA_API_KEY_SEQ"
```

Para listar os secrets configurados:

```bash
dotnet user-secrets list --project src/Fcg.Api
```

### Opção 2 — Arquivo local (não versionado)

Crie `src/Fcg.Api/appsettings.Development.local.json` (já ignorado pelo `.gitignore`) com os valores sensíveis:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=fcg;Username=postgres;Password=SUA_SENHA_POSTGRES"
  },
  "Jwt": {
    "SecretKey": "SUA_CHAVE_SECRETA_COM_PELO_MENOS_32_CARACTERES"
  },
  "Seed": {
    "AdminPassword": "SUA_SENHA_ADMIN_SEGURA"
  },
  "Seq": {
    "ApiKey": "SUA_API_KEY_SEQ"
  }
}
```

O arquivo é carregado automaticamente pela API em ambiente de desenvolvimento.

### Opção 3 — Variáveis de ambiente

| Variável | Descrição |
|----------|-----------|
| `ConnectionStrings__DefaultConnection` | Connection string do PostgreSQL |
| `Jwt__SecretKey` | Chave simétrica do JWT (mín. 32 caracteres) |
| `Seed__AdminPassword` | Senha do usuário admin criado no seed |
| `Seq__ServerUrl` | URL do Seq (ex.: `http://localhost:5341`) |
| `Seq__ApiKey` | API key do Seq (se necessário) |

### Valores não sensíveis

Estes podem ficar em `appsettings.json` ou `appsettings.Development.json`:

| Chave | Padrão | Descrição |
|-------|--------|-----------|
| `Jwt:Issuer` | `Fcg.Api` | Emissor do token |
| `Jwt:Audience` | `Fcg.Clients` | Audiência do token |
| `Jwt:ExpirationMinutes` | `120` | Validade do token em minutos |
| `Seed:AdminName` | `Administrador FCG` | Nome do admin inicial |
| `Seed:AdminEmail` | `admin@fcg.com` | E-mail do admin inicial |
| `Seq:ServerUrl` | `http://localhost:5341` | Endpoint do Seq |

## Como rodar

```bash
dotnet restore
dotnet build
dotnet ef database update --project src/Fcg.Infrastructure --startup-project src/Fcg.Api
dotnet run --project src/Fcg.Api
```

Na subida, a API tenta aplicar migrations e criar o admin seed se ainda não existir (usando `Seed:AdminEmail` e `Seed:AdminPassword` configurados).

- Swagger (Development): `https://localhost:7xxx/swagger`
- Login do admin: use o e-mail definido em `Seed:AdminEmail` e a senha configurada em `Seed:AdminPassword`

### Nova migration (se alterar o modelo)

```bash
dotnet ef migrations add NomeDaMigration --project src/Fcg.Infrastructure --startup-project src/Fcg.Api --output-dir Persistence/Migrations
```

## Endpoints principais

| Método | Rota | Papel |
|--------|------|-------|
| POST | `/api/auth/register` | anônimo |
| POST | `/api/auth/login` | anônimo |
| GET/PUT/DELETE | `/api/users`, `/api/users/{id}` | Admin |
| GET | `/api/games` | autenticado |
| POST/PUT/DELETE | `/api/games` | Admin |
| GET | `/api/library` | autenticado |
| POST | `/api/library/{gameId}` | User |
| GET | `/api/promotions` | autenticado |
| POST/PUT/DELETE | `/api/promotions` | Admin |

### Exemplos

```bash
# Registro
curl -X POST https://localhost:7xxx/api/auth/register \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Aluno\",\"email\":\"aluno@fiap.com.br\",\"password\":\"Senha@123\"}"

# Login
curl -X POST https://localhost:7xxx/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@fcg.com\",\"password\":\"SUA_SENHA_ADMIN\"}"

# Criar jogo (Admin)
curl -X POST https://localhost:7xxx/api/games \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Clean Code Game\",\"description\":\"Aprenda boas práticas\",\"price\":49.90}"
```

## Testes

```bash
dotnet test
```

O módulo de usuário (e-mail e senha) foi modelado com TDD em `tests/Fcg.Domain.Tests`.

## Documentação DDD

- Event Storming e diagramas: [`docs/ddd/`](docs/ddd/)
