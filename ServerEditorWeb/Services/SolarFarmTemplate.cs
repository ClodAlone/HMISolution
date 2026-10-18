// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

using System.Text.Json;
using System.Text.Json.Nodes;

namespace ServerEditorWeb.Services;

public static class SolarFarmTemplate
{
    public static string Json { get; } = CreateJson();

    public const string SimulationCode = """
        const string p = "__TASK__.Solar.";
        const double dt = 0.05; // Simulated hours per 250 ms cycle: one day in two minutes.
        const double capacity = 80.0;
        const double efficiency = 0.96;
        var reset = ReadBool(p + "ResetRequested");
        if (reset)
        {
            Write(p + "RunRequested", false);
            Write(p + "ResetRequested", false);
            Write(p + "Hour", 9.0);
            Write(p + "BatterySoc", 55.0);
            Write(p + "Scenario", 0);
        }
        var hour = ReadDouble(p + "Hour");
        var soc = ReadDouble(p + "BatterySoc");
        var scenario = ReadDouble(p + "Scenario");
        var valid = double.IsFinite(hour) && hour >= 0 && hour < 24 &&
            double.IsFinite(soc) && soc >= 10 && soc <= 95 &&
            double.IsFinite(scenario) && scenario >= 0 && scenario <= 3 && scenario == Math.Truncate(scenario);
        var running = ReadBool(p + "RunRequested") && valid;
        if (!valid) Write(p + "RunRequested", false);
        double irradiance = 0, dc = 0, pv = 0, load = 0, battery = 0, grid = 0, self = 0;
        var status = !valid ? "Invalid inputs - press Reset" : reset ? "Reset - press Start / Pause" : "Paused - press Start / Pause";
        if (running)
        {
            if (scenario == 0) hour = (hour + dt) % 24;
            else hour = scenario == 1 ? 12 : scenario == 2 ? 14 : 20;
            irradiance = scenario == 1 ? 0.98 : scenario == 2 ? 0.10 : scenario == 3 ? 0 :
                Math.Max(0, Math.Sin(Math.PI * (hour - 6) / 12)) * (0.8 + 0.2 * Math.Sin(hour * 1.7));
            dc = 24 * irradiance;
            pv = dc * efficiency;
            load = scenario == 1 ? 5 : scenario == 2 ? 11 : scenario == 3 ? 8.5 :
                4.5 + (hour >= 17 && hour < 22 ? 8 : 2) + 0.8 * Math.Sin(hour * 2);
            var surplus = pv - load;
            if (surplus > 0)
                battery = Math.Min(surplus, Math.Min(8, (95 - soc) / 100 * capacity / (dt * efficiency)));
            else
                battery = -Math.Min(-surplus, Math.Min(8, (soc - 10) / 100 * capacity * efficiency / dt));
            soc = Math.Clamp(soc + (battery >= 0 ? battery * efficiency : battery / efficiency) * dt / capacity * 100, 10, 95);
            grid = load + battery - pv; // Positive imports, negative exports; PV + grid = load + battery.
            if (Math.Abs(grid) < 0.000001) grid = 0;
            if (Math.Abs(battery) < 0.000001) battery = 0;
            self = Math.Min(pv, load);
            status = scenario == 0 ? "Auto day cycle - 24 h in 120 s" : scenario == 1 ? "Sunny - surplus to battery / grid" :
                scenario == 2 ? "Cloudy - battery support / grid import" : "Evening - battery discharge / grid import";
            Write(p + "Hour", hour);
            Write(p + "BatterySoc", soc);
        }
        Write(p + "Irradiance", irradiance * 1000);
        Write(p + "PvDcKw", dc);
        Write(p + "PvAcKw", pv);
        Write(p + "Inverter1Kw", pv / 2);
        Write(p + "Inverter2Kw", pv / 2);
        Write(p + "LoadKw", load);
        Write(p + "BatteryKw", battery);
        Write(p + "GridKw", grid);
        Write(p + "SelfConsumedKw", self);
        Write(p + "SelfConsumptionPct", pv > 0 ? self / pv * 100 : 0);
        Write(p + "GridMode", grid > 0 ? "IMPORT" : grid < 0 ? "EXPORT" : "IDLE / BALANCED");
        Write(p + "BatteryMode", battery > 0 ? "CHARGING" : battery < 0 ? "DISCHARGING" : "STANDBY");
        Write(p + "Status", status);
        """;

