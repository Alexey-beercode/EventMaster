using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.DAL.Repositories.Interfaces;

public interface IRoleRepository:IBaseRepository<Role>
{
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> CheckHasUserRole(Guid roleId, CancellationToken cancellationToken);
}