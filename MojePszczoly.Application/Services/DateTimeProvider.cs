using MojePszczoly.Application.Interfaces;

namespace MojePszczoly.Application.Services
{
    public class DateTimeProvider : IDateTimeProvider
{
    private readonly IClock _clock;
    private static readonly TimeZoneInfo PolandTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    public DateTimeProvider(IClock clock)
    {
        _clock = clock;
    }

    public DateOnly GetPolandNow()
    {
        var polandTime = TimeZoneInfo.ConvertTimeFromUtc(_clock.UtcNow, PolandTimeZone).Date;
        return DateOnly.FromDateTime(polandTime);
    }
}
}
