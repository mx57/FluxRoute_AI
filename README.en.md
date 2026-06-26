<div align="center">

# BSDPI_AI

**Intelligent DPI-bypass strategy auto-selection system**

A Windows application that uses **Thompson Sampling** and **genetic evolution** to automatically find and switch optimal Deep Packet Inspection (DPI) bypass parameters.

</div>

---

## Features

- **AI Orchestrator** — automated cycle: site probing → scoring → strategy switching/evolution
- **Thompson Sampling (Bandit)** — Bayesian approach to strategy selection with exploration/exploitation balance
- **Genetic Evolution** — crossover and mutations of top strategies to generate new ones
- **Network Fingerprint** — per-network strategy binding (Ethernet, Wi-Fi, VPN)
- **Visual Chain Builder** — drag-and-drop canvas for constructing DPI bypass pipelines
- **Dual Engine** — supports **zapret** (winws.exe) and **ByeDPI** (ciadpi.exe), including hybrid mode
- **Telegram WS Proxy** — built-in WebSocket proxy for Telegram
- **Auto-Update** — engine updates from GitHub releases
- **Diagnostics** — file, process, and connectivity checks

## Architecture

```
BSDPI_AI.slnx
├── BSDPI_AI/              — WPF GUI (MVVM, 11 tabs)
├── BSDPI_AI.AI/           — AI subsystem: Thompson Sampling + genetic evolution
├── BSDPI_AI.Core/         — Core: DPI engines, orchestrator, ChainBuilder
├── BSDPI_AI.Updater/      — Auto-update engine/
└── BSDPI_AI.Core.Tests/   — Unit tests
```

**Dependency chain:** `BSDPI_AI` → `BSDPI_AI.AI` → `BSDPI_AI.Core` ← `BSDPI_AI.Updater`

## How the AI Works

```
1. SyncBuiltins()         — scan engine/*.bat → genome registry
2. PickAndApplyInitial()  — Thompson Sampling → select best strategy
3. LoopAsync()            — probe cycle every N minutes:
   ├─ Capture()              — network fingerprint
   ├─ ProbeCurrent()         — check sites via current strategy
   ├─ RecordBanditSuccess/Failure — update Alpha/Beta
   ├─ MaybeEvolve()          — evolve when enough probes collected
   └─ SwitchToAlternative()  — switch strategy after 2+ consecutive failures
```

## Build & Run

**Requirements:** .NET 10 SDK, Windows

```bash
dotnet restore BSDPI_AI.slnx
dotnet build BSDPI_AI.slnx
dotnet run --project BSDPI_AI
```

### Tests

```bash
dotnet test BSDPI_AI.slnx
```

### Publish Release

```bash
dotnet publish BSDPI_AI/BSDPI_AI.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

## GUI Tabs

| # | Tab | Description |
|---|-----|-------------|
| 0 | Main | Status, start/stop, domains |
| 1 | TG Proxy | Telegram WebSocket Proxy |
| 2 | Orchestrator | Classic orchestrator (rating) |
| 3 | AI | AI orchestrator, strategies, evolution |
| 4 | Update | Update engine/ |
| 5 | Diagnostics | File and process checks |
| 6 | Service | Game filter, ipset, zapret service |
| 7 | About | Application info |
| 8 | Logs | Application logs |
| 9 | ByeDPI | ByeDPI settings |
| 10 | WARP | Cloudflare Warp config generator |
| 11 | Chain Builder | Visual DPI bypass chain constructor |

## AI Settings

| Parameter | Default | Description |
|-----------|:-------:|-------------|
| `Enabled` | `false` | Enable AI orchestrator |
| `ExplorationRatePermil` | `100` | Exploration in ‰ (100‰ = 10%) |
| `MaxEvolvedStrategies` | `24` | Max evolved strategies |
| `EvolutionIntervalMinutes` | `60` | Evolution interval |
| `MinProbesBeforeEvolve` | `6` | Min probes before evolution |
| `KeepHistoryDays` | `14` | History retention (days) |
| `AutoDeleteBelowScore` | `60` | Auto-delete strategies below this threshold |

## License

This project is licensed under [GPL-3.0](LICENSE).

Based on [klondike0x/FluxRoute](https://github.com/klondike0x/FluxRoute) (GPLv3).

---

> **Disclaimer.** **BSDPI_AI** is educational and research software designed for studying network technologies. This software **is not** a tool for violating applicable laws. Use of this software must comply with the applicable laws of the user's jurisdiction. The author assumes no responsibility for any consequences arising from the use of this software.
