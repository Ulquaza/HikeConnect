using HikeConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HikeConnect.Infrastructure.Configurations.EntityFramework
{
    internal class BehavioralProfileConfiguration : IEntityTypeConfiguration<BehavioralProfile>
    {
        public void Configure(EntityTypeBuilder<BehavioralProfile> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.RiskTolerance)
                .IsRequired(true);

            builder.Property(e => e.PacingStyle)
                .HasMaxLength(1024)
                .IsRequired(true);

            builder.Property(e => e.DisciplineLevel)
                .IsRequired(true);

            builder.Property(e => e.ConflictStrategy)
                .HasMaxLength(1024)
                .IsRequired(true);

            builder.Property(e => e.LastUpdatedAt)
                .IsRequired(true);

            builder.HasOne(e => e.User)
                .WithOne(e => e.BehavioralProfile)
                .HasForeignKey<BehavioralProfile>(e => e.UserId)
                .IsRequired(true);
        }
    }
}
