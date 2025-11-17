using ClaudeCollectionApp.Models.Entities;

namespace ClaudeCollectionApp.Services.Interfaces;

/// <summary>
/// Service interface for Customer Management
/// </summary>
public interface ICustomerService
{
    // Customer operations
    Task<Customer?> GetCustomerByIdAsync(Guid customerId);
    Task<Customer?> GetCustomerByPhoneAsync(string phoneNumber);
    Task<Customer?> GetCustomerByEmailAsync(string email);
    Task<Customer?> GetCustomerByPANAsync(string panNumber);
    Task<(IEnumerable<Customer> Customers, int TotalCount)> SearchCustomersAsync(
        string searchTerm, int pageNumber, int pageSize);

    // Customer 360 View
    Task<Dictionary<string, object>> GetCustomer360ViewAsync(Guid customerId);

    // Contact Information
    Task<bool> UpdateContactInformationAsync(Guid customerId, string? phone = null,
        string? email = null, string? address = null);
    Task<bool> VerifyContactAsync(Guid customerId, string contactType, bool isVerified);

    // Alternate Contacts
    Task<AlternateContact> AddAlternateContactAsync(Guid customerId, AlternateContact contact);
    Task<IEnumerable<AlternateContact>> GetAlternateContactsAsync(Guid customerId);
    Task<bool> UpdateAlternateContactAsync(Guid contactId, AlternateContact contact);

    // Behavioral Scoring
    Task<bool> UpdateBehavioralScoreAsync(Guid customerId);
    Task<bool> UpdateProbabilityOfPaymentAsync(Guid customerId);

    // Customer Analytics
    Task<Dictionary<string, object>> GetCustomerBehaviorAnalyticsAsync(Guid customerId);
    Task<Dictionary<string, object>> GetCustomerPaymentPatternsAsync(Guid customerId);
}
