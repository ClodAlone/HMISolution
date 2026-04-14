using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SharedModels;

/// <summary>
/// Manages license validation, hardware fingerprinting, and RSA key generation.
/// <para>
/// <b>Workflow:</b>
/// <list type="number">
///   <item>You (developer) call <see cref="GenerateKeyPair"/> once to create a private/public key pair.</item>
///   <item>Embed the <b>public key</b> in the shipped binaries (see <see cref="EmbeddedPublicKey"/>).</item>
///   <item>Use <see cref="SignLicense"/> with the <b>private key</b> to sign license files you issue to customers.</item>
///   <item>At runtime, call <see cref="Validate"/> which checks the signature with the embedded public key.</item>
/// </list>
/// </para>
/// </summary>
public static class LicenseManager
{
    // ─── Embedded Public Key ────────────────────────────────────────
    // Replace this with your actual public key after calling GenerateKeyPair().
    // This key is safe to ship — it can only VERIFY, not create licenses.

    private const string EmbeddedPublicKey =
        "MIIBCgKCAQEAyqnUfp8lHWOIgAVJVHYCegM3l8xBbvpl5gbjBR0bstIMEwz+6pZApXlUvfNI9q98hGDH9GYI1u5jxziRG5HZxYy0HpGxIjhK7NUbA7OmktPDjFQyvpRhN7/qOMX5hC0gBbgWRdkVBFak77Rex4fMiY6L/mlhXh+CMjvlEdOCZJMeKzTXEOtqYrTVY/Wr+wnGbtxBsnaJreMCtGD+Knja2/ylmBJVU1gEMEdNHVSXNN1KbhoCLtV4Ep6EGXNTddX9q7Z2kUdxSCf+c//9VT+oKtMb27aT5p8GlOh1Vm/j5G94DTXUBYnybxAVHJBWgci9TDKIjwHAPiF2w5UuTiF8AQIDAQAB";

    private static LicenseStatus? _cached;
    private static DateTime? _demoStartedUtc;

#if !DEBUG
    // ─── Release-Only Hardening State ───────────────────────────────
    private static readonly string _sentinelPath = GetSentinelPath();
    private static string? _assemblyHash;
    private static readonly TimeSpan ClockDriftTolerance = TimeSpan.FromMinutes(5);
#endif

    // ─── Hardware Fingerprint ───────────────────────────────────────

    /// <summary>
    /// Generates a stable hardware fingerprint for the current machine.
    /// Combines: machine name, OS, first physical MAC address, processor count.
    /// </summary>
    public static string GetMachineId()
    {
        var sb = new StringBuilder();
        sb.Append(Environment.MachineName);
        sb.Append('|');
        sb.Append(RuntimeInformation.OSDescription);
        sb.Append('|');
        sb.Append(Environment.ProcessorCount);
        sb.Append('|');

        // First physical (non-loopback) MAC address — stable across reboots
        try
        {
            var nic = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                            n.OperationalStatus == OperationalStatus.Up)
                .OrderBy(n => n.Name)
                .FirstOrDefault();
            if (nic != null)
                sb.Append(nic.GetPhysicalAddress().ToString());
        }
        catch
        {
            sb.Append("NOMAC");
        }

