using EventMaster.BLL.DTOs.Implementations.Requests.Event;
using EventMaster.BLL.DTOs.Responses.Event;
using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.BLL.Services.Interfaces;

public interface IEventService
{
    Task CreateAsync(CreateEventDTO createEventDto, CancellationToken cancellationToken=default);
    Task<IEnumerable<EventResponseDTO>> GetFilteredEventsAsync(EventFilterDto filter, CancellationToken cancellationToken=default);
    Task<IEnumerable<EventResponseDTO>> GetAllAsync(CancellationToken cancellationToken=default);
    Task UpdateAsync(UpdateEventDTO updateEventDto, CancellationToken cancellationToken=default);
    Task DeleteAsync(Guid eventId, CancellationToken cancellationToken=default);
    Task<EventResponseDTO> GetByIdAsync(Guid eventId, CancellationToken cancellationToken=default);

}