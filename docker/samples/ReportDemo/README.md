# Report Demo — Water Treatment Plant

This sample demonstrates the **Report** feature using a water treatment plant
scenario. Three reports cover daily operations, quality compliance, and shift
handover — showcasing every report section type (Header, Text, Chart, Table,
Value, PageBreak) with both scheduled and on-demand generation.

## Reports

| Name | Format | Orientation | Delivery | Description |
|---|---|---|---|---|
| **DailyOperations** | HTML | Landscape | Email + Disk | 24-hour summary: flow/quality/tank charts, process stats table, energy consumption |
| **QualityCompliance** | HTML | Portrait | Disk | 8-hour pH, chlorine, dissolved oxygen, turbidity charts with compliance stats |
| **ShiftHandover** | HTML | Portrait | Disk | Snapshot of all key process values, 4-hour trend chart, and 8-hour stats table |
| **DailyAiSummary** | HTML | Portrait | Email + Disk | AI-generated plain-language daily overview (trends, alarms, compliance flags) |

### Report Section Types Demonstrated

- **Header** — Report title rendered as a page header
- **Text** — Rich-text introduction / notes (HTML supported)
- **Chart** — Historical data line charts with configurable time range and max points
- **Table** — Variable statistics tables with min/max/avg (historical)
- **Value** — Single real-time value display with label, format, and unit
- **PageBreak** — Explicit page break for multi-page reports
- **AiSummary** — AI-generated natural-language summary of variable trends and events (see below)

### Delivery Options

- **Disk** — Save generated reports to configurable output folders
- **Email** — SMTP delivery with configurable subject, recipients, and TLS
- **Both** — DailyOperations uses combined email + disk delivery

## Schedulers

| Name | Purpose |
|---|---|
| **DailyReportSchedule** | Triggers the DailyOperations report once per day at 06:00 |
| **ShiftReportSchedule** | Triggers the ShiftHandover report three times daily (06:00, 14:00, 22:00) |
| **DailyAiSummarySchedule** | Triggers the DailyAiSummary report once per day at 06:00 |

## Screens

| Screen | Description |
|---|---|
| **Dashboard** | Overview with intake/treatment gauges, tank levels, power, live trends, and alarm list |
| **Reports** | Report viewer panel — select, generate, and preview reports |
| **History** | HDA charts and grid for flow rates, quality, and energy history |
| **Control** | Operator controls: sliders, switches, edit-boxes for dosing, pumps, and valves |
| **Events** | Event log viewer |

## Variables

Plant/Intake (FlowRate, Pressure, Temperature, Turbidity, Pumps, Power)
Plant/Treatment (pH, Chlorine, DissolvedOxygen, ChemicalDosing, FilterPressure)
Plant/Storage (Tank1/Tank2 Levels and Temperatures, OutletValve)
Plant/Distribution (OutflowRate, Pressure, BoosterPump)
Plant/Energy (TotalPower, DailyEnergy, DailyWaterVolume)

All process variables have **DataLogging enabled** for historical report charts and statistics.

## Scripts

- **SimulateProcess** — Generates realistic process values using waveforms with noise
- **SimulateEnergy** — Calculates total power and accumulates daily energy/volume

## Key Features Demonstrated

1. **Multi-Section Reports** — Headers, text, charts, tables, and values in one report
2. **Historical Charts** — Line charts from logged data (1h, 4h, 8h, 24h ranges)
3. **Statistics Tables** — Automatic min/max/avg from historical data
4. **Real-Time Values** — Snapshot of current values with engineering units
5. **Scheduled Generation** — Schedulers trigger reports at fixed times
6. **Multiple Delivery Methods** — Disk, email, or both
7. **On-Demand Generation** — Reports can be triggered from the Reports screen
8. **AI Daily Summary** — `DailyAiSummary` uses an `AiSummary` section to turn 24h of variable
   statistics and alarm/event log entries into a plain-language operator summary, powered by
   the same `Settings.NaturalLanguageQuery` engine (OpenAI/Gemini/Claude/Ollama) used for
   natural-language queries elsewhere in the platform.

## AI Summary Configuration

This sample enables `Settings.NaturalLanguageQuery` with `Engine: "Ollama"` (free, local, no API key
required) so the `DailyAiSummary` report works out of the box if you have
[Ollama](https://ollama.com) running locally with the `mistral` model pulled:

```bash
ollama pull mistral
ollama serve
```

To use a cloud engine instead (Claude, OpenAI, or Gemini), edit `Server.NaturalLanguageQuery` in
`nodes.json`:

```json
"NaturalLanguageQuery": {
  "Enabled": true,
  "Engine": "Claude",
  "Model": "claude-3-5-sonnet-20241022",
  "ApiKey": "sk-ant-api03-..."
}
```

The `DailyAiSummary` report's `AiSummary` section (`Sections[1]` in `nodes.json`) pulls 24h of
statistics for the key process variables plus any `Alarm`/`System` events, and asks the AI to focus
on water-quality compliance and tank-level flags via `AiSummaryInstructions`.

## How to Use

1. Load this sample from the editor (File then Open Sample then ReportDemo).
2. Start the server — simulation scripts generate realistic data.
3. Wait a few minutes for historical data to accumulate.
4. Open the **Reports** screen to select and generate a report.
5. The **DailyOperations** report is scheduled at 06:00 daily.
6. The **ShiftHandover** report runs at each shift change (06:00, 14:00, 22:00).
7. Check **History** to view raw historical data used by reports.
