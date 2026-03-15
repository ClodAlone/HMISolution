using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

//#if DEBUG
//            if (!System.Diagnostics.Debugger.IsAttached &&
//                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
//                    String.Format("DebugMe - {0}", Assembly.GetExecutingAssembly().GetName().Name), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
//                System.Diagnostics.Debugger.Launch();
//#endif

            string[] serverargs = new string[args.Length + 1];
            args.CopyTo(serverargs, 0);
            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(args);
            if (commandArgs.ArgPairs.ContainsKey("instanceId"))
            {
                var instanceId = commandArgs.ArgPairs["instanceId"];
                System.Windows.Forms.Application.Run(new SysTrayApp(instanceId));
            }
        }
    }
}
