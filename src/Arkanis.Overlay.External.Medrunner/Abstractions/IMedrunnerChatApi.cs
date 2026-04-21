namespace Arkanis.Overlay.External.Medrunner.Abstractions;

using Models;

public interface IMedrunnerChatApi
{
    Task<ApiResponse<ChatMessage>> GetMessageAsync(string id, CancellationToken ct = default);
    Task<ApiResponse<PaginatedResponse<ChatMessage>>> GetMessageHistoryAsync(string emergencyId, int limit, string? paginationToken = null, CancellationToken ct = default);
    Task<ApiResponse<ChatMessage>> SendMessageAsync(string emergencyId, string contents, CancellationToken ct = default);
    Task<ApiResponse<ChatMessage>> UpdateMessageAsync(string id, string contents, CancellationToken ct = default);
    Task DeleteMessageAsync(string id, CancellationToken ct = default);
}
