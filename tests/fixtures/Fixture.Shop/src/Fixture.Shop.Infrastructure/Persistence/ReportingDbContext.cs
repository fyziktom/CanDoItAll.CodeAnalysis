using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixture.Shop.Infrastructure.Persistence;

public sealed class ReportingDbContext : DbContext
{
    public ReportingDbContext(DbContextOptions<ReportingDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("reporting");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

public sealed class ReportingSnapshot
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedUtc { get; set; }

    public string Label { get; set; } = string.Empty;
}

internal sealed class ReportingSnapshotConfiguration : IEntityTypeConfiguration<ReportingSnapshot>
{
    public void Configure(EntityTypeBuilder<ReportingSnapshot> builder)
    {
        builder.ToTable("ReportingSnapshots");
        builder.HasKey(snapshot => snapshot.Id);
        builder.Property(snapshot => snapshot.Label).HasMaxLength(120).IsRequired();
    }
}
