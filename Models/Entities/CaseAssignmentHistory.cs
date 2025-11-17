using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClaudeCollectionApp.Models.Entities;

public class CaseAssignmentHistory : BaseEntity
{
    [Required]
    public Guid CollectionCaseId { get; set; }

    [ForeignKey(nameof(CollectionCaseId))]
    public virtual CollectionCase CollectionCase { get; set; } = null!;

    public Guid? FromUserId { get; set; }

    [ForeignKey(nameof(FromUserId))]
    public virtual ApplicationUser? FromUser { get; set; }

    [Required]
    public Guid ToUserId { get; set; }

    [ForeignKey(nameof(ToUserId))]
    public virtual ApplicationUser ToUser { get; set; } = null!;

    public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string AssignmentType { get; set; } = string.Empty; // Initial, Reassignment, Escalation

    [MaxLength(500)]
    public string? Reason { get; set; }

    public Guid? AssignedByUserId { get; set; }

    [ForeignKey(nameof(AssignedByUserId))]
    public virtual ApplicationUser? AssignedByUser { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
