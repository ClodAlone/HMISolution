using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartUpdater
{
    internal class Program
    {
        static void Main(string[] args)
        {

#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                    "DebugMe - StartupUpdater", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif

            string path = Properties.Settings.Default.ProjectUpdaterExe;
            System.Reflection.Assembly callingMainAssembly = System.Reflection.Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

            bool error = false;
            if (File.Exists(path))
            {
                //var args = string.Format(Properties.Settings.Default.ProjectUpdaterArg, doc.FilePath);
                StringBuilder sb = new StringBuilder();
                
                foreach (string line in args)
                    sb.Append(string.Format("\"{0} \"", line));

                var startInfo = new System.Diagnostics.ProcessStartInfo(path, sb.ToString())
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new System.Diagnostics.Process())
                {
                    process.StartInfo = startInfo;
                    process.ErrorDataReceived += (o, e) =>
                    {
                        if (!String.IsNullOrEmpty(e.Data))
                        {
                            if (!String.IsNullOrEmpty(e.Data))
                            {
                                Console.Error.WriteLine(e.Data);//redirect to standarderror stream
                                //log.Error(e.Data);
                                error = true;
                            }
                        }
                    };

                    process.Start();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                }
                //doc.SaveToFile();
            }
            else
            {
                //redirect to standarderror stream
                Console.Error.WriteLine(string.Format(Properties.Resources.CannotOpen, path));
                //log.Error(string.Format(Properties.Resources.CannotOpen, path));
                error = true;
            }

        }
    }
}
