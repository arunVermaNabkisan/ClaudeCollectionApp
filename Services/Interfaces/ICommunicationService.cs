using ClaudeCollectionApp.Models.Entities;
using ClaudeCollectionApp.Models.Enums;

namespace ClaudeCollectionApp.Services.Interfaces;

/// <summary>
/// Service interface for Multi-Channel Communication
/// </summary>
public interface ICommunicationService
{
    // Voice/Call Operations
    Task<CustomerInteraction> InitiateCallAsync(Guid customerId, Guid caseId, string phoneNumber);
    Task<bool> LogCallOutcomeAsync(Guid interactionId, DispositionCode disposition,
        int durationSeconds, string? notes = null);

    // SMS Operations
    Task<CustomerInteraction> SendSMSAsync(Guid customerId, Guid? caseId, string phoneNumber,
        string message, string? templateName = null);
    Task<bool> SendBulkSMSAsync(List<Guid> customerIds, string message, string? templateName = null);

    // Email Operations
    Task<CustomerInteraction> SendEmailAsync(Guid customerId, Guid? caseId, string email,
        string subject, string body, string? templateName = null);
    Task<bool> SendBulkEmailAsync(List<Guid> customerIds, string subject, string body);

    // WhatsApp Operations
    Task<CustomerInteraction> SendWhatsAppMessageAsync(Guid customerId, Guid? caseId,
        string phoneNumber, string message, string? templateName = null);
    Task<bool> SendWhatsAppWithPaymentLinkAsync(Guid customerId, string phoneNumber, string paymentLinkUrl);

    // Interaction History
    Task<IEnumerable<CustomerInteraction>> GetCustomerInteractionsAsync(Guid customerId, int count = 50);
    Task<IEnumerable<CustomerInteraction>> GetCaseInteractionsAsync(Guid caseId);
    Task<CustomerInteraction?> GetLastInteractionAsync(Guid customerId, CommunicationChannel? channel = null);

    // Templates
    Task<string> GetTemplateContentAsync(string templateName, Dictionary<string, string> placeholders);
    Task<IEnumerable<string>> GetAvailableTemplatesAsync(CommunicationChannel channel);

    // Delivery Status
    Task<bool> UpdateDeliveryStatusAsync(Guid interactionId, string status, DateTime? timestamp = null);
    Task<Dictionary<string, int>> GetDeliveryStatusSummaryAsync(DateTime fromDate, DateTime toDate);

    // Communication Analytics
    Task<Dictionary<string, object>> GetChannelEffectivenessAsync(DateTime fromDate, DateTime toDate);
    Task<Dictionary<string, object>> GetContactSuccessRatesAsync(Guid? userId = null);
}
