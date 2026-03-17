# Cloud Relay — Firewall-Transparent Tunnel for HMI

## Overview

The **Cloud Relay** system enables the RuntimeViewer (HMI client) to communicate with
the OPC UA Server even when a firewall separates them. It works by introducing a
cloud-hosted SignalR hub as a rendezvous point: both sides make **outbound
connections only**, so no inbound firewall ports are required on either network.

```
 VIEWER NETWORK                        CLOUD                          SERVER NETWORK
┌─────────────────┐              ┌──────────────────┐              ┌────────────────┐
│  RuntimeViewer   │  outbound   │                  │  outbound    │  CloudBridge   │
│  (Blazor app)    │────WSS────►│   CloudRelay     │◄────WSS─────│  (console app) │
│                  │   :443      │   (SignalR Hub)   │    :443      │                │
└─────────────────┘              └──────────────────┘              └───────┬────────┘
                                  Azure App Service                       │
                                  or any cloud host                       │ opc.tcp://
                                                                          │ (localhost)
                                                                    ┌─────▼──────┐
                                                                    │  OPC UA     │
                                                                    │  Server     │
                                                                    └────────────┘
```

### Key Properties

| Property | Value |
|---|---|
| Firewall changes needed | **None** — both sides use outbound HTTPS/WSS (port 443) |
| Protocol | SignalR over WebSocket (WSS), falls back to Server-Sent Events or Long Polling |
| Authentication | Shared API key sent as query-string parameter during handshake |
| Real-time latency | Typically < 100 ms for tag value updates (depends on cloud region) |
| Reconnection | Automatic on both sides with SignalR's built-in retry logic |

---

## Architecture Components

### 1. CloudRelay (Cloud Hub)

**Project:** `CloudRelay/`  
**Type:** ASP.NET Core Minimal API with SignalR  
**Deployment target:** Azure App Service, Docker container, or any cloud host

The relay is a lightweight message router. It does **not** store data or interpret
OPC UA semantics — it simply forwards messages between "Bridge" and "Viewer" groups.

```
CloudRelay/
├── Program.cs           # App startup, API-key middleware, hub mapping
├── RelayHub.cs          # SignalR hub — routes messages between groups
├── RelayOptions.cs      # Configuration model
├── CloudRelay.csproj    # ASP.NET Core web project
└── appsettings.json     # API key configuration
```

**Endpoints:**

| Endpoint | Description |
|---|---|
| `/relay` | SignalR hub endpoint (WebSocket upgrade) |
| `/health` | Health check — returns `200 OK` |

### 2. CloudBridge (Server-Side Agent)

**Project:** `CloudBridge/`  
**Type:** .NET console application  
**Deployment target:** Any machine on the OPC UA server's local network

The bridge makes two outbound connections:
1. **Local:** `opc.tcp://` to the OPC UA server (on the same LAN)
2. **Cloud:** WSS to the CloudRelay hub

It forwards tag values and alarms from OPC UA to the cloud, and routes write
commands and alarm actions from the cloud back to OPC UA.

```
CloudBridge/
├── Program.cs           # Entry point — reads nodes.json, starts bridge
├── BridgeService.cs     # Core logic — OPC UA client + SignalR client
└── CloudBridge.csproj   # Console app project
```

### 3. CloudRuntimeClient (Viewer-Side Client)

**File:** `RuntimeViewer.Shared/Services/CloudRuntimeClient.cs`  
**Type:** Scoped Blazor service

Alternative to `OpcRuntimeClient` — instead of connecting directly to OPC UA, it
connects to the CloudRelay hub via SignalR. The RuntimeViewer injects both clients
and uses `CloudRuntimeClient` when cloud relay mode is enabled.

### 4. Shared Contracts

**File:** `SharedModels/CloudRelay/CloudRelayContracts.cs`

Defines DTOs and hub method name constants shared by all three components:

| Type | Purpose |
|---|---|
| `HubMethods` | String constants for all SignalR method names |
| `TagValueDto` | Variable path + value pair |
| `AlarmEntryDto` | Alarm entry with condition ID, event ID (Base64), severity, timestamps |
| `WriteRequestDto` | Write command with correlation ID for response matching |
| `WriteResultDto` | Success/failure response for a write operation |
| `AlarmActionDto` | Acknowledge or confirm request with condition/event identifiers |

---

## Message Flow

### Real-Time Tag Values (Server → Viewer)

```
OPC UA Server
    │
    │ MonitoredItem notification (opc.tcp)
    ▼
CloudBridge
    │
    │ hub.SendAsync("ValuesUpdated", List<TagValueDto>)
    ▼
CloudRelay Hub ──► Clients.Group("Viewers").SendAsync(...)
    │
    │ WebSocket
    ▼
CloudRuntimeClient
    │
    │ ValuesChanged event
    ▼
Blazor UI updates
```

### Write Command (Viewer → Server)

