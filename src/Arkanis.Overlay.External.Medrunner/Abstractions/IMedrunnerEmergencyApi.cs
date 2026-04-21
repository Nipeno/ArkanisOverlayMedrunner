namespace Arkanis.Overlay.External.Medrunner.Abstractions;

using Models;

public interface IMedrunnerEmergencyApi
{
    Task<ApiResponse<Emergency>> GetEmergencyAsync(string id, CancellationToken ct = default);
    Task<ApiResponse<Emergency[]>> GetEmergenciesAsync(string[] ids, CancellationToken ct = default);
    Task<ApiResponse<Emergency>> CreateEmergencyAsync(CreateEmergencyRequest request, CancellationToken ct = default);
    Task<ApiResponse<LocationDetail[]>> GetLocationsAsync(CancellationToken ct = default);
    Task CancelEmergencyAsync(string id, CancellationReason reason, CancellationToken ct = default);
    Task RateServicesAsync(string id, ResponseRating rating, string? remarks = null, CancellationToken ct = default);
    Task<ApiResponse<TeamDetailsResponse>> GetTeamDetailsAsync(string id, CancellationToken ct = default);
}
