using MojePszczoly.Application.Interfaces;

namespace MojePszczoly.Application.Services
{
    public class SystemClock : IClock
    {
    public DateTime UtcNow => DateTime.UtcNow;
    }
}
