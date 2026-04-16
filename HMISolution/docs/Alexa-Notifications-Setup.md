# Alexa Alarm Notifications — Setup Guide

This guide walks you through setting up Alexa notifications for HMI Server
alarms. When an alarm fires, Alexa-enabled devices linked to your skill will
receive a notification (yellow ring + chime) with the alarm details.

---

## Table of Contents

1. [Overview](#1-overview)
2. [Architecture](#2-architecture)
3. [Prerequisites](#3-prerequisites)
4. [Create an Alexa Skill](#4-create-an-alexa-skill)
5. [Enable Proactive Events](#5-enable-proactive-events)
6. [Get LWA Credentials](#6-get-lwa-credentials)
7. [Configure HMI Server](#7-configure-hmi-server)
8. [Test the Integration](#8-test-the-integration)
9. [Troubleshooting](#9-troubleshooting)
10. [JSON Configuration Reference](#10-json-configuration-reference)

---

## 1. Overview

The Alexa notification channel uses the **Alexa Proactive Events API** to send
push notifications to all Alexa-enabled devices linked to your skill. This is
the officially supported Amazon approach — no unofficial APIs or screen scraping
required.

| Feature | Details |
|---|---|
| API | Alexa Proactive Events API v1 |
| Authentication | Login with Amazon (LWA) OAuth2 `client_credentials` |
| Event schema | `AMAZON.MessageAlert.Activated` |
| Delivery | Push notification (yellow ring + chime on Echo devices) |
| Audience | All users who enabled your skill (multicast) |
| Regions | NA (North America), EU (Europe), FE (Far East) |

---

## 2. Architecture

```
HMI Server                         Amazon Cloud
┌──────────────────┐          ┌─────────────────────┐
│ NotificationService│         │  Login with Amazon  │
│                    │──POST──►│  (OAuth2 token)     │
│  SendAlexaAnnounce │         └─────────────────────┘
│         │          │                │ access_token
│         ▼          │                ▼
│  POST proactive    │         ┌─────────────────────┐
│  event with token  │──POST──►│  Alexa Proactive    │
│                    │         │  Events API         │
└──────────────────┘          └─────────┬───────────┘
                                        │ notification
                                        ▼
                               ┌─────────────────┐
                               │  Echo / Alexa    │
                               │  devices (ring)  │
                               └─────────────────┘
```

**Flow:**
1. Alarm fires → `NotificationService` detects Alexa channel is enabled
2. Acquires (or reuses cached) LWA access token via `client_credentials` grant
3. POSTs an `AMAZON.MessageAlert.Activated` proactive event to the regional API
4. All Alexa devices linked to the skill receive a notification

---

## 3. Prerequisites

- An [Amazon Developer Account](https://developer.amazon.com/) (free)
- At least one Alexa-enabled device (Echo, Echo Dot, Echo Show, etc.) signed
  into the same Amazon account
- The HMI Server must have outbound HTTPS access to:
  - `api.amazon.com` (LWA token endpoint)
  - `api.amazonalexa.com` (NA), `api.eu.amazonalexa.com` (EU), or
    `api.fe.amazonalexa.com` (FE)

---

## 4. Create an Alexa Skill

### Step 1 — Open the Alexa Developer Console

Navigate to **https://developer.amazon.com/alexa/console/ask** and sign in with
your Amazon developer account.

### Step 2 — Create a new skill

1. Click **Create Skill**
2. Enter a skill name, e.g. `HMI Alarm Monitor`
3. Choose your **Primary locale** (e.g. English (US))
4. For **Experience type**, select **Other**
5. For **Model**, select **Custom**
6. For **Hosting**, select **Provision your own** (we only need the skill to
   host proactive events, not a Lambda backend)
7. Click **Next**, then **Create Skill**
8. On the template screen, choose **Start from Scratch** → **Continue with template**

### Step 3 — Set an invocation name

1. In the left sidebar, go to **Invocations** → **Skill Invocation Name**
2. Set a name like `h m i alarms` (Alexa invocation names must be lowercase and
   at least two words)
3. Click **Save**

### Step 4 — Add a minimal intent

The skill needs at least one intent to be valid. You don't need any real
interaction — a placeholder intent is enough.

1. In the left sidebar, go to **Interaction Model** → **Intents**
2. There should already be the required built-in intents. If the console
   requires a custom intent, add one named `CheckAlarmsIntent` with a sample
   utterance like `check my alarms`
3. Click **Save** then **Build Skill**

### Step 5 — Skip the endpoint

Since this skill only sends proactive events (server → Alexa), you do **not**
need to configure an endpoint (Lambda or HTTPS). The skill will work for
notifications without one.

> **Note:** Users will see the skill in the Alexa app but it won't respond to
> voice commands — it only sends notifications. You can optionally add a simple
> Lambda to respond with "Check the HMI dashboard for alarm details."

---

## 5. Enable Proactive Events

### Step 1 — Open skill permissions

1. In the Alexa Developer Console, open your skill
2. Go to **Build** → **Permissions** (in the left sidebar under Tools)

### Step 2 — Enable Alexa Notifications

1. Toggle on **Alexa Notifications — Send** (also labeled
   **"Send Notifications to Users"** in some console versions)
2. Under the event schemas, check **AMAZON.MessageAlert.Activated**
3. Click **Save**

### Step 3 — Build and enable the skill

1. Click **Build Skill** to apply the permissions
2. Go to **Test** tab and set the testing toggle to **Development**
3. On your Alexa app (phone), go to **Skills & Games** → **Your Skills** →
   **Dev** and enable the skill. Grant the notification permission when prompted.

> **Important:** Each user who wants to receive notifications must enable the
> skill and grant the notification permission in their Alexa app.

---

## 6. Get LWA Credentials

### Step 1 — Find Client ID and Secret

1. In the Alexa Developer Console, open your skill
2. Go to **Build** → **Permissions**
3. At the bottom of the permissions page, find the section
   **Alexa Client Id** and **Alexa Client Secret**
4. Copy both values — you will paste them into the HMI Server configuration

> **Security:** Treat the Client Secret like a password. Do not commit it to
> version control or share it publicly.

### Step 2 — Verify credentials (optional)

You can verify your credentials work by requesting a token manually:

```bash
curl -X POST https://api.amazon.com/auth/o2/token \
  -d "grant_type=client_credentials" \
  -d "client_id=YOUR_CLIENT_ID" \
  -d "client_secret=YOUR_CLIENT_SECRET" \
  -d "scope=alexa::proactive_events"
```

A successful response looks like:

```json
{
  "access_token": "Atza|IQEBLjAs...",
  "expires_in": 3600,
  "scope": "alexa::proactive_events",
  "token_type": "bearer"
}
```

---

## 7. Configure HMI Server

### Option A — Via the Editor UI

1. Open the **ServerEditorWeb** Blazor application
2. Select the **Server Settings** node in the project tree
3. Scroll to the **📨 Alarm Notifications** section
4. Ensure **Enable Alarm Notifications** is checked
5. Toggle the **🔊 Alexa** switch ON
6. Fill in:
   - **Client ID** — the Alexa Client Id from step 6
   - **Client Secret** — the Alexa Client Secret from step 6
   - **Region** — select EU (Europe), NA (North America), or FE (Far East)
     matching your Amazon developer account region
   - **Sandbox** — check this while testing; uncheck for production
7. Save the project

### Option B — Via nodes.json

Add or update the `Alexa` section under `AlarmNotification` in your
`nodes.json` (or equivalent project file):

```json
{
  "ServerSettings": {
    "AlarmNotification": {
      "Enabled": true,
      "MinSeverity": 500,
      "CooldownSeconds": 60,
      "Alexa": {
        "Enabled": true,
        "ClientId": "amzn1.application-oa2-client.abc123def456...",
        "ClientSecret": "your-client-secret-here",
        "Region": "EU",
        "UseSandbox": false
      }
    }
  }
}
```

### Per-alarm opt-in

Only alarms with **Notify on Activation** enabled will trigger Alexa
notifications. In the editor, go to any variable's **Alarm** section and check
the *Notify on Activation* checkbox.

---

## 8. Test the Integration

### Step 1 — Use sandbox mode

Set **Sandbox (development)** to checked in the editor (or `"UseSandbox": true`
in JSON). This sends events to the development stage, which only reaches devices
linked to your developer account.

### Step 2 — Trigger a test alarm

Force a variable above its alarm threshold. If configured correctly, within a
few seconds your Echo device will:

1. Play the notification chime sound
2. Show a yellow ring (on Echo devices with a light ring)
3. On Echo Show devices, display the notification text

You can ask Alexa: *"Alexa, what are my notifications?"* to hear the alarm
details read aloud.

### Step 3 — Check logs

Look in the HMI Server console output for:

```
[INF] Alexa alarm notification sent for MyVariable.
```

Or for errors:

```
[WRN] Alexa Proactive Events API returned Forbidden: { ... }
```

### Step 4 — Switch to live mode

Once testing is successful, uncheck **Sandbox** (or set `"UseSandbox": false`).
Submit your skill for certification if you want other users beyond your
developer account to receive notifications.

---

## 9. Troubleshooting

| Symptom | Likely Cause | Fix |
|---|---|---|
| `401 Unauthorized` from token endpoint | Wrong Client ID or Secret | Verify credentials in the Alexa Developer Console under Permissions |
| `403 Forbidden` from Proactive Events API | Skill not enabled or notifications permission not granted | Enable the skill in the Alexa app and grant notification permission |
| `400 Bad Request` | Invalid event payload or schema not enabled | Ensure `AMAZON.MessageAlert.Activated` is checked in skill Permissions |
| No notification on device | Sandbox mode on but device not linked to dev account, or notification permission not granted | Use same Amazon account; re-enable skill and grant permissions |
| Token expires frequently | Normal — LWA tokens last 3600s | The server caches tokens automatically and refreshes 60s before expiry |
| Wrong region | Events sent to NA but account is EU | Match the **Region** setting to your Amazon developer account marketplace |

### Common Log Messages

| Log | Meaning |
|---|---|
| `Alexa LWA token acquired, expires in 3600s` | Token obtained successfully |
| `Alexa alarm notification sent for {Path}` | Event delivered to the API |
| `Alarm notification Alexa skipped — ClientId or ClientSecret not configured` | Credentials are empty |
| `Alexa Proactive Events API returned {Status}: {Body}` | API rejected the event — check the body for details |

---

## 10. JSON Configuration Reference

### `AlexaNotificationChannel` Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Enabled` | `bool` | `false` | Master toggle for Alexa notifications |
| `ClientId` | `string` | `""` | Login with Amazon (LWA) Client ID from the Alexa developer console |
| `ClientSecret` | `string` | `""` | Login with Amazon (LWA) Client Secret |
| `Region` | `string` | `"EU"` | Alexa API region: `"NA"` (North America), `"EU"` (Europe), or `"FE"` (Far East) |
| `UseSandbox` | `bool` | `false` | When `true`, sends events to the development sandbox (only reaches dev account devices) |

### Regional API Endpoints

| Region | Proactive Events Endpoint |
|---|---|
| NA | `https://api.amazonalexa.com/v1/proactiveEvents/stages/{live\|development}` |
| EU | `https://api.eu.amazonalexa.com/v1/proactiveEvents/stages/{live\|development}` |
| FE | `https://api.fe.amazonalexa.com/v1/proactiveEvents/stages/{live\|development}` |

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
        "To": "ops@mycompany.com, manager@mycompany.com"
      },
      "Telegram": {
        "Enabled": true,
        "BotToken": "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11",
        "ChatId": "-1001234567890",
        "UseHtml": true
      },
      "WhatsApp": {
        "Enabled": false
      },
      "Alexa": {
        "Enabled": true,
        "ClientId": "amzn1.application-oa2-client.abc123...",
        "ClientSecret": "your-secret",
        "Region": "EU",
        "UseSandbox": false
      }
    }
  }
}
```

---

## Further Resources

- [Alexa Proactive Events API documentation](https://developer.amazon.com/en-US/docs/alexa/smapi/proactive-events-api.html)
- [Login with Amazon — Getting Started](https://developer.amazon.com/docs/login-with-amazon/web-docs.html)
- [Proactive Events Schemas Reference](https://developer.amazon.com/en-US/docs/alexa/smapi/schemas-for-proactive-events.html)
- [Alexa Skills Kit CLI (ASK CLI)](https://developer.amazon.com/en-US/docs/alexa/smapi/quick-start-alexa-skills-kit-command-line-interface.html) — optional, for managing skills from the command line
