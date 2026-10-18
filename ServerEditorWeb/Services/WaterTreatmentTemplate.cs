// Copyright (c) 2026 Claudio Fiorani - All rights reserved.
using System.Text.Json.Nodes;

namespace ServerEditorWeb.Services;

public static class WaterTreatmentTemplate
{
    public static string Json { get; } = CreateJson();

    public const string SimulationCode = """
        const string p = "__TASK__.WaterTreatment.";
        // One simulated second per 250 ms invocation. All inventories and totals are litres.
        var reset = ReadBool(p + "ResetRequested");
        if (reset)
        {
            string[] names = { "RawLevel", "ClarifierLevel", "CleanLevel", "Clogging", "BackwashRemaining", "Elapsed", "SourceTotal", "DeliveryTotal", "WasteTotal", "DirtyInfluent", "Demand", "Progress", "DifferentialPressure", "IntakeFlow", "FeedFlow", "FilteredFlow", "BackwashFlow", "DeliveryFlow", "SludgeFlow" };
            double[] defaults = { 600, 300, 600, 0, 0, 0, 0, 0, 0, 1, 2, 0, .2, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < names.Length; i++) Write(p + names[i], defaults[i]);
            Write(p + "RunRequested", false); Write(p + "ResetRequested", false);
            Write(p + "BackwashRequested", false); Write(p + "AutoBackwash", true); Write(p + "PumpFault", false);
        }
        var raw = ReadDouble(p + "RawLevel"); var clarifier = ReadDouble(p + "ClarifierLevel");
        var clean = ReadDouble(p + "CleanLevel"); var clog = ReadDouble(p + "Clogging");
        var remaining = ReadDouble(p + "BackwashRemaining"); var elapsed = ReadDouble(p + "Elapsed");
        var source = ReadDouble(p + "SourceTotal"); var delivered = ReadDouble(p + "DeliveryTotal"); var waste = ReadDouble(p + "WasteTotal");
        var dirt = ReadDouble(p + "DirtyInfluent"); var demand = ReadDouble(p + "Demand");
        double[] values = { raw, clarifier, clean, clog, remaining, elapsed, source, delivered, waste, dirt, demand, ReadDouble(p + "Progress"), ReadDouble(p + "DifferentialPressure"), ReadDouble(p + "IntakeFlow"), ReadDouble(p + "FeedFlow"), ReadDouble(p + "FilteredFlow"), ReadDouble(p + "BackwashFlow"), ReadDouble(p + "DeliveryFlow"), ReadDouble(p + "SludgeFlow") };
        double[] maxima = { 1000, 800, 1200, 100, 12, 1e12, 1e12, 1e12, 1e12, 5, 8, 100, 3, 8, 6, 5, 10, 8, .18 };
        bool valid = true;
        for (int i = 0; i < values.Length; i++) valid &= double.IsFinite(values[i]) && values[i] >= 0 && values[i] <= maxima[i];
        valid &= dirt >= 1 && remaining == Math.Truncate(remaining);
        if (!valid) Write(p + "RunRequested", false);
        bool running = valid && ReadBool(p + "RunRequested");
        double intake = 0, feed = 0, filtered = 0, backwash = 0, delivery = 0, sludge = 0;
        string status = !valid ? "Invalid inputs - press Reset" : reset ? "Reset - press Start / Pause" : "Paused - inventories frozen";
        if (running)
        {
            elapsed += 1;
            if (ReadBool(p + "PumpFault")) status = "PUMP FAULT - all pumps interlocked";
            else
            {
                intake = Math.Min(8, 1000 - raw); raw += intake; source += intake;
                if (remaining == 0 && (ReadBool(p + "BackwashRequested") || (ReadBool(p + "AutoBackwash") && clog >= 70)))
                {
                    remaining = 12; Write(p + "BackwashRequested", false);
                }
                if (remaining > 0)
                {
                    backwash = Math.Min(10, clean); clean -= backwash; waste += backwash;
                    clog = Math.Max(0, clog - backwash * .65);
                    remaining = backwash > 0 ? remaining - 1 : 0;
                    status = backwash > 0 ? "BACKWASH - clean storage to waste" : "Dry clean tank - backwash interlocked";
                }
                else
                {
                    feed = Math.Min(6, Math.Min(raw, (800 - clarifier) / .97));
                    raw -= feed; sludge = feed * .03; clarifier += feed - sludge; waste += sludge;
                    filtered = Math.Min(5 * (1 - clog / 125), Math.Min(clarifier, 1200 - clean));
                    clarifier -= filtered; clean += filtered; clog = Math.Min(100, clog + filtered * .12 * dirt);
                    status = feed == 0 ? "Intake dry / clarifier full - feed interlocked" : "Treating - clarification / filtration / clean storage";
                }
                delivery = Math.Min(demand, clean); clean -= delivery; delivered += delivery;
            }
            Write(p + "RawLevel", raw); Write(p + "ClarifierLevel", clarifier); Write(p + "CleanLevel", clean);
            Write(p + "Clogging", clog); Write(p + "BackwashRemaining", remaining); Write(p + "Elapsed", elapsed);
            Write(p + "SourceTotal", source); Write(p + "DeliveryTotal", delivered); Write(p + "WasteTotal", waste);
            Write(p + "Progress", (elapsed % 120) / 120 * 100);
        }
        Write(p + "DifferentialPressure", valid ? .2 + clog * .025 : 0);
        Write(p + "IntakeFlow", intake); Write(p + "FeedFlow", feed); Write(p + "FilteredFlow", filtered);
        Write(p + "BackwashFlow", backwash); Write(p + "DeliveryFlow", delivery); Write(p + "SludgeFlow", sludge);
        Write(p + "PumpOn", feed > 0); Write(p + "FilterOn", filtered > 0); Write(p + "BackwashOn", backwash > 0);
        Write(p + "Status", status);
        """;

