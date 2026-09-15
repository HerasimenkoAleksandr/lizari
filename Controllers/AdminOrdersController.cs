using lizari.Data;
using lizari.Entities;
using lizari.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace lizari.Controllers;

[Authorize(Roles = "Administrator")]
[Route("admin/orders")]
public class AdminOrdersController : Controller
{
    private readonly DataContext _context;

    public AdminOrdersController(DataContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(bool archived = false)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.IsArchived == archived)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync();

        ViewBag.Archived = archived;

        return View(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Include(order => order.PaymentDetails)
            .FirstOrDefaultAsync(order => order.Id == id);

        if (order is null)
        {
            return NotFound();
        }

        var paymentDetails =
            await GetActivePaymentDetailsAsync();

        int? selectedPaymentDetailsId =
            order.PaymentDetailsId;

        if (!selectedPaymentDetailsId.HasValue)
        {
            selectedPaymentDetailsId =
                paymentDetails
                    .FirstOrDefault(details => details.IsDefault)
                    ?.Id;
        }

        var model = new AdminOrderEditViewModel
        {
            Id = order.Id,
            Order = order,
            Status = order.Status,

            PaymentMethod = order.PaymentMethod,
            PaymentDetailsId = selectedPaymentDetailsId,
            PaymentDetailsOptions =
                CreatePaymentDetailsOptions(paymentDetails),

            ConfirmedByCustomer =
                order.ConfirmedByCustomerAt is not null,

            SentToSupplier =
                order.SentToSupplierAt is not null,

            AcceptedBySupplier =
                order.AcceptedBySupplierAt is not null,

            SupplierPaid =
                order.SupplierPaidAt is not null,

            SupplierPaymentAmount =
                order.SupplierPaymentAmount,

            RequiresReturnRiskPayment =
                order.RequiresReturnRiskPayment,

            ReturnRiskPaid =
                order.ReturnRiskPaidAt is not null,

            ReturnRiskPaymentAmount =
                order.ReturnRiskPaymentAmount,

            ShippedBySupplier =
                order.ShippedBySupplierAt is not null,

            TrackingNumber =
                order.TrackingNumber,

            CustomerPaid =
                order.CustomerPaidAt is not null,

            CustomerPaidAmount =
                order.CustomerPaidAmount,

            PaymentReceived =
                order.PaymentReceivedAt is not null,

            ReceivedPaymentAmount =
                order.ReceivedPaymentAmount,

            ReceivedByCustomer =
                order.ReceivedByCustomerAt is not null,

            IsArchived =
                order.IsArchived
        };

        return View(model);
    }

