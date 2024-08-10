using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.User;
using EventMaster.BLL.DTOs.Responses.User;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Helpers;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace EventMaster.BLL.UseCases.User;

public class RegisterUserUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public RegisterUserUseCase(IMapper mapper, IUnitOfWork unitOfWork, ITokenService tokenService, IConfiguration configuration)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<TokenDTO> ExecuteAsync(UserDTO userDto, CancellationToken cancellationToken = default)
    {
        var userFromDb = await _unitOfWork.Users.GetByLoginAsync(userDto.Login, cancellationToken);

        if (userFromDb != null)
        {
            throw new AlreadyExistsException($"User with login {userDto.Login} exists");
        }

        var newUser = _mapper.Map<Domain.Entities.User>(userDto);
        var refreshToken = _tokenService.GenerateRefreshToken();

        newUser.PasswordHash = PasswordHelper.HashPassword(userDto.Password);
        newUser.RefreshToken = refreshToken;
        newUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetSection("Jwt:RefreshTokenExpirationDays").Get<int>());

        await _unitOfWork.Users.CreateAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var user = await _unitOfWork.Users.GetByLoginAsync(newUser.Login, cancellationToken);
        var role = await _unitOfWork.Roles.GetByNameAsync("Resident", cancellationToken) ?? throw new InvalidOperationException("Base resident role is not found");

        await _unitOfWork.Roles.SetRoleToUserAsync(user.Id, role.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var claims = _tokenService.CreateClaims(user, new List<Domain.Entities.Role> { role });
        var accessToken = _tokenService.GenerateAccessToken(claims);
        return new TokenDTO { RefreshToken = refreshToken, AccessToken = accessToken };
    }
}