using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegistryWriter
{
    class Program
    {
        static string argspattern = "/\\s*(\".+?\"|[^:\\s])+((\\s*:\\s*(\".+?\"|[^\\s])+)|)|(\".+?\"|[^\"\\s])+";
        static void Main(string[] args)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

            if (args.Length > 0)
            {
                CommandLineOptions cl = null;
                if (args.Length == 1)
                {
                    string decryptedargs = null;
                    try
                    {
                        decryptedargs = WPFUtilities.CryptString.CryptString.DecryptString(args[0]);
                    }
                    catch
                    { }

                    if (decryptedargs != null)
                    {
                        var matches = Regex.Matches(decryptedargs.Replace("\"", ""), argspattern);
                        if (matches.Count > 0)
                        {
                            var arguments = new List<string>(matches.Count);
                            for (int ii = 0; ii < matches.Count; ii++)
                                arguments.Add(matches[ii].ToString());
                            cl = new CommandLineOptions(arguments.ToArray());
                        }
                    }
                    else
                        cl = new CommandLineOptions(args);
                }
                else
                    cl = new CommandLineOptions(args);

                if (cl.IsValid)
                {
#if !DEBUG
                    if (!hasAdministrativeRight && cl.RegistryHive != Microsoft.Win32.RegistryHive.CurrentUser)
                    {
                        RunElevated(System.Windows.Forms.Application.ExecutablePath);
                        return;
                    }
#endif
                    try
                    {
                        if (cl.CommandType == CommandType.RegistryWriteValue)
                        {
                            using (var registry = new RegistryWriteValue(cl))
                            {
                                registry.Execute();
                            }
                        }
                        else if (cl.CommandType == CommandType.RegistryUpdateValue)
                        {
                            using (var registry = new RegistryUpdateValue(cl))
                            {
                                registry.Execute();
                            }
                        }
                        else if (cl.CommandType == CommandType.RegistryDeleteValue)
                        {
                            using (var registry = new RegistryDeleteValue(cl))
                            {
                                registry.Execute();
                            }
                        }
                        else if (cl.CommandType == CommandType.RegistryDeleteKey)
                        {
                            using (var registry = new RegistryDeleteKey(cl))
                            {
                                registry.Execute();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine(Properties.Resources.InvalidOptions);
                }

            }
            else
                Console.WriteLine(Properties.Resources.InvalidOptions);
        }

        static bool RunElevated(string fileName)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                Verb = "runas",
                FileName = fileName,
                Arguments = Environment.CommandLine
            };

            try
            {
                Process.Start(processInfo);
                return true;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                //Do nothing. Probably the user canceled the UAC window
            }
            return false;
        }
    }
}
