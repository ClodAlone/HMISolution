# Future Features Roadmap

Suggested features identified through comprehensive codebase analysis.
Grouped by impact and implementation area.

---

## High-Impact Features

### 1. SFC Editor (Sequential Function Chart)
**Area:** PLC / IEC 61131-3 Editors

The project already covers ST, IL, and LD — SFC would complete the full IEC 61131-3 language suite.
A graphical state-machine editor with Steps, Transitions, and Actions drawn as a flow diagram.
The existing `PlcEditorPanel`, `PlcProgramConfig`, and CodeMirror infrastructure provide a strong foundation.

**Benefits:**
- Completes IEC 61131-3 compliance
- Enables sequential batch and machine control programs without scripting
- Natural companion to the existing Ladder editor

---

### 2. Variable Dependency Graph
**Area:** Cross-Reference / Analysis

The existing `CrossReferencePanel` returns text results. A complementary **visual network graph** would
render variables, scripts, alarms, and screens as nodes, with directed edges showing read/write
relationships. Hovering a node highlights all consumers and producers.

**Benefits:**
- Instant impact analysis before renaming or deleting a tag
- Identifies dead variables and circular dependencies
- Visually impressive demo feature for sales/presentations

---

### 3. Screen Navigation Map
**Area:** Screen Editor

A **visual flow diagram** of all screens in the project, with arrows between them representing
navigation links (which button on Screen A opens Screen B). Generated automatically by inspecting
`NavigateToScreen` commands in symbol properties.

**Benefits:**
- Instant overview of the entire HMI navigation structure
- Detects orphaned screens with no incoming links
- Standard feature in tools like Wonderware InTouch and Siemens WinCC

---

### 4. Live Tag Browser with Inline Values
**Area:** Project Tree / Runtime Integration

While the server is running, overlay **live current values** directly next to variable names in the
project tree — similar to Visual Studio's Locals/Watch window during debugging. Taps into the existing
`OpcClientService` and `WatchTableService`. Should also allow **inline write** from the tree node.

**Benefits:**
- Eliminates the need to open `OpcBrowsePanel` for quick diagnostics
- Speeds up commissioning significantly
- Deeply integrated feel — values update in context

---

### 5. No-Code Event/Action Rule Builder
**Area:** Automation / Scripting

A visual rule builder with a "**When [condition] → Then [action]**" paradigm, not requiring C# code.

- **Conditions:** variable threshold crossed, time schedule, alarm state change
- **Actions:** write variable, send notification, navigate screen, activate recipe, trigger script

Targets configuration-only users (technicians, integrators) who are not developers.
Can be backed by `SchedulerConfig` and `ScriptConfig` at the model level.

**Benefits:**
- Lowers barrier to entry for non-developer users
- Covers 80% of automation needs without a single line of code

---

### 6. Script Unit Test Framework
**Area:** Script Editor

An in-IDE panel to write **xUnit-style unit tests for C# scripts**: mock variable values, run the
script in isolation, and assert output values. Built on top of the existing `ScriptCompletionService`,
Roslyn compiler infrastructure, and `ScriptDebugService`.

**Benefits:**
- Enables test-driven development for script logic
- Catches regressions when scripts are edited
- Differentiator — no mainstream HMI/SCADA tool offers this

---

### 7. Protocol Traffic Monitor
**Area:** Driver / Communication

A **live message sniffer panel** inside the editor — displays raw Modbus frames, MQTT publish/subscribe
events, OPC UA reads/writes, and REST calls in real time while the server is running. Think
Wireshark scoped to the project's configured drivers.

**Benefits:**
- Invaluable for driver commissioning without leaving the IDE
- Instant diagnosis of polling failures, bad addresses, or malformed responses
- Builds trust with industrial engineers used to protocol analysers

**Implementation status: ✅ Implemented**

The panel is available in the bottom dock as **"🦈 Protocol Traffic Monitor"**.

**Architecture:**
- `ProtocolTrafficService` tails the same Serilog log file as the Server Logs panel, classifying
  each structured log line by protocol keyword (Modbus, MQTT, OPC UA, REST, S7, TCP, EtherNet/IP,
  KNX) and by direction (↑ Tx / ↓ Rx / • Internal). A ring buffer of 5 000 frames is maintained.
