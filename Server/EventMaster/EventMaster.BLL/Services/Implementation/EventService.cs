
using EventMaster.BLL.DTOs.Implementations.Requests.Event;
using EventMaster.BLL.DTOs.Implementations.Responses.Event;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.BLL.UseCases.Event;

namespace EventMaster.BLL.Services.Implementation;

public class EventService : IEventService
{
    private readonly CreateEventUseCase _createEventUseCase;
    private readonly GetAllEventsUseCase _getAllEventsUseCase;
    private readonly GetFilteredEventsUseCase _getFilteredEventsUseCase;
    private readonly UpdateEventUseCase _updateEventUseCase;
    private readonly DeleteEventUseCase _deleteEventUseCase;
    private readonly GetEventByIdUseCase _getEventByIdUseCase;

    public EventService(CreateEventUseCase createEventUseCase,
        GetAllEventsUseCase getAllEventsUseCase,
        GetFilteredEventsUseCase getFilteredEventsUseCase,
        UpdateEventUseCase updateEventUseCase,
        DeleteEventUseCase deleteEventUseCase,
        GetEventByIdUseCase getEventByIdUseCase)
    {
        _createEventUseCase = createEventUseCase;
        _getAllEventsUseCase = getAllEventsUseCase;
        _getFilteredEventsUseCase = getFilteredEventsUseCase;
        _updateEventUseCase = updateEventUseCase;
        _deleteEventUseCase = deleteEventUseCase;
        _getEventByIdUseCase = getEventByIdUseCase;
    }

    public Task CreateAsync(CreateEventDTO createEventDto, CancellationToken cancellationToken = default) =>
        _createEventUseCase.ExecuteAsync(createEventDto, cancellationToken);

    public Task<IEnumerable<EventResponseDTO>> GetFilteredEventsAsync(EventFilterDto filter, CancellationToken cancellationToken = default) =>
        _getFilteredEventsUseCase.ExecuteAsync(filter, cancellationToken);

    public Task<IEnumerable<EventResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _getAllEventsUseCase.ExecuteAsync(cancellationToken);

    public Task UpdateAsync(UpdateEventDTO updateEventDto, CancellationToken cancellationToken = default) =>
        _updateEventUseCase.ExecuteAsync(updateEventDto, cancellationToken);

    public Task DeleteAsync(Guid eventId, CancellationToken cancellationToken = default) =>
        _deleteEventUseCase.ExecuteAsync(eventId, cancellationToken);

    public Task<EventResponseDTO> GetByIdAsync(Guid eventId, CancellationToken cancellationToken = default) =>
        _getEventByIdUseCase.ExecuteAsync(eventId, cancellationToken);
}