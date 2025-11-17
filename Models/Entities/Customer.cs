using System.ComponentModel.DataAnnotations;

namespace ClaudeCollectionApp.Models.Entities;

/// <summary>
/// Customer master data
/// </summary>
public class Customer : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MiddleName { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public int? Age { get; set; }

    [MaxLength(10)]
    public string? Gender { get; set; }

    [MaxLength(100)]
    public string? Occupation { get; set; }

    [MaxLength(100)]
    public string? EmploymentType { get; set; }

    [MaxLength(200)]
    public string? EmployerName { get; set; }

    public decimal? MonthlyIncome { get; set; }

    // Contact Information
    [Required]
    [MaxLength(15)]
    public string PrimaryPhone { get; set; } = string.Empty;

    public bool PrimaryPhoneVerified { get; set; }

    [MaxLength(15)]
    public string? AlternatePhone1 { get; set; }

    [MaxLength(15)]
    public string? AlternatePhone2 { get; set; }

    [MaxLength(15)]
    public string? WhatsAppNumber { get; set; }

    [EmailAddress]
    [MaxLength(100)]
    public string? Email { get; set; }

    public bool EmailVerified { get; set; }

    // Address Information
    [MaxLength(500)]
    public string? CurrentAddress { get; set; }

    [MaxLength(100)]
    public string? CurrentCity { get; set; }

    [MaxLength(100)]
    public string? CurrentState { get; set; }

    [MaxLength(10)]
    public string? CurrentPincode { get; set; }

    public decimal? CurrentAddressLatitude { get; set; }

    public decimal? CurrentAddressLongitude { get; set; }

    [MaxLength(500)]
    public string? PermanentAddress { get; set; }

    [MaxLength(100)]
    public string? PermanentCity { get; set; }

    [MaxLength(100)]
    public string? PermanentState { get; set; }

    [MaxLength(10)]
    public string? PermanentPincode { get; set; }

    // KYC Information
    [MaxLength(50)]
    public string? PANNumber { get; set; }

    [MaxLength(50)]
    public string? AadharNumber { get; set; }

    [MaxLength(50)]
    public string? VoterIdNumber { get; set; }

    [MaxLength(50)]
    public string? DrivingLicenseNumber { get; set; }

    // Preferences
    [MaxLength(10)]
    public string? PreferredLanguage { get; set; }

    [MaxLength(50)]
    public string? PreferredContactTime { get; set; }

    [MaxLength(20)]
    public string? PreferredContactChannel { get; set; }

    // Risk & Behavior
    public int BehavioralScore { get; set; }

    public decimal ProbabilityOfPayment { get; set; }

    [MaxLength(20)]
    public string? RiskCategory { get; set; }

    // LMS Integration
    [MaxLength(50)]
    public string? LMSCustomerId { get; set; }

    public DateTime? LastSyncedFromLMS { get; set; }

    // Navigation Properties
    public virtual ICollection<LoanAccount> LoanAccounts { get; set; } = new List<LoanAccount>();
    public virtual ICollection<CustomerInteraction> Interactions { get; set; } = new List<CustomerInteraction>();
    public virtual ICollection<AlternateContact> AlternateContacts { get; set; } = new List<AlternateContact>();
}
