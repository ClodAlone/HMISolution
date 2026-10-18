// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ServerEditorWeb.Services;

public static class HvacPlantTemplate
{
    public static string Json { get; } = CreateJson();

    public const string SimulationCode = """
        const string p = "__TASK__.Hvac.";
        const double dt = 1.0 / 60; // One simulated minute per 250 ms invocation.
        var reset = ReadBool(p + "ResetRequested");
        if (reset)
        {
            Write(p + "RunRequested", false);
            Write(p + "ResetRequested", false);
            Write(p + "AutoControl", true);
            Write(p + "Demand", false);
            Write(p + "Mode", 0);
            Write(p + "Scenario", 0);
            Write(p + "Setpoint", 21.0);
            Write(p + "ManualSpeed", 65.0);
            Write(p + "RoomTemp", 18.0);
            Write(p + "BufferTemp", 30.0);
            Write(p + "Hour", 6.0);
            Write(p + "EnergyKwh", 0.0);
        }
        var room = ReadDouble(p + "RoomTemp");
        var buffer = ReadDouble(p + "BufferTemp");
        var hour = ReadDouble(p + "Hour");
        var energy = ReadDouble(p + "EnergyKwh");
        var setpoint = ReadDouble(p + "Setpoint");
        var manualSpeed = ReadDouble(p + "ManualSpeed");
        var mode = ReadDouble(p + "Mode");
        var scenario = ReadDouble(p + "Scenario");
        var automatic = ReadBool(p + "AutoControl");
        var valid = double.IsFinite(room) && room >= -20 && room <= 60 &&
            double.IsFinite(buffer) && buffer >= -10 && buffer <= 80 &&
            double.IsFinite(hour) && hour >= 0 && hour < 24 && double.IsFinite(energy) && energy >= 0 &&
            double.IsFinite(setpoint) && setpoint >= 16 && setpoint <= 28 &&
            double.IsFinite(manualSpeed) && manualSpeed >= 20 && manualSpeed <= 100 &&
            double.IsFinite(mode) && (mode == 0 || mode == 1) &&
            double.IsFinite(scenario) && scenario >= 0 && scenario <= 3 && scenario == Math.Truncate(scenario);
        if (!valid) Write(p + "RunRequested", false);
        var running = valid && ReadBool(p + "RunRequested");
        var fault = valid && scenario == 3;
        var outside = !valid ? 0 : scenario == 1 ? 34.0 : scenario == 2 ? 5.0 : -4.0;
        var demand = running && !fault && (!automatic || ReadBool(p + "Demand"));
        var sign = mode == 0 ? 1.0 : -1.0;
        if (running && !fault && automatic)
        {
            var error = sign * (setpoint - room);
            if (error > 0.3) demand = true;
            else if (error < -0.3) demand = false;
        }
        double speed = 0, flow = 0, thermal = 0, coil = 0, cop = 0, electrical = 0, loss = 0, internalHeat = 0, bufferLoss = 0;
        var supply = valid ? buffer : 0;
        var returning = supply;
        if (running)
        {
            hour = (hour + dt) % 24;
            if (demand)
            {
                speed = automatic ? Math.Clamp(45 + Math.Abs(setpoint - room) * 18, 45, 100) : manualSpeed;
                flow = 3 * speed / 100;
                thermal = sign * Math.Clamp(sign * ((mode == 0 ? 45 : 8) - buffer) * 2, 0, 12);
                coil = sign * Math.Clamp(sign * (buffer - room) * 1.1 * speed / 100, 0, 12);
                cop = Math.Abs(thermal) > 0.000001 ? Math.Clamp(mode == 0 ? 4.8 - 0.05 * (45 - outside) : 4.8 - 0.04 * (outside - 8), 2, 5) : 0;
                electrical = (cop > 0 ? Math.Abs(thermal) / cop : 0) + 0.12 * Math.Pow(speed / 100, 3);
                returning = supply - coil / (1.163 * flow);
            }
            loss = (scenario == 2 ? 0.65 : 0.18) * (room - outside);
            internalHeat = scenario == 1 ? 2 : 0.8;
            bufferLoss = 0.015 * (buffer - outside);
            buffer += (thermal - coil - bufferLoss) * dt / 0.7;
            room += (coil - loss + internalHeat) * dt / 3;
            energy += electrical * dt;
            Write(p + "Hour", hour);
            Write(p + "BufferTemp", buffer);
            Write(p + "RoomTemp", room);
            Write(p + "EnergyKwh", energy);
        }
        Write(p + "Demand", demand);
        Write(p + "PumpOn", flow > 0);
        Write(p + "FanOn", Math.Abs(thermal) > 0.000001);
        Write(p + "PumpFault", fault);
        Write(p + "OutdoorTemp", outside);
        Write(p + "SupplyTemp", supply);
        Write(p + "ReturnTemp", returning);
        Write(p + "PumpSpeed", speed);
        Write(p + "FlowM3h", flow);
        Write(p + "ThermalKw", thermal);
        Write(p + "CoilKw", coil);
        Write(p + "Cop", cop);
        Write(p + "ElectricalKw", electrical);
        Write(p + "BuildingLossKw", loss);
        Write(p + "InternalHeatKw", internalHeat);
        Write(p + "BufferLossKw", bufferLoss);
        Write(p + "HeatingFlow", flow > 0 && mode == 0);
        Write(p + "CoolingFlow", flow > 0 && mode == 1);
        Write(p + "ModeText", mode == 0 ? "HEATING" : "COOLING");
        Write(p + "ControlText", automatic ? "AUTO / THERMOSTAT" : "MANUAL / CONTINUOUS");
        Write(p + "Status", !valid ? "Invalid inputs - press Reset" : reset ? "Reset - press Start / Pause" :
            !running ? "Paused - press Start / Pause" : fault ? "PUMP FAULT - compressor interlocked" :
            !demand ? "Setpoint satisfied - thermostat standby" : scenario == 2 ? "Window open - increased heat loss" :
            mode == 0 ? "Heating - buffer / building energy balance" : "Cooling - buffer / building energy balance");
        """;

