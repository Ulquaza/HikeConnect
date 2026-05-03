using HikeConnect.Core.Dtos;
using HikeConnect.WebApp.Routing;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace HikeConnect.WebApp.Providers
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private static readonly AuthenticationState NotAuthenticatedState = new AuthenticationState(new ClaimsPrincipal());
        private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

        private ClaimsPrincipal? _user;
        private bool _initialized;

        public bool IsLoggedIn { get; private set; } = false;
        public bool RefreshFailed { get; private set; } = false;
        public Guid UserId { get; private set; } = Guid.Empty;
        public string UserEmail { get; private set; } = string.Empty;
        public string UserRole { get; private set; } = string.Empty;
        public string UserName { get; private set; } = string.Empty;
        public string AccessToken { get; private set; } = string.Empty;

        public bool IsAccessTokenExpired
        {
            get
            {
                // Без токена обновлять нечего — иначе каждый анонимный запрос лишний раз бьёт /auth/refresh.
                if (string.IsNullOrWhiteSpace(AccessToken)) return false;

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(AccessToken);
                var expiry = jwtToken.ValidTo;
                return expiry < DateTime.UtcNow.AddMinutes(1);
            }
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult((_user is null) ? NotAuthenticatedState : new(_user));

        public async Task InitializeAsync(HttpClient httpClient, Uri baseUri, CancellationToken cancellationToken = default)
        {
            if (_initialized) return;
            _initialized = true;

            try
            {
                var refreshUri = new Uri(baseUri, ApiRoutes.Auth.Refresh);
                using var refreshRequest = new HttpRequestMessage(HttpMethod.Get, refreshUri);
                refreshRequest.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

                using var response = await httpClient.SendAsync(refreshRequest, cancellationToken);
                if (!response.IsSuccessStatusCode) return;

                var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                var content = JsonSerializer.Deserialize<RefreshTokenResponse>(raw, JsonSerializerOptions);
                if (content?.AccessToken is not null)
                {
                    ResetRefreshFailed();
                    Login(content.AccessToken);
                }
            }
            catch
            {
                // На старте нельзя валить приложение из-за неудачного refresh.
            }
        }

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

        public void SetRefreshFailed()
        {
            RefreshFailed = true;
        }

        public void ResetRefreshFailed()
        {
            RefreshFailed = false;
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
