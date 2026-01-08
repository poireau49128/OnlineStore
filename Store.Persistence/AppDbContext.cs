using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;

namespace Store.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>

{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<ProductType> ProductTypes => Set<ProductType>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<ProductStock> ProductStocks => Set<ProductStock>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<CustomerCategoryDiscount> CustomerCategoryDiscount => Set<CustomerCategoryDiscount>();

    

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

        ConfigureCustomerCategoryDiscount(modelBuilder);

        ConfigureProductStock(modelBuilder);

        ConfigureCartItem(modelBuilder);

    }

    // ---------------- PRODUCT ----------------

    private static void ConfigureProduct(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Product>();

        entity.HasKey(p => p.Id);

        entity.HasIndex(p => p.Sku).IsUnique();
        entity.Property(p => p.RowVersion).IsRowVersion();

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
    }

    // ---------------- PRODUCT VARIANT ----------------

    private static void ConfigureProductVariant(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ProductVariant>();

        entity.HasKey(v => v.Id);

        entity.HasOne(v => v.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(v => v.ProductId);

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

        entity.HasIndex(c => new { c.ProductTypeId, c.Slug })
            .IsUnique();

        entity.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(150);

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

        entity.Property(pt => pt.Slug)
            .IsRequired()
            .HasMaxLength(150);

        entity.HasIndex(pt => pt.Slug).IsUnique();

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

        entity.HasOne<ApplicationUser>() 
            .WithMany() 
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        entity.Navigation(o => o.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    // ---------------- ORDER ITEM ----------------

    // private static void ConfigureOrderItem(ModelBuilder modelBuilder)
    // {
    //     var entity = modelBuilder.Entity<OrderItem>();

    //     entity.HasKey(oi => oi.Id);

    //     entity.Property(oi => oi.Comment)
    //         .HasMaxLength(1000);
        
    //     entity.Property(oi => oi.DiscountPercent)
    //         .HasPrecision(5, 2);

    //     entity.HasOne(oi => oi.Order)
    //         .WithMany(o => o.Items)
    //         .HasForeignKey("OrderId")
    //         .IsRequired()
    //         .OnDelete(DeleteBehavior.Cascade);

    //     entity.OwnsOne(oi => oi.UnitPrice, money =>
    //     {
    //         money.Property(m => m.Amount)
    //             .HasColumnName("UnitPriceAmount")
    //             .HasColumnType("decimal(18,2)")
    //             .IsRequired();

    //         money.Property(m => m.Currency)
    //             .HasColumnName("UnitPriceCurrency")
    //             .HasMaxLength(3)
    //             .IsRequired();
    //     });

    //     entity.Navigation(oi => oi.UnitPrice)
    //         .HasField("_unitPrice")
    //         .UsePropertyAccessMode(PropertyAccessMode.Field);
    // }
    private static void ConfigureOrderItem(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<OrderItem>();

        entity.HasKey(oi => oi.Id);

        entity.Property(oi => oi.Comment)
            .HasMaxLength(1000);
        
        entity.Property(oi => oi.DiscountPercent)
            .HasPrecision(5, 2);

        entity.HasOne(oi => oi. Order)
            .WithMany(o => o.Items)
            .HasForeignKey("OrderId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        entity. OwnsOne(oi => oi.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("UnitPriceAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });
    }

    // ---------------- DISCOUNT ----------------

    private static void ConfigureCustomerCategoryDiscount(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<CustomerCategoryDiscount>();

        entity.HasKey(d => d.Id);

        entity.HasOne<Store.Persistence.ApplicationUser>() 
            .WithMany(u => u.CategoryDiscounts)
            .HasForeignKey(d => d.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(d => d.Category)
          .WithMany()
          .HasForeignKey(d => d.CategoryId)
          .OnDelete(DeleteBehavior.Cascade);

        entity.Property(d => d.DiscountPercent)
            .HasPrecision(5, 2);
    }

    // ---------------- PRODUCTSTOCK ----------------

    private static void ConfigureProductStock(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ProductStock>();

        entity.HasKey(ps => ps.Id);

        entity.HasOne(ps => ps.ProductVariant)
            .WithMany(v => v.Stocks)
            .HasForeignKey(ps => ps.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(ps => ps.Warehouse)
            .WithMany()
            .HasForeignKey(ps => ps.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.Property(ps => ps.Quantity)
            .IsRequired();
    }

    // ---------------- CARTITEM ----------------

    private static void ConfigureCartItem(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<CartItem>();

        entity.HasKey(c => c.Id);

        entity.HasOne(c => c.ProductVariant)
            .WithMany()
            .HasForeignKey(c => c.ProductVariantId);

        entity.HasOne(c => c.Warehouse)
            .WithMany()
            .HasForeignKey(c => c.WarehouseId);

        entity.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.UserId);
    }
}
