using System.Diagnostics;
using FluxRoute.Core.Models;

namespace FluxRoute.Core.Services;

public sealed class SingBoxEngine : IDpiEngine
{
    public DpiEngineType EngineType => DpiEngineType.SingBox;
    public string DisplayName => "Sing-Box";
    public EngineStatus Status { get; private set; } = EngineStatus.Stopped;
    public EngineProcessInfo? ProcessInfo { get; private set; }

    private Process? _process;
    private readonly string _engineDir;
    private readonly object _gate = new();
    private bool _disposed;

    public event EventHandler<EngineStatus>? StatusChanged;

    public SingBoxEngine(string engineDir)
    {
        _engineDir = engineDir;
    }

    public async Task<bool> StartAsync(EngineProfile profile, CancellationToken ct = default)
    {
        lock (_gate)
        {
            if (Status == EngineStatus.Running || Status == EngineStatus.Starting)
                return true;
            Status = EngineStatus.Starting;
        }

        try
        {
            var singboxDir = Path.Combine(_engineDir, "sing-box");
            Directory.CreateDirectory(singboxDir);
            var executable = Path.Combine(singboxDir, "sing-box.exe");

            if (!File.Exists(executable))
            {
                Status = EngineStatus.Failed;
                NotifyStatus();
                return false;
            }

            // In a real scenario, we would generate or use a config file.
            // For now, we assume a config.json exists in the same directory or passed via profile.
            var configPath = profile.ExtraArgs.FirstOrDefault(x => x.EndsWith(".json"))
                             ?? Path.Combine(singboxDir, "config.json");

            if (!File.Exists(configPath))
            {
                // Create a very basic dummy config if missing,
                // but usually Sing-Box requires a valid setup.
                Status = EngineStatus.Failed;
                NotifyStatus();
                return false;
            }

            var psi = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = $"run -c \"{configPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = singboxDir,
            };

            _process = new Process { StartInfo = psi };
            _process.Exited += (_, _) =>
            {
                lock (_gate)
                {
                    Status = EngineStatus.Crashed;
                    _process = null;
                    ProcessInfo = null;
                }
                NotifyStatus();
            };
            _process.EnableRaisingEvents = true;

            if (!_process.Start())
            {
                Status = EngineStatus.Failed;
                NotifyStatus();
                return false;
            }

            ProcessInfo = new EngineProcessInfo(
                _process.Id, "sing-box.exe", EngineStatus.Running,
                DateTimeOffset.Now, null);

            Status = EngineStatus.Running;
            NotifyStatus();
            return true;
        }
        catch
        {
            Status = EngineStatus.Failed;
            NotifyStatus();
            return false;
        }
    }

    public Task<bool> StopAsync(CancellationToken ct = default)
    {
        lock (_gate)
        {
            TryKillProcess(_process);
            _process = null;
            Status = EngineStatus.Stopped;
            ProcessInfo = null;
        }
        NotifyStatus();
        return Task.FromResult(true);
    }

    public Task<EngineStatus> ProbeStatusAsync(CancellationToken ct = default)
    {
        lock (_gate)
        {
            if (_process is not null && _process.HasExited)
            {
                Status = EngineStatus.Crashed;
                _process = null;
                ProcessInfo = null;
            }
            return Task.FromResult(Status);
        }
    }

    private void NotifyStatus()
    {
        StatusChanged?.Invoke(this, Status);
    }

    private static void TryKillProcess(Process? process)
    {
        if (process is null) return;
        try
        {
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);
            process.WaitForExit(2000);
            process.Dispose();
        }
        catch { }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        StopAsync().GetAwaiter().GetResult();
    }
}
