using HikeConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HikeConnect.Infrastructure.Configurations.EntityFramework
{
    internal class ParticipationRequestConfiguration : IEntityTypeConfiguration<ParticipationRequest>
    {
        public void Configure(EntityTypeBuilder<ParticipationRequest> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Status)
                .IsRequired(true);

            builder.Property(e => e.CompatibilityPercentage)
                .IsRequired(true);

            builder.Property(e => e.AppliedAt)
                .IsRequired(true);

            builder.HasOne(e => e.Trip)
                .WithMany(e => e.ParticipationRequests)
                .HasForeignKey(e => e.TripId)
                .IsRequired(true);

            builder.HasOne(e => e.User)
                .WithMany(e => e.ParticipationRequests)
                .HasForeignKey(e => e.UserId)
                .IsRequired(true);
        }
    }
}
