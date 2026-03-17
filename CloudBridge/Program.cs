using CloudBridge;

Console.WriteLine("╔═══════════════════════════════════════╗");
Console.WriteLine("║        HMI Cloud Bridge               ║");
Console.WriteLine("║  OPC UA Server ←→ Cloud Relay Hub     ║");
Console.WriteLine("╚═══════════════════════════════════════╝");

// Parse CLI arguments: CloudBridge [nodes.json]
var configPath = args.FirstOrDefault(a => !a.StartsWith("-")) ?? "nodes.json";
if (!Path.IsPathRooted(configPath))
    configPath = Path.GetFullPath(configPath);

if (!File.Exists(configPath))
{
    Console.Error.WriteLine($"Configuration file not found: {configPath}");
    Console.Error.WriteLine("Usage: CloudBridge [path-to-nodes.json]");
    return 1;
}

var json = System.Text.Json.JsonSerializer.Deserialize<SharedModels.NodeModel>(
    File.ReadAllText(configPath));

if (json?.Server?.CloudRelay is not { Enabled: true } relayConfig
    || string.IsNullOrEmpty(relayConfig.HubUrl))
{
    Console.Error.WriteLine("Cloud relay is not configured or not enabled in the project settings.");
    Console.Error.WriteLine("Set Server.CloudRelay.Enabled = true and provide a HubUrl in nodes.json.");
    return 1;
}

var opcEndpoint = json.Server.EndpointUrl;
var hubUrl = relayConfig.HubUrl;
var apiKey = relayConfig.ApiKey;

Console.WriteLine($"  OPC UA endpoint : {opcEndpoint}");
Console.WriteLine($"  Cloud relay hub : {hubUrl}");
Console.WriteLine();

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var bridge = new BridgeService(opcEndpoint, hubUrl, apiKey);
await bridge.RunAsync(cts.Token);

return 0;
