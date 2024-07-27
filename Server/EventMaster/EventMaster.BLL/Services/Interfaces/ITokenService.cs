using System.Security.Claims;
using EventMaster.BLL.DTOs.Responses.Role;
using EventMaster.Domain.Entities.Implementations;

namespace EventMaster.BLL.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    List<Claim> CreateClaims(User user,List<Role> roles);
}