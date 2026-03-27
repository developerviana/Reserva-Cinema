# 🧪 Estrutura de Testes - ReservaCinema

## 📁 Organização das Pastas

```
tests/
├── ReservaCinema.Domain.Tests/          # Testes da camada Domain
│   ├── Entities/                        # Testes de entidades
│   │   └── SessionTests.csclear
│   └── ReservaCinema.Domain.Tests.csproj
│
├── ReservaCinema.Application.Tests/     # Testes da camada Application
│   ├── DTOs/                            # Testes de Data Transfer Objects
│   │   └── CreateSessionRequestTests.cs
│   ├── UseCases/                        # Testes de Use Cases
│   ├── Services/                        # Testes de serviços
│   ├── Validators/                      # Testes de validadores
│   └── ReservaCinema.Application.Tests.csproj
│
├── ReservaCinema.API.Tests/             # Testes da camada API
│   ├── Unit/                            # Testes unitários
│   │   ├── Controllers/
│   │   ├── DTOs/
│   │   │   └── CreateSessionRequestDTOTests.cs
│   │   └── ...
│   ├── Integration/                     # Testes de integração
│   │   ├── Sessions/
│   │   │   └── CreateSessionIntegrationTests.cs
│   │   └── ...
│   ├── Fixtures/                        # Fixtures reutilizáveis
│   └── ReservaCinema.API.Tests.csproj
│
├── ReservaCinema.Infrastructure.Tests/  # Testes da camada Infrastructure
│   ├── Repositories/                    # Testes de repositórios
│   ├── Cache/                           # Testes de cache
│   ├── Messaging/                       # Testes de mensageria
│   └── ReservaCinema.Infrastructure.Tests.csproj
│
└── Shared/                              # Código compartilhado entre testes
    ├── Fixtures/                        # Fixtures base
    │   └── BaseFixture.cs
    ├── Builders/                        # Builders para criar dados de teste
    │   ├── SessionBuilder.cs
    │   └── CreateSessionRequestBuilder.cs
    ├── Mocks/                           # Mocks e factories
    │   └── RepositoryMockFactory.cs
    ├── Constants/                       # Constantes de teste
    │   └── TestDataConstants.cs
    └── Extensions/                      # Extensões úteis
```

## 🎯 Camadas de Teste

### 1. **Domain Tests** (ReservaCinema.Domain.Tests)
- Testa entidades, value objects e lógica de negócio pura
- Sem dependências externas
- Deve ser rápido e determinístico

**Exemplo:**
```csharp
[Fact]
public void Session_WithValidData_ShouldBeCreated()
{
    // Arrange & Act
    var session = new SessionBuilder().Build();
    
    // Assert
    session.Should().NotBeNull();
}
```

### 2. **Application Tests** (ReservaCinema.Application.Tests)
- Testa DTOs, validators, use cases e services
- Utiliza Moq para mockar repositórios
- Testa a lógica da camada de aplicação

**Exemplo:**
```csharp
[Fact]
public void CreateSessionRequest_WithValidData_ShouldBeValid()
{
    // Arrange
    var request = new CreateSessionRequestBuilder().Build();
    
    // Act & Assert
    request.Should().NotBeNull();
}
```

### 3. **API Tests** (ReservaCinema.API.Tests)
- **Unit Tests**: Testa controllers, middleware isoladamente
- **Integration Tests**: Testa fluxo completo com banco de dados em memória
- Utiliza `WebApplicationFactory` para testes de integração

**Exemplo:**
```csharp
[Fact]
public async Task CreateSession_WithValidRequest_ShouldReturnOk()
{
    // Arrange
    var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();
    var request = new CreateSessionRequestBuilder().Build();
    
    // Act
    var response = await client.PostAsJsonAsync("/api/sessions", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
}
```

### 4. **Infrastructure Tests** (ReservaCinema.Infrastructure.Tests)
- Testa repositórios, implementações de cache, mensageria
- Pode usar Testcontainers para testes com dependencies reais

## 🔨 Padrões e Boas Práticas

### Builder Pattern
Utilize builders para criar dados de teste complexos:

```csharp
var session = new SessionBuilder()
    .WithMovieTitle("The Matrix")
    .WithTotalSeats(100)
    .WithTicketPrice(25.50m)
    .Build();
```

### Test Data Constants
Use constantes centralizadas para dados de teste:

```csharp
var request = new CreateSessionRequestBuilder()
    .WithMovieTitle(TestDataConstants.Sessions.ValidMovieTitle)
    .Build();
```

### Mocks e Fixtures
Reutilize mocks através de factories:

```csharp
var mockRepository = RepositoryMockFactory
    .CreateSessionRepositoryMock();
```

### AAA Pattern
Organize cada teste com Arrange, Act, Assert:

```csharp
[Fact]
public void Test_Scenario_ExpectedResult()
{
    // Arrange - Setup
    var data = CreateTestData();
    
    // Act - Execute
    var result = ExecuteOperation(data);
    
    // Assert - Verify
    result.Should().Be(expected);
}
```

## 📦 Pacotes NuGet

- **xUnit** - Framework de testes
- **FluentAssertions** - Assertions mais legíveis
- **Moq** - Mocking de dependências
- **Microsoft.AspNetCore.Mvc.Testing** - Teste de integração para APIs
- **Microsoft.EntityFrameworkCore.InMemory** - DbContext em memória

## 🚀 Executando Testes

### Via CLI
```bash
# Todos os testes
dotnet test

# Testes específicos
dotnet test --filter "Namespace=ReservaCinema.Domain.Tests"

# Com cobertura
dotnet test /p:CollectCoverage=true
```

### Via Visual Studio
- **Test Explorer** (Ctrl + E, T)
- Run All Tests
- Run Tests by Category

## 📊 Estrutura Recomendada para Novos Testes

1. Crie a pasta correspondente à camada
2. Use o padrão de nomenclatura: `[Class/Feature]Tests.cs`
3. Use `BaseFixture` se precisar de setup/teardown
4. Use builders para criar dados
5. Siga o padrão AAA (Arrange, Act, Assert)
6. Use `FluentAssertions` para assertions

## ✅ Checklist para Novo Teste

- [ ] Teste possui nome descritivo
- [ ] Teste está na pasta correta (por camada)
- [ ] Usa padrão AAA (Arrange, Act, Assert)
- [ ] Usa builders para dados complexos
- [ ] Usa constantes de teste quando aplicável
- [ ] Tem apenas uma razão para falhar
- [ ] É independente de outros testes
- [ ] Utiliza FluentAssertions
- [ ] Cobre casos positivos e negativos

---

**Última atualização:** 2024
**Autor:** Estrutura de Testes