    [HttpPost("{id:int}/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(
        int id,
        AdminOrderEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var order = await _context.Orders
            .Include(order => order.Items)
            .Include(order => order.PaymentDetails)
            .FirstOrDefaultAsync(order => order.Id == id);

        if (order is null)
        {
            return NotFound();
        }

        var activePaymentDetails =
            await GetActivePaymentDetailsAsync();

        PaymentDetailsEntity? selectedPaymentDetails = null;

        if (model.PaymentDetailsId.HasValue)
        {
            selectedPaymentDetails =
                activePaymentDetails.FirstOrDefault(
                    details =>
                        details.Id == model.PaymentDetailsId.Value);

            if (selectedPaymentDetails is null)
            {
                ModelState.AddModelError(
                    nameof(model.PaymentDetailsId),
                    "Обрані реквізити не знайдено або вимкнено.");
            }
        }

        // СПОСОБ ОПЛАТЫ И ПРЕДОПЛАТА РИСКА

        if (!Enum.IsDefined(
                typeof(PaymentMethod),
                model.PaymentMethod))
        {
            ModelState.AddModelError(
                nameof(model.PaymentMethod),
                "Оберіть спосіб оплати.");
        }

        if (model.PaymentMethod !=
            PaymentMethod.CashOnDelivery)
        {
            model.RequiresReturnRiskPayment = false;
            model.ReturnRiskPaid = false;
            model.ReturnRiskPaymentAmount = 0;
        }
        else if (!model.RequiresReturnRiskPayment)
        {
            model.ReturnRiskPaid = false;
            model.ReturnRiskPaymentAmount = 0;
        }

        if (model.RequiresReturnRiskPayment &&
            model.ReturnRiskPaymentAmount <= 0)
        {
            ModelState.AddModelError(
                nameof(model.ReturnRiskPaymentAmount),
                "Укажіть суму передплати ризику.");
        }

        if (model.ReturnRiskPaymentAmount >
            order.TotalPrice)
        {
            ModelState.AddModelError(
                nameof(model.ReturnRiskPaymentAmount),
                "Передплата ризику не може перевищувати суму замовлення.");
        }

        // ПРОВЕРКА ОПЛАТ

        if (model.SupplierPaid &&
            model.SupplierPaymentAmount <= 0)
        {
            ModelState.AddModelError(
                nameof(model.SupplierPaymentAmount),
                "Укажите сумму оплаты поставщику.");
        }

        if (model.CustomerPaid &&
            model.CustomerPaidAmount <= 0)
        {
            ModelState.AddModelError(
                nameof(model.CustomerPaidAmount),
                "Укажите сумму, оплаченную покупателем.");
        }

        if (model.PaymentReceived &&
            model.ReceivedPaymentAmount <= 0)
        {
            ModelState.AddModelError(
                nameof(model.ReceivedPaymentAmount),
                "Укажите сумму полученных денег.");
        }

        // ПРОВЕРКА ОТПРАВКИ

        if (model.ShippedBySupplier &&
            string.IsNullOrWhiteSpace(model.TrackingNumber))
        {
            ModelState.AddModelError(
                nameof(model.TrackingNumber),
                "Для отправленного товара укажите номер ТТН.");
        }

        // ПРОВЕРКА АРХИВА

        bool canArchive =
            model.ReceivedByCustomer ||
            model.Status == OrderStatus.Cancelled ||
            model.Status == OrderStatus.Returned;

        if (model.IsArchived && !canArchive)
        {
            ModelState.AddModelError(
                nameof(model.IsArchived),
                "В архив можно перенести полученный, " +
                "отменённый или возвращённый заказ.");
        }

        if (!ModelState.IsValid)
        {
            model.Order = order;

            model.PaymentDetailsOptions =
                CreatePaymentDetailsOptions(
                    activePaymentDetails);

            return View(nameof(Details), model);
        }

        DateTime updatedAt = DateTime.UtcNow;

        // СПОСОБ ОПЛАТЫ
        // Здесь сохраняется решение после разговора с покупателем.

        order.PaymentMethod = model.PaymentMethod;

        // ПЛАТЕЖНЫЕ РЕКВИЗИТЫ

        order.PaymentDetailsId =
            selectedPaymentDetails?.Id;

        order.PaymentDetailsSnapshot =
            selectedPaymentDetails is null
                ? string.Empty
                : CreatePaymentDetailsText(
                    selectedPaymentDetails);

        // ДАТЫ ЭТАПОВ

        order.ConfirmedByCustomerAt =
            SetDate(
                model.ConfirmedByCustomer,
                order.ConfirmedByCustomerAt,
                updatedAt);

        order.SentToSupplierAt =
            SetDate(
                model.SentToSupplier,
                order.SentToSupplierAt,
                updatedAt);

        order.AcceptedBySupplierAt =
            SetDate(
                model.AcceptedBySupplier,
                order.AcceptedBySupplierAt,
                updatedAt);

        order.SupplierPaidAt =
            SetDate(
                model.SupplierPaid,
                order.SupplierPaidAt,
                updatedAt);

        order.ReturnRiskPaidAt =
            SetDate(
                model.ReturnRiskPaid,
                order.ReturnRiskPaidAt,
                updatedAt);

        order.ShippedBySupplierAt =
            SetDate(
                model.ShippedBySupplier,
                order.ShippedBySupplierAt,
                updatedAt);

        order.CustomerPaidAt =
            SetDate(
                model.CustomerPaid,
                order.CustomerPaidAt,
                updatedAt);

        order.PaymentReceivedAt =
            SetDate(
                model.PaymentReceived,
                order.PaymentReceivedAt,
                updatedAt);

        order.ReceivedByCustomerAt =
            SetDate(
                model.ReceivedByCustomer,
                order.ReceivedByCustomerAt,
                updatedAt);

        // СУММЫ, РИСК И ТТН

        order.SupplierPaymentAmount =
            model.SupplierPaymentAmount;

        order.RequiresReturnRiskPayment =
            model.RequiresReturnRiskPayment;

        order.ReturnRiskPaymentAmount =
            model.ReturnRiskPaymentAmount;

        order.CustomerPaidAmount =
            model.CustomerPaidAmount;

        order.ReceivedPaymentAmount =
            model.ReceivedPaymentAmount;

        order.TrackingNumber =
            model.TrackingNumber?.Trim()
            ?? string.Empty;

        // ТЕКУЩИЙ СТАТУС

        if (model.Status is
            OrderStatus.Cancelled or
            OrderStatus.Returned)
        {
            order.Status = model.Status;
        }
        else if (model.ReceivedByCustomer)
        {
            order.Status =
                OrderStatus.ReceivedByCustomer;
        }
        else if (model.ShippedBySupplier)
        {
            order.Status =
                OrderStatus.ShippedBySupplier;
        }
        else if (model.AcceptedBySupplier)
        {
            order.Status =
                OrderStatus.AcceptedBySupplier;
        }
        else if (model.SentToSupplier)
        {
            order.Status =
                OrderStatus.SentToSupplier;
        }
        else if (model.ConfirmedByCustomer)
        {
            order.Status =
                OrderStatus.ConfirmedByCustomer;
        }
        else
        {
            order.Status =
                OrderStatus.New;
        }

        // АРХИВ

        order.IsArchived =
            model.IsArchived;

        order.ArchivedAt =
            SetDate(
                model.IsArchived,
                order.ArchivedAt,
                updatedAt);

        order.UpdatedAt =
            updatedAt;

        await _context.SaveChangesAsync();

        TempData["Success"] =
            $"Заказ {order.OrderNumber} сохранён.";

        if (order.IsArchived)
        {
            return RedirectToAction(
                nameof(Index),
                new { archived = true });
        }

        return RedirectToAction(
            nameof(Details),
            new { id = order.Id });
    }

    private async Task<List<PaymentDetailsEntity>>
        GetActivePaymentDetailsAsync()
    {
        return await _context.PaymentDetails
            .AsNoTracking()
            .Where(details => details.IsActive)
            .OrderByDescending(details => details.IsDefault)
            .ThenBy(details => details.Name)
            .ToListAsync();
    }

    private static IReadOnlyList<SelectListItem>
        CreatePaymentDetailsOptions(
            IEnumerable<PaymentDetailsEntity> paymentDetails)
    {
        return paymentDetails
            .Select(details => new SelectListItem
            {
                Value = details.Id.ToString(),
                Text = details.Name
            })
            .ToList();
    }

    private static string CreatePaymentDetailsText(
        PaymentDetailsEntity details)
    {
        var lines = new List<string>();

        if (!string.IsNullOrWhiteSpace(
            details.CardNumber))
        {
            lines.Add(
                $"Картка: {details.CardNumber}");
        }

        if (!string.IsNullOrWhiteSpace(
            details.Iban))
        {
            lines.Add(
                $"IBAN: {details.Iban}");
        }

        if (!string.IsNullOrWhiteSpace(
            details.RecipientName))
        {
            lines.Add(
                $"Отримувач: {details.RecipientName}");
        }

        if (!string.IsNullOrWhiteSpace(
            details.TaxNumber))
        {
            lines.Add(
                $"ІПН / ЄДРПОУ: {details.TaxNumber}");
        }

        if (!string.IsNullOrWhiteSpace(
            details.BankName))
        {
            lines.Add(
                $"Банк: {details.BankName}");
        }

        if (!string.IsNullOrWhiteSpace(
            details.AdditionalInformation))
        {
            lines.Add(
                details.AdditionalInformation);
        }

        return string.Join(
            Environment.NewLine,
            lines);
    }

    private static DateTime? SetDate(
        bool isCompleted,
        DateTime? currentDate,
        DateTime updatedAt)
    {
        if (!isCompleted)
        {
            return null;
        }

        return currentDate ?? updatedAt;
    }
}