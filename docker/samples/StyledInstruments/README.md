# Styled Instruments - nodes.json sample

Open `C:\Work\samples\StyledInstruments\nodes.json` in ServerEditorWeb.
This is a normal HMI project, not a C# application and does not need a generator.
It requires the updated editor and runtime that include display, gauge, pipe, tank and pump appearance options.

## Screens

- **Styled Dashboard** (startup): Midnight, Ivory and Brushed Metal gauges, four display backgrounds, editable setpoints and a text field.
- **Responsive Lab**: displays and editable values that adapt to their actual grid cell sizes and current value lengths.
- **Gauge Gallery**: Needle, Arc, Semicircle, Horizontal Bar, Vertical Bar and Thermometer in three coordinated appearance presets.
- **Tank Transfer**: two cylindrical isometric tanks with conical bases and gradient liquid, connected through shaded pipes and an isometric animated pump. The liquid surface narrows inside each cone. Start/stop and refill/restart controls drive a server-side simulation. Readouts represent level percentage, not calculated vessel volume.
- **Solar Farm**: original shaded vector PV arrays, dual hybrid inverters, an isometric house, utility pylon and battery/BMS rack. Animated colored pipes show solar production, house demand, grid import/export and battery charge/discharge. A separate server-side simulation balances power and models battery state of charge.
- **HVAC Plant**: reversible heat pump with animated fan, shaded thermal buffer, isometric circulation pump and a building cutaway/coil. Red/cyan supply and blue return pipes show heating/cooling circulation. Thermostat, manual pump-speed control, weather disturbances and pump interlock drive a room/buffer energy-balance simulation.
- **EV Charging Station**: isometric vehicles and chargers with live battery levels and animated energy branches. Arrivals and departures share a bounded site-power supply.
- **Water Treatment Plant**: intake, treatment/filter and clean-water tanks, pumps, valves and colored flows. Filter loading triggers a backwash cycle using clean water.
- **Conveyor Production Line**: moving packages, drive motor, detection sensor and sorting diverter. Good/reject counting and jam recovery follow tracked package state.
- **Microgrid Control Center**: PV, storage, house, EV and utility network. Tariff-aware dispatch, import-cap peak shaving and island operation expose power balance and unmet demand.
- **Compressed-Air Station**: lead/standby compressors, shaded pressure receiver, factory network and leak branch. Independent hysteresis stages capacity against normal/peak demand.
- **Quick Controls**: bottom auto-hide drawer with live temperature, pressure and flow, an editable shared setpoint, and buttons to all eleven pages.

## Screen transitions and bottom auto-hide panel

- Styled Dashboard: **fade**, 320 ms.
- Responsive Lab: **slide-left**, 350 ms.
- Gauge Gallery: **slide-up**, 400 ms.
- Tank Transfer, Solar Farm, HVAC Plant and the five industrial/energy examples: **fade**, 320 ms.
- Use the hamburger navigation or the drawer buttons to see the destination page's transition. Horizontal swipes cycle through Dashboard, Responsive Lab, Gauge Gallery, Tank Transfer, Solar Farm, HVAC Plant, EV Charging Station, Water Treatment Plant, Conveyor Production Line, Microgrid Control Center and Compressed-Air Station; swipe transitions follow the gesture direction.
- **Quick Controls** uses `RuntimeDock: bottom`, `DockMode: swipe`, `DockSize: 340`. It starts hidden and overlays the current page without reserving space. The larger drawer accommodates all eleven page buttons.
- Open it by tapping/dragging the **Quick Controls** bottom handle, or swiping upward from the bottom edge. Close it by swiping down, tapping the backdrop or close button, or pressing Escape when not editing a field.
- The drawer is available on every page and is excluded from page navigation. Up/down page swipe destinations are left empty so they do not compete with the bottom drawer gesture.
- Reduced-motion OS/browser preferences disable page animations.

Temperature, pressure and flow use the built-in Simulation driver.
Setpoint, long signed value and batch-name fields are writable local variables. The setpoint is shared by all four styled fields, so a write in one updates the others.

## Run live

