using System;
using System.Threading;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleAppDroneDock
{
    public enum DockState
    {
        Idle,
        Charging,
        Ready,
        MissionInProgress,
        Error
    }

    public enum StreamState
    {
        Off,
        Live,
        Error,
        Buffering
    }

    public class CameraStreamInfo
    {
        [JsonPropertyName("stream_url")]
        public string StreamUrl { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StreamState Status { get; set; }

        [JsonPropertyName("resolution")]
        public string Resolution { get; set; } = "1080p";

        public override string ToString()
        {
            return $"Stream: {Status} | Resolution: {Resolution} | URL: {StreamUrl}";
        }
    }

    public class Waypoint
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("altitude")]
        public double Altitude { get; set; } // meters

        [JsonPropertyName("speed")]
        public double Speed { get; set; } // m/s

        public override string ToString() => $"Lat: {Latitude:F6}, Lon: {Longitude:F6}, Alt: {Altitude}m";
    }

    public class FlyPath
    {
        [JsonPropertyName("mission_id")]
        public string MissionId { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("waypoints")]
        public List<Waypoint> Waypoints { get; set; } = new();

        public override string ToString() => $"Mission ID: {MissionId}, Waypoints: {Waypoints.Count}";
    }

    public class DockTelemetry
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // --- Dock Environment & Status ---
        [JsonPropertyName("dock_temperature")]
        public double DockTemperature { get; set; } // Celsius

        [JsonPropertyName("wind_speed")]
        public double WindSpeed { get; set; } // m/s

        [JsonPropertyName("rain_detected")]
        public bool IsRainDetected { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DockState Status { get; set; }

        // --- Drone Power & Link ---
        [JsonPropertyName("drone_battery")]
        public double DroneBattery { get; set; } // Percentage

        [JsonPropertyName("drone_voltage")]
        public double DroneVoltage { get; set; } // Volts

        [JsonPropertyName("link_quality")]
        public int LinkQuality { get; set; } // 0-100

        [JsonPropertyName("satellite_count")]
        public int SatelliteCount { get; set; }

        [JsonPropertyName("rtk_status")]
        public int RtkStatus { get; set; } // 0:None, 1:Float, 2:Fixed

        // --- Positioning (WGS84) ---
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("altitude")]
        public double Altitude { get; set; } // meters (MSL)

        [JsonPropertyName("height_agl")]
        public double HeightAboveGround { get; set; } // meters

        [JsonPropertyName("home_latitude")]
        public double HomeLatitude { get; set; }

        [JsonPropertyName("home_longitude")]
        public double HomeLongitude { get; set; }

        // --- Attitude & Velocity ---
        [JsonPropertyName("pitch")]
        public double Pitch { get; set; }

        [JsonPropertyName("roll")]
        public double Roll { get; set; }

        [JsonPropertyName("yaw")]
        public double Yaw { get; set; }

        [JsonPropertyName("velocity_x")]
        public double VelocityX { get; set; }

        [JsonPropertyName("velocity_y")]
        public double VelocityY { get; set; }

        [JsonPropertyName("velocity_z")]
        public double VelocityZ { get; set; }

        // --- Flight State ---
        [JsonPropertyName("flight_mode")]
        public string FlightMode { get; set; } = "Unknown";

        [JsonPropertyName("mission_id")]
        public string CurrentMissionId { get; set; } = "None";

        [JsonPropertyName("waypoint_index")]
        public int CurrentWaypointIndex { get; set; }

        [JsonPropertyName("storage_capacity")]
        public double StorageCapacity { get; set; } // MB

        [JsonPropertyName("storage_available")]
        public double StorageAvailable { get; set; } // MB

        public override string ToString()
        {
            return $"[{Timestamp:T}] {Status} | Bat: {DroneBattery:F0}% ({DroneVoltage:F1}V) | Signal: {LinkQuality}% | Sats: {SatelliteCount} (RTK:{RtkStatus})\n" +
                   $"      Pos: {Latitude:F6}, {Longitude:F6} | Alt: {Altitude:F1}m (AGL: {HeightAboveGround:F1}m)\n" +
                   $"      Att: P{Pitch:F1} R{Roll:F1} Y{Yaw:F1} | Vel: {Math.Sqrt(VelocityX*VelocityX + VelocityY*VelocityY):F1}m/s\n" +
                   $"      Env: {DockTemperature:F1}°C | Wind: {WindSpeed:F1}m/s | Rain: {(IsRainDetected ? "YES" : "NO")}\n" +
                   $"      Mode: {FlightMode} | Mission: {CurrentMissionId} (WP:{CurrentWaypointIndex})";
        }
    }

    public class DJIDockClient
    {
        private IMqttClient? _mqttClient;
        private readonly string _brokerHost;
        private readonly int _brokerPort;
        private readonly string _telemetryTopic;
        private readonly string _cameraTopic;

        // Events for real-time updates
        public event Action<string>? OnLog;
        public event Action<DockTelemetry>? OnTelemetryReceived;
        public event Action<CameraStreamInfo>? OnCameraStreamReceived;

        public DJIDockClient(string brokerHost = "localhost", int brokerPort = 1883,
            string telemetryTopic = "dji/dock/telemetry", string cameraTopic = "dji/dock/camera")
        {
            _brokerHost = brokerHost;
            _brokerPort = brokerPort;
            _telemetryTopic = telemetryTopic;
            _cameraTopic = cameraTopic;
        }

        public async Task StartMonitoringAsync(CancellationToken token)
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_brokerHost, _brokerPort)
                .WithClientId($"DJIDockClient_{Guid.NewGuid()}")
                .WithCleanSession()
                .Build();

            _mqttClient.ApplicationMessageReceivedAsync += HandleMessageReceivedAsync;
            _mqttClient.DisconnectedAsync += HandleDisconnectedAsync;

            OnLog?.Invoke($"Connecting to MQTT Broker at {_brokerHost}:{_brokerPort}...");

            try
            {
                await _mqttClient.ConnectAsync(options, token);
                OnLog?.Invoke("Connected to MQTT Broker.");

                await _mqttClient.SubscribeAsync(_telemetryTopic);
                OnLog?.Invoke($"Subscribed to telemetry topic: {_telemetryTopic}");

                await _mqttClient.SubscribeAsync(_cameraTopic);
                OnLog?.Invoke($"Subscribed to camera topic: {_cameraTopic}");

                // Keep the task alive until cancellation is requested
                // Use a loop to allow other operations (like sending commands) while monitoring
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(100, token);
                }
            }
            catch (TaskCanceledException)
            {
                // Normal cancellation
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"Connection Error: {ex.Message}");
            }
            finally
            {
                await StopAsync();
            }

            OnLog?.Invoke("Monitoring stopped.");
        }

        public async Task UploadFlyPathAsync(FlyPath flyPath, CancellationToken token = default)
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                OnLog?.Invoke("Error: Not connected to MQTT broker.");
                return;
            }

            var payload = JsonSerializer.Serialize(flyPath);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("dji/dock/mission/upload")
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(message, token);
            OnLog?.Invoke($"Uploaded FlyPath: {flyPath.MissionId} with {flyPath.Waypoints.Count} waypoints.");
        }

        public async Task StartFlyPathAsync(string missionId, CancellationToken token = default)
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                OnLog?.Invoke("Error: Not connected to MQTT broker.");
                return;
            }

            var command = new { action = "start_mission", mission_id = missionId };
            var payload = JsonSerializer.Serialize(command);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("dji/dock/mission/start")
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(message, token);
            OnLog?.Invoke($"Sent Start Command for Mission ID: {missionId}");
        }

        public async Task StopAsync()
        {
            if (_mqttClient != null)
            {
                if (_mqttClient.IsConnected)
                {
                    await _mqttClient.DisconnectAsync();
                }
                _mqttClient.Dispose();
                _mqttClient = null;
            }
        }

        // Expected JSON payload example:
        // {
        //   "timestamp": "2023-10-27T10:00:00",
        //   "drone_battery": 85.0,
        //   "dock_temperature": 25.5,
        //   "wind_speed": 3.2,
        //   "rain_detected": false,
        //   "status": "Ready"
        // }
        private Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                var payload = e.ApplicationMessage.ConvertPayloadToString();
                var topic = e.ApplicationMessage.Topic;
                // OnLog?.Invoke($"Received payload on {topic}: {payload}"); // Optional debug log

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                if (topic == _telemetryTopic)
                {
                    var telemetry = JsonSerializer.Deserialize<DockTelemetry>(payload, options);
                    if (telemetry != null)
                    {
                        if (telemetry.Timestamp == default) 
                            telemetry.Timestamp = DateTime.Now;

                        OnTelemetryReceived?.Invoke(telemetry);
                    }
                }
                else if (topic == _cameraTopic)
                {
                    var cameraInfo = JsonSerializer.Deserialize<CameraStreamInfo>(payload, options);
                    if (cameraInfo != null)
                    {
                        OnCameraStreamReceived?.Invoke(cameraInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"Error parsing message: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        private Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs e)
        {
            OnLog?.Invoke("Disconnected from broker.");
            return Task.CompletedTask;
        }
    }
}

