namespace ClaudeCollectionApp.Models.Enums;

/// <summary>
/// Promise to Pay statuses
/// </summary>
public enum PTPStatus
{
    Active = 1,           // PTP is active, date not reached yet
    Kept = 2,             // Customer paid as promised
    PartiallyKept = 3,    // Customer paid partial amount
    Broken = 4,           // Customer did not pay
    Expired = 5,          // PTP date passed without payment
    Cancelled = 6         // PTP cancelled by agent
}
