namespace Arkanis.Overlay.External.Medrunner;

using Overlay.Common;
using Overlay.Common.Models;

public static class MedrunnerConstants
{
    public const string ApiBaseUrl = "https://api.medrunner.space";
    public const string SignalRHubUrl = "https://api.medrunner.space/hub/emergency";
    public const string WebBaseUrl = "https://medrunner.space";

    public static readonly ExternalAuthenticatorInfo ProviderInfo = new()
    {
        ServiceId = ExternalService.MedRunner,
        DisplayName = "Medrunner",
        Description =
            "Medrunner is a player-run emergency medical service for Star Citizen. "
            + "Connect your account to request rescue as a client or manage emergencies as staff.",
    };
}
