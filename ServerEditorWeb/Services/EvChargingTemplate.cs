// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

namespace ServerEditorWeb.Services;

public static class EvChargingTemplate
{
    public const string SimulationCode = """
        const string p = "__TASK__.EvCharging.";
        const double dtHours = 1.0 / 60.0;
        const double capacityKwh = 60.0;
        const double efficiency = 0.92;
        const double targetSoc = 85.0;
        var reset = ReadBool(p + "ResetRequested");
        if (reset)
        {
            Write(p + "RunRequested", false); Write(p + "ResetRequested", false); Write(p + "ArrivalRequested", false);
            Write(p + "SiteLimitKw", 22.0); Write(p + "ArrivalBatch", 1);
            Write(p + "Hour", 8.0); Write(p + "Cycle", 0.0); Write(p + "QueueCount", 0.0);
            Write(p + "Arrived", 3.0); Write(p + "Departed", 0.0); Write(p + "ActiveCount", 3.0); Write(p + "EnergyKwh", 0.0);
            Write(p + "SitePowerKw", 0.0); Write(p + "Charger1Kw", 0.0); Write(p + "Charger2Kw", 0.0); Write(p + "Charger3Kw", 0.0);
            Write(p + "Present1", true); Write(p + "Present2", true); Write(p + "Present3", true);
            Write(p + "Soc1", 35.0); Write(p + "Soc2", 55.0); Write(p + "Soc3", 72.0);
        }
        var siteLimit = ReadDouble(p + "SiteLimitKw");
        var batch = ReadDouble(p + "ArrivalBatch");
        var hour = ReadDouble(p + "Hour");
        var cycle = ReadDouble(p + "Cycle");
        var queue = ReadDouble(p + "QueueCount");
        var arrived = ReadDouble(p + "Arrived");
        var departed = ReadDouble(p + "Departed");
        var activeCount = ReadDouble(p + "ActiveCount");
        var energy = ReadDouble(p + "EnergyKwh");
        var present = new[] { ReadBool(p + "Present1"), ReadBool(p + "Present2"), ReadBool(p + "Present3") };
        var occupied = 0;
        for (var i = 0; i < 3; i++) if (present[i]) occupied++;
        var soc = new[] { ReadDouble(p + "Soc1"), ReadDouble(p + "Soc2"), ReadDouble(p + "Soc3") };
        var priorPower = new[] { ReadDouble(p + "Charger1Kw"), ReadDouble(p + "Charger2Kw"), ReadDouble(p + "Charger3Kw") };
        var priorSitePower = ReadDouble(p + "SitePowerKw");
        var valid = double.IsFinite(siteLimit) && siteLimit >= 3 && siteLimit <= 33 &&
            double.IsFinite(batch) && batch >= 1 && batch <= 3 && batch == Math.Truncate(batch) &&
            double.IsFinite(hour) && hour >= 0 && hour < 24 && double.IsFinite(cycle) && cycle >= 0 && cycle == Math.Truncate(cycle) &&
            double.IsFinite(queue) && queue >= 0 && queue <= 500 && queue == Math.Truncate(queue) &&
            double.IsFinite(arrived) && arrived >= 0 && arrived == Math.Truncate(arrived) &&
            double.IsFinite(departed) && departed >= 0 && departed <= arrived && departed == Math.Truncate(departed) &&
            double.IsFinite(activeCount) && activeCount >= 0 && activeCount <= 3 && activeCount == Math.Truncate(activeCount) &&
            double.IsFinite(energy) && energy >= 0 && double.IsFinite(priorSitePower) && priorSitePower >= 0 && priorSitePower <= 33;
        for (var i = 0; i < 3; i++)
            valid = valid && double.IsFinite(soc[i]) && (present[i] ? soc[i] >= 0 && soc[i] <= targetSoc : soc[i] == 0) &&
                double.IsFinite(priorPower[i]) && priorPower[i] >= 0 && priorPower[i] <= 11;
        valid = valid && arrived == departed + queue + occupied && activeCount == occupied;
        var run = ReadBool(p + "RunRequested") && valid && !reset;
        var power = new double[3];
        var status = !valid ? "Invalid numeric input/state - press Reset" : reset ? "Reset - press Start / Pause" : "Paused - press Start / Pause";
        if (!valid) Write(p + "RunRequested", false);
        if (run)
        {
            void AddArrivals(int count)
            {
                for (var n = 0; n < count; n++)
                {
                    if (queue < 500) { queue++; arrived++; }
                }
            }
            if (ReadBool(p + "ArrivalRequested")) AddArrivals((int)batch);
            Write(p + "ArrivalRequested", false);
            cycle++;
            if (cycle % 120 == 0) AddArrivals(1);
            for (var i = 0; i < 3; i++)
            {
                if (present[i] && soc[i] >= targetSoc - 0.0000001)
                {
                    present[i] = false; soc[i] = 0; departed++;
                }
            }
            var active = 0;
            for (var i = 0; i < 3; i++) if (present[i]) active++;
            if (active > 0)
            {
                var fairShare = Math.Min(11.0, siteLimit / active);
                for (var i = 0; i < 3; i++)
                    if (present[i]) power[i] = Math.Min(fairShare, (targetSoc - soc[i]) * capacityKwh / (100.0 * efficiency * dtHours));
            }
            var sitePower = 0.0;
            for (var i = 0; i < 3; i++)
            {
                if (present[i]) soc[i] = Math.Min(targetSoc, soc[i] + power[i] * efficiency * dtHours / capacityKwh * 100.0);
                sitePower += power[i];
            }
            for (var i = 0; i < 3 && queue > 0; i++)
            {
                if (!present[i]) { present[i] = true; soc[i] = 20; queue--; }
            }
            active = 0;
            for (var i = 0; i < 3; i++) if (present[i]) active++;
            energy += sitePower * dtHours;
            hour = (hour + dtHours) % 24.0;
            status = queue > 0 ? "Charging - queue waiting / fair site-power sharing" : "Charging - shared site limit / target 85%";
            Write(p + "RunRequested", true);
            Write(p + "Hour", hour); Write(p + "Cycle", cycle); Write(p + "QueueCount", queue);
            Write(p + "Arrived", arrived); Write(p + "Departed", departed); Write(p + "ActiveCount", active); Write(p + "EnergyKwh", energy);
            for (var i = 0; i < 3; i++)
            {
                Write(p + "Present" + (i + 1), present[i]); Write(p + "Soc" + (i + 1), soc[i]);
                Write(p + "Charger" + (i + 1) + "Kw", power[i]);
            }
        }
        else
        {
            for (var i = 0; i < 3; i++) Write(p + "Charger" + (i + 1) + "Kw", 0.0);
        }
        Write(p + "SitePowerKw", power[0] + power[1] + power[2]);
        Write(p + "Status", status);
        """;

