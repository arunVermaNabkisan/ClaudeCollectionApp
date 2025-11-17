using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClaudeCollectionApp.Models.Entities;

public class FieldVisitPhoto : BaseEntity
{
    [Required]
    public Guid FieldVisitId { get; set; }

    [ForeignKey(nameof(FieldVisitId))]
    public virtual FieldVisit FieldVisit { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? FileUrl { get; set; }

    [MaxLength(100)]
    public string FileName { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    [MaxLength(50)]
    public string PhotoType { get; set; } = string.Empty; // Residence, Business, Asset, Document, Person

    [MaxLength(500)]
    public string? Caption { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime CaptureTimestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? DeviceInfo { get; set; }

    public bool IsVerified { get; set; }

    [MaxLength(100)]
    public string? ChecksumHash { get; set; }
}
