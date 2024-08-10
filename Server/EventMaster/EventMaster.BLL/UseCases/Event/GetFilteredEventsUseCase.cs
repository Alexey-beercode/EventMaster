using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Event;
using EventMaster.BLL.DTOs.Implementations.Responses.Event;
using EventMaster.DAL.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.BLL.UseCases.Event;

public class GetFilteredEventsUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetFilteredEventsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EventResponseDTO>> ExecuteAsync(EventFilterDto filter, CancellationToken cancellationToken = default)
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
}