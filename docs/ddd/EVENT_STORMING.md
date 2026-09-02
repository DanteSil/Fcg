# Event Storming — FCG Fase 1

Bounded context único do MVP: **IdentityAndCatalog** (usuários, jogos, biblioteca e promoções).

## Fluxo: Criação de usuários

```mermaid
flowchart LR
  ActorUser[Usuario] -->|RegisterUser| CmdRegister[RegisterUser]
  ActorAdmin[Admin] -->|CreateUserOrChangeRole| CmdAdmin[AdministerUser]
  CmdRegister --> AggUser[User Aggregate]
  CmdAdmin --> AggUser
  AggUser -->|UserRegistered| EvtRegistered[UserRegistered]
  EvtRegistered --> PolicyJwt[Emitir JWT]
  AggUser -->|Politica| RuleEmail[Email unico e formato valido]
  AggUser -->|Politica| RulePassword[Senha segura]
```

**Comandos:** `RegisterUser`, `LoginUser`, `UpdateUser`, `DeleteUser`  
**Agregado:** `User` (Name, Email, PasswordHash, Role)  
**Eventos:** `UserRegistered`  
**Regras:** e-mail válido/único; senha ≥ 8 com letras, números e especial; papéis User | Admin.

## Fluxo: Criação de jogos

```mermaid
flowchart LR
  ActorAdmin[Admin] -->|CreateGame| CmdCreate[CreateGame]
  CmdCreate --> AggGame[Game Aggregate]
  AggGame -->|GameCreated| EvtGame[GameCreated]
  EvtGame --> ReadModel[Catalogo de jogos]
  ActorUser[Usuario] -->|AcquireGame| CmdAcquire[AcquireGame]
  CmdAcquire --> AggLibrary[LibraryItem Aggregate]
  AggLibrary -->|GameAcquired| EvtAcquired[GameAcquired]
  ActorAdmin -->|CreatePromotion| CmdPromo[CreatePromotion]
  CmdPromo --> AggPromo[Promotion Aggregate]
  AggPromo -->|PromotionCreated| EvtPromo[PromotionCreated]
```

**Comandos:** `CreateGame`, `UpdateGame`, `DeleteGame`, `AcquireGame`, `CreatePromotion`  
**Agregados:** `Game`, `LibraryItem`, `Promotion`  
**Eventos:** `GameCreated`, `GameAcquired`, `PromotionCreated`  
**Regras:** só Admin cria/edita jogos e promoções; User adquire jogos na própria biblioteca; preço ≥ 0; desconto 0–100%.
