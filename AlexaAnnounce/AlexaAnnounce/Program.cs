
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using System.Media;
using NAudio.Wave;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Jishi.Intel.SonosUPnP;
using ByteDev.Sonos.Models;
using ByteDev.Sonos;
using System.Net.Http.Headers;
using Telegram.Bot;
using System.Net.Mail;
using System.Net;
using WeatherAPI_CSharp;
using System.Reflection;
using HardwareInformation;
using Hardware.Info;

public class SynthesizeSpeechMarks
{
    public static async Task Main()
    {
        IHardwareInfo hardwareInfo = new HardwareInfo();
        hardwareInfo.RefreshAll();

        // var info = MachineInformationGatherer.GatherInformation();

        // GetWeather().Wait();
        // playMp3OnSonos();
        return;

        var region = RegionEndpoint.USWest2;
        var accessKey = "AKIARBQH5QPS4C2JB35Q";
        var secretKey = "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";
        var credentials = new BasicAWSCredentials(accessKey, secretKey);
        var pollyClient = new AmazonPollyClient(credentials, region);

        var request = new SynthesizeSpeechRequest
        {
            Text = "Alexa announce to Echo that this is a sample text to be synthesized by Amazon Polly.",
            VoiceId = VoiceId.Joanna,
            //Text = "Ciao, grazie di provare ad usare il servizio Polly.",
            //VoiceId = VoiceId.Bianca,
            OutputFormat = OutputFormat.Mp3
        };

        var response = await pollyClient.SynthesizeSpeechAsync(request);

        using (var fileStream = File.Create("sample.mp3"))
        {
            response.AudioStream.CopyTo(fileStream);
        }

        var reader = new Mp3FileReader("sample.mp3");
        var waveOut = new WaveOutEvent();
        waveOut.Init(reader);
        waveOut.Play();

        Console.ReadKey();
        Console.WriteLine("Speech synthesized successfully!");


        string accessToken = "2:P1C5_sfaofbIsPSkGa3NtbFQMQf_tUnA5bK5dnkTTTB4UU4k1_OR-VLyYLgdhi2Sj1gyse0fZqfyuisKuf_cnw==:5KiCkWcpKDnvhSTzHLIy8Q=="; // Replace with your access token
        string deviceId = "EchoDot di Claudio"; // Replace with your device ID
        string announcementText = "This is an emergency announcement."; // Replace with your announcement text

        await SendAnnouncement(accessToken, deviceId, announcementText);    
    }

    static async Task uploadpdf(String file, String apiKey)
    {
        var pdfPath = "path/to/your/file.pdf";
        var pdfBytes = File.ReadAllBytes(pdfPath);
        var pdfBase64 = Convert.ToBase64String(pdfBytes);

        var client = new LangChainClient(apiKey);
        var loader = new UnstructuredFileLoader(pdfBase64, "pdf");
        var documents = await loader.LoadAsync();

        foreach (var document in documents)
        {
            var splitter = new CharacterTextSplitter(document);
            var chunks = splitter.Split();

            foreach (var chunk in chunks)
            {
                var embeddings = new OpenAIEmbeddings(apiKey);
                var context = await embeddings.GetEmbeddingAsync(chunk);

                var vectorStore = new Chroma(apiKey);
                var vector = await vectorStore.GetVectorAsync(context);

                var chain = new RetrievalQA(apiKey);
                var response = await chain.GetAnswerAsync(vector);

                Console.WriteLine(response);
            }
        }
    }

    static async Task GetWeather()
    {
        var properties = typeof(Forecast).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        var client = new APIClient("00f68ba415234e6d8e9100308231910");

        var weather = await client.GetWeatherForecastDailyAsync("44.557778,10.975278");

        // Forecast
        // ForecastDaily
        weather = weather;
    }

    static internal void sendMailText()
    {
        SmtpClient smtpClient = new SmtpClient();
        smtpClient.Host = "smtp.gmail.com";
        smtpClient.Port = 587;
        smtpClient.Credentials = new NetworkCredential("clodprogea@gmail.com", "bqxh sqnt qinr tbsm");
        smtpClient.EnableSsl = true;

        MailMessage message = new MailMessage();
        message.To.Add("clodprogea@gmail.com");
        message.Subject = "Password Manager Sync Account Created";
        message.From = new MailAddress("clodprogea@gmail.com");
        message.Body = "My Email message";
        smtpClient.Send(message);

    }

