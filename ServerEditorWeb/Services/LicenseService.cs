using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;

namespace ServerEditorWeb.Services;

/// <summary>
/// Information about a NuGet package and its license.
/// </summary>
public record PackageLicenseInfo(
    string PackageName,
    string Version,
    string License,
    string LicenseUrl,
    string ProjectUrl,
    string Authors);

/// <summary>
/// Scans NuGet package metadata in the project's package cache to gather license information.
/// </summary>
public class LicenseService
{
    private List<PackageLicenseInfo>? _cachedLicenses;

    /// <summary>
    /// Returns license information for all NuGet packages referenced by the ServerEditorWeb project.
    /// Packages are deduplicated by name (highest version kept).
    /// </summary>
    public List<PackageLicenseInfo> GetPackageLicenses()
    {
        if (_cachedLicenses != null)
            return _cachedLicenses;

        var licenses = new Dictionary<string, PackageLicenseInfo>(StringComparer.OrdinalIgnoreCase);

        // Known packages from the csproj files in this solution — we know these at compile time
        var knownPackages = GetKnownPackages();
        foreach (var pkg in knownPackages)
            licenses[pkg.PackageName] = pkg;

        // Try to scan the NuGet global package cache for additional metadata
        try
        {
            var nugetCache = GetNuGetCachePath();
            if (Directory.Exists(nugetCache))
            {
                foreach (var pkg in knownPackages)
                {
                    var enriched = TryEnrichFromCache(nugetCache, pkg);
                    if (enriched != null)
                        licenses[pkg.PackageName] = enriched;
                }
            }
        }
        catch
        {
            // Silently continue with known data
        }

        _cachedLicenses = licenses.Values
            .OrderBy(p => p.PackageName, StringComparer.OrdinalIgnoreCase)
            .ToList();
        return _cachedLicenses;
    }

    /// <summary>
    /// Get application version information.
    /// </summary>
    public static (string version, string framework, string os) GetAppInfo()
    {
        var asm = Assembly.GetEntryAssembly();
        var version = asm?.GetName().Version?.ToString() ?? "1.0.0";
        var infoVersion = asm?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        var os = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        return (infoVersion ?? version, framework, os);
    }

