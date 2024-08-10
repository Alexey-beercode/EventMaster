using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Responses.Event;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Event;

public class GetAllEventsUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllEventsUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EventResponseDTO>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var events = await _unitOfWork.Events.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<EventResponseDTO>>(events);
    }
}