    static public async Task sendMessage(string destID, string text)
    {
        try
        {
            var bot = new Telegram.Bot.TelegramBotClient("6576727908:AAGloWf2uTq5NVRI5WbR9hpInLtOegRVPVk");

            // Example 1: Retrieving the latest update and extracting the chat ID
            var updates = await bot.GetUpdatesAsync();

            await bot.SendTextMessageAsync(destID, text);
        }
        catch (Exception e)
        {
            Console.WriteLine("err");
        }
    }

    async static void playMp3OnSonos()
    {
        SonosController controller = new SonosControllerFactory().Create("192.168.1.131");

        SonosVolume volume = await controller.GetVolumeAsync();

        volume.Increase(10);

        await controller.SetVolumeAsync(volume);
    }




    static async Task sendWAText()
    {
        var client = new System.Net.Http.HttpClient();

        client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "EAAE5TRNty3kBOwW3n84qGXAq44TWnI0tI2emttHB2mb9WRwkRpbVYHQGo6gY9ByDsn2fU3YgyNdU9nzu6XXl0SOatr30T2HeFLgDZApeXIZCF36h8NIXaZARX3EhnpXRye53FEAVHXQxZCNOOihfJ6ZBFX7EyRthl0s8MMVTASQrpUYzW6Ei8xC2bH5W1lH1NwTO1nFLJbgUKOlaSwv8ZD");

        var url = "https://graph.facebook.com/v17.0/133661936503071/messages";
        var message = new
        {
            messaging_product = "whatsapp",
            to = "393474183850",
            type = "template",
            template = new
            {
                name = "hello_world",
                language = new
                {
                    code = "en_US"
                }
            }
        };
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(message);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);
    }

    void receiveWAText()
    {
        var accountSid = "ACe077ed3316ee7ca86fe6359f12aa23aa";
        var authToken = "[AuthToken]";
        TwilioClient.Init(accountSid, authToken);

        var messageOptions = new CreateMessageOptions(
          new PhoneNumber("whatsapp:+393474183850"));
        messageOptions.From = new PhoneNumber("whatsapp:+14155238886");


        var message = MessageResource.Create(messageOptions);
        Console.WriteLine(message.Body);
    }

    public static async Task SendAnnouncement(string accessToken, string deviceId, string announcementText)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            string url = $"https://alexa-api.amazon.com/v1/devices/{deviceId}/settings/notifications";
            string json = $"{{ \"alarm\": {{ \"asset\": {{ \"id\": \"system_alerts_siren_01\" }}, \"behavior\": {{ \"deviceActions\": [{{ \"type\": \"ANNOUNCE\", \"textToAnnounce\": \"{announcementText}\" }}] }} }} }}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Announcement sent successfully.");
            }
            else
            {
                Console.WriteLine("Failed to send announcement.");
            }
        }
    }

}



/*
 *Crea un account di sviluppatore AVS: Per iniziare, dovrai creare un account di sviluppatore AVS su https://developer.amazon.com/alexa/console/avs/home. Segui le istruzioni per registrarti come sviluppatore AVS.
Registra il tuo dispositivo: Dopo aver creato l'account di sviluppatore AVS, dovrai registrare il tuo dispositivo per ottenere le credenziali di autenticazione necessarie per l'accesso all'API di AVS. Segui le istruzioni fornite nella documentazione ufficiale di Amazon per registrare il tuo dispositivo
1
.
Configura l'accesso all'API di AVS: Utilizzando le credenziali ottenute durante la registrazione del dispositivo, configura l'accesso all'API di AVS nel tuo progetto C#. Puoi utilizzare librerie come AVS.NET (https://github.com/kevbite/AVS.NET) per semplificare l'interazione con l'API di AVS in C#.
Sviluppa la logica per l'annuncio: Utilizzando l'API di AVS, puoi sviluppare la logica per l'annuncio in remoto. Puoi utilizzare le funzionalità di sintesi vocale per convertire il testo dell'annuncio in un file audio che verrà riprodotto sul dispositivo Echo.
Programma l'invio dell'annuncio: Utilizzando il tuo codice C#, puoi programmare l'invio dell'annuncio in remoto. Puoi definire il momento in cui l'annuncio deve essere inviato e il testo che deve essere pronunciato.
Esegui il codice: Dopo aver programmato l'annuncio, esegui il tuo codice C# per inviare l'annuncio al dispositivo Echo in remoto.
È importante notare che l'utilizzo dell'API di AVS richiede una corretta configurazione e l'adesione alle linee guida di Amazon. Assicurati di rispettare le politiche di utilizzo e di utilizzare l'API di AVS in conformità con le leggi e le normative locali.
1
 https://developer.amazon.com/docs/alexa-voice-service/register-a-product.html
*/
