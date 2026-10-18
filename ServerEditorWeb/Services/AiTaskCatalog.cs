// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

namespace ServerEditorWeb.Services;

public sealed record AiTaskTemplate(string Id, string Category, string Title, string Prompt)
{
    public bool CreatesResources => Id != "project-organize";
    public string? ResourceBundle { get; init; }
}

public static class AiTaskCatalog
{
    public const string ResourceInstructions = """
        Create only new resources and their dependencies. Use local demo variables with no external device connections.
        Preserve existing resources and settings; they are not supplied to this request.
        Return a standalone resource bundle using the __TASK__ namespace placeholder, not full project JSON.
        Create executable scripts, PLC programs, schedules, events, automation rules, and batch sequences disabled for review before enabling.
        Summarize created resources and any setup still required in the comments.
        """;

    public const string CommonInstructions = """
        Preserve existing resources and settings unless explicitly asked to change them. Use unique resource names and symbol IDs.
        Create any missing variables needed by the new resources and make all variable paths resolve, excluding the root folder name.
        Use local demo variables with no external device connections for examples. Do not repurpose existing device outputs.
        Create executable scripts, PLC programs, schedules, events, automation rules, and batch sequences disabled for review before enabling.
        Return the full updated project JSON, not prose or code snippets. Summarize created resources and any setup still required in the comments.
        """;

