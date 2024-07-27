using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.DAL.Repositories.Interfaces;

public interface IRoleRepository:IBaseRepository<Role>
{
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> CheckUserHasRoleAsync(Guid roleId, CancellationToken cancellationToken);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task SetRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}