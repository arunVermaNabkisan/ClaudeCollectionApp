using ClaudeCollectionApp.Data;
using ClaudeCollectionApp.Models.Entities;
using ClaudeCollectionApp.Models.Enums;
using ClaudeCollectionApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClaudeCollectionApp.Services.Implementations;

/// <summary>
/// Implementation of Promise to Pay Management Service
/// </summary>
public class PromiseToPayService : IPromiseToPayService
{
    private readonly ApplicationDbContext _context;

    public PromiseToPayService(ApplicationDbContext context)
    {
        _context = context;
    }

    // PTP CRUD operations
    public async Task<PromiseToPay?> GetPTPByIdAsync(Guid ptpId)
    {
        return await _context.PromisesToPay
            .Include(p => p.CollectionCase)
                .ThenInclude(c => c.Customer)
            .Include(p => p.CollectionCase)
                .ThenInclude(c => c.LoanAccount)
            .Include(p => p.CreatedByUser)
            .FirstOrDefaultAsync(p => p.Id == ptpId);
    }

    public async Task<PromiseToPay?> GetPTPByNumberAsync(string ptpNumber)
    {
        return await _context.PromisesToPay
            .Include(p => p.CollectionCase)
            .FirstOrDefaultAsync(p => p.PTPNumber == ptpNumber);
    }

