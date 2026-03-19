# Report Demo — Water Treatment Plant

This sample demonstrates the **Report** feature in a water treatment plant
scenario. Three report definitions showcase different report types: daily
operations, quality compliance, and shift handover.

## Reports

| Name | Purpose | Sections | Format | Delivery | Scheduled |
|---|---|---|---|---|---|
| **DailyOperations** | 24h overview of flows, quality, tanks, energy | Header, Text, 3 Charts, PageBreak, Table, Bar Chart, 2 Values | HTML | Email + Disk | Daily at 23:00 |
| **QualityCompliance** | 8h water quality analysis with compliance checks | Header, Text, 4 Charts (pH, Cl, DO, Turbidity), Table, 2 Values | HTML | Disk | On-demand |
| **ShiftHandover** | Plant snapshot for shift changes | Header, Text, 10 Values, PageBreak, Chart, Table | HTML | Disk | Every 8h (06:00, 14:00, 22:00) |

## Report Section Types Demonstrated

- **Header** — Report title section
- **Text** — Free-text description / instructions
- **Chart** — Historical data chart (Line, Area, Bar types)
- **Table** — Variable table with Min/Max/Avg statistics
- **Value** — Single real-time variable display with label and unit
- **PageBreak** — Page separator for multi-page reports

## Screens

| Screen | Description |
|---|---|
| **Dashboard** | Overview with gauges (flow, pH, tanks, power, chlorine, pressure), live trend, alarm list, and navigation |
| **Reports** | Three GenerateReport buttons and three ReportViewer widgets showing each report |
| **History** | HDA charts for flow, quality, tanks, and power; plus an HDA data grid |
| **Control** | Toggle switches for pumps/dosing/valve, dosing rate slider + editbox, live pH and tank trends |
| **Events** | Event log viewer (72h) |

## Variables

```
Plant/
├── Intake/           FlowRate_m3h, Pressure_bar, Temperature_C,
│                     Turbidity_NTU, Pump1_Running, Pump2_Running,
│                     Pump1_Power_kW
├── Treatment/        pH, Chlorine_ppm, DissolvedOxygen_mgL,
│                     ChemicalDosing_Running, DosingRate_mlmin,
│                     FilterPressure_bar
├── Storage/          Tank1_Level_pct, Tank2_Level_pct,
│                     Tank1_Temperature_C, Tank2_Temperature_C,
│                     OutletValve_Open
├── Distribution/     OutflowRate_m3h, Pressure_bar,
│                     BoosterPump_Running, BoosterPump_Power_kW
└── Energy/           TotalPower_kW, DailyEnergy_kWh,
                      DailyWaterVolume_m3
```

## Schedulers

| Name | Purpose | Schedule |
|---|---|---|
| **DailyReportSchedule** | Auto-generates the DailyOperations report | Every day at 23:00 |
| **ShiftReportSchedule** | Auto-generates the ShiftHandover report | Every day at 06:00, 14:00, 22:00 |

## Scripts

- **SimulateProcess** — Simulates intake flow (based on pump states), water
  quality (pH/chlorine drift toward dosing targets), tank levels, and
  distribution pressure (5 s interval).
- **SimulateEnergy** — Calculates total power from running equipment and
  accumulates daily energy and water volume (10 s interval).

## Key Features Demonstrated

1. **Report Definitions** — Three report configs with different section
   combinations (charts, tables, values, headers, text, page breaks).
2. **Chart Types** — Line, Area, and Bar charts with historical data queries.
3. **Statistics Tables** — Tables with Min/Max/Avg columns from data logging.
4. **Real-Time Values** — Current OPC variable values with format and units.
5. **Report Delivery** — Email + disk delivery (DailyOperations) and
   disk-only delivery (Quality, Shift).
6. **Scheduled Generation** — Schedulers trigger GenerateReport commands
   at configured times.
7. **On-Demand Generation** — Buttons on the Reports screen trigger
   GenerateReport for each report.
8. **Report Viewer Widgets** — `reportviewer` widgets embedded in the
   Reports screen to display generated reports inline.
9. **Data Logging** — All key variables have data logging enabled with
   hysteresis and max interval for chart data.
10. **Alarms** — pH, chlorine, and tank level alarms with limit triggers.

## How to Use

1. Load this sample from the editor (File → Open Sample → ReportDemo).
2. Start the server — scripts will begin simulating process data.
3. Open the **Reports** screen and click a Generate button to create a report.
4. Use the **Report Viewer** widgets to see the generated HTML output.
5. Adjust dosing rate or toggle pumps on the **Control** screen to see how
   process values change in reports.
6. Check the **History** screen for historical charts that feed report data.
