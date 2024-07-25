using EventMaster.BLL.DTOs.Requests.UserRole;
using EventMaster.BLL.DTOs.Responses.Role;
using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.BLL.Services.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDTO>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> CheckHasUserRole(Guid roleId, CancellationToken cancellationToken);
    Task SetRoleToUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken);
    Task RemoveRoleFromUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken);
    Task<IEnumerable<RoleDTO>> GetAllAsync(CancellationToken cancellationToken);
}