    public async Task<IEnumerable<PromiseToPay>> GetPTPsByCaseAsync(Guid caseId)
    {
        return await _context.PromisesToPay
            .Where(p => p.CollectionCaseId == caseId)
            .OrderByDescending(p => p.PromisedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PromiseToPay>> GetPTPsByCustomerAsync(Guid customerId)
    {
        return await _context.PromisesToPay
            .Include(p => p.CollectionCase)
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.PromisedDate)
            .ToListAsync();
    }

    public async Task<(IEnumerable<PromiseToPay> PTPs, int TotalCount)> GetPTPsPagedAsync(
        int pageNumber, int pageSize, PTPStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.PromisesToPay
            .Include(p => p.CollectionCase)
                .ThenInclude(c => c.Customer)
            .Include(p => p.CreatedByUser)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (fromDate.HasValue)
            query = query.Where(p => p.PromisedDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(p => p.PromisedDate <= toDate.Value);

        var totalCount = await query.CountAsync();
        var ptps = await query
            .OrderByDescending(p => p.PromisedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (ptps, totalCount);
    }

    // PTP Creation & Management
    public async Task<PromiseToPay> CreatePTPAsync(Guid caseId, decimal amount, DateTime promisedDate,
        PaymentMode paymentMode, int confidenceLevel)
    {
        var collectionCase = await _context.CollectionCases
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == caseId);

        if (collectionCase == null)
            throw new InvalidOperationException("Collection case not found");

        var ptpNumber = await GeneratePTPNumberAsync();

        var ptp = new PromiseToPay
        {
            Id = Guid.NewGuid(),
            PTPNumber = ptpNumber,
            CollectionCaseId = caseId,
            CustomerId = collectionCase.CustomerId,
            PromisedAmount = amount,
            PromisedDate = promisedDate,
            PaymentMode = paymentMode,
            ConfidenceLevel = confidenceLevel,
            Status = PTPStatus.Active,
            RemindersSent = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.PromisesToPay.Add(ptp);
        await _context.SaveChangesAsync();

        return ptp;
    }

    public async Task<IEnumerable<PromiseToPay>> CreateSplitPTPAsync(Guid caseId,
        List<(decimal Amount, DateTime Date, PaymentMode Mode)> splits, int confidenceLevel)
    {
        var collectionCase = await _context.CollectionCases
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == caseId);

        if (collectionCase == null)
            throw new InvalidOperationException("Collection case not found");

        // Create parent PTP
        var totalAmount = splits.Sum(s => s.Amount);
        var parentPTP = await CreatePTPAsync(caseId, totalAmount, splits.First().Date,
            splits.First().Mode, confidenceLevel);

        var splitPTPs = new List<PromiseToPay>();

        foreach (var split in splits)
        {
            var ptpNumber = await GeneratePTPNumberAsync();

            var splitPTP = new PromiseToPay
            {
                Id = Guid.NewGuid(),
                PTPNumber = ptpNumber,
                CollectionCaseId = caseId,
                CustomerId = collectionCase.CustomerId,
                PromisedAmount = split.Amount,
                PromisedDate = split.Date,
                PaymentMode = split.Mode,
                ConfidenceLevel = confidenceLevel,
                Status = PTPStatus.Active,
                ParentPTPId = parentPTP.Id,
                RemindersSent = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.PromisesToPay.Add(splitPTP);
            splitPTPs.Add(splitPTP);
        }

        await _context.SaveChangesAsync();
        return splitPTPs;
    }

    public async Task<bool> CancelPTPAsync(Guid ptpId, string reason)
    {
        var ptp = await _context.PromisesToPay.FindAsync(ptpId);
        if (ptp == null) return false;

        ptp.Status = PTPStatus.Cancelled;
        ptp.CancellationReason = reason;
        ptp.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // PTP Status Management
    public async Task<bool> MarkPTPAsKeptAsync(Guid ptpId, Guid paymentId)
    {
        var ptp = await _context.PromisesToPay.FindAsync(ptpId);
        if (ptp == null) return false;

        ptp.Status = PTPStatus.Kept;
        ptp.ActualPaymentDate = DateTime.UtcNow;
        ptp.LinkedPaymentId = paymentId;
        ptp.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkPTPAsPartiallyKeptAsync(Guid ptpId, Guid paymentId, decimal paidAmount)
    {
        var ptp = await _context.PromisesToPay.FindAsync(ptpId);
        if (ptp == null) return false;

        ptp.Status = PTPStatus.PartiallyKept;
        ptp.ActualPaymentDate = DateTime.UtcNow;
        ptp.LinkedPaymentId = paymentId;
        ptp.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkPTPAsBrokenAsync(Guid ptpId)
    {
        var ptp = await _context.PromisesToPay.FindAsync(ptpId);
        if (ptp == null) return false;

        ptp.Status = PTPStatus.Broken;
        ptp.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task ProcessExpiredPTPsAsync()
    {
        var expiredPTPs = await _context.PromisesToPay
            .Where(p => p.Status == PTPStatus.Active &&
                       p.PromisedDate < DateTime.UtcNow.Date)
            .ToListAsync();

        foreach (var ptp in expiredPTPs)
        {
            ptp.Status = PTPStatus.Broken;
            ptp.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    // PTP Follow-ups
    public async Task<PTPFollowUp> CreatePTPFollowUpAsync(Guid ptpId, DateTime followUpDate, string channel)
    {
        var followUp = new PTPFollowUp
        {
            Id = Guid.NewGuid(),
            PTPId = ptpId,
            FollowUpDate = followUpDate,
            Channel = channel,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.PTPFollowUps.Add(followUp);
        await _context.SaveChangesAsync();

        return followUp;
    }

    public async Task<IEnumerable<PTPFollowUp>> GetPendingFollowUpsAsync(Guid userId)
    {
        return await _context.PTPFollowUps
            .Include(f => f.PromiseToPay)
                .ThenInclude(p => p.CollectionCase)
            .Where(f => f.Status == "Pending" &&
                       f.FollowUpDate <= DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<bool> CompletePTPFollowUpAsync(Guid followUpId, string notes)
    {
        var followUp = await _context.PTPFollowUps.FindAsync(followUpId);
        if (followUp == null) return false;

        followUp.Status = "Completed";
        followUp.CompletedAt = DateTime.UtcNow;
        followUp.Notes = notes;
        followUp.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // PTP Reminders
    public async Task SendPTPRemindersAsync()
    {
        var upcomingPTPs = await _context.PromisesToPay
            .Include(p => p.CollectionCase)
                .ThenInclude(c => c.Customer)
            .Where(p => p.Status == PTPStatus.Active &&
                       p.PromisedDate >= DateTime.UtcNow.Date &&
                       p.PromisedDate <= DateTime.UtcNow.Date.AddDays(3))
            .ToListAsync();

        foreach (var ptp in upcomingPTPs)
        {
            // TODO: Implement actual reminder sending via communication service
            ptp.RemindersSent++;
            ptp.LastReminderSent = DateTime.UtcNow;
            ptp.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> SendPTPReminderAsync(Guid ptpId)
    {
        var ptp = await _context.PromisesToPay
            .Include(p => p.CollectionCase)
                .ThenInclude(c => c.Customer)
            .FirstOrDefaultAsync(p => p.Id == ptpId);

        if (ptp == null) return false;

        // TODO: Implement actual reminder sending via communication service
        ptp.RemindersSent++;
        ptp.LastReminderSent = DateTime.UtcNow;
        ptp.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // PTP Analytics
    public async Task<Dictionary<string, object>> GetPTPPerformanceAsync(Guid? userId = null)
    {
        var query = _context.PromisesToPay.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(p => p.CreatedByUserId == userId.Value);
        }

        var totalPTPs = await query.CountAsync();
        var keptPTPs = await query.CountAsync(p => p.Status == PTPStatus.Kept);
        var brokenPTPs = await query.CountAsync(p => p.Status == PTPStatus.Broken);
        var partiallyKeptPTPs = await query.CountAsync(p => p.Status == PTPStatus.PartiallyKept);

        var keepRatio = totalPTPs > 0 ? (decimal)keptPTPs / totalPTPs * 100 : 0;

        return new Dictionary<string, object>
        {
            ["TotalPTPs"] = totalPTPs,
            ["KeptPTPs"] = keptPTPs,
            ["BrokenPTPs"] = brokenPTPs,
            ["PartiallyKeptPTPs"] = partiallyKeptPTPs,
            ["KeepRatio"] = keepRatio
        };
    }

    public async Task<Dictionary<string, object>> GetPTPAnalyticsAsync(Guid caseId)
    {
        var ptps = await _context.PromisesToPay
            .Where(p => p.CollectionCaseId == caseId)
            .ToListAsync();

        var totalAmount = ptps.Sum(p => p.PromisedAmount);
        var keptAmount = ptps.Where(p => p.Status == PTPStatus.Kept).Sum(p => p.PromisedAmount);

        return new Dictionary<string, object>
        {
            ["TotalPTPs"] = ptps.Count,
            ["TotalPromisedAmount"] = totalAmount,
            ["KeptAmount"] = keptAmount,
            ["KeepRatio"] = totalAmount > 0 ? (keptAmount / totalAmount) * 100 : 0
        };
    }

    public async Task<decimal> CalculatePTPKeepRatioAsync(Guid customerId)
    {
        var ptps = await _context.PromisesToPay
            .Where(p => p.CustomerId == customerId)
            .ToListAsync();

        if (ptps.Count == 0) return 0;

        var keptCount = ptps.Count(p => p.Status == PTPStatus.Kept);
        return (decimal)keptCount / ptps.Count * 100;
    }

    // Helper Methods
    private async Task<string> GeneratePTPNumberAsync()
    {
        var date = DateTime.UtcNow;
        var prefix = $"PTP{date:yyyyMM}";
        var count = await _context.PromisesToPay
            .CountAsync(p => p.PTPNumber.StartsWith(prefix));

        return $"{prefix}{(count + 1):D6}";
    }
}
