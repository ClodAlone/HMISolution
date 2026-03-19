$json = @'
{
  "Server": {
    "EndpointUrl": "opc.tcp://localhost:14870/ReportDemo",
    "EnableAnonymous": false,
    "EnableEditorLogin": true,
    "EnableRuntimeLogin": true,
    "RequireStrongPassword": false,
    "StartupScreen": "Dashboard",
    "ShowNavigationBar": true,
    "DiagnosticsPort": 14871,
    "EventLog": { "Enabled": true, "DbPath": "events.db", "MaxAgeDays": 90 }
  },
  "Database": null,
  "Users": [
    { "Username": "admin", "PasswordHash": "", "Group": "Administrators", "CanEdit": true },
    { "Username": "operator", "PasswordHash": "", "Group": "Operators", "CanEdit": false }
  ],
  "UserGroups": [
    { "Name": "Administrators", "Permissions": "Read,Write,Admin" },
    { "Name": "Operators", "Permissions": "Read,Write" }
  ],
  "Folder": {
    "Name": "Plant",
    "Folders": [
      {
        "Name": "Intake",
        "Variables": [
          { "Name": "FlowRate_m3h", "Type": "Double", "Access": "Read", "Value": 120.0, "Statistics": { "Enabled": true }, "DataLogging": { "Enabled": true, "Hysteresis": 2.0, "MaxIntervalSeconds": 60 } },
          { "Name": "Pressure_bar", "Type": "Double", "Access": "Read", "Value": 3.2, "Statistics": { "Enabled": true }, "DataLogging": { "Enabled": true, "Hysteresis": 0.1, "MaxIntervalSeconds": 60 } },
          { "Name": "Temperature_C", "Type": "Double", "Access": "Read", "Value": 14.5, "Statistics": { "Enabled": true }, "DataLogging": { "Enabled": true, "Hysteresis": 0.2, "MaxIntervalSeconds": 120 } },
          { "Name": "Turbidity_NTU", "Type": "Double", "Access": "Read", "Value": 2.8, "Statistics": { "Enabled": true }, "DataLogging": { "Enabled": true, "Hysteresis": 0.5, "MaxIntervalSeconds": 60 } },
          { "Name": "Pump1_Running", "Type": "Boolean", "Access": "ReadWrite", "Value": true, "InitialValue": "true", "Retentive": true },
          { "Name": "Pump2_Running", "Type": "Boolean", "Access": "ReadWrite", "Value": false, "InitialValue": "false", "Retentive": true },
          { "Name": "Pump1_Power_kW", "Type": "Double", "Access": "Read", "Value": 15.5, "DataLogging": { "Enabled": true, "Hysteresis": 1.0, "MaxIntervalSeconds": 120 } }
        ]
      },
      {
        "Name": "Treatment",
        "Variables": [
          { "Name": "pH", "Type": "Double", "Access": "Read", "Value": 7.1, "Statistics": { "Enabled": true }, "Alarm": { "TriggerType": "Limit", "HighHighLimit": 8.5, "HighLimit": 7.8, "LowLimit": 6.5, "LowLowLimit": 6.0, "Message": "pH out of compliance range", "Hysteresis": 0.1 }, "DataLogging": { "Enabled": true, "Hysteresis": 0.05, "MaxIntervalSeconds": 30 } },
          { "Name": "Chlorine_ppm", "Type": "Double", "Access": "Read", "Value": 1.2, "Statistics": { "Enabled": true }, "Alarm": { "TriggerType": "Limit", "HighHighLimit": 4.0, "HighLimit": 2.5, "LowLimit": 0.5, "LowLowLimit": 0.2, "Message": "Chlorine level out of range", "Hysteresis": 0.1 }, "DataLogging": { "Enabled": true, "Hysteresis": 0.1, "MaxIntervalSeconds": 30 } },
          { "Name": "DissolvedOxygen_mgL", "Type": "Double", "Access": "Read", "Value": 8.2, "Statistics": { "Enabled": true }, "DataLogging": { "Enabled": true, "Hysteresis": 0.2, "MaxIntervalSeconds": 60 } },
          { "Name": "ChemicalDosing_Running", "Type": "Boolean", "Access": "ReadWrite", "Value": true, "InitialValue": "true", "Retentive": true },
          { "Name": "DosingRate_mlmin", "Type": "Double", "Access": "ReadWrite", "Value": 25.0, "InitialValue": "25.0", "Retentive": true },
          { "Name": "FilterPressure_bar", "Type": "Double", "Access": "Read", "Value": 1.8, "Statistics": { "Enabled": true }, "DataLogging": { "Enabled": true, "Hysteresis": 0.1, "MaxIntervalSeconds": 60 } }
        ]
      },
      {
        "Name": "Storage",
        "Variables": [
          { "Name": "Tank1_Level_pct", "Type": "Double", "Access": "Read", "Value": 72.0, "Statistics": { "Enabled": true }, "Alarm": { "TriggerType": "Limit", "HighHighLimit": 95.0, "HighLimit": 90.0, "LowLimit": 15.0, "LowLowLimit": 10.0, "Message": "Tank 1 level alarm", "Hysteresis": 2.0 }, "DataLogging": { "Enabled": true, "Hysteresis": 1.0, "MaxIntervalSeconds": 60 } },
          { "Name": "Tank2_Level_pct", "Type": "Double", "Access": "Read", "Value": 58.0, "Statistics": { "Enabled": true }, "Alarm": { "TriggerType": "Limit", "HighHighLimit": 95.0, "HighLimit": 90.0, "LowLimit": 15.0, "LowLowLimit": 10.0, "Message": "Tank 2 level alarm", "Hysteresis": 2.0 }, "DataLogging": { "Enabled": true, "Hysteresis": 1.0, "MaxIntervalSeconds": 60 } },
          { "Name": "Tank1_Temperature_C", "Type": "Double", "Access": "Read", "Value": 15.2, "DataLogging": { "Enabled": true, "Hysteresis": 0.3, "MaxIntervalSeconds": 120 } },
          { "Name": "Tank2_Temperature_C", "Type": "Double", "Access": "Read", "Value": 14.8, "DataLogging": { "Enabled": true, "Hysteresis": 0.3, "MaxIntervalSeconds": 120 } },
          { "Name": "OutletValve_Open", "Type": "Boolean", "Access": "ReadWrite", "Value": true, "InitialValue": "true", "Retentive": true }
        ]
      },
      {
        "Name": "Distribution",
        "Variables": [
          { "Name": "OutflowRate_m3h", "Type": "Double", "Access": "Read", "Value": 95.0, "Statistics": { "Enabled": true }, "DataLogging": { "Enabled": true, "Hysteresis": 2.0, "MaxIntervalSeconds": 60 } },
          { "Name": "Pressure_bar", "Type": "Double", "Access": "Read", "Value": 4.1, "Statistics": { "Enabled": true }, "DataLogging": { "Enabled": true, "Hysteresis": 0.1, "MaxIntervalSeconds": 60 } },
          { "Name": "BoosterPump_Running", "Type": "Boolean", "Access": "ReadWrite", "Value": true, "InitialValue": "true", "Retentive": true },
          { "Name": "BoosterPump_Power_kW", "Type": "Double", "Access": "Read", "Value": 22.0, "DataLogging": { "Enabled": true, "Hysteresis": 1.0, "MaxIntervalSeconds": 120 } }
        ]
      },
      {
        "Name": "Energy",
        "Variables": [
          { "Name": "TotalPower_kW", "Type": "Double", "Access": "Read", "Value": 0.0, "Statistics": { "Enabled": true }, "DataLogging": { "Enabled": true, "Hysteresis": 2.0, "MaxIntervalSeconds": 60 } },
          { "Name": "DailyEnergy_kWh", "Type": "Double", "Access": "Read", "Value": 0.0, "DataLogging": { "Enabled": true, "Hysteresis": 5.0, "MaxIntervalSeconds": 300 } },
          { "Name": "DailyWaterVolume_m3", "Type": "Double", "Access": "Read", "Value": 0.0, "DataLogging": { "Enabled": true, "Hysteresis": 10.0, "MaxIntervalSeconds": 300 } }
        ]
      }
    ]
  },
  "Schedulers": [
    {
      "Name": "DailyReportSchedule",
      "Enabled": true,
      "SlotMinutes": 60,
      "ActivateCommands": [{ "Id": "cmd_genrpt1", "Action": "GenerateReport", "Trigger": "Click", "TargetReport": "DailyOperations" }],
      "DeactivateCommands": [],
      "WeeklySlots": [
        { "DayOfWeek": 0, "StartMinute": 1380, "EndMinute": 1440 },
        { "DayOfWeek": 1, "StartMinute": 1380, "EndMinute": 1440 },
        { "DayOfWeek": 2, "StartMinute": 1380, "EndMinute": 1440 },
        { "DayOfWeek": 3, "StartMinute": 1380, "EndMinute": 1440 },
        { "DayOfWeek": 4, "StartMinute": 1380, "EndMinute": 1440 },
        { "DayOfWeek": 5, "StartMinute": 1380, "EndMinute": 1440 },
        { "DayOfWeek": 6, "StartMinute": 1380, "EndMinute": 1440 }
      ],
      "WeekendMode": "Same", "WeekendSlots": [], "HolidayMode": "Same", "HolidayLocale": "", "CustomHolidays": []
    },
    {
      "Name": "ShiftReportSchedule",
      "Enabled": true,
      "SlotMinutes": 60,
      "ActivateCommands": [{ "Id": "cmd_genrpt2", "Action": "GenerateReport", "Trigger": "Click", "TargetReport": "ShiftHandover" }],
      "DeactivateCommands": [],
      "WeeklySlots": [
        { "DayOfWeek": 0, "StartMinute": 360, "EndMinute": 420 }, { "DayOfWeek": 0, "StartMinute": 840, "EndMinute": 900 }, { "DayOfWeek": 0, "StartMinute": 1320, "EndMinute": 1380 },
        { "DayOfWeek": 1, "StartMinute": 360, "EndMinute": 420 }, { "DayOfWeek": 1, "StartMinute": 840, "EndMinute": 900 }, { "DayOfWeek": 1, "StartMinute": 1320, "EndMinute": 1380 },
        { "DayOfWeek": 2, "StartMinute": 360, "EndMinute": 420 }, { "DayOfWeek": 2, "StartMinute": 840, "EndMinute": 900 }, { "DayOfWeek": 2, "StartMinute": 1320, "EndMinute": 1380 },
        { "DayOfWeek": 3, "StartMinute": 360, "EndMinute": 420 }, { "DayOfWeek": 3, "StartMinute": 840, "EndMinute": 900 }, { "DayOfWeek": 3, "StartMinute": 1320, "EndMinute": 1380 },
        { "DayOfWeek": 4, "StartMinute": 360, "EndMinute": 420 }, { "DayOfWeek": 4, "StartMinute": 840, "EndMinute": 900 }, { "DayOfWeek": 4, "StartMinute": 1320, "EndMinute": 1380 },
        { "DayOfWeek": 5, "StartMinute": 360, "EndMinute": 420 }, { "DayOfWeek": 5, "StartMinute": 840, "EndMinute": 900 }, { "DayOfWeek": 5, "StartMinute": 1320, "EndMinute": 1380 },
        { "DayOfWeek": 6, "StartMinute": 360, "EndMinute": 420 }, { "DayOfWeek": 6, "StartMinute": 840, "EndMinute": 900 }, { "DayOfWeek": 6, "StartMinute": 1320, "EndMinute": 1380 }
      ],
      "WeekendMode": "Same", "WeekendSlots": [], "HolidayMode": "Same", "HolidayLocale": "", "CustomHolidays": []
    }
  ],
  "Reports": [
    {
      "Name": "DailyOperations", "Enabled": true, "Format": "HTML", "Title": "Daily Operations Report",
      "Description": "Comprehensive daily overview of water treatment plant operations including flow rates, quality parameters, tank levels, and energy consumption.",
      "PageSize": "A4", "Orientation": "Landscape",
      "Sections": [
        { "Id": "sec_hdr", "Type": "Header", "Title": "Water Treatment Plant \u2014 Daily Operations Summary" },
        { "Id": "sec_intro", "Type": "Text", "Content": "This report provides a 24-hour overview of plant operations. Generated automatically at end of day." },
        { "Id": "sec_flow_chart", "Type": "Chart", "Title": "Intake & Distribution Flow Rates (24h)", "ChartType": "Line", "ChartVariablePaths": ["Intake.FlowRate_m3h", "Distribution.OutflowRate_m3h"], "ChartTimeRangeMinutes": 1440, "ChartMaxPoints": 500, "ChartWidth": 800, "ChartHeight": 300, "ChartShowLegend": true, "ChartShowGrid": true },
        { "Id": "sec_quality_chart", "Type": "Chart", "Title": "Water Quality \u2014 pH & Chlorine (24h)", "ChartType": "Line", "ChartVariablePaths": ["Treatment.pH", "Treatment.Chlorine_ppm"], "ChartTimeRangeMinutes": 1440, "ChartMaxPoints": 500, "ChartWidth": 800, "ChartHeight": 300, "ChartShowLegend": true, "ChartShowGrid": true },
        { "Id": "sec_tank_chart", "Type": "Chart", "Title": "Storage Tank Levels (24h)", "ChartType": "Area", "ChartVariablePaths": ["Storage.Tank1_Level_pct", "Storage.Tank2_Level_pct"], "ChartTimeRangeMinutes": 1440, "ChartMaxPoints": 500, "ChartWidth": 800, "ChartHeight": 250, "ChartShowLegend": true, "ChartShowGrid": true },
        { "Id": "sec_pb", "Type": "PageBreak" },
        { "Id": "sec_stats_tbl", "Type": "Table", "Title": "Process Statistics (24h)", "TableVariablePaths": ["Intake.FlowRate_m3h","Intake.Pressure_bar","Intake.Temperature_C","Intake.Turbidity_NTU","Treatment.pH","Treatment.Chlorine_ppm","Treatment.DissolvedOxygen_mgL","Treatment.FilterPressure_bar","Storage.Tank1_Level_pct","Storage.Tank2_Level_pct","Distribution.OutflowRate_m3h","Distribution.Pressure_bar"], "TableShowStatistics": true, "TableTimeRangeMinutes": 1440 },
        { "Id": "sec_energy_chart", "Type": "Chart", "Title": "Power Consumption (24h)", "ChartType": "Bar", "ChartVariablePaths": ["Energy.TotalPower_kW","Intake.Pump1_Power_kW","Distribution.BoosterPump_Power_kW"], "ChartTimeRangeMinutes": 1440, "ChartMaxPoints": 200, "ChartWidth": 800, "ChartHeight": 250, "ChartShowLegend": true, "ChartShowGrid": true },
        { "Id": "sec_energy_vals", "Type": "Value", "ValueVariablePath": "Energy.DailyEnergy_kWh", "ValueLabel": "Total Daily Energy Consumption", "ValueFormat": "F1", "ValueUnit": "kWh" },
        { "Id": "sec_water_vol", "Type": "Value", "ValueVariablePath": "Energy.DailyWaterVolume_m3", "ValueLabel": "Total Daily Water Volume", "ValueFormat": "F0", "ValueUnit": "m\u00b3" }
      ],
      "Delivery": { "Method": "Both", "EmailRecipients": "plant.manager@example.com, ops.team@example.com", "EmailSubject": "Daily Operations Report \u2014 {ReportName} \u2014 {DateTime}", "SmtpServer": "smtp.example.com", "SmtpPort": 587, "SmtpUser": "reports@example.com", "SmtpPassword": "", "SmtpUseSsl": true, "SmtpFromAddress": "reports@example.com", "DiskPath": "Reports", "FileNamePattern": "{ReportName}_{Date}" },
      "Group": ""
    },
    {
      "Name": "QualityCompliance", "Enabled": true, "Format": "HTML", "Title": "Water Quality Compliance Report",
      "Description": "Detailed water quality analysis with compliance checks against regulatory limits for pH, chlorine, dissolved oxygen, and turbidity.",
      "PageSize": "A4", "Orientation": "Portrait",
      "Sections": [
        { "Id": "qc_hdr", "Type": "Header", "Title": "Water Quality Compliance Report" },
        { "Id": "qc_intro", "Type": "Text", "Content": "This report verifies that all treated water quality parameters remain within regulatory compliance limits." },
        { "Id": "qc_ph_chart", "Type": "Chart", "Title": "pH Level (Last 8 Hours)", "ChartType": "Line", "ChartVariablePaths": ["Treatment.pH"], "ChartTimeRangeMinutes": 480, "ChartMaxPoints": 500, "ChartWidth": 600, "ChartHeight": 250, "ChartShowLegend": false, "ChartShowGrid": true },
        { "Id": "qc_cl_chart", "Type": "Chart", "Title": "Residual Chlorine (Last 8 Hours)", "ChartType": "Line", "ChartVariablePaths": ["Treatment.Chlorine_ppm"], "ChartTimeRangeMinutes": 480, "ChartMaxPoints": 500, "ChartWidth": 600, "ChartHeight": 250, "ChartShowLegend": false, "ChartShowGrid": true },
        { "Id": "qc_do_chart", "Type": "Chart", "Title": "Dissolved Oxygen (Last 8 Hours)", "ChartType": "Area", "ChartVariablePaths": ["Treatment.DissolvedOxygen_mgL"], "ChartTimeRangeMinutes": 480, "ChartMaxPoints": 500, "ChartWidth": 600, "ChartHeight": 250, "ChartShowLegend": false, "ChartShowGrid": true },
        { "Id": "qc_turb_chart", "Type": "Chart", "Title": "Intake Turbidity (Last 8 Hours)", "ChartType": "Line", "ChartVariablePaths": ["Intake.Turbidity_NTU"], "ChartTimeRangeMinutes": 480, "ChartMaxPoints": 500, "ChartWidth": 600, "ChartHeight": 250, "ChartShowLegend": false, "ChartShowGrid": true },
        { "Id": "qc_tbl", "Type": "Table", "Title": "Quality Parameter Statistics (8h)", "TableVariablePaths": ["Treatment.pH","Treatment.Chlorine_ppm","Treatment.DissolvedOxygen_mgL","Treatment.FilterPressure_bar","Intake.Turbidity_NTU"], "TableShowStatistics": true, "TableTimeRangeMinutes": 480 },
        { "Id": "qc_current_ph", "Type": "Value", "ValueVariablePath": "Treatment.pH", "ValueLabel": "Current pH", "ValueFormat": "F2", "ValueUnit": "" },
        { "Id": "qc_current_cl", "Type": "Value", "ValueVariablePath": "Treatment.Chlorine_ppm", "ValueLabel": "Current Chlorine", "ValueFormat": "F2", "ValueUnit": "ppm" }
      ],
      "Delivery": { "Method": "Disk", "EmailRecipients": "", "EmailSubject": "", "SmtpServer": "", "SmtpPort": 587, "SmtpUser": "", "SmtpPassword": "", "SmtpUseSsl": true, "SmtpFromAddress": "", "DiskPath": "Reports/Quality", "FileNamePattern": "{ReportName}_{DateTime}" },
      "Group": ""
    },
    {
      "Name": "ShiftHandover", "Enabled": true, "Format": "HTML", "Title": "Shift Handover Report",
      "Description": "Quick snapshot of current plant status for shift changes.",
      "PageSize": "A4", "Orientation": "Portrait",
      "Sections": [
        { "Id": "sh_hdr", "Type": "Header", "Title": "Shift Handover \u2014 Plant Status Snapshot" },
        { "Id": "sh_intro", "Type": "Text", "Content": "Current plant status at time of shift change. Review all values and confirm equipment states before accepting handover." },
        { "Id": "sh_intake_flow", "Type": "Value", "ValueVariablePath": "Intake.FlowRate_m3h", "ValueLabel": "Intake Flow Rate", "ValueFormat": "F1", "ValueUnit": "m\u00b3/h" },
        { "Id": "sh_intake_press", "Type": "Value", "ValueVariablePath": "Intake.Pressure_bar", "ValueLabel": "Intake Pressure", "ValueFormat": "F2", "ValueUnit": "bar" },
        { "Id": "sh_intake_temp", "Type": "Value", "ValueVariablePath": "Intake.Temperature_C", "ValueLabel": "Intake Temperature", "ValueFormat": "F1", "ValueUnit": "\u00b0C" },
        { "Id": "sh_ph", "Type": "Value", "ValueVariablePath": "Treatment.pH", "ValueLabel": "Treatment pH", "ValueFormat": "F2", "ValueUnit": "" },
        { "Id": "sh_chlorine", "Type": "Value", "ValueVariablePath": "Treatment.Chlorine_ppm", "ValueLabel": "Residual Chlorine", "ValueFormat": "F2", "ValueUnit": "ppm" },
        { "Id": "sh_do", "Type": "Value", "ValueVariablePath": "Treatment.DissolvedOxygen_mgL", "ValueLabel": "Dissolved Oxygen", "ValueFormat": "F1", "ValueUnit": "mg/L" },
        { "Id": "sh_tank1", "Type": "Value", "ValueVariablePath": "Storage.Tank1_Level_pct", "ValueLabel": "Tank 1 Level", "ValueFormat": "F1", "ValueUnit": "%" },
        { "Id": "sh_tank2", "Type": "Value", "ValueVariablePath": "Storage.Tank2_Level_pct", "ValueLabel": "Tank 2 Level", "ValueFormat": "F1", "ValueUnit": "%" },
        { "Id": "sh_outflow", "Type": "Value", "ValueVariablePath": "Distribution.OutflowRate_m3h", "ValueLabel": "Distribution Flow", "ValueFormat": "F1", "ValueUnit": "m\u00b3/h" },
        { "Id": "sh_power", "Type": "Value", "ValueVariablePath": "Energy.TotalPower_kW", "ValueLabel": "Total Power", "ValueFormat": "F1", "ValueUnit": "kW" },
        { "Id": "sh_pb", "Type": "PageBreak" },
        { "Id": "sh_trend_chart", "Type": "Chart", "Title": "Last 4 Hours \u2014 Key Process Trends", "ChartType": "Line", "ChartVariablePaths": ["Intake.FlowRate_m3h","Treatment.pH","Storage.Tank1_Level_pct","Energy.TotalPower_kW"], "ChartTimeRangeMinutes": 240, "ChartMaxPoints": 300, "ChartWidth": 600, "ChartHeight": 300, "ChartShowLegend": true, "ChartShowGrid": true },
        { "Id": "sh_summary_tbl", "Type": "Table", "Title": "Shift Statistics (Last 8 Hours)", "TableVariablePaths": ["Intake.FlowRate_m3h","Intake.Pressure_bar","Treatment.pH","Treatment.Chlorine_ppm","Storage.Tank1_Level_pct","Storage.Tank2_Level_pct","Distribution.OutflowRate_m3h","Energy.TotalPower_kW"], "TableShowStatistics": true, "TableTimeRangeMinutes": 480 }
      ],
      "Delivery": { "Method": "Disk", "EmailRecipients": "", "EmailSubject": "", "SmtpServer": "", "SmtpPort": 587, "SmtpUser": "", "SmtpPassword": "", "SmtpUseSsl": true, "SmtpFromAddress": "", "DiskPath": "Reports/Shifts", "FileNamePattern": "ShiftHandover_{DateTime}" },
      "Group": ""
    }
  ],
  "Scripts": [
    { "Name": "SimulateProcess", "Enabled": true, "IntervalMs": 5000, "Code": "var rng = new Random();\nvar flow = 120.0 + (rng.NextDouble() - 0.5) * 10.0;\nvar pump1 = Globals.ReadBool(\"Intake.Pump1_Running\");\nvar pump2 = Globals.ReadBool(\"Intake.Pump2_Running\");\nif (!pump1 && !pump2) flow = 0;\nelse if (!pump1 || !pump2) flow *= 0.55;\nGlobals.Write(\"Intake.FlowRate_m3h\", Math.Round(flow, 1));\nGlobals.Write(\"Intake.Pressure_bar\", Math.Round(3.2 + (flow - 120) * 0.01 + (rng.NextDouble() - 0.5) * 0.2, 2));\nGlobals.Write(\"Intake.Temperature_C\", Math.Round(14.5 + Math.Sin(DateTime.Now.TimeOfDay.TotalHours / 24 * Math.PI * 2) * 2.0 + (rng.NextDouble() - 0.5) * 0.3, 1));\nGlobals.Write(\"Intake.Turbidity_NTU\", Math.Round(Math.Max(0.5, 2.8 + (rng.NextDouble() - 0.5) * 1.5), 1));\nGlobals.Write(\"Intake.Pump1_Power_kW\", pump1 ? Math.Round(15.5 + (rng.NextDouble() - 0.5) * 2.0, 1) : 0.0);\nvar dosing = Globals.ReadBool(\"Treatment.ChemicalDosing_Running\");\nvar rate = Globals.ReadDouble(\"Treatment.DosingRate_mlmin\");\nvar ph = Globals.ReadDouble(\"Treatment.pH\");\nvar phTarget = dosing ? 7.0 + rate * 0.004 : 6.8;\nph += (phTarget - ph) * 0.1 + (rng.NextDouble() - 0.5) * 0.05;\nGlobals.Write(\"Treatment.pH\", Math.Round(Math.Clamp(ph, 5.5, 9.0), 2));\nvar cl = Globals.ReadDouble(\"Treatment.Chlorine_ppm\");\nvar clTarget = dosing ? 0.8 + rate * 0.016 : 0.3;\ncl += (clTarget - cl) * 0.08 + (rng.NextDouble() - 0.5) * 0.05;\nGlobals.Write(\"Treatment.Chlorine_ppm\", Math.Round(Math.Clamp(cl, 0.0, 5.0), 2));\nGlobals.Write(\"Treatment.DissolvedOxygen_mgL\", Math.Round(8.2 + (rng.NextDouble() - 0.5) * 0.6, 1));\nGlobals.Write(\"Treatment.FilterPressure_bar\", Math.Round(1.8 + (rng.NextDouble() - 0.5) * 0.3, 2));\nvar t1 = Globals.ReadDouble(\"Storage.Tank1_Level_pct\");\nvar t2 = Globals.ReadDouble(\"Storage.Tank2_Level_pct\");\nvar inRate = flow / 300.0;\nvar valveOpen = Globals.ReadBool(\"Storage.OutletValve_Open\");\nvar outRate = valveOpen ? 95.0 / 300.0 : 0;\nt1 += (inRate * 0.6 - outRate * 0.5 + (rng.NextDouble() - 0.5) * 0.3);\nt2 += (inRate * 0.4 - outRate * 0.5 + (rng.NextDouble() - 0.5) * 0.3);\nGlobals.Write(\"Storage.Tank1_Level_pct\", Math.Round(Math.Clamp(t1, 5, 98), 1));\nGlobals.Write(\"Storage.Tank2_Level_pct\", Math.Round(Math.Clamp(t2, 5, 98), 1));\nGlobals.Write(\"Storage.Tank1_Temperature_C\", Math.Round(15.2 + (rng.NextDouble() - 0.5) * 0.4, 1));\nGlobals.Write(\"Storage.Tank2_Temperature_C\", Math.Round(14.8 + (rng.NextDouble() - 0.5) * 0.4, 1));\nvar outflow = valveOpen ? 95.0 + (rng.NextDouble() - 0.5) * 8.0 : 5.0;\nvar booster = Globals.ReadBool(\"Distribution.BoosterPump_Running\");\nGlobals.Write(\"Distribution.OutflowRate_m3h\", Math.Round(booster ? outflow : outflow * 0.3, 1));\nGlobals.Write(\"Distribution.Pressure_bar\", Math.Round(booster ? 4.1 + (rng.NextDouble() - 0.5) * 0.3 : 1.5, 2));\nGlobals.Write(\"Distribution.BoosterPump_Power_kW\", booster ? Math.Round(22.0 + (rng.NextDouble() - 0.5) * 3.0, 1) : 0.0);" },
    { "Name": "SimulateEnergy", "Enabled": true, "IntervalMs": 10000, "Code": "var power = 0.0;\nif (Globals.ReadBool(\"Intake.Pump1_Running\")) power += Globals.ReadDouble(\"Intake.Pump1_Power_kW\");\nif (Globals.ReadBool(\"Intake.Pump2_Running\")) power += 14.0;\nif (Globals.ReadBool(\"Treatment.ChemicalDosing_Running\")) power += 3.5;\nif (Globals.ReadBool(\"Distribution.BoosterPump_Running\")) power += Globals.ReadDouble(\"Distribution.BoosterPump_Power_kW\");\npower += 5.0;\npower += (new Random().NextDouble() - 0.5) * 2.0;\nGlobals.Write(\"Energy.TotalPower_kW\", Math.Round(Math.Max(0, power), 1));\nvar daily = Globals.ReadDouble(\"Energy.DailyEnergy_kWh\");\nGlobals.Write(\"Energy.DailyEnergy_kWh\", Math.Round(daily + power * 10.0 / 3600.0, 1));\nvar vol = Globals.ReadDouble(\"Energy.DailyWaterVolume_m3\");\nvar outflow = Globals.ReadDouble(\"Distribution.OutflowRate_m3h\");\nGlobals.Write(\"Energy.DailyWaterVolume_m3\", Math.Round(vol + outflow * 10.0 / 3600.0, 1));" }
  ],
  "Screens": [
    { "Name": "Dashboard", "Width": 1200, "Height": 800, "Background": "#0f172a", "LayoutMode": "responsive", "GridColumns": 12, "GridGap": 8, "Symbols": [
      { "Id": "title", "Type": "text", "X": 0, "Y": 0, "Width": 400, "Height": 45, "Label": "\ud83c\udf0a Water Treatment Plant \u2014 Report Demo", "Fill": "#e2e8f0", "FontSize": 20, "FontWeight": "bold", "ColSpan": 12, "RowSpan": 1, "RowHeight": 50, "Order": 0 },
      { "Id": "intake_flow_gauge", "Type": "gauge", "X": 0, "Y": 60, "Width": 200, "Height": 140, "Label": "Intake Flow", "Fill": "#3b82f6", "VariablePath": "Intake.FlowRate_m3h", "MinValue": 0, "MaxValue": 200, "GaugeStyle": "arc", "GaugeTicks": 5, "GaugeShowValue": true, "GaugeUnit": "m\u00b3/h", "GaugeValueFormat": "F1", "ColSpan": 3, "RowSpan": 1, "RowHeight": 180, "Order": 1 },
      { "Id": "ph_gauge", "Type": "gauge", "X": 0, "Y": 60, "Width": 200, "Height": 140, "Label": "pH Level", "Fill": "#10b981", "VariablePath": "Treatment.pH", "MinValue": 5.0, "MaxValue": 9.0, "GaugeStyle": "semicircle", "GaugeTicks": 8, "GaugeShowValue": true, "GaugeUnit": "", "GaugeValueFormat": "F2", "ColSpan": 3, "RowSpan": 1, "RowHeight": 180, "Order": 2 },
      { "Id": "tank1_gauge", "Type": "gauge", "X": 0, "Y": 60, "Width": 200, "Height": 140, "Label": "Tank 1", "Fill": "#06b6d4", "VariablePath": "Storage.Tank1_Level_pct", "MinValue": 0, "MaxValue": 100, "GaugeStyle": "vbar", "GaugeTicks": 5, "GaugeShowValue": true, "GaugeUnit": "%", "GaugeValueFormat": "F0", "ColSpan": 3, "RowSpan": 1, "RowHeight": 180, "Order": 3 },
      { "Id": "tank2_gauge", "Type": "gauge", "X": 0, "Y": 60, "Width": 200, "Height": 140, "Label": "Tank 2", "Fill": "#06b6d4", "VariablePath": "Storage.Tank2_Level_pct", "MinValue": 0, "MaxValue": 100, "GaugeStyle": "vbar", "GaugeTicks": 5, "GaugeShowValue": true, "GaugeUnit": "%", "GaugeValueFormat": "F0", "ColSpan": 3, "RowSpan": 1, "RowHeight": 180, "Order": 4 },
      { "Id": "power_gauge", "Type": "gauge", "X": 0, "Y": 260, "Width": 200, "Height": 140, "Label": "Total Power", "Fill": "#f59e0b", "VariablePath": "Energy.TotalPower_kW", "MinValue": 0, "MaxValue": 80, "GaugeStyle": "arc", "GaugeTicks": 8, "GaugeShowValue": true, "GaugeUnit": "kW", "GaugeValueFormat": "F1", "ColSpan": 3, "RowSpan": 1, "RowHeight": 180, "Order": 5 },
      { "Id": "chlorine_gauge", "Type": "gauge", "X": 0, "Y": 260, "Width": 200, "Height": 140, "Label": "Chlorine", "Fill": "#a855f7", "VariablePath": "Treatment.Chlorine_ppm", "MinValue": 0, "MaxValue": 4.0, "GaugeStyle": "semicircle", "GaugeTicks": 8, "GaugeShowValue": true, "GaugeUnit": "ppm", "GaugeValueFormat": "F2", "ColSpan": 3, "RowSpan": 1, "RowHeight": 180, "Order": 6 },
      { "Id": "outflow_gauge", "Type": "gauge", "X": 0, "Y": 260, "Width": 200, "Height": 140, "Label": "Distribution", "Fill": "#14b8a6", "VariablePath": "Distribution.OutflowRate_m3h", "MinValue": 0, "MaxValue": 150, "GaugeStyle": "arc", "GaugeTicks": 6, "GaugeShowValue": true, "GaugeUnit": "m\u00b3/h", "GaugeValueFormat": "F1", "ColSpan": 3, "RowSpan": 1, "RowHeight": 180, "Order": 7 },
      { "Id": "dist_pressure", "Type": "gauge", "X": 0, "Y": 260, "Width": 200, "Height": 140, "Label": "Pressure", "Fill": "#ef4444", "VariablePath": "Distribution.Pressure_bar", "MinValue": 0, "MaxValue": 6.0, "GaugeStyle": "needle", "GaugeTicks": 6, "GaugeShowValue": true, "GaugeUnit": "bar", "GaugeValueFormat": "F2", "ColSpan": 3, "RowSpan": 1, "RowHeight": 180, "Order": 8 },
      { "Id": "flow_trend", "Type": "trend", "X": 0, "Y": 460, "Width": 700, "Height": 300, "Label": "Live Flow Rates", "TrendPens": [{ "VariablePath": "Intake.FlowRate_m3h", "Color": "#3b82f6", "Width": 2, "Label": "Intake" }, { "VariablePath": "Distribution.OutflowRate_m3h", "Color": "#14b8a6", "Width": 2, "Label": "Distribution" }], "TrendTimeWindowSeconds": 120, "TrendShowGrid": true, "TrendShowCursor": true, "TrendShowLegend": true, "TrendYMin": 0, "TrendYMax": 200, "TrendBackground": "#111827", "ColSpan": 6, "RowSpan": 1, "RowHeight": 300, "Order": 9 },
      { "Id": "alarm_list", "Type": "alarmlist", "X": 0, "Y": 460, "Width": 500, "Height": 300, "Label": "Active Alarms", "ColSpan": 6, "RowSpan": 1, "RowHeight": 300, "Order": 10 },
      { "Id": "nav_reports", "Type": "button", "X": 0, "Y": 770, "Width": 140, "Height": 35, "Label": "\ud83d\udcca Reports", "ButtonStyle": "pill", "ButtonColor": "#8b5cf6", "ButtonTextColor": "#ffffff", "ButtonBorderRadius": 999, "ColSpan": 3, "RowSpan": 1, "RowHeight": 45, "Order": 11, "Commands": [{ "Id": "nav_r1", "Action": "NavigateScreen", "Trigger": "Click", "TargetScreen": "Reports" }] },
      { "Id": "nav_history", "Type": "button", "X": 0, "Y": 770, "Width": 140, "Height": 35, "Label": "\ud83d\udcc8 History", "ButtonStyle": "pill", "ButtonColor": "#0ea5e9", "ButtonTextColor": "#ffffff", "ButtonBorderRadius": 999, "ColSpan": 3, "RowSpan": 1, "RowHeight": 45, "Order": 12, "Commands": [{ "Id": "nav_h1", "Action": "NavigateScreen", "Trigger": "Click", "TargetScreen": "History" }] },
      { "Id": "nav_control", "Type": "button", "X": 0, "Y": 770, "Width": 140, "Height": 35, "Label": "\ud83d\udd27 Control", "ButtonStyle": "pill", "ButtonColor": "#64748b", "ButtonTextColor": "#ffffff", "ButtonBorderRadius": 999, "ColSpan": 3, "RowSpan": 1, "RowHeight": 45, "Order": 13, "Commands": [{ "Id": "nav_c1", "Action": "NavigateScreen", "Trigger": "Click", "TargetScreen": "Control" }] },
      { "Id": "nav_events", "Type": "button", "X": 0, "Y": 770, "Width": 140, "Height": 35, "Label": "\ud83d\udcdd Events", "ButtonStyle": "pill", "ButtonColor": "#64748b", "ButtonTextColor": "#ffffff", "ButtonBorderRadius": 999, "ColSpan": 3, "RowSpan": 1, "RowHeight": 45, "Order": 14, "Commands": [{ "Id": "nav_e1", "Action": "NavigateScreen", "Trigger": "Click", "TargetScreen": "Events" }] }
    ]},
    { "Name": "Reports", "Width": 1200, "Height": 800, "Background": "#0f172a", "LayoutMode": "responsive", "GridColumns": 12, "GridGap": 8, "Symbols": [
      { "Id": "rpt_title", "Type": "text", "X": 0, "Y": 0, "Width": 400, "Height": 40, "Label": "\ud83d\udcca Report Viewers", "Fill": "#e2e8f0", "FontSize": 18, "FontWeight": "bold", "ColSpan": 10, "RowSpan": 1, "RowHeight": 45, "Order": 0 },
      { "Id": "rpt_back", "Type": "button", "X": 0, "Y": 0, "Width": 100, "Height": 35, "Label": "\u2190 Dashboard", "ButtonStyle": "outline", "ButtonColor": "#64748b", "ButtonTextColor": "#94a3b8", "ButtonBorderRadius": 6, "ColSpan": 2, "RowSpan": 1, "RowHeight": 45, "Order": 1, "Commands": [{ "Id": "rb01", "Action": "NavigateScreen", "Trigger": "Click", "TargetScreen": "Dashboard" }] },
      { "Id": "rpt_gen_daily", "Type": "button", "X": 0, "Y": 50, "Width": 200, "Height": 40, "Label": "\u25b6 Generate Daily Report", "ButtonStyle": "raised", "ButtonColor": "#8b5cf6", "ButtonTextColor": "#ffffff", "ButtonBorderRadius": 8, "ColSpan": 4, "RowSpan": 1, "RowHeight": 50, "Order": 2, "Commands": [{ "Id": "gen_d1", "Action": "GenerateReport", "Trigger": "Click", "TargetReport": "DailyOperations" }] },
      { "Id": "rpt_gen_quality", "Type": "button", "X": 0, "Y": 50, "Width": 200, "Height": 40, "Label": "\u25b6 Generate Quality Report", "ButtonStyle": "raised", "ButtonColor": "#10b981", "ButtonTextColor": "#ffffff", "ButtonBorderRadius": 8, "ColSpan": 4, "RowSpan": 1, "RowHeight": 50, "Order": 3, "Commands": [{ "Id": "gen_q1", "Action": "GenerateReport", "Trigger": "Click", "TargetReport": "QualityCompliance" }] },
      { "Id": "rpt_gen_shift", "Type": "button", "X": 0, "Y": 50, "Width": 200, "Height": 40, "Label": "\u25b6 Generate Shift Report", "ButtonStyle": "raised", "ButtonColor": "#f59e0b", "ButtonTextColor": "#ffffff", "ButtonBorderRadius": 8, "ColSpan": 4, "RowSpan": 1, "RowHeight": 50, "Order": 4, "Commands": [{ "Id": "gen_s1", "Action": "GenerateReport", "Trigger": "Click", "TargetReport": "ShiftHandover" }] },
      { "Id": "rpt_viewer_daily", "Type": "reportviewer", "X": 0, "Y": 100, "Width": 600, "Height": 500, "Label": "Daily Operations Report", "ReportName": "DailyOperations", "ColSpan": 12, "RowSpan": 1, "RowHeight": 500, "Order": 5 },
      { "Id": "rpt_viewer_quality", "Type": "reportviewer", "X": 0, "Y": 610, "Width": 600, "Height": 500, "Label": "Quality Compliance Report", "ReportName": "QualityCompliance", "ColSpan": 12, "RowSpan": 1, "RowHeight": 500, "Order": 6 },
      { "Id": "rpt_viewer_shift", "Type": "reportviewer", "X": 0, "Y": 1120, "Width": 600, "Height": 400, "Label": "Shift Handover Report", "ReportName": "ShiftHandover", "ColSpan": 12, "RowSpan": 1, "RowHeight": 400, "Order": 7 }
    ]},
    { "Name": "History", "Width": 1200, "Height": 800, "Background": "#0f172a", "LayoutMode": "responsive", "GridColumns": 12, "GridGap": 8, "Symbols": [
      { "Id": "hist_title", "Type": "text", "X": 0, "Y": 0, "Width": 400, "Height": 40, "Label": "\ud83d\udcc8 Historical Data", "Fill": "#e2e8f0", "FontSize": 18, "FontWeight": "bold", "ColSpan": 10, "RowSpan": 1, "RowHeight": 45, "Order": 0 },
      { "Id": "hist_back", "Type": "button", "X": 0, "Y": 0, "Width": 100, "Height": 35, "Label": "\u2190 Dashboard", "ButtonStyle": "outline", "ButtonColor": "#64748b", "ButtonTextColor": "#94a3b8", "ButtonBorderRadius": 6, "ColSpan": 2, "RowSpan": 1, "RowHeight": 45, "Order": 1, "Commands": [{ "Id": "hb01", "Action": "NavigateScreen", "Trigger": "Click", "TargetScreen": "Dashboard" }] },
      { "Id": "hist_flow_chart", "Type": "hdachart", "X": 0, "Y": 50, "Width": 700, "Height": 350, "Label": "Flow Rates (24h)", "HdaVariablePaths": ["Intake.FlowRate_m3h","Distribution.OutflowRate_m3h"], "HdaTimeRangeMinutes": 1440, "HdaMaxPoints": 1000, "ColSpan": 6, "RowSpan": 1, "RowHeight": 380, "Order": 2 },
      { "Id": "hist_quality_chart", "Type": "hdachart", "X": 0, "Y": 50, "Width": 700, "Height": 350, "Label": "Water Quality (24h)", "HdaVariablePaths": ["Treatment.pH","Treatment.Chlorine_ppm","Treatment.DissolvedOxygen_mgL"], "HdaTimeRangeMinutes": 1440, "HdaMaxPoints": 1000, "ColSpan": 6, "RowSpan": 1, "RowHeight": 380, "Order": 3 },
      { "Id": "hist_tank_chart", "Type": "hdachart", "X": 0, "Y": 440, "Width": 700, "Height": 350, "Label": "Tank Levels (24h)", "HdaVariablePaths": ["Storage.Tank1_Level_pct","Storage.Tank2_Level_pct"], "HdaTimeRangeMinutes": 1440, "HdaMaxPoints": 1000, "ColSpan": 6, "RowSpan": 1, "RowHeight": 380, "Order": 4 },
      { "Id": "hist_power_chart", "Type": "hdachart", "X": 0, "Y": 440, "Width": 700, "Height": 350, "Label": "Power Consumption (24h)", "HdaVariablePaths": ["Energy.TotalPower_kW","Intake.Pump1_Power_kW","Distribution.BoosterPump_Power_kW"], "HdaTimeRangeMinutes": 1440, "HdaMaxPoints": 1000, "ColSpan": 6, "RowSpan": 1, "RowHeight": 380, "Order": 5 },
      { "Id": "hist_grid", "Type": "hdagrid", "X": 0, "Y": 830, "Width": 700, "Height": 300, "Label": "Process Data Grid", "HdaVariablePaths": ["Intake.FlowRate_m3h","Treatment.pH","Treatment.Chlorine_ppm","Storage.Tank1_Level_pct","Storage.Tank2_Level_pct","Energy.TotalPower_kW"], "HdaTimeRangeMinutes": 480, "HdaMaxPoints": 500, "ColSpan": 12, "RowSpan": 1, "RowHeight": 320, "Order": 6 }
    ]},
    { "Name": "Control", "Width": 1200, "Height": 800, "Background": "#0f172a", "LayoutMode": "responsive", "GridColumns": 12, "GridGap": 8, "Symbols": [
      { "Id": "ctrl_title", "Type": "text", "X": 0, "Y": 0, "Width": 400, "Height": 40, "Label": "\ud83d\udd27 Manual Control", "Fill": "#e2e8f0", "FontSize": 18, "FontWeight": "bold", "ColSpan": 10, "RowSpan": 1, "RowHeight": 45, "Order": 0 },
      { "Id": "ctrl_back", "Type": "button", "X": 0, "Y": 0, "Width": 100, "Height": 35, "Label": "\u2190 Dashboard", "ButtonStyle": "outline", "ButtonColor": "#64748b", "ButtonTextColor": "#94a3b8", "ButtonBorderRadius": 6, "ColSpan": 2, "RowSpan": 1, "RowHeight": 45, "Order": 1, "Commands": [{ "Id": "cb01", "Action": "NavigateScreen", "Trigger": "Click", "TargetScreen": "Dashboard" }] },
      { "Id": "ctrl_pump1_lbl", "Type": "text", "X": 0, "Y": 50, "Width": 150, "Height": 30, "Label": "Intake Pump 1", "Fill": "#94a3b8", "FontSize": 14, "ColSpan": 2, "RowSpan": 1, "RowHeight": 60, "Order": 2 },
      { "Id": "ctrl_pump1", "Type": "switch", "X": 0, "Y": 50, "Width": 80, "Height": 40, "VariablePath": "Intake.Pump1_Running", "SwitchStyle": "apple", "SwitchOnColor": "#3b82f6", "SwitchOffColor": "#64748b", "ColSpan": 2, "RowSpan": 1, "RowHeight": 60, "Order": 3 },
      { "Id": "ctrl_pump2_lbl", "Type": "text", "X": 0, "Y": 50, "Width": 150, "Height": 30, "Label": "Intake Pump 2", "Fill": "#94a3b8", "FontSize": 14, "ColSpan": 2, "RowSpan": 1, "RowHeight": 60, "Order": 4 },
      { "Id": "ctrl_pump2", "Type": "switch", "X": 0, "Y": 50, "Width": 80, "Height": 40, "VariablePath": "Intake.Pump2_Running", "SwitchStyle": "apple", "SwitchOnColor": "#3b82f6", "SwitchOffColor": "#64748b", "ColSpan": 2, "RowSpan": 1, "RowHeight": 60, "Order": 5 },
      { "Id": "ctrl_dosing_lbl", "Type": "text", "X": 0, "Y": 50, "Width": 150, "Height": 30, "Label": "Chemical Dosing", "Fill": "#94a3b8", "FontSize": 14, "ColSpan": 2, "RowSpan": 1, "RowHeight": 60, "Order": 6 },
      { "Id": "ctrl_dosing", "Type": "switch", "X": 0, "Y": 50, "Width": 80, "Height": 40, "VariablePath": "Treatment.ChemicalDosing_Running", "SwitchStyle": "apple", "SwitchOnColor": "#10b981", "SwitchOffColor": "#64748b", "ColSpan": 2, "RowSpan": 1, "RowHeight": 60, "Order": 7 },
      { "Id": "ctrl_doserate_lbl", "Type": "text", "X": 0, "Y": 120, "Width": 200, "Height": 30, "Label": "Dosing Rate (ml/min)", "Fill": "#94a3b8", "FontSize": 14, "ColSpan": 3, "RowSpan": 1, "RowHeight": 80, "Order": 8 },
      { "Id": "ctrl_doserate", "Type": "hslider", "X": 0, "Y": 120, "Width": 300, "Height": 50, "VariablePath": "Treatment.DosingRate_mlmin", "MinValue": 0, "MaxValue": 100, "SliderTrackColor": "#334155", "SliderFillColor": "#10b981", "SliderThumbColor": "#ffffff", "SliderShowValue": true, "SliderShowTicks": true, "ColSpan": 5, "RowSpan": 1, "RowHeight": 80, "Order": 9 },
      { "Id": "ctrl_doserate_val", "Type": "editbox", "X": 0, "Y": 120, "Width": 120, "Height": 40, "Label": "ml/min", "VariablePath": "Treatment.DosingRate_mlmin", "EditBoxStep": 1, "EditBoxFormat": "F0", "ColSpan": 2, "RowSpan": 1, "RowHeight": 80, "Order": 10 },
      { "Id": "ctrl_booster_lbl", "Type": "text", "X": 0, "Y": 210, "Width": 200, "Height": 30, "Label": "Booster Pump", "Fill": "#94a3b8", "FontSize": 14, "ColSpan": 2, "RowSpan": 1, "RowHeight": 60, "Order": 11 },
      { "Id": "ctrl_booster", "Type": "switch", "X": 0, "Y": 210, "Width": 80, "Height": 40, "VariablePath": "Distribution.BoosterPump_Running", "SwitchStyle": "apple", "SwitchOnColor": "#14b8a6", "SwitchOffColor": "#64748b", "ColSpan": 2, "RowSpan": 1, "RowHeight": 60, "Order": 12 },
      { "Id": "ctrl_valve_lbl", "Type": "text", "X": 0, "Y": 210, "Width": 200, "Height": 30, "Label": "Outlet Valve", "Fill": "#94a3b8", "FontSize": 14, "ColSpan": 2, "RowSpan": 1, "RowHeight": 60, "Order": 13 },
      { "Id": "ctrl_valve", "Type": "switch", "X": 0, "Y": 210, "Width": 80, "Height": 40, "VariablePath": "Storage.OutletValve_Open", "SwitchStyle": "apple", "SwitchOnColor": "#06b6d4", "SwitchOffColor": "#64748b", "ColSpan": 2, "RowSpan": 1, "RowHeight": 60, "Order": 14 },
      { "Id": "ctrl_ph_trend", "Type": "trend", "X": 0, "Y": 280, "Width": 500, "Height": 250, "Label": "pH & Chlorine (Live)", "TrendPens": [{ "VariablePath": "Treatment.pH", "Color": "#10b981", "Width": 2, "Label": "pH" }, { "VariablePath": "Treatment.Chlorine_ppm", "Color": "#a855f7", "Width": 2, "Label": "Cl ppm" }], "TrendTimeWindowSeconds": 180, "TrendShowGrid": true, "TrendShowCursor": true, "TrendShowLegend": true, "TrendBackground": "#111827", "ColSpan": 6, "RowSpan": 1, "RowHeight": 280, "Order": 15 },
      { "Id": "ctrl_tank_trend", "Type": "trend", "X": 0, "Y": 280, "Width": 500, "Height": 250, "Label": "Tank Levels (Live)", "TrendPens": [{ "VariablePath": "Storage.Tank1_Level_pct", "Color": "#06b6d4", "Width": 2, "Label": "Tank 1 %" }, { "VariablePath": "Storage.Tank2_Level_pct", "Color": "#0ea5e9", "Width": 2, "Label": "Tank 2 %" }], "TrendTimeWindowSeconds": 180, "TrendShowGrid": true, "TrendShowCursor": true, "TrendShowLegend": true, "TrendYMin": 0, "TrendYMax": 100, "TrendBackground": "#111827", "ColSpan": 6, "RowSpan": 1, "RowHeight": 280, "Order": 16 }
    ]},
    { "Name": "Events", "Width": 1200, "Height": 800, "Background": "#0f172a", "LayoutMode": "responsive", "GridColumns": 12, "GridGap": 8, "Symbols": [
      { "Id": "evt_title", "Type": "text", "X": 0, "Y": 0, "Width": 400, "Height": 40, "Label": "\ud83d\udcdd Event Log", "Fill": "#e2e8f0", "FontSize": 18, "FontWeight": "bold", "ColSpan": 10, "RowSpan": 1, "RowHeight": 45, "Order": 0 },
      { "Id": "evt_back", "Type": "button", "X": 0, "Y": 0, "Width": 100, "Height": 35, "Label": "\u2190 Dashboard", "ButtonStyle": "outline", "ButtonColor": "#64748b", "ButtonTextColor": "#94a3b8", "ButtonBorderRadius": 6, "ColSpan": 2, "RowSpan": 1, "RowHeight": 45, "Order": 1, "Commands": [{ "Id": "eb01", "Action": "NavigateScreen", "Trigger": "Click", "TargetScreen": "Dashboard" }] },
      { "Id": "evt_log", "Type": "eventlog", "X": 0, "Y": 50, "Width": 900, "Height": 700, "Label": "Event Log", "EventLogMaxRows": 500, "EventLogTimeRangeMinutes": 4320, "ColSpan": 12, "RowSpan": 1, "RowHeight": 700, "Order": 2 }
    ]}
  ],
  "Strings": [{ "Id": "app_title", "Values": {} }, { "Id": "intake", "Values": {} }, { "Id": "treatment", "Values": {} }, { "Id": "storage", "Values": {} }, { "Id": "distribution", "Values": {} }],
  "Images": [],
  "PlcPrograms": [],
  "Recipes": [],
  "Cameras": []
}
'@

[System.IO.File]::WriteAllText("C:\Work\samples\ReportDemo\nodes.json", $json, [System.Text.Encoding]::UTF8)
Write-Host "nodes.json created successfully"
