using System.ComponentModel.DataAnnotations;

namespace lizari.Models.Admin;

public class ProductEditViewModel
{
    public int Id { get; set; }

    // Данные поставщика только для просмотра
    public int SupplierProductId { get; set; }
    public decimal SupplierPrice { get; set; }
    public bool Available { get; set; }
    public int QuantityInStock { get; set; }
    public string SupplierPicture { get; set; } = string.Empty;
    public string VendorCode { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public DateTime? SupplierUpdatedAt { get; set; }

    // Основная информация
    [Required(ErrorMessage = "Введите название товара.")]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string ShortDescription { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // Цена
    [Range(0, 9999999)]
    public decimal Price { get; set; }

    public bool AutoUpdatePrice { get; set; }

    [Range(0, 9999999)]
    public decimal WholesalePrice { get; set; }

    [Range(0, 9999999)]
    public decimal? DiscountPrice { get; set; }

    // Дополнительные изображения
    [Url(ErrorMessage = "Первая ссылка имеет неправильный формат.")]
    public string? AdditionalPicture1 { get; set; }

    [Url(ErrorMessage = "Вторая ссылка имеет неправильный формат.")]
    public string? AdditionalPicture2 { get; set; }

    [Url(ErrorMessage = "Третья ссылка имеет неправильный формат.")]
    public string? AdditionalPicture3 { get; set; }

    // Преимущества
    public string Feature1Title { get; set; } = string.Empty;
   
    public string Feature1Description { get; set; } = string.Empty;

    public string Feature2Title { get; set; } = string.Empty;
    public string Feature2Description { get; set; } = string.Empty;

    public string Feature3Title { get; set; } = string.Empty;
    public string Feature3Description { get; set; } = string.Empty;

    public string Feature4Title { get; set; } = string.Empty;
    public string Feature4Description { get; set; } = string.Empty;

    // Характеристики
    public string Type { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    // Блок подбора
    public string SelectionTitle { get; set; } = string.Empty;
    public string SelectionDescription { get; set; } = string.Empty;

    public bool IsPublished { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public string SupplierDescription { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public bool IsMainProduct { get; set; }
}