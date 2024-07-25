using EventMaster.BLL.DTOs.Requests.User;

namespace EventMaster.BLL.Services.Interfaces;

public interface IUserService
{
    Task<TokenDTO> RegisterAsync(UserDTO userDto,CancellationToken cancellationToken);
    Task<TokenDTO> LoginAsync(UserDTO userDto,CancellationToken cancellationToken);
    Task<TokenDTO> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task RevokeAsync(Guid userId, CancellationToken cancellationToken);
}