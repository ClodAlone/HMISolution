using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using MQTTnet;
using MQTTnet.Client;

namespace DataLogger
{
    public class MqttInfluxConfig
    {
        public string MqttBroker { get; set; } = "localhost";
        public int MqttPort { get; set; } = 1883;
        public string MqttTopic { get; set; } = "sensors/#";
        
        public string InfluxUrl { get; set; } = "http://localhost:8086";
        public string InfluxToken { get; set; } = "my-token";
        public string InfluxBucket { get; set; } = "my-bucket";
        public string InfluxOrg { get; set; } = "my-org";
    }

    public class MqttInfluxBridge : IDisposable
    {
        private readonly MqttInfluxConfig _config;
        private IMqttClient? _mqttClient;
        private InfluxDBClient? _influxClient;
        private WriteApi? _writeApi;

        public MqttInfluxBridge(MqttInfluxConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task StartAsync()
        {
            // Setup InfluxDB
            _influxClient = new InfluxDBClient(_config.InfluxUrl, _config.InfluxToken);
            _writeApi = _influxClient.GetWriteApi();

            // Setup MQTT
            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();

            var mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_config.MqttBroker, _config.MqttPort)
                .Build();

            _mqttClient.ApplicationMessageReceivedAsync += HandleMessageReceived;

            await _mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);

            var mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(_config.MqttTopic))
                .Build();

            await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        }

        public async Task StopAsync()
        {
            if (_mqttClient != null)
            {
                await _mqttClient.DisconnectAsync();
            }
            _writeApi?.Dispose();
            _influxClient?.Dispose();
        }

        private Task HandleMessageReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            try
            {
                var buffer = arg.ApplicationMessage.Payload;
                var payload = buffer != null && buffer.Length > 0 
                    ? Encoding.UTF8.GetString(buffer)
                    : string.Empty;

                var topic = arg.ApplicationMessage.Topic;

                var point = PointData.Measurement("mqtt_data")
                    .Tag("topic", topic)
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                if (double.TryParse(payload, out double doubleVal))
                {
                    point = point.Field("value", doubleVal);
                }
                else
                {
                    point = point.Field("value_string", payload);
                }

                _writeApi?.WritePoint(point, _config.InfluxBucket, _config.InfluxOrg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _writeApi?.Dispose();
            _influxClient?.Dispose();
            _mqttClient?.Dispose();
        }

        public static async Task<System.Collections.Generic.List<InfluxRecord>> QueryAsync(MqttInfluxConfig config, string range)
        {
            using var client = new InfluxDBClient(config.InfluxUrl, config.InfluxToken);
            var query = $"from(bucket: \"{config.InfluxBucket}\") |> range(start: {range}) |> filter(fn: (r) => r[\"_measurement\"] == \"mqtt_data\") |> pivot(rowKey:[\"_time\"], columnKey: [\"_field\"], valueColumn: \"_value\")";
            
            var tables = await client.GetQueryApi().QueryAsync(query, config.InfluxOrg);
            var records = new System.Collections.Generic.List<InfluxRecord>();

            foreach (var table in tables)
            {
                foreach (var record in table.Records)
                {
                    var rec = new InfluxRecord
                    {
                        Time = record.GetTime().GetValueOrDefault().ToDateTimeUtc(),
                        Topic = record.GetValueByKey("topic")?.ToString() ?? ""
                    };
                    
                    if (record.Values.TryGetValue("value", out var val)) rec.Value = val;
                    else if (record.Values.TryGetValue("value_string", out var valStr)) rec.Value = valStr;

                    records.Add(rec);
                }
            }
            return records;
        }
    }

    public class InfluxRecord
    {
        public DateTime Time { get; set; }
        public string Topic { get; set; } = string.Empty;
        public object? Value { get; set; }
    }
}
