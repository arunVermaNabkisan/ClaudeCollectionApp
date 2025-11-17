using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Models.Entities;

/// <summary>
/// Document management for collection activities
/// </summary>
public class Document : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string DocumentNumber { get; set; } = string.Empty;

    public DocumentType DocumentType { get; set; }

    [MaxLength(200)]
    public string DocumentName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    // Related Entities
    public Guid? CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    public Guid? LoanAccountId { get; set; }

    [ForeignKey(nameof(LoanAccountId))]
    public virtual LoanAccount? LoanAccount { get; set; }

    public Guid? CollectionCaseId { get; set; }

    [ForeignKey(nameof(CollectionCaseId))]
    public virtual CollectionCase? CollectionCase { get; set; }

    public Guid? FieldVisitId { get; set; }

    [ForeignKey(nameof(FieldVisitId))]
    public virtual FieldVisit? FieldVisit { get; set; }

    // File Information
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? FileUrl { get; set; }

    [MaxLength(100)]
    public string FileName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string FileExtension { get; set; } = string.Empty;

    [MaxLength(100)]
    public string MimeType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    // Metadata
    public DateTime DocumentDate { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? DocumentSource { get; set; } // Upload, Scan, Email, Field Visit

    public bool IsEncrypted { get; set; }

    [MaxLength(100)]
    public string? ChecksumHash { get; set; }

    // Geo-tagging (for field documents)
    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime? CaptureTimestamp { get; set; }

    // OCR & Processing
    public bool IsOCRProcessed { get; set; }

    [MaxLength(4000)]
    public string? ExtractedText { get; set; }

    public decimal? OCRConfidence { get; set; }

    // Access Control
    public bool IsConfidential { get; set; }

    [MaxLength(500)]
    public string? AccessRestrictedTo { get; set; } // Role or User IDs

    // Versioning
    public int Version { get; set; } = 1;

    public Guid? ParentDocumentId { get; set; }

    [ForeignKey(nameof(ParentDocumentId))]
    public virtual Document? ParentDocument { get; set; }

    // Upload Information
    public Guid UploadedByUserId { get; set; }

    [ForeignKey(nameof(UploadedByUserId))]
    public virtual ApplicationUser UploadedByUser { get; set; } = null!;

    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

    // Verification
    public bool IsVerified { get; set; }

    public Guid? VerifiedByUserId { get; set; }

    public DateTime? VerifiedDate { get; set; }

    [MaxLength(500)]
    public string? VerificationNotes { get; set; }

    // Retention
    public DateTime? ExpiryDate { get; set; }

    public bool IsArchived { get; set; }

    public DateTime? ArchivedDate { get; set; }

    // Tags & Categories
    [MaxLength(500)]
    public string? Tags { get; set; } // Comma-separated

    [MaxLength(100)]
    public string? Category { get; set; }

    // Navigation Properties
    public virtual ICollection<Document> ChildDocuments { get; set; } = new List<Document>();
}
