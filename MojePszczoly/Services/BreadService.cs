using AutoMapper;
using MojePszczoly.Contracts.Responses;
using MojePszczoly.Interfaces;
using MojePszczoly.Interfaces.Repositories;

namespace MojePszczoly.Services
{
    public class BreadService : IBreadService
    {
        private readonly IBreadRepository _breadRepository;
        private readonly IMapper _mapper;

        public BreadService(IBreadRepository breadRepository, IMapper mapper)
        {
            _breadRepository = breadRepository;
            _mapper = mapper;
        }

        public async Task<List<BreadResponse>> GetBreads()
        {
            var breads = await _breadRepository.GetAllAsync();
            return _mapper.Map<List<BreadResponse>>(breads);
        }
    }
}