From `C:\Users\cfior\source\repos`, in two terminals:

```powershell
dotnet run --project Server\Server.csproj -- C:\Work\samples\StyledInstruments\nodes.json
```

```powershell
dotnet run --project RuntimeViewer\RuntimeViewer.csproj -- C:\Work\samples\StyledInstruments\nodes.json
```

Open the viewer URL printed by the second command.
The sample OPC endpoint is `opc.tcp://localhost:14930/StyledInstruments`; diagnostics use port 14931.
Login is disabled for this local demonstration. Do not expose the sample server to an untrusted network.

## Tank transfer simulation

1. Run the server and viewer above, then open **Tank Transfer** from navigation or Quick Controls. The editor alone previews equipment but does not execute this simulation.
2. Initially Tank 1 is 100%, Tank 2 is empty, the pump is stopped and the pipes have no flow.
3. Press **Start / Stop** to start. The `TankTransferSimulation` C# script runs every 250 ms, moving 1 percentage point per cycle (approximately 25 seconds for a full transfer). Source and receiver level percentages sum to 100%; this is a level-based demonstration, not a volume-conserving flow model.
4. Pump rotation and both pipe flows use the actual `Transfer.PumpOn` state. Press **Start / Stop** again to pause without losing the levels; press it again to resume.
5. When Tank 1 empties or Tank 2 fills, the script stops automatically and displays completion status. Starting an already completed transfer does not run the pump dry.
6. Press **Refill & Restart** at any time: on the next script cycle, Tank 1 returns to 100%, Tank 2 to 0%, and transfer restarts on the following cycle.
7. Non-finite/out-of-range or inconsistent levels stop the simulation with a status error; Refill & Restart recovers it. The script owns the level, pump and status variables; only the request tags are intended as controls.

The simulation continues on the server when you navigate away and is shared by connected viewers. Values reset to their initial configuration on server restart.
Tank levels now animate smoothly between updates, including refill/restart. The liquid surface and percentage readout transition together over 400 ms; frequent updates continue from the current visible level. This is visual only and does not change the simulated values. Reduced-motion preferences show new levels immediately.

The saved sample is `nodes.json`. The pre-existing `nodes.json.recovery` was left untouched; reopen the saved sample rather than restoring that older recovery file.

## Solar farm simulation

1. Open **Solar Farm** from navigation or Quick Controls while the server and viewer are running. SolarFarmSimulation is enabled in this sample, but starts paused; the editor does not execute scripts.
2. Press **Start / Pause**. **Auto day cycle** advances 0.05 simulated hours every 250 ms: a complete 24-hour day takes 120 seconds. Sunlight, household demand and battery dispatch change through the day.
3. **Sunny / export** pins noon with strong PV: the house consumes solar first, surplus charges the battery (up to 8 kW), and the remainder exports. **Cloudy / import** reduces PV and supplements house demand from battery/grid. **Evening / discharge** has no PV, battery support and grid import. These buttons choose a scenario; Start / Pause still controls execution.
4. **Reset** restores 09:00, 55% SOC, Auto mode, and pauses. Pause freezes clock/SOC and stops all indicated power flows; resume continues. No physical devices are controlled.
5. Flow colors: **gold** solar; **blue** house demand; **orange** grid export toward the network; **cyan** grid import toward the AC bus; **purple** battery charging toward BMS; **green** battery discharging toward the bus. Zero-flow branches have no moving indicators; reduced-motion preferences disable animation.
6. The model is a 24 kWp array, two 12 kW inverters, and 80 kWh battery with 8 kW charge/discharge limit, 10-95% SOC reserve, and 96% inverter / battery conversion efficiency. Battery energy integration uses simulated hours. Grid kW is positive importing / negative exporting; battery kW is positive charging / negative discharging. At the AC bus, **PV + grid = house + battery**.
7. Self-consumption is PV delivered directly to the house; its percentage divides that power by current PV generation and excludes battery charging. All simulation tags live under **Solar.** Invalid hour, SOC or scenario inputs stop the model with an explicit status; Reset recovers it. Only RunRequested, ResetRequested and Scenario are intended as operator controls.

