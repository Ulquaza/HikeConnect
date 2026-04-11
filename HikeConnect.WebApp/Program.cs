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

            var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
            var apiBaseUri = Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var absoluteUri)
                ? absoluteUri
                : new Uri(new Uri(builder.HostEnvironment.BaseAddress), apiBaseUrl ?? "api/");

            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = apiBaseUri });

            await builder.Build().RunAsync();
        }
    }
}