- `ProtocolTrafficMonitorPanel.razor` renders the frame table with:
  - Per-protocol colour-coded badges (red = Modbus, purple = MQTT, blue = OPC UA, green = REST …)
  - Protocol filter chips and direction (Tx / Rx / Internal) toggles
  - Free-text search across protocol, source, and message fields
  - ⏸ Pause / ▶ Resume without losing buffered frames
  - 🗑 Clear and 📥 CSV export to the project's Logs directory
  - Tail-mode auto-scroll toggle
  - Click-to-expand raw log line detail pane at the bottom

**Possible future enhancements:**
- Server-side binary frame capture (actual Modbus ADU bytes via NModbus hooks)
- MQTT message payload preview with JSON/hex formatting
- Timeline heat-map showing polling rate per driver
- Per-variable trace filter to watch a single tag's traffic

---

### 8. Integrated REST API Tester
**Area:** REST API / Server Integration

A **Postman-like panel** targeting the server's own REST API (`/api/variables`, `/api/alarms`, etc.),
pre-populated with the current project's variable paths. Supports one-click GET/PUT/POST directly
from the editor. Builds entirely on the existing `RestApiServer` at port 14842.

**Benefits:**
- Lets integrators test the API without leaving the editor
- Auto-generates example `curl` / Python snippets for documentation
- Low implementation effort — server already exposes all endpoints

---

### 9. Project Documentation Generator
**Area:** Documentation / Reporting

Auto-generates a **PDF or HTML document** from the open project containing:
- Variable list with descriptions, ranges, engineering units, and driver bindings
- Alarm list with limits, priorities, and notification settings
- Screen list with thumbnails
- Script and PLC program signatures
- User and group list

Reuses the existing `ReportConfig` rendering backend and `ReportEditorPanel` infrastructure.

**Benefits:**
- Required deliverable for most industrial projects (FAT/SAT documentation)
- Zero manual effort for the engineer
- Consistent, professional output

---

### 10. Offline Simulation Mode
**Area:** Testing / Developer Experience

A **"Run in Simulation" button** that temporarily swaps all driver bindings to the already-present
`SimulationDriver` and launches the `RuntimeViewer` against a fully-simulated server — without needing
real hardware. Signal waveforms (sine, ramp, random, etc.) are auto-assigned to match each variable's
type.

**Benefits:**
- Allows full project testing with realistic moving data in one click
- Best single improvement to new-user onboarding and demo experience
- Simulation driver already exists — only the swap and launch logic is new

---

### 11. Keyboard Shortcut Customizer
**Area:** Editor UX

A settings panel to **remap all editor hotkeys**. Power users coming from VS Code, TIA Portal, or
CODESYS have established muscle memory. The existing `_shortcutRef` infrastructure in `Home.razor`
provides a natural anchor point; shortcut bindings could be persisted alongside the dock layout via
`DockLayoutService` or a dedicated `ShortcutService`.

**Benefits:**
- Removes a common friction point for experienced users
- Matches expectations set by modern IDEs

---

### 12. Alarm Rationalization Worksheet (ISA-18.2)
**Area:** Alarm Management

The `AlarmAnalyticsDashboardPanel` provides statistics, but there is no structured **ISA-18.2
rationalization table** — a spreadsheet-style editor where each alarm is assigned:
- Consequence, cause, and safeguards
- Priority classification (P1–P4)
- Suppression conditions and shelving rules
- Required response time

This is a mandatory deliverable in regulated industries (pharmaceutical, oil & gas, nuclear).

**Benefits:**
- Opens the product to regulated/compliance-driven market segments
- Pairs naturally with the existing `ComplianceConfig` and FDA 21 CFR Part 11 support

---

## Quick-Win Smaller Features

