using EventMaster.BLL.DTOs.Requests.UserRole;
using EventMaster.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.Areas.Admin.Controllers;


[Route("api/admin/role")]
[Authorize(Policy = "AdminArea")]
public class RoleController:Controller
{
    private IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleService.GetAllAsync(cancellationToken);
        return Ok(roles);
    }
    
    [HttpPut("setRoleToUser")]
    public async Task<IActionResult> SetRoleToUserAsync([FromBody] UserRoleDTO userRoleDto,
        CancellationToken cancellationToken = default)
    {
        await _roleService.SetRoleToUserAsync(userRoleDto, cancellationToken);
        return Ok();
    }
    
    [HttpPut("removeRoleFromUser")]
    public async Task<IActionResult> RemoveRoleFromUserAsync([FromBody] UserRoleDTO userRoleDto,
        CancellationToken cancellationToken = default)
    {
        await _roleService.RemoveRoleFromUserAsync(userRoleDto, cancellationToken);
        return Ok();
    }
    
    [HttpGet("getRolesByUser/{userId}")]
    public async Task<IActionResult> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var rolesByUser = await _roleService.GetRolesByUserIdAsync(userId, cancellationToken);
        return Ok(rolesByUser);
    }
    
}