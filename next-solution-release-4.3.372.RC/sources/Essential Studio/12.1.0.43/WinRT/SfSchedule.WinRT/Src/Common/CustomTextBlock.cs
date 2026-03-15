#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a custom text block.
    /// </summary>
    public class CustomTextBlock : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.CustomTextBlock">CustomTextBlock</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public CustomTextBlock()
        {
            DefaultStyleKey = typeof(CustomTextBlock);
        }

        #endregion

        #region Dependency Properties

        #region DateTimeFormat
        /// <summary>
        /// Gets or sets a format for formatting date time value.
        /// </summary>
        public string DateTimeFormat
        {
            get { return (string)GetValue(DateTimeFormatProperty); }
            set { SetValue(DateTimeFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DateTimeFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateTimeFormatProperty =
            DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(CustomTextBlock), new PropertyMetadata(null, OnDateTimeFormatChanged));

        private static void OnDateTimeFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as CustomTextBlock;
            if (e.NewValue != null)
            {
                string str = e.NewValue.ToString();
                str = str.Trim('"', '\\', '%');
				// Since slash character is meant for date time separator
                str = str.Replace("/", "'/'");

                if (str.Length > 1)
                {
                    if (str.Equals(string.Empty))
                    {
                        str = "dddd dd";
                    }

                    if (textbox != null && textbox.DateTimeValue != null)
                    {
                        DateTime date = DateTime.Parse(textbox.DateTimeValue);
                        textbox.Text = date.ToString(str, CultureInfo.CurrentCulture);
                    }
                }
            }
        }
        #endregion

        #region TimeSpanValue
        /// <summary>
        /// Gets or sets a time span value for custom text block.
        /// </summary>
        public TimeSpan TimeSpanValue
        {
            get { return (TimeSpan)GetValue(TimeSpanValueProperty); }
            set { SetValue(TimeSpanValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeSpanValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeSpanValueProperty =
            DependencyProperty.Register("TimeSpanValue", typeof(TimeSpan), typeof(CustomTextBlock), new PropertyMetadata(null));
        #endregion

        #region HourFormat
        /// <summary>
        /// Gets or sets a format for formatting hour value.
        /// </summary>
        public string HourFormat
        {
            get { return (string)GetValue(HourFormatProperty); }
            set { SetValue(HourFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HourFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HourFormatProperty =
            DependencyProperty.Register("HourFormat", typeof(string), typeof(CustomTextBlock), new PropertyMetadata(null, OnHourFormatChanged));

        private static void OnHourFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as CustomTextBlock;
            if (e.NewValue != null)
            {
                string str = e.NewValue.ToString();
                str = str.Trim('"', '\\', '%', '\'');
                // Since slash character is meant for date time separator
                str = str.Replace("/", "'/'");
                if (str.Length > 1)
                {
                    if (textbox != null)
                    {
                        var date = new DateTime(textbox.TimeSpanValue.Ticks);
                        textbox.Text = date.ToString(str);
                    }
                }
            }
        }
        #endregion

        #region MinuteFormat
        /// <summary>
        /// Gets or sets a format for formatting minute value.
        /// </summary>
        public string MinuteFormat
        {
            get { return (string)GetValue(MinuteFormatProperty); }
            set { SetValue(MinuteFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinuteFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinuteFormatProperty =
            DependencyProperty.Register("MinuteFormat", typeof(string), typeof(CustomTextBlock), new PropertyMetadata(null, OnMinuteFormatChanged));

        private static void OnMinuteFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as CustomTextBlock;
            if (e.NewValue != null)
            {
                string str = e.NewValue.ToString();
                str = str.Trim('"', '\\', '%', '\'');
                // Since slash character is meant for date time separator
                str = str.Replace("/", "'/'");
                if (str.Length > 1 && textbox != null)
                {
                    var date = new DateTime(textbox.TimeSpanValue.Ticks);
                    textbox.Text = date.ToString(str);
                }
            }
        }
        #endregion

        #region DateTimeValue
        /// <summary>
        /// Gets or sets a date time content of custom text block.
        /// </summary>
        public string DateTimeValue
        {
            get { return (string)GetValue(DateTimeValueProperty); }
            set { SetValue(DateTimeValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DateTimeValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateTimeValueProperty =
            DependencyProperty.Register("DateTimeValue", typeof(string), typeof(CustomTextBlock), new PropertyMetadata(null, OnDateTimeValueChanged));

        private static void OnDateTimeValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as CustomTextBlock;
            if (e.NewValue != null)
            {
                string str = e.NewValue.ToString();
                str = str.Trim('"', '\\', '%');
                DateTime date = DateTime.Parse(str);
                if (textbox != null)
                    textbox.Text = date.ToString(textbox.DateTimeFormat);
            }
        }
        #endregion

        #region MonthDateFormat
        /// <summary>
        /// Gets or sets a format for formatting date in month.
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
            DependencyProperty.Register("MonthDateFormat", typeof(string), typeof(CustomTextBlock), new PropertyMetadata("dd", OnMonthDateFormatChanged));

        private static void OnMonthDateFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as CustomTextBlock;
            if (e.NewValue != null)
            {
                string str = e.NewValue.ToString();
                str = str.Trim('"', '\\', '%');
                // Since slash character is meant for date time separator
                str = str.Replace("/", "'/'");

                if (str.Length == 0 || str.Length == 1)
                {
                    str = "dd";
                }
                if (textbox != null)
                {
                    DateTime date = DateTime.Parse(textbox.DateTimeValue);
                    textbox.Text = date.ToString(str);
                }
            }
        }
        #endregion

        #region Text
        /// <summary>
        /// Represents a text content of custom text block.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CustomTextBlock), new PropertyMetadata(null));
        #endregion

        #region TextWrapping
        /// <summary>
        /// Gets or sets how the custom text block should wrap text. 
        /// </summary>
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TextWrapping.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(CustomTextBlock), new PropertyMetadata(TextWrapping.NoWrap));
        #endregion

        #endregion
    }
}
