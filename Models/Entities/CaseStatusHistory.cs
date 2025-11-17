using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Models.Entities;

public class CaseStatusHistory : BaseEntity
{
    [Required]
    public Guid CollectionCaseId { get; set; }

    [ForeignKey(nameof(CollectionCaseId))]
    public virtual CollectionCase CollectionCase { get; set; } = null!;

    public CaseStatus FromStatus { get; set; }

    public CaseStatus ToStatus { get; set; }

    public DateTime StatusChangeDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Reason { get; set; }

    public Guid ChangedByUserId { get; set; }

    [ForeignKey(nameof(ChangedByUserId))]
    public virtual ApplicationUser ChangedByUser { get; set; } = null!;

    [MaxLength(1000)]
    public string? Notes { get; set; }
}
