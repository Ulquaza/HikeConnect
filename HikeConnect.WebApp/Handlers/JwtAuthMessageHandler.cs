using HikeConnect.Core.Dtos;
using HikeConnect.WebApp.Providers;
using HikeConnect.WebApp.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HikeConnect.WebApp.Handlers
{
    public class JwtAuthMessageHandler : DelegatingHandler
    {
        private readonly JwtAuthStateProvider _authProvider;
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUri;
        private static readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);

        public JwtAuthMessageHandler(JwtAuthStateProvider authProvider, Uri baseUri, IHttpClientFactory factory)
        {
            _authProvider = authProvider;
            _baseUri = baseUri;
            _httpClient = factory.CreateClient("NoAuth");
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _authProvider.AccessToken;

            if (_authProvider.IsAccessTokenExpired && !_authProvider.RefreshFailed)
            {
                await _refreshLock.WaitAsync(cancellationToken);
                try
                {
                    // повторно проверить: возможно, другой запрос уже обновил токен
                    if (_authProvider.IsAccessTokenExpired)
                    {
                        var refreshUri = new Uri(_baseUri, ApiRoutes.Auth.Refresh);
                        var refreshRequest = new HttpRequestMessage(HttpMethod.Get, refreshUri);
                        refreshRequest.SetBrowserRequestCredentials(BrowserRequestCredentials.Include); // не добавляем токен к запросу обновления

                        var response = await _httpClient.SendAsync(refreshRequest, cancellationToken); // кидает ошибку 401 в браузере
                        var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                        Console.WriteLine($"REFRESH: {response.StatusCode} {raw}"); // временно, для дебага на сервере
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>(cancellationToken: cancellationToken);
                            if (content?.AccessToken is not null)
                            {
                                _authProvider.ResetRefreshFailed();
                                _authProvider.Login(content.AccessToken);
                            }
                        }
                        else
                        {
                            _authProvider.SetRefreshFailed();
                            _authProvider.Logout();
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
