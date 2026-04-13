using System.Security.Claims;

namespace HikeConnect.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        int GetRefreshTokenExpirationDays();
    }
}
