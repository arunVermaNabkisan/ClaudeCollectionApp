using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClaudeCollectionApp.Models.Entities;

public class LoanTransaction : BaseEntity
{
    [Required]
    public Guid LoanAccountId { get; set; }

    [ForeignKey(nameof(LoanAccountId))]
    public virtual LoanAccount LoanAccount { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string TransactionNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string TransactionType { get; set; } = string.Empty; // Disbursement, Payment, Charge, Refund, Adjustment

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public DateTime ValueDate { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAfter { get; set; }

    public bool IsReversed { get; set; }

    public Guid? ReversalTransactionId { get; set; }

    [MaxLength(50)]
    public string? LMSTransactionId { get; set; }

    public DateTime? SyncedFromLMSDate { get; set; }
}
