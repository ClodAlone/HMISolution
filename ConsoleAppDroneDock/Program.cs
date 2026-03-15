using ConsoleAppDroneDock;

if (args.Contains("--help"))
{
    Console.WriteLine("Usage: ConsoleAppDroneDock [broker-address] [telemetry-topic] [camera-topic]");
    return;
}

string broker = args.Length > 0 ? args[0] : "localhost";
string telemetryTopic = args.Length > 1 ? args[1] : "dji/dock/telemetry";
string cameraTopic = args.Length > 2 ? args[2] : "dji/dock/camera";

Console.Title = "DJI Dock Monitor";
Console.WriteLine($"Initializing DJI Dock Real-time Monitor...");
Console.WriteLine($"Target Broker:   {broker}");
Console.WriteLine($"Telemetry Topic: {telemetryTopic}");
Console.WriteLine($"Camera Topic:    {cameraTopic}");

var cts = new CancellationTokenSource();
var dockClient = new DJIDockClient(broker, 1883, telemetryTopic, cameraTopic);

dockClient.OnLog += (message) => 
{
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine($"[{DateTime.Now:T}] [SYSTEM] {message}");
    Console.ResetColor();
};

dockClient.OnTelemetryReceived += (telemetry) =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[{DateTime.Now:T}] [LIVE] {telemetry}");
    Console.ResetColor();
};

dockClient.OnCameraStreamReceived += (streamInfo) =>
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"[{DateTime.Now:T}] [VIDEO] {streamInfo}");
    Console.ResetColor();

    // Optional: Open stream in browser if live
    // if (streamInfo.Status == StreamState.Live && !string.IsNullOrEmpty(streamInfo.StreamUrl)) ...
};

// Handle Ctrl+C or process exit
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    Console.WriteLine("Stop requested...");
    cts.Cancel();
    // Client handles cleanup via cancellation token
};

Console.WriteLine("Press 'q' to quit, 'n' to new mission, 's' to start mission.");

var monitorTask = dockClient.StartMonitoringAsync(cts.Token);

FlyPath? currentMission = null;

try
{
    while (!cts.Token.IsCancellationRequested)
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.Q)
            {
                cts.Cancel();
                break;
            }
            else if (key == ConsoleKey.N)
            {
                Console.WriteLine("\n[COMMAND] Creating new FlyPath...");
                // Create a demo mission
                currentMission = new FlyPath
                {
                    MissionId = Guid.NewGuid().ToString(),
                    Waypoints = new List<Waypoint>
                    {
                        new Waypoint { Latitude = 34.0522, Longitude = -118.2437, Altitude = 50, Speed = 10 },
                        new Waypoint { Latitude = 34.0525, Longitude = -118.2440, Altitude = 60, Speed = 12 },
                        new Waypoint { Latitude = 34.0528, Longitude = -118.2435, Altitude = 50, Speed = 8 }
                    }
                };
                Console.WriteLine($"[COMMAND] New Mission Created: {currentMission}");

                Console.WriteLine("[COMMAND] Uploading...");
                await dockClient.UploadFlyPathAsync(currentMission);
            }
            else if (key == ConsoleKey.S)
            {
                if (currentMission != null)
                {
                    Console.WriteLine($"\n[COMMAND] Starting Mission: {currentMission.MissionId}");
                    await dockClient.StartFlyPathAsync(currentMission.MissionId);
                }
                else
                {
                    Console.WriteLine("\n[COMMAND] No mission loaded. Press 'n' to create and upload one.");
                }
            }
        }
        await Task.Delay(100);
    }

    await monitorTask;
}
catch (OperationCanceledException)
{
    Console.WriteLine("Monitoring canceled.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    Console.WriteLine("Program exited.");
}
