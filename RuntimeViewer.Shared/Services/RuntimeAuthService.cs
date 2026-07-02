// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using SharedModels;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Scoped service tracking the currently authenticated runtime viewer user.
/// </summary>
public class RuntimeAuthService
{
    public bool IsAuthenticated { get; private set; }
    public string? Username { get; private set; }
    public string? Group { get; private set; }
    public string? AccessLevel { get; private set; }

    /// <summary>True when the user must change their password before using the app.</summary>
    public bool MustChangePassword { get; private set; }

    /// <summary>Auto log-off timeout in seconds for the current user. 0 = disabled.</summary>
    public int AutoLogOffSeconds { get; private set; }

    /// <summary>The current user's config reference (for persisting changes).</summary>
    private UserConfig? _currentUserConfig;

    public event Action? StateChanged;

    /// <summary>
    /// Check whether the runtime viewer requires login.
    /// </summary>
    public static bool IsLoginRequired(NodeModel? model)
    {
        if (model?.Server == null) return false;
        return model.Server.EnableRuntimeLogin && model.Users.Count > 0;
    }

    /// <summary>
    /// Attempt login against the project's user list.
    /// Returns (success, error message).
    /// </summary>
    public (bool Success, string Message) Login(string username, string password,
        List<UserConfig> users, List<UserGroupConfig> groups)
    {
        if (string.IsNullOrWhiteSpace(username))
            return (false, "Username is required.");

        var user = users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (user == null)
            return (false, "Invalid username or password.");

        // Try PasswordHash first, then fall back to legacy Password
        bool valid = false;
        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            valid = PasswordHasher.Verify(password, user.PasswordHash);
        }
        else if (!string.IsNullOrEmpty(user.Password))
        {
            valid = user.Password == password;
        }

        if (!valid)
            return (false, "Invalid username or password.");

        var group = groups.FirstOrDefault(g =>
            g.Name.Equals(user.Group, StringComparison.OrdinalIgnoreCase));

        if (group == null)
            return (false, $"User group '{user.Group}' not found.");

        if (!group.CanAccessRuntime)
            return (false, "Your user group does not have runtime viewer access.");

        IsAuthenticated = true;
        Username = user.Username;
        Group = user.Group;
        AccessLevel = group.AccessLevel;
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

        StateChanged?.Invoke();

