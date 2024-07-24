namespace EventMaster.BLL.Services.Interfaces;

public interface IEventCategoryService
{
    Task<IEnumerable<IEventCategoryService>> GetAllAsync(CancellationToken cancellationToken);
}