    private const string Artwork = """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 1080">
          <defs>
            <linearGradient id="__TASK___hv_card" x2="0.8" y2="1"><stop stop-color="#20384c"/><stop offset="1" stop-color="#111c30"/></linearGradient>
            <linearGradient id="__TASK___hv_metal"><stop stop-color="#496177"/><stop offset="0.3" stop-color="#cfdfeb"/><stop offset="0.55" stop-color="#91aabc"/><stop offset="1" stop-color="#354e65"/></linearGradient>
            <linearGradient id="__TASK___hv_tank"><stop stop-color="#274859"/><stop offset="0.35" stop-color="#a0c8d8"/><stop offset="0.62" stop-color="#5b8a9e"/><stop offset="1" stop-color="#1a384d"/></linearGradient>
            <linearGradient id="__TASK___hv_coil" x2="0" y2="1"><stop stop-color="#fb923c"/><stop offset="1" stop-color="#38bdf8"/></linearGradient>
            <pattern id="__TASK___hv_grille" width="8" height="8" patternUnits="userSpaceOnUse"><path d="M0 0H8M0 4H8" stroke="#243c51" stroke-width="2"/></pattern>
          </defs>
          <g font-family="Segoe UI,Arial,sans-serif" fill="#e2e8f0">
            <text x="44" y="50" font-size="30" font-weight="700" letter-spacing="2">CLIMATE / THERMAL CAMPUS</text>
            <text x="44" y="79" font-size="13" fill="#7db3cf">REVERSIBLE HEAT PUMP / HYDRONIC BUFFER / BUILDING THERMOSTAT</text>
            <rect x="1140" y="30" width="256" height="44" rx="22" fill="#123e3b" stroke="#28786d"/>
            <circle cx="1162" cy="52" r="5" fill="#34d399"/><text x="1178" y="57" font-size="12" fill="#99f6e4">LOCAL THERMAL SIMULATION</text>
            <g fill="url(#__TASK___hv_card)" stroke="#355069"><rect x="44" y="106" width="318" height="96" rx="16"/><rect x="388" y="106" width="318" height="96" rx="16"/><rect x="732" y="106" width="318" height="96" rx="16"/><rect x="1076" y="106" width="320" height="96" rx="16"/></g>
            <g font-size="12" font-weight="600" letter-spacing="1"><text x="62" y="130" fill="#fbbf24">INDOOR TEMPERATURE</text><text x="406" y="130" fill="#93c5fd">OUTDOOR TEMPERATURE</text><text x="750" y="130" fill="#6ee7b7">COMPRESSOR COP</text><text x="1094" y="130" fill="#c4b5fd">TOTAL ELECTRICAL INPUT</text></g>
            <path d="M0 224H1440M44 746H1396" stroke="#1d3045"/>
            <g font-size="15" font-weight="600"><text x="44" y="265">01 / AIR-SOURCE HEAT PUMP</text><text x="447" y="265">02 / BUFFER</text><text x="724" y="265">03 / CIRCULATION</text><text x="1040" y="265">04 / BUILDING + COIL</text></g>
            <g font-size="12" fill="#7db3cf"><text x="44" y="286">12 kW reversible / fan + compressor</text><text x="447" y="286">0.7 kWh/K thermal mass</text><text x="1040" y="286">3 kWh/K / simulated room</text></g>
            <ellipse cx="206" cy="628" rx="156" ry="18" fill="#040b16"/>
            <path d="M66 351L96 327H360V587L330 613H66Z" fill="#526e82" stroke="#8faabd"/>
            <rect x="66" y="351" width="264" height="262" rx="12" fill="url(#__TASK___hv_metal)" stroke="#a7c2d2"/>
            <path d="M330 351L360 327V587L330 613Z" fill="#30495c"/>
            <rect x="88" y="368" width="116" height="27" rx="5" fill="#102c3e"/><text x="100" y="386" font-size="11" fill="#67e8f9">AIR / WATER HP-01</text>
            <circle cx="200" cy="479" r="78" fill="#0b1c2d" stroke="#6b8d9f" stroke-width="5"/>
            <circle cx="200" cy="479" r="68" fill="url(#__TASK___hv_grille)"/>
            <path d="M88 580H305M88 589H305M88 598H305" stroke="#3e596c" stroke-width="3"/>
            <path d="M94 613V625M300 613V625" stroke="#5d788e" stroke-width="10"/>
            <ellipse cx="536" cy="628" rx="105" ry="15" fill="#040b16"/>
            <path d="M456 341V590C456 624 622 624 622 590V341Z" fill="url(#__TASK___hv_tank)" stroke="#88aebe"/>
            <ellipse cx="539" cy="341" rx="83" ry="27" fill="#9dbbc9" stroke="#c8e1e9"/>
            <ellipse cx="539" cy="341" rx="63" ry="17" fill="#618899"/>
            <path d="M470 376V575" stroke="#d5eef3" stroke-width="4" opacity="0.4"/>
            <rect x="496" y="417" width="86" height="106" rx="12" fill="#163748" stroke="#7fa7b9"/>
            <text x="539" y="452" text-anchor="middle" font-size="15" fill="#bae6fd">BUFFER</text><text x="539" y="477" text-anchor="middle" font-size="12" fill="#7db3cf">THERMAL</text><text x="539" y="497" text-anchor="middle" font-size="12" fill="#7db3cf">STORAGE</text>
            <path d="M480 614V628M598 614V628" stroke="#496879" stroke-width="8"/>
            <g fill="#7db3cf" font-size="12"><text x="680" y="301">SUPPLY / C</text><text x="680" y="598">RETURN / C</text><text x="730" y="462">PUMP SPEED / FLOW</text></g>
            <path d="M1006 347L1194 287L1370 358L1194 427Z" fill="#476a80" stroke="#91aec1"/>
            <path d="M1006 347V594L1194 665V427Z" fill="#28495f" stroke="#557b92"/>
            <path d="M1194 427L1370 358V594L1194 665Z" fill="#18354d" stroke="#557b92"/>
            <path d="M1029 390V574L1170 628V444Z" fill="#0e2638" stroke="#4c829b"/>
            <path d="M1040 383H1128V416H1063V456H1128V496H1063V536H1128V583H1040" fill="none" stroke="url(#__TASK___hv_coil)" stroke-width="11" stroke-linejoin="round"/>
            <path d="M1149 460V590M1159 465V594" stroke="#355a6c"/>
            <g fill="#7bd5ed" stroke="#497990"><path d="M1220 448L1265 430V483L1220 501Z"/><path d="M1290 420L1340 401V455L1290 473Z"/><path d="M1220 535L1265 517V570L1220 588Z"/><path d="M1290 507L1340 488V541L1290 559Z"/></g>
            <g font-size="12" fill="#7db3cf"><text x="44" y="659">HEAT PUMP OUTPUT / +HEAT -COOL</text><text x="447" y="659">BUFFER TEMPERATURE</text><text x="1040" y="694">COIL TO ROOM / +HEAT -COOL</text></g>
            <g font-size="12"><circle cx="686" cy="655" r="4" fill="#fb7185"/><text x="700" y="660">Heating supply</text><circle cx="686" cy="680" r="4" fill="#22d3ee"/><text x="700" y="685">Cooling supply</text><circle cx="686" cy="705" r="4" fill="#60a5fa"/><text x="700" y="710">Return to plant</text></g>
            <g font-size="12" fill="#7db3cf"><text x="1000" y="774">THERMOSTAT / 16-28 C</text><text x="1190" y="774">MANUAL PUMP / 20-100%</text><text x="1000" y="870">Enter writes / Esc cancels</text><text x="1000" y="892">Manual: continuous conditioning</text><text x="1000" y="914">Auto: thermostat +/-0.3 C</text></g>
            <text x="44" y="920" font-size="12" fill="#7db3cf">SCENARIOS SET WEATHER; COLD / WINDOW SELECT HEATING, HOT SELECTS COOLING. START / PAUSE CONTROLS TIME.</text>
            <text x="44" y="1033" font-size="12" fill="#7db3cf">SIMULATION ONLY / 1 min per 250 ms / signed thermal kW / COP excludes circulation pump / no external devices</text>
          </g>
        </svg>
        """;

