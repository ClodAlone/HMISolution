#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represent a day view item header.
    /// </summary>
    public class DayViewItemHeader : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.DayViewItemHeader">DayViewItemHeader</see>
        /// class.
        /// </summary>
        public DayViewItemHeader()
        {
            DefaultStyleKey = typeof(DayViewItemHeader);
        }

        #endregion
    }
}
