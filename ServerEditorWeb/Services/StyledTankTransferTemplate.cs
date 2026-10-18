// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

namespace ServerEditorWeb.Services;

public static class StyledTankTransferTemplate
{
    public const string Json = """
        {
          "Folder": {
            "Name": "__TASK__",
            "Folders": [{
              "Name": "Transfer",
              "Folders": [],
              "Variables": [
                { "Name": "SourceLevel", "Type": "Double", "Access": "ReadWrite", "Value": 100, "InitialValue": "100", "Description": "Source tank fill percentage; simulation-owned." },
                { "Name": "DestinationLevel", "Type": "Double", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "Description": "Receiving tank fill percentage; simulation-owned." },
                { "Name": "RunRequested", "Type": "Boolean", "Access": "ReadWrite", "Value": false, "InitialValue": "false", "Description": "Start/pause request from the screen." },
                { "Name": "RefillRequested", "Type": "Boolean", "Access": "ReadWrite", "Value": false, "InitialValue": "false", "Description": "Refill source, empty destination, and restart request." },
                { "Name": "PumpOn", "Type": "Boolean", "Access": "ReadWrite", "Value": false, "InitialValue": "false", "Description": "Actual pump and pipe flow state; simulation-owned." },
                { "Name": "Status", "Type": "String", "Access": "ReadWrite", "Value": "Ready - press Start / Stop", "InitialValue": "Ready - press Start / Stop", "Description": "Transfer simulation state; simulation-owned." }
              ]
            }],
            "Variables": []
          },
          "Scripts": [{
            "Name": "__TASK___TankTransferSimulation",
            "Language": "CSharp",
            "Enabled": false,
            "IntervalMs": 250,
            "Code": "const string p = \"__TASK__.Transfer.\";\nif (ReadBool(p + \"RefillRequested\"))\n{\n    Write(p + \"PumpOn\", false);\n    Write(p + \"SourceLevel\", 100.0);\n    Write(p + \"DestinationLevel\", 0.0);\n    Write(p + \"RunRequested\", true);\n    Write(p + \"RefillRequested\", false);\n    Write(p + \"Status\", \"Refilled - restarting transfer\");\n}\nelse\n{\n    var source = ReadDouble(p + \"SourceLevel\");\n    var destination = ReadDouble(p + \"DestinationLevel\");\n    var requested = ReadBool(p + \"RunRequested\");\n    if (!double.IsFinite(source) || !double.IsFinite(destination) || source < 0 || source > 100 || destination < 0 || destination > 100 || Math.Abs(source + destination - 100) > 0.0001)\n    {\n        Write(p + \"PumpOn\", false);\n        Write(p + \"RunRequested\", false);\n        Write(p + \"Status\", \"Invalid levels - press Refill & Restart\");\n    }\n    else if (source <= 0 || destination >= 100)\n    {\n        Write(p + \"PumpOn\", false);\n        Write(p + \"RunRequested\", false);\n        Write(p + \"Status\", \"Transfer complete - press Refill & Restart\");\n    }\n    else if (!requested)\n    {\n        Write(p + \"PumpOn\", false);\n        Write(p + \"Status\", source >= 100 ? \"Ready - press Start / Stop\" : \"Paused - press Start / Stop to resume\");\n    }\n    else\n    {\n        var amount = Math.Min(1.0, Math.Min(source, 100.0 - destination));\n        source -= amount;\n        destination += amount;\n        Write(p + \"SourceLevel\", source);\n        Write(p + \"DestinationLevel\", destination);\n        var active = source > 0 && destination < 100;\n        Write(p + \"PumpOn\", active);\n        if (!active) Write(p + \"RunRequested\", false);\n        Write(p + \"Status\", active ? \"Transferring - 4% per second\" : \"Transfer complete - press Refill & Restart\");\n    }\n}"
          }],
          "Screens": [{
            "Name": "__TASK___Tank Transfer",
            "Width": 1280,
            "Height": 820,
            "Background": "#101827",
            "LayoutMode": "svg",
            "ShowInNavigation": true,
            "FitMode": "contain",
            "NavigationTransition": "fade",
            "NavigationDurationMs": 320,
            "Symbols": [
              {
                "Id": "transfer_title", "Type": "text", "X": 40, "Y": 24, "Width": 1200, "Height": 40,
                "Stroke": "none", "StrokeWidth": 0,
                "Label": "TANK TRANSFER | Isometric equipment + live filling", "Fill": "#e2e8f0", "FontSize": 26, "FontFamily": "sans-serif"
              },
              {
                "Id": "transfer_subtitle", "Type": "text", "X": 40, "Y": 78, "Width": 1200, "Height": 30,
                "Stroke": "none", "StrokeWidth": 0,
                "Label": "Start / Stop pauses and resumes. Refill & Restart resets both tanks and starts a new transfer.", "Fill": "#94a3b8", "FontSize": 16, "FontFamily": "sans-serif"
              },
              {
                "Id": "transfer_source_title", "Type": "text", "X": 100, "Y": 140, "Width": 300, "Height": 32,
                "Stroke": "none", "StrokeWidth": 0,
                "Label": "TANK 1 / SOURCE", "Fill": "#e2e8f0", "FontSize": 20, "FontFamily": "sans-serif"
              },
              {
                "Id": "transfer_destination_title", "Type": "text", "X": 940, "Y": 140, "Width": 300, "Height": 32,
                "Stroke": "none", "StrokeWidth": 0,
                "Label": "TANK 2 / RECEIVER", "Fill": "#e2e8f0", "FontSize": 20, "FontFamily": "sans-serif"
              },
              {
                "Id": "transfer_inlet", "Type": "pipe", "X": 220, "Y": 413.333333, "Width": 340, "Height": 33.333334,
                "Stroke": "none", "StrokeWidth": 0,
                "PipeOrientation": "horizontal", "PipeUseGradient": true, "PipeColor": "#64748b", "PipeFlowColor": "#67e8f9",
                "PipeShowFlow": true, "PipeAnimate": true, "PipeFlowSpeed": 0.65, "PipeFlowDirection": "left-right", "VariablePath": "__TASK__.Transfer.PumpOn"
              },
              {
                "Id": "transfer_outlet", "Type": "pipe", "X": 720, "Y": 413.333333, "Width": 340, "Height": 33.333334,
                "Stroke": "none", "StrokeWidth": 0,
                "PipeOrientation": "horizontal", "PipeUseGradient": true, "PipeColor": "#64748b", "PipeFlowColor": "#67e8f9",
                "PipeShowFlow": true, "PipeAnimate": true, "PipeFlowSpeed": 0.65, "PipeFlowDirection": "left-right", "VariablePath": "__TASK__.Transfer.PumpOn"
              },
              {
                "Id": "transfer_source", "Type": "tank", "X": 100, "Y": 180, "Width": 240, "Height": 320,
                "Stroke": "none", "StrokeWidth": 0,
                "TankStyle": "isometric-cone", "TankUseGradient": true, "TankFillStyle": "gradient", "TankBodyColor": "#475569",
                "TankLiquidColor": "#0369a1", "TankGradientEndColor": "#67e8f9", "TankShowLevel": true,
                "MinValue": 0, "MaxValue": 100, "VariablePath": "__TASK__.Transfer.SourceLevel"
              },
              {
                "Id": "transfer_destination", "Type": "tank", "X": 940, "Y": 180, "Width": 240, "Height": 320,
                "Stroke": "none", "StrokeWidth": 0,
                "TankStyle": "isometric-cone", "TankUseGradient": true, "TankFillStyle": "gradient", "TankBodyColor": "#475569",
                "TankLiquidColor": "#0d9488", "TankGradientEndColor": "#a7f3d0", "TankShowLevel": true,
                "MinValue": 0, "MaxValue": 100, "VariablePath": "__TASK__.Transfer.DestinationLevel"
              },
              {
                "Id": "transfer_pump", "Type": "motorcontrol", "X": 560, "Y": 370, "Width": 160, "Height": 120,
                "Stroke": "none", "StrokeWidth": 0,
                "MotorStyle": "pump", "MotorAppearance": "isometric", "MotorUseGradient": true, "MotorBodyColor": "#64748b",
                "MotorRunColor": "#22d3ee", "MotorStopColor": "#94a3b8", "MotorFaultColor": "#ef4444", "MotorAnimateRotation": true,
                "VariablePath": "__TASK__.Transfer.PumpOn"
              },
              {
                "Id": "transfer_pump_title", "Type": "text", "X": 520, "Y": 315, "Width": 260, "Height": 32,
                "Stroke": "none", "StrokeWidth": 0,
                "Label": "TRANSFER PUMP P-101", "Fill": "#94a3b8", "FontSize": 18, "FontFamily": "sans-serif"
              },
              {
                "Id": "transfer_direction", "Type": "text", "X": 450, "Y": 510, "Width": 420, "Height": 28,
                "Stroke": "none", "StrokeWidth": 0,
                "Label": "SOURCE  >  PUMP  >  RECEIVER", "Fill": "#67e8f9", "FontSize": 16, "FontFamily": "sans-serif"
              },
              {
                "Id": "transfer_status_title", "Type": "text", "X": 40, "Y": 570, "Width": 120, "Height": 32,
                "Stroke": "none", "StrokeWidth": 0,
                "Label": "STATUS", "Fill": "#94a3b8", "FontSize": 16, "FontFamily": "sans-serif"
              },
              {
                "Id": "transfer_status", "Type": "numericdisplay", "X": 170, "Y": 556, "Width": 1050, "Height": 64,
                "Stroke": "none", "StrokeWidth": 0,
                "VariablePath": "__TASK__.Transfer.Status", "DisplayBackgroundStyle": "midnight", "DisplayFontStyle": "terminal",
                "DisplayTextColor": "#e2e8f0", "DisplayAutoFit": true, "NumericDisplayFormat": "", "FontSize": 24, "Fill": "#e2e8f0"
              },
              {
                "Id": "transfer_toggle", "Type": "button", "X": 260, "Y": 650, "Width": 340, "Height": 58,
                "Stroke": "none", "StrokeWidth": 0,
                "Label": "Start / Stop", "ButtonStyle": "raised", "ButtonColor": "#0369a1", "ButtonTextColor": "#ffffff", "ButtonBorderRadius": 10, "FontSize": 20,
                "Commands": [{ "Id": "transfer_cmd_toggle", "Action": "ToggleVariable", "Trigger": "Click", "VariablePath": "__TASK__.Transfer.RunRequested", "Value": "" }]
              },
              {
                "Id": "transfer_refill", "Type": "button", "X": 650, "Y": 650, "Width": 340, "Height": 58,
                "Stroke": "none", "StrokeWidth": 0,
                "Label": "Refill & Restart", "ButtonStyle": "raised", "ButtonColor": "#0f766e", "ButtonTextColor": "#ffffff", "ButtonBorderRadius": 10, "FontSize": 20,
                "Commands": [{ "Id": "transfer_cmd_refill", "Action": "WriteVariable", "Trigger": "Click", "VariablePath": "__TASK__.Transfer.RefillRequested", "Value": "true" }]
              },
              {
                "Id": "transfer_note", "Type": "text", "X": 40, "Y": 750, "Width": 1200, "Height": 26,
                "Stroke": "none", "StrokeWidth": 0,
                "Label": "Source + receiver levels = 100%. Automatic stop at empty/full. Simulation only - no physical devices.", "Fill": "#94a3b8", "FontSize": 16, "FontFamily": "sans-serif"
              }
            ]
          }]
        }
        """;
}
