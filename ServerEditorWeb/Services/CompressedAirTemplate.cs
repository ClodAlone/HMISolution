// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

using System.Text.Json.Nodes;

namespace ServerEditorWeb.Services;

public static class CompressedAirTemplate
{
    public static string Json { get; } = CreateJson();

    public const string SimulationCode = """
        const string p = "__TASK__.CompressedAir.";
        const double dt = 0.05; // Simulated minutes per 250 ms invocation.
        const double volume = 2.0; // Receiver m3; isothermal normal-air balance at 1 bar reference.
        var reset = ReadBool(p + "ResetRequested");
        if (reset)
        {
            Write(p + "ResetRequested", false);
            Write(p + "RunRequested", false);
            Write(p + "LeakRequested", false);
            Write(p + "FaultRequested", false);
            Write(p + "PeakRequested", false);
            Write(p + "Stage1Demand", false);
            Write(p + "Stage2Demand", false);
            Write(p + "PressureBar", 7.2);
            Write(p + "Minute", 0.0);
            Write(p + "EnergyKwh", 0.0);
            Write(p + "DeliveredNm3", 0.0);
            Write(p + "LeakedNm3", 0.0);
        }
        var pressure = ReadDouble(p + "PressureBar");
        var minute = ReadDouble(p + "Minute");
        var energy = ReadDouble(p + "EnergyKwh");
        var delivered = ReadDouble(p + "DeliveredNm3");
        var leaked = ReadDouble(p + "LeakedNm3");
        var valid = double.IsFinite(pressure) && pressure >= 0 && pressure <= 8.6 &&
            double.IsFinite(minute) && minute >= 0 && double.IsFinite(energy) && energy >= 0 &&
            double.IsFinite(delivered) && delivered >= 0 && double.IsFinite(leaked) && leaked >= 0;
        if (!valid) Write(p + "RunRequested", false);
        var running = valid && ReadBool(p + "RunRequested");
        var fault = ReadBool(p + "FaultRequested");
        var leak = ReadBool(p + "LeakRequested");
        var peak = ReadBool(p + "PeakRequested");
        var stage1 = ReadBool(p + "Stage1Demand");
        var stage2 = ReadBool(p + "Stage2Demand");
        double inlet = 0, load = 0, leakFlow = 0, vent = 0, unmet = 0, power = 0;
        var c1 = false; var c2 = false;
        if (running)
        {
            if (pressure <= 6.6) stage1 = true;
            else if (pressure >= 8.0) stage1 = false;
            if (pressure <= 6.1) stage2 = true;
            else if (pressure >= 7.4) stage2 = false;
            c1 = stage1 && !fault;
            c2 = stage2;
            inlet = (c1 ? 1.8 : 0) + (c2 ? 1.8 : 0);
            var requested = peak ? 2.8 : 1.0;
            var wantedLeak = leak ? 0.9 * Math.Sqrt(pressure / 7.2) : 0;
            var availableRate = pressure * volume / dt + inlet;
            var fraction = Math.Min(1, availableRate / (requested + wantedLeak));
            load = requested * fraction;
            leakFlow = wantedLeak * fraction;
            unmet = requested - load;
            var next = pressure + (inlet - load - leakFlow) * dt / volume;
            if (next > 8.6) vent = (next - 8.6) * volume / dt;
            pressure = Math.Clamp(next - vent * dt / volume, 0, 8.6);
            power = (c1 ? 11 : 0) + (c2 ? 11 : 0);
            minute += dt;
            energy += power * dt / 60;
            delivered += load * dt;
            leaked += leakFlow * dt;
            Write(p + "PressureBar", pressure);
            Write(p + "Minute", minute);
            Write(p + "EnergyKwh", energy);
            Write(p + "DeliveredNm3", delivered);
            Write(p + "LeakedNm3", leaked);
            Write(p + "Stage1Demand", stage1);
            Write(p + "Stage2Demand", stage2);
        }
        Write(p + "Compressor1On", c1);
        Write(p + "Compressor2On", c2);
        Write(p + "Compressor1Flow", c1 ? 1.8 : 0.0);
        Write(p + "Compressor2Flow", c2 ? 1.8 : 0.0);
        Write(p + "InletFlow", inlet);
        Write(p + "LoadFlow", load);
        Write(p + "LeakFlow", leakFlow);
        Write(p + "VentFlow", vent);
        Write(p + "UnmetFlow", unmet);
        Write(p + "PowerKw", power);
        Write(p + "LoadText", peak ? "PEAK / 2.8 Nm3/min" : "NORMAL / 1.0 Nm3/min");
        Write(p + "LeakText", leak ? "LEAK OPEN" : "LEAK SEALED");
        Write(p + "Status", !valid ? "Invalid inputs - press Reset" : reset ? "Reset - press Start / Pause" :
            !running ? "Paused - pressure / totals held" : unmet > 0.000001 ? "LOW PRESSURE - air demand not fully served" :
            fault ? "C1 FAULT - standby compressor available" : leak ? "Leak detected - increased compressor duty" :
            c2 ? "Two-stage pressure control - standby compressor assisting" : c1 ? "Lead compressor charging receiver" : "Receiver supplying demand - compressors idle");
        """;

