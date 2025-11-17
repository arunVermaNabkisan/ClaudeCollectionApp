using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClaudeCollectionApp.Models.Entities;

public class AlternateContact : BaseEntity
{
    [Required]
    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Relationship { get; set; } // Spouse, Parent, Sibling, Friend, Employer

    [MaxLength(15)]
    public string? PhoneNumber { get; set; }

    [MaxLength(15)]
    public string? AlternatePhoneNumber { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerifiedDate { get; set; }

    public bool IsPreferredContact { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public int ContactAttempts { get; set; }

    public int SuccessfulContacts { get; set; }

    public DateTime? LastContactDate { get; set; }
}
