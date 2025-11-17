using ClaudeCollectionApp.Models.Entities;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Services.Interfaces;

/// <summary>
/// Service interface for Payment Management
/// </summary>
public interface IPaymentService
{
    // Payment CRUD operations
    Task<Payment?> GetPaymentByIdAsync(Guid paymentId);
    Task<Payment?> GetPaymentByReferenceAsync(string referenceNumber);
    Task<IEnumerable<Payment>> GetPaymentsByLoanAccountAsync(Guid loanAccountId);
    Task<IEnumerable<Payment>> GetPaymentsByCustomerAsync(Guid customerId);
    Task<(IEnumerable<Payment> Payments, int TotalCount)> GetPaymentsPagedAsync(
        int pageNumber, int pageSize, PaymentStatus? status = null,
        DateTime? fromDate = null, DateTime? toDate = null);

    // Payment Processing
    Task<Payment> RecordPaymentAsync(Guid loanAccountId, decimal amount, PaymentMode paymentMode,
        Dictionary<string, string>? additionalDetails = null);
    Task<bool> UpdatePaymentStatusAsync(Guid paymentId, PaymentStatus status);
    Task<bool> ReversePaymentAsync(Guid paymentId, string reason);
    Task<bool> MarkPaymentAsBouncedAsync(Guid paymentId, string bounceReason);

    // Payment Links
    Task<PaymentLink> GeneratePaymentLinkAsync(Guid loanAccountId, decimal amount,
        TimeSpan validity, bool allowPartial = false);
    Task<PaymentLink?> GetPaymentLinkByIdAsync(string linkId);
    Task<bool> ProcessPaymentLinkPaymentAsync(string linkId, string transactionId);
    Task<IEnumerable<PaymentLink>> GetActivePaymentLinksAsync(Guid loanAccountId);

    // Payment Reconciliation
    Task<bool> ReconcilePaymentAsync(Guid paymentId);
    Task ReconcilePendingPaymentsAsync();
    Task<IEnumerable<Payment>> GetUnreconciledPaymentsAsync();

    // LMS Integration
    Task<bool> PostPaymentToLMSAsync(Guid paymentId);
    Task<bool> SyncPaymentStatusFromLMSAsync(string lmsTransactionId);

    // Payment Analytics
    Task<Dictionary<string, object>> GetPaymentAnalyticsAsync(Guid? userId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<decimal> GetCollectionEfficiencyIndexAsync(DateTime fromDate, DateTime toDate);
    Task<Dictionary<PaymentMode, decimal>> GetPaymentModeDistributionAsync(DateTime fromDate, DateTime toDate);
}
