// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Serilog;

namespace SimpleOpcFileServer
{
    public class KnxConfig
    {
        public string IpAddress { get; set; } = "";
        public int Port { get; set; }
        public string GroupAddress { get; set; } = "";
        public string DptType { get; set; } = "";
    }

    public class KnxDriver : IDriver, IDisposable
    {
        public string Key => "Knx";
        public event Action<string, string>? OnError;
        public event Action<double>? OnCycleCompleted;

        private readonly List<KnxItem> _items = new();
        private readonly Dictionary<string, KnxClient> _clients = new();
        private readonly object _lock = new();
        private bool _disposed;
        private readonly ISystemContext _context;

        public KnxDriver(ISystemContext context) { _context = context; }

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var knxConfig = JsonSerializer.Deserialize<KnxConfig>(configJson);
            if (knxConfig == null) return;

            lock (_lock)
            {
                var item = new KnxItem { Variable = variable, Config = knxConfig };
                _items.Add(item);
                variable.OnSimpleWriteValue = (ISystemContext ctx, NodeState node, ref object value) =>
                {
                    if (!_clients.TryGetValue($"{knxConfig.IpAddress}:{knxConfig.Port}", out var client))
                        return ServiceResult.Create(StatusCodes.BadNotConnected, "KNX client not initialized");

                    client.Write(knxConfig.GroupAddress, value);
                    return ServiceResult.Good;
                };
                InitializeClient(knxConfig);
            }
        }

