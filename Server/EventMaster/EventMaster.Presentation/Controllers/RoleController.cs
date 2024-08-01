using EventMaster.BLL.DTOs.Requests.UserRole;
using EventMaster.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.Controllers;

[Route("api/role")]
public class RoleController:Controller
{
    private IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [Authorize(Policy = "AdminArea")]
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleService.GetAllAsync(cancellationToken);
        return Ok(roles);
    }

    [Authorize(Policy = "AdminArea")]
    [HttpPut("setRoleToUser")]
    public async Task<IActionResult> SetRoleToUserAsync([FromBody] UserRoleDTO userRoleDto,
        CancellationToken cancellationToken = default)
    {
        await _roleService.SetRoleToUserAsync(userRoleDto, cancellationToken);
        return Ok();
    }

    [Authorize(Policy = "AdminArea")]
    [HttpPut("removeRoleFromUser")]
    public async Task<IActionResult> RemoveRoleFromUserAsync([FromBody] UserRoleDTO userRoleDto,
        CancellationToken cancellationToken = default)
    {
        await _roleService.RemoveRoleFromUserAsync(userRoleDto, cancellationToken);
        return Ok();
    }

    [Authorize(Policy = "AdminArea")]
    [HttpGet("getRolesByUser/{userId}")]
    public async Task<IActionResult> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var rolesByUser = await _roleService.GetRolesByUserIdAsync(userId, cancellationToken);
        return Ok(rolesByUser);
    }
    
}