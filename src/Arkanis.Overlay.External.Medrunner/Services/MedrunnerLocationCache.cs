namespace Arkanis.Overlay.External.Medrunner.Services;

using Abstractions;
using Models;

internal sealed class MedrunnerLocationCache(IMedrunnerEmergencyApi emergencyApi) : IMedrunnerLocationCache, IDisposable
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private LocationDetail[]? _cache;

    public async Task<LocationDetail[]> GetLocationsAsync(CancellationToken ct = default)
    {
        if (_cache is not null)
            return _cache;

        await _lock.WaitAsync(ct);
        try
        {
            if (_cache is not null)
                return _cache;

            var response = await emergencyApi.GetLocationsAsync(ct);
            _cache = response.Data ?? [];
            return _cache;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Invalidate() => _cache = null;

    public void Dispose() => _lock.Dispose();
}
