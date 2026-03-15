#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a header in day view.
    /// </summary>
    public class ScheduleDaysHeaderViewControl : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleDaysHeaderViewControl">ScheduleDaysHeaderViewControl</see>
        /// class.
        /// </summary>
        public ScheduleDaysHeaderViewControl()
        {
            DefaultStyleKey = typeof(ScheduleDaysHeaderViewControl);
        }

        #endregion

        #region Dependency Properties

        #region Format
        internal string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(null));
        #endregion

        #region DayText
        internal string DayText
        {
            get { return (string)GetValue(DayTextProperty); }
            set { SetValue(DayTextProperty, value); }
        }

        internal static readonly DependencyProperty DayTextProperty = 
            DependencyProperty.Register("DayText", typeof(string), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(string.Empty));
        #endregion

        #region TextForeground
        internal Brush TextForeground
        {
            get { return (Brush)GetValue(TextForegroundProperty); }
            set { SetValue(TextForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextForeground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TextForegroundProperty =
            DependencyProperty.Register("TextForeground", typeof(Brush), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region DateTime
        /// <summary>
        /// Gets or sets the header date.
        /// </summary>
        public DateTime DateTime
        {
            get { return (DateTime)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DateTime.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register("DateTime", typeof(DateTime), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(null));
        #endregion

        #region IsCurrentDate
        /// <summary>
        /// Gets or sets a value indicating whether the header date will be current date.
        /// </summary>
        public bool IsCurrentDate
        {
            get { return (bool)GetValue(IsCurrentDateProperty); }
            set { SetValue(IsCurrentDateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCurrentDate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCurrentDateProperty = 
            DependencyProperty.Register("IsCurrentDate", typeof(bool), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(false));
        #endregion

        #region HeaderBrush
        public Brush HeaderBrush
        {
            get { return (Brush)GetValue(HeaderBrushProperty); }
            set { SetValue(HeaderBrushProperty, value); }
        }

        public static readonly DependencyProperty HeaderBrushProperty = DependencyProperty.Register("HeaderBrush", typeof(Brush),
            typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(null));
        #endregion

        #region DayViewVerticaLineStroke
        public Brush DayViewVerticaLineStroke
        {
            get { return (Brush)GetValue(DayViewVerticaLineStrokeProperty); }
            set { SetValue(DayViewVerticaLineStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayViewVerticaLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayViewVerticaLineStrokeProperty =
            DependencyProperty.Register("DayViewVerticaLineStroke", typeof(Brush), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #endregion
    }
}
