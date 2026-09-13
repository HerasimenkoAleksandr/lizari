namespace lizari.Entities;

public class OrderItemEntity
{
    public int Id { get; set; }

    // Заказ
    public int OrderId { get; set; }

    public OrderEntity Order { get; set; } = null!;


    // =========================
    // ДАННЫЕ ТОВАРА
    // НА МОМЕНТ ЗАКАЗА
    // =========================

    // Внутренний ID товара LIZARI
    public int ProductId { get; set; }

    // ID товара в XML поставщика
    public int SupplierProductId { get; set; }

    // Артикул поставщика
    public string VendorCode { get; set; } = string.Empty;

    // Название на сайте LIZARI
    public string ProductTitle { get; set; } = string.Empty;

    // Название товара у поставщика
    public string SupplierProductName { get; set; } = string.Empty;


    // =========================
    // ЦЕНА И КОЛИЧЕСТВО
    // =========================

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }
}