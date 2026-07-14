using Ets2Optimizer.Models;

namespace Ets2Optimizer.Services;

/// <summary>
/// Valori per Low/Mid/High derivati da test manuali su hardware reale (RTX 2060 Super,
/// Ryzen 5 3600, 16GB RAM) e dalla documentazione della community su Prism3D (ETS2/ATS).
/// Il motore di Prism3D è poco parallelizzato: anche su hardware potente, città affollate
/// restano CPU-bound, per questo i fattori LOD/traffico scalano su tutti i livelli.
/// </summary>
public static class RuleSet
{
    public static readonly IReadOnlyList<OptimizationRule> Rules = new List<OptimizationRule>
    {
        new("r_scale_x", "1.0", "1.0", "1.0",
            "Il supersampling (>1.0) renderizza a risoluzione più alta del monitor: costo enorme, beneficio visivo minimo."),
        new("r_scale_y", "1.0", "1.0", "1.0",
            "Vedi r_scale_x."),
        new("r_ssao", "0", "1", "2",
            "Ambient Occlusion: costoso su GPU con poca VRAM/potenza."),
        new("r_sunshafts", "0", "0", "1",
            "Raggi di luce volumetrici: effetto costoso, disattivato sotto la fascia alta."),
        new("r_cloud_shadows", "0", "1", "1",
            "Ombre delle nuvole: risparmio GPU su hardware debole."),
        new("r_interior_shadow", "0", "1", "1",
            "Ombre nell'abitacolo: poco visibili, costo non trascurabile su GPU deboli."),
        new("r_sun_shadow_texture_size", "1024", "2048", "2048",
            "Risoluzione texture ombre dinamiche."),
        new("r_sun_shadow_quality", "1", "1", "2",
            "Qualità ombre dinamiche."),
        new("r_mirror_view_distance", "60", "90", "120",
            "Distanza di rendering degli specchi: si vede raramente oltre i 60-90m in autostrada."),
        new("g_rain_reflection", "0", "0", "1",
            "Riflessi sulla pioggia sull'asfalto."),
        new("g_reflection", "0", "1", "1",
            "Riflessi ambientali generali."),
        new("g_reflection_scale", "0", "1", "2",
            "Risoluzione dei riflessi."),
        new("g_grass_density", "0", "1", "2",
            "Densità dell'erba lungo la strada."),
        new("g_veg_detail", "0", "1", "1",
            "Dettaglio della vegetazione."),
        new("g_lod_factor_pedestrian", "0.5", "0.75", "1.0",
            "Distanza a cui i pedoni passano a modelli a bassa qualità."),
        new("g_lod_factor_parked", "0.5", "0.75", "1.0",
            "Distanza a cui i veicoli parcheggiati passano a modelli a bassa qualità."),
        new("g_lod_factor_traffic", "0.5", "0.75", "1.0",
            "Distanza a cui il traffico passa a modelli a bassa qualità."),
        new("g_traffic", "0.5", "0.75", "1.0",
            "Densità del traffico simulato: il fattore con più impatto sulla CPU nelle città."),
        new("r_anisotropy_factor", "1", "4", "8",
            "Filtro anisotropico sulle texture: costo GPU moderato."),
        new("r_texture_detail", "0", "1", "1",
            "Livello di dettaglio delle texture: impatta VRAM, non solo FPS."),
    };
}
