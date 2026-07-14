using Ets2Optimizer.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== ETS2 Optimizer ===");
Console.WriteLine();
Console.WriteLine("Come funziona, passo per passo:");
Console.WriteLine("  1. Chiudi Euro Truck Simulator 2 / American Truck Simulator, se aperto");
Console.WriteLine("     (il gioco riscrive config.cfg alla chiusura, sovrascrivendo le modifiche fatte ora).");
Console.WriteLine("  2. Il programma rileva GPU/CPU/RAM e calcola le modifiche proposte (nessuna scrittura ancora).");
Console.WriteLine("  3. Conferma quando richiesto: verrà creato prima un backup con data e ora, poi il file viene scritto.");
Console.WriteLine("  4. Avvia il gioco e controlla gli FPS. Se qualcosa non convince, chiudi il gioco e ripristina");
Console.WriteLine("     il backup rinominandolo in config.cfg (si trova nella stessa cartella).");
Console.WriteLine("  5. Puoi anche copiare i comandi di avvio Steam consigliati, mostrati alla fine.");
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

if (GameProcessChecker.IsGameRunning())
{
    Console.WriteLine();
    Console.WriteLine("Il gioco è aperto: ETS2 riscrive config.cfg quando lo chiudi, sovrascrivendo qualsiasi");
    Console.WriteLine("modifica fatta ora. Chiudi il gioco e riavvia questo strumento.");
    return 1;
}

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
Console.WriteLine();
Console.WriteLine("Suggerimento: puoi anche aggiungere questi comandi di avvio in Steam");
Console.WriteLine("(tasto destro su ETS2 > Proprietà > Opzioni di avvio):");
Console.WriteLine($"  {LaunchOptionsAdvisor.RecommendedFor(tier)}");
foreach (var (flag, reason) in LaunchOptionsAdvisor.Explanations)
{
    Console.WriteLine($"    {flag}: {reason}");
}
return 0;
