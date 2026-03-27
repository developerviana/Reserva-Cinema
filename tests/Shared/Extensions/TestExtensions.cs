namespace ReservaCinema.Tests.Shared.Extensions;

/// <summary>
/// Extensões úteis para testes.
/// </summary>
public static class TestExtensions
{
    /// <summary>
    /// Verifica se uma data está próxima (dentro de X segundos).
    /// Útil para comparar datas em testes.
    /// </summary>
    public static bool IsCloseTo(this DateTime actual, DateTime expected, int toleranceSeconds = 5)
    {
        var difference = Math.Abs((actual - expected).TotalSeconds);
        return difference <= toleranceSeconds;
    }

    /// <summary>
    /// Retorna uma data no futuro para testes.
    /// </summary>
    public static DateTime GetFutureDateTime(int hoursFromNow = 1)
    {
        return DateTime.UtcNow.AddHours(hoursFromNow);
    }

    /// <summary>
    /// Retorna uma data no passado para testes.
    /// </summary>
    public static DateTime GetPastDateTime(int hoursAgo = 1)
    {
        return DateTime.UtcNow.AddHours(-hoursAgo);
    }

    /// <summary>
    /// Gera um GUID sequencial para testes (melhor para debugging).
    /// </summary>
    private static int _guidCounter = 0;

    public static Guid GetSequentialGuid()
    {
        var guidBytes = Guid.NewGuid().ToByteArray();
        var counterBytes = BitConverter.GetBytes(Interlocked.Increment(ref _guidCounter));
        
        Array.Copy(counterBytes, 0, guidBytes, 0, 4);
        return new Guid(guidBytes);
    }

    /// <summary>
    /// Gera strings aleatórias para testes.
    /// </summary>
    public static string GenerateRandomString(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }
}
