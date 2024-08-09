using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.User;
using EventMaster.BLL.DTOs.Responses.Role;
using EventMaster.BLL.DTOs.Responses.User;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Helpers;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;
using EventMaster.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace EventMaster.BLL.Services.Implementation;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public UserService(IMapper mapper, IUnitOfWork unitOfWork, ITokenService tokenService, IConfiguration configuration)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<TokenDTO> RegisterAsync(UserDTO userDto, CancellationToken cancellationToken=default)
    {
        var userFromDb = await _unitOfWork.Users.GetByLoginAsync(userDto.Login, cancellationToken);

        if (userFromDb is not null)
        {
            throw new AuthorizationException($"User with login {userDto.Login} exists");
        }

        var newUser = _mapper.Map<User>(userDto); 
        var refreshToken = _tokenService.GenerateRefreshToken();

        newUser.PasswordHash = PasswordHelper.HashPassword(userDto.Password);
        newUser.RefreshToken = refreshToken;
        newUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt:RefreshTokenExpirationDays").Get<int>());

        await _unitOfWork.Users.CreateAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var user = await _unitOfWork.Users.GetByLoginAsync(newUser.Login, cancellationToken);
        var role = await _unitOfWork.Roles.GetByNameAsync("Resident", cancellationToken) ?? throw new InvalidOperationException("Base resident role are not found");

        await _unitOfWork.Roles.SetRoleToUserAsync(user.Id, role.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var claims = _tokenService.CreateClaims(user, new List<Role>(){role});
        var accessToken = _tokenService.GenerateAccessToken(claims);
        return new TokenDTO { RefreshToken = refreshToken, AccessToken = accessToken };
    }

    public async Task<TokenDTO> LoginAsync(UserDTO userDto, CancellationToken cancellationToken=default)
    {
        var user = await _unitOfWork.Users.GetByLoginAsync(userDto.Login, cancellationToken)
                     ?? throw new AuthorizationException($"User with login: {userDto.Login} does not exist");
        var isPasswordCorrect = PasswordHelper.VerifyPassword(user.PasswordHash, userDto.Password);

        if (!isPasswordCorrect)
        {
            throw new AuthorizationException("Incorrect password");
        }

        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt:RefreshTokenExpirationDays").Get<int>());

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var rolesByUser = await _unitOfWork.Roles.GetRolesByUserIdAsync(user.Id, cancellationToken);

        var claims = _tokenService.CreateClaims(user, rolesByUser.ToList());
        var accessToken = _tokenService.GenerateAccessToken(claims);

        return new TokenDTO { RefreshToken = refreshToken, AccessToken = accessToken };
    }

    public async Task<TokenDTO> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken=default)
    {
        var user = await _unitOfWork.Users.GetByRefreshTokenAsync(refreshToken, cancellationToken)
                     ?? throw new EntityNotFoundException("User not found");

        var rolesByUser = await _unitOfWork.Roles.GetRolesByUserIdAsync(user.Id, cancellationToken);

        var claims = _tokenService.CreateClaims(user, rolesByUser.ToList());
        var accessToken = _tokenService.GenerateAccessToken(claims);

        return new TokenDTO { RefreshToken = refreshToken, AccessToken = accessToken };
    }

    public async Task RevokeAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
                   ?? throw new EntityNotFoundException("User", userId);

        user.RefreshToken = string.Empty;
        user.RefreshTokenExpiryTime = DateTime.MinValue;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }


    public async Task<IEnumerable<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
    }
}