    private const string Artwork = """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 1040">
          <defs>
            <linearGradient id="__TASK___evc_floor" x2="0.7" y2="1"><stop stop-color="#1d3448"/><stop offset="1" stop-color="#101b2a"/></linearGradient>
            <linearGradient id="__TASK___evc_metal" x2="1" y2="0.4"><stop stop-color="#617990"/><stop offset=".45" stop-color="#d6e2eb"/><stop offset="1" stop-color="#7890a5"/></linearGradient>
            <linearGradient id="__TASK___evc_screen" x2="0" y2="1"><stop stop-color="#164a67"/><stop offset="1" stop-color="#071624"/></linearGradient>
            <linearGradient id="__TASK___evc_car" x2="0" y2="1"><stop stop-color="#f1f5f9"/><stop offset=".55" stop-color="#a9c3d5"/><stop offset="1" stop-color="#52748c"/></linearGradient>
            <linearGradient id="__TASK___evc_canopy" x2="0" y2="1"><stop stop-color="#173f5b"/><stop offset="1" stop-color="#10283c"/></linearGradient>
            <pattern id="__TASK___evc_grid" width="44" height="24" patternUnits="userSpaceOnUse"><path d="M0 0H44V24H0Z" fill="none" stroke="#75bde0" stroke-opacity=".38"/><path d="M22 0V24" stroke="#b8ebff" stroke-opacity=".3"/></pattern>
            <g id="__TASK___evc_charger"><path d="M0 12l13-8h70l12 8v134H0Z" fill="#435d74" stroke="#9cb4c7"/><rect x="9" y="11" width="71" height="132" rx="9" fill="url(#__TASK___evc_metal)" stroke="#dbe7ef"/><rect x="19" y="24" width="51" height="51" rx="6" fill="url(#__TASK___evc_screen)" stroke="#29485e"/><path d="M28 62V37h7v14h8V37h7v25Z" fill="#67e8f9"/><circle cx="60" cy="38" r="3" fill="#34d399"/><path d="M23 91h43m-43 9h43m-43 9h43" stroke="#758ba0" stroke-width="3"/><path d="M16 143v10m57-10v10" stroke="#273e52" stroke-width="6"/></g>
            <g id="__TASK___evc_carshape"><path d="M7 50l13-22q7-12 23-15h46q15 2 25 16l16 14 28 9q9 4 9 13v13H1V62q0-8 6-12Z" fill="url(#__TASK___evc_car)" stroke="#e2f1fa" stroke-width="2"/><path d="M47 20h39q13 2 20 14l8 14H31l9-19q2-6 7-9Z" fill="#183e56" stroke="#a5d9f3" stroke-width="2"/><path d="M66 21v26m-34-2h83" stroke="#bde8f8" stroke-opacity=".7"/><path d="M8 56h16m111-2 13 5" stroke="#fde68a" stroke-width="5"/><circle cx="36" cy="78" r="12" fill="#101a24" stroke="#92a9ba" stroke-width="3"/><circle cx="126" cy="78" r="12" fill="#101a24" stroke="#92a9ba" stroke-width="3"/><circle cx="36" cy="78" r="4" fill="#b8c8d4"/><circle cx="126" cy="78" r="4" fill="#b8c8d4"/></g>
          </defs>
          <g font-family="Segoe UI,Arial,sans-serif">
            <path d="M70 379 720 265l650 114-650 145Z" fill="none" stroke="#38536b" stroke-width="2"/>
            <path d="M89 380 720 274l629 106-629 132Z" fill="none" stroke="#6c8ba2" stroke-opacity=".28" stroke-dasharray="8 10"/>
            <path d="M160 501 720 604l560-103" fill="none" stroke="#8aa2b7" stroke-width="2" stroke-dasharray="10 12" opacity=".55"/>
            <path d="M207 359l79-49 753 0 87 49-87 61H286Z" fill="url(#__TASK___evc_canopy)" stroke="#64839a" stroke-width="3"/>
            <path d="M218 355l70-42h744l75 42-80 51H292Z" fill="url(#__TASK___evc_grid)" stroke="#8ac6e5" stroke-width="2"/>
            <path d="M300 402v68m743-68v68" stroke="#7893a7" stroke-width="12"/><path d="M283 468h35m706 0h36" stroke="#a4b7c5" stroke-width="9"/>
            <g transform="translate(228 398) scale(.86)"><use href="#__TASK___evc_charger"/></g>
            <g transform="translate(634 398) scale(.86)"><use href="#__TASK___evc_charger"/></g>
            <g transform="translate(1040 398) scale(.86)"><use href="#__TASK___evc_charger"/></g>
            <g fill="#e2e8f0" font-size="12" font-weight="600" letter-spacing="1"><text x="226" y="553">CHARGER 01</text><text x="632" y="553">CHARGER 02</text><text x="1038" y="553">CHARGER 03</text></g>
            <g fill="#8ba4bc" font-size="12"><text x="195" y="577">11 kW MAX / 85% TARGET</text><text x="601" y="577">11 kW MAX / 85% TARGET</text><text x="1007" y="577">11 kW MAX / 85% TARGET</text></g>
            <path d="M171 646 301 589l110 25-129 66Z" fill="#203a50" stroke="#57748a"/><path d="M282 680v67m112-108v63" stroke="#728ba0" stroke-width="6"/>
            <rect x="188" y="623" width="185" height="93" rx="14" fill="#102237" stroke="#3b5b73"/><rect x="201" y="635" width="160" height="44" rx="7" fill="#091521" stroke="#35526a"/>
            <path d="M217 658h23l12-13 11 24 14-18 10 7h57" fill="none" stroke="#38bdf8" stroke-width="3"/><circle cx="214" cy="696" r="4" fill="#34d399"/><text x="230" y="701" fill="#aac4d7" font-size="11">SITE LOAD MANAGER</text>
            <g fill="none" stroke="#8ca4b7" stroke-width="2" opacity=".65"><path d="M75 761h1290M94 774h1250"/><path d="M115 744v27m205-27v27m205-27v27m205-27v27m205-27v27m205-27v27m205-27v27"/></g>
            <g font-size="12" fill="#a9c1d3"><circle cx="820" cy="746" r="5" fill="#fbbf24"/><text x="834" y="750">AMBER / LIVE CHARGING</text><circle cx="1040" cy="746" r="5" fill="#354e64"/><text x="1054" y="750">GREY / IDLE BRANCH</text></g>
          </g>
        </svg>
        """;

