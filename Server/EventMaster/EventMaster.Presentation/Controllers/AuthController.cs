using EventMaster.BLL.DTOs.Implementations.Requests.User;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.Controllers;

[Route("api/auth")]
[ModelValidator]
public class AuthController:Controller
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserDTO userDto,CancellationToken cancellationToken=default)
    {
        var token=await _userService.LoginAsync(userDto, cancellationToken);
        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserDTO userDto, CancellationToken cancellationToken = default)
    {
        var token = await _userService.RegisterAsync(userDto, cancellationToken);
        return Ok(token);
    }

    [Authorize]
    [HttpDelete("logout/{userId}")]
    public async Task<IActionResult> LogoutAsync(Guid userId, CancellationToken cancellationToken)
    {
        await _userService.RevokeAsync(userId, cancellationToken);
        return Ok();
    }
    
    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] string refreshToken, CancellationToken cancellationToken)
    {
        var token = await _userService.RefreshTokenAsync(refreshToken, cancellationToken);
        return Ok(token);
    }
}