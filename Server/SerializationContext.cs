using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text.Json;
using SharedModels;

namespace SimpleOpcFileServer
{
    [JsonSerializable(typeof(Dictionary<string, Folder>))]
    [JsonSerializable(typeof(Folder))]
    [JsonSerializable(typeof(Variable))]
    [JsonSerializable(typeof(AlarmConfig))]
    [JsonSerializable(typeof(AlarmTriggerType))]
    [JsonSerializable(typeof(Dictionary<string, JsonElement>))]
    [JsonSerializable(typeof(ScriptConfig))]
    [JsonSerializable(typeof(List<ScriptConfig>))]
    [JsonSerializable(typeof(PlcProgramConfig))]
    [JsonSerializable(typeof(List<PlcProgramConfig>))]
    public partial class AppJsonContext : JsonSerializerContext
    {
    }
}
