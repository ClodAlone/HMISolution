# PID Controller & Auto-Tune Demo

A complete sample project demonstrating PID control with all three IEC 61131-3 PLC languages and the built-in PID auto-tuning capabilities.

## What's Included

### 3 PLC Programs (one per language)

| Program | Language | Default State | Description |
|---------|----------|---------------|-------------|
| **PID_StructuredText** | ST | ✅ Enabled | Full PID + process simulation using `PID()` function, `Read()`/`Write()`, conditionals |
| **PID_InstructionList** | IL | ❌ Disabled | Same logic in Instruction List: `LD`/`ST`/`CAL` instructions with labels & jumps |
| **PID_LadderDiagram** | LD | ❌ Disabled | PID function block in Ladder Diagram with `CONTACT NO` enable gate |

> **Important:** Only one program should be enabled at a time — they all control the same process variables.

### 3 Runtime Screens

| Screen | Purpose |
|--------|---------|
| **PID Dashboard** | Main operator screen — realtime trend, gauges, Kp/Ki/Kd knobs, setpoint slider, process simulation controls |
| **Auto-Tune** | Relay feedback auto-tune monitor — configure experiment, watch oscillation, read computed gains |
| **PLC Programs** | Information screen describing each PLC program's features |

### Simulated Process

A first-order thermal system:
```
dT/dt = (-T + K × Output - Disturbance × τ) / τ
```

Configurable parameters:
- **Process Gain** (K) — how strongly the heater affects temperature
- **Time Constant** (τ) — how fast the process responds
- **Cooling Disturbance** — constant heat loss (simulates ambient cooling)
- **Measurement Noise** — random noise on the temperature reading

## Quick Start

1. **Open** the project in the Editor (File → Open → select `nodes.json`)
2. **Deploy** to the server (or run locally)
3. **Open** the Runtime Viewer → navigate to **PID Dashboard**
4. Watch the PID controller stabilize the temperature to the setpoint
5. **Experiment:**
   - Drag the **Setpoint** slider to change the target
   - Turn the **Kp/Ki/Kd knobs** to tune the controller live
   - Increase **Cooling Disturbance** to test disturbance rejection
   - Toggle **PID Enable** off to see the process drift

## Auto-Tuning

1. Navigate to the **Auto-Tune** screen
2. Set relay amplitude (default 15) and hysteresis (default 0.5)
3. Toggle **Start Auto-Tune** ON
4. Watch the relay feedback oscillation on the trend chart
5. When complete, read the **Computed Kp/Ki/Kd** values
6. Copy those values to the PID gains on the Dashboard

### Using the Editor Auto-Tune Wizard

1. Open the project in the Editor
2. Select the **PID_StructuredText** PLC program node
3. In the Properties panel, expand **🎛️ PID Auto-Tune Wizard**
4. Configure variable paths (pre-filled in this sample)
5. Enter observed Ku/Pu/Amplitude → click **Calculate**
6. Click **Generate ST Code & Apply** to replace the program code

## Switching PLC Languages

To try the IL or LD version:

1. Open the project in the Editor
2. Select **PID_StructuredText** → uncheck **Enabled**
3. Select **PID_InstructionList** or **PID_LadderDiagram** → check **Enabled**
4. Save and redeploy

## OPC Variable Map

| Variable | Type | Description |
|----------|------|-------------|
| `Process.Temperature` | Double | Simulated process variable (°C) |
| `Process.Setpoint` | Double | Target temperature (°C) |
| `Process.HeaterOutput` | Double | Controller output (0–100%) |
| `Process.CoolingDisturbance` | Double | Ambient heat loss rate |
| `PID.Kp` | Double | Proportional gain |
| `PID.Ki` | Double | Integral gain |
| `PID.Kd` | Double | Derivative gain |
| `PID.Enabled` | Boolean | Enable/disable PID control |
| `Simulation.ProcessGain` | Double | Plant gain factor |
| `Simulation.TimeConstant` | Double | Plant time constant (s) |
| `Simulation.NoiseAmplitude` | Double | Measurement noise level |
| `AutoTune.Running` | Boolean | Start/stop auto-tune experiment |
| `AutoTune.RelayAmplitude` | Double | Relay experiment amplitude |
| `AutoTune.ComputedKp/Ki/Kd` | Double | Auto-tune results |
