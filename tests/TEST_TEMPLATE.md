// TEMPLATE: Esboço para criar novos testes
// Use este arquivo como referência ao criar novos testes

/*
using ReservaCinema.Tests.Shared.Builders;
using ReservaCinema.Tests.Shared.Constants;

namespace ReservaCinema.[Layer].Tests.[Category];

/// <summary>
/// Descrição breve do que está sendo testado.
/// </summary>
public class [Class/Feature]Tests
{
    // ============================================================
    // Testes de Sucesso (Happy Path)
    // ============================================================

    [Fact]
    public void [MethodName]_[Scenario]_ShouldReturn[Result]()
    {
        // Arrange - Preparar dados
        var data = new Builder().WithValue(x).Build();

        // Act - Executar ação
        var result = PerformAction(data);

        // Assert - Verificar resultado
        result.Should().Be(expectedValue);
    }

    // ============================================================
    // Testes de Falha (Error Cases)
    // ============================================================

    [Fact]
    public void [MethodName]_[NegativeScenario]_ShouldFail()
    {
        // Arrange
        var invalidData = new Builder().WithInvalidValue().Build();

        // Act
        var result = PerformAction(invalidData);

        // Assert
        result.Should().BeNull();
    }

    // ============================================================
    // Testes Paramétricos (Multiple Scenarios)
    // ============================================================

    [Theory]
    [InlineData(value1)]
    [InlineData(value2)]
    [InlineData(value3)]
    public void [MethodName]_WithDifferentValues_ShouldHandle(object parameter)
    {
        // Arrange
        var data = new Builder().WithValue(parameter).Build();

        // Act
        var result = PerformAction(data);

        // Assert
        result.Should().NotBeNull();
    }

    // ============================================================
    // Boas Práticas
    // ============================================================

    // ✅ DO:
    // - Use nomes descritivos: Test_Scenario_ExpectedResult
    // - Organize com Arrange, Act, Assert
    // - Um assert por teste (preferencialmente)
    // - Use builders para dados complexos
    // - Use constantes para dados repetitivos

    // ❌ DON'T:
    // - Use nomes genéricos: Test1, Test2
    // - Misture múltiplas ações (Act)
    // - Teste múltiplos cenários em um teste
    // - Crie dependências entre testes
    // - Use valores hardcoded (sem necessidade)
}
*/
