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
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    #region ScheduleAppointmentStatus

    /// <summary>
    /// Represents a schedule appointment's status. 
    /// </summary>
    public class ScheduleAppointmentStatus : DependencyObject
    {
        #region Dependency Properties

        #region Status
        /// <summary>
        /// Gets or sets the status of appointment.
        /// </summary>
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        //Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(ScheduleAppointmentStatus), new PropertyMetadata(string.Empty));
        #endregion

        #region Brush
        /// <summary>
        /// Gets or sets the color for representing corresponding status.
        /// </summary>
        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Brush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register("Brush", typeof(Brush), typeof(ScheduleAppointmentStatus), new PropertyMetadata(null));
        #endregion

        #endregion
    }

    #endregion

    #region ScheduleAppointmentStatusCollection

    /// <summary>
    /// Represents a collection of schedule appointment's status.
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointmentStatus"/>
    public class ScheduleAppointmentStatusCollection : ObservableCollection<ScheduleAppointmentStatus>
    {
    }

    #endregion
}
