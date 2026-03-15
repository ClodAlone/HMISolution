#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    #region TimeZone

    /// <summary>
    /// Represents a TimeZone of Schedule.
    /// </summary>
    public class TimeZone : DependencyObject
    {
        #region Dependency Properties

        #region TimeZoneValue
        /// <summary>
        /// Gets or sets the value for time zone.
        /// </summary>
        public string TimeZoneValue
        {
            get { return (string)GetValue(TimeZoneValueProperty); }
            set { SetValue(TimeZoneValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeZoneValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeZoneValueProperty =
            DependencyProperty.Register("TimeZoneValue", typeof(string), typeof(TimeZone), new PropertyMetadata(null));
        #endregion

        #endregion
    }

    #endregion

    #region TimeZoneCollection

    /// <summary>
    /// Represents a collection of TimeZone.
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.TimeZone"/>
    public class TimeZoneCollection : ObservableCollection<TimeZone>
    {
    }

    #endregion
}