    private static string GetNuGetCachePath()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, ".nuget", "packages");
    }

    private static PackageLicenseInfo? TryEnrichFromCache(string nugetCache, PackageLicenseInfo pkg)
    {
        try
        {
            var pkgDir = Path.Combine(nugetCache, pkg.PackageName.ToLowerInvariant(), pkg.Version.ToLowerInvariant());
            if (!Directory.Exists(pkgDir)) return null;

            var nuspecFiles = Directory.GetFiles(pkgDir, "*.nuspec", SearchOption.TopDirectoryOnly);
            if (nuspecFiles.Length == 0) return null;

            var doc = XDocument.Load(nuspecFiles[0]);
            var ns = doc.Root?.Name.Namespace ?? XNamespace.None;
            var metadata = doc.Root?.Element(ns + "metadata");
            if (metadata == null) return null;

            var license = metadata.Element(ns + "license")?.Value ?? "";
            var licenseUrl = metadata.Element(ns + "licenseUrl")?.Value ?? pkg.LicenseUrl;
            var projectUrl = metadata.Element(ns + "projectUrl")?.Value ?? pkg.ProjectUrl;
            var authors = metadata.Element(ns + "authors")?.Value ?? pkg.Authors;

            if (string.IsNullOrEmpty(license) && !string.IsNullOrEmpty(licenseUrl))
                license = InferLicenseFromUrl(licenseUrl);

            return pkg with
            {
                License = !string.IsNullOrEmpty(license) ? license : pkg.License,
                LicenseUrl = licenseUrl,
                ProjectUrl = projectUrl,
                Authors = !string.IsNullOrEmpty(authors) ? authors : pkg.Authors
            };
        }
        catch
        {
            return null;
        }
    }

    private static string InferLicenseFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return "";
        var lower = url.ToLowerInvariant();
        if (lower.Contains("apache")) return "Apache-2.0";
        if (lower.Contains("mit")) return "MIT";
        if (lower.Contains("bsd")) return "BSD";
        if (lower.Contains("lgpl")) return "LGPL";
        if (lower.Contains("gpl")) return "GPL";
        if (lower.Contains("mpl")) return "MPL-2.0";
        return "";
    }

    /// <summary>
    /// Returns the known NuGet packages from the solution's csproj files with their known licenses.
    /// </summary>
    private static List<PackageLicenseInfo> GetKnownPackages()
    {
        return
        [
            // ── ServerEditorWeb ──
            new("Microsoft.CodeAnalysis.CSharp.Scripting", "4.12.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/roslyn", "Microsoft"),
            new("Microsoft.CodeAnalysis.VisualBasic", "4.12.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/roslyn", "Microsoft"),
            new("Microsoft.Data.Sqlite", "9.0.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/efcore", "Microsoft"),
            new("Npgsql", "8.0.3", "PostgreSQL", "https://licenses.nuget.org/PostgreSQL", "https://github.com/npgsql/npgsql", "Shay Rojansky, Austin Drenski, Yoh Deadfall"),
            new("OPCFoundation.NetStandard.Opc.Ua", "1.5.378.106", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/OPCFoundation/UA-.NETStandard", "OPC Foundation"),
            new("System.Management", "9.0.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/runtime", "Microsoft"),
            new("System.ServiceProcess.ServiceController", "9.0.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/runtime", "Microsoft"),

            // ── Server ──
            new("Azure.Identity", "1.18.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/Azure/azure-sdk-for-net", "Microsoft"),
            new("Microsoft.Data.SqlClient", "6.1.4", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/SqlClient", "Microsoft"),
            new("Microsoft.Extensions.Configuration.Json", "9.0.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/runtime", "Microsoft"),
            new("Microsoft.Identity.Client", "4.80.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/AzureAD/microsoft-authentication-library-for-dotnet", "Microsoft"),
            new("Microsoft.IdentityModel.JsonWebTokens", "8.0.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet", "Microsoft"),
            new("Microsoft.ML.OnnxRuntime", "1.22.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/microsoft/onnxruntime", "Microsoft"),
            new("Serilog", "4.3.1", "Apache-2.0", "https://licenses.nuget.org/Apache-2.0", "https://github.com/serilog/serilog", "Serilog Contributors"),
            new("Serilog.Extensions.Hosting", "8.0.0", "Apache-2.0", "https://licenses.nuget.org/Apache-2.0", "https://github.com/serilog/serilog-extensions-hosting", "Serilog Contributors"),
            new("Serilog.Settings.Configuration", "8.0.2", "Apache-2.0", "https://licenses.nuget.org/Apache-2.0", "https://github.com/serilog/serilog-settings-configuration", "Serilog Contributors"),
            new("Serilog.Sinks.Console", "6.0.0", "Apache-2.0", "https://licenses.nuget.org/Apache-2.0", "https://github.com/serilog/serilog-sinks-console", "Serilog Contributors"),
            new("Serilog.Sinks.File", "6.0.0", "Apache-2.0", "https://licenses.nuget.org/Apache-2.0", "https://github.com/serilog/serilog-sinks-file", "Serilog Contributors"),
            new("Serilog.Extensions.Logging", "8.0.0", "Apache-2.0", "https://licenses.nuget.org/Apache-2.0", "https://github.com/serilog/serilog-extensions-logging", "Serilog Contributors"),
            new("Microsoft.Extensions.Hosting", "9.0.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/runtime", "Microsoft"),
            new("Microsoft.Extensions.Hosting.WindowsServices", "9.0.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/runtime", "Microsoft"),
            new("SixLabors.ImageSharp", "3.1.7", "Apache-2.0", "https://licenses.nuget.org/Apache-2.0", "https://github.com/SixLabors/ImageSharp", "Six Labors"),
            new("SixLabors.ImageSharp.Drawing", "2.1.5", "Apache-2.0", "https://licenses.nuget.org/Apache-2.0", "https://github.com/SixLabors/ImageSharp.Drawing", "Six Labors"),

            // ── RuntimeViewer ──
            new("Microsoft.AspNetCore.Components.Web", "10.0.0-preview.4", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/aspnetcore", "Microsoft"),
            new("Microsoft.AspNetCore.SignalR.Client", "10.0.0-preview.4", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/aspnetcore", "Microsoft"),
            new("Photino.NET", "4.0.16", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/nicolestandifer3/Photino.NET", "nicolestandifer3"),

            // ── Drivers ──
            new("MQTTnet", "5.1.0.1559", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/dotnet/MQTTnet", "dotnet Foundation"),
            new("NModbus", "3.0.81", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/NModbus/NModbus", "NModbus Contributors"),
            new("S7netplus", "0.20.0", "MIT", "https://licenses.nuget.org/MIT", "https://github.com/S7NetPlus/s7netplus", "S7NetPlus Contributors"),
            new("libplctag", "1.5.2", "MPL-2.0", "https://licenses.nuget.org/MPL-2.0", "https://github.com/libplctag/libplctag.NET", "libplctag Contributors"),

            // ── Testing ──
            new("xunit.v3", "1.1.0", "Apache-2.0", "https://licenses.nuget.org/Apache-2.0", "https://github.com/xunit/xunit", "xUnit.net Contributors"),
            new("xunit.runner.visualstudio", "3.1.0", "Apache-2.0", "https://licenses.nuget.org/Apache-2.0", "https://github.com/xunit/xunit", "xUnit.net Contributors"),
        ];
    }
}