```
Blazor UI
    │
    │ WriteValueAsync("Plant.Temp", "42.5")
    ▼
CloudRuntimeClient
    │
    │ hub.SendAsync("WriteValue", WriteRequestDto { CorrelationId = "abc" })
    ▼
CloudRelay Hub ──► Clients.Group("Bridges").SendAsync(...)
    │
    │ WebSocket
    ▼
CloudBridge
    │
    │ OPC UA Write (opc.tcp, local)
    │
    │ hub.SendAsync("WriteResult", WriteResultDto { CorrelationId = "abc", Success = true })
    ▼
CloudRelay Hub ──► Clients.Group("Viewers").SendAsync(...)
    │
    ▼
CloudRuntimeClient matches CorrelationId → completes Task<bool>
```

### Alarm Operations

The same pattern applies for alarm acknowledgement and confirmation:

| Viewer Action | Hub Method | Bridge Action |
|---|---|---|
| Request alarm list | `RequestAlarms` | Collects active alarms via ConditionRefresh, sends `AlarmsUpdated` |
| Acknowledge alarm | `AcknowledgeAlarm` | Calls `AcknowledgeableConditionType_Acknowledge` on OPC UA |
| Confirm/reset alarm | `ConfirmAlarm` | Calls `AcknowledgeableConditionType_Confirm` on OPC UA |

---

## Configuration

### nodes.json — Project Settings

Add the `CloudRelay` section to the `Server` settings in your `nodes.json` project file:

```json
{
  "Server": {
    "EndpointUrl": "opc.tcp://localhost:14840/SimpleOpcFileServer",
    "CloudRelay": {
      "Enabled": true,
      "HubUrl": "https://my-relay.azurewebsites.net/relay",
      "ApiKey": "a-strong-random-key-shared-by-all-components"
    }
  }
}
```

| Property | Type | Description |
|---|---|---|
| `Enabled` | `bool` | Set to `true` to activate cloud relay mode |
| `HubUrl` | `string` | Full URL of the CloudRelay SignalR hub endpoint |
| `ApiKey` | `string` | Shared secret for authenticating connections to the hub |

### CloudRelay — appsettings.json

```json
{
  "CloudRelay": {
    "ApiKey": "a-strong-random-key-shared-by-all-components"
  }
}
```

The API key can also be set via environment variable for production deployments:

```bash
CloudRelay__ApiKey=your-secret-key
```

---

## Deployment Guide

### Step 1: Deploy CloudRelay to Azure

#### Option A: Azure App Service

```bash
# From the CloudRelay/ directory
dotnet publish -c Release -o ./publish

# Deploy to Azure App Service (requires Azure CLI)
az webapp create --resource-group MyRG --plan MyPlan --name my-hmi-relay --runtime "DOTNETCORE:10.0"
az webapp deploy --resource-group MyRG --name my-hmi-relay --src-path ./publish --type zip

# Set the API key
az webapp config appsettings set --resource-group MyRG --name my-hmi-relay \
    --settings CloudRelay__ApiKey="your-secret-key"
```

#### Option B: Docker Container

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["CloudRelay/CloudRelay.csproj", "CloudRelay/"]
COPY ["SharedModels/SharedModels.csproj", "SharedModels/"]
RUN dotnet restore "CloudRelay/CloudRelay.csproj"
COPY . .
RUN dotnet publish "CloudRelay/CloudRelay.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CloudRelay.dll"]
```

### Step 2: Run CloudBridge on the Server Network

Place the CloudBridge executable on any machine that has local network access to the
OPC UA server. It only needs outbound HTTPS access to the cloud.

```bash
# Run with the project's nodes.json
CloudBridge /path/to/nodes.json
```

Expected output:

```
╔═══════════════════════════════════════╗
║        HMI Cloud Bridge               ║
║  OPC UA Server ←→ Cloud Relay Hub     ║
╚═══════════════════════════════════════╝
  OPC UA endpoint : opc.tcp://localhost:14840/SimpleOpcFileServer
  Cloud relay hub : https://my-relay.azurewebsites.net/relay

[14:30:01.234] Connecting to cloud relay hub...
[14:30:01.891] ✓ Connected to cloud relay hub
[14:30:02.015] Discovering OPC endpoints at opc.tcp://localhost:14840/SimpleOpcFileServer...
[14:30:02.234] Creating OPC session...
[14:30:02.567] ✓ Connected to OPC UA — SessionId=...
```

#### Running as a Service

**Windows (sc.exe):**

```bash
sc create CloudBridge binPath="C:\HMI\CloudBridge.exe C:\HMI\nodes.json" start=auto
sc start CloudBridge
```

**Linux (systemd):**

```ini
# /etc/systemd/system/cloudbridge.service
[Unit]
Description=HMI Cloud Bridge
After=network.target

