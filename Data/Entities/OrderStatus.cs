namespace lizari.Entities;

public enum OrderStatus
{
    New = 1,
    ConfirmedByCustomer = 2,
    SentToSupplier = 3,
    AcceptedBySupplier = 4,
    ShippedBySupplier = 5,
    ReceivedByCustomer = 6,
    Cancelled = 7,
    Returned = 8
}