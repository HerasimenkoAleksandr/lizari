namespace lizari.Entities;

public class ProductEntity
{
    public int Id { get; set; }

    public int SupplierProductId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool AutoUpdatePrice { get; set; } = true;

    public decimal WholesalePrice { get; set; }

    public decimal? DiscountPercent { get; set; }

    public decimal? DiscountPrice { get; set; }

    public string Feature1Title { get; set; } = string.Empty;
    public string Feature1Description { get; set; } = string.Empty;

    public string Feature2Title { get; set; } = string.Empty;
    public string Feature2Description { get; set; } = string.Empty;

    public string Feature3Title { get; set; } = string.Empty;
    public string Feature3Description { get; set; } = string.Empty;

    public string Feature4Title { get; set; } = string.Empty;
    public string Feature4Description { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Size { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string SelectionTitle { get; set; } = string.Empty;

    public string SelectionDescription { get; set; } = string.Empty;

    public bool IsPublished { get; set; }

    public decimal SupplierPrice { get; set; }

    public bool Available { get; set; }

    public int QuantityInStock { get; set; }

    // Оригинальное название из XML
    public string SupplierName { get; set; } = string.Empty;

    // Оригинальное описание из XML
    public string SupplierDescription { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string Picture { get; set; } = string.Empty;

    public string AdditionalPicture1 { get; set; } = string.Empty;

    public string AdditionalPicture2 { get; set; } = string.Empty;

    public string AdditionalPicture3 { get; set; } = string.Empty;

    public string VendorCode { get; set; } = string.Empty;

    public string Vendor { get; set; } = string.Empty;

    public DateTime? SupplierUpdatedAt { get; set; }
}