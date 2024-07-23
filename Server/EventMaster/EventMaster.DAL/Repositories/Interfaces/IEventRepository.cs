using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.DAL.Repositories.Interfaces;

public interface IEventRepository:IBaseRepository<Event>
{
    Task<Event> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<IEnumerable<Event>> GetByDateAsync(DateTime date, CancellationToken cancellationToken);
    Task<IEnumerable<Event>> GetByLocationAsync(string location, CancellationToken cancellationToken);
    Task<IEnumerable<Event>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken);
    void Update(Event entity);
}