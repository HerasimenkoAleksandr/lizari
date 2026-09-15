
using System.ComponentModel.DataAnnotations.Schema;

namespace lizari.Entities;

public class OrderEntity
{
    public int Id { get; set; }

    // Номер, который показываем клиенту
    public string OrderNumber { get; set; } = string.Empty;

    public int? PaymentDetailsId { get; set; }

    public PaymentDetailsEntity? PaymentDetails { get; set; }

    public string PaymentDetailsSnapshot { get; set; } =
        string.Empty;

    public bool RequiresReturnRiskPayment { get; set; }


    // =========================
    // ПОЛУЧАТЕЛЬ
    // =========================

    public string RecipientName { get; set; } = string.Empty;

    public string RecipientPhone { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    // Отделение, почтомат или адрес
    public string DeliveryAddress { get; set; } = string.Empty;

    public string DeliveryMethod { get; set; } =
        "Нова Пошта";

    public string Comment { get; set; } = string.Empty;

    // Идентификатор города в Новой Почте
    public string CityRef { get; set; } = string.Empty;

    // Идентификатор отделения или почтомата
    public string WarehouseRef { get; set; } = string.Empty;


    // =========================
    // СТАТУС
    // =========================

    public OrderStatus Status { get; set; } =
        OrderStatus.New;

    // Когда покупатель подтвердил заказ
    public DateTime? ConfirmedByCustomerAt { get; set; }

    // =========================
    // СТОИМОСТЬ И ОПЛАТА
    // =========================

    public decimal TotalPrice { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    // Предоплата риска при наложенном платеже
    public decimal ReturnRiskPaymentAmount { get; set; }

    public DateTime? ReturnRiskPaidAt { get; set; }

    // Сколько покупатель оплатил
    public decimal CustomerPaidAmount { get; set; }

    public DateTime? CustomerPaidAt { get; set; }

    // Сколько денег фактически поступило нам
    public decimal ReceivedPaymentAmount { get; set; }

    public DateTime? PaymentReceivedAt { get; set; }


    // =========================
    // ОПЛАТА ПОСТАВЩИКУ
    // =========================

    public decimal SupplierPaymentAmount { get; set; }

    public DateTime? SupplierPaidAt { get; set; }


    // =========================
    // ПОСТАВЩИК И ДОСТАВКА
    // =========================

    public DateTime? SentToSupplierAt { get; set; }

    public DateTime? AcceptedBySupplierAt { get; set; }

    public DateTime? ShippedBySupplierAt { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;

    public DateTime? ReceivedByCustomerAt { get; set; }


    // =========================
    // АРХИВ
    // =========================

    public bool IsArchived { get; set; }

    public DateTime? ArchivedAt { get; set; }


    // =========================
    // ДАТЫ
    // =========================

    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } =
        DateTime.UtcNow;


    // =========================
    // ПОЗИЦИИ ЗАКАЗА
    // =========================

    public List<OrderItemEntity> Items { get; set; } = new();


    // =========================
    // РАСЧЁТ НАЛОЖЕННОГО ПЛАТЕЖА
    // =========================

    [NotMapped]

    public decimal CashOnDeliveryAmount
    {
        get
        {
            if (PaymentMethod !=
                lizari.Entities.PaymentMethod.CashOnDelivery)
            {
                return 0;
            }

            return Math.Max(
                0,
                TotalPrice - ReturnRiskPaymentAmount);
        }
    }
}