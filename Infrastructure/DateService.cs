namespace Infrastructure;

internal class DateService : IDateService
{
    public DateTime Now => DateTime.UtcNow;
    public readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public long ToUnixTimestamp(DateTime dateTime)
    {
        return (long)(Epoch - dateTime).TotalSeconds;
    }
}
