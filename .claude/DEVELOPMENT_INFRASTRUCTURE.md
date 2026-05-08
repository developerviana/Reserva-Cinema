# Infrastructure — Guia de Desenvolvimento

## Responsabilidade da Camada

Implementa os contratos definidos pela Application. Contém tudo que depende de tecnologia externa: banco de dados, cache, mensageria, serviços de terceiros.

## Dependências Permitidas

- `ReservaCinema.Application` — interfaces a implementar, entidades via Domain
- `ReservaCinema.Domain` — entidades para mapeamento ORM
- Entity Framework Core, Npgsql, StackExchange.Redis e outros pacotes de infraestrutura

**Proibido:** a API não pode referenciar a Infrastructure diretamente — toda dependência passa pelas interfaces da Application.

## Estrutura Interna

```
Infrastructure/
├── Persistence/
│   ├── ReservaCinemaDbContext.cs
│   ├── Repositories/
│   │   └── SessionRepository.cs
│   └── Migrations/
├── Distributed/
│   ├── Configuration/
│   │   └── DistributedLockOptions.cs
│   ├── IRedisConnectionProvider.cs
│   ├── RedisConnectionProvider.cs
│   └── RedisLockService.cs
└── InfrastructureExtensions.cs
```

## Banco de Dados (EF Core + PostgreSQL)

### DbContext

`ReservaCinemaDbContext` é a única porta de entrada para o banco. Contém os `DbSet<>` das entidades e configurações de mapeamento.

### Repositórios

- Implementam as interfaces definidas em `Application/Persistence/Repositories/`
- Usam apenas `DbContext` como dependência — nunca SQL bruto sem justificativa
- Todos os métodos são assíncronos com suporte a `CancellationToken`

Padrão de implementação:
```csharp
public class SessionRepository : ISessionRepository
{
    private readonly ReservaCinemaDbContext _context;

    public SessionRepository(ReservaCinemaDbContext context) => _context = context;

    public async Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id && s.IsActive, cancellationToken);
}
```

### Soft Delete

Entidades com `IsActive` nunca são removidas fisicamente. Toda query de leitura filtra `IsActive == true`.

### Migrações

```bash
# Criar migration
dotnet ef migrations add NomeDaMigration --project src/ReservaCinema.Infrastructure --startup-project src/ReservaCinema.API

# Aplicar
dotnet ef database update --project src/ReservaCinema.Infrastructure --startup-project src/ReservaCinema.API
```

Migrações são aplicadas automaticamente no startup via `Program.cs`. Nunca deletar ou alterar migrações já aplicadas em produção.

## Concorrência Distribuída (Redis)

### Arquitetura

- `IRedisConnectionProvider` abstrai a conexão com o Redis — permite mock em testes
- `RedisConnectionProvider` implementa a conexão via `StackExchange.Redis`
- `RedisLockService` implementa `IDistributedLockService` usando o padrão Redlock com scripts Lua para atomicidade

### Configuração

```json
{
  "DistributedLock": {
    "ExpirySeconds": 30,
    "RetryCount": 3,
    "RetryDelayMs": 100
  }
}
```

Configurações em `DistributedLockOptions` — nunca hardcoded no serviço.

### Observabilidade

`RedisLockService` registra logs em operações críticas (aquisição, liberação, falha de lock). Manter logging nos pontos de entrada e saída do lock.

## Registro de Dependências

Todos os serviços de infraestrutura são registrados em `InfrastructureExtensions.cs`:

```csharp
public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services.AddDbContext<ReservaCinemaDbContext>(...);
    services.AddScoped<ISessionRepository, SessionRepository>();
    services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();
    services.AddScoped<IDistributedLockService, RedisLockService>();
    return services;
}
```

Ao adicionar um novo repositório ou serviço de infraestrutura, registrá-lo aqui **no mesmo commit**.

## Adicionando um Novo Repositório

1. A interface já deve existir em `Application/Persistence/Repositories/`
2. Criar a implementação em `Infrastructure/Persistence/Repositories/`
3. Adicionar o `DbSet<>` correspondente no `ReservaCinemaDbContext` se necessário
4. Criar a migration se houver mudança de schema
5. Registrar no `InfrastructureExtensions.cs`
6. Escrever testes em `ReservaCinema.Infrastructure.Tests/` **antes** de implementar
