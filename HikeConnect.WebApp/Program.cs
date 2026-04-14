using HikeConnect.WebApp.Handlers;
using HikeConnect.WebApp.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace HikeConnect.WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddAuthorizationCore();

            var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
            var apiBaseUri = Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var absoluteUri)
                ? absoluteUri
                : new Uri(new Uri(builder.HostEnvironment.BaseAddress), apiBaseUrl ?? "api/");

            builder.Services.AddSingleton<JwtAuthStateProvider>();
            builder.Services.AddSingleton<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<JwtAuthStateProvider>());

            builder.Services.AddScoped<JwtAuthMessageHandler>();

            builder.Services.AddHttpClient("HikeConnect.Api", client =>
            {
                client.BaseAddress = apiBaseUri;
            }).AddHttpMessageHandler<JwtAuthMessageHandler>();

            builder.Services.AddScoped(sp =>
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("HikeConnect.Api"));

            await builder.Build().RunAsync();
        }
    }
}
