using ByteDev.Sonos;
using ByteDev.Sonos.Models;
using IronPython.Hosting;
using Jishi.Intel.SonosUPnP;
using LangChain.Providers.OpenAI;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using tryAGI.OpenAI;
using LangChain;

namespace WindowsFormsApp1
{
    internal static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            const string apiKey = "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";

            uploadjsontoopenai(@"c:\temp\agenda.json", apiKey).Wait();
            // PromptAI(apiKey, "cosa posso chiederti").Wait();
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            createLocalTTS();
            // playMp3OnSonos();

            Application.Run(new Form1());
        }

        //destID: destination ID
        //text: text to send


        static async void sendWAText()
        {
            var client = new HttpClient();
            var url = "https://graph.facebook.com/v17.0/133661936503071/messages?access_token=EAAE5TRNty3kBOwW3n84qGXAq44TWnI0tI2emttHB2mb9WRwkRpbVYHQGo6gY9ByDsn2fU3YgyNdU9nzu6XXl0SOatr30T2HeFLgDZApeXIZCF36h8NIXaZARX3EhnpXRye53FEAVHXQxZCNOOihfJ6ZBFX7EyRthl0s8MMVTASQrpUYzW6Ei8xC2bH5W1lH1NwTO1nFLJbgUKOlaSwv8ZD";
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

        static void createLocalTTS()
        {
            // Create a new SpeechSynthesizer instance
            SpeechSynthesizer synth = new SpeechSynthesizer();

        // Set the voice to use
            synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, new System.Globalization.CultureInfo("IT-it"));

            // Set the output format
            synth.SetOutputToWaveFile("output.wav");

            // Speak the text
            synth.Speak("ciao belli, parlate italiano ?");

            // Close the synthesizer
            synth.Dispose();
        }

        static void playOpenAI()
        {
 
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope ret = engine.ExecuteFile(@"C:\temp\chatgpt-retrieval-main\chatgpt.py");

            dynamic var;
            ret.TryGetVariable("var", out var);
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

        async static Task<int> uploadjsontoopenai(string filePath, String apiKey)
        {
            var url = "https://api.openai.com/v1/files";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var fileContent = File.ReadAllText(filePath);
                var response = await client.PostAsync(url, new StringContent(fileContent, Encoding.UTF8, "application/json"));


//                 var response = await client.PostAsync(url, fileContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responseJson);

                    var fileId = responseObject.id;

                    Console.WriteLine("File ID: " + fileId);

                    return fileId;
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                }
            }

            return -1;
        }
    

        static async Task<String> PromptAI(String apiKey, String prompt)
        {
            var url = "https://api.openai.com/v1/chat/completions";

            var fileIds = new List<string> { "FILE_ID_1", "FILE_ID_2", "FILE_ID_3" };
            // var prompt = "What's on my agenda today?";

            var message = new
            {
                role = "user",
                content = prompt
            };
            var messages = new[]
            {
                message
            };


            var requestBody = new
            {
                messages = messages,
                model = "gpt-3.5-turbo",
                max_tokens = 256,
                n = 1,
                stop = "\n",
                // file = fileIds,
                temperature = 0.5,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responseJson);

                    var generatedText = responseObject.choices[0].message;

                    Console.WriteLine(generatedText);

                    return generatedText;
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);

                    return "Error: " + response.StatusCode;
                }
            }
        }
    
    


        async static Task playMp3OnSonos()
        {
            // var discovery = new ByteDev.Sonos.Upnp.


            SonosController controller = new SonosControllerFactory().Create("192.168.1.131");
            SonosVolume volume = await controller.GetVolumeAsync();
            await controller.SetVolumeAsync(new SonosVolume(70));

            var queue = await controller.GetQueueAsync();

            await controller.StopAsync();
            await controller.ClearQueueAsync();
            // queue = await controller.GetQueueAsync();

            var ret = await controller.AddQueueTrackAsync("http://192.168.1.125/Music/mp3/annouce.mp3", 1, true);
            // await controller.RestartQueueAsync();
            // queue = await controller.GetQueueAsync();


            await controller.SeekTrackAsync(new SonosTrackNumber(ret.NumTracksAdded));
            await controller.PlayAsync();
            // await controller.RemoveQueueTrackAsync(ret.NumTracksAdded);


            volume.Increase(10);

            await controller.SetVolumeAsync(volume);

            // await controller.PauseAsync();
        }

    }
}
