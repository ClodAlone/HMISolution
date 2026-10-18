// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

namespace ServerEditorWeb.Services;

public static class MicrogridTemplate
{
    public const string SimulationCode = """
        const string p = "__TASK__.Microgrid.";
        const double dtHours = 0.05;
        const double batteryCapacityKwh = 40.0;
        const double efficiency = 0.95;
        var reset = ReadBool(p + "ResetRequested");
        if (reset)
        {
            Write(p + "RunRequested", false); Write(p + "ResetRequested", false); Write(p + "IslandMode", false);
            Write(p + "Scenario", 0); Write(p + "ImportCapKw", 8.0); Write(p + "Hour", 12.0); Write(p + "BatterySoc", 55.0);
            Write(p + "ImportEnergyKwh", 0.0); Write(p + "OperatingCost", 0.0);
            Write(p + "PvKw", 0.0); Write(p + "PvUsedKw", 0.0); Write(p + "LoadTotalKw", 0.0);
            Write(p + "HouseDemandKw", 0.0); Write(p + "EvDemandKw", 0.0); Write(p + "HouseServedKw", 0.0); Write(p + "EvServedKw", 0.0);
            Write(p + "BatteryKw", 0.0); Write(p + "GridKw", 0.0); Write(p + "UnservedKw", 0.0); Write(p + "CurtailedPvKw", 0.0);
            Write(p + "Tariff", 0.12); Write(p + "ExportTariff", 0.04);
        }
        var scenario = ReadDouble(p + "Scenario");
        var cap = ReadDouble(p + "ImportCapKw");
        var hour = ReadDouble(p + "Hour");
        var soc = ReadDouble(p + "BatterySoc");
        var importEnergy = ReadDouble(p + "ImportEnergyKwh");
        var cost = ReadDouble(p + "OperatingCost");
        var island = ReadBool(p + "IslandMode");
        var valid = double.IsFinite(scenario) && scenario >= 0 && scenario <= 1 && scenario == Math.Truncate(scenario) &&
            double.IsFinite(cap) && cap >= 0.5 && cap <= 30 && double.IsFinite(hour) && hour >= 0 && hour < 24 &&
            double.IsFinite(soc) && soc >= 15 && soc <= 95 && double.IsFinite(importEnergy) && importEnergy >= 0 &&
            double.IsFinite(cost) && double.IsFinite(ReadDouble(p + "LoadTotalKw")) && ReadDouble(p + "LoadTotalKw") >= 0 && ReadDouble(p + "LoadTotalKw") <= 30 &&
            double.IsFinite(ReadDouble(p + "PvKw")) && ReadDouble(p + "PvKw") >= 0 && ReadDouble(p + "PvKw") <= 18 &&
            double.IsFinite(ReadDouble(p + "PvUsedKw")) && ReadDouble(p + "PvUsedKw") >= 0 && ReadDouble(p + "PvUsedKw") <= ReadDouble(p + "PvKw") + 1e-9 &&
            double.IsFinite(ReadDouble(p + "HouseDemandKw")) && ReadDouble(p + "HouseDemandKw") >= 0 && ReadDouble(p + "HouseDemandKw") <= 20 &&
            double.IsFinite(ReadDouble(p + "EvDemandKw")) && ReadDouble(p + "EvDemandKw") >= 0 && ReadDouble(p + "EvDemandKw") <= 10 &&
            double.IsFinite(ReadDouble(p + "HouseServedKw")) && ReadDouble(p + "HouseServedKw") >= 0 && ReadDouble(p + "HouseServedKw") <= ReadDouble(p + "HouseDemandKw") + 1e-9 &&
            double.IsFinite(ReadDouble(p + "EvServedKw")) && ReadDouble(p + "EvServedKw") >= 0 && ReadDouble(p + "EvServedKw") <= ReadDouble(p + "EvDemandKw") + 1e-9 &&
            double.IsFinite(ReadDouble(p + "GridKw")) && Math.Abs(ReadDouble(p + "GridKw")) <= 30 &&
            double.IsFinite(ReadDouble(p + "BatteryKw")) && ReadDouble(p + "BatteryKw") >= -8 && ReadDouble(p + "BatteryKw") <= 6 &&
            double.IsFinite(ReadDouble(p + "UnservedKw")) && ReadDouble(p + "UnservedKw") >= 0 && ReadDouble(p + "UnservedKw") <= ReadDouble(p + "HouseDemandKw") + ReadDouble(p + "EvDemandKw") + 1e-9 &&
            double.IsFinite(ReadDouble(p + "CurtailedPvKw")) && ReadDouble(p + "CurtailedPvKw") >= 0 && ReadDouble(p + "CurtailedPvKw") <= 18 &&
            double.IsFinite(ReadDouble(p + "Tariff")) && ReadDouble(p + "Tariff") >= 0 && ReadDouble(p + "Tariff") <= 1 &&
            double.IsFinite(ReadDouble(p + "ExportTariff")) && ReadDouble(p + "ExportTariff") >= 0 && ReadDouble(p + "ExportTariff") <= 1;
        var run = ReadBool(p + "RunRequested") && valid && !reset;
        var pv = 0.0; var pvUsed = 0.0; var house = 0.0; var ev = 0.0; var houseServed = 0.0; var evServed = 0.0;
        var battery = 0.0; var grid = 0.0; var unserved = 0.0; var curtailed = 0.0;
        var price = scenario == 0 ? 0.12 : 0.48;
        var exportPrice = scenario == 0 ? 0.04 : 0.16;
        var status = !valid ? "Invalid numeric input/state - press Reset" : reset ? "Reset - press Start / Pause" : island ? "Islanded - paused / grid flows locked out" : "Paused - press Start / Pause";
        if (!valid) Write(p + "RunRequested", false);
        if (run)
        {
            hour = (hour + dtHours) % 24.0;
            pv = 18.0 * Math.Max(0.0, Math.Sin(Math.PI * (hour - 6.0) / 12.0));
            house = 5.5 + (hour >= 17 && hour < 22 ? 2.2 : 0.0) + 0.4 * (1.0 + Math.Sin(hour * 2.0));
            ev = 3.5;
            var demand = house + ev;
            var surplus = Math.Max(0.0, pv - demand);
            var deficit = Math.Max(0.0, demand - pv);
            var chargeRoomKw = Math.Max(0.0, (95.0 - soc) / 100.0 * batteryCapacityKwh / (dtHours * efficiency));
            var dischargeRoomKw = Math.Max(0.0, (soc - 15.0) / 100.0 * batteryCapacityKwh * efficiency / dtHours);
            if (island)
            {
                if (surplus > 0)
                {
                    battery = Math.Min(surplus, Math.Min(6.0, chargeRoomKw));
                    curtailed = surplus - battery;
                }
                else
                {
                    battery = -Math.Min(deficit, Math.Min(8.0, dischargeRoomKw));
                    unserved = deficit + battery;
                }
            }
            else if (surplus > 0)
            {
                battery = Math.Min(surplus, Math.Min(6.0, chargeRoomKw));
                grid = -(surplus - battery);
            }
            else if (scenario == 1)
            {
                var shave = Math.Max(0.0, deficit - cap);
                battery = -Math.Min(shave, Math.Min(8.0, dischargeRoomKw));
                var residual = deficit + battery;
                grid = Math.Min(cap, residual);
                unserved = Math.Max(0.0, residual - grid);
            }
            else
            {
                var gridChargeHeadroom = Math.Min(4.0, Math.Min(chargeRoomKw, Math.Max(0.0, cap - deficit)));
                battery = gridChargeHeadroom;
                grid = Math.Min(cap, deficit + battery);
                unserved = Math.Max(0.0, deficit + battery - grid);
            }
            if (unserved < 1e-9) unserved = 0.0;
            if (curtailed < 1e-9) curtailed = 0.0;
            pvUsed = Math.Clamp(pv - curtailed, 0.0, pv);
            if (unserved == 0.0)
            {
                houseServed = house; evServed = ev;
            }
            else
            {
                // Proportional shedding; clamp so rounding can never report more served than demanded.
                var servedShare = Math.Clamp((demand - unserved) / demand, 0.0, 1.0);
                houseServed = Math.Min(house, house * servedShare);
                evServed = Math.Min(ev, ev * servedShare);
            }
            soc = Math.Clamp(soc + (battery >= 0 ? battery * efficiency : battery / efficiency) * dtHours / batteryCapacityKwh * 100.0, 15.0, 95.0);
            importEnergy += Math.Max(0.0, grid) * dtHours;
            cost += (Math.Max(0.0, grid) * price - Math.Max(0.0, -grid) * exportPrice) * dtHours;
            status = island ? unserved > 0 ? "Island mode - unmet load tracked / no grid exchange" : curtailed > 0 ? "Island mode - PV surplus curtailed / no grid exchange" : "Island mode - local PV and battery dispatch" :
                scenario == 0 ? "Cheap tariff 0.12/kWh - PV first / spare import charges battery" : "Peak tariff 0.48/kWh - battery shaves imports to editable cap";
            Write(p + "RunRequested", true); Write(p + "Hour", hour); Write(p + "BatterySoc", soc);
            Write(p + "ImportEnergyKwh", importEnergy); Write(p + "OperatingCost", cost);
        }
        else
        {
            if (!valid) status = "Invalid numeric input/state - press Reset";
            Write(p + "RunRequested", false);
        }
        Write(p + "PvKw", pv); Write(p + "PvUsedKw", pvUsed); Write(p + "LoadTotalKw", house + ev); Write(p + "HouseDemandKw", house); Write(p + "EvDemandKw", ev);
        Write(p + "HouseServedKw", houseServed); Write(p + "EvServedKw", evServed); Write(p + "BatteryKw", battery);
        Write(p + "GridKw", grid); Write(p + "UnservedKw", unserved); Write(p + "CurtailedPvKw", curtailed);
        Write(p + "Tariff", price); Write(p + "ExportTariff", exportPrice);
        Write(p + "GridMode", island ? run ? "ISLANDED" : "ISLANDED / PAUSED" : grid > 0 ? "IMPORT" : grid < 0 ? "EXPORT" : run ? "CONNECTED / BALANCED" : "GRID READY / PAUSED");
        Write(p + "BatteryMode", battery > 0 ? "CHARGING" : battery < 0 ? "DISCHARGING" : "STANDBY");
        Write(p + "Status", status);
        """;

