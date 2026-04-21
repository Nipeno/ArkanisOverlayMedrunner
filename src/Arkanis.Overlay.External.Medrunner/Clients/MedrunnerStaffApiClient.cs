namespace Arkanis.Overlay.External.Medrunner.Clients;

using Abstractions;
using Microsoft.Extensions.Options;
using Models;

internal sealed class MedrunnerStaffApiClient(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<MedrunnerOptions> options,
    MedrunnerTokenManager tokenManager
) : MedrunnerApiClientBase(httpClientFactory, options, tokenManager), IMedrunnerStaffApi
{
    protected override string Endpoint => "staff";

    public Task<ApiResponse<MedalInformation[]>> GetMedalsInformationAsync(CancellationToken ct = default)
        => GetAsync<MedalInformation[]>("/medals", ct: ct);
}
