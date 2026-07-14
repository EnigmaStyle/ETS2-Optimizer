using System.IO;
using System.Windows;
using Ets2Optimizer.Services;

namespace Ets2Optimizer;

public partial class RestoreWindow : Window
{
    private readonly string _configPath;
    private readonly List<string> _backups;

    public string? StatusMessage { get; private set; }

    public RestoreWindow(string configPath)
    {
        InitializeComponent();
        _configPath = configPath;
        _backups = ConfigLocator.ListBackups(configPath);

        if (_backups.Count == 0)
        {
            BackupsList.Items.Add("Nessun backup trovato in questa cartella.");
            BackupsList.IsEnabled = false;
        }
        else
        {
            foreach (var backup in _backups)
            {
                BackupsList.Items.Add(DescribeBackup(backup));
            }
        }

        BackupsList.SelectionChanged += (_, _) => RestoreButton.IsEnabled = BackupsList.SelectedIndex >= 0 && _backups.Count > 0;
    }

    private static string DescribeBackup(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        var timestampPart = name.Replace("config_backup_", string.Empty);

        if (DateTime.TryParseExact(timestampPart, "yyyyMMdd_HHmmss", null,
                System.Globalization.DateTimeStyles.None, out var parsed))
        {
            return $"{parsed:dd/MM/yyyy HH:mm:ss}  —  {Path.GetFileName(path)}";
        }

        return Path.GetFileName(path);
    }

    private void RestoreButton_Click(object sender, RoutedEventArgs e)
    {
        if (BackupsList.SelectedIndex < 0 || BackupsList.SelectedIndex >= _backups.Count) return;

        var selectedBackup = _backups[BackupsList.SelectedIndex];

        if (GameProcessChecker.IsGameRunning())
        {
            MessageBox.Show(
                "Il gioco è aperto. Chiudilo prima di ripristinare un backup, altrimenti la modifica verrà persa alla chiusura.",
                "Gioco in esecuzione",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Ripristinare questo backup su:\n{_configPath}\n\nLo stato attuale verrà comunque salvato prima. Continuare?",
            "Conferma ripristino",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        var preRestoreBackup = ConfigLocator.RestoreBackup(_configPath, selectedBackup);
        StatusMessage = $"Backup ripristinato. Lo stato precedente è stato salvato in: {preRestoreBackup}";

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
