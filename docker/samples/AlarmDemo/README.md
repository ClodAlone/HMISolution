# Alarm Demo

A simple project demonstrating alarm configuration, real-time monitoring, and event logging.

## Screens

| Screen | Description |
|---|---|
| **Alarms** | Active alarm list with sliders and edit boxes to manipulate three process variables (Temperature, Pressure, Level). Includes a real-time trend chart. |
| **AlarmHistory** | Full event log showing historical alarm and system events. |

## Variables

| Variable | Range | High Limit | Low Limit | Hysteresis |
|---|---|---|---|---|
| Temperature | 0–100 °C | 80 | 20 | 2.0 |
| Pressure | 0–10 bar | 8 | 2 | 0.3 |
| Level | 0–100 % | 90 | 10 | 2.0 |

## How to Use

1. Open the project in the HMI editor.
2. Start the runtime — the **Alarms** screen loads by default.
3. Drag a slider or type a value in an edit box to change a process variable.
4. When a variable crosses its high or low limit, an alarm appears in the alarm list.
5. Navigate to **Alarm History** to review all past alarm events.

## Features Demonstrated

- Limit-based alarm configuration (High, HighHigh, Low, LowLow)
- Alarm hysteresis
- Alarm list widget
- Event log widget with category filtering
- Horizontal sliders and edit boxes bound to variables
- Real-time trend chart with multiple pens
- Screen navigation via buttons
- Data logging and statistics
