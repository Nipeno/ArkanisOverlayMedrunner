namespace Arkanis.Overlay.External.Medrunner;

using System.Net;
using System.Security.Claims;
using Abstractions;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Overlay.Common.Errors;
using Overlay.Common.Extensions;
using Overlay.Common.Models;
using Overlay.Common.Services;

public class MedrunnerAuthenticator(IServiceProvider serviceProvider) : ExternalAuthenticator<MedrunnerAuthenticator.AuthenticationTask>
{
    public override ExternalAuthenticatorInfo AuthenticatorInfo
        => MedrunnerConstants.ProviderInfo;

    public override Result ValidateCredentials(AccountCredentials? serviceCredentials)
        => serviceCredentials switch
        {
            AccountApiTokenCredentials => Result.Ok(),
            null => Result.Ok(),
            _ => Result.Fail("Provided credentials are not valid Medrunner API token credentials."),
        };

    public override AuthenticationTask AuthenticateAsync(AccountCredentials credentials, CancellationToken cancellationToken)
        => ActivatorUtilities.CreateInstance<AuthenticationTask>(serviceProvider, credentials, cancellationToken);

    public class AuthenticationTask(
        MedrunnerTokenManager tokenManager,
        IMedrunnerClientApi clientApi,
        ILogger<AuthenticationTask> logger,
        AccountCredentials credentials,
        CancellationToken cancellationToken
    ) : AuthTaskBase(credentials, cancellationToken)
    {
        public override ExternalAuthenticatorInfo ProviderInfo
            => MedrunnerConstants.ProviderInfo;

        public Person? Person { get; private set; }

        protected override async Task<Result<ClaimsIdentity>> RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (Credentials is not AccountApiTokenCredentials tokenCredentials)
                    return Result.Fail("Provided credentials are not valid Medrunner API token credentials.");

                tokenManager.Configure(tokenCredentials.SecretToken);

                var response = await clientApi.GetSelfAsync(cancellationToken);

                if (!response.Success || response.Data is null)
                    return Result.Fail(new ExternalAccountError(response.ErrorMessage ?? "Could not authenticate with the provided token."));

                Person = response.Data;
                Identity = BuildClaimsIdentity(Person);

                return Result.Ok(Identity);
            }
            catch (MedrunnerApiException ex) when (ex.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                tokenManager.Configure(null);
                logger.LogWarning("Medrunner authentication failed: invalid token");
                return Result.Fail(new ExternalAccountUnauthorizedError("Provided token is not valid."));
            }
            catch (MedrunnerApiException ex) when (ex.StatusCode == (int)HttpStatusCode.NotFound)
            {
                tokenManager.Configure(null);
                logger.LogWarning("Medrunner authentication failed: account not found");
                return Result.Fail(new ExternalAccountNotFoundError("Account with the provided token could not be found."));
            }
            catch (Exception ex)
            {
                tokenManager.Configure(null);
                logger.LogError(ex, "Failed to authenticate with Medrunner");
                return ex.ToResult();
            }
        }

        private static ClaimsIdentity BuildClaimsIdentity(Person person)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, person.Id),
                new(ClaimTypes.Name, person.DiscordId),
            };

            if (person.RsiHandle is not null)
            {
                claims.Add(new Claim(AccountClaimTypes.DisplayName, person.RsiHandle));
            }

            if (person.IsStaff)
                claims.Add(new Claim(ClaimTypes.Role, "Staff"));

            // Client access is open to all non-bot Medrunner accounts
            if (person.PersonType != PersonType.Bot)
                claims.Add(new Claim(ClaimTypes.Role, "Client"));

            return new ClaimsIdentity(claims, MedrunnerConstants.ProviderInfo.ServiceId, AccountClaimTypes.DisplayName, ClaimTypes.Role);
        }
    }
}
