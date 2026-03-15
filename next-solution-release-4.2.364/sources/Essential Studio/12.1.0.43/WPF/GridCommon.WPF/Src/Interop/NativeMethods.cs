#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Internal
{
    class NativeMethods
    {
        // SystemInformation class.
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetDoubleClickTime();

        // MeasureTime class.
        [DllImport("Kernel32.dll")]
        public static extern int QueryPerformanceFrequency(ref Int64 lpFrequency);

        [DllImport("Kernel32.dll")]
        public static extern int QueryPerformanceCounter(ref Int64 lpPerformanceCount);
    }
}
