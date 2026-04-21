namespace Arkanis.Overlay.Infrastructure.Services.External;

using System.Security.Claims;
using Common.Abstractions;
using Microsoft.Extensions.Logging;
using Overlay.External.Medrunner;
using Overlay.External.Medrunner.Models;

public class MedrunnerAccountContext(
    MedrunnerAuthenticator authenticator,
    IUserPreferencesManager userPreferences,
    ILogger<MedrunnerAccountContext> logger
) : ExternalAccountContext<MedrunnerAuthenticator.AuthenticationTask>(authenticator, userPreferences, logger)
{
    public Person? Person => CurrentAuthentication?.Person;

    public bool IsStaff => IsAuthenticated && Identity.HasClaim(ClaimTypes.Role, "Staff");
    public bool IsClient => IsAuthenticated && Identity.HasClaim(ClaimTypes.Role, "Client");
}
