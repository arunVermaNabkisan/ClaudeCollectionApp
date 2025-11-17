using System.ComponentModel.DataAnnotations;

namespace ClaudeCollectionApp.Models.Entities;

/// <summary>
/// External recovery agency management
/// </summary>
public class RecoveryAgency : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string AgencyCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string AgencyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string LegalEntityName { get; set; } = string.Empty;

    // Contact Information
    [Required]
    [MaxLength(15)]
    public string PrimaryContactNumber { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? AlternateContactNumber { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Website { get; set; }

    // Address
    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string State { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Pincode { get; set; } = string.Empty;

    // Contact Person
    [Required]
    [MaxLength(100)]
    public string ContactPersonName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ContactPersonDesignation { get; set; }

    [MaxLength(15)]
    public string? ContactPersonPhone { get; set; }

    [MaxLength(100)]
    public string? ContactPersonEmail { get; set; }

    // Legal & Compliance
    [MaxLength(50)]
    public string? GSTNumber { get; set; }

    [MaxLength(50)]
    public string? PANNumber { get; set; }

    [MaxLength(50)]
    public string? RegistrationNumber { get; set; }

    public DateTime? RegistrationDate { get; set; }

    [MaxLength(100)]
    public string? LicenseNumber { get; set; }

    public DateTime? LicenseValidTill { get; set; }

    // Contract Details
    public DateTime ContractStartDate { get; set; }

    public DateTime ContractEndDate { get; set; }

    public bool IsContractActive { get; set; } = true;

    [MaxLength(100)]
    public string? ContractNumber { get; set; }

    // Commission Structure
    public decimal CommissionPercentage { get; set; }

    public decimal? FixedFeePerCase { get; set; }

    [MaxLength(50)]
    public string CommissionModel { get; set; } = string.Empty; // Percentage, Fixed, Hybrid

    public decimal? MinimumCommission { get; set; }

    public decimal? MaximumCommission { get; set; }

    // Operational Limits
    public int MaxActiveCases { get; set; }

    public decimal? MaxPortfolioValue { get; set; }

    [MaxLength(500)]
    public string? GeographicCoverage { get; set; } // Comma-separated states/cities

    [MaxLength(500)]
    public string? ProductCoverage { get; set; } // Which loan products they can handle

    // Performance
    public int TotalCasesAssigned { get; set; }

    public int TotalCasesResolved { get; set; }

    public decimal TotalAmountCollected { get; set; }

    public decimal CollectionEfficiencyIndex { get; set; }

    public decimal AverageResolutionDays { get; set; }

    // Rating & Compliance
    public decimal PerformanceRating { get; set; } // 1-5

    public int ComplianceScore { get; set; } // 1-100

    public int CustomerComplaintCount { get; set; }

    public int RegulatoryViolationCount { get; set; }

    // Status
    public bool IsActive { get; set; } = true;

    public bool IsBlacklisted { get; set; }

    [MaxLength(500)]
    public string? BlacklistReason { get; set; }

    public DateTime? BlacklistedDate { get; set; }

    // Bank Details for Payments
    [MaxLength(100)]
    public string? BankAccountNumber { get; set; }

    [MaxLength(50)]
    public string? IFSCCode { get; set; }

    [MaxLength(100)]
    public string? BankName { get; set; }

    [MaxLength(100)]
    public string? BranchName { get; set; }

    // Notes
    [MaxLength(2000)]
    public string? Notes { get; set; }

    // Navigation Properties
    public virtual ICollection<ApplicationUser> Agents { get; set; } = new List<ApplicationUser>();
}
