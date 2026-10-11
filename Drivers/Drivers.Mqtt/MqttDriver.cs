// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using Opc.Ua;
using Serilog;

namespace SimpleOpcFileServer
{
    public class MqttConfig
    {
        public string Broker { get; set; } = "";
        public int Port { get; set; }
        public string Topic { get; set; } = "";
        public string? JsonPath { get; set; }
    }

    public class MqttDriver : IDriver, IDisposable
    {
        public string Key => "Mqtt";
        public event Action<string, string>? OnError;
        public event Action<double>? OnCycleCompleted;

        private readonly List<MqttItem> _items = new();
        private readonly Dictionary<string, IMqttClient> _clients = new();
        private readonly Dictionary<string, MqttConfig> _brokerConfigs = new();
        private readonly MqttClientFactory _factory = new();
        private readonly object _lock = new();
        private bool _disposed;
        private bool _started;
        private readonly ISystemContext _context;

        public MqttDriver(ISystemContext context) { _context = context; }

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var mqttConfig = JsonSerializer.Deserialize<MqttConfig>(configJson);
            if (mqttConfig == null) return;

            var key = $"{mqttConfig.Broker}:{mqttConfig.Port}";
            lock (_lock)
            {
                _items.Add(new MqttItem { Variable = variable, Config = mqttConfig });
                // Remember one representative config per broker; actual connection + batched
                // subscription of all topics for this broker happens once in Start(), rather
                // than firing a separate connect/subscribe task for every single item added.
                _brokerConfigs[key] = mqttConfig;
                if (_started) Task.Run(() => InitializeClient(key));
            }

            // Publish to MQTT when an OPC UA client writes this variable.
            variable.OnSimpleWriteValue = (ISystemContext ctx, NodeState node, ref Variant valueVariant) =>
            {
                object? value = valueVariant.Value;
                try
                {
                    var key = $"{mqttConfig.Broker}:{mqttConfig.Port}";
                    IMqttClient client;
                    lock (_lock)
                    {
                        if (!_clients.TryGetValue(key, out client) || client == null || !client.IsConnected)
                        {
                            // Try to (re)initialize the client in background and report not connected now.
                            Task.Run(() => InitializeClient(key));
                            return ServiceResult.Create(StatusCodes.BadNotConnected, "MQTT client not connected");
                        }
                    }

                    string payload;
                    if (!string.IsNullOrEmpty(mqttConfig.JsonPath))
                    {
                        // Build a nested JSON object for the JsonPath (e.g. "a.b.c")
                        try
                        {
                            var parts = mqttConfig.JsonPath.Split('.');
                            var root = new Dictionary<string, object?>();
                            IDictionary<string, object?> current = root;
                            for (int i = 0; i < parts.Length; i++)
                            {
                                var part = parts[i];
                                if (i == parts.Length - 1)
                                {
                                    current[part] = value;
                                }
                                else
                                {
                                    var next = new Dictionary<string, object?>();
                                    current[part] = next;
                                    current = next;
                                }
                            }
                            payload = JsonSerializer.Serialize(root);
                        }
                        catch { payload = value?.ToString() ?? string.Empty; }
                    }
                    else
                    {
                        payload = value switch
                        {
                            double d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
                            float f => f.ToString(System.Globalization.CultureInfo.InvariantCulture),
                            int i => i.ToString(),
                            long l => l.ToString(),
                            bool b => b ? "true" : "false",
                            _ => value?.ToString() ?? string.Empty
                        };
                    }

                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(mqttConfig.Topic)
                        .WithPayload(payload)
                        .Build();

                    try
                    {
                        client.PublishAsync(message, CancellationToken.None).GetAwaiter().GetResult();
                        return ServiceResult.Good;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "MQTT publish error for {Key}: {Message}", key, ex.Message);
                        OnError?.Invoke(key, $"Publish error: {ex.Message}");
                        return ServiceResult.Create(ex, StatusCodes.BadUnexpectedError, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "OnWrite publish error: {Message}", ex.Message);
                    return ServiceResult.Create(ex, StatusCodes.BadUnexpectedError, ex.Message);
                }
            };
        }

        /// <summary>
        /// Connects to every distinct broker referenced by added items, subscribing to all of
        /// that broker's distinct topics in a single batched MQTT SUBSCRIBE request per broker
        /// instead of one connection + subscribe round-trip per item.
        /// </summary>
        public void Start()
        {
            List<string> keys;
            lock (_lock)
            {
                _started = true;
                keys = _brokerConfigs.Keys.ToList();
            }
            foreach (var key in keys) Task.Run(() => InitializeClient(key));
        }

