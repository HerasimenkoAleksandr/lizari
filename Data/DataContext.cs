using lizari.Entities;
using Microsoft.EntityFrameworkCore;

namespace lizari.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<ProductEntity> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasKey(product => product.Id);

            // Один товар поставщика нельзя добавить дважды
            entity.HasIndex(product => product.SupplierProductId)
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
    }
}