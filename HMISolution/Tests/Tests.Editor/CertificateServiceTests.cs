using Xunit;
using ServerEditorWeb.Services;

namespace Tests.Editor;

/// <summary>
/// Tests for CertificateService model classes: CertificateInfo and GdsOperationResult.
/// </summary>
public class CertificateServiceTests
{
    // ──────────────────────────────────────────────────────────────
    //  CertificateInfo defaults
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CertificateInfo_DefaultValues()
    {
        var info = new CertificateInfo();
        Assert.Equal("", info.Thumbprint);
        Assert.Equal("", info.Subject);
        Assert.Equal("", info.Issuer);
        Assert.Equal(default, info.NotBefore);
        Assert.Equal(default, info.NotAfter);
        Assert.Equal("", info.SerialNumber);
        Assert.Equal("", info.ApplicationUri);
        Assert.NotNull(info.DomainNames);
        Assert.Empty(info.DomainNames);
        Assert.Equal("", info.StoreName);
        Assert.Equal("", info.FilePath);
        Assert.Equal(0, info.KeySizeBits);
    }

    // ──────────────────────────────────────────────────────────────
    //  CertificateInfo.IsExpired / IsNotYetValid
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CertificateInfo_IsExpired_WhenNotAfterInPast()
    {
        var info = new CertificateInfo
        {
            NotAfter = DateTime.UtcNow.AddDays(-1)
        };
        Assert.True(info.IsExpired);
    }

    [Fact]
    public void CertificateInfo_IsNotExpired_WhenNotAfterInFuture()
    {
        var info = new CertificateInfo
        {
            NotAfter = DateTime.UtcNow.AddYears(1)
        };
        Assert.False(info.IsExpired);
    }

    [Fact]
    public void CertificateInfo_IsNotYetValid_WhenNotBeforeInFuture()
    {
        var info = new CertificateInfo
        {
            NotBefore = DateTime.UtcNow.AddDays(1)
        };
        Assert.True(info.IsNotYetValid);
    }

    [Fact]
    public void CertificateInfo_IsValid_WhenInRange()
    {
        var info = new CertificateInfo
        {
            NotBefore = DateTime.UtcNow.AddDays(-30),
            NotAfter = DateTime.UtcNow.AddYears(1)
        };
        Assert.False(info.IsExpired);
        Assert.False(info.IsNotYetValid);
    }

    // ──────────────────────────────────────────────────────────────
    //  CertificateInfo property assignment
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CertificateInfo_SetProperties()
    {
        var info = new CertificateInfo
        {
            Thumbprint = "AABBCCDD",
            Subject = "CN=TestServer",
            Issuer = "CN=TestCA",
            NotBefore = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            NotAfter = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            SerialNumber = "001122",
            ApplicationUri = "urn:TestServer",
            DomainNames = ["localhost", "server.local"],
            StoreName = "Trusted",
            FilePath = "/pki/trusted/cert.der",
            KeySizeBits = 2048
        };

        Assert.Equal("AABBCCDD", info.Thumbprint);
        Assert.Equal("CN=TestServer", info.Subject);
        Assert.Equal("CN=TestCA", info.Issuer);
        Assert.Equal("001122", info.SerialNumber);
        Assert.Equal("urn:TestServer", info.ApplicationUri);
        Assert.Equal(2, info.DomainNames.Count);
        Assert.Contains("localhost", info.DomainNames);
        Assert.Equal("Trusted", info.StoreName);
        Assert.Equal(2048, info.KeySizeBits);
    }

    // ──────────────────────────────────────────────────────────────
    //  GdsOperationResult
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void GdsOperationResult_DefaultValues()
    {
        var result = new GdsOperationResult();
        Assert.False(result.Success);
        Assert.Equal("", result.Message);
    }

    [Fact]
    public void GdsOperationResult_SetProperties()
    {
        var result = new GdsOperationResult
        {
            Success = true,
            Message = "Application registered successfully."
        };
        Assert.True(result.Success);
        Assert.Equal("Application registered successfully.", result.Message);
    }
}
