using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.UserRole;
using EventMaster.BLL.DTOs.Responses.Role;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.Services.Implementation;

public class RoleService:IRoleService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<RoleDTO>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken=default)
    {
        var rolesByUser = await _unitOfWork.Roles.GetRolesByUserIdAsync(userId, cancellationToken);
        return _mapper.Map<IEnumerable<RoleDTO>>(rolesByUser);
    }

    public async Task<bool> CheckUserHasRoleAsync(Guid roleId, CancellationToken cancellationToken=default)
    {
        return await _unitOfWork.Roles.CheckUserHasRoleAsync(roleId, cancellationToken);
    }

    public async Task SetRoleToUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken=default)
    {
        var isSuccess=await _unitOfWork.Roles.SetRoleToUserAsync(userRoleDto.UserId,userRoleDto.RoleId,cancellationToken);
        if (!isSuccess)
        {
            throw new Exception($"Role with id : {userRoleDto.RoleId} cannot have set to user with id : {userRoleDto.UserId}");
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRoleFromUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken=default)
    {
        var isSuccess =
            await _unitOfWork.Roles.RemoveRoleFromUserAsync(userRoleDto.UserId, userRoleDto.RoleId, cancellationToken);
        if (!isSuccess)
        {
            throw new InvalidOperationException($"Role with id : {userRoleDto.RoleId} cannot have removed from user with id : {userRoleDto.UserId}");
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

    }

    public async Task<IEnumerable<RoleDTO>> GetAllAsync(CancellationToken cancellationToken=default)
    {
        var roles = await _unitOfWork.Roles.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<RoleDTO>>(roles);
    }
}