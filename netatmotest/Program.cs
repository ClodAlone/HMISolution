// See https://aka.ms/new-console-template for more information
using Flurl.Http;
using Netatmo;
using Netatmo.Models.Client.Energy.RoomMeasure;
using Netatmo.Models.Client.Energy;
using NodaTime;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using Netatmo.Models.Client.Air.HomesCoachs;
using Netatmo.Models.Client;
using Netatmo.Models;

Console.WriteLine("Hello, World!");





        // Replace with your credentials
        string refreshToken = "674eb43d31c192c2ea0d7720|90dd45f35d7396f01c8ac1fece87d400";
        string clientId = "674ebac5c93db239a203ef83";
        string clientSecret = "dfwmK8NHHY7eCcX3KnelgcS6BXBp";
        string tokenUrl = "https://api.netatmo.com/oauth2/token";

        try
        {
            using (HttpClient c = new HttpClient())
            {
                // Prepare request data
                var postData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                });

                // Send POST request
                HttpResponseMessage response = await c.PostAsync(tokenUrl, postData);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Parse JSON response
                    var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

                    if (tokenResponse != null)
                    {
                        Console.WriteLine("Token refreshed successfully!");
                        Console.WriteLine($"Access Token: {tokenResponse["access_token"]}");
                        Console.WriteLine($"Refresh Token: {tokenResponse["refresh_token"]}");
                        Console.WriteLine($"Expires In: {tokenResponse["expires_in"]} seconds");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to refresh token. Status: {response.StatusCode}");
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {errorContent}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }






var jsonSerializerOptions = Configuration.JsonSerializerOptions;
jsonSerializerOptions.WriteIndented = true;


var client = new Client(
    SystemClock.Instance,
    "https://api.netatmo.com/","674ebac5c93db239a203ef83", "dfwmK8NHHY7eCcX3KnelgcS6BXBp");


Token token = null;
var tokenFile = "token.json";
if (File.Exists(tokenFile))
{
    token = JsonSerializer.Deserialize<Token>(File.ReadAllText(tokenFile));
    client.ProvideOAuth2Token(token.AccessToken, token.RefreshToken);

}
else
    client.ProvideOAuth2Token("674eb43d31c192c2ea0d7720|7ee79cabc7d717015eae011cef693f3f", "674eb43d31c192c2ea0d7720|c16b2877c71fa6474d569fbbbcbc7331");

// workaround here 
//if (token == null)
//    token = new Token(10800, null,
//        "674eb43d31c192c2ea0d7720|96bbbe5807e5a70c93be2fe2f9e3cff0");

//var credentialToken = new Netatmo.Models.CredentialToken(token, NodaTime.SystemClock.Instance);
////CredentialToken is read-only, using reflection to set it
//typeof(CredentialManager).GetProperty("CredentialToken").SetValue(client.CredentialManager, credentialToken, null);



var credentialtoken = client.CredentialManager.CredentialToken;

do
{ 
if (client.CredentialManager.CredentialToken?.ExpiresAt.ToDateTimeUtc().ToLocalTime() < DateTime.Now
    || client.CredentialManager.CredentialToken?.AccessToken == null)
    await client.RefreshToken();

File.WriteAllText(tokenFile, JsonSerializer.Serialize<Token>(new Token(10280, credentialtoken.AccessToken, credentialtoken.RefreshToken)));

Console.WriteLine($"Token : {credentialtoken.AccessToken}");

Console.WriteLine("HomeCoach data :");
var homeCoach = await client.Air.GetHomeCoachsData();
foreach(var device in homeCoach.Body.Devices)
{
    Console.WriteLine(string.Format("Device Name : {0}", device.Name));

    Console.WriteLine(string.Format("Device Name : {0}", device.DashboardData));

    var dataProperties = typeof(DashBoardData).GetProperties(BindingFlags.Instance | BindingFlags.Public);

    foreach (var property in dataProperties)
    {
        var value = property.GetValue(device.DashboardData);
    }

}

//Console.WriteLine(JsonSerializer.Serialize(homeCoach, jsonSerializerOptions));

//Console.WriteLine("Stations data :");
//var stationsData = await client.Weather.GetStationsData();
//Console.WriteLine(JsonSerializer.Serialize(stationsData, jsonSerializerOptions));

//Console.WriteLine("Energy Homes data :");
//var homesData = await client.Energy.GetHomesData();
//Console.WriteLine(JsonSerializer.Serialize(homesData, jsonSerializerOptions));

//Console.WriteLine("Energy Homes data :");
//    foreach (var home in homesData.Body.Homes)
//    {
//        Console.WriteLine(home.Name);
//        // var homeStatus = await client.Energy.GetHomeStatus(home.Id);
//        // Console.WriteLine(JsonSerializer.Serialize(homeStatus, jsonSerializerOptions));

//        //Console.WriteLine("Energy room measure :");
//        //foreach (var room in home.Rooms)
//        //{
//        //    if (room.ModuleIds == null || room.ModuleIds.Length == 0)
//        //    {
//        //        continue;
//        //    }

//        //    Console.WriteLine(room.Name);
//        //    var parameters = new GetRoomMeasureParameters
//        //    {
//        //        HomeId = home.Id,
//        //        RoomId = room.Id,
//        //        Scale = Scale.Max,
//        //        Type = ThermostatMeasurementType.Temperature,
//        //        BeginAt = SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromDays(-1)),
//        //        EndAt = SystemClock.Instance.GetCurrentInstant()
//        //    };

//        //    try
//        //    {
//        //        var roomMeasure = await client.Energy.GetRoomMeasure<TemperatureStep>(parameters);
//        //        Console.WriteLine(JsonSerializer.Serialize(roomMeasure, jsonSerializerOptions));
//        //    }
//        //    catch (FlurlHttpException exception)
//        //    {
//        //        var error = await exception.GetResponseStringAsync();
//        //        Console.WriteLine($"exception : {error}");
//        //    }
//        // }
//    }

    Thread.Sleep(60000);
} while (true);
