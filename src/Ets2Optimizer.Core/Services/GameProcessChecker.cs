using System.Diagnostics;

namespace Ets2Optimizer.Services;

public static class GameProcessChecker
{
    private static readonly string[] ProcessNames = { "eurotrucks2", "amtrucks" };

    /// <summary>
    /// ETS2/ATS scrivono il config.cfg in memoria e lo salvano su disco solo alla chiusura,
    /// sovrascrivendo qualunque modifica esterna fatta nel frattempo. Va quindi verificato
    /// che il gioco non sia in esecuzione prima di scrivere il file.
    /// </summary>
    public static bool IsGameRunning()
    {
        return ProcessNames.Any(name => Process.GetProcessesByName(name).Length > 0);
    }
}
