using SharedModels;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ServerEditorWeb.Services;

/// <summary>
/// License management and feature gating service for Community/Professional/Enterprise editions
/// </summary>
public class EditionLicenseManager
{
    private LicenseConfig _currentLicense;
    private readonly string _licenseFilePath;
    private const string DefaultLicenseFile = "license.json";

    public event EventHandler<LicenseConfig>? LicenseChanged;

    public EditionLicenseManager()
    {
        _licenseFilePath = Path.Combine(AppContext.BaseDirectory, DefaultLicenseFile);
        _currentLicense = LoadLicense();
    }

    /// <summary>
    /// Get current active license
    /// </summary>
    public LicenseConfig CurrentLicense => _currentLicense;

    /// <summary>
    /// Get current license edition
    /// </summary>
    public LicenseEdition Edition => _currentLicense.Edition;

    /// <summary>
    /// Check if a specific feature is enabled
    /// </summary>
    public bool IsFeatureEnabled(string feature)
    {
        // If license is expired or invalid, only allow Community features
        if (_currentLicense.State != LicenseState.Active && 
            _currentLicense.State != LicenseState.Trial)
        {
            return LicenseFeatures.GetDefaultFeatures(LicenseEdition.Community).Contains(feature);
        }

        // Check if feature is explicitly enabled in license
        if (_currentLicense.EnabledFeatures.Contains(feature))
            return true;

        // Fall back to edition defaults
        return LicenseFeatures.IsFeatureAvailable(_currentLicense.Edition, feature);
    }

    /// <summary>
    /// Check if current edition is at least the specified tier
    /// </summary>
    public bool IsEditionOrHigher(LicenseEdition requiredEdition)
    {
        return _currentLicense.Edition >= requiredEdition;
    }

    /// <summary>
    /// Get user-friendly license description
    /// </summary>
    public string GetLicenseDescription()
    {
        if (_currentLicense.Edition == LicenseEdition.Community)
            return "AI Core HMI Community Edition - Free Forever";

        var edition = _currentLicense.Edition.ToString();
        var state = _currentLicense.State.ToString();

        if (_currentLicense.IsTrial)
        {
            var daysLeft = GetDaysUntilExpiration();
            return $"{edition} Edition - Trial ({daysLeft} days remaining)";
        }

        if (_currentLicense.ExpirationDate.HasValue)
        {
            var daysLeft = GetDaysUntilExpiration();
            if (daysLeft < 0)
                return $"{edition} Edition - Expired";
            if (daysLeft < 30)
                return $"{edition} Edition - Expires in {daysLeft} days";
            return $"{edition} Edition - Active";
        }

        return $"{edition} Edition - Perpetual License";
    }

    /// <summary>
    /// Get days until license expiration (-1 if perpetual, negative if expired)
    /// </summary>
    public int GetDaysUntilExpiration()
    {
        var expirationDate = _currentLicense.IsTrial 
            ? _currentLicense.TrialExpirationDate 
            : _currentLicense.ExpirationDate;

        if (!expirationDate.HasValue)
            return -1; // Perpetual

        return (int)(expirationDate.Value - DateTime.UtcNow).TotalDays;
    }

    /// <summary>
    /// Validate current license
    /// </summary>
    public LicenseValidationResult Validate()
    {
        var result = new LicenseValidationResult
        {
            ValidatedAt = DateTime.UtcNow,
            DaysUntilExpiration = GetDaysUntilExpiration()
        };

        // Community edition is always valid
        if (_currentLicense.Edition == LicenseEdition.Community)
        {
            result.IsValid = true;
            result.State = LicenseState.Active;
            result.Message = "Community Edition - No license required";
            return result;
        }

        // Check expiration
        if (_currentLicense.ExpirationDate.HasValue && 
            DateTime.UtcNow > _currentLicense.ExpirationDate.Value)
        {
            result.IsValid = false;
            result.State = LicenseState.Expired;
            result.Message = "License has expired. Please renew or revert to Community Edition.";
            _currentLicense.State = LicenseState.Expired;
            SaveLicense();
            return result;
        }

        // Check trial expiration
        if (_currentLicense.IsTrial && 
            _currentLicense.TrialExpirationDate.HasValue &&
            DateTime.UtcNow > _currentLicense.TrialExpirationDate.Value)
        {
            result.IsValid = false;
            result.State = LicenseState.Expired;
            result.Message = "Trial period has ended. Please purchase a license or use Community Edition.";
            _currentLicense.State = LicenseState.Expired;
            _currentLicense.Edition = LicenseEdition.Community; // Auto-downgrade
            SaveLicense();
            return result;
        }

        // Check if license key is present
        if (string.IsNullOrEmpty(_currentLicense.LicenseKey))
        {
            result.IsValid = false;
            result.State = LicenseState.Invalid;
            result.Message = "No valid license key found.";
            return result;
        }

        // Warnings for upcoming expiration
        if (result.DaysUntilExpiration >= 0 && result.DaysUntilExpiration < 30)
        {
            result.Warnings.Add($"License expires in {result.DaysUntilExpiration} days");
        }

        result.IsValid = true;
        result.State = _currentLicense.State;
        result.Message = $"{_currentLicense.Edition} Edition - Active";
        _currentLicense.LastValidated = DateTime.UtcNow;
        SaveLicense();

        return result;
    }

