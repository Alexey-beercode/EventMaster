using AutoMapper;
using EventMaster.BLL.DTOs.Responses.EventCategory;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.Services.Implementation;

public class EventCategoryService:IEventCategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EventCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EventCategoryDTO>> GetAllAsync(CancellationToken cancellationToken=default)
    {
        var eventCategories = await _unitOfWork.EventCategories.GetAllAsync(cancellationToken);
        return _mapper.Map<List<EventCategoryDTO>>(eventCategories);
    }

    public async Task<EventCategoryDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken=default)
    {
        var eventCategoryById = await _unitOfWork.EventCategories.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<EventCategoryDTO>(eventCategoryById);
    }
}