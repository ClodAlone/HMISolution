// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Text.Json.Serialization;

namespace SharedModels;

/// <summary>
/// License edition tier
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LicenseEdition
{
    Community,
    Professional,
    Enterprise
}

/// <summary>
/// License activation/validation state
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LicenseState
{
    Active,
    Expired,
    Suspended,
    Trial,
    Invalid
}

/// <summary>
/// License configuration and validation
/// </summary>
public class LicenseConfig
{
    /// <summary>
    /// Unique license key (GUID or encrypted string)
    /// </summary>
    public string? LicenseKey { get; set; }

    /// <summary>
    /// License edition tier
    /// </summary>
    public LicenseEdition Edition { get; set; } = LicenseEdition.Community;

    /// <summary>
    /// License activation/validation state
    /// </summary>
    public LicenseState State { get; set; } = LicenseState.Active;

    /// <summary>
    /// Company/organization name
    /// </summary>
    public string? LicensedTo { get; set; }

    /// <summary>
    /// Email address associated with license
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// License issue date
    /// </summary>
    public DateTime? IssuedDate { get; set; }

    /// <summary>
    /// License expiration date (null = perpetual)
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Maximum number of server instances allowed
    /// </summary>
    public int MaxInstances { get; set; } = 1;

    /// <summary>
    /// Feature flags enabled for this license
    /// </summary>
    public List<string> EnabledFeatures { get; set; } = new();

    /// <summary>
    /// Trial mode (true if this is a trial license)
    /// </summary>
    public bool IsTrial { get; set; }

    /// <summary>
    /// Trial expiration date
    /// </summary>
    public DateTime? TrialExpirationDate { get; set; }

    /// <summary>
    /// Offline activation (for air-gapped systems)
    /// </summary>
    public bool OfflineActivation { get; set; }

    /// <summary>
    /// Last validation timestamp (online check)
    /// </summary>
    public DateTime? LastValidated { get; set; }

    /// <summary>
    /// Support tier name
    /// </summary>
    public string? SupportTier { get; set; }

    /// <summary>
    /// Custom metadata (for enterprise-specific features)
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Feature availability by edition
/// </summary>
public static class LicenseFeatures
{
    // AI & Analytics
    public const string OpenAI = "ai.openai";
    public const string Gemini = "ai.gemini";
    public const string Ollama = "ai.ollama";
    public const string Claude = "ai.claude";
    public const string CustomAiModels = "ai.custom-models";
    public const string AdvancedAnomalyDetection = "ai.anomaly-advanced";
    public const string PredictiveAlarms = "ai.predictive-alarms";

    // Historian
    public const string SqliteHistorian = "historian.sqlite";
    public const string TimescaleDb = "historian.timescale";
    public const string UnlimitedRetention = "historian.unlimited-retention";

    // Drivers
    public const string BasicDrivers = "drivers.basic"; // OPC UA, Modbus, MQTT, REST
    public const string PremiumDrivers = "drivers.premium"; // EtherNet/IP, S7, Sparkplug, etc.
    public const string AllDrivers = "drivers.all";

    // Security & Authentication
    public const string BasicAuth = "auth.basic";
    public const string SsoLdap = "auth.sso-ldap";
    public const string AdvancedRbac = "auth.rbac-advanced";
    public const string AuditLogging = "auth.audit-logging";
    public const string ComplianceMode = "auth.compliance-21cfr11";

    // High Availability
    public const string Redundancy = "ha.redundancy";
    public const string MultiSite = "ha.multi-site";
    public const string LoadBalancing = "ha.load-balancing";

    // Support
    public const string CommunitySupport = "support.community";
    public const string EmailSupport = "support.email";
    public const string PhoneSupport = "support.phone";
    public const string DedicatedEngineer = "support.dedicated";

    // Customization
    public const string SourceCodeAccess = "custom.source-view";
    public const string SourceCodeModify = "custom.source-modify";
    public const string WhiteLabeling = "custom.white-label";
    public const string OemLicensing = "custom.oem";

    // Cloud
    public const string CloudHosting = "cloud.hosting";
    public const string CloudManagement = "cloud.management";

    /// <summary>
    /// Get default features for a license edition
    /// </summary>
    public static List<string> GetDefaultFeatures(LicenseEdition edition)
    {
        return edition switch
        {
            LicenseEdition.Community => new List<string>
            {
                OpenAI, Gemini, Ollama,
                SqliteHistorian,
                BasicDrivers,
                BasicAuth,
                CommunitySupport,
                SourceCodeAccess
            },
            LicenseEdition.Professional => new List<string>
            {
                // All Community features
                OpenAI, Gemini, Ollama, Claude, CustomAiModels, AdvancedAnomalyDetection, PredictiveAlarms,
                SqliteHistorian, TimescaleDb, UnlimitedRetention,
                BasicDrivers, PremiumDrivers,
                BasicAuth, SsoLdap, AuditLogging,
                Redundancy,
                CommunitySupport, EmailSupport,
                SourceCodeAccess, SourceCodeModify
            },
            LicenseEdition.Enterprise => new List<string>
            {
                // All Professional features
                OpenAI, Gemini, Ollama, Claude, CustomAiModels, AdvancedAnomalyDetection, PredictiveAlarms,
                SqliteHistorian, TimescaleDb, UnlimitedRetention,
                BasicDrivers, PremiumDrivers, AllDrivers,
                BasicAuth, SsoLdap, AdvancedRbac, AuditLogging, ComplianceMode,
                Redundancy, MultiSite, LoadBalancing,
                CommunitySupport, EmailSupport, PhoneSupport, DedicatedEngineer,
                SourceCodeAccess, SourceCodeModify, WhiteLabeling, OemLicensing,
                CloudHosting, CloudManagement
            },
            _ => new List<string>()
        };
    }

    /// <summary>
    /// Check if a feature is available in the given edition
    /// </summary>
    public static bool IsFeatureAvailable(LicenseEdition edition, string feature)
    {
        var features = GetDefaultFeatures(edition);
        return features.Contains(feature);
    }
}

/// <summary>
/// License validation result
/// </summary>
public class LicenseValidationResult
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }
    public LicenseState State { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public int DaysUntilExpiration { get; set; }
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// License activation request
/// </summary>
public class LicenseActivationRequest
{
    public string LicenseKey { get; set; } = "";
    public string Email { get; set; } = "";
    public string MachineId { get; set; } = "";
    public string ProductVersion { get; set; } = "";
    public bool OfflineMode { get; set; }
}

/// <summary>
/// License activation response
/// </summary>
public class LicenseActivationResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public LicenseConfig? License { get; set; }
    public string? ActivationToken { get; set; }
}
