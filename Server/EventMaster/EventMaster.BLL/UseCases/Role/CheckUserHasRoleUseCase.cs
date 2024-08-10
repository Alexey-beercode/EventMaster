using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Role
{
    public class CheckUserHasRoleUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckUserHasRoleUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ExecuteAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Roles.CheckUserHasRoleAsync(roleId, cancellationToken);
        }
    }
}