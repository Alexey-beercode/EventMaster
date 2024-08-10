using EventMaster.BLL.DTOs.Implementations.Requests.UserRole;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Role
{
    public class RemoveRoleFromUserUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveRoleFromUserUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken = default)
        {
            var isSuccess = await _unitOfWork.Roles.RemoveRoleFromUserAsync(userRoleDto.UserId, userRoleDto.RoleId, cancellationToken);
            if (!isSuccess)
            {
                throw new InvalidOperationException($"Role with id : {userRoleDto.RoleId} cannot be removed from user with id : {userRoleDto.UserId}");
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}