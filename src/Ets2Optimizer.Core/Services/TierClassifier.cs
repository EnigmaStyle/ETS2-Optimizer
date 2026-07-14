using Ets2Optimizer.Models;

namespace Ets2Optimizer.Services;

public static class TierClassifier
{
    public static PerformanceTier Classify(HardwareInfo hw)
    {
        if (hw.GpuVramGb < 3.0 || hw.TotalRamGb < 6.0 || hw.CpuCores <= 2)
        {
            return PerformanceTier.Low;
        }

        if (hw.GpuVramGb >= 6.0 && hw.TotalRamGb >= 12.0 && hw.CpuCores >= 6)
        {
            return PerformanceTier.High;
        }

        return PerformanceTier.Mid;
    }
}
