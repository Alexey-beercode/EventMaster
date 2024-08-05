using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EventMaster.BLL.Services.Implementation;
using EventMaster.Domain.Entities.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();

        var jwtConfig = new Dictionary<string, string>
        {
            // Use a longer secret key (at least 32 bytes for HS256)
            { "Jwt:Secret", "super_secret_key_12345678901234567890123456789012" }, // 32 bytes key
            { "Jwt:Issuer", "http://localhost" },
            { "Jwt:Audience", "http://localhost" },
            { "Jwt:Expire", "1" }  // 1 hour for expiration
        };

        _configurationMock.Setup(c => c[It.IsAny<string>()]).Returns((string key) => jwtConfig[key]);

        _tokenService = new TokenService(_configurationMock.Object);
    }

    [Fact]
    public void GenerateAccessToken_ValidClaims_ReturnsToken()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "testuser")
        };

        // Act
        var token = _tokenService.GenerateAccessToken(claims);

        // Assert
        Assert.NotNull(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        Assert.Equal("http://localhost", jwtToken.Issuer);
        Assert.Equal("http://localhost", jwtToken.Audiences.First());
        Assert.Contains(claims, claim => jwtToken.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value));
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsValidToken()
    {
        // Act
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotNull(refreshToken);
        Assert.True(refreshToken.Length > 0);
    }

    [Fact]
    public void CreateClaims_ValidUserAndRoles_ReturnsClaims()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Login = "testuser" };
        var roles = new List<Role>
        {
            new Role { Name = "Admin" },
            new Role { Name = "User" }
        };

        // Act
        var claims = _tokenService.CreateClaims(user, roles);

        // Assert
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.NameIdentifier && claim.Value == user.Id.ToString());
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Name && claim.Value == user.Login);
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Admin");
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "User");
    }
}
