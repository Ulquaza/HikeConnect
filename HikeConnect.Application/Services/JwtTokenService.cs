using HikeConnect.Core.Interfaces;
using System.Security.Claims;

namespace HikeConnect.Application.Services
{
    internal class JwtTokenService : ITokenService
    {
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            throw new NotImplementedException();
        }

        public string GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }

        public int GetRefreshTokenExpirationDays()
        {
            throw new NotImplementedException();
        }
    }
}
