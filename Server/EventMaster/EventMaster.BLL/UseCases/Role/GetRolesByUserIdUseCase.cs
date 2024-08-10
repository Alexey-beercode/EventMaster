using AutoMapper;
using EventMaster.BLL.DTOs.Responses.Role;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Role
{
    public class GetRolesByUserIdUseCase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GetRolesByUserIdUseCase(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoleDTO>> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var roles = await _unitOfWork.Roles.GetRolesByUserIdAsync(userId, cancellationToken);
            return _mapper.Map<IEnumerable<RoleDTO>>(roles);
        }
    }
}