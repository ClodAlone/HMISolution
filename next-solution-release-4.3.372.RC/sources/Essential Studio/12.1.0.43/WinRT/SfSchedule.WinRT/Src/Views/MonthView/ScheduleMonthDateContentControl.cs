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
    /// Represents a date in month view.
    /// </summary>
    public class ScheduleMonthDateContentControl : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleMonthDateContentControl">ScheduleMonthDateContentControl</see>
        /// class.
        /// </summary>
        public ScheduleMonthDateContentControl()
        {
            DefaultStyleKey = typeof(ScheduleMonthDateContentControl);
        }

        #endregion

        #region Dependency Properties

        #region MonthDateFormat
        /// <summary>
        /// Gets or sets the DateTime format for date in month view.
        /// </summary>
        public string MonthDateFormat
        {
            get { return (string)GetValue(MonthDateFormatProperty); }
            set { SetValue(MonthDateFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthDateFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthDateFormatProperty =
            DependencyProperty.Register("MonthDateFormat", typeof(string), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(null));
        #endregion

        #region Date
        /// <summary>
        /// Gets or sets the date for date in month view.
        /// </summary>
        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(DateTime.Now.Date));
        #endregion

        #region DateText
        /// <summary>
        /// Gets or sets the date as text for date in month view.
        /// </summary>
        public string DateText
        {
            get { return (string)GetValue(DateTextProperty); }
            set { SetValue(DateTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DateText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateTextProperty =
            DependencyProperty.Register("DateText", typeof(string), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(string.Empty));
        #endregion

        #region ScheduleBackground
        /// <summary>
        /// Gets or sets the background for date in month view.
        /// </summary>
        public Brush ScheduleBackground
        {
            get { return (Brush)GetValue(ScheduleBackgroundProperty); }
            set { SetValue(ScheduleBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ScheduleBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScheduleBackgroundProperty =
            DependencyProperty.Register("ScheduleBackground", typeof(Brush), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(null));
        #endregion

        #region MonthViewLineStroke
        
        public Brush MonthViewLineStroke
        {
            get { return (Brush)GetValue(MonthViewLineStrokeProperty); }
            set { SetValue(MonthViewLineStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthViewLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthViewLineStrokeProperty =
            DependencyProperty.Register("MonthViewLineStroke", typeof(Brush), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region StrokeLine
        /// <summary>
        /// Gets or sets the border color of date in month view.
        /// </summary>
        public Brush StrokeLine
        {
            get { return (Brush)GetValue(StrokeLineProperty); }
            set { SetValue(StrokeLineProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeLine.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeLineProperty =
            DependencyProperty.Register("StrokeLine", typeof(Brush), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        #endregion

        #region StrokeThickness
        /// <summary>
        /// Gets or sets the border thickness of date in month view.
        /// </summary>
        public Thickness StrokeThickness
        {
            get { return (Thickness)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(Thickness), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(null));
        #endregion

        #region IsCurrentDate
        /// <summary>
        /// Gets or sets a value indicating whether the date in month view will be current date.
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
            DependencyProperty.Register("IsCurrentDate", typeof(bool), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(false));
        #endregion

        #region IsCurrentMonth
        /// <summary>
        /// Gets or sets a value indicating whether the date will be in current month.
        /// </summary>
        public bool IsCurrentMonth
        {
            get { return (bool)GetValue(IsCurrentMonthProperty); }
            set { SetValue(IsCurrentMonthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCurrentMonth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCurrentMonthProperty =
            DependencyProperty.Register("IsCurrentMonth", typeof(bool), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(true));
        #endregion

        #region TextForeground
        internal Brush TextForeground
        {
            get { return (Brush)GetValue(TextForegroundProperty); }
            set { SetValue(TextForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextForeground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TextForegroundProperty =
            DependencyProperty.Register("TextForeground", typeof(Brush), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #endregion
    }
}
