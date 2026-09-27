namespace MojePszczoly.Data.Models
{
    public class OrderFormSettings
    {
        public int Id { get; set; }
        public bool UseCustomDates { get; set; }
        public bool NoticeEnabled { get; set; }
        public string? NoticeTitle { get; set; }
        public string? NoticeMessage { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