    private const string Artwork = """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 1040">
          <defs>
            <linearGradient id="__TASK___sf_card" x2="0.8" y2="1"><stop stop-color="#23384c"/><stop offset="1" stop-color="#101d30"/></linearGradient>
            <linearGradient id="__TASK___sf_pv" x2="0.4" y2="1"><stop stop-color="#3289bb"/><stop offset="0.45" stop-color="#154b7a"/><stop offset="1" stop-color="#0b2348"/></linearGradient>
            <linearGradient id="__TASK___sf_glass" x2="1" y2="1"><stop stop-color="#b9f5ff" stop-opacity="0.5"/><stop offset="0.5" stop-color="#38bdf8" stop-opacity="0.05"/><stop offset="1" stop-color="#b9f5ff" stop-opacity="0.2"/></linearGradient>
            <linearGradient id="__TASK___sf_metal" x2="1" y2="0.4"><stop stop-color="#a9baca"/><stop offset="0.4" stop-color="#e2edf4"/><stop offset="1" stop-color="#74889f"/></linearGradient>
            <radialGradient id="__TASK___sf_sun"><stop stop-color="#fef3c7"/><stop offset="0.5" stop-color="#fbbf24"/><stop offset="1" stop-color="#f59e0b" stop-opacity="0"/></radialGradient>
            <pattern id="__TASK___sf_cells" width="26" height="23" patternUnits="userSpaceOnUse"><path d="M0 0H26V23H0Z" fill="none" stroke="#7dd3fc" stroke-width="0.9" opacity="0.65"/><path d="M13 0V23" stroke="#e0f2fe" stroke-width="0.4" opacity="0.45"/></pattern>
            <g id="__TASK___sf_panel"><path d="M12 91v20m132-20v20" stroke="#7891a8" stroke-width="5"/><path d="M0 0H158V91H0Z" fill="url(#__TASK___sf_pv)" stroke="#a5d7ed" stroke-width="3"/><path d="M0 0H158V91H0Z" fill="url(#__TASK___sf_cells)"/><path d="M0 0H158V91H0Z" fill="url(#__TASK___sf_glass)"/><path d="M0 91H158l7 7H7Z" fill="#09213a" stroke="#6388a7"/></g>
            <g id="__TASK___sf_inverter"><path d="M0 5L16 0h74v146l-16 9H0Z" fill="#59718a"/><rect y="5" width="74" height="150" rx="10" fill="url(#__TASK___sf_metal)" stroke="#cbd5e1"/><rect x="12" y="23" width="50" height="45" rx="5" fill="#0c2338"/><path d="M18 46q6-20 12 0t12 0t12 0" fill="none" stroke="#67e8f9" stroke-width="2"/><circle cx="21" cy="84" r="4" fill="#34d399"/><path d="M12 108h50m-50 7h50m-50 7h50m-50 7h50" stroke="#5b7087" stroke-width="3"/><rect x="22" y="148" width="30" height="13" rx="3" fill="#445c74"/></g>
          </defs>
          <g fill="none" stroke="#2b465f" opacity="0.25"><path d="M0 212H1440M0 838H1440"/><path d="M980 212V838M462 212V838"/></g>
          <g font-family="Segoe UI, sans-serif">
            <text x="44" y="49" fill="#f1f5f9" font-size="30" font-weight="700" letter-spacing="2">SOLAR / ENERGY CAMPUS</text>
            <text x="45" y="78" fill="#94aec5" font-size="14">24 kWp PV  /  2 x 12 kW INVERTERS  /  80 kWh STORAGE  /  LOCAL SIMULATION</text>
            <rect x="1150" y="30" width="246" height="44" rx="22" fill="#123e39" stroke="#2d7768"/><circle cx="1171" cy="52" r="5" fill="#34d399"/><text x="1185" y="57" fill="#a7f3d0" font-size="13">SMART SELF-CONSUMPTION</text>
            <g fill="url(#__TASK___sf_card)" stroke="#334e68"><rect x="44" y="106" width="316" height="90" rx="14"/><rect x="388" y="106" width="316" height="90" rx="14"/><rect x="732" y="106" width="316" height="90" rx="14"/><rect x="1076" y="106" width="320" height="90" rx="14"/></g>
            <g font-size="12" font-weight="600" letter-spacing="1.5"><text x="62" y="128" fill="#fde68a">SOLAR GENERATION / AC</text><text x="406" y="128" fill="#93c5fd">HOUSE DEMAND</text><text x="750" y="128" fill="#fdba74">GRID / +IMPORT -EXPORT</text><text x="1094" y="128" fill="#c4b5fd">BATTERY STATE OF CHARGE</text></g>
            <text x="44" y="244" fill="#e2e8f0" font-size="16" font-weight="600">01 / PHOTOVOLTAIC FIELD</text>
            <text x="44" y="265" fill="#8ba4bc" font-size="12">Monocrystalline arrays / dual MPPT</text>
            <ellipse cx="240" cy="475" rx="175" ry="20" fill="#020617" opacity="0.5"/>
            <circle cx="382" cy="302" r="48" fill="url(#__TASK___sf_sun)"/><circle cx="382" cy="302" r="14" fill="#fde68a"/>
            <g transform="translate(66 295) skewX(-16) scale(0.88 0.75)"><use href="#__TASK___sf_panel"/><use href="#__TASK___sf_panel" x="180"/></g>
            <g transform="translate(94 393) skewX(-16) scale(0.88 0.75)"><use href="#__TASK___sf_panel"/><use href="#__TASK___sf_panel" x="180"/></g>
            <text x="44" y="528" fill="#8ba4bc" font-size="12">DC power</text><text x="255" y="528" fill="#8ba4bc" font-size="12">Irradiance</text>
            <text x="514" y="268" fill="#e2e8f0" font-size="16" font-weight="600">02 / HYBRID INVERTERS</text>
            <g transform="translate(535 296)"><use href="#__TASK___sf_inverter"/><use href="#__TASK___sf_inverter" x="124"/></g>
            <text x="550" y="483" fill="#8ba4bc" font-size="12">INV-01 / AC</text><text x="674" y="483" fill="#8ba4bc" font-size="12">INV-02 / AC</text>
            <text x="810" y="335" fill="#8ba4bc" font-size="12">AC BUS</text>
            <text x="1050" y="244" fill="#e2e8f0" font-size="16" font-weight="600">03 / HOUSE + LOCAL LOADS</text>
            <ellipse cx="1225" cy="419" rx="125" ry="15" fill="#020617" opacity="0.5"/>
            <path d="M1112 328L1223 263l119 58-108 67Z" fill="#4b718b" stroke="#9dc3dc" stroke-width="2"/><path d="M1112 328v79l122 35v-88Z" fill="#abc5d5"/><path d="M1234 354l108-33v86l-108 35Z" fill="#5d7f98"/>
            <path d="M1098 331l125-81 130 68-14 15-116-61-111 72Z" fill="#1d394f" stroke="#7294ab"/><path d="M1237 281l75 37-54 16-70-36Z" fill="#18527d" stroke="#7dd3fc"/><path d="M1207 290l69 35m-49-26 69 34m-39-42-53 16" stroke="#7dd3fc" opacity="0.6"/>
            <path d="M1130 354l33 10v33l-33-10Zm48 14 28 8v34l-28-8Z" fill="#fde68a" stroke="#d8e9f2" stroke-width="2"/><path d="M1252 366l25-8v33l-25 8Zm40-13 28-8v32l-28 9Z" fill="#67e8f9"/><path d="M1210 390l20 6v45l-20-6Z" fill="#29495f"/>
            <text x="1050" y="470" fill="#8ba4bc" font-size="12">Direct solar self-consumption / kW</text>
            <text x="1050" y="530" fill="#e2e8f0" font-size="16" font-weight="600">04 / UTILITY NETWORK</text>
            <path d="M1145 687l65-140 64 140m-100-66h71m-89 28h106m-77-62h47m-63 100 80-66m-80 0 80 66m-57-135 33 35m-33 0 33-35" fill="none" stroke="#8ba4bc" stroke-width="5" stroke-linejoin="round"/>
            <path d="M1144 575h132m-116 25h100m-71-54h43" stroke="#b9d0e2" stroke-width="5"/><path d="M1054 566q95 30 185 0t157 0m-342 24q95 30 185 0t157 0" fill="none" stroke="#526e89" stroke-width="2"/>
            <text x="1050" y="747" fill="#8ba4bc" font-size="12">Orange: export &gt; grid / Cyan: import &lt; grid</text>
            <text x="514" y="605" fill="#e2e8f0" font-size="16" font-weight="600">05 / BATTERY + BMS</text>
            <path d="M530 643l22-15h197l21 15v148l-22 16H530Z" fill="#39546d" stroke="#7291ab"/><rect x="530" y="643" width="219" height="164" rx="8" fill="url(#__TASK___sf_card)" stroke="#90a9bd"/>
            <rect x="544" y="656" width="48" height="131" rx="5" fill="#081322" stroke="#426078"/>
            <g fill="#122638" stroke="#4e6c85"><rect x="606" y="655" width="128" height="27" rx="3"/><rect x="606" y="688" width="128" height="27" rx="3"/><rect x="606" y="721" width="128" height="27" rx="3"/><rect x="606" y="754" width="128" height="27" rx="3"/></g>
            <path d="M615 665h76m-76 8h76m-76 25h76m-76 8h76m-76 25h76m-76 8h76m-76 25h76m-76 8h76" stroke="#718ba3" stroke-width="2"/><g fill="#34d399"><circle cx="720" cy="668" r="3"/><circle cx="720" cy="701" r="3"/><circle cx="720" cy="734" r="3"/><circle cx="720" cy="767" r="3"/></g>
            <text x="44" y="650" fill="#e2e8f0" font-size="15" font-weight="600">POWER FLOW LEGEND</text>
            <g font-size="13" fill="#adc3d6"><circle cx="52" cy="678" r="4" fill="#fbbf24"/><text x="68" y="683">Solar DC / AC</text><circle cx="52" cy="707" r="4" fill="#60a5fa"/><text x="68" y="712">House load</text><circle cx="52" cy="736" r="4" fill="#fb923c"/><text x="68" y="741">Grid export</text><circle cx="52" cy="765" r="4" fill="#22d3ee"/><text x="68" y="770">Grid import</text><circle cx="270" cy="678" r="4" fill="#a78bfa"/><text x="286" y="683">BMS charge</text><circle cx="270" cy="707" r="4" fill="#34d399"/><text x="286" y="712">BMS discharge</text></g>
            <text x="44" y="809" fill="#6e8ba4" font-size="12">PV + grid = house + battery</text>
            <text x="1050" y="802" fill="#8ba4bc" font-size="12">Self-consumption / direct PV to house</text>
            <text x="44" y="1008" fill="#7895ae" font-size="12">SIMULATION ONLY / No external devices. Battery +/- = charge/discharge; grid +/- = import/export. Ideal AC bus, 96% conversion efficiency.</text>
          </g>
        </svg>
        """;