    private static string CreateJson()
    {
        var variables = new JsonArray();
        void Variable(string name, string type, object initial, string unit = "", string description = "")
        {
            variables.Add(new JsonObject
            {
                ["Name"] = name, ["Type"] = type, ["Access"] = "ReadWrite", ["Value"] = JsonSerializer.SerializeToNode(initial),
                ["InitialValue"] = initial is bool flag ? (flag ? "true" : "false") : Convert.ToString(initial, CultureInfo.InvariantCulture),
                ["EngineeringUnit"] = unit, ["Description"] = description
            });
        }
        foreach (var name in new[] { "RunRequested", "ResetRequested", "Demand", "PumpOn", "FanOn", "PumpFault", "HeatingFlow", "CoolingFlow" }) Variable(name, "Boolean", false);
        Variable("AutoControl", "Boolean", true);
        Variable("Mode", "Int32", 0, description: "0 heating; 1 cooling.");
        Variable("Scenario", "Int32", 0, description: "0 cold morning; 1 hot afternoon; 2 open window; 3 pump fault.");
        Variable("Setpoint", "Double", 21, "C");
        Variable("ManualSpeed", "Double", 65, "%");
        Variable("RoomTemp", "Double", 18, "C");
        Variable("BufferTemp", "Double", 30, "C");
        Variable("Hour", "Double", 6, "h");
        Variable("EnergyKwh", "Double", 0, "kWh");
        Variable("OutdoorTemp", "Double", -4, "C");
        Variable("SupplyTemp", "Double", 30, "C");
        Variable("ReturnTemp", "Double", 30, "C");
        foreach (var name in new[] { "PumpSpeed", "FlowM3h", "ThermalKw", "CoilKw", "Cop", "ElectricalKw", "BuildingLossKw", "InternalHeatKw", "BufferLossKw" })
            Variable(name, "Double", 0, name == "PumpSpeed" ? "%" : name == "FlowM3h" ? "m3/h" : name == "Cop" ? "" : "kW");
        Variable("ModeText", "String", "HEATING");
        Variable("ControlText", "String", "AUTO / THERMOSTAT");
        Variable("Status", "String", "Paused - press Start / Pause");
        var symbols = new JsonArray();
        const string path = "__TASK__.Hvac.";
        void Symbol(string id, string type, double x, double y, double w, double h, JsonObject props)
        {
            props["Id"] = "hvac_" + id; props["Type"] = type;
            props["X"] = x; props["Y"] = y; props["Width"] = w; props["Height"] = h;
            props["Stroke"] = "none"; props["StrokeWidth"] = 0;
            symbols.Add(props);
        }
        void Pipe(string id, double x, double y, double w, string variable, string color, string direction = "left-right", bool gated = false)
            => Symbol(id, "pipe", x, y, w, 26, new JsonObject
            {
                ["PipeOrientation"] = "horizontal", ["PipeUseGradient"] = true, ["PipeColor"] = "#425e74", ["PipeFlowColor"] = color,
                ["PipeFlowDirection"] = direction, ["PipeShowFlow"] = id != "supply_idle", ["PipeAnimate"] = true, ["PipeFlowSpeed"] = 0.8,
                ["VariablePath"] = path + variable, ["VisibilityBinding"] = gated ? "value == true" : ""
            });
        Pipe("supply_idle", 340, 370, 710, "PumpOn", "#94a3b8");
        Pipe("supply_heat", 340, 370, 710, "HeatingFlow", "#fb7185", gated: true);
        Pipe("supply_cool", 340, 370, 710, "CoolingFlow", "#22d3ee", gated: true);
        Pipe("return", 340, 570, 710, "PumpOn", "#60a5fa", "right-left");
        Symbol("artwork", "svg", 0, 0, 1440, 1080, new JsonObject { ["SvgContent"] = Artwork });
        const string blades = "<g fill=\"#67e8f9\" opacity=\"0.8\"><path d=\"M75 75C40 35 65 4 88 13C107 21 90 58 75 75ZM75 75C115 40 146 65 137 88C129 107 92 90 75 75ZM75 75C110 115 85 146 62 137C43 129 60 92 75 75ZM75 75C35 110 4 85 13 62C21 43 58 60 75 75Z\"/><circle cx=\"75\" cy=\"75\" r=\"13\" fill=\"#b9dce8\"/></g>";
        foreach (var active in new[] { false, true })
        {
            var animation = active ? "<style>@keyframes __TASK___hv_spin{to{transform:rotate(360deg)}}.__TASK___hv_rotor{transform-origin:75px 75px;animation:__TASK___hv_spin 1.2s linear infinite}@media(prefers-reduced-motion:reduce){.__TASK___hv_rotor{animation:none!important}}</style>" : "";
            Symbol(active ? "fan_running" : "fan_idle", "svg", 125, 404, 150, 150, new JsonObject
            {
                ["SvgContent"] = "<svg viewBox=\"0 0 150 150\">" + animation + "<g class=\"__TASK___hv_rotor\">" + blades + "</g></svg>",
                ["VariablePath"] = path + "FanOn", ["VisibilityBinding"] = active ? "value == true" : "value == false"
            });
        }
        Symbol("pump", "motorcontrol", 730, 344, 128, 92, new JsonObject
        {
            ["VariablePath"] = path + "PumpOn", ["MotorFaultPath"] = path + "PumpFault", ["MotorStyle"] = "pump", ["MotorAppearance"] = "isometric",
            ["MotorUseGradient"] = true, ["MotorAnimateRotation"] = true, ["MotorRunColor"] = "#2dd4bf", ["MotorBodyColor"] = "#52758a"
        });
        void Display(string id, double x, double y, double w, double h, string variable, string unit = "", string color = "#e2e8f0", string format = "F1")
            => Symbol(id, "numericdisplay", x, y, w, h, new JsonObject
            {
                ["VariablePath"] = path + variable, ["DisplayBackgroundStyle"] = "midnight", ["DisplayFontStyle"] = "terminal", ["DisplayAutoFit"] = true,
                ["Fill"] = color, ["NumericDisplayUnit"] = unit, ["NumericDisplayFormat"] = format, ["FontSize"] = 24
            });
        Display("room", 62, 142, 280, 50, "RoomTemp", "C", "#fde68a");
        Display("outdoor", 406, 142, 280, 50, "OutdoorTemp", "C", "#93c5fd");
        Display("cop", 750, 142, 280, 50, "Cop", "heat pump COP", "#6ee7b7", "F2");
        Display("power", 1094, 142, 284, 50, "ElectricalKw", "kW", "#c4b5fd", "F2");
        Display("supply", 680, 310, 155, 32, "SupplyTemp", "C", "#fda4af");
        Display("return_value", 680, 606, 155, 38, "ReturnTemp", "C", "#93c5fd");
        Display("speed", 730, 477, 100, 44, "PumpSpeed", "%", "#67e8f9", "F0");
        Display("flow", 842, 477, 118, 44, "FlowM3h", "m3/h", "#67e8f9", "F2");
        Display("thermal", 44, 670, 225, 52, "ThermalKw", "kW thermal", "#fda4af");
        Display("energy", 282, 670, 142, 52, "EnergyKwh", "kWh input", "#c4b5fd", "F2");
        Display("buffer", 447, 670, 190, 52, "BufferTemp", "C", "#7dd3fc");
        Display("coil", 1040, 704, 202, 32, "CoilKw", "kW", "#fda4af");
        Display("mode", 44, 754, 225, 28, "ModeText", format: "");
        Display("control", 282, 754, 430, 28, "ControlText", format: "");
        void Button(string id, double x, double y, double w, string label, string variable, string value, string color, string action = "WriteVariable", int? mode = null)
        {
            var commands = new JsonArray(new JsonObject { ["Id"] = "hvac_cmd_" + id, ["Action"] = action, ["Trigger"] = "Click", ["VariablePath"] = path + variable, ["Value"] = value });
            if (mode.HasValue) commands.Add(new JsonObject { ["Id"] = "hvac_mode_" + id, ["Action"] = "WriteVariable", ["Trigger"] = "Click", ["VariablePath"] = path + "Mode", ["Value"] = mode.Value.ToString(CultureInfo.InvariantCulture) });
            Symbol(id, "button", x, y, w, 42, new JsonObject
            {
                ["Label"] = label, ["ButtonStyle"] = "raised", ["ButtonColor"] = color, ["ButtonTextColor"] = "#f1f5f9", ["ButtonBorderRadius"] = 9, ["FontSize"] = 14, ["Commands"] = commands
            });
        }
        Button("run", 44, 792, 160, "Start / Pause", "RunRequested", "", "#0f766e", "ToggleVariable");
        Button("reset", 216, 792, 110, "Reset", "ResetRequested", "true", "#475569");
        Button("heat", 338, 792, 120, "Heating", "Mode", "0", "#9f3e47");
        Button("cool", 470, 792, 120, "Cooling", "Mode", "1", "#155e75");
        Button("auto", 606, 792, 226, "Auto / Manual", "AutoControl", "", "#31597a", "ToggleVariable");
        Button("cold", 44, 853, 188, "Cold morning", "Scenario", "0", "#334e71", mode: 0);
        Button("hot", 244, 853, 188, "Hot afternoon", "Scenario", "1", "#915720", mode: 1);
        Button("window", 444, 853, 188, "Open window", "Scenario", "2", "#4d5083", mode: 0);
        Button("fault", 644, 853, 188, "Pump fault", "Scenario", "3", "#8b3046");
        void Input(string id, double x, double w, string variable, double min, double max, double step)
            => Symbol(id, "editbox", x, 788, w, 58, new JsonObject
            {
                ["VariablePath"] = path + variable, ["MinValue"] = min, ["MaxValue"] = max, ["EditBoxStep"] = step, ["EditBoxFormat"] = step < 1 ? "F1" : "F0",
                ["DisplayBackgroundStyle"] = "ivory", ["DisplayFontStyle"] = "terminal", ["DisplayAutoFit"] = true, ["DisplayTextColor"] = "#16324c"
            });
        Input("setpoint", 1000, 170, "Setpoint", 16, 28, 0.5);
        Input("manual_speed", 1190, 206, "ManualSpeed", 20, 100, 5);
        Display("clock", 44, 944, 185, 60, "Hour", "simulated hour", "#93c5fd", "F2");
        Display("status", 250, 944, 1146, 60, "Status", format: "");
        var bundle = new JsonObject
        {
            ["Folder"] = new JsonObject { ["Name"] = "__TASK__", ["Variables"] = new JsonArray(), ["Folders"] = new JsonArray(new JsonObject { ["Name"] = "Hvac", ["Variables"] = variables, ["Folders"] = new JsonArray() }) },
            ["Scripts"] = new JsonArray(new JsonObject { ["Name"] = "__TASK___HvacPlantSimulation", ["Language"] = "CSharp", ["IntervalMs"] = 250, ["Enabled"] = false, ["Code"] = SimulationCode }),
            ["Screens"] = new JsonArray(new JsonObject { ["Name"] = "__TASK___HVAC Plant", ["Width"] = 1440, ["Height"] = 1080, ["Background"] = "#080f1e", ["LayoutMode"] = "svg", ["FitMode"] = "contain", ["ShowInNavigation"] = true, ["NavigationTransition"] = "fade", ["NavigationDurationMs"] = 320, ["Symbols"] = symbols })
        };
        return bundle.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }
}
