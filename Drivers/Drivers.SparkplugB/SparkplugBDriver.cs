// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using Opc.Ua;
using ProtoBuf;
using Serilog;
using SimpleOpcFileServer.SparkplugB;

namespace SimpleOpcFileServer;

/// <summary>
/// Per-variable configuration for the Sparkplug B subscriber driver.
/// Placed in the variable's DriverConfigs under the key "SparkplugB".
/// </summary>
public class SparkplugBConfig
{
    public string Broker { get; set; } = "";
    public int Port { get; set; } = 1883;
    public string GroupId { get; set; } = "";
    public string NodeId { get; set; } = "";
    public string DeviceId { get; set; } = "";
    public string MetricName { get; set; } = "";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool UseTls { get; set; }
}

/// <summary>
/// Sparkplug B subscriber (Host Application) driver.
///
/// Subscribes to Sparkplug B topics on MQTT brokers and maps received
/// metrics to OPC UA variables. Handles BIRTH (metric registration),
/// DATA (value updates), and DEATH (status→Bad) messages per the
/// Sparkplug B specification.
///
/// Variable config example:
/// <code>
/// {
///   "Name": "ExternalPLC.Temperature",
///   "Type": "Double",
///   "SparkplugB": {
///     "Broker": "mqtt.local",
///     "Port": 1883,
///     "GroupId": "Plant1",
///     "NodeId": "PLC-01",
///     "DeviceId": "Furnace",
///     "MetricName": "Temperature"
///   }
/// }
/// </code>
/// </summary>
public class SparkplugBDriver : IDriver, IDisposable
{
    public string Key => "SparkplugB";
    public event Action<string, string>? OnError;
    public event Action<double>? OnCycleCompleted;

    private readonly ISystemContext _context;
    private readonly object _lock = new();
    private readonly List<SparkplugBItem> _items = new();
    private readonly Dictionary<string, IMqttClient> _clients = new();
    private readonly MqttClientFactory _factory = new();
    private bool _disposed;

    // Alias-to-name map per node: "group/node/device" → { alias → metricName }
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<ulong, string>> _aliasMap = new();

    public SparkplugBDriver(ISystemContext context) { _context = context; }

    public void AddItem(BaseDataVariableState variable, string configJson)
    {
        var cfg = JsonSerializer.Deserialize<SparkplugBConfig>(configJson);
        if (cfg == null || string.IsNullOrEmpty(cfg.Broker) || string.IsNullOrEmpty(cfg.GroupId))
            return;

        var item = new SparkplugBItem { Variable = variable, Config = cfg };
        lock (_lock) { _items.Add(item); }
        Task.Run(() => EnsureSubscription(cfg));
    }

    // ─── MQTT connection & subscription ──────────────────────────

    private async Task EnsureSubscription(SparkplugBConfig cfg)
    {
        var brokerKey = $"{cfg.Broker}:{cfg.Port}";
        IMqttClient client;
        bool isNew = false;

        lock (_lock)
        {
            if (!_clients.TryGetValue(brokerKey, out client!))
            {
                client = _factory.CreateMqttClient();
                _clients[brokerKey] = client;
                isNew = true;
            }
        }

        if (isNew)
        {
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(cfg.Broker, cfg.Port);

            if (!string.IsNullOrEmpty(cfg.Username))
                optionsBuilder.WithCredentials(cfg.Username, cfg.Password ?? "");

            if (cfg.UseTls)
                optionsBuilder.WithTlsOptions(o => { });

            var options = optionsBuilder.Build();

            client.ApplicationMessageReceivedAsync += e =>
            {
                OnSparkplugMessage(e);
                return Task.CompletedTask;
            };

            client.DisconnectedAsync += e =>
            {
                Log.Warning("[SparkplugB] Disconnected from {Broker}: {Reason}", brokerKey, e.Reason);
                OnError?.Invoke(brokerKey, $"Disconnected: {e.Reason}");
                SetAllBrokerItemsBad(brokerKey);
                return Task.CompletedTask;
            };

            try
            {
                await client.ConnectAsync(options);
                Log.Information("[SparkplugB] Connected to {Broker}", brokerKey);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[SparkplugB] Connection error for {Broker}", brokerKey);
                OnError?.Invoke(brokerKey, $"Connection error: {ex.Message}");
                SetAllBrokerItemsBad(brokerKey);
                return;
            }
        }

        if (!client.IsConnected) return;

        // Subscribe to all message types for this group/node/device
        // spBv1.0/{group_id}/{message_type}/{edge_node_id}/{device_id}
        var topics = new[] { "DBIRTH", "DDATA", "DDEATH", "NBIRTH", "NDATA", "NDEATH" };
        var subBuilder = new MqttClientSubscribeOptionsBuilder();
        foreach (var msgType in topics)
        {
            var topic = string.IsNullOrEmpty(cfg.DeviceId)
                ? $"spBv1.0/{cfg.GroupId}/{msgType}/{cfg.NodeId}"
                : $"spBv1.0/{cfg.GroupId}/{msgType}/{cfg.NodeId}/{cfg.DeviceId}";
            subBuilder.WithTopicFilter(topic);
        }

        try
        {
            await client.SubscribeAsync(subBuilder.Build());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[SparkplugB] Subscription error for {Group}/{Node}", cfg.GroupId, cfg.NodeId);
            OnError?.Invoke($"{cfg.GroupId}/{cfg.NodeId}", $"Subscribe error: {ex.Message}");
        }
    }

