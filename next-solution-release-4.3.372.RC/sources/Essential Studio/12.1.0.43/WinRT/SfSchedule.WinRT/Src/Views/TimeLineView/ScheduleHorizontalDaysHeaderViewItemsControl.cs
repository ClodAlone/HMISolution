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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an items control for arranging timeline view headers.
    /// </summary>
    public class ScheduleHorizontalDaysHeaderViewItemsControl : ItemsControl
    {
        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleHorizontalDaysHeaderViewItemsControl">ScheduleHorizontalDaysHeaderViewItemsControl</see> class. 
        /// </summary>
        public ScheduleHorizontalDaysHeaderViewItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleHorizontalDaysHeaderViewItemsControl);
        }

        #endregion

        #region Private Fields

        private bool isTemplateApplied;

        #endregion

        #region Dependency Properties

        #region SelectedDates
        /// <summary>
        /// Gets or sets the selected dates in timeline view.
        /// </summary>
        public ObservableCollection<DateTime> SelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty); }
            set { SetValue(SelectedDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(ScheduleHorizontalDaysHeaderViewItemsControl), new PropertyMetadata(null, SelectedDatesChanged));

        private static void SelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var obj = dpo as ScheduleHorizontalDaysHeaderViewItemsControl;
            if (obj != null && obj.isTemplateApplied)
            {
                var observableCollection = args.OldValue as ObservableCollection<DateTime>;
                if (obj.Items != null && (observableCollection != null && (observableCollection.Count != obj.SelectedDates.Count || obj.Items.Count != obj.SelectedDates.Count)))
                {
                    obj.GenerateItems();
                }
                else
                {
                    obj.UpdateItems();
                }
            }
        }
        #endregion

        #region TimeInterval
        /// <summary>
        /// Gets or sets the time interval for timeline view.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.TimeInterval"></seealso>
        public TimeInterval TimeInterval
        {
            get { return (TimeInterval)GetValue(TimeIntervalProperty); }
            set { SetValue(TimeIntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeIntervalProperty =
            DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleHorizontalDaysHeaderViewItemsControl), new PropertyMetadata(TimeInterval.OneHour, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeLineItemsControl = dpo as ScheduleHorizontalDaysHeaderViewItemsControl;
            if (timeLineItemsControl != null && timeLineItemsControl.isTemplateApplied)
            {
                timeLineItemsControl.GenerateItems();
            }
        }
        #endregion

        #region CurrentDateBackground
        /// <summary>
        /// Gets or sets the background color for current date's header.
        /// </summary>
        public Brush CurrentDateBackground
        {
            get { return (Brush)GetValue(CurrentDateBackgroundProperty); }
            set { SetValue(CurrentDateBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentDateBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentDateBackgroundProperty =
            DependencyProperty.Register("CurrentDateBackground", typeof(Brush), typeof(ScheduleHorizontalDaysHeaderViewItemsControl), new PropertyMetadata(null));
        #endregion

        #region HeaderBackground
        /// <summary>
        /// Gets or sets the background color for dates' header other than current date.
        /// </summary>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(ScheduleHorizontalDaysHeaderViewItemsControl), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Methods

        private void GenerateItems()
        {
            var schedule = this.FindParentElementOfType<SfSchedule>();
            if (schedule == null)
            {
                return;
            }

            GenerateItems(SelectedDates.Count);
        }

        private void UpdateItems()
        {
            UpdateItems(SelectedDates.Count);
        }

        private void GenerateItems(int count)
        {
            if (Items != null)
                Items.Clear();
            var schedule = this.FindParentElementOfType<SfSchedule>();
            var timelineview = this.FindParentElementOfType<ScheduleTimeLineView>();
            if (schedule != null && schedule.isIntervalHeightset)
                Width = timelineview.GetTimeSlotWidth(count);
            var currentDateBackgroundBinding = new Binding { Path = new PropertyPath("CurrentDateBackground"), Source = this };
            var headerBackgroundBinding = new Binding { Path = new PropertyPath("HeaderBackground"), Source = this };
            var formatbinding = new Binding { Source = schedule, Path = new PropertyPath("HeaderDateFormat") };
            for (int i = 0; i < count; i++)
            {
                var dateTime = SelectedDates[i];
                var item = (ScheduleHorizontalDaysHeaderViewControl)GetContainerForItemOverride();

                item.BorderThickness = new Thickness(1, 0, 1, 0);
                item.SetBinding(ScheduleHorizontalDaysHeaderViewControl.FormatProperty, formatbinding);
                item.DayText = dateTime.ToString();
                item.TextForeground = (dateTime.Date == DateTime.Now.Date) ? new SolidColorBrush(Colors.White) : Foreground;

                item.SetBinding(ScheduleHorizontalDaysHeaderViewControl.HeaderBrushProperty,
                dateTime == DateTime.Now.Date ? currentDateBackgroundBinding : headerBackgroundBinding);
                if (Items != null)
                    Items.Add(item);
            }
        }

        private void UpdateItems(int count)
        {
            var currentDateBackgroundBinding = new Binding { Path = new PropertyPath("CurrentDateBackground"), Source = this };
            var headerBackgroundBinding = new Binding { Path = new PropertyPath("HeaderBackground"), Source = this };
            for (int i = 0; i < count; i++)
            {
                var dateTime = SelectedDates[i];

                if (Items != null)
                {
                    var scheduleHorizontalDaysHeaderViewControl = Items[i] as ScheduleHorizontalDaysHeaderViewControl;
                    if (scheduleHorizontalDaysHeaderViewControl != null)
                    {
                        scheduleHorizontalDaysHeaderViewControl.DayText = dateTime.ToString();
                        scheduleHorizontalDaysHeaderViewControl.SetBinding(ScheduleHorizontalDaysHeaderViewControl.HeaderBrushProperty,
                              dateTime == DateTime.Now.Date ? currentDateBackgroundBinding : headerBackgroundBinding);

                        scheduleHorizontalDaysHeaderViewControl.TextForeground = (dateTime.Date == DateTime.Now.Date) ? new SolidColorBrush(Colors.White) : Foreground;
                    }
                }
            }
        }

        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            isTemplateApplied = true;
            GenerateItems();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var horizontaldaysHeaderControl = new ScheduleHorizontalDaysHeaderViewControl
            {
                Background = Background,
                BorderBrush = BorderBrush,
                BorderThickness = BorderThickness
            };
#if WINRT
            if (ItemContainerStyle != null)
            {
                horizontaldaysHeaderControl.Style = ItemContainerStyle;
            }
#endif
            return horizontaldaysHeaderControl;
        }

        #endregion
    }
}
