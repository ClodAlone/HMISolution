using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using Opc.Ua;
using NModbus;
using Serilog;

namespace SimpleOpcFileServer
{
    public class ModbusConfig
    {
        public string IpAddress { get; set; } = "";
        public int Port { get; set; }
        public byte UnitId { get; set; }
        public ushort Register { get; set; }
        public string? RegisterType { get; set; }
        public int PollTime { get; set; } = 1000;
    }

    public class ModbusDriver : IDriver, IDisposable
    {
        public string Key => "Modbus";
        public event Action<string, string>? OnError;
        public event Action<double>? OnCycleCompleted;

        private readonly Dictionary<string, ModbusDevice> _devices = new();
        private readonly object _lock = new();
        private bool _disposed;
        private readonly ISystemContext _context;

        public ModbusDriver(ISystemContext context)
        {
            _context = context;
        }

        private void RaiseError(string source, string message) => OnError?.Invoke(source, message);
        private void RaiseCycle(double ms) => OnCycleCompleted?.Invoke(ms);

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var modbusConfig = JsonSerializer.Deserialize<ModbusConfig>(configJson);
            if (modbusConfig == null) return;

            lock (_lock)
            {
                var key = $"{modbusConfig.IpAddress}:{modbusConfig.Port}";
                if (!_devices.TryGetValue(key, out var device))
                {
                    device = new ModbusDevice(key, _context, RaiseError, RaiseCycle);
                    _devices[key] = device;
                }
                device.AddItem(new ModbusItem { Variable = variable, Config = modbusConfig });
            }
        }

        public void Dispose()
        {
            _disposed = true;
            foreach (var d in _devices.Values) d.Dispose();
            _devices.Clear();
        }

        private class ModbusDevice : IDisposable
        {
            private readonly string _key;
            private readonly ISystemContext _context;
            private readonly Action<string, string>? _onError;
            private readonly Action<double>? _onCycle;
            private readonly List<ModbusItem> _items = new();
            private Timer? _timer;
            private TcpClient? _client;
            private IModbusMaster? _master;
            private int _pollInterval = 1000;
            private bool _disposed;
            private readonly object _deviceLock = new();

            public ModbusDevice(string key, ISystemContext context, Action<string, string>? onError = null, Action<double>? onCycle = null)
            {
                _key = key;
                _context = context;
                _onError = onError;
                _onCycle = onCycle;
            }

            public void AddItem(ModbusItem item)
            {
                lock (_deviceLock)
                {
                    _items.Add(item);
                    if (_items.Count == 1)
                        _pollInterval = item.Config.PollTime > 0 ? item.Config.PollTime : 1000;
                    else
                    {
                        var configured = _items.Where(x => x.Config.PollTime > 0).ToList();
                        if (configured.Any()) _pollInterval = configured.Min(x => x.Config.PollTime);
                    }
                    UpdateTimer();
                }
            }

            private void UpdateTimer()
            {
                if (_timer != null)
                    _timer.Change(0, Timeout.Infinite);
                else
                    _timer = new Timer(Poll, null, 0, Timeout.Infinite);
            }

            private void Poll(object? state)
            {
                if (_disposed) return;
                var _sw = System.Diagnostics.Stopwatch.StartNew();

                List<ModbusItem> itemsToPoll;
                TcpClient? client;
                IModbusMaster? master;

                lock (_deviceLock)
                {
                    if (_disposed) return;
                    itemsToPoll = new List<ModbusItem>(_items);
                    client = _client;
                    master = _master;
                }

                try
                {
                    if (client == null || !client.Connected)
                    {
                        var split = _key.Split(':');
                        var newClient = new TcpClient();
                        try
                        {
                            newClient.Connect(split[0], int.Parse(split[1]));
                            var factory = new ModbusFactory();
                            var newMaster = factory.CreateMaster(newClient);
                            lock (_deviceLock)
                            {
                                if (_disposed) { newClient.Close(); return; }
                                DisposeClient();
                                _client = newClient;
                                _master = newMaster;
                                client = _client;
                                master = _master;
                            }
                        }
                        catch (Exception ex)
                        {
                            newClient.Close();
                            Log.Error(ex, "Modbus connection error for {Key}: {Message}", _key, ex.Message);
                            _onError?.Invoke(_key, $"Connection error: {ex.Message}");
                            foreach (var item in itemsToPoll) UpdateError(item.Variable, ex.Message);
                            return;
                        }
                    }

                    if (client != null && client.Connected && master != null)
                    {
                        bool connectionFailed = false;
                        foreach (var item in itemsToPoll)
                        {
                            try { ReadItem(master, item); }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Modbus read error for {Name}: {Message}", item.Variable.DisplayName, ex.Message);
                                _onError?.Invoke(_key, $"Read error ({item.Variable.DisplayName}): {ex.Message}");
                                UpdateError(item.Variable, ex.Message);
                                connectionFailed = true;
                            }
                        }
                        if (connectionFailed)
                        {
                            lock (_deviceLock) { if (_client == client) DisposeClient(); }
                        }
                    }
                    else
                    {
                        foreach (var item in itemsToPoll) UpdateError(item.Variable, "Not connected");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Modbus poll error for {Key}: {Message}", _key, ex.Message);
                    _onError?.Invoke(_key, $"Poll error: {ex.Message}");
                    foreach (var item in itemsToPoll) UpdateError(item.Variable, ex.Message);
                }
                finally
                {
                    _onCycle?.Invoke(_sw.Elapsed.TotalMilliseconds);
                    if (!_disposed) _timer?.Change(_pollInterval, Timeout.Infinite);
                }
            }

