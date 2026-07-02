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
        private readonly MqttClientFactory _factory = new();
        private readonly object _lock = new();
        private bool _disposed;
        private readonly ISystemContext _context;

        public MqttDriver(ISystemContext context) { _context = context; }

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var mqttConfig = JsonSerializer.Deserialize<MqttConfig>(configJson);
            if (mqttConfig == null) return;

            lock (_lock) { _items.Add(new MqttItem { Variable = variable, Config = mqttConfig }); }
            Task.Run(() => InitializeClient(mqttConfig));
        }

        private async Task InitializeClient(MqttConfig config)
        {
            var key = $"{config.Broker}:{config.Port}";
            IMqttClient client;
            bool newClient = false;

            lock (_lock)
            {
                if (!_clients.ContainsKey(key))
                {
                    client = _factory.CreateMqttClient();
                    _clients[key] = client;
                    newClient = true;
                }
                else client = _clients[key];
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
                }
            }

            if (client.IsConnected)
            {
                var subscribeOptions = new MqttClientSubscribeOptionsBuilder().WithTopicFilter(config.Topic).Build();
                await client.SubscribeAsync(subscribeOptions);
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
                    item.Variable.Value = value; item.Variable.StatusCode = StatusCodes.Good;
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
                if (n is BaseVariableState v) { v.Value = message; v.Timestamp = DateTime.UtcNow; v.ClearChangeMasks(_context, false); }
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
