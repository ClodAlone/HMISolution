// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

namespace ServerEditorWeb.Services;

public sealed record AiTaskTemplate(string Id, string Category, string Title, string Prompt)
{
    public bool CreatesResources => Id != "project-organize";
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
