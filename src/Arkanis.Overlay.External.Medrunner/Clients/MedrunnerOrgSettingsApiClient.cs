namespace Arkanis.Overlay.External.Medrunner.Clients;

using Abstractions;
using Microsoft.Extensions.Options;
using Models;

internal sealed class MedrunnerOrgSettingsApiClient(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<MedrunnerOptions> options,
    MedrunnerTokenManager tokenManager
) : MedrunnerApiClientBase(httpClientFactory, options, tokenManager), IMedrunnerOrgSettingsApi
{
    protected override string Endpoint => "orgSettings";

    public Task<ApiResponse<PublicOrgSettings>> GetPublicSettingsAsync(CancellationToken ct = default)
        => GetAsync<PublicOrgSettings>(string.Empty, ct: ct);
}
