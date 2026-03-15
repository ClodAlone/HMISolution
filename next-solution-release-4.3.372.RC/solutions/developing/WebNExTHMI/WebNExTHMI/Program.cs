using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebNExTHMI
{
    public class Program
    {
        static IConfigurationRoot configuration;
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var configFile = "appsettings.json";
            var argsDict = ResolveArguments(args);
            if (argsDict.ContainsKey("configFile"))
            {
                var cf = argsDict["configFile"].Trim();
                if (!String.IsNullOrEmpty(cf) && File.Exists(cf))
                    configFile = cf;
            }
            Console.WriteLine("Loaded Configuration File: " + configFile);
            configuration = new ConfigurationBuilder()
                .AddJsonFile(configFile, optional: false)
                .Build();

            //CreateHostBuilder(args)
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    //config.Sources.Clear();
                    config.AddConfiguration(configuration);
                    //if (args != null)
                    //{
                    //    config.AddCommandLine(args);
                    //}
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    string usedUrls = "";
                    if (configuration["WebNExTHMISettings:HttpListeningPort"] != "0")
                        if (configuration.GetSection("Kestrel") == null || configuration["WebNExTHMISettings:HttpListeningPort"] != null)
                            usedUrls = String.Format("http://*:{0};", configuration["WebNExTHMISettings:HttpListeningPort"] ?? "5000");
                    if (configuration["WebNExTHMISettings:HttpsListeningPort"] != "0")
                        if (configuration.GetSection("Kestrel") == null || configuration["WebNExTHMISettings:HttpsListeningPort"] != null)
                            usedUrls = String.Format("{0}https://*:{1}", usedUrls, configuration["WebNExTHMISettings:HttpsListeningPort"] ?? "5001");
                    if (!string.IsNullOrEmpty(usedUrls))
                        webBuilder.UseUrls(usedUrls);
                })
                .UseWindowsService()
                .Build()
                .Run();
        }

        private static Dictionary<string, string> ResolveArguments(string[] args)
        {
            var arguments = new Dictionary<string, string>();
            if (args != null && args.Length > 0)
            {
                foreach (string argument in args)
                {
                    int idx = argument.IndexOf('=');
                    if (idx > 0)
                        arguments[argument.Substring(0, idx)] = argument.Substring(idx + 1);
                }
            }
            return arguments;
        }
    }
}
