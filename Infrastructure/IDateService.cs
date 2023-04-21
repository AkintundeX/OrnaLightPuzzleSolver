namespace Infrastructure;

public interface IDateService
{
    DateTime Now { get; }

    long ToUnixTimestamp(DateTime dateTime);
}