using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Models.Entities;

/// <summary>
/// Payment transactions
/// </summary>
public class Payment : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string PaymentReferenceNumber { get; set; } = string.Empty;

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

    // Payment Details
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public PaymentMode PaymentMode { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Initiated;

    public DateTime PaymentDate { get; set; }

    public DateTime? ValueDate { get; set; }

    // Payment Instrument Details
    [MaxLength(100)]
    public string? InstrumentNumber { get; set; } // Cheque no, Transaction ID, etc.

    public DateTime? InstrumentDate { get; set; }

    [MaxLength(100)]
    public string? BankName { get; set; }

    [MaxLength(50)]
    public string? BranchName { get; set; }

    [MaxLength(50)]
    public string? IFSCCode { get; set; }

    // Online Payment Details
    [MaxLength(100)]
    public string? TransactionId { get; set; }

    [MaxLength(100)]
    public string? UTRNumber { get; set; }

    [MaxLength(100)]
    public string? PaymentGatewayTransactionId { get; set; }

    [MaxLength(50)]
    public string? PaymentGateway { get; set; }

    // UPI Details
    [MaxLength(100)]
    public string? UPITransactionId { get; set; }

    [MaxLength(100)]
    public string? VPA { get; set; }

    // Collection Details
    public Guid? CollectedByUserId { get; set; }

    [ForeignKey(nameof(CollectedByUserId))]
    public virtual ApplicationUser? CollectedByUser { get; set; }

    public bool IsFieldCollection { get; set; }

    public Guid? FieldVisitId { get; set; }

    [ForeignKey(nameof(FieldVisitId))]
    public virtual FieldVisit? FieldVisit { get; set; }

    // Payment Link
    public Guid? PaymentLinkId { get; set; }

    [ForeignKey(nameof(PaymentLinkId))]
    public virtual PaymentLink? PaymentLink { get; set; }

    // Allocation
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrincipalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal InterestAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PenaltyAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OtherCharges { get; set; }

    // Reconciliation
    public bool IsReconciled { get; set; }

    public DateTime? ReconciledDate { get; set; }

    public Guid? ReconciledByUserId { get; set; }

    [MaxLength(500)]
    public string? ReconciliationNotes { get; set; }

    // Reversal/Refund
    public bool IsReversed { get; set; }

    public DateTime? ReversalDate { get; set; }

    [MaxLength(500)]
    public string? ReversalReason { get; set; }

    public Guid? ReversalPaymentId { get; set; }

    // Bounce Information
    public bool IsBounced { get; set; }

    public DateTime? BounceDate { get; set; }

    [MaxLength(200)]
    public string? BounceReason { get; set; }

    // Related PTP
    public Guid? PTPId { get; set; }

    [ForeignKey(nameof(PTPId))]
    public virtual PromiseToPay? PromiseToPay { get; set; }

    // LMS Integration
    public bool IsPostedToLMS { get; set; }

    public DateTime? PostedToLMSDate { get; set; }

    [MaxLength(100)]
    public string? LMSTransactionId { get; set; }

    [MaxLength(500)]
    public string? LMSPostingError { get; set; }

    // Notes
    [MaxLength(1000)]
    public string? PaymentNotes { get; set; }

    [MaxLength(500)]
    public string? CustomerRemarks { get; set; }
}