    public static string Json { get; } = CreateJson();

    private static string CreateJson()
    {
        var b = new DemoScreenBuilder("EvCharging", "EV Charging Station", "EvChargingSimulation");
        b.Variable("RunRequested", "Boolean", false); b.Variable("ResetRequested", "Boolean", false);
        b.Variable("ArrivalRequested", "Boolean", false, description: "Click once to add the selected batch; paused arrivals wait for Start.");
        b.Variable("SiteLimitKw", "Double", 22.0, "kW", "Editable aggregate site import limit, 3 to 33 kW.");
        b.Variable("ArrivalBatch", "Int32", 1, "cars", "Manual arrival batch, 1 to 3 cars.");
        b.Variable("Hour", "Double", 8.0, "h", "Simulated time; one minute advances per 250 ms while running.");
        b.Variable("Cycle", "Double", 0.0, "cycles"); b.Variable("QueueCount", "Double", 0.0, "cars");
        b.Variable("Arrived", "Double", 3.0, "cars"); b.Variable("Departed", "Double", 0.0, "cars"); b.Variable("ActiveCount", "Double", 3.0, "cars");
        b.Variable("EnergyKwh", "Double", 0.0, "kWh", "Site energy delivered to vehicles; SOC gain includes 92% charging efficiency.");
        for (var i = 1; i <= 3; i++)
        {
            b.Variable("Present" + i, "Boolean", true, description: "A vehicle is visible only while occupying this stall.");
            b.Variable("Soc" + i, "Double", new[] { 0.0, 35.0, 55.0, 72.0 }[i], "%", "Vehicle SOC; target is 85%.");
            b.Variable("Charger" + i + "Kw", "Double", 0.0, "kW", "Positive branch power; fair site sharing, 11 kW charger limit.");
        }
        b.Variable("SitePowerKw", "Double", 0.0, "kW");
        b.Variable("Status", "String", "Paused - press Start / Pause");
        b.Header("EV CHARGING / SITE OPERATIONS", "Three independently metered 11 kW stalls / fair shared limit / 1 simulated minute per 250 ms cycle",
            "SITE POWER / TOTAL", "VEHICLES / ACTIVE", "ARRIVAL QUEUE", "ENERGY / DELIVERED");
        b.Pipe("site_idle", 331, 575, 825, 24, null, "#475569");
        b.Pipe("site_feed", 331, 575, 825, 24, "SitePowerKw", "#fbbf24", visible: "value > 0");
        b.Pipe("branch1_idle", 320, 530, 24, 57, null, "#475569");
        b.Pipe("branch1", 320, 530, 24, 57, "Charger1Kw", "#fbbf24", "top-bottom", "value > 0");
        b.Pipe("branch2_idle", 726, 530, 24, 57, null, "#475569");
        b.Pipe("branch2", 726, 530, 24, 57, "Charger2Kw", "#fbbf24", "top-bottom", "value > 0");
        b.Pipe("branch3_idle", 1132, 530, 24, 57, null, "#475569");
        b.Pipe("branch3", 1132, 530, 24, 57, "Charger3Kw", "#fbbf24", "top-bottom", "value > 0");
        b.Art("artwork", 0, 0, 1440, 1040, Artwork);
        b.Art("car1", 0, 0, 1440, 1040, Car(390, 530, "#7dd3fc"), "Present1", "value == true");
        b.Art("car2", 0, 0, 1440, 1040, Car(796, 530, "#a7f3d0"), "Present2", "value == true");
        b.Art("car3", 0, 0, 1440, 1040, Car(1202, 530, "#fda4af"), "Present3", "value == true");
        b.Display("site_power", 60, 140, 286, 48, "SitePowerKw", "kW", "#fde68a");
        b.Display("active_count", 404, 140, 286, 48, "ActiveCount", "vehicles connected", "#93c5fd", "F0");
        b.Display("queue_count", 748, 140, 286, 48, "QueueCount", "waiting", "#fdba74", "F0");
        b.Display("energy", 1092, 140, 286, 48, "EnergyKwh", "kWh", "#c4b5fd");
        for (var i = 1; i <= 3; i++)
        {
            var x = 230 + (i - 1) * 406;
            b.Display("soc" + i, x, 640, 150, 42, "Soc" + i, "%", "#c4b5fd", "F0");
            b.Display("power" + i, x, 688, 150, 38, "Charger" + i + "Kw", "kW", "#fde68a");
            b.Symbol("socbar" + i, "progressbar", x + 164, 649, 168, 24, new System.Text.Json.Nodes.JsonObject
            {
                ["VariablePath"] = b.Path + "Soc" + i, ["MinValue"] = 0, ["MaxValue"] = 100,
                ["ProgressBarOrientation"] = "horizontal", ["ProgressBarShowValue"] = false,
                ["ProgressBarTrackColor"] = "#14283a", ["Fill"] = "#34d399"
            });
        }
        b.Display("arrived", 185, 714, 142, 35, "Arrived", "arrived", "#93c5fd", "F0");
        b.Display("departed", 343, 714, 142, 35, "Departed", "departed", "#93c5fd", "F0");
        b.Display("clock", 505, 714, 142, 35, "Hour", "simulated h", "#e2e8f0", "F1");
        b.Text("arrivals_label", 638, 788, 140, "MANUAL BATCH", "#9fb8ca", 12);
        b.Text("site_limit_label", 820, 788, 170, "SITE LIMIT / kW", "#9fb8ca", 12);
        b.Button("run", 44, 820, 180, "Start / Pause", "RunRequested", action: "ToggleVariable", color: "#0f766e");
        b.Button("reset", 238, 820, 130, "Reset", "ResetRequested", "true", color: "#475569");
        b.Button("arrival", 400, 820, 180, "Add arrivals", "ArrivalRequested", "true", color: "#1d4ed8");
        b.Input("batch", 638, 817, 140, "ArrivalBatch", 1, 3, 1, "F0");
        b.Input("limit", 820, 817, 170, "SiteLimitKw", 3, 33, 1, "F0");
        b.Display("status", 44, 925, 1352, 58, "Status", color: "#e2e8f0", format: "");
        return b.Build(SimulationCode);
    }

