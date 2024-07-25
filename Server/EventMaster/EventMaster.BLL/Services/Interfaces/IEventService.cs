using EventMaster.BLL.DTOs.Requests.Event;
using EventMaster.BLL.DTOs.Responses.Event;
using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.BLL.Services.Interfaces;

public interface IEventService
{
    Task CreateAsync(EventDTO eventDto, CancellationToken cancellationToken);
    Task<IEnumerable<EventResponseDTO>> GetFilteredEventsAsync(EventFilterDto filter, CancellationToken cancellationToken);
    Task<IEnumerable<EventResponseDTO>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(EventDTO eventDto, CancellationToken cancellationToken);
    Task DeleteAsync(Guid eventId, CancellationToken cancellationToken);
    Task<EventResponseDTO> GetByIdAsync(Guid eventId, CancellationToken cancellationToken);

}