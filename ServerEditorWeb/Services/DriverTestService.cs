using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ServerEditorWeb.Services;

public record DriverTestResult(bool Success, string Message, string? Value = null);

/// <summary>
/// Tests driver configurations directly by connecting to the target device/service
/// and performing a one-shot read using the same protocol the runtime driver uses.
/// </summary>
public class DriverTestService
{
    private static readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<DriverTestResult> TestReadAsync(string driverKey, string configJson)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var result = driverKey switch
            {
                "Modbus" => await TestModbusAsync(configJson),
                "S7" => await TestS7Async(configJson),
                "OpcUaClient" => await TestOpcUaClientAsync(configJson),
                "Mqtt" => await TestMqttAsync(configJson),
                "Rest" => await TestRestAsync(configJson),
                "Csv" => TestCsv(configJson),
                "Tcp" => await TestTcpAsync(configJson),
                "Sql" => await TestSqlAsync(configJson),
                "EtherNetIP" => await TestTcpConnectionAsync(configJson, 44818, "EtherNet/IP"),
                "Knx" => await TestTcpConnectionAsync(configJson, 3671, "KNX"),
                "Simulation" => TestSimulation(configJson),
                _ => new DriverTestResult(false, $"Unknown driver: {driverKey}")
            };
            return result with { Message = $"{result.Message} ({sw.ElapsedMilliseconds} ms)" };
        }
        catch (Exception ex) { return new DriverTestResult(false, $"Error: {ex.Message}"); }
    }

    private async Task<DriverTestResult> TestModbusAsync(string configJson)
    {
        var cfg = JsonSerializer.Deserialize<ModbusTestCfg>(configJson, _jsonOpts);
        if (cfg is null || string.IsNullOrWhiteSpace(cfg.IpAddress))
            return new DriverTestResult(false, "Invalid Modbus configuration");
        int port = cfg.Port > 0 ? cfg.Port : 502;
        using var tcp = new TcpClient();
        var ct = tcp.ConnectAsync(cfg.IpAddress, port);
        if (await Task.WhenAny(ct, Task.Delay(3000)) != ct)
            return new DriverTestResult(false, $"Connection timeout to {cfg.IpAddress}:{port}");
        await ct;
        var factory = new NModbus.ModbusFactory();
        using var master = factory.CreateMaster(tcp);
        ushort register = cfg.Register;
        byte unitId = cfg.UnitId > 0 ? cfg.UnitId : (byte)1;
        if (cfg.RegisterType == "Coil")
        {
            bool[] res = await master.ReadCoilsAsync(unitId, register, 1);
            return new DriverTestResult(true, "Modbus coil read OK", res.Length > 0 ? res[0].ToString() : "(empty)");
        }
        if (cfg.RegisterType == "DiscreteInput")
        {
            bool[] res = await master.ReadInputsAsync(unitId, register, 1);
            return new DriverTestResult(true, "Modbus discrete input read OK", res.Length > 0 ? res[0].ToString() : "(empty)");
        }
        ushort[] regs = cfg.RegisterType == "InputRegister"
            ? await master.ReadInputRegistersAsync(unitId, register, 1)
            : await master.ReadHoldingRegistersAsync(unitId, register, 1);
        return new DriverTestResult(true, "Modbus register read OK", regs.Length > 0 ? regs[0].ToString() : "(empty)");
    }
    private record ModbusTestCfg(string IpAddress, int Port, byte UnitId, ushort Register, string? RegisterType);

    private async Task<DriverTestResult> TestS7Async(string configJson)
    {
        var cfg = JsonSerializer.Deserialize<S7TestCfg>(configJson, _jsonOpts);
        if (cfg is null || string.IsNullOrWhiteSpace(cfg.IpAddress))
            return new DriverTestResult(false, "Invalid S7 configuration");
        using var plc = new S7.Net.Plc(S7.Net.CpuType.S71200, cfg.IpAddress, (short)cfg.Rack, (short)cfg.Slot);
        await plc.OpenAsync();
        if (!plc.IsConnected)
            return new DriverTestResult(false, $"Cannot connect to S7 at {cfg.IpAddress}");
        if (string.IsNullOrWhiteSpace(cfg.Address))
            return new DriverTestResult(true, "S7 connected OK (no address to read)");
        var val = await plc.ReadAsync(cfg.Address);
        return new DriverTestResult(true, "S7 read OK", val?.ToString() ?? "(null)");
    }
    private record S7TestCfg(string IpAddress, int Rack, int Slot, string? Address);

    private async Task<DriverTestResult> TestOpcUaClientAsync(string configJson)
    {
        var cfg = JsonSerializer.Deserialize<OpcUaTestCfg>(configJson, _jsonOpts);
        if (cfg is null || string.IsNullOrWhiteSpace(cfg.EndpointUrl))
            return new DriverTestResult(false, "Invalid OPC UA Client configuration");
        var pkiRoot = "%LocalApplicationData%/ServerEditorWeb/pki";
        var appCfg = new Opc.Ua.ApplicationConfiguration
        {
            ApplicationName = "DriverTestOpcClient",
            ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:DriverTestOpcClient",
            ApplicationType = Opc.Ua.ApplicationType.Client,
            SecurityConfiguration = new Opc.Ua.SecurityConfiguration
            {
                ApplicationCertificate = new Opc.Ua.CertificateIdentifier { StoreType = Opc.Ua.CertificateStoreType.Directory, StorePath = $"{pkiRoot}/own", SubjectName = "DriverTestOpcClient" },
                TrustedPeerCertificates = new Opc.Ua.CertificateTrustList { StoreType = Opc.Ua.CertificateStoreType.Directory, StorePath = $"{pkiRoot}/trusted" },
                TrustedIssuerCertificates = new Opc.Ua.CertificateTrustList { StoreType = Opc.Ua.CertificateStoreType.Directory, StorePath = $"{pkiRoot}/issuer" },
                RejectedCertificateStore = new Opc.Ua.CertificateTrustList { StoreType = Opc.Ua.CertificateStoreType.Directory, StorePath = $"{pkiRoot}/rejected" }
            },
            TransportQuotas = new Opc.Ua.TransportQuotas { OperationTimeout = 10000 },
            ClientConfiguration = new Opc.Ua.ClientConfiguration { DefaultSessionTimeout = 30000 }
        };
        await appCfg.Validate(Opc.Ua.ApplicationType.Client);
        appCfg.CertificateValidator.CertificateValidation += (_, e) =>
        { if (e.Error.StatusCode == Opc.Ua.StatusCodes.BadCertificateUntrusted) e.Accept = true; };
        var selectedEndpoint = Opc.Ua.Client.CoreClientUtils.SelectEndpoint(appCfg, cfg.EndpointUrl, false, 10000);
        var epCfg = Opc.Ua.EndpointConfiguration.Create(appCfg);
        var endpoint = new Opc.Ua.ConfiguredEndpoint(null, selectedEndpoint, epCfg);
        var session = await Opc.Ua.Client.Session.Create(appCfg, endpoint, false, "DriverTestSession", 30000, new Opc.Ua.UserIdentity(new Opc.Ua.AnonymousIdentityToken()), null);
        try
        {
            if (string.IsNullOrWhiteSpace(cfg.NodeId))
                return new DriverTestResult(true, "OPC UA connected OK (no NodeId to read)");
            var nodeId = Opc.Ua.NodeId.Parse(cfg.NodeId);
            var dv = await Opc.Ua.Client.SessionClientExtensions.ReadValueAsync(session, nodeId);
            if (Opc.Ua.StatusCode.IsBad(dv.StatusCode))
                return new DriverTestResult(false, $"Read failed: {Opc.Ua.StatusCodes.GetBrowseName(dv.StatusCode.Code)}");
            return new DriverTestResult(true, "OPC UA read OK", dv.WrappedValue.ToString() ?? "(null)");
        }
        finally { await session.CloseAsync(); session.Dispose(); }
    }
    private record OpcUaTestCfg(string EndpointUrl, string? NodeId);

    private async Task<DriverTestResult> TestMqttAsync(string configJson)
    {
        var cfg = JsonSerializer.Deserialize<MqttTestCfg>(configJson, _jsonOpts);
        if (cfg is null || string.IsNullOrWhiteSpace(cfg.Broker))
            return new DriverTestResult(false, "Invalid MQTT configuration");
        int port = cfg.Port > 0 ? cfg.Port : 1883;
        var factory = new MQTTnet.MqttClientFactory();
        using var mqttClient = factory.CreateMqttClient();
        var options = new MQTTnet.MqttClientOptionsBuilder().WithTcpServer(cfg.Broker, port).WithTimeout(TimeSpan.FromSeconds(5)).Build();
        var connectResult = await mqttClient.ConnectAsync(options);
        if (connectResult.ResultCode != MQTTnet.MqttClientConnectResultCode.Success)
            return new DriverTestResult(false, $"MQTT connect failed: {connectResult.ResultCode}");
        if (string.IsNullOrWhiteSpace(cfg.Topic))
        {
            await mqttClient.DisconnectAsync(new MQTTnet.MqttClientDisconnectOptions());
            return new DriverTestResult(true, "MQTT connected OK (no topic to subscribe)");
        }
        var tcs = new TaskCompletionSource<string>();
        mqttClient.ApplicationMessageReceivedAsync += e =>
        { tcs.TrySetResult(Encoding.UTF8.GetString(e.ApplicationMessage.Payload)); return Task.CompletedTask; };
        var subOpts = new MQTTnet.MqttClientSubscribeOptionsBuilder().WithTopicFilter(cfg.Topic).Build();
        await mqttClient.SubscribeAsync(subOpts);
        var completed = await Task.WhenAny(tcs.Task, Task.Delay(5000));
        if (completed == tcs.Task)
        {
            var payload = await tcs.Task;
            string display = payload;
            if (!string.IsNullOrEmpty(cfg.JsonPath))
            {
                try
                {
                    var doc = JsonDocument.Parse(payload);
                    var elem = doc.RootElement;
                    foreach (var p in cfg.JsonPath.Split('.')) { if (elem.TryGetProperty(p, out var next)) elem = next; else break; }
                    display = elem.ToString()!;
                }
                catch { }
            }
            await mqttClient.DisconnectAsync(new MQTTnet.MqttClientDisconnectOptions());
            return new DriverTestResult(true, $"MQTT received on topic", display);
        }
        await mqttClient.DisconnectAsync(new MQTTnet.MqttClientDisconnectOptions());
        return new DriverTestResult(true, "MQTT subscribed OK, no message within 5s");
    }
    private record MqttTestCfg(string Broker, int Port, string? Topic, string? JsonPath);

    private async Task<DriverTestResult> TestRestAsync(string configJson)
    {
        var cfg = JsonSerializer.Deserialize<RestTestCfg>(configJson, _jsonOpts);
        if (cfg is null || string.IsNullOrWhiteSpace(cfg.Url))
            return new DriverTestResult(false, "Invalid REST configuration");
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var response = await http.GetAsync(cfg.Url);
        if (!response.IsSuccessStatusCode)
            return new DriverTestResult(false, $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
        var body = await response.Content.ReadAsStringAsync();
        string display = body.Length > 200 ? body[..200] + "..." : body;
        if (!string.IsNullOrEmpty(cfg.JsonPath))
        {
            try
            {
                var doc = JsonDocument.Parse(body);
                var elem = doc.RootElement;
                foreach (var p in cfg.JsonPath.Split('.')) { if (elem.TryGetProperty(p, out var next)) elem = next; else return new DriverTestResult(false, $"JsonPath not found in response"); }
                display = elem.ToString()!;
            }
            catch (JsonException ex) { return new DriverTestResult(false, $"JSON parse error: {ex.Message}"); }
        }
        return new DriverTestResult(true, "REST read OK", display);
    }
    private record RestTestCfg(string Url, string? JsonPath);

    private DriverTestResult TestCsv(string configJson)
    {
        var cfg = JsonSerializer.Deserialize<CsvTestCfg>(configJson, _jsonOpts);
        if (cfg is null || string.IsNullOrWhiteSpace(cfg.FilePath))
            return new DriverTestResult(false, "Invalid CSV configuration");
        if (!File.Exists(cfg.FilePath))
            return new DriverTestResult(false, $"File not found: {cfg.FilePath}");
        using var fs = new FileStream(cfg.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs);
        var rows = new List<string[]>();
        string? line;
        while ((line = sr.ReadLine()) != null) rows.Add(line.Split(new[] { ',', ';' }));
        string valStr = "";
        if (!string.IsNullOrEmpty(cfg.Key))
        {
            var row = rows.FirstOrDefault(r => r.Length > 0 && r[0] == cfg.Key);
            if (row != null && row.Length > cfg.ColumnIndex) valStr = row[cfg.ColumnIndex];
            else return new DriverTestResult(false, $"Key not found in CSV");
        }
        else
        {
            if (rows.Count > cfg.RowIndex && rows[cfg.RowIndex].Length > cfg.ColumnIndex) valStr = rows[cfg.RowIndex][cfg.ColumnIndex];
            else return new DriverTestResult(false, $"Row/Column out of range ({rows.Count} rows)");
        }
        return new DriverTestResult(true, "CSV read OK", valStr);
    }
    private record CsvTestCfg(string FilePath, int RowIndex, int ColumnIndex, string? Key);

    private async Task<DriverTestResult> TestTcpAsync(string configJson)
    {
        var cfg = JsonSerializer.Deserialize<TcpTestCfg>(configJson, _jsonOpts);
        if (cfg is null || string.IsNullOrWhiteSpace(cfg.IpAddress) || cfg.Port <= 0)
            return new DriverTestResult(false, "Invalid TCP configuration");
        using var tcp = new TcpClient();
        var ct = tcp.ConnectAsync(cfg.IpAddress, cfg.Port);
        if (await Task.WhenAny(ct, Task.Delay(3000)) != ct)
            return new DriverTestResult(false, $"Connection timeout to {cfg.IpAddress}:{cfg.Port}");
        await ct;
        using var stream = tcp.GetStream();
        stream.ReadTimeout = 2000; stream.WriteTimeout = 2000;
        if (!string.IsNullOrEmpty(cfg.Command))
        {
            var cmdStr = cfg.Command.Replace("\\r", "\r").Replace("\\n", "\n");
            await stream.WriteAsync(Encoding.ASCII.GetBytes(cmdStr));
            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer);
            if (bytesRead > 0)
            {
                var response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                string display = response;
                if (!string.IsNullOrEmpty(cfg.Regex))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(response, cfg.Regex);
                    if (match.Success) display = match.Groups.Count > 1 ? match.Groups[1].Value : match.Value;
                    else display = $"(regex no match) {response}";
                }
                return new DriverTestResult(true, "TCP read OK", display.Trim());
            }
            return new DriverTestResult(true, "TCP connected, command sent, no response");
        }
        return new DriverTestResult(true, $"TCP connected to {cfg.IpAddress}:{cfg.Port}");
    }
    private record TcpTestCfg(string IpAddress, int Port, string? Command, string? Regex);

    private async Task<DriverTestResult> TestSqlAsync(string configJson)
    {
        var cfg = JsonSerializer.Deserialize<SqlTestCfg>(configJson, _jsonOpts);
        if (cfg is null || string.IsNullOrWhiteSpace(cfg.ConnectionString))
            return new DriverTestResult(false, "Invalid SQL configuration");
        System.Data.Common.DbConnection conn = cfg.ConnectionString.Contains(".db", StringComparison.OrdinalIgnoreCase)
                || (cfg.ConnectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase)
                    && !cfg.ConnectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase))
            ? new Microsoft.Data.Sqlite.SqliteConnection(cfg.ConnectionString)
            : new Npgsql.NpgsqlConnection(cfg.ConnectionString);
        await using (conn)
        {
            await conn.OpenAsync();
            if (string.IsNullOrWhiteSpace(cfg.Query))
                return new DriverTestResult(true, "SQL connected OK (no query)");
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = cfg.Query;
            var result = await cmd.ExecuteScalarAsync();
            return new DriverTestResult(true, "SQL query OK", result?.ToString() ?? "(null)");
        }
    }
    private record SqlTestCfg(string ConnectionString, string? Query);

    private DriverTestResult TestSimulation(string configJson)
    {
        var cfg = JsonSerializer.Deserialize<SimTestCfg>(configJson, _jsonOpts);
        if (cfg is null) return new DriverTestResult(false, "Invalid Simulation configuration");
        var period = cfg.Period > 0 ? cfg.Period : 10000.0;
        var phaseRad = cfg.Phase * Math.PI / 180.0;
        var elapsed = Environment.TickCount64 % (long)period;
        var t = elapsed / period;
        var angle = t * 2.0 * Math.PI + phaseRad;
        object value = cfg.Function?.ToLowerInvariant() switch
        {
            "sine" or "sin" => cfg.Offset + cfg.Amplitude * Math.Sin(angle),
            "cosine" or "cos" => cfg.Offset + cfg.Amplitude * Math.Cos(angle),
            "ramp" => cfg.Offset + (t * cfg.Amplitude),
            "triangle" => cfg.Offset + cfg.Amplitude * (2.0 * Math.Abs(2.0 * t - 1.0) - 1.0),
            "square" => cfg.Offset + (t < cfg.DutyCycle ? cfg.Amplitude : -cfg.Amplitude),
            "sawtooth" => cfg.Offset + cfg.Amplitude * (2.0 * t - 1.0),
            "random" => cfg.Min + Random.Shared.NextDouble() * (cfg.Max - cfg.Min),
            "randomint" => Random.Shared.Next((int)cfg.Min, (int)cfg.Max + 1),
            "blink" => t < cfg.DutyCycle,
            "counter" => Environment.TickCount64 / (long)Math.Max(cfg.Period, 1),
            "pulse" => t < cfg.DutyCycle ? 1.0 : 0.0,
            _ => cfg.Offset + cfg.Amplitude * Math.Sin(angle)
        };
        var display = value is double d ? d.ToString("F4") : value.ToString();
        return new DriverTestResult(true, $"Simulation ({cfg.Function ?? "Sine"})", display);
    }
    private record SimTestCfg
    {
        public string? Function { get; init; } = "Sine";
        public double Period { get; init; } = 10000;
        public double Amplitude { get; init; } = 100;
        public double Offset { get; init; }
        public double Phase { get; init; }
        public double DutyCycle { get; init; } = 0.5;
        public double Min { get; init; }
        public double Max { get; init; } = 100;
    }

    private async Task<DriverTestResult> TestTcpConnectionAsync(string configJson, int defaultPort, string label)
    {
        using var doc = JsonDocument.Parse(configJson);
        var root = doc.RootElement;
        string ip = "";
        int port = defaultPort;
        if (root.TryGetProperty("IpAddress", out var ipEl)) ip = ipEl.GetString() ?? "";
        if (root.TryGetProperty("Port", out var portEl) && portEl.TryGetInt32(out var p) && p > 0) port = p;
        if (string.IsNullOrWhiteSpace(ip))
            return new DriverTestResult(false, $"Invalid {label} configuration");
        using var tcp = new TcpClient();
        var ct = tcp.ConnectAsync(ip, port);
        if (await Task.WhenAny(ct, Task.Delay(3000)) != ct)
            return new DriverTestResult(false, $"{label} connection timeout to {ip}:{port}");
        await ct;
        return new DriverTestResult(true, $"{label} connected to {ip}:{port}");
    }
}
