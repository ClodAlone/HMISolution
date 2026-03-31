using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Manages alarm notifications: email, webhook, and escalation.
/// When an alarm activates, the notification manager sends immediate notifications
/// and schedules escalation if the alarm is not acknowledged within the configured time.
/// </summary>
public sealed class NotificationManager : IDisposable
{
    private readonly NotificationsConfig? _globalConfig;
    private readonly ConcurrentDictionary<string, EscalationEntry> _pendingEscalations = new();
    private Timer? _escalationTimer;
    private readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(30) };

    public NotificationManager(NotificationsConfig? globalConfig)
    {
        _globalConfig = globalConfig;

        // Check for pending escalations every 30 seconds
        _escalationTimer = new Timer(CheckEscalations, null,
            TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
    }

    /// <summary>
    /// Called when an alarm activates. Sends immediate notifications and schedules escalation.
    /// </summary>
    public void NotifyAlarmActivated(string alarmPath, string message, ushort severity,
        AlarmNotificationConfig? notifConfig)
    {
        if (notifConfig == null) return;

        var payload = new AlarmNotificationPayload
        {
            AlarmPath = alarmPath,
            Message = message,
            Severity = severity,
            State = "Active",
            Timestamp = DateTime.UtcNow
        };

        // Send immediate email
        if (notifConfig.EmailEnabled && !string.IsNullOrEmpty(notifConfig.EmailRecipients))
        {
            _ = SendEmailAsync(notifConfig.EmailRecipients,
                $"[ALARM] {alarmPath} — Severity {severity}",
                FormatEmailBody(payload));
        }

        // Send webhook
        if (notifConfig.WebhookEnabled)
        {
            var url = !string.IsNullOrEmpty(notifConfig.WebhookUrl)
                ? notifConfig.WebhookUrl
                : _globalConfig?.DefaultWebhookUrl ?? "";
            if (!string.IsNullOrEmpty(url))
            {
                _ = SendWebhookAsync(url, payload);
            }
        }

        // Schedule escalation
        if (notifConfig.EscalationMinutes > 0 && !string.IsNullOrEmpty(notifConfig.EscalationRecipients))
        {
            _pendingEscalations[alarmPath] = new EscalationEntry
            {
                AlarmPath = alarmPath,
                Message = message,
                Severity = severity,
                ActivatedAt = DateTime.UtcNow,
                EscalationTime = DateTime.UtcNow.AddMinutes(notifConfig.EscalationMinutes),
                Recipients = notifConfig.EscalationRecipients,
                Escalated = false
            };
        }
    }

    /// <summary>
    /// Called when an alarm is acknowledged. Cancels pending escalation.
    /// </summary>
    public void CancelEscalation(string alarmPath)
    {
        _pendingEscalations.TryRemove(alarmPath, out _);
    }

    /// <summary>
    /// Called when an alarm deactivates (returns to normal). Sends clear notification.
    /// </summary>
    public void NotifyAlarmCleared(string alarmPath, string message, AlarmNotificationConfig? notifConfig)
    {
        _pendingEscalations.TryRemove(alarmPath, out _);

        if (notifConfig == null) return;

        var payload = new AlarmNotificationPayload
        {
            AlarmPath = alarmPath,
            Message = message,
            Severity = 1,
            State = "Cleared",
            Timestamp = DateTime.UtcNow
        };

        if (notifConfig.WebhookEnabled)
        {
            var url = !string.IsNullOrEmpty(notifConfig.WebhookUrl)
                ? notifConfig.WebhookUrl
                : _globalConfig?.DefaultWebhookUrl ?? "";
            if (!string.IsNullOrEmpty(url))
                _ = SendWebhookAsync(url, payload);
        }
    }

    private void CheckEscalations(object? state)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in _pendingEscalations.Values)
        {
            if (!entry.Escalated && now >= entry.EscalationTime)
            {
                entry.Escalated = true;
                Log.Warning("Alarm escalation triggered for {AlarmPath} — not acknowledged within timeout", entry.AlarmPath);

                _ = SendEmailAsync(entry.Recipients,
                    $"[ESCALATION] {entry.AlarmPath} — NOT ACKNOWLEDGED",
                    $"<h2>Alarm Escalation</h2>" +
                    $"<p>The following alarm has not been acknowledged:</p>" +
                    $"<p><b>Alarm:</b> {entry.AlarmPath}<br/>" +
                    $"<b>Message:</b> {entry.Message}<br/>" +
                    $"<b>Severity:</b> {entry.Severity}<br/>" +
                    $"<b>Active since:</b> {entry.ActivatedAt:yyyy-MM-dd HH:mm:ss} UTC<br/>" +
                    $"<b>Escalation time:</b> {(now - entry.ActivatedAt).TotalMinutes:F0} minutes</p>");
            }
        }
    }

    private async Task SendEmailAsync(string recipients, string subject, string htmlBody)
    {
        var smtp = _globalConfig?.Smtp;
        if (smtp == null || string.IsNullOrEmpty(smtp.Host))
        {
            Log.Warning("Notification email skipped — SMTP not configured");
            return;
        }

        try
        {
            using var client = new SmtpClient(smtp.Host, smtp.Port)
            {
                EnableSsl = smtp.UseSsl,
                Credentials = new NetworkCredential(smtp.Username, smtp.Password)
            };

            var msg = new MailMessage
            {
                From = new MailAddress(smtp.FromAddress),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            foreach (var addr in recipients.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                msg.To.Add(addr);

            await client.SendMailAsync(msg);
            Log.Information("Notification email sent to {Recipients}: {Subject}", recipients, subject);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to send notification email to {Recipients}", recipients);
        }
    }

    private async Task SendWebhookAsync(string url, AlarmNotificationPayload payload)
    {
        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            Log.Information("Webhook sent to {Url}: {StatusCode}", url, response.StatusCode);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to send webhook to {Url}", url);
        }
    }

    private static string FormatEmailBody(AlarmNotificationPayload payload)
    {
        return $"<h2>Alarm Notification</h2>" +
               $"<p><b>Alarm:</b> {payload.AlarmPath}<br/>" +
               $"<b>State:</b> {payload.State}<br/>" +
               $"<b>Message:</b> {payload.Message}<br/>" +
               $"<b>Severity:</b> {payload.Severity}<br/>" +
               $"<b>Time:</b> {payload.Timestamp:yyyy-MM-dd HH:mm:ss} UTC</p>";
    }

    public void Dispose()
    {
        _escalationTimer?.Dispose();
        _httpClient.Dispose();
    }

    private class EscalationEntry
    {
        public string AlarmPath { get; set; } = "";
        public string Message { get; set; } = "";
        public ushort Severity { get; set; }
        public DateTime ActivatedAt { get; set; }
        public DateTime EscalationTime { get; set; }
        public string Recipients { get; set; } = "";
        public bool Escalated { get; set; }
    }
}

public class AlarmNotificationPayload
{
    public string AlarmPath { get; set; } = "";
    public string Message { get; set; } = "";
    public ushort Severity { get; set; }
    public string State { get; set; } = "";
    public DateTime Timestamp { get; set; }
}