    // ─── Message handling ────────────────────────────────────────

    private void OnSparkplugMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var topicParts = e.ApplicationMessage.Topic.Split('/');
            // spBv1.0 / groupId / messageType / nodeId / [deviceId]
            if (topicParts.Length < 4 || topicParts[0] != "spBv1.0") return;

            var groupId = topicParts[1];
            var messageType = topicParts[2];
            var nodeId = topicParts[3];
            var deviceId = topicParts.Length > 4 ? topicParts[4] : "";
            var nodeKey = $"{groupId}/{nodeId}/{deviceId}";

            var payload = DeserializePayload(e.ApplicationMessage.Payload.ToArray());
            if (payload == null) return;

            switch (messageType)
            {
                case "NBIRTH":
                case "DBIRTH":
                    HandleBirth(nodeKey, payload);
                    break;

                case "NDATA":
                case "DDATA":
                    HandleData(nodeKey, groupId, nodeId, deviceId, payload);
                    break;

                case "NDEATH":
                case "DDEATH":
                    HandleDeath(groupId, nodeId, deviceId);
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Debug("[SparkplugB] Message processing error: {Error}", ex.Message);
        }

        sw.Stop();
        OnCycleCompleted?.Invoke(sw.Elapsed.TotalMilliseconds);
    }

    private void HandleBirth(string nodeKey, SparkplugPayload payload)
    {
        // Birth messages define the metric catalog with names and aliases
        var aliases = _aliasMap.GetOrAdd(nodeKey, _ => new ConcurrentDictionary<ulong, string>());
        aliases.Clear();

        foreach (var metric in payload.Metrics)
        {
            if (!string.IsNullOrEmpty(metric.Name) && metric.Alias > 0)
                aliases[metric.Alias] = metric.Name;

            // Also apply initial values from birth
            ApplyMetricToVariables(metric, nodeKey);
        }

        Log.Information("[SparkplugB] BIRTH: {NodeKey} — {Count} metrics registered", nodeKey, payload.Metrics.Count);
    }

    private void HandleData(string nodeKey, string groupId, string nodeId, string deviceId, SparkplugPayload payload)
    {
        foreach (var metric in payload.Metrics)
        {
            // Resolve alias to name if Name is empty
            if (string.IsNullOrEmpty(metric.Name) && metric.Alias > 0)
            {
                if (_aliasMap.TryGetValue(nodeKey, out var aliases) && aliases.TryGetValue(metric.Alias, out var resolved))
                    metric.Name = resolved;
                else
                    continue; // unknown alias, skip
            }

            ApplyMetricToVariables(metric, nodeKey, groupId, nodeId, deviceId);
        }
    }

    private void HandleDeath(string groupId, string nodeId, string deviceId)
    {
        List<SparkplugBItem> matching;
        lock (_lock)
        {
            matching = _items.Where(i =>
                i.Config.GroupId == groupId &&
                i.Config.NodeId == nodeId &&
                (string.IsNullOrEmpty(deviceId) || i.Config.DeviceId == deviceId))
                .ToList();
        }

        foreach (var item in matching)
        {
            item.Variable.StatusCode = StatusCodes.BadNotConnected;
            item.Variable.Timestamp = DateTime.UtcNow;
            item.Variable.ClearChangeMasks(_context, false);
        }

        Log.Information("[SparkplugB] DEATH: {Group}/{Node}/{Device} — {Count} variables set to Bad",
            groupId, nodeId, deviceId, matching.Count);
    }

    private void ApplyMetricToVariables(SparkplugPayload.Metric metric,
        string nodeKey, string? groupId = null, string? nodeId = null, string? deviceId = null)
    {
        if (string.IsNullOrEmpty(metric.Name)) return;

        List<SparkplugBItem> matching;
        lock (_lock)
        {
            matching = _items.Where(i =>
            {
                if (i.Config.MetricName != metric.Name) return false;
                var key = $"{i.Config.GroupId}/{i.Config.NodeId}/{i.Config.DeviceId}";
                return key == nodeKey;
            }).ToList();
        }

        if (matching.Count == 0) return;

        foreach (var item in matching)
        {
            try
            {
                var value = ExtractValue(metric, item.Variable.DataType);
                if (value != null || metric.IsNull)
                {
                    item.Variable.Value = metric.IsNull ? null : value;
                    item.Variable.StatusCode = StatusCodes.Good;
                    item.Variable.Timestamp = metric.Timestamp > 0
                        ? DateTimeOffset.FromUnixTimeMilliseconds((long)metric.Timestamp).UtcDateTime
                        : DateTime.UtcNow;
                    item.Variable.ClearChangeMasks(_context, false);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("[SparkplugB] Value conversion error for {Metric}: {Error}", metric.Name, ex.Message);
                item.Variable.StatusCode = StatusCodes.BadTypeMismatch;
                item.Variable.Timestamp = DateTime.UtcNow;
                item.Variable.ClearChangeMasks(_context, false);
            }
        }
    }

    // ─── Helpers ─────────────────────────────────────────────────

    private static object? ExtractValue(SparkplugPayload.Metric metric, NodeId dataType)
    {
        // Try to extract the right value based on the metric's populated field
        if (metric.BooleanValue.HasValue)
        {
            if (dataType == DataTypeIds.Boolean) return metric.BooleanValue.Value;
            return metric.BooleanValue.Value ? 1.0 : 0.0;
        }
        if (metric.DoubleValue.HasValue)
        {
            if (dataType == DataTypeIds.Double) return metric.DoubleValue.Value;
            if (dataType == DataTypeIds.Float) return (float)metric.DoubleValue.Value;
            if (dataType == DataTypeIds.Int32) return (int)metric.DoubleValue.Value;
            if (dataType == DataTypeIds.Int64) return (long)metric.DoubleValue.Value;
            return metric.DoubleValue.Value;
        }
        if (metric.FloatValue.HasValue)
        {
            if (dataType == DataTypeIds.Float) return metric.FloatValue.Value;
            if (dataType == DataTypeIds.Double) return (double)metric.FloatValue.Value;
            if (dataType == DataTypeIds.Int32) return (int)metric.FloatValue.Value;
            return (double)metric.FloatValue.Value;
        }
        if (metric.LongValue.HasValue)
        {
            if (dataType == DataTypeIds.Int64) return (long)metric.LongValue.Value;
            if (dataType == DataTypeIds.UInt64) return metric.LongValue.Value;
            if (dataType == DataTypeIds.Int32) return (int)metric.LongValue.Value;
            if (dataType == DataTypeIds.Double) return (double)metric.LongValue.Value;
            return (long)metric.LongValue.Value;
        }
        if (metric.IntValue.HasValue)
        {
            if (dataType == DataTypeIds.Int32) return (int)metric.IntValue.Value;
            if (dataType == DataTypeIds.UInt32) return metric.IntValue.Value;
            if (dataType == DataTypeIds.UInt16) return (ushort)metric.IntValue.Value;
            if (dataType == DataTypeIds.Int16) return (short)metric.IntValue.Value;
            if (dataType == DataTypeIds.Double) return (double)metric.IntValue.Value;
            return (int)metric.IntValue.Value;
        }
        if (metric.StringValue != null) return metric.StringValue;

        return null;
    }

    private static SparkplugPayload? DeserializePayload(byte[]? data)
    {
        if (data == null || data.Length == 0) return null;
        try
        {
            using var ms = new MemoryStream(data);
            return Serializer.Deserialize<SparkplugPayload>(ms);
        }
        catch (Exception ex)
        {
            Log.Debug("[SparkplugB] Protobuf deserialization error: {Error}", ex.Message);
            return null;
        }
    }

    private void SetAllBrokerItemsBad(string brokerKey)
    {
        List<SparkplugBItem> items;
        lock (_lock) { items = _items.Where(i => $"{i.Config.Broker}:{i.Config.Port}" == brokerKey).ToList(); }
        foreach (var item in items)
        {
            item.Variable.StatusCode = StatusCodes.BadNotConnected;
            item.Variable.Timestamp = DateTime.UtcNow;
            item.Variable.ClearChangeMasks(_context, false);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var c in _clients.Values)
        {
            try { c.Dispose(); } catch { }
        }
    }

    private class SparkplugBItem
    {
        public BaseDataVariableState Variable { get; set; } = null!;
        public SparkplugBConfig Config { get; set; } = null!;
    }
}
