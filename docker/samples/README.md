# HMI Solution â€” Sample Projects

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
| **WaterTreatment** | Water / Utilities | Modbus | Intake, coagulation, filtration, disinfection (UV + Clâ‚‚), distribution pumps, quality |
| **FoodAndBeverage** | Food & Beverage (Brewery) | S7 (Siemens) | Mashing, boiling, fermentation tanks, packaging line, CIP, batch control, recipes |
| **SmartWarehouse** | Logistics / Warehousing | S7, OPC UA Client, MQTT, REST | Conveyors, sorter, ASRS cranes, WMS integration, barcode tracking, IP camera |
| **PLCScriptLab** | Education / Lab | Simulation | PLC programs (ST, IL), C# scripts, interactive script console, JS commands, logic gates, PID, state machine, retentive PID tuning, variable statistics |
| **AiImportDemo** | Cross-Domain | Simulation | AI-imported variables, cross-domain monitoring, PLC control, real PLC export files (TIA Portal XML, Modbus CSV, KNX ETS CSV) for AI-assisted driver import |
| **ReportDemo** | Water / Utilities | Simulation | Report generation (HTML), historical charts, statistics tables, real-time values, scheduled/on-demand reports, email + disk delivery |
| **SchedulerDemo** | Facilities / BMS | Simulation | Weekly schedulers, HVAC/lighting/equipment time-based control, 15-min granularity, weekend/holiday modes, runtime editing |
| **WidgetShowcase** | Demo / Reference | Simulation | Comprehensive widget gallery: gauges, trends, sliders, switches, buttons, knobs, edit-boxes, alarm/event lists |
| **AlarmDemo** | Demo / Reference | Simulation | Alarm configuration, limit alarms (High/Low), active alarm list, event log history, sliders & edit boxes, real-time trend |

---

## Features Demonstrated

### Variables & Drivers
- **Modbus TCP** â€” Registers, coils, discrete inputs (Building, Water, Energy/Grid)
- **Siemens S7** â€” Data blocks with various types (Automotive, Brewery, Warehouse)
- **MQTT** â€” Broker subscribe with JSON path extraction (Energy/Solar, Energy/Wind, Warehouse/Environment)
- **REST API** â€” HTTP polling with JSON path (Energy/Weather, Warehouse/WMS)
- **OPC UA Client** â€” Gateway integration (Warehouse/ASRS cranes)
- **KNX** â€” Building bus for lighting and sensors (Building/Lighting)
- **Simulation** â€” All waveforms: Sine, Cosine, Triangle, Sawtooth, Square, Ramp, Random, Counter, Blink, Pulse (PLCScriptLab)

### Variable Properties
- **Initial Value** â€” Configurable starting value applied when the server starts (e.g., Speed=2.5, Setpoint=50.0)
- **Retentive** â€” Values persisted to `retentive.json` and automatically restored on server restart (setpoints, tuning parameters, counters)
- **Statistics** â€” When enabled, the server creates live OPC UA sub-variables: Min, Max, Average, Count, and a writable Reset flag under `.Statistics`

### AI Import
- **Reference File Selection** â€” select a sample project or custom nodes.json as context for the AI Assistant
- **Import Variables** â€” AI extracts variables, folders, screens, scripts, PLC programs from a reference project
- **Cross-Domain** â€” combine variables from multiple industry samples into a single unified project (see AiImportDemo)
- **Driver Import Files** â€” real PLC program export files in `AiImportDemo/imports/` for AI-assisted variable import:
  - `s7_bottling_line.xml` â€” Siemens TIA Portal DB export (SimaticML) â€” bottling line (filler, capper, labeler, conveyor, production)
  - `modbus_hvac_system.csv` â€” Modbus register map CSV â€” HVAC system (AHU, chiller, boiler, energy meter)
  - `knx_smart_office.csv` â€” KNX ETS group address export CSV â€” smart office (lighting, blinds, sensors, server room)
