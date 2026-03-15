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
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an items control for arranging all day appointments.
    /// </summary>
    public class ScheduleAllDaysAppointmentItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAllDaysAppointmentItemsControl">ScheduleAllDaysAppointmentItemsControl</see>
        /// class.
        /// </summary>
        public ScheduleAllDaysAppointmentItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleAllDaysAppointmentItemsControl);
        }

        #endregion

        #region Dependency Properties

        #region AppointmentSelectionBrush
        /// <summary>
        /// Gets the border color of all day appointment while it gets selected.
        /// </summary>
        public Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            internal set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleAllDaysAppointmentItemsControl), new PropertyMetadata(null));
        #endregion

        #region AppointmentTemplate
        /// <summary>
        /// Gets or sets the template for customizing all day appointment.
        /// </summary>
        public DataTemplate AppointmentTemplate
        {
            get { return (DataTemplate)GetValue(AppointmentTemplateProperty); }
            set { SetValue(AppointmentTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentTemplateProperty =
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(ScheduleAllDaysAppointmentItemsControl), new PropertyMetadata(null));
        #endregion

        #endregion
    }
}
