using HighHeavyShipment.Domain;
using Microsoft.EntityFrameworkCore;

namespace HighHeavyShipment.Infrastructure;

public class ShipmentDbContext : DbContext
{
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<ShipmentQuote> ShipmentQuotes { get; set; }

    public ShipmentDbContext(DbContextOptions<ShipmentDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Shipment configuration
        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Reference)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.Reference).IsUnique();

            entity.Property(e => e.Mode)
                .HasConversion<int>();

            entity.Property(e => e.Status)
                .HasConversion<int>();

            entity.Property(e => e.Origin)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Destination)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.WeightKg)
                .HasPrecision(18, 4);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasMany(e => e.Quotes)
                .WithOne(q => q.Shipment)
                .HasForeignKey(q => q.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ShipmentQuote configuration
        modelBuilder.Entity<ShipmentQuote>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Phase)
                .HasConversion<int>();

            entity.Property(e => e.Amount)
                .HasPrecision(18, 2);

            entity.Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(3);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            // One quote per shipment per phase
            entity.HasIndex(e => new { e.ShipmentId, e.Phase })
                .IsUnique();
        });
    }
}
