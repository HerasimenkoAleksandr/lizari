using lizari.Data;
using lizari.Entities;
using lizari.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lizari.Controllers;

[Authorize(Roles = "Administrator")]
[Route("admin/payment-details")]
public class AdminPaymentDetailsController : Controller
{
    private readonly DataContext _context;

    public AdminPaymentDetailsController(
        DataContext context)
    {
        _context = context;
    }

    // =========================
    // СПИСОК РЕКВИЗИТОВ
    // =========================

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var paymentDetails = await _context.PaymentDetails
            .AsNoTracking()
            .OrderByDescending(details => details.IsDefault)
            .ThenByDescending(details => details.IsActive)
            .ThenBy(details => details.Name)
            .ToListAsync();

        return View(paymentDetails);
    }

    // =========================
    // ДОБАВЛЕНИЕ
    // =========================

    [HttpGet("create")]
    public IActionResult Create()
    {
        var model = new PaymentDetailsEditViewModel
        {
            IsActive = true
        };

        return View(model);
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        PaymentDetailsEditViewModel model)
    {
        ValidatePaymentDetails(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.IsDefault)
        {
            await RemoveDefaultFlagAsync();
        }

        var entity = new PaymentDetailsEntity();

        FillEntity(entity, model);

        _context.PaymentDetails.Add(entity);

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Реквізити успішно додано.";

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // РЕДАКТИРОВАНИЕ
    // =========================

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _context.PaymentDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(details =>
                details.Id == id);

        if (entity is null)
        {
            return NotFound();
        }

        var model = new PaymentDetailsEditViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Type = entity.Type,
            CardNumber = entity.CardNumber,
            Iban = entity.Iban,
            RecipientName = entity.RecipientName,
            TaxNumber = entity.TaxNumber,
            BankName = entity.BankName,
            AdditionalInformation =
                entity.AdditionalInformation,
            IsActive = entity.IsActive,
            IsDefault = entity.IsDefault
        };

        return View(model);
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        PaymentDetailsEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        ValidatePaymentDetails(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entity = await _context.PaymentDetails
            .FirstOrDefaultAsync(details =>
                details.Id == id);

        if (entity is null)
        {
            return NotFound();
        }

        if (model.IsDefault)
        {
            await RemoveDefaultFlagAsync(id);
        }

        FillEntity(entity, model);

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Реквізити успішно збережено.";

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // НАЗНАЧИТЬ ОСНОВНЫМИ
    // =========================

    [HttpPost("set-default/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetDefault(int id)
    {
        var entity = await _context.PaymentDetails
            .FirstOrDefaultAsync(details =>
                details.Id == id);

        if (entity is null)
        {
            return NotFound();
        }

        await RemoveDefaultFlagAsync(id);

        entity.IsDefault = true;
        entity.IsActive = true;

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Основні реквізити змінено.";

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // УДАЛЕНИЕ
    // =========================

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.PaymentDetails
            .FirstOrDefaultAsync(details =>
                details.Id == id);

        if (entity is null)
        {
            return NotFound();
        }

        _context.PaymentDetails.Remove(entity);

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Реквізити видалено.";

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // ПРОВЕРКА ПОЛЕЙ
    // =========================

    private void ValidatePaymentDetails(
        PaymentDetailsEditViewModel model)
    {
        if (model.Type == PaymentDetailsType.Card &&
            string.IsNullOrWhiteSpace(model.CardNumber))
        {
            ModelState.AddModelError(
                nameof(model.CardNumber),
                "Вкажіть номер картки.");
        }

        if ((model.Type == PaymentDetailsType.Iban ||
             model.Type == PaymentDetailsType.FopAccount) &&
            string.IsNullOrWhiteSpace(model.Iban))
        {
            ModelState.AddModelError(
                nameof(model.Iban),
                "Вкажіть IBAN.");
        }

        if (model.Type == PaymentDetailsType.Other &&
            string.IsNullOrWhiteSpace(
                model.AdditionalInformation))
        {
            ModelState.AddModelError(
                nameof(model.AdditionalInformation),
                "Вкажіть платіжні реквізити.");
        }

        if (model.IsDefault && !model.IsActive)
        {
            ModelState.AddModelError(
                nameof(model.IsActive),
                "Основні реквізити мають бути активними.");
        }
    }

    // =========================
    // ЗАПОЛНЕНИЕ ENTITY
    // =========================

    private static void FillEntity(
        PaymentDetailsEntity entity,
        PaymentDetailsEditViewModel model)
    {
        entity.Name = model.Name.Trim();
        entity.Type = model.Type;

        entity.CardNumber =
            model.CardNumber?.Trim() ?? string.Empty;

        entity.Iban =
            model.Iban?.Trim() ?? string.Empty;

        entity.RecipientName =
            model.RecipientName?.Trim() ?? string.Empty;

        entity.TaxNumber =
            model.TaxNumber?.Trim() ?? string.Empty;

        entity.BankName =
            model.BankName?.Trim() ?? string.Empty;

        entity.AdditionalInformation =
            model.AdditionalInformation?.Trim()
            ?? string.Empty;

        entity.IsActive = model.IsActive;
        entity.IsDefault = model.IsDefault;
    }

    // =========================
    // СНЯТЬ ФЛАГ С ДРУГИХ
    // =========================

    private async Task RemoveDefaultFlagAsync(
        int? exceptId = null)
    {
        var query = _context.PaymentDetails
            .Where(details => details.IsDefault);

        if (exceptId.HasValue)
        {
            query = query.Where(details =>
                details.Id != exceptId.Value);
        }

        var defaultItems = await query.ToListAsync();

        foreach (var item in defaultItems)
        {
            item.IsDefault = false;
        }
    }
}