// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

namespace ServerEditorWeb.Services;

/// <summary>
/// Locates bundled sample projects both inside the Docker image, where the entrypoint
/// seeds them into <c>$HMI_DATA</c>, and on local development machines.
/// </summary>
public static class FeaturedSampleLocator
{
    public const string StyledInstrumentsName = "StyledInstruments";
    public const string DefaultDockerDataDirectory = "/data";

    /// <summary>Directory containing the seeded sample folders when running in Docker.</summary>
    public static string DockerDataDirectory(string? hmiData) =>
        string.IsNullOrWhiteSpace(hmiData) ? DefaultDockerDataDirectory : hmiData;

    public static string? FindStyledInstruments() =>
        Find(StyledInstrumentsName, ServerProcessService.IsDocker, Environment.GetEnvironmentVariable("HMI_DATA"),
            AppContext.BaseDirectory, File.Exists);

    public static string? Find(string sampleName, bool isDocker, string? hmiData, string baseDirectory, Func<string, bool> fileExists) =>
        Candidates(sampleName, isDocker, hmiData, baseDirectory).FirstOrDefault(fileExists);

    public static IEnumerable<string> Candidates(string sampleName, bool isDocker, string? hmiData, string baseDirectory)
    {
        if (isDocker)
        {
            // The persistent seeded copy is preferred so edits survive container restarts.
            yield return Path.Combine(DockerDataDirectory(hmiData), sampleName, "nodes.json");
            yield break;
        }

        if (!string.IsNullOrWhiteSpace(hmiData))
            yield return Path.Combine(hmiData, sampleName, "nodes.json");

        if (OperatingSystem.IsWindows())
            yield return Path.Combine(@"C:\Work\samples", sampleName, "nodes.json");

        // Development runs from bin/<config>/<tfm>; walk up to the repository's docker/samples.
        for (var dir = new DirectoryInfo(baseDirectory); dir != null; dir = dir.Parent)
            yield return Path.Combine(dir.FullName, "docker", "samples", sampleName, "nodes.json");
    }
}
