using Xunit;
using ServerEditorWeb.Services;

namespace Tests.Editor;

/// <summary>
/// Tests for HelpService covering new help topics, TOC entries,
/// panel mappings, and localization for all 6 supported languages.
/// </summary>
public class HelpServiceTests
{
    private readonly HelpService _svc = new();

    // ──────────────────────────────────────────────────────────────
    //  New English help topics exist
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("certificates")]
    [InlineData("backups")]
    [InlineData("notifications")]
    [InlineData("tagbrowser")]
    [InlineData("restapi")]
    public void GetHelp_NewTopic_ReturnsContent(string topic)
    {
        var help = _svc.GetHelp(topic, "en");
        Assert.NotNull(help);
        Assert.False(string.IsNullOrWhiteSpace(help.Title));
        Assert.False(string.IsNullOrWhiteSpace(help.Body));
    }

    [Theory]
    [InlineData("certificates")]
    [InlineData("backups")]
    [InlineData("notifications")]
    [InlineData("tagbrowser")]
    [InlineData("restapi")]
    public void GetHelp_NewTopic_TitleIsNotWelcome(string topic)
    {
        var help = _svc.GetHelp(topic, "en");
        // Should NOT fall back to the welcome topic
        var welcome = _svc.GetHelp("welcome", "en");
        Assert.NotEqual(welcome.Title, help.Title);
    }

    // ──────────────────────────────────────────────────────────────
    //  Localized help topics exist for all 5 non-EN languages
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("de", "certificates")]
    [InlineData("de", "backups")]
    [InlineData("de", "notifications")]
    [InlineData("de", "tagbrowser")]
    [InlineData("de", "restapi")]
    [InlineData("it", "certificates")]
    [InlineData("it", "backups")]
    [InlineData("it", "notifications")]
    [InlineData("it", "tagbrowser")]
    [InlineData("it", "restapi")]
    [InlineData("fr", "certificates")]
    [InlineData("fr", "backups")]
    [InlineData("fr", "notifications")]
    [InlineData("fr", "tagbrowser")]
    [InlineData("fr", "restapi")]
    [InlineData("ja", "certificates")]
    [InlineData("ja", "backups")]
    [InlineData("ja", "notifications")]
    [InlineData("ja", "tagbrowser")]
    [InlineData("ja", "restapi")]
    [InlineData("zh", "certificates")]
    [InlineData("zh", "backups")]
    [InlineData("zh", "notifications")]
    [InlineData("zh", "tagbrowser")]
    [InlineData("zh", "restapi")]
    public void GetHelp_LocalizedTopic_ReturnsNonEnglishContent(string locale, string topic)
    {
        var help = _svc.GetHelp(topic, locale);
        Assert.NotNull(help);
        Assert.False(string.IsNullOrWhiteSpace(help.Title));
        Assert.False(string.IsNullOrWhiteSpace(help.Body));

        // Should differ from the English version
        var enHelp = _svc.GetHelp(topic, "en");
        Assert.NotEqual(enHelp.Title, help.Title);
    }

    // ──────────────────────────────────────────────────────────────
    //  Table of contents includes new entries
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("certificates")]
    [InlineData("backups")]
    [InlineData("notifications")]
    [InlineData("tagbrowser")]
    [InlineData("restapi")]
    public void GetTableOfContents_IncludesNewTopic(string topicKey)
    {
        var toc = _svc.GetTableOfContents("en");
        Assert.Contains(toc, t => t.Key == topicKey);
    }

    [Fact]
    public void GetTableOfContents_NewTopics_HaveTitlesAndIcons()
    {
        var toc = _svc.GetTableOfContents("en");
        var newKeys = new[] { "certificates", "backups", "notifications", "tagbrowser", "restapi" };
        foreach (var key in newKeys)
        {
            var entry = toc.FirstOrDefault(t => t.Key == key);
            Assert.False(string.IsNullOrWhiteSpace(entry.Title), $"Title missing for TOC key '{key}'");
            Assert.False(string.IsNullOrWhiteSpace(entry.Icon), $"Icon missing for TOC key '{key}'");
        }
    }

    // ──────────────────────────────────────────────────────────────
    //  Localized TOC titles
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("de")]
    [InlineData("it")]
    [InlineData("fr")]
    [InlineData("ja")]
    [InlineData("zh")]
    public void GetTableOfContents_LocalizedTitles_DifferFromEnglish(string locale)
    {
        var enToc = _svc.GetTableOfContents("en");
        var locToc = _svc.GetTableOfContents(locale);

        // Check that at least one new topic has a localized title
        var newKeys = new[] { "certificates", "backups", "notifications", "tagbrowser", "restapi" };
        int localizedCount = 0;
        foreach (var key in newKeys)
        {
            var enEntry = enToc.FirstOrDefault(t => t.Key == key);
            var locEntry = locToc.FirstOrDefault(t => t.Key == key);
            if (enEntry.Title != locEntry.Title)
                localizedCount++;
        }
        Assert.True(localizedCount >= 4, $"Expected at least 4 localized TOC titles for '{locale}', got {localizedCount}");
    }

    // ──────────────────────────────────────────────────────────────
    //  Panel mappings
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("certificates", "certificates")]
    [InlineData("backups", "backups")]
    public void GetHelpForPanel_MapsNewPanels(string panelId, string expectedKeyFragment)
    {
        var help = _svc.GetHelpForPanel(panelId, "en");
        Assert.NotNull(help);
        // Verify it returns the correct topic, not the fallback welcome
        var welcome = _svc.GetHelp("welcome", "en");
        Assert.NotEqual(welcome.Title, help.Title);
    }

    // ──────────────────────────────────────────────────────────────
    //  Fallback behavior
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void GetHelp_UnknownTopic_FallsBackToWelcome()
    {
        var help = _svc.GetHelp("nonexistent_topic_xyz", "en");
        var welcome = _svc.GetHelp("welcome", "en");
        Assert.Equal(welcome.Title, help.Title);
    }

    [Fact]
    public void GetHelp_UnsupportedLocale_FallsBackToEnglish()
    {
        var help = _svc.GetHelp("certificates", "xx");
        var enHelp = _svc.GetHelp("certificates", "en");
        Assert.Equal(enHelp.Title, help.Title);
    }

    [Fact]
    public void GetHelpForPanel_UnknownPanel_FallsBackToWelcome()
    {
        var help = _svc.GetHelpForPanel("unknown_panel_xyz", "en");
        var welcome = _svc.GetHelp("welcome", "en");
        Assert.Equal(welcome.Title, help.Title);
    }

    // ──────────────────────────────────────────────────────────────
    //  All existing topics still work (smoke test)
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("welcome")]
    [InlineData("project")]
    [InlineData("variables")]
    [InlineData("scripts")]
    [InlineData("server")]
    [InlineData("keyboard")]
    [InlineData("camera")]
    public void GetHelp_ExistingTopics_StillWork(string topic)
    {
        var help = _svc.GetHelp(topic, "en");
        Assert.NotNull(help);
        Assert.False(string.IsNullOrWhiteSpace(help.Title));
        Assert.False(string.IsNullOrWhiteSpace(help.Body));
    }
}
