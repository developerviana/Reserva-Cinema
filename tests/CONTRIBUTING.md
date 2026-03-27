# 📝 Guia de Contribuição para Testes

## Adicionando Novos Testes

### 1. Identifique a Camada
Primeiro, determine em qual camada seu teste pertence:

| Camada | Pasta | Quando Usar |
|--------|-------|------------|
| **Domain** | `ReservaCinema.Domain.Tests` | Testa entidades, value objects, lógica pura |
| **Application** | `ReservaCinema.Application.Tests` | Testa DTOs, validators, use cases |
| **API** | `ReservaCinema.API.Tests` | Testa endpoints, controllers |
| **Infrastructure** | `ReservaCinema.Infrastructure.Tests` | Testa repositórios, cache, mensageria |

### 2. Crie a Pasta e Arquivo de Teste

```bash
# Exemplo: Adicionar teste para SessionService
# Criar arquivo em:
tests/ReservaCinema.Application.Tests/Services/SessionServiceTests.cs
```

### 3. Use o Template

```csharp
using ReservaCinema.Tests.Shared.Builders;
using ReservaCinema.Tests.Shared.Constants;

namespace ReservaCinema.Application.Tests.Services;

public class SessionServiceTests
{
    [Fact]
    public void Method_Scenario_ExpectedResult()
    {
        // Arrange
        var session = new SessionBuilder()
            .WithMovieTitle("Test Movie")
            .Build();

        // Act
        var result = /* executar ação */;

        // Assert
        result.Should().NotBeNull();
    }
}
```

## Padrões de Nomenclatura

### Arquivos de Teste
```
[ClassOuFeatureTestada]Tests.cs
```

**Exemplos:**
- `SessionTests.cs` - Testa classe `Session`
- `CreateSessionRequestTests.cs` - Testa DTO `CreateSessionRequest`
- `SessionRepositoryTests.cs` - Testa repositório `SessionRepository`

### Métodos de Teste
```
[MethodName]_[Scenario/Condition]_Should[ExpectedResult]
```

**Exemplos:**
```csharp
// ✅ BOM
public void AddAsync_WithValidSession_ShouldReturnSession()
public void MovieTitle_WhenEmpty_ShouldBeInvalid()
public void GetByIdAsync_WithInvalidId_ShouldReturnNull()

// ❌ RUIM
public void Test()
public void TestAddAsync()
public void ValidateMovie()
```

## Estrutura AAA (Arrange, Act, Assert)

Todos os testes devem seguir esse padrão:

```csharp
[Fact]
public void CreateSession_WithValidData_ShouldSucceed()
{
    // ========== ARRANGE ==========
    // Preparar dados necessários
    var sessionBuilder = new SessionBuilder()
        .WithMovieTitle("The Matrix")
        .WithTotalSeats(100)
        .WithTicketPrice(25.50m);
    
    var session = sessionBuilder.Build();
    var mockRepository = new Mock<ISessionRepository>();
    
    // ========== ACT ==========
    // Executar a ação que está sendo testada
    var service = new SessionService(mockRepository.Object);
    var result = await service.CreateSessionAsync(session);
    
    // ========== ASSERT ==========
    // Verificar os resultados
    result.Should().NotBeNull();
    result.Id.Should().NotBeEmpty();
    mockRepository.Verify(r => r.AddAsync(session), Times.Once);
}
```

## Usando Builders

Nunca crie dados de teste manualmente. Use builders:

```csharp
// ❌ RUIM - Dados hardcoded
var session = new Session
{
    Id = Guid.NewGuid(),
    MovieTitle = "The Matrix",
    StartTime = DateTime.UtcNow.AddHours(1),
    RoomNumber = "A1",
    TotalSeats = 100,
    TicketPrice = 25.50m,
    CreatedAt = DateTime.UtcNow
};

// ✅ BOM - Usando builder
var session = new SessionBuilder()
    .WithMovieTitle("The Matrix")
    .WithTotalSeats(100)
    .WithTicketPrice(25.50m)
    .Build();
```

## Usando Constantes

Centralize dados de teste em constantes:

