using System.ComponentModel.DataAnnotations;
using lizari.Entities;

namespace lizari.Models.Admin;

public class PaymentDetailsEditViewModel
{
    public int Id { get; set; }


    // =========================
    // ОСНОВНАЯ ИНФОРМАЦИЯ
    // =========================

    [Required(
        ErrorMessage = "Вкажіть назву реквізитів.")]
    [StringLength(
        100,
        ErrorMessage = "Назва занадто довга.")]
    [Display(Name = "Назва")]
    public string Name { get; set; } =
        string.Empty;


    [Required(
        ErrorMessage = "Оберіть тип реквізитів.")]
    [Display(Name = "Тип реквізитів")]
    public PaymentDetailsType Type { get; set; } =
        PaymentDetailsType.Card;


    // =========================
    // БАНКОВСКАЯ КАРТА
    // =========================

    [StringLength(
        30,
        ErrorMessage = "Номер картки занадто довгий.")]
    [Display(Name = "Номер картки")]
    public string? CardNumber { get; set; }


    // =========================
    // БАНКОВСКИЙ СЧЁТ
    // =========================

    [StringLength(
        34,
        ErrorMessage = "IBAN занадто довгий.")]
    [Display(Name = "IBAN")]
    public string? Iban { get; set; }


    // =========================
    // ПОЛУЧАТЕЛЬ
    // =========================

    [StringLength(
        200,
        ErrorMessage = "Ім’я отримувача занадто довге.")]
    [Display(Name = "Отримувач")]
    public string? RecipientName { get; set; }


    [StringLength(
        20,
        ErrorMessage = "ІПН або ЄДРПОУ занадто довгий.")]
    [Display(Name = "ІПН / ЄДРПОУ")]
    public string? TaxNumber { get; set; }


    [StringLength(
        200,
        ErrorMessage = "Назва банку занадто довга.")]
    [Display(Name = "Назва банку")]
    public string? BankName { get; set; }


    // =========================
    // ДОПОЛНИТЕЛЬНАЯ ИНФОРМАЦИЯ
    // =========================

    [StringLength(
        500,
        ErrorMessage =
            "Додаткова інформація занадто довга.")]
    [Display(Name = "Додаткова інформація")]
    public string? AdditionalInformation { get; set; }


    // =========================
    // НАСТРОЙКИ
    // =========================

    [Display(Name = "Активні реквізити")]
    public bool IsActive { get; set; } = true;


    [Display(
        Name = "Використовувати за замовчуванням")]
    public bool IsDefault { get; set; }
}