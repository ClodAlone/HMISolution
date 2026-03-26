using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using SharedModels;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Sends alarm notifications via Email (SMTP), Telegram (Bot API), and WhatsApp (Cloud API).
    /// Notifications are dispatched asynchronously on a background thread to avoid blocking
    /// the OPC UA alarm evaluation loop.
    /// </summary>
    public sealed class NotificationService : IDisposable
    {
        private readonly AlarmNotificationConfig _config;
        private readonly HttpClient _httpClient;
        private readonly ConcurrentDictionary<string, DateTime> _cooldowns = new();
        private readonly CancellationTokenSource _cts = new();

        public NotificationService(AlarmNotificationConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            DiagnosticsCollector.Instance.Register("NotificationService", "AlarmNotification");
        }

        /// <summary>
        /// Queue an alarm notification for asynchronous delivery.
        /// Respects the cooldown period per alarm path.
        /// </summary>
        public void NotifyAlarmActivated(string variablePath, string message, ushort severity)
        {
            if (!_config.Enabled) return;
            if (severity < _config.MinSeverity) return;

            // Cooldown check
            if (_config.CooldownSeconds > 0 &&
                _cooldowns.TryGetValue(variablePath, out var lastSent) &&
                (DateTime.UtcNow - lastSent).TotalSeconds < _config.CooldownSeconds)
            {
                return;
            }
            _cooldowns[variablePath] = DateTime.UtcNow;

            // Fire-and-forget on thread pool
            _ = Task.Run(() => SendAllChannelsAsync(variablePath, message, severity));
        }

        private async Task SendAllChannelsAsync(string variablePath, string message, ushort severity)
        {
            var tasks = new List<Task>();

            if (_config.Email is { Enabled: true })
                tasks.Add(SendEmailAsync(variablePath, message, severity));

            if (_config.Telegram is { Enabled: true })
                tasks.Add(SendTelegramAsync(variablePath, message, severity));

            if (_config.WhatsApp is { Enabled: true })
                tasks.Add(SendWhatsAppAsync(variablePath, message, severity));

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "One or more alarm notification channels failed for {Path}", variablePath);
            }
        }

        // ──────────────────────────── Email (SMTP) ────────────────────────────

        private async Task SendEmailAsync(string variablePath, string message, ushort severity)
        {
            var cfg = _config.Email!;
            try
            {
                if (string.IsNullOrWhiteSpace(cfg.SmtpHost) || string.IsNullOrWhiteSpace(cfg.To))
                {
                    Log.Warning("Alarm notification email skipped — SMTP host or recipients not configured.");
                    return;
                }

                using var client = new SmtpClient(cfg.SmtpHost, cfg.SmtpPort)
                {
                    EnableSsl = cfg.UseSsl,
                    Credentials = !string.IsNullOrWhiteSpace(cfg.Username)
                        ? new NetworkCredential(cfg.Username, cfg.Password)
                        : null
                };

                var from = !string.IsNullOrWhiteSpace(cfg.From) ? cfg.From : "noreply@hmi-server.local";
                var severityLabel = severity >= 800 ? "CRITICAL" : severity >= 500 ? "WARNING" : "INFO";
                var subject = $"[{severityLabel}] Alarm: {variablePath}";

                var body = $"""
                    <html><body style="font-family: Arial, sans-serif;">
                    <h2 style="color: {(severity >= 800 ? "#dc3545" : "#ffc107")};">⚠ Alarm Notification</h2>
                    <table style="border-collapse: collapse;">
                    <tr><td style="padding: 4px 12px; font-weight: bold;">Variable:</td><td>{WebUtility.HtmlEncode(variablePath)}</td></tr>
                    <tr><td style="padding: 4px 12px; font-weight: bold;">Severity:</td><td>{severity} ({severityLabel})</td></tr>
                    <tr><td style="padding: 4px 12px; font-weight: bold;">Message:</td><td>{WebUtility.HtmlEncode(message)}</td></tr>
                    <tr><td style="padding: 4px 12px; font-weight: bold;">Time (UTC):</td><td>{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</td></tr>
                    </table>
                    <hr/><small>Sent by HMI Server Alarm Notification</small>
                    </body></html>
                    """;

                var recipients = cfg.To.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var recipient in recipients)
                {
                    var msg = new MailMessage(from, recipient, subject, body) { IsBodyHtml = true };
                    await client.SendMailAsync(msg, _cts.Token);
                }

                Log.Information("Alarm email sent for {Path} to {Count} recipient(s).", variablePath, recipients.Length);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send alarm email for {Path}.", variablePath);
            }
        }

        // ──────────────────────────── Telegram ────────────────────────────

        private async Task SendTelegramAsync(string variablePath, string message, ushort severity)
        {
            var cfg = _config.Telegram!;
            try
            {
                if (string.IsNullOrWhiteSpace(cfg.BotToken) || string.IsNullOrWhiteSpace(cfg.ChatId))
                {
                    Log.Warning("Alarm notification Telegram skipped — BotToken or ChatId not configured.");
                    return;
                }

                var severityLabel = severity >= 800 ? "🔴 CRITICAL" : severity >= 500 ? "🟡 WARNING" : "🔵 INFO";
                string text;
                if (cfg.UseHtml)
                {
                    text = $"<b>⚠ Alarm Notification</b>\n" +
                           $"<b>Variable:</b> <code>{WebUtility.HtmlEncode(variablePath)}</code>\n" +
                           $"<b>Severity:</b> {severity} ({severityLabel})\n" +
                           $"<b>Message:</b> {WebUtility.HtmlEncode(message)}\n" +
                           $"<b>Time (UTC):</b> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                }
                else
                {
                    text = $"⚠ Alarm Notification\n" +
                           $"Variable: {variablePath}\n" +
                           $"Severity: {severity} ({severityLabel})\n" +
                           $"Message: {message}\n" +
                           $"Time (UTC): {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                }

                var chatIds = cfg.ChatId.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var chatId in chatIds)
                {
                    var url = $"https://api.telegram.org/bot{cfg.BotToken}/sendMessage";
                    var payload = new Dictionary<string, string>
                    {
                        ["chat_id"] = chatId,
                        ["text"] = text
                    };
                    if (cfg.UseHtml) payload["parse_mode"] = "HTML";

                    var content = new FormUrlEncodedContent(payload);
                    var response = await _httpClient.PostAsync(url, content, _cts.Token);

                    if (!response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync(_cts.Token);
                        Log.Warning("Telegram API returned {Status} for chat {ChatId}: {Body}",
                            response.StatusCode, chatId, body);
                    }
                }

                Log.Information("Alarm Telegram sent for {Path} to {Count} chat(s).", variablePath, chatIds.Length);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send alarm Telegram for {Path}.", variablePath);
            }
        }

        // ──────────────────────────── WhatsApp (Cloud API) ────────────────────────────

        private async Task SendWhatsAppAsync(string variablePath, string message, ushort severity)
        {
            var cfg = _config.WhatsApp!;
            try
            {
                if (string.IsNullOrWhiteSpace(cfg.AccessToken) || string.IsNullOrWhiteSpace(cfg.PhoneNumberId) ||
                    string.IsNullOrWhiteSpace(cfg.To))
                {
                    Log.Warning("Alarm notification WhatsApp skipped — AccessToken, PhoneNumberId, or recipients not configured.");
                    return;
                }

                var recipients = cfg.To.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var url = $"https://graph.facebook.com/v21.0/{cfg.PhoneNumberId}/messages";

                foreach (var phone in recipients)
                {
                    object payload;
                    if (!string.IsNullOrWhiteSpace(cfg.TemplateName))
                    {
                        // Template message (required for initiating conversations)
                        payload = new
                        {
                            messaging_product = "whatsapp",
                            to = phone,
                            type = "template",
                            template = new
                            {
                                name = cfg.TemplateName,
                                language = new { code = cfg.TemplateLanguage },
                                components = new[]
                                {
                                    new
                                    {
                                        type = "body",
                                        parameters = new object[]
                                        {
                                            new { type = "text", text = variablePath },
                                            new { type = "text", text = severity.ToString() },
                                            new { type = "text", text = message }
                                        }
                                    }
                                }
                            }
                        };
                    }
                    else
                    {
                        // Free-form text message (only within 24-hour session window)
                        var severityLabel = severity >= 800 ? "🔴 CRITICAL" : severity >= 500 ? "🟡 WARNING" : "🔵 INFO";
                        var body = $"⚠ *Alarm Notification*\n\n" +
                                   $"*Variable:* {variablePath}\n" +
                                   $"*Severity:* {severity} ({severityLabel})\n" +
                                   $"*Message:* {message}\n" +
                                   $"*Time (UTC):* {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";

                        payload = new
                        {
                            messaging_product = "whatsapp",
                            to = phone,
                            type = "text",
                            text = new { body }
                        };
                    }

                    var json = JsonSerializer.Serialize(payload);
                    var request = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cfg.AccessToken);

                    var response = await _httpClient.SendAsync(request, _cts.Token);
                    if (!response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync(_cts.Token);
                        Log.Warning("WhatsApp API returned {Status} for {Phone}: {Body}",
                            response.StatusCode, phone, responseBody);
                    }
                }

                Log.Information("Alarm WhatsApp sent for {Path} to {Count} recipient(s).", variablePath, recipients.Length);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send alarm WhatsApp for {Path}.", variablePath);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _httpClient.Dispose();
            _cts.Dispose();
        }
    }
}
