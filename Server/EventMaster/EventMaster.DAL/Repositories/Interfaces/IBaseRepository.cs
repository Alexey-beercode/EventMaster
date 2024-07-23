namespace EventMaster.DAL.Repositories.Interfaces;

public interface IBaseRepository<T>
{
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
    Task CreateAsync(T entity, CancellationToken cancellationToken);
    void Delete(T entity);
}