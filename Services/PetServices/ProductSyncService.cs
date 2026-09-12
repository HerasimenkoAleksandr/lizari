using lizari.Data;
using lizari.Entities;
using Microsoft.EntityFrameworkCore;

namespace lizari.Services.PetServices;

public class ProductSyncService : IProductSyncService
{
    private readonly DataContext _context;
    private readonly IProductFeedService _feedService;
    private readonly ILogger<ProductSyncService> _logger;

    public async Task<(bool Success, string Message)>
    RefreshSupplierProductAsync(int productId)
    {
        var productEntity = await _context.Products
            .FirstOrDefaultAsync(product => product.Id == productId);

        if (productEntity is null)
        {
            return (false, "Товар не найден в базе.");
        }

        var supplierProducts =
            await _feedService.GetProductsAsync();

        var supplierProduct = supplierProducts.FirstOrDefault(
            product =>
                product.Id == productEntity.SupplierProductId);

        if (supplierProduct is null)
        {
            return (
                false,
                "Товар не найден в XML поставщика.");
        }

        productEntity.SupplierName =
            supplierProduct.Name;

        productEntity.SupplierDescription =
            supplierProduct.Description;

        productEntity.SupplierPrice =
            supplierProduct.Price;

        productEntity.Available =
            supplierProduct.Available;

        productEntity.QuantityInStock =
            supplierProduct.QuantityInStock;

        productEntity.CategoryId =
            supplierProduct.CategoryId;

        productEntity.Picture =
            supplierProduct.Picture;

        productEntity.VendorCode =
            supplierProduct.VendorCode;

        productEntity.Vendor =
            supplierProduct.Vendor;

        if (productEntity.AutoUpdatePrice)
        {
            productEntity.Price =
                supplierProduct.Price;
        }

        productEntity.SupplierUpdatedAt =
            DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (
            true,
            "Информация поставщика полностью обновлена.");
    }
    public ProductSyncService(
        DataContext context,
        IProductFeedService feedService,
        ILogger<ProductSyncService> logger)
    {
        _context = context;
        _feedService = feedService;
        _logger = logger;
    }

    public async Task<(bool Success, string Message)>
        AddSelectedProductAsync(int supplierProductId)
    {
        if (supplierProductId <= 0)
        {
            return (false, "Укажите правильный ID товара.");
        }

        bool alreadyExists = await _context.Products.AnyAsync(
            product => product.SupplierProductId == supplierProductId);

        if (alreadyExists)
        {
            return (false, "Этот товар уже добавлен.");
        }

        var supplierProducts = await _feedService.GetProductsAsync();

        var supplierProduct = supplierProducts.FirstOrDefault(
            product => product.Id == supplierProductId);

        if (supplierProduct is null)
        {
            return (false, "Товар с таким ID не найден в XML.");
        }

        var productEntity = new ProductEntity
        {
            SupplierProductId = supplierProduct.Id,

            // Первоначальное название можно потом изменить вручную
            Title = supplierProduct.Name,
            Description = supplierProduct.Description,

            // Товар не показываем до ручного заполнения цены
            IsPublished = false,

            SupplierPrice = supplierProduct.Price,
            Available = supplierProduct.Available,
            QuantityInStock = supplierProduct.QuantityInStock,
            CategoryId = supplierProduct.CategoryId,
            Picture = supplierProduct.Picture,
            VendorCode = supplierProduct.VendorCode,
            Vendor = supplierProduct.Vendor,
            SupplierUpdatedAt = DateTime.UtcNow,
            SupplierName = supplierProduct.Name,
            SupplierDescription = supplierProduct.Description
        };

        _context.Products.Add(productEntity);
        await _context.SaveChangesAsync();

        return (true, "Товар успешно добавлен.");
    }

    public async Task<int> SyncSelectedProductsAsync()
    {
        var selectedProducts = await _context.Products.ToListAsync();

        if (selectedProducts.Count == 0)
        {
            return 0;
        }

        var selectedSupplierIds = selectedProducts
            .Select(product => product.SupplierProductId)
            .ToHashSet();

        var supplierProducts = await _feedService.GetProductsAsync();

        var supplierProductsById = supplierProducts
            .Where(product => selectedSupplierIds.Contains(product.Id))
            .GroupBy(product => product.Id)
            .ToDictionary(
                group => group.Key,
                group => group.First());

        int updatedCount = 0;
        DateTime updatedAt = DateTime.UtcNow;

        foreach (var productEntity in selectedProducts)
        {
            if (!supplierProductsById.TryGetValue(
                    productEntity.SupplierProductId,
                    out var supplierProduct))
            {
                _logger.LogWarning(
                    "Товар {SupplierProductId} отсутствует в XML.",
                    productEntity.SupplierProductId);

                continue;
            }

            // Обновляются только данные поставщика
            productEntity.SupplierPrice = supplierProduct.Price;
            productEntity.Available = supplierProduct.Available;
            productEntity.QuantityInStock =
                supplierProduct.QuantityInStock;
            productEntity.CategoryId = supplierProduct.CategoryId;
            productEntity.Picture = supplierProduct.Picture;
            productEntity.VendorCode = supplierProduct.VendorCode;
            productEntity.Vendor = supplierProduct.Vendor;
            productEntity.SupplierUpdatedAt = updatedAt;


            updatedCount++;
        }

        await _context.SaveChangesAsync();

        return updatedCount;
    }
}