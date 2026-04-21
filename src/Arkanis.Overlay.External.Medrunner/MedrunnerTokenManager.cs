namespace Arkanis.Overlay.External.Medrunner;

using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Models;

public class MedrunnerTokenManager(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<MedrunnerOptions> options
) : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _refreshToken;
    private string? _accessToken;
    private DateTimeOffset _accessTokenExpiration = DateTimeOffset.MinValue;

    public event EventHandler? TokenChanged;

    public bool HasToken => _refreshToken is not null;

    public void Configure(string? refreshToken)
    {
        _refreshToken = refreshToken;
        _accessToken = null;
        _accessTokenExpiration = DateTimeOffset.MinValue;
        TokenChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken ct = default)
    {
        if (_refreshToken is null) return null;

        if (_accessToken is not null && DateTimeOffset.UtcNow < _accessTokenExpiration - TimeSpan.FromSeconds(30))
            return _accessToken;

        await _lock.WaitAsync(ct);
        try
        {
            if (_accessToken is not null && DateTimeOffset.UtcNow < _accessTokenExpiration - TimeSpan.FromSeconds(30))
                return _accessToken;

            return await ExchangeTokenAsync(ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<string?> ExchangeTokenAsync(CancellationToken ct)
    {
        var url = $"{options.CurrentValue.BaseUrl}/auth/exchange";
        using var client = httpClientFactory.CreateClient("MedrunnerAuth");
        using var response = await client.PostAsJsonAsync(url, new { refreshToken = _refreshToken }, ct);

        if (!response.IsSuccessStatusCode) return null;

        var grant = await response.Content.ReadFromJsonAsync<TokenGrant>(JsonOptions, ct);
        if (grant is null) return null;

        _accessToken = grant.AccessToken;
        _accessTokenExpiration = DateTimeOffset.Parse(grant.AccessTokenExpiration, CultureInfo.InvariantCulture);

        return _accessToken;
    }

    public void Dispose()
    {
        _lock.Dispose();
        GC.SuppressFinalize(this);
    }
}
