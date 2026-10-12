// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SharedModels.Import
{
    /// <summary>
    /// Result of a Movicon 11 project import: the converted <see cref="NodeModel"/> plus
    /// diagnostics describing what was mapped and what could not be recognized.
    /// </summary>
    public class MoviconImportResult
    {
        /// <summary>The converted project, ready to be saved as a JSON project file or loaded into the editor.</summary>
        public NodeModel Project { get; set; } = new();

        /// <summary>Non-fatal issues encountered during import (unmapped types, ambiguous elements, etc.).</summary>
        public List<string> Warnings { get; } = new();

        public int TagCount { get; set; }
        public int FolderCount { get; set; }
        public int ScreenCount { get; set; }
        public int SymbolCount { get; set; }
        public int AlarmCount { get; set; }
        public int DataLoggerGroupCount { get; set; }
        public int RecipeCount { get; set; }

        /// <summary>True when nothing usable was found in the source project (likely wrong/unsupported path).</summary>
        public bool IsEmpty => TagCount == 0 && ScreenCount == 0;
    }

    /// <summary>
    /// Converts a real Movicon 11 (Progea) project into this application's <see cref="NodeModel"/>
    /// project format.
    /// </summary>
    /// <remarks>
    /// A Movicon 11 project is <b>not</b> a single XML file — it is a folder containing a
    /// "&lt;ProjectName&gt;.movprj" project-settings file plus a set of sibling files sharing the
    /// same base name but different extensions:
    /// <list type="bullet">
    /// <item><c>.movprj</c> — project settings (&lt;ProjectSettings&gt;): users/groups, startup screen.</item>
    /// <item><c>.movrealtimedb</c> — the tag database (&lt;RealTimeDB&gt;/&lt;VariableList&gt;/&lt;Variable&gt;),
    /// with the variable name stored as the &lt;Name&gt; element's <b>text content</b> (not an attribute),
    /// and a flat <c>Group</c> attribute used as the tag's single-level folder.</item>
    /// <item><c>.movalr</c> — reusable, named alarm <b>definitions</b> (&lt;Alarms&gt;/&lt;AlarmList&gt;/&lt;Alarm&gt;)
    /// with one or more named threshold levels (Digital/Low/LowLow/High/HighHigh/...). Tags reference these
    /// definitions by name via a nested &lt;AlarmList n0="..." n1=".../&gt; element, rather than storing limits inline.</item>
    /// <item><c>RESOURCES\&lt;ProjectName&gt;\*.movscr</c> — one XML file per screen (&lt;MovResource&gt;/&lt;ListSynopticDraw&gt;),
    /// each containing a recursive tree of &lt;child&gt;/&lt;object&gt; elements. The variable-bound, interactive
    /// controls use a composite element structure per child (e.g. &lt;Mechanic CheckVariable="..."/&gt; or
    /// &lt;Name StatusVariable="..."/&gt; for the binding, &lt;text&gt; for the label) rather than simple
    /// Type/X/Y/Tag attributes on one element.</item>
    /// </list>
    /// This importer accepts the path to the <c>.movprj</c> file (or its containing folder) and locates the
    /// sibling files by convention. It is intentionally tolerant of missing/extra files and unrecognized
    /// constructs — it records a warning and continues rather than throwing, since Movicon configurations/add-ons
    /// (driver settings, scripts, schedulers, recipes, ...) vary widely between projects.
    /// </remarks>
    public static class MoviconProjectImporter
    {
        /// <summary>
        /// Maps the numeric Movicon RealTimeDB <c>Type</c> code to this app's <see cref="Variable.Type"/> string.
        /// Inferred from the sample project (ServerHMI) by cross-referencing variable-name prefix conventions
        /// (b=Boolean, w=analog/word, t=timer/delayed-analog, n=counter/long) with the VAR.EXP Modbus export
        /// (WORD-only addresses for prefix 'w'), <c>Factor</c>/<c>EU</c> presence (analog), and known system
        /// variables (e.g. Type 8 used by internal Double-valued diagnostics like MemoryAvailPhys).
        /// Not an officially documented enum — extend/correct as more samples become available.
        /// </summary>
        private static readonly Dictionary<int, string> TagTypeMap = new()
        {
            [0] = "Boolean",   // Digital / bit variables (b* prefix convention)
            [1] = "Int32",     // Short/analog integer (SimSinInt, etc.)
            [2] = "Int32",     // Integer (ActTimeSec/Min/Hour, counters)
            [3] = "Int32",     // Integer variant (SimRampInt, ActivePassLevel)
            [4] = "Double",    // Analog / timer-delay / threshold values (t* prefix convention)
            [5] = "Int32",     // Bitmask (ActiveMaskLevel)
            [6] = "Double",    // Counter / accumulator (n* prefix, e.g. liter counters)
            [7] = "Double",    // Analog engineering value, often with Factor/EU (temperatures, setpoints)
            [8] = "Double",    // Double-precision float (system diagnostics, simulation)
            [9] = "String",    // Recipe / list / string-like structured value
            [10] = "String",
            [11] = "String",   // System/internal string variable (e.g. _SysVar_)
        };

        /// <summary>Movicon screen <c>&lt;object&gt;</c> type names mapped to <see cref="ScreenSymbol.Type"/>.</summary>
        private static readonly Dictionary<string, string> SymbolTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["button"] = "button",
            ["edit"] = "editbox",
            ["trend"] = "trend",
            ["alarmwnd"] = "alarmlist",
            ["dlrwnd"] = "hdagrid",
            ["embview"] = "screenembed",
            ["base"] = "rect",
            ["poly"] = "rect",
            ["polygon"] = "rect",
            ["polyline"] = "line",
            ["gauge"] = "gauge",
            ["hourselector"] = "numericdisplay", // date/time range picker; closest available widget type
            ["hislogwnd"] = "eventlog",
            ["chart"] = "hdachart",
        };

        // ── Entry points ─────────────────────────────────────────────────────

        /// <summary>
        /// Imports a real Movicon 11 project given the path to its <c>.movprj</c> file (or the path to the
        /// project folder itself, in which case the single <c>.movprj</c> file found inside is used).
        /// </summary>
        public static MoviconImportResult ImportFromProjectFile(string movprjOrFolderPath)
        {
            string movprjPath = ResolveMovprjPath(movprjOrFolderPath);
            string projectDir = Path.GetDirectoryName(movprjPath) ?? ".";
            string baseName = Path.GetFileNameWithoutExtension(movprjPath);

            var result = new MoviconImportResult();
            var project = new NodeModel
            {
                Folder = new Folder { Name = baseName },
                Server = new ServerSettings(),
            };

            string? startupScreen = null;

            // .movprj — project settings (startup screen name, users/groups informational only).
            var movprjDoc = TryLoadXml(movprjPath, result);
            if (movprjDoc?.Root != null)
            {
                var exec = movprjDoc.Root.Descendants("ProjectExecution").FirstOrDefault();
                startupScreen = (string?)exec?.Attribute("StartupSynoptic");
            }

            // Tag database: find "<baseName>.movrealtimedb" case-insensitively.
            var dbFile = FindSibling(projectDir, baseName, ".movrealtimedb");
            var alarmFile = FindSibling(projectDir, baseName, ".movalr");

            Dictionary<string, AlarmDefinition> alarmDefs = new(StringComparer.OrdinalIgnoreCase);
            if (alarmFile != null)
            {
                var alarmDoc = TryLoadXml(alarmFile, result);
                if (alarmDoc?.Root != null)
                    alarmDefs = ParseAlarmDefinitions(alarmDoc.Root);
            }
            else
            {
                result.Warnings.Add($"No alarm definition file (.movalr) found next to '{baseName}.movprj'.");
            }

            // Screens/driver-settings live under RESOURCES\<ProjectFolderName>\ (folder name match is case-insensitive).
            var resourcesDir = FindResourcesDir(projectDir, baseName);

            Dictionary<string, Variable> variablesByName = new(StringComparer.OrdinalIgnoreCase);
            if (dbFile != null)
            {
                var dbDoc = TryLoadXml(dbFile, result);
                if (dbDoc?.Root != null)
                {
                    var modbusStations = LoadModbusStations(resourcesDir);
                    variablesByName = ImportTagDatabase(dbDoc.Root, project, result, alarmDefs, modbusStations);
                }
            }
            else
            {
                result.Warnings.Add($"No tag database file (.movrealtimedb) found next to '{baseName}.movprj'.");
            }

            // Data logger groups + recipes: "<baseName>.movdlrec" sibling file.
            var dlrecFile = FindSibling(projectDir, baseName, ".movdlrec");
            if (dlrecFile != null)
            {
                var dlrecDoc = TryLoadXml(dlrecFile, result);
                if (dlrecDoc?.Root != null)
                    ImportDataLoggerAndRecipes(dlrecDoc.Root, project, result, variablesByName);
            }

            if (resourcesDir != null)
            {
                // Screens can live directly in RESOURCES\<Project>\ or in nested subfolders used by Movicon
                // to organize large projects (e.g. IO\, ROOMS\, NEWSCREENS\, SMALLSIZESCREENS\). Scan
                // recursively so none are silently dropped. Screen names are not guaranteed unique across
                // folders (observed duplicates like "Lights"/"Main" in different subfolders in real
                // projects), so screens found in a subfolder are prefixed with their relative folder path
                // to keep names unique and to preserve the organizational grouping from the source project.
                var screenFiles = Directory.GetFiles(resourcesDir, "*.movscr", SearchOption.AllDirectories)
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                foreach (var screenFile in screenFiles)
                {
                    var relativeDir = Path.GetDirectoryName(Path.GetRelativePath(resourcesDir, screenFile));
                    var namePrefix = string.IsNullOrEmpty(relativeDir) ? null : relativeDir!.Replace(Path.DirectorySeparatorChar, '/');
                    ImportScreenFile(screenFile, project, result, namePrefix);
                }
            }
            else
            {
                result.Warnings.Add($"No RESOURCES\\{baseName} screens folder found; screens were not imported.");
            }

            if (!string.IsNullOrWhiteSpace(startupScreen))
            {
                var match = project.Screens.FirstOrDefault(s => string.Equals(s.Name, startupScreen, StringComparison.OrdinalIgnoreCase));
                if (match != null && project.Screens.Count > 1)
                {
                    project.Screens.Remove(match);
                    project.Screens.Insert(0, match);
                }
            }

            if (result.IsEmpty)
            {
                result.Warnings.Add(
                    "No tags or screens were imported. Verify the selected .movprj file has sibling " +
                    ".movrealtimedb / RESOURCES folder files alongside it.");
            }

            result.Project = project;
            return result;
        }

        /// <summary>Backwards-compatible alias kept for existing call sites/UI wiring; delegates to <see cref="ImportFromProjectFile"/>.</summary>
        public static MoviconImportResult ImportFromFile(string filePath) => ImportFromProjectFile(filePath);

        private static string ResolveMovprjPath(string path)
        {
            if (Directory.Exists(path))
            {
                var found = Directory.GetFiles(path, "*.movprj", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (found == null)
                    throw new FileNotFoundException("No .movprj file found in the specified folder.", path);
                return found;
            }
            if (!File.Exists(path))
                throw new FileNotFoundException("Movicon project file not found.", path);
            return path;
        }

        /// <summary>
        /// Attempts to parse a tag's <c>DynamicSettings</c> attribute into a structured
        /// <c>Variable.DriverConfigs["Modbus"]</c> entry matching <c>ModbusConfig</c> (IpAddress, Port,
        /// UnitId, Register, RegisterType, PollTime). Only driver names containing "modbus"
        /// (case-insensitive) are handled; the raw Description note set by the caller is always kept
        /// as a human-readable fallback/audit trail regardless of whether structured mapping succeeds.
        /// </summary>
        private static void TryBuildModbusDriverConfig(string variableName, string dynamicSettings,
            Dictionary<string, ModbusStation> modbusStations, Variable variable, MoviconImportResult result)
        {
            var (driverName, fields) = ParseDynamicSettings(dynamicSettings);
            if (driverName == null)
                return;

            if (driverName.IndexOf("modbus", StringComparison.OrdinalIgnoreCase) < 0)
                return; // Other drivers (e.g. Fronius) are out of scope; Description fallback already set.

            if (!fields.TryGetValue("Sta", out var station) || string.IsNullOrWhiteSpace(station))
            {
                result.Warnings.Add($"Tag '{variableName}': Modbus driver settings have no station ('Sta') reference.");
                return;
            }

            if (!modbusStations.TryGetValue(station, out var stationInfo))
            {
                result.Warnings.Add($"Tag '{variableName}': Modbus station '{station}' was not found in any .drvsettings file; driver config not imported.");
                return;
            }

            if (!fields.TryGetValue("SA", out var saStr) || !int.TryParse(saStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var registerAddress))
            {
                result.Warnings.Add($"Tag '{variableName}': Modbus driver settings have no numeric 'SA' (register address).");
                return;
            }

            byte unitId = 1;
            if (fields.TryGetValue("Unit", out var unitStr) && byte.TryParse(unitStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedUnit))
                unitId = parsedUnit;

            string? registerType = null;
            if (fields.TryGetValue("FC", out var fcStr))
            {
                registerType = MapModbusRegisterType(fcStr);
                if (registerType == null)
                    result.Warnings.Add($"Tag '{variableName}': unrecognized Modbus FC code '{fcStr}', RegisterType left unset.");
            }

            var config = new
            {
                IpAddress = stationInfo.IpAddress,
                Port = stationInfo.Port,
                UnitId = unitId,
                Register = (ushort)registerAddress,
                RegisterType = registerType,
                PollTime = 1000,
            };

            variable.DriverConfigs ??= new Dictionary<string, JsonElement>();
            variable.DriverConfigs["Modbus"] = JsonSerializer.SerializeToElement(config);
        }

        private static string? FindSibling(string dir, string baseName, string extension)
        {
            var exact = Path.Combine(dir, baseName + extension);
            if (File.Exists(exact)) return exact;
            return Directory.GetFiles(dir, "*" + extension, SearchOption.TopDirectoryOnly)
                .FirstOrDefault(f => string.Equals(Path.GetFileNameWithoutExtension(f), baseName, StringComparison.OrdinalIgnoreCase))
                ?? Directory.GetFiles(dir, "*" + extension, SearchOption.TopDirectoryOnly).FirstOrDefault();
        }

        /// <summary>Resolved Modbus TCP/IP station: IP address + port, read from a `.drvsettings` file.</summary>
        private sealed record ModbusStation(string IpAddress, int Port);

        /// <summary>
        /// Scans `RESOURCES\&lt;Project&gt;\*.drvsettings` for Modbus TCP/IP driver files and builds a
        /// station-name -&gt; (IP, Port) lookup used to resolve each tag's <c>DynamicSettings</c> station
        /// reference into a concrete address. Only driver files whose display name contains "modbus"
        /// (case-insensitive) are parsed; other drivers (e.g. "Fronius") are left for the Description
        /// fallback since this importer does not know how to map their register layout.
        /// </summary>
        private static Dictionary<string, ModbusStation> LoadModbusStations(string? resourcesDir)
        {
            var stations = new Dictionary<string, ModbusStation>(StringComparer.OrdinalIgnoreCase);
            if (resourcesDir == null || !Directory.Exists(resourcesDir))
                return stations;

            foreach (var file in Directory.GetFiles(resourcesDir, "*.drvsettings", SearchOption.TopDirectoryOnly))
            {
                if (Path.GetFileNameWithoutExtension(file).IndexOf("modbus", StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                XDocument doc;
                try
                {
                    using var stream = File.OpenRead(file);
                    doc = XDocument.Load(stream, LoadOptions.None);
                }
                catch
                {
                    continue;
                }

                if (doc.Root == null)
                    continue;

                foreach (var stationEl in doc.Root.Descendants("Station"))
                {
                    var stationName = stationEl.Element("Name")?.Value?.Trim();
                    var serverEl = stationEl.Element("Server");
                    if (string.IsNullOrWhiteSpace(stationName) || serverEl == null)
                        continue;

                    var ip = (string?)serverEl.Attribute("ServerAddress");
                    var portStr = (string?)serverEl.Attribute("ServerPort");
                    if (string.IsNullOrWhiteSpace(ip))
                        continue;

                    int.TryParse(portStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var port);
                    if (port == 0)
                        port = 502;

                    stations[stationName!] = new ModbusStation(ip!, port);
                }
            }

            return stations;
        }

        /// <summary>
        /// Parses a Movicon <c>DynamicSettings</c> attribute, e.g.
        /// <c>[DRV]Modbus TCPIP.Sta=AmaldiPLC1|Unit=1|FC=2|SA=12689|Typ=4|TaskType=2</c>, into its driver
        /// name and pipe-delimited key/value pairs.
        /// </summary>
        private static (string? DriverName, Dictionary<string, string> Fields) ParseDynamicSettings(string dynamicSettings)
        {
            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var match = Regex.Match(dynamicSettings, @"^\[DRV\](?<driver>[^.]+)\.(?<rest>.*)$");
            if (!match.Success)
                return (null, fields);

            var driverName = match.Groups["driver"].Value.Trim();
            var rest = match.Groups["rest"].Value;
            foreach (var part in rest.Split('|', StringSplitOptions.RemoveEmptyEntries))
            {
                var kv = part.Split('=', 2);
                if (kv.Length == 2)
                    fields[kv[0].Trim()] = kv[1].Trim();
            }

            return (driverName, fields);
        }

        /// <summary>
        /// Maps a Movicon Modbus "FC" (function code) field from <c>DynamicSettings</c> to the app's
        /// Modbus driver <c>RegisterType</c> string. Observed in real samples: FC=0 corresponds to
        /// single-bit boolean tags (Coil), FC=2 corresponds to multi-register analog tags (HoldingRegister).
        /// Movicon's FC here is an internal "connection type" code, not the raw Modbus wire-protocol
        /// function code — values outside the two observed codes are not currently recognized.
        /// </summary>
        private static string? MapModbusRegisterType(string fc) => fc switch
        {
            "0" => "Coil",
            "2" => "HoldingRegister",
            _ => null,
        };

        private static string? FindResourcesDir(string projectDir, string baseName)
        {
            var resourcesRoot = Path.Combine(projectDir, "RESOURCES");
            if (!Directory.Exists(resourcesRoot))
                return null;
            var exact = Path.Combine(resourcesRoot, baseName);
            if (Directory.Exists(exact)) return exact;
            return Directory.GetDirectories(resourcesRoot)
                .FirstOrDefault(d => string.Equals(Path.GetFileName(d), baseName, StringComparison.OrdinalIgnoreCase))
                ?? Directory.GetDirectories(resourcesRoot).FirstOrDefault();
        }

        private static XDocument? TryLoadXml(string path, MoviconImportResult result)
        {
            try
            {
                using var stream = File.OpenRead(path);
                return XDocument.Load(stream, LoadOptions.None);
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Failed to parse '{Path.GetFileName(path)}': {ex.Message}");
                return null;
            }
        }

        // ── Tag database (.movrealtimedb) ───────────────────────────────────

        private static Dictionary<string, Variable> ImportTagDatabase(XElement root, NodeModel project, MoviconImportResult result,
            Dictionary<string, AlarmDefinition> alarmDefs, Dictionary<string, ModbusStation> modbusStations)
        {
            var variablesByName = new Dictionary<string, Variable>(StringComparer.OrdinalIgnoreCase);
            var variableList = root.Descendants("VariableList").FirstOrDefault();
            if (variableList == null)
            {
                result.Warnings.Add("The .movrealtimedb file has no <VariableList> element.");
                return variablesByName;
            }

            // Group (folder) name -> Folder instance. Movicon stores a single flat Group attribute per tag.
            var folders = new Dictionary<string, Folder>(StringComparer.OrdinalIgnoreCase);

            foreach (var varEl in variableList.Elements("Variable"))
            {
                var nameEl = varEl.Element("Name");
                var name = nameEl?.Value?.Trim();
                if (string.IsNullOrWhiteSpace(name) || nameEl == null)
                {
                    result.Warnings.Add("Skipped a <Variable> with no name text.");
                    continue;
                }

                var typeAttr = (string?)nameEl.Attribute("Type");
                var mappedType = "Double";
                if (int.TryParse(typeAttr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var typeCode))
                {
                    if (!TagTypeMap.TryGetValue(typeCode, out mappedType!))
                    {
                        mappedType = "Double";
                        result.Warnings.Add($"Tag '{name}': unrecognized Movicon Type code '{typeCode}', defaulted to Double.");
                    }
                }

                var group = ((string?)nameEl.Attribute("Group"))?.Trim();
                var targetFolder = project.Folder;
                if (!string.IsNullOrWhiteSpace(group))
                {
                    if (!folders.TryGetValue(group!, out var folder))
                    {
                        folder = new Folder { Name = group! };
                        project.Folder.Folders.Add(folder);
                        folders[group!] = folder;
                        result.FolderCount++;
                    }
                    targetFolder = folder;
                }

                var retentiveAttr = (string?)nameEl.Attribute("Retentive");
                var variable = new Variable
                {
                    Name = name!,
                    Type = mappedType,
                    Retentive = retentiveAttr == "1",
                };

                var eu = (string?)nameEl.Attribute("EU");
                if (!string.IsNullOrWhiteSpace(eu))
                    variable.EngineeringUnit = eu!;

                var address = (string?)nameEl.Attribute("Address");
                var dynSettings = (string?)nameEl.Attribute("DynamicSettings");
                if (!string.IsNullOrWhiteSpace(dynSettings))
                {
                    variable.Description = $"Movicon driver settings: {dynSettings}";
                    TryBuildModbusDriverConfig(name!, dynSettings!, modbusStations, variable, result);
                }
                else if (!string.IsNullOrWhiteSpace(address) && address != "0")
                    variable.Description = $"Movicon address: {address}";

                var factor = (string?)nameEl.Attribute("Factor");
                var rawMax = (string?)nameEl.Attribute("RawMax");
                if ((!string.IsNullOrWhiteSpace(factor) && factor != "1") || !string.IsNullOrWhiteSpace(rawMax))
                {
                    if (double.TryParse(factor, NumberStyles.Float, CultureInfo.InvariantCulture, out var factorVal) && factorVal != 0 && factorVal != 1)
                    {
                        variable.Scaling = new ScalingConfig
                        {
                            RawMin = 0,
                            RawMax = double.TryParse(rawMax, NumberStyles.Float, CultureInfo.InvariantCulture, out var rm) ? rm : 100,
                            EngMin = 0,
                            EngMax = factorVal,
                        };
                    }
                }

                var statisticsAttr = (string?)nameEl.Attribute("StatisticData");
                if (statisticsAttr == "1")
                    variable.Statistics = new VariableStatisticsConfig { Enabled = true };

                // Map tag-referenced alarm definitions (<AlarmList n0="..." n1=".../>) down into one AlarmConfig.
                var alarmListEl = varEl.Element("AlarmList");
                if (alarmListEl != null)
                {
                    var refNames = alarmListEl.Attributes()
                        .Where(a => a.Name.LocalName.StartsWith("n", StringComparison.OrdinalIgnoreCase))
                        .Select(a => a.Value)
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .ToList();

                    if (refNames.Count > 1)
                        result.Warnings.Add($"Tag '{name}' references {refNames.Count} alarm definitions ({string.Join(", ", refNames)}); only the first ('{refNames[0]}') was mapped (single-alarm-per-variable limitation).");

                    if (refNames.Count > 0 && alarmDefs.TryGetValue(refNames[0], out var def))
                    {
                        variable.Alarm = BuildAlarmConfig(def, name!, result);
                        result.AlarmCount++;
                    }
                    else if (refNames.Count > 0)
                    {
                        result.Warnings.Add($"Tag '{name}' references undefined alarm definition '{refNames[0]}'.");
                    }
                }

                targetFolder.Variables.Add(variable);
                variablesByName[name!] = variable;
                result.TagCount++;
            }

            return variablesByName;
        }

        // ── Data logger groups + recipes (.movdlrec) ────────────────────────

        /// <summary>
        /// Parses a Movicon 11 <c>.movdlrec</c> file (&lt;DLAndRecipe&gt;/&lt;DLRecipeList&gt;/&lt;DLRecipe&gt;),
        /// which stores both historical data-logger groups (<c>IsRecipe="0"</c>: a named group of variables
        /// recorded on a timer or on-change, with a retention duration) and recipe definitions
        /// (<c>IsRecipe="1"</c>: a named set of variables whose values are saved/loaded as a unit, with a
        /// first "Recipe Index" pseudo-column). Data-logger groups are mapped onto the already-imported
        /// <see cref="Variable.DataLogging"/> property of each referenced tag (enabling it and deriving
        /// <c>MaxAge</c> from the group's retention duration and <c>Hysteresis</c> from its dead-band, when
        /// configured); recipes are mapped onto <see cref="NodeModel.Recipes"/>.
        /// </summary>
        private static void ImportDataLoggerAndRecipes(XElement root, NodeModel project, MoviconImportResult result,
            Dictionary<string, Variable> variablesByName)
        {
            var recipeList = root.Descendants("DLRecipeList").FirstOrDefault();
            if (recipeList == null)
            {
                result.Warnings.Add("The .movdlrec file has no <DLRecipeList> element.");
                return;
            }

            foreach (var recipeEl in recipeList.Elements("DLRecipe"))
            {
                var nameEl = recipeEl.Element("Name");
                var groupName = nameEl?.Value?.Trim();
                if (string.IsNullOrWhiteSpace(groupName) || nameEl == null)
                {
                    result.Warnings.Add("Skipped a <DLRecipe> with no name text.");
                    continue;
                }

                var columns = recipeEl.Element("ColumnList")?.Elements("Column").ToList() ?? new List<XElement>();
                var isRecipe = (string?)nameEl.Attribute("IsRecipe") == "1";

                if (isRecipe)
                {
                    var recipe = new RecipeConfig { Name = groupName!, Enabled = (string?)nameEl.Attribute("Enabled") != "0" };
                    var index = 0;
                    foreach (var columnEl in columns)
                    {
                        var colNameEl = columnEl.Element("Name");
                        var variableName = ((string?)colNameEl?.Attribute("Variable"))?.Trim();
                        var recipeIndexAttr = (string?)colNameEl?.Attribute("RecipeIndex");
                        // Movicon's "Recipe Index" pseudo-column (RecipeIndex="1") is the active-record
                        // selector, not an actual data variable to save/restore; skip it.
                        if (string.IsNullOrWhiteSpace(variableName) || recipeIndexAttr == "1")
                            continue;

                        recipe.Variables.Add(new RecipeVariable
                        {
                            Index = index++,
                            VariablePath = variableName!,
                            DisplayName = colNameEl?.Value?.Trim() ?? variableName!,
                        });

                        if (!variablesByName.ContainsKey(variableName!))
                            result.Warnings.Add($"Recipe '{groupName}' references undefined variable '{variableName}'.");
                    }

                    project.Recipes.Add(recipe);
                    result.RecipeCount++;
                    continue;
                }

                // Data-logger group: apply DataLogging settings to each referenced variable.
                var recordOnChange = (string?)nameEl.Attribute("RecordOnChange") == "1";
                var deadBandEnabled = (string?)nameEl.Attribute("EnableDeadBand") == "1";
                double.TryParse((string?)nameEl.Attribute("DeadBand"), NumberStyles.Float, CultureInfo.InvariantCulture, out var deadBand);

                var dsnEl = recipeEl.Element("DSN");
                int.TryParse((string?)dsnEl?.Attribute("DurationDays"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var durationDays);
                int.TryParse((string?)dsnEl?.Attribute("DurationHours"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var durationHours);
                int.TryParse((string?)dsnEl?.Attribute("DurationMinutes"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var durationMinutes);
                TimeSpan? maxAge = (durationDays, durationHours, durationMinutes) == (0, 0, 0)
                    ? null
                    : new TimeSpan(durationDays, durationHours, durationMinutes, 0);

                if (columns.Count == 0)
                {
                    // Groups with no <ColumnList> (e.g. "Log5sec", "Log1Hr") are reusable timing presets with
                    // no variables directly attached in this file; nothing to map onto a Variable here.
                    result.DataLoggerGroupCount++;
                    continue;
                }

                var mappedCount = 0;
                foreach (var columnEl in columns)
                {
                    var variableName = ((string?)columnEl.Element("Name")?.Attribute("Variable"))?.Trim();
                    if (string.IsNullOrWhiteSpace(variableName))
                        continue;

                    if (!variablesByName.TryGetValue(variableName!, out var variable))
                    {
                        result.Warnings.Add($"Data logger group '{groupName}' references undefined variable '{variableName}'.");
                        continue;
                    }

                    // A variable can belong to more than one data-logger group in Movicon (different retention/
                    // rate tiers); this importer keeps only the first (most specific/smallest) group found per
                    // variable, since Variable.DataLogging holds a single configuration, not a list of groups.
                    if (variable.DataLogging != null)
                        continue;

                    variable.DataLogging = new DataLoggingConfig
                    {
                        Enabled = true,
                        Hysteresis = deadBandEnabled ? deadBand : 0,
                        MaxAge = maxAge,
                    };
                    mappedCount++;
                }

                if (mappedCount > 0)
                    result.DataLoggerGroupCount++;
                else if (recordOnChange || columns.Count > 0)
                    result.Warnings.Add($"Data logger group '{groupName}' had {columns.Count} column(s) but none could be mapped (all variables already assigned to another group, or undefined).");
            }
        }

        private class AlarmDefinition
        {
            public string Name = "";
            public List<(string Level, int Condition, string Threshold, double Hysteresis)> Thresholds = new();
        }

        /// <summary>Parses the .movalr file's named, reusable alarm definitions (with per-level thresholds).</summary>
        private static Dictionary<string, AlarmDefinition> ParseAlarmDefinitions(XElement root)
        {
            var defs = new Dictionary<string, AlarmDefinition>(StringComparer.OrdinalIgnoreCase);
            var alarmList = root.Descendants("AlarmList").FirstOrDefault();
            if (alarmList == null) return defs;

            foreach (var alarmEl in alarmList.Elements("Alarm"))
            {
                var nameEl = alarmEl.Element("Name");
                var name = nameEl?.Value?.Trim();
                if (string.IsNullOrWhiteSpace(name)) continue;

                var hysteresisAttr = (string?)nameEl!.Attribute("Hysteresis");
                double.TryParse(hysteresisAttr, NumberStyles.Float, CultureInfo.InvariantCulture, out var hysteresis);

                var def = new AlarmDefinition { Name = name! };
                var thresholdList = alarmEl.Element("ThresholdList");
                if (thresholdList != null)
                {
                    foreach (var th in thresholdList.Elements("Threshold"))
                    {
                        var levelName = th.Element("Name")?.Value?.Trim() ?? "";
                        var exec = th.Element("Execution");
                        int.TryParse((string?)exec?.Attribute("Condition"), out var condition);
                        var threshold = (string?)exec?.Attribute("Threshold") ?? "";
                        def.Thresholds.Add((levelName, condition, threshold, hysteresis));
                    }
                }
                defs[name!] = def;
            }
            return defs;
        }

        /// <summary>
        /// Collapses a Movicon alarm definition (named threshold levels, e.g. Low/LowLow/High/HighHigh or a
        /// single "Digital" level) into this app's single <see cref="AlarmConfig"/>.
        /// </summary>
        private static AlarmConfig BuildAlarmConfig(AlarmDefinition def, string tagName, MoviconImportResult result)
        {
            var alarm = new AlarmConfig { Message = $"{tagName} ({def.Name})", Hysteresis = def.Thresholds.FirstOrDefault().Hysteresis };

            bool isDigital = def.Thresholds.Any(t => string.Equals(t.Level, "Digital", StringComparison.OrdinalIgnoreCase));
            if (isDigital)
            {
                var digital = def.Thresholds.First(t => string.Equals(t.Level, "Digital", StringComparison.OrdinalIgnoreCase));
                alarm.TriggerType = AlarmTriggerType.Condition;
                alarm.Operator = digital.Threshold == "0" ? "False" : "True";
                return alarm;
            }

            alarm.TriggerType = AlarmTriggerType.Limit;
            var unmapped = new List<string>();
            foreach (var (level, _, threshold, _) in def.Thresholds)
            {
                if (!double.TryParse(threshold, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                    continue;
                switch (level.ToLowerInvariant())
                {
                    case "highhigh": alarm.HighHighLimit = value; break;
                    case "high": alarm.HighLimit = value; break;
                    case "low": alarm.LowLimit = value; break;
                    case "lowlow": alarm.LowLowLimit = value; break;
                    default: unmapped.Add(level); break;
                }
            }
            if (unmapped.Count > 0)
                result.Warnings.Add($"Alarm definition '{def.Name}' (used by tag '{tagName}') has threshold level(s) {string.Join(", ", unmapped)} with no equivalent in AlarmConfig; they were dropped.");

            return alarm;
        }

        // ── Screens (RESOURCES\<Project>\*.movscr) ──────────────────────────

        private static void ImportScreenFile(string screenFile, NodeModel project, MoviconImportResult result, string? namePrefix = null)
        {
            XDocument? doc;
            try
            {
                using var stream = File.OpenRead(screenFile);
                doc = XDocument.Load(stream, LoadOptions.None);
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Failed to parse screen file '{Path.GetFileName(screenFile)}': {ex.Message}");
                return;
            }

            var generalEl = doc.Root?.Element("General");
            var baseName = Path.GetFileNameWithoutExtension(screenFile);
            var screen = new ScreenConfig
            {
                Name = string.IsNullOrEmpty(namePrefix) ? baseName : $"{namePrefix}/{baseName}",
                Width = GetIntAttr(generalEl, 1280, "SizeX"),
                Height = GetIntAttr(generalEl, 1024, "SizeY"),
            };

            var drawList = doc.Root?.Element("ListSynopticDraw");
            if (drawList != null)
            {
                foreach (var child in drawList.Elements("child"))
                    ImportScreenChild(child, screen, result);
            }

            project.Screens.Add(screen);
            result.ScreenCount++;
        }

        /// <summary>
        /// Recursively imports one &lt;child&gt; element of a .movscr draw list. "group" objects recurse
        /// into their nested &lt;childs&gt;/&lt;child&gt; without producing a symbol of their own.
        /// </summary>
        private static void ImportScreenChild(XElement child, ScreenConfig screen, MoviconImportResult result)
        {
            // Most interactive widgets (Button, Edit, HourSelector, EmbView, AlarmWnd, ...) carry an
            // <object>Name</object> element. Pure vector-art shapes exported by Movicon's drawing tools
            // (freeform polygons/polylines, and sometimes even nested groups of them, making up
            // decorative graphics — e.g. GeneralLayout.movscr's template artwork) have NO <object>
            // element at all — they're identified only by a bare <type>poly</type> or <type>group</type>
            // element instead. Both forms (<object> and bare <type>) must be recognized identically so
            // vector-art groups still recurse and poly art doesn't flood the screen with placeholders.
            var objectEl = child.Element("object");
            var objectType = objectEl?.Value?.Trim();
            if (string.IsNullOrEmpty(objectType))
                objectType = child.Element("type")?.Value?.Trim() ?? "";

            if (string.Equals(objectType, "group", StringComparison.OrdinalIgnoreCase))
            {
                var nestedChilds = child.Element("childs");
                if (nestedChilds != null)
                {
                    foreach (var nested in nestedChilds.Elements("child"))
                        ImportScreenChild(nested, screen, result);
                }
                return;
            }

            // Skip pure static vector art objects: no useful tag binding, too complex geometry to map 1:1.
            if (string.Equals(objectType, "poly", StringComparison.OrdinalIgnoreCase)
                || string.Equals(objectType, "polygon", StringComparison.OrdinalIgnoreCase)
                || string.Equals(objectType, "polyline", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!SymbolTypeMap.TryGetValue(objectType, out var mappedType))
            {
                mappedType = "rect";
                if (!string.IsNullOrWhiteSpace(objectType))
                    result.Warnings.Add($"Screen '{screen.Name}': unrecognized object type '{objectType}', mapped to generic rect.");
            }

            // Geometry lives on an element named after the (lowercased) object type: <editbox>, <trend>, <button>...
            var geomEl = child.Elements().FirstOrDefault(e =>
                e.Attribute("x") != null && e.Attribute("y") != null && e.Attribute("width") != null);

            var symbol = new ScreenSymbol
            {
                Id = Guid.NewGuid().ToString("N"),
                Type = mappedType,
                X = GetDoubleAttr(geomEl, 0, "x"),
                Y = GetDoubleAttr(geomEl, 0, "y"),
                Width = GetDoubleAttr(geomEl, 80, "width"),
                Height = GetDoubleAttr(geomEl, 40, "height"),
            };

            var fill = (string?)geomEl?.Attribute("fill");
            if (!string.IsNullOrWhiteSpace(fill)) symbol.Fill = NormalizeColor(fill!);
            var stroke = (string?)geomEl?.Attribute("stroke");
            if (!string.IsNullOrWhiteSpace(stroke)) symbol.Stroke = NormalizeColor(stroke!);

            // Variable binding: Mechanic.CheckVariable (buttons) or Name.StatusVariable (most other widgets),
            // or <execution var="..."/> (edit boxes).
            var mechanic = child.Element("Mechanic");
            var nameEl = child.Element("Name");
            var execution = child.Element("execution");
            var varBinding = (string?)mechanic?.Attribute("CheckVariable")
                ?? (string?)nameEl?.Attribute("StatusVariable")
                ?? (string?)execution?.Attribute("var");
            if (!string.IsNullOrWhiteSpace(varBinding))
                symbol.VariablePath = varBinding;

            var textEl = child.Element("text");
            var label = textEl?.Value?.Trim();
            symbol.Label = !string.IsNullOrWhiteSpace(label) ? label! : (varBinding ?? "");

            var rotation = child.Element("Rotation")?.Value;
            if (double.TryParse(rotation, NumberStyles.Float, CultureInfo.InvariantCulture, out var rot))
                symbol.Rotation = rot;

            screen.Symbols.Add(symbol);
            result.SymbolCount++;
        }

        // ── small XML helpers ────────────────────────────────────────────────

        private static int GetIntAttr(XElement? el, int fallback, string name)
        {
            var raw = (string?)el?.Attribute(name);
            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : fallback;
        }

        private static double GetDoubleAttr(XElement? el, double fallback, string name)
        {
            var raw = (string?)el?.Attribute(name);
            return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : fallback;
        }

        private static readonly Regex RgbRegex = new(@"rgb\((\d+),(\d+),(\d+)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Normalizes a Movicon screen color value: <c>rgb(r,g,b)</c> function syntax, a
        /// <c>syscolor(NAME SysColor)</c> symbolic system color reference (left as a neutral default since it
        /// has no fixed RGB value), or a plain "none"/hex string.
        /// </summary>
        private static string NormalizeColor(string raw)
        {
            raw = raw.Trim();
            var m = RgbRegex.Match(raw);
            if (m.Success)
            {
                int r = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                int g = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
                int b = int.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);
                return $"#{r:X2}{g:X2}{b:X2}";
            }
            if (raw.StartsWith("syscolor(", StringComparison.OrdinalIgnoreCase) || string.Equals(raw, "none", StringComparison.OrdinalIgnoreCase))
                return "#cccccc";
            if (raw.StartsWith("#")) return raw;
            if (raw.Length is 6 or 8 && raw.All(Uri.IsHexDigit)) return "#" + raw[^6..];
            return raw;
        }
    }
}