    private static string CreateJson()
    {
        var b = new DemoScreenBuilder("CompressedAir", "Compressed-Air Station", "CompressedAirSimulation");
        foreach (var name in new[] { "RunRequested", "ResetRequested", "LeakRequested", "FaultRequested", "PeakRequested", "Stage1Demand", "Stage2Demand", "Compressor1On", "Compressor2On" }) b.Variable(name, "Boolean", false);
        b.Variable("PressureBar", "Double", 7.2, "bar(g)");
        foreach (var name in new[] { "Minute", "EnergyKwh", "DeliveredNm3", "LeakedNm3", "Compressor1Flow", "Compressor2Flow", "InletFlow", "LoadFlow", "LeakFlow", "VentFlow", "UnmetFlow", "PowerKw" })
            b.Variable(name, "Double", 0, name == "Minute" ? "min" : name == "EnergyKwh" ? "kWh" : name.EndsWith("Nm3", StringComparison.Ordinal) ? "Nm3" : name == "PowerKw" ? "kW" : "Nm3/min");
        b.Variable("LoadText", "String", "NORMAL / 1.0 Nm3/min");
        b.Variable("LeakText", "String", "LEAK SEALED");
        b.Variable("Status", "String", "Paused - press Start / Pause");
        b.Header("PNEUMATICS / AIR UTILITY", "STAGED COMPRESSORS / 2 m3 RECEIVER / ISOTHERMAL AIR BALANCE", "RECEIVER PRESSURE", "STATION INPUT", "PLANT AIR DELIVERY", "SIMULATED LEAK");
        b.Pipe("c1_pipe", 327, 379, 415, 26, "Compressor1Flow", "#22d3ee");
        b.Pipe("c2_pipe", 327, 584, 415, 26, "Compressor2Flow", "#a78bfa");
        b.Pipe("plant_pipe", 815, 420, 365, 26, "LoadFlow", "#34d399");
        b.Pipe("leak_branch", 1000, 433, 26, 245, "LeakFlow", "#fb923c", "top-bottom");
        b.Art("equipment", 0, 0, 1440, 1040, """
            <svg viewBox="0 0 1440 1040"><defs>
              <linearGradient id="__TASK___air_case"><stop stop-color="#506a80"/><stop offset="0.35" stop-color="#b7cbd7"/><stop offset="1" stop-color="#304b60"/></linearGradient>
              <linearGradient id="__TASK___air_receiver"><stop stop-color="#24445a"/><stop offset="0.35" stop-color="#b0d3df"/><stop offset="0.65" stop-color="#6899ae"/><stop offset="1" stop-color="#244559"/></linearGradient>
              <pattern id="__TASK___air_grille" width="7" height="8" patternUnits="userSpaceOnUse"><path d="M0 0H7M0 4H7" stroke="#244457" stroke-width="2"/></pattern>
            </defs><g font-family="Segoe UI,Arial,sans-serif" fill="#e2e8f0">
              <g font-size="15" font-weight="600"><text x="50" y="267">01 / LEAD + STANDBY COMPRESSORS</text><text x="646" y="267">02 / AIR RECEIVER</text><text x="1100" y="267">03 / PNEUMATIC LOAD</text></g>
              <g font-size="12" fill="#7db3cf"><text x="50" y="289">C1: 6.6 / 8.0 bar | C2: 6.1 / 7.4 bar</text><text x="646" y="289">Pressure, not liquid level</text><text x="1100" y="289">Normal / peak consumption</text></g>
              <g id="__TASK___air_compressor"><ellipse cx="206" cy="477" rx="153" ry="13" fill="#040b16"/><path d="M65 338L91 318H355V446L329 466H65Z" fill="#506d82" stroke="#84a4b7"/><rect x="65" y="338" width="264" height="128" rx="8" fill="url(#__TASK___air_case)"/><rect x="82" y="355" width="118" height="89" rx="5" fill="#173044"/><rect x="92" y="366" width="98" height="69" fill="url(#__TASK___air_grille)"/><rect x="218" y="351" width="92" height="29" rx="4" fill="#102638"/><text x="228" y="371" font-size="12" fill="#67e8f9">ROTARY AIR</text><path d="M84 466V478M307 466V478" stroke="#5c7b91" stroke-width="10"/></g>
              <use href="#__TASK___air_compressor" transform="translate(0 205)"/>
              <ellipse cx="758" cy="688" rx="116" ry="15" fill="#040b16"/><path d="M653 349V649C653 684 863 684 863 649V349Z" fill="url(#__TASK___air_receiver)" stroke="#97bacc"/><ellipse cx="758" cy="349" rx="105" ry="31" fill="#98baca" stroke="#c1dce7"/><path d="M676 385V623" stroke="#d8edf5" stroke-width="5" opacity="0.3"/>
              <rect x="700" y="570" width="116" height="55" rx="8" fill="#1e4055" stroke="#6b97ac"/><text x="758" y="593" text-anchor="middle" font-size="13" fill="#c0dce8">2 m3 / 8.6 bar</text><text x="758" y="613" text-anchor="middle" font-size="11" fill="#7db3cf">simulated relief</text><path d="M685 677V695M832 677V695" stroke="#496c82" stroke-width="10"/>
              <path d="M1102 381L1160 345V378L1220 342V378L1280 342V380L1370 348V590H1102Z" fill="#27465b" stroke="#6995ad"/><path d="M1102 590L1160 619H1390V380L1370 348V590Z" fill="#19354b" stroke="#4c748c"/>
              <g fill="#64c6df"><rect x="1123" y="418" width="45" height="40"/><rect x="1185" y="418" width="45" height="40"/><rect x="1247" y="418" width="45" height="40"/><rect x="1309" y="418" width="40" height="40"/></g><rect x="1195" y="509" width="78" height="81" fill="#122b3c"/><path d="M1025 677H990M1000 665L1013 680L1000 695" fill="none" stroke="#fb923c" stroke-width="4"/>
              <g fill="#7db3cf" font-size="12"><text x="390" y="354">C1 / 1.8 Nm3/min</text><text x="390" y="562">C2 / 1.8 Nm3/min</text><text x="1090" y="654">AIR DELIVERED / Nm3</text><text x="943" y="716">LEAK TO ATMOSPHERE</text></g>
            </g></svg>
            """);
        for (var i = 1; i <= 2; i++) b.Symbol("motor" + i, "motorcontrol", 226, i == 1 ? 388 : 593, 88, 62, new JsonObject
        {
            ["VariablePath"] = b.Path + "Compressor" + i + "On", ["MotorStyle"] = "motor", ["MotorUseGradient"] = true,
            ["MotorRunColor"] = i == 1 ? "#22d3ee" : "#a78bfa", ["MotorFaultPath"] = i == 1 ? b.Path + "FaultRequested" : ""
        });
        b.Symbol("pressure_dial", "gauge", 675, 386, 166, 158, new JsonObject
        {
            ["VariablePath"] = b.Path + "PressureBar", ["MinValue"] = 0, ["MaxValue"] = 10, ["GaugeStyle"] = "arc", ["GaugeFillStyle"] = "gradient",
            ["GaugeGradientEndColor"] = "#22d3ee", ["GaugeBackground"] = "slate", ["GaugeUnit"] = "bar(g)", ["Fill"] = "#818cf8",
            ["GaugeNeedleColor"] = "#e2e8f0", ["GaugeTrackColor"] = "#334155", ["GaugeShowTickLabels"] = false
        });
        b.Display("pressure", 62, 142, 280, 50, "PressureBar", "bar(g)", "#67e8f9", "F2");
        b.Display("power", 406, 142, 280, 50, "PowerKw", "kW", "#c4b5fd");
        b.Display("load", 750, 142, 280, 50, "LoadFlow", "Nm3/min", "#6ee7b7", "F2");
        b.Display("leak", 1094, 142, 280, 50, "LeakFlow", "Nm3/min", "#fdba74", "F2");
        b.Display("c1_flow", 390, 411, 178, 46, "Compressor1Flow", "Nm3/min", "#67e8f9");
        b.Display("c2_flow", 390, 616, 178, 46, "Compressor2Flow", "Nm3/min", "#c4b5fd");
        b.Display("total_air", 1090, 665, 280, 48, "DeliveredNm3", "Nm3", "#6ee7b7");
        b.Display("leak_state", 918, 736, 190, 32, "LeakText", color: "#fdba74", format: "");
        b.Display("load_state", 44, 736, 340, 32, "LoadText", format: "");
        b.Display("energy", 400, 721, 170, 48, "EnergyKwh", "kWh", "#c4b5fd", "F2");
        b.Display("minute", 646, 721, 210, 48, "Minute", "simulated minutes", format: "F2");
        b.StandardControls();
        b.Button("peak", 400, 820, 220, "Normal / Peak load", "PeakRequested", action: "ToggleVariable");
        b.Button("leak_toggle", 636, 820, 220, "Open / Seal leak", "LeakRequested", action: "ToggleVariable", color: "#9a5b16");
        b.Button("fault", 872, 820, 220, "C1 fault / Recover", "FaultRequested", action: "ToggleVariable", color: "#8b3046");
        b.Text("model", 44, 878, 1350, "Cyan C1 / purple C2 / green plant / orange leak | 3 simulated seconds per tick | bounded receiver mass balance");
        return b.Build(SimulationCode);
    }
}
