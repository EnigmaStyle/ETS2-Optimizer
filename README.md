# ETS2 Optimizer

Tool open source per ottimizzare automaticamente il `config.cfg` di Euro Truck Simulator 2 (e American Truck Simulator, stesso motore Prism3D) in base all'hardware rilevato sul PC dell'utente.

## Cosa fa

1. Rileva GPU (modello + VRAM), CPU (core/thread) e RAM disponibile tramite WMI.
2. Individua automaticamente il `config.cfg` di ETS2/ATS in `Documents\Euro Truck Simulator 2\config.cfg` (o ATS equivalente).
3. Crea **sempre** un backup con timestamp prima di modificare qualsiasi file.
4. Applica un profilo di ottimizzazione (Low / Mid / High) scelto in base all'hardware rilevato, regolando risoluzione interna, ombre, riflessi, densità traffico/vegetazione, LOD.
5. Mostra un report di cosa è stato cambiato, valore per valore.

## Cosa NON fa

- Non modifica impostazioni VR/OpenXR/OpenVR, multimonitor o stereo manuale (troppo rischioso senza sapere se l'utente le usa).
- Non aggiunge eccezioni antivirus, non modifica servizi di sistema, non tocca altri programmi.
- Non richiede privilegi di amministratore.

## Download

I binari precompilati sono disponibili nella pagina [Releases](https://github.com/EnigmaStyle/ETS2-Optimizer/releases).

## Build

Richiede [.NET 8 SDK](https://dotnet.microsoft.com/download).

```
dotnet build
dotnet run --project src/Ets2Optimizer
```

## Firma del codice

Questo progetto ha richiesto un certificato di firma del codice gratuito tramite [SignPath Foundation](https://signpath.org), il programma no-profit di SignPath per progetti open source. Se e quando la candidatura verrà approvata, le release verranno firmate automaticamente da SignPath tramite la pipeline in `.github/workflows/build.yml`.

## Licenza

MIT — vedi [LICENSE](LICENSE).
