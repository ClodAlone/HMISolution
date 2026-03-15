// See https://aka.ms/new-console-template for more information
using System.Dynamic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

Console.WriteLine("Hello, World!");



// Replace with your inverter's local IP address
string inverterIp = "192.168.10.240";
// Hypothetical endpoint – you may need to experiment with this value.
string url = $"http://{inverterIp}/data";

// Create a JSON payload.
// The structure below is a guess – adjust it as necessary.
var payload = new
{
    command = "getStatus"
    // Add any other required parameters here.
};

// Serialize the payload to JSON
string jsonPayload = JsonSerializer.Serialize(payload);
var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

using HttpClient client = new HttpClient();

// Optional: Add headers if required by the inverter.
// client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

try
{
    HttpResponseMessage response = await client.PostAsync(url, content);

    if (response.IsSuccessStatusCode)
    {
        string responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Response from inverter:");
        Console.WriteLine(responseContent);
    }
    else
    {
        Console.WriteLine($"Error: {response.StatusCode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Exception occurred: {ex.Message}");
}
        
//string url = "http://192.168.10.241/api/getData";
//var values = new FormUrlEncodedContent(new[]
//{
//            new KeyValuePair<string, string>("optType", "ReadRealTimeData"),
//            new KeyValuePair<string, string>("pwd", "SYSNSTDLSL")
//        });

//using (HttpClient client = new HttpClient())
//{
//    HttpResponseMessage response = await client.PostAsync(url, values);
//    string responseString = await response.Content.ReadAsStringAsync();
//    var obj = JsonSerializer.Deserialize<QCellDataRaw>(responseString);

//    Console.WriteLine(responseString);
//}

/*
def response_decoder(cls):
        return {
    "Grid 1 Voltage": (0, Units.V, div10),
            "Grid 2 Voltage": (1, Units.V, div10),
            "Grid 3 Voltage": (2, Units.V, div10),
            "Grid 1 Current": (3, Units.A, twoway_div10),
            "Grid 2 Current": (4, Units.A, twoway_div10),
            "Grid 3 Current": (5, Units.A, twoway_div10),
            "Grid 1 Power": (6, Units.W, to_signed),
            "Grid 2 Power": (7, Units.W, to_signed),
            "Grid 3 Power": (8, Units.W, to_signed),
            "PV1 Voltage": (10, Units.V, div10),
            "PV2 Voltage": (11, Units.V, div10),
            "PV1 Current": (12, Units.A, div10),
            "PV2 Current": (13, Units.A, div10),
            "PV1 Power": (14, Units.W),
            "PV2 Power": (15, Units.W),
            "Grid 1 Frequency": (16, Units.HZ, div100),
            "Grid 2 Frequency": (17, Units.HZ, div100),
            "Grid 3 Frequency": (18, Units.HZ, div100),
            # "Run mode": (19, Units.NONE), # Only use the index once due to HA uids
            "Run mode text": (19, Units.NONE, X3HybridG4._decode_run_mode),
            "EPS 1 Voltage": (23, Units.V, div10),
            "EPS 2 Voltage": (24, Units.V, div10),
            "EPS 3 Voltage": (25, Units.V, div10),
            "EPS 1 Current": (26, Units.A, twoway_div10),
            "EPS 2 Current": (27, Units.A, twoway_div10),
            "EPS 3 Current": (28, Units.A, twoway_div10),
            "EPS 1 Power": (29, Units.W, to_signed),
            "EPS 2 Power": (30, Units.W, to_signed),
            "EPS 3 Power": (31, Units.W, to_signed),
            "Grid Power": (pack_u16(34, 35), Units.W, to_signed32),
            # 'Battery Voltage' is twice in the json response and covered with 169, 170 below.
            # "Battery Voltage": (39, Units.V, div100),
            "Battery Current": (40, Units.A, twoway_div100),
            "Battery Power": (41, Units.W, to_signed),
            "Load/Generator Power": (47, Units.W, to_signed),
            "Radiator Temperature": (54, Units.C, to_signed),
            "Yield total": (pack_u16(68, 69), Total(Units.KWH), div10),
            "Yield today": (70, DailyTotal(Units.KWH), div10),
            "Battery Discharge Energy total": (
                pack_u16(74, 75),
                Total(Units.KWH),
                div10,
            ),
            "Battery Charge Energy total": (pack_u16(76, 77), Total(Units.KWH), div10),
            "Battery Discharge Energy today": (78, DailyTotal(Units.KWH), div10),
            "Battery Charge Energy today": (79, DailyTotal(Units.KWH), div10),
            "PV Energy total": (pack_u16(80, 81), Total(Units.KWH), div10),
            "EPS Energy total": (pack_u16(83, 84), Total(Units.KWH), div10),
            "EPS Energy today": (85, DailyTotal(Units.KWH), div10),
            # Partially reverting b2a9ac5 until last_reset is fixed in
            # HA core - closed PR https://github.com/home-assistant/core/pull/114743
            "Feed-in Energy today": (pack_u16(90, 91), Total(Units.KWH), div100),
            "Consumed Energy today": (pack_u16(92, 93), Total(Units.KWH), div100),
            "Feed-in Energy total": (pack_u16(86, 87), Total(Units.KWH), div100),
            "Consumed Energy total": (pack_u16(88, 89), Total(Units.KWH), div100),
            "Battery Remaining Capacity": (103, Units.PERCENT),
            "Battery Temperature": (105, Units.C, to_signed),
            "Battery Remaining Energy": (
                106,
                Measurement(Units.KWH, storage = True),
                div10,
            ),
            # "Battery mode": (168, Units.NONE), # Only use the index once due to HA uids
            "Battery mode": (168, Units.NONE, X3HybridG4._decode_battery_mode),
            "Battery Voltage": (pack_u16(169, 170), Units.V, div100),
        }

*/






string apiUrl = $"https://qhome-ess-g3.q-cells.eu/proxyApp/proxy/api/getRealtimeInfo.do?tokenId=202501312236513102988410&sn=SR2JUZKAHN";

try
{
    using HttpClient c = new HttpClient();
    HttpResponseMessage response = await c.GetAsync(apiUrl);

    if (response.IsSuccessStatusCode)
    {
        string jsonResponse = await response.Content.ReadAsStringAsync();

        // Deserialize JSON
        // var realtimeInfo = JsonSerializer.Deserialize<Dictionary<String, String>>(jsonResponse);
        var obj = JsonSerializer.Deserialize<QCellRealtimeInfo>(jsonResponse);


        // Process the data
        if (obj != null)
        {
            obj.result.NormalizeValues();
            Console.WriteLine("Data successfully retrieved!");
            // Console.WriteLine($"Example Field: {realtimeInfo.ExampleField}");
            // Replace "ExampleField" with actual fields in your JSON response.
        }
        else
        {
            Console.WriteLine("Failed to parse JSON response.");
        }
    }
    else
    {
        Console.WriteLine($"Error: {response.StatusCode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Exception occurred: {ex.Message}");
}













