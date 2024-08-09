using System.Text;
using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Event;
using EventMaster.BLL.DTOs.Responses.Event;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Entities;
using EventMaster.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EventMaster.BLL.Services.Implementation;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public EventService(IUnitOfWork unitOfWork, IEmailService emailService, IMapper mapper, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task CreateAsync(CreateEventDTO createEventDto, CancellationToken cancellationToken = default)
    {
        var newEvent = _mapper.Map<Event>(createEventDto);

        if (createEventDto.Image != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await createEventDto.Image.CopyToAsync(memoryStream, cancellationToken);
                newEvent.Image = memoryStream.ToArray();
            }
        }

        await _unitOfWork.Events.CreateAsync(newEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
    }

    public async Task<IEnumerable<EventResponseDTO>> GetFilteredEventsAsync(EventFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = await _unitOfWork.Events.GetEventsQueryableAsync(cancellationToken);

        if (!string.IsNullOrEmpty(filter.Name))
        {
            var eventsByName = await _unitOfWork.Events.GetByNameAsync(filter.Name, filter.PageNumber, filter.PageSize, cancellationToken);
            return _mapper.Map<IEnumerable<EventResponseDTO>>(eventsByName);
        }

        if (filter.Date.HasValue)
        {
            query = query.Where(e => e.Date.Date == filter.Date.Value.Date);
        }

        if (filter.Location != null)
        {
            query = query.Where(e => e.Location.City.Contains(filter.Location.City) &&
                                     e.Location.Street.Contains(filter.Location.Street) &&
                                     e.Location.Building.Contains(filter.Location.Building));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == filter.CategoryId.Value);
        }

        var events = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);  

        return _mapper.Map<IEnumerable<EventResponseDTO>>(events);
    }

    public async Task<IEnumerable<EventResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var events = await _unitOfWork.Events.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<EventResponseDTO>>(events);
    }

    public async Task UpdateAsync(UpdateEventDTO updateEventDto, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(updateEventDto.Id, cancellationToken) ?? 
                          throw new EntityNotFoundException("Event", updateEventDto.Id);

        _mapper.Map(updateEventDto, eventEntity);

        if (updateEventDto.Image != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await updateEventDto.Image.CopyToAsync(memoryStream, cancellationToken);
                eventEntity.Image = memoryStream.ToArray();
            }
        }

        _unitOfWork.Events.Update(eventEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        await NotifyParticipantsAsync(eventEntity.Id, cancellationToken);
    }

    public async Task DeleteAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var eventById = await _unitOfWork.Events.GetByIdAsync(eventId, cancellationToken) ??
                        throw new EntityNotFoundException("Event", eventId);
        _unitOfWork.Events.Delete(eventById);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<EventResponseDTO> GetByIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"Event_{eventId}";

        if (!_cache.TryGetValue(cacheKey, out EventResponseDTO eventDto))
        {
            var eventById = await _unitOfWork.Events.GetByIdAsync(eventId, cancellationToken) ??
                            throw new EntityNotFoundException("Event", eventId);
            eventDto = _mapper.Map<EventResponseDTO>(eventById);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, eventDto, cacheOptions);
        }

        return eventDto;
    }

    private async Task NotifyParticipantsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var participants = await _unitOfWork.Participants.GetByEventIdAsync(eventId, cancellationToken);
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId, cancellationToken);

        foreach (var participant in participants)
        {
            var email = _mapper.Map<EventUpdateEmail>(eventEntity);
            email.Name = $"{participant.FirstName} {participant.LastName}";
            email.ToEmail = participant.Email;
            await _emailService.SendEmailAsync(email);
        }
    }
}
