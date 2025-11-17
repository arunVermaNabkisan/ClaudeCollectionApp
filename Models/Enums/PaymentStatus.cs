namespace ClaudeCollectionApp.Models.Enums;

/// <summary>
/// Payment transaction statuses
/// </summary>
public enum PaymentStatus
{
    Initiated = 1,
    Pending = 2,
    Success = 3,
    Failed = 4,
    Reversed = 5,
    Refunded = 6,
    Bounced = 7,
    UnderReconciliation = 8,
    Reconciled = 9
}
