using MojePszczoly.Models;

namespace MojePszczoly.Interfaces
{
    public interface IDateService
    {
        List<OrderDateDto> GetUpcomingOrderDates();
        List<DateTime> GetUpcomingDates();
        List<DateTime> GetGeneratedDates();
        List<DateTime> GetPastDates();
        List<DateTime> GetCurrentWeekDates();

        DateTime GetCurrentWeekMonday();
    }
}
