using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClaudeCollectionApp.Models.Entities;

public class CaseNote : BaseEntity
{
    [Required]
    public Guid CollectionCaseId { get; set; }

    [ForeignKey(nameof(CollectionCaseId))]
    public virtual CollectionCase CollectionCase { get; set; } = null!;

    [Required]
    [MaxLength(4000)]
    public string NoteText { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? NoteType { get; set; } // General, Important, Follow-up, Alert

    public bool IsPinned { get; set; }

    public bool IsAlert { get; set; }

    public Guid CreatedByUserId { get; set; }

    [ForeignKey(nameof(CreatedByUserId))]
    public virtual ApplicationUser CreatedByUser { get; set; } = null!;

    public DateTime NoteDate { get; set; } = DateTime.UtcNow;
}
