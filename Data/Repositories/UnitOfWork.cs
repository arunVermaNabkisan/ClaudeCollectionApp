using ClaudeCollectionApp.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClaudeCollectionApp.Data.Repositories;

/// <summary>
/// Unit of Work implementation
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy initialization of repositories
    private IRepository<Customer>? _customers;
    private IRepository<LoanAccount>? _loanAccounts;
    private IRepository<CollectionCase>? _collectionCases;
    private IRepository<PromiseToPay>? _promisesToPay;
    private IRepository<PTPFollowUp>? _ptpFollowUps;
    private IRepository<CustomerInteraction>? _customerInteractions;
    private IRepository<AlternateContact>? _alternateContacts;
    private IRepository<Payment>? _payments;
    private IRepository<PaymentLink>? _paymentLinks;
    private IRepository<LoanTransaction>? _loanTransactions;
    private IRepository<FieldVisit>? _fieldVisits;
    private IRepository<FieldVisitPhoto>? _fieldVisitPhotos;
    private IRepository<Document>? _documents;
    private IRepository<CaseAssignmentHistory>? _caseAssignmentHistories;
    private IRepository<CaseStatusHistory>? _caseStatusHistories;
    private IRepository<CaseNote>? _caseNotes;
    private IRepository<RecoveryAgency>? _recoveryAgencies;
    private IRepository<ApplicationUser>? _users;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Customer> Customers =>
        _customers ??= new Repository<Customer>(_context);

    public IRepository<LoanAccount> LoanAccounts =>
        _loanAccounts ??= new Repository<LoanAccount>(_context);

    public IRepository<CollectionCase> CollectionCases =>
        _collectionCases ??= new Repository<CollectionCase>(_context);

    public IRepository<PromiseToPay> PromisesToPay =>
        _promisesToPay ??= new Repository<PromiseToPay>(_context);

    public IRepository<PTPFollowUp> PTPFollowUps =>
        _ptpFollowUps ??= new Repository<PTPFollowUp>(_context);

    public IRepository<CustomerInteraction> CustomerInteractions =>
        _customerInteractions ??= new Repository<CustomerInteraction>(_context);

    public IRepository<AlternateContact> AlternateContacts =>
        _alternateContacts ??= new Repository<AlternateContact>(_context);

    public IRepository<Payment> Payments =>
        _payments ??= new Repository<Payment>(_context);

    public IRepository<PaymentLink> PaymentLinks =>
        _paymentLinks ??= new Repository<PaymentLink>(_context);

    public IRepository<LoanTransaction> LoanTransactions =>
        _loanTransactions ??= new Repository<LoanTransaction>(_context);

    public IRepository<FieldVisit> FieldVisits =>
        _fieldVisits ??= new Repository<FieldVisit>(_context);

    public IRepository<FieldVisitPhoto> FieldVisitPhotos =>
        _fieldVisitPhotos ??= new Repository<FieldVisitPhoto>(_context);

    public IRepository<Document> Documents =>
        _documents ??= new Repository<Document>(_context);

    public IRepository<CaseAssignmentHistory> CaseAssignmentHistories =>
        _caseAssignmentHistories ??= new Repository<CaseAssignmentHistory>(_context);

    public IRepository<CaseStatusHistory> CaseStatusHistories =>
        _caseStatusHistories ??= new Repository<CaseStatusHistory>(_context);

    public IRepository<CaseNote> CaseNotes =>
        _caseNotes ??= new Repository<CaseNote>(_context);

    public IRepository<RecoveryAgency> RecoveryAgencies =>
        _recoveryAgencies ??= new Repository<RecoveryAgency>(_context);

    public IRepository<ApplicationUser> Users =>
        _users ??= new Repository<ApplicationUser>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
