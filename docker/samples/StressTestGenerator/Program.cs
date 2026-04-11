using System.Text.Json;

// ─── Stress-test project generator for HMI Solution ────────────────────────
// Generates nodes.json files with 1K, 10K, 100K, and 1M variables.
// Each variable includes: Simulation driver, Alarm, Anomaly Detection,
// and DataLogging (every 1s, MaxAge 10 min).

var samplesRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

var scenarios = new (string Name, int Count, int MaxAlarms)[]
{
    ("StressTest_1K",   1_000,     2_000),
    ("StressTest_10K",  10_000,    3_000),
    ("StressTest_100K", 100_000,   4_000),
    ("StressTest_1M",   1_000_000, 5_000),
};

string[] simulationFunctions = ["Sine", "Cosine", "Triangle", "Square", "Sawtooth", "Random", "Ramp", "Counter"];
string[] variableTypes = ["Double", "Double", "Double", "Double", "Double", "Double", "Double", "Double"];

foreach (var (name, totalCount, maxAlarms) in scenarios)
{
    var dir = Path.Combine(samplesRoot, name);
    Directory.CreateDirectory(dir);
    var filePath = Path.Combine(dir, "nodes.json");

    Console.WriteLine($"Generating {name} ({totalCount:N0} variables, {Math.Min(maxAlarms, totalCount):N0} alarms) ...");

    using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 65536);
    using var writer = new Utf8JsonWriter(fs, new JsonWriterOptions { Indented = false });

    writer.WriteStartObject(); // root

    // ── Database (Sqlite for portability) ──
    writer.WritePropertyName("Database");
    writer.WriteStartObject();
    writer.WriteString("Provider", "Sqlite");
    writer.WriteString("ConnectionString", "history.db");
    writer.WriteString("TableName", "variable_data");
    writer.WriteEndObject();

    // ── Empty collections ──
    writer.WritePropertyName("Users");
    writer.WriteStartArray(); writer.WriteEndArray();

    writer.WritePropertyName("UserGroups");
    writer.WriteStartArray(); writer.WriteEndArray();

    writer.WritePropertyName("Scripts");
    writer.WriteStartArray(); writer.WriteEndArray();

    writer.WritePropertyName("PlcPrograms");
    writer.WriteStartArray(); writer.WriteEndArray();

    // ── Screens ──
    writer.WritePropertyName("Screens");
    writer.WriteStartArray();

    WriteScreens(writer, totalCount);

    writer.WriteEndArray();

    writer.WritePropertyName("Recipes");
    writer.WriteStartArray(); writer.WriteEndArray();

    writer.WritePropertyName("Strings");
    writer.WriteStartArray(); writer.WriteEndArray();

    writer.WritePropertyName("Images");
    writer.WriteStartArray(); writer.WriteEndArray();

    writer.WritePropertyName("Cameras");
    writer.WriteStartArray(); writer.WriteEndArray();

    writer.WritePropertyName("Schedulers");
    writer.WriteStartArray(); writer.WriteEndArray();

    writer.WritePropertyName("Reports");
    writer.WriteStartArray(); writer.WriteEndArray();

    writer.WritePropertyName("CalculatedVariables");
    writer.WriteStartArray(); writer.WriteEndArray();

    // ── Server settings with AnomalyDetection enabled ──
    writer.WritePropertyName("Server");
    writer.WriteStartObject();
    writer.WriteString("EndpointUrl", "opc.tcp://localhost:14840/SimpleOpcFileServer");
    writer.WriteBoolean("EnableAnonymous", true);

    writer.WritePropertyName("AnomalyDetection");
    writer.WriteStartObject();
    writer.WriteBoolean("Enabled", true);
    writer.WriteNumber("IntervalSeconds", 10);  // aggressive scan interval for stress
    writer.WriteNumber("MinSampleCount", 5);
    writer.WriteEndObject();

    writer.WriteEndObject(); // Server

    // ── Root Folder ──
    // Strategy: group variables into sub-folders of up to 1000 each
    // Structure: Plant / Area_NNN / Var_NNNNNN
    int varsPerFolder = 1000;
    int folderCount = (int)Math.Ceiling((double)totalCount / varsPerFolder);

    writer.WritePropertyName("Folder");
    writer.WriteStartObject();
    writer.WriteString("Name", "Plant");

    // Sub-folders
    writer.WritePropertyName("Folders");
    writer.WriteStartArray();

    int globalVarIndex = 0;
    for (int f = 0; f < folderCount; f++)
    {
        writer.WriteStartObject();
        writer.WriteString("Name", $"Area_{f:D4}");

        writer.WritePropertyName("Folders");
        writer.WriteStartArray(); writer.WriteEndArray(); // no deeper nesting

        writer.WritePropertyName("Variables");
        writer.WriteStartArray();

        int varsInThisFolder = Math.Min(varsPerFolder, totalCount - globalVarIndex);
        for (int v = 0; v < varsInThisFolder; v++)
        {
            int funcIdx = globalVarIndex % simulationFunctions.Length;
            string simFunc = simulationFunctions[funcIdx];
            double amplitude = 50.0 + (globalVarIndex % 50);        // 50-99
            double offset = (globalVarIndex % 30);                    // 0-29
            int period = 5000 + (globalVarIndex % 10) * 1000;        // 5s-14s
            double phase = (globalVarIndex % 360);
            double highLimit = 80.0 + (globalVarIndex % 20);          // 80-99
            double lowLimit = 5.0 + (globalVarIndex % 15);            // 5-19
            double highHighLimit = highLimit + 10;
            double lowLowLimit = Math.Max(0, lowLimit - 5);

            writer.WriteStartObject();
            writer.WriteString("Name", $"Var_{globalVarIndex:D7}");
            writer.WriteString("Type", "Double");
            writer.WriteString("Access", "ReadWrite");
            writer.WriteNumber("Value", 0.0);
            writer.WriteNull("MaxAge");

            // ── Alarm (Limit-based) — only for the first maxAlarms variables ──
            if (globalVarIndex < maxAlarms)
            {
            writer.WritePropertyName("Alarm");
            writer.WriteStartObject();
            writer.WriteString("TriggerType", "Limit");
            writer.WriteNumber("HighHighLimit", highHighLimit);
            writer.WriteNumber("HighLimit", highLimit);
            writer.WriteNumber("LowLimit", lowLimit);
            writer.WriteNumber("LowLowLimit", lowLowLimit);
            writer.WriteNumber("Hysteresis", 2.0);
            writer.WriteString("Message", $"Alarm on Var_{globalVarIndex:D7}");
            writer.WriteBoolean("NotifyOnActivation", globalVarIndex % 100 == 0); // every 100th
            writer.WriteEndObject();
            }

            // ── DataLogging (every 1s poll, MaxAge 10 min) ──
            writer.WritePropertyName("DataLogging");
            writer.WriteStartObject();
            writer.WriteBoolean("Enabled", true);
            writer.WriteNumber("Hysteresis", 0);            // log every change
            writer.WriteString("MaxAge", "00:10:00");        // 10 minutes
            writer.WriteEndObject();

            // ── AnomalyDetection ──
            writer.WritePropertyName("AnomalyDetection");
            writer.WriteStartObject();
            writer.WriteBoolean("Enabled", true);
            writer.WriteNumber("Sensitivity", 3.0);
            writer.WriteNumber("WindowMinutes", 5);
            writer.WriteBoolean("EnableTrendPrediction", true);
            writer.WriteNumber("PredictionHorizonMinutes", 5);
            writer.WriteNumber("CooldownSeconds", 60);
            writer.WriteNumber("Severity", (ushort)(500 + globalVarIndex % 500)); // 500-999
            writer.WriteEndObject();

            // ── Simulation driver ──
            writer.WritePropertyName("Simulation");
            writer.WriteStartObject();
            writer.WriteString("Function", simFunc);
            writer.WriteNumber("Period", period);
            writer.WriteNumber("Amplitude", amplitude);
            writer.WriteNumber("Offset", offset);
            writer.WriteNumber("Phase", phase);
            writer.WriteNumber("PollTime", 1000);             // every 1 second
            writer.WriteNumber("Min", 0);
            writer.WriteNumber("Max", amplitude + offset);
            writer.WriteNumber("Step", 1);
            writer.WriteNumber("DutyCycle", 0.5);
            writer.WriteEndObject();

            writer.WriteEndObject(); // variable

            globalVarIndex++;
        }

        writer.WriteEndArray();  // Variables
        writer.WriteEndObject(); // folder
    }

    writer.WriteEndArray(); // Folders

    // Root-level variables (empty)
    writer.WritePropertyName("Variables");
    writer.WriteStartArray(); writer.WriteEndArray();

    writer.WriteEndObject(); // Folder (Plant)
    writer.WriteEndObject(); // root

    writer.Flush();
    fs.Flush();

    var fileInfo = new FileInfo(filePath);
    Console.WriteLine($"  -> {filePath}  ({fileInfo.Length / (1024.0 * 1024.0):F1} MB)");
}

Console.WriteLine("Done! All stress test projects generated.");

// ── Screen generation helpers ──────────────────────────────────────────────

static void WriteScreens(Utf8JsonWriter writer, int totalCount)
{
    // Screen 1: 10 editboxes
    WriteEditboxScreen(writer, "Editbox_10", 10, totalCount);
    // Screen 2: 100 editboxes
    WriteEditboxScreen(writer, "Editbox_100", 100, totalCount);
    // Screen 3: 1000 editboxes
    WriteEditboxScreen(writer, "Editbox_1000", 1000, totalCount);
    // Screen 4: 10000 editboxes
    WriteEditboxScreen(writer, "Editbox_10000", 10000, totalCount);
    // Screen 5: HDA chart + HDA grid
    WriteChartGridScreen(writer, totalCount);
}

static string VarPath(int varIndex)
{
    // Root folder "Plant" is transparent in OPC — the server strips it from node paths.
    // OPC node ID is "Area_NNNN.Var_NNNNNNN", not "Plant.Area_NNNN.Var_NNNNNNN".
    int area = varIndex / 1000;
    return $"Area_{area:D4}.Var_{varIndex:D7}";
}

static void WriteEditboxScreen(Utf8JsonWriter writer, string screenName,
    int editboxCount, int totalCount)
{
    // Clamp to available variables
    int count = Math.Min(editboxCount, totalCount);

    // Layout: grid of editboxes, 10 columns
    int cols = 10;
    int cellW = 190;
    int cellH = 60;
    int padX = 2;
    int padY = 2;
    int rows = (int)Math.Ceiling((double)count / cols);

    // Screen dimensions to fit all editboxes
    int screenW = cols * (cellW + padX) + 20;
    int screenH = rows * (cellH + padY) + 20;

    writer.WriteStartObject();
    writer.WriteString("Name", screenName);
    writer.WriteNumber("Width", screenW);
    writer.WriteNumber("Height", screenH);
    writer.WriteString("Background", "#0d1117");

    writer.WritePropertyName("Symbols");
    writer.WriteStartArray();

    for (int i = 0; i < count; i++)
    {
        int col = i % cols;
        int row = i / cols;
        int x = 10 + col * (cellW + padX);
        int y = 10 + row * (cellH + padY);

        writer.WriteStartObject();
        writer.WriteString("Id", $"eb_{i}");
        writer.WriteString("Type", "editbox");
        writer.WriteNumber("X", x);
        writer.WriteNumber("Y", y);
        writer.WriteNumber("Width", cellW);
        writer.WriteNumber("Height", cellH);
        writer.WriteString("VariablePath", VarPath(i));
        writer.WriteString("Label", $"Var_{i:D7}");
        writer.WriteNumber("EditBoxStep", 1);
        writer.WriteString("EditBoxFormat", "F1");
        writer.WriteBoolean("EditBoxShowStatistics", false);
        writer.WriteEndObject();
    }

    writer.WriteEndArray(); // Symbols
    writer.WriteEndObject(); // Screen
}

static void WriteChartGridScreen(Utf8JsonWriter writer, int totalCount)
{
    // Pick up to 20 variables evenly spaced across the address space
    int penCount = Math.Min(20, totalCount);
    int step = Math.Max(1, totalCount / penCount);
    var paths = new List<string>();
    for (int i = 0; i < penCount; i++)
        paths.Add(VarPath(i * step));

    writer.WriteStartObject();
    writer.WriteString("Name", "ChartAndGrid");
    writer.WriteNumber("Width", 1920);
    writer.WriteNumber("Height", 1080);
    writer.WriteString("Background", "#0d1117");

    writer.WritePropertyName("Symbols");
    writer.WriteStartArray();

    // HDA Chart — top half
    writer.WriteStartObject();
    writer.WriteString("Id", "hda_chart");
    writer.WriteString("Type", "hdachart");
    writer.WriteNumber("X", 20);
    writer.WriteNumber("Y", 20);
    writer.WriteNumber("Width", 1880);
    writer.WriteNumber("Height", 490);
    writer.WritePropertyName("HdaVariablePaths");
    writer.WriteStartArray();
    foreach (var p in paths)
        writer.WriteStringValue(p);
    writer.WriteEndArray();
    writer.WriteNumber("HdaTimeRangeMinutes", 1440);
    writer.WriteNumber("HdaMaxPoints", 2000);
    writer.WriteEndObject();

    // HDA Grid — bottom half
    writer.WriteStartObject();
    writer.WriteString("Id", "hda_grid");
    writer.WriteString("Type", "hdagrid");
    writer.WriteNumber("X", 20);
    writer.WriteNumber("Y", 530);
    writer.WriteNumber("Width", 1880);
    writer.WriteNumber("Height", 490);
    writer.WritePropertyName("HdaVariablePaths");
    writer.WriteStartArray();
    foreach (var p in paths)
        writer.WriteStringValue(p);
    writer.WriteEndArray();
    writer.WriteNumber("HdaTimeRangeMinutes", 1440);
    writer.WriteNumber("HdaMaxPoints", 500);
    writer.WriteEndObject();

    writer.WriteEndArray(); // Symbols
    writer.WriteEndObject(); // Screen
}
