using Microsoft.EntityFrameworkCore;

namespace Store.DataMigration;

public class OldProduct
{
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    public int Quantity_Grodno { get; set; }
    public int Quantity_Moscow { get; set; }
    public decimal Price { get; set; }
    public string? Sizes { get; set; }
    public string Category { get; set; } = null!;
    public string? FileName { get; set; }
    public string SubCategory { get; set; } = null!;
    public string? Description { get; set; }
}

public class OldStoreDbContext : DbContext
{
    public DbSet<OldProduct> Products => Set<OldProduct>();

    public OldStoreDbContext(DbContextOptions<OldStoreDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OldProduct>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(p => p.ProductId);

            entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Color).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Category).HasMaxLength(50).IsRequired();
            entity.Property(p => p.SubCategory).HasMaxLength(50).IsRequired();
        });
    }
}