        return (true, $"Welcome, {user.Username}!");
    }

    /// <summary>
    /// Authenticate an external (OAuth) user. Looks up the email in the local user list.
    /// If not found and a default group is configured, creates a transient session.
    /// </summary>
    public (bool Success, string Message) LoginExternal(
        string email, string displayName, string provider,
        List<UserConfig> users, List<UserGroupConfig> groups,
        ExternalAuthConfig authConfig)
    {
        if (string.IsNullOrWhiteSpace(email))
            return (false, "External login did not provide an email address.");

        // Check domain restriction on the provider
        var provCfg = authConfig.Providers.FirstOrDefault(p =>
            p.Name.Equals(provider, StringComparison.OrdinalIgnoreCase) && p.Enabled);
        if (provCfg != null && !string.IsNullOrWhiteSpace(provCfg.AllowedDomains))
        {
            var domain = email.Contains('@') ? email.Split('@')[1] : "";
            var allowed = provCfg.AllowedDomains
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!allowed.Any(d => d.Equals(domain, StringComparison.OrdinalIgnoreCase)))
                return (false, $"Email domain '{domain}' is not allowed for {provider} login.");
        }

        // Try to match to an existing local user by username (email)
        var user = users.FirstOrDefault(u =>
            u.Username.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (user != null)
        {
            var group = groups.FirstOrDefault(g =>
                g.Name.Equals(user.Group, StringComparison.OrdinalIgnoreCase));
            if (group == null)
                return (false, $"User group '{user.Group}' not found.");
            if (!group.CanAccessRuntime)
                return (false, "Your user group does not have runtime viewer access.");

            IsAuthenticated = true;
            Username = user.Username;
            Group = user.Group;
            AccessLevel = group.AccessLevel;
            AutoLogOffSeconds = user.AutoLogOffSeconds;
            MustChangePassword = false;
            _currentUserConfig = user;
            StateChanged?.Invoke();
            return (true, $"Welcome, {displayName}!");
        }

        // No local user — use default group if configured
        if (string.IsNullOrWhiteSpace(authConfig.DefaultGroup))
            return (false, $"No local account found for '{email}' and no default group is configured.");

        var defaultGroup = groups.FirstOrDefault(g =>
            g.Name.Equals(authConfig.DefaultGroup, StringComparison.OrdinalIgnoreCase));
        if (defaultGroup == null)
            return (false, $"Default group '{authConfig.DefaultGroup}' not found.");
        if (!defaultGroup.CanAccessRuntime)
            return (false, "The default group does not have runtime viewer access.");

        IsAuthenticated = true;
        Username = email;
        Group = defaultGroup.Name;
        AccessLevel = defaultGroup.AccessLevel;
        AutoLogOffSeconds = 0;
        MustChangePassword = false;
        _currentUserConfig = null; // transient — not in the local user list
        StateChanged?.Invoke();
        return (true, $"Welcome, {displayName}!");
    }

    /// <summary>
    /// Change the current user's password. Returns (success, message).
    /// </summary>
    public (bool Success, string Message) ChangePassword(string currentPassword, string newPassword, string confirmPassword, bool requireStrong = false)
    {
        if (_currentUserConfig == null)
            return (false, "No user is logged in.");

        if (string.IsNullOrWhiteSpace(newPassword))
            return (false, "New password cannot be empty.");

        if (newPassword != confirmPassword)
            return (false, "Passwords do not match.");

        var (valid, error) = PasswordValidator.Validate(newPassword, requireStrong);
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

        // Set new password
        _currentUserConfig.PasswordHash = PasswordHasher.Hash(newPassword);
        _currentUserConfig.Password = "";
        _currentUserConfig.PasswordChangedDate = DateTime.UtcNow;
        MustChangePassword = false;

        StateChanged?.Invoke();
        return (true, "Password changed successfully.");
    }

    public void Logout()
    {
        IsAuthenticated = false;
        Username = null;
        Group = null;
        AccessLevel = null;
        AutoLogOffSeconds = 0;
        MustChangePassword = false;
        _currentUserConfig = null;
        StateChanged?.Invoke();
    }

    /// <summary>
    /// Check if the current user satisfies a symbol's required access level.
    /// Returns true if no access restriction is set.
    /// </summary>
    public bool HasReadAccess(string requiredAccess)
    {
        if (string.IsNullOrEmpty(requiredAccess)) return true;
        if (!IsAuthenticated) return false;
        // Any authenticated user with Read, Write, or ReadWrite can read
        return AccessLevel is "Read" or "Write" or "ReadWrite";
    }

    /// <summary>
    /// Check if the current user has write access for command execution.
    /// </summary>
    public bool HasWriteAccess(string requiredAccess)
    {
        if (string.IsNullOrEmpty(requiredAccess)) return true;
        if (!IsAuthenticated) return false;
        return AccessLevel is "Write" or "ReadWrite";
    }

    /// <summary>
    /// Check if the current user's group is in the allowed groups list.
    /// Returns true if the list is empty (no restriction) or the user's group is included.
    /// When login is not required, always returns true.
    /// </summary>
    public bool IsGroupAllowed(List<string>? allowedGroups)
    {
        if (allowedGroups == null || allowedGroups.Count == 0)
            return true;
        if (!IsAuthenticated || string.IsNullOrEmpty(Group))
            return false;
        return allowedGroups.Any(g => g.Equals(Group, StringComparison.OrdinalIgnoreCase));
    }
}

