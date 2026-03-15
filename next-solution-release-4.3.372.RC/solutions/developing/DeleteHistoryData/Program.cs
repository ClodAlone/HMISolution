using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteHistoryData
{
    class Program
    {
        #region Declarations
        static CommandLineOptions cl;
        #endregion

        #region Entry Point
        static void Main(string[] args)
        {
#if !NET_CORE
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
#if DEBUG
            //if (!System.Diagnostics.Debugger.IsAttached &&
            //    Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
            //        "DebugMe - DeleteHistoryData", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            //    System.Diagnostics.Debugger.Launch();
#endif
#else
#if DEBUG
            //Console.WriteLine("if you would like to attach a debugger now is the right moment !");
            //Console.WriteLine("Press enter to continue...");
            //Console.ReadLine();
#endif
#endif

            if (args.Length > 0)
            {
                cl = new CommandLineOptions(args);
                if (!cl.IsValid)
                {
                    Console.WriteLine(Properties.Resources.InvalidOptions);
                    return;
                }

                if (cl.CallingProcessId > 0)
                {
                    try
                    {
                        var process = Process.GetProcessById(cl.CallingProcessId);
                        if (process != null)
                        {
                            process.EnableRaisingEvents = true;
                            process.Exited += (o, e) =>
                            {
                                Environment.Exit(0);
                            };
                        }
                    }
                    catch { }
                }

                DeleteHistoryData();
            }
        }
#endregion

#region Methods
        static void DeleteHistoryData()
        {
            if (cl.SchemaType == SchemaType.Historian)
            {
                if(cl.ResetTime != DateTime.MinValue)
                    cl.MaxAge = DateTime.UtcNow - cl.ResetTime;
                var job = new DeleteHistorianJob(cl.Connection, cl.MaxAge, cl.MaxTake, cl.NodeId);
                job.Execute();
            }
            else if (cl.SchemaType == SchemaType.EventLogger)
            {
                var job = new DeleteEventLogJob(cl.Connection, cl.MaxAge, cl.MaxTake);
                job.Execute();
            }
        }
#endregion
    }
}
