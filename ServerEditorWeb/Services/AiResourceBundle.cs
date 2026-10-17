// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ServerEditorWeb.Models;
using SharedModels;

namespace ServerEditorWeb.Services;

public sealed record AiResourceImportRequest(ProjectNode Project, string Json);

public static class AiResourceBundle
{
    public const string NamespaceToken = "__TASK__";
    public static IReadOnlySet<string> Sections { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "Folder", "Scripts", "PlcPrograms", "Screens", "Recipes", "Schedulers", "Reports", "CalculatedVariables",
        "Assets", "BatchSequences", "Events", "AutomationRules", "Users", "UserGroups", "Strings", "Images",
        "Cameras", "AliasMaps", "UserSymbolGroups", "AiAgents", "McpConnections"
    };

    public static NodeModel Parse(string json, string importNamespace)
    {
        var root = JsonNode.Parse(json) as JsonObject ?? throw new JsonException("Return one resource bundle object.");
        foreach (var (section, value) in root)
        {
            if (!Sections.Contains(section))
                throw new JsonException($"'{section}' is not a resource section. Do not include Server, Database, project settings, or existing resources.");
            if (value == null || (section == "Folder" ? value is not JsonObject : value is not JsonArray))
                throw new JsonException($"'{section}' must be {(section == "Folder" ? "an object" : "an array")}, not null.");
        }

        ReplaceNamespace(root, importNamespace);
        ValidateArrays(root);
        NormalizeScreenSymbols(root);
        foreach (var (section, value) in root)
        {
            if (section == "Folder")
            {
                if (value!["Name"]?.GetValue<string>() != importNamespace)
                    throw new JsonException($"Folder.Name must be '{NamespaceToken}'. All new variables belong inside that folder.");
                ValidateFolder((JsonObject)value);
                continue;
            }

            var key = section == "Strings" ? "Key" : section == "Images" ? "Id"
                : section == "Users" ? "Username" : section == "Cameras" ? "CameraId" : "Name";
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in (JsonArray)value!)
            {
                var name = item?[key]?.GetValue<string>();
                if (string.IsNullOrWhiteSpace(name) || !name.StartsWith(importNamespace + "_", StringComparison.Ordinal)
                    || name.IndexOfAny(['/', '\\', ':']) >= 0 || !names.Add(name))
                    throw new JsonException($"Each {section}.{key} must be unique, contain no path separators, and start with '{NamespaceToken}_'.");
                if (key != "Id" && item?["Id"] is JsonValue idValue)
                {
                    var id = idValue.GetValue<string>();
                    if (!id.StartsWith(importNamespace + "_", StringComparison.Ordinal) || !ids.Add(id))
                        throw new JsonException($"Each {section}.Id must be unique and start with '{NamespaceToken}_'.");
                }
            }
        }

        var resolver = new DefaultJsonTypeInfoResolver();
        resolver.Modifiers.Add(typeInfo =>
        {
            foreach (var property in typeInfo.Properties)
            {
                if (property.PropertyType == typeof(string) && property.Name.EndsWith("Id", StringComparison.Ordinal))
                    property.CustomConverter = new NumericIdentifierConverter();
            }
        });
        var options = new JsonSerializerOptions
        {
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            RespectNullableAnnotations = true,
            TypeInfoResolver = resolver
        };
        var model = root.Deserialize<NodeModel>(options) ?? throw new JsonException("The resource bundle is empty.");
        model.Folder.Name = importNamespace;
        var cameraSymbols = model.Screens.SelectMany(screen => screen.Symbols).Where(symbol => symbol.Type == "ipcamera" && symbol.Camera != null).ToArray();
        foreach (var camera in model.Cameras)
        {
            var hosts = cameraSymbols.Where(symbol => symbol.Camera!.CameraId == camera.CameraId).ToArray();
            if (hosts.Length == 0) throw new JsonException("Each camera must be hosted by a new ipcamera screen symbol so it is retained on save.");
            foreach (var host in hosts) host.Camera = camera;
        }
        foreach (var symbol in cameraSymbols)
        {
            var camera = symbol.Camera!;
            if (!camera.CameraId.StartsWith(importNamespace + "_", StringComparison.Ordinal))
                throw new JsonException($"Camera.CameraId must start with '{NamespaceToken}_'.");
            if (model.Cameras.All(item => item.CameraId != camera.CameraId)) model.Cameras.Add(camera);
        }
        ValidateReferences(root, model);

