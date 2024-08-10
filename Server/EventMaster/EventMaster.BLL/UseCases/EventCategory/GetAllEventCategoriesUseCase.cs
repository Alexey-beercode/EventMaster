using AutoMapper;
using EventMaster.BLL.DTOs.Responses.EventCategory;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases;

public class GetAllEventCategoriesUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllEventCategoriesUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EventCategoryDTO>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var eventCategories = await _unitOfWork.EventCategories.GetAllAsync(cancellationToken);
        return _mapper.Map<List<EventCategoryDTO>>(eventCategories);
    }
}