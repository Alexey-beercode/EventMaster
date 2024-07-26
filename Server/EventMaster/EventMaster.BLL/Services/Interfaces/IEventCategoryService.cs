using EventMaster.BLL.DTOs.Responses.EventCategory;

namespace EventMaster.BLL.Services.Interfaces;

public interface IEventCategoryService
{
    Task<IEnumerable<EventCategoryDTO>> GetAllAsync(CancellationToken cancellationToken=default);
    Task<EventCategoryDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken=default);
}