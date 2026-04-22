using SharedModels;

namespace ServerEditorWeb.Services;

/// <summary>
/// Scoped service tracking the currently authenticated editor user.
/// Enhanced with FDA 21 CFR Part 11 compliance features.
/// </summary>
public class AuthServiceCompliance
{
    private readonly ILogger<AuthServiceCompliance>? _logger;
    private readonly ComplianceConfig? _complianceConfig;

    public bool IsAuthenticated { get; private set; }
    public string? Username { get; private set; }
    public string? FullName { get; private set; }
    public string? Group { get; private set; }
    public string? AccessLevel { get; private set; }
    public bool CanAccessEditor { get; private set; }

    /// <summary>Plain-text password kept in memory for OPC UA connections when anonymous is disabled.</summary>
    public string? OpcPassword { get; private set; }

    /// <summary>True when the user must change their password before using the app.</summary>
    public bool MustChangePassword { get; private set; }

    /// <summary>Auto log-off timeout in seconds for the current user. 0 = disabled.</summary>
    public int AutoLogOffSeconds { get; private set; }

    private UserConfig? _currentUserConfig;
    private List<UserConfig>? _allUsers;

    public event Action? StateChanged;

    public AuthServiceCompliance()
    {
        // Parameterless constructor for existing usage
    }

    public AuthServiceCompliance(ILogger<AuthServiceCompliance> logger, ComplianceConfig complianceConfig)
    {
        _logger = logger;
        _complianceConfig = complianceConfig;
    }

    /// <summary>
    /// Attempt login against the project's user list with FDA 21 CFR Part 11 compliance checks.
    /// Returns (success, error message).
    /// </summary>
    public (bool Success, string Message) Login(string username, string password,
        List<UserConfig> users, List<UserGroupConfig> groups)
    {
        _allUsers = users; // Store for later use

        if (string.IsNullOrWhiteSpace(username))
            return (false, "Username is required.");

        var user = users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            _logger?.LogWarning("Login attempt with unknown username: {Username}", username);
            return (false, "Invalid username or password.");
        }

        // Check if account is locked out (compliance feature)
        if (user.IsLockedOut())
        {
            var remainingMinutes = (int)(user.LockedOutUntil!.Value - DateTime.UtcNow).TotalMinutes + 1;
            _logger?.LogWarning("Login attempt on locked account: {Username} (locked for {Minutes} more minutes)",
                username, remainingMinutes);
            return (false, $"Account is locked. Please try again in {remainingMinutes} minutes.");
        }

