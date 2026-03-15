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
    /// Represents an items control for arranging resource headers.
    /// </summary>
    public class ResourceHeaderItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ResourceHeaderItemsControl">ResourceHeaderItemsControl</see>
        /// class.
        /// </summary>
        public ResourceHeaderItemsControl()
        {
            DefaultStyleKey = typeof(ResourceHeaderItemsControl);
        }

        #endregion
    }
}
