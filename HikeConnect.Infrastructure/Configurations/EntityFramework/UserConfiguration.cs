using HikeConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HikeConnect.Infrastructure.Configurations.EntityFramework
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FullName)
                .HasMaxLength(128)
                .IsRequired(true);

            builder.Property(e => e.Bio)
                .HasMaxLength(2048)
                .IsRequired(false);

            builder.Property(e => e.RegisteredAt)
                .IsRequired(true);

            builder.Property(e => e.RefreshToken)
                .IsRequired(false);

            builder.Property(e => e.RefreshTokenExpiresAt)
                .IsRequired(false);

            builder.HasOne(e => e.Trip)
                .WithMany(e => e.ParticipationRequests)
                .HasForeignKey(e => e.TripId)
                .IsRequired(true);
        }
    }
}