### Add this screen to another project

In the JSON editor's **AI Assistant**, choose **Common task > Screens > Solar farm - energy flows and battery simulation**, then **Load exact sample resources** and **Add resources**. This includes the screen, artwork, all variables and the simulation without needing this sample file or an AI call. Each import receives a fresh namespace. Review and enable the imported simulation script, save and run the server/viewer. Customize the request and use **Ask AI** instead if a different design is wanted. Existing project settings/startup screen remain unchanged.

## HVAC plant simulation

1. Open **HVAC Plant** while the server and viewer are running. The enabled **HvacPlantSimulation** script starts paused. **Start / Pause** advances/freezes simulated time and room/buffer temperatures. Each 250 ms invocation represents one minute (one hour in 15 real seconds). The script runs on the server even when this page is hidden.
2. **Heating / Cooling** chooses the reversible heat-pump mode. Auto uses a thermostat with +/-0.3 C hysteresis and a 45-100% variable-speed circulation pump. **Auto / Manual** switches to continuous conditioning at the **Manual pump** setting, bypassing room demand but retaining buffer regulation. Edit the thermostat (16-28 C, step 0.5) or manual speed (20-100%, step 5); Enter writes, Esc cancels.
3. **Cold morning** sets -4 C outdoors and heating. **Hot afternoon** sets 34 C and cooling. **Open window** sets 5 C, heating, and increased building heat leakage. These buttons do not reset temperatures, advance the clock to a preset hour, or start the model: existing thermal mass responds gradually. A room already below the cooling setpoint waits for ambient warming; use Manual to demonstrate immediate cooling circulation.
4. **Pump fault** stops circulation and interlocks the compressor/fan. The running clock and ambient heat transfer continue, so indoor temperature drifts. Choose a weather scenario to clear the injected fault, or **Reset** to restore 06:00, room 18 C, buffer 30 C, setpoint 21 C, automatic heating, 65% manual speed and zero accumulated input energy. Reset always pauses.
5. **Red** supply indicates heating, **cyan** supply cooling, and **blue** flow returns to the plant. Moving flow uses actual pump state, not the Start request. The fan follows compressor output. Fault, pause and thermostat standby stop the relevant animations; reduced-motion preferences disable visual rotation/flow. Coil colors are a fixed heat-exchanger legend, not a temperature map.
6. The educational model has a 12 kW reversible heat pump, buffer thermal capacity 0.7 kWh/K, building capacity 3 kWh/K, up to 3 m3/h circulation, and a coil limited to +/-12 kW. Heating/cooling buffer targets are 45/8 C. Building conductance is 0.18 kW/K, or 0.65 with an open window; internal gains are 0.8 kW (2 kW in hot weather). The buffer loses heat at 0.015 kW/K relative to outdoors. Finite-step energy balances update both temperatures, including passive losses while the model runs.
7. Thermal power is positive heating / negative cooling. COP models compressor efficiency versus weather; electrical input is absolute heat-pump output divided by COP plus circulation-pump power (0.12 kW times speed fraction cubed). COP is zero when the compressor is idle; displayed electrical power may still include circulation. Input kWh integrates simulated time. Supply/return readings describe the water entering/leaving the coil during the latest simulation step; the buffer readout is the updated stored temperature.
8. All 32 tags are local under **Hvac.** Only RunRequested, ResetRequested, Mode, Scenario, AutoControl, Setpoint and ManualSpeed are intended as controls. Invalid numeric inputs stop the model with an explicit error; Reset restores valid defaults. This demonstration is not a device controller, engineering design or safety system.

To reproduce the full screen in another project, choose **AI Assistant > Common task > Screens > HVAC plant - heat pump and building simulation**, **Load exact sample resources**, then **Add resources**. The screen, embedded artwork, variables and script import into an independent namespace without an AI call. Review and enable the imported script, save, then run the server/viewer. Startup settings are preserved. Use **Ask AI** for customized designs.

## Additional industrial and energy simulations

