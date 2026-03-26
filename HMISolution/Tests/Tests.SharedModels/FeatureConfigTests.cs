using Xunit;
using System.Text.Json;
using SharedModels;

namespace Tests.SharedModels;

/// <summary>
/// Tests for configuration model classes added with new features:
/// BackupConfig, GdsConfig, AlarmNotificationConfig, notification channels,
/// AlarmConfig.NotifyOnActivation, and VariableStatisticsConfig.
/// </summary>
public class FeatureConfigTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    // ──────────────────────────────────────────────────────────────
    //  BackupConfig
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void BackupConfig_Defaults()
    {
        var cfg = new BackupConfig();
        Assert.False(cfg.Enabled);
        Assert.True(cfg.SnapshotOnSave);
        Assert.Equal(0, cfg.ScheduleMinutes);
        Assert.Equal(20, cfg.MaxSnapshots);
    }

    [Fact]
    public void BackupConfig_Roundtrip()
    {
        var cfg = new BackupConfig
        {
            Enabled = true,
            SnapshotOnSave = false,
            ScheduleMinutes = 30,
            MaxSnapshots = 50
        };
        var json = JsonSerializer.Serialize(cfg, JsonOptions);
        var restored = JsonSerializer.Deserialize<BackupConfig>(json);

        Assert.NotNull(restored);
        Assert.True(restored.Enabled);
        Assert.False(restored.SnapshotOnSave);
        Assert.Equal(30, restored.ScheduleMinutes);
        Assert.Equal(50, restored.MaxSnapshots);
    }

    // ──────────────────────────────────────────────────────────────
    //  GdsConfig
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void GdsConfig_Defaults()
    {
        var cfg = new GdsConfig();
        Assert.Equal("", cfg.EndpointUrl);
        Assert.Equal("", cfg.UserName);
        Assert.Equal("", cfg.Password);
    }

    [Fact]
    public void GdsConfig_Roundtrip()
    {
        var cfg = new GdsConfig
        {
            EndpointUrl = "opc.tcp://gds.local:58810/GDS",
            UserName = "admin",
            Password = "secret"
        };
        var json = JsonSerializer.Serialize(cfg, JsonOptions);
        var restored = JsonSerializer.Deserialize<GdsConfig>(json);

        Assert.NotNull(restored);
        Assert.Equal("opc.tcp://gds.local:58810/GDS", restored.EndpointUrl);
        Assert.Equal("admin", restored.UserName);
        Assert.Equal("secret", restored.Password);
    }

    // ──────────────────────────────────────────────────────────────
    //  AlarmNotificationConfig
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void AlarmNotificationConfig_Defaults()
    {
        var cfg = new AlarmNotificationConfig();
        Assert.False(cfg.Enabled);
        Assert.Equal((ushort)1, cfg.MinSeverity);
        Assert.Equal(60, cfg.CooldownSeconds);
        Assert.Null(cfg.Email);
        Assert.Null(cfg.Telegram);
        Assert.Null(cfg.WhatsApp);
    }

    [Fact]
    public void AlarmNotificationConfig_Roundtrip_WithAllChannels()
    {
        var cfg = new AlarmNotificationConfig
        {
            Enabled = true,
            MinSeverity = 500,
            CooldownSeconds = 120,
            Email = new EmailNotificationChannel
            {
                Enabled = true,
                SmtpHost = "smtp.test.com",
                SmtpPort = 465,
                UseSsl = true,
                Username = "user",
                Password = "pass",
                From = "noreply@test.com",
                To = "ops@test.com"
            },
            Telegram = new TelegramNotificationChannel
            {
                Enabled = true,
                BotToken = "123456:ABC",
                ChatId = "-100123",
                UseHtml = false
            },
            WhatsApp = new WhatsAppNotificationChannel
            {
                Enabled = true,
                AccessToken = "token123",
                PhoneNumberId = "12345",
                To = "+1234567890",
                TemplateName = "alarm_template",
                TemplateLanguage = "en_US"
            }
        };

        var json = JsonSerializer.Serialize(cfg, JsonOptions);
        var restored = JsonSerializer.Deserialize<AlarmNotificationConfig>(json);

        Assert.NotNull(restored);
        Assert.True(restored.Enabled);
        Assert.Equal((ushort)500, restored.MinSeverity);
        Assert.Equal(120, restored.CooldownSeconds);

        // Email
        Assert.NotNull(restored.Email);
        Assert.True(restored.Email.Enabled);
        Assert.Equal("smtp.test.com", restored.Email.SmtpHost);
        Assert.Equal(465, restored.Email.SmtpPort);
        Assert.Equal("ops@test.com", restored.Email.To);

        // Telegram
        Assert.NotNull(restored.Telegram);
        Assert.True(restored.Telegram.Enabled);
        Assert.Equal("123456:ABC", restored.Telegram.BotToken);
        Assert.Equal("-100123", restored.Telegram.ChatId);
        Assert.False(restored.Telegram.UseHtml);

        // WhatsApp
        Assert.NotNull(restored.WhatsApp);
        Assert.True(restored.WhatsApp.Enabled);
        Assert.Equal("token123", restored.WhatsApp.AccessToken);
        Assert.Equal("+1234567890", restored.WhatsApp.To);
        Assert.Equal("alarm_template", restored.WhatsApp.TemplateName);
        Assert.Equal("en_US", restored.WhatsApp.TemplateLanguage);
    }

    // ──────────────────────────────────────────────────────────────
    //  EmailNotificationChannel defaults
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void EmailNotificationChannel_Defaults()
    {
        var ch = new EmailNotificationChannel();
        Assert.False(ch.Enabled);
        Assert.Equal("", ch.SmtpHost);
        Assert.Equal(587, ch.SmtpPort);
        Assert.True(ch.UseSsl);
        Assert.Equal("", ch.Username);
        Assert.Equal("", ch.Password);
        Assert.Equal("", ch.From);
        Assert.Equal("", ch.To);
    }

    // ──────────────────────────────────────────────────────────────
    //  TelegramNotificationChannel defaults
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void TelegramNotificationChannel_Defaults()
    {
        var ch = new TelegramNotificationChannel();
        Assert.False(ch.Enabled);
        Assert.Equal("", ch.BotToken);
        Assert.Equal("", ch.ChatId);
        Assert.True(ch.UseHtml);
    }

    // ──────────────────────────────────────────────────────────────
    //  WhatsAppNotificationChannel defaults
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void WhatsAppNotificationChannel_Defaults()
    {
        var ch = new WhatsAppNotificationChannel();
        Assert.False(ch.Enabled);
        Assert.Equal("", ch.AccessToken);
        Assert.Equal("", ch.PhoneNumberId);
        Assert.Equal("", ch.To);
        Assert.Equal("", ch.TemplateName);
        Assert.Equal("en_US", ch.TemplateLanguage);
    }

    // ──────────────────────────────────────────────────────────────
    //  AlarmConfig.NotifyOnActivation
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void AlarmConfig_NotifyOnActivation_DefaultFalse()
    {
        var alarm = new AlarmConfig();
        Assert.False(alarm.NotifyOnActivation);
    }

    [Fact]
    public void AlarmConfig_NotifyOnActivation_Roundtrip()
    {
        var alarm = new AlarmConfig
        {
            Enabled = true,
            Mode = "Limit",
            HighLimit = 100,
            Severity = 800,
            NotifyOnActivation = true
        };
        var json = JsonSerializer.Serialize(alarm, JsonOptions);
        var restored = JsonSerializer.Deserialize<AlarmConfig>(json);

        Assert.NotNull(restored);
        Assert.True(restored.NotifyOnActivation);
        Assert.Equal((ushort)800, restored.Severity);
    }

    // ──────────────────────────────────────────────────────────────
    //  VariableStatisticsConfig
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void VariableStatisticsConfig_DefaultDisabled()
    {
        var cfg = new VariableStatisticsConfig();
        Assert.False(cfg.Enabled);
    }

    [Fact]
    public void VariableStatisticsConfig_Roundtrip()
    {
        var cfg = new VariableStatisticsConfig { Enabled = true };
        var json = JsonSerializer.Serialize(cfg, JsonOptions);
        var restored = JsonSerializer.Deserialize<VariableStatisticsConfig>(json);

        Assert.NotNull(restored);
        Assert.True(restored.Enabled);
    }

    // ──────────────────────────────────────────────────────────────
    //  ServerSettings includes new config sections
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void ServerSettings_BackupConfig_Roundtrip()
    {
        var settings = new ServerSettings
        {
            Backup = new BackupConfig { Enabled = true, MaxSnapshots = 10 }
        };
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        var restored = JsonSerializer.Deserialize<ServerSettings>(json);

        Assert.NotNull(restored?.Backup);
        Assert.True(restored.Backup.Enabled);
        Assert.Equal(10, restored.Backup.MaxSnapshots);
    }

    [Fact]
    public void ServerSettings_GdsConfig_Roundtrip()
    {
        var settings = new ServerSettings
        {
            Gds = new GdsConfig { EndpointUrl = "opc.tcp://gds:58810" }
        };
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        var restored = JsonSerializer.Deserialize<ServerSettings>(json);

        Assert.NotNull(restored?.Gds);
        Assert.Equal("opc.tcp://gds:58810", restored.Gds.EndpointUrl);
    }

    [Fact]
    public void ServerSettings_AlarmNotification_Roundtrip()
    {
        var settings = new ServerSettings
        {
            AlarmNotification = new AlarmNotificationConfig
            {
                Enabled = true,
                MinSeverity = 200,
                Telegram = new TelegramNotificationChannel
                {
                    Enabled = true,
                    BotToken = "tok",
                    ChatId = "cid"
                }
            }
        };
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        var restored = JsonSerializer.Deserialize<ServerSettings>(json);

        Assert.NotNull(restored?.AlarmNotification);
        Assert.True(restored.AlarmNotification.Enabled);
        Assert.Equal((ushort)200, restored.AlarmNotification.MinSeverity);
        Assert.NotNull(restored.AlarmNotification.Telegram);
        Assert.Equal("tok", restored.AlarmNotification.Telegram.BotToken);
    }
}
