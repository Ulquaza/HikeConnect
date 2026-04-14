using System.Security.Claims;

namespace HikeConnect.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool TryGetUserId(this ClaimsPrincipal user, out Guid userId)
        {
            var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(value, out userId);
        }
    }
}