            private void ReadItem(IModbusMaster master, ModbusItem item)
            {
                ushort start = item.Config.Register;
                byte unitId = item.Config.UnitId;

                if (item.Config.RegisterType == "Coil")
                {
                    bool[] res = master.ReadCoils(unitId, start, 1);
                    if (res.Length > 0) Update(item.Variable, res[0]);
                }
                else if (item.Config.RegisterType == "DiscreteInput")
                {
                    bool[] res = master.ReadInputs(unitId, start, 1);
                    if (res.Length > 0) Update(item.Variable, res[0]);
                }
                else
                {
                    ushort count = 1;
                    if (item.Variable.DataType == DataTypeIds.Double) count = 4;
                    else if (item.Variable.DataType == DataTypeIds.Float) count = 2;
                    else if (item.Variable.DataType == DataTypeIds.Int32 || item.Variable.DataType == DataTypeIds.UInt32) count = 2;

                    ushort[] res = item.Config.RegisterType == "InputRegister"
                        ? master.ReadInputRegisters(unitId, start, count)
                        : master.ReadHoldingRegisters(unitId, start, count);

                    if (res.Length == count)
                    {
                        var bytes = new List<byte>();
                        foreach (var r in res) { bytes.Add((byte)(r >> 8)); bytes.Add((byte)(r & 0xFF)); }
                        var buffer = bytes.ToArray();
                        if (BitConverter.IsLittleEndian) Array.Reverse(buffer);

                        object? val = null;
                        if (item.Variable.DataType == DataTypeIds.Double) val = BitConverter.ToDouble(buffer, 0);
                        else if (item.Variable.DataType == DataTypeIds.Float) val = BitConverter.ToSingle(buffer, 0);
                        else if (item.Variable.DataType == DataTypeIds.Int32) val = BitConverter.ToInt32(buffer, 0);
                        else if (item.Variable.DataType == DataTypeIds.UInt32) val = BitConverter.ToUInt32(buffer, 0);
                        else if (item.Variable.DataType == DataTypeIds.Int16) val = BitConverter.ToInt16(buffer, 0);
                        else if (item.Variable.DataType == DataTypeIds.UInt16) val = BitConverter.ToUInt16(buffer, 0);

                        if (val != null) Update(item.Variable, val);
                    }
                }
            }

            private void Update(BaseDataVariableState variable, object value)
            {
                variable.Value = value;
                variable.StatusCode = StatusCodes.Good;
                variable.Timestamp = DateTime.UtcNow;
                variable.ClearChangeMasks(_context, false);
            }

            private void UpdateError(BaseDataVariableState variable, string? message = null)
            {
                variable.StatusCode = StatusCodes.Bad;
                variable.Timestamp = DateTime.UtcNow;
                variable.ClearChangeMasks(_context, false);
                if (!string.IsNullOrEmpty(message))
                {
                    var lastErrorNode = variable.FindChild(_context, new QualifiedName("LastError", variable.BrowseName.NamespaceIndex));
                    if (lastErrorNode is BaseVariableState lastErrorVar)
                    {
                        lastErrorVar.Value = message;
                        lastErrorVar.Timestamp = DateTime.UtcNow;
                        lastErrorVar.ClearChangeMasks(_context, false);
                    }
                }
            }

            private void DisposeClient()
            {
                try { _client?.Close(); } catch { }
                _client = null;
                _master = null;
            }

            public void Dispose()
            {
                _disposed = true;
                _timer?.Dispose();
                DisposeClient();
            }
        }

        private class ModbusItem
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public ModbusConfig Config { get; set; } = null!;
        }
    }
}
