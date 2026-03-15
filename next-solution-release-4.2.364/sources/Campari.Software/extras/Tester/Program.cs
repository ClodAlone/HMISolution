using System;
using System.Collections.Generic;
using System.Text;

using Campari.Software;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            bool iis4Installed = 
                InternetInformationServicesDetection.IsInstalled(InternetInformationServicesVersion.IIS4);

            bool iis5Installed =
                InternetInformationServicesDetection.IsInstalled(InternetInformationServicesVersion.IIS5);

            bool iis51Installed =
                InternetInformationServicesDetection.IsInstalled(InternetInformationServicesVersion.IIS51);

            bool iis6Installed =
                InternetInformationServicesDetection.IsInstalled(InternetInformationServicesVersion.IIS6);

            bool iis7Installed =
                InternetInformationServicesDetection.IsInstalled(InternetInformationServicesVersion.IIS7);

            bool iis75Installed =
                InternetInformationServicesDetection.IsInstalled(InternetInformationServicesVersion.IIS75);

            bool iis8Installed =
                InternetInformationServicesDetection.IsInstalled(InternetInformationServicesVersion.IIS8);

            bool iis85Installed =
                InternetInformationServicesDetection.IsInstalled(InternetInformationServicesVersion.IIS85);
            
            Console.WriteLine("IIS 4 installed? {0}", iis4Installed);
            Console.WriteLine("IIS 5 installed? {0}", iis5Installed);
            Console.WriteLine("IIS 5.1 installed? {0}", iis51Installed);
            Console.WriteLine("IIS 6 installed? {0}", iis6Installed);
            Console.WriteLine("IIS 7 installed? {0}", iis7Installed);
            Console.WriteLine("IIS 7.5 installed? {0}", iis75Installed);
            Console.WriteLine("IIS 8 installed? {0}", iis8Installed);
            Console.WriteLine("IIS 8.5 installed? {0}", iis85Installed);

            bool iis4OrHigherInstalled =
                InternetInformationServicesDetection.IsEqualOrHigherInstalled(InternetInformationServicesVersion.IIS4);

            bool iis5OrHigherInstalled =
                InternetInformationServicesDetection.IsEqualOrHigherInstalled(InternetInformationServicesVersion.IIS5);

            bool iis51OrHigherInstalled =
                InternetInformationServicesDetection.IsEqualOrHigherInstalled(InternetInformationServicesVersion.IIS51);

            bool iis6OrHigherInstalled =
                InternetInformationServicesDetection.IsEqualOrHigherInstalled(InternetInformationServicesVersion.IIS6);

            bool iis7OrHigherInstalled =
                InternetInformationServicesDetection.IsEqualOrHigherInstalled(InternetInformationServicesVersion.IIS7);

            bool iis75OrHigherInstalled =
                InternetInformationServicesDetection.IsEqualOrHigherInstalled(InternetInformationServicesVersion.IIS75);

            bool iis8OrHigherInstalled =
                InternetInformationServicesDetection.IsEqualOrHigherInstalled(InternetInformationServicesVersion.IIS8);

            bool iis85OrHigherInstalled =
                InternetInformationServicesDetection.IsEqualOrHigherInstalled(InternetInformationServicesVersion.IIS85);

            Console.WriteLine("IIS 4 equal or higher installed? {0}", iis4OrHigherInstalled);
            Console.WriteLine("IIS 5 equal or higher  installed? {0}", iis5OrHigherInstalled);
            Console.WriteLine("IIS 5.1 equal or higher  installed? {0}", iis51OrHigherInstalled);
            Console.WriteLine("IIS 6 equal or higher  installed? {0}", iis6OrHigherInstalled);
            Console.WriteLine("IIS 7 equal or higher  installed? {0}", iis7OrHigherInstalled);
            Console.WriteLine("IIS 7.5 equal or higher  installed? {0}", iis75OrHigherInstalled);
            Console.WriteLine("IIS 8 equal or higher  installed? {0}", iis8OrHigherInstalled);
            Console.WriteLine("IIS 8.5 equal or higher  installed? {0}", iis85OrHigherInstalled);

            if (iis4Installed || iis5Installed || iis51Installed || iis6Installed || iis7Installed || iis75Installed || iis8Installed || iis85Installed)
            {
                Console.WriteLine("ASP Registered? {0}",
                    InternetInformationServicesDetection.IsAspRegistered());

                Console.WriteLine("ASP.NET 1.0 Registered? {0}",
                    InternetInformationServicesDetection.IsAspNetRegistered(FrameworkVersion.Fx10));

                Console.WriteLine("ASP.NET 1.1 Registered? {0}",
                    InternetInformationServicesDetection.IsAspNetRegistered(FrameworkVersion.Fx11));

                Console.WriteLine("ASP.NET 2.0 Registered? {0}",
                    InternetInformationServicesDetection.IsAspNetRegistered(FrameworkVersion.Fx20));

                // These really don't exist, they are actually the .NET 2.0 version of ASP.NET.
                Console.WriteLine("ASP.NET 3.0 Registered? {0}",
                    InternetInformationServicesDetection.IsAspNetRegistered(FrameworkVersion.Fx30));

                Console.WriteLine("ASP.NET 3.5 Registered? {0}",
                    InternetInformationServicesDetection.IsAspNetRegistered(FrameworkVersion.Fx35));

                Console.WriteLine("ASP.NET 4.0 Registered? {0}",
                    InternetInformationServicesDetection.IsAspNetRegistered(FrameworkVersion.Fx45));


                Console.WriteLine("World Wide Web (WWW) service? {0}",
                    InternetInformationServicesDetection.IsInstalled(InternetInformationServicesComponent.WWW));

            }

            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