    /// <summary>
    /// Activate a license key
    /// </summary>
    public async Task<LicenseActivationResponse> ActivateLicenseAsync(string licenseKey, string email)
    {
        // In production, this would call a license server API
        // For now, we'll do basic validation and offline activation

        var response = new LicenseActivationResponse();

        if (string.IsNullOrWhiteSpace(licenseKey))
        {
            response.Success = false;
            response.Message = "License key is required";
            return response;
        }

        // Parse license key format: EDITION-GUID-SIGNATURE
        var parts = licenseKey.Split('-');
        if (parts.Length < 3)
        {
            response.Success = false;
            response.Message = "Invalid license key format";
            return response;
        }

        var edition = parts[0].ToLowerInvariant() switch
        {
            "pro" or "professional" => LicenseEdition.Professional,
            "ent" or "enterprise" => LicenseEdition.Enterprise,
            "trial" => LicenseEdition.Professional,
            _ => LicenseEdition.Community
        };

        var isTrial = parts[0].ToLowerInvariant() == "trial";

        // Create new license
        var newLicense = new LicenseConfig
        {
            LicenseKey = licenseKey,
            Edition = edition,
            State = LicenseState.Active,
            LicensedTo = email.Split('@').FirstOrDefault() ?? "User",
            Email = email,
            IssuedDate = DateTime.UtcNow,
            IsTrial = isTrial,
            OfflineActivation = true,
            EnabledFeatures = LicenseFeatures.GetDefaultFeatures(edition)
        };

        if (isTrial)
        {
            newLicense.TrialExpirationDate = DateTime.UtcNow.AddDays(14);
        }

        // For Professional/Enterprise, this would validate with server
        // For now, accept offline activation
        _currentLicense = newLicense;
        SaveLicense();
        LicenseChanged?.Invoke(this, _currentLicense);

        response.Success = true;
        response.Message = $"Successfully activated {edition} Edition";
        response.License = newLicense;

        return response;
    }

    /// <summary>
    /// Start a free trial
    /// </summary>
    public LicenseActivationResponse StartTrial(LicenseEdition edition = LicenseEdition.Professional)
    {
        if (_currentLicense.IsTrial || _currentLicense.Edition != LicenseEdition.Community)
        {
            return new LicenseActivationResponse
            {
                Success = false,
                Message = "Trial already activated or license already active"
            };
        }

        var trialKey = $"TRIAL-{Guid.NewGuid():N}-{GenerateSignature()}";
        var trial = new LicenseConfig
        {
            LicenseKey = trialKey,
            Edition = edition,
            State = LicenseState.Trial,
            LicensedTo = "Trial User",
            IssuedDate = DateTime.UtcNow,
            IsTrial = true,
            TrialExpirationDate = DateTime.UtcNow.AddDays(14),
            EnabledFeatures = LicenseFeatures.GetDefaultFeatures(edition)
        };

        _currentLicense = trial;
        SaveLicense();
        LicenseChanged?.Invoke(this, _currentLicense);

        return new LicenseActivationResponse
        {
            Success = true,
            Message = $"14-day {edition} trial started",
            License = trial
        };
    }

    /// <summary>
    /// Deactivate license and revert to Community
    /// </summary>
    public void DeactivateLicense()
    {
        _currentLicense = new LicenseConfig
        {
            Edition = LicenseEdition.Community,
            State = LicenseState.Active,
            EnabledFeatures = LicenseFeatures.GetDefaultFeatures(LicenseEdition.Community)
        };
        SaveLicense();
        LicenseChanged?.Invoke(this, _currentLicense);
    }

    /// <summary>
    /// Get feature gate message for disabled features
    /// </summary>
    public string GetFeatureGateMessage(string feature, LicenseEdition requiredEdition)
    {
        return $"This feature requires {requiredEdition} Edition or higher. " +
               $"Current edition: {_currentLicense.Edition}. " +
               $"Upgrade at https://clodalone.github.io/HMISolution/pricing";
    }

    private LicenseConfig LoadLicense()
    {
        try
        {
            if (File.Exists(_licenseFilePath))
            {
                var json = File.ReadAllText(_licenseFilePath);
                var license = JsonSerializer.Deserialize<LicenseConfig>(json);
                if (license != null)
                {
                    // Ensure features are populated
                    if (license.EnabledFeatures.Count == 0)
                    {
                        license.EnabledFeatures = LicenseFeatures.GetDefaultFeatures(license.Edition);
                    }
                    return license;
                }
            }
        }
        catch
        {
            // Fall back to default
        }

        // Default to Community Edition
        return new LicenseConfig
        {
            Edition = LicenseEdition.Community,
            State = LicenseState.Active,
            EnabledFeatures = LicenseFeatures.GetDefaultFeatures(LicenseEdition.Community)
        };
    }

    private void SaveLicense()
    {
        try
        {
            var json = JsonSerializer.Serialize(_currentLicense, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(_licenseFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save license: {ex.Message}");
        }
    }

    private static string GenerateSignature()
    {
        var bytes = RandomNumberGenerator.GetBytes(8);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
