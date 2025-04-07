using Microsoft.EntityFrameworkCore;
using MojePszczoly.Data;
using MojePszczoly.Interfaces; 
using MojePszczoly.Services;
using Moq;
using Xunit;

namespace MojePszczoly.Tests.Services
{
    public class DateServiceTests
    {
        private readonly IDateService _dateService;
        private readonly Mock<AppDbContext> _mockContext;

        public DateServiceTests()
        {
            var options = new DbContextOptions<AppDbContext>();
            _mockContext = new Mock<AppDbContext>(options);
            _dateService = new DateService(_mockContext.Object);
        }

        [Fact]
        public void GetUpcomingDates_ReturnsThreeUpcomingDates()
        {
            // Arrange - Already set up in the constructor

            // Act
            var result = _dateService.GetUpcomingDates();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.True(result[0] < result[1]);
            Assert.True(result[1] < result[2]);
        }

        [Fact]
        public void GetUpcomingDates_DatesAreInUtc()
        {
            // Act
            var result = _dateService.GetUpcomingDates();

            // Assert
            foreach (var date in result)
            {
                Assert.Equal(DateTimeKind.Utc, date.Kind);
            }
        }

        [Fact]
        public void GetUpcomingDates_DatesHaveCorrectTime()
        {
            // Act
            var result = _dateService.GetUpcomingDates();

            // Assert
            foreach (var date in result)
            {
                Assert.Equal(0, date.Hour);
                Assert.Equal(0, date.Minute);
                Assert.Equal(0, date.Second);
            }
        }

        [Fact]
        public void GetUpcomingDates_DatesIncludeSpecifiedDaysOfWeek()
        {
            // Act
            var result = _dateService.GetUpcomingDates();

            Assert.Contains(result, date => date.DayOfWeek == DayOfWeek.Tuesday);
            Assert.Contains(result, date => date.DayOfWeek == DayOfWeek.Wednesday);
            Assert.Contains(result, date => date.DayOfWeek == DayOfWeek.Thursday);
        }
    }
}
