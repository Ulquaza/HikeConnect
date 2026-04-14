using HikeConnect.Core.Dtos;
using HikeConnect.WebApp.Providers;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HikeConnect.WebApp.Handlers
{
    public class JwtAuthMessageHandler : DelegatingHandler
    {
        private readonly JwtAuthStateProvider _authProvider;
        private readonly string _baseUri;
        private static readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);
        private bool _refreshFailed = false;

        public JwtAuthMessageHandler(JwtAuthStateProvider authProvider, IConfiguration config)
        {
            _authProvider = authProvider;
            _baseUri = config["ApiBaseUrl"]!;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _authProvider.AccessToken;

            if (_authProvider.IsAccessTokenExpired && !_refreshFailed)
            {
                await _refreshLock.WaitAsync(cancellationToken);
                try
                {
                    // повторно проверить: возможно, другой запрос уже обновил токен
                    if (_authProvider.IsAccessTokenExpired)
                    {
                        var refreshUri = _baseUri + ApiRoutes.ApiRoutes.Auth.Refresh;
                        var refreshRequest = new HttpRequestMessage(HttpMethod.Get, refreshUri);
                        refreshRequest.SetBrowserRequestCredentials(BrowserRequestCredentials.Include); // не добавляем токен к запросу обновления

                        var response = await base.SendAsync(refreshRequest, cancellationToken); // кидает ошибку 401 в браузере
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>(cancellationToken: cancellationToken);
                            if (content?.AccessToken is not null)
                            {
                                _authProvider.Login(content.AccessToken);
                                _refreshFailed = false;
                            }
                        }
                        else
                        {
                            _authProvider.Logout();
                            _refreshFailed = true;
                        }
                    }
                }
                finally
                {
                    _refreshLock.Release();
                }

                token = _authProvider.AccessToken;
            }

            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);    // разрешить принимать куки с других origin
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
