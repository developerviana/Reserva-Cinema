# 📊 Estrutura de Testes Criada - ReservaCinema

## 🎯 Visão Geral

A pasta de testes foi estruturada seguindo **boas práticas de arquitetura** com separação clara por **camada** e **tipo de teste**.

```
tests/
│
├── 📂 ReservaCinema.Domain.Tests/              ✅ Camada Domain
│   ├── 📂 Entities/
│   │   └── SessionTests.cs                     • Teste de entidade Session
│   └── ReservaCinema.Domain.Tests.csproj       • Configuração do projeto
│
├── 📂 ReservaCinema.Application.Tests/         ✅ Camada Application
│   ├── 📂 DTOs/
│   │   └── CreateSessionRequestTests.cs        • Teste de DTO Request
│   ├── 📂 UseCases/                            • Para adicionar testes de casos de uso
│   ├── 📂 Services/                            • Para adicionar testes de serviços
│   ├── 📂 Validators/                          • Para adicionar testes de validadores
│   └── ReservaCinema.Application.Tests.csproj  • Configuração do projeto
│
├── 📂 ReservaCinema.API.Tests/                 ✅ Camada API
│   ├── 📂 Unit/                                • Testes unitários
│   │   ├── 📂 Controllers/
│   │   ├── 📂 DTOs/
│   │   │   └── CreateSessionRequestDTOTests.cs
│   │   └── ...
│   ├── 📂 Integration/                         • Testes de integração
│   │   ├── 📂 Sessions/
│   │   │   └── CreateSessionIntegrationTests.cs
│   │   └── ...
│   ├── 📂 Fixtures/                            • Fixtures para setup
│   └── ReservaCinema.API.Tests.csproj          • Configuração do projeto
│
├── 📂 ReservaCinema.Infrastructure.Tests/      ✅ Camada Infrastructure
│   ├── 📂 Repositories/
│   │   └── SessionRepositoryTests.cs           • Teste de repositório
│   ├── 📂 Cache/                               • Para adicionar testes de cache
│   ├── 📂 Messaging/                           • Para adicionar testes de mensageria
│   └── ReservaCinema.Infrastructure.Tests.csproj • Configuração do projeto
│
├── 📂 Shared/                                  ✅ Código Compartilhado
│   ├── 📂 Fixtures/
│   │   └── BaseFixture.cs                      • Classe base para fixtures
│   ├── 📂 Builders/
│   │   ├── SessionBuilder.cs                   • Builder para entidade Session
│   │   └── CreateSessionRequestBuilder.cs      • Builder para DTO Request
│   ├── 📂 Mocks/
│   │   └── RepositoryMockFactory.cs            • Factory de mocks de repositórios
│   ├── 📂 Constants/
│   │   └── TestDataConstants.cs                • Constantes de dados de teste
│   ├── 📂 Extensions/
│   │   └── TestExtensions.cs                   • Extensões úteis para testes
│   └── GlobalUsings.cs                         • Global usings para todo projeto de testes
│
├── 📋 README.md                                • Documentação principal dos testes
├── 📋 CONTRIBUTING.md                          • Guia para contribuir com testes
├── 📋 TEST_TEMPLATE.md                         • Template para criar novos testes
├── 📄 run-tests.sh                             • Script para executar testes (Linux/Mac)
├── 📄 run-tests.bat                            • Script para executar testes (Windows)
├── 📄 coverletSettings.json                    • Configuração de cobertura
├── 📄 xunit.runner.json                        • Configuração de xUnit
└── 📄 .gitignore                               • Arquivo de exclusão Git
```

## 📦 Pacotes Adicionados

### Configurações dos Projetos .csproj

```xml
<!-- Pacotes adicionados a todos os projetos de teste -->

<!-- xUnit & Test Runner -->
<PackageReference Include="xunit" Version="2.9.3" />
<PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />

<!-- Assertions mais legíveis -->
<PackageReference Include="FluentAssertions" Version="6.12.0" />

<!-- Mocking de dependências -->
<PackageReference Include="Moq" Version="4.20.70" />

<!-- Específico para testes de integração (API.Tests) -->
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.5" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.5" />
```

## 🔧 Arquivos Criados para Suporte

