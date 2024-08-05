using EventMaster.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.Areas.Admin.Controllers;

[Area("admin")]
[Route("api/user")]
[Authorize(Policy = "AdminArea")]
public class UserController:Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }
    
}