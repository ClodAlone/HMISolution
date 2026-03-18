# HMI Solution — Sample Projects

These sample `nodes.json` projects demonstrate the HMI/SCADA solution across
different industrial scenarios. Each folder contains a ready-to-run configuration
that showcases specific features and driver integrations.

---

## Samples Overview

| Sample | Industry | Drivers Used | Key Features |
|---|---|---|---|
| **BuildingAutomation** | Facilities / BMS | Modbus, KNX | HVAC, lighting, energy metering, fire/security alarms, KNX bus |
| **AutomotiveAssembly** | Automotive Manufacturing | S7 (Siemens) | Welding robots, paint shop, torque stations, OEE, PLC programs, recipes |
| **EnergyManagement** | Renewable Energy | MQTT, REST, Modbus | Solar arrays, wind turbines, battery storage, weather API, grid monitoring |
| **WaterTreatment** | Water / Utilities | Modbus | Intake, coagulation, filtration, disinfection (UV + Cl₂), distribution pumps, quality |
| **FoodAndBeverage** | Food & Beverage (Brewery) | S7 (Siemens) | Mashing, boiling, fermentation tanks, packaging line, CIP, batch control, recipes |
| **SmartWarehouse** | Logistics / Warehousing | S7, OPC UA Client, MQTT, REST | Conveyors, sorter, ASRS cranes, WMS integration, barcode tracking, IP camera |
| **PLCScriptLab** | Education / Lab | Simulation | PLC programs (ST, IL), C# scripts, interactive script console, JS commands, logic gates, PID, state machine, retentive PID tuning, variable statistics |
| **AiImportDemo** | Cross-Domain | Simulation | AI-imported variables, cross-domain monitoring, PLC control, S7/Modbus/KNX import files for AI reference |

---

## Features Demonstrated

### Variables & Drivers
- **Modbus TCP** — Registers, coils, discrete inputs (Building, Water, Energy/Grid)
- **Siemens S7** — Data blocks with various types (Automotive, Brewery, Warehouse)
- **MQTT** — Broker subscribe with JSON path extraction (Energy/Solar, Energy/Wind, Warehouse/Environment)
- **REST API** — HTTP polling with JSON path (Energy/Weather, Warehouse/WMS)
- **OPC UA Client** — Gateway integration (Warehouse/ASRS cranes)
- **KNX** — Building bus for lighting and sensors (Building/Lighting)
- **Simulation** — All waveforms: Sine, Cosine, Triangle, Sawtooth, Square, Ramp, Random, Counter, Blink, Pulse (PLCScriptLab)

### Variable Properties
- **Initial Value** — Configurable starting value applied when the server starts (e.g., Speed=2.5, Setpoint=50.0)
- **Retentive** — Values persisted to `retentive.json` and automatically restored on server restart (setpoints, tuning parameters, counters)
- **Statistics** — When enabled, the server creates live OPC UA sub-variables: Min, Max, Average, Count, and a writable Reset flag under `.Statistics`

### AI Import
- **Reference File Selection** — select a sample project or custom nodes.json as context for the AI Assistant
- **Import Variables** — AI extracts variables, folders, screens, scripts, PLC programs from a reference project
- **Cross-Domain** — combine variables from multiple industry samples into a single unified project (see AiImportDemo)
- **Import Files** — ready-to-use driver templates in `AiImportDemo/imports/`:
  - `s7_bottling_line.json` — Siemens S7 bottling line (filler, capper, labeler, conveyor, production)
  - `modbus_hvac_system.json` — Modbus HVAC system (AHU, chiller, boiler, energy meter)
  - `knx_smart_office.json` — KNX smart office (lighting zones, blinds, sensors, server room)

### Alarms
- **Limit alarms** with High-High, High, Low, Low-Low thresholds + hysteresis
- **Condition alarms** with operators: ==, !=, >, True, False, Changed

### Data Logging
- Historical data archiving with hysteresis deadband and MaxAge settings
- HDA chart and HDA grid widgets for historical analysis