        private async Task InitializeClient(string key)
        {
            MqttConfig? config;
            IMqttClient client;
            bool newClient = false;

            lock (_lock)
            {
                if (!_brokerConfigs.TryGetValue(key, out config) || config == null) return;
                if (!_clients.TryGetValue(key, out client!))
                {
                    client = _factory.CreateMqttClient();
                    _clients[key] = client;
                    newClient = true;
                }
            }

            if (newClient)
            {
                var options = new MqttClientOptionsBuilder().WithTcpServer(config.Broker, config.Port).Build();
                client.ApplicationMessageReceivedAsync += e => { OnMessageReceived(e); return Task.CompletedTask; };
                client.DisconnectedAsync += e =>
                {
                    lock (_lock)
                    {
                        foreach (var item in _items)
                            if ($"{item.Config.Broker}:{item.Config.Port}" == key) UpdateError(item.Variable);
                    }
                    return Task.CompletedTask;
                };

                try { await client.ConnectAsync(options); }
                catch (Exception ex)
                {
                    Log.Error(ex, "MQTT connection error for {Key}: {Message}", key, ex.Message);
                    OnError?.Invoke(key, $"Connection error: {ex.Message}");
                    lock (_lock)
                    {
                        foreach (var item in _items)
                            if ($"{item.Config.Broker}:{item.Config.Port}" == key) UpdateError(item.Variable, ex.Message);
                    }
                    return;
                }
            }

            if (client.IsConnected)
            {
                // Batch every distinct topic for this broker into one SubscribeAsync call
                // (MQTT's SUBSCRIBE packet natively supports multiple topic filters in one
                // request), instead of issuing a separate subscribe round-trip per item/topic.
                List<string> topics;
                lock (_lock)
                {
                    topics = _items
                        .Where(i => $"{i.Config.Broker}:{i.Config.Port}" == key)
                        .Select(i => i.Config.Topic)
                        .Where(t => !string.IsNullOrEmpty(t))
                        .Distinct()
                        .ToList();
                }
                if (topics.Count > 0)
                {
                    var subscribeBuilder = new MqttClientSubscribeOptionsBuilder();
                    foreach (var topic in topics) subscribeBuilder.WithTopicFilter(topic);
                    try { await client.SubscribeAsync(subscribeBuilder.Build()); }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "MQTT subscribe error for {Key}: {Message}", key, ex.Message);
                        OnError?.Invoke(key, $"Subscribe error: {ex.Message}");
                    }
                }
            }
        }

        private Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            string topic = e.ApplicationMessage.Topic;
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            List<MqttItem> matching;
            lock (_lock) { matching = _items.Where(i => i.Config.Topic == topic).ToList(); }
            foreach (var item in matching) UpdateItem(item, payload);
            OnCycleCompleted?.Invoke(sw.Elapsed.TotalMilliseconds);
            return Task.CompletedTask;
        }

        private void UpdateItem(MqttItem item, string payload)
        {
            try
            {
                object? value = null;
                if (!string.IsNullOrEmpty(item.Config.JsonPath))
                {
                    try
                    {
                        var doc = JsonDocument.Parse(payload);
                        var elem = doc.RootElement;
                        foreach (var p in item.Config.JsonPath.Split('.'))
                        {
                            if (!elem.TryGetProperty(p, out var next)) { UpdateError(item.Variable); return; }
                            elem = next;
                        }
                        if (item.Variable.DataType == DataTypeIds.Double) value = elem.GetDouble();
                        else if (item.Variable.DataType == DataTypeIds.Int32) value = elem.GetInt32();
                        else if (item.Variable.DataType == DataTypeIds.Boolean) value = elem.GetBoolean();
                        else value = elem.ToString();
                    }
                    catch { UpdateError(item.Variable); return; }
                }
                else
                {
                    if (item.Variable.DataType == DataTypeIds.Double) value = double.Parse(payload);
                    else if (item.Variable.DataType == DataTypeIds.Int32) value = int.Parse(payload);
                    else if (item.Variable.DataType == DataTypeIds.Boolean) value = bool.Parse(payload);
                    else value = payload;
                }

                if (value != null)
                {
                    item.Variable.Value = new Variant(value); item.Variable.StatusCode = StatusCodes.Good;
                    item.Variable.Timestamp = DateTime.UtcNow; item.Variable.ClearChangeMasks(_context, false);
                }
            }
            catch { UpdateError(item.Variable); }
        }

        private void UpdateError(BaseDataVariableState variable, string? message = null)
        {
            variable.StatusCode = StatusCodes.Bad; variable.Timestamp = DateTime.UtcNow;
            variable.ClearChangeMasks(_context, false);
            if (!string.IsNullOrEmpty(message))
            {
                var n = variable.FindChild(_context, new QualifiedName("LastError", variable.BrowseName.NamespaceIndex));
                if (n is BaseVariableState v) { v.Value = new Variant(message); v.Timestamp = DateTime.UtcNow; v.ClearChangeMasks(_context, false); }
            }
        }

        public void Dispose()
        {
            _disposed = true;
            foreach (var c in _clients.Values) c.Dispose();
        }

        private class MqttItem
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public MqttConfig Config { get; set; } = null!;
        }
    }
}
