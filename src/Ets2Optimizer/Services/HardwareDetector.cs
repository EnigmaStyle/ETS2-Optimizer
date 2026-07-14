using System.Management;
using Microsoft.Win32;
using Ets2Optimizer.Models;

namespace Ets2Optimizer.Services;

public static class HardwareDetector
{
    public static HardwareInfo Detect()
    {
        var (gpuName, gpuVram) = DetectGpu();
        var (cpuName, cores, logical) = DetectCpu();
        var totalRam = DetectTotalRamBytes();

        return new HardwareInfo(gpuName, gpuVram, cpuName, cores, logical, totalRam);
    }

    private static (string name, long vramBytes) DetectGpu()
    {
        string name = "Sconosciuta";
        long vram = 0;

        using var searcher = new ManagementObjectSearcher("SELECT Name, AdapterRAM FROM Win32_VideoController");
        foreach (var obj in searcher.Get())
        {
            var candidateName = obj["Name"]?.ToString();
            if (string.IsNullOrWhiteSpace(candidateName)) continue;

            // Salta adattatori virtuali/remoti (es. RDP, Basic Render Driver)
            if (candidateName.Contains("Remote", StringComparison.OrdinalIgnoreCase) ||
                candidateName.Contains("Basic Render", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            name = candidateName;
            vram = Convert.ToInt64(obj["AdapterRAM"] ?? 0L);
            break;
        }

        // WMI AdapterRAM è un campo a 32 bit: tronca a ~4GB su molte GPU moderne.
        // Se sospettiamo un troncamento, proviamo a leggere la VRAM reale dal registro.
        if (vram <= 0 || vram >= 4_200_000_000)
        {
            var registryVram = TryGetVramFromRegistry();
            if (registryVram > vram) vram = registryVram;
        }

        return (name, vram);
    }

    private static long TryGetVramFromRegistry()
    {
        const string basePath = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";
        try
        {
            using var classKey = Registry.LocalMachine.OpenSubKey(basePath);
            if (classKey is null) return 0;

            foreach (var subKeyName in classKey.GetSubKeyNames())
            {
                if (!subKeyName.StartsWith("00")) continue;
                using var subKey = classKey.OpenSubKey(subKeyName);
                var value = subKey?.GetValue("HardwareInformation.qwMemorySize");
                if (value is long qwMemorySize && qwMemorySize > 0)
                {
                    return qwMemorySize;
                }
            }
        }
        catch
        {
            // Chiave non accessibile o formato inatteso: si ripiega sul valore WMI.
        }

        return 0;
    }

    private static (string name, int cores, int logical) DetectCpu()
    {
        string name = "Sconosciuta";
        int cores = Environment.ProcessorCount;
        int logical = Environment.ProcessorCount;

        using var searcher = new ManagementObjectSearcher("SELECT Name, NumberOfCores, NumberOfLogicalProcessors FROM Win32_Processor");
        foreach (var obj in searcher.Get())
        {
            name = obj["Name"]?.ToString() ?? name;
            cores = Convert.ToInt32(obj["NumberOfCores"] ?? cores);
            logical = Convert.ToInt32(obj["NumberOfLogicalProcessors"] ?? logical);
            break;
        }

        return (name, cores, logical);
    }

    private static long DetectTotalRamBytes()
    {
        using var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
        foreach (var obj in searcher.Get())
        {
            // TotalVisibleMemorySize è espresso in KB
            var kb = Convert.ToInt64(obj["TotalVisibleMemorySize"] ?? 0L);
            return kb * 1024L;
        }

        return 0;
    }
}