    private static string CreateJson()
    {
        var b = new DemoScreenBuilder("WaterTreatment", "Water Treatment Plant", "WaterTreatmentSimulation");
        foreach (var name in new[] { "RunRequested", "ResetRequested", "BackwashRequested", "PumpFault", "PumpOn", "FilterOn", "BackwashOn" }) b.Variable(name, "Boolean", false);
        b.Variable("AutoBackwash", "Boolean", true);
        string[] names = ["RawLevel", "ClarifierLevel", "CleanLevel", "Clogging", "BackwashRemaining", "Elapsed", "SourceTotal", "DeliveryTotal", "WasteTotal", "DirtyInfluent", "Demand", "Progress", "DifferentialPressure", "IntakeFlow", "FeedFlow", "FilteredFlow", "BackwashFlow", "DeliveryFlow", "SludgeFlow"];
        double[] defaults = [600, 300, 600, 0, 0, 0, 0, 0, 0, 1, 2, 0, .2, 0, 0, 0, 0, 0, 0];
        for (var i = 0; i < names.Length; i++) b.Variable(names[i], "Double", defaults[i]);
        b.Variable("Status", "String", "Paused - press Start / Pause");
        b.Header("WATER / TREATMENT WORKS", "Closed water balance / 1 simulated second per update / automatic filter recovery", "SOURCE / L", "DELIVERED / L", "WASTE + SLUDGE / L", "FILTER DIFFERENTIAL / BAR");
        b.Display("source", 62, 143, 270, 45, "SourceTotal", "L"); b.Display("delivered", 406, 143, 270, 45, "DeliveryTotal", "L");
        b.Display("waste_total", 750, 143, 270, 45, "WasteTotal", "L"); b.Display("dp", 1094, 143, 270, 45, "DifferentialPressure", "bar");
        b.Pipe("intake", 65, 477, 160, 18, "IntakeFlow", "#d6a75a");
        b.Pipe("raw", 290, 477, 265, 18, "FeedFlow", "#d6a75a");
        b.Pipe("treated", 680, 477, 435, 18, "FilteredFlow", "#22d3ee");
        b.Pipe("delivery", 1235, 477, 135, 18, "DeliveryFlow", "#34d399");
        b.Pipe("wash_return", 875, 650, 320, 16, "BackwashFlow", "#c084fc", "right-left");
        b.Pipe("wash_drop", 1180, 495, 16, 165, "BackwashFlow", "#c084fc", "top-bottom");
        b.Pipe("waste", 885, 540, 16, 180, "BackwashFlow", "#c084fc", "top-bottom");
        b.Pipe("sludge", 585, 535, 16, 185, "SludgeFlow", "#d6a75a", "top-bottom");
        b.Art("artwork", 0, 230, 1440, 530, """
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 530">
            <defs><linearGradient id="__TASK___wt_steel" x2="1" y2="0"><stop stop-color="#375069"/><stop offset=".4" stop-color="#a6bdca"/><stop offset="1" stop-color="#203c54"/></linearGradient><linearGradient id="__TASK___wt_filter" x2="1" y2="1"><stop stop-color="#42708a"/><stop offset="1" stop-color="#15283d"/></linearGradient></defs>
            <g font-family="Segoe UI,Arial" fill="#dceaf3"><g font-size="17" font-weight="600"><text x="110" y="36">01 / RAW INTAKE</text><text x="445" y="36">02 / CLARIFICATION</text><text x="795" y="36">03 / MEDIA FILTER</text><text x="1100" y="36">04 / CLEAN STORAGE</text></g>
            <g fill="#13273d" stroke="#416179"><path d="M90 325l145-65 145 65-145 65Z"/><path d="M432 325l155-65 155 65-155 65Z"/><path d="M765 325l115-55 115 55-115 55Z"/><path d="M1055 325l145-65 145 65-145 65Z"/></g>
            <path d="M470 145v130c0 48 235 48 235 0V145" fill="url(#__TASK___wt_steel)" stroke="#95b6c8"/><ellipse cx="587" cy="145" rx="117" ry="43" fill="#537986" stroke="#adc8ce"/><ellipse cx="587" cy="145" rx="99" ry="32" fill="#8e9d89"/><path d="M492 145h191m-96-31v65m0-34 48 20" stroke="#d1dfe5" stroke-width="6"/><path d="M489 117v-18h196v18" fill="none" stroke="#7793a8" stroke-width="4"/>
            <path d="M805 145v135c0 40 145 40 145 0V145Z" fill="url(#__TASK___wt_filter)" stroke="#83b2c6" stroke-width="2"/><ellipse cx="877" cy="145" rx="72" ry="25" fill="#749bad"/><ellipse cx="877" cy="142" rx="49" ry="15" fill="#233f53"/><path d="M820 188h115m-115 25h115m-115 25h115" stroke="#c7ab69" stroke-width="8" opacity=".8"/><path d="M820 270h115" stroke="#29c9db" stroke-width="6"/><circle cx="877" cy="105" r="19" fill="#ecf3f6" stroke="#46627a" stroke-width="4"/><path d="M877 105l9-9" stroke="#ea726e" stroke-width="3"/>
            <g transform="translate(0 -15)"><path d="M510 483l90-28 90 28-90 31Z" fill="#233e52" stroke="#7793a8"/><path d="M510 483v32l90 25 90-25v-32l-90 31Z" fill="#152c40"/><text x="530" y="451" font-size="12" fill="#d6a75a">SLUDGE / ACCOUNTED SINK</text>
            <path d="M805 483l90-28 90 28-90 31Z" fill="#573d71" stroke="#c084fc"/><path d="M805 483v32l90 25 90-25v-32l-90 31Z" fill="#352745"/><text x="825" y="451" font-size="12" fill="#d8b4fe">BACKWASH WASTE</text></g>
            <g font-size="12" fill="#9bb8cb"><text x="95" y="405">1000 L / raw water buffer</text><text x="447" y="405">800 L / 3% sludge extraction</text><text x="786" y="405">Auto wash at 70% clogging</text><text x="1090" y="405">1200 L / dry-pump interlock</text></g></g></svg>
            """);
        void Tank(string id, double x, string variable, double max, string color) => b.Symbol(id, "tank", x, 320, 180, 245, new JsonObject { ["VariablePath"] = b.Path + variable, ["TankStyle"] = "isometric", ["TankUseGradient"] = true, ["TankFillStyle"] = "gradient", ["TankLiquidColor"] = color, ["TankShowLevel"] = true, ["TankBodyColor"] = "#64748b", ["MinValue"] = 0, ["MaxValue"] = max });
        Tank("raw_tank", 145, "RawLevel", 1000, "#b99a62"); Tank("clean_tank", 1110, "CleanLevel", 1200, "#22d3ee");
        b.Symbol("pump", "motorcontrol", 350, 443, 90, 75, new JsonObject { ["VariablePath"] = b.Path + "PumpOn", ["MotorFaultPath"] = b.Path + "PumpFault", ["MotorStyle"] = "pump", ["MotorAppearance"] = "isometric", ["MotorUseGradient"] = true, ["MotorAnimateRotation"] = true });
        b.Display("raw_level", 140, 580, 195, 40, "RawLevel", "L"); b.Display("clarifier_level", 495, 580, 195, 40, "ClarifierLevel", "L");
        b.Display("clog", 795, 580, 185, 40, "Clogging", "%"); b.Display("clean_level", 1100, 580, 195, 40, "CleanLevel", "L");
        b.Symbol("progress", "progressbar", 1055, 730, 290, 22, new JsonObject { ["VariablePath"] = b.Path + "Progress", ["MinValue"] = 0, ["MaxValue"] = 100 });
        b.StandardControls(); b.Button("wash", 384, 820, 165, "Backwash now", "BackwashRequested", "true", color: "#69408c");
        b.Button("auto", 562, 820, 165, "Auto wash on/off", "AutoBackwash", action: "ToggleVariable");
        b.Button("fault", 740, 820, 145, "Pump fault", "PumpFault", action: "ToggleVariable", color: "#924347");
        b.Text("dirt_label", 915, 793, 205, "Influent dirt / 1 - 5"); b.Input("dirt", 915, 820, 175, "DirtyInfluent", 1, 5, 1);
        b.Text("demand_label", 1120, 793, 230, "Clean demand / L per step"); b.Input("demand", 1120, 820, 220, "Demand", 0, 8, 1);
        b.Text("legend", 44, 880, 1300, "AMBER raw + sludge   /   CYAN treated   /   PURPLE clean-water backwash to waste   /   inventory + delivery + waste = 1500 L + source");
        return b.Build(SimulationCode);
    }
}
