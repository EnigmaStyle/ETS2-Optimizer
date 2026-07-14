using Ets2Optimizer.Models;

namespace Ets2Optimizer.Services;

/// <summary>
/// Opzioni di avvio Steam raccomandate dalla community e dalla documentazione SCS.
/// Deliberatamente conservative: niente flag di tuning memoria (-mm_max_*) o modalità
/// -safe/-gl come default, perché su hardware sbagliato peggiorano le cose invece di
/// aiutare. Vanno incollate manualmente in Steam (proprietà del gioco > Opzioni di
/// avvio): il tool non modifica mai la configurazione di Steam.
/// </summary>
public static class LaunchOptionsAdvisor
{
    public static string RecommendedFor(PerformanceTier tier)
    {
        // -nointro: salta i filmati introduttivi, nessun rischio, utile per tutti.
        // -force_high_perf_gpu: forza la GPU dedicata sui laptop con doppia scheda video.
        return "-nointro -force_high_perf_gpu";
    }

    public static readonly IReadOnlyList<(string Flag, string Reason)> Explanations = new List<(string, string)>
    {
        ("-nointro", "Salta i filmati introduttivi di Steam/SCS all'avvio, nessun impatto su grafica o stabilità."),
        ("-force_high_perf_gpu", "Sui portatili con GPU integrata + dedicata, forza l'uso della scheda dedicata invece di quella integrata."),
    };
}
