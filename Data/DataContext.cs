using lizari.Entities;
using Microsoft.EntityFrameworkCore;

namespace lizari.Data;

public class DataContext : DbContext
{
    public DataContext(
        DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<ProductEntity> Products { get; set; } = null!;

    public DbSet<OrderEntity> Orders { get; set; } = null!;

    public DbSet<OrderItemEntity> OrderItems { get; set; } = null!;


    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // =========================
        // ТОВАРЫ
        // =========================

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasKey(product => product.Id);

            // Один товар поставщика нельзя добавить дважды
            entity.HasIndex(product =>
                    product.SupplierProductId)
                .IsUnique();

            entity.Property(product => product.Price)
                .HasPrecision(18, 2);

            entity.Property(product => product.WholesalePrice)
                .HasPrecision(18, 2);

            entity.Property(product => product.DiscountPercent)
                .HasPrecision(5, 2);

            entity.Property(product => product.DiscountPrice)
                .HasPrecision(18, 2);

            entity.Property(product => product.SupplierPrice)
                .HasPrecision(18, 2);

            entity.Property(product => product.AutoUpdatePrice)
                .HasDefaultValue(true);

            entity.Property(product => product.Picture)
                .HasMaxLength(2048);

            entity.Property(product => product.AdditionalPicture1)
                .HasMaxLength(2048);

            entity.Property(product => product.AdditionalPicture2)
                .HasMaxLength(2048);

            entity.Property(product => product.AdditionalPicture3)
                .HasMaxLength(2048);

        });


        // =========================
        // ЗАКАЗЫ
        // =========================

        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.HasKey(order => order.Id);

            entity.HasIndex(order => order.OrderNumber)
                .IsUnique();

            entity.Property(order => order.OrderNumber)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(order => order.RecipientName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(order => order.RecipientPhone)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(order => order.City)
    .HasMaxLength(150)
    .IsRequired();

            entity.Property(order => order.CityRef)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(order => order.DeliveryAddress)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(order => order.WarehouseRef)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(order => order.DeliveryMethod)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(order => order.Comment)
                .HasMaxLength(1000);

            entity.Property(order => order.TrackingNumber)
                .HasMaxLength(100);

            entity.Property(order => order.TotalPrice)
                .HasPrecision(18, 2);

            entity.Property(order =>
                    order.ReturnRiskPaymentAmount)
                .HasPrecision(18, 2);

            entity.Property(order =>
                    order.CustomerPaidAmount)
                .HasPrecision(18, 2);

            entity.Property(order =>
                    order.ReceivedPaymentAmount)
                .HasPrecision(18, 2);

            entity.Property(order =>
                    order.SupplierPaymentAmount)
                .HasPrecision(18, 2);

            entity.HasMany(order => order.Items)
                .WithOne(item => item.Order)
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

        });


        // =========================
        // ПОЗИЦИИ ЗАКАЗА
        // =========================

        modelBuilder.Entity<OrderItemEntity>(entity =>
        {
            entity.HasKey(item => item.Id);

            entity.Property(item => item.VendorCode)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(item => item.ProductTitle)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(item =>
                    item.SupplierProductName)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(item => item.UnitPrice)
                .HasPrecision(18, 2);

            entity.Property(item => item.TotalPrice)
                .HasPrecision(18, 2);
        });
    }
}