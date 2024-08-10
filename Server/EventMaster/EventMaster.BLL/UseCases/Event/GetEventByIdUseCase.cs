using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Responses.Event;
using EventMaster.BLL.Exceptions;
using EventMaster.DAL.Infrastructure;
using Microsoft.Extensions.Caching.Memory;

namespace EventMaster.BLL.UseCases.Event;

public class GetEventByIdUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public GetEventByIdUseCase(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<EventResponseDTO> ExecuteAsync(Guid eventId, CancellationToken cancellationToken = default)
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
}