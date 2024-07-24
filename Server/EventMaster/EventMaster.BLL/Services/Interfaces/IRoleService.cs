using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.BLL.Services.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> CheckHasUserRole(Guid roleId, CancellationToken cancellationToken);
}