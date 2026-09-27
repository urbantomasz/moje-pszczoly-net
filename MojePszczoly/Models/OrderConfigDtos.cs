namespace MojePszczoly.Models
{
    public sealed record OrderDateDto(DateTime Date, string? Note);

    public sealed record NoticeDto(string? Title, string? Message);

    /// <summary>Widok publiczny dla formularza zamowienia - tylko to, co widzi klient.</summary>
    public sealed record PublicOrderConfigDto(
        NoticeDto? Notice,
        IReadOnlyList<OrderDateDto> Dates
    );

    /// <summary>Widok panelu administracyjnego - surowe ustawienia plus podglad dat wyliczanych.</summary>
    public sealed record AdminOrderConfigDto(
        bool UseCustomDates,
        bool NoticeEnabled,
        string? NoticeTitle,
        string? NoticeMessage,
        IReadOnlyList<OrderDateDto> CustomDates,
        IReadOnlyList<DateTime> GeneratedDates,
        DateTime UpdatedAt
    );

    public sealed record UpdateOrderConfigRequest(
        bool UseCustomDates,
        bool NoticeEnabled,
        string? NoticeTitle,
        string? NoticeMessage,
        IEnumerable<OrderDateDto> CustomDates
    );
}
