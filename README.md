# ETS2 Optimizer

Tool open source per ottimizzare automaticamente il `config.cfg` di Euro Truck Simulator 2 (e American Truck Simulator, stesso motore Prism3D) in base all'hardware rilevato sul PC dell'utente.

## Cosa fa

1. Rileva GPU (modello + VRAM), CPU (core/thread) e RAM disponibile tramite WMI.
2. Individua automaticamente il `config.cfg` di ETS2/ATS in `Documents\Euro Truck Simulator 2\config.cfg` (o ATS equivalente).
3. Crea **sempre** un backup con timestamp prima di modificare qualsiasi file.
4. Applica un profilo di ottimizzazione (Low / Mid / High) scelto in base all'hardware rilevato, regolando risoluzione interna, ombre, riflessi, densità traffico/vegetazione, LOD.
5. Mostra un report di cosa è stato cambiato, valore per valore.
6. Suggerisce comandi di avvio Steam sicuri (es. `-nointro`), da incollare manualmente in Steam → Proprietà → Opzioni di avvio.
7. Permette di **ripristinare un backup precedente** con un click (GUI) o con `--restore` (CLI), salvando comunque lo stato corrente prima di sovrascrivere.
8. Include una guida "Come si usa" integrata (mostrata al primo avvio e richiamabile in ogni momento).

## Nota importante: config.cfg e gioco aperto

ETS2/ATS tengono le impostazioni grafiche in memoria mentre giocate e **riscrivono `config.cfg` su disco quando chiudete il gioco**, sovrascrivendo qualsiasi modifica esterna fatta nel frattempo. Per questo il tool rileva se il gioco è in esecuzione (processo `eurotrucks2`/`amtrucks`) e blocca l'applicazione delle modifiche finché non lo chiudete.

## Cosa NON fa

- Non modifica impostazioni VR/OpenXR/OpenVR, multimonitor o stereo manuale (troppo rischioso senza sapere se l'utente le usa).
- Non aggiunge eccezioni antivirus, non modifica servizi di sistema, non tocca altri programmi.
- Non richiede privilegi di amministratore.
- Non modifica mai le opzioni di avvio di Steam: le suggerisce soltanto, sta all'utente incollarle.

## Download

I binari precompilati sono disponibili nella pagina [Releases](https://github.com/EnigmaStyle/ETS2-Optimizer/releases):

- **Ets2Optimizer.exe** — interfaccia grafica (consigliata per la maggior parte degli utenti)
- **Ets2Optimizer.Cli.exe** — versione da riga di comando, utile per script/automazione

Per ciascuna è disponibile una variante *standalone* (nessuna dipendenza esterna, file più pesante) e una *framework-dependent* (richiede [.NET 8 Runtime](https://dotnet.microsoft.com/download) già installato, file più leggero).

## Struttura del progetto

- `src/Ets2Optimizer.Core` — logica condivisa: rilevamento hardware, individuazione config.cfg, motore di regole
- `src/Ets2Optimizer` — interfaccia grafica (WPF)
- `src/Ets2Optimizer.Cli` — interfaccia a riga di comando

## Build

Richiede [.NET 8 SDK](https://dotnet.microsoft.com/download).

```
dotnet build
dotnet run --project src/Ets2Optimizer       # GUI
dotnet run --project src/Ets2Optimizer.Cli   # CLI
```

## Firma del codice

Questo progetto ha richiesto un certificato di firma del codice gratuito tramite [SignPath Foundation](https://signpath.org), il programma no-profit di SignPath per progetti open source. Se e quando la candidatura verrà approvata, le release verranno firmate automaticamente da SignPath tramite la pipeline in `.github/workflows/build.yml`.

## Licenza

MIT — vedi [LICENSE](LICENSE).
