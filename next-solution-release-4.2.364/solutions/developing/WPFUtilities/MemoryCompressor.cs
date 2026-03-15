using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WPFUtilities
{
    public static class MemoryCompressor
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);
        static DispatcherOperation mimMemoryOper;
        public static void minimizeMemory(bool bUseDispatcher = true, bool bSynchro = false)
        {
            Action action = () =>
            {
                if (!bSynchro)
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        MinimizeCore();
                    });
                }
                else
                    MinimizeCore();
            };
            if (bUseDispatcher && Dispatcher.CurrentDispatcher != null)
            {
                if (mimMemoryOper == null)
                {
                    mimMemoryOper = Dispatcher.CurrentDispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
                    mimMemoryOper.Completed += (o, e) =>
                    {
                        mimMemoryOper = null;
                    };
                }
            }
            else
                action();
        }

        static void MinimizeCore()
        {
            try
            {
                GC.Collect();
                // GC.Collect(GC.MaxGeneration);
                // GC.WaitForPendingFinalizers();
                //unchecked
                //{
                //    SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)(-1), (UIntPtr)(-1));
                //}
            }
            catch
            {
                //SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
    }
}
