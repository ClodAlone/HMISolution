# HMISolution — Architecture & Feature Reference

> **Version:** 1.0 · **Date:** July 2025 · **Platform:** .NET 10 · **License:** Proprietary (RSA-signed)

---

## Table of Contents

1. [Overview](#1-overview)
2. [Solution Structure](#2-solution-structure)
3. [System Architecture](#3-system-architecture)
4. [SharedModels — Data Model Layer](#4-sharedmodels--data-model-layer)
5. [Server — OPC UA Runtime Engine](#5-server--opc-ua-runtime-engine)
6. [Communication Drivers](#6-communication-drivers)
7. [ServerEditorWeb — Project Editor](#7-servereditorweb--project-editor)
8. [RuntimeViewer — HMI Client](#8-runtimeviewer--hmi-client)
9. [RuntimeViewer.Desktop — Native Desktop Shell](#9-runtimeviewerdesktop--native-desktop-shell)
10. [Screens & Visualization](#10-screens--visualization)
11. [Animation System](#11-animation-system)
12. [Scripting Engine](#12-scripting-engine)
13. [PLC Programs (IEC 61131-3)](#13-plc-programs-iec-61131-3)
14. [Recipe Management](#14-recipe-management)
15. [Alarm System](#15-alarm-system)
16. [Data Logging & Historical Data Access (HDA)](#16-data-logging--historical-data-access-hda)
17. [Event Journal](#17-event-journal)
18. [IP Camera & YOLO Object Detection](#18-ip-camera--yolo-object-detection)
19. [User Management & Security](#19-user-management--security)
20. [Localization (i18n)](#20-localization-i18n)
21. [Diagnostics & Monitoring](#21-diagnostics--monitoring)
22. [Crash Reporting](#22-crash-reporting)
23. [Licensing](#23-licensing)
24. [Project File Format](#24-project-file-format)
25. [Testing](#25-testing)
26. [Deployment](#26-deployment)
27. [Technology Stack](#27-technology-stack)

---

## 1. Overview

**HMISolution** is a full-featured industrial HMI/SCADA platform built entirely on .NET 10. It provides:

- An **OPC UA server** that reads a single JSON project file and exposes a complete variable address space
- A **web-based project editor** (Blazor Server) for designing screens, configuring variables, writing scripts, and managing the entire project
- A **web-based runtime viewer** (Blazor Server) that connects to the server via OPC UA and renders live HMI screens
- A **native desktop viewer** (Photino/WebView) for kiosk and standalone deployments
- A **plugin-based driver architecture** supporting 11 industrial protocols
- Built-in **scripting** (C#/VB.NET), **PLC programs** (IEC 61131-3 ST/IL/LD), **recipes**, **alarms**, **data logging**, **event journal**, **IP cameras with YOLO detection**, and **AI-assisted editing**

The entire project configuration is stored in a single `nodes.json` file (with optional external resource files for screens, scripts, and PLC programs), making it easy to version-control, backup, and deploy.

---

## 2. Solution Structure

```
HMISolution/
├── SharedModels/                  # Shared data model & utilities (all projects reference this)
├── Server/                        # OPC UA server runtime engine
├── Drivers/                       # Communication driver plugins
│   ├── Drivers.Abstractions/      #   IDriver interface contract
│   ├── Drivers.Simulation/        #   Waveform signal generator
│   ├── Drivers.Modbus/            #   Modbus TCP client
│   ├── Drivers.S7/                #   Siemens S7 (S7comm)
│   ├── Drivers.Mqtt/              #   MQTT pub/sub client
│   ├── Drivers.OpcUaClient/       #   OPC UA client (gateway)
│   ├── Drivers.Knx/               #   KNX/IP building automation
│   ├── Drivers.EtherNetIP/        #   Allen-Bradley EtherNet/IP CIP
│   ├── Drivers.Rest/              #   HTTP/REST API poller
│   ├── Drivers.Sql/               #   SQL database query
│   ├── Drivers.Csv/               #   CSV file reader
│   └── Drivers.Tcp/               #   Raw TCP socket (regex parsing)
├── ServerEditorWeb/               # Blazor Server web editor application
├── RuntimeViewer/                 # Blazor Server web runtime viewer
├── RuntimeViewer.Shared/          # Shared Razor Class Library for viewer components
├── RuntimeViewer.Desktop/         # Photino native desktop shell for the viewer
├── Tests/                         # xUnit v3 test projects
│   ├── Tests.SharedModels/
│   ├── Tests.Editor/
│   ├── Tests.RuntimeViewer/
│   └── Tests.Drivers/
└── docs/                          # Documentation
```

### Project Dependency Graph

```
SharedModels ◄──── Server
     ▲               ▲
     │               │
     ├──── Drivers.Abstractions ◄── Drivers.* (all 11 drivers)
     │
     ├──── ServerEditorWeb
     │
     ├──── RuntimeViewer.Shared ◄── RuntimeViewer
     │                           ◄── RuntimeViewer.Desktop
     └──── Tests.*
```

---

## 3. System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        nodes.json                                │
│               (single-file project configuration)                │
└──────────┬───────────────────────┬──────────────────────────────┘
           │                       │
           ▼                       ▼
┌─────────────────────┐  ┌──────────────────────────┐
│   ServerEditorWeb   │  │        Server             │
│   (Blazor Server)   │  │   (OPC UA + Hosted Svc)   │
│                     │  │                            │
│  • Tree editor      │  │  • OPC UA address space    │
│  • Screen designer  │  │  • Driver loader (plugin)  │
│  • Script editor    │  │  • Script engine (Roslyn)  │
│  • PLC editor       │  │  • PLC runtime (ST/IL/LD)  │
│  • Property grid    │  │  • Alarm evaluator         │
│  • AI assistant     │  │  • Recipe manager (SQLite)  │
│  • Git integration  │  │  • Data logger (TS/SQLite)  │
│  • Undo/Redo        │  │  • Event journal (SQLite)   │
│  • Symbol library   │  │  • Camera + YOLO (ONNX)    │
│  • Diagnostics      │  │  • Diagnostics HTTP API     │
│  • OPC browse       │  │  • Crash reporter           │
│  • Crash reports    │  │  • Windows Service support   │
└─────────┬───────────┘  └──────────┬─────────────────┘
          │                         │ OPC UA TCP
          │ Reads/writes            │
          │ nodes.json              ▼
          │              ┌──────────────────────────┐
          │              │   RuntimeViewer (Web)     │
          │              │   or RuntimeViewer.Desktop│
          │              │                           │
          │              │  • Screen renderer (SVG)  │
          │              │  • Live variable binding   │
          │              │  • Gauge/Chart/Trend       │
          │              │  • Alarm list              │
          │              │  • Event log viewer        │
          │              │  • HDA chart/grid          │
          │              │  • Recipe widget           │
          │              │  • IP camera stream        │
          │              │  • User authentication     │
          │              │  • Localization (i18n)     │
          │              │  • Kiosk mode              │
          │              └──────────────────────────┘
          │
          ▼
┌─────────────────────┐
│   drivers/ folder   │
│   (plugin DLLs)     │
│                     │
│  Drivers.Modbus.dll │
│  Drivers.S7.dll     │
│  Drivers.Mqtt.dll   │
│  ... (11 total)     │
└─────────────────────┘
```

### Data Flow

1. **Design time:** The editor reads/writes `nodes.json` and external resource files (`screens/`, `scripts/`, `plcprograms/`, `recipes/`)
2. **Runtime:** The server loads `nodes.json`, builds the OPC UA address space, discovers and loads driver DLLs, starts scripts/PLC programs, initializes alarms and data logging
3. **Visualization:** The runtime viewer connects to the server via OPC UA, subscribes to variable changes, and renders screens with live data bindings

---

## 4. SharedModels — Data Model Layer

The `SharedModels` project defines the complete data model shared across all applications. Everything is serialized to/from JSON.

### Core Model (`NodeModel`)

| Property | Type | Description |
|---|---|---|
| `Database` | `DatabaseConfig?` | TimescaleDB/SQLite data logging configuration |
| `Users` | `List<UserConfig>` | User accounts with PBKDF2-hashed passwords |
| `UserGroups` | `List<UserGroupConfig>` | Access groups (Read/Write/ReadWrite, editor/runtime access) |
| `Scripts` | `List<ScriptConfig>` | C#/VB.NET cyclic scripts |
| `PlcPrograms` | `List<PlcProgramConfig>` | IEC 61131-3 programs (ST/IL/LD) |
| `Screens` | `List<ScreenConfig>` | HMI screen definitions |
| `Recipes` | `List<RecipeConfig>` | Recipe definitions with variable mappings |
| `Folder` | `Folder` | Root folder of the OPC UA variable tree |
| `Strings` | `List<LocalizedStringEntry>` | Localization string table |
| `Cameras` | `List<CameraConfig>` | IP camera configurations |
| `Server` | `ServerSettings` | OPC UA endpoint, auth, diagnostics settings |

### Variable Model

Each `Variable` in the folder tree has:

| Property | Description |
|---|---|
| `Name` | Variable name (forms the OPC UA browse path) |
| `Type` | Data type: `Double`, `Int32`, `Boolean`, `String`, `DateTime`, `Float`, `Int16`, `UInt16`, `UInt32` |
| `Access` | `Read`, `Write`, or `ReadWrite` |
| `Value` | Initial/current value |
| `MaxAge` | Optional maximum cache age |
| `Alarm` | Alarm configuration (limit or condition trigger) |
| `DataLogging` | Data logging configuration (interval, deadband, retention) |
| `DriverConfigs` | JSON extension data — driver-specific configs keyed by driver name |

Driver configurations are stored as `[JsonExtensionData]` on the variable, allowing any driver to attach its own config block without modifying the core model.

### Utility Classes

| Class | Purpose |
|---|---|
| `PasswordHasher` | PBKDF2-SHA256 password hashing (16-byte salt, 32-byte hash, 100K iterations) |
| `PasswordValidator` | Configurable password strength validation (weak: 4+ chars, strong: 8+ with complexity) |
| `LicenseManager` | RSA-signed license validation with hardware fingerprinting |
| `CrashReporter` | Global unhandled exception handler with JSON crash reports and SMTP email |
| `ResourceFileManager` | External resource file management (screens/, scripts/, plcprograms/, recipes/) |

---

## 5. Server — OPC UA Runtime Engine

The server is a .NET Generic Host application that:

1. **Loads** `nodes.json` and builds an OPC UA address space from the folder/variable tree
2. **Discovers** driver plugin DLLs from a `drivers/` folder and loads them via reflection
3. **Wires** each variable's driver config to the appropriate driver instance
4. **Starts** cyclic subsystems: scripts, PLC programs, alarm evaluator, data loggers
5. **Exposes** an OPC UA TCP endpoint (default: `opc.tcp://localhost:14840`)
6. **Runs** as a console app or Windows Service

### Key Server Components

| Component | File | Description |
|---|---|---|
| `OpcUaWorker` | `Program.cs` | `BackgroundService` that starts/stops the OPC UA server |
| `OpcUaServerApp` | `Program.cs` | Configures the OPC UA stack (certificates, endpoints, security) |
| `SimpleFileServerNodeManager` | `SimpleFileServerNodeManager.cs` | The OPC UA node manager — creates nodes from the folder tree, handles read/write, alarm evaluation |
| `DriverLoader` | `DriverLoader.cs` | Discovers and loads `IDriver` implementations from DLLs via reflection |
| `ScriptManager` | `ScriptManager.cs` | Compiles and runs C#/VB.NET scripts using Roslyn |
| `PlcManager` | `PlcManager.cs` | Compiles and runs IEC 61131-3 PLC programs (ST, IL, LD) |
| `RecipeManager` | `RecipeManager.cs` | SQLite-backed recipe storage with Load/Save/Activate/Delete OPC commands |
| `EventLogger` | `EventLogger.cs` | SQLite event journal for alarms, auth, driver events, system events |
| `TimescaleLogger` | `TimescaleLogger.cs` | TimescaleDB historical data logger with hypertable support |
| `SqliteLogger` | `SqliteLogger.cs` | SQLite-based historical data logger (lightweight alternative) |
| `DiagnosticsCollector` | `DiagnosticsCollector.cs` | Per-subsystem performance metrics exposed via HTTP (`GET /diag`) |
| `CameraStreamService` | `CameraStreamService.cs` | IP camera capture (RTSP/MJPEG/HTTP), YOLO detection (ONNX), MJPEG streaming |

### Server Startup Sequence

```
1. Parse CLI args → configPath, optional --service
2. Install CrashReporter
3. Validate license (RSA signature + hardware fingerprint)
4. Configure Serilog (console + rolling file in Logs/)
5. Register as Windows Service (if applicable)
6. Start DiagnosticsCollector HTTP endpoint
7. Load nodes.json → NodeModel
8. Build OPC UA configuration (endpoint URL, certificates, security)
9. Create address space from Folder/Variable tree
10. Load and wire driver plugins
11. Initialize ScriptManager, PlcManager, RecipeManager
12. Initialize EventLogger, DataLogger
13. Start CameraStreamService for configured cameras
14. Enter host.Run() loop
```

---

## 6. Communication Drivers

All drivers implement the `IDriver` interface:

```csharp
public interface IDriver : IDisposable
{
    string Key { get; }                                    // e.g. "Modbus", "S7"
    void AddItem(BaseDataVariableState variable, string configJson);
    event Action<string, string>? OnError;                 // source, error message
    event Action<double>? OnCycleCompleted;                // elapsed ms
}
```

Drivers are **loaded at runtime as plugins** from DLLs in the `drivers/` folder. The server discovers any class implementing `IDriver` and instantiates it. Each variable's `[JsonExtensionData]` block is matched to a driver by key name.

### Driver Catalog

| Driver | Key | Protocol | Use Case |
|---|---|---|---|
| **Simulation** | `Simulation` | Internal | Signal generator (Sine, Cosine, Ramp, Triangle, Square, Sawtooth, Random, RandomInt, Blink, Counter, Pulse) |
| **Modbus** | `Modbus` | Modbus TCP | PLCs, sensors, meters, motor drives |
| **S7** | `S7` | S7comm | Siemens S7-300/400/1200/1500 PLCs |
| **MQTT** | `Mqtt` | MQTT 3.1.1/5.0 | IoT devices, message brokers |
| **OPC UA Client** | `OpcUaClient` | OPC UA | Gateway to other OPC UA servers |
| **KNX** | `Knx` | KNXnet/IP | Building automation (lighting, HVAC, blinds) |
| **EtherNet/IP** | `EtherNetIP` | CIP over EtherNet/IP | Allen-Bradley/Rockwell PLCs |
| **REST** | `Rest` | HTTP/REST | Web APIs, cloud services |
| **SQL** | `Sql` | ADO.NET | Database queries |
| **CSV** | `Csv` | File I/O | CSV file data source |
| **TCP** | `Tcp` | Raw TCP socket | Serial-over-Ethernet devices (scales, barcode readers) |

### Example Variable with Driver Config

```json
{
  "Name": "Temperature",
  "Type": "Int16",
  "Access": "ReadWrite",
  "Value": 250,
  "Modbus": {
    "IpAddress": "192.168.10.245",
    "Port": 502,
    "UnitId": 1,
    "Register": 2741,
    "RegisterType": "HoldingRegister",
    "PollTime": 2000
  }
}
```

### Simulation Driver Functions

| Function | Output | Description |
|---|---|---|
| `Sine` | Double | Sine wave: `offset + amplitude × sin(2πt/period + phase)` |
| `Cosine` | Double | Cosine wave |
| `Ramp` | Double | Incrementing value with step and wrap-around |
| `Triangle` | Double | Triangle wave |
| `Square` | Double | Square wave with configurable duty cycle |
| `Sawtooth` | Double | Sawtooth wave |
| `Random` | Double | Random value in [min, max] |
| `RandomInt` | Int32 | Random integer in [min, max] |
| `Blink` | Boolean | Alternating true/false with duty cycle |
| `Counter` | Double | Monotonically incrementing counter |
| `Pulse` | Double | Pulse (1.0 during duty cycle, 0.0 otherwise) |

---

## 7. ServerEditorWeb — Project Editor

The editor is a **Blazor Server** web application providing a full IDE-like experience for designing HMI projects.

### Editor Features

| Feature | Component | Description |
|---|---|---|
| **Project Tree** | `TreeView.razor` | Hierarchical view of folders, variables, screens, scripts, PLC programs, recipes, cameras |
| **Screen Designer** | `ScreenEditorPanel.razor` | SVG canvas editor with drag-and-drop symbol placement, multi-select, alignment |
| **Responsive Layout** | `ResponsiveScreenEditorPanel.razor` | CSS Grid-based responsive screen editor |
| **Property Inspector** | `NodeProperties.razor` | Context-sensitive property editor for selected tree node |
| **Property Grid** | `PropertyGridPanel.razor` | Detailed property grid for symbols with categorized fields |
| **Symbol Library** | `SymbolLibraryPanel.razor` | Built-in and custom SVG symbol palette for drag-to-canvas |
| **Code Editor** | `CodeEditor.razor` | Monaco-based editor for C# scripts, VB.NET, and PLC programs |
| **Ladder Editor** | `LadderEditorPanel.razor` | Visual ladder diagram editor for IEC 61131-3 LD programs |
| **Variable Picker** | `VariablePathPicker.razor` | Searchable variable path selector with tree navigation |
| **Driver Config** | `VariableDriverEditor.razor` | Per-driver configuration UI (auto-generated from driver schema) |
| **User Management** | `UserManagementPanel.razor` | User/group CRUD, password management, auth settings |
| **String Editor** | `StringEditorPanel.razor` | Localization string table editor (key + translations per language) |
| **Event Log Settings** | `EventLogSettingsPanel.razor` | Event journal configuration |
| **Data Logging** | `DataLoggingPanel.razor` | HDA data viewer with chart and grid |
| **OPC Browse** | `OpcBrowsePanel.razor` | Live OPC UA address space browser with value monitoring |
| **JSON Editor** | `JsonEditor.razor` | Raw JSON editor with syntax highlighting |
| **AI Assistant** | `AiPanel.razor` | OpenAI/Gemini integration for AI-assisted JSON editing |
| **Git Integration** | `GitPanel.razor` | Git commit/push/pull from within the editor |
| **Server Control** | `ServerPanel.razor` | Start/stop the OPC UA server process, view diagnostics |
| **Log Viewer** | `LogViewer.razor` | Real-time server log streaming |
| **Crash Reports** | `CrashReportsPanel.razor` | View/email crash reports from all processes |
| **Undo/Redo** | `UndoRedoService.cs` | Full undo/redo with property change tracking and collection operations |
| **Clipboard** | `ClipboardService.cs` | Cut/Copy/Paste for tree nodes and screen symbols |
| **Dockable Layout** | `DockContainer.razor` | Resizable, dockable panel layout with persistent state |
| **Themes** | `ThemeService.cs` | Light/dark theme switching |
| **Context Menu** | `ContextMenu.razor`, `TreeViewContextMenu.razor` | Right-click context menus for tree and canvas operations |
| **Syntax Check** | `SyntaxCheckService.cs` | Real-time C# and PLC code validation using Roslyn |
| **Login** | `LoginDialog.razor` | Optional editor authentication |

### Editor Services

| Service | Scope | Description |
|---|---|---|
| `NodeEditorService` | Singleton | Core project model management — load, save, new, tree building, selection |
| `ServerProcessService` | Singleton | Manages the server process lifecycle (start/stop/restart) |
| `RuntimeViewerProcessService` | Singleton | Manages the runtime viewer process lifecycle |
| `ServerDiagnosticsClient` | Singleton | Polls the server's diagnostics HTTP API |
| `UndoRedoService` | Singleton | Undo/redo stack with configurable history depth |
| `ClipboardService` | Singleton | Internal clipboard for tree nodes and symbols |
| `SymbolLibraryService` | Singleton | Loads/manages SVG symbol libraries |
| `DockLayoutService` | Singleton | Persistent dock panel layout state |
| `ThemeService` | Singleton | Theme management |
| `PropertyGridService` | Singleton | Property grid metadata resolution |
| `AiService` | Singleton | OpenAI (GPT-4o) and Gemini API integration |
| `SyntaxCheckService` | Singleton | Roslyn-based C# syntax validation |
| `DataLoggingReaderService` | Singleton | Reads HDA data from TimescaleDB/SQLite for the editor chart |
| `GitService` | Scoped | Git operations (commit, push, pull, status, log) |
| `AuthService` | Scoped | Editor authentication against project user list |
| `OpcClientService` | Scoped | OPC UA client for live browse/monitoring |

---

## 8. RuntimeViewer — HMI Client

The runtime viewer is a **Blazor Server** web application that renders HMI screens with live OPC UA data.

### Runtime Architecture

```
Browser ◄──── SignalR ────► Blazor Server (RuntimeViewer)
                                │
                                ▼
                        OPC UA Subscription
                                │
                                ▼
                         Server (OPC UA)
```

### Runtime Services

| Service | Scope | Description |
|---|---|---|
| `ProjectService` | Singleton | Loads the project JSON and provides screen/settings access |
| `OpcRuntimeClient` | Scoped | OPC UA client with subscription-based live value updates |
| `CommandService` | Scoped | Executes symbol commands (navigate, set/toggle/write variable, scripts) |
| `RuntimeAuthService` | Scoped | User login/logout, access level checking, password change |
| `LocalizationService` | Scoped | Resolves `@key` string references to translated text |
| `HdaReaderService` | Singleton | Reads historical data from the server for HDA widgets |
| `EventLogReaderService` | Singleton | Reads event log entries from the server's SQLite journal |

### Runtime Widgets

| Widget | Type Key | Description |
|---|---|---|
| `ScreenRenderer` | — | Core SVG/responsive renderer for all screen symbols |
| `AlarmListWidget` | `alarmlist` | Live alarm list with acknowledge/reset |
| `EditBoxWidget` | `editbox` | Numeric/text input with spin buttons and format string |
| `EventLogWidget` | `eventlog` | Scrollable event journal viewer with category filters |
| `HdaChartWidget` | `hdachart` | Historical data chart (line/area) with time range selection |
| `HdaGridWidget` | `hdagrid` | Historical data table with export |
| `IpCameraWidget` | `ipcamera` | Live MJPEG camera stream with detection overlay |
| `RealtimeTrendWidget` | `trend` | Real-time scrolling trend chart with multiple pens |
| `RecipeWidget` | `recipe` | Recipe load/save/activate/delete UI |
| `ScreenEmbedWidget` | `screenembed` | Embed other screens (single or tabbed) |

---

## 9. RuntimeViewer.Desktop — Native Desktop Shell

A **Photino** (lightweight Chromium WebView) wrapper that:

1. Finds and starts the RuntimeViewer web process on a random port
2. Opens a native borderless window pointing to the web app
3. Supports `--kiosk` flag for fullscreen, chromeless operation
4. Handles process lifecycle (auto-restart on crash)

---

## 10. Screens & Visualization

### Screen Model

| Property | Description |
|---|---|
| `Name` | Screen name (used for navigation) |
| `Width` / `Height` | Canvas size in pixels |
| `Background` | Background color |
| `BackgroundImage` | Base64 data URI background image |
| `LayoutMode` | `"svg"` (fixed SVG canvas) or `"responsive"` (CSS Grid) |
| `GridColumns` / `GridGap` | Responsive grid settings |
| `Group` | Editor folder organization |
| `Symbols` | List of `ScreenSymbol` elements |

### Symbol Types

| Type | Description |
|---|---|
| `rect` | Rectangle |
| `circle` | Circle |
| `ellipse` | Ellipse |
| `text` | Text label |
| `line` | Line |
| `svg` | Custom SVG content (from symbol library) |
| `gauge` | Gauge widget (needle, arc, semicircle, hbar, vbar, thermometer) |
| `indicator` | Status indicator |
| `editbox` | Numeric/text input with spin buttons |
| `alarmlist` | Live alarm list widget |
| `hdachart` | Historical data chart |
| `hdagrid` | Historical data grid |
| `eventlog` | Event log viewer |
| `ipcamera` | IP camera stream |
| `recipe` | Recipe management widget |
| `screenembed` | Embedded screen(s) with optional tabs |
| `imagemap` | Condition-to-image mapping widget |
| `trend` | Real-time scrolling trend chart |

### Gauge Styles

| Style | Description |
|---|---|
| `needle` | Classic dial with rotating needle |
| `arc` | Arc/donut with filled progress stroke |
| `semicircle` | 180° half-circle arc gauge |
| `hbar` | Horizontal bar fill |
| `vbar` | Vertical bar fill (bottom-up) |
| `thermometer` | Vertical thermometer with bulb |

### Runtime Bindings (Expression-Based)

| Binding | Type | Example |
|---|---|---|
| `FillBinding` | Color expression | `"value > 50 ? '#ff0000' : '#00ff00'"` |
| `VisibilityBinding` | Boolean expression | `"value == true"` |
| `RotationBinding` | Numeric expression | `"value * 3.6"` |
| `LabelBinding` | String expression | `"value.ToString('F1') + ' °C'"` |

### Symbol Commands

Commands are executed at runtime when the user interacts with a symbol:

| Action | Description |
|---|---|
| `NavigateScreen` | Switch to another screen |
| `OpenScreenPopup` | Open screen in a popup |
| `OpenScreenModal` | Open screen as a modal dialog |
| `SetVariable` | Write a value to an OPC variable |
| `ResetVariable` | Reset variable to its default |
| `ToggleVariable` | Toggle a boolean variable |
| `IncrementVariable` | Increment by a step amount |
| `DecrementVariable` | Decrement by a step amount |
| `WriteVariable` | Write an arbitrary value |
| `ExecuteJavaScript` | Run browser-side JavaScript |
| `ExecuteScript` | Run a C# script |
| `Login` / `Logout` | Trigger runtime authentication |
| `AcknowledgeAllAlarms` | Acknowledge all active alarms |
| `ResetAllAlarms` | Reset all acknowledged alarms |
| `ChangeLanguage` | Switch the active localization language |

Command triggers: `Click`, `MouseDown`, `MouseUp`, `WhilePressed`

---

## 11. Animation System

### Keyframe Animations (Editor)

Each symbol can have multiple `SymbolAnimation` entries:

| Property | Description |
|---|---|
| `Property` | Animated property: `Position`, `Scale`, `Rotation`, `Opacity`, `Fill`, `Stroke`, `StrokeWidth`, `Width`, `Height` |
| `Trigger` | `Always` (loop), `OnVariable` (condition-based), `OnClick` |
| `TriggerVariable` | OPC variable path for `OnVariable` trigger |
| `TriggerCondition` | Expression: `"value > 60"` |
| `Keyframes` | List of `{ Percent, Value, Easing? }` defining the timeline |
| `DurationMs` | Animation duration in milliseconds |
| `Easing` | `Linear`, `EaseIn`, `EaseOut`, `EaseInOut`, `Bounce`, `Elastic` |
| `RepeatCount` | 0 = infinite loop |
| `AutoReverse` | Ping-pong playback |

### Animatable Properties (9 total)

| Property | Value Format | Example |
|---|---|---|
| Position | `"x,y"` | `"100,200"` |
| Scale | `"factor"` | `"1.5"` |
| Rotation | `"degrees"` | `"360"` |
| Opacity | `"0.0–1.0"` | `"0.5"` |
| Fill | `"#rrggbb"` | `"#ff0000"` |
| Stroke | `"#rrggbb"` | `"#00ff00"` |
| StrokeWidth | `"pixels"` | `"4"` |
| Width | `"pixels"` | `"120"` |
| Height | `"pixels"` | `"80"` |

---

## 12. Scripting Engine

Scripts are compiled and executed using **Roslyn** (Microsoft.CodeAnalysis):

| Feature | Description |
|---|---|
| **Languages** | C# and VB.NET |
| **Execution** | Cyclic at configurable interval (default 1000ms) |
| **API** | Full access to OPC variables via `Read("path")` and `Write("path", value)` |
| **Events** | `OnVariableChanged` subscription for reactive scripting |
| **Diagnostics** | Per-script execution time and error tracking |
| **Hot reload** | Scripts are recompiled when the project is saved |

### Script Example (C#)

```csharp
// Cyclic script: calculate average and write alarm
var temp1 = (double)Read("Plant.Line1.Temperature");
var temp2 = (double)Read("Plant.Furnace.FurnaceTemperature");
var avg = (temp1 + temp2) / 2.0;
Write("Plant.Calculated.AvgTemperature", avg);

if (avg > 80)
    Write("Plant.Calculated.TempAlarm", true);
```

---

## 13. PLC Programs (IEC 61131-3)

The server includes a built-in IEC 61131-3 runtime supporting three programming languages:

| Language | Code | Description |
|---|---|---|
| **Structured Text** | `ST` | Pascal-like imperative language (most common) |
| **Instruction List** | `IL` | Assembly-like stack-based language |
| **Ladder Diagram** | `LD` | Visual relay logic (JSON-serialized rungs) |

### Features

- Cyclic execution at configurable interval (default 100ms)
- Full variable access via OPC variable paths
- Compiled to an internal representation for efficient execution
- Timer, counter, and edge detection function blocks
- Visual ladder editor in the ServerEditorWeb

---

## 14. Recipe Management

Recipes store named parameter sets in **SQLite databases**:

| Feature | Description |
|---|---|
| **Storage** | One SQLite database per recipe definition |
| **Operations** | Load, Save, Activate, Delete — all via OPC variables |
| **Variables** | Index-based mapping to OPC variable paths |
| **OPC Interface** | `Recipe.{Name}.Load`, `.Save`, `.Activate`, `.Delete`, `.ActiveName`, `.RecipeList`, `.LastStatus` |
| **Runtime Widget** | `RecipeWidget` provides a full UI for recipe operations |

---

## 15. Alarm System

### Alarm Configuration

| Property | Description |
|---|---|
| `TriggerType` | `Limit` (numeric thresholds) or `Condition` (boolean expression) |
| `HighHighLimit` | Critical high threshold |
| `HighLimit` | Warning high threshold |
| `LowLimit` | Warning low threshold |
| `LowLowLimit` | Critical low threshold |
| `Hysteresis` | Deadband to prevent alarm flickering |
| `Message` | Alarm message text |

### Alarm Lifecycle

```
Normal → Active (threshold exceeded) → Acknowledged → Normal (value returns)
                                    → Reset (manual)
```

- Alarms are evaluated cyclically by the server's node manager
- Active alarms are exposed as OPC UA events
- The `AlarmListWidget` shows live alarms with acknowledge/reset buttons
- Alarm events are recorded in the event journal

---

## 16. Data Logging & Historical Data Access (HDA)

### Data Logging Configuration (per variable)

| Property | Description |
|---|---|
| `Enabled` | Whether logging is active |
| `IntervalMs` | Minimum logging interval |
| `DeadbandType` | `None`, `Absolute`, or `Percent` |
| `DeadbandValue` | Deadband threshold |
| `RetentionDays` | Auto-purge after N days (0 = keep forever) |

### Storage Backends

| Backend | Class | Description |
|---|---|---|
| **TimescaleDB** | `TimescaleLogger` | PostgreSQL + TimescaleDB hypertables for high-volume time-series data |
| **SQLite** | `SqliteLogger` | Lightweight local SQLite database for smaller deployments |

Both implement `IVariableLogger`:

```csharp
public interface IVariableLogger : IDisposable
{
    void Initialize();
    void Log(BaseDataVariableState variable, DataLoggingConfig config);
    List<DataValue> ReadHistory(string variableNodeId, DateTime startTime, DateTime endTime);
}
```

### HDA Widgets

- **HDA Chart** (`hdachart`): Line/area chart with configurable time range and multiple series
- **HDA Grid** (`hdagrid`): Tabular data view with timestamp, variable, and value columns

---

## 17. Event Journal

A centralized **SQLite-backed** event log recording all server events:

| Category | Events |
|---|---|
| `Alarm` | Alarm active, acknowledged, reset, cleared |
| `Auth` | User login, logout, password change, failed attempts |
| `Driver` | Connection errors, poll failures, reconnections |
| `System` | Server start/stop, configuration changes, license events |

| Feature | Description |
|---|---|
| **Storage** | SQLite with WAL mode for concurrent read/write |
| **Auto-purge** | Configurable max age (default 90 days) |
| **Indexes** | Optimized for time-range and category queries |
| **Runtime Widget** | `EventLogWidget` with scrollable view, category filter, and time range |
| **Configuration** | `EventLogConfig` in `ServerSettings` |

---

## 18. IP Camera & YOLO Object Detection

### Camera Support

| Protocol | Description |
|---|---|
| **RTSP** | Via FFmpeg subprocess (H.264/H.265 → MJPEG) |
| **MJPEG** | Direct HTTP MJPEG stream parsing |
| **HTTP** | Periodic snapshot polling at configurable FPS |

### YOLO Object Detection

- Runs **YOLOv8 ONNX** models via `Microsoft.ML.OnnxRuntime`
- Detects objects in each frame and draws bounding boxes
- Writes detection results to OPC variables: `{Prefix}.Label`, `{Prefix}.Confidence`, `{Prefix}.Count`
- Configurable confidence threshold (default 0.5)
- Image processing via `SixLabors.ImageSharp`

### Camera Widget

The `IpCameraWidget` in the runtime viewer displays the live MJPEG stream served by the server.

---

## 19. User Management & Security

### Authentication Layers

| Layer | Description |
|---|---|
| **OPC UA** | Anonymous or username/password authentication |
| **Editor** | Optional web login (configurable via `EnableEditorLogin`) |
| **Runtime** | Optional web login (configurable via `EnableRuntimeLogin`) |

### Password Security

- **PBKDF2-SHA256** with 16-byte random salt and 100,000 iterations
- Automatic migration from legacy plaintext passwords on login
- Configurable password strength policy (weak: 4+ chars, strong: 8+ with complexity rules)
- Password expiry with configurable days and forced change on first login

### Access Control

| Group Property | Description |
|---|---|
| `AccessLevel` | `Read`, `Write`, or `ReadWrite` |
| `CanAccessEditor` | Whether group members can use the editor |
| `CanAccessRuntime` | Whether group members can use the runtime viewer |

Symbols can have a `RequiredAccess` property to restrict visibility/interaction based on the user's access level.

### User Features

| Feature | Description |
|---|---|
| Auto log-off | Configurable inactivity timeout per user |
| First login password change | Force password reset on first authentication |
| Password expiry | Automatic expiry after N days |

---

## 20. Localization (i18n)

### String Table

The `Strings` collection in `NodeModel` stores localization entries:

```json
{
  "Key": "btn_start",
  "Translations": {
    "en": "Start",
    "de": "Starten",
    "it": "Avvia",
    "fr": "Démarrer"
  }
}
```

### Usage

- Symbol labels prefixed with `@` are resolved at runtime: `@btn_start` → `"Start"` (English)
- Language can be changed at runtime via the `ChangeLanguage` command
- Fallback chain: active language → first available translation → key name
- The editor provides a `StringEditorPanel` for managing the string table

---

## 21. Diagnostics & Monitoring

### Server Diagnostics HTTP API

The server exposes a lightweight HTTP endpoint (default port 14841):

**`GET /diag`** returns JSON:

```json
{
  "UptimeSeconds": 3600,
  "CpuPercent": 12.5,
  "MemoryMB": 245,
  "Subsystems": [
    {
      "Category": "Driver",
      "Name": "Simulation",
      "Enabled": true,
      "Status": "Running",
      "LastCycleMs": 0.42,
      "AvgCycleMs": 0.38,
      "MaxCycleMs": 1.2,
      "CycleCount": 7200,
      "ErrorCount": 0,
      "LastError": null
    }
  ]
}
```

### Editor Diagnostics

- `ServerDiagnosticsClient` polls the diagnostics API and displays metrics in the `ServerPanel`
- `ProcessPerformanceTracker` monitors server process CPU/memory from the editor
- `OpcBrowsePanel` provides live OPC UA address space browsing with value monitoring
- `LogViewer` streams the server's Serilog output in real time

### Runtime Diagnostics

- The runtime viewer has a `🔍 Diag` button in the top bar that shows OPC connection status, subscription metrics, and variable values

---

## 22. Crash Reporting

All three processes (Server, Editor, RuntimeViewer) install the `CrashReporter`:

| Feature | Description |
|---|---|
| **Capture** | `AppDomain.UnhandledException`, `TaskScheduler.UnobservedTaskException` |
| **Format** | Structured JSON with exception details, stack trace, memory, thread count, OS info |
| **Storage** | `crash_reports/` folder next to the executable |
| **Email** | Optional SMTP email notification via `CrashEmailConfig` |
| **Auto-send** | Configurable auto-send on crash or manual send from the editor's Crash Reports panel |

### Crash Report Structure

```json
{
  "Id": "a1b2c3d4e5f6",
  "TimestampUtc": "2025-07-14T18:30:00Z",
  "Process": "Server",
  "ProcessId": 12345,
  "MachineName": "PROD-HMI-01",
  "OsDescription": "Microsoft Windows 10.0.19045",
  "RuntimeVersion": ".NET 10.0.4",
  "ExceptionType": "System.NullReferenceException",
  "Message": "Object reference not set...",
  "StackTrace": "...",
  "Source": "AppDomain",
  "MemoryMB": 245,
  "ThreadCount": 18
}
```

---

## 23. Licensing

The `LicenseManager` provides RSA-signed license validation:

| Feature | Description |
|---|---|
| **Key pair** | RSA key generation for developer (private key) and deployment (public key) |
| **Hardware fingerprint** | Machine name + OS + MAC address + processor count → SHA-256 hash |
| **Tiers** | Configurable license tiers (e.g., Basic, Professional, Enterprise) |
| **Validation** | RSA signature verification against embedded public key |
| **License file** | JSON file with signature, placed next to `nodes.json` |
| **Graceful fallback** | Runs in demo/unlicensed mode if no valid license found |

---

## 24. Project File Format

### Main File: `nodes.json`

A single JSON file containing the complete project configuration. See the [Core Model](#core-model-nodemodel) for structure.

### External Resource Files

For large projects, resources are stored in external files:

```
project-folder/
├── nodes.json                    # Main configuration
├── screens/
│   ├── MainScreen.json           # Individual screen definitions
│   └── AlarmOverview.json
├── scripts/
│   ├── AvgCalc.json              # Individual script definitions
│   └── AlarmHandler.json
├── plcprograms/
│   ├── MotionControl.json        # Individual PLC program definitions
│   └── TemperatureLoop.json
├── recipes/
│   └── BatchRecipe.db            # SQLite recipe database
├── Logs/
│   └── log-nodes-20250714.txt    # Rolling log files
├── crash_reports/
│   └── crash-Server-20250714.json
└── events.db                     # Event journal SQLite database
```

The `ResourceFileManager` handles transparent loading/saving of external resources, with backward compatibility for inline resources.

---

## 25. Testing

The solution includes **4 xUnit v3 test projects** with **87 tests**:

| Project | Tests | Coverage |
|---|---|---|
| `Tests.SharedModels` | 27 | PasswordHasher, PasswordValidator, NodeModel serialization, JSON round-trips |
| `Tests.Editor` | 17 | UndoRedoService, NodeEditorService (delete, undo/redo, multi-select) |
| `Tests.RuntimeViewer` | 30 | RuntimeAuthService (login/logout, access levels, groups), LocalizationService (resolve, fallback, languages) |
| `Tests.Drivers` | 13 | SimulationDriver (all waveforms, polling, dispose), SimulationConfig serialization |

Run all tests:
```bash
dotnet run --project Tests/Tests.SharedModels
dotnet run --project Tests/Tests.Editor
dotnet run --project Tests/Tests.RuntimeViewer
dotnet run --project Tests/Tests.Drivers
```

---

## 26. Deployment

### Console Application

```bash
dotnet run --project Server -- "C:\projects\myplant\nodes.json"
```

### Windows Service

```bash
# Install as Windows Service
sc create SimpleOpcFileServer_myplant binPath= "C:\deploy\Server.exe C:\projects\myplant\nodes.json"
sc start SimpleOpcFileServer_myplant
```

The server uses `Microsoft.Extensions.Hosting.WindowsServices` for native service integration.

### Editor

```bash
dotnet run --project ServerEditorWeb
# Opens at https://localhost:5001
```

### Runtime Viewer (Web)

```bash
dotnet run --project RuntimeViewer -- "C:\projects\myplant\nodes.json"
# Opens at https://localhost:5002
```

### Runtime Viewer (Desktop)

```bash
dotnet run --project RuntimeViewer.Desktop -- "C:\projects\myplant\nodes.json"
# Kiosk mode:
dotnet run --project RuntimeViewer.Desktop -- "C:\projects\myplant\nodes.json" --kiosk
```

### Server Settings

| Setting | Default | Description |
|---|---|---|
| `EndpointUrl` | `opc.tcp://localhost:14840/SimpleOpcFileServer` | OPC UA endpoint |
| `EnableAnonymous` | `true` | Allow anonymous OPC connections |
| `EnableEditorLogin` | `false` | Require editor authentication |
| `EnableRuntimeLogin` | `false` | Require runtime viewer authentication |
| `RequireStrongPassword` | `false` | Enforce strong password policy |
| `ShowNavigationBar` | `false` | Show screen navigation bar in kiosk mode |
| `StartupScreen` | `""` | Initial screen in runtime viewer |
| `DiagnosticsPort` | `14841` | HTTP diagnostics API port (0 = disabled) |

---

## 27. Technology Stack

| Layer | Technology |
|---|---|
| **Runtime** | .NET 10 |
| **OPC UA** | OPCFoundation.NetStandard.Opc.Ua |
| **Web UI** | Blazor Server (Interactive Server Components) |
| **Desktop Shell** | Photino.NET (Chromium WebView) |
| **Scripting** | Roslyn (Microsoft.CodeAnalysis.CSharp.Scripting) |
| **Data Logging** | TimescaleDB (Npgsql) / SQLite (Microsoft.Data.Sqlite) |
| **Camera** | FFmpeg (RTSP), SixLabors.ImageSharp, ONNX Runtime (YOLO) |
| **Modbus** | NModbus4 |
| **S7** | S7NetPlus |
| **MQTT** | MQTTnet |
| **KNX** | Knx.Falcon |
| **EtherNet/IP** | libplctag |
| **Logging** | Serilog (Console + Rolling File) |
| **Testing** | xUnit v3 |
| **Build** | MSBuild / `dotnet build` |
| **Source Control** | Git |

---

*This document was generated from the HMISolution source code at [github.com/ClodAlone/HMISolution](https://github.com/ClodAlone/HMISolution).*
