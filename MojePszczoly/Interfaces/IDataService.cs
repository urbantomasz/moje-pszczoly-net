namespace MojePszczoly.Interfaces
{
    public interface IDateService
    {
        List<DateTime> GetUpcomingDates();
        List<DateTime> GetPastDates();
        List<DateTime> GetCurrentWeekDates();

        DateTime GetCurrentWeekMonday();
    }
}
