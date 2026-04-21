namespace Arkanis.Overlay.External.Medrunner.Clients;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Models;

internal abstract class MedrunnerApiClientBase(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<MedrunnerOptions> options,
    MedrunnerTokenManager tokenManager
)
{
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    protected abstract string Endpoint { get; }

    private string EndpointUrl => $"{options.CurrentValue.BaseUrl}/{Endpoint}";

    private async Task<HttpClient> CreateHttpClientAsync(CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient(GetType().Name);
        client.Timeout = options.CurrentValue.Timeout;

        var accessToken = await tokenManager.GetAccessTokenAsync(ct);
        if (accessToken is not null)
        {
            client.DefaultRequestHeaders.Authorization = new("Bearer", accessToken);
        }

        return client;
    }

    protected Task<ApiResponse<T>> GetAsync<T>(string path, Dictionary<string, string>? queryParams = null, CancellationToken ct = default)
        => MedrunnerResiliency.Pipeline.ExecuteAsync(async token =>
        {
            using var client = await CreateHttpClientAsync(token);
            using var response = await client.GetAsync(BuildUrl(path, queryParams), token);
            return await ReadResponseAsync<T>(response, token);
        }, ct).AsTask();

    protected Task<ApiResponse<T>> PostAsync<T>(string path, object? body = null, CancellationToken ct = default)
        => MedrunnerResiliency.Pipeline.ExecuteAsync(async token =>
        {
            using var client = await CreateHttpClientAsync(token);
            using var response = await client.PostAsJsonAsync(BuildUrl(path), body, JsonOptions, token);
            return await ReadResponseAsync<T>(response, token);
        }, ct).AsTask();

    protected Task<ApiResponse> PostAsync(string path, object? body = null, CancellationToken ct = default)
        => MedrunnerResiliency.Pipeline.ExecuteAsync(async token =>
        {
            using var client = await CreateHttpClientAsync(token);
            using var response = await client.PostAsJsonAsync(BuildUrl(path), body, JsonOptions, token);
            return await ReadResponseAsync(response, token);
        }, ct).AsTask();

    protected Task<ApiResponse> PatchAsync(string path, object? body = null, CancellationToken ct = default)
        => MedrunnerResiliency.Pipeline.ExecuteAsync(async token =>
        {
            using var client = await CreateHttpClientAsync(token);
            using var request = new HttpRequestMessage(HttpMethod.Patch, BuildUrl(path))
            {
                Content = JsonContent.Create(body, options: JsonOptions),
            };
            using var response = await client.SendAsync(request, token);
            return await ReadResponseAsync(response, token);
        }, ct).AsTask();

    protected Task<ApiResponse> DeleteAsync(string path, CancellationToken ct = default)
        => MedrunnerResiliency.Pipeline.ExecuteAsync(async token =>
        {
            using var client = await CreateHttpClientAsync(token);
            using var response = await client.DeleteAsync(BuildUrl(path), token);
            return await ReadResponseAsync(response, token);
        }, ct).AsTask();

    private string BuildUrl(string path, Dictionary<string, string>? queryParams = null)
    {
        var baseUrl = string.IsNullOrEmpty(path) ? EndpointUrl :
            path.StartsWith('/') ? $"{EndpointUrl}{path}" : $"{EndpointUrl}/{path}";

        if (queryParams is not { Count: > 0 })
        {
            return baseUrl;
        }

        var query = string.Join("&", queryParams.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        return $"{baseUrl}?{query}";
    }

    private static async Task<ApiResponse<T>> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new MedrunnerApiException("Unauthorized — token may be invalid or expired.", (int)response.StatusCode);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            var retryAfter = response.Headers.RetryAfter?.Delta;
            throw new MedrunnerApiException("Rate limited by Medrunner API.", (int)response.StatusCode, retryAfter: retryAfter);
        }

        if (!response.IsSuccessStatusCode)
        {
            string? errorMessage = null;
            try { errorMessage = await response.Content.ReadAsStringAsync(ct); } catch { /* swallow */ }
            throw new MedrunnerApiException(
                errorMessage ?? $"Medrunner API returned {(int)response.StatusCode}.",
                (int)response.StatusCode,
                errorMessage
            );
        }

        var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
        return new ApiResponse<T> { Success = true, Data = data };
    }

    private static async Task<ApiResponse> ReadResponseAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new MedrunnerApiException("Unauthorized — token may be invalid or expired.", (int)response.StatusCode);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            var retryAfter = response.Headers.RetryAfter?.Delta;
            throw new MedrunnerApiException("Rate limited by Medrunner API.", (int)response.StatusCode, retryAfter: retryAfter);
        }

        if (!response.IsSuccessStatusCode)
        {
            string? errorMessage = null;
            try { errorMessage = await response.Content.ReadAsStringAsync(ct); } catch { /* swallow */ }
            throw new MedrunnerApiException(
                errorMessage ?? $"Medrunner API returned {(int)response.StatusCode}.",
                (int)response.StatusCode,
                errorMessage
            );
        }

        return new ApiResponse { Success = true };
    }
}

internal record ApiResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}