    private static string CreateJson()
    {
        var bundle = JsonNode.Parse("""
            {
              "Folder": { "Name": "__TASK__", "Variables": [], "Folders": [{ "Name": "Solar", "Folders": [], "Variables": [
                { "Name": "RunRequested", "Type": "Boolean", "Access": "ReadWrite", "Value": false, "InitialValue": "false" },
                { "Name": "ResetRequested", "Type": "Boolean", "Access": "ReadWrite", "Value": false, "InitialValue": "false" },
                { "Name": "Scenario", "Type": "Int32", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "Description": "0 auto day cycle; 1 sunny; 2 cloudy; 3 evening." },
                { "Name": "Hour", "Type": "Double", "Access": "ReadWrite", "Value": 9, "InitialValue": "9", "EngineeringUnit": "h" },
                { "Name": "BatterySoc", "Type": "Double", "Access": "ReadWrite", "Value": 55, "InitialValue": "55", "EngineeringUnit": "%", "Description": "80 kWh battery; operating reserve 10% to 95%." },
                { "Name": "Irradiance", "Type": "Double", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "EngineeringUnit": "W/m2" },
                { "Name": "PvDcKw", "Type": "Double", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "EngineeringUnit": "kW" },
                { "Name": "PvAcKw", "Type": "Double", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "EngineeringUnit": "kW" },
                { "Name": "Inverter1Kw", "Type": "Double", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "EngineeringUnit": "kW" },
                { "Name": "Inverter2Kw", "Type": "Double", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "EngineeringUnit": "kW" },
                { "Name": "LoadKw", "Type": "Double", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "EngineeringUnit": "kW" },
                { "Name": "BatteryKw", "Type": "Double", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "EngineeringUnit": "kW", "Description": "Positive charging; negative discharging." },
                { "Name": "GridKw", "Type": "Double", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "EngineeringUnit": "kW", "Description": "Positive importing; negative exporting." },
                { "Name": "SelfConsumedKw", "Type": "Double", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "EngineeringUnit": "kW" },
                { "Name": "SelfConsumptionPct", "Type": "Double", "Access": "ReadWrite", "Value": 0, "InitialValue": "0", "EngineeringUnit": "%", "Description": "PV directly serving the house / PV generation. Excludes battery charging." },
                { "Name": "GridMode", "Type": "String", "Access": "ReadWrite", "Value": "IDLE / BALANCED", "InitialValue": "IDLE / BALANCED" },
                { "Name": "BatteryMode", "Type": "String", "Access": "ReadWrite", "Value": "STANDBY", "InitialValue": "STANDBY" },
                { "Name": "Status", "Type": "String", "Access": "ReadWrite", "Value": "Paused - press Start / Pause", "InitialValue": "Paused - press Start / Pause" }
              ]}] },
              "Scripts": [{ "Name": "__TASK___SolarFarmSimulation", "Language": "CSharp", "Enabled": false, "IntervalMs": 250 }],
              "Screens": [{ "Name": "__TASK___Solar Farm", "Width": 1440, "Height": 1040, "Background": "#080f1e", "LayoutMode": "svg", "ShowInNavigation": true, "FitMode": "contain", "NavigationTransition": "fade", "NavigationDurationMs": 320, "Symbols": [] }]
            }
            """)!.AsObject();
        bundle["Scripts"]![0]!["Code"] = SimulationCode;
        var symbols = bundle["Screens"]![0]!["Symbols"]!.AsArray();
        void Symbol(string id, string type, double x, double y, double width, double height, JsonObject properties)
        {
            properties["Id"] = "solar_" + id;
            properties["Type"] = type;
            properties["X"] = x; properties["Y"] = y; properties["Width"] = width; properties["Height"] = height;
            properties["Stroke"] = "none"; properties["StrokeWidth"] = 0;
            symbols.Add(properties);
        }
        void Pipe(string id, double x, double y, double width, double height, string path, string color, string direction = "left-right", string? visible = null)
        {
            var properties = new JsonObject
            {
                ["PipeOrientation"] = width > height ? "horizontal" : "vertical", ["PipeUseGradient"] = true,
                ["PipeColor"] = "#425e74", ["PipeFlowColor"] = color, ["PipeShowFlow"] = path.Length > 0,
                ["PipeAnimate"] = true, ["PipeFlowSpeed"] = 0.8, ["PipeFlowDirection"] = direction
            };
            if (path.Length > 0) properties["VariablePath"] = "__TASK__.Solar." + path;
            if (visible != null) properties["VisibilityBinding"] = visible;
            Symbol(id, "pipe", x, y, width, height, properties);
        }
        void Display(string id, double x, double y, double width, double height, string path, string color, string unit = "", string format = "F1")
            => Symbol(id, "numericdisplay", x, y, width, height, new JsonObject
            {
                ["VariablePath"] = "__TASK__.Solar." + path, ["DisplayBackgroundStyle"] = "midnight", ["DisplayFontStyle"] = "terminal",
                ["DisplayAutoFit"] = true, ["Fill"] = color, ["NumericDisplayUnit"] = unit, ["NumericDisplayFormat"] = format, ["FontSize"] = 24
            });
        void Button(string id, double x, double width, string label, string path, string value, string color, string action = "WriteVariable")
            => Symbol(id, "button", x, 862, width, 42, new JsonObject
            {
                ["Label"] = label, ["ButtonColor"] = color, ["ButtonTextColor"] = "#f1f5f9", ["ButtonStyle"] = "raised", ["ButtonBorderRadius"] = 9, ["FontSize"] = 15,
                ["Commands"] = new JsonArray(new JsonObject { ["Id"] = "solar_cmd_" + id, ["Action"] = action, ["Trigger"] = "Click", ["VariablePath"] = "__TASK__.Solar." + path, ["Value"] = value })
            });
        // Pipes precede the equipment artwork so the opaque housings cover their endpoints.
        Pipe("pv_dc", 398, 380, 143, 26, "PvDcKw", "#fbbf24");
        Pipe("pv_ac", 738, 380, 159, 26, "PvAcKw", "#fbbf24");
        Pipe("bus", 871, 393, 26, 330, "", "#fbbf24");
        Pipe("house", 884, 380, 245, 26, "LoadKw", "#60a5fa");
        Pipe("grid_idle", 884, 580, 270, 26, "", "#22d3ee");
        Pipe("grid_import", 884, 580, 270, 26, "GridKw", "#22d3ee", "right-left", "value > 0");
        Pipe("grid_export", 884, 580, 270, 26, "GridKw", "#fb923c", "left-right", "value < 0");
        Pipe("battery_idle", 736, 710, 148, 26, "", "#a78bfa");
        Pipe("battery_charge", 736, 710, 148, 26, "BatteryKw", "#a78bfa", "right-left", "value > 0");
        Pipe("battery_discharge", 736, 710, 148, 26, "BatteryKw", "#34d399", "left-right", "value < 0");
        Symbol("artwork", "svg", 0, 0, 1440, 1040, new JsonObject { ["SvgContent"] = Artwork });
        Display("pv_total", 62, 140, 280, 48, "PvAcKw", "#fde68a", "kW");
        Display("house_total", 406, 140, 280, 48, "LoadKw", "#93c5fd", "kW");
        Display("grid_total", 750, 140, 280, 48, "GridKw", "#fdba74", "kW");
        Display("soc_total", 1094, 140, 284, 48, "BatterySoc", "#c4b5fd", "%");
        Display("pv_dc_value", 44, 540, 182, 52, "PvDcKw", "#fde68a", "kW DC");
        Display("irradiance", 252, 540, 182, 52, "Irradiance", "#fde68a", "W/m2", "F0");
        Display("inv1", 514, 492, 108, 50, "Inverter1Kw", "#67e8f9", "kW");
        Display("inv2", 650, 492, 108, 50, "Inverter2Kw", "#67e8f9", "kW");
        Display("self_kw", 1050, 480, 142, 38, "SelfConsumedKw", "#93c5fd", "kW");
        Display("grid_mode", 1050, 693, 326, 38, "GridMode", "#fdba74", format: "");
        Display("battery_mode", 514, 811, 264, 30, "BatteryMode", "#c4b5fd", format: "");
        Display("battery_power", 790, 754, 156, 58, "BatteryKw", "#c4b5fd", "kW (+ charge)");
        Display("self_pct", 1050, 811, 158, 30, "SelfConsumptionPct", "#93c5fd", "%");
        Symbol("battery_level", "progressbar", 552, 665, 32, 112, new JsonObject
        {
            ["VariablePath"] = "__TASK__.Solar.BatterySoc", ["MinValue"] = 0, ["MaxValue"] = 100,
            ["ProgressBarOrientation"] = "vertical", ["ProgressBarShowValue"] = false, ["ProgressBarTrackColor"] = "#122638", ["Fill"] = "#34d399"
        });
        Button("run", 44, 198, "Start / Pause", "RunRequested", "", "#0f766e", "ToggleVariable");
        Button("reset", 256, 140, "Reset", "ResetRequested", "true", "#475569");
        Button("auto", 436, 210, "Auto day cycle", "Scenario", "0", "#1e4971");
        Button("sunny", 660, 210, "Sunny / export", "Scenario", "1", "#9a5b16");
        Button("cloudy", 884, 210, "Cloudy / import", "Scenario", "2", "#155e75");
        Button("evening", 1108, 268, "Evening / discharge", "Scenario", "3", "#5b4486");
        Display("clock", 44, 925, 198, 56, "Hour", "#e2e8f0", "simulated hour");
        Display("status", 266, 925, 1110, 56, "Status", "#e2e8f0", format: "");
        return bundle.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }
}
