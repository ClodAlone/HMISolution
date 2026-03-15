#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an items control for arranging non working days in day view.
    /// </summary>
    public class ScheduleNonWorkingDayItemsControl : ItemsControl
    {
        #region Overrides

        protected override DependencyObject GetContainerForItemOverride()
        {
            var grid = new Grid();
            return grid;
        }
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is Grid;
        }

        #endregion
    }
}
