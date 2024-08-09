using EventMaster.BLL.DTOs.Implementations.Requests.UserRole;
using EventMaster.BLL.DTOs.Responses.Role;

namespace EventMaster.BLL.Services.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDTO>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken=default);
    Task<bool> CheckUserHasRoleAsync(Guid roleId, CancellationToken cancellationToken=default);
    Task SetRoleToUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken=default);
    Task RemoveRoleFromUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken=default);
    Task<IEnumerable<RoleDTO>> GetAllAsync(CancellationToken cancellationToken=default);
}