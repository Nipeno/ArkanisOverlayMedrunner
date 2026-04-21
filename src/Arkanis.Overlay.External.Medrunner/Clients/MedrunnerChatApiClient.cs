namespace Arkanis.Overlay.External.Medrunner.Clients;

using Abstractions;
using Microsoft.Extensions.Options;
using Models;

internal sealed class MedrunnerChatApiClient(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<MedrunnerOptions> options,
    MedrunnerTokenManager tokenManager
) : MedrunnerApiClientBase(httpClientFactory, options, tokenManager), IMedrunnerChatApi
{
    protected override string Endpoint => "chatMessage";

    public Task<ApiResponse<ChatMessage>> GetMessageAsync(string id, CancellationToken ct = default)
        => GetAsync<ChatMessage>($"/{id}", ct: ct);

    public Task<ApiResponse<PaginatedResponse<ChatMessage>>> GetMessageHistoryAsync(string emergencyId, int limit, string? paginationToken = null, CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["emergencyId"] = emergencyId,
            ["limit"] = limit.ToString(System.Globalization.CultureInfo.InvariantCulture),
        };

        if (paginationToken is not null)
            queryParams["paginationToken"] = paginationToken;

        return GetAsync<PaginatedResponse<ChatMessage>>("/history", queryParams, ct);
    }

    public Task<ApiResponse<ChatMessage>> SendMessageAsync(string emergencyId, string contents, CancellationToken ct = default)
        => PostAsync<ChatMessage>(string.Empty, new { emergencyId, contents }, ct);

    public Task<ApiResponse<ChatMessage>> UpdateMessageAsync(string id, string contents, CancellationToken ct = default)
        => PatchAsync($"/{id}", new { contents }, ct)
            .ContinueWith(_ => new ApiResponse<ChatMessage> { Success = true }, ct);

    public async Task DeleteMessageAsync(string id, CancellationToken ct = default)
        => await DeleteAsync($"/{id}", ct);
}
