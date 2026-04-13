using HikeConnect.Application.Services;
using HikeConnect.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HikeConnect.Application.Configurations
{
    public static class ApplicationConfiguration
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBehavioralProfileService, BehavioralProfileService>();
            services.AddScoped<IMatchingService, MatchingService>();
            services.AddScoped<IParticipationRequestService, ParticipationRequestService>();
            services.AddScoped<ITripService, TripService>();
        }
    }
}
