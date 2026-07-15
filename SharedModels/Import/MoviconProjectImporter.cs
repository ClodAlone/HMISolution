// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SharedModels.Import
{
    /// <summary>
    /// Result of a Movicon 11 XML project import: the converted <see cref="NodeModel"/> plus
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

        /// <summary>True when nothing usable was found in the source document (likely wrong/unsupported schema).</summary>
        public bool IsEmpty => TagCount == 0 && ScreenCount == 0;
    }

    /// <summary>
    /// Converts a Movicon 11 XML project export into this application's <see cref="NodeModel"/> project format.
    /// </summary>
    /// <remarks>
    /// Movicon 11 (Progea) project files are not published as a fixed public schema, and exports can vary
    /// between installations/localizations (e.g. Italian vs English element names, "Tags" vs "Variables",
    /// "Schemas"/"Pages" vs "Screens"). This importer is therefore intentionally <b>tolerant</b>: instead of
    /// requiring exact element/attribute names, it recognizes a configurable set of known aliases for each
    /// concept (tag database, folders/groups, screens, symbols, alarms) and falls back gracefully — recording
    /// a warning — when it encounters something it doesn't recognize, rather than throwing.
    ///
    /// When a real Movicon export is available, extend <see cref="Aliases"/> and the type/symbol mapping
    /// tables below with the exact names observed, rather than rewriting the traversal logic.
    /// </remarks>
    public static class MoviconProjectImporter
    {
        /// <summary>Known element-name aliases (case-insensitive) per logical concept, in priority order.</summary>
        private static class Aliases
        {
            public static readonly string[] TagContainer = { "Tags", "Variables", "TagDatabase", "RealTimeDB" };
            public static readonly string[] Tag = { "Tag", "Variable", "RTVariable" };
            public static readonly string[] Folder = { "Group", "Folder", "Area" };
            public static readonly string[] ScreenContainer = { "Screens", "Schemas", "Pages", "Graphics" };
            public static readonly string[] Screen = { "Screen", "Schema", "Page" };
            public static readonly string[] SymbolContainer = { "Objects", "Symbols", "Items", "Shapes" };
            public static readonly string[] Symbol = { "Object", "Symbol", "Item", "Shape" };
            public static readonly string[] AlarmContainer = { "Alarms", "AlarmList" };
            public static readonly string[] Alarm = { "Alarm" };
        }

        /// <summary>Movicon tag data-type names (case-insensitive) mapped to <see cref="Variable.Type"/> values.</summary>
        private static readonly Dictionary<string, string> TypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["bool"] = "Boolean",
            ["boolean"] = "Boolean",
            ["bit"] = "Boolean",
            ["digital"] = "Boolean",
            ["byte"] = "Int32",
            ["word"] = "Int32",
            ["dword"] = "Int32",
            ["short"] = "Int32",
            ["int"] = "Int32",
            ["integer"] = "Int32",
            ["long"] = "Int32",
            ["analog"] = "Double",
            ["float"] = "Double",
            ["single"] = "Double",
            ["real"] = "Double",
            ["double"] = "Double",
            ["string"] = "String",
            ["text"] = "String",
        };

        /// <summary>Movicon graphic-object type names (case-insensitive) mapped to <see cref="ScreenSymbol.Type"/> values.</summary>
        private static readonly Dictionary<string, string> SymbolTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["rectangle"] = "rect",
            ["rect"] = "rect",
            ["square"] = "rect",
            ["ellipse"] = "ellipse",
            ["circle"] = "circle",
            ["line"] = "line",
            ["text"] = "text",
            ["label"] = "text",
            ["button"] = "button",
            ["pushbutton"] = "button",
            ["led"] = "indicator",
            ["indicator"] = "indicator",
            ["bargraph"] = "progressbar",
            ["progressbar"] = "progressbar",
            ["gauge"] = "gauge",
            ["trend"] = "trend",
            ["historicaltrend"] = "trend",
            ["alarmwindow"] = "alarmlist",
            ["alarmlist"] = "alarmlist",
            ["valve"] = "valve",
            ["pump"] = "mimicpump",
            ["tank"] = "tank",
            ["pipe"] = "pipe",
            ["bitmap"] = "svg",
            ["image"] = "svg",
            ["symbollibrary"] = "svg",
        };

        /// <summary>Imports a Movicon 11 XML project file from disk.</summary>
        public static MoviconImportResult ImportFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Movicon project file not found.", filePath);

            using var stream = File.OpenRead(filePath);
            var doc = XDocument.Load(stream, LoadOptions.None);
            var result = Import(doc);
            if (string.IsNullOrWhiteSpace(result.Project.Folder.Name))
                result.Project.Folder.Name = Path.GetFileNameWithoutExtension(filePath);
            return result;
        }

        /// <summary>Imports a Movicon 11 XML project from a raw XML string.</summary>
        public static MoviconImportResult ImportFromXml(string xml)
        {
            var doc = XDocument.Parse(xml, LoadOptions.None);
            return Import(doc);
        }

        /// <summary>Imports a Movicon 11 XML project from an already-parsed <see cref="XDocument"/>.</summary>
        public static MoviconImportResult Import(XDocument doc)
        {
            var result = new MoviconImportResult();
            var project = new NodeModel
            {
                Folder = new Folder { Name = "Root" },
                Server = new ServerSettings(),
            };

            var root = doc.Root;
            if (root == null)
            {
                result.Warnings.Add("The XML document has no root element.");
                result.Project = project;
                return result;
            }

            // Project display name, if present.
            var nameAttr = GetAttr(root, "Name", "ProjectName", "Title");
            if (!string.IsNullOrWhiteSpace(nameAttr))
                project.Folder.Name = nameAttr!;

            ImportTags(root, project, result);
            ImportScreens(root, project, result);

            project.Screens ??= new List<ScreenConfig>();

            if (result.IsEmpty)
            {
                result.Warnings.Add(
                    "No recognizable tag database or screens were found. The file may use a Movicon " +
                    "schema/localization not yet covered by the importer's alias tables — check " +
                    "MoviconProjectImporter.Aliases and extend it with the actual element names.");
            }

            result.Project = project;
            return result;
        }

        // ── Tag database / folders ─────────────────────────────────────────

        private static void ImportTags(XElement root, NodeModel project, MoviconImportResult result)
        {
            var container = FindFirst(root, Aliases.TagContainer);
            if (container == null)
            {
                // Some exports place tags directly under the root without a wrapping container.
                var looseTags = root.Elements().Where(e => IsNamed(e, Aliases.Tag)).ToList();
                if (looseTags.Count == 0) return;
                foreach (var tagEl in looseTags)
                    ImportTagElement(tagEl, project.Folder, result);
                return;
            }

            ImportFolderContents(container, project.Folder, result);
        }

        /// <summary>Recursively imports nested Group/Folder elements and Tag elements into the folder tree.</summary>
        private static void ImportFolderContents(XElement container, Folder targetFolder, MoviconImportResult result)
        {
            foreach (var el in container.Elements())
            {
                if (IsNamed(el, Aliases.Tag))
                {
                    ImportTagElement(el, targetFolder, result);
                }
                else if (IsNamed(el, Aliases.Folder))
                {
                    var subFolder = new Folder { Name = GetAttr(el, "Name", "Path") ?? "Group" };
                    targetFolder.Folders.Add(subFolder);
                    result.FolderCount++;
                    ImportFolderContents(el, subFolder, result);
                }
                else
                {
                    // Unknown wrapper element (e.g. a versioned container) - recurse into it looking for tags/groups.
                    ImportFolderContents(el, targetFolder, result);
                }
            }
        }

        private static void ImportTagElement(XElement tagEl, Folder targetFolder, MoviconImportResult result)
        {
            var name = GetAttr(tagEl, "Name", "TagName", "Id");
            if (string.IsNullOrWhiteSpace(name))
            {
                result.Warnings.Add("Skipped a tag with no Name attribute.");
                return;
            }

            var rawType = GetAttr(tagEl, "Type", "DataType", "VarType") ?? "";
            if (!TypeMap.TryGetValue(rawType, out var mappedType))
            {
                mappedType = "Double";
                if (!string.IsNullOrWhiteSpace(rawType))
                    result.Warnings.Add($"Tag '{name}': unrecognized data type '{rawType}', defaulted to Double.");
            }

            var variable = new Variable
            {
                Name = name!,
                Type = mappedType,
                Access = ParseAccess(GetAttr(tagEl, "Access", "ReadOnly", "Mode")),
                InitialValue = GetAttr(tagEl, "InitialValue", "Default", "DefaultValue") ?? "",
                Description = GetAttr(tagEl, "Description", "Comment") ?? "",
                EngineeringUnit = GetAttr(tagEl, "Unit", "EngUnit", "EngineeringUnit") ?? "",
            };

            var address = GetAttr(tagEl, "Address", "IOAddress", "PLCAddress");
            if (!string.IsNullOrWhiteSpace(address))
                variable.Description = string.IsNullOrEmpty(variable.Description)
                    ? $"Movicon address: {address}"
                    : $"{variable.Description} (Movicon address: {address})";

            var alarm = TryImportAlarm(tagEl, name!);
            if (alarm != null)
            {
                variable.Alarm = alarm;
                result.AlarmCount++;
            }

            targetFolder.Variables.Add(variable);
            result.TagCount++;
        }

        private static string ParseAccess(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "ReadWrite";
            return raw.Trim().ToLowerInvariant() switch
            {
                "true" or "readonly" or "read" => "Read",
                "write" or "writeonly" => "Write",
                _ => "ReadWrite",
            };
        }

        /// <summary>
        /// Extracts alarm thresholds if present directly on the tag element (Movicon commonly stores
        /// Hi/HiHi/Lo/LoLo attributes inline on the tag rather than a separate alarm element).
        /// </summary>
        private static AlarmConfig? TryImportAlarm(XElement tagEl, string tagName)
        {
            var hi = GetAttr(tagEl, "Hi", "HiLimit", "HighLimit");
            var lo = GetAttr(tagEl, "Lo", "LoLimit", "LowLimit");
            var hiHi = GetAttr(tagEl, "HiHi", "HiHiLimit", "HighHighLimit");
            var loLo = GetAttr(tagEl, "LoLo", "LoLoLimit", "LowLowLimit");

            if (hi == null && lo == null && hiHi == null && loLo == null)
                return null;

            var alarm = new AlarmConfig
            {
                TriggerType = AlarmTriggerType.Limit,
                Message = GetAttr(tagEl, "AlarmMessage", "AlarmText") ?? $"{tagName} alarm",
            };
            if (double.TryParse(hi, NumberStyles.Float, CultureInfo.InvariantCulture, out var hiVal)) alarm.HighLimit = hiVal;
            if (double.TryParse(lo, NumberStyles.Float, CultureInfo.InvariantCulture, out var loVal)) alarm.LowLimit = loVal;
            if (double.TryParse(hiHi, NumberStyles.Float, CultureInfo.InvariantCulture, out var hiHiVal)) alarm.HighHighLimit = hiHiVal;
            if (double.TryParse(loLo, NumberStyles.Float, CultureInfo.InvariantCulture, out var loLoVal)) alarm.LowLowLimit = loLoVal;
            return alarm;
        }

        // ── Screens / symbols ───────────────────────────────────────────────

        private static void ImportScreens(XElement root, NodeModel project, MoviconImportResult result)
        {
            var container = FindFirst(root, Aliases.ScreenContainer);
            IEnumerable<XElement> screenElements = container != null
                ? container.Elements().Where(e => IsNamed(e, Aliases.Screen))
                : root.Descendants().Where(e => IsNamed(e, Aliases.Screen));

            foreach (var screenEl in screenElements)
            {
                var screen = new ScreenConfig
                {
                    Name = GetAttr(screenEl, "Name", "Title") ?? $"Screen{result.ScreenCount + 1}",
                    Width = GetIntAttr(screenEl, 800, "Width", "SizeX"),
                    Height = GetIntAttr(screenEl, 600, "Height", "SizeY"),
                    Background = GetAttr(screenEl, "BackColor", "Background") is string bg && !string.IsNullOrWhiteSpace(bg)
                        ? NormalizeColor(bg)
                        : "#ffffff",
                };

                var symbolContainer = FindFirst(screenEl, Aliases.SymbolContainer) ?? screenEl;
                foreach (var symEl in symbolContainer.Elements().Where(e => IsNamed(e, Aliases.Symbol)))
                {
                    screen.Symbols.Add(ImportSymbol(symEl, result));
                    result.SymbolCount++;
                }

                project.Screens.Add(screen);
                result.ScreenCount++;
            }
        }

        private static ScreenSymbol ImportSymbol(XElement symEl, MoviconImportResult result)
        {
            var rawType = GetAttr(symEl, "Type", "ObjectType", "Class") ?? "";
            if (!SymbolTypeMap.TryGetValue(rawType, out var mappedType))
            {
                mappedType = "rect";
                if (!string.IsNullOrWhiteSpace(rawType))
                    result.Warnings.Add($"Symbol '{GetAttr(symEl, "Name") ?? "(unnamed)"}': unrecognized object type '{rawType}', mapped to generic rect.");
            }

            var symbol = new ScreenSymbol
            {
                Id = GetAttr(symEl, "Id", "Name") ?? Guid.NewGuid().ToString("N"),
                Type = mappedType,
                X = GetDoubleAttr(symEl, 0, "X", "Left", "PosX"),
                Y = GetDoubleAttr(symEl, 0, "Y", "Top", "PosY"),
                Width = GetDoubleAttr(symEl, 80, "Width", "SizeX", "W"),
                Height = GetDoubleAttr(symEl, 40, "Height", "SizeY", "H"),
                Label = GetAttr(symEl, "Text", "Caption", "Label") ?? "",
                Rotation = GetDoubleAttr(symEl, 0, "Rotation", "Angle"),
            };

            var fill = GetAttr(symEl, "FillColor", "BackColor", "Color");
            if (!string.IsNullOrWhiteSpace(fill)) symbol.Fill = NormalizeColor(fill);

            var stroke = GetAttr(symEl, "BorderColor", "LineColor", "PenColor");
            if (!string.IsNullOrWhiteSpace(stroke)) symbol.Stroke = NormalizeColor(stroke);

            var tagBinding = GetAttr(symEl, "Tag", "Variable", "VariablePath", "AnimationTag");
            if (!string.IsNullOrWhiteSpace(tagBinding))
                symbol.VariablePath = tagBinding;

            return symbol;
        }

        // ── XML helpers ──────────────────────────────────────────────────────

        private static bool IsNamed(XElement el, string[] aliases) =>
            aliases.Any(a => string.Equals(el.Name.LocalName, a, StringComparison.OrdinalIgnoreCase));

        /// <summary>Finds the first descendant (including self's children) whose local name matches one of the aliases.</summary>
        private static XElement? FindFirst(XElement scope, string[] aliases) =>
            scope.Descendants().FirstOrDefault(e => IsNamed(e, aliases));

        /// <summary>Returns the first matching attribute value (case-insensitive name), or the first matching child element's value.</summary>
        private static string? GetAttr(XElement el, params string[] names)
        {
            foreach (var name in names)
            {
                var attr = el.Attributes().FirstOrDefault(a => string.Equals(a.Name.LocalName, name, StringComparison.OrdinalIgnoreCase));
                if (attr != null) return attr.Value;
            }
            foreach (var name in names)
            {
                var child = el.Elements().FirstOrDefault(c => string.Equals(c.Name.LocalName, name, StringComparison.OrdinalIgnoreCase));
                if (child != null) return child.Value;
            }
            return null;
        }

        private static int GetIntAttr(XElement el, int fallback, params string[] names)
        {
            var raw = GetAttr(el, names);
            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : fallback;
        }

        private static double GetDoubleAttr(XElement el, double fallback, params string[] names)
        {
            var raw = GetAttr(el, names);
            return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : fallback;
        }

        /// <summary>Normalizes a Movicon color value (hex "#RRGGBB", "RRGGBB", or decimal BGR integer) to a CSS hex color.</summary>
        private static string NormalizeColor(string raw)
        {
            raw = raw.Trim();
            if (raw.StartsWith("#")) return raw;
            if (raw.Length is 6 or 8 && raw.All(Uri.IsHexDigit)) return "#" + raw[^6..];
            if (long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var dec) && dec >= 0)
            {
                // Movicon/OLE colors are commonly stored as 0x00BBGGRR decimal.
                int r = (int)(dec & 0xFF);
                int g = (int)((dec >> 8) & 0xFF);
                int b = (int)((dec >> 16) & 0xFF);
                return $"#{r:X2}{g:X2}{b:X2}";
            }
            return raw;
        }
    }
}
