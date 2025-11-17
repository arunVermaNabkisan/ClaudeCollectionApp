using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Models.Entities;

/// <summary>
/// Loan account details
/// </summary>
public class LoanAccount : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string LoanAccountNumber { get; set; } = string.Empty;

    [Required]
    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;

    public LoanProduct ProductType { get; set; }

    [MaxLength(100)]
    public string? ProductName { get; set; }

    public bool IsSecured { get; set; }

    // Loan Amounts
    [Column(TypeName = "decimal(18,2)")]
    public decimal SanctionedAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DisbursedAmount { get; set; }

    public DateTime? DisbursementDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentOutstandingPrincipal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentOutstandingInterest { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PenaltyCharges { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OtherCharges { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalOutstanding { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OverdueAmount { get; set; }

    // EMI Information
    [Column(TypeName = "decimal(18,2)")]
    public decimal EMIAmount { get; set; }

    [MaxLength(20)]
    public string? EMIFrequency { get; set; } // Monthly, Quarterly, etc.

    public int TotalEMIs { get; set; }

    public int EMIsPaid { get; set; }

    public int EMIsOverdue { get; set; }

    public DateTime? NextEMIDueDate { get; set; }

    // Interest Details
    [Column(TypeName = "decimal(5,2)")]
    public decimal InterestRate { get; set; }

    [MaxLength(20)]
    public string? InterestType { get; set; } // Fixed, Floating

    // Tenure
    public int TenureMonths { get; set; }

    public DateTime? MaturityDate { get; set; }

    // Payment Information
    public DateTime? LastPaymentDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LastPaymentAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmountPaid { get; set; }

    // Delinquency Information
    public int DaysPastDue { get; set; }

    public DelinquencyBucket CurrentBucket { get; set; } = DelinquencyBucket.Current;

    public DateTime? FirstDelinquencyDate { get; set; }

    public int ContinuousDelinquencyDays { get; set; }

    public int NumberOfBounces { get; set; }

    // Status
    [MaxLength(20)]
    public string LoanStatus { get; set; } = "Active"; // Active, Closed, WrittenOff, NPA

    public bool IsNPA { get; set; }

    public DateTime? NPADate { get; set; }

    // Assignment
    public Guid? AssignedRMId { get; set; }

    [ForeignKey(nameof(AssignedRMId))]
    public virtual ApplicationUser? AssignedRM { get; set; }

    [MaxLength(100)]
    public string? VerticalName { get; set; }

    [MaxLength(100)]
    public string? BranchName { get; set; }

    [MaxLength(100)]
    public string? RegionName { get; set; }

    // LMS Integration
    [MaxLength(50)]
    public string? LMSLoanId { get; set; }

    public DateTime? LastSyncedFromLMS { get; set; }

    // Navigation Properties
    public virtual CollectionCase? CollectionCase { get; set; }
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<LoanTransaction> Transactions { get; set; } = new List<LoanTransaction>();
}
