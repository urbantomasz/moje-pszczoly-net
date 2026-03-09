using MojePszczoly.Interfaces;

namespace MojePszczoly.Services
{
    public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
}
