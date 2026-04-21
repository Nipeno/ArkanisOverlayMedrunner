namespace Arkanis.Overlay.Domain.Models.Medrunner;

public record MedrunnerUserProfile
{
    public required string DiscordId { get; init; }
    public string? RsiHandle { get; init; }
    public bool IsStaff { get; init; }
    public bool IsClient { get; init; }
}
