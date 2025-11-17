namespace ClaudeCollectionApp.Models.Enums;

/// <summary>
/// Delinquency buckets based on Days Past Due (DPD)
/// </summary>
public enum DelinquencyBucket
{
    Current = 0,          // 0 DPD - No delinquency
    Bucket1 = 1,          // 1-30 DPD
    Bucket2 = 2,          // 31-60 DPD
    Bucket3 = 3,          // 61-90 DPD
    Bucket4 = 4,          // 91-180 DPD
    Bucket5 = 5,          // 181-360 DPD
    Bucket6Plus = 6       // 360+ DPD
}
