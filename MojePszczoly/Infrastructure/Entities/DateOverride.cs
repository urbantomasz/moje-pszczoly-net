namespace MojePszczoly.Infrastructure.Entities
{
    public class DateOverride
    {
        public int DateOverrideId { get; set; }
        public DateOnly Date { get; set; }
        public bool IsAdded { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