### Screens
- **Gauges** — needle, arc, semicircle, thermometer, hbar, vbar styles
- **Realtime Trends** — multi-pen live charts
- **Alarm Lists** — active alarm display
- **Event Logs** — with category filtering
- **Edit Boxes** — operator setpoint entry with spin buttons, optional statistics bar (Min/Avg/Max/Count with reset)
- **Navigation** — multi-screen with NavigateScreen commands
- **Dynamic Bindings** — FillBinding, VisibilityBinding, LabelBinding for live status colors
- **HDA Charts** — historical data visualization
- **Screen Embed** — tabbed embedded screens for composite layouts (PLCScriptLab)

### Interactive Script Console (PLCScriptLab)
- **ExecuteScript** — run C# code on button click at runtime (read/write variables, compute Fibonacci, bulk operations, reset all)
- **ExecuteJavaScript** — run JS in the browser (alerts, prompts, DOM manipulation, fullscreen toggle)
- **Scratchpad variables** — editable Double/Boolean/String scratch registers for ad-hoc testing

### Scripts
- C# scripts for calculations (OEE, energy balance, efficiency)
- Monitoring scripts for quality and maintenance alerts
- **Math calculator** — real-time Sqrt, Sin, Cos, Log10, running average (PLCScriptLab)
- **Signal data processor** — exponential filter, min/max tracking, rate of change (PLCScriptLab)
- **Event watcher** — OnChanged subscription monitoring (PLCScriptLab)

### PLC Programs
- IEC 61131-3 Structured Text (ST) programs
- Temperature PID control, conveyor speed regulation, pump standby logic
- **PID controller** — full P-I-D loop with anti-windup, manual/auto, simulated process (PLCScriptLab)
- **State machine** — 5-state sequential control: Idle→Starting→Running→Stopping→Fault (PLCScriptLab)
- **Up/Down counter** — count with preset, done flag (PLCScriptLab)
- **Logic gates** — AND, OR, XOR, NOT, NAND, SR latch (PLCScriptLab)
- **Instruction List (IL)** — blink generator in IL language (PLCScriptLab)

### Recipes
- Welding parameters (Automotive)
- Water treatment chemical dosing (Water)
- Brew recipes with fermentation setpoints (Brewery)

### Security
- User accounts with groups (Admins, Operators, Managers, Viewers)
- Access levels (Read, Write, ReadWrite)
- Editor and runtime login control
- Auto-logoff timers

### Internationalization
- Localized strings in English, German, Italian, French, Spanish

---

## How to Use

### With the Server (OPC UA)

```bash
cd C:\Work\samples\BuildingAutomation
dotnet run --project C:\path\to\Server\Server.csproj -- nodes.json
```

### With the Web Editor

1. Start the editor: `dotnet run --project ServerEditorWeb\ServerEditorWeb.csproj`
2. Open `http://localhost:5119`
3. Open a sample `nodes.json` from `C:\Work\samples\<SampleName>\`

### With the Runtime Viewer (Web)

```bash
dotnet run --project RuntimeViewer\RuntimeViewer.csproj -- C:\Work\samples\BuildingAutomation\nodes.json
```

### With the Runtime Viewer (Desktop)

```bash
RuntimeViewer.Desktop.exe "C:\Work\samples\BuildingAutomation\nodes.json"
# or kiosk mode:
RuntimeViewer.Desktop.exe "C:\Work\samples\BuildingAutomation\nodes.json" --kiosk
```

### With Docker

```bash
docker run -d --name hmi-server \
  -p 4840:4840 \
  -v C:\Work\samples\BuildingAutomation:/data \
  hmi-server /data/nodes.json
```

---

## Notes

- All IP addresses in driver configurations are fictional examples.
  Update them to match your actual hardware/network before running.
- Password hashes are empty — set proper passwords via the editor before production use.
- Database connection strings point to `localhost` — update for your PostgreSQL/TimescaleDB instance.
- The Simulation driver can be added to any variable via the editor for testing without real hardware.
- The **PLCScriptLab** sample requires no external hardware — it runs entirely on the Simulation driver
  and is ideal for learning PLC programming, script development, and screen design.
