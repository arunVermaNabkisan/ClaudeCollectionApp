using ClaudeCollectionApp.Models.Entities;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Services.Interfaces;

/// <summary>
/// Service interface for Promise to Pay Management
/// </summary>
public interface IPromiseToPayService
{
    // PTP CRUD operations
    Task<PromiseToPay?> GetPTPByIdAsync(Guid ptpId);
    Task<PromiseToPay?> GetPTPByNumberAsync(string ptpNumber);
    Task<IEnumerable<PromiseToPay>> GetPTPsByCaseAsync(Guid caseId);
    Task<IEnumerable<PromiseToPay>> GetPTPsByCustomerAsync(Guid customerId);
    Task<(IEnumerable<PromiseToPay> PTPs, int TotalCount)> GetPTPsPagedAsync(
        int pageNumber, int pageSize, PTPStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null);

    // PTP Creation & Management
    Task<PromiseToPay> CreatePTPAsync(Guid caseId, decimal amount, DateTime promisedDate,
        PaymentMode paymentMode, int confidenceLevel);
    Task<IEnumerable<PromiseToPay>> CreateSplitPTPAsync(Guid caseId,
        List<(decimal Amount, DateTime Date, PaymentMode Mode)> splits, int confidenceLevel);
    Task<bool> CancelPTPAsync(Guid ptpId, string reason);

    // PTP Status Management
    Task<bool> MarkPTPAsKeptAsync(Guid ptpId, Guid paymentId);
    Task<bool> MarkPTPAsPartiallyKeptAsync(Guid ptpId, Guid paymentId, decimal paidAmount);
    Task<bool> MarkPTPAsBrokenAsync(Guid ptpId);
    Task ProcessExpiredPTPsAsync();

    // PTP Follow-ups
    Task<PTPFollowUp> CreatePTPFollowUpAsync(Guid ptpId, DateTime followUpDate, string channel);
    Task<IEnumerable<PTPFollowUp>> GetPendingFollowUpsAsync(Guid userId);
    Task<bool> CompletePTPFollowUpAsync(Guid followUpId, string notes);

    // PTP Reminders
    Task SendPTPRemindersAsync();
    Task<bool> SendPTPReminderAsync(Guid ptpId);

    // PTP Analytics
    Task<Dictionary<string, object>> GetPTPPerformanceAsync(Guid? userId = null);
    Task<Dictionary<string, object>> GetPTPAnalyticsAsync(Guid caseId);
    Task<decimal> CalculatePTPKeepRatioAsync(Guid customerId);
}
