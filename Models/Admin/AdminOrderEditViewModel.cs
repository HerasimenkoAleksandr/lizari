using System.ComponentModel.DataAnnotations;
using lizari.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace lizari.Models.Admin;

public class AdminOrderEditViewModel
{
    public int Id { get; set; }


    // =========================
    // ДАННЫЕ ЗАКАЗА
    // =========================

    [ValidateNever]
    public OrderEntity Order { get; set; } = null!;


    // =========================
    // СТАТУС
    // =========================

    [Display(Name = "Статус заказа")]
    public OrderStatus Status { get; set; }

    [Display(Name = "Заказ подтверждён покупателем")]
    public bool ConfirmedByCustomer { get; set; }

    [Display(Name = "Заказ отправлен поставщику")]
    public bool SentToSupplier { get; set; }

    [Display(Name = "Заказ принят поставщиком")]
    public bool AcceptedBySupplier { get; set; }


    // =========================
    // РЕКВИЗИТЫ ДЛЯ ОПЛАТЫ
    // =========================

    [Display(Name = "Реквизиты для оплаты")]
    public int? PaymentDetailsId { get; set; }

    [Display(Name = "Спосіб оплати покупця")]
    public PaymentMethod PaymentMethod { get; set; }

    [ValidateNever]
    public IReadOnlyList<SelectListItem>
        PaymentDetailsOptions
    { get; set; } =
            Array.Empty<SelectListItem>();


    // =========================
    // ОПЛАТА ПОСТАВЩИКУ
    // =========================

    [Display(Name = "Поставщик оплачен")]
    public bool SupplierPaid { get; set; }

    [Range(
        0,
        1000000,
        ErrorMessage = "Укажите правильную сумму.")]
    [Display(Name = "Оплачено поставщику")]
    public decimal SupplierPaymentAmount { get; set; }


    // =========================
    // ПРЕДОПЛАТА РИСКА
    // =========================

    [Display(Name = "Требовать предоплату риска")]
    public bool RequiresReturnRiskPayment { get; set; }

    [Display(Name = "Предоплата риска получена")]
    public bool ReturnRiskPaid { get; set; }

    [Range(
        0,
        1000000,
        ErrorMessage = "Укажите правильную сумму.")]
    [Display(Name = "Сумма предоплаты риска")]
    public decimal ReturnRiskPaymentAmount { get; set; }


    // =========================
    // ОТПРАВКА
    // =========================

    [Display(Name = "Товар отправлен поставщиком")]
    public bool ShippedBySupplier { get; set; }

    [StringLength(
        100,
        ErrorMessage = "Номер ТТН слишком длинный.")]
    [Display(Name = "Номер ТТН")]
    public string? TrackingNumber { get; set; }


    // =========================
    // ОПЛАТА ПОКУПАТЕЛЕМ
    // =========================

    [Display(Name = "Товар оплачен покупателем")]
    public bool CustomerPaid { get; set; }

    [Range(
        0,
        1000000,
        ErrorMessage = "Укажите правильную сумму.")]
    [Display(Name = "Оплачено покупателем")]
    public decimal CustomerPaidAmount { get; set; }




    // =========================
    // ПОСТУПЛЕНИЕ ДЕНЕГ
    // =========================

    [Display(Name = "Деньги получены")]
    public bool PaymentReceived { get; set; }

    [Range(
        0,
        1000000,
        ErrorMessage = "Укажите правильную сумму.")]
    [Display(Name = "Получено денег")]
    public decimal ReceivedPaymentAmount { get; set; }


    // =========================
    // ПОЛУЧЕНИЕ И АРХИВ
    // =========================

    [Display(Name = "Товар получен покупателем")]
    public bool ReceivedByCustomer { get; set; }

    [Display(Name = "Перенести заказ в архив")]
    public bool IsArchived { get; set; }
}