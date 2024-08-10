using AutoMapper;
using EventMaster.BLL.DTOs.Responses.EventCategory;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.EventCategory;

public class GetEventCategoryByIdUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEventCategoryByIdUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EventCategoryDTO> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var eventCategoryById = await _unitOfWork.EventCategories.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<EventCategoryDTO>(eventCategoryById);
    }
}