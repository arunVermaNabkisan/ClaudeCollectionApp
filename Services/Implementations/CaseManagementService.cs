using ClaudeCollectionApp.Data;
using ClaudeCollectionApp.Models.Entities;
using ClaudeCollectionApp.Models.Enums;
using ClaudeCollectionApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClaudeCollectionApp.Services.Implementations;

/// <summary>
/// Implementation of Collection Case Management Service
/// </summary>
public class CaseManagementService : ICaseManagementService
{
    private readonly ApplicationDbContext _context;

    public CaseManagementService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Case CRUD operations
    public async Task<CollectionCase?> GetCaseByIdAsync(Guid caseId)
    {
        return await _context.CollectionCases
            .Include(c => c.Customer)
            .Include(c => c.LoanAccount)
            .Include(c => c.AssignedToUser)
            .FirstOrDefaultAsync(c => c.Id == caseId);
    }

    public async Task<CollectionCase?> GetCaseByNumberAsync(string caseNumber)
    {
        return await _context.CollectionCases
            .Include(c => c.Customer)
            .Include(c => c.LoanAccount)
            .Include(c => c.AssignedToUser)
            .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
    }

    public async Task<CollectionCase?> GetCaseByLoanAccountAsync(Guid loanAccountId)
    {
        return await _context.CollectionCases
            .Include(c => c.Customer)
            .Include(c => c.LoanAccount)
            .Include(c => c.AssignedToUser)
            .FirstOrDefaultAsync(c => c.LoanAccountId == loanAccountId);
    }