| # | Feature | Area | Notes |
|---|---|---|---|
| QW-1 | **Tab history navigation (← →)** | Editor UX | Jump back to previously active dock tabs like browser history |
| QW-2 | **Split editor panes** | Screen / Script Editors | Show two scripts or screens side-by-side in the center zone |
| QW-3 | **Pinned / favourite variables** | Project Tree | Star variables to a quick-access list at the top of the tree — ✅ Implemented |
| QW-4 | **Diff view for scripts in Git panel** | Git Integration | Side-by-side diff of script files within `GitPanel` — ✅ Implemented |
| QW-5 | **Screen thumbnail strip** | Screen Editor | Horizontal scrollable miniature preview of all project screens — ✅ Implemented |
| QW-6 | **Live filter in project tree** | Project Tree | Filter tree nodes to matching variables as you type — ✅ Implemented |
| QW-7 | **Export / Import variable list as Excel** | Variable Editor | Common engineer workflow for bulk tag configuration via spreadsheet — ✅ Implemented (CSV) |
| QW-8 | **Zoom / Pan on screen canvas** | Screen Editor | Ctrl+scroll or pinch to zoom; middle-mouse drag to pan the design canvas — ✅ Implemented |
| QW-9 | **Recent projects list** | Editor UX | Quick-open dropdown of last N project files on the home / start panel — ✅ Implemented |
| QW-10 | **Variable rename with propagation** | Project Tree | Rename a tag and auto-update all referencing scripts, alarms, and screens in one pass — ✅ Implemented |
| QW-11 | **Column chooser for grids** | Alarm / Variable Editors | Show / hide columns in variable grids to reduce visual noise — ✅ Implemented |
| QW-12 | **Multi-select delete in variable editor** | Variable Editor | Checkbox-select multiple rows and delete or move them in a single action — ✅ Implemented |
| QW-13 | **Script execution time badge** | Script Editor | Display last run duration and cycle count in script toolbar — ✅ Implemented |
| QW-14 | **Tag description tooltip** | Project Tree | Show full description, engineering unit, range, and driver address on hover over a tree node — ✅ Implemented |
| QW-15 | **Connection status badge in nav bar** | Runtime Integration | OPC UA server online / offline indicator in the top navigation bar — ✅ Implemented |
| QW-16 | **Auto-save / crash recovery** | Project Management | 60 s recovery snapshot; restore prompt on next open after unclean exit — ✅ Implemented |
| QW-17 | **Search in alarm history** | Alarm Management | Full-text search + window selector in `AlarmAnalyticsDashboardPanel` — ✅ Implemented |
| QW-18 | **Collapse / Expand all in project tree** | Project Tree | Single toolbar button to collapse or expand the entire tree in one click — ✅ Implemented |
| QW-19 | **Copy screen as PNG** | Screen Editor | Export the current screen canvas to clipboard or a file as a PNG image with one click — ✅ Implemented |
| QW-20 | **Undo history panel** | Editor UX | Caret dropdown on the Undo button showing up to 20 recent actions; click any row to undo to that point — ✅ Implemented |
| QW-21 | **Variable unit quick-convert** | Variable Editor | Inline badges showing converted values (°C ↔ °F, bar ↔ psi, m³/h ↔ GPM, etc.) — ✅ Implemented |
| QW-22 | **Alarm shelving from alarm list** | Alarm Management | Right-click an active alarm to shelve it for N minutes (available in `AlarmListWidget` in RuntimeViewer) — ✅ Implemented in RuntimeViewer |
| QW-23 | **Driver polling interval override** | Driver Config | Per-variable `PollIntervalMs` override in the variable properties panel — ✅ Implemented |
| QW-24 | **Duplicate screen / script** | Screen / Script Editors | Right-click to clone a screen, script, or PLC program as a starting point for a new one — ✅ Implemented |
| QW-25 | **Variable bulk enable / disable logging** | Variable Editor | Multi-select rows and toggle historian logging on or off in a single action — ✅ Implemented |
| QW-26 | **Keyboard shortcut cheatsheet overlay** | Editor UX | Press `?` or Help → Keyboard Shortcuts to reveal a full hotkey reference card — ✅ Implemented |
| QW-27 | **Inline alarm limit editing** | Variable Editor | HH/High/Low/LL quick-edit inputs at the top of the variable properties panel — ✅ Implemented |
| QW-28 | **Script output / log viewer** | Script Editor | Collapsible console pane at the bottom of the script editor showing `Log()` output and errors — ✅ Implemented |
| QW-29 | **Driver connection test button** | Driver Config | One-click "Test Connection" in the driver editor that returns a pass/fail with round-trip time — ✅ Implemented |
| QW-30 | **Dark / light theme toggle** | Editor UX | ☀️/🌙 button in the status bar; also accessible from Settings → Theme menu — ✅ Implemented |

