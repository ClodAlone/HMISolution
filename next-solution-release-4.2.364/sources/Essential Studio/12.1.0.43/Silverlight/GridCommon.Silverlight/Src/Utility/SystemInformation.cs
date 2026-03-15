#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
#if !WinRT

namespace Syncfusion.Windows.GridCommon
#else
using Windows.Foundation;

namespace Syncfusion.WinRT.GridCommon
#endif
{
    /// <summary>
    /// Wraps native method calls to determine system settings.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
                return 250;
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
                return new Size(4, 4);
            }
        }

    }

}
