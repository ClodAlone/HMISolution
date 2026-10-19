using ServerEditorWeb.Services;
using Xunit;

namespace Tests.Editor;

public class FeaturedSampleLocatorTests
{
    private const string Sample = FeaturedSampleLocator.StyledInstrumentsName;

    [Theory]
    [InlineData(null, "/data")]
    [InlineData("", "/data")]
    [InlineData("/srv/hmi", "/srv/hmi")]
    public void Docker_UsesSeededDataDirectory(string? hmiData, string expectedRoot)
    {
        var expected = Path.Combine(expectedRoot, Sample, "nodes.json");
        var candidates = FeaturedSampleLocator.Candidates(Sample, isDocker: true, hmiData, "/opt/hmi/editor").ToArray();

        Assert.Equal(expected, Assert.Single(candidates));
        Assert.Equal(expected, FeaturedSampleLocator.Find(Sample, true, hmiData, "/opt/hmi/editor", p => p == expected));
    }

    [Fact]
    public void Docker_ReturnsNullWhenSampleWasNotSeeded()
    {
        Assert.Null(FeaturedSampleLocator.Find(Sample, true, null, "/opt/hmi/editor", _ => false));
    }

    [Fact]
    public void Local_FindsRepositoryDockerSampleFromBuildOutput()
    {
        var repo = Path.Combine(Path.GetTempPath(), "repo");
        var bin = Path.Combine(repo, "ServerEditorWeb", "bin", "Debug", "net10.0");
        var expected = Path.Combine(repo, "docker", "samples", Sample, "nodes.json");

        Assert.Equal(expected, FeaturedSampleLocator.Find(Sample, false, null, bin, p => p == expected));
    }

    [Fact]
    public void Local_PrefersExplicitDataDirectoryOverRepositoryCopy()
    {
        var data = Path.Combine(Path.GetTempPath(), "hmi-data");
        var preferred = Path.Combine(data, Sample, "nodes.json");
        var bin = Path.Combine(Path.GetTempPath(), "repo", "bin");

        Assert.Equal(preferred, FeaturedSampleLocator.Find(Sample, false, data, bin, _ => true));
    }

    [Fact]
    public void RealRepositorySample_IsDiscoverableFromTestOutput()
    {
        var path = FeaturedSampleLocator.Find(Sample, false, null, AppContext.BaseDirectory, File.Exists);

        Assert.NotNull(path);
        Assert.EndsWith(Path.Combine(Sample, "nodes.json"), path);
    }
}
