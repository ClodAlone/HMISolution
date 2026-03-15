using Xunit;
using SharedModels;

namespace Tests.SharedModels;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_ReturnsNonEmptyBase64()
    {
        var hash = PasswordHasher.Hash("TestPassword1!");
        Assert.False(string.IsNullOrEmpty(hash));
        // Must be valid Base64
        var bytes = Convert.FromBase64String(hash);
        Assert.Equal(48, bytes.Length); // 16 salt + 32 hash
    }

    [Fact]
    public void Hash_ProducesDifferentHashesForSamePassword()
    {
        var hash1 = PasswordHasher.Hash("SamePassword!");
        var hash2 = PasswordHasher.Hash("SamePassword!");
        // Different salts → different hashes
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        var hash = PasswordHasher.Hash("CorrectHorse!");
        Assert.True(PasswordHasher.Verify("CorrectHorse!", hash));
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        var hash = PasswordHasher.Hash("CorrectHorse!");
        Assert.False(PasswordHasher.Verify("WrongPassword", hash));
    }

    [Fact]
    public void Verify_EmptyStoredHash_ReturnsFalse()
    {
        Assert.False(PasswordHasher.Verify("any", ""));
        Assert.False(PasswordHasher.Verify("any", null!));
    }

    [Fact]
    public void Verify_InvalidBase64_ReturnsFalse()
    {
        Assert.False(PasswordHasher.Verify("any", "not-valid-base64!!!"));
    }

    [Fact]
    public void Verify_TruncatedHash_ReturnsFalse()
    {
        // Valid Base64, but wrong length
        var shortHash = Convert.ToBase64String(new byte[10]);
        Assert.False(PasswordHasher.Verify("any", shortHash));
    }
}
