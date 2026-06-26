# AGENTS.md

## Build & Run

```bash
dotnet restore FluxRoute.slnx
dotnet build FluxRoute.slnx
dotnet run --project FluxRoute
```

- **Solution file:** `FluxRoute.slnx` (not `.sln`)
- **Target:** .NET 10 (`net10.0-windows`), Windows only
- **WPF app** — requires Windows, `UseWPF` + `UseWindowsForms` are both enabled

## Test

```bash
dotnet test FluxRoute.slnx
```

- Framework: xUnit (`FluxRoute.Core.Tests`)
- Tests reference `FluxRoute.Core`, `FluxRoute.AI`, and `FluxRoute.Updater`

## Release / Publish

```bash
dotnet publish FluxRoute/FluxRoute.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

- Version is set in `FluxRoute/FluxRoute.csproj` (`<Version>`)
- CI validates tag (`v*`) matches csproj version before building

## Project Structure

```
FluxRoute/              — WPF UI (Views, ViewModels, App.xaml, styles, Controls)
FluxRoute.Core/         — Orchestrator, connectivity checks, settings, ChainBuilder models
FluxRoute.AI/           — AI engine: Thompson Sampling bandit, genetic evolver
FluxRoute.Updater/      — Auto-update engine/ from GitHub releases
FluxRoute.Core.Tests/   — xUnit tests
engine/                 — Flowseal scripts (downloaded at runtime, not in repo)
```

**Dependency chain:** `FluxRoute` → `FluxRoute.AI` → `FluxRoute.Core` ← `FluxRoute.Updater`

## Key Libraries

- **CommunityToolkit.Mvvm** — MVVM helpers (ObservableObject, RelayCommand)
- **Serilog** — structured logging (file sink)
- **Microsoft.Extensions.Hosting / DI** — dependency injection in the WPF app
- **LiveChartsCore.SkiaSharpView.WPF** — charting
- **xUnit + coverlet** — testing

## Conventions

- Nullable enabled across all projects
- Implicit usings enabled
- Test namespace mirrors source: `FluxRoute.Core.Tests`
- `FluxRoute.Core.Tests` is the **only** test project
- `engine/` is **not** committed — downloaded at runtime
- The app requires **administrator privileges** at runtime

## Type Conflicts

The project uses both WPF and WinForms, causing ambiguous type references. Always add explicit aliases:
```csharp
using Point = System.Windows.Point;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
```

## GPL v3 Compliance

- `LICENSE` — full GPL v3 text
- `NOTICE` — upstream and third-party attributions
- `Directory.Build.props` — centralized license metadata
- `AppUpdaterService` — must point to `mx57/FluxRoute_AI`, not upstream
