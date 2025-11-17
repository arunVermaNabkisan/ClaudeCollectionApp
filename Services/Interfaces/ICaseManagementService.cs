using ClaudeCollectionApp.Models.Entities;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Services.Interfaces;

/// <summary>
/// Service interface for Collection Case Management
/// </summary>
public interface ICaseManagementService
{
    // Case CRUD operations
    Task<CollectionCase?> GetCaseByIdAsync(Guid caseId);
    Task<CollectionCase?> GetCaseByNumberAsync(string caseNumber);
    Task<CollectionCase?> GetCaseByLoanAccountAsync(Guid loanAccountId);
    Task<IEnumerable<CollectionCase>> GetCasesByCustomerAsync(Guid customerId);
    Task<(IEnumerable<CollectionCase> Cases, int TotalCount)> GetCasesPagedAsync(
        int pageNumber, int pageSize, string? searchTerm = null,
        DelinquencyBucket? bucket = null, CaseStatus? status = null,
        Guid? assignedToUserId = null);

    // Case Creation & Lifecycle
    Task<CollectionCase> CreateCaseAsync(Guid loanAccountId);
    Task<bool> UpdateCaseStatusAsync(Guid caseId, CaseStatus newStatus, string? reason = null);
    Task<bool> CloseCaseAsync(Guid caseId, string resolutionType);

    // Case Assignment
    Task<bool> AssignCaseAsync(Guid caseId, Guid toUserId, string assignmentType, string? reason = null);
    Task<bool> AssignCasesBulkAsync(IEnumerable<Guid> caseIds, Guid toUserId, string reason);
    Task<IEnumerable<CollectionCase>> GetMyCasesAsync(Guid userId);
    Task<IEnumerable<CollectionCase>> GetTeamCasesAsync(Guid teamLeaderId);

    // Delinquency Bucketing
    Task UpdateDelinquencyBucketsAsync();
    Task<bool> MoveCaseToBucketAsync(Guid caseId, DelinquencyBucket bucket);
    Task<Dictionary<DelinquencyBucket, int>> GetBucketDistributionAsync(Guid? userId = null);

    // Case Scoring & Priority
    Task<bool> UpdateCaseScoreAsync(Guid caseId);
    Task<bool> UpdateCasePriorityAsync(Guid caseId, int priority);
    Task RecalculateProbabilityOfPaymentAsync(Guid caseId);

    // Case History
    Task<IEnumerable<CaseStatusHistory>> GetCaseStatusHistoryAsync(Guid caseId);
    Task<IEnumerable<CaseAssignmentHistory>> GetCaseAssignmentHistoryAsync(Guid caseId);

    // Case Notes
    Task<CaseNote> AddCaseNoteAsync(Guid caseId, string noteText, string? noteType = null, bool isPinned = false);
    Task<IEnumerable<CaseNote>> GetCaseNotesAsync(Guid caseId);

    // Case Analytics
    Task<Dictionary<string, object>> GetCaseAnalyticsAsync(Guid caseId);
    Task<Dictionary<string, object>> GetPortfolioAnalyticsAsync(Guid? userId = null, string? vertical = null);
}
