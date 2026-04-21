namespace Arkanis.Overlay.External.Medrunner.SignalR;

using System.Text.Json;
using Abstractions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;

public class MedrunnerSignalRService : IHostedService, IMedrunnerSignalRService, IAsyncDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    private readonly MedrunnerTokenManager _tokenManager;
    private readonly IOptionsMonitor<MedrunnerOptions> _options;
    private readonly ILogger<MedrunnerSignalRService> _logger;
    private HubConnection? _connection;
    private CancellationToken _appStopping;

    public event EventHandler<Person>? PersonUpdated;
    public event EventHandler<Emergency>? EmergencyCreated;
    public event EventHandler<Emergency>? EmergencyUpdated;
    public event EventHandler<ChatMessage>? ChatMessageCreated;
    public event EventHandler<ChatMessage>? ChatMessageUpdated;
    public event EventHandler<PublicOrgSettings>? OrgSettingsUpdated;
    public event EventHandler<Deployment>? DeploymentCreated;
    public event EventHandler? Connected;
    public event EventHandler? Disconnected;

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    public MedrunnerSignalRService(
        MedrunnerTokenManager tokenManager,
        IOptionsMonitor<MedrunnerOptions> options,
        ILogger<MedrunnerSignalRService> logger
    )
    {
        _tokenManager = tokenManager;
        _options = options;
        _logger = logger;

        _tokenManager.TokenChanged += OnTokenChanged;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _appStopping = cancellationToken;

        if (_tokenManager.HasToken)
            await ConnectAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _tokenManager.TokenChanged -= OnTokenChanged;

        if (_connection is not null)
            await _connection.StopAsync(cancellationToken);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "AsyncVoidMethod")]
    private async void OnTokenChanged(object? _, EventArgs __)
    {
        try
        {
            if (_connection is not null)
            {
                await _connection.StopAsync(_appStopping);
                await _connection.DisposeAsync();
                _connection = null;
            }

            if (_tokenManager.HasToken)
                await ConnectAsync(_appStopping);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Medrunner token change");
        }
    }

    private async Task ConnectAsync(CancellationToken ct)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(_options.CurrentValue.SignalRHubUrl, opts =>
            {
                opts.AccessTokenProvider = () => _tokenManager.GetAccessTokenAsync(CancellationToken.None)!;
            })
            .WithAutomaticReconnect(new RetryPolicy())
            .AddJsonProtocol(opts =>
            {
                opts.PayloadSerializerOptions.PropertyNamingPolicy = JsonOptions.PropertyNamingPolicy;
                opts.PayloadSerializerOptions.PropertyNameCaseInsensitive = JsonOptions.PropertyNameCaseInsensitive;
            })
            .Build();

        RegisterHandlers(_connection);

        _connection.Reconnected += _ =>
        {
            _logger.LogInformation("Reconnected to Medrunner SignalR hub");
            Connected?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        };

        _connection.Closed += ex =>
        {
            _logger.LogWarning(ex, "Medrunner SignalR connection closed");
            Disconnected?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        };

        try
        {
            await _connection.StartAsync(ct);
            _logger.LogInformation("Connected to Medrunner SignalR hub");
            Connected?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to Medrunner SignalR hub");
        }
    }

    private void RegisterHandlers(HubConnection connection)
    {
        connection.On<Person>("PersonUpdate", person =>
        {
            _logger.LogDebug("Medrunner: PersonUpdate received");
            PersonUpdated?.Invoke(this, person);
        });

        connection.On<Emergency>("EmergencyCreate", emergency =>
        {
            _logger.LogDebug("Medrunner: EmergencyCreate received for {EmergencyId}", emergency.Id);
            EmergencyCreated?.Invoke(this, emergency);
        });

        connection.On<Emergency>("EmergencyUpdate", emergency =>
        {
            _logger.LogDebug("Medrunner: EmergencyUpdate received for {EmergencyId} — status {Status}", emergency.Id, emergency.Status);
            EmergencyUpdated?.Invoke(this, emergency);
        });

        connection.On<ChatMessage>("ChatMessageCreate", message =>
        {
            _logger.LogDebug("Medrunner: ChatMessageCreate received");
            ChatMessageCreated?.Invoke(this, message);
        });

        connection.On<ChatMessage>("ChatMessageUpdate", message =>
        {
            _logger.LogDebug("Medrunner: ChatMessageUpdate received");
            ChatMessageUpdated?.Invoke(this, message);
        });

        connection.On<OrgSettings>("OrgSettingsUpdate", settings =>
        {
            _logger.LogDebug("Medrunner: OrgSettingsUpdate received — status {Status}", settings.Public.Status);
            OrgSettingsUpdated?.Invoke(this, settings.Public);
        });

        connection.On<Deployment>("DeploymentCreate", deployment =>
        {
            _logger.LogDebug("Medrunner: DeploymentCreate received — version {Version}", deployment.Version);
            DeploymentCreated?.Invoke(this, deployment);
        });

        connection.On<object>("TeamCreate", _ => _logger.LogDebug("Medrunner: TeamCreate received"));
        connection.On<object>("TeamUpdate", _ => _logger.LogDebug("Medrunner: TeamUpdate received"));
        connection.On<object>("TeamDelete", _ => _logger.LogDebug("Medrunner: TeamDelete received"));
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    private sealed class RetryPolicy : IRetryPolicy
    {
        private static readonly TimeSpan[] Delays =
        [
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromSeconds(60),
        ];

        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            var index = Math.Min(retryContext.PreviousRetryCount, Delays.Length - 1);
            return Delays[index];
        }
    }
}
