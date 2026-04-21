namespace Arkanis.Overlay.External.Medrunner.Clients;

using Abstractions;
using Microsoft.Extensions.Options;
using Models;

internal sealed class MedrunnerEmergencyApiClient(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<MedrunnerOptions> options,
    MedrunnerTokenManager tokenManager
) : MedrunnerApiClientBase(httpClientFactory, options, tokenManager), IMedrunnerEmergencyApi
{
    protected override string Endpoint => "emergency";

    public Task<ApiResponse<Emergency>> GetEmergencyAsync(string id, CancellationToken ct = default)
        => GetAsync<Emergency>($"/{id}", ct: ct);

    public Task<ApiResponse<Emergency[]>> GetEmergenciesAsync(string[] ids, CancellationToken ct = default)
        => PostAsync<Emergency[]>("/bulk", new { ids }, ct);

    public Task<ApiResponse<Emergency>> CreateEmergencyAsync(CreateEmergencyRequest request, CancellationToken ct = default)
        => PostAsync<Emergency>(string.Empty, request, ct);

    public Task<ApiResponse<LocationDetail[]>> GetLocationsAsync(CancellationToken ct = default)
        => GetAsync<LocationDetail[]>("/meta/locations", ct: ct);

    public async Task CancelEmergencyAsync(string id, CancellationReason reason, CancellationToken ct = default)
        => await PostAsync($"/{id}/cancel", new { reason = (int)reason }, ct);

    public async Task RateServicesAsync(string id, ResponseRating rating, string? remarks = null, CancellationToken ct = default)
        => await PostAsync($"/{id}/rate", new { rating = (int)rating, remarks }, ct);

    public Task<ApiResponse<TeamDetailsResponse>> GetTeamDetailsAsync(string id, CancellationToken ct = default)
        => GetAsync<TeamDetailsResponse>($"/{id}/teamDetails", ct: ct);
}
