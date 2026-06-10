using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using FluxRoute.AI.Models;
using FluxRoute.Core.Models;

namespace FluxRoute.AI.Services;

public sealed class CloudSyncService
{
    private const string ApiBaseUrl = "https://api.fluxroute.ai/v1"; // Placeholder
    private readonly HttpClient _http;
    private readonly AiStrategyRegistry _registry;

    public CloudSyncService(HttpClient http, AiStrategyRegistry registry)
    {
        _http = http;
        _registry = registry;
    }

    public async Task<bool> ShareGenomeAsync(StrategyGenome genome, CancellationToken ct = default)
    {
        try
        {
            // Anonymous sharing: remove local paths and identifiers
            var anonymized = new StrategyGenome
            {
                Id = Guid.NewGuid(),
                EngineType = genome.EngineType,
                FilterTcp = genome.FilterTcp,
                FilterUdp = genome.FilterUdp,
                DesyncMode = genome.DesyncMode,
                SplitPos = genome.SplitPos,
                SplitPosSemantic = genome.SplitPosSemantic,
                FakeTlsMod = genome.FakeTlsMod,
                FakeTtl = genome.FakeTtl,
                RepeatCount = genome.RepeatCount,
                DesyncAnyProtocol = genome.DesyncAnyProtocol,
                DesyncFooling = genome.DesyncFooling,
                FakeResend = genome.FakeResend,
                DisplayName = "Community Strategy",
                Origin = StrategyOrigin.Evolved
            };

            var response = await _http.PostAsJsonAsync($"{ApiBaseUrl}/share", anonymized, ct);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<StrategyGenome>> FetchTopGenomesAsync(DpiEngineType type, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.GetAsync($"{ApiBaseUrl}/top?engine={type}", ct);
            if (!response.IsSuccessStatusCode) return [];

            var list = await response.Content.ReadFromJsonAsync<List<StrategyGenome>>(ct);
            return list ?? [];
        }
        catch { return []; }
    }
}
