using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Models.Entities;

/// <summary>
/// Promise to Pay (PTP) records
/// </summary>
public class PromiseToPay : BaseEntity
{
    [Required]
    public Guid CollectionCaseId { get; set; }

    [ForeignKey(nameof(CollectionCaseId))]
    public virtual CollectionCase CollectionCase { get; set; } = null!;

    [Required]
    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;

    // PTP Details
    [Required]
    [MaxLength(50)]
    public string PTPNumber { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PromisedAmount { get; set; }

    [Required]
    public DateTime PromisedDate { get; set; }

    public PaymentMode IntendedPaymentMode { get; set; }

    public PTPStatus Status { get; set; } = PTPStatus.Active;

    // For Split PTPs
    public bool IsSplitPTP { get; set; }

    public Guid? ParentPTPId { get; set; }

    [ForeignKey(nameof(ParentPTPId))]
    public virtual PromiseToPay? ParentPTP { get; set; }

    public int SplitSequence { get; set; }

    public int TotalSplits { get; set; }

    // Confidence & Quality
    public int ConfidenceLevel { get; set; } // 1-5, 5 being highest confidence

    [MaxLength(100)]
    public string? CustomerSentiment { get; set; }

    [MaxLength(500)]
    public string? ReasonForDelay { get; set; }

    // Fulfillment
    public DateTime? ActualPaymentDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ActualPaymentAmount { get; set; }

    public Guid? PaymentId { get; set; }

    [ForeignKey(nameof(PaymentId))]
    public virtual Payment? Payment { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountShortfall { get; set; }

    // Tracking
    public DateTime? FollowUpDate { get; set; }

    public bool ReminderSent { get; set; }

    public DateTime? ReminderSentDate { get; set; }

    public int NumberOfReminders { get; set; }

    // Agent Information
    public Guid CreatedByUserId { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public virtual ApplicationUser CreatedByUser { get; set; } = null!;

    [MaxLength(1000)]
    public string? AgentNotes { get; set; }

    // Validity
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;

    public DateTime ValidTo { get; set; }

    public bool IsExpired { get; set; }

    public bool IsCancelled { get; set; }

    public DateTime? CancelledDate { get; set; }

    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    // Navigation Properties
    public virtual ICollection<PromiseToPay> SplitPTPs { get; set; } = new List<PromiseToPay>();
    public virtual ICollection<PTPFollowUp> FollowUps { get; set; } = new List<PTPFollowUp>();
}
