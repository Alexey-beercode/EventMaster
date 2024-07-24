using EventMaster.BLL.DTOs.Requests.User;

namespace EventMaster.BLL.Services.Interfaces;

public interface IUserService
{
    Task<string> RegisterAsync(UserDTO userDto,CancellationToken cancellationToken);
    Task<string> LoginAsync(UserDTO userDto,CancellationToken cancellationToken);
    Task<string> RefreshTokenAsync(string token, CancellationToken cancellationToken);
    Task RevokeAsync(Guid userId, CancellationToken cancellationToken);
}