using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SharedModels;

/// <summary>
/// Simple file-based storage for Web Push subscriptions.
/// Used by both the RuntimeViewer (to add/remove subscriptions via API)
/// and the Server (via WebPushSender to load subscriptions when sending notifications).
/// </summary>
public sealed class WebPushSubscriptionStore
{
    private readonly string _filePath;
    private readonly object _lock = new();

    public WebPushSubscriptionStore(WebPushNotificationChannel config, string projectDirectory)
    {
        var file = config.SubscriptionsFile;
        if (string.IsNullOrWhiteSpace(file)) file = "push-subscriptions.json";
        _filePath = Path.IsPathRooted(file) ? file : Path.Combine(projectDirectory, file);
    }

    public string FilePath => _filePath;

    public List<PushSubscriptionInfo> Load()
    {
        lock (_lock)
        {
            try
            {
                if (!File.Exists(_filePath)) return [];
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<PushSubscriptionInfo>>(json) ?? [];
            }
            catch
            {
                return [];
            }
        }
    }

    public void Save(List<PushSubscriptionInfo> subs)
    {
        lock (_lock)
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(subs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
