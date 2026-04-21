namespace Arkanis.Overlay.External.Medrunner.Clients;

using Abstractions;
using Microsoft.Extensions.Options;
using Models;

internal sealed class MedrunnerClientApiClient(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<MedrunnerOptions> options,
    MedrunnerTokenManager tokenManager
) : MedrunnerApiClientBase(httpClientFactory, options, tokenManager), IMedrunnerClientApi
{
    protected override string Endpoint => "client";

    public Task<ApiResponse<Person>> GetSelfAsync(CancellationToken ct = default)
        => GetAsync<Person>(string.Empty, ct: ct);

    public Task<ApiResponse<PaginatedResponse<ClientHistory>>> GetHistoryAsync(int limit, string? paginationToken = null, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string> { ["limit"] = limit.ToString(System.Globalization.CultureInfo.InvariantCulture) };
        if (paginationToken is not null)
            queryParams["paginationToken"] = paginationToken;

        return GetAsync<PaginatedResponse<ClientHistory>>("/history", queryParams, ct);
    }

    public Task<ApiResponse<BlockedStatus>> GetBlockedStatusAsync(CancellationToken ct = default)
        => GetAsync<BlockedStatus>("/blockedStatus", ct: ct);

    public Task<ApiResponse<Person>> LinkClientAsync(string rsiHandle, CancellationToken ct = default)
        => PostAsync<Person>("/link", new { rsiHandle }, ct);
}
