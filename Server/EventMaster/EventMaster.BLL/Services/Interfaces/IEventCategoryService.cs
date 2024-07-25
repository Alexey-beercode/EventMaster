using EventMaster.BLL.DTOs.Responses.EventCategory;

namespace EventMaster.BLL.Services.Interfaces;

public interface IEventCategoryService
{
    Task<IEnumerable<EventCategoryDTO>> GetAllAsync(CancellationToken cancellationToken);
    Task<EventCategoryDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}