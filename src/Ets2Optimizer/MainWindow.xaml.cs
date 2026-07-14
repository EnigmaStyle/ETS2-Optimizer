using System.IO;
using System.Windows;
using System.Windows.Media;
using Ets2Optimizer.Models;
using Ets2Optimizer.Services;

namespace Ets2Optimizer;

public partial class MainWindow : Window
{
    private string? _configPath;
    private PerformanceTier _tier;
    private List<string>? _originalLines;
    private List<string>? _newLines;
    private List<ConfigChange> _pendingChanges = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            DetectHardwareAndConfig();
            ShowInstructionsOnFirstRun();
        };
    }

    private static void ShowInstructionsOnFirstRun()
    {
        var markerPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Ets2Optimizer", "first_run_done");

        if (File.Exists(markerPath)) return;

        Directory.CreateDirectory(Path.GetDirectoryName(markerPath)!);
        File.WriteAllText(markerPath, DateTime.Now.ToString("O"));

        new InstructionsWindow().ShowDialog();
    }

    private void DetectHardwareAndConfig()
    {
        var hardware = HardwareDetector.Detect();
        _tier = TierClassifier.Classify(hardware);

        GpuText.Text = $"GPU: {hardware.GpuName} ({hardware.GpuVramGb:0.#} GB VRAM)";
        CpuText.Text = $"CPU: {hardware.CpuName} ({hardware.CpuCores} core / {hardware.CpuLogicalProcessors} thread)";
        RamText.Text = $"RAM: {hardware.TotalRamGb:0.#} GB";

        TierText.Text = _tier.ToString();
        TierBadge.Background = _tier switch
        {
            PerformanceTier.Low => new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C)),
            PerformanceTier.Mid => new SolidColorBrush(Color.FromRgb(0xF1, 0xC4, 0x0F)),
            PerformanceTier.High => new SolidColorBrush(Color.FromRgb(0x2E, 0xCC, 0x71)),
            _ => TierBadge.Background
        };

        _configPath = ConfigLocator.FindConfigFile();
        ConfigPathText.Text = _configPath ?? "Nessun file config.cfg trovato. Avvia il gioco almeno una volta.";

        LaunchOptionsText.Text = LaunchOptionsAdvisor.RecommendedFor(_tier);

        GameRunningWarning.Visibility = GameProcessChecker.IsGameRunning() ? Visibility.Visible : Visibility.Collapsed;

        StatusText.Text = _configPath is null
            ? "Impossibile procedere: config.cfg non trovato."
            : "Premi \"Rileva ed Analizza\" per calcolare le modifiche proposte.";
    }

    private void CopyLaunchOptions_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(LaunchOptionsText.Text);
        StatusText.Text = "Comandi di avvio copiati negli appunti. Incollali in Steam → Proprietà → Opzioni di avvio.";
    }

    private void HelpButton_Click(object sender, RoutedEventArgs e)
    {
        new InstructionsWindow { Owner = this }.ShowDialog();
    }

    private void RestoreBackupButton_Click(object sender, RoutedEventArgs e)
    {
        if (_configPath is null) return;

        var window = new RestoreWindow(_configPath) { Owner = this };
        var restored = window.ShowDialog();

        if (restored == true)
        {
            StatusText.Text = window.StatusMessage;
            ApplyButton.IsEnabled = false;
            ChangesList.ItemsSource = null;
        }
    }

    private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_configPath is null)
        {
            DetectHardwareAndConfig();
            if (_configPath is null) return;
        }

        _originalLines = File.ReadAllLines(_configPath).ToList();
        var (newLines, changes) = ConfigOptimizer.Apply(_originalLines, _tier);
        _newLines = newLines;
        _pendingChanges = changes;

        ChangesList.ItemsSource = _pendingChanges;

        if (_pendingChanges.Count == 0)
        {
            StatusText.Text = "Il config.cfg è già in linea con il profilo consigliato: nessuna modifica necessaria.";
            ApplyButton.IsEnabled = false;
        }
        else
        {
            StatusText.Text = $"Trovate {_pendingChanges.Count} modifiche da applicare. Verrà creato un backup automatico prima di scrivere.";
            ApplyButton.IsEnabled = true;
        }
    }

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        if (_configPath is null || _newLines is null || _pendingChanges.Count == 0) return;

        if (GameProcessChecker.IsGameRunning())
        {
            MessageBox.Show(
                "Il gioco è aperto. ETS2 riscrive config.cfg quando lo chiudi, sovrascrivendo qualsiasi modifica fatta ora. Chiudi il gioco e riprova.",
                "Gioco in esecuzione",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            GameRunningWarning.Visibility = Visibility.Visible;
            return;
        }

        var result = MessageBox.Show(
            $"Verranno applicate {_pendingChanges.Count} modifiche a:\n{_configPath}\n\nVerrà creato un backup automatico prima di scrivere. Continuare?",
            "Conferma ottimizzazione",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        var backupPath = ConfigLocator.CreateBackup(_configPath);
        File.WriteAllLines(_configPath, _newLines);

        StatusText.Text = $"Fatto. Backup creato in: {backupPath}";
        ApplyButton.IsEnabled = false;
    }
}
