using System.ComponentModel.DataAnnotations;

namespace lizari.Entities;

public class PaymentDetailsEntity
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public PaymentDetailsType Type { get; set; }

    [StringLength(30)]
    public string CardNumber { get; set; } = string.Empty;

    [StringLength(34)]
    public string Iban { get; set; } = string.Empty;

    [StringLength(200)]
    public string RecipientName { get; set; } = string.Empty;

    [StringLength(20)]
    public string TaxNumber { get; set; } = string.Empty;

    [StringLength(200)]
    public string BankName { get; set; } = string.Empty;

    [StringLength(500)]
    public string AdditionalInformation { get; set; } =
        string.Empty;

    public bool IsActive { get; set; } = true;

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; } =
        DateTime.UtcNow;
}