- **Example Prompts** â€” select a driver import file from the dropdown, then use a prompt like:
  - **S7** (select `s7_bottling_line.xml`):
    > Import all variables from this TIA Portal export. Create a folder for each DB block (Filler, Capper, Labeler, Conveyor, Production) and an IO folder for the tag table. Configure S7 driver for each variable with IP 192.168.0.10, Rack 0, Slot 1. Add a screen with gauges for the analog values and an alarm for each FaultCode != 0.
  - **Modbus** (select `modbus_hvac_system.csv`):
    > Import all registers from this Modbus map. Group variables by slave (AHU1, Chiller, Boiler, EnergyMeter). Set up Modbus driver configs using the IP and port from the file header. Add limit alarms on temperatures and a screen with trends for energy data.
  - **KNX** (select `knx_smart_office.csv`):
    > Import all group addresses from this KNX ETS export. Organize variables by floor and room. Configure KNX driver with gateway 192.168.1.100 and the correct DPT for each address. Add a screen per floor showing lights, blinds and sensor values.

### Alarms
- **Limit alarms** with High-High, High, Low, Low-Low thresholds + hysteresis
- **Condition alarms** with operators: ==, !=, >, True, False, Changed

### Data Logging
- Historical data archiving with hysteresis deadband and MaxAge settings
- HDA chart and HDA grid widgets for historical analysis

### Screens
- **Gauges** â€” needle, arc, semicircle, thermometer, hbar, vbar styles
- **Realtime Trends** â€” multi-pen live charts
- **Alarm Lists** â€” active alarm display
- **Event Logs** â€” with category filtering
- **Edit Boxes** â€” operator setpoint entry with spin buttons, optional statistics bar (Min/Avg/Max/Count with reset)
- **Navigation** â€” multi-screen with NavigateScreen commands
- **Dynamic Bindings** â€” FillBinding, VisibilityBinding, LabelBinding for live status colors
- **HDA Charts** â€” historical data visualization
- **Screen Embed** â€” tabbed embedded screens for composite layouts (PLCScriptLab)

### Interactive Script Console (PLCScriptLab)
- **ExecuteScript** â€” run C# code on button click at runtime (read/write variables, compute Fibonacci, bulk operations, reset all)
- **ExecuteJavaScript** â€” run JS in the browser (alerts, prompts, DOM manipulation, fullscreen toggle)
- **Scratchpad variables** â€” editable Double/Boolean/String scratch registers for ad-hoc testing

### Scripts
- C# scripts for calculations (OEE, energy balance, efficiency)
- Monitoring scripts for quality and maintenance alerts
- **Math calculator** â€” real-time Sqrt, Sin, Cos, Log10, running average (PLCScriptLab)
- **Signal data processor** â€” exponential filter, min/max tracking, rate of change (PLCScriptLab)
- **Event watcher** â€” OnChanged subscription monitoring (PLCScriptLab)

### PLC Programs
- IEC 61131-3 Structured Text (ST) programs
- Temperature PID control, conveyor speed regulation, pump standby logic
- **PID controller** â€” full P-I-D loop with anti-windup, manual/auto, simulated process (PLCScriptLab)
- **State machine** â€” 5-state sequential control: Idleâ†’Startingâ†’Runningâ†’Stoppingâ†’Fault (PLCScriptLab)
- **Up/Down counter** â€” count with preset, done flag (PLCScriptLab)
- **Logic gates** â€” AND, OR, XOR, NOT, NAND, SR latch (PLCScriptLab)
- **Instruction List (IL)** â€” blink generator in IL language (PLCScriptLab)

### Reports
- **Report Definitions** -- Multi-section reports with Header, Text, Chart, Table, Value, PageBreak sections (ReportDemo)
- **Historical Charts** -- Line charts from data-logged variables over configurable time ranges
- **Statistics Tables** -- Automatic min/max/avg calculations from historical data
- **Real-Time Values** -- Snapshot display with label, format, and engineering unit
- **Scheduled Reports** -- Trigger generation via schedulers at fixed daily/shift times
- **Delivery Methods** -- Disk output, SMTP email, or both

### Schedulers
- **Weekly Planners** -- Time-based scheduling with 15/60-minute granularity (SchedulerDemo)
- **Weekend/Holiday Modes** -- Off, Same, Custom weekend handling; holiday locale support
- **Activate/Deactivate Actions** -- Set variables when entering/leaving time slots
- **Runtime Editing** -- Operators can modify schedules from the planner widget

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
- Password hashes are empty â€” set proper passwords via the editor before production use.
- Database connection strings point to `localhost` â€” update for your PostgreSQL/TimescaleDB instance.
- The Simulation driver can be added to any variable via the editor for testing without real hardware.
- The **PLCScriptLab** sample requires no external hardware â€” it runs entirely on the Simulation driver
  and is ideal for learning PLC programming, script development, and screen design.


