using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Models.Entities;

/// <summary>
/// Collection case entity - core of the collection management system
/// </summary>
public class CollectionCase : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string CaseNumber { get; set; } = string.Empty;

    [Required]
    public Guid LoanAccountId { get; set; }

    [ForeignKey(nameof(LoanAccountId))]
    public virtual LoanAccount LoanAccount { get; set; } = null!;

    [Required]
    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;

    // Case Status
    public CaseStatus Status { get; set; } = CaseStatus.NewCase;

    [MaxLength(50)]
    public string? SubStatus { get; set; }

    // Delinquency Information
    public DelinquencyBucket CurrentBucket { get; set; }

    public DelinquencyBucket? PreviousBucket { get; set; }

    public int CurrentDPD { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalOutstanding { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OverdueAmount { get; set; }

    public DateTime? FirstDelinquencyDate { get; set; }

    public DateTime? LastBucketChangeDate { get; set; }

    // Assignment Information
    public Guid? AssignedToUserId { get; set; }

    [ForeignKey(nameof(AssignedToUserId))]
    public virtual ApplicationUser? AssignedToUser { get; set; }

    public DateTime? AssignedDate { get; set; }

    public Guid? PreviousAssignedUserId { get; set; }

    [MaxLength(50)]
    public string? AllocationMethod { get; set; } // Auto, Manual, RoundRobin, etc.

    // Strategy & Priority
    [MaxLength(100)]
    public string? CollectionStrategy { get; set; }

    public int Priority { get; set; } // 1-5, 1 being highest

    public decimal ProbabilityOfPayment { get; set; }

    public int BehavioralScore { get; set; }

    [MaxLength(20)]
    public string? RiskSegment { get; set; }

    // Contact Information
    public int TotalContactAttempts { get; set; }

    public int SuccessfulContacts { get; set; }

    public DateTime? LastContactDate { get; set; }

    public DateTime? LastSuccessfulContactDate { get; set; }

    [MaxLength(50)]
    public string? BestTimeToContact { get; set; }

    [MaxLength(20)]
    public string? PreferredChannel { get; set; }

    // PTP Information
    public int TotalPTPs { get; set; }

    public int PTPsKept { get; set; }

    public int PTPsBroken { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal PTPKeepRatio { get; set; }

    public DateTime? LastPTPDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LastPTPAmount { get; set; }

    public DateTime? NextPTPDueDate { get; set; }

    // Payment Information
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalCollected { get; set; }

    public int NumberOfPayments { get; set; }

    public DateTime? LastPaymentDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LastPaymentAmount { get; set; }

    // Field Collection
    public int TotalFieldVisits { get; set; }

    public DateTime? LastFieldVisitDate { get; set; }

    public bool IsAddressVerified { get; set; }

    public DateTime? AddressVerificationDate { get; set; }

    // Legal & Escalation
    public bool IsLegalAction { get; set; }

    public DateTime? LegalActionDate { get; set; }

    [MaxLength(100)]
    public string? LegalActionType { get; set; }

    public bool IsWrittenOff { get; set; }

    public DateTime? WrittenOffDate { get; set; }

    // Resolution
    public DateTime? ResolvedDate { get; set; }

    [MaxLength(50)]
    public string? ResolutionType { get; set; } // Full Recovery, Settlement, Write-off

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SettlementAmount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? SettlementDiscountPercentage { get; set; }

    // Notes & Remarks
    [MaxLength(2000)]
    public string? CurrentNotes { get; set; }

    [MaxLength(500)]
    public string? SpecialInstructions { get; set; }

    public bool HasDispute { get; set; }

    [MaxLength(1000)]
    public string? DisputeDetails { get; set; }

    // Dates
    public DateTime CaseOpenDate { get; set; } = DateTime.UtcNow;

    public DateTime? CaseClosedDate { get; set; }

    public int CaseAgeInDays { get; set; }

    // Navigation Properties
    public virtual ICollection<PromiseToPay> PromisesToPay { get; set; } = new List<PromiseToPay>();
    public virtual ICollection<CustomerInteraction> Interactions { get; set; } = new List<CustomerInteraction>();
    public virtual ICollection<FieldVisit> FieldVisits { get; set; } = new List<FieldVisit>();
    public virtual ICollection<CaseAssignmentHistory> AssignmentHistory { get; set; } = new List<CaseAssignmentHistory>();
    public virtual ICollection<CaseStatusHistory> StatusHistory { get; set; } = new List<CaseStatusHistory>();
    public virtual ICollection<CaseNote> Notes { get; set; } = new List<CaseNote>();
}