### 1. **Builders** (Padrão Builder)
Permitem criar dados de teste de forma fluente:

```csharp
var session = new SessionBuilder()
    .WithMovieTitle("The Matrix")
    .WithTotalSeats(100)
    .Build();
```

### 2. **Constantes** (TestDataConstants)
Centralizam dados de teste para fácil manutenção:

```csharp
TestDataConstants.Sessions.ValidMovieTitle  // "The Matrix"
TestDataConstants.Sessions.ValidTotalSeats  // 100
TestDataConstants.Sessions.InvalidTotalSeats // 0
```

### 3. **Mock Factory** (RepositoryMockFactory)
Cria mocks pré-configurados:

```csharp
var mockRepository = RepositoryMockFactory
    .CreateSessionRepositoryMock();
```

### 4. **Extensões** (TestExtensions)
Funções úteis para testes:

```csharp
DateTime.IsCloseTo(otherDate, toleranceSeconds: 5)
DateTime.GetFutureDateTime(hoursFromNow: 1)
Guid.GetSequentialGuid()
string.GenerateRandomString(length: 10)
```

## ✅ Tipos de Teste Implementados

| Tipo | Localização | Exemplo |
|------|------------|---------|
| **Unit Tests** | Domain.Tests, Application.Tests | SessionTests.cs |
| **DTO Tests** | Application.Tests/DTOs | CreateSessionRequestTests.cs |
| **Integration Tests** | API.Tests/Integration | CreateSessionIntegrationTests.cs |
| **Repository Tests** | Infrastructure.Tests | SessionRepositoryTests.cs |

## 🚀 Como Usar

### Executar Todos os Testes
```bash
# Windows
run-tests.bat all

# Linux/Mac
./run-tests.sh all
```

### Executar por Camada
```bash
run-tests.bat domain           # Apenas testes de Domain
run-tests.bat application      # Apenas testes de Application
run-tests.bat api              # Apenas testes de API
run-tests.bat infrastructure   # Apenas testes de Infrastructure
```

### Executar com Cobertura
```bash
run-tests.bat coverage
```

## 🎓 Estrutura de um Teste

Todos os testes seguem o padrão **AAA (Arrange, Act, Assert)**:

```csharp
[Fact]
public void Session_WithValidData_ShouldBeCreated()
{
    // ===== ARRANGE =====
    // Preparar dados necessários
    var session = new SessionBuilder()
        .WithMovieTitle("The Matrix")
        .Build();

    // ===== ACT =====
    // Executar a ação testada
    var result = ValidateSession(session);

    // ===== ASSERT =====
    // Verificar resultado com FluentAssertions
    result.Should().Be(true);
    session.MovieTitle.Should().Equal("The Matrix");
}
```

## 📈 Progresso do Projeto

- ✅ Estrutura de pastas por camada
- ✅ Projetos .csproj configurados
- ✅ Builders para dados de teste
- ✅ Mocks factories
- ✅ Constantes de teste
- ✅ Exemplos de testes em todas as camadas
- ✅ Fixtures base
- ✅ Extensões de teste
- ✅ Scripts para executar testes
- ✅ Documentação completa

## 📚 Documentação Disponível

1. **README.md** - Guia completo sobre a estrutura
2. **CONTRIBUTING.md** - Como adicionar novos testes
3. **TEST_TEMPLATE.md** - Template para novos testes

## 🎯 Próximos Passos

1. Adicionar mais testes em cada camada conforme necessário
2. Implementar Testcontainers para testes de Infrastructure
3. Adicionar testes de performance
4. Implementar CI/CD para rodar testes automaticamente
5. Manter cobertura de código acima de 80%

## 💡 Boas Práticas Implementadas

- ✅ Separação clara por camada
- ✅ Código compartilhado (Shared)
- ✅ Builders para dados complexos
- ✅ Mocks factories reutilizáveis
- ✅ Constantes centralizadas
- ✅ Documentação completa
- ✅ Scripts de automação
- ✅ Global usings configurado
- ✅ Padrão AAA em todos os testes
- ✅ FluentAssertions para melhor legibilidade

---

**A estrutura de testes está pronta para uso! 🎉**

Comece adicionando testes específicos do seu projeto seguindo os padrões estabelecidos.
