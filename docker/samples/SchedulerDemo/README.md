# Scheduler Demo — Building Automation

This sample demonstrates the **Scheduler** feature in a building automation
scenario. Five schedulers manage HVAC zones, lighting, and equipment on
recurring weekly timetables.

## Schedulers

| Name | Purpose | Schedule | Weekend | Holiday | Runtime Edit |
|---|---|---|---|---|---|
| **HVAC_Office_Schedule** | Office HVAC on Mon–Fri 07:00–18:00 | 60 min slots | Off | Off (US holidays) | ✅ Yes |
| **HVAC_Workshop_Schedule** | Workshop HVAC Mon–Fri 06:00–20:00, Sat morning | 60 min slots | Custom (Sat 08–12) | Use weekend sched | ✅ Yes |
| **Lighting_Office_Schedule** | Office lights Mon–Fri 06:45–18:15 | **15 min** slots | Off | Off | ✅ Yes |
| **Equipment_DayShift** | Compressor + water pump Mon–Fri 06:00–22:00 | 60 min slots | Off | Off (US holidays) | ❌ Admin only |
| **NightSecurity_Lights** | Parking & security lights every night | 60 min slots | Same as weekday | Same | ❌ Admin only |

## Screens

| Screen | Description |
|---|---|
| **Dashboard** | Overview with zone temperature gauges, equipment indicators, live trend, and alarm list |
| **Schedules** | Four Weekly Planner widgets showing each scheduler's timetable (runtime-editable where allowed) |
| **ManualControl** | Toggle switches, sliders, knobs, and edit-boxes for manual HVAC/lighting/equipment override |
| **History** | HDA charts and grid for temperature and power consumption history |
| **Events** | Event log viewer |

## Variables

```
Plant/
├── HVAC/
│   ├── Zone1_Office/     Running, Setpoint, Temperature, FanSpeed, Mode
│   ├── Zone2_Workshop/   Running, Setpoint, Temperature, FanSpeed
│   └── Zone3_ServerRoom/ Running, Setpoint, Temperature
├── Lighting/             Office_Lights, Office_Brightness, Workshop_Lights,
│                         Workshop_Brightness, Parking_Lights, SecurityLights
├── Equipment/            Compressor_Enable, Compressor_Power_kW,
│                         WaterPump_Enable, EmergencyMode
└── Energy/               TotalPower_kW, DailyEnergy_kWh
```

## Scripts

- **SimulateTemperatures** — Drifts zone temperatures toward setpoint when HVAC
  is running, toward ambient when off (5 s interval).
- **SimulateEnergy** — Calculates total power draw from all running equipment
  and accumulates daily energy (10 s interval).

## Key Features Demonstrated

1. **Activate / Deactivate Commands** — Each scheduler sets variables when
   entering a time slot (e.g. turn on HVAC, set fan speed) and resets them when
   leaving (turn off, fan to 0).
2. **Weekend Modes** — `Off` (office), `Custom` (workshop Saturday morning),
   `Same` (night security runs every day).
3. **Holiday Modes** — `Off` (no HVAC on US holidays), `Weekend` (workshop
   follows weekend schedule on holidays).
4. **Custom Holidays** — "Company Founders Day" (March 15) added to the office
   HVAC scheduler.
5. **15-Minute Granularity** — Lighting scheduler uses 15-minute slots for
   precise on/off timing (06:45 / 18:15).
6. **Runtime Editing** — HVAC and lighting planners allow operators to modify
   schedules at runtime; equipment schedule is admin-only.
7. **Night Schedule** — Security lights use inverted slots (active during night
   hours on every day including weekends).

## How to Use

1. Load this sample from the editor (File → Open Sample → SchedulerDemo).
2. Start the server — schedulers will immediately begin evaluating.
3. Open the **Schedules** screen to view/edit the weekly planners.
4. Use **Manual Control** to override individual zones.
5. Watch the **Dashboard** to see how temperature and power respond when
   schedulers activate and deactivate equipment.
