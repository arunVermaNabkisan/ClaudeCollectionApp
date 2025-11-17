using ClaudeCollectionApp.Models.Entities;

namespace ClaudeCollectionApp.Services.Interfaces;

/// <summary>
/// Service interface for LMS (Loan Management System) Integration
/// </summary>
public interface ILMSIntegrationService
{
    // Batch Sync Operations (EOD/BOD)
    Task<bool> ExecuteEODSyncAsync();
    Task<bool> ExecuteBODSyncAsync();
    Task<(int Customers, int Loans, int Transactions)> SyncCustomerDataAsync();
    Task<(int Loans, int Updates)> SyncLoanAccountDataAsync();
    Task<int> SyncPaymentTransactionsAsync(DateTime fromDate, DateTime toDate);

    // Real-time API Operations
    Task<Dictionary<string, object>?> GetLoanAccountDetailsAsync(string lmsLoanId);
    Task<decimal> GetCurrentOutstandingAsync(string lmsLoanId);
    Task<IEnumerable<Dictionary<string, object>>> GetRecentTransactionsAsync(string lmsLoanId, int count = 10);
    Task<bool> PostPaymentToLMSAsync(Payment payment);
    Task<bool> UpdateContactInformationInLMSAsync(string lmsCustomerId, Customer customer);

    // Sync Status & Monitoring
    Task<Dictionary<string, object>> GetLastSyncStatusAsync();
    Task<bool> ValidateLMSConnectionAsync();
    Task<IEnumerable<string>> GetSyncErrorsAsync(DateTime fromDate, DateTime toDate);

    // Manual Sync Triggers
    Task<bool> SyncSpecificLoanAsync(string lmsLoanId);
    Task<bool> SyncSpecificCustomerAsync(string lmsCustomerId);
}
