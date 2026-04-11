using System.Text.Json.Serialization;

namespace SharedModels;

/// <summary>
/// Represents a signed license file. The <see cref="Signature"/> is an RSA-SHA256
/// signature over the canonical JSON of all other properties.
/// </summary>
public class License
{
    /// <summary>Unique license identifier.</summary>
    public string Id { get; set; } = "";

    /// <summary>License tier: "Trial", "Starter", "Professional", "Enterprise".</summary>
    public string Tier { get; set; } = "Trial";

    /// <summary>Licensee name (company or individual).</summary>
    public string LicensedTo { get; set; } = "";

    /// <summary>Optional project/site name this license is valid for.</summary>
    public string ProjectName { get; set; } = "";

    /// <summary>
    /// Hardware fingerprint the license is locked to.
    /// Empty string means the license is not machine-locked.
    /// </summary>
    public string MachineId { get; set; } = "";

    /// <summary>Date the license was issued (UTC).</summary>
    public DateTime IssuedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date the license expires (UTC). <c>DateTime.MaxValue</c> means perpetual.
    /// </summary>
    public DateTime ExpiresUtc { get; set; } = DateTime.UtcNow.AddDays(30);

    // ─── Feature limits ─────────────────────────────────────────────

    /// <summary>Maximum number of OPC variables. 0 = unlimited.</summary>
    public int MaxVariables { get; set; } = 20;

    /// <summary>Maximum number of loaded drivers. 0 = unlimited.</summary>
    public int MaxDrivers { get; set; } = 1;

    /// <summary>Maximum number of scripts. 0 = unlimited.</summary>
    public int MaxScripts { get; set; } = 2;

    /// <summary>Maximum number of PLC programs. 0 = unlimited.</summary>
    public int MaxPlcPrograms { get; set; } = 1;

    /// <summary>Maximum number of screens. 0 = unlimited.</summary>
    public int MaxScreens { get; set; } = 1;

    /// <summary>Maximum number of recipes. 0 = unlimited.</summary>
    public int MaxRecipes { get; set; }

    /// <summary>Whether data logging (historian) is allowed.</summary>
    public bool AllowDataLogging { get; set; }

    /// <summary>Whether the AI assistant feature is allowed.</summary>
    public bool AllowAi { get; set; }

    // ─── Signature ──────────────────────────────────────────────────

    /// <summary>
    /// RSA-SHA256 signature (Base64) over the canonical license payload.
    /// This field is excluded from the signed payload itself.
    /// </summary>
    public string Signature { get; set; } = "";
}

/// <summary>
/// Runtime license status after validation.
/// </summary>
public class LicenseStatus
{
    public bool IsValid { get; set; }
    public string Tier { get; set; } = "Unlicensed";
    public string LicensedTo { get; set; } = "";
    public string Message { get; set; } = "";
    public DateTime ExpiresUtc { get; set; }
    public int DaysRemaining { get; set; }
    public License? License { get; set; }

    /// <summary>UTC timestamp when the demo grace period started. <c>null</c> if not in demo mode.</summary>
    public DateTime? DemoStartedUtc { get; set; }

    /// <summary>Duration of the demo grace period in minutes. Default 10.</summary>
    public int DemoGraceMinutes { get; set; } = 10;

    /// <summary>Whether the demo grace period has expired and the license should degrade to Trial limits.</summary>
    public bool IsDemoExpired => DemoStartedUtc.HasValue &&
                                 DateTime.UtcNow >= DemoStartedUtc.Value.AddMinutes(DemoGraceMinutes);

    /// <summary>Time remaining in the demo grace period. <see cref="TimeSpan.Zero"/> if expired or not in demo mode.</summary>
    public TimeSpan DemoTimeRemaining => DemoStartedUtc.HasValue
        ? TimeSpan.FromTicks(Math.Max(0, (DemoStartedUtc.Value.AddMinutes(DemoGraceMinutes) - DateTime.UtcNow).Ticks))
        : TimeSpan.Zero;

    /// <summary>Check whether a feature count is within the license limit. 0 = unlimited.</summary>
    public bool IsWithinLimit(int current, int max) => max == 0 || current <= max;
}

/// <summary>
/// Default limits per tier (used by the license generator tool).
/// </summary>
public static class LicenseTiers
{
    public static License CreateTrial(string licensedTo, string machineId) => new()
    {
        Id = Guid.NewGuid().ToString("N")[..12],
        Tier = "Trial",
        LicensedTo = licensedTo,
        MachineId = machineId,
        ExpiresUtc = DateTime.UtcNow.AddDays(30),
        MaxVariables = 20,
        MaxDrivers = 1,
        MaxScripts = 2,
        MaxPlcPrograms = 1,
        MaxScreens = 1,
        MaxRecipes = 0,
        AllowDataLogging = false,
        AllowAi = false
    };

    public static License CreateStarter(string licensedTo, string machineId, int daysValid = 365) => new()
    {
        Id = Guid.NewGuid().ToString("N")[..12],
        Tier = "Starter",
        LicensedTo = licensedTo,
        MachineId = machineId,
        ExpiresUtc = DateTime.UtcNow.AddDays(daysValid),
        MaxVariables = 100,
        MaxDrivers = 2,
        MaxScripts = 5,
        MaxPlcPrograms = 3,
        MaxScreens = 3,
        MaxRecipes = 2,
        AllowDataLogging = true,
        AllowAi = false
    };

    public static License CreateProfessional(string licensedTo, string machineId, int daysValid = 365) => new()
    {
        Id = Guid.NewGuid().ToString("N")[..12],
        Tier = "Professional",
        LicensedTo = licensedTo,
        MachineId = machineId,
        ExpiresUtc = DateTime.UtcNow.AddDays(daysValid),
        MaxVariables = 1000,
        MaxDrivers = 0,
        MaxScripts = 0,
        MaxPlcPrograms = 0,
        MaxScreens = 0,
        MaxRecipes = 0,
        AllowDataLogging = true,
        AllowAi = true
    };

    public static License CreateEnterprise(string licensedTo, string machineId) => new()
    {
        Id = Guid.NewGuid().ToString("N")[..12],
        Tier = "Enterprise",
        LicensedTo = licensedTo,
        MachineId = machineId,
        ExpiresUtc = DateTime.MaxValue,
        MaxVariables = 0,
        MaxDrivers = 0,
        MaxScripts = 0,
        MaxPlcPrograms = 0,
        MaxScreens = 0,
        MaxRecipes = 0,
        AllowDataLogging = true,
        AllowAi = true
    };

    /// <summary>
    /// Creates a Demo license with full (Enterprise-level) features.
    /// Used during the demo grace period before degrading to Trial limits.
    /// </summary>
    public static License CreateDemo() => new()
    {
        Id = Guid.NewGuid().ToString("N")[..12],
        Tier = "Demo",
        LicensedTo = "Demo User",
        MachineId = "",
        ExpiresUtc = DateTime.MaxValue,
        MaxVariables = 0,
        MaxDrivers = 0,
        MaxScripts = 0,
        MaxPlcPrograms = 0,
        MaxScreens = 0,
        MaxRecipes = 0,
        AllowDataLogging = true,
        AllowAi = true
    };
}

[JsonSerializable(typeof(License))]
public partial class LicenseJsonContext : JsonSerializerContext { }