    public static IReadOnlyList<AiTaskTemplate> Tasks { get; } = Array.AsReadOnly<AiTaskTemplate>(
    [
        new("script-read-write-loop", "Scripts", "Read and write variables in a loop", """
            Create a CSharp script named DemoReadWriteLoop, IntervalMs 1000. Create three Double input variables
            DemoLoop.Input1, DemoLoop.Input2, DemoLoop.Input3 and three writable Double output variables
            DemoLoop.Output1, DemoLoop.Output2, DemoLoop.Output3. Give the inputs distinct local demo values.
            In Code, use a bounded for loop over the three input/output path pairs; ReadDouble each input,
            multiply it by 2, and Write the corresponding output. Process the list once per invocation;
            IntervalMs provides repetition. Do not use while(true), sleeps, background tasks, or subscriptions.
            """),
        new("script-average", "Scripts", "Calculate average, minimum, and maximum", """
            Create a CSharp script named DemoStatistics running every 1000 ms. Read three local Double variables
            DemoStats.Value1, DemoStats.Value2, DemoStats.Value3 in a bounded loop and Write their average,
            minimum, and maximum to DemoStats.Average, DemoStats.Minimum, DemoStats.Maximum.
            Create missing input/output variables with sensible initial values. Use ReadDouble and Write.
            """),
        new("script-scale", "Scripts", "Scale raw readings into engineering units", """
            Create a CSharp script named DemoScaling with IntervalMs 1000. Read DemoScaling.Raw using ReadDouble,
            clamp it to 0..4095, scale it linearly to 0..100, and Write DemoScaling.Percent.
            Create both local Double variables and label the output engineering unit %. No device connection.
            """),
        new("script-heartbeat", "Scripts", "Create a heartbeat and cycle counter", """
            Create a CSharp script named DemoHeartbeat with IntervalMs 1000. Toggle local Boolean DemoHeartbeat.Pulse
            using ReadBool and Write, and increment local Int32 DemoHeartbeat.CycleCount using ReadInt and Write.
            Create both variables. Perform one update per invocation, with no endless loop or sleeping.
            """),
        new("screen-two-tanks", "Screens", "Two tanks connected by a pipe", """
            Create an SVG screen named TwoTanksDemo, Width 1000, Height 600, Background #0f172a.
            Add two actual Type tank symbols: tank1 at X=100,Y=140,Width=160,Height=240 and
            tank2 at X=700,Y=140,Width=160,Height=240. Label them Tank 1 and Tank 2, set TankMin=0,
            TankMax=100, TankUnit=%, TankShowLevel=true, and bind them to TwoTanks.Tank1Level and TwoTanks.Tank2Level.
            Connect their sides visually with a horizontal Type pipe at X=260,Y=310,Width=440,Height=20,
            PipeOrientation=horizontal, PipeShowFlow=true, PipeFlowSpeed=1, bound to TwoTanks.Flow.
            Add readable numeric level displays and a screen title without overlapping equipment.
            Create the three local Double variables with initial values 65, 30, and 1 respectively.
            Use TankLiquidColor, TankBodyColor, PipeColor, and PipeFlowColor for styling. No real-device writes.
            """),
        new("screen-styled-tank-transfer", "Screens", "Styled tank transfer - exact sample with simulation", """
            Create the Tank Transfer screen exactly as the StyledInstruments sample: two cylindrical isometric
            tanks with conical bases, gradient shells and liquid, shaded animated pipes behind the tanks,
            a central isometric pump, status display, Start / Stop and Refill & Restart buttons.
            Use the complete resource bundle below as the authoritative template. Preserve symbol order,
            coordinates, sizes, colors, bindings, commands, initial values, and the 250 ms CSharp simulation.
            Include all six local Transfer variables and the simulation script. Retain __TASK__ placeholders.
            Do not add navigation references to other sample pages, change the startup screen, or add devices.
            Keep the script disabled for review; explain that it must be enabled and the project saved before
            running the server and viewer. Tank filling transitions are automatic; percentages indicate level, not volume.
            Return this bundle unchanged unless the request explicitly customizes it:

            """ + StyledTankTransferTemplate.Json) { ResourceBundle = StyledTankTransferTemplate.Json },
        new("screen-solar-farm", "Screens", "Solar farm - energy flows and battery simulation", """
            Create the Solar Farm screen exactly as the StyledInstruments example using the complete bundle below.
            Include the original vector solar arrays, two hybrid inverters, house, utility network and battery/BMS,
            gradient equipment and displays, animated pipe flows, six controls, all Solar variables and the 250 ms simulation.
            Preserve coordinates, SVG artwork, symbol ordering, initial values and commands unless explicitly customized.
            Grid positive imports cyan toward the AC bus; grid negative exports orange toward the network.
            Battery positive charges purple toward BMS; battery negative discharges green toward the bus.
            Solar flow is gold; house load is blue. Do not animate zero flow. Retain the __TASK__ namespace placeholders.
            Keep the script disabled for review. Explain how to enable it, save and run the server/viewer, then Start / Pause.
            Auto day cycle compresses 24 h into 120 seconds; scenario buttons select sunny, cloudy or evening operation.
            Preserve PV + grid = load + battery and battery SOC bounds 10..95%. This is a local simulation, not real device control.
            Do not import other sample pages or change the project's startup screen. Return this bundle unless customized:

            """ + SolarFarmTemplate.Json) { ResourceBundle = SolarFarmTemplate.Json },
        new("screen-hvac-plant", "Screens", "HVAC plant - heat pump and building simulation", """
            Create the HVAC Plant screen exactly as the StyledInstruments sample using the authoritative bundle below.
            Preserve the original vector heat pump with animated fan, shaded buffer, isometric circulation pump,
            building cutaway and coil, red heating / cyan cooling supply and blue returning flow, live displays,
            thermostat and manual speed editors, all controls, local Hvac variables and the 250 ms simulation.
            Auto uses +/-0.3 C thermostat hysteresis and variable-speed circulation; Manual conditions continuously
            at the chosen pump speed, still regulating buffer temperature. Heating/cooling target 45/8 C buffer water.
            Cold morning and open window select heating; hot afternoon selects cooling. Pump fault stops circulation
            and interlocks the compressor while the building continues losing/gaining heat. Reset restores defaults and pauses.
            One invocation advances one simulated minute. Preserve the buffer and room energy balances, 12 kW limit,
            and COP-based electrical consumption. This is an educational local model, not external device control.
            Use __TASK__ namespaces and keep the script disabled for review. Explain how to enable it, save and run
            the server/viewer, then Start / Pause. Do not change startup settings or link unrelated sample pages.
            Return this bundle unchanged unless the request explicitly customizes it:

            """ + HvacPlantTemplate.Json) { ResourceBundle = HvacPlantTemplate.Json },
        new("screen-ev-charging", "Screens", "EV charging station - arrivals and shared site power", """
            Reproduce the EV Charging Station from the authoritative bundle below, including isometric cars,
            chargers, arrival/departure simulation, independent batteries, shared site-power allocation, flows,
            live readouts and all controls. Preserve energy accounting, capacity limits and the arrival queue.
            Keep the simulation disabled for review. Explain enable/save/run and Start / Pause / Reset.
            Preserve __TASK__ placeholders, symbol order and embedded SVG artwork. No external devices or startup changes.
            Return the complete bundle unchanged unless the request explicitly customizes it:

            """ + EvChargingTemplate.Json) { ResourceBundle = EvChargingTemplate.Json },
        new("screen-water-treatment", "Screens", "Water treatment plant - filtration and backwash", """
            Reproduce the Water Treatment Plant from the authoritative bundle below: tanks, filters, valves,
            pumps, colored stage flows, filter loading and backwash. Include every local variable and script.
            Preserve water balance, clean-water consumption during backwash, dry-pump interlocks and all controls.
            Keep the simulation disabled for review. Explain enable/save/run and Start / Pause / Reset.
            Preserve __TASK__ placeholders, symbol order and embedded artwork. No external devices or startup changes.
            Return the complete bundle unchanged unless the request explicitly customizes it:

            """ + WaterTreatmentTemplate.Json) { ResourceBundle = WaterTreatmentTemplate.Json },
        new("screen-conveyor-line", "Screens", "Conveyor production line - sorting and jam detection", """
            Reproduce the Conveyor Production Line from the authoritative bundle below: moving packages, motor,
            sensors, diverter and sorted outputs. Include all controls, local variables and the bounded simulation.
            Preserve package tracking, exactly-once good/reject counts and jam detection/recovery. Packages must
            stop when the line is paused or jammed. Keep the script disabled for review; explain enable/save/run.
            Preserve __TASK__ placeholders, symbol order and embedded artwork. No external devices or startup changes.
            Return the complete bundle unchanged unless the request explicitly customizes it:

            """ + ConveyorLineTemplate.Json) { ResourceBundle = ConveyorLineTemplate.Json },
        new("screen-microgrid", "Screens", "Microgrid control center - tariffs and island operation", """
            Reproduce the Microgrid Control Center from the authoritative bundle below: PV, storage, house,
            EV and utility network with directional flows. Include every control, local variable and script.
            Preserve tariff-aware dispatch, peak-shaving import limits, SOC/efficiency accounting, and island
            operation with zero utility exchange and explicitly reported unmet demand / curtailed PV.
            Keep the script disabled for review; explain enable/save/run and Start / Pause / Reset.
            Preserve __TASK__ placeholders, symbol order and embedded artwork. No external devices or startup changes.
            Return the complete bundle unchanged unless the request explicitly customizes it:

            """ + MicrogridTemplate.Json) { ResourceBundle = MicrogridTemplate.Json },
        new("screen-compressed-air", "Screens", "Compressed-air station - staged pressure control and leaks", """
            Reproduce the Compressed-Air Station using the authoritative bundle below: lead and standby compressors,
            shaded receiver with pressure gauge, industrial air load, simulated leak branch, actual motor/flow states,
            controls and local variables. The 250 ms script advances 0.05 simulated minutes, balancing normal-air volume
            in a 2 m3 receiver. Preserve independent compressor hysteresis, peak load, leak, C1 fault, unmet demand,
            energy accounting and receiver bounds. This is an educational model, not a physical safety controller.
            Keep the script disabled for review; explain enable/save/run and Start / Pause / Reset.
            Preserve __TASK__ placeholders, symbol order and embedded artwork. No external devices or startup changes.
            Return the complete bundle unchanged unless the request explicitly customizes it:

            """ + CompressedAirTemplate.Json) { ResourceBundle = CompressedAirTemplate.Json },
        new("screen-equipment", "Screens", "Equipment status overview", """
            Create an SVG screen named EquipmentOverviewDemo with three labeled equipment cards.
            Each card contains a Type motorcontrol symbol, a Boolean status indicator, and a numeric runtime display.
            Create local Running Boolean and RuntimeHours Double variables under DemoEquipment.Pump1,
            DemoEquipment.Pump2, and DemoEquipment.Pump3 and bind the symbols to the correct paths.
            Arrange the cards with clear spacing and a title; do not add real equipment commands.
            """),
        new("screen-trends", "Screens", "Historical trends and current readings", """
            Create a screen named TrendsDemo containing an hdachart and three numericdisplay symbols.
            Create three local Double variables under DemoTrends and enable their DataLogging.Enabled.
            Put their exact paths in the chart HdaVariablePaths, set HdaTimeRangeMinutes=60 and HdaMaxPoints=500,
            and bind each numeric display to one variable. Add engineering-unit labels and a clear layout.
            Do not invent database credentials; mention that historical storage must be configured separately.
            """),
        new("screen-alarms", "Screens", "Alarm summary screen", """
            Create a screen named AlarmSummaryDemo containing an alarmbanner, a large alarmlist, and explanatory text.
            Add local Double DemoAlarms.Temperature with Alarm TriggerType Limit, LowLowLimit=5, LowLimit=10,
            HighLimit=80, HighHighLimit=90, Hysteresis=2 and a meaningful Message. Initialize it to 40.
            Keep existing alarm settings unchanged and do not configure external notifications.
            """),
        new("screen-navigation", "Screens", "Overview and detail screens with navigation", """
            Create two SVG screens named DemoOverview and DemoDetail. Put a title and local numeric status on each,
            with navbutton symbols linking from overview to detail and back using NavButtonTarget.
            Bind the numeric displays to a shared local Double DemoNavigation.Value. Use distinct symbol IDs.
            Preserve the existing Server.StartupScreen; do not replace the project's startup screen.
            """),
        new("variables-demo", "Variables", "Create a structured set of demo tags", """
            Create a DemoTags folder with nested Tank1 and Tank2 folders. In each create Double Level and Temperature,
            Boolean Running, and Int32 CycleCount variables. Set appropriate units, descriptions, types,
            ReadWrite access, and nonzero demo readings. Use Folder.Folders and Folder.Variables for hierarchy.
            """),
        new("variables-simulation", "Variables", "Generate simulated process readings", """
            Create three local Double variables in DemoSimulation with Simulation DriverConfigs:
            Level as Sine (Period 60000, Amplitude 30, Offset 50), Temperature as Sine (Period 120000, Amplitude 5, Offset 25),
            Flow as Random (Min 0, Max 10). Use PollTime=1000, appropriate EngineeringUnit and Description,
            and initial values matching their ranges. Do not attach external protocol drivers.
            """),
        new("variables-alarms", "Variables", "Configure four-level demo alarms", """
            Create local Double DemoLimits.Level initialized to 50 with EngineeringUnit %.
            Set Alarm.TriggerType=Limit, LowLowLimit=5, LowLimit=15, HighLimit=85, HighHighLimit=95,
            Hysteresis=2 and Message describing a level out of range. Preserve existing variables' alarms.
            """),
        new("variables-logging", "Variables", "Add logged demo measurements", """
            Create local Double DemoLogging.Temperature, DemoLogging.Pressure, and DemoLogging.Flow with realistic
            initial values and engineering units. Set DataLogging.Enabled=true and DataLogging.Hysteresis=0.1.
            Do not change server storage settings or invent credentials. Explain that the historical storage
            connection must be configured before historical data can be queried.
            """),
        new("calculated-average", "Calculated Tags", "Average and total of demo measurements", """
            Create three local Double variables DemoCalculated.Value1, Value2, Value3 with initial values 10,20,30.
            Add two CalculatedVariables named DemoAverage and DemoTotal, Type Double, IntervalMs 1000,
            FolderPath _Calculated, using AggregateFunction Avg and Sum respectively and
            AggregateSourcePath DemoCalculated.Value1;DemoCalculated.Value2;DemoCalculated.Value3.
            """),
        new("calculated-conversion", "Calculated Tags", "Convert temperature units", """
            Create local Double DemoCalculated.Celsius initialized to 20. Add a CalculatedVariables entry
            DemoFahrenheit, Type Double, FolderPath _Calculated, IntervalMs 1000, EngineeringUnit degF,
            with Expression Convert.ToDouble(Read("DemoCalculated.Celsius")) * 9.0 / 5.0 + 32.0.
            """),
        new("plc-counter", "PLC Programs", "Structured Text demo counter", """
            Create a disabled ST PlcPrograms entry named DemoCounter, IntervalMs 100, declaring an INT counter
            initialized to zero and incrementing it once per scan with wraparound at 1000.
            Use VAR/END_VAR and valid Structured Text syntax. This is a local PLC variable, not a device output;
            do not invent functions for accessing OPC variables.
            """),
        new("recipe-setpoints", "Recipes", "Recipe definition for demo setpoints", """
            Create local writable Double DemoRecipe.TemperatureSetpoint, SpeedSetpoint, and VolumeSetpoint.
            Add Recipes entry DemoSetpoints with Variables containing unique stable Index values 0,1,2,
            each with VariablePath and DisplayName matching those variables, and DbPath recipes/DemoSetpoints.db.
            Create the definition only; do not invent an inline recipe-values array or activate a recipe.
            """),
        new("schedule-weekdays", "Schedulers", "Weekday demo schedule", """
            Create local Boolean DemoSchedule.Active initialized false and a disabled Schedulers entry DemoWeekdays.
            Use SlotMinutes=60 and WeeklySlots for DayOfWeek 1 through 5, StartMinute=480, EndMinute=1080.
            Set WeekendMode=Off and HolidayMode=Off. Commands should use Action WriteVariable,
            VariablePath DemoSchedule.Active, Value "true"; DeactivateCommands should use the same Action and path
            with Value "false". No physical equipment bindings.
            """),
        new("report-summary", "Reports", "Process summary report", """
            Create local Double DemoReport.Temperature, Pressure, Flow with units and initial values.
            Add an HTML Reports entry named DemoProcessSummary with a Header section, an explanatory Text section,
            and a Table section listing those paths in TableVariablePaths. Give each section a unique Id.
            Do not configure email delivery or credentials.
            """),
        new("report-history", "Reports", "Historical chart report", """
            Create local Double DemoReportHistory.Temperature and Flow with DataLogging.Enabled=true.
            Add an HTML report DemoHistoricalReport with Header and Chart sections. Set ChartType=Line,
            ChartVariablePaths to both paths, ChartTimeRangeMinutes=60, and ChartMaxPoints=500.
            Mention that historical storage must be configured separately; do not invent credentials.
            """),
        new("asset-maintenance", "Assets", "Asset runtime and maintenance tracking", """
            Create local Boolean DemoAsset.Running and DemoAsset.Fault initialized false.
            Add an Assets entry DemoPump with RunningVariablePath and FaultVariablePath pointing to those tags,
            InitialRuntimeHours=0, and one MaintenanceSchedules entry named Inspection with unique Id,
            IntervalHours=500 and IntervalDays=30. This tracks demo status only and does not command equipment.
            """),
        new("event-threshold", "Events", "Threshold event with a demo status flag", """
            Create local Double DemoEvent.Level initialized to 40 and Boolean DemoEvent.HighLevel initialized false.
            Add a disabled Events entry DemoHighLevel with ConditionVariablePath DemoEvent.Level,
            ConditionOperator >, ConditionValue "80", TriggerMode RisingEdge, HoldTimeSeconds=5.
            Commands should use Action WriteVariable, VariablePath DemoEvent.HighLevel, Value "true";
            ClearCommands should use the same Action and path with Value "false". No external notifications or device writes.
            """),
        new("automation-threshold", "Automation Rules", "No-code threshold rule", """
            Create local Double DemoRule.Level initialized to 40 and Boolean DemoRule.High initialized false.
            Add a disabled AutomationRules entry DemoThreshold with ConditionLogic All, CooldownSeconds=5,
            one Conditions item Type VariableThreshold, VariablePath DemoRule.Level, Operator >, ThresholdValue "80".
            Actions should contain Type WriteVariable, VariablePath DemoRule.High, Value "true";
            ClearActions should write "false" to the same local tag. Use unique rule, condition, and action Ids.
            Do not configure notifications or real device outputs.
            """),
        new("batch-workflow", "Batch Sequences", "Three-step demo workflow", """
            Create a disabled BatchSequences entry DemoWorkflow with three Steps: Prepare, Process, Complete,
            unique step Ids, descriptions, and TimeoutSeconds=10 for each. Connect them with Transitions using
            FromStepId and ToStepId, leaving ConditionVariablePath empty for timer-only transitions.
            Do not invent a ConditionType property. Set AutoRestart=false.
            Create local Boolean DemoBatch.Start, Abort, Hold tags and set StartVariablePath, AbortVariablePath,
            HoldVariablePath accordingly. Do not attach equipment entry or exit actions.
            """),
        new("users-groups", "Users and Groups", "Read-only operator group", """
            Add a UserGroups entry named DemoObservers with AccessLevel Read, CanAccessEditor=false,
            CanAccessRuntime=true. Do not add users, passwords, or credentials, and do not alter existing groups,
            access permissions, or login settings. Explain how to assign an existing user manually.
            """),
        new("localization-labels", "Localization", "English and Italian operator labels", """
            Add Strings entries for demo screen labels TankLevel, Temperature, Flow, Running, Fault, and Overview.
            Provide en and it translations with the existing LocalizedStringEntry schema (Key and Translations dictionary).
            Keep existing translations and keys unchanged. Do not change the project's selected locale.
            """),
        new("project-import", "Project Tasks", "Import variables from a reference file", """
            Import variables and their folder hierarchy from the selected reference file into a new ImportedDemo folder.
            Preserve types, units, and descriptions. If no reference file is provided, explain that one is required
            and do not invent imported content. Do not create external connections or change existing resources.
            """),
        new("project-organize", "Project Tasks", "Organize resource folders without renaming tags", """
            Organize existing Scripts, PlcPrograms, and Screens into sensible editor folders using each resource's
            Group property. Preserve all resource names, code, symbols, variable paths, bindings, and runtime settings.
            Do not reorganize the variable Folder hierarchy, since that would change OPC paths.
            """),
        new("project-tank-demo", "Project Tasks", "Complete two-tank demo with script and screen", """
            Create a complete local demo under a new TwoTankDemo variable folder: Double Tank1Level initialized 65,
            Double Tank2Level initialized 30, Double Flow initialized 1, Boolean Running initialized false.
            Add a disabled CSharp script with IntervalMs=1000 that, when the local Running flag is true, transfers
            at most 1 unit per cycle from Tank1Level to Tank2Level, clamping both levels to 0..100.
            Use ReadBool, ReadDouble, and Write, with no endless loop and no physical-device connection.
            Add an SVG screen with two labeled Type tank symbols bound to the levels, a Type pipe visually joining
            their sides and bound to Flow, and numeric displays. Use TankMin=0,TankMax=100,TankUnit=%.
            Add a recipe definition containing the level paths and an HTML report table for the demo readings.
            Keep the existing startup screen unchanged. Explain that the script must be reviewed and enabled manually.
            """)
    ]);
}
