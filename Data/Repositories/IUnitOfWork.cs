using ClaudeCollectionApp.Models.Entities;

namespace ClaudeCollectionApp.Data.Repositories;

/// <summary>
/// Unit of Work pattern for managing transactions across multiple repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Core Repositories
    IRepository<Customer> Customers { get; }
    IRepository<LoanAccount> LoanAccounts { get; }
    IRepository<CollectionCase> CollectionCases { get; }

    // Promise to Pay
    IRepository<PromiseToPay> PromisesToPay { get; }
    IRepository<PTPFollowUp> PTPFollowUps { get; }

    // Communication
    IRepository<CustomerInteraction> CustomerInteractions { get; }
    IRepository<AlternateContact> AlternateContacts { get; }

    // Payments
    IRepository<Payment> Payments { get; }
    IRepository<PaymentLink> PaymentLinks { get; }
    IRepository<LoanTransaction> LoanTransactions { get; }

    // Field Collection
    IRepository<FieldVisit> FieldVisits { get; }
    IRepository<FieldVisitPhoto> FieldVisitPhotos { get; }

    // Documents
    IRepository<Document> Documents { get; }

    // Case Management
    IRepository<CaseAssignmentHistory> CaseAssignmentHistories { get; }
    IRepository<CaseStatusHistory> CaseStatusHistories { get; }
    IRepository<CaseNote> CaseNotes { get; }

    // Recovery Agencies
    IRepository<RecoveryAgency> RecoveryAgencies { get; }
    IRepository<ApplicationUser> Users { get; }

    // Transaction management
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
