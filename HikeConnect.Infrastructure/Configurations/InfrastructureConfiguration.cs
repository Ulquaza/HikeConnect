using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;
using HikeConnect.Core.Settings;
using HikeConnect.Infrastructure.Contexts;
using HikeConnect.Infrastructure.Crm;
using HikeConnect.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Net.Http.Headers;

namespace HikeConnect.Infrastructure.Configurations
{
    public static class InfrastructureConfiguration
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddScoped<IBehavioralProfileRepository, BehavioralProfileRepository>();
            services.AddScoped<ICompatibilityReportRepository, CompatibilityReportRepository>();
            services.AddScoped<IParticipationRequestRepository, ParticipationRequestRepository>();
            services.AddScoped<ITripRepository, TripRepository>();

            services.Configure<CrmTwentySettings>(configuration.GetSection(CrmTwentySettings.SectionName));
            services
                .AddHttpClient<TwentyCrmClient>((sp, client) =>
                {
                    var settings = sp.GetRequiredService<IOptions<CrmTwentySettings>>().Value;
                    if (!string.IsNullOrWhiteSpace(settings.BaseUrl)
                        && Uri.TryCreate(settings.BaseUrl.TrimEnd('/') + "/", UriKind.Absolute, out var baseUri))
                    {
                        client.BaseAddress = baseUri;
                    }

                    if (!string.IsNullOrWhiteSpace(settings.ApiKey))
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
                });
            services.AddScoped<ICrmClient>(sp => sp.GetRequiredService<TwentyCrmClient>());
            services.AddScoped<ICrmSyncService, CrmSyncService>();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.EnableDynamicJson();
            var npgsqlDataSource = dataSourceBuilder.Build();
            services.AddSingleton(npgsqlDataSource);

            services.AddDbContext<HikeConnectContext>(options =>
                options.UseNpgsql(npgsqlDataSource));

            services.AddIdentityCore<User>(options =>
            {
                // все эти значения стоят по умолчанию
                // options.Password.RequiredLength = 6;
                // options.Password.RequireNonAlphanumeric = true;
                // options.Password.RequireUppercase = true;
                // options.Password.RequireLowercase = true;
                // options.Password.RequireDigit = true;

                options.SignIn.RequireConfirmedEmail = false;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
            })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<HikeConnectContext>()
                .AddDefaultTokenProviders();
        }
    }
}
