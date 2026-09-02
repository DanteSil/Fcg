# Contexto e agregados — DDD FCG

## Context Map (MVP)

```mermaid
flowchart TB
  subgraph identityAndCatalog [IdentityAndCatalog]
    UserAgg[User]
    GameAgg[Game]
    LibraryAgg[LibraryItem]
    PromoAgg[Promotion]
  end
  FutureMatch[Matchmaking futuro]
  FutureServers[Servidores futuros]
  identityAndCatalog -.->|fase seguinte| FutureMatch
  identityAndCatalog -.->|fase seguinte| FutureServers
```

No MVP há um único bounded context monolítico. Matchmaking e servidores ficam fora do escopo desta fase.

## Diagrama de agregados

```mermaid
classDiagram
  class User {
    Guid Id
    string Name
    Email Email
    string PasswordHash
    UserRole Role
    DateTime CreatedAt
  }
  class Email {
    string Value
  }
  class Game {
    Guid Id
    string Title
    string Description
    decimal Price
    DateTime CreatedAt
  }
  class LibraryItem {
    Guid Id
    Guid UserId
    Guid GameId
    DateTime AcquiredAt
  }
  class Promotion {
    Guid Id
    Guid GameId
    decimal DiscountPercent
    DateTime StartsAt
    DateTime EndsAt
  }
  User --> Email : possui
  LibraryItem --> User : referencia
  LibraryItem --> Game : referencia
  Promotion --> Game : referencia
```

## Domain Storytelling (resumo)

1. Um aluno se cadastra com nome, e-mail e senha segura e recebe um JWT de **User**.
2. Um **Admin** autentica e cadastra jogos no catálogo.
3. O aluno lista jogos e adquire um título; o item entra na biblioteca.
4. O Admin cria uma promoção com desconto e período de vigência.