    private static string Car(int x, int y, string accent)
    {
        var gradientId = "__TASK___evc_car_" + (x / 406 + 1);
        return $"""
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 1040"><defs><linearGradient id="{gradientId}" x2="0" y2="1"><stop stop-color="#f1f5f9"/><stop offset=".55" stop-color="#a9c3d5"/><stop offset="1" stop-color="#52748c"/></linearGradient></defs><g transform="translate({x} {y})"><path d="M-6 91l28-12 128 10 26 15-22 10-143-8Z" fill="#020617" opacity=".65"/><g><path d="M7 50l13-22q7-12 23-15h46q15 0 25 16l16 14 28 9q9 4 9 13v13H1V62q0-8 6-12Z" fill="url(#{gradientId})" stroke="{accent}" stroke-width="3"/><path d="M47 20h39q13 2 20 14l8 14H31l9-19q2-6 7-9Z" fill="#173d55" stroke="#b7e8fb" stroke-width="2"/><path d="M66 21v26m-34-2h83" stroke="#d4f1ff" stroke-opacity=".7"/><path d="M8 56h16m111-2 13 5" stroke="#fde68a" stroke-width="5"/><circle cx="36" cy="78" r="12" fill="#101a24" stroke="#92a9ba" stroke-width="3"/><circle cx="126" cy="78" r="12" fill="#101a24" stroke="#92a9ba" stroke-width="3"/><circle cx="36" cy="78" r="4" fill="#d5e3ed"/><circle cx="126" cy="78" r="4" fill="#d5e3ed"/></g><path d="M92 29q12 8 19 18" fill="none" stroke="#67e8f9" stroke-width="3"/><circle cx="112" cy="46" r="4" fill="#67e8f9"/></g></svg>
        """;
    }
}