    public async Task<IEnumerable<CollectionCase>> GetCasesByCustomerAsync(Guid customerId)
    {
        return await _context.CollectionCases
            .Include(c => c.LoanAccount)
            .Include(c => c.AssignedToUser)
            .Where(c => c.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<(IEnumerable<CollectionCase> Cases, int TotalCount)> GetCasesPagedAsync(
        int pageNumber, int pageSize, string? searchTerm = null,
        DelinquencyBucket? bucket = null, CaseStatus? status = null,
        Guid? assignedToUserId = null)
    {
        var query = _context.CollectionCases
            .Include(c => c.Customer)
            .Include(c => c.LoanAccount)
            .Include(c => c.AssignedToUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c =>
                c.CaseNumber.Contains(searchTerm) ||
                c.Customer.FullName.Contains(searchTerm) ||
                c.LoanAccount.LoanAccountNumber.Contains(searchTerm));
        }

        if (bucket.HasValue)
            query = query.Where(c => c.CurrentBucket == bucket.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (assignedToUserId.HasValue)
            query = query.Where(c => c.AssignedToUserId == assignedToUserId.Value);

        var totalCount = await query.CountAsync();
        var cases = await query
            .OrderByDescending(c => c.Priority)
            .ThenByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (cases, totalCount);
    }

    // Case Creation & Lifecycle
    public async Task<CollectionCase> CreateCaseAsync(Guid loanAccountId)
    {
        var loanAccount = await _context.LoanAccounts
            .Include(l => l.Customer)
            .FirstOrDefaultAsync(l => l.Id == loanAccountId);

        if (loanAccount == null)
            throw new InvalidOperationException("Loan account not found");

        var existingCase = await _context.CollectionCases
            .FirstOrDefaultAsync(c => c.LoanAccountId == loanAccountId);

        if (existingCase != null)
            throw new InvalidOperationException("Collection case already exists for this loan account");

        var caseNumber = await GenerateCaseNumberAsync();

        var collectionCase = new CollectionCase
        {
            Id = Guid.NewGuid(),
            CaseNumber = caseNumber,
            LoanAccountId = loanAccountId,
            CustomerId = loanAccount.CustomerId,
            Status = CaseStatus.New,
            CurrentBucket = loanAccount.CurrentBucket,
            DaysPastDue = loanAccount.DaysPastDue,
            TotalOutstandingAmount = loanAccount.TotalOutstanding,
            PrincipalOutstanding = loanAccount.PrincipalOutstanding,
            InterestOutstanding = loanAccount.InterestOutstanding,
            PenaltyOutstanding = loanAccount.PenaltyOutstanding,
            Priority = CalculateInitialPriority(loanAccount),
            OpenedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.CollectionCases.Add(collectionCase);
        await _context.SaveChangesAsync();

        return collectionCase;
    }

    public async Task<bool> UpdateCaseStatusAsync(Guid caseId, CaseStatus newStatus, string? reason = null)
    {
        var collectionCase = await _context.CollectionCases.FindAsync(caseId);
        if (collectionCase == null) return false;

        var oldStatus = collectionCase.Status;
        collectionCase.Status = newStatus;
        collectionCase.UpdatedAt = DateTime.UtcNow;

        // Add status history
        var statusHistory = new CaseStatusHistory
        {
            Id = Guid.NewGuid(),
            CollectionCaseId = caseId,
            FromStatus = oldStatus,
            ToStatus = newStatus,
            StatusChangeDate = DateTime.UtcNow,
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };

        _context.CaseStatusHistories.Add(statusHistory);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CloseCaseAsync(Guid caseId, string resolutionType)
    {
        var collectionCase = await _context.CollectionCases.FindAsync(caseId);
        if (collectionCase == null) return false;

        collectionCase.Status = CaseStatus.Closed;
        collectionCase.ClosedDate = DateTime.UtcNow;
        collectionCase.ResolutionType = resolutionType;
        collectionCase.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // Case Assignment
    public async Task<bool> AssignCaseAsync(Guid caseId, Guid toUserId, string assignmentType, string? reason = null)
    {
        var collectionCase = await _context.CollectionCases.FindAsync(caseId);
        if (collectionCase == null) return false;

        var previousUserId = collectionCase.AssignedToUserId;
        collectionCase.AssignedToUserId = toUserId;
        collectionCase.LastAssignmentDate = DateTime.UtcNow;
        collectionCase.UpdatedAt = DateTime.UtcNow;

        // Add assignment history
        var assignmentHistory = new CaseAssignmentHistory
        {
            Id = Guid.NewGuid(),
            CollectionCaseId = caseId,
            FromUserId = previousUserId,
            ToUserId = toUserId,
            AssignmentType = assignmentType,
            AssignmentDate = DateTime.UtcNow,
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        };

        _context.CaseAssignmentHistories.Add(assignmentHistory);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AssignCasesBulkAsync(IEnumerable<Guid> caseIds, Guid toUserId, string reason)
    {
        foreach (var caseId in caseIds)
        {
            await AssignCaseAsync(caseId, toUserId, "Bulk Assignment", reason);
        }
        return true;
    }

    public async Task<IEnumerable<CollectionCase>> GetMyCasesAsync(Guid userId)
    {
        return await _context.CollectionCases
            .Include(c => c.Customer)
            .Include(c => c.LoanAccount)
            .Where(c => c.AssignedToUserId == userId && c.Status != CaseStatus.Closed)
            .OrderByDescending(c => c.Priority)
            .ToListAsync();
    }

    public async Task<IEnumerable<CollectionCase>> GetTeamCasesAsync(Guid teamLeaderId)
    {
        var teamMembers = await _context.Users
            .Where(u => u.ReportingManagerId == teamLeaderId)
            .Select(u => u.Id)
            .ToListAsync();

        teamMembers.Add(teamLeaderId);

        return await _context.CollectionCases
            .Include(c => c.Customer)
            .Include(c => c.LoanAccount)
            .Include(c => c.AssignedToUser)
            .Where(c => c.AssignedToUserId.HasValue && teamMembers.Contains(c.AssignedToUserId.Value))
            .OrderByDescending(c => c.Priority)
            .ToListAsync();
    }

    // Delinquency Bucketing
    public async Task UpdateDelinquencyBucketsAsync()
    {
        var cases = await _context.CollectionCases
            .Include(c => c.LoanAccount)
            .Where(c => c.Status != CaseStatus.Closed)
            .ToListAsync();

        foreach (var collectionCase in cases)
        {
            var newBucket = CalculateBucketFromDPD(collectionCase.DaysPastDue);
            if (collectionCase.CurrentBucket != newBucket)
            {
                await MoveCaseToBucketAsync(collectionCase.Id, newBucket);
            }
        }
    }

    public async Task<bool> MoveCaseToBucketAsync(Guid caseId, DelinquencyBucket bucket)
    {
        var collectionCase = await _context.CollectionCases.FindAsync(caseId);
        if (collectionCase == null) return false;

        collectionCase.CurrentBucket = bucket;
        collectionCase.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Dictionary<DelinquencyBucket, int>> GetBucketDistributionAsync(Guid? userId = null)
    {
        var query = _context.CollectionCases.AsQueryable();

        if (userId.HasValue)
            query = query.Where(c => c.AssignedToUserId == userId.Value);

        var distribution = await query
            .Where(c => c.Status != CaseStatus.Closed)
            .GroupBy(c => c.CurrentBucket)
            .Select(g => new { Bucket = g.Key, Count = g.Count() })
            .ToListAsync();

        return distribution.ToDictionary(x => x.Bucket, x => x.Count);
    }

    // Case Scoring & Priority
    public async Task<bool> UpdateCaseScoreAsync(Guid caseId)
    {
        var collectionCase = await _context.CollectionCases
            .Include(c => c.LoanAccount)
            .FirstOrDefaultAsync(c => c.Id == caseId);

        if (collectionCase == null) return false;

        // Calculate collection score based on various factors
        var score = CalculateCollectionScore(collectionCase);
        collectionCase.CollectionScore = score;
        collectionCase.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateCasePriorityAsync(Guid caseId, int priority)
    {
        var collectionCase = await _context.CollectionCases.FindAsync(caseId);
        if (collectionCase == null) return false;

        collectionCase.Priority = priority;
        collectionCase.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task RecalculateProbabilityOfPaymentAsync(Guid caseId)
    {
        var collectionCase = await _context.CollectionCases
            .Include(c => c.PromisesToPay)
            .Include(c => c.Interactions)
            .FirstOrDefaultAsync(c => c.Id == caseId);

        if (collectionCase == null) return;

        // Calculate POP based on historical PTP keep ratio, interaction patterns, etc.
        var pop = CalculateProbabilityOfPayment(collectionCase);
        collectionCase.ProbabilityOfPayment = pop;
        collectionCase.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    // Case History
    public async Task<IEnumerable<CaseStatusHistory>> GetCaseStatusHistoryAsync(Guid caseId)
    {
        return await _context.CaseStatusHistories
            .Where(h => h.CollectionCaseId == caseId)
            .OrderByDescending(h => h.StatusChangeDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<CaseAssignmentHistory>> GetCaseAssignmentHistoryAsync(Guid caseId)
    {
        return await _context.CaseAssignmentHistories
            .Where(h => h.CollectionCaseId == caseId)
            .OrderByDescending(h => h.AssignmentDate)
            .ToListAsync();
    }

    // Case Notes
    public async Task<CaseNote> AddCaseNoteAsync(Guid caseId, string noteText, string? noteType = null, bool isPinned = false)
    {
        var note = new CaseNote
        {
            Id = Guid.NewGuid(),
            CollectionCaseId = caseId,
            NoteText = noteText,
            NoteType = noteType,
            IsPinned = isPinned,
            NoteDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.CaseNotes.Add(note);
        await _context.SaveChangesAsync();

        return note;
    }

    public async Task<IEnumerable<CaseNote>> GetCaseNotesAsync(Guid caseId)
    {
        return await _context.CaseNotes
            .Where(n => n.CollectionCaseId == caseId)
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.NoteDate)
            .ToListAsync();
    }

    // Case Analytics
    public async Task<Dictionary<string, object>> GetCaseAnalyticsAsync(Guid caseId)
    {
        var collectionCase = await GetCaseByIdAsync(caseId);
        if (collectionCase == null) return new Dictionary<string, object>();

        var ptps = await _context.PromisesToPay
            .Where(p => p.CollectionCaseId == caseId)
            .ToListAsync();

        var payments = await _context.Payments
            .Where(p => p.CollectionCaseId == caseId)
            .ToListAsync();

        var interactions = await _context.CustomerInteractions
            .Where(i => i.CollectionCaseId == caseId)
            .ToListAsync();

        return new Dictionary<string, object>
        {
            ["TotalPTPs"] = ptps.Count,
            ["KeptPTPs"] = ptps.Count(p => p.Status == PTPStatus.Kept),
            ["BrokenPTPs"] = ptps.Count(p => p.Status == PTPStatus.Broken),
            ["TotalPayments"] = payments.Sum(p => p.Amount),
            ["PaymentCount"] = payments.Count,
            ["TotalInteractions"] = interactions.Count,
            ["LastInteractionDate"] = interactions.OrderByDescending(i => i.InteractionDateTime).FirstOrDefault()?.InteractionDateTime,
            ["DaysSinceOpened"] = (DateTime.UtcNow - collectionCase.OpenedDate).Days
        };
    }

    public async Task<Dictionary<string, object>> GetPortfolioAnalyticsAsync(Guid? userId = null, string? vertical = null)
    {
        var query = _context.CollectionCases.AsQueryable();

        if (userId.HasValue)
            query = query.Where(c => c.AssignedToUserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(vertical))
            query = query.Where(c => c.LoanAccount.ProductVertical == vertical);

        var totalCases = await query.CountAsync();
        var activeCases = await query.CountAsync(c => c.Status == CaseStatus.Active);
        var totalOutstanding = await query.SumAsync(c => c.TotalOutstandingAmount);
        var totalCollected = await _context.Payments
            .Where(p => query.Any(c => c.Id == p.CollectionCaseId))
            .SumAsync(p => p.Amount);

        return new Dictionary<string, object>
        {
            ["TotalCases"] = totalCases,
            ["ActiveCases"] = activeCases,
            ["TotalOutstanding"] = totalOutstanding,
            ["TotalCollected"] = totalCollected,
            ["CollectionRate"] = totalOutstanding > 0 ? (totalCollected / totalOutstanding) * 100 : 0
        };
    }

    // Helper Methods
    private async Task<string> GenerateCaseNumberAsync()
    {
        var date = DateTime.UtcNow;
        var prefix = $"CASE{date:yyyyMM}";
        var count = await _context.CollectionCases
            .CountAsync(c => c.CaseNumber.StartsWith(prefix));

        return $"{prefix}{(count + 1):D6}";
    }

    private int CalculateInitialPriority(LoanAccount loanAccount)
    {
        var priority = 0;

        // Higher outstanding amount = higher priority
        if (loanAccount.TotalOutstanding > 1000000) priority += 50;
        else if (loanAccount.TotalOutstanding > 500000) priority += 30;
        else if (loanAccount.TotalOutstanding > 100000) priority += 20;

        // Higher DPD = higher priority
        if (loanAccount.DaysPastDue > 90) priority += 40;
        else if (loanAccount.DaysPastDue > 60) priority += 30;
        else if (loanAccount.DaysPastDue > 30) priority += 20;

        return Math.Min(priority, 100);
    }

    private DelinquencyBucket CalculateBucketFromDPD(int dpd)
    {
        return dpd switch
        {
            <= 30 => DelinquencyBucket.X,
            <= 60 => DelinquencyBucket.X1_30,
            <= 90 => DelinquencyBucket.X31_60,
            <= 120 => DelinquencyBucket.X61_90,
            <= 150 => DelinquencyBucket.X91_120,
            <= 180 => DelinquencyBucket.X121_150,
            _ => DelinquencyBucket.X151Plus
        };
    }

    private decimal CalculateCollectionScore(CollectionCase collectionCase)
    {
        // Implement scoring logic based on various factors
        var score = 50m; // Base score

        // Adjust based on DPD
        if (collectionCase.DaysPastDue > 90) score += 20;
        else if (collectionCase.DaysPastDue > 60) score += 10;

        // Adjust based on outstanding amount
        if (collectionCase.TotalOutstandingAmount > 500000) score += 15;

        // Adjust based on probability of payment
        if (collectionCase.ProbabilityOfPayment.HasValue)
        {
            score += (decimal)collectionCase.ProbabilityOfPayment.Value * 15;
        }

        return Math.Min(score, 100);
    }

    private double CalculateProbabilityOfPayment(CollectionCase collectionCase)
    {
        // Simple POP calculation - can be enhanced with ML models
        var score = 0.5; // Base 50%

        var keptPTPs = collectionCase.PromisesToPay.Count(p => p.Status == PTPStatus.Kept);
        var totalPTPs = collectionCase.PromisesToPay.Count;

        if (totalPTPs > 0)
        {
            var keepRatio = (double)keptPTPs / totalPTPs;
            score = (score + keepRatio) / 2;
        }

        return Math.Min(score, 1.0);
    }
}
