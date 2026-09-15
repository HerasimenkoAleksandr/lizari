
using lizari.Entities;
using System.ComponentModel.DataAnnotations;

namespace lizari.Models.OrderModels;

public class OrderCreateViewModel
{
    // =========================
    // ТОВАР
    // =========================

    public int ProductId { get; set; }

    public string ProductTitle { get; set; } = string.Empty;

    public string ProductPicture { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }


    // =========================
    // КОЛИЧЕСТВО
    // =========================

    [Required(
        ErrorMessage = "Вкажіть кількість товару.")]
    [Range(
        1,
        10,
        ErrorMessage = "Кількість повинна бути від 1 до 10.")]
    public int Quantity { get; set; } = 1;


    // =========================
    // ПОЛУЧАТЕЛЬ
    // =========================

    [Required(
        ErrorMessage = "Вкажіть ім’я отримувача.")]
    [StringLength(
        150,
        ErrorMessage = "Ім’я занадто довге.")]
    [Display(Name = "Ім’я та прізвище")]
    public string RecipientName { get; set; } = string.Empty;


    [Required(
        ErrorMessage = "Вкажіть номер телефону.")]
    [StringLength(
        30,
        MinimumLength = 10,
        ErrorMessage = "Перевірте номер телефону.")]
    [RegularExpression(
        @"^[0-9+\s()\-]+$",
        ErrorMessage = "Вкажіть правильний номер телефону.")]
    [Display(Name = "Телефон")]
    public string RecipientPhone { get; set; } = string.Empty;


    // =========================
    // ДОСТАВКА
    // =========================

    [Required(
        ErrorMessage = "Вкажіть місто.")]
    [StringLength(
        150,
        ErrorMessage = "Назва міста занадто довга.")]
    [Display(Name = "Місто")]
    public string City { get; set; } = string.Empty;


    // Идентификатор города Новой Почты
    [Required(
        ErrorMessage = "Оберіть місто зі списку.")]
    public string CityRef { get; set; } = string.Empty;


    [Required(
        ErrorMessage =
            "Вкажіть відділення або поштомат.")]
    [StringLength(
        250,
        ErrorMessage = "Адреса занадто довга.")]
    [Display(
        Name = "Відділення або поштомат Нової Пошти")]
    public string DeliveryAddress { get; set; } = string.Empty;


    // Идентификатор отделения или почтомата
    [Required(
        ErrorMessage = "Оберіть відділення зі списку.")]
    public string WarehouseRef { get; set; } = string.Empty;


    // =========================
    // ОПЛАТА
    // =========================

    [Required(
        ErrorMessage = "Оберіть спосіб оплати.")]
    [Display(Name = "Спосіб оплати")]
    public PaymentMethod PaymentMethod { get; set; } =
    lizari.Entities.PaymentMethod.CashOnDelivery;


    // =========================
    // КОММЕНТАРИЙ
    // =========================

    [StringLength(
        1000,
        ErrorMessage = "Коментар занадто довгий.")]
    [Display(Name = "Коментар до замовлення")]
    public string Comment { get; set; } = string.Empty;
}