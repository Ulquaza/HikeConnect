using HikeConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HikeConnect.Infrastructure.Configurations.EntityFramework
{
    internal class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Description)
                .HasMaxLength(2048)
                .IsRequired(true);

            builder.Property(e => e.Status)
                .IsRequired(true);

            builder.Property(e => e.StartAt)
                .IsRequired(true);

            builder.Property(e => e.CreatedAt)
                .IsRequired(true);

            builder.HasOne(e => e.Author)
                .WithMany(e => e.Trips)
                .HasForeignKey(e => e.AuthorId)
                .IsRequired(true);
        }
    }
}
