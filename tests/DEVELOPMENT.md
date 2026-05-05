# Testes — Guia de Desenvolvimento

## Princípio Fundamental: TDD

Todo código de produção nasce de um teste que falha. A ordem é sempre:

```
1. Escrever o teste (RED)
2. Escrever o mínimo de código para passar (GREEN)
3. Refatorar o código de produção (REFACTOR)
```

### Regra Inviolável

**Testes nunca são alterados para o código passar.**

Testes são contratos de negócio. Se um teste passou a falhar após uma mudança no código de produção, o código de produção está errado, não o teste.

```
✅ Corrija o código
❌ Nunca ajuste o teste para mascarar erro
```

Testes só podem ser modificados se:
- A **regra de negócio mudou**
- A mudança estiver **claramente documentada** no commit (`test: atualiza contrato de X por mudança em Y`)

## Estrutura dos Projetos

```
tests/
├── ReservaCinema.Domain.Tests/         # Entidades e regras puras de domínio
├── ReservaCinema.Application.Tests/    # Serviços e validadores (mocks de repositório)
├── ReservaCinema.Infrastructure.Tests/ # Repositórios e Redis (banco real em memória)
├── ReservaCinema.API.Tests/            # Controllers (WebApplicationFactory)
└── ReservaCinema.Tests.Shared/         # Builders e constantes reutilizáveis
```

## O Que Cada Projeto Testa

### Domain.Tests
- Comportamento das entidades (métodos, invariantes)
- Sem mocks, sem banco, sem rede — apenas objetos do domínio

### Application.Tests
- Lógica dos serviços (`ReservationService`, `SessionService`)
- Validadores FluentValidation
- Dependências externas (repositórios, lock) sempre mockadas via Moq

### Infrastructure.Tests
- Repositórios contra banco EF Core InMemory
- `RedisLockService` com conexão Redis mockada
- Comportamento de logging nos serviços de infraestrutura

### API.Tests
- Endpoints HTTP de ponta a ponta
- Usa `CustomWebApplicationFactory` com banco InMemory e Redis mockado
- Valida status codes, contratos de resposta e efeitos colaterais

### Tests.Shared
- Builders fluentes para construção de dados de teste
- Constantes compartilhadas (`TestDataConstants`)
- Nunca contém lógica de asserção

## Builders

Toda criação de objeto em testes usa o Builder do projeto `Tests.Shared`:

```csharp
// Correto
var session = new SessionBuilder().Build();
var request = new CreateSessionRequestBuilder().WithMovieTitle("Duna").Build();

// Proibido — acopla o teste à estrutura interna da entidade
var session = new Session { MovieTitle = "Duna", ... };
```

Builders sempre expõem valores padrão válidos. Sobrescreva apenas o que é relevante para o cenário testado.

## Nomenclatura

### Métodos de Teste

```
{Método}_{Cenário}_{ResultadoEsperado}
```

Exemplos:
- `CreateReservation_WhenSeatsUnavailable_ThrowsConflictException`
- `CreateSession_WithValidData_ReturnsCreatedSession`
- `Validate_WhenTitleIsEmpty_ReturnsValidationError`

### Classes de Teste

```
{ClasseTestada}Tests
```

Exemplos:
- `ReservationServiceTests`
- `CreateSessionRequestValidatorTests`
- `SessionRepositoryTests`

## Stack de Testes

| Biblioteca | Uso |
|---|---|
| xUnit | Framework de testes |
| FluentAssertions | Asserções legíveis (`result.Should().Be(...)`) |
| Moq | Mocks de interfaces |
| EF Core InMemory | Banco em memória para repositórios |
| WebApplicationFactory | Servidor HTTP em memória para API |

## Cobertura Esperada por Camada

| Camada | Cobertura Mínima |
|---|---|
| Domain | 100% dos métodos públicos das entidades |
| Application | 100% dos serviços e validadores |
| Infrastructure | Cenários de sucesso e falha para cada repositório |
| API | Todos os endpoints com cenários de sucesso e erro principal |

## O Que Não Testar

- Construtores e propriedades triviais sem lógica
- Código gerado pelo framework (migrations, scaffolding)
- Comportamento interno de bibliotecas de terceiros
