namespace Arkanis.Overlay.External.Medrunner;

using Abstractions;
using Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Overlay.Common.Extensions;
using Overlay.Common.Services;
using Services;
using SignalR;

public static class DependencyInjection
{
    public static IServiceCollection AddMedrunnerApiClients(
        this IServiceCollection services,
        Func<IServiceProvider, IConfigureOptions<MedrunnerOptions>>? createOptions = null
    )
        => services
            .AddSingleton(createOptions ?? (_ => new ConfigureOptions<MedrunnerOptions>(_ => { })))
            .AddSingleton<MedrunnerTokenManager>()
            .AddSingleton<IMedrunnerClientApi, MedrunnerClientApiClient>()
            .AddSingleton<IMedrunnerEmergencyApi, MedrunnerEmergencyApiClient>()
            .AddSingleton<IMedrunnerChatApi, MedrunnerChatApiClient>()
            .AddSingleton<IMedrunnerOrgSettingsApi, MedrunnerOrgSettingsApiClient>()
            .AddSingleton<IMedrunnerStaffApi, MedrunnerStaffApiClient>()
            .AddSingleton<MedrunnerLocationCache>()
            .Alias<IMedrunnerLocationCache, MedrunnerLocationCache>();

    public static IServiceCollection AddMedrunnerSignalR(this IServiceCollection services)
        => services
            .AddSingleton<MedrunnerSignalRService>()
            .Alias<IMedrunnerSignalRService, MedrunnerSignalRService>()
            .AddHostedService(sp => sp.GetRequiredService<MedrunnerSignalRService>());

    public static IServiceCollection AddMedrunnerAuthenticatorServices(this IServiceCollection services)
        => services
            .AddSingleton<MedrunnerAuthenticator>()
            .Alias<ExternalAuthenticator, MedrunnerAuthenticator>();
}
