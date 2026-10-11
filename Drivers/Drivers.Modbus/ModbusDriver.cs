// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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

        /// <summary>
        /// Maximum gap (in registers/coils) allowed between two items for them to be merged
        /// into a single block read. 0 means only strictly contiguous items are merged.
        /// Can be set per-item; the device uses the maximum value configured across its items.
        /// </summary>
        public int AggregationGapThreshold { get; set; } = 10;
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
                variable.OnSimpleWriteValue = (ISystemContext ctx, NodeState node, ref Variant value) =>
                {
                    try
                    {
                        // Delegate to the device write method.
                        var res = device.Write(item, value.Value);
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

            // Aggregation tuning: how many "empty" registers/coils between two items are tolerated
            // before they are read as separate blocks, and the protocol-imposed max block size.
            private int _gapThreshold = 10;
            private const int MaxRegisterBlockSize = 120;  // Modbus limit is 125 registers per read
            private const int MaxCoilBlockSize = 2000;     // Modbus limit is 2000 coils/bits per read

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
                    if (item.Config.AggregationGapThreshold > _gapThreshold)
                        _gapThreshold = item.Config.AggregationGapThreshold;
                }
            }

            public void Start()
            {
                lock (_deviceLock)
                {
                    // The timer callback kicks off an async poll and returns immediately;
                    // the Timer thread is never blocked waiting on network I/O.
                    if (_timer == null && _items.Count > 0)
                        _timer = new Timer(_ => _ = PollAsync(), null, _pollInterval, Timeout.Infinite);
                }
            }

            private async Task PollAsync()
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
                            await newClient.ConnectAsync(split[0], int.Parse(split[1])).ConfigureAwait(false);
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
                        foreach (var block in BuildReadBlocks(itemsToPoll))
                        {
                            try { await ReadItemsBlockAsync(master, block).ConfigureAwait(false); }
                            catch (Exception ex)
                            {
                                if (ex.Message != _lastLoggedError)
                                {
                                    Log.Error(ex, "Modbus read error for block {Key} (unit {UnitId}, {Type} {Start}-{End}): {Message}",
                                        _key, block.UnitId, block.RegisterType, block.Start, block.Start + block.Length - 1, ex.Message);
                                    _lastLoggedError = ex.Message;
                                }
                                else Log.Debug("Modbus read error (repeated) for block {Key}: {Message}", _key, ex.Message);
                                _onError?.Invoke(_key, $"Read error (block {block.Start}-{block.Start + block.Length - 1}): {ex.Message}");
                                foreach (var item in block.Items) UpdateError(item.Variable, ex.Message);
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

            // Cache of OPC UA DataType NodeId -> register length (in 16-bit words), so the
            // mapping does not need to be recomputed via chained equality checks on every poll.
            private static readonly Dictionary<NodeId, int> RegisterLengthByDataType = new()
            {
                [DataTypeIds.Double] = 4,
                [DataTypeIds.Float] = 2,
                [DataTypeIds.Int32] = 2,
                [DataTypeIds.UInt32] = 2,
            };

            /// <summary>
            /// Returns the number of registers (or coils/bits) that the given item occupies,
            /// starting at its configured address.
            /// </summary>
            private static int GetItemLength(ModbusItem item)
            {
                if (item.Config.RegisterType == "Coil" || item.Config.RegisterType == "DiscreteInput")
                    return 1;

                return RegisterLengthByDataType.TryGetValue(item.Variable.DataType, out var len) ? len : 1;
            }

            /// <summary>
            /// Groups items sharing the same unit id and register kind (Coil / DiscreteInput /
            /// InputRegister / HoldingRegister) into contiguous read blocks. Items whose address
            /// gap (relative to the end of the previous item) is within the configured
            /// aggregation gap threshold are merged into one block read, up to the protocol's
            /// maximum block size. This reduces the number of round-trips to the device when
            /// many variables map to nearby addresses.
            /// </summary>
            private List<ReadBlock> BuildReadBlocks(List<ModbusItem> items)
            {
                var blocks = new List<ReadBlock>();

                var groups = items
                    .GroupBy(i => (i.Config.UnitId, Type: NormalizeType(i.Config.RegisterType)))
                    .OrderBy(g => g.Key.UnitId).ThenBy(g => g.Key.Type);

                foreach (var group in groups)
                {
                    var maxBlockSize = (group.Key.Type == "Coil" || group.Key.Type == "DiscreteInput")
                        ? MaxCoilBlockSize : MaxRegisterBlockSize;

                    var sorted = group
                        .Select(i => (Item: i, Start: i.Config.Register, Len: GetItemLength(i)))
                        .OrderBy(x => x.Start)
                        .ToList();

                    ReadBlock? current = null;
                    foreach (var entry in sorted)
                    {
                        int entryEnd = entry.Start + entry.Len;
                        if (current != null)
                        {
                            int currentEnd = current.Start + current.Length;
                            int gap = entry.Start - currentEnd;
                            int candidateLength = entryEnd - current.Start;
                            if (gap >= 0 && gap <= _gapThreshold && candidateLength <= maxBlockSize)
                            {
                                current.Length = candidateLength;
                                current.Items.Add(entry.Item);
                                continue;
                            }
                        }

                        current = new ReadBlock
                        {
                            UnitId = group.Key.UnitId,
                            RegisterType = group.Key.Type,
                            Start = entry.Start,
                            Length = entry.Len,
                        };
                        current.Items.Add(entry.Item);
                        blocks.Add(current);
                    }
                }

                return blocks;
            }

            private static string NormalizeType(string? registerType) => registerType switch
            {
                "Coil" => "Coil",
                "DiscreteInput" => "DiscreteInput",
                "InputRegister" => "InputRegister",
                _ => "HoldingRegister",
            };

            private async Task ReadItemsBlockAsync(IModbusMaster master, ReadBlock block)
            {
                if (block.RegisterType == "Coil" || block.RegisterType == "DiscreteInput")
                {
                    bool[] res = block.RegisterType == "Coil"
                        ? await master.ReadCoilsAsync(block.UnitId, (ushort)block.Start, (ushort)block.Length).ConfigureAwait(false)
                        : await master.ReadInputsAsync(block.UnitId, (ushort)block.Start, (ushort)block.Length).ConfigureAwait(false);

                    foreach (var item in block.Items)
                    {
                        int idx = item.Config.Register - block.Start;
                        if (idx >= 0 && idx < res.Length) Update(item.Variable, res[idx]);
                    }
                    return;
                }

                ushort[] regs = block.RegisterType == "InputRegister"
                    ? await master.ReadInputRegistersAsync(block.UnitId, (ushort)block.Start, (ushort)block.Length).ConfigureAwait(false)
                    : await master.ReadHoldingRegistersAsync(block.UnitId, (ushort)block.Start, (ushort)block.Length).ConfigureAwait(false);

                if (regs.Length != block.Length) return;

                foreach (var item in block.Items)
                {
                    int itemLen = GetItemLength(item);
                    int offset = item.Config.Register - block.Start;
                    if (offset < 0 || offset + itemLen > regs.Length) continue;

                    var bytes = new List<byte>();
                    for (int i = 0; i < itemLen; i++)
                    {
                        var r = regs[offset + i];
                        bytes.Add((byte)(r >> 8));
                        bytes.Add((byte)(r & 0xFF));
                    }
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

            private class ReadBlock
            {
                public byte UnitId { get; set; }
                public string RegisterType { get; set; } = "HoldingRegister";
                public int Start { get; set; }
                public int Length { get; set; }
                public List<ModbusItem> Items { get; } = new();
            }


            private void Update(BaseDataVariableState variable, object value)
            {
                variable.Value = new Variant(value);
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
                        lastErrorVar.Value = new Variant(message);
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
                    item.Variable.Value = new Variant(value); item.Variable.StatusCode = StatusCodes.Good;
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