        private void InitializeClient(KnxConfig config)
        {
            var key = $"{config.IpAddress}:{config.Port}";
            if (!_clients.ContainsKey(key))
            {
                try
                {
                    var client = new KnxClient(config.IpAddress, config.Port);
                    client.KnxEvent += OnKnxEvent;
                    client.ConnectAsync();
                    _clients[key] = client;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "KNX init error for {Key}: {Message}", key, ex.Message);
                    OnError?.Invoke(key, $"Init error: {ex.Message}");
                    foreach (var item in _items)
                        if (item.Config.IpAddress == config.IpAddress && item.Config.Port == config.Port)
                            UpdateError(item.Variable, ex.Message);
                }
            }
        }

        private void OnKnxEvent(object? sender, KnxEventArgs e)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            List<KnxItem> matching;
            lock (_lock) { matching = _items.Where(i => i.Config.GroupAddress == e.Address).ToList(); }
            foreach (var item in matching) Update(item.Variable, e.Value);
            OnCycleCompleted?.Invoke(sw.Elapsed.TotalMilliseconds);
        }

        private void Update(BaseDataVariableState variable, object value)
        {
            variable.Value = value; variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow; variable.ClearChangeMasks(_context, false);
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

        private class KnxItem
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public KnxConfig Config { get; set; } = null!;
        }

        private class KnxClient : IDisposable
        {
            public event EventHandler<KnxEventArgs>? KnxEvent;
            private readonly UdpClient _udpClient;
            private readonly IPEndPoint _remoteEndpoint;
            private readonly CancellationTokenSource _cts;
            private Task? _receiveTask;
            private byte _channelId;

            public KnxClient(string ip, int port)
            {
                _remoteEndpoint = new IPEndPoint(IPAddress.Parse(ip), port);
                _udpClient = new UdpClient();
                _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 0));
                _cts = new CancellationTokenSource();
            }

            public async Task ConnectAsync()
            {
                var localEp = (IPEndPoint)_udpClient.Client.LocalEndPoint!;
                byte[] ip = localEp.Address.GetAddressBytes();
                if (localEp.AddressFamily == AddressFamily.InterNetworkV6) ip = new byte[] { 0, 0, 0, 0 };
                byte[] port = BitConverter.GetBytes((ushort)localEp.Port);
                if (BitConverter.IsLittleEndian) Array.Reverse(port);

                var connectRequest = new List<byte>
                {
                    0x06, 0x10, 0x02, 0x05, 0x00, 0x1A,
                    0x08, 0x01, ip[0], ip[1], ip[2], ip[3], port[0], port[1],
                    0x08, 0x01, ip[0], ip[1], ip[2], ip[3], port[0], port[1],
                    0x04, 0x04, 0x02, 0x00
                };
                await _udpClient.SendAsync(connectRequest.ToArray(), connectRequest.Count, _remoteEndpoint);
                _receiveTask = ReceiveLoop(_cts.Token);
            }

            public void Write(string groupAddress, object value)
            {
                if (string.IsNullOrWhiteSpace(groupAddress)) return;

                var packet = BuildGroupWritePacket(groupAddress, value);
                if (packet == null || packet.Length == 0) return;

                _udpClient.Send(packet, packet.Length, _remoteEndpoint);
            }

            private byte[]? BuildGroupWritePacket(string groupAddress, object value)
            {
                var address = ParseGroupAddress(groupAddress);
                if (address == null) return null;

                var payload = EncodeKnxPayload(value);
                if (payload == null || payload.Length == 0) return null;

                var packet = new byte[2 + payload.Length];
                packet[0] = (byte)((address.Value >> 8) & 0xFF);
                packet[1] = (byte)(address.Value & 0xFF);
                Buffer.BlockCopy(payload, 0, packet, 2, payload.Length);
                return packet;
            }

            private ushort? ParseGroupAddress(string groupAddress)
            {
                var cleaned = groupAddress.Trim();
                var parts = cleaned.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length != 3) return null;

                if (!ushort.TryParse(parts[0], out var main)
                    || !byte.TryParse(parts[1], out var middle)
                    || !byte.TryParse(parts[2], out var sub))
                {
                    return null;
                }

                if (main > 31 || middle > 7 || sub > 255) return null;
                var addr = ((main & 0x1F) << 11) | ((middle & 0x07) << 8) | (sub & 0xFF);
                return (ushort)addr;
            }

            private byte[]? EncodeKnxPayload(object value)
            {
                if (value == null) return null;

                if (value is bool b)
                {
                    return new[] { (byte)(b ? 0x01 : 0x00) };
                }

                if (value is byte by)
                {
                    return new[] { by };
                }

                if (value is short s)
                {
                    return BitConverter.GetBytes((short)s);
                }

                if (value is ushort us)
                {
                    return BitConverter.GetBytes(us);
                }

                if (value is int i)
                {
                    return BitConverter.GetBytes(i);
                }

                if (value is uint ui)
                {
                    return BitConverter.GetBytes(ui);
                }

                if (value is float f)
                {
                    return BitConverter.GetBytes(f);
                }

                if (value is double d)
                {
                    return BitConverter.GetBytes(d);
                }

                if (value is decimal dec)
                {
                    return BitConverter.GetBytes((double)dec);
                }

                var text = value.ToString();
                if (string.IsNullOrEmpty(text)) return null;
                return Encoding.UTF8.GetBytes(text);
            }

            private async Task ReceiveLoop(CancellationToken token)
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        var result = await _udpClient.ReceiveAsync(token);
                        var data = result.Buffer;
                        if (data.Length < 6 || data[0] != 0x06 || data[1] != 0x10) continue;

                        ushort serviceType = (ushort)((data[2] << 8) | data[3]);

                        if (serviceType == 0x0206 && data.Length >= 8)
                            _channelId = data[6];
                        else if (serviceType == 0x0420)
                        {
                            byte seq = data[7];
                            var ack = new byte[] { 0x06, 0x10, 0x04, 0x21, 0x00, 0x0A, 0x04, _channelId, seq, 0x00 };
                            await _udpClient.SendAsync(ack, ack.Length, _remoteEndpoint);

                            if (data.Length > 10)
                            {
                                int cemiStart = 10;
                                if (data[cemiStart] == 0x29)
                                {
                                    int addInfoLen = data[cemiStart + 1];
                                    int frameStart = cemiStart + 2 + addInfoLen;
                                    if (data.Length >= frameStart + 9)
                                    {
                                        byte destHigh = data[frameStart + 4];
                                        byte destLow = data[frameStart + 5];
                                        string destAddr = ParseGroupAddress(destHigh, destLow);
                                        byte dataLen = data[frameStart + 6];
                                        byte tpciApciHigh = data[frameStart + 7];
                                        byte tpciApciLow = data[frameStart + 8];
                                        bool isWrite = (tpciApciHigh & 0x03) == 0x00 && (tpciApciLow & 0xC0) == 0x80;

                                        if (isWrite)
                                        {
                                            object val;
                                            if (dataLen == 1) val = (tpciApciLow & 0x3F);
                                            else
                                            {
                                                int payloadStart = frameStart + 9;
                                                if (data.Length > payloadStart)
                                                {
                                                    if (dataLen == 2) val = data[payloadStart];
                                                    else if (dataLen == 3 && data.Length >= payloadStart + 1) val = (data[payloadStart] << 8) | data[payloadStart + 1];
                                                    else val = 0;
                                                }
                                                else val = 0;
                                            }
                                            KnxEvent?.Invoke(this, new KnxEventArgs { Address = destAddr, Value = val });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception ex) { Log.Error(ex, "KNX receiver error: {Message}", ex.Message); }
            }

            private string ParseGroupAddress(byte high, byte low)
            {
                int address = (high << 8) | low;
                return $"{(address >> 11) & 0x1F}/{(address >> 8) & 0x07}/{address & 0xFF}";
            }

            public void Dispose() { _cts.Cancel(); _udpClient.Close(); _udpClient.Dispose(); }
        }

        private class KnxEventArgs : EventArgs
        {
            public string Address { get; set; } = "";
            public object Value { get; set; } = new object();
        }
    }
}