All five pages have independent local variable folders and server-side scripts. In this saved sample the scripts are enabled, but each model starts paused. **Start / Pause** freezes/resumes model time and stops output animations while paused. **Reset** restores initial state and pauses. Navigation does not stop a running model. These are accelerated educational demonstrations, not equipment controllers or safety systems; no external devices are connected.

Each is also available in **AI Assistant > Common task > Screens**:

| Sample page | Common task | Local folder / script |
|---|---|---|
| EV Charging Station | EV charging station - arrivals and shared site power | EvCharging / EvChargingSimulation |
| Water Treatment Plant | Water treatment plant - filtration and backwash | WaterTreatment / WaterTreatmentSimulation |
| Conveyor Production Line | Conveyor production line - sorting and jam detection | ConveyorLine / ConveyorLineSimulation |
| Microgrid Control Center | Microgrid control center - tariffs and island operation | Microgrid / MicrogridSimulation |
| Compressed-Air Station | Compressed-air station - staged pressure control and leaks | CompressedAir / CompressedAirSimulation |

Select **Load exact sample resources**, review the editable bundle, then **Add resources**. No AI call or sample-file dependency is required. All artwork, tags, bindings, controls and scripts are included. Imports use independent namespaces and leave startup/project settings intact. Imported scripts are disabled for review: enable the desired script, save and run the server/viewer. Use **Ask AI** instead to customize the design.

### EV charging station

- Three 11 kW chargers serve independent 60 kWh batteries with 92% efficiency. Active cars share the editable **Site limit** (3-33 kW); power is bounded by remaining battery headroom, so departures never overshoot the 85% target. Isometric cars appear only at occupied stalls and charging flows follow actual allocated power.
- One cycle advances one simulated minute. Automatic arrivals occur every 120 cycles; **Add arrivals** admits a one-shot batch of 1-3 vehicles, queuing when all stalls are busy. Replacement vehicles arrive at 20% SOC after the current tick's dispatch and begin charging next tick. The queue is capped at 500 with explicit overflow status.
- Pause preserves vehicles, queue, batteries and meters. Reset clears requests/totals and restores initial cars and site limits. Only RunRequested, ResetRequested, ArrivalRequested, ArrivalBatch and SiteLimitKw are operator inputs. Imported energy measures AC site input; SOC includes charging losses.

### Water treatment plant

- Raw intake, clarifier, filter and clean-water storage use conserved litre inventories. Initially raw storage has 600 L, the clarifier 300 L and clean storage 600 L. Pump/dry interlocks and tank limits constrain every stage; readouts distinguish source intake, treatment, consumption and waste.
- Each cycle represents one simulated second. **Influent dirt** (1-5) increases filter clogging/differential pressure; **Demand** (0-8 L/step) draws treated water. **Auto wash** starts cleaning at 70% clogging; **Backwash now** requests it manually. Backwash takes actual water from clean storage and sends it to visible waste, reducing clogging over the cleaning cycle rather than instantly inventing capacity.
- **Pump fault** stops equipment and active flow. Clear the fault to resume, or Reset to restore inventories, controls and totals. Pause freezes inventories and backwash progress. Controls are RunRequested, ResetRequested, BackwashRequested, AutoBackwash, PumpFault, DirtyInfluent and Demand; see variable descriptions for model-owned state.

### Conveyor production line

- Four persistent package slots move original isometric cartons along an actual conveyor widget. A detector drives the diverter; each configured Nth package takes the reject branch. Good/reject counters increment only at exit, once per package; the tracked in-flight packages account for the remainder of the created batch.
- One cycle represents one simulated second. Edit **Batch size** (1-200, default 24), **Speed** (2, 4, 6 or 8 position units per step, default 4), and **Reject every** (2-5, default 3). The finite batch stops when all created packages have exited. Reset starts a fresh paused batch.
- Cartons move smoothly between the 250 ms simulation updates, including around the reject turn. Interpolation follows actual slot positions without changing simulation speed or counts; new arrivals and resets do not animate backward. Reduced-motion preferences disable interpolation.
- **Inject / Clear jam** blocks package motion, the conveyor and motor; clearing resumes the same package state without duplicate counts. Start / Pause also freezes positions. Packages are drawn from simulated positions, not an unrelated free-running animation, so no cartons continue moving while paused/jammed. Slot positions are simulation-owned state, not operator controls.

