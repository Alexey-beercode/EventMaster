using EventMaster.BLL.DTOs.Responses.User;
using EventMaster.BLL.Exceptions;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.User;

public class RefreshTokenUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public RefreshTokenUseCase(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<TokenDTO> ExecuteAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByRefreshTokenAsync(refreshToken, cancellationToken)
                   ?? throw new EntityNotFoundException("User not found");

        var rolesByUser = await _unitOfWork.Roles.GetRolesByUserIdAsync(user.Id, cancellationToken);

        var claims = _tokenService.CreateClaims(user, rolesByUser.ToList());
        var accessToken = _tokenService.GenerateAccessToken(claims);

        return new TokenDTO { RefreshToken = refreshToken, AccessToken = accessToken };
    }
}