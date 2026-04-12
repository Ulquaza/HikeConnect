using HikeConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HikeConnect.Infrastructure.Configurations.EntityFramework
{
    internal class CompatibilityReportConfiguration : IEntityTypeConfiguration<CompatibilityReport>
    {
        public void Configure(EntityTypeBuilder<CompatibilityReport> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.RiskPoints)
                .HasColumnType("jsonb");

            builder.Property(e => e.MatchPoints)
                .HasColumnType("jsonb");

            builder.Property(e => e.SummaryText)
                .HasMaxLength(2048)
                .IsRequired(true);

            builder.HasOne(e => e.Author)
                .WithMany(e => e.CompatibilityReportsWhereAuthor)
                .HasForeignKey(e => e.AuthorId)
                .IsRequired(true);

            builder.HasOne(e => e.Target)
                .WithMany(e => e.CompatibilityReportsWhereTarget)
                .HasForeignKey(e => e.TargetId)
                .IsRequired(true);
        }
    }
}
