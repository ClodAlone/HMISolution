using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Utilities
{
    public static class RunElevated
    {
        public static void Run(String processName, String arguments)
        {
            using (var winProcess = new Process())
            {
                //// winProcess.StartInfo.StandardOutputEncoding = Encoding.Default;
                //winProcess.StartInfo.CreateNoWindow = false;
                //winProcess.StartInfo.UseShellExecute = false;
                //winProcess.StartInfo.ErrorDialog = true;
                //// False because we want to redirect output
                //// winProcess.StartInfo.Verb = "runas";
                //winProcess.StartInfo.RedirectStandardInput = false;
                //// If UseShellExecute is set to true I can't read or write to the process.
                //winProcess.StartInfo.RedirectStandardOutput = false;
                //winProcess.StartInfo.RedirectStandardError = false;
                //winProcess.StartInfo.FileName = processName;
                //winProcess.StartInfo.Arguments = arguments;
                ////winProcess.OutputDataReceived += (o, e) =>
                ////{
                ////    Debug.WriteLine(e.Data);
                ////    //textBlock.Dispatcher.InvokeIfRequired(() =>
                ////    //{
                ////    //    textBlock.FontSize = 12;
                ////    //    textBlock.Text = String.Format("{0}{1}\n", textBlock.Text, e.Data);
                ////    //    scroll.ScrollToBottom();
                ////    //});
                ////};

                winProcess.StartInfo.FileName = processName;
                winProcess.StartInfo.Arguments = arguments;
                winProcess.StartInfo.Verb = "runas";
                winProcess.StartInfo.UseShellExecute = true;
                winProcess.Start();
                winProcess.WaitForExit();
            }
            // winProcess.BeginOutputReadLine();
        }


        public static Process RunNoWait(String processName, String arguments)
        {
            var winProcess = new Process();
            winProcess.StartInfo.FileName = processName;
            winProcess.StartInfo.Arguments = arguments;
            winProcess.StartInfo.Verb = "runas";
            winProcess.StartInfo.UseShellExecute = true;
            winProcess.Start();
            return winProcess;
        }
    }
}
