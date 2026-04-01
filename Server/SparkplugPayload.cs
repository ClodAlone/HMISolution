using ProtoBuf;

namespace SimpleOpcFileServer.SparkplugB;

/// <summary>
/// Sparkplug B Protobuf payload — matches the official sparkplug_b.proto schema.
/// Used for encoding/decoding NBIRTH, NDEATH, DBIRTH, DDEATH, NDATA, DDATA messages.
/// Duplicated from the SparkplugB driver for use by the server-side publisher.
/// </summary>
[ProtoContract]
public class SparkplugPayload
{
    [ProtoMember(1)]
    public ulong Timestamp { get; set; }

    [ProtoMember(2)]
    public List<Metric> Metrics { get; set; } = new();

    [ProtoMember(3)]
    public ulong Seq { get; set; }

    [ProtoContract]
    public class Metric
    {
        [ProtoMember(1)]
        public string Name { get; set; } = "";

        [ProtoMember(2)]
        public ulong Alias { get; set; }

        [ProtoMember(3)]
        public ulong Timestamp { get; set; }

        [ProtoMember(4)]
        public uint Datatype { get; set; }

        [ProtoMember(5)]
        public bool IsHistorical { get; set; }

        [ProtoMember(6)]
        public bool IsTransient { get; set; }

        [ProtoMember(7)]
        public bool IsNull { get; set; }

        [ProtoMember(10)]
        public uint? IntValue { get; set; }

        [ProtoMember(11)]
        public ulong? LongValue { get; set; }

        [ProtoMember(12)]
        public float? FloatValue { get; set; }

        [ProtoMember(13)]
        public double? DoubleValue { get; set; }

        [ProtoMember(14)]
        public bool? BooleanValue { get; set; }

        [ProtoMember(15)]
        public string? StringValue { get; set; }

        [ProtoMember(16)]
        public byte[]? BytesValue { get; set; }
    }
}

/// <summary>
/// Sparkplug B data type constants (from the specification).
/// </summary>
public static class SparkplugDataType
{
    public const uint Unknown = 0;
    public const uint Int8 = 1;
    public const uint Int16 = 2;
    public const uint Int32 = 3;
    public const uint Int64 = 4;
    public const uint UInt8 = 5;
    public const uint UInt16 = 6;
    public const uint UInt32 = 7;
    public const uint UInt64 = 8;
    public const uint Float = 9;
    public const uint Double = 10;
    public const uint Boolean = 11;
    public const uint String = 12;
    public const uint DateTime = 13;
    public const uint Text = 14;
}
