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
}
