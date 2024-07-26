using EventMaster.BLL.DTOs.Requests.Participant;
using EventMaster.BLL.DTOs.Responses.Participant;

namespace EventMaster.BLL.Services.Interfaces;

public interface IParticipantService
{
    Task CreateAsync(CreateParticipantDTO participantDto, CancellationToken cancellationToken=default);
    Task DeleteAsync(Guid participantId, CancellationToken cancellationToken=default);
    Task<IEnumerable<ParticipantDTO>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken=default);
    Task<IEnumerable<ParticipantDTO>> GetByUserId(Guid userId, CancellationToken cancellationToken=default);
}