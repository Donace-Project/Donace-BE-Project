using AutoMapper;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Entities.Event;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.EventParticipation;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;

namespace Donace_BE_Project.Services
{
    public class EventParticipationService : IEventParticipationService
    {
        private readonly ILogger<EventParticipationService> _logger;
        private readonly IEventParticipationRepository _eventParticipationRepository;
        private readonly IMapper _mapper;
        public EventParticipationService(ILogger<EventParticipationService> logger,
                                         IEventParticipationRepository eventParticipationRepository,
                                         IMapper mapper)
        {
            _logger = logger;
            _eventParticipationRepository = eventParticipationRepository;
            _mapper = mapper;
        }
        public async Task CreateAsync(EventParticipationModel req)
        {
            try
            {
                var data = _mapper.Map<EventParticipation>(req);
                await _eventParticipationRepository.CreateAsync(data);
            }
            catch (FriendlyException ex)
            {
                _logger.LogError($"EventParticipationService.Exception: {ex.Message}", JsonConvert.SerializeObject(req));
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_EventParticipationService, ex.Message);
            }
        }

        public async Task<List<Guid>> ListIdEventSubAsync(Guid userId)
        {
            try
            {
                return await _eventParticipationRepository.ListIdEventByUserIdAsync(userId);
            }
            catch (FriendlyException ex)
            {
                _logger.LogError($"EventParticipationService.Exception: {ex.Message}", JsonConvert.SerializeObject(userId));
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_EventParticipationService, ex.Message);
            }
        }
    }
}
