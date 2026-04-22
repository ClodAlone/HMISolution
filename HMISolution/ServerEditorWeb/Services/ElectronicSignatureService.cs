using SharedModels;
using System.Security.Cryptography;
using System.Text;

namespace ServerEditorWeb.Services;

/// <summary>
/// Service for managing electronic signatures per FDA 21 CFR Part 11 requirements.
/// Handles signature capture, verification, and validation for critical operations.
/// </summary>
public class ElectronicSignatureService
{
    private readonly AuthService _authService;
    private readonly ILogger<ElectronicSignatureService> _logger;
    private readonly ComplianceConfig _complianceConfig;

    // In-memory store for pending signature requests (would be better in a distributed cache for production)
    private readonly Dictionary<string, SignatureRequest> _pendingRequests = new();
    private readonly object _lock = new();

    public ElectronicSignatureService(
        AuthService authService,
        ILogger<ElectronicSignatureService> logger,
        ComplianceConfig complianceConfig)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _complianceConfig = complianceConfig ?? throw new ArgumentNullException(nameof(complianceConfig));
    }

    /// <summary>
    /// Check if a signature is required for the given event type.
    /// </summary>
    public bool IsSignatureRequired(ComplianceEventType eventType)
    {
        if (!_complianceConfig.Enabled || !_complianceConfig.RequireElectronicSignatures)
            return false;

        return _complianceConfig.SignatureRequiredEvents.Contains(eventType);
    }

    /// <summary>
    /// Check if dual signature is required for the given event type.
    /// </summary>
    public bool IsDualSignatureRequired(ComplianceEventType eventType)
    {
        if (!_complianceConfig.Enabled || !_complianceConfig.RequireDualSignatures)
            return false;

        return _complianceConfig.DualSignatureRequiredEvents.Contains(eventType);
    }

    /// <summary>
    /// Create a pending signature request.
    /// </summary>
    public string CreateSignatureRequest(SignatureRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var requestId = Guid.NewGuid().ToString();

        lock (_lock)
        {
            _pendingRequests[requestId] = request;
        }

        _logger.LogInformation("Created signature request {RequestId} for {EventType} on {AffectedEntity}",
            requestId, request.EventType, request.AffectedEntity);

        return requestId;
    }

    /// <summary>
    /// Get a pending signature request.
    /// </summary>
    public SignatureRequest? GetSignatureRequest(string requestId)
    {
        lock (_lock)
        {
            return _pendingRequests.TryGetValue(requestId, out var request) ? request : null;
        }
    }

    /// <summary>
    /// Complete a signature request with a single signature.
    /// </summary>
    public async Task<SignatureResult> CompleteSignatureAsync(
        string requestId,
        string username,
        string password,
        string meaning,
        string? comment,
        string? reasonForChange,
        List<UserConfig> users)
    {
        var request = GetSignatureRequest(requestId);
        if (request == null)
        {
            return new SignatureResult
            {
                Success = false,
                Message = "Signature request not found or has expired"
            };
        }

        // Verify user credentials
        var user = users.FirstOrDefault(u => 
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            _logger.LogWarning("Electronic signature attempt with invalid username: {Username}", username);
            return new SignatureResult
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        // Verify password
        bool isValid = false;
        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            isValid = PasswordHasher.Verify(password, user.PasswordHash);
        }
        else if (!string.IsNullOrEmpty(user.Password))
        {
            // Legacy plain-text (should not be used in compliance mode)
            isValid = user.Password == password;
            _logger.LogWarning("Electronic signature using legacy plain-text password for user: {Username}", username);
        }

        if (!isValid)
        {
            _logger.LogWarning("Electronic signature failed: Invalid password for user {Username}", username);
            return new SignatureResult
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        // Check if user is locked out
        if (user.IsLockedOut())
        {
            _logger.LogWarning("Electronic signature attempt from locked account: {Username}", username);
            return new SignatureResult
            {
                Success = false,
                Message = "Account is locked. Please contact an administrator."
            };
        }

        // Require reason for change if configured
        if (_complianceConfig.RequireReasonForChange && string.IsNullOrWhiteSpace(reasonForChange))
        {
            return new SignatureResult
            {
                Success = false,
                Message = "Reason for change is required"
            };
        }

        // Create signature
        var signature = new ElectronicSignature
        {
            Username = user.Username,
            FullName = user.FullName,
            SignedAt = DateTime.UtcNow,
            Meaning = meaning,
            Comment = comment,
            SignatureHash = HashPassword(password), // Store hashed password as proof
            IsVerified = true
        };

        // Check if dual signature is required
        if (request.RequiresDualSignature)
        {
            return new SignatureResult
            {
                Success = false,
                Message = "This operation requires a second signature for approval",
                Signature = signature
            };
        }

        // Remove pending request
        lock (_lock)
        {
            _pendingRequests.Remove(requestId);
        }

        _logger.LogInformation("Electronic signature completed by {Username} for {EventType} on {AffectedEntity}",
            username, request.EventType, request.AffectedEntity);

        return new SignatureResult
        {
            Success = true,
            Message = "Signature verified successfully",
            Signature = signature
        };
    }

    /// <summary>
    /// Complete a signature request with dual signatures (primary and approver).
    /// </summary>
    public async Task<SignatureResult> CompleteDualSignatureAsync(
        string requestId,
        string primaryUsername,
        string primaryPassword,
        string primaryMeaning,
        string? primaryComment,
        string secondaryUsername,
        string secondaryPassword,
        string secondaryMeaning,
        string? secondaryComment,
        string? reasonForChange,
        List<UserConfig> users)
    {
        var request = GetSignatureRequest(requestId);
        if (request == null)
        {
            return new SignatureResult
            {
                Success = false,
                Message = "Signature request not found or has expired"
            };
        }

        // Validate that both users are different
        if (primaryUsername.Equals(secondaryUsername, StringComparison.OrdinalIgnoreCase))
        {
            return new SignatureResult
            {
                Success = false,
                Message = "Dual signatures must be from different users"
            };
        }

        // Verify primary signature
        var primaryResult = await VerifySignatureAsync(primaryUsername, primaryPassword, primaryMeaning, 
            primaryComment, reasonForChange, users);
        if (!primaryResult.Success)
        {
            return primaryResult;
        }

        // Verify secondary signature
        var secondaryResult = await VerifySignatureAsync(secondaryUsername, secondaryPassword, secondaryMeaning,
            secondaryComment, reasonForChange, users);
        if (!secondaryResult.Success)
        {
            return secondaryResult;
        }

        // Remove pending request
        lock (_lock)
        {
            _pendingRequests.Remove(requestId);
        }

        _logger.LogInformation("Dual electronic signature completed by {Primary} and {Secondary} for {EventType} on {AffectedEntity}",
            primaryUsername, secondaryUsername, request.EventType, request.AffectedEntity);

        return new SignatureResult
        {
            Success = true,
            Message = "Dual signatures verified successfully",
            Signature = primaryResult.Signature,
            SecondSignature = secondaryResult.Signature
        };
    }

    /// <summary>
    /// Verify a single signature without completing a request.
    /// </summary>
    private async Task<SignatureResult> VerifySignatureAsync(
        string username,
        string password,
        string meaning,
        string? comment,
        string? reasonForChange,
        List<UserConfig> users)
    {
        var user = users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            _logger.LogWarning("Electronic signature verification failed: Unknown user {Username}", username);
            return new SignatureResult
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        // Verify password
        bool isValid = false;
        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            isValid = PasswordHasher.Verify(password, user.PasswordHash);
        }
        else if (!string.IsNullOrEmpty(user.Password))
        {
            isValid = user.Password == password;
            _logger.LogWarning("Electronic signature using legacy plain-text password for user: {Username}", username);
        }

        if (!isValid)
        {
            _logger.LogWarning("Electronic signature verification failed: Invalid password for user {Username}", username);
            return new SignatureResult
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        // Check if user is locked out
        if (user.IsLockedOut())
        {
            _logger.LogWarning("Electronic signature attempt from locked account: {Username}", username);
            return new SignatureResult
            {
                Success = false,
                Message = "Account is locked. Please contact an administrator."
            };
        }

        // Require reason for change if configured
        if (_complianceConfig.RequireReasonForChange && string.IsNullOrWhiteSpace(reasonForChange))
        {
            return new SignatureResult
            {
                Success = false,
                Message = "Reason for change is required"
            };
        }

        var signature = new ElectronicSignature
        {
            Username = user.Username,
            FullName = user.FullName,
            SignedAt = DateTime.UtcNow,
            Meaning = meaning,
            Comment = comment,
            SignatureHash = HashPassword(password),
            IsVerified = true
        };

        return new SignatureResult
        {
            Success = true,
            Message = "Signature verified",
            Signature = signature
        };
    }

    /// <summary>
    /// Cancel a pending signature request.
    /// </summary>
    public void CancelSignatureRequest(string requestId)
    {
        lock (_lock)
        {
            if (_pendingRequests.Remove(requestId))
            {
                _logger.LogInformation("Signature request {RequestId} cancelled", requestId);
            }
        }
    }

    /// <summary>
    /// Clean up expired signature requests (older than 30 minutes).
    /// </summary>
    public void CleanupExpiredRequests()
    {
        lock (_lock)
        {
            var expired = new List<string>();
            // Note: In production, you'd store timestamp with each request
            // For now, this is a placeholder for cleanup logic
            foreach (var key in expired)
            {
                _pendingRequests.Remove(key);
            }

            if (expired.Count > 0)
            {
                _logger.LogInformation("Cleaned up {Count} expired signature requests", expired.Count);
            }
        }
    }

    /// <summary>
    /// Hash a password for signature verification storage.
    /// </summary>
    private string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Verify that a signature was created with the correct credentials.
    /// </summary>
    public bool VerifySignatureAuthenticity(ElectronicSignature signature, string password)
    {
        if (signature == null || string.IsNullOrWhiteSpace(signature.SignatureHash))
            return false;

        var computedHash = HashPassword(password);
        return signature.SignatureHash == computedHash;
    }

    /// <summary>
    /// Get signature requirements for a specific event type.
    /// </summary>
    public string GetSignatureRequirements(ComplianceEventType eventType)
    {
        if (!_complianceConfig.Enabled)
            return "Compliance mode is disabled - signatures are not required";

        if (!IsSignatureRequired(eventType))
            return "No electronic signature required for this operation";

        if (IsDualSignatureRequired(eventType))
            return "This operation requires TWO electronic signatures (dual approval)";

        return "This operation requires ONE electronic signature";
    }
}
