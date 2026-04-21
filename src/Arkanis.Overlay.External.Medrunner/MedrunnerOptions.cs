namespace Arkanis.Overlay.External.Medrunner;

using System.ComponentModel.DataAnnotations;

public record MedrunnerOptions
{
    [Url]
    public string BaseUrl { get; set; } = MedrunnerConstants.ApiBaseUrl;

    public string SignalRHubUrl { get; set; } = MedrunnerConstants.SignalRHubUrl;

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(15);
}
