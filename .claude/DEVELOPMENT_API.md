# API — Guia de Desenvolvimento

## Responsabilidade da Camada

Ponto de entrada HTTP do sistema. Recebe requisições, delega para os serviços da Application e traduz resultados em respostas HTTP. Não contém lógica de negócio.

## Dependências Permitidas

- `ReservaCinema.Application` — interfaces de serviço e DTOs
- Pacotes web: `FluentValidation.AspNetCore`, `Swashbuckle.AspNetCore`

**Proibido:** importar entidades do Domain ou implementações da Infrastructure nos controllers.

## Estrutura Interna

```
API/
├── Controllers/
│   ├── ReservationsController.cs
│   └── SessionsController.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── appsettings.Development.json
├── Dockerfile
└── Program.cs
```

## Controllers

### Regras de Implementação

- Controllers recebem e retornam DTOs da camada Application — nunca entidades do Domain
- Toda lógica de negócio fica nos serviços da Application; o controller apenas orquestra
- Validação de input é automática via FluentValidation registrado no pipeline — não repetir validação no controller
- Exceções de domínio mapeadas para status HTTP via middleware ou tratamento explícito

### Mapeamento de Exceções para HTTP

| Exceção | Status HTTP |
|---|---|
| `ConflictException` | `409 Conflict` |
| `KeyNotFoundException` | `404 Not Found` |
| Erros de validação FluentValidation | `400 Bad Request` |

### Padrão de Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionsController(ISessionService sessionService)
        => _sessionService = sessionService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSessionRequest request)
    {
        var response = await _sessionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}
```

## Program.cs

Responsabilidades do `Program.cs`:
1. Registrar serviços via `AddApplication()` e `AddInfrastructure()`
2. Configurar middleware (validação, Swagger, tratamento de exceções)
3. Aplicar migrações pendentes no startup
4. Mapear controllers

Não adicionar lógica de negócio ou acesso a dados no `Program.cs`.

## Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=cinema_db;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  }
}
```

Segredos de produção (senhas, chaves) nunca no `appsettings.json` versionado — usar variáveis de ambiente ou Azure Key Vault.

### Environments

- `appsettings.json` — configuração base
- `appsettings.Development.json` — sobrescreve para desenvolvimento local

## Swagger

Disponível em `/swagger` no ambiente de desenvolvimento. Todos os endpoints devem ter documentação XML suficiente para uso via Swagger sem precisar ler o código.

## Docker

```bash
# Build da imagem
docker build -t reserva-cinema-api -f src/ReservaCinema.API/Dockerfile .

# Subir todos os serviços (API + PostgreSQL + Redis)
docker-compose up
```

O `Dockerfile` usa multi-stage build: estágio de build com SDK e estágio final com apenas o runtime.

## Adicionando um Novo Endpoint

1. O caso de uso já deve existir como método na interface do serviço (`Application/Services/Interfaces/`)
2. Adicionar o método no controller correspondente ou criar um novo controller
3. Verificar se o mapeamento de exceção para HTTP está coberto
4. Escrever testes de integração em `ReservaCinema.API.Tests/` **antes** de implementar
5. Documentar o endpoint no Swagger se necessário
