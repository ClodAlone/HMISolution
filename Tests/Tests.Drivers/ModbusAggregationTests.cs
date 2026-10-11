// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Opc.Ua;
using SimpleOpcFileServer;
using Xunit;

namespace Tests.Drivers;

public class ModbusAggregationTests
{
    [Fact]
    public async Task Poll_WithNearbyRegisters_AggregatesIntoSingleReadRequest()
    {
        await using var server = new ModbusReadTestServer();
        await server.StartAsync();

        using var driver = new ModbusDriver(null!);

        // Two holding-register items 3 registers apart (gap=2 < default threshold of 10)
        // so they should be read with a single ReadHoldingRegisters call.
        var variable1 = DriverTestHelpers.CreateVariable("Reg10", DataTypeIds.UInt16);
        var config1 = JsonSerializer.Serialize(new ModbusConfig
        {
            IpAddress = "127.0.0.1",
            Port = server.Port,
            UnitId = 1,
            Register = 10,
            RegisterType = "HoldingRegister",
            PollTime = 200
        });
        driver.AddItem(variable1, config1);

        var variable2 = DriverTestHelpers.CreateVariable("Reg13", DataTypeIds.UInt16);
        var config2 = JsonSerializer.Serialize(new ModbusConfig
        {
            IpAddress = "127.0.0.1",
            Port = server.Port,
            UnitId = 1,
            Register = 13,
            RegisterType = "HoldingRegister",
            PollTime = 200
        });
        driver.AddItem(variable2, config2);

        var cycleSignal = new TaskCompletionSource();
        driver.OnCycleCompleted += _ => cycleSignal.TrySetResult();

        driver.Start();

        await cycleSignal.Task.WaitAsync(TimeSpan.FromSeconds(5));
        // Give the server a brief moment to record the request.
        await Task.Delay(100);

        Assert.Single(server.Requests);
        var request = server.Requests.First();
        Assert.Equal((ushort)10, request.Start);
        Assert.Equal((ushort)4, request.Count); // covers registers 10..13 inclusive
    }

    [Fact]
    public async Task Poll_WithNearbyCoils_AggregatesIntoSingleReadRequest()
    {
        await using var server = new ModbusReadTestServer();
        await server.StartAsync();

        using var driver = new ModbusDriver(null!);

        // Two coil items 3 bits apart (gap=2 < default threshold of 10)
        // so they should be read with a single ReadCoils call.
        var variable1 = DriverTestHelpers.CreateVariable("Coil10", DataTypeIds.Boolean);
        var config1 = JsonSerializer.Serialize(new ModbusConfig
        {
            IpAddress = "127.0.0.1",
            Port = server.Port,
            UnitId = 1,
            Register = 10,
            RegisterType = "Coil",
            PollTime = 200
        });
        driver.AddItem(variable1, config1);

        var variable2 = DriverTestHelpers.CreateVariable("Coil13", DataTypeIds.Boolean);
        var config2 = JsonSerializer.Serialize(new ModbusConfig
        {
            IpAddress = "127.0.0.1",
            Port = server.Port,
            UnitId = 1,
            Register = 13,
            RegisterType = "Coil",
            PollTime = 200
        });
        driver.AddItem(variable2, config2);

        var cycleSignal = new TaskCompletionSource();
        driver.OnCycleCompleted += _ => cycleSignal.TrySetResult();

        driver.Start();

        await cycleSignal.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await Task.Delay(100);

        Assert.Single(server.Requests);
        var request = server.Requests.First();
        Assert.Equal(0x01, request.FunctionCode);
        Assert.Equal((ushort)10, request.Start);
        Assert.Equal((ushort)4, request.Count); // covers coils 10..13 inclusive
    }

    private sealed class ModbusReadTestServer : IAsyncDisposable
    {
        private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _serverTask;

        public ModbusReadTestServer()
        {
            _listener.Start();
            _serverTask = Task.Run(() => RunAsync(_cts.Token));
        }

        public int Port => ((IPEndPoint)_listener.LocalEndpoint).Port;
        public ConcurrentBag<(byte FunctionCode, ushort Start, ushort Count)> Requests { get; } = new();

        public Task StartAsync() => Task.CompletedTask;

        private async Task RunAsync(CancellationToken token)
        {
            try
            {
                using var client = await _listener.AcceptTcpClientAsync(token);
                using var stream = client.GetStream();

                while (!token.IsCancellationRequested)
                {
                    var header = new byte[12];
                    try { await ReadExactlyAsync(stream, header, token); }
                    catch (IOException) { break; }

                    var functionCode = header[7];
                    ushort start = (ushort)((header[8] << 8) | header[9]);
                    ushort count = (ushort)((header[10] << 8) | header[11]);
                    Requests.Add((functionCode, start, count));

                    byte[] response;
                    if (functionCode == 0x03 || functionCode == 0x04) // Holding/Input registers
                    {
                        response = new byte[9 + count * 2];
                        Array.Copy(header, 0, response, 0, 6);
                        response[4] = (byte)(((3 + count * 2) >> 8) & 0xFF);
                        response[5] = (byte)((3 + count * 2) & 0xFF);
                        response[6] = header[6];
                        response[7] = functionCode;
                        response[8] = (byte)(count * 2);
                    }
                    else if (functionCode == 0x01 || functionCode == 0x02) // Coils/Discrete inputs
                    {
                        int byteCount = (count + 7) / 8;
                        response = new byte[9 + byteCount];
                        Array.Copy(header, 0, response, 0, 6);
                        response[4] = (byte)(((3 + byteCount) >> 8) & 0xFF);
                        response[5] = (byte)((3 + byteCount) & 0xFF);
                        response[6] = header[6];
                        response[7] = functionCode;
                        response[8] = (byte)byteCount;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unexpected Modbus function code: 0x{functionCode:X2}");
                    }

                    // register/coil values left as zero; test only checks the request shape
                    await stream.WriteAsync(response, token);
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
        }

        private static async Task ReadExactlyAsync(NetworkStream stream, byte[] buffer, CancellationToken token)
        {
            var offset = 0;
            while (offset < buffer.Length)
            {
                var read = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length - offset), token);
                if (read == 0)
                    throw new IOException("Socket closed while reading Modbus frame");
                offset += read;
            }
        }

        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            _listener.Stop();
            try { await _serverTask; } catch { }
            _cts.Dispose();
        }
    }
}
