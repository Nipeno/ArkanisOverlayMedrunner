namespace Arkanis.Overlay.External.Medrunner.Abstractions;

using Models;

public interface IMedrunnerOrgSettingsApi
{
    Task<ApiResponse<PublicOrgSettings>> GetPublicSettingsAsync(CancellationToken ct = default);
}
