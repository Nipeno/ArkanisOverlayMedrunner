namespace Arkanis.Overlay.External.Medrunner.Abstractions;

using Models;

public interface IMedrunnerStaffApi
{
    Task<ApiResponse<MedalInformation[]>> GetMedalsInformationAsync(CancellationToken ct = default);
}
