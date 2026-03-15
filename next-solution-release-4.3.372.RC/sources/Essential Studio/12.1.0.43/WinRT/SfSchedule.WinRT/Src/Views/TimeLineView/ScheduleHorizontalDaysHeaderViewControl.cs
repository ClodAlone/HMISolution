#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI;
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
    /// Represents a timeline view header.
    /// </summary>
    public class ScheduleHorizontalDaysHeaderViewControl : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleHorizontalDaysHeaderViewControl">ScheduleHorizontalDaysHeaderViewControl</see>
        /// class.
        /// </summary>
        public ScheduleHorizontalDaysHeaderViewControl()
        {
            DefaultStyleKey = typeof(ScheduleHorizontalDaysHeaderViewControl);
        }

        #endregion

        #region Dependency Properties

        #region TextForeground
        /// <summary>
        /// Gets or sets the text color for timeline view header.
        /// </summary>
        public Brush TextForeground
        {
            get { return (Brush)GetValue(TextForegroundProperty); }
            set { SetValue(TextForegroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TextForeground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextForegroundProperty =
            DependencyProperty.Register("TextForeground", typeof(Brush), typeof(ScheduleHorizontalDaysHeaderViewControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region HeaderBrush
        /// <summary>
        /// Gets or sets the background color for timeline view header.
        /// </summary>
        public Brush HeaderBrush
        {
            get { return (Brush)GetValue(HeaderBrushProperty); }
            set { SetValue(HeaderBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderBrushProperty = 
            DependencyProperty.Register("HeaderBrush", typeof(Brush), typeof(ScheduleHorizontalDaysHeaderViewControl), new PropertyMetadata(null));
        #endregion

        #region Format
        internal string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(ScheduleHorizontalDaysHeaderViewControl), new PropertyMetadata(null));
        #endregion

        #region DayText
        internal string DayText
        {
            get { return (string)GetValue(DayTextProperty); }
            set { SetValue(DayTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DayText.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DayTextProperty =
            DependencyProperty.Register("DayText", typeof(string), typeof(ScheduleHorizontalDaysHeaderViewControl), new PropertyMetadata(string.Empty));
        #endregion

        #endregion
    }
}
