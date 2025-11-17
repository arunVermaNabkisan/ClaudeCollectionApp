using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Models.Entities;

/// <summary>
/// Customer interaction history
/// </summary>
public class CustomerInteraction : BaseEntity
{
    [Required]
    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;

    public Guid? CollectionCaseId { get; set; }

    [ForeignKey(nameof(CollectionCaseId))]
    public virtual CollectionCase? CollectionCase { get; set; }

    // Interaction Details
    public CommunicationChannel Channel { get; set; }

    public DispositionCode Disposition { get; set; }

    [MaxLength(100)]
    public string? SubDisposition { get; set; }

    public bool IsSuccessfulContact { get; set; } // RPC - Right Party Contact

    public DateTime InteractionDateTime { get; set; } = DateTime.UtcNow;

    public int DurationSeconds { get; set; } // For calls

    // Contact Details
    [MaxLength(15)]
    public string? ContactNumber { get; set; }

    [MaxLength(100)]
    public string? ContactEmail { get; set; }

    public bool IsCustomerInitiated { get; set; }

    // Content
    [MaxLength(2000)]
    public string? Notes { get; set; }

    [MaxLength(100)]
    public string? TemplateUsed { get; set; }

    [MaxLength(4000)]
    public string? MessageContent { get; set; }

    // Response & Outcome
    [MaxLength(1000)]
    public string? CustomerResponse { get; set; }

    [MaxLength(500)]
    public string? Commitment { get; set; }

    public bool ResultedInPTP { get; set; }

    public Guid? PTPId { get; set; }

    [ForeignKey(nameof(PTPId))]
    public virtual PromiseToPay? PromiseToPay { get; set; }

    public bool ResultedInPayment { get; set; }

    public Guid? PaymentId { get; set; }

    [ForeignKey(nameof(PaymentId))]
    public virtual Payment? Payment { get; set; }

    // Agent Information
    public Guid? AgentId { get; set; }

    [ForeignKey(nameof(AgentId))]
    public virtual ApplicationUser? Agent { get; set; }

    [MaxLength(100)]
    public string? AgentName { get; set; }

    // Call/Recording Details
    [MaxLength(500)]
    public string? RecordingUrl { get; set; }

    [MaxLength(100)]
    public string? CallId { get; set; }

    public bool IsRecorded { get; set; }

    // Sentiment & Quality
    [MaxLength(20)]
    public string? CustomerSentiment { get; set; } // Positive, Neutral, Negative, Angry

    public int? SatisfactionScore { get; set; } // 1-5

    public int? QualityScore { get; set; } // For call quality monitoring

    // Follow-up
    public DateTime? NextFollowUpDate { get; set; }

    [MaxLength(500)]
    public string? NextAction { get; set; }

    // Delivery Status (for SMS, Email, WhatsApp)
    [MaxLength(20)]
    public string? DeliveryStatus { get; set; } // Sent, Delivered, Failed, Read

    public DateTime? DeliveryTimestamp { get; set; }

    public DateTime? ReadTimestamp { get; set; }

    // Campaign Information
    public Guid? CampaignId { get; set; }

    [MaxLength(100)]
    public string? CampaignName { get; set; }

    // System Fields
    [MaxLength(50)]
    public string? SourceSystem { get; set; }

    [MaxLength(100)]
    public string? ExternalReferenceId { get; set; }
}
