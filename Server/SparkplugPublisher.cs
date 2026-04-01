using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using Opc.Ua;
using ProtoBuf;
using Serilog;
using SharedModels;

namespace SimpleOpcFileServer;

/// <summary>
/// Sparkplug B edge-node publisher.
///
/// Publishes OPC UA variables as Sparkplug B metrics to an MQTT broker,
/// enabling interoperability with external SCADA systems (Ignition, AVEVA, etc.).
///
/// Lifecycle:
///   NBIRTH on connect → NDATA periodically / on-change → NDEATH via LWT on disconnect.
///
/// Integrates with redundancy: only the active server publishes.
/// </summary>
public sealed class SparkplugPublisher : IDisposable
{
    private readonly SparkplugConfig _config;
    private readonly ConcurrentDictionary<string, BaseDataVariableState> _variables;
    private readonly RedundancyService? _redundancy;
    private readonly EventLogger? _eventLogger;

    private IMqttClient? _client;
    private Timer? _publishTimer;
    private ulong _seq;
    private bool _disposed;
    private bool _connected;

    // Track which variables have changed since last publish
    private readonly ConcurrentDictionary<string, bool> _changedVariables = new();

    // Metric alias map (path → numeric alias for compact DDATA)
    private readonly ConcurrentDictionary<string, ulong> _aliasMap = new();
    private ulong _nextAlias = 1;

    // Compiled include/exclude patterns
    private readonly List<Regex> _includePatterns = new();
    private readonly List<Regex> _excludePatterns = new();

    public SparkplugPublisher(
        SparkplugConfig config,
        ConcurrentDictionary<string, BaseDataVariableState> variables,
        RedundancyService? redundancy,
        EventLogger? eventLogger)
    {
        _config = config;
        _variables = variables;
        _redundancy = redundancy;
        _eventLogger = eventLogger;

        // Compile wildcard patterns
        foreach (var pattern in config.IncludeVariables)
            _includePatterns.Add(WildcardToRegex(pattern));
        foreach (var pattern in config.ExcludeVariables)
            _excludePatterns.Add(WildcardToRegex(pattern));
    }

    // ─── Lifecycle ───────────────────────────────────────────────

    public async Task StartAsync()
    {
        if (string.IsNullOrEmpty(_config.Broker) || string.IsNullOrEmpty(_config.GroupId) || string.IsNullOrEmpty(_config.EdgeNodeId))
        {
            Log.Warning("[SparkplugB Publisher] Missing required config (Broker, GroupId, or EdgeNodeId)");
            return;
        }

        // Redundancy gating: only active server publishes
        if (_redundancy is { IsActive: false })
        {
            Log.Information("[SparkplugB Publisher] Standby mode — deferring publish until active");
            return;
        }

        try
        {
            var factory = new MqttClientFactory();
            _client = factory.CreateMqttClient();

            // NDEATH payload as Last Will and Testament — broker publishes this if we disconnect unexpectedly
            var ndeathPayload = BuildDeathPayload();
            var ndeathTopic = $"spBv1.0/{_config.GroupId}/NDEATH/{_config.EdgeNodeId}";

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(_config.Broker, _config.Port)
                .WithWillTopic(ndeathTopic)
                .WithWillPayload(ndeathPayload)
                .WithWillRetain(false);

            if (!string.IsNullOrEmpty(_config.Username))
                optionsBuilder.WithCredentials(_config.Username, _config.Password ?? "");

            if (_config.UseTls)
                optionsBuilder.WithTlsOptions(o => { });

            var options = optionsBuilder.Build();

            _client.DisconnectedAsync += e =>
            {
                _connected = false;
                Log.Warning("[SparkplugB Publisher] Disconnected: {Reason}", e.Reason);
                // Attempt reconnect after delay
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    if (!_disposed) await ReconnectAsync(options);
                });
                return Task.CompletedTask;
            };

            await _client.ConnectAsync(options);
            _connected = true;
            Log.Information("[SparkplugB Publisher] Connected to {Broker}:{Port}", _config.Broker, _config.Port);
            _eventLogger?.LogSystem("Info", "SparkplugB", $"Publisher connected to {_config.Broker}:{_config.Port}");

            // Publish NBIRTH
            await PublishBirthAsync();

