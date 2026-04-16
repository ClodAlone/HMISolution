# IP Speaker (Sonos) Alarm Notifications — Setup Guide

This guide walks you through setting up audible text-to-speech alarm
announcements on Sonos speakers (or other UPnP-compatible IP speakers)
from the HMI Server.

---

## Table of Contents

1. [Overview](#1-overview)
2. [How It Works](#2-how-it-works)
3. [Prerequisites](#3-prerequisites)
4. [Find Your Speaker IP Addresses](#4-find-your-speaker-ip-addresses)
5. [Configure HMI Server](#5-configure-hmi-server)
6. [Custom TTS Providers](#6-custom-tts-providers)
7. [Test the Integration](#7-test-the-integration)
8. [Troubleshooting](#8-troubleshooting)
9. [JSON Configuration Reference](#9-json-configuration-reference)

---

## 1. Overview

The IP Speaker notification channel plays a spoken alarm announcement on one or
more Sonos speakers (or any UPnP AVTransport-compatible speaker) on the local
network. When an alarm fires, the server:

1. Converts the alarm text to an audio URL via a TTS provider
2. Sends UPnP SOAP commands to each configured speaker
3. The speaker plays the announcement at the configured volume
4. Optionally restores the previous volume afterward

| Feature | Details |
|---|---|
| Protocol | UPnP / SOAP (AVTransport + RenderingControl) |
| Default port | 1400 (Sonos standard) |
| TTS | Google Translate TTS (built-in) or custom provider |
| Volume control | Set announcement volume + auto-restore |
| Network | Local network only (no cloud required) |
| Compatible speakers | Sonos (S1 & S2), other UPnP/DLNA renderers |

---

## 2. How It Works

```
HMI Server                           Local Network
┌─────────────────────┐          ┌──────────────────┐
│  NotificationService │         │                  │
│                      │         │  Sonos Speaker   │
│  1. Build TTS URL    │         │  (192.168.1.50)  │
│  2. SOAP: SetVolume  │──HTTP──►│  :1400           │
│  3. SOAP: SetURI     │──HTTP──►│                  │
│  4. SOAP: Play       │──HTTP──►│  🔊 plays audio  │
│  5. Wait ~8s         │         │                  │
│  6. SOAP: Restore Vol│──HTTP──►│                  │
└─────────────────────┘          └──────────────────┘
                                          │
                               Speaker fetches audio
                                          │
                                          ▼
                               ┌──────────────────┐
                               │  TTS Provider     │
                               │  (Google or       │
                               │   custom server)  │
                               └──────────────────┘
```

The HMI Server uses standard UPnP SOAP calls (no proprietary Sonos API, no
cloud account, no authentication). This works with any speaker that implements
the UPnP AVTransport and RenderingControl services.

---

## 3. Prerequisites

- One or more Sonos speakers (or UPnP/DLNA renderers) on the same network as
  the HMI Server
- The HMI Server must have:
  - HTTP access to the speakers on port 1400 (or custom port)
  - Outbound HTTPS access to the TTS provider (Google Translate or your own)
- No Sonos account, app, or API key is required

### Compatible Speakers

| Speaker | Compatible | Notes |
|---|---|---|
| Sonos One / One SL | ✅ | Port 1400 |
| Sonos Era 100 / 300 | ✅ | Port 1400 |
| Sonos Five / Play:5 | ✅ | Port 1400 |
| Sonos Beam / Arc / Ray | ✅ | Port 1400 |
| Sonos Move / Roam | ✅ | Must be on Wi-Fi (not Bluetooth) |
| DLNA/UPnP renderers | ✅ | Port varies — check device docs |
| Sonos Ikea Symfonisk | ✅ | Port 1400 |

---

## 4. Find Your Speaker IP Addresses

### Option A — Sonos App

1. Open the **Sonos** app on your phone
2. Go to **Settings** → **System** → tap your speaker
3. The IP address is shown under **About** or **IP Address**

### Option B — Router Admin Panel

1. Open your router's admin page (usually `http://192.168.1.1`)
2. Look for connected devices named `Sonos-*` or with MAC addresses starting
   with `B8:E9:37`, `00:0E:58`, `5C:AA:FD`, `78:28:CA`, `48:A6:B8`, or `34:7E:5C`
3. Note the IP addresses

### Option C — Network Scan

From a terminal on the same network:

```bash
# Linux/Mac
nmap -p 1400 192.168.1.0/24 --open

# Windows (PowerShell)
1..254 | ForEach-Object {
    $ip = "192.168.1.$_"
    $tcp = New-Object System.Net.Sockets.TcpClient
    try { $tcp.Connect($ip, 1400); Write-Host "$ip - Sonos found"; $tcp.Close() }
    catch {}
}
```

### Option D — UPnP Discovery

Sonos speakers respond to SSDP discovery:

```bash
# Linux/Mac
gssdp-discover --timeout=5 | grep -i sonos
```

> **Tip:** Assign static IPs (or DHCP reservations) to your Sonos speakers so
> the addresses don't change after a router reboot.

---

## 5. Configure HMI Server

### Option A — Via the Editor UI

1. Open the **ServerEditorWeb** Blazor application
2. Select the **Server Settings** node in the project tree
3. Scroll to the **📨 Alarm Notifications** section
4. Ensure **Enable Alarm Notifications** is checked
5. Toggle the **🔈 IP Speaker (Sonos)** switch ON
6. Fill in:
   - **Speaker Addresses** — comma-separated list of speaker IPs
     (e.g. `192.168.1.50, 192.168.1.51:1400`)
   - **Volume** — announcement volume (0-100, 0 = keep current)
   - **TTS Language** — language code (e.g. `en`, `it`, `de`, `fr`)
   - **Restore volume** — check to restore previous volume after announcement
   - **Custom TTS URL** — leave empty for Google Translate TTS, or enter your
     own TTS endpoint
7. Save the project

### Option B — Via nodes.json

```json
{
  "ServerSettings": {
    "AlarmNotification": {
      "Enabled": true,
      "MinSeverity": 500,
      "CooldownSeconds": 60,
      "IpSpeaker": {
        "Enabled": true,
        "SpeakerAddresses": "192.168.1.50, 192.168.1.51",
        "Volume": 50,
        "TtsLanguage": "en",
        "TtsUrlTemplate": "",
        "RestoreVolume": true,
        "VolumeCritical": 100,
        "VolumeWarning": 70,
        "VolumeInfo": 0
      }
    }
  }
}
```

---

## 6. Custom TTS Providers

The built-in Google Translate TTS is convenient for testing but has limitations:

- Not an official API (may rate-limit or change without notice)
- Limited to ~200 characters per request
- No voice customization

For production, configure a custom TTS URL. The template supports two
placeholders:

| Placeholder | Replaced With |
|---|---|
| `{text}` | URL-encoded alarm message text |
| `{lang}` | Language code from config (e.g. `en`) |

### Example TTS Providers

**Local Piper TTS (open-source, self-hosted):**

```
http://192.168.1.100:5000/api/tts?text={text}&lang={lang}
```

**Amazon Polly (via proxy):**

```
https://my-tts-proxy.example.com/speak?text={text}&voice=Joanna&format=mp3
```

**Google Cloud TTS (via proxy):**

```
https://my-tts-proxy.example.com/synthesize?text={text}&lang={lang}
```

**Microsoft Azure TTS (via proxy):**

```
https://my-tts-proxy.example.com/tts?text={text}&locale={lang}
```

> **Important:** The TTS URL must return a direct audio stream (MP3 or other
> format supported by the speaker). The speaker will fetch this URL directly,
> so it must be reachable from the speaker's network.

---

## 7. Test the Integration

### Step 1 — Verify speaker reachability

From the HMI Server machine, test connectivity to each speaker:

```powershell
# PowerShell
Test-NetConnection -ComputerName 192.168.1.50 -Port 1400
```

You should see `TcpTestSucceeded: True`.

### Step 2 — Trigger a test alarm

Force a variable above its alarm threshold. If configured correctly, the
speaker will:

1. Change volume to the configured level
2. Play the TTS announcement (e.g. "CRITICAL alarm. Tank1.Level. High level
   exceeded. Severity 900")
3. Restore the original volume (if enabled)

### Step 3 — Check logs

Look for these messages in the HMI Server console:

```
[INF] Alarm IP Speaker announcement sent for Tank1.Level to 2 speaker(s).
```

Or errors:

```
[WRN] IP Speaker 192.168.1.50:1400 failed.
```

---

## 8. Troubleshooting

| Symptom | Likely Cause | Fix |
|---|---|---|
| No sound, no error in logs | Speaker is on a different VLAN/subnet | Ensure HMI Server and speakers are on the same network segment |
| Connection refused on port 1400 | Wrong IP, speaker offline, or firewall | Verify IP with `Test-NetConnection`; check speaker is powered on |
| Sound plays but no speech | TTS URL returns empty/invalid audio | Test the TTS URL in a browser — it should play an audio file |
| Volume too loud/quiet | Volume setting mismatch | Adjust the Volume setting (0-100) |
| Speaker plays but cuts off | TTS text too long for Google Translate | Use a custom TTS provider for longer messages or split the text |
| Previous music doesn't resume | UPnP SetAVTransportURI replaces the queue | This is a Sonos limitation; the speaker will be silent after the announcement until the user starts playback again |
| Wrong language pronunciation | TtsLanguage not matching content | Set `TtsLanguage` to match the message language (e.g. `it` for Italian) |

### Common Log Messages

| Log | Meaning |
|---|---|
| `Alarm IP Speaker announcement sent for {Path} to N speaker(s)` | All speakers were contacted |
| `Alarm notification IP Speaker skipped — no speaker addresses configured` | `SpeakerAddresses` is empty |
| `IP Speaker {Address} failed` | Could not reach that specific speaker |

---

## 9. JSON Configuration Reference

### `IpSpeakerNotificationChannel` Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Enabled` | `bool` | `false` | Master toggle for IP Speaker notifications |
| `SpeakerAddresses` | `string` | `""` | Comma-separated speaker IPs or IP:port pairs |
| `Volume` | `int` | `50` | Announcement volume (0-100). 0 = don't change |
| `TtsLanguage` | `string` | `"en"` | Language code for TTS |
| `TtsUrlTemplate` | `string` | `""` | Custom TTS URL with `{text}` and `{lang}` placeholders. Empty = Google Translate |
| `RestoreVolume` | `bool` | `true` | Restore previous volume after announcement |

### UPnP SOAP Services Used

| Service | Actions | Purpose |
|---|---|---|
| `RenderingControl:1` | `GetVolume`, `SetVolume` | Read/set speaker volume |
| `AVTransport:1` | `SetAVTransportURI`, `Play` | Load and play the TTS audio |

### Full Example (all notification channels)

```json
{
  "ServerSettings": {
    "AlarmNotification": {
      "Enabled": true,
      "MinSeverity": 1,
      "CooldownSeconds": 60,
      "Email": {
        "Enabled": true,
        "SmtpHost": "smtp.gmail.com",
        "SmtpPort": 587,
        "UseSsl": true,
        "Username": "alerts@mycompany.com",
        "Password": "app-password",
        "From": "alerts@mycompany.com",
        "To": "ops@mycompany.com"
      },
      "Telegram": {
        "Enabled": true,
        "BotToken": "123456:ABC-DEF...",
        "ChatId": "-1001234567890",
        "UseHtml": true
      },
      "Alexa": {
        "Enabled": true,
        "ClientId": "amzn1.application-oa2-client.abc123...",
        "ClientSecret": "your-secret",
        "Region": "EU",
        "UseSandbox": false
      },
      "IpSpeaker": {
        "Enabled": true,
        "SpeakerAddresses": "192.168.1.50, 192.168.1.51",
        "Volume": 60,
        "TtsLanguage": "en",
        "TtsUrlTemplate": "",
        "RestoreVolume": true,
        "VolumeCritical": 100,
        "VolumeWarning": 70,
        "VolumeInfo": 0
      }
    }
  }
}
```

---

## 10. Severity-based Volume

You can set different volume levels based on alarm severity:

| Field | Severity Range | Description |
|-------|---------------|-------------|
| `VolumeCritical` | >= 800 | Volume for critical alarms |
| `VolumeWarning` | 500-799 | Volume for warning alarms |
| `VolumeInfo` | < 500 | Volume for informational alarms |

Set any to `0` to fall back to the default `Volume`. Per-alarm `VolumeOverride` takes highest priority.

## 11. Per-alarm Overrides

Each alarm variable can override the global speaker list and volume:

- **Speaker Override** - comma-separated IP addresses; only these speakers play for this alarm
- **Volume Override** - 0-100; overrides both global and severity-based volume

Set these in **Variable Properties > Alarm > Notify on activation** section.

## 12. Script API

Scripts can trigger notifications programmatically:

```csharp
// Send to all enabled notification channels
SendNotification("Tank overflow", "Level exceeded 95%", severity: 800);

// Play TTS on specific speakers with custom volume
PlayOnSpeakers("Emergency evacuation", "192.168.1.50, 192.168.1.51", volume: 100);

// Play with custom language
PlayOnSpeakers("Allarme livello alto", "192.168.1.50", volume: 80, language: "it");
```

---

## Further Resources

- [UPnP AVTransport Service Specification](http://upnp.org/specs/av/UPnP-av-AVTransport-v1-Service.pdf)
- [Sonos UPnP API (community docs)](https://sonos.svrooij.io/)
- [Piper TTS — open-source local TTS](https://github.com/rhasspy/piper)
- [Google Cloud Text-to-Speech](https://cloud.google.com/text-to-speech)
- [Amazon Polly](https://aws.amazon.com/polly/)
