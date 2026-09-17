// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Opc.Ua;
using SimpleOpcFileServer;
using Xunit;

namespace Tests.Drivers;

public class ModbusWriteDriverTests
{
    [Fact]
    public async Task OnSimpleWriteValue_ForHoldingRegister_WritesToModbusSlave()
    {
        await using var server = new ModbusWriteTestServer();
        await server.StartAsync();

        using var driver = new ModbusDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("ModbusWrite", DataTypeIds.UInt16);
        variable.Value = (ushort)0;

        var config = JsonSerializer.Serialize(new ModbusConfig
        {
            IpAddress = "127.0.0.1",
            Port = server.Port,
            UnitId = 1,
            Register = 7,
            RegisterType = "HoldingRegister"
        });

        driver.AddItem(variable, config);

        object writeValue = (ushort)123;
        var result = variable.OnSimpleWriteValue!(null!, variable, ref writeValue);

        Assert.Equal(StatusCodes.Good, result.StatusCode);
        Assert.Equal((ushort)123, server.LastWrittenValue);
        Assert.Equal((ushort)7, server.LastRegister);
        Assert.Equal((ushort)123, variable.Value);
    }

    private sealed class ModbusWriteTestServer : IAsyncDisposable
    {
        private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _serverTask;

        public ModbusWriteTestServer()
        {
            _listener.Start();
            _serverTask = Task.Run(() => RunAsync(_cts.Token));
        }

        public int Port => ((IPEndPoint)_listener.LocalEndpoint).Port;
        public ushort LastWrittenValue { get; private set; }
        public ushort LastRegister { get; private set; }

        public Task StartAsync() => Task.CompletedTask;

        private async Task RunAsync(CancellationToken token)
        {
            try
            {
                using var client = await _listener.AcceptTcpClientAsync(token);
                using var stream = client.GetStream();
                var buffer = new byte[12];
                await ReadExactlyAsync(stream, buffer, token);

                var functionCode = buffer[7];
                if (functionCode != 0x06)
                    throw new InvalidOperationException($"Unexpected Modbus function code: 0x{functionCode:X2}");

                LastRegister = (ushort)((buffer[8] << 8) | buffer[9]);
                LastWrittenValue = (ushort)((buffer[10] << 8) | buffer[11]);

                var response = new byte[12];
                Array.Copy(buffer, response, buffer.Length);
                await stream.WriteAsync(response, token);
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
