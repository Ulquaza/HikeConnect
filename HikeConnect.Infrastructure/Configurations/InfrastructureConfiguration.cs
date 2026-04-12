using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;
using HikeConnect.Infrastructure.Contexts;
using HikeConnect.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddDbContext<HikeConnectContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

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
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<HikeConnectContext>()
                .AddDefaultTokenProviders();
        }
    }
}