```csharp
// ❌ RUIM - Valores espalhados
var request = new CreateSessionRequestBuilder()
    .WithMovieTitle("The Matrix")
    .WithTotalSeats(100)
    .Build();

// ✅ BOM - Usando constantes
var request = new CreateSessionRequestBuilder()
    .WithMovieTitle(TestDataConstants.Sessions.ValidMovieTitle)
    .WithTotalSeats(TestDataConstants.Sessions.ValidTotalSeats)
    .Build();
```

## Testes Paramétricos

Use `[Theory]` e `[InlineData]` para testar múltiplos cenários:

```csharp
[Theory]
[InlineData(50)]
[InlineData(100)]
[InlineData(200)]
[InlineData(500)]
public void Session_WithDifferentSeatsCount_ShouldAcceptAll(int seatsCount)
{
    // Arrange & Act
    var session = new SessionBuilder()
        .WithTotalSeats(seatsCount)
        .Build();

    // Assert
    session.TotalSeats.Should().Be(seatsCount);
}
```

## Fixtures

Para setup/teardown complexo, use `BaseFixture`:

```csharp
public class DatabaseFixture : BaseFixture
{
    private DbContext _context = null!;

    public override async Task InitializeAsync()
    {
        _context = new TestDbContext();
        await _context.Database.EnsureCreatedAsync();
    }

    public override async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        _context.Dispose();
    }
}
```

## Assertions com FluentAssertions

Use FluentAssertions para assertions mais legíveis:

```csharp
// ❌ Usando xUnit puro
Assert.NotNull(result);
Assert.Equal("The Matrix", result.MovieTitle);
Assert.True(result.TotalSeats > 0);

// ✅ Usando FluentAssertions
result.Should().NotBeNull();
result.MovieTitle.Should().Be("The Matrix");
result.TotalSeats.Should().BeGreaterThan(0);
```

## Mocks com Moq

Configure mocks apropriadamente:

```csharp
[Fact]
public async Task CreateSession_ShouldCallRepository()
{
    // Arrange
    var session = new SessionBuilder().Build();
    var mockRepository = new Mock<ISessionRepository>();
    
    mockRepository
        .Setup(r => r.AddAsync(It.IsAny<Session>()))
        .ReturnsAsync(session);

    // Act
    var service = new SessionService(mockRepository.Object);
    await service.CreateSessionAsync(session);

    // Assert
    mockRepository.Verify(
        r => r.AddAsync(It.Is<Session>(s => s.MovieTitle == "The Matrix")),
        Times.Once);
}
```

## Checklist Antes de Fazer Commit

- [ ] Teste tem nome descritivo
- [ ] Segue padrão AAA (Arrange, Act, Assert)
- [ ] Usa builders para dados complexos
- [ ] Usa constantes quando apropriado
- [ ] Usa FluentAssertions
- [ ] Tem uma única razão para falhar
- [ ] É independente de outros testes
- [ ] Está na pasta correta
- [ ] Compila sem erros
- [ ] Passa quando executado
- [ ] Falha quando a lógica está quebrada

## Executando Testes Localmente

```bash
# Windows
run-tests.bat all

# Linux/Mac
./run-tests.sh all

# Teste específico
dotnet test --filter "MethodName=Test_Scenario_ExpectedResult"

# Com cobertura
run-tests.bat coverage
```

## Frequência de Testes

| Tipo | Frequência | Pré-requisitos |
|------|-----------|----------------|
| **Unit Tests** | Todos os métodos públicos | Unit de negócio isolada |
| **Integration Tests** | Fluxos críticos | Pode usar DB em memória |
| **End-to-End** | Cenários críticos | Pode usar Testcontainers |

## Padrão de Repositório de Testes

```
ReservaCinema.[Layer].Tests/
├── [Category]/              # Por domínio (Sessions, Bookings, etc)
│   └── [Entity]Tests.cs     # Testes da entidade
├── [Category]2/
│   └── ...
└── [Layer].Tests.csproj
```

## Dúvidas?

Consulte:
- `/tests/README.md` - Visão geral da estrutura
- `/tests/TEST_TEMPLATE.md` - Template padrão
- `/tests/Shared/` - Builders, mocks, constantes

---

**Contribuições são bem-vindas! Siga este guia para manter a qualidade e consistência dos testes.**