### Microgrid control center

- Local PV, 5.5 kW base household load (with evening/time variation), 3.5 kW EV demand and 40 kWh storage share the AC bus. PV peaks at 18 kW. Battery SOC remains 15-95%; charge/discharge limits are 6/8 kW with 95% one-way efficiency. All imports/exports, storage directions and served/unserved loads are explicit.
- **Cheap tariff** (0.12/kWh import, 0.04 export credit) uses spare import capacity to charge storage after PV allocation. **Peak / shave** (0.48 import, 0.16 export) dispatches storage to respect the editable **Import cap** (0.5-30 kW). Insufficient supply appears as unserved demand; it is not hidden by violating the cap.
- **Island / grid** removes utility exchange entirely while islanded. Battery/PV serve local demand first; remaining deficits and curtailed surplus PV are displayed. One cycle advances 0.05 hours (3 minutes); 24 hours takes 120 real seconds. Imported kWh and net operating cost integrate actual utility exchange; export credits may make net cost negative.
- Pause freezes SOC/clock/meters and stops flows. Reset restores scenario/cap/state/meters and pauses. Operator inputs are RunRequested, ResetRequested, Scenario, IslandMode and ImportCapKw. This is independent of the Solar Farm page, so neither demo writes the other's tags.

### Compressed-air station

- **Normal / Peak load** selects 1.0 or 2.8 Nm3/min plant consumption. **Open / Seal leak** toggles a pressure-dependent leak (0.9 Nm3/min at 7.2 bar). **C1 fault / Recover** interlocks the lead compressor; standby capacity remains available.
- Each invocation advances 0.05 minutes (3 simulated seconds). C1 starts at/below 6.6 bar and stops at/above 8.0; C2 starts at/below 6.1 and stops at/above 7.4. Each delivers 1.8 Nm3/min and draws 11 kW when running. Thresholds use hysteresis, not continuously toggling around one setpoint.
- A 2 m3 receiver uses an isothermal normal-air balance: pressure change equals (inlet - actual delivered load - leak - vent) times simulated minutes divided by volume. Pressure is gauge pressure relative to a 1 bar reference; storage pressure is not a liquid fill percentage. A modeled 8.6 bar relief limits storage; zero-pressure capacity shortages are explicitly reported as unmet air demand rather than negative storage.
- Cyan/purple flow indicates C1/C2 supply; green feeds the factory and orange escapes through the leak. Flow and motor animation follow actual output states. Totals integrate delivered/leaked air and electrical kWh. Pause holds pressure, totals and stage hysteresis memory. Reset clears injected faults, leak and peak request, restores 7.2 bar and pauses.
- Only RunRequested, ResetRequested, PeakRequested, LeakRequested and FaultRequested are controls. Invalid numeric state stops with an explicit status; Reset recovers valid defaults.

## Try it

1. Select an EditBox or Numeric Display in the editor and open **Display Appearance**.
2. Try Midnight, Ivory, Brushed Metal or LCD; background and font can also be selected independently.
3. Seven-segment digits are self-contained vector graphics, not an installed/downloaded font. Other text uses a terminal font automatically.
4. Keep **Auto-fit value to width and height** enabled, resize a display, and enter a longer signed value. Numeric displays keep units separate from the fitted readout.
5. Disable Auto-fit and set **Font Size** for fixed-size readouts.
6. Click an editable field, type a value and press Enter to write. Escape cancels. Setpoint spin arrows use a step of 0.5, bounded to 0-100.
7. Resize the browser on Responsive Lab and compare the readouts. Change the long signed value in either editable field.
8. Select a gauge and adjust the background, gradient end color and arc thickness under **Gauge Settings**.

Existing projects remain visually unchanged until these appearance options are enabled. Presets support undo/redo, and appearance is retained by save/load, copy/paste and duplicate.
