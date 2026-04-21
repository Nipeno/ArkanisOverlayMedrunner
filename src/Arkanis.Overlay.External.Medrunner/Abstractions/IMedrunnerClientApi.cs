namespace Arkanis.Overlay.External.Medrunner.Abstractions;

using Models;

public interface IMedrunnerClientApi
{
    Task<ApiResponse<Person>> GetSelfAsync(CancellationToken ct = default);
    Task<ApiResponse<PaginatedResponse<ClientHistory>>> GetHistoryAsync(int limit, string? paginationToken = null, CancellationToken ct = default);
    Task<ApiResponse<BlockedStatus>> GetBlockedStatusAsync(CancellationToken ct = default);
    Task<ApiResponse<Person>> LinkClientAsync(string rsiHandle, CancellationToken ct = default);
}
