# HMI Solution — Feature Reference

This document describes all major features added to the HMI Solution platform,
including server-side services, editor panels, runtime viewer capabilities, and
the REST API demo project.

---

## Table of Contents

1. [Alarm Notification System](#1-alarm-notification-system)
2. [Alarm Shelving](#2-alarm-shelving)
3. [Certificate Management & GDS](#3-certificate-management--gds)
4. [Project Backup & Restore](#4-project-backup--restore)
5. [Runtime Tag Browser](#5-runtime-tag-browser)
6. [REST API](#6-rest-api)
7. [Variable Statistics](#7-variable-statistics)
8. [Localization](#8-localization)
9. [Diagnostics Panel](#9-diagnostics-panel)
10. [User Authentication & Security](#10-user-authentication--security)

---

## 1. Alarm Notification System

**Components:**
- `Server/NotificationService.cs` — Sends alarm notifications via Email (SMTP),
  Telegram Bot API, and WhatsApp (Twilio).
- `SharedModels/NodeModels.cs` — `AlarmNotificationConfig` in `ServerSettings`;
  `NotifyOnActivation` flag on `AlarmConfig`.

**Configuration (nodes.json):**
```json
{
  "Server": {
    "AlarmNotification": {
      "SmtpHost": "smtp.example.com",
      "SmtpPort": 587,
      "SmtpUser": "alerts@example.com",
      "SmtpPassword": "secret",
      "SmtpUseSsl": true,
      "EmailRecipients": "ops@example.com",
      "TelegramBotToken": "123456:ABC...",
      "TelegramChatId": "-1001234567890",
      "WhatsAppAccountSid": "AC...",
      "WhatsAppAuthToken": "...",
      "WhatsAppFrom": "whatsapp:+14155238886",
      "WhatsAppTo": "whatsapp:+1234567890"
    }
  }
}
```

**Per-Variable:**
Set `Alarm.NotifyOnActivation = true` on any variable's `AlarmConfig` to trigger
a notification when the alarm activates.

**Editor UI:**
- Variable Properties → Alarm section → *Notify on Activation* checkbox
- Server Settings → *Alarm Notification* section with SMTP/Telegram/WhatsApp fields

---

## 2. Alarm Shelving

**Components:**
- `Server/SimpleFileServerNodeManager.cs` — `ShelveAlarm` / `UnshelveAlarm`
  OPC UA methods under the `_AlarmManagement` object.
- `RuntimeViewer.Shared/Services/OpcRuntimeClient.cs` — `ShelveAlarmAsync()` /
  `UnshelveAlarmAsync()` client methods.
- `RuntimeViewer.Shared/Components/Viewer/AlarmListWidget.razor` — Shelve/Unshelve
  buttons per alarm row.

**How it works:**
1. Operator selects an alarm in the alarm list widget.
2. Clicks **Shelve** and specifies a duration (in minutes).
3. The server suppresses the alarm condition for the specified duration.
4. After the duration, or if the operator clicks **Unshelve**, the alarm returns
   to normal evaluation.

---

## 3. Certificate Management & GDS

**Components:**
- `ServerEditorWeb/Services/CertificateService.cs` — Lists, generates, and
  manages OPC UA PKI certificates. Supports Global Discovery Server (GDS)
  registration, certificate signing requests, and trust list management.
- `ServerEditorWeb/Components/Editor/CertificatePanel.razor` — Full UI panel
  in the editor for certificate operations.
- `SharedModels/NodeModels.cs` — `GdsConfig` class with GDS endpoint, credentials,
  and application settings.

**Capabilities:**
- View application certificate, trusted/rejected/issuer stores
- Generate self-signed certificates
- Register with a GDS, submit CSR, pull signed certificate
- Accept or reject certificates in the rejected store
- Configure GDS settings in Server Settings

**Editor UI:**
Panel registered as **Certificates** (🔐) in the dock layout.

---

## 4. Project Backup & Restore

**Components:**
- `ServerEditorWeb/Services/BackupService.cs` — Creates ZIP snapshots of the
  project file (nodes.json) and all resource directories (scripts/, screens/,
  plcprograms/). Supports manual, auto-save, and scheduled triggers.
- `ServerEditorWeb/Components/Editor/BackupPanel.razor` — Lists snapshots,
  allows restore, manual snapshot creation, and retention management.
- `SharedModels/NodeModels.cs` — `BackupConfig` class with schedule interval,
  retention count, and auto-snapshot-on-save flag.

**Configuration:**
```json
{
  "Server": {
    "Backup": {
      "AutoSnapshotOnSave": true,
      "ScheduleIntervalMinutes": 60,
      "MaxSnapshots": 50
    }
  }
}
```

**Behavior:**
- **Auto-snapshot on save** — Creates a backup whenever the project is saved.
- **Scheduled snapshots** — Timer-based backups at a configurable interval.
- **Retention** — Oldest snapshots are pruned when `MaxSnapshots` is exceeded.
- **Restore** — Creates a safety backup of the current state, then restores from
  the selected snapshot.
- **File menu** → *Create Snapshot* entry for manual backups.

**Editor UI:**
Panel registered as **Backups** (💾) in the dock layout.

---

## 5. Runtime Tag Browser

**Components:**
- `RuntimeViewer.Shared/Services/OpcRuntimeClient.cs` — `BrowseChildrenAsync()`,
  `BrowseAllTagsAsync()`, `ReadTagValueAsync()`, `WriteTagValueAsync()` methods
  for OPC UA address space browsing.
- `RuntimeViewer.Shared/Services/OpcRuntimeClient.cs` — `BrowsedTag` model class
  with NodeId, DisplayName, BrowsePath, NodeClass, DataType, and tree state.
- `RuntimeViewer.Shared/Components/Viewer/TagBrowserPanel.razor` — Full-featured
  slide-out panel with tree view, search, live value display, and write capability.

**Features:**
- **Tree View** — Hierarchical browsing of the OPC UA Objects folder with
  lazy-loaded expand/collapse.
- **Search** — Real-time multi-term filtering across all browsed tags.
  Matching text is highlighted.
- **Live Values** — Selecting a variable shows its current value, polled every
  second. NodeId, data type, and browse path are displayed.
- **Write** — Operators can write new values to writable variables directly from
  the tag browser.
- **Refresh** — Re-browse the server to pick up runtime changes.

**RuntimeViewer UI:**
Toolbar button **🏷️ Tags** toggles the slide-out panel from the right side.

---

## 6. REST API

The OPC UA server exposes a built-in REST API on the diagnostics HTTP port
(default 14841). Endpoints cover variable read/write, browsing, alarms,
recipes, historical data, and server management.

**Demo Project:**
The `RestApiDemo` Blazor Web App (in the `RestApiDemo/` directory) provides
7 interactive pages demonstrating all API endpoints:
- Variables (read/write/browse)
- Alarms (list/acknowledge/confirm)
- Historical data queries
- Recipe management
- Server info and diagnostics

---

## 7. Variable Statistics

Variables with `Statistics.Enabled = true` track runtime min, max, average,
and count as sub-variables:
- `{path}.Statistics.Min`
- `{path}.Statistics.Max`
- `{path}.Statistics.Average`
- `{path}.Statistics.Count`

These are visible in the RuntimeViewer EditBox widget when `ShowStatistics`
is enabled.

---

## 8. Localization

**Editor Help:**
The help system supports **6 languages**: English (en), German (de), Italian (it),
French (fr), Japanese (ja), and Chinese (zh). Both TOC titles and full help
content are localized.

**Runtime Strings:**
The `Strings` table in the project model stores translated strings by ID.
Screen labels referencing `@StringId` are resolved at runtime based on the
active language.

---

## 9. Diagnostics Panel

The RuntimeViewer includes an **OPC Diagnostics** panel (🔍 Diag button)
showing:
- Connection state
- Diagnostic log entries
- Live value cache with current readings

---

## 10. User Authentication & Security

**Features:**
- PBKDF2-SHA256 password hashing
- User groups with Read/Write/ReadWrite access levels
- Editor login and runtime login modes
- Auto log-off on idle timeout
- Password expiry with forced change on next login
- Project-level password protection
- Strong password policy enforcement
