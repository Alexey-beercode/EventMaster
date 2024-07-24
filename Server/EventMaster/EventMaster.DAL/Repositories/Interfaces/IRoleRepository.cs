using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.DAL.Repositories.Interfaces;

public interface IRoleRepository:IBaseRepository<Role>
{
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> CheckHasUserRoleAsync(Guid roleId, CancellationToken cancellationToken);
    Task SetRoleToUserAsync(UserRole userRole, CancellationToken cancellationToken);
}