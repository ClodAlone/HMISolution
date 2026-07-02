// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Net;
using System.Text;

Console.WriteLine("=== IP Speaker / Sonos Test Tool ===");
Console.WriteLine();

string speakerAddress = Ask("Speaker IP (e.g. 192.168.1.50)", "192.168.1.50");
string language = Ask("TTS language code", "en");
int volume = int.Parse(Ask("Volume (1-100, 0=skip)", "40"));
string message = Ask("TTS message", "Hello, this is a test announcement");
var parts = speakerAddress.Split(':', 2);
var host = parts[0];
var port = parts.Length > 1 && int.TryParse(parts[1], out var p) ? p : 1400;
if (!IPAddress.TryParse(host, out _))
{
    var resolved = Dns.GetHostAddresses(host);
    if (resolved.Length > 0) { Console.WriteLine($"  Resolved {host} -> {resolved[0]}"); host = resolved[0].ToString(); }
}
var baseUrl = $"http://{host}:{port}";
using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };


while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Get volume  2) Set volume  3) Play TTS  4) Play+Restore");
    Console.WriteLine("5) Play URL    6) Stop        7) Transport  8) Media info");
    Console.WriteLine("9) Settings    0) Exit");
    Console.Write("> ");
    var choice = Console.ReadLine()?.Trim();
    try
    {
        switch (choice)
        {
            case "1":
                var vol = await GetVolumeAsync(http, baseUrl);
                Console.WriteLine(vol >= 0 ? $"  Volume: {vol}" : "  Could not read.");
                break;
            case "2":
                var nv = int.Parse(Ask("  Volume (1-100)", volume.ToString()));
                await SetVolumeAsync(http, baseUrl, nv);
                Console.WriteLine($"  Set to {nv}.");
                break;
            case "3":
                var u3 = BuildTtsUrl(message, language);
                Console.WriteLine($"  URL: {u3}");
                await PlayUrlAsync(http, baseUrl, u3);
                Console.WriteLine("  Playing...");
                break;
            case "4":
                var prev = await GetVolumeAsync(http, baseUrl);
                Console.WriteLine($"  Previous volume: {prev}");
                if (volume > 0) await SetVolumeAsync(http, baseUrl, volume);
                await PlayUrlAsync(http, baseUrl, BuildTtsUrl(message, language));
                Console.WriteLine("  Playing... restoring in 8s");
                await Task.Delay(8000);
                if (prev >= 0) await SetVolumeAsync(http, baseUrl, prev);
                Console.WriteLine($"  Restored to {prev}.");
                break;
            case "5":
                var cu = Ask("  Audio URL", "");
                if (!string.IsNullOrWhiteSpace(cu))
                {
                    await PlayUrlAsync(http, baseUrl, cu);
                    Console.WriteLine("  Playing...");
                }
                break;
            case "6":
                await StopAsync(http, baseUrl);
                Console.WriteLine("  Stopped.");
                break;
            case "7":
                Console.WriteLine(await GetTransportInfoAsync(http, baseUrl));
                break;
            case "8":
                Console.WriteLine(await GetMediaInfoAsync(http, baseUrl));
                break;
            case "9":
                speakerAddress = Ask("Speaker IP", speakerAddress);
                language = Ask("Language", language);
                volume = int.Parse(Ask("Volume", volume.ToString()));
                message = Ask("Message", message);
                parts = speakerAddress.Split(':', 2);
                host = parts[0];
                port = parts.Length > 1 && int.TryParse(parts[1], out var p2) ? p2 : 1400;
                if (!IPAddress.TryParse(host, out _)) { var r = Dns.GetHostAddresses(host); if (r.Length > 0) { Console.WriteLine($"  Resolved {host} -> {r[0]}"); host = r[0].ToString(); } }
                baseUrl = $"http://{host}:{port}";
                break;
            case "0":
            case null:
                return;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ERROR: {ex.Message}");
        Console.ResetColor();
    }
}

static string Ask(string prompt, string def)
{
    Console.Write($"{prompt} [{def}]: ");
    var v = Console.ReadLine()?.Trim();
    return string.IsNullOrEmpty(v) ? def : v;
}

static string BuildTtsUrl(string text, string lang) =>
    $"x-rincon-mp3radio://translate.google.com/translate_tts?ie=UTF-8&tl={Uri.EscapeDataString(lang)}&client=tw-ob&q={Uri.EscapeDataString(text)}";


static async Task<int> GetVolumeAsync(HttpClient http, string baseUrl)
{
    var xml = await SoapAsync(http, baseUrl, "/MediaRenderer/RenderingControl/Control",
        "urn:schemas-upnp-org:service:RenderingControl:1", "GetVolume",
        "<InstanceID>0</InstanceID><Channel>Master</Channel>");
    var s = xml.IndexOf("<CurrentVolume>", StringComparison.Ordinal);
    var e = xml.IndexOf("</CurrentVolume>", StringComparison.Ordinal);
    return s >= 0 && e > s && int.TryParse(xml.AsSpan(s + 15, e - s - 15), out var v) ? v : -1;
}

static Task SetVolumeAsync(HttpClient http, string baseUrl, int vol) =>
    SoapAsync(http, baseUrl, "/MediaRenderer/RenderingControl/Control",
        "urn:schemas-upnp-org:service:RenderingControl:1", "SetVolume",
        $"<InstanceID>0</InstanceID><Channel>Master</Channel><DesiredVolume>{vol}</DesiredVolume>");

static async Task PlayUrlAsync(HttpClient http, string baseUrl, string audioUrl)
{
    var esc = System.Security.SecurityElement.Escape(audioUrl);
    await SoapAsync(http, baseUrl, "/MediaRenderer/AVTransport/Control",
        "urn:schemas-upnp-org:service:AVTransport:1", "SetAVTransportURI",
        $"<InstanceID>0</InstanceID><CurrentURI>{esc}</CurrentURI><CurrentURIMetaData></CurrentURIMetaData>");
    await SoapAsync(http, baseUrl, "/MediaRenderer/AVTransport/Control",
        "urn:schemas-upnp-org:service:AVTransport:1", "Play",
        "<InstanceID>0</InstanceID><Speed>1</Speed>");
}

static Task StopAsync(HttpClient http, string baseUrl) =>
    SoapAsync(http, baseUrl, "/MediaRenderer/AVTransport/Control",
        "urn:schemas-upnp-org:service:AVTransport:1", "Stop",
        "<InstanceID>0</InstanceID>");

static Task<string> GetTransportInfoAsync(HttpClient http, string baseUrl) =>
    SoapAsync(http, baseUrl, "/MediaRenderer/AVTransport/Control",
        "urn:schemas-upnp-org:service:AVTransport:1", "GetTransportInfo",
        "<InstanceID>0</InstanceID>");

static Task<string> GetMediaInfoAsync(HttpClient http, string baseUrl) =>
    SoapAsync(http, baseUrl, "/MediaRenderer/AVTransport/Control",
        "urn:schemas-upnp-org:service:AVTransport:1", "GetMediaInfo",
        "<InstanceID>0</InstanceID>");

static async Task<string> SoapAsync(HttpClient http, string baseUrl, string path,
    string svc, string action, string body)
{
    var soap = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
  <s:Body>
    <u:{action} xmlns:u=""{svc}"">
      {body}
    </u:{action}>
  </s:Body>
</s:Envelope>";

    var req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}{path}")
    {
        Content = new StringContent(soap, Encoding.UTF8, "text/xml")
    };
    req.Headers.Add("SOAPAction", $"\"{svc}#{action}\"");

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"  > {action} {baseUrl}{path}");
    Console.ResetColor();

    var resp = await http.SendAsync(req);
    if (!resp.IsSuccessStatusCode)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  < HTTP {(int)resp.StatusCode}");
        Console.ResetColor();
    }
    return await resp.Content.ReadAsStringAsync();
}

