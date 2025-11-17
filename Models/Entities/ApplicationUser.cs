using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Models.Entities;

/// <summary>
/// Application user entity extending IdentityUser
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
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

    [Required]
    [MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    public UserRole UserRole { get; set; }

    [MaxLength(100)]
    public string? Designation { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    // Organizational Hierarchy
    [MaxLength(100)]
    public string? VerticalName { get; set; } // FPO, AVCF, Corporate, etc.

    [MaxLength(100)]
    public string? BranchName { get; set; }

    [MaxLength(100)]
    public string? RegionName { get; set; }

    [MaxLength(100)]
    public string? ZoneName { get; set; }

    // Reporting Structure
    public Guid? ReportingManagerId { get; set; }

    public virtual ApplicationUser? ReportingManager { get; set; }

    // External Agency (if applicable)
    public bool IsExternalAgent { get; set; }

    public Guid? RecoveryAgencyId { get; set; }

    public virtual RecoveryAgency? RecoveryAgency { get; set; }

    // Work Details
    public DateTime JoiningDate { get; set; }

    public DateTime? LastWorkingDate { get; set; }

    public bool IsActive { get; set; } = true;

    // Contact Information
    [MaxLength(15)]
    public string? MobileNumber { get; set; }

    [MaxLength(15)]
    public string? AlternateNumber { get; set; }

    // Targets & Performance
    public decimal? MonthlyCollectionTarget { get; set; }

    public decimal? CurrentMonthCollection { get; set; }

    public int? MonthlyCaseTarget { get; set; }

    public int? ActiveCasesCount { get; set; }

    // Permissions & Capabilities
    public bool CanAccessFieldModule { get; set; }

    public bool CanApprovePTPs { get; set; }

    public bool CanProcessPayments { get; set; }

    public bool CanViewReports { get; set; }

    [MaxLength(500)]
    public string? LanguagesSpoken { get; set; } // Comma-separated

    [MaxLength(500)]
    public string? GeographicCoverage { get; set; } // Comma-separated pincodes/cities

    // Security
    public DateTime? LastLoginDate { get; set; }

    public int FailedLoginAttempts { get; set; }

    public DateTime? AccountLockedUntil { get; set; }

    public bool MustChangePassword { get; set; }

    public DateTime? PasswordChangedDate { get; set; }

    // Audit Fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    // Navigation Properties
    public virtual ICollection<CollectionCase> AssignedCases { get; set; } = new List<CollectionCase>();
    public virtual ICollection<LoanAccount> AssignedLoans { get; set; } = new List<LoanAccount>();
    public virtual ICollection<FieldVisit> AssignedFieldVisits { get; set; } = new List<FieldVisit>();
    public virtual ICollection<ApplicationUser> Subordinates { get; set; } = new List<ApplicationUser>();
}
