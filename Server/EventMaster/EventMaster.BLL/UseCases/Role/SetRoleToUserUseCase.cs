using EventMaster.BLL.DTOs.Implementations.Requests.UserRole;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Role
{
    public class SetRoleToUserUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public SetRoleToUserUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken = default)
        {
            var isSuccess = await _unitOfWork.Roles.SetRoleToUserAsync(userRoleDto.UserId, userRoleDto.RoleId, cancellationToken);
            if (!isSuccess)
            {
                throw new InvalidOperationException($"Role with id : {userRoleDto.RoleId} cannot be set to user with id : {userRoleDto.UserId}");
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}