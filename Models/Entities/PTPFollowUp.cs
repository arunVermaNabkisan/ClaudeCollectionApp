using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClaudeCollectionApp.Models.Entities;

public class PTPFollowUp : BaseEntity
{
    [Required]
    public Guid PTPId { get; set; }

    [ForeignKey(nameof(PTPId))]
    public virtual PromiseToPay PromiseToPay { get; set; } = null!;

    public DateTime FollowUpDate { get; set; }

    [MaxLength(50)]
    public string FollowUpType { get; set; } = string.Empty; // Reminder, Check, Escalation

    [MaxLength(50)]
    public string Channel { get; set; } = string.Empty; // Call, SMS, Email, WhatsApp

    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CompletedDate { get; set; }

    public Guid? CompletedByUserId { get; set; }

    [ForeignKey(nameof(CompletedByUserId))]
    public virtual ApplicationUser? CompletedByUser { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [MaxLength(500)]
    public string? CustomerResponse { get; set; }
}
