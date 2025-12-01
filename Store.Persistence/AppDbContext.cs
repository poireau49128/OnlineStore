using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;

namespace Store.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<ProductType> ProductTypes => Set<ProductType>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<StockEntry> StockEntries => Set<StockEntry>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureProduct(modelBuilder);
        ConfigureProductVariant(modelBuilder);
        ConfigureProductImage(modelBuilder);

        ConfigureCategory(modelBuilder);
        ConfigureProductType(modelBuilder);

        ConfigureOrder(modelBuilder);
        ConfigureOrderItem(modelBuilder);
    }

    // ---------------- PRODUCT ----------------

    private static void ConfigureProduct(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Product>();

        entity.HasKey(p => p.Id);

        entity.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(p => p.Description)
            .HasMaxLength(2000);

        entity.Property(p => p.BaseColor)
            .HasMaxLength(50);

        entity.Property(p => p.BaseSize)
            .HasMaxLength(50);

        entity.OwnsOne(p => p.BasePrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("BasePriceAmount")
                .HasColumnType("decimal(18,2)");

            money.Property(m => m.Currency)
                .HasColumnName("BasePriceCurrency")
                .HasMaxLength(3);
        });

        entity.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany<ProductImage>()
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    // ---------------- PRODUCT VARIANT ----------------

    private static void ConfigureProductVariant(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ProductVariant>();

        entity.HasKey(v => v.Id);

        entity.Property(v => v.Color)
            .IsRequired()
            .HasMaxLength(50);

        entity.Property(v => v.Size)
            .HasMaxLength(50);

        // OwnsOne для OverridePrice (если Money уже есть)
        entity.OwnsOne(v => v.OverridePrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("OverridePriceAmount")
                .HasColumnType("decimal(18,2)");

            money.Property(m => m.Currency)
                .HasColumnName("OverridePriceCurrency")
                .HasMaxLength(3);
        });

        entity.HasMany<ProductImage>()
            .WithOne(i => i.ProductVariant)
            .HasForeignKey(i => i.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    // ---------------- PRODUCT IMAGE ----------------

    private static void ConfigureProductImage(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ProductImage>();

        entity.HasKey(i => i.Id);

        entity.Property(i => i.RelativePath)
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(i => i.SortOrder)
            .IsRequired();

        entity.HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(i => i.ProductVariant)
            .WithMany(v => v.Images)
            .HasForeignKey(i => i.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    // ---------------- CATEGORY ----------------

    private static void ConfigureCategory(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Category>();

        entity.HasKey(c => c.Id);

        entity.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(c => c.ImagePath)
            .HasMaxLength(500);

        entity.HasOne(c => c.ProductType)
            .WithMany(pt => pt.Categories)
            .HasForeignKey(c => c.ProductTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    // ---------------- PRODUCT TYPE ----------------

    private static void ConfigureProductType(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ProductType>();

        entity.HasKey(pt => pt.Id);

        entity.Property(pt => pt.Name)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(pt => pt.ImagePath)
            .HasMaxLength(500);

        entity.Property(pt => pt.SortOrder);
    }

    // ---------------- ORDER ----------------

    private static void ConfigureOrder(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Order>();

        entity.HasKey(o => o.Id);

        entity.Property(o => o.UserId)
            .IsRequired();

        entity.Property(o => o.Comment)
            .HasMaxLength(1000);
    }

    // ---------------- ORDER ITEM ----------------

    private static void ConfigureOrderItem(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<OrderItem>();

        entity.HasKey(oi => oi.Id);

        entity.Property(oi => oi.Comment)
            .HasMaxLength(1000);

        entity.OwnsOne(oi => oi.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("UnitPriceAmount")
                .HasColumnType("decimal(18,2)");

            money.Property(m => m.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3);
        });
    }
}