[Service]
ExecStart=/opt/hmi/CloudBridge /opt/hmi/nodes.json
WorkingDirectory=/opt/hmi
Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
```

### Step 3: Configure RuntimeViewer

The RuntimeViewer automatically registers both `OpcRuntimeClient` and
`CloudRuntimeClient` via dependency injection. To switch the viewer to cloud mode,
set `CloudRelay.Enabled = true` in the project's `nodes.json`.

No code changes or recompilation required — the mode is determined at runtime from
the configuration file.

---

## Security Considerations

### API Key

The shared API key prevents unauthorized connections to the relay hub. For
production deployments:

- Use a cryptographically random key (minimum 32 characters)
- Store it in Azure Key Vault or environment variables, not in source control
- Rotate the key periodically and update all three components

### Transport Encryption

SignalR over WebSocket uses **TLS** (WSS) when the hub URL starts with `https://`.
Always use HTTPS for the CloudRelay endpoint in production.

### OPC UA Certificates

The CloudBridge auto-generates OPC UA application certificates for its local
connection to the server. These are stored in:

```
%LocalApplicationData%/CloudBridge/pki/
├── own/        # Bridge's application certificate
├── trusted/    # Trusted server certificates
├── issuer/     # CA certificates
└── rejected/   # Rejected certificates (review and trust as needed)
```

### Network Requirements

| Component | Outbound Access Required |
|---|---|
| CloudBridge | `opc.tcp://<server>:14840` (local LAN) + `https://<relay>:443` (cloud) |
| RuntimeViewer | `https://<relay>:443` (cloud) |
| CloudRelay | Inbound on port 443 (or 8080 behind a load balancer) |

No inbound ports are required on the CloudBridge or RuntimeViewer networks.

---

## Solution Structure

```
HMISolution.slnx
├── /Cloud/
│   ├── CloudRelay/                         ← Deploy to Azure
│   │   ├── Program.cs
│   │   ├── RelayHub.cs
│   │   ├── RelayOptions.cs
│   │   ├── appsettings.json
│   │   └── CloudRelay.csproj
│   └── CloudBridge/                        ← Deploy to server LAN
│       ├── Program.cs
│       ├── BridgeService.cs
│       └── CloudBridge.csproj
├── SharedModels/
│   ├── CloudRelay/
│   │   └── CloudRelayContracts.cs          ← Shared DTOs & constants
│   └── NodeModels.cs                       ← CloudRelayConfig added
├── RuntimeViewer.Shared/
│   └── Services/
│       ├── OpcRuntimeClient.cs             ← Direct OPC UA (existing)
│       └── CloudRuntimeClient.cs           ← Cloud relay mode (new)
└── RuntimeViewer/
    └── Program.cs                          ← Both clients registered in DI
```

---

## Troubleshooting

### Bridge cannot connect to the cloud hub

- Verify the `HubUrl` is reachable: `curl https://my-relay.azurewebsites.net/health`
- Check that the `ApiKey` matches between `nodes.json` and the relay's `appsettings.json`
- Ensure outbound HTTPS (port 443) is allowed from the bridge machine

### Bridge connects to hub but OPC UA session fails

- Verify the OPC UA server is running on the local network
- Check the `EndpointUrl` in `nodes.json` — the bridge must be able to reach it locally
- Review the bridge console output for endpoint discovery details

### Viewer shows "Bridge is OFFLINE"

- The CloudBridge process is not running or lost connection to the hub
- Check the bridge console output for reconnection attempts
- The hub notifies all viewers when a bridge disconnects via `BridgeStatusChanged`

### Write commands time out

- The write has a 10-second timeout in `CloudRuntimeClient`
- Verify the bridge is online and the OPC UA session is active
- Check bridge logs for write errors (data type conversion, access denied, etc.)

### High latency on tag updates

- Tag updates are pushed on every OPC UA notification — no batching delay
- Latency depends on: OPC sampling interval (250 ms default), cloud network round-trip
- Consider adjusting `PublishingInterval` and `SamplingInterval` for the subscription

---

## API Reference — Hub Methods

### Bridge → Hub → Viewers

| Method | Payload | Description |
|---|---|---|
| `ValuesUpdated` | `List<TagValueDto>` | Batch of current tag values |
| `AlarmsUpdated` | `List<AlarmEntryDto>` | Complete list of active alarms |
| `WriteResult` | `WriteResultDto` | Success/failure response (matched by `CorrelationId`) |
| `BridgeStatusChanged` | `bool` | `true` when bridge connects, `false` on disconnect |

### Viewers → Hub → Bridge

| Method | Payload | Description |
|---|---|---|
| `Subscribe` | `List<string>, int` | Variable paths + namespace index to monitor |
| `WriteValue` | `WriteRequestDto` | Write a value to an OPC UA variable |
| `AcknowledgeAlarm` | `AlarmActionDto` | Acknowledge an active alarm |
| `ConfirmAlarm` | `AlarmActionDto` | Confirm (reset) an active alarm |
| `RequestAlarms` | *(none)* | Request a fresh alarm snapshot |

### Connection Lifecycle

| Method | Called by | Description |
|---|---|---|
| `JoinAsBridge` | CloudBridge | Joins the `"Bridges"` SignalR group |
| `JoinAsViewer` | RuntimeViewer | Joins the `"Viewers"` SignalR group |
