using EventMaster.BLL.DTOs.Implementations.Requests.UserRole;
using EventMaster.BLL.DTOs.Responses.Role;
using EventMaster.BLL.UseCases.Role;
using EventMaster.BLL.Services.Interfaces;

namespace EventMaster.BLL.Services.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly GetRolesByUserIdUseCase _getRolesByUserIdUseCase;
        private readonly CheckUserHasRoleUseCase _checkUserHasRoleUseCase;
        private readonly SetRoleToUserUseCase _setRoleToUserUseCase;
        private readonly RemoveRoleFromUserUseCase _removeRoleFromUserUseCase;
        private readonly GetAllRolesUseCase _getAllRolesUseCase;

        public RoleService(
            GetRolesByUserIdUseCase getRolesByUserIdUseCase,
            CheckUserHasRoleUseCase checkUserHasRoleUseCase,
            SetRoleToUserUseCase setRoleToUserUseCase,
            RemoveRoleFromUserUseCase removeRoleFromUserUseCase,
            GetAllRolesUseCase getAllRolesUseCase)
        {
            _getRolesByUserIdUseCase = getRolesByUserIdUseCase;
            _checkUserHasRoleUseCase = checkUserHasRoleUseCase;
            _setRoleToUserUseCase = setRoleToUserUseCase;
            _removeRoleFromUserUseCase = removeRoleFromUserUseCase;
            _getAllRolesUseCase = getAllRolesUseCase;
        }

        public Task<IEnumerable<RoleDTO>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
            _getRolesByUserIdUseCase.ExecuteAsync(userId, cancellationToken);

        public Task<bool> CheckUserHasRoleAsync(Guid roleId, CancellationToken cancellationToken = default) =>
            _checkUserHasRoleUseCase.ExecuteAsync(roleId, cancellationToken);

        public Task SetRoleToUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken = default) =>
            _setRoleToUserUseCase.ExecuteAsync(userRoleDto, cancellationToken);

        public Task RemoveRoleFromUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken = default) =>
            _removeRoleFromUserUseCase.ExecuteAsync(userRoleDto, cancellationToken);

        public Task<IEnumerable<RoleDTO>> GetAllAsync(CancellationToken cancellationToken = default) =>
            _getAllRolesUseCase.ExecuteAsync(cancellationToken);
    }
}
