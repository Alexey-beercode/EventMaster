using EventMaster.BLL.DTOs.Implementations.Requests.Participant;
using EventMaster.BLL.DTOs.Implementations.Responses.Participant;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.BLL.UseCases.Participant;

namespace EventMaster.BLL.Services.Implementation
{
    public class ParticipantService : IParticipantService
    {
        private readonly CreateParticipantUseCase _createParticipantUseCase;
        private readonly DeleteParticipantUseCase _deleteParticipantUseCase;
        private readonly GetParticipantsByEventIdUseCase _getParticipantsByEventIdUseCase;
        private readonly GetParticipantsByUserIdUseCase _getParticipantsByUserIdUseCase;

        public ParticipantService(
            CreateParticipantUseCase createParticipantUseCase,
            DeleteParticipantUseCase deleteParticipantUseCase,
            GetParticipantsByEventIdUseCase getParticipantsByEventIdUseCase,
            GetParticipantsByUserIdUseCase getParticipantsByUserIdUseCase)
        {
            _createParticipantUseCase = createParticipantUseCase;
            _deleteParticipantUseCase = deleteParticipantUseCase;
            _getParticipantsByEventIdUseCase = getParticipantsByEventIdUseCase;
            _getParticipantsByUserIdUseCase = getParticipantsByUserIdUseCase;
        }

        public async Task CreateAsync(CreateParticipantDTO participantDto, CancellationToken cancellationToken = default)
        {
            await _createParticipantUseCase.ExecuteAsync(participantDto, cancellationToken);
        }

        public async Task DeleteAsync(Guid participantId, CancellationToken cancellationToken = default)
        {
            await _deleteParticipantUseCase.ExecuteAsync(participantId, cancellationToken);
        }

        public async Task<IEnumerable<ParticipantDTO>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await _getParticipantsByEventIdUseCase.ExecuteAsync(eventId, cancellationToken);
        }
        
        public async Task<IEnumerable<ParticipantDTO>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _getParticipantsByUserIdUseCase.ExecuteAsync(userId, cancellationToken);
        }
    }
}
