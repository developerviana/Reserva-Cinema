# 🧪 Testes - ReservaCinema

Bem-vindo à pasta de testes do projeto ReservaCinema! Esta estrutura foi organizada seguindo **boas práticas de engenharia de software** para garantir qualidade, manutenibilidade e escalabilidade.

## 📖 Documentação

| Documento | Descrição |
|-----------|-----------|
| **[README.md](README.md)** | 📚 Visão geral completa da estrutura de testes |
| **[STRUCTURE_OVERVIEW.md](STRUCTURE_OVERVIEW.md)** | 🎯 Visão visual e organização das pastas |
| **[CONTRIBUTING.md](CONTRIBUTING.md)** | 👨‍💻 Guia passo a passo para adicionar novos testes |
| **[TEST_TEMPLATE.md](TEST_TEMPLATE.md)** | 📋 Template padrão para novos testes |

## 🚀 Quick Start

### Executar Testes
```bash
# Windows
run-tests.bat all                # Todos os testes
run-tests.bat domain             # Apenas Domain
run-tests.bat coverage           # Com cobertura

# Linux/Mac
./run-tests.sh all
./run-tests.sh domain
./run-tests.sh coverage
```

### Visual Studio
- **Abrir Test Explorer:** Ctrl + E, T
- **Rodar todos:** Alt + R, A
- **Rodar testes selecionados:** Alt + R, C

## 📁 Estrutura Principal

```
tests/
├── ReservaCinema.Domain.Tests/           ← Testes de entidades e lógica pura
├── ReservaCinema.Application.Tests/      ← Testes de DTOs e casos de uso
├── ReservaCinema.API.Tests/              ← Testes de endpoints (unit + integration)
├── ReservaCinema.Infrastructure.Tests/   ← Testes de repositórios e cache
└── Shared/                               ← Builders, mocks, constantes, extensões
```

## 🛠️ Ferramentas Disponíveis

### Builders (Criar dados de teste)
```csharp
var session = new SessionBuilder()
    .WithMovieTitle("The Matrix")
    .WithTotalSeats(100)
    .Build();
```

### Constantes (Dados centralizados)
```csharp
TestDataConstants.Sessions.ValidMovieTitle
TestDataConstants.Sessions.ValidTotalSeats
TestDataConstants.Validation.MovieTitleRequired
```

### Mock Factory (Mocks pré-configurados)
```csharp
var mockRepo = RepositoryMockFactory
    .CreateSessionRepositoryMock();
```

### Extensões (Funções úteis)
```csharp
DateTime.GetFutureDateTime(hoursFromNow: 1)
DateTime.IsCloseTo(otherDate, toleranceSeconds: 5)
Guid.GetSequentialGuid()
string.GenerateRandomString(length: 10)
```

## 📊 Padrão de Teste

Todos os testes seguem o padrão **AAA (Arrange, Act, Assert)**:

```csharp
[Fact]
public void MethodName_Scenario_ExpectedResult()
{
    // ARRANGE - Preparar
    var data = new Builder().Build();
    
    // ACT - Executar
    var result = DoSomething(data);
    
    // ASSERT - Verificar
    result.Should().NotBeNull();
}
```

## ✅ Checklist para Novo Teste

- [ ] Teste em pasta correta (por camada)
- [ ] Nome descritivo: `Method_Scenario_Result`
- [ ] Segue padrão AAA
- [ ] Usa builders para dados complexos
- [ ] Usa constantes para valores repetitivos
- [ ] Usa FluentAssertions
- [ ] Uma razão para falhar
- [ ] Independente de outros testes
- [ ] Compila sem erros
- [ ] Passa quando deve passar

## 🧬 Camadas e Responsabilidades

### Domain Tests
**O quê:** Testa entidades, value objects, lógica pura
**Quando:** Lógica de negócio sem dependências
**Exemplos:** SessionTests.cs

### Application Tests
**O quê:** Testa DTOs, validadores, use cases
**Quando:** Lógica da camada de aplicação
**Exemplos:** CreateSessionRequestTests.cs

### API Tests
**O quê:** Unit tests de controllers + integration tests
**Quando:** Endpoints e fluxos completos
**Exemplos:** CreateSessionIntegrationTests.cs

### Infrastructure Tests
**O quê:** Testa repositórios, cache, mensageria
**Quando:** Implementações de I/O
**Exemplos:** SessionRepositoryTests.cs

## 📚 Bibliotecas Usadas

- **xUnit** - Framework de testes
- **FluentAssertions** - Assertions legíveis
- **Moq** - Mocking de dependências
- **Microsoft.AspNetCore.Mvc.Testing** - Testes de integração

## 🔗 Recursos Úteis

### Dentro do Projeto
- `/tests/Shared/Builders/` - Builders reutilizáveis
- `/tests/Shared/Mocks/` - Mock factories
- `/tests/Shared/Constants/` - Dados de teste
- `/tests/Shared/Extensions/` - Funções auxiliares

### Documentação Externa
- [xUnit Docs](https://xunit.net/)
- [FluentAssertions Docs](https://fluentassertions.com/)
- [Moq Docs](https://github.com/moq/moq4)

## 📞 Dúvidas?

1. Consulte **CONTRIBUTING.md** para instruções detalhadas
2. Veja **TEST_TEMPLATE.md** para exemplos
3. Estude **README.md** para entender a arquitetura

## 🎯 Meta de Cobertura

- **Target:** 80%+ de cobertura
- **Mínimo Aceitável:** 70%
- **Verificação:** Execute `run-tests coverage`

## 📝 Últimas Mudanças

- ✅ Estrutura de pastas por camada
- ✅ Projetos .csproj configurados com pacotes
- ✅ Builders e Mock factories
- ✅ Constantes centralizadas
- ✅ Exemplos de testes em todas as camadas
- ✅ Scripts de automação (Windows + Linux)
- ✅ Documentação completa

---

**Status:** ✅ Pronto para uso

**Última atualização:** 2024
**Maintainer:** Equipe de Testes
