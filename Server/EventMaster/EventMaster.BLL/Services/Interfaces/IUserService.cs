using EventMaster.BLL.DTOs.Requests.User;
using EventMaster.BLL.DTOs.Responses.User;

namespace EventMaster.BLL.Services.Interfaces;

public interface IUserService
{
    Task<TokenDTO> RegisterAsync(UserDTO userDto,CancellationToken cancellationToken=default);
    Task<TokenDTO> LoginAsync(UserDTO userDto,CancellationToken cancellationToken=default);
    Task<TokenDTO> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken=default);
    Task RevokeAsync(Guid userId, CancellationToken cancellationToken=default);
    Task<IEnumerable<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
}