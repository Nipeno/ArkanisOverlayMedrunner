namespace Arkanis.Overlay.External.Medrunner.Abstractions;

using Models;

public interface IMedrunnerLocationCache
{
    Task<LocationDetail[]> GetLocationsAsync(CancellationToken ct = default);
    void Invalidate();
}