        foreach (var item in model.Scripts) { item.Group = importNamespace; item.Enabled = false; }
        foreach (var item in model.PlcPrograms) { item.Group = importNamespace; item.Enabled = false; }
        foreach (var item in model.Screens) item.Group = importNamespace;
        foreach (var item in model.Recipes) item.DbPath = Path.Combine("recipes", importNamespace, item.Name + ".db");
        foreach (var item in model.CalculatedVariables) item.FolderPath = importNamespace + ".Calculated";
        foreach (var item in model.Assets) item.Group = importNamespace;
        foreach (var item in model.Events) { item.Group = importNamespace; item.Enabled = false; }
        foreach (var item in model.Reports) item.Group = importNamespace;
        foreach (var item in model.Schedulers) item.Enabled = false;
        foreach (var item in model.BatchSequences) item.Enabled = false;
        foreach (var item in model.AutomationRules) { item.Group = importNamespace; item.Enabled = false; }
        foreach (var item in model.AiAgents) item.Enabled = false;
        foreach (var item in model.McpConnections) item.Enabled = false;
        return model;
    }

    private static void NormalizeScreenSymbols(JsonObject root)
    {
        void Normalize(JsonObject symbol)
        {
            var type = symbol["Type"]?.GetValue<string>();
            void Map(string alias, string target)
            {
                if (!symbol.TryGetPropertyValue(alias, out var value)) return;
                if (symbol.TryGetPropertyValue(target, out var existing) && !JsonNode.DeepEquals(existing, value))
                    throw new JsonException($"Symbol '{symbol["Id"]}' has conflicting '{alias}' and '{target}' values. Keep only '{target}'.");
                symbol[target] = value?.DeepClone();
                symbol.Remove(alias);
            }

            Map("Text", "Label");
            Map("Color", "Fill");
            Map("BorderColor", "Stroke");
            if (type == "numericdisplay")
            {
                Map("Unit", "NumericDisplayUnit");
                Map("BackgroundColor", "NumericDisplayBackground");
                if (symbol.TryGetPropertyValue("DecimalPlaces", out var decimals))
                {
                    if (decimals is not JsonValue value || !value.TryGetValue<int>(out var count) || count is < 0 or > 99)
                        throw new JsonException("Numeric display DecimalPlaces must be an integer from 0 to 99.");
                    var format = "F" + count.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    if (symbol.TryGetPropertyValue("NumericDisplayFormat", out var existing) && existing?.GetValue<string>() != format)
                        throw new JsonException("DecimalPlaces conflicts with NumericDisplayFormat. Keep only NumericDisplayFormat.");
                    symbol["NumericDisplayFormat"] = format;
                    Map("DecimalPlaces", "NumericDisplayDecimals");
                }
            }
            if (symbol.TryGetPropertyValue("HorizontalAlignment", out var alignment) && type is "text" or "numericdisplay")
            {
                if (alignment?.GetValue<string>() != "center")
                    throw new JsonException("Text and numeric displays use centered alignment. Remove HorizontalAlignment or set it to 'center'.");
                symbol.Remove("HorizontalAlignment");
            }
        }

        void NormalizeCollection(JsonNode? resources, string property)
        {
            if (resources is not JsonArray array) return;
            foreach (var resource in array)
            {
                if (resource is not JsonObject obj || obj[property] is not JsonArray symbols) continue;
                foreach (var symbol in symbols.OfType<JsonObject>()) Normalize(symbol);
            }
        }
        NormalizeCollection(root["Screens"], "Symbols");
        NormalizeCollection(root["UserSymbolGroups"], "TemplateSymbols");
    }

    private sealed class NumericIdentifierConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String) return reader.GetString();
            if (reader.TokenType == JsonTokenType.Number)
            {
                using var number = JsonDocument.ParseValue(ref reader);
                return number.RootElement.GetRawText();
            }
            throw new JsonException("Identifiers must be strings or numbers, not objects, arrays, or booleans.");
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            => writer.WriteStringValue(value);
    }

    private static void ReplaceNamespace(JsonNode node, string importNamespace)
    {
        if (node is JsonObject obj)
        {
            foreach (var key in obj.Select(pair => pair.Key).ToArray())
            {
                if (obj[key] is JsonValue value && value.TryGetValue<string>(out var text))
                    obj[key] = text.Replace(NamespaceToken, importNamespace, StringComparison.Ordinal);
                else if (obj[key] is { } child)
                    ReplaceNamespace(child, importNamespace);
                var renamedKey = key.Replace(NamespaceToken, importNamespace, StringComparison.Ordinal);
                if (renamedKey != key)
                {
                    if (obj.ContainsKey(renamedKey)) throw new JsonException($"Namespace substitution creates a duplicate dictionary key '{renamedKey}'.");
                    var renamedValue = obj[key];
                    obj.Remove(key);
                    obj.Add(renamedKey, renamedValue);
                }
            }
        }
        else if (node is JsonArray array)
        {
            for (var i = 0; i < array.Count; i++)
            {
                if (array[i] is JsonValue value && value.TryGetValue<string>(out var text))
                    array[i] = text.Replace(NamespaceToken, importNamespace, StringComparison.Ordinal);
                else if (array[i] is { } child)
                    ReplaceNamespace(child, importNamespace);
            }
        }
    }

    private static void ValidateArrays(JsonNode node)
    {
        if (node is JsonArray array)
        {
            foreach (var item in array)
            {
                if (item == null) throw new JsonException("Resource arrays cannot contain null items.");
                ValidateArrays(item);
            }
        }
        else if (node is JsonObject obj)
        {
            foreach (var (_, child) in obj)
                if (child != null) ValidateArrays(child);
        }
    }

    private static void ValidateFolder(JsonObject folder)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var section in new[] { "Folders", "Variables" })
        {
            if (folder[section] is not JsonArray items) continue;
            foreach (var item in items)
            {
                var name = item?["Name"]?.GetValue<string>();
                if (string.IsNullOrWhiteSpace(name) || name.IndexOfAny(['.', '/', '\\']) >= 0 || !names.Add(name))
                    throw new JsonException("Folder and variable names must be unique within their folder and cannot contain path separators.");
                if (section == "Folders") ValidateFolder((JsonObject)item!);
            }
        }
    }

    private static void ValidateReferences(JsonObject root, NodeModel model)
    {
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var folders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var aliases = model.AliasMaps.SelectMany(map => map.Mappings.Keys)
            .Concat(model.UserSymbolGroups.SelectMany(group => group.AliasDefinitions)).ToHashSet(StringComparer.Ordinal);
        void Walk(Folder folder, string prefix)
        {
            folders.Add(prefix);
            foreach (var variable in folder.Variables) paths.Add(prefix + "." + variable.Name);
            foreach (var child in folder.Folders) Walk(child, prefix + "." + child.Name);
        }
        Walk(model.Folder, model.Folder.Name);
        foreach (var item in model.CalculatedVariables)
        {
            if (item.FolderPath != model.Folder.Name + ".Calculated")
                throw new JsonException($"Calculated FolderPath must be '{NamespaceToken}.Calculated'.");
            if (!paths.Add(item.FolderPath + "." + item.Name))
                throw new JsonException($"Calculated tag '{item.Name}' conflicts with a variable in this bundle.");
        }

        var references = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        void Register(IEnumerable<string> names, params string[] fields)
        {
            var set = names.ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var field in fields) references.Add(field, set);
        }
        Register(model.Scripts.Select(item => item.Name), "Script", "ScriptName", "AllowedScripts");
        Register(model.Screens.Select(item => item.Name), "NavButtonTarget", "TargetScreen", "PopupScreen", "EmbeddedScreens");
        Register(model.Recipes.Select(item => item.Name), "RecipeName");
        Register(model.Schedulers.Select(item => item.Name), "SchedulerName");
        Register(model.Reports.Select(item => item.Name), "TargetReport", "ReportName");
        Register(model.AliasMaps.Select(item => item.Name), "AliasMapName", "PopupAliasMapName", "EmbeddedScreenAliasMaps");
        Register(model.UserGroups.Select(item => item.Name), "AllowedGroups");
        Register(model.Images.Select(item => item.Id), "BackgroundImageId");
        Register(model.Cameras.Select(item => item.CameraId), "CameraId");
        Register(model.UserSymbolGroups.Select(item => item.Name), "UserSymbolGroupName");
        foreach (var user in model.Users)
            if (!string.IsNullOrEmpty(user.Group) && !model.UserGroups.Any(group => group.Name == user.Group))
                throw new JsonException($"User group '{user.Group}' does not exist in this bundle.");
        foreach (var agent in model.AiAgents)
            foreach (var tool in agent.AllowedMcpTools)
                if (!model.McpConnections.Any(connection => tool.StartsWith(connection.Name + ".", StringComparison.Ordinal)))
                    throw new JsonException($"MCP tool '{tool}' needs a matching connection in this bundle.");

        void Check(string path, bool allowFolder = false)
        {
            var aliasEnd = path.IndexOf('}');
            if (path.StartsWith('{') && aliasEnd > 1 && aliases.Contains(path[1..aliasEnd])) return;
            if (!string.IsNullOrEmpty(path) && !paths.Contains(path) && !(allowFolder && folders.Contains(path)))
                throw new JsonException($"Variable reference '{path}' does not resolve inside this bundle. Add the variable under Folder or fix the reference.");
        }
        void CheckResource(string field, string name)
        {
            if (!string.IsNullOrEmpty(name) && !references[field].Contains(name))
                throw new JsonException($"Resource reference {field}='{name}' does not resolve inside this bundle.");
        }
        void Visit(JsonNode node)
        {
            if (node is JsonArray array)
            {
                foreach (var item in array) if (item != null) Visit(item);
            }
            else if (node is JsonObject obj)
            {
                foreach (var (key, child) in obj)
                {
                    if (child is JsonValue value && value.TryGetValue<string>(out var text))
                    {
                        if (key.EndsWith("VariablePath", StringComparison.Ordinal) || key is
                            "OutputPath" or "SetpointPath" or "RangeLowPath" or "RangeHighPath" or "MotorFaultPath" or
                            "MotorControlFaultPath" or "MotorControlSpeedPath" or "ValvePositionPath" or "ConveyorFaultPath" or
                            "HeatExchangerHotInPath" or "HeatExchangerColdInPath" or "SetpointRatePath" or "SetpointTargetPath" or
                            "FlowMeterTotalizerPath" or "XYPlotXPath" or "XYPlotYPath" or "KpiAvailabilityPath" or "KpiPerformancePath" or
                            "KpiQualityPath" or "KpiUptimePath" or "KpiThroughputPath" or "KpiThroughputTargetPath" or
                            "YoloEnableVariable" or "RecordingTriggerVariable" or "TriggerVariable") Check(text);
                        else if (key is "AggregateSourcePath" or "PopupVariables")
                            foreach (var path in text.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) Check(path);
                        else if (references.ContainsKey(key)) CheckResource(key, text);
                    }
                    else if (child is JsonArray list && (key.EndsWith("VariablePaths", StringComparison.Ordinal)
                        || key is "ReadableVariables" or "WritableVariables"))
                    {
                        foreach (var item in list) Check(item!.GetValue<string>());
                    }
                    else if (child is JsonArray resourceList && references.ContainsKey(key))
                    {
                        foreach (var item in resourceList) CheckResource(key, item!.GetValue<string>());
                    }
                    else if (child is JsonObject mappings && key is "Mappings" or "InstanceAliases")
                    {
                        foreach (var (_, path) in mappings)
                        {
                            if (path is not JsonValue pathValue || !pathValue.TryGetValue<string>(out var mappedPath))
                                throw new JsonException("Alias mappings must contain non-null variable or folder paths.");
                            Check(mappedPath, allowFolder: true);
                        }
                    }
                    else if (child != null) Visit(child);
                }
            }
        }
        Visit(root);
    }
}
