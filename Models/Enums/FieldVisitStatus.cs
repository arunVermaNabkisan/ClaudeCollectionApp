namespace ClaudeCollectionApp.Models.Enums;

/// <summary>
/// Field visit statuses
/// </summary>
public enum FieldVisitStatus
{
    Planned = 1,
    InProgress = 2,
    Completed = 3,
    CustomerNotFound = 4,
    AddressIncorrect = 5,
    Rescheduled = 6,
    Cancelled = 7
}
