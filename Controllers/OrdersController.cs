using lizari.Data;
using lizari.Entities;
using lizari.Models.OrderModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lizari.Controllers;

[Route("order")]
public class OrdersController : Controller
{
    private readonly DataContext _context;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        DataContext context,
        ILogger<OrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }


    // =========================
    // ОТКРЫТИЕ ФОРМЫ ЗАКАЗА
    // =========================

    [HttpGet("create/{productId:int}")]
    public async Task<IActionResult> Create(int productId)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product =>
                product.Id == productId &&
                product.IsPublished);

        if (product is null)
        {
            return NotFound();
        }

        if (!product.Available ||
            product.QuantityInStock <= 0)
        {
            TempData["Error"] =
                "Цього товару зараз немає в наявності.";

            return RedirectToAction(
                "Product",
                "Pet",
                new { id = productId });
        }

        decimal unitPrice =
            product.DiscountPrice is > 0
                ? product.DiscountPrice.Value
                : product.Price;

        var model = new OrderCreateViewModel
        {
            ProductId = product.Id,
            ProductTitle = product.Title,
            ProductPicture = product.Picture,
            UnitPrice = unitPrice,
            Quantity = 1,
            PaymentMethod = PaymentMethod.CashOnDelivery
        };

        return View(model);
    }


    // =========================
    // СОХРАНЕНИЕ ЗАКАЗА
    // =========================

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        OrderCreateViewModel model)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product =>
                product.Id == model.ProductId &&
                product.IsPublished);

        if (product is null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Товар не знайдено.");
        }
        else
        {
            if (!product.Available ||
                product.QuantityInStock <= 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Цього товару зараз немає в наявності.");
            }

            if (model.Quantity >
                product.QuantityInStock)
            {
                ModelState.AddModelError(
                    nameof(model.Quantity),
                    $"Доступна кількість: " +
                    $"{product.QuantityInStock}.");
            }
        }

        if (!Enum.IsDefined(model.PaymentMethod))
        {
            ModelState.AddModelError(
                nameof(model.PaymentMethod),
                "Оберіть правильний спосіб оплати.");
        }

        if (!ModelState.IsValid ||
            product is null)
        {
            if (product is not null)
            {
                FillProductFields(model, product);
            }

            return View(model);
        }

        decimal unitPrice =
            product.DiscountPrice is > 0
                ? product.DiscountPrice.Value
                : product.Price;

        decimal totalPrice =
            unitPrice * model.Quantity;

        DateTime createdAt = DateTime.UtcNow;

        var order = new OrderEntity
        {
            OrderNumber = GenerateOrderNumber(),

            RecipientName =
                model.RecipientName.Trim(),

            RecipientPhone =
                model.RecipientPhone.Trim(),

            City =
    model.City.Trim(),

            CityRef =
    model.CityRef.Trim(),

            DeliveryAddress =
    model.DeliveryAddress.Trim(),

            WarehouseRef =
    model.WarehouseRef.Trim(),

            DeliveryMethod =
                "Нова Пошта",

            Comment =
                model.Comment?.Trim() ?? string.Empty,

            Status =
                OrderStatus.New,

            PaymentMethod =
                model.PaymentMethod,

            TotalPrice =
                totalPrice,

            ReturnRiskPaymentAmount =
                0,

            CustomerPaidAmount =
                0,

            ReceivedPaymentAmount =
                0,

            SupplierPaymentAmount =
                product.SupplierPrice * model.Quantity,

            CreatedAt =
                createdAt,

            UpdatedAt =
                createdAt,

            Items =
            {
                new OrderItemEntity
                {
                    ProductId =
                        product.Id,

                    SupplierProductId =
                        product.SupplierProductId,

                    VendorCode =
                        product.VendorCode?.Trim()
                        ?? string.Empty,

                    ProductTitle =
                        product.Title,

                    SupplierProductName =
                        string.IsNullOrWhiteSpace(
                            product.SupplierName)
                            ? product.Title
                            : product.SupplierName,

                    UnitPrice =
                        unitPrice,

                    Quantity =
                        model.Quantity,

                    TotalPrice =
                        totalPrice
                }
            }
        };

        _context.Orders.Add(order);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Создан заказ {OrderNumber}.",
            order.OrderNumber);

        return RedirectToAction(
            nameof(Success),
            new { orderNumber = order.OrderNumber });
    }


    // =========================
    // УСПЕШНОЕ ОФОРМЛЕНИЕ
    // =========================

    [HttpGet("success")]
    public async Task<IActionResult> Success(
     string orderNumber)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
        {
            return RedirectToAction(
                "Index",
                "Pet");
        }

        var order = await _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order =>
                order.OrderNumber == orderNumber);

        if (order is null)
        {
            return NotFound();
        }

        return View(order);
    }


    // =========================
    // ЗАПОЛНЕНИЕ ДАННЫХ ТОВАРА
    // =========================

    private static void FillProductFields(
        OrderCreateViewModel model,
        ProductEntity product)
    {
        model.ProductId = product.Id;
        model.ProductTitle = product.Title;
        model.ProductPicture = product.Picture;

        model.UnitPrice =
            product.DiscountPrice is > 0
                ? product.DiscountPrice.Value
                : product.Price;
    }


    // =========================
    // НОМЕР ЗАКАЗА
    // =========================

    private static string GenerateOrderNumber()
    {
        string uniquePart = Guid.NewGuid()
            .ToString("N")[..6]
            .ToUpperInvariant();

        return $"LZ-{DateTime.UtcNow:yyyyMMdd}-{uniquePart}";
    }
}