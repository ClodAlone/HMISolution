// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

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
                var item = new ModbusItem { Variable = variable, Config = modbusConfig };
                device.AddItem(item);

                // Attach write handler to allow OPC UA client writes to be sent to Modbus device.
                variable.OnSimpleWriteValue = (ISystemContext ctx, NodeState node, ref object value) =>
                {
                    try
                    {
                        // Delegate to the device write method.
                        var res = device.Write(item, value);
                        return res;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Modbus write handler error for {Key}: {Message}", key, ex.Message);
                        return ServiceResult.Create(ex, StatusCodes.BadUnexpectedError, ex.Message);
                    }
                };
            }
        }

        public void Start()
        {
            lock (_lock) { foreach (var d in _devices.Values) d.Start(); }
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
            private int _minPollTime = int.MaxValue;
            private bool _disposed;
            private readonly object _deviceLock = new();
            private string? _lastLoggedError;
            private int _consecutiveErrors;
            private const int MaxBackoffMs = 30_000;

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
                    if (item.Config.PollTime > 0 && item.Config.PollTime < _minPollTime)
                        _minPollTime = item.Config.PollTime;
                    _pollInterval = _minPollTime < int.MaxValue ? _minPollTime : 1000;
                }
            }

            public void Start()
            {
                lock (_deviceLock)
                {
                    if (_timer == null && _items.Count > 0)
                        _timer = new Timer(Poll, null, _pollInterval, Timeout.Infinite);
                }
            }


            private void Poll(object? state)
            {
                if (_disposed) return;
                var _sw = System.Diagnostics.Stopwatch.StartNew();
                bool anyError = false;

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
                            if (ex.Message != _lastLoggedError)
                            {
                                Log.Error(ex, "Modbus connection error for {Key}: {Message}", _key, ex.Message);
                                _lastLoggedError = ex.Message;
                            }
                            else Log.Debug("Modbus connection error (repeated) for {Key}: {Message}", _key, ex.Message);
                            _onError?.Invoke(_key, $"Connection error: {ex.Message}");
                            foreach (var item in itemsToPoll) UpdateError(item.Variable, ex.Message);
                            anyError = true;
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
                                if (ex.Message != _lastLoggedError)
                                {
                                    Log.Error(ex, "Modbus read error for {Name}: {Message}", item.Variable.DisplayName, ex.Message);
                                    _lastLoggedError = ex.Message;
                                }
                                else Log.Debug("Modbus read error (repeated) for {Name}: {Message}", item.Variable.DisplayName, ex.Message);
                                _onError?.Invoke(_key, $"Read error ({item.Variable.DisplayName}): {ex.Message}");
                                UpdateError(item.Variable, ex.Message);
                                connectionFailed = true;
                                anyError = true;
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
                    if (ex.Message != _lastLoggedError)
                    {
                        Log.Error(ex, "Modbus poll error for {Key}: {Message}", _key, ex.Message);
                        _lastLoggedError = ex.Message;
                    }
                    else Log.Debug("Modbus poll error (repeated) for {Key}: {Message}", _key, ex.Message);
                    _onError?.Invoke(_key, $"Poll error: {ex.Message}");
                    foreach (var item in itemsToPoll) UpdateError(item.Variable, ex.Message);
                    anyError = true;
                }
                finally
                {
                    if (anyError) _consecutiveErrors++; else { _consecutiveErrors = 0; _lastLoggedError = null; }
                    var delay = _consecutiveErrors > 0 ? Math.Min(_pollInterval * (1 << Math.Min(_consecutiveErrors, 10)), MaxBackoffMs) : _pollInterval;
                    _onCycle?.Invoke(_sw.Elapsed.TotalMilliseconds);
                    if (!_disposed) _timer?.Change(delay, Timeout.Infinite);
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

            /// <summary>
            /// Write a value for the given item to the remote Modbus device.
            /// Returns a ServiceResult indicating success or failure.
            /// </summary>
            public ServiceResult Write(ModbusItem item, object value)
            {
                if (_disposed) return ServiceResult.Create(StatusCodes.BadNotConnected, "Device disposed");

                ushort start = item.Config.Register;
                byte unitId = item.Config.UnitId;

                lock (_deviceLock)
                {
                    // Ensure connection; attempt to connect if missing (same as Poll)
                    if ((_client == null || !_client.Connected) && !_disposed)
                    {
                        var split = _key.Split(':');
                        var newClient = new TcpClient();
                        try
                        {
                            newClient.Connect(split[0], int.Parse(split[1]));
                            var factory = new ModbusFactory();
                            var newMaster = factory.CreateMaster(newClient);
                            if (_disposed) { newClient.Close(); return ServiceResult.Create(StatusCodes.BadNotConnected, "Not connected"); }
                            DisposeClient();
                            _client = newClient;
                            _master = newMaster;
                        }
                        catch (Exception ex)
                        {
                            try { newClient.Close(); } catch { }
                            Log.Error(ex, "Modbus connection error for write {Key}: {Message}", _key, ex.Message);
                            _onError?.Invoke(_key, $"Connection error: {ex.Message}");
                            UpdateError(item.Variable, ex.Message);
                            return ServiceResult.Create(ex, StatusCodes.BadNotConnected, ex.Message);
                        }
                    }
                }

                // At this point try to write using current master
                IModbusMaster? master;
                lock (_deviceLock) { master = _master; }
                if (master == null) return ServiceResult.Create(StatusCodes.BadNotConnected, "Not connected to Modbus master");

                try
                {
                    // Disallow writes to InputRegister / DiscreteInput (read-only)
                    if (string.Equals(item.Config.RegisterType, "DiscreteInput", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(item.Config.RegisterType, "InputRegister", StringComparison.OrdinalIgnoreCase))
                    {
                        return ServiceResult.Create(StatusCodes.BadNotWritable, "Target register is read-only");
                    }

                    if (string.Equals(item.Config.RegisterType, "Coil", StringComparison.OrdinalIgnoreCase))
                    {
                        bool b = Convert.ToBoolean(value);
                        master.WriteSingleCoil(unitId, start, b);
                    }
                    else
                    {
                        // Holding registers (or unspecified) — write as registers based on variable datatype
                        ushort count = 1;
                        byte[] buffer;

                        if (item.Variable.DataType == DataTypeIds.Double)
                        {
                            count = 4;
                            var d = Convert.ToDouble(value);
                            buffer = BitConverter.GetBytes(d);
                        }
                        else if (item.Variable.DataType == DataTypeIds.Float)
                        {
                            count = 2;
                            var f = Convert.ToSingle(value);
                            buffer = BitConverter.GetBytes(f);
                        }
                        else if (item.Variable.DataType == DataTypeIds.Int32 || item.Variable.DataType == DataTypeIds.UInt32)
                        {
                            count = 2;
                            var i = Convert.ToInt32(value);
                            buffer = BitConverter.GetBytes(i);
                        }
                        else if (item.Variable.DataType == DataTypeIds.Int16 || item.Variable.DataType == DataTypeIds.UInt16)
                        {
                            count = 1;
                            var s = Convert.ToInt16(value);
                            buffer = BitConverter.GetBytes(s);
                        }
                        else // default to 1 register with string/other behavior: attempt convert to ushort or 0
                        {
                            count = 1;
                            ushort v = 0;
                            try { v = Convert.ToUInt16(value); } catch { v = 0; }
                            buffer = BitConverter.GetBytes(v);
                        }

                        // Modbus expects big-endian register byte order; read path reversed on little-endian, so mirror that here.
                        if (BitConverter.IsLittleEndian) Array.Reverse(buffer);

                        // Create ushort array (two bytes per register)
                        var ushorts = new ushort[count];
                        for (int i = 0; i < count; i++)
                        {
                            int idx = i * 2;
                            ushort u = (ushort)((buffer[idx] << 8) | buffer[idx + 1]);
                            ushorts[i] = u;
                        }

                        if (count == 1)
                            master.WriteSingleRegister(unitId, start, ushorts[0]);
                        else
                            master.WriteMultipleRegisters(unitId, start, ushorts);
                    }

                    // On success, update variable in address space to reflect new value
                    item.Variable.Value = value; item.Variable.StatusCode = StatusCodes.Good;
                    item.Variable.Timestamp = DateTime.UtcNow; item.Variable.ClearChangeMasks(_context, false);

                    return ServiceResult.Good;
                }
                catch (Exception ex)
                {
                    if (ex.Message != _lastLoggedError)
                    {
                        Log.Error(ex, "Modbus write error for {Key}: {Message}", _key, ex.Message);
                        _lastLoggedError = ex.Message;
                    }
                    else Log.Debug("Modbus write error (repeated) for {Key}: {Message}", _key, ex.Message);
                    _onError?.Invoke(_key, $"Write error ({item.Variable.DisplayName}): {ex.Message}");
                    UpdateError(item.Variable, ex.Message);
                    return ServiceResult.Create(ex, StatusCodes.BadUnexpectedError, ex.Message);
                }
            }
        }

        private class ModbusItem
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public ModbusConfig Config { get; set; } = null!;
        }
    }
}
