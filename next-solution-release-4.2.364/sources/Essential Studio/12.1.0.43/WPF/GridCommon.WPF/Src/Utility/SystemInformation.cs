#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace Syncfusion.Windows.GridCommon
{
    /// <summary>
    /// Wraps native method calls to determine system settings.
    /// </summary>
    public class SystemInformation
    {
        /// <summary>
        /// Gets the double click time.
        /// </summary>
        /// <value>The double click time.</value>
        public static int DoubleClickTime
        {
            get
            {
                return GetDoubleClickTime();
            }
        }

        /// <summary>
        /// Gets the size of the double click area.
        /// </summary>
        /// <value>The size of the double click.</value>
        public static Size DoubleClickSize
        {
            get
            {
                return new Size(GetSystemMetrics(0x24), GetSystemMetrics(0x25));
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern int GetDoubleClickTime();

    }

}
