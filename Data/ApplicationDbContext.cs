using ClaudeCollectionApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClaudeCollectionApp.Data;

/// <summary>
/// Main database context for Collection Management System
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Core Entities
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<LoanAccount> LoanAccounts => Set<LoanAccount>();
    public DbSet<CollectionCase> CollectionCases => Set<CollectionCase>();

    // Promise to Pay
    public DbSet<PromiseToPay> PromisesToPay => Set<PromiseToPay>();
    public DbSet<PTPFollowUp> PTPFollowUps => Set<PTPFollowUp>();

    // Communication & Interactions
    public DbSet<CustomerInteraction> CustomerInteractions => Set<CustomerInteraction>();
    public DbSet<AlternateContact> AlternateContacts => Set<AlternateContact>();

    // Payments
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentLink> PaymentLinks => Set<PaymentLink>();
    public DbSet<LoanTransaction> LoanTransactions => Set<LoanTransaction>();

    // Field Collections
    public DbSet<FieldVisit> FieldVisits => Set<FieldVisit>();
    public DbSet<FieldVisitPhoto> FieldVisitPhotos => Set<FieldVisitPhoto>();

    // Documents
    public DbSet<Document> Documents => Set<Document>();

    // Case Management
    public DbSet<CaseAssignmentHistory> CaseAssignmentHistories => Set<CaseAssignmentHistory>();
    public DbSet<CaseStatusHistory> CaseStatusHistories => Set<CaseStatusHistory>();
    public DbSet<CaseNote> CaseNotes => Set<CaseNote>();

    // Recovery Agencies
    public DbSet<RecoveryAgency> RecoveryAgencies => Set<RecoveryAgency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Identity tables with custom names
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
        });

        modelBuilder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.ToTable("Roles");
        });

        modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("UserTokens");
        });

        modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        // Configure Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.PrimaryPhone);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.PANNumber).IsUnique().HasFilter("[PANNumber] IS NOT NULL");
            entity.HasIndex(e => e.AadharNumber).IsUnique().HasFilter("[AadharNumber] IS NOT NULL");
            entity.HasIndex(e => e.LMSCustomerId).IsUnique().HasFilter("[LMSCustomerId] IS NOT NULL");
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure LoanAccount
        modelBuilder.Entity<LoanAccount>(entity =>
        {
            entity.HasIndex(e => e.LoanAccountNumber).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.LoanStatus);
            entity.HasIndex(e => e.CurrentBucket);
            entity.HasIndex(e => e.DaysPastDue);
            entity.HasIndex(e => e.AssignedRMId);
            entity.HasIndex(e => e.LMSLoanId).IsUnique().HasFilter("[LMSLoanId] IS NOT NULL");
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.LoanAccounts)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedRM)
                .WithMany(u => u.AssignedLoans)
                .HasForeignKey(e => e.AssignedRMId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure CollectionCase
        modelBuilder.Entity<CollectionCase>(entity =>
        {
            entity.HasIndex(e => e.CaseNumber).IsUnique();
            entity.HasIndex(e => e.LoanAccountId).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CurrentBucket);
            entity.HasIndex(e => e.AssignedToUserId);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.LoanAccount)
                .WithOne(l => l.CollectionCase)
                .HasForeignKey<CollectionCase>(e => e.LoanAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedToUser)
                .WithMany(u => u.AssignedCases)
                .HasForeignKey(e => e.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure PromiseToPay
        modelBuilder.Entity<PromiseToPay>(entity =>
        {
            entity.HasIndex(e => e.PTPNumber).IsUnique();
            entity.HasIndex(e => e.CollectionCaseId);
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.PromisedDate);
            entity.HasIndex(e => e.ParentPTPId);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.CollectionCase)
                .WithMany(c => c.PromisesToPay)
                .HasForeignKey(e => e.CollectionCaseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ParentPTP)
                .WithMany(p => p.SplitPTPs)
                .HasForeignKey(e => e.ParentPTPId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.PaymentReferenceNumber).IsUnique();
            entity.HasIndex(e => e.LoanAccountId);
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.PaymentDate);
            entity.HasIndex(e => e.IsReconciled);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.LoanAccount)
                .WithMany(l => l.Payments)
                .HasForeignKey(e => e.LoanAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CollectionCase)
                .WithMany()
                .HasForeignKey(e => e.CollectionCaseId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure FieldVisit
        modelBuilder.Entity<FieldVisit>(entity =>
        {
            entity.HasIndex(e => e.VisitNumber).IsUnique();
            entity.HasIndex(e => e.CollectionCaseId);
            entity.HasIndex(e => e.AssignedToUserId);
            entity.HasIndex(e => e.PlannedDate);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.CollectionCase)
                .WithMany(c => c.FieldVisits)
                .HasForeignKey(e => e.CollectionCaseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedToUser)
                .WithMany(u => u.AssignedFieldVisits)
                .HasForeignKey(e => e.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure CustomerInteraction
        modelBuilder.Entity<CustomerInteraction>(entity =>
        {
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.CollectionCaseId);
            entity.HasIndex(e => e.InteractionDateTime);
            entity.HasIndex(e => e.Channel);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Interactions)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CollectionCase)
                .WithMany(c => c.Interactions)
                .HasForeignKey(e => e.CollectionCaseId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Document
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasIndex(e => e.DocumentNumber).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.LoanAccountId);
            entity.HasIndex(e => e.CollectionCaseId);
            entity.HasIndex(e => e.DocumentType);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.ParentDocument)
                .WithMany(d => d.ChildDocuments)
                .HasForeignKey(e => e.ParentDocumentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure ApplicationUser
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(e => e.EmployeeId).IsUnique();
            entity.HasIndex(e => e.UserRole);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasOne(e => e.ReportingManager)
                .WithMany(u => u.Subordinates)
                .HasForeignKey(e => e.ReportingManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RecoveryAgency)
                .WithMany(r => r.Agents)
                .HasForeignKey(e => e.RecoveryAgencyId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure RecoveryAgency
        modelBuilder.Entity<RecoveryAgency>(entity =>
        {
            entity.HasIndex(e => e.AgencyCode).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure AlternateContact
        modelBuilder.Entity<AlternateContact>(entity =>
        {
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.AlternateContacts)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure PaymentLink
        modelBuilder.Entity<PaymentLink>(entity =>
        {
            entity.HasIndex(e => e.LinkId).IsUnique();
            entity.HasIndex(e => e.LoanAccountId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure LoanTransaction
        modelBuilder.Entity<LoanTransaction>(entity =>
        {
            entity.HasIndex(e => e.TransactionNumber).IsUnique();
            entity.HasIndex(e => e.LoanAccountId);
            entity.HasIndex(e => e.TransactionDate);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasQueryFilter(e => !e.IsDeleted);

            entity.HasOne(e => e.LoanAccount)
                .WithMany(l => l.Transactions)
                .HasForeignKey(e => e.LoanAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Case History Tables
        modelBuilder.Entity<CaseAssignmentHistory>(entity =>
        {
            entity.HasIndex(e => e.CollectionCaseId);
            entity.HasIndex(e => e.AssignmentDate);

            entity.HasOne(e => e.CollectionCase)
                .WithMany(c => c.AssignmentHistory)
                .HasForeignKey(e => e.CollectionCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CaseStatusHistory>(entity =>
        {
            entity.HasIndex(e => e.CollectionCaseId);
            entity.HasIndex(e => e.StatusChangeDate);

            entity.HasOne(e => e.CollectionCase)
                .WithMany(c => c.StatusHistory)
                .HasForeignKey(e => e.CollectionCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CaseNote>(entity =>
        {
            entity.HasIndex(e => e.CollectionCaseId);
            entity.HasIndex(e => e.NoteDate);
            entity.HasIndex(e => e.IsPinned);

            entity.HasOne(e => e.CollectionCase)
                .WithMany(c => c.Notes)
                .HasForeignKey(e => e.CollectionCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PTPFollowUp>(entity =>
        {
            entity.HasIndex(e => e.PTPId);
            entity.HasIndex(e => e.FollowUpDate);

            entity.HasOne(e => e.PromiseToPay)
                .WithMany(p => p.FollowUps)
                .HasForeignKey(e => e.PTPId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FieldVisitPhoto>(entity =>
        {
            entity.HasIndex(e => e.FieldVisitId);
            entity.HasIndex(e => e.CaptureTimestamp);

            entity.HasOne(e => e.FieldVisit)
                .WithMany(f => f.Photos)
                .HasForeignKey(e => e.FieldVisitId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Apply configurations from separate files if needed
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    /// <summary>
    /// Override SaveChanges to automatically update audit fields
    /// </summary>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateAuditFields();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically update audit fields
    /// </summary>
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                // Set CreatedBy from current user context (implement user context service)
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                // Set UpdatedBy from current user context
            }
        }
    }
}
