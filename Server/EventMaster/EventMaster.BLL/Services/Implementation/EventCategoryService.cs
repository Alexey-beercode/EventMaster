using EventMaster.BLL.DTOs.Responses.EventCategory;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.BLL.UseCases;
using EventMaster.BLL.UseCases.EventCategory;

namespace EventMaster.BLL.Services.Implementation;

public class EventCategoryService : IEventCategoryService
{
    private readonly GetAllEventCategoriesUseCase _getAllEventCategoriesUseCase;
    private readonly GetEventCategoryByIdUseCase _getEventCategoryByIdUseCase;

    public EventCategoryService(GetAllEventCategoriesUseCase getAllEventCategoriesUseCase, GetEventCategoryByIdUseCase getEventCategoryByIdUseCase)
    {
        _getAllEventCategoriesUseCase = getAllEventCategoriesUseCase;
        _getEventCategoryByIdUseCase = getEventCategoryByIdUseCase;
    }

    public async Task<IEnumerable<EventCategoryDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _getAllEventCategoriesUseCase.ExecuteAsync(cancellationToken);
    }

    public async Task<EventCategoryDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _getEventCategoryByIdUseCase.ExecuteAsync(id, cancellationToken);
    }
}