using ClaudeCollectionApp.Data;
using ClaudeCollectionApp.Models.Entities;
using ClaudeCollectionApp.Models.Enums;
using ClaudeCollectionApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClaudeCollectionApp.Services.Implementations;

/// <summary>
/// Implementation of Payment Management Service
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Payment CRUD operations
    public async Task<Payment?> GetPaymentByIdAsync(Guid paymentId)
    {
        return await _context.Payments
            .Include(p => p.LoanAccount)
                .ThenInclude(l => l.Customer)
            .Include(p => p.CollectionCase)
            .FirstOrDefaultAsync(p => p.Id == paymentId);
    }

    public async Task<Payment?> GetPaymentByReferenceAsync(string referenceNumber)
    {
        return await _context.Payments
            .Include(p => p.LoanAccount)
            .FirstOrDefaultAsync(p => p.PaymentReferenceNumber == referenceNumber);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByLoanAccountAsync(Guid loanAccountId)
    {
        return await _context.Payments
            .Where(p => p.LoanAccountId == loanAccountId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByCustomerAsync(Guid customerId)
    {
        return await _context.Payments
            .Include(p => p.LoanAccount)
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Payment> Payments, int TotalCount)> GetPaymentsPagedAsync(
        int pageNumber, int pageSize, PaymentStatus? status = null,
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.Payments
            .Include(p => p.LoanAccount)
                .ThenInclude(l => l.Customer)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (fromDate.HasValue)
            query = query.Where(p => p.PaymentDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(p => p.PaymentDate <= toDate.Value);

        var totalCount = await query.CountAsync();
        var payments = await query
            .OrderByDescending(p => p.PaymentDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (payments, totalCount);
    }

    // Payment Processing
    public async Task<Payment> RecordPaymentAsync(Guid loanAccountId, decimal amount, PaymentMode paymentMode,
        Dictionary<string, string>? additionalDetails = null)
    {
        var loanAccount = await _context.LoanAccounts
            .Include(l => l.CollectionCase)
            .FirstOrDefaultAsync(l => l.Id == loanAccountId);

        if (loanAccount == null)
            throw new InvalidOperationException("Loan account not found");

        var referenceNumber = await GeneratePaymentReferenceAsync();

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            PaymentReferenceNumber = referenceNumber,
            LoanAccountId = loanAccountId,
            CustomerId = loanAccount.CustomerId,
            CollectionCaseId = loanAccount.CollectionCase?.Id,
            Amount = amount,
            PaymentMode = paymentMode,
            PaymentDate = DateTime.UtcNow,
            Status = PaymentStatus.Pending,
            IsReconciled = false,
            CreatedAt = DateTime.UtcNow
        };

        if (additionalDetails != null)
        {
            payment.TransactionId = additionalDetails.GetValueOrDefault("TransactionId");
            payment.BankReferenceNumber = additionalDetails.GetValueOrDefault("BankReferenceNumber");
            payment.UPIReferenceNumber = additionalDetails.GetValueOrDefault("UPIReferenceNumber");
            payment.ChequeNumber = additionalDetails.GetValueOrDefault("ChequeNumber");
            payment.ChequeDate = additionalDetails.ContainsKey("ChequeDate")
                ? DateTime.Parse(additionalDetails["ChequeDate"])
                : null;
        }

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Update loan account balances
        await UpdateLoanAccountBalancesAsync(loanAccountId, amount);

        return payment;
    }

    public async Task<bool> UpdatePaymentStatusAsync(Guid paymentId, PaymentStatus status)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null) return false;

        payment.Status = status;
        payment.UpdatedAt = DateTime.UtcNow;

        if (status == PaymentStatus.Success)
        {
            payment.SettlementDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReversePaymentAsync(Guid paymentId, string reason)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null) return false;

        payment.Status = PaymentStatus.Reversed;
        payment.ReversalReason = reason;
        payment.UpdatedAt = DateTime.UtcNow;

        // Reverse loan account balances
        await UpdateLoanAccountBalancesAsync(payment.LoanAccountId, -payment.Amount);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkPaymentAsBouncedAsync(Guid paymentId, string bounceReason)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null) return false;

        payment.Status = PaymentStatus.Bounced;
        payment.BounceReason = bounceReason;
        payment.UpdatedAt = DateTime.UtcNow;

        // Reverse loan account balances
        await UpdateLoanAccountBalancesAsync(payment.LoanAccountId, -payment.Amount);

        await _context.SaveChangesAsync();
        return true;
    }

    // Payment Links
    public async Task<PaymentLink> GeneratePaymentLinkAsync(Guid loanAccountId, decimal amount,
        TimeSpan validity, bool allowPartial = false)
    {
        var linkId = Guid.NewGuid().ToString("N");
        var expiresAt = DateTime.UtcNow.Add(validity);

        var paymentLink = new PaymentLink
        {
            Id = Guid.NewGuid(),
            LinkId = linkId,
            LoanAccountId = loanAccountId,
            Amount = amount,
            AllowPartialPayment = allowPartial,
            LinkUrl = $"https://payment.example.com/pay/{linkId}",
            ExpiresAt = expiresAt,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.PaymentLinks.Add(paymentLink);
        await _context.SaveChangesAsync();

        return paymentLink;
    }

    public async Task<PaymentLink?> GetPaymentLinkByIdAsync(string linkId)
    {
        return await _context.PaymentLinks
            .Include(pl => pl.LoanAccount)
                .ThenInclude(l => l.Customer)
            .FirstOrDefaultAsync(pl => pl.LinkId == linkId);
    }

    public async Task<bool> ProcessPaymentLinkPaymentAsync(string linkId, string transactionId)
    {
        var paymentLink = await _context.PaymentLinks
            .FirstOrDefaultAsync(pl => pl.LinkId == linkId && pl.IsActive);

        if (paymentLink == null || paymentLink.ExpiresAt < DateTime.UtcNow)
            return false;

        // Record the payment
        var payment = await RecordPaymentAsync(
            paymentLink.LoanAccountId,
            paymentLink.Amount,
            PaymentMode.PaymentLink,
            new Dictionary<string, string> { ["TransactionId"] = transactionId }
        );

        // Mark payment link as used
        paymentLink.IsActive = false;
        paymentLink.UsedAt = DateTime.UtcNow;
        paymentLink.LinkedPaymentId = payment.Id;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PaymentLink>> GetActivePaymentLinksAsync(Guid loanAccountId)
    {
        return await _context.PaymentLinks
            .Where(pl => pl.LoanAccountId == loanAccountId &&
                        pl.IsActive &&
                        pl.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(pl => pl.CreatedAt)
            .ToListAsync();
    }

    // Payment Reconciliation
    public async Task<bool> ReconcilePaymentAsync(Guid paymentId)
    {
        var payment = await _context.Payments.FindAsync(paymentId);
        if (payment == null) return false;

        payment.IsReconciled = true;
        payment.ReconciledAt = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task ReconcilePendingPaymentsAsync()
    {
        var pendingPayments = await _context.Payments
            .Where(p => !p.IsReconciled && p.Status == PaymentStatus.Success)
            .ToListAsync();

        foreach (var payment in pendingPayments)
        {
            // TODO: Implement actual reconciliation logic with bank statements
            await ReconcilePaymentAsync(payment.Id);
        }
    }

    public async Task<IEnumerable<Payment>> GetUnreconciledPaymentsAsync()
    {
        return await _context.Payments
            .Include(p => p.LoanAccount)
                .ThenInclude(l => l.Customer)
            .Where(p => !p.IsReconciled)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    // LMS Integration
    public async Task<bool> PostPaymentToLMSAsync(Guid paymentId)
    {
        var payment = await _context.Payments
            .Include(p => p.LoanAccount)
            .FirstOrDefaultAsync(p => p.Id == paymentId);

        if (payment == null) return false;

        // TODO: Implement actual LMS API integration
        payment.PostedToLMSAt = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SyncPaymentStatusFromLMSAsync(string lmsTransactionId)
    {
        // TODO: Implement actual LMS API integration to fetch payment status
        return await Task.FromResult(true);
    }

    // Payment Analytics
    public async Task<Dictionary<string, object>> GetPaymentAnalyticsAsync(Guid? userId = null,
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.Payments.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(p => p.PaymentDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(p => p.PaymentDate <= toDate.Value);

        var totalPayments = await query.CountAsync();
        var totalAmount = await query.Where(p => p.Status == PaymentStatus.Success).SumAsync(p => p.Amount);
        var successfulPayments = await query.CountAsync(p => p.Status == PaymentStatus.Success);
        var bouncedPayments = await query.CountAsync(p => p.Status == PaymentStatus.Bounced);

        return new Dictionary<string, object>
        {
            ["TotalPayments"] = totalPayments,
            ["TotalAmount"] = totalAmount,
            ["SuccessfulPayments"] = successfulPayments,
            ["BouncedPayments"] = bouncedPayments,
            ["SuccessRate"] = totalPayments > 0 ? (decimal)successfulPayments / totalPayments * 100 : 0
        };
    }

    public async Task<decimal> GetCollectionEfficiencyIndexAsync(DateTime fromDate, DateTime toDate)
    {
        var totalDemand = await _context.CollectionCases
            .Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate)
            .SumAsync(c => c.TotalOutstandingAmount);

        var totalCollected = await _context.Payments
            .Where(p => p.PaymentDate >= fromDate && p.PaymentDate <= toDate &&
                       p.Status == PaymentStatus.Success)
            .SumAsync(p => p.Amount);

        return totalDemand > 0 ? (totalCollected / totalDemand) * 100 : 0;
    }

    public async Task<Dictionary<PaymentMode, decimal>> GetPaymentModeDistributionAsync(DateTime fromDate, DateTime toDate)
    {
        var distribution = await _context.Payments
            .Where(p => p.PaymentDate >= fromDate && p.PaymentDate <= toDate &&
                       p.Status == PaymentStatus.Success)
            .GroupBy(p => p.PaymentMode)
            .Select(g => new { Mode = g.Key, Amount = g.Sum(p => p.Amount) })
            .ToListAsync();

        return distribution.ToDictionary(x => x.Mode, x => x.Amount);
    }

    // Helper Methods
    private async Task<string> GeneratePaymentReferenceAsync()
    {
        var date = DateTime.UtcNow;
        var prefix = $"PAY{date:yyyyMMdd}";
        var count = await _context.Payments
            .CountAsync(p => p.PaymentReferenceNumber.StartsWith(prefix));

        return $"{prefix}{(count + 1):D6}";
    }

    private async Task UpdateLoanAccountBalancesAsync(Guid loanAccountId, decimal amount)
    {
        var loanAccount = await _context.LoanAccounts.FindAsync(loanAccountId);
        if (loanAccount == null) return;

        // Simple allocation: interest first, then penalty, then principal
        var remainingAmount = amount;

        if (loanAccount.InterestOutstanding > 0 && remainingAmount > 0)
        {
            var interestPayment = Math.Min(loanAccount.InterestOutstanding, remainingAmount);
            loanAccount.InterestOutstanding -= interestPayment;
            remainingAmount -= interestPayment;
        }

        if (loanAccount.PenaltyOutstanding > 0 && remainingAmount > 0)
        {
            var penaltyPayment = Math.Min(loanAccount.PenaltyOutstanding, remainingAmount);
            loanAccount.PenaltyOutstanding -= penaltyPayment;
            remainingAmount -= penaltyPayment;
        }

        if (loanAccount.PrincipalOutstanding > 0 && remainingAmount > 0)
        {
            var principalPayment = Math.Min(loanAccount.PrincipalOutstanding, remainingAmount);
            loanAccount.PrincipalOutstanding -= principalPayment;
            remainingAmount -= principalPayment;
        }

        loanAccount.TotalOutstanding = loanAccount.PrincipalOutstanding +
                                       loanAccount.InterestOutstanding +
                                       loanAccount.PenaltyOutstanding;

        loanAccount.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
