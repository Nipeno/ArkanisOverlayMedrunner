namespace Arkanis.Overlay.External.Medrunner.Abstractions;

using Models;

public interface IMedrunnerSignalRService
{
    bool IsConnected { get; }

    event EventHandler<Person> PersonUpdated;
    event EventHandler<Emergency> EmergencyCreated;
    event EventHandler<Emergency> EmergencyUpdated;
    event EventHandler<ChatMessage> ChatMessageCreated;
    event EventHandler<ChatMessage> ChatMessageUpdated;
    event EventHandler<PublicOrgSettings> OrgSettingsUpdated;
    event EventHandler<Deployment> DeploymentCreated;
    event EventHandler Connected;
    event EventHandler Disconnected;
}
