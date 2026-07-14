namespace Ets2Optimizer.Models;

public sealed record OptimizationRule(
    string Key,
    string LowValue,
    string MidValue,
    string HighValue,
    string Reason)
{
    public string ValueFor(PerformanceTier tier) => tier switch
    {
        PerformanceTier.Low => LowValue,
        PerformanceTier.Mid => MidValue,
        PerformanceTier.High => HighValue,
        _ => throw new ArgumentOutOfRangeException(nameof(tier))
    };
}