            // Subscribe to NCMD for remote commands (rebirth, etc.)
            var cmdTopic = $"spBv1.0/{_config.GroupId}/NCMD/{_config.EdgeNodeId}";
            await _client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(cmdTopic).Build());
            _client.ApplicationMessageReceivedAsync += HandleCommandAsync;

            // Wire on-change hooks to variables
            if (_config.PublishOnChange)
            {
                foreach (var (path, variable) in _variables)
                {
                    if (!ShouldPublish(path)) continue;
                    variable.OnStateChanged += (ctx, state, masks) =>
                    {
                        if ((masks & NodeStateChangeMasks.Value) != 0)
                        {
                            _changedVariables[path] = true;
                        }
                    };
                }
            }

            // Periodic publish timer
            if (_config.PublishIntervalMs > 0)
            {
                _publishTimer = new Timer(_ => _ = PublishDataAsync(), null,
                    TimeSpan.FromMilliseconds(_config.PublishIntervalMs),
                    TimeSpan.FromMilliseconds(_config.PublishIntervalMs));
            }

            DiagnosticsCollector.Instance.Register("SparkplugB", "Publisher", status: "Connected");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[SparkplugB Publisher] Startup error");
            _eventLogger?.LogSystem("Error", "SparkplugB", $"Publisher startup error: {ex.Message}");
            DiagnosticsCollector.Instance.Register("SparkplugB", "Publisher", status: $"Error: {ex.Message}");
        }
    }

    private async Task ReconnectAsync(MqttClientOptions options)
    {
        try
        {
            if (_client == null || _disposed) return;
            await _client.ConnectAsync(options);
            _connected = true;
            Log.Information("[SparkplugB Publisher] Reconnected");
            await PublishBirthAsync();
        }
        catch (Exception ex)
        {
            Log.Debug("[SparkplugB Publisher] Reconnect failed: {Error}", ex.Message);
        }
    }

    // ─── NBIRTH — metric catalog ────────────────────────────────

    private async Task PublishBirthAsync()
    {
        if (_client == null || !_connected) return;

        var payload = new SparkplugB.SparkplugPayload
        {
            Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Seq = Interlocked.Increment(ref _seq)
        };

        _aliasMap.Clear();
        _nextAlias = 1;

        int count = 0;
        foreach (var (path, variable) in _variables)
        {
            if (!ShouldPublish(path)) continue;

            var alias = _nextAlias++;
            _aliasMap[path] = alias;

            var metric = new SparkplugB.SparkplugPayload.Metric
            {
                Name = path,
                Alias = alias,
                Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Datatype = MapOpcToSparkplugType(variable.DataType),
            };
            SetMetricValue(metric, variable.Value, variable.DataType);
            payload.Metrics.Add(metric);
            count++;
        }

        var topic = $"spBv1.0/{_config.GroupId}/NBIRTH/{_config.EdgeNodeId}";
        await PublishPayloadAsync(topic, payload);
        Log.Information("[SparkplugB Publisher] NBIRTH published: {Count} metrics", count);
        _eventLogger?.LogSystem("Info", "SparkplugB", $"NBIRTH published: {count} metrics");
    }

    // ─── NDATA — periodic / on-change data ──────────────────────

    private async Task PublishDataAsync()
    {
        if (_client == null || !_connected || _disposed) return;
        if (_redundancy is { IsActive: false }) return;

        try
        {
            var payload = new SparkplugB.SparkplugPayload
            {
                Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Seq = Interlocked.Increment(ref _seq)
            };

            // Collect changed variables (or all on periodic)
            var varsToPublish = _config.PublishOnChange
                ? _changedVariables.Keys.ToList()
                : _variables.Keys.Where(ShouldPublish).ToList();

            // Clear changed flags
            foreach (var path in varsToPublish)
                _changedVariables.TryRemove(path, out _);

            if (varsToPublish.Count == 0) return;

            foreach (var path in varsToPublish)
            {
                if (!_variables.TryGetValue(path, out var variable)) continue;
                if (!_aliasMap.TryGetValue(path, out var alias)) continue;

                var metric = new SparkplugB.SparkplugPayload.Metric
                {
                    Alias = alias,
                    Timestamp = variable.Timestamp != DateTime.MinValue
                        ? (ulong)new DateTimeOffset(variable.Timestamp).ToUnixTimeMilliseconds()
                        : (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    Datatype = MapOpcToSparkplugType(variable.DataType),
                };
                SetMetricValue(metric, variable.Value, variable.DataType);
                payload.Metrics.Add(metric);
            }

            if (payload.Metrics.Count > 0)
            {
                var topic = $"spBv1.0/{_config.GroupId}/NDATA/{_config.EdgeNodeId}";
                await PublishPayloadAsync(topic, payload);
                DiagnosticsCollector.Instance.RecordCycle("SparkplugB", "Publisher", 0);
            }
        }
        catch (Exception ex)
        {
            Log.Debug("[SparkplugB Publisher] Publish error: {Error}", ex.Message);
        }
    }

    // ─── NCMD — rebirth command from host ───────────────────────

    private Task HandleCommandAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        try
        {
            var payloadBytes = e.ApplicationMessage.Payload.ToArray();
            if (payloadBytes.Length == 0) return Task.CompletedTask;

            using var ms = new MemoryStream(payloadBytes);
            var payload = Serializer.Deserialize<SparkplugB.SparkplugPayload>(ms);

            // Look for "Node Control/Rebirth" metric
            var rebirth = payload?.Metrics?.FirstOrDefault(m =>
                m.Name == "Node Control/Rebirth" && m.BooleanValue == true);

            if (rebirth != null)
            {
                Log.Information("[SparkplugB Publisher] Rebirth command received — re-publishing NBIRTH");
                _ = PublishBirthAsync();
            }
        }
        catch (Exception ex)
        {
            Log.Debug("[SparkplugB Publisher] Command handler error: {Error}", ex.Message);
        }
        return Task.CompletedTask;
    }

    // ─── Helpers ─────────────────────────────────────────────────

    private bool ShouldPublish(string variablePath)
    {
        // Skip system variables
        if (variablePath.StartsWith("_System.") || variablePath.StartsWith("_Diagnostics."))
            return false;

        // Include filter (empty = all)
        if (_includePatterns.Count > 0 && !_includePatterns.Any(p => p.IsMatch(variablePath)))
            return false;

        // Exclude filter
        if (_excludePatterns.Count > 0 && _excludePatterns.Any(p => p.IsMatch(variablePath)))
            return false;

        return true;
    }

    private static uint MapOpcToSparkplugType(NodeId dataType)
    {
        if (dataType == DataTypeIds.Boolean) return SparkplugB.SparkplugDataType.Boolean;
        if (dataType == DataTypeIds.Int16) return SparkplugB.SparkplugDataType.Int16;
        if (dataType == DataTypeIds.Int32) return SparkplugB.SparkplugDataType.Int32;
        if (dataType == DataTypeIds.Int64) return SparkplugB.SparkplugDataType.Int64;
        if (dataType == DataTypeIds.UInt16) return SparkplugB.SparkplugDataType.UInt16;
        if (dataType == DataTypeIds.UInt32) return SparkplugB.SparkplugDataType.UInt32;
        if (dataType == DataTypeIds.UInt64) return SparkplugB.SparkplugDataType.UInt64;
        if (dataType == DataTypeIds.Float) return SparkplugB.SparkplugDataType.Float;
        if (dataType == DataTypeIds.Double) return SparkplugB.SparkplugDataType.Double;
        if (dataType == DataTypeIds.String) return SparkplugB.SparkplugDataType.String;
        if (dataType == DataTypeIds.DateTime) return SparkplugB.SparkplugDataType.DateTime;
        return SparkplugB.SparkplugDataType.String; // fallback
    }

    private static void SetMetricValue(SparkplugB.SparkplugPayload.Metric metric, object? value, NodeId dataType)
    {
        if (value == null) { metric.IsNull = true; return; }

        try
        {
            if (dataType == DataTypeIds.Boolean && value is bool b) { metric.BooleanValue = b; return; }
            if (dataType == DataTypeIds.Float && value is float f) { metric.FloatValue = f; return; }
            if (dataType == DataTypeIds.Double && value is double d) { metric.DoubleValue = d; return; }
            if (dataType == DataTypeIds.Int32 && value is int i32) { metric.IntValue = (uint)i32; return; }
            if (dataType == DataTypeIds.UInt32 && value is uint u32) { metric.IntValue = u32; return; }
            if (dataType == DataTypeIds.Int16 && value is short i16) { metric.IntValue = (uint)i16; return; }
            if (dataType == DataTypeIds.UInt16 && value is ushort u16) { metric.IntValue = u16; return; }
            if (dataType == DataTypeIds.Int64 && value is long i64) { metric.LongValue = (ulong)i64; return; }
            if (dataType == DataTypeIds.UInt64 && value is ulong u64) { metric.LongValue = u64; return; }

            // Fallback: convert to string
            metric.StringValue = value.ToString();
        }
        catch
        {
            metric.StringValue = value.ToString();
        }
    }

    private static byte[] BuildDeathPayload()
    {
        var payload = new SparkplugB.SparkplugPayload
        {
            Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, payload);
        return ms.ToArray();
    }

    private async Task PublishPayloadAsync(string topic, SparkplugB.SparkplugPayload payload)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, payload);
        var bytes = ms.ToArray();

        var msg = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(bytes)
            .Build();

        await _client!.PublishAsync(msg);
    }

    private static Regex WildcardToRegex(string pattern)
    {
        var escaped = Regex.Escape(pattern).Replace("\\*", ".*");
        return new Regex($"^{escaped}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _publishTimer?.Dispose();

        // Publish explicit NDEATH before disconnecting
        if (_client?.IsConnected == true)
        {
            try
            {
                var topic = $"spBv1.0/{_config.GroupId}/NDEATH/{_config.EdgeNodeId}";
                var deathBytes = BuildDeathPayload();
                var msg = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(deathBytes).Build();
                _client.PublishAsync(msg).GetAwaiter().GetResult();
            }
            catch { }
        }

        _client?.Dispose();
    }
}