---

## Priority Matrix

| # | Feature | Impact | Effort | Priority |
|---|---|---|---|---|
| 1 | SFC Editor | ★★★★★ | High | 🔴 Strategic |
| 2 | Variable Dependency Graph | ★★★★★ | Medium | 🔴 Strategic |
| 10 | Offline Simulation Mode | ★★★★★ | Low | 🟢 Quick Win |
| 3 | Screen Navigation Map | ★★★★ | Low | 🟢 Quick Win |
| 4 | Live Tag Browser with Inline Values | ★★★★ | Medium | 🟡 High Value |
| 8 | Integrated REST API Tester | ★★★★ | Low | 🟢 Quick Win |
| 9 | Project Documentation Generator | ★★★★ | Medium | 🟡 High Value |
| 5 | No-Code Rule Builder | ★★★★ | High | 🔵 Long Term |
| 6 | Script Unit Test Framework | ★★★ | Medium | 🟡 High Value |
| 7 | Protocol Traffic Monitor | ★★★ | Medium | 🟡 High Value |
| 12 | Alarm Rationalization (ISA-18.2) | ★★★ | High | 🔵 Long Term |
| 11 | Keyboard Shortcut Customizer | ★★★ | Low | 🟢 Quick Win |
| QW-10 | Variable rename with propagation | ★★★★★ | Low | 🟢 Quick Win |
| QW-16 | Auto-save / crash recovery | ★★★★ | Low | 🟢 Quick Win |
| QW-15 | Connection status badge in nav bar | ★★★★ | Low | 🟢 Quick Win |
| QW-29 | Driver connection test button | ★★★★ | Low | 🟢 Quick Win |
| QW-8 | Zoom / Pan on screen canvas | ★★★★ | Low | 🟢 Quick Win |
| QW-2 | Split editor panes | ★★★★ | Low | 🟢 Quick Win |
| QW-17 | Search in alarm history | ★★★ | Low | 🟢 Quick Win |
| QW-13 | Script execution time badge | ★★★ | Low | 🟢 Quick Win |
| QW-28 | Script output / log viewer | ★★★ | Low | 🟢 Quick Win |
| QW-21 | Variable unit quick-convert | ★★★ | Low | 🟢 Quick Win |
| QW-22 | Alarm shelving from alarm list | ★★★ | Low | 🟢 Quick Win |
| QW-23 | Driver polling interval override | ★★★ | Low | 🟢 Quick Win |
| QW-9 | Recent projects list | ★★★ | Low | 🟢 Quick Win |
| QW-30 | Dark / light theme toggle | ★★★ | Low | 🟢 Quick Win |
| QW-6 | Live filter in project tree | ★★★ | Low | 🟢 Quick Win |
| QW-7 | Export / Import variable list as Excel | ★★★ | Low | 🟢 Quick Win |
| QW-25 | Variable bulk enable / disable logging | ★★★ | Low | 🟢 Quick Win |
| QW-24 | Duplicate screen / script | ★★★ | Low | 🟢 Quick Win |
| QW-19 | Copy screen as PNG | ★★ | Low | 🟢 Quick Win |
| QW-11 | Column chooser for grids | ★★ | Low | 🟢 Quick Win |
| QW-14 | Tag description tooltip | ★★ | Low | 🟢 Quick Win |
| QW-3 | Pinned / favourite variables | ★★ | Low | 🟢 Quick Win |
| QW-20 | Undo history panel | ★★ | Low | 🟢 Quick Win |
| QW-26 | Keyboard shortcut cheatsheet overlay | ★★ | Low | 🟢 Quick Win |
| QW-27 | Inline alarm limit editing | ★★ | Low | 🟢 Quick Win |
| QW-5 | Screen thumbnail strip | ★★ | Low | 🟢 Quick Win |
| QW-4 | Diff view for scripts in Git panel | ★★ | Low | 🟢 Quick Win |
| QW-18 | Collapse / Expand all in project tree | ★★ | Low | 🟢 Quick Win |
| QW-12 | Multi-select delete in variable editor | ★★ | Low | 🟢 Quick Win |
| QW-1 | Tab history navigation | ★★ | Low | 🟢 Quick Win |
