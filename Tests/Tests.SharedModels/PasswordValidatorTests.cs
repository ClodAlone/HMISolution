// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Xunit;
using SharedModels;

namespace Tests.SharedModels;

public class PasswordValidatorTests
{
    // ─── Weak mode (requireStrong = false) ───────────────────

    [Fact]
    public void WeakMode_EmptyPassword_Fails()
    {
        var (valid, error) = PasswordValidator.Validate("", false);
        Assert.False(valid);
        Assert.Contains("empty", error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WeakMode_TooShort_Fails()
    {
        var (valid, _) = PasswordValidator.Validate("abc", false);
        Assert.False(valid);
    }

    [Fact]
    public void WeakMode_FourChars_Passes()
    {
        var (valid, error) = PasswordValidator.Validate("abcd", false);
        Assert.True(valid);
        Assert.Null(error);
    }

    [Fact]
    public void WeakMode_LongSimplePassword_Passes()
    {
        var (valid, _) = PasswordValidator.Validate("simplelong", false);
        Assert.True(valid);
    }

    // ─── Strong mode (requireStrong = true) ──────────────────

    [Fact]
    public void StrongMode_TooShort_Fails()
    {
        var (valid, error) = PasswordValidator.Validate("Aa1!xyz", true);
        Assert.False(valid);
        Assert.Contains("8 characters", error!);
    }

    [Fact]
    public void StrongMode_NoUppercase_Fails()
    {
        var (valid, error) = PasswordValidator.Validate("abcdefg1!", true);
        Assert.False(valid);
        Assert.Contains("uppercase", error!);
    }

    [Fact]
    public void StrongMode_NoLowercase_Fails()
    {
        var (valid, error) = PasswordValidator.Validate("ABCDEFG1!", true);
        Assert.False(valid);
        Assert.Contains("lowercase", error!);
    }

    [Fact]
    public void StrongMode_NoDigit_Fails()
    {
        var (valid, error) = PasswordValidator.Validate("Abcdefgh!", true);
        Assert.False(valid);
        Assert.Contains("digit", error!);
    }

    [Fact]
    public void StrongMode_NoSpecialChar_Fails()
    {
        var (valid, error) = PasswordValidator.Validate("Abcdefg1x", true);
        Assert.False(valid);
        Assert.Contains("special", error!);
    }

    [Fact]
    public void StrongMode_ValidPassword_Passes()
    {
        var (valid, error) = PasswordValidator.Validate("Str0ng!Pass", true);
        Assert.True(valid);
        Assert.Null(error);
    }

    // ─── Requirements text ───────────────────────────────────

    [Fact]
    public void GetRequirementsText_WeakMode()
    {
        var text = PasswordValidator.GetRequirementsText(false);
        Assert.Contains("4 characters", text);
    }

    [Fact]
    public void GetRequirementsText_StrongMode()
    {
        var text = PasswordValidator.GetRequirementsText(true);
        Assert.Contains("8 characters", text);
        Assert.Contains("uppercase", text);
    }
}
