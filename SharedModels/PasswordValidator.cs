namespace SharedModels;

/// <summary>
/// Validates passwords against configurable strength rules.
/// </summary>
public static class PasswordValidator
{
    /// <summary>
    /// Validate a password. Returns (isValid, errorMessage).
    /// When <paramref name="requireStrong"/> is false, only a minimum length of 4 is enforced.
    /// When true, the password must be at least 8 characters and contain uppercase, lowercase, digit, and special character.
    /// </summary>
    public static (bool IsValid, string? Error) Validate(string password, bool requireStrong)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password cannot be empty.");

        if (!requireStrong)
        {
            if (password.Length < 4)
                return (false, "Password must be at least 4 characters.");

            return (true, null);
        }

        // Strong password rules
        if (password.Length < 8)
            return (false, "Password must be at least 8 characters.");

        if (!password.Any(char.IsUpper))
            return (false, "Password must contain at least one uppercase letter.");

        if (!password.Any(char.IsLower))
            return (false, "Password must contain at least one lowercase letter.");

        if (!password.Any(char.IsDigit))
            return (false, "Password must contain at least one digit.");

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            return (false, "Password must contain at least one special character.");

        return (true, null);
    }

    /// <summary>
    /// Returns a human-readable description of the password requirements.
    /// </summary>
    public static string GetRequirementsText(bool requireStrong)
    {
        if (!requireStrong)
            return "Minimum 4 characters.";

        return "Minimum 8 characters, with at least one uppercase letter, one lowercase letter, one digit, and one special character.";
    }

    /// <summary>
    /// Validate a password with custom minimum length. Returns (isValid, errorMessage).
    /// When <paramref name="requireStrong"/> is true, enforces uppercase, lowercase, digit, and special character.
    /// </summary>
    public static (bool IsValid, string? Error) Validate(string password, bool requireStrong, int minLength)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password cannot be empty.");

        if (password.Length < minLength)
            return (false, $"Password must be at least {minLength} characters.");

        if (!requireStrong)
            return (true, null);

        // Strong password rules
        if (!password.Any(char.IsUpper))
            return (false, "Password must contain at least one uppercase letter.");

        if (!password.Any(char.IsLower))
            return (false, "Password must contain at least one lowercase letter.");

        if (!password.Any(char.IsDigit))
            return (false, "Password must contain at least one digit.");

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            return (false, "Password must contain at least one special character.");

        return (true, null);
    }
}
