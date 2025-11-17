namespace ClaudeCollectionApp.Models.Enums;

/// <summary>
/// Collection case lifecycle statuses
/// </summary>
public enum CaseStatus
{
    NewCase = 1,
    New = 1,
    InProgress = 2,
    Active = 2,
    PromiseToPay = 3,
    PartialRecovery = 4,
    FullRecovery = 5,
    Resolved = 5,
    LegalActionInitiated = 6,
    WrittenOff = 7,
    Closed = 8,
    OnHold = 9,
    Escalated = 10
}
