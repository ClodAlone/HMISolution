// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Security.Cryptography;

namespace SharedModels;

/// <summary>
/// PBKDF2-SHA256 password hashing with embedded salt.
/// Format: Base64( salt[16] + hash[32] )
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <summary>Hash a plain-text password and return a Base64-encoded string.</summary>
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        var result = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);

        return Convert.ToBase64String(result);
    }

    /// <summary>Verify a plain-text password against a stored hash.</summary>
    public static bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(storedHash)) return false;

        byte[] data;
        try { data = Convert.FromBase64String(storedHash); }
        catch { return false; }

        if (data.Length != SaltSize + HashSize) return false;

        var salt = data[..SaltSize];
        var expectedHash = data[SaltSize..];

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
