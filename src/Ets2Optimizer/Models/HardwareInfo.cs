namespace Ets2Optimizer.Models;

public sealed record HardwareInfo(
    string GpuName,
    long GpuVramBytes,
    string CpuName,
    int CpuCores,
    int CpuLogicalProcessors,
    long TotalRamBytes)
{
    public double GpuVramGb => GpuVramBytes / 1024.0 / 1024.0 / 1024.0;
    public double TotalRamGb => TotalRamBytes / 1024.0 / 1024.0 / 1024.0;
}
