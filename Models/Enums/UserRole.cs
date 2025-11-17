namespace ClaudeCollectionApp.Models.Enums;

/// <summary>
/// User roles in the collection management system
/// </summary>
public enum UserRole
{
    RelationshipManager = 1,      // BDM/AM/Credit Manager
    ExternalRecoveryAgent = 2,     // External agency executive
    TeamLeader = 3,                // Sr BDM / Supervisor
    VerticalHead = 4,              // FPO/AVCF/Corporate vertical head
    SeniorManagement = 5,          // CXO level
    SystemAdmin = 6                // System administrator
}
