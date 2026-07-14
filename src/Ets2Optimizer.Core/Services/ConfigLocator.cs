namespace Ets2Optimizer.Services;

public static class ConfigLocator
{
    private static readonly string[] GameFolderNames =
    {
        "Euro Truck Simulator 2",
        "American Truck Simulator"
    };

    public static string? FindConfigFile()
    {
        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        foreach (var gameFolder in GameFolderNames)
        {
            var candidate = Path.Combine(documents, gameFolder, "config.cfg");
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    public static string CreateBackup(string configPath)
    {
        var directory = Path.GetDirectoryName(configPath)!;
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupPath = Path.Combine(directory, $"config_backup_{timestamp}.cfg");

        File.Copy(configPath, backupPath, overwrite: false);
        return backupPath;
    }

    /// <summary>
    /// Elenca i backup disponibili per un config.cfg, più recente per primo.
    /// Il timestamp nel nome file rende l'ordine alfabetico inverso equivalente a quello cronologico.
    /// </summary>
    public static List<string> ListBackups(string configPath)
    {
        var directory = Path.GetDirectoryName(configPath)!;
        if (!Directory.Exists(directory)) return new List<string>();

        return Directory.GetFiles(directory, "config_backup_*.cfg")
            .OrderByDescending(f => f)
            .ToList();
    }

    /// <summary>
    /// Ripristina un backup sul config.cfg attivo, creando prima un backup dello stato
    /// corrente (con suffisso "_before_restore") così anche questa operazione è reversibile.
    /// </summary>
    public static string RestoreBackup(string configPath, string backupPath)
    {
        var directory = Path.GetDirectoryName(configPath)!;
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var preRestoreBackup = Path.Combine(directory, $"config_before_restore_{timestamp}.cfg");

        File.Copy(configPath, preRestoreBackup, overwrite: false);
        File.Copy(backupPath, configPath, overwrite: true);
        return preRestoreBackup;
    }
}
