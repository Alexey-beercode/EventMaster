using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.User;
using EventMaster.BLL.DTOs.Responses.User;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Helpers;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace EventMaster.BLL.UseCases.User;

public class LoginUserUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public LoginUserUseCase(IMapper mapper, IUnitOfWork unitOfWork, ITokenService tokenService, IConfiguration configuration)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<TokenDTO> ExecuteAsync(UserDTO userDto, CancellationToken cancellationToken = default)
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
}