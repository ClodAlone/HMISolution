// Copyright (c) 2026 Claudio Fiorani - All rights reserved.
using System.Text.Json.Nodes;

namespace ServerEditorWeb.Services;

public static class ConveyorLineTemplate
{
    public static string Json { get; } = CreateJson();

    public const string SimulationCode = """
        const string p = "__TASK__.ConveyorLine.";
        // Four fixed slots: -1 empty; 0..99 good route; 200..299 reject route.
        // One update is one simulated second. No autonomous package animation.
        var reset = ReadBool(p + "ResetRequested");
        if (reset)
        {
            string[] names = { "Slot0", "Slot1", "Slot2", "Slot3", "CreatedTotal", "GoodCount", "RejectCount", "ArrivalClock", "Elapsed", "BatchSize", "Speed", "RejectEvery", "Progress", "BeltSpeed" };
            double[] defaults = { -1, -1, -1, -1, 0, 0, 0, 0, 0, 24, 4, 3, 0, 0 };
            for (int i = 0; i < names.Length; i++) Write(p + names[i], defaults[i]);
            Write(p + "RunRequested", false); Write(p + "ResetRequested", false);
            Write(p + "JamRequested", false); Write(p + "ClearJamRequested", false); Write(p + "Jammed", false);
        }
        double[] slots = { ReadDouble(p + "Slot0"), ReadDouble(p + "Slot1"), ReadDouble(p + "Slot2"), ReadDouble(p + "Slot3") };
        var created = ReadDouble(p + "CreatedTotal"); var good = ReadDouble(p + "GoodCount"); var reject = ReadDouble(p + "RejectCount");
        var clock = ReadDouble(p + "ArrivalClock"); var elapsed = ReadDouble(p + "Elapsed");
        var batch = ReadDouble(p + "BatchSize"); var speed = ReadDouble(p + "Speed"); var every = ReadDouble(p + "RejectEvery");
        double[] values = { created, good, reject, clock, elapsed, batch, speed, every, ReadDouble(p + "Progress"), ReadDouble(p + "BeltSpeed") };
        double[] maxima = { 200, 200, 200, 14, 1e12, 200, 8, 5, 100, 8 };
        bool valid = true; int active = 0;
        for (int i = 0; i < values.Length; i++) valid &= double.IsFinite(values[i]) && values[i] >= 0 && values[i] <= maxima[i];
        valid &= batch >= 1 && batch == Math.Truncate(batch) && speed >= 2 && speed % 2 == 0 && every >= 2 && every == Math.Truncate(every);
        valid &= created == Math.Truncate(created) && good == Math.Truncate(good) && reject == Math.Truncate(reject) && clock == Math.Truncate(clock);
        for (int i = 0; i < slots.Length; i++)
        {
            var s = slots[i];
            valid &= double.IsFinite(s) && (s == -1 || s % 2 == 0 && ((s >= 0 && s < 100) || (s >= 200 && s < 300)));
            if (s >= 0) active++;
        }
        valid &= good + reject + active == created;
        if (!valid) Write(p + "RunRequested", false);
        if (ReadBool(p + "JamRequested"))
        {
            Write(p + "Jammed", true); Write(p + "JamRequested", false);
        }
        else if (ReadBool(p + "ClearJamRequested")) Write(p + "Jammed", false);
        Write(p + "ClearJamRequested", false);
        bool jammed = ReadBool(p + "Jammed");
        bool moving = valid && ReadBool(p + "RunRequested") && !jammed && (created < batch || active > 0);
        bool arrival = false, sensor = false, diverter = false;
        string status = !valid ? "Invalid inputs - press Reset" : reset ? "Reset - press Start / Pause" : jammed ? "JAM - motor and arrivals interlocked / Clear jam" : "Paused - packages frozen";
        if (moving)
        {
            elapsed += 1;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] < 0) continue;
                bool bad = slots[i] >= 200;
                var position = slots[i] - (bad ? 200 : 0) + speed;
                if (position >= 100)
                {
                    if (bad) reject++; else good++;
                    slots[i] = -1; active--;
                }
                else slots[i] = position + (bad ? 200 : 0);
            }
            if (clock > 0) clock--;
            if (created < batch && clock == 0)
            {
                int free = -1;
                for (int i = 0; i < slots.Length; i++) if (free < 0 && slots[i] == -1) free = i;
                if (free >= 0)
                {
                    created++; slots[free] = created % every == 0 ? 200 : 0;
                    active++; clock = 14; arrival = true;
                }
            }
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] < 0) continue;
                var position = slots[i] >= 200 ? slots[i] - 200 : slots[i];
                sensor |= position >= 44 && position <= 56;
                diverter |= slots[i] >= 200 && position >= 64 && position <= 84;
            }
            for (int i = 0; i < slots.Length; i++) Write(p + "Slot" + i, slots[i]);
            Write(p + "CreatedTotal", created); Write(p + "GoodCount", good); Write(p + "RejectCount", reject);
            Write(p + "ArrivalClock", clock); Write(p + "Elapsed", elapsed);
            Write(p + "Progress", Math.Min(100, (good + reject) / batch * 100));
            status = "Running - detector / color sort / counted at exit only";
        }
        if (valid && !reset && !jammed && created >= batch && active == 0)
        {
            status = "Batch complete - Reset for a new batch";
            Write(p + "RunRequested", false); moving = false;
        }
        Write(p + "MotorOn", moving); Write(p + "ArrivalOn", moving && arrival);
        Write(p + "SensorOn", moving && sensor); Write(p + "DiverterOn", moving && diverter);
        Write(p + "BeltSpeed", moving ? speed : 0); Write(p + "Status", status);
        """;

    private static string CreateJson()
    {
        var b = new DemoScreenBuilder("ConveyorLine", "Conveyor Production Line", "ConveyorLineSimulation");
        foreach (var name in new[] { "RunRequested", "ResetRequested", "JamRequested", "ClearJamRequested", "Jammed", "MotorOn", "ArrivalOn", "SensorOn", "DiverterOn" }) b.Variable(name, "Boolean", false);
        for (var i = 0; i < 4; i++) b.Variable("Slot" + i, "Double", -1);
        string[] names = ["CreatedTotal", "GoodCount", "RejectCount", "ArrivalClock", "Elapsed", "BatchSize", "Speed", "RejectEvery", "Progress", "BeltSpeed"];
        double[] defaults = [0, 0, 0, 0, 0, 24, 4, 3, 0, 0];
        for (var i = 0; i < names.Length; i++) b.Variable(names[i], "Double", defaults[i]);
        b.Variable("Status", "String", "Paused - press Start / Pause");
        b.Header("CONVEYOR / SORTING CELL", "Deterministic slot tracking / cyan accepted, coral rejected / 1 simulated second per update", "CREATED / PACKAGES", "ACCEPTED / EXIT A", "REJECTED / EXIT B", "BATCH PROGRESS / %");
        b.Display("created", 62, 143, 270, 45, "CreatedTotal", "pcs", format: "F0"); b.Display("good", 406, 143, 270, 45, "GoodCount", "pcs", format: "F0");
        b.Display("reject", 750, 143, 270, 45, "RejectCount", "pcs", format: "F0"); b.Display("completed", 1094, 143, 270, 45, "Progress", "%");
        b.Symbol("belt", "conveyor", 70, 530, 210, 48, new JsonObject { ["VariablePath"] = b.Path + "MotorOn", ["ConveyorFaultPath"] = b.Path + "Jammed", ["ConveyorBeltColor"] = "#273b50", ["ConveyorRunColor"] = "#38bdf8", ["ConveyorAnimate"] = true, ["ConveyorSpeed"] = .6 });
        b.Art("artwork", 0, 230, 1440, 530, """
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 530">
            <defs><linearGradient id="__TASK___cl_belt" x2="0" y2="1"><stop stop-color="#49647c"/><stop offset="1" stop-color="#1d3046"/></linearGradient><linearGradient id="__TASK___cl_metal" x2="1" y2="1"><stop stop-color="#c5d6e0"/><stop offset=".5" stop-color="#7994a8"/><stop offset="1" stop-color="#36516a"/></linearGradient><pattern id="__TASK___cl_rollers" width="22" height="22" patternUnits="userSpaceOnUse" patternTransform="rotate(-9)"><path d="M0 0v22" stroke="#8194a9" stroke-width="3"/></pattern></defs>
            <g font-family="Segoe UI,Arial"><g fill="#dceaf3" font-size="17" font-weight="600"><text x="80" y="38">01 / METERED INFEED</text><text x="530" y="38">02 / OPTICAL INSPECTION</text><text x="1020" y="38">03 / TWO-WAY SORT</text></g>
            <path d="M135 326l980-148 115 61-980 148Z" fill="#14283f" stroke="#57718b" stroke-width="3"/><path d="M135 296l980-148 115 61-980 148Z" fill="url(#__TASK___cl_belt)" stroke="#a8c1d2" stroke-width="3"/><path d="M135 296l980-148 115 61-980 148Z" fill="url(#__TASK___cl_rollers)" opacity=".45"/><path d="M135 296v30l115 61v-30Z" fill="#6c8599"/><path d="M250 357v30l980-148v-30Z" fill="#2c455e" stroke="#7993a9"/>
            <g fill="url(#__TASK___cl_metal)"><path d="M250 387v78l16 8v-89Zm315-48v77l16 8v-88Zm350-53v80l16 8v-89Zm278-42v78l16 8v-88Z"/></g>
            <path d="M820 264l118-18 260 177-116 23Z" fill="#344e65" stroke="#89a6ba" stroke-width="3"/><path d="M820 264l262 182v25l-262-181Z" fill="#1a3047"/><path d="M1082 446l116-23v25l-116 23Z" fill="#6b8395"/>
            <path d="M570 267V144l108-18 0 126m-108-108 31 18 108-18-31-18m31 18v127" fill="none" stroke="url(#__TASK___cl_metal)" stroke-width="12"/><rect x="608" y="153" width="43" height="26" rx="5" fill="#173b50" stroke="#a7cfdf"/><circle cx="630" cy="166" r="8" fill="#39a1c5"/><path d="M629 183v74" stroke="#22d3ee" stroke-width="2" stroke-dasharray="5 6" opacity=".3"/>
            <path d="M813 235l59-8 26 15-58 9Z" fill="#f8cd76" stroke="#ffe4b5"/><circle cx="840" cy="236" r="9" fill="#c98736"/>
            <path d="M98 199v49l100 51 39-7v-50l-98-52Z" fill="#2d4a62" stroke="#809db2"/><path d="M98 199l100 51 39-8-98-52Z" fill="#8aa3b8"/><path d="M1094 142l134-22 70 36-132 23Z" fill="#184c56" stroke="#40cfca"/><path d="M1166 179v40l132-23v-40Z" fill="#123e49"/><text x="1160" y="109" fill="#67e8f9" font-size="15">A / ACCEPT</text>
            <path d="M1080 446l127-23 70 35-127 27Z" fill="#704552" stroke="#f39d97"/><path d="M1150 485v38l127-28v-37Z" fill="#442d40"/><text x="1240" y="414" fill="#fda4af" font-size="15">B / REJECT</text>
            <g fill="#9db9cc" font-size="13"><text x="70" y="495">Four persistent package slots / colors assigned on arrival / exits counted once</text><text x="70" y="518">Jam interlocks belt, motor and feeder. Clear jam retains package positions and route.</text></g></g></svg>
            """);
        b.Symbol("motor", "motorcontrol", 355, 580, 120, 90, new JsonObject { ["VariablePath"] = b.Path + "MotorOn", ["MotorFaultPath"] = b.Path + "Jammed", ["MotorStyle"] = "motor", ["MotorAppearance"] = "isometric", ["MotorUseGradient"] = true, ["MotorAnimateRotation"] = true });
        b.Symbol("sensor", "indicator", 612, 381, 30, 22, new JsonObject { ["VariablePath"] = b.Path + "SensorOn", ["Fill"] = "#334155", ["FillBinding"] = "value == true ? '#22d3ee' : '#334155'", ["Label"] = "DETECT" });
        b.Art("diverter_active", 805, 452, 110, 60, """<svg viewBox="0 0 110 60"><path d="M10 10l80 40 12-10-80-38Z" fill="#fb923c" stroke="#ffedd5" stroke-width="2"/></svg>""", "DiverterOn", "value == true");
        b.Art("motion_state", 0, 0, 1, 1, """<svg viewBox="0 0 1 1"><g data-conveyor-running="__TASK___cl"></g></svg>""", "MotorOn", "value == true");
        // Position poses retain deterministic simulation state; the browser interpolates their visible parcel groups.
        for (var slot = 0; slot < 4; slot++)
        for (var route = 0; route < 2; route++)
        for (var bin = 0; bin < 50; bin++)
        {
            var position = bin * 2;
            var x = 180 + position * 9.5;
            var y = 538 - position * 1.45;
            if (route == 1 && position >= 64) { x = 788 + (position - 64) * 8; y = 445 + (position - 64) * 6.2; }
            var top = route == 0 ? "#9af2ef" : "#ffb3a8";
            var left = route == 0 ? "#248a9c" : "#bc5c68";
            var right = route == 0 ? "#166477" : "#873e53";
            var lower = position + route * 200;
            var svg = $"<svg viewBox=\"0 0 66 62\"><g data-conveyor-line=\"__TASK___cl\" data-conveyor-parcel=\"{slot}\" data-position=\"{position}\" data-route=\"{route}\"><path d=\"M3 23l29-15 30 15-29 16Z\" fill=\"{top}\" stroke=\"#e3eef6\"/><path d=\"M3 23v22l30 15V39Z\" fill=\"{left}\"/><path d=\"M33 39l29-16v22L33 60Z\" fill=\"{right}\"/><path d=\"M17 16l30 16v10l-9 5V37L9 21Z\" fill=\"#f5deb0\" opacity=\".85\"/></g></svg>";
            b.Art($"package_{slot}_{route}_{bin}", x, y - 50, 66, 62, svg, "Slot" + slot, $"value == {lower}");
        }
        b.Text("motor_label", 325, 675, 235, "M-101 / DRIVE + JAM INTERLOCK");
        b.Display("speed_display", 620, 650, 200, 44, "BeltSpeed", "u/step", format: "F0");
        b.Symbol("progress", "progressbar", 1000, 718, 320, 24, new JsonObject { ["VariablePath"] = b.Path + "Progress", ["MinValue"] = 0, ["MaxValue"] = 100 });
        b.StandardControls(); b.Button("jam", 384, 820, 150, "Inject jam", "JamRequested", "true", color: "#924347");
        b.Button("clear", 548, 820, 150, "Clear jam", "ClearJamRequested", "true", color: "#0f766e");
        b.Text("batch_label", 730, 793, 190, "Batch / 1 - 200 pcs"); b.Input("batch", 730, 820, 190, "BatchSize", 1, 200, 1, "F0");
        b.Text("speed_label", 945, 793, 190, "Speed / 2, 4, 6, 8"); b.Input("speed", 945, 820, 190, "Speed", 2, 8, 2, "F0");
        b.Text("reject_label", 1160, 793, 210, "Reject every Nth / 2 - 5"); b.Input("every", 1160, 820, 210, "RejectEvery", 2, 5, 1, "F0");
        b.Text("legend", 44, 880, 1300, "CYAN accept / CORAL reject / Detector pulses at inspection / Diverter follows tracked reject packages / Reset begins a fresh paused batch");
        return b.Build(SimulationCode);
    }
}
