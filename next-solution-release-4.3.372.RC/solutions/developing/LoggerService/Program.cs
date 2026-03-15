using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace LoggerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(params String[] args)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();

            LoggerService ServicesToRun = new LoggerService();

            if (new List<String>(args).Contains("-noservice"))
            {
                ServicesToRun.StartService(args);
                Console.WriteLine("Press any keys to stop the service");
                Console.ReadLine();
                ServicesToRun.StopService();
            }
            else
                ServiceBase.Run(ServicesToRun);
        }
    }
}
