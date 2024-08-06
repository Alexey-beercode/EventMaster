using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EventMaster.BLL.Services.Implementation;
using EventMaster.Domain.Entities.Implementations;
using Microsoft.Extensions.Configuration;
using Moq;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();

        var jwtConfig = new Dictionary<string, string>
        {
            { "Jwt:Secret", "super_secret_key_12345678901234567890123456789012" },
            { "Jwt:Issuer", "http://localhost" },
            { "Jwt:Audience", "http://localhost" },
            { "Jwt:Expire", "1" } 
        };

        _configurationMock.Setup(c => c[It.IsAny<string>()]).Returns((string key) => jwtConfig[key]);

        _tokenService = new TokenService(_configurationMock.Object);
    }

    [Fact]
    public void GenerateAccessToken_ValidClaims_ReturnsToken()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "testuser")
        };
        
        var token = _tokenService.GenerateAccessToken(claims);
        
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
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        Assert.NotNull(refreshToken);
        Assert.True(refreshToken.Length > 0);
    }

    [Fact]
    public void CreateClaims_ValidUserAndRoles_ReturnsClaims()
    {
        var user = new User { Id = Guid.NewGuid(), Login = "testuser" };
        var roles = new List<Role>
        {
            new Role { Name = "Admin" },
            new Role { Name = "User" }
        };

        var claims = _tokenService.CreateClaims(user, roles);
        
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.NameIdentifier && claim.Value == user.Id.ToString());
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Name && claim.Value == user.Login);
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Admin");
        Assert.Contains(claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "User");
    }
}
