namespace ClaudeCollectionApp.Models.Enums;

/// <summary>
/// Loan product types
/// </summary>
public enum LoanProduct
{
    FPOLoan = 1,              // Farmer Producer Organization
    AVCFLoan = 2,             // Agriculture Value Chain Finance
    CorporateLoan = 3,        // Corporate loans
    AgriCorporateLoan = 4,    // Agri-corporate loans
    AgriStartupLoan = 5,      // Agri startup financing
    SecuredLoan = 6,          // Secured loans
    UnsecuredLoan = 7         // Unsecured loans
}