        // Try PasswordHash first, then fall back to legacy Password (with warning in compliance mode)
        bool valid = false;
        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            valid = PasswordHasher.Verify(password, user.PasswordHash);
        }
        else if (!string.IsNullOrEmpty(user.Password))
        {
            // Legacy plain-text comparison - warn if compliance mode is enabled
            if (_complianceConfig?.Enabled == true)
            {
                _logger?.LogWarning("FDA 21 CFR Part 11 compliance violation: User {Username} is using legacy plain-text password", username);
            }
            valid = user.Password == password;
        }

        if (!valid)
        {
            // Track failed login attempt (compliance feature)
            user.FailedLoginAttempts++;
            user.LastFailedLoginAt = DateTime.UtcNow;

            _logger?.LogWarning("Failed login attempt for user {Username} (attempt {Count})",
                username, user.FailedLoginAttempts);

            // Lock account if max attempts exceeded (compliance feature)
            if (_complianceConfig?.Enabled == true &&
                user.FailedLoginAttempts >= (_complianceConfig.MaxFailedLoginAttempts))
            {
                user.LockAccount(_complianceConfig.LockoutDurationMinutes);
                _logger?.LogWarning("Account {Username} locked due to {Count} failed login attempts",
                    username, user.FailedLoginAttempts);
                return (false, $"Account locked due to too many failed login attempts. Please try again in {_complianceConfig.LockoutDurationMinutes} minutes.");
            }

            return (false, "Invalid username or password.");
        }

        var group = groups.FirstOrDefault(g =>
            g.Name.Equals(user.Group, StringComparison.OrdinalIgnoreCase));

        if (group == null)
            return (false, $"User group '{user.Group}' not found.");

        if (!group.CanAccessEditor)
            return (false, "Your user group does not have editor access.");

        // Successful login - reset failed attempts and update last login (compliance feature)
        user.FailedLoginAttempts = 0;
        user.LastLoginAt = DateTime.UtcNow;

        IsAuthenticated = true;
        OpcPassword = password;
        Username = user.Username;
        FullName = user.FullName;
        Group = user.Group;
        AccessLevel = group.AccessLevel;
        CanAccessEditor = group.CanAccessEditor;
        AutoLogOffSeconds = user.AutoLogOffSeconds;
        _currentUserConfig = user;

        // Determine if password change is required
        MustChangePassword = false;
        if (user.MustChangePasswordOnFirstLogin && !user.HasLoggedInBefore)
        {
            MustChangePassword = true;
        }
        else if (user.PasswordExpiryDays > 0 && user.PasswordChangedDate.HasValue)
        {
            var daysSinceChange = (DateTime.UtcNow - user.PasswordChangedDate.Value).TotalDays;
            if (daysSinceChange >= user.PasswordExpiryDays)
                MustChangePassword = true;
        }

        // Mark first login
        if (!user.HasLoggedInBefore)
            user.HasLoggedInBefore = true;

        _logger?.LogInformation("User {Username} logged in successfully", username);
        StateChanged?.Invoke();

        return (true, $"Welcome, {user.FullName ?? user.Username}!");
    }

    /// <summary>
    /// Change the current user's password with FDA 21 CFR Part 11 compliance checks.
    /// Returns (success, message).
    /// </summary>
    public (bool Success, string Message) ChangePassword(string currentPassword, string newPassword, string confirmPassword, bool requireStrong = false)
    {
        if (_currentUserConfig == null)
            return (false, "No user is logged in.");

        if (string.IsNullOrWhiteSpace(newPassword))
            return (false, "New password cannot be empty.");

        if (newPassword != confirmPassword)
            return (false, "Passwords do not match.");

        // Use compliance config for password strength if available
        var enforceComplexity = _complianceConfig?.EnforcePasswordComplexity ?? requireStrong;
        var minLength = _complianceConfig?.MinPasswordLength ?? 8;

        var (valid, error) = PasswordValidator.Validate(newPassword, enforceComplexity, minLength);
        if (!valid)
            return (false, error!);

        // Verify current password
        bool currentValid = false;
        if (!string.IsNullOrEmpty(_currentUserConfig.PasswordHash))
            currentValid = PasswordHasher.Verify(currentPassword, _currentUserConfig.PasswordHash);
        else if (!string.IsNullOrEmpty(_currentUserConfig.Password))
            currentValid = _currentUserConfig.Password == currentPassword;

        if (!currentValid)
            return (false, "Current password is incorrect.");

        // Check password history to prevent reuse (compliance feature)
        if (_complianceConfig?.Enabled == true)
        {
            var passwordHistory = _currentUserConfig.GetPasswordHistory();

            foreach (var oldHash in passwordHistory)
            {
                if (PasswordHasher.Verify(newPassword, oldHash))
                {
                    _logger?.LogWarning("User {Username} attempted to reuse a previous password", _currentUserConfig.Username);
                    return (false, $"You cannot reuse any of your last {_complianceConfig.PasswordHistoryCount} passwords.");
                }
            }

            // Add current password to history before changing
            if (!string.IsNullOrEmpty(_currentUserConfig.PasswordHash))
            {
                _currentUserConfig.AddPasswordToHistory(_currentUserConfig.PasswordHash, _complianceConfig.PasswordHistoryCount);
            }
        }

        _currentUserConfig.PasswordHash = PasswordHasher.Hash(newPassword);
        OpcPassword = newPassword;
        _currentUserConfig.Password = ""; // Clear legacy plain-text password
        _currentUserConfig.PasswordChangedDate = DateTime.UtcNow;
        MustChangePassword = false;

        _logger?.LogInformation("User {Username} changed their password", _currentUserConfig.Username);
        StateChanged?.Invoke();
        return (true, "Password changed successfully.");
    }

    public void Logout()
    {
        _logger?.LogInformation("User {Username} logged out", Username);

        IsAuthenticated = false;
        OpcPassword = null;
        Username = null;
        FullName = null;
        Group = null;
        AccessLevel = null;
        CanAccessEditor = false;
        AutoLogOffSeconds = 0;
        MustChangePassword = false;
        _currentUserConfig = null;
        StateChanged?.Invoke();
    }

    /// <summary>
    /// Get the current user configuration.
    /// </summary>
    public UserConfig? GetCurrentUser() => _currentUserConfig;

    /// <summary>
    /// Check if compliance mode is enabled.
    /// </summary>
    public bool IsComplianceModeEnabled() => _complianceConfig?.Enabled ?? false;

    /// <summary>Check if login is required based on server settings and user list.</summary>
    public static bool IsLoginRequired(NodeModel? model)
    {
        if (model?.Server == null) return false;
        return model.Server.EnableEditorLogin && model.Users.Count > 0;
    }
}
