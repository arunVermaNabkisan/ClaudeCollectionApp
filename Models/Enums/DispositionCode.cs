namespace ClaudeCollectionApp.Models.Enums;

/// <summary>
/// Interaction disposition codes
/// </summary>
public enum DispositionCode
{
    // Successful Contact
    RPC_PromiseToPay = 1,
    RPC_PartialPayment = 2,
    RPC_FullPayment = 3,
    RPC_Dispute = 4,
    RPC_Hardship = 5,
    RPC_RefusedToPay = 6,
    RPC_RequestCallback = 7,

    // Unsuccessful Contact
    NoAnswer = 20,
    PhoneSwitchedOff = 21,
    InvalidNumber = 22,
    NumberBusy = 23,
    CallDropped = 24,
    LeftVoicemail = 25,

    // Field Visit
    CustomerMet = 40,
    CustomerNotAvailable = 41,
    AddressNotFound = 42,
    RelativeMet = 43,
    NeighborMet = 44,

    // Other
    DoNotDisturb = 60,
    RequestedLegal = 61,
    Deceased = 62,
    SkipTracing = 63
}
