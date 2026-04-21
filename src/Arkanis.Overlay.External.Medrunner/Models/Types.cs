namespace Arkanis.Overlay.External.Medrunner.Models;

using System.Text.Json.Serialization;

public record ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }
    public int? StatusCode { get; init; }
}

public record TokenGrant
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required string AccessTokenExpiration { get; init; }
    public string? RefreshTokenExpiration { get; init; }
}

public record DbItem
{
    public required string Id { get; init; }
    public required string Created { get; init; }
    public required string Updated { get; init; }
}

public record WritableDbItem : DbItem;

public record Person : WritableDbItem
{
    public required string DiscordId { get; init; }
    public string? RsiHandle { get; init; }
    public UserRoles Roles { get; init; }
    public PersonType PersonType { get; init; }
    public bool Active { get; init; }
    public AccountDeactivationReason DeactivationReason { get; init; }
    public required ClientStats ClientStats { get; init; }
    public string? ActiveEmergency { get; init; }
    public bool AllowAnonymousAlert { get; init; }
    public string? InitialJoinDate { get; init; }
    public string? ClientPortalPreferencesBlob { get; init; }

    [JsonIgnore]
    public bool IsStaff => (Roles & UserRoles.Staff) != 0;

    [JsonIgnore]
    public bool IsClient => (Roles & UserRoles.Client) != 0;
}

public record ClientStats
{
    public required EmergencyStats Missions { get; init; }
}

public record EmergencyStats
{
    public int Success { get; init; }
    public int Failed { get; init; }
    public int NoContact { get; init; }
    public int Refused { get; init; }
    public int Aborted { get; init; }
    public int ServerError { get; init; }
    public int Canceled { get; init; }
}

public record BlockedStatus
{
    public bool Blocked { get; init; }
}

public record ClientHistory : DbItem
{
    public required string EmergencyId { get; init; }
    public required string ClientId { get; init; }
    public required string EmergencyCreationTimestamp { get; init; }
}

public record Emergency : WritableDbItem
{
    public required string System { get; init; }
    public required string Subsystem { get; init; }
    public string? TertiaryLocation { get; init; }
    public ThreatLevel ThreatLevel { get; init; }
    public required string ClientRsiHandle { get; init; }
    public string? ClientDiscordId { get; init; }
    public string? ClientId { get; init; }
    public required string SubscriptionTier { get; init; }
    public MissionStatus Status { get; init; }
    public MessageCache? AlertMessage { get; init; }
    public MessageCache? ClientMessage { get; init; }
    public MessageCache? CoordinationThread { get; init; }
    public MessageCache? AfterActionReportMessage { get; init; }
    public required Team RespondingTeam { get; init; }
    public required RespondingTeam[] RespondingTeams { get; init; }
    public long CreationTimestamp { get; init; }
    public long? AcceptedTimestamp { get; init; }
    public long? CompletionTimestamp { get; init; }
    public ResponseRating Rating { get; init; }
    public string? RatingRemarks { get; init; }
    public bool Test { get; init; }
    public CancellationReason CancellationReason { get; init; }
    public string? RefusalReason { get; init; }
    public EmergencyOrigin Origin { get; init; }
    public ClientData? ClientData { get; init; }
    public bool IsComplete { get; init; }
    public string? MissionName { get; init; }
    public AfterActionReport? AfterActionReport { get; init; }
    public SubmissionSource SubmissionSource { get; init; }
}

public record MessageCache
{
    public required string Id { get; init; }
    public required string ChannelId { get; init; }
}

public record Team
{
    public required TeamMember[] Staff { get; init; }
    public required TeamMember[] Dispatchers { get; init; }
    public required TeamMember[] AllMembers { get; init; }
    public int MaxMembers { get; init; }
}

public record TeamMember
{
    public required string DiscordId { get; init; }
    public required string Id { get; init; }
    public string? RsiHandle { get; init; }

