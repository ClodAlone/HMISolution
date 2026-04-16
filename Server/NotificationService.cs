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
        private readonly WebPushSender? _webPushSender;
        private string? _alexaAccessToken;
        private DateTime _alexaTokenExpiry;

        public NotificationService(AlarmNotificationConfig config, string? projectDirectory = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            DiagnosticsCollector.Instance.Register("NotificationService", "AlarmNotification");

            if (config.WebPush is { Enabled: true } wp &&
                !string.IsNullOrEmpty(wp.VapidPublicKey) &&
                !string.IsNullOrEmpty(wp.VapidPrivateKey) &&
                !string.IsNullOrEmpty(projectDirectory))
            {
                try
                {
                    _webPushSender = new WebPushSender(wp, projectDirectory);
                    Log.Information("Web Push notifications enabled");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to initialize Web Push sender");
                }
            }
        }

        /// <summary>
        /// Queue an alarm notification for asynchronous delivery.
        /// Respects the cooldown period per alarm path.
        /// </summary>
        public void NotifyAlarmActivated(string variablePath, string message, ushort severity, string? speakerOverride = null, int volumeOverride = 0)
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

            _ = Task.Run(() => SendAllChannelsAsync(variablePath, message, severity, speakerOverride, volumeOverride));
        }

        /// <summary>Send a custom notification from a script to all enabled channels.</summary>
        public void SendNotification(string title, string message, ushort severity = 500, string? speakerAddresses = null, int volume = 0)
        {
            _ = Task.Run(() => SendAllChannelsAsync(title, message, severity, speakerAddresses, volume));
        }

        /// <summary>Play a TTS message on specific IP speakers from a script.</summary>
        public void PlayOnSpeakers(string message, string speakerAddresses, int volume = 50, string? language = null)
        {
            var cfg = _config.IpSpeaker;
            var lang = language ?? cfg?.TtsLanguage ?? "en";
            var ttsTemplate = cfg?.TtsUrlTemplate ?? "";
            _ = Task.Run(async () =>
            {
                try
                {
                    string audioUrl;
                    if (!string.IsNullOrWhiteSpace(ttsTemplate))
                        audioUrl = ttsTemplate.Replace("{text}", Uri.EscapeDataString(message)).Replace("{lang}", Uri.EscapeDataString(lang));
                    else
                        audioUrl = $"x-rincon-mp3radio://translate.google.com/translate_tts?ie=UTF-8&tl={Uri.EscapeDataString(lang)}&client=tw-ob&q={Uri.EscapeDataString(message)}";
                    var speakers = speakerAddresses.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var tasks = new List<Task>();
                    foreach (var s in speakers) tasks.Add(PlayOnSpeakerAsync(s, audioUrl, volume, false));
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex) { Log.Error(ex, "Script PlayOnSpeakers failed."); }
            });
        }

                private async Task SendAllChannelsAsync(string variablePath, string message, ushort severity, string? speakerOverride = null, int volumeOverride = 0)
        {
            var tasks = new List<Task>();

            if (_config.Email is { Enabled: true })
                tasks.Add(SendEmailAsync(variablePath, message, severity));

            if (_config.Telegram is { Enabled: true })
                tasks.Add(SendTelegramAsync(variablePath, message, severity));

            if (_config.WhatsApp is { Enabled: true })
                tasks.Add(SendWhatsAppAsync(variablePath, message, severity));

            if (_config.Alexa is { Enabled: true })
                tasks.Add(SendAlexaAnnounceAsync(variablePath, message, severity));

            if (_config.IpSpeaker is { Enabled: true })
                tasks.Add(SendIpSpeakerAsync(variablePath, message, severity, speakerOverride, volumeOverride));

            if (_webPushSender != null)
            {
                var severityLabel = severity >= 800 ? "CRITICAL" : severity >= 500 ? "WARNING" : "INFO";
                tasks.Add(_webPushSender.SendToAllAsync(
                    $"[{severityLabel}] Alarm: {variablePath}",
                    $"{message} \u2014 Severity {severity}",
                    $"alarm-{variablePath}"));
            }

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

        // â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€ Alexa (Proactive Events API) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        private async Task SendAlexaAnnounceAsync(string variablePath, string message, ushort severity)
        {
            var cfg = _config.Alexa!;
            try
            {
                if (string.IsNullOrWhiteSpace(cfg.ClientId) || string.IsNullOrWhiteSpace(cfg.ClientSecret))
                {
                    Log.Warning("Alarm notification Alexa skipped â€” ClientId or ClientSecret not configured.");
                    return;
                }

                // Acquire or reuse LWA access token
                if (_alexaAccessToken == null || DateTime.UtcNow >= _alexaTokenExpiry)
                {
                    var tokenPayload = new Dictionary<string, string>
                    {
                        ["grant_type"] = "client_credentials",
                        ["client_id"] = cfg.ClientId,
                        ["client_secret"] = cfg.ClientSecret,
                        ["scope"] = "alexa::proactive_events"
                    };
                    var tokenResponse = await _httpClient.PostAsync(
                        "https://api.amazon.com/auth/o2/token",
                        new FormUrlEncodedContent(tokenPayload), _cts.Token);
                    tokenResponse.EnsureSuccessStatusCode();

                    var tokenJson = await tokenResponse.Content.ReadAsStringAsync(_cts.Token);
                    using var tokenDoc = JsonDocument.Parse(tokenJson);
                    _alexaAccessToken = tokenDoc.RootElement.GetProperty("access_token").GetString();
                    var expiresIn = tokenDoc.RootElement.GetProperty("expires_in").GetInt32();
                    _alexaTokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60); // refresh 60s early
                    Log.Debug("Alexa LWA token acquired, expires in {ExpiresIn}s", expiresIn);
                }

                // Determine regional API endpoint
                var apiBase = (cfg.Region?.ToUpperInvariant()) switch
                {
                    "NA" => "https://api.amazonalexa.com",
                    "FE" => "https://api.fe.amazonalexa.com",
                    _ => "https://api.eu.amazonalexa.com" // EU default
                };
                var stage = cfg.UseSandbox ? "development" : "live";

                var severityLabel = severity >= 800 ? "CRITICAL" : severity >= 500 ? "WARNING" : "INFO";
                var now = DateTime.UtcNow;
                var expiryTime = now.AddHours(24);

                // Build Proactive Events API payload (AMAZON.MessageAlert.Activated schema)
                var eventPayload = new
                {
                    timestamp = now.ToString("o"),
                    referenceId = $"alarm-{variablePath}-{now.Ticks}",
                    expiryTime = expiryTime.ToString("o"),
                    @event = new
                    {
                        name = "AMAZON.MessageAlert.Activated",
                        payload = new
                        {
                            state = new { status = "UNREAD" },
                            messageGroup = new
                            {
                                creator = new { name = "HMI Server" },
                                count = 1,
                                urgency = severity >= 800 ? "URGENT" : null
                            }
                        }
                    },
                    localizedAttributes = new[]
                    {
                        new
                        {
                            locale = "en-US",
                            providerName = "HMI Server",
                            contentBody = $"[{severityLabel}] {variablePath}: {message} â€” Severity {severity}"
                        }
                    },
                    relevantAudience = new
                    {
                        type = "Multicast",
                        payload = (object?)null
                    }
                };

                var json = JsonSerializer.Serialize(eventPayload);
                var request = new HttpRequestMessage(HttpMethod.Post, $"{apiBase}/v1/proactiveEvents/stages/{stage}")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _alexaAccessToken);

                var response = await _httpClient.SendAsync(request, _cts.Token);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(_cts.Token);
                    // Invalidate token on 401 so next call re-authenticates
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        _alexaAccessToken = null;
                    Log.Warning("Alexa Proactive Events API returned {Status}: {Body}", response.StatusCode, body);
                }
                else
                {
                    Log.Information("Alexa alarm notification sent for {Path}.", variablePath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send Alexa alarm notification for {Path}.", variablePath);
            }
        }

        // â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€ IP Speaker (Sonos / UPnP) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        private async Task SendIpSpeakerAsync(string variablePath, string message, ushort severity, string? speakerOverride = null, int volumeOverride = 0)
        {
            var cfg = _config.IpSpeaker!;
            try
            {
                // Resolve speaker addresses: per-alarm override > global config
                var addressSource = !string.IsNullOrWhiteSpace(speakerOverride) ? speakerOverride : cfg.SpeakerAddresses;
                if (string.IsNullOrWhiteSpace(addressSource))
                {
                    Log.Warning("Alarm notification IP Speaker skipped â€” no speaker addresses configured.");
                    return;
                }

                var severityLabel = severity >= 800 ? "CRITICAL" : severity >= 500 ? "WARNING" : "INFO";
                var ttsText = $"{severityLabel} alarm. {variablePath}. {message}. Severity {severity}";

                // Build TTS audio URI
                string audioUrl;
                if (!string.IsNullOrWhiteSpace(cfg.TtsUrlTemplate))
                {
                    audioUrl = cfg.TtsUrlTemplate
                        .Replace("{text}", Uri.EscapeDataString(ttsText))
                        .Replace("{lang}", Uri.EscapeDataString(cfg.TtsLanguage));
                }
                else
                {
                    // Google Translate TTS (simple, zero-config â€” not for heavy production use)
                    var lang = string.IsNullOrWhiteSpace(cfg.TtsLanguage) ? "en" : cfg.TtsLanguage;
                    audioUrl = $"x-rincon-mp3radio://translate.google.com/translate_tts?ie=UTF-8&tl={Uri.EscapeDataString(lang)}&client=tw-ob&q={Uri.EscapeDataString(ttsText)}";
                }

                // Resolve volume: per-alarm override > severity tier > default
                int resolvedVolume = volumeOverride > 0 ? volumeOverride
                    : severity >= 800 && cfg.VolumeCritical > 0 ? cfg.VolumeCritical
                    : severity >= 500 && cfg.VolumeWarning > 0 ? cfg.VolumeWarning
                    : severity < 500 && cfg.VolumeInfo > 0 ? cfg.VolumeInfo
                    : cfg.Volume;

                var speakers = addressSource.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var tasks = new List<Task>();
                foreach (var speaker in speakers)
                {
                    tasks.Add(PlayOnSpeakerAsync(speaker, audioUrl, resolvedVolume, cfg.RestoreVolume));
                }

                await Task.WhenAll(tasks);
                Log.Information("Alarm IP Speaker announcement sent for {Path} to {Count} speaker(s).", variablePath, speakers.Length);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send IP Speaker alarm for {Path}.", variablePath);
            }
        }

        private async Task PlayOnSpeakerAsync(string speakerAddress, string audioUrl, int volume, bool restoreVolume)
        {
            // Parse host:port (default Sonos port 1400)
            var parts = speakerAddress.Split(':', 2);
            var host = parts[0];
            var port = parts.Length > 1 && int.TryParse(parts[1], out var p) ? p : 1400;
            var baseUrl = $"http://{host}:{port}";

            try
            {
                int previousVolume = -1;

                // Set volume if specified
                if (volume > 0)
                {
                    if (restoreVolume)
                    {
                        previousVolume = await GetSonosVolumeAsync(baseUrl);
                    }
                    await SendSoapAsync(baseUrl, "/MediaRenderer/RenderingControl/Control",
                        "urn:schemas-upnp-org:service:RenderingControl:1", "SetVolume",
                        $"<InstanceID>0</InstanceID><Channel>Master</Channel><DesiredVolume>{volume}</DesiredVolume>");
                }

                // Set the audio URI on the speaker
                var escapedUrl = System.Security.SecurityElement.Escape(audioUrl);
                await SendSoapAsync(baseUrl, "/MediaRenderer/AVTransport/Control",
                    "urn:schemas-upnp-org:service:AVTransport:1", "SetAVTransportURI",
                    $"<InstanceID>0</InstanceID><CurrentURI>{escapedUrl}</CurrentURI><CurrentURIMetaData></CurrentURIMetaData>");

                // Play
                await SendSoapAsync(baseUrl, "/MediaRenderer/AVTransport/Control",
                    "urn:schemas-upnp-org:service:AVTransport:1", "Play",
                    "<InstanceID>0</InstanceID><Speed>1</Speed>");

                // Wait for announcement to finish, then restore volume
                if (restoreVolume && previousVolume >= 0)
                {
                    await Task.Delay(8000, _cts.Token); // approximate TTS duration
                    await SendSoapAsync(baseUrl, "/MediaRenderer/RenderingControl/Control",
                        "urn:schemas-upnp-org:service:RenderingControl:1", "SetVolume",
                        $"<InstanceID>0</InstanceID><Channel>Master</Channel><DesiredVolume>{previousVolume}</DesiredVolume>");
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "IP Speaker {Address} failed.", speakerAddress);
            }
        }

        private async Task<int> GetSonosVolumeAsync(string baseUrl)
        {
            try
            {
                var responseXml = await SendSoapAsync(baseUrl, "/MediaRenderer/RenderingControl/Control",
                    "urn:schemas-upnp-org:service:RenderingControl:1", "GetVolume",
                    "<InstanceID>0</InstanceID><Channel>Master</Channel>");

                // Extract <CurrentVolume>nn</CurrentVolume>
                var start = responseXml.IndexOf("<CurrentVolume>", StringComparison.Ordinal);
                var end = responseXml.IndexOf("</CurrentVolume>", StringComparison.Ordinal);
                if (start >= 0 && end > start)
                {
                    var val = responseXml.Substring(start + 15, end - start - 15);
                    if (int.TryParse(val, out var vol)) return vol;
                }
            }
            catch { /* fall through */ }
            return -1;
        }

        private async Task<string> SendSoapAsync(string baseUrl, string controlPath, string serviceType, string action, string bodyContent)
        {
            var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
  <s:Body>
    <u:{action} xmlns:u=""{serviceType}"">
      {bodyContent}
    </u:{action}>
  </s:Body>
</s:Envelope>";

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}{controlPath}")
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
            };
            request.Headers.Add("SOAPAction", $"\"{serviceType}#{action}\"");

            var response = await _httpClient.SendAsync(request, _cts.Token);
            return await response.Content.ReadAsStringAsync(_cts.Token);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _httpClient.Dispose();
            _cts.Dispose();
            _webPushSender?.Dispose();
        }
    }
}
