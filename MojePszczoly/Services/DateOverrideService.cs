using MojePszczoly.Infrastructure.Entities;
using MojePszczoly.Interfaces;
using MojePszczoly.Interfaces.Repositories;

namespace MojePszczoly.Services
{
    public class DateOverrideService : IDateOverrideService
    {
        private readonly IDateOverrideRepository _dateOverrideRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public DateOverrideService(IDateOverrideRepository dateOverrideRepository, IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
        {
            _dateOverrideRepository = dateOverrideRepository;
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
        }


        public async Task AddExtraDay(DateOnly date)
        {
            var existing = await _dateOverrideRepository.GetByDateAsync(date);
            if (existing != null)
            {
                if (existing.IsAdded)
                    return;
                existing.IsAdded = true;
            }
            else
            {
                await _dateOverrideRepository.AddAsync(new DateOverride { Date = date, IsAdded = true });
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ExcludeDay(DateOnly date)
        {
            var existing = await _dateOverrideRepository.GetByDateAsync(date);
            if (existing != null)
            {
                if (!existing.IsAdded)
                    return;
                existing.IsAdded = false;
            }
            else
            {
                await _dateOverrideRepository.AddAsync(new DateOverride { Date = date, IsAdded = false });
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> RevertOverride(DateOnly date)
        {
            var existing = await _dateOverrideRepository.GetByDateAsync(date);
            if (existing == null) return false;
            await _dateOverrideRepository.DeleteAsync(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<DateOverride>> GetOverridesForNextTwoWeeks()
        {
            var today = _dateTimeProvider.GetPolandNow();
            var endDate = GetEndOfNextWeek(today);

            return await _dateOverrideRepository.GetBetweenDatesAsync(today, endDate);
        }

          public async Task<List<DateOverride>> GetOverrides()
        {
            var today = _dateTimeProvider.GetPolandNow();
            return await _dateOverrideRepository.GetFromDateAsync(today);
        }

        private DateOnly GetEndOfNextWeek(DateOnly today)
        {
            int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
            var thisSunday = today.AddDays(daysUntilSunday);

            return thisSunday.AddDays(7); // niedziela następnego tygodnia
        }
    }
}