    [JsonPropertyName("class")]
    public StaffClass Class { get; init; }

    public string? TeamId { get; init; }
}

public record RespondingTeam
{
    public required string Id { get; init; }
    public required string TeamName { get; init; }
}

public record ClientData
{
    public required string RsiHandle { get; init; }
    public required string RsiProfileLink { get; init; }
    public bool GotClientData { get; init; }
    public bool RedactedOrgOnProfile { get; init; }
    public bool Reported { get; init; }
}

public record AfterActionReport
{
    public string? Remarks { get; init; }
    public required string SubmitterStaffId { get; init; }
    public MissionServices ServicesProvided { get; init; }
    public bool SuspectedTrap { get; init; }
    public bool HasBeenEdited { get; init; }
    public long SubmittedOn { get; init; }
    public required AfterActionReportEdit[] EditHistory { get; init; }
}

public record AfterActionReportEdit
{
    public required string EditorStaffId { get; init; }
    public long EditTime { get; init; }
}

public record ChatMessage : WritableDbItem
{
    public required string EmergencyId { get; init; }
    public required string SenderId { get; init; }
    public long MessageSentTimestamp { get; init; }
    public required string Contents { get; init; }
    public bool Edited { get; init; }
    public bool Deleted { get; init; }
}

public record PaginatedResponse<T>
{
    public required T[] Data { get; init; }
    public string? PaginationToken { get; init; }
}

public record CreateEmergencyRequest
{
    public required EmergencyLocation Location { get; init; }
    public required ThreatLevel ThreatLevel { get; init; }
    public string? RsiHandle { get; init; }
}

public record EmergencyLocation
{
    public required string System { get; init; }
    public required string Subsystem { get; init; }
    public string? TertiaryLocation { get; init; }
}

public record LocationDetail
{
    public required string Name { get; init; }
    public LocationType Type { get; init; }
    public required LocationDetail[] Children { get; init; }
}

public record PublicOrgSettings
{
    public ServiceStatus Status { get; init; }
    public bool EmergenciesEnabled { get; init; }
    public bool AnonymousAlertsEnabled { get; init; }
    public bool RegistrationEnabled { get; init; }
    public MessageOfTheDay? MessageOfTheDay { get; init; }
    public required LocationSettings LocationSettings { get; init; }
}

public record LocationSettings
{
    public required SpaceLocation[] Locations { get; init; }
}

public record SpaceLocation
{
    public required string Name { get; init; }
    public SpaceLocationType Type { get; init; }
    public required LocationCharacteristic[] Characteristics { get; init; }
    public required SpaceLocation[] Children { get; init; }
    public bool Enabled { get; init; }
}

public record MessageOfTheDay
{
    public required string Message { get; init; }
    public DateRange? DateRange { get; init; }
}

public record DateRange
{
    public required string StartDate { get; init; }
    public required string EndDate { get; init; }
}

public record OrgSettings : WritableDbItem
{
    public required PublicOrgSettings Public { get; init; }
}

public record MedalInformation
{
    public MedrunnerLevel Level { get; init; }
    public int SuccessfulMissions { get; init; }
}

public record TeamDetailsResponse
{
    public required ResponderDetails[] Stats { get; init; }
    public double AggregatedSuccessRate { get; init; }
}

public record ResponderDetails
{
    public required string Id { get; init; }
    public MedrunnerLevel Level { get; init; }
    public double MissionSuccessRate { get; init; }
    public double DispatchSuccessRate { get; init; }
}

public record ApiToken : DbItem
{
    public required string UserId { get; init; }
    public required string Name { get; init; }
    public long? ExpirationDate { get; init; }
    public string? LastUsed { get; init; }
}

public record Deployment
{
    public ClientType ClientType { get; init; }
    public required string Version { get; init; }
}

public record RedeemedCode
{
    public required string Code { get; init; }
    public CodeType Type { get; init; }
}
