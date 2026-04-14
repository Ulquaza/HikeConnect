using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HikeConnect.WebApp.Providers
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private static readonly AuthenticationState NotAuthenticatedState = new AuthenticationState(new ClaimsPrincipal());

        private ClaimsPrincipal? _user;
        public bool IsLoggedIn { get; private set; } = false;
        public Guid UserId { get; private set; } = Guid.Empty;
        public string UserEmail { get; private set; } = string.Empty;
        public string UserRole { get; private set; } = string.Empty;
        public string UserName { get; private set; } = string.Empty;
        public string AccessToken { get; private set; } = string.Empty;

        public bool IsAccessTokenExpired
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AccessToken)) return true;

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(AccessToken);
                var expiry = jwtToken.ValidTo;
                return expiry < DateTime.UtcNow.AddMinutes(1);
            }
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult((_user is null) ? NotAuthenticatedState : new(_user));

        public void Login(string accessToken)
        {
            AccessToken = accessToken;
            _user = CreateClaimsPrincipalFromJwt(AccessToken);
            IsLoggedIn = true;
            UserId = Guid.TryParse(_user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId) ? userId : Guid.Empty;
            UserEmail = _user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            UserRole = _user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            UserName = _user.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void Logout()
        {
            _user = null;
            IsLoggedIn = false;
            UserId = Guid.Empty;
            UserEmail = string.Empty;
            UserRole = string.Empty;
            UserName = string.Empty;
            AccessToken = string.Empty;

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private static ClaimsPrincipal CreateClaimsPrincipalFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var identity = new ClaimsIdentity(token.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
    }
}
