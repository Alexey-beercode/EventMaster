using AutoMapper;
using EventMaster.BLL.DTOs.Responses.Role;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Role
{
    public class GetAllRolesUseCase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GetAllRolesUseCase(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoleDTO>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var roles = await _unitOfWork.Roles.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<RoleDTO>>(roles);
        }
    }
}