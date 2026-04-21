namespace Arkanis.Overlay.External.Medrunner.Models;

[Flags]
public enum UserRoles
{
    Client = 1,
    Staff = 2,
    Developer = 524288,
    Bot = 1048576,
}

public enum PersonType
{
    Client = 0,
    Staff = 1,
    Bot = 2,
}

public enum AccountDeactivationReason
{
    None = 0,
    ClientDrivenDeletion = 1,
    Terminated = 2,
    Blocked = 3,
}

public enum MissionStatus
{
    Created = 0,
    Received = 1,
    InProgress = 2,
    Success = 3,
    Failed = 4,
    NoContact = 5,
    Canceled = 6,
    Refused = 7,
    Aborted = 8,
    ServerError = 9,
}

public enum ThreatLevel
{
    Unknown = 0,
    Low = 1,
    Medium = 2,
    High = 3,
}

public enum CancellationReason
{
    None = 0,
    Other = 1,
    SuccumbedToWounds = 2,
    ServerError = 3,
    Respawned = 4,
    Rescued = 5,
}

public enum ResponseRating
{
    None = 0,
    Good = 1,
    Bad = 2,
}

public enum StaffClass
{
    None = 0,
    Medic = 1,
    Security = 2,
    Pilot = 3,
    Lead = 4,
    Dispatch = 5,
    DispatchLead = 6,
    DispatchTrainee = 7,
    DispatchObserver = 8,
    Qrf = 9,
    Logistics = 10,
}

[Flags]
public enum MissionServices
{
    None = 0,
    Pve = 1,
    Pvp = 2,
    RevivedHealed = 4,
    HealedInShip = 8,
    ExtractSafeZone = 16,
}

public enum EmergencyOrigin
{
    Unknown = 0,
    Report = 1,
    Beacon = 2,
    Evaluation = 3,
}

public enum SubmissionSource
{
    Unknown = 0,
    Api = 1,
    Bot = 2,
}

public enum ServiceStatus
{
    Unknown = 0,
    Healthy = 1,
    SlightlyDegraded = 2,
    HeavilyDegraded = 3,
    Offline = 4,
}

public enum SpaceLocationType
{
    Unknown = 0,
    System = 1,
    Planet = 2,
    Moon = 3,
}

public enum LocationCharacteristic
{
    Unknown = 0,
    HighTemperature = 1,
    LowTemperature = 2,
    HostileAtmosphere = 3,
}

public enum LocationType
{
    Unknown = 0,
    System = 1,
    Planet = 2,
    Moon = 3,
}

public enum ClientType
{
    ClientPortal = 1,
    StaffPortal = 2,
}

public enum CodeType
{
    Unknown = 0,
    CitizenCon2954 = 1,
}

public enum MedrunnerLevel
{
    None = 0,
    Tier1Section1 = 101,
    Tier1Section2 = 102,
    Tier1Section3 = 103,
    Tier2Section1 = 201,
    Tier2Section2 = 202,
    Tier2Section3 = 203,
    Tier3Section1 = 301,
    Tier3Section2 = 302,
    Tier3Section3 = 303,
    Tier4Section1 = 401,
    Tier4Section2 = 402,
    Tier4Section3 = 403,
    Tier5Section1 = 501,
    Tier5Section2 = 502,
    Tier5Section3 = 503,
    Tier6Section1 = 601,
    Tier6Section2 = 602,
    Tier6Section3 = 603,
    Tier7Section1 = 701,
    Tier7Section2 = 702,
    Tier7Section3 = 703,
    Tier8Section1 = 801,
    Tier8Section2 = 802,
    Tier8Section3 = 803,
    Tier9Section1 = 901,
    Tier9Section2 = 902,
    Tier9Section3 = 903,
    Tier10Section1 = 1001,
    Tier10Section2 = 1002,
    Tier10Section3 = 1003,
}
