using EventMaster.BLL.DTOs.Implementations.Requests.User;
using EventMaster.BLL.DTOs.Responses.User;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.BLL.UseCases.User;

namespace EventMaster.BLL.Services.Implementation;

public class UserService : IUserService
{
    private readonly RegisterUserUseCase _registerUserUseCase;
    private readonly LoginUserUseCase _loginUserUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly RevokeTokenUseCase _revokeTokenUseCase;
    private readonly GetAllUsersUseCase _getAllUsersUseCase;

    public UserService(
        RegisterUserUseCase registerUserUseCase,
        LoginUserUseCase loginUserUseCase,
        RefreshTokenUseCase refreshTokenUseCase,
        RevokeTokenUseCase revokeTokenUseCase,
        GetAllUsersUseCase getAllUsersUseCase)
    {
        _registerUserUseCase = registerUserUseCase ?? throw new ArgumentNullException(nameof(registerUserUseCase));
        _loginUserUseCase = loginUserUseCase ?? throw new ArgumentNullException(nameof(loginUserUseCase));
        _refreshTokenUseCase = refreshTokenUseCase ?? throw new ArgumentNullException(nameof(refreshTokenUseCase));
        _revokeTokenUseCase = revokeTokenUseCase ?? throw new ArgumentNullException(nameof(revokeTokenUseCase));
        _getAllUsersUseCase = getAllUsersUseCase ?? throw new ArgumentNullException(nameof(getAllUsersUseCase));
    }

    public Task<TokenDTO> RegisterAsync(UserDTO userDto, CancellationToken cancellationToken = default)
    {
        return _registerUserUseCase.ExecuteAsync(userDto, cancellationToken);
    }

    public Task<TokenDTO> LoginAsync(UserDTO userDto, CancellationToken cancellationToken = default)
    {
        return _loginUserUseCase.ExecuteAsync(userDto, cancellationToken);
    }

    public Task<TokenDTO> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return _refreshTokenUseCase.ExecuteAsync(refreshToken, cancellationToken);
    }

    public Task RevokeAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _revokeTokenUseCase.ExecuteAsync(userId, cancellationToken);
    }

    public Task<IEnumerable<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _getAllUsersUseCase.ExecuteAsync(cancellationToken);
    }
}