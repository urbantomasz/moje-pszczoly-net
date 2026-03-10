namespace MojePszczoly.Application.Interfaces
{
    public interface IDateService
    {
        Task<List<DateOnly>> GetUpcomingDates();
        List<DateOnly> GetCurrentWeekDates();
        DateOnly GetCurrentWeekMonday();
    }
}
