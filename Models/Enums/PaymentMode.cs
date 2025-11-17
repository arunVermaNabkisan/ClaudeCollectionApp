namespace ClaudeCollectionApp.Models.Enums;

/// <summary>
/// Payment modes/methods
/// </summary>
public enum PaymentMode
{
    Cash = 1,
    Cheque = 2,
    UPI = 3,
    NEFT = 4,
    RTGS = 5,
    IMPS = 6,
    DebitCard = 7,
    CreditCard = 8,
    NetBanking = 9,
    PaymentGateway = 10,
    PaymentLink = 11,
    AutoDebit = 12,
    PDC = 13              // Post-Dated Cheque
}
