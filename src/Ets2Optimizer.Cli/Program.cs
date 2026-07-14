using Ets2Optimizer.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== ETS2 Optimizer ===");
Console.WriteLine();

Console.WriteLine("Rilevamento hardware in corso...");
var hardware = HardwareDetector.Detect();

Console.WriteLine($"  GPU : {hardware.GpuName} ({hardware.GpuVramGb:0.#} GB VRAM)");
Console.WriteLine($"  CPU : {hardware.CpuName} ({hardware.CpuCores} core / {hardware.CpuLogicalProcessors} thread)");
Console.WriteLine($"  RAM : {hardware.TotalRamGb:0.#} GB");
Console.WriteLine();

var tier = TierClassifier.Classify(hardware);
Console.WriteLine($"Profilo scelto in base all'hardware: {tier}");
Console.WriteLine();

var configPath = ConfigLocator.FindConfigFile();
if (configPath is null)
{
    Console.WriteLine("Non ho trovato il config.cfg di Euro Truck Simulator 2 / American Truck Simulator.");
    Console.WriteLine("Verifica che il gioco sia stato avviato almeno una volta.");
    return 1;
}

Console.WriteLine($"File trovato: {configPath}");
Console.Write("Vuoi procedere con l'ottimizzazione? Verrà creato un backup automatico. [s/N] ");
var answer = Console.ReadLine();
if (!string.Equals(answer?.Trim(), "s", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("Operazione annullata, nessun file modificato.");
    return 0;
}

var originalLines = File.ReadAllLines(configPath).ToList();
var (newLines, changes) = ConfigOptimizer.Apply(originalLines, tier);

if (changes.Count == 0)
{
    Console.WriteLine();
    Console.WriteLine("Il config.cfg è già in linea con il profilo consigliato: nessuna modifica necessaria.");
    return 0;
}

var backupPath = ConfigLocator.CreateBackup(configPath);
Console.WriteLine($"Backup creato: {backupPath}");

File.WriteAllLines(configPath, newLines);

Console.WriteLine();
Console.WriteLine($"Applicate {changes.Count} modifiche:");
foreach (var change in changes)
{
    Console.WriteLine($"  {change.Key}: \"{change.OldValue}\" -> \"{change.NewValue}\"");
    Console.WriteLine($"    motivo: {change.Reason}");
}

Console.WriteLine();
Console.WriteLine("Fatto. Se qualcosa non va, ripristina il backup sovrascrivendo config.cfg.");
return 0;
