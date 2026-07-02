// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Xunit;
using RuntimeViewer.Shared.Services;
using SharedModels;

namespace Tests.RuntimeViewer;

public class LocalizationServiceTests
{
    private static ProjectService CreateProject(List<LocalizedStringEntry>? strings = null)
    {
        var project = new ProjectService();
        if (strings != null)
        {
            // Use reflection to set the model since Load requires a file
            var model = new NodeModel
            {
                Folder = new Folder { Name = "Root" },
                Strings = strings
            };
            var field = typeof(ProjectService).GetField("_model",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(project, model);
        }
        return project;
    }

    [Fact]
    public void Resolve_PlainLabel_ReturnedAsIs()
    {
        var project = CreateProject();
        var loc = new LocalizationService(project);

        Assert.Equal("Hello", loc.Resolve("Hello"));
    }

    [Fact]
    public void Resolve_NullOrEmpty_ReturnsEmpty()
    {
        var project = CreateProject();
        var loc = new LocalizationService(project);

        Assert.Equal("", loc.Resolve(null));
        Assert.Equal("", loc.Resolve(""));
    }

    [Fact]
    public void Resolve_StringId_ReturnsTranslation()
    {
        var strings = new List<LocalizedStringEntry>
        {
            new() { Key = "btn_start", Translations = new() { { "en", "Start" }, { "de", "Starten" } } }
        };
        var project = CreateProject(strings);
        var loc = new LocalizationService(project);

        loc.SetLanguage("en");
        Assert.Equal("Start", loc.Resolve("@btn_start"));

        loc.SetLanguage("de");
        Assert.Equal("Starten", loc.Resolve("@btn_start"));
    }

    [Fact]
    public void Resolve_MissingLanguage_FallsBackToFirstAvailable()
    {
        var strings = new List<LocalizedStringEntry>
        {
            new() { Key = "msg", Translations = new() { { "en", "Hello" } } }
        };
        var project = CreateProject(strings);
        var loc = new LocalizationService(project);

        loc.SetLanguage("fr"); // not available
        Assert.Equal("Hello", loc.Resolve("@msg")); // falls back to "en"
    }

    [Fact]
    public void Resolve_MissingKey_ReturnsKeyWithoutAt()
    {
        var strings = new List<LocalizedStringEntry>();
        var project = CreateProject(strings);
        var loc = new LocalizationService(project);

        Assert.Equal("unknown_key", loc.Resolve("@unknown_key"));
    }

    [Fact]
    public void SetLanguage_TriggersEvent()
    {
        var project = CreateProject();
        var loc = new LocalizationService(project);
        int fired = 0;
        loc.LanguageChanged += () => fired++;

        loc.SetLanguage("de");
        Assert.Equal(1, fired);
        Assert.Equal("de", loc.ActiveLanguage);
    }

    [Fact]
    public void SetLanguage_NormalizesToLowercase()
    {
        var project = CreateProject();
        var loc = new LocalizationService(project);

        loc.SetLanguage("DE");
        Assert.Equal("de", loc.ActiveLanguage);
    }

    [Fact]
    public void SetLanguage_EmptyOrNull_Ignored()
    {
        var project = CreateProject();
        var loc = new LocalizationService(project);

        loc.SetLanguage("fr");
        loc.SetLanguage("");
        Assert.Equal("fr", loc.ActiveLanguage);

        loc.SetLanguage(null!);
        Assert.Equal("fr", loc.ActiveLanguage);
    }

    [Fact]
    public void GetAvailableLanguages_ReturnsAllFromStrings()
    {
        var strings = new List<LocalizedStringEntry>
        {
            new() { Key = "a", Translations = new() { { "en", "A" }, { "de", "A" } } },
            new() { Key = "b", Translations = new() { { "en", "B" }, { "fr", "B" } } }
        };
        var project = CreateProject(strings);
        var loc = new LocalizationService(project);

        var langs = loc.GetAvailableLanguages();
        Assert.Contains("en", langs);
        Assert.Contains("de", langs);
        Assert.Contains("fr", langs);
    }

    [Fact]
    public void GetAvailableLanguages_NoStrings_ReturnsDefaultEn()
    {
        var project = CreateProject();
        var loc = new LocalizationService(project);

        var langs = loc.GetAvailableLanguages();
        Assert.Single(langs);
        Assert.Equal("en", langs[0]);
    }

    [Fact]
    public void DefaultLanguage_IsEnglish()
    {
        var project = CreateProject();
        var loc = new LocalizationService(project);
        Assert.Equal("en", loc.ActiveLanguage);
    }
}