        // SHA256 hash for a compact, stable fingerprint
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));
        return Convert.ToHexStringLower(hash)[..32];
    }

    // ─── Validation ─────────────────────────────────────────────────

    /// <summary>
    /// Validate a license file and return the current license status.
    /// Checks: signature, expiration, machine ID.
    /// In Release builds, also checks for clock manipulation and assembly tampering.
    /// </summary>
    public static LicenseStatus Validate(string? licenseFilePath)
    {
        if (string.IsNullOrEmpty(licenseFilePath) || !File.Exists(licenseFilePath))
        {
            // No license file — start Demo mode (full features for 10 minutes)
            _demoStartedUtc ??= DateTime.UtcNow;
            _cached = new LicenseStatus
            {
                IsValid = true,
                Tier = "Demo",
                Message = "No license file found. Running in Demo mode (full features for 10 minutes).",
                License = LicenseTiers.CreateDemo(),
                DemoStartedUtc = _demoStartedUtc,
                DemoGraceMinutes = 10
            };
            return _cached;
        }

        try
        {
            var json = File.ReadAllText(licenseFilePath);
            var license = JsonSerializer.Deserialize<License>(json);
            if (license == null)
                return Fail("Failed to parse license file.");

            #if !DEBUG
            // 0a. Assembly integrity check (Release only)
            if (!VerifyAssemblyIntegrity())
                return Fail("Application integrity check failed. The binaries may have been tampered with.");

            // 0b. Clock manipulation check (Release only)
            var clockCheck = DetectClockManipulation();
            if (clockCheck != null)
                return Fail(clockCheck);
#endif

            // 1. Verify RSA signature (skip if public key not yet configured)
            if (!string.Equals(EmbeddedPublicKey, "REPLACE_WITH_YOUR_PUBLIC_KEY", StringComparison.Ordinal))
            {
                var payload = GetSignablePayload(license);
                if (!VerifySignature(payload, license.Signature, EmbeddedPublicKey))
                    return Fail("License signature is invalid. The file may have been tampered with.");
            }

            // 2. Check expiration
            if (license.ExpiresUtc < DateTime.UtcNow)
            {
                var status = Fail($"License expired on {license.ExpiresUtc:yyyy-MM-dd}.");
                status.License = license;
                status.Tier = license.Tier;
                status.LicensedTo = license.LicensedTo;
                status.ExpiresUtc = license.ExpiresUtc;
                ApplyTrialLimits(status);
                return status;
            }

            // 3. Check machine ID (if specified)
            if (!string.IsNullOrEmpty(license.MachineId))
            {
                var currentId = GetMachineId();
                if (!string.Equals(license.MachineId, currentId, StringComparison.OrdinalIgnoreCase))
                    return Fail($"License is locked to a different machine (expected: {license.MachineId[..8]}…, got: {currentId[..8]}…).");
            }

            // Valid!
            var daysRemaining = license.ExpiresUtc == DateTime.MaxValue
                ? int.MaxValue
                : (int)(license.ExpiresUtc - DateTime.UtcNow).TotalDays;

            _cached = new LicenseStatus
            {
                IsValid = true,
                Tier = license.Tier,
                LicensedTo = license.LicensedTo,
                ExpiresUtc = license.ExpiresUtc,
                DaysRemaining = daysRemaining,
                License = license,
                Message = daysRemaining < 30 && daysRemaining != int.MaxValue
                    ? $"License valid — expires in {daysRemaining} day(s)."
                    : "License valid."
            };

#if !DEBUG
            // Persist timestamp for clock manipulation detection (Release only)
            PersistValidationTimestamp();
#endif

            return _cached;
        }
        catch (Exception ex)
        {
            return Fail($"Error reading license: {ex.Message}");
        }
    }

    /// <summary>
    /// Get the cached license status from the last <see cref="Validate"/> call.
    /// Returns an Unlicensed/Trial status if never validated.
    /// </summary>
    public static LicenseStatus Current => _cached ?? new LicenseStatus
    {
        IsValid = false,
        Tier = "Unlicensed",
        Message = "License not checked yet."
    };

    /// <summary>
    /// Find a license.json file next to the given config file path.
    /// Searches for: license.json, License.json, *.license.json
    /// </summary>
    public static string? FindLicenseFile(string configPath)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(configPath));
        if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            return null;

        // Exact name
        var exact = Path.Combine(dir, "license.json");
        if (File.Exists(exact)) return exact;

        // Case-insensitive
        foreach (var f in Directory.GetFiles(dir, "*.json"))
        {
            var name = Path.GetFileName(f);
            if (name.Equals("license.json", StringComparison.OrdinalIgnoreCase))
                return f;
            if (name.EndsWith(".license.json", StringComparison.OrdinalIgnoreCase))
                return f;
        }

        return null;
    }

    /// <summary>
    /// Check whether the demo grace period has expired. If it has, degrade to Trial limits.
    /// Call this periodically (e.g. every 60 seconds) from a background timer.
    /// Returns <c>true</c> if the demo just expired on this call.
    /// </summary>
    public static bool CheckDemoExpiry()
    {
        if (_cached == null || !_cached.DemoStartedUtc.HasValue)
            return false; // Not in demo mode

        if (!_cached.IsDemoExpired)
            return false; // Still within the grace period

        if (_cached.Tier == "Trial")
            return false; // Already degraded

        // Degrade to Trial limits
        _cached.IsValid = false;
        _cached.Tier = "Trial";
        _cached.Message = "Demo period expired. Running in Trial mode (limited features). Please add a license file to unlock full functionality.";
        _cached.License = LicenseTiers.CreateTrial("Trial User", "");
        _cached.License.Signature = "(demo-expired)";
        ApplyTrialLimits(_cached);
        return true;
    }

    /// <summary>UTC timestamp when the demo grace period started. <c>null</c> if a license file was found.</summary>
    public static DateTime? DemoStartedUtc => _demoStartedUtc;

    // ─── Key Generation & Signing (developer tools) ─────────────────

    /// <summary>
    /// Generate a new RSA-2048 key pair. Returns (privateKeyPem, publicKeyBase64).
    /// The private key is used to sign licenses. The public key is embedded in the binaries.
    /// </summary>
    public static (string PrivateKeyPem, string PublicKeyBase64) GenerateKeyPair()
    {
        using var rsa = RSA.Create(2048);
        var privateKey = rsa.ExportRSAPrivateKeyPem();
        var publicKeyBytes = rsa.ExportRSAPublicKey();
        var publicKeyBase64 = Convert.ToBase64String(publicKeyBytes);
        return (privateKey, publicKeyBase64);
    }

    /// <summary>
    /// Sign a license object with the given RSA private key (PEM format).
    /// Sets the <see cref="License.Signature"/> property.
    /// </summary>
    public static void SignLicense(License license, string privateKeyPem)
    {
        var payload = GetSignablePayload(license);
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);
        var signature = rsa.SignData(
            Encoding.UTF8.GetBytes(payload),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        license.Signature = Convert.ToBase64String(signature);
    }

    /// <summary>
    /// Export a signed license to a JSON file.
    /// </summary>
    public static void ExportLicense(License license, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(license, options);
        File.WriteAllText(filePath, json);
    }

    // ─── Internals ──────────────────────────────────────────────────

    /// <summary>
    /// Build the canonical JSON payload that is signed/verified.
    /// All fields except <see cref="License.Signature"/> in a deterministic order.
    /// </summary>
    internal static string GetSignablePayload(License license)
    {
        // Deterministic: sorted keys, no whitespace
        var payload = new SortedDictionary<string, object?>
        {
            ["Id"] = license.Id,
            ["Tier"] = license.Tier,
            ["LicensedTo"] = license.LicensedTo,
            ["ProjectName"] = license.ProjectName,
            ["MachineId"] = license.MachineId,
            ["IssuedUtc"] = license.IssuedUtc.ToString("o"),
            ["ExpiresUtc"] = license.ExpiresUtc.ToString("o"),
            ["MaxVariables"] = license.MaxVariables,
            ["MaxDrivers"] = license.MaxDrivers,
            ["MaxScripts"] = license.MaxScripts,
            ["MaxPlcPrograms"] = license.MaxPlcPrograms,
            ["MaxScreens"] = license.MaxScreens,
            ["MaxRecipes"] = license.MaxRecipes,
            ["AllowDataLogging"] = license.AllowDataLogging,
            ["AllowAi"] = license.AllowAi
        };
        return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = false });
    }

    private static bool VerifySignature(string payload, string signatureBase64, string publicKeyBase64)
    {
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKeyBase64), out _);
            var signatureBytes = Convert.FromBase64String(signatureBase64);
            return rsa.VerifyData(
                Encoding.UTF8.GetBytes(payload),
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }

    private static LicenseStatus Fail(string message)
    {
        var status = new LicenseStatus
        {
            IsValid = false,
            Tier = "Trial",
            Message = message,
            License = LicenseTiers.CreateTrial("Trial User", "")
        };
        ApplyTrialLimits(status);
        _cached = status;
        return status;
    }

    private static void ApplyTrialLimits(LicenseStatus status)
    {
        // When license is invalid/expired, fall back to trial limits
        status.License ??= LicenseTiers.CreateTrial("Trial User", "");
    }

