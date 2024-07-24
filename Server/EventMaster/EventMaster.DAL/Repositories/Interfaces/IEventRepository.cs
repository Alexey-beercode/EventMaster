using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.DAL.Repositories.Interfaces;

public interface IEventRepository:IBaseRepository<Event>
{
    Task<IEnumerable<Event>> GetByNameAsync(string name, int pageNumber, int pageSize, CancellationToken cancellationToken);
    void Update(Event entity);
    Task<IQueryable<Event>> GetEventsQueryableAsync(CancellationToken cancellationToken);
}