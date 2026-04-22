using System.Text.Json.Serialization;

namespace SharedModels;

/// <summary>
/// FDA 21 CFR Part 11 compliant audit record with cryptographic integrity.
/// Each record contains a hash chain linking to the previous record to prevent tampering.
/// </summary>
public class AuditRecord
{
    /// <summary>Unique identifier for this audit record.</summary>
    public long Id { get; set; }

    /// <summary>UTC timestamp when the event occurred.</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>Type of event (e.g., DataChange, ConfigChange, AlarmAck, Login, Logout).</summary>
    public ComplianceEventType EventType { get; set; }

    /// <summary>Username of the person who performed the action.</summary>
    public string Username { get; set; } = "";

    /// <summary>User's full name for display purposes.</summary>
    public string FullName { get; set; } = "";

    /// <summary>The action performed (e.g., "Modified Recipe", "Acknowledged Alarm").</summary>
    public string Action { get; set; } = "";

    /// <summary>Entity affected (e.g., recipe name, variable path, alarm ID).</summary>
    public string AffectedEntity { get; set; } = "";

    /// <summary>Previous value before the change (JSON serialized if complex).</summary>
    public string? OldValue { get; set; }

    /// <summary>New value after the change (JSON serialized if complex).</summary>
    public string? NewValue { get; set; }

    /// <summary>Reason for the change provided by the user.</summary>
    public string? ReasonForChange { get; set; }

    /// <summary>Electronic signature associated with this action (if required).</summary>
    public ElectronicSignature? Signature { get; set; }

    /// <summary>Second signature for dual-approval workflows (e.g., recipe changes).</summary>
    public ElectronicSignature? SecondSignature { get; set; }

    /// <summary>Additional context or details in JSON format.</summary>
    public string? Details { get; set; }

    /// <summary>Source system or component that generated this record.</summary>
    public string Source { get; set; } = "";

    /// <summary>SHA256 hash of the previous audit record, creating a tamper-proof chain.</summary>
    public string? PreviousRecordHash { get; set; }

    /// <summary>SHA256 hash of this record's content (excluding this field and Id).</summary>
    public string RecordHash { get; set; } = "";

    /// <summary>Session ID to correlate related actions.</summary>
    public string? SessionId { get; set; }

    /// <summary>IP address of the client that initiated the action.</summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>Whether this record has been verified for integrity.</summary>
    [JsonIgnore]
    public bool IsVerified { get; set; }
}

/// <summary>
/// Electronic signature capturing who, what, when, and why for critical operations.
/// Compliant with FDA 21 CFR Part 11 electronic signature requirements.
/// </summary>
public class ElectronicSignature
{
    /// <summary>Username of the person signing.</summary>
    public string Username { get; set; } = "";

    /// <summary>Full name of the person signing.</summary>
    public string FullName { get; set; } = "";

    /// <summary>UTC timestamp when the signature was applied.</summary>
    public DateTime SignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Meaning of the signature (e.g., "Reviewed", "Approved", "Executed").</summary>
    public string Meaning { get; set; } = "";

    /// <summary>Optional comment from the signer.</summary>
    public string? Comment { get; set; }

    /// <summary>Hashed password or biometric signature for authentication.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string? SignatureHash { get; set; }

    /// <summary>Whether the signature was verified successfully.</summary>
    [JsonIgnore]
    public bool IsVerified { get; set; }
}

/// <summary>
/// Types of compliance events that require audit trails per FDA 21 CFR Part 11.
/// </summary>
public enum ComplianceEventType
{
    /// <summary>User logged in to the system.</summary>
    Login,

    /// <summary>User logged out of the system.</summary>
    Logout,

    /// <summary>Failed login attempt.</summary>
    LoginFailed,

    /// <summary>User password was changed.</summary>
    PasswordChanged,

    /// <summary>User account was created.</summary>
    UserCreated,

    /// <summary>User account was modified.</summary>
    UserModified,

    /// <summary>User account was deleted.</summary>
    UserDeleted,

    /// <summary>User account was locked out.</summary>
    UserLockedOut,

    /// <summary>User account was unlocked.</summary>
    UserUnlocked,

    /// <summary>Configuration data was changed.</summary>
    ConfigChange,

    /// <summary>Recipe was created.</summary>
    RecipeCreated,

    /// <summary>Recipe was modified.</summary>
    RecipeModified,

    /// <summary>Recipe was deleted.</summary>
    RecipeDeleted,

    /// <summary>Recipe execution started.</summary>
    RecipeExecutionStarted,

    /// <summary>Recipe execution completed.</summary>
    RecipeExecutionCompleted,

    /// <summary>Recipe execution was aborted.</summary>
    RecipeExecutionAborted,

    /// <summary>Alarm was acknowledged.</summary>
    AlarmAcknowledged,

    /// <summary>Alarm configuration was changed.</summary>
    AlarmConfigChanged,

    /// <summary>Critical variable value was changed.</summary>
    CriticalDataChange,

    /// <summary>System configuration was changed.</summary>
    SystemConfigChange,

    /// <summary>Audit trail was exported.</summary>
    AuditExport,

    /// <summary>Backup was created.</summary>
    BackupCreated,

    /// <summary>System was restored from backup.</summary>
    SystemRestored,

    /// <summary>Security settings were modified.</summary>
    SecurityChange,

    /// <summary>Electronic signature was applied.</summary>
    ElectronicSignature,

    /// <summary>Report was generated.</summary>
    ReportGenerated,

    /// <summary>Access was denied due to insufficient permissions.</summary>
    AccessDenied,

    /// <summary>System startup event.</summary>
    SystemStartup,

    /// <summary>System shutdown event.</summary>
    SystemShutdown,

    /// <summary>System time was changed.</summary>
    TimeChanged,

    /// <summary>Other compliance-relevant event.</summary>
    Other
}

/// <summary>
/// Configuration for FDA 21 CFR Part 11 compliance features.
/// </summary>
public class ComplianceConfig
{
    /// <summary>Whether FDA 21 CFR Part 11 compliance mode is enabled.</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>Path to the SQLite audit trail database.</summary>
    public string AuditDbPath { get; set; } = "audit_trail.db";

    /// <summary>Retention period for audit records in days (0 = keep forever).</summary>
    public int AuditRetentionDays { get; set; } = 2555; // 7 years

    /// <summary>Require electronic signatures for critical operations.</summary>
    public bool RequireElectronicSignatures { get; set; } = true;

    /// <summary>Require dual signatures for high-risk operations (e.g., recipe changes).</summary>
    public bool RequireDualSignatures { get; set; } = false;

    /// <summary>Operations that require electronic signatures.</summary>
    public List<ComplianceEventType> SignatureRequiredEvents { get; set; } = new()
    {
        ComplianceEventType.RecipeModified,
        ComplianceEventType.RecipeDeleted,
        ComplianceEventType.ConfigChange,
        ComplianceEventType.AlarmConfigChanged,
        ComplianceEventType.CriticalDataChange,
        ComplianceEventType.SystemConfigChange,
        ComplianceEventType.SecurityChange,
        ComplianceEventType.UserCreated,
        ComplianceEventType.UserModified,
        ComplianceEventType.UserDeleted
    };

    /// <summary>Operations that require dual approval signatures.</summary>
    public List<ComplianceEventType> DualSignatureRequiredEvents { get; set; } = new()
    {
        ComplianceEventType.RecipeModified,
        ComplianceEventType.RecipeDeleted,
        ComplianceEventType.SystemConfigChange,
        ComplianceEventType.SecurityChange
    };

    /// <summary>Whether to enforce password complexity requirements.</summary>
    public bool EnforcePasswordComplexity { get; set; } = true;

    /// <summary>Minimum password length.</summary>
    public int MinPasswordLength { get; set; } = 8;

    /// <summary>Maximum number of failed login attempts before lockout.</summary>
    public int MaxFailedLoginAttempts { get; set; } = 5;

    /// <summary>Account lockout duration in minutes.</summary>
    public int LockoutDurationMinutes { get; set; } = 30;

    /// <summary>Number of previous passwords to remember (prevent reuse).</summary>
    public int PasswordHistoryCount { get; set; } = 5;

    /// <summary>Require reason for change for all audit events.</summary>
    public bool RequireReasonForChange { get; set; } = true;

    /// <summary>Automatically verify audit trail integrity on startup.</summary>
    public bool VerifyIntegrityOnStartup { get; set; } = true;

    /// <summary>Enable automatic audit trail backup.</summary>
    public bool EnableAuditBackup { get; set; } = true;

    /// <summary>Audit trail backup interval in hours.</summary>
    public int AuditBackupIntervalHours { get; set; } = 24;

    /// <summary>Password policy configuration.</summary>
    public PasswordPolicy PasswordPolicy { get; set; } = new();

    /// <summary>Auto log-off timeout in minutes (0 = disabled).</summary>
    public int AutoLogOffMinutes { get; set; } = 15;
}

/// <summary>
/// Password policy configuration for compliance.
/// </summary>
public class PasswordPolicy
{
    /// <summary>Minimum password length.</summary>
    public int MinLength { get; set; } = 8;

    /// <summary>Require at least one uppercase letter.</summary>
    public bool RequireUppercase { get; set; } = true;

    /// <summary>Require at least one lowercase letter.</summary>
    public bool RequireLowercase { get; set; } = true;

    /// <summary>Require at least one digit.</summary>
    public bool RequireDigit { get; set; } = true;

    /// <summary>Require at least one special character.</summary>
    public bool RequireSpecialChar { get; set; } = true;

    /// <summary>Maximum password age in days (0 = never expires).</summary>
    public int MaxAgeDays { get; set; } = 90;

    /// <summary>Number of previous passwords to remember (prevent reuse).</summary>
    public int HistoryCount { get; set; } = 5;

    /// <summary>Minimum days between password changes.</summary>
    public int MinAgeDays { get; set; } = 1;
}

/// <summary>
/// Represents a request to perform an action requiring electronic signature.
/// </summary>
public class SignatureRequest
{
    /// <summary>Type of event requiring signature.</summary>
    public ComplianceEventType EventType { get; set; }

    /// <summary>Description of the action being performed.</summary>
    public string Action { get; set; } = "";

    /// <summary>Entity being affected by the action.</summary>
    public string AffectedEntity { get; set; } = "";

    /// <summary>Old value before change (if applicable).</summary>
    public string? OldValue { get; set; }

    /// <summary>New value after change (if applicable).</summary>
    public string? NewValue { get; set; }

    /// <summary>Whether dual signatures are required.</summary>
    public bool RequiresDualSignature { get; set; }

    /// <summary>Suggested meaning for the signature.</summary>
    public string SuggestedMeaning { get; set; } = "Approved";
}

/// <summary>
/// Result of a signature verification or validation.
/// </summary>
public class SignatureResult
{
    /// <summary>Whether the signature was successful.</summary>
    public bool Success { get; set; }

    /// <summary>Error or success message.</summary>
    public string Message { get; set; } = "";

    /// <summary>The verified signature (if successful).</summary>
    public ElectronicSignature? Signature { get; set; }

    /// <summary>Second signature (if dual signature was required).</summary>
    public ElectronicSignature? SecondSignature { get; set; }
}

/// <summary>
/// Audit trail integrity verification result.
/// </summary>
public class AuditIntegrityResult
{
    /// <summary>Whether the audit trail is intact and unmodified.</summary>
    public bool IsIntact { get; set; }

    /// <summary>Total number of records verified.</summary>
    public int TotalRecords { get; set; }

    /// <summary>Number of records that failed verification.</summary>
    public int FailedRecords { get; set; }

    /// <summary>List of record IDs that failed verification.</summary>
    public List<long> FailedRecordIds { get; set; } = new();

    /// <summary>Detailed message about the verification.</summary>
    public string Message { get; set; } = "";

    /// <summary>Timestamp when verification was performed.</summary>
    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Helper methods for UserConfig compliance features.
/// </summary>
public static class UserConfigComplianceExtensions
{
    /// <summary>Get the list of previous password hashes to prevent reuse.</summary>
    public static List<string> GetPasswordHistory(this UserConfig user)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(user.PasswordHistory))
                return new List<string>();

            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(user.PasswordHistory) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    /// <summary>Set the password history for compliance tracking.</summary>
    public static void SetPasswordHistory(this UserConfig user, List<string> history)
    {
        user.PasswordHistory = System.Text.Json.JsonSerializer.Serialize(history);
    }

    /// <summary>Add a password hash to the history.</summary>
    public static void AddPasswordToHistory(this UserConfig user, string passwordHash, int maxHistory = 5)
    {
        var history = user.GetPasswordHistory();
        history.Insert(0, passwordHash);

        // Keep only the most recent passwords
        if (history.Count > maxHistory)
            history = history.Take(maxHistory).ToList();

        user.SetPasswordHistory(history);
    }

    /// <summary>Check if the account is currently locked out.</summary>
    public static bool IsLockedOut(this UserConfig user)
    {
        return user.LockedOutUntil.HasValue && user.LockedOutUntil.Value > DateTime.UtcNow;
    }

    /// <summary>Lock the user account for a specified duration.</summary>
    public static void LockAccount(this UserConfig user, int durationMinutes)
    {
        user.LockedOutUntil = DateTime.UtcNow.AddMinutes(durationMinutes);
    }

    /// <summary>Unlock the user account.</summary>
    public static void UnlockAccount(this UserConfig user)
    {
        user.LockedOutUntil = null;
        user.FailedLoginAttempts = 0;
    }
}
