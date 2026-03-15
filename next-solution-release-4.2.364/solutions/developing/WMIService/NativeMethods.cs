using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WMIService
{
    internal class NativeMethods
    {
        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        internal static extern Int32 GetCurrentWin32ThreadId();
    }
}
