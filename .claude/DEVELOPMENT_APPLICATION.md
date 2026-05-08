# Application — Guia de Desenvolvimento

## Responsabilidade da Camada

Orquestra os casos de uso do sistema. Recebe dados validados, aplica regras de negócio usando as entidades do Domain, e coordena repositórios e serviços externos — sem conhecer detalhes de infraestrutura.

## Dependências Permitidas

- `ReservaCinema.Domain` — entidades e exceções de domínio
- Pacotes de validação (`FluentValidation`)
- Interfaces que ela própria define (repositórios, serviços distribuídos)

**Proibido:** referenciar `Infrastructure` diretamente. A Application define interfaces; a Infrastructure as implementa.

## Estrutura Interna

```
Application/
├── Services/
│   ├── Interfaces/          # IReservationService, ISessionService
│   ├── ReservationService.cs
│   └── SessionService.cs
├── DTOs/
│   ├── Reservations/        # CreateReservationRequest, CreateReservationResponse
│   └── Sessions/            # CreateSessionRequest, SessionResponse
├── Persistence/
│   └── Repositories/        # Interfaces de repositório (ISessionRepository)
├── Validators/
│   └── Sessions/            # CreateSessionRequestValidator
├── IDistributedLockService.cs
└── ApplicationExtensions.cs
```

## Serviços

### Regras de Implementação

- Serviços recebem DTOs de entrada e retornam DTOs de saída — nunca expõem entidades do Domain diretamente
- Toda dependência é injetada via construtor (interfaces, nunca implementações concretas)
- Operações que podem conflitar entre requisições concorrentes usam `IDistributedLockService`
- Exceções de domínio (`ConflictException`) propagam naturalmente; o Controller decide o status HTTP

### Serviços Atuais

#### `ReservationService`
- Valida disponibilidade de assentos
- Calcula `TotalAmount` com base no `TicketPrice` da entidade `Session`
- Adquire lock distribuído por sessão + assento antes de persistir

#### `SessionService`
- CRUD completo de sessões de cinema
- Delega persistência ao `ISessionRepository`

## DTOs

- DTOs de entrada (`Request`) são objetos simples sem lógica — apenas propriedades
- DTOs de saída (`Response`) são montados a partir das entidades pelo serviço
- Nenhum DTO referencia entidades do Domain — o mapeamento é feito manualmente no serviço

## Interfaces de Repositório

Ficam em `Persistence/Repositories/` e definem o contrato de acesso a dados **sem mencionar EF Core ou SQL**.

Exemplo de convenção:
```csharp
public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Session> CreateAsync(Session session, CancellationToken cancellationToken = default);
    Task UpdateAsync(Session session, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
```

## Validação

- Toda validação de input fica em `Validators/` usando `FluentValidation`
- Um validator por DTO de entrada
- Regras de formato e obrigatoriedade ficam no validator; regras de negócio (ex: sessão existe?) ficam no serviço

## Concorrência Distribuída

A interface `IDistributedLockService` é definida aqui e implementada na Infrastructure. A chave de lock deve incluir todos os escopos do recurso protegido:

```csharp
// Correto — escopo completo
var lockKey = $"reservation:session:{sessionId}:seat:{seatNumber}";

// Errado — causa contenção falsa entre sessões diferentes
var lockKey = $"reservation:{seatNumber}";
```

## Registro de Dependências

Todos os serviços e validators da camada são registrados em `ApplicationExtensions.cs`:

```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddScoped<IReservationService, ReservationService>();
    services.AddScoped<ISessionService, SessionService>();
    services.AddValidatorsFromAssemblyContaining<CreateSessionRequestValidator>();
    return services;
}
```

Ao adicionar um novo serviço, registrá-lo aqui **no mesmo commit**.

## Adicionando um Novo Caso de Uso

1. Criar o DTO de entrada em `DTOs/{Entidade}/`
2. Criar o validator correspondente em `Validators/{Entidade}/`
3. Criar ou atualizar a interface do serviço em `Services/Interfaces/`
4. Implementar o serviço em `Services/`
5. Registrar no `ApplicationExtensions.cs`
6. Escrever testes em `ReservaCinema.Application.Tests/` **antes** de implementar
