using MojePszczoly.Infrastructure.Entities;
using MojePszczoly.Interfaces; 
using MojePszczoly.Services;
using Moq;

namespace MojePszczoly.Tests.Services
{
    public class DateServiceTests
    {
        private readonly IDateService _dateService;
        private readonly Mock<IDateOverrideService> _mockDateOverrideService;
        private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;

        public DateServiceTests()
        {
            _mockDateOverrideService = new Mock<IDateOverrideService>();
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();

            _mockDateTimeProvider
                .Setup(p => p.GetPolandNow())
                .Returns(new DateOnly(2026, 3, 4));

            _mockDateOverrideService
                .Setup(s => s.GetOverrides())
                .ReturnsAsync(new List<DateOverride>());

            _dateService = new DateService(_mockDateOverrideService.Object, _mockDateTimeProvider.Object);
        }

        [Fact]
        public async Task GetUpcomingDates_ReturnsThreeUpcomingDates()
        {
            // Arrange - Already set up in the constructor

            // Act
            var result = await _dateService.GetUpcomingDates();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.True(result[0] < result[1]);
            Assert.True(result[1] < result[2]);
        }


        [Fact]
        public async Task GetUpcomingDates_DatesIncludeSpecifiedDaysOfWeek()
        {
            // Act
            var result = await _dateService.GetUpcomingDates();

            Assert.Contains(result, date => date.DayOfWeek == DayOfWeek.Tuesday);
            Assert.Contains(result, date => date.DayOfWeek == DayOfWeek.Wednesday);
            Assert.Contains(result, date => date.DayOfWeek == DayOfWeek.Thursday);
        }
    }
}
