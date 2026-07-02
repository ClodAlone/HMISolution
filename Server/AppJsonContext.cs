// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using SharedModels;

namespace SimpleOpcFileServer
{
    [JsonSerializable(typeof(NodeModel))]
    public partial class ServerJsonContext : JsonSerializerContext
    {
    }
}
