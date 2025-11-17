using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Models.Entities;

/// <summary>
/// Field visit records
/// </summary>
public class FieldVisit : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string VisitNumber { get; set; } = string.Empty;

    [Required]
    public Guid CollectionCaseId { get; set; }

    [ForeignKey(nameof(CollectionCaseId))]
    public virtual CollectionCase CollectionCase { get; set; } = null!;

    [Required]
    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;

    // Visit Planning
    public DateTime PlannedDate { get; set; }

    [MaxLength(500)]
    public string PlannedAddress { get; set; } = string.Empty;

    public decimal? PlannedLatitude { get; set; }

    public decimal? PlannedLongitude { get; set; }

    public FieldVisitStatus Status { get; set; } = FieldVisitStatus.Planned;

    // Assignment
    [Required]
    public Guid AssignedToUserId { get; set; }

    [ForeignKey(nameof(AssignedToUserId))]
    public virtual ApplicationUser AssignedToUser { get; set; } = null!;

    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

    // Visit Execution
    public DateTime? ActualStartTime { get; set; }

    public DateTime? ActualEndTime { get; set; }

    public int DurationMinutes { get; set; }

    // Check-in Details
    public DateTime? CheckInTime { get; set; }

    public decimal? CheckInLatitude { get; set; }

    public decimal? CheckInLongitude { get; set; }

    public bool IsGeoFenceValid { get; set; }

    public decimal? DistanceFromPlannedLocation { get; set; } // in meters

    // Visit Outcome
    public DispositionCode Disposition { get; set; }

    [MaxLength(100)]
    public string? SubDisposition { get; set; }

    public bool IsCustomerMet { get; set; }

    [MaxLength(100)]
    public string? PersonMet { get; set; } // Customer, Spouse, Relative, etc.

    [MaxLength(100)]
    public string? RelationshipWithCustomer { get; set; }

    // Customer Interaction
    [MaxLength(2000)]
    public string? DiscussionNotes { get; set; }

    [MaxLength(500)]
    public string? CustomerResponse { get; set; }

    [MaxLength(1000)]
    public string? ReasonForNonPayment { get; set; }

    // Commitment
    public bool ResultedInPTP { get; set; }

    public Guid? PTPId { get; set; }

    [ForeignKey(nameof(PTPId))]
    public virtual PromiseToPay? PromiseToPay { get; set; }

    public bool ResultedInPayment { get; set; }

    public Guid? PaymentId { get; set; }

    [ForeignKey(nameof(PaymentId))]
    public virtual Payment? Payment { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AmountCollected { get; set; }

    // Address Verification
    public bool IsAddressVerified { get; set; }

    public bool IsAddressCorrect { get; set; }

    [MaxLength(500)]
    public string? CorrectedAddress { get; set; }

    public decimal? CorrectedLatitude { get; set; }

    public decimal? CorrectedLongitude { get; set; }

    [MaxLength(500)]
    public string? AddressRemarks { get; set; }

    // Asset Verification (for secured loans)
    public bool IsAssetVerified { get; set; }

    [MaxLength(500)]
    public string? AssetCondition { get; set; }

    [MaxLength(500)]
    public string? AssetLocation { get; set; }

    public bool IsAssetAtRisk { get; set; }

    // Evidence Collection
    public int NumberOfPhotos { get; set; }

    public int NumberOfDocuments { get; set; }

    public bool HasVoiceNote { get; set; }

    // Next Steps
    public DateTime? NextVisitDate { get; set; }

    [MaxLength(500)]
    public string? NextAction { get; set; }

    public bool RequiresEscalation { get; set; }

    [MaxLength(500)]
    public string? EscalationReason { get; set; }

    // Route Information
    public int RouteSequence { get; set; }

    [MaxLength(100)]
    public string? RouteName { get; set; }

    // Cancellation/Rescheduling
    public DateTime? CancelledDate { get; set; }

    [MaxLength(500)]
    public string? CancellationReason { get; set; }

    public DateTime? RescheduledTo { get; set; }

    [MaxLength(500)]
    public string? RescheduleReason { get; set; }

    // Navigation Properties
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    public virtual ICollection<FieldVisitPhoto> Photos { get; set; } = new List<FieldVisitPhoto>();
}
