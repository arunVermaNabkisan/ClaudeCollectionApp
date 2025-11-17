using ClaudeCollectionApp.Models.Entities;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Services.Interfaces;

/// <summary>
/// Service interface for Field Collection Management
/// </summary>
public interface IFieldCollectionService
{
    // Field Visit CRUD
    Task<FieldVisit?> GetFieldVisitByIdAsync(Guid visitId);
    Task<IEnumerable<FieldVisit>> GetVisitsByCaseAsync(Guid caseId);
    Task<IEnumerable<FieldVisit>> GetVisitsByAgentAsync(Guid agentId, DateTime? date = null);
    Task<(IEnumerable<FieldVisit> Visits, int TotalCount)> GetVisitsPagedAsync(
        int pageNumber, int pageSize, FieldVisitStatus? status = null, Guid? agentId = null);

    // Visit Planning & Assignment
    Task<FieldVisit> PlanFieldVisitAsync(Guid caseId, Guid agentId, DateTime plannedDate, string address);
    Task<bool> AssignFieldVisitAsync(Guid visitId, Guid agentId);
    Task<IEnumerable<FieldVisit>> GetDailyWorklistAsync(Guid agentId, DateTime date);
    Task<IEnumerable<FieldVisit>> OptimizeRouteAsync(Guid agentId, DateTime date);

    // Visit Execution
    Task<bool> CheckInVisitAsync(Guid visitId, decimal latitude, decimal longitude);
    Task<bool> CompleteVisitAsync(Guid visitId, DispositionCode disposition, string notes);
    Task<bool> RecordVisitOutcomeAsync(Guid visitId, bool customerMet, string? personMet = null);

    // Address Verification
    Task<bool> VerifyAddressAsync(Guid visitId, bool isCorrect, string? correctedAddress = null,
        decimal? latitude = null, decimal? longitude = null);
    Task<IEnumerable<FieldVisit>> GetPendingAddressVerificationsAsync();

    // Payment Collection
    Task<Payment> CollectPaymentInFieldAsync(Guid visitId, decimal amount, PaymentMode paymentMode);

    // Evidence Management
    Task<FieldVisitPhoto> UploadVisitPhotoAsync(Guid visitId, string fileName, byte[] fileData,
        string photoType, string? caption = null);
    Task<IEnumerable<FieldVisitPhoto>> GetVisitPhotosAsync(Guid visitId);
    Task<Document> UploadVisitDocumentAsync(Guid visitId, string fileName, byte[] fileData,
        DocumentType documentType);

    // Visit Rescheduling
    Task<bool> RescheduleVisitAsync(Guid visitId, DateTime newDate, string reason);
    Task<bool> CancelVisitAsync(Guid visitId, string reason);

    // Field Analytics
    Task<Dictionary<string, object>> GetFieldProductivityAsync(Guid agentId, DateTime fromDate, DateTime toDate);
    Task<Dictionary<string, object>> GetVisitSuccessRatesAsync(Guid? agentId = null);
    Task<decimal> CalculateAverageVisitDurationAsync(Guid agentId);
}
