#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Schedule
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Threading;
	using System.Collections.ObjectModel;

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  class that hold side content for the MonthView fo the Schedule
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleMonthViewSideContentControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewSideContentControl"/>
        /// class.
        /// </summary>
        public ScheduleMonthViewSideContentControl()
        {
            this.DefaultStyleKey = typeof(ScheduleMonthViewSideContentControl);
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SideText.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SideTextProperty = DependencyProperty.Register("SideText", typeof(string), typeof(ScheduleMonthViewSideContentControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets string value for side text
        /// </summary>
        public string SideText
        {
            get
            {
                return (string)this.GetValue(ScheduleMonthViewSideContentControl.SideTextProperty);
            }
            set
            {
                this.SetValue(ScheduleMonthViewSideContentControl.SideTextProperty, value);
            }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for Dates.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DatesProperty = DependencyProperty.Register("Dates", typeof(ObservableCollection<DateTime>),typeof(ScheduleMonthViewSideContentControl), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets collection of DateTime value
        /// </summary>
        public ObservableCollection<DateTime> Dates
        {
            get 
            {
                return (ObservableCollection<DateTime>)this.GetValue(ScheduleMonthViewSideContentControl.DatesProperty);
            }
            set
            { 
                this.SetValue(ScheduleMonthViewSideContentControl.DatesProperty,value);
            }            
        }       

        #region SelectionBackground (DependencyProperty)

        /// <summary>
        /// Gets / Sets the SelectionBackground property.
        /// </summary>
        public Brush SelectionBackground
        {
            get { return (Brush)GetValue(SelectionBackgroundProperty); }
            set { SetValue(SelectionBackgroundProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SelectionBackground.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionBackgroundProperty = DependencyProperty.Register("SelectionBackground",
            typeof(Brush), typeof(ScheduleMonthViewSideContentControl), new PropertyMetadata(null));



        #endregion
    }
}