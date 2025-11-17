using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Models.Entities;

/// <summary>
/// Payment link generation and tracking
/// </summary>
public class PaymentLink : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string LinkId { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public Guid LoanAccountId { get; set; }

    [ForeignKey(nameof(LoanAccountId))]
    public virtual LoanAccount LoanAccount { get; set; } = null!;

    [Required]
    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;

    public Guid? CollectionCaseId { get; set; }

    [ForeignKey(nameof(CollectionCaseId))]
    public virtual CollectionCase? CollectionCase { get; set; }

    // Link Details
    [Required]
    [MaxLength(500)]
    public string PaymentUrl { get; set; } = string.Empty;

    [MaxLength(200)]
    public string ShortUrl { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public bool AllowPartialPayment { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MinimumAmount { get; set; }

    // Validity
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;

    public DateTime ValidTo { get; set; }

    public bool IsExpired { get; set; }

    public bool IsActive { get; set; } = true;

    // Delivery
    [MaxLength(20)]
    public string DeliveryChannel { get; set; } = string.Empty; // SMS, Email, WhatsApp

    public DateTime? SentDate { get; set; }

    [MaxLength(15)]
    public string? SentToPhone { get; set; }

    [MaxLength(100)]
    public string? SentToEmail { get; set; }

    // Usage Tracking
    public int OpenCount { get; set; }

    public DateTime? FirstOpenedDate { get; set; }

    public DateTime? LastOpenedDate { get; set; }

    public bool IsUsed { get; set; }

    public DateTime? UsedDate { get; set; }

    // Payment Information
    public Guid? PaymentId { get; set; }

    [ForeignKey(nameof(PaymentId))]
    public virtual Payment? Payment { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PaidAmount { get; set; }

    // Analytics
    public int TimeToPaymentMinutes { get; set; }

    public bool AbandonedAtPaymentPage { get; set; }

    [MaxLength(50)]
    public string? PaymentMethodSelected { get; set; }

    // Creator
    public Guid CreatedByUserId { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public virtual ApplicationUser CreatedByUser { get; set; } = null!;

    [MaxLength(500)]
    public string? Notes { get; set; }
}
