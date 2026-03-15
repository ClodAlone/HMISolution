#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an items control for arranging month view items.
    /// </summary>
    public class ScheduleMonthViewItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleMonthViewItemsControl">ScheduleMonthViewItemsControl</see>
        /// class.
        /// </summary>
        public ScheduleMonthViewItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleMonthViewItemsControl);
        }

        #endregion

        #region Internal Fields

        internal SfSchedule schedule;

        #endregion

        #region Dependency Properties

        #region MonthHeaderDateFormat
        /// <summary>
        /// Gets the DateTime format for header in month view.
        /// </summary>
        public string MonthHeaderDateFormat
        {
            get { return (string)GetValue(MonthHeaderDateFormatProperty); }
            internal set { SetValue(MonthHeaderDateFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MonthHeaderDateFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthHeaderDateFormatProperty =
            DependencyProperty.Register("MonthHeaderDateFormat", typeof(string), typeof(ScheduleMonthViewItemsControl), new PropertyMetadata("dd", OnEnableAutoFormatChanged));
        #endregion        

        #region SelectedDates
        /// <summary>
        /// Gets the collection of selected dates in month view.
        /// </summary>
        public ObservableCollection<DateTime> SelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty); }
            internal set { SetValue(SelectedDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(ScheduleMonthViewItemsControl), new PropertyMetadata(null, OnSelectedDatesChanged));

        private static void OnSelectedDatesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var MonthViewCtr = obj as ScheduleMonthViewItemsControl;
            if (MonthViewCtr != null) MonthViewCtr.GenerateDates();
        }
        #endregion

        #region FocusedMonth
        /// <summary>
        /// Gets the color for dates of selected month.
        /// </summary>
        public Brush FocusedMonth
        {
            get { return (Brush)GetValue(FocusedMonthProperty); }
            internal set { SetValue(FocusedMonthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for FocusedMonth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FocusedMonthProperty =
            DependencyProperty.Register("FocusedMonth", typeof(Brush), typeof(ScheduleMonthViewItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region NoFocusedMonth
        /// <summary>
        /// Gets the color for dates of previous and next months.
        /// </summary>
        public Brush NonFocusedMonth
        {
            get { return (Brush)GetValue(NonFocusedMonthProperty); }
            internal set { SetValue(NonFocusedMonthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NonFocusedMonth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NonFocusedMonthProperty =
            DependencyProperty.Register("NonFocusedMonth", typeof(Brush), typeof(ScheduleMonthViewItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region CurrentDateBackground
        /// <summary>
        /// Gets the background for current date in month view.
        /// </summary>
        public Brush CurrentDateBackground
        {
            get { return (Brush)GetValue(CurrentDateBackgroundProperty); }
            internal set { SetValue(CurrentDateBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentDateBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentDateBackgroundProperty =
            DependencyProperty.Register("CurrentDateBackground", typeof(Brush), typeof(ScheduleMonthViewItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
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
            DependencyProperty.Register("MonthViewLineStroke", typeof(Brush), typeof(ScheduleMonthViewItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region EnableAutoFormat

        internal bool EnableAutoFormat
        {
            get { return (bool)GetValue(EnableAutoFormatProperty); }
            set { SetValue(EnableAutoFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableAutoFormat.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty EnableAutoFormatProperty =
            DependencyProperty.Register("EnableAutoFormat", typeof(bool), typeof(ScheduleMonthViewItemsControl), new PropertyMetadata(true, OnEnableAutoFormatChanged));

        private static void OnEnableAutoFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleMonthViewItemsControl)
            {
                var scheduleMonthViewItemsControl = (d as ScheduleMonthViewItemsControl);
                var schedule = scheduleMonthViewItemsControl.schedule;
                if (schedule != null && scheduleMonthViewItemsControl.Items != null)
                {
                    string autoFormat = schedule.EnableAutoFormat && schedule.needAutoFormat ? "dd" : scheduleMonthViewItemsControl.MonthHeaderDateFormat; 
                    foreach (ScheduleMonthDateContentControl item in scheduleMonthViewItemsControl.Items)
                    {
                        item.MonthDateFormat = autoFormat;
                    }
                }
            }
        }

        #endregion

        #region AutoHeaderFormat
        /// <summary>
        /// Gets the DateTime format for header in Auto view.
        /// </summary>
        internal string AutoHeaderFormat
        {
            get { return (string)GetValue(AutoHeaderFormatProperty); }
            set { SetValue(AutoHeaderFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoHeaderFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        internal static readonly DependencyProperty AutoHeaderFormatProperty =
            DependencyProperty.Register("AutoHeaderFormat", typeof(string), typeof(ScheduleMonthViewItemsControl), new PropertyMetadata("dd", OnEnableAutoFormatChanged));
        #endregion

        #endregion

        #region Methods

        #region GenerateDates

        internal void GenerateDates()
        {
            var count = SelectedDates.Count;
            int diff = count % 7;
            if (diff != 0)
            {
                count += 7 - diff;
            }
            var firstDate = SelectedDates[0];
            var lastDate = firstDate.AddDays(count);
            var firstDateDayCount = DateTime.DaysInMonth(firstDate.Year, firstDate.Month) - firstDate.Day;
            var lastDateDayCount = lastDate.Day;
            bool middle = false;
            int val = 0;


            for (int i = 0; i < count; i++)
            {
                if (SelectedDates[0].AddDays(i).Month != firstDate.Month && SelectedDates[0].AddDays(i).Month != lastDate.Month)
                {
                    middle = true;
                    val = SelectedDates[i].Month;
                    break;
                }
            }

            if (!middle)
            {
                val = firstDateDayCount > lastDateDayCount ? firstDate.Month : lastDate.Month;
            }
            if (Items != null)
            {
#if WINRT
                if (Items.Count != 0)
                {
                    if (Items.Count == count)
                    {
                        #region Modifying value to existing items
                        for (int i = 0; i < count; i++)
                        {
                            var item = Items[i] as ScheduleMonthDateContentControl;
                            var date = SelectedDates[0].AddDays(i);
                            item.Date = date;
                            item.DateText = date.ToString();
                            item.IsCurrentMonth = date.Month == val;
                            item.IsCurrentDate = (date == DateTime.Now.Date);
                            if (schedule != null && schedule.SelectedDate.Date == item.Date && schedule.currentitem != null && schedule.SelectedDate.Date != schedule.currentitem.Date)
                            {
                                var backgroundBinding = new Binding { Source = this };
                                if (!schedule.currentitem.IsCurrentDate)
                                {
                                    backgroundBinding.Path = schedule.currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                                    BindingOperations.SetBinding(schedule.currentitem, BackgroundProperty, backgroundBinding);
                                }
                                schedule.currentitem = item;
                            }
                            var FocusedBinding = new Binding { Source = this };
                            if (!item.IsCurrentMonth && item.Date != DateTime.Now)
                            {
                                FocusedBinding.Path = new PropertyPath("NonFocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (!item.IsCurrentDate)
                            {
                                FocusedBinding.Path = new PropertyPath("FocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (item.IsCurrentDate)
                            {
                                FocusedBinding.Path = new PropertyPath("CurrentDateBackground");
                                item.TextForeground = new SolidColorBrush(Colors.White);
                            }

                            BindingOperations.SetBinding(item, BackgroundProperty, FocusedBinding);
                            if (!item.IsCurrentDate && schedule != null && ((schedule.currentitem != null && schedule.currentitem.Date == item.Date) && (schedule.SelectedDate == schedule.currentitem.Date)))
                            {
                                item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                                schedule.currentitem = item;
                            }
                        }
                        #endregion
                    }
                    else if (Items.Count > count)
                    {
                        #region Removing and modifying value to existing items
                        int diffcount = Items.Count - count;
                        for (int j = 0; j < diffcount; j++)
                        {
                            Items.RemoveAt(0);
                        }
                        for (int i = 0; i < count; i++)
                        {
                            var item = Items[i] as ScheduleMonthDateContentControl;
                            var date = SelectedDates[0].AddDays(i);
                            item.Date = date;
                            item.DateText = date.ToString();
                            item.IsCurrentMonth = date.Month == val;
                            item.IsCurrentDate = (date == DateTime.Now.Date);
                            if (schedule != null && schedule.SelectedDate.Date == item.Date && schedule.currentitem != null && schedule.SelectedDate.Date != schedule.currentitem.Date)
                            {
                                var backgroundBinding = new Binding { Source = this };
                                if (!schedule.currentitem.IsCurrentDate)
                                {
                                    backgroundBinding.Path = schedule.currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                                    BindingOperations.SetBinding(schedule.currentitem, BackgroundProperty, backgroundBinding);
                                }
                                schedule.currentitem = item;
                            }
                            var FocusedBinding = new Binding { Source = this };

                            if (!item.IsCurrentMonth && item.Date != DateTime.Now)
                            {
                                FocusedBinding.Path = new PropertyPath("NonFocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (!item.IsCurrentDate)
                            {
                                FocusedBinding.Path = new PropertyPath("FocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (item.IsCurrentDate)
                            {
                                FocusedBinding.Path = new PropertyPath("CurrentDateBackground");
                                item.TextForeground = new SolidColorBrush(Colors.White);
                            }

                            BindingOperations.SetBinding(item, BackgroundProperty, FocusedBinding);
                            if (!item.IsCurrentDate && schedule != null && ((schedule.currentitem != null && schedule.currentitem.Date == item.Date) && (schedule.SelectedDate == schedule.currentitem.Date)))
                            {
                                item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                                schedule.currentitem = item;
                            }
                        }
                        #endregion
                    }
                    else if (Items.Count < count)
                    {
                        #region Adding and modifying/setting value to existing items

                        for (int i = 0; i < Items.Count; i++)
                        {
                            var item = Items[i] as ScheduleMonthDateContentControl;
                            var date = SelectedDates[0].AddDays(i);
                            item.Date = date;
                            item.DateText = date.ToString();
                            item.IsCurrentMonth = date.Month == val;
                            item.IsCurrentDate = (date == DateTime.Now.Date);
                            if (schedule != null && schedule.SelectedDate.Date == item.Date && schedule.currentitem != null && schedule.SelectedDate.Date != schedule.currentitem.Date)
                            {
                                var backgroundBinding = new Binding { Source = this };
                                if (!schedule.currentitem.IsCurrentDate)
                                {
                                    backgroundBinding.Path = schedule.currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                                    BindingOperations.SetBinding(schedule.currentitem, BackgroundProperty, backgroundBinding);
                                }
                                schedule.currentitem = item;
                            }
                            var FocusedBinding = new Binding { Source = this };

                            if (!item.IsCurrentMonth && item.Date != DateTime.Now)
                            {
                                FocusedBinding.Path = new PropertyPath("NonFocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (!item.IsCurrentDate)
                            {
                                FocusedBinding.Path = new PropertyPath("FocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (item.IsCurrentDate)
                            {
                                FocusedBinding.Path = new PropertyPath("CurrentDateBackground");
                                item.TextForeground = new SolidColorBrush(Colors.White);
                            }

                            BindingOperations.SetBinding(item, BackgroundProperty, FocusedBinding);
                            if (!item.IsCurrentDate && schedule != null && ((schedule.currentitem != null && schedule.currentitem.Date == item.Date) && (schedule.SelectedDate == schedule.currentitem.Date)))
                            {
                                item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                                schedule.currentitem = item;
                            }
                        }
                        for (int k = Items.Count; k < count; k++)
                        {
                            var item = (ScheduleMonthDateContentControl)GetContainerForItemOverride();
                            var date = SelectedDates[0].AddDays(k);
                            item.Date = date;
                            item.DateText = date.ToString();
                            item.IsCurrentMonth = date.Month == val;
                            item.IsCurrentDate = (date == DateTime.Now.Date);
                            if (schedule != null && schedule.SelectedDate.Date == item.Date && schedule.currentitem != null && schedule.SelectedDate.Date != schedule.currentitem.Date)
                            {
                                var backgroundBinding = new Binding { Source = this };
                                if (!schedule.currentitem.IsCurrentDate)
                                {
                                    backgroundBinding.Path = schedule.currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                                    BindingOperations.SetBinding(schedule.currentitem, BackgroundProperty, backgroundBinding);
                                }
                                schedule.currentitem = item;
                            }
                            var FocusedBinding = new Binding { Source = this };

                            if (!item.IsCurrentMonth && item.Date != DateTime.Now)
                            {
                                FocusedBinding.Path = new PropertyPath("NonFocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (!item.IsCurrentDate)
                            {
                                FocusedBinding.Path = new PropertyPath("FocusedMonth");
                                item.TextForeground = Foreground;
                            }
                            else if (item.IsCurrentDate)
                            {
                                FocusedBinding.Path = new PropertyPath("CurrentDateBackground");
                                item.TextForeground = new SolidColorBrush(Colors.White);
                            }


                            BindingOperations.SetBinding(item, BackgroundProperty, FocusedBinding);
                            if (!item.IsCurrentDate && schedule != null && ((schedule.currentitem != null && schedule.currentitem.Date == item.Date) && (schedule.SelectedDate == schedule.currentitem.Date)))
                            {
                                item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                                schedule.currentitem = item;
                            }
                            Items.Add(item);
                        }
                        #endregion
                    }

                }
                else
#endif
                {
                    Items.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        var item = (ScheduleMonthDateContentControl)GetContainerForItemOverride();
                        var date = SelectedDates[0].AddDays(i);
                        item.Date = date;
                        item.DateText = date.ToString();

                        item.IsCurrentMonth = date.Month == val;
                        item.IsCurrentDate = (date == DateTime.Now.Date);
                        if (schedule != null && schedule.SelectedDate.Date == item.Date && schedule.currentitem != null && schedule.SelectedDate.Date != schedule.currentitem.Date)
                        {
                            var backgroundBinding = new Binding { Source = this };
                            if (!schedule.currentitem.IsCurrentDate)
                            {
                                backgroundBinding.Path = schedule.currentitem.IsCurrentMonth ? new PropertyPath("FocusedMonth") : new PropertyPath("NonFocusedMonth");
                                BindingOperations.SetBinding(schedule.currentitem, BackgroundProperty, backgroundBinding);
                            }
                            schedule.currentitem = item;
                        }
                        var FocusedBinding = new Binding { Source = this };

                        if (!item.IsCurrentMonth && item.Date != DateTime.Now)
                        {
                            FocusedBinding.Path = new PropertyPath("NonFocusedMonth");
                            item.TextForeground = Foreground;
                        }
                        else if (!item.IsCurrentDate)
                        {
                            FocusedBinding.Path = new PropertyPath("FocusedMonth");
                            item.TextForeground = Foreground;
                        }
                        else if (item.IsCurrentDate)
                        {
                            FocusedBinding.Path = new PropertyPath("CurrentDateBackground");
                            item.TextForeground = new SolidColorBrush(Colors.White);
                        }

                        BindingOperations.SetBinding(item, BackgroundProperty, FocusedBinding);
                        if (!item.IsCurrentDate && schedule != null && ((schedule.currentitem != null && schedule.currentitem.Date == item.Date) && (schedule.SelectedDate == schedule.currentitem.Date)))
                        {
                            item.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3));
                            schedule.currentitem = item;
                        }
                        Items.Add(item);
                    }
                }

            }

        }

        #endregion

        #endregion

        #region Overrides

        #region GetContainerForItemOverride

        protected override DependencyObject GetContainerForItemOverride()
        {
            var daysContent = new ScheduleMonthDateContentControl();
            Binding linestroke = new Binding();
            linestroke.Path = new PropertyPath("MonthViewLineStroke");
            linestroke.Source = this;
            BindingOperations.SetBinding(daysContent, ScheduleMonthDateContentControl.MonthViewLineStrokeProperty, linestroke);
            daysContent.MonthDateFormat = MonthHeaderDateFormat;
#if WINRT
            if (ItemContainerStyle != null)
            {
                daysContent.Style = ItemContainerStyle;

            }
#endif
            return daysContent;
        }

        #endregion

        #endregion
    }
}
