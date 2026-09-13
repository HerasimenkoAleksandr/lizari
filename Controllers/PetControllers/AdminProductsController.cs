using lizari.Data;
using lizari.Entities;
using lizari.Models.Admin;
using lizari.Services.PetServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lizari.Controllers;

[Authorize(Roles = "Administrator")]
[Route("admin/products")]
public class AdminProductsController : Controller
{
    private readonly DataContext _context;
    private readonly IProductSyncService _productSyncService;

    public AdminProductsController(
        DataContext context,
        IProductSyncService productSyncService)
    {
        _context = context;
        _productSyncService = productSyncService;
    }

    // =========================
    // СПИСОК ТОВАРОВ
    // =========================

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .AsNoTracking()
            .OrderByDescending(product => product.Id)
            .ToListAsync();

        return View(products);
    }

    // =========================
    // ДОБАВЛЕНИЕ ТОВАРА
    // =========================
    [HttpPost("add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string? supplierProductId)
    {
        supplierProductId = supplierProductId?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(supplierProductId))
        {
            TempData["Error"] =
                "Введите ID товара или артикул поставщика.";

            return RedirectToAction(nameof(Index));
        }

        var result =
     await _productSyncService.AddSelectedProductAsync(
         supplierProductId);

        TempData[result.Success ? "Success" : "Error"] =
            result.Message;

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // ОБНОВЛЕНИЕ ЦЕН И ОСТАТКОВ
    // =========================

    [HttpPost("sync")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sync()
    {
        int updatedCount =
            await _productSyncService.SyncSelectedProductsAsync();

        TempData["Success"] =
            $"Обновлено товаров: {updatedCount}.";

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // ПОЛНОЕ ОБНОВЛЕНИЕ ТОВАРА
    // =========================

    [HttpPost("refresh-supplier/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RefreshSupplier(int id)
    {
        var result =
            await _productSyncService
                .RefreshSupplierProductAsync(id);

        TempData[result.Success ? "Success" : "Error"] =
            result.Message;

        return RedirectToAction(
            nameof(Edit),
            new { id });
    }

    // =========================
    // ОТКРЫТИЕ ФОРМЫ
    // =========================

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        var model = new ProductEditViewModel
        {
            Id = product.Id,

            // Данные поставщика
            SupplierProductId = product.SupplierProductId,
            SupplierName = product.SupplierName,
            SupplierDescription = product.SupplierDescription,
            SupplierPrice = product.SupplierPrice,
            Available = product.Available,
            QuantityInStock = product.QuantityInStock,
            CategoryId = product.CategoryId,
            SupplierPicture = product.Picture,
            VendorCode = product.VendorCode,
            Vendor = product.Vendor,
            SupplierUpdatedAt = product.SupplierUpdatedAt,

            // Ручные данные
            Title = product.Title,
            ShortDescription = product.ShortDescription,
            Description = product.Description,

            Price = product.Price,
            AutoUpdatePrice = product.AutoUpdatePrice,
            WholesalePrice = product.WholesalePrice,
            DiscountPrice = product.DiscountPrice,

            AdditionalPicture1 = product.AdditionalPicture1,
            AdditionalPicture2 = product.AdditionalPicture2,
            AdditionalPicture3 = product.AdditionalPicture3,

            Feature1Title = product.Feature1Title,
            Feature1Description = product.Feature1Description,

            Feature2Title = product.Feature2Title,
            Feature2Description = product.Feature2Description,

            Feature3Title = product.Feature3Title,
            Feature3Description = product.Feature3Description,

            Feature4Title = product.Feature4Title,
            Feature4Description = product.Feature4Description,

            Type = product.Type,
            Size = product.Size,
            Color = product.Color,

            SelectionTitle = product.SelectionTitle,
            SelectionDescription = product.SelectionDescription,
            IsMainProduct = product.IsMainProduct,

            IsPublished = product.IsPublished
        };

        return View(model);
    }

    // =========================
    // СОХРАНЕНИЕ ФОРМЫ
    // =========================

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ProductEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        decimal effectivePrice = model.AutoUpdatePrice
            ? product.SupplierPrice
            : model.Price;

        if (effectivePrice <= 0)
        {
            ModelState.AddModelError(
                nameof(model.Price),
                "Цена товара должна быть больше нуля.");
        }

        if (model.DiscountPrice is > 0 &&
            model.DiscountPrice >= effectivePrice)
        {
            ModelState.AddModelError(
                nameof(model.DiscountPrice),
                "Цена со скидкой должна быть меньше обычной цены.");
        }

        if (model.IsPublished)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError(
                    nameof(model.Title),
                    "Для публикации укажите название.");
            }

            bool hasImage =
                !string.IsNullOrWhiteSpace(product.Picture) ||
                !string.IsNullOrWhiteSpace(model.AdditionalPicture1) ||
                !string.IsNullOrWhiteSpace(model.AdditionalPicture2) ||
                !string.IsNullOrWhiteSpace(model.AdditionalPicture3);

            if (!hasImage)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Для публикации необходимо изображение.");
            }
        }

        if (!ModelState.IsValid)
        {
            FillSupplierFields(model, product);

            return View(model);
        }

        // Основная информация
        product.Title =
            model.Title?.Trim() ?? string.Empty;

        product.ShortDescription =
            model.ShortDescription?.Trim() ?? string.Empty;

        product.Description =
            model.Description?.Trim() ?? string.Empty;

        // Цены
        product.AutoUpdatePrice = model.AutoUpdatePrice;
        product.Price = effectivePrice;
        product.WholesalePrice = model.WholesalePrice;

        if (model.DiscountPrice is > 0)
        {
            product.DiscountPrice = model.DiscountPrice;

            product.DiscountPercent = Math.Round(
                (1 - model.DiscountPrice.Value / effectivePrice) * 100,
                2,
                MidpointRounding.AwayFromZero);
        }
        else
        {
            product.DiscountPrice = null;
            product.DiscountPercent = null;
        }

        // Дополнительные изображения
        product.AdditionalPicture1 =
            model.AdditionalPicture1?.Trim() ?? string.Empty;

        product.AdditionalPicture2 =
            model.AdditionalPicture2?.Trim() ?? string.Empty;

        product.AdditionalPicture3 =
            model.AdditionalPicture3?.Trim() ?? string.Empty;

        // Преимущества
        product.Feature1Title =
            model.Feature1Title?.Trim() ?? string.Empty;

        product.Feature1Description =
            model.Feature1Description?.Trim() ?? string.Empty;

        product.Feature2Title =
            model.Feature2Title?.Trim() ?? string.Empty;

        product.Feature2Description =
            model.Feature2Description?.Trim() ?? string.Empty;

        product.Feature3Title =
            model.Feature3Title?.Trim() ?? string.Empty;

        product.Feature3Description =
            model.Feature3Description?.Trim() ?? string.Empty;

        product.Feature4Title =
            model.Feature4Title?.Trim() ?? string.Empty;

        product.Feature4Description =
            model.Feature4Description?.Trim() ?? string.Empty;

        // Характеристики
        product.Type =
            model.Type?.Trim() ?? string.Empty;

        product.Size =
            model.Size?.Trim() ?? string.Empty;

        product.Color =
            model.Color?.Trim() ?? string.Empty;

        // Блок подбора
        product.SelectionTitle =
            model.SelectionTitle?.Trim() ?? string.Empty;

        product.SelectionDescription =
            model.SelectionDescription?.Trim() ?? string.Empty;

        if (model.IsMainProduct)
        {
            var otherMainProducts = await _context.Products
                .Where(item =>
                    item.Id != product.Id &&
                    item.IsMainProduct)
                .ToListAsync();

            foreach (var otherProduct in otherMainProducts)
            {
                otherProduct.IsMainProduct = false;
            }
        }

        product.IsMainProduct = model.IsMainProduct;
        product.IsPublished = model.IsPublished;

        product.IsPublished = model.IsPublished;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Товар успешно сохранён.";

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // ПОВТОРНОЕ ЗАПОЛНЕНИЕ
    // ДАННЫХ ПОСТАВЩИКА
    // =========================

    private static void FillSupplierFields(
        ProductEditViewModel model,
        ProductEntity product)
    {
        model.SupplierProductId = product.SupplierProductId;
        model.SupplierName = product.SupplierName;
        model.SupplierDescription = product.SupplierDescription;
        model.SupplierPrice = product.SupplierPrice;
        model.Available = product.Available;
        model.QuantityInStock = product.QuantityInStock;
        model.CategoryId = product.CategoryId;
        model.SupplierPicture = product.Picture;
        model.VendorCode = product.VendorCode;
        model.Vendor = product.Vendor;
        model.SupplierUpdatedAt = product.SupplierUpdatedAt;
    }

    // =========================
    // УДАЛЕНИЕ ТОВАРА
    // =========================

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product is null)
        {
            TempData["Error"] = "Товар не найден в базе.";

            return RedirectToAction(nameof(Index));
        }

        string productTitle = product.Title;

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        TempData["Success"] =
            $"Товар «{productTitle}» удалён из базы.";

        return RedirectToAction(nameof(Index));
    }
}