// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SharedModels.Import
{
    /// <summary>
    /// Result of importing an Ignition tag export (JSON) into this application's tag database format.
    /// </summary>
    public class IgnitionTagImportResult
    {
        /// <summary>The imported tags, structured as a folder tree ready to be merged into a project's <see cref="Folder"/>.</summary>
        public Folder Root { get; set; } = new();
        public List<string> Warnings { get; } = new();
        public int TagCount { get; set; }
        public int FolderCount { get; set; }
        public int AlarmCount { get; set; }
    }

    /// <summary>
    /// Result of importing an Ignition Perspective view (view.json) into a <see cref="ScreenConfig"/>.
    /// </summary>
    public class IgnitionViewImportResult
    {
        public ScreenConfig Screen { get; set; } = new();
        public List<string> Warnings { get; } = new();
        public int SymbolCount { get; set; }
    }

    /// <summary>
    /// Converts Ignition (Inductive Automation) project exports into this application's project model.
    /// </summary>
    /// <remarks>
    /// Ignition projects are not a single file: the Designer exports the tag database separately
    /// (Tag Export → JSON) from Perspective views (one <c>view.json</c> per view, under
    /// <c>com.inductiveautomation.perspective/views/&lt;path&gt;/view.json</c> in an unpacked project).
    /// This importer therefore exposes two independent entry points — <see cref="ImportTags"/> and
    /// <see cref="ImportView"/> — that are meant to be merged into an existing or new project, mirroring
    /// how Ignition itself separates tags from views. A convenience <see cref="ImportProjectFolder"/>
    /// walks an unpacked project directory and imports everything it can find.
    ///
    /// The JSON shapes recognized here follow Ignition 8.x's documented/observed tag-export and
    /// Perspective view-definition formats (tag "tagType"/"dataType"/"valueSource"/"opcItemPath"/"alarms",
    /// view "root"/"children"/"type"/"position"/"propConfig" binding). Field access is tolerant (missing
    /// properties default rather than throw) so minor version differences don't break the import; extend
    /// <see cref="ComponentTypeMap"/> / <see cref="DataTypeMap"/> as real-world exports are seen.
    /// </remarks>
    public static class IgnitionProjectImporter
    {
        /// <summary>Ignition tag "dataType" values mapped to <see cref="Variable.Type"/>.</summary>
        private static readonly Dictionary<string, string> DataTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Boolean"] = "Boolean",
            ["Short"] = "Int32",
            ["Integer"] = "Int32",
            ["Int1"] = "Int32",
            ["Int2"] = "Int32",
            ["Int4"] = "Int32",
            ["Int8"] = "Int32",
            ["Long"] = "Int32",
            ["Float4"] = "Double",
            ["Float8"] = "Double",
            ["Float"] = "Double",
            ["Double"] = "Double",
            ["String"] = "String",
            ["Text"] = "String",
            ["Document"] = "String",
            ["DateTime"] = "String",
        };

        /// <summary>Perspective component "type" values mapped to <see cref="ScreenSymbol.Type"/>.</summary>
        private static readonly Dictionary<string, string> ComponentTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["ia.display.label"] = "text",
            ["ia.display.led"] = "indicator",
            ["ia.display.gauge"] = "gauge",
            ["ia.display.progressbar"] = "progressbar",
            ["ia.display.image"] = "svg",
            ["ia.input.button"] = "button",
            ["ia.input.numeric-entry-field"] = "numericdisplay",
            ["ia.input.dropdown"] = "dropdown",
            ["ia.shapes.rectangle"] = "rect",
            ["ia.shapes.ellipse"] = "ellipse",
            ["ia.shapes.circle"] = "circle",
            ["ia.shapes.line"] = "line",
            ["ia.chart.timeseries"] = "trend",
            ["ia.chart.xyChart"] = "xyplot",
            ["ia.display.table"] = "datatable",
            ["ia.display.alarm-status-table"] = "alarmlist",
        };

        /// <summary>Container component types whose children are still walked, but which do not themselves become a symbol.</summary>
        private static readonly HashSet<string> ContainerTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "ia.container.coord", "ia.container.flex", "ia.container.column", "ia.container.tab",
            "ia.container.breakpoint", "ia.container.view",
        };

        // ── Tags ──────────────────────────────────────────────────────────

        /// <summary>Imports an Ignition tag export (JSON, as produced by Designer's "Export selected tags..." → JSON) from disk.</summary>
        public static IgnitionTagImportResult ImportTagsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Ignition tag export file not found.", filePath);
            return ImportTags(File.ReadAllText(filePath));
        }

        /// <summary>Imports an Ignition tag export from a raw JSON string.</summary>
        public static IgnitionTagImportResult ImportTags(string json)
        {
            var result = new IgnitionTagImportResult();
            JsonNode? node;
            try
            {
                node = JsonNode.Parse(json);
            }
            catch (JsonException ex)
            {
                result.Warnings.Add($"Could not parse tag export JSON: {ex.Message}");
                return result;
            }

            if (node is JsonArray array)
            {
                // Some exports are a bare array of top-level tags/folders instead of one root object.
                foreach (var child in array)
                {
                    if (child is JsonObject childObj)
                        ImportTagNode(childObj, result.Root, result);
                }
            }
            else if (node is JsonObject rootObj)
            {
                // If the root object is itself a folder wrapper with a "tags" array, import its children
                // directly into the result root (so the root folder isn't wrapped in an extra "root" level).
                var tagType = GetString(rootObj, "tagType");
                var childTags = rootObj["tags"] as JsonArray;
                if (childTags != null && (tagType == null || tagType.Equals("Provider", StringComparison.OrdinalIgnoreCase) || tagType.Equals("Folder", StringComparison.OrdinalIgnoreCase)))
                {
                    foreach (var child in childTags)
                    {
                        if (child is JsonObject childObj)
                            ImportTagNode(childObj, result.Root, result);
                    }
                }
                else
                {
                    ImportTagNode(rootObj, result.Root, result);
                }
            }
            else
            {
                result.Warnings.Add("Tag export JSON root was neither an object nor an array.");
            }

            if (result.TagCount == 0)
                result.Warnings.Add("No tags were found in the export. Check that this is an Ignition tag-export JSON file.");

            return result;
        }

        private static void ImportTagNode(JsonObject tagObj, Folder targetFolder, IgnitionTagImportResult result)
        {
            var name = GetString(tagObj, "name");
            if (string.IsNullOrWhiteSpace(name))
            {
                result.Warnings.Add("Skipped a tag/folder entry with no 'name' property.");
                return;
            }

            var tagType = GetString(tagObj, "tagType") ?? "AtomicTag";
            var children = tagObj["tags"] as JsonArray;

            if (tagType.Equals("Folder", StringComparison.OrdinalIgnoreCase) ||
                tagType.Equals("UdtType", StringComparison.OrdinalIgnoreCase) ||
                (children != null && !tagType.Equals("UdtInstance", StringComparison.OrdinalIgnoreCase)))
            {
                var subFolder = new Folder { Name = name };
                targetFolder.Folders.Add(subFolder);
                result.FolderCount++;
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        if (child is JsonObject childObj)
                            ImportTagNode(childObj, subFolder, result);
                    }
                }
                return;
            }

            // UDT instances: flatten their member tags into a sub-folder named after the instance.
            if (tagType.Equals("UdtInstance", StringComparison.OrdinalIgnoreCase) && children != null)
            {
                var udtFolder = new Folder { Name = name };
                targetFolder.Folders.Add(udtFolder);
                result.FolderCount++;
                foreach (var child in children)
                {
                    if (child is JsonObject childObj)
                        ImportTagNode(childObj, udtFolder, result);
                }
                return;
            }

            ImportAtomicTag(tagObj, name!, targetFolder, result);
        }

        private static void ImportAtomicTag(JsonObject tagObj, string name, Folder targetFolder, IgnitionTagImportResult result)
        {
            var rawType = GetString(tagObj, "dataType") ?? "";
            if (!DataTypeMap.TryGetValue(rawType, out var mappedType))
            {
                mappedType = "Double";
                if (!string.IsNullOrWhiteSpace(rawType))
                    result.Warnings.Add($"Tag '{name}': unrecognized dataType '{rawType}', defaulted to Double.");
            }

            var valueSource = GetString(tagObj, "valueSource") ?? "memory";
            var variable = new Variable
            {
                Name = name,
                Type = mappedType,
                Access = "ReadWrite",
                EngineeringUnit = GetString(tagObj, "engUnit") ?? "",
                Description = GetString(tagObj, "documentation") ?? "",
            };

            var valueNode = tagObj["value"];
            if (valueNode != null)
                variable.InitialValue = valueNode.ToString();

            var opcItemPath = GetString(tagObj, "opcItemPath");
            if (valueSource.Equals("opc", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(opcItemPath))
            {
                var opcServer = GetString(tagObj, "opcServer") ?? "";
                var note = $"Ignition OPC binding: {opcServer} {opcItemPath}".Trim();
                variable.Description = string.IsNullOrEmpty(variable.Description) ? note : $"{variable.Description} ({note})";
            }
            else if (valueSource.Equals("expression", StringComparison.OrdinalIgnoreCase))
            {
                var expr = GetString(tagObj, "expression") ?? "";
                variable.Description = string.IsNullOrEmpty(variable.Description) ? $"Ignition expression: {expr}" : variable.Description;
            }

            var alarms = tagObj["alarms"] as JsonArray;
            if (alarms != null && alarms.Count > 0)
            {
                variable.Alarm = ImportAlarms(alarms, name);
                if (variable.Alarm != null) result.AlarmCount++;
            }

            targetFolder.Variables.Add(variable);
            result.TagCount++;
        }

        /// <summary>
        /// Ignition tags can carry multiple named alarms (e.g. "Hi", "HiHi", "Lo", "LoLo"), each with its
        /// own mode ("AboveValue"/"BelowValue") and setpoint. This app's <see cref="AlarmConfig"/> models a
        /// single 4-level limit alarm per tag, so alarms are merged by matching their name/mode against the
        /// conventional Hi/HiHi/Lo/LoLo pattern; anything else becomes the High/Low limit on a best-effort basis.
        /// </summary>
        private static AlarmConfig? ImportAlarms(JsonArray alarms, string tagName)
        {
            var alarm = new AlarmConfig { TriggerType = AlarmTriggerType.Limit };
            bool any = false;

            foreach (var a in alarms)
            {
                if (a is not JsonObject alarmObj) continue;
                var alarmName = (GetString(alarmObj, "name") ?? "").ToLowerInvariant();
                var mode = GetString(alarmObj, "mode") ?? "";
                var setpoint = GetDouble(alarmObj, "setpointA");
                if (setpoint == null) continue;

                bool isHigh = mode.Equals("AboveValue", StringComparison.OrdinalIgnoreCase);
                bool isLow = mode.Equals("BelowValue", StringComparison.OrdinalIgnoreCase);
                if (!isHigh && !isLow) continue;

                if (isHigh && alarmName.Contains("hihi")) { alarm.HighHighLimit = setpoint; any = true; }
                else if (isLow && alarmName.Contains("lolo")) { alarm.LowLowLimit = setpoint; any = true; }
                else if (isHigh) { alarm.HighLimit = setpoint.Value; any = true; }
                else if (isLow) { alarm.LowLimit = setpoint.Value; any = true; }

                if (string.IsNullOrEmpty(alarm.Message))
                    alarm.Message = GetString(alarmObj, "displayPath") ?? GetString(alarmObj, "label") ?? $"{tagName} alarm";
            }

            return any ? alarm : null;
        }

        // ── Perspective views ────────────────────────────────────────────────

        /// <summary>Imports an Ignition Perspective <c>view.json</c> from disk. The view's display name defaults to the file's parent folder name.</summary>
        public static IgnitionViewImportResult ImportViewFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Ignition view.json file not found.", filePath);
            var name = Path.GetFileName(Path.GetDirectoryName(filePath)) ?? Path.GetFileNameWithoutExtension(filePath);
            return ImportView(File.ReadAllText(filePath), name);
        }

        /// <summary>Imports an Ignition Perspective view definition (view.json content) into a <see cref="ScreenConfig"/>.</summary>
        public static IgnitionViewImportResult ImportView(string json, string viewName)
        {
            var result = new IgnitionViewImportResult { Screen = new ScreenConfig { Name = viewName } };
            JsonNode? node;
            try
            {
                node = JsonNode.Parse(json);
            }
            catch (JsonException ex)
            {
                result.Warnings.Add($"Could not parse view JSON: {ex.Message}");
                return result;
            }

            if (node is not JsonObject rootObj)
            {
                result.Warnings.Add("View JSON root was not an object.");
                return result;
            }

            var props = rootObj["props"] as JsonObject;
            var defaultSize = props?["defaultSize"] as JsonObject;
            if (defaultSize != null)
            {
                result.Screen.Width = (int)(GetDouble(defaultSize, "width") ?? 800);
                result.Screen.Height = (int)(GetDouble(defaultSize, "height") ?? 600);
            }

            var root = rootObj["root"] as JsonObject;
            if (root == null)
            {
                result.Warnings.Add("View JSON has no 'root' component tree.");
                return result;
            }

            WalkComponent(root, result);
            return result;
        }

        private static void WalkComponent(JsonObject component, IgnitionViewImportResult result)
        {
            var type = GetString(component, "type") ?? "";
            var isContainer = ContainerTypes.Contains(type) || string.IsNullOrEmpty(type);

            if (!isContainer)
            {
                result.Screen.Symbols.Add(ImportComponent(component, type, result));
                result.SymbolCount++;
            }

            if (component["children"] is JsonArray children)
            {
                foreach (var child in children)
                {
                    if (child is JsonObject childObj)
                        WalkComponent(childObj, result);
                }
            }
        }

        private static ScreenSymbol ImportComponent(JsonObject component, string rawType, IgnitionViewImportResult result)
        {
            if (!ComponentTypeMap.TryGetValue(rawType, out var mappedType))
            {
                mappedType = "rect";
                result.Warnings.Add($"Component '{GetComponentName(component)}': unrecognized type '{rawType}', mapped to generic rect.");
            }

            var meta = component["meta"] as JsonObject;
            var position = component["position"] as JsonObject;

            var symbol = new ScreenSymbol
            {
                Id = GetString(meta, "name") ?? Guid.NewGuid().ToString("N"),
                Type = mappedType,
                X = GetDouble(position, "x") ?? 0,
                Y = GetDouble(position, "y") ?? 0,
                Width = GetDouble(position, "width") ?? 80,
                Height = GetDouble(position, "height") ?? 40,
            };

            var props = component["props"] as JsonObject;
            var text = GetString(props, "text") ?? GetString(props, "label");
            if (!string.IsNullOrWhiteSpace(text)) symbol.Label = text!;

            var tagPath = FindTagBinding(component["propConfig"] as JsonObject);
            if (!string.IsNullOrWhiteSpace(tagPath))
                symbol.VariablePath = tagPath;

            return symbol;
        }

        private static string GetComponentName(JsonObject component) =>
            GetString(component["meta"] as JsonObject, "name") ?? "(unnamed)";

        /// <summary>
        /// Searches a component's <c>propConfig</c> map for the first tag binding
        /// (<c>{ "binding": { "type": "tag", "config": { "tagPath": "..." } } }</c>) and returns its tag path.
        /// </summary>
        private static string? FindTagBinding(JsonObject? propConfig)
        {
            if (propConfig == null) return null;
            foreach (var kvp in propConfig)
            {
                if (kvp.Value is not JsonObject entry) continue;
                if (entry["binding"] is not JsonObject binding) continue;
                var bindingType = GetString(binding, "type");
                if (bindingType != null && bindingType.Equals("tag", StringComparison.OrdinalIgnoreCase))
                {
                    var tagPath = GetString(binding["config"] as JsonObject, "tagPath");
                    if (!string.IsNullOrWhiteSpace(tagPath)) return tagPath;
                }
            }
            return null;
        }

        // ── Whole-project (unpacked directory) convenience import ───────────────

        /// <summary>
        /// Walks an unpacked Ignition project directory, importing every tag-export JSON file it finds
        /// (heuristically identified) and every <c>view.json</c> under a Perspective views folder.
        /// </summary>
        public static (NodeModel Project, List<string> Warnings) ImportProjectFolder(string projectDirectory)
        {
            if (!Directory.Exists(projectDirectory))
                throw new DirectoryNotFoundException($"Directory not found: {projectDirectory}");

            var project = new NodeModel { Folder = new Folder { Name = Path.GetFileName(projectDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) } };
            var warnings = new List<string>();

            foreach (var jsonFile in Directory.EnumerateFiles(projectDirectory, "*.json", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(jsonFile);
                try
                {
                    if (fileName.Equals("view.json", StringComparison.OrdinalIgnoreCase))
                    {
                        var viewResult = ImportViewFromFile(jsonFile);
                        project.Screens.Add(viewResult.Screen);
                        warnings.AddRange(viewResult.Warnings.Select(w => $"[{fileName} @ {jsonFile}] {w}"));
                    }
                    else if (LooksLikeTagExport(jsonFile))
                    {
                        var tagResult = ImportTagsFromFile(jsonFile);
                        MergeFolder(tagResult.Root, project.Folder);
                        warnings.AddRange(tagResult.Warnings.Select(w => $"[{fileName}] {w}"));
                    }
                }
                catch (Exception ex)
                {
                    warnings.Add($"Failed to import '{jsonFile}': {ex.Message}");
                }
            }

            if (project.Screens.Count == 0 && project.Folder.Variables.Count == 0 && project.Folder.Folders.Count == 0)
                warnings.Add("No tags or views were found under the given directory.");

            return (project, warnings);
        }

        private static bool LooksLikeTagExport(string jsonFile)
        {
            // Cheap heuristic: peek at the first few KB for tag-export markers without fully parsing every file.
            try
            {
                using var reader = new StreamReader(jsonFile);
                var buffer = new char[4096];
                int read = reader.Read(buffer, 0, buffer.Length);
                var head = new string(buffer, 0, read);
                return head.Contains("\"tagType\"", StringComparison.OrdinalIgnoreCase)
                    || head.Contains("\"dataType\"", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Merges an imported tag folder tree into an existing project folder (e.g. the currently open
        /// project's root <see cref="Folder"/>), combining sub-folders with matching names instead of
        /// duplicating them. Exposed publicly so callers can import tags into an already-open project.
        /// </summary>
        public static void MergeInto(Folder source, Folder target) => MergeFolder(source, target);

        private static void MergeFolder(Folder source, Folder target)
        {
            target.Variables.AddRange(source.Variables);
            foreach (var sub in source.Folders)
            {
                var existing = target.Folders.FirstOrDefault(f => f.Name == sub.Name);
                if (existing == null)
                {
                    target.Folders.Add(sub);
                }
                else
                {
                    MergeFolder(sub, existing);
                }
            }
        }

        // ── JSON helpers ─────────────────────────────────────────────────────

        private static string? GetString(JsonObject? obj, string name)
        {
            if (obj == null) return null;
            if (obj.TryGetPropertyValue(name, out var node) && node != null)
                return node.GetValueKind() == JsonValueKind.String ? node.GetValue<string>() : node.ToString();
            return null;
        }

        private static double? GetDouble(JsonObject? obj, string name)
        {
            if (obj == null) return null;
            if (obj.TryGetPropertyValue(name, out var node) && node is JsonValue value)
            {
                if (value.TryGetValue<double>(out var d)) return d;
                if (value.TryGetValue<string>(out var s) && double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)) return parsed;
            }
            return null;
        }
    }
}
