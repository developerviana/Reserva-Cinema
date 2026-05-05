namespace ReservaCinema.Infrastructure.Distributed.Configuration;

public class DistributedLockOptions
{
    public int LockExpirationSeconds { get; set; } = 5;
}
