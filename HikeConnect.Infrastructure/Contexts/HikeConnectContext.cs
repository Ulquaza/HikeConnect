using HikeConnect.Core.Entities;
using HikeConnect.Infrastructure.Configurations.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HikeConnect.Infrastructure.Contexts
{
    internal class HikeConnectContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<BehavioralProfile> BehavioralProfiles { get; set; }
        public DbSet<CompatibilityReport> CompatibilityReports { get; set; }
        public DbSet<ParticipationRequest> ParticipationRequests { get; set; }
        public DbSet<Trip> Trips { get; set; }

        public HikeConnectContext(DbContextOptions options) : base(options)
        {
            Database.CanConnect();
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new BehavioralProfileConfiguration());
            builder.ApplyConfiguration(new CompatibilityReportConfiguration());
            builder.ApplyConfiguration(new ParticipationRequestConfiguration());
            builder.ApplyConfiguration(new TripConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());

            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        }
    }
}