    private const string Artwork = """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 1040">
          <defs>
            <linearGradient id="__TASK___mg_panel" x2=".6" y2="1"><stop stop-color="#3189b8"/><stop offset=".5" stop-color="#175179"/><stop offset="1" stop-color="#102746"/></linearGradient>
            <linearGradient id="__TASK___mg_glass" x2="1" y2="1"><stop stop-color="#b7efff" stop-opacity=".52"/><stop offset=".6" stop-color="#38bdf8" stop-opacity=".05"/><stop offset="1" stop-color="#b7efff" stop-opacity=".22"/></linearGradient>
            <linearGradient id="__TASK___mg_house" x2=".2" y2="1"><stop stop-color="#e7eef2"/><stop offset="1" stop-color="#8098a9"/></linearGradient>
            <linearGradient id="__TASK___mg_battery" x2="1" y2=".4"><stop stop-color="#687f91"/><stop offset=".5" stop-color="#dae4e9"/><stop offset="1" stop-color="#71899b"/></linearGradient>
            <pattern id="__TASK___mg_cells" width="26" height="21" patternUnits="userSpaceOnUse"><path d="M0 0H26V21H0Z" fill="none" stroke="#83d6f3" stroke-opacity=".55"/><path d="M13 0V21" stroke="#d9f7ff" stroke-opacity=".38"/></pattern>
            <g id="__TASK___mg_solar"><path d="M0 76 35 92v15L0 91Zm208 0-35 16v15l35-16Z" fill="#667d8d"/><path d="M0 0 198 0l10 76L10 76Z" fill="url(#__TASK___mg_panel)" stroke="#a8d9ed" stroke-width="3"/><path d="M0 0 198 0l10 76L10 76Z" fill="url(#__TASK___mg_cells)"/><path d="M0 0 198 0l10 76L10 76Z" fill="url(#__TASK___mg_glass)"/><path d="M0 0 10 76m57-76 4 76m58-76 0 76m57-76-4 76m-163-38h190" fill="none" stroke="#9adcf3" stroke-opacity=".4"/></g>
            <g id="__TASK___mg_inverter"><path d="M0 10 17 0h104v156H0Z" fill="#334d63" stroke="#93aebf"/><rect x="10" y="8" width="98" height="143" rx="9" fill="url(#__TASK___mg_battery)" stroke="#d5e3eb"/><rect x="22" y="22" width="74" height="49" rx="6" fill="#0b1d2c"/><path d="M29 52q9-23 18 0t18 0t18 0" fill="none" stroke="#67e8f9" stroke-width="3"/><circle cx="35" cy="88" r="5" fill="#34d399"/><circle cx="53" cy="88" r="5" fill="#fbbf24"/><path d="M23 108h70m-70 9h70m-70 9h70" stroke="#62798b" stroke-width="3"/></g>
            <g id="__TASK___mg_car"><path d="M3 44 18 24q8-11 23-12h43q16 0 25 15l12 14 23 7q7 3 7 11v14H0V54q0-7 3-10Z" fill="#bfdbeb" stroke="#e4f7ff" stroke-width="2"/><path d="M45 18h35q11 1 17 12l7 13H29l9-18q2-6 7-7Z" fill="#183c53" stroke="#a4dbf2"/><path d="M64 18v25" stroke="#bcecff"/><circle cx="32" cy="70" r="10" fill="#111a23" stroke="#9bb0be" stroke-width="3"/><circle cx="115" cy="70" r="10" fill="#111a23" stroke="#9bb0be" stroke-width="3"/></g>
          </defs>
          <g font-family="Segoe UI,Arial,sans-serif">
            <path d="M108 447 351 365l221 93-236 91Z" fill="none" stroke="#49667c" stroke-width="3"/>
            <g transform="translate(144 365) skewX(-14) scale(.94 .83)"><use href="#__TASK___mg_solar"/><use href="#__TASK___mg_solar" x="224"/><use href="#__TASK___mg_solar" y="112"/><use href="#__TASK___mg_solar" x="224" y="112"/></g>
            <text x="135" y="578" fill="#e2e8f0" font-size="16" font-weight="700" letter-spacing="1">01 / PV FIELD</text><text x="135" y="597" fill="#8ba4bc" font-size="12">Daylight curve / locally available generation</text>
            <g transform="translate(594 365)"><use href="#__TASK___mg_inverter"/><use href="#__TASK___mg_inverter" x="122"/></g>
            <text x="600" y="543" fill="#e2e8f0" font-size="14" font-weight="700">MICROGRID AC BUS</text>
            <path d="M1005 353 1116 291l132 56-113 68Z" fill="#31536a" stroke="#8daaba" stroke-width="2"/><path d="M1005 353v100l130 45v-83Z" fill="url(#__TASK___mg_house)" stroke="#c7d8e2"/><path d="M1135 415l113-68v95l-113 56Z" fill="#6d8a9e" stroke="#a5bdcc"/><path d="m989 350 126-78 146 65-18 17-129-53-108 66Z" fill="#203e55" stroke="#7da3ba" stroke-width="2"/><path d="m1138 303 72 32-48 19-71-34Z" fill="#18527d" stroke="#7dd3fc"/><path d="M1030 379l28 9v40l-28-10Zm58 18 26 9v39l-26-9Z" fill="#fde68a" stroke="#fff1bf"/><path d="m1163 425 23-13v43l-23 12Z" fill="#67e8f9"/><text x="1010" y="258" fill="#e2e8f0" font-size="16" font-weight="700">02 / COMMUNITY HOUSE</text>
            <g transform="translate(600 585) scale(.68)"><path d="M0 16 23 0h186l22 16v151H0Z" fill="#263f54" stroke="#718ca0" stroke-width="2"/><rect x="14" y="15" width="200" height="143" rx="9" fill="url(#__TASK___mg_battery)" stroke="#c0d0da"/><rect x="29" y="30" width="42" height="112" rx="5" fill="#091522" stroke="#4a667b"/><path d="M50 42v84" stroke="#34d399" stroke-width="7"/><g fill="#1d3447" stroke="#71889a"><rect x="85" y="31" width="113" height="24" rx="4"/><rect x="85" y="61" width="113" height="24" rx="4"/><rect x="85" y="91" width="113" height="24" rx="4"/><rect x="85" y="121" width="113" height="24" rx="4"/></g></g><text x="430" y="618" fill="#e2e8f0" font-size="14" font-weight="700">03 / 40 kWh BATTERY</text>
            <text x="1180" y="590" fill="#e2e8f0" font-size="14" font-weight="700">04 / EV CHARGING</text><path d="M1267 613 1308 594l51 16v91l-51 19-41-17Z" fill="#314b62" stroke="#829aad" stroke-width="2"/><path d="M1267 613l41 16 51-19m-92 36 41 17 51-19" fill="none" stroke="#91a9ba"/><g transform="translate(1264 644) scale(.57)"><use href="#__TASK___mg_car"/></g>
            <path d="M1355 397v91m-43-69 43-22 43 22m-83 15h80m-71 17h62m-52 18h42m-21-71v113m-42-74 84 67m-84 0 84-67" fill="none" stroke="#9ab0bf" stroke-width="4" stroke-linecap="round"/><path d="M1309 398h92m-82 13h72" stroke="#c5d4df" stroke-width="4"/><text x="1310" y="385" fill="#e2e8f0" font-size="13" font-weight="700">UTILITY</text>
            <g font-size="12" fill="#a9c1d3"><circle cx="60" cy="714" r="5" fill="#fbbf24"/><text x="74" y="718">PV / LOCAL GENERATION</text><circle cx="270" cy="714" r="5" fill="#a78bfa"/><text x="284" y="718">BATTERY CHARGE</text><circle cx="430" cy="714" r="5" fill="#34d399"/><text x="444" y="718">BATTERY SUPPORT</text><circle cx="600" cy="714" r="5" fill="#22d3ee"/><text x="614" y="718">GRID IMPORT</text><circle cx="740" cy="714" r="5" fill="#fb923c"/><text x="754" y="718">GRID EXPORT</text><circle cx="880" cy="714" r="5" fill="#60a5fa"/><text x="894" y="718">HOUSE / EV LOAD</text><text x="1050" y="718" fill="#fda4af">RED READOUTS = UNMET / CURTAILED</text></g>
          </g>
        </svg>
        """;

    public static string Json { get; } = CreateJson();

    private static string CreateJson()
    {
        var b = new DemoScreenBuilder("Microgrid", "Microgrid Control Center", "MicrogridSimulation");
        b.Variable("RunRequested", "Boolean", false); b.Variable("ResetRequested", "Boolean", false);
        b.Variable("IslandMode", "Boolean", false, description: "Disconnects all utility exchange; deficit and PV curtailment remain visible.");
        b.Variable("Scenario", "Int32", 0, description: "0 cheap tariff 0.12/kWh; 1 peak tariff 0.48/kWh.");
        b.Variable("ImportCapKw", "Double", 8.0, "kW", "Peak import cap, editable from 0.5 to 30 kW.");
        b.Variable("Hour", "Double", 12.0, "h"); b.Variable("BatterySoc", "Double", 55.0, "%", "40 kWh storage; SOC limits 15 to 95%; 95% one-way efficiency.");
        b.Variable("ImportEnergyKwh", "Double", 0.0, "kWh"); b.Variable("OperatingCost", "Double", 0.0, "currency");
        b.Variable("PvKw", "Double", 0.0, "kW", "Gross PV availability before curtailment.");
        b.Variable("PvUsedKw", "Double", 0.0, "kW"); b.Variable("LoadTotalKw", "Double", 0.0, "kW"); b.Variable("HouseDemandKw", "Double", 0.0, "kW");
        b.Variable("EvDemandKw", "Double", 0.0, "kW"); b.Variable("HouseServedKw", "Double", 0.0, "kW");
        b.Variable("EvServedKw", "Double", 0.0, "kW"); b.Variable("BatteryKw", "Double", 0.0, "kW", "Positive charging; negative discharging.");
        b.Variable("GridKw", "Double", 0.0, "kW", "Positive import; negative export. Island mode forces zero.");
        b.Variable("UnservedKw", "Double", 0.0, "kW", "Demand not served by PV, battery, or capped grid import.");
        b.Variable("CurtailedPvKw", "Double", 0.0, "kW", "PV curtailed in island mode when local load and battery cannot absorb it.");
        b.Variable("Tariff", "Double", 0.12, "currency/kWh"); b.Variable("ExportTariff", "Double", 0.04, "currency/kWh");
        b.Variable("GridMode", "String", "GRID READY / PAUSED"); b.Variable("BatteryMode", "String", "STANDBY");
        b.Variable("Status", "String", "Paused - press Start / Pause");
        b.Header("MICROGRID / CONTROL CENTER", "Local PV + storage dispatch / house + EV demand / editable peak import cap / 24 h in 120 s / island model",
            "PV / AVAILABLE", "LOAD / HOUSE + EV", "GRID / +IMPORT -EXPORT", "BATTERY / STATE OF CHARGE");
        b.Pipe("pv_idle", 400, 557, 437, 24, null, "#fbbf24"); b.Pipe("pv", 400, 557, 437, 24, "PvUsedKw", "#fbbf24", visible: "value > 0");
        b.Pipe("house_idle", 837, 390, 169, 24, null, "#60a5fa"); b.Pipe("house", 837, 390, 169, 24, "HouseServedKw", "#60a5fa", visible: "value > 0");
        b.Pipe("bus", 837, 365, 24, 289, null, "#fbbf24");
        b.Pipe("ev_idle", 837, 617, 417, 24, null, "#38bdf8"); b.Pipe("ev", 837, 617, 417, 24, "EvServedKw", "#38bdf8", visible: "value > 0");
        b.Pipe("battery_idle", 757, 630, 80, 24, null, "#a78bfa");
        b.Pipe("battery_charge", 757, 630, 80, 24, "BatteryKw", "#a78bfa", "right-left", "value > 0");
        b.Pipe("battery_discharge", 757, 630, 80, 24, "BatteryKw", "#34d399", "left-right", "value < 0");
        b.Pipe("grid_idle", 837, 500, 465, 24, null, "#22d3ee");
        b.Pipe("grid_import", 837, 500, 465, 24, "GridKw", "#22d3ee", "right-left", "value > 0");
        b.Pipe("grid_export", 837, 500, 465, 24, "GridKw", "#fb923c", "left-right", "value < 0");
        b.Art("artwork", 0, 0, 1440, 1040, Artwork);
        b.Display("pv_total", 60, 140, 286, 48, "PvKw", "kW", "#fde68a");
        b.Display("load_total", 404, 140, 286, 48, "LoadTotalKw", "house + EV kW", "#93c5fd");
        b.Display("grid_total", 748, 140, 286, 48, "GridKw", "kW", "#fdba74");
        b.Display("soc_total", 1092, 140, 286, 48, "BatterySoc", "%", "#c4b5fd");
        b.Display("pv_used", 135, 612, 200, 38, "PvUsedKw", "kW to bus", "#fde68a");
        b.Display("house_served", 1003, 534, 160, 36, "HouseServedKw", "kW served", "#93c5fd");
        b.Display("battery_mode", 430, 628, 160, 32, "BatteryMode", format: "", color: "#c4b5fd");
        b.Display("battery_power", 430, 664, 160, 34, "BatteryKw", "kW (+ charge)", "#c4b5fd");
        b.Display("grid_mode", 1236, 534, 164, 32, "GridMode", format: "", color: "#fdba74");
        b.Display("ev_served", 1040, 652, 200, 36, "EvServedKw", "EV kW served", "#7dd3fc");
        b.Display("tariff", 44, 736, 200, 36, "Tariff", "currency/kWh", "#fde68a", "F2");
        b.Display("cost", 260, 736, 200, 36, "OperatingCost", "currency", "#a7f3d0");
        b.Display("import_energy", 476, 736, 200, 36, "ImportEnergyKwh", "kWh imported", "#67e8f9");
        b.Display("unserved", 692, 736, 200, 36, "UnservedKw", "kW unmet", "#fda4af");
        b.Display("curtailed", 908, 736, 200, 36, "CurtailedPvKw", "kW curtailed", "#fdba74");
        b.Display("clock", 44, 866, 190, 36, "Hour", "simulated h", "#e2e8f0");
        b.Text("cap_label", 1023, 788, 178, "PEAK IMPORT CAP / kW", "#9fb8ca", 12);
        b.Button("run", 44, 820, 180, "Start / Pause", "RunRequested", action: "ToggleVariable", color: "#0f766e");
        b.Button("reset", 238, 820, 130, "Reset", "ResetRequested", "true", color: "#475569");
        b.Button("cheap", 400, 820, 150, "Cheap tariff", "Scenario", "0", color: "#166534");
        b.Button("peak", 565, 820, 150, "Peak / shave", "Scenario", "1", color: "#9a3412");
        b.Button("island", 730, 820, 190, "Island / grid", "IslandMode", action: "ToggleVariable", color: "#5b4486");
        b.Input("cap", 1023, 817, 178, "ImportCapKw", 0.5, 30, 0.5, "F1");
        b.Display("status", 44, 925, 1352, 58, "Status", color: "#e2e8f0", format: "");
        return b.Build(SimulationCode);
    }
}
