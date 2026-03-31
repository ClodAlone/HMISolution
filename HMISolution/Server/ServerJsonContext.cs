using System.Text.Json;
using System.Text.Json.Serialization;
using SharedModels;

namespace SimpleOpcFileServer;

/// <summary>
/// Source-generated JSON serializer context for the server.
/// Provides optimised (de)serialization for model types used during config loading and comparison.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(NodeModel))]
[JsonSerializable(typeof(Variable))]
[JsonSerializable(typeof(DatabaseConfig))]
[JsonSerializable(typeof(List<Variable>))]
[JsonSerializable(typeof(List<CameraConfig>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class ServerJsonContext : JsonSerializerContext
{
}