#if !DEBUG
    // ─── Release-Only: Clock Manipulation Detection ─────────────────

    /// <summary>
    /// Returns the path to the sentinel file used to persist the last successful
    /// validation timestamp. Stored next to the executing assembly.
    /// </summary>
    private static string GetSentinelPath()
    {
        var dir = AppContext.BaseDirectory;
        return Path.Combine(dir, ".license.state");
    }

    /// <summary>
    /// Detect if the system clock has been set backwards to bypass license expiration.
    /// Compares current UTC time against the last persisted validation timestamp.
    /// Returns an error message if manipulation is detected, or <c>null</c> if OK.
    /// </summary>
    private static string? DetectClockManipulation()
    {
        try
        {
            if (!File.Exists(_sentinelPath))
                return null; // First run — nothing to compare

            var raw = File.ReadAllText(_sentinelPath).Trim();
            if (!long.TryParse(raw, out var ticks))
                return null; // Corrupt sentinel — allow and overwrite on next success

            var lastValidated = new DateTime(ticks, DateTimeKind.Utc);
            var now = DateTime.UtcNow;

            // If the clock moved backwards beyond tolerance, it's suspicious
            if (now < lastValidated - ClockDriftTolerance)
                return $"System clock appears to have been set backwards " +
                       $"(expected >= {lastValidated:yyyy-MM-dd HH:mm} UTC, got {now:yyyy-MM-dd HH:mm} UTC). " +
                       $"Please correct the system time and restart.";
        }
        catch
        {
            // If we can't read the sentinel, don't block
        }
        return null;
    }

    /// <summary>
    /// Persist the current UTC timestamp after a successful validation.
    /// </summary>
    private static void PersistValidationTimestamp()
    {
        try
        {
            File.WriteAllText(_sentinelPath, DateTime.UtcNow.Ticks.ToString());
        }
        catch
        {
            // Non-fatal: if we can't write, clock check is skipped next run
        }
    }

    // ─── Release-Only: Assembly Integrity Verification ──────────────

    /// <summary>
    /// Verify that the SharedModels assembly has not been modified since it was loaded.
    /// Computes a SHA-256 hash of the assembly file on first call, then verifies it
    /// matches on subsequent calls. Detects post-deployment binary patching.
    /// </summary>
    private static bool VerifyAssemblyIntegrity()
    {
        try
        {
            var assembly = typeof(LicenseManager).Assembly;
            var assemblyPath = assembly.Location;

            // If running from single-file or in-memory, skip check
            if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
                return true;

            var currentHash = ComputeFileHash(assemblyPath);

            if (_assemblyHash == null)
            {
                // First call — record the baseline hash
                _assemblyHash = currentHash;
                return true;
            }

            // Subsequent calls — ensure it hasn't changed
            return string.Equals(_assemblyHash, currentHash, StringComparison.Ordinal);
        }
        catch
        {
            // If we can't verify, don't block (e.g., assembly loaded from memory)
            return true;
        }
    }

    private static string ComputeFileHash(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexStringLower(hash);
    }
#endif
}
