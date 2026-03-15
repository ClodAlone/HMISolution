#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Represents an items control for arranging day view headers.
    /// </summary>
    public class ScheduleDaysHeaderViewItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleDaysHeaderViewItemsControl">ScheduleDaysHeaderViewItemsControl</see>
        /// class.
        /// </summary>
        public ScheduleDaysHeaderViewItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleDaysHeaderViewItemsControl);
            Loaded += ScheduleDaysHeaderViewItemsControl_Loaded;
        }

        #endregion

        #region Dependency Properties

        #region ResourceCount
        /// <summary>
        /// Gets the count of resources in day view header.
        /// </summary>
        public int ResourceCount
        {
            get { return (int)GetValue(ResourceCountProperty); }
            internal set { SetValue(ResourceCountProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResourceCount.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResourceCountProperty =
            DependencyProperty.Register("ResourceCount", typeof(int), typeof(ScheduleDaysHeaderViewItemsControl), new PropertyMetadata(1, OnResourceCount));

        private static void OnResourceCount(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduleDaysHeaderViewItemsControl = d as ScheduleDaysHeaderViewItemsControl;
            if (scheduleDaysHeaderViewItemsControl != null) scheduleDaysHeaderViewItemsControl.GenerateItems();
        }
        #endregion

        #region CurrentDateBackground
        /// <summary>
        /// Gets the header background for current date.
        /// </summary>
        public Brush CurrentDateBackground
        {
            get { return (Brush)GetValue(CurrentDateBackgroundProperty); }
            internal set { SetValue(CurrentDateBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentDateBackgroundProperty =
            DependencyProperty.Register("CurrentDateBackground", typeof(Brush), typeof(ScheduleDaysHeaderViewItemsControl), new PropertyMetadata(null));
        #endregion

        #region SelectedDates
        /// <summary>
        /// Gets the collection of selected dates.
        /// </summary>
        /// <seealso cref="T:System.DateTime"></seealso>
        public ObservableCollection<DateTime> SelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty); }
            internal set { SetValue(SelectedDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(ScheduleDaysHeaderViewItemsControl), new PropertyMetadata(null, OnSelectedDatesPropertyChanged));

        private static void OnSelectedDatesPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var scheduleDaysHeaderViewItemsControl = dpo as ScheduleDaysHeaderViewItemsControl;
            if (scheduleDaysHeaderViewItemsControl != null) scheduleDaysHeaderViewItemsControl.GenerateItems();
        }
        #endregion

        #region CurrentScheduleType
        /// <summary>
        /// Gets the schedule type of current view.
        /// </summary>
        public ScheduleType CurrentScheduleType
        {
            get { return (ScheduleType)GetValue(CurrentScheduleTypeProperty); }
            internal set { SetValue(CurrentScheduleTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentScheduleType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentScheduleTypeProperty =
            DependencyProperty.Register("CurrentScheduleType", typeof(ScheduleType), typeof(ScheduleDaysHeaderViewItemsControl), new PropertyMetadata(ScheduleType.Day, OnCurrentScheduleTypeChanged));

        private static void OnCurrentScheduleTypeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var scheduleDaysHeaderViewItemsControl = dpo as ScheduleDaysHeaderViewItemsControl;
            if (scheduleDaysHeaderViewItemsControl != null) scheduleDaysHeaderViewItemsControl.GenerateItems();
        }
        #endregion

        #region DayHeaderOrder
        /// <summary>
        /// Gets the order by which resources in day view have to be arranged.
        /// </summary>
        public DayHeaderOrder DayHeaderOrder
        {
            get { return (DayHeaderOrder)GetValue(DayHeaderOrderProperty); }
            internal set { SetValue(DayHeaderOrderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayHeaderOrder.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayHeaderOrderProperty =
            DependencyProperty.Register("DayHeaderOrder", typeof(DayHeaderOrder), typeof(ScheduleDaysHeaderViewItemsControl), new PropertyMetadata(DayHeaderOrder.OrderByResource, OnDayResourceViewChanged));

        private static void OnDayResourceViewChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs arg)
        {
            var dayview = dpo as ScheduleDaysHeaderViewItemsControl;
            if (dayview != null) dayview.GenerateItems();

            var schedule = (dpo as ScheduleDaysHeaderViewItemsControl).FindParentElementOfType<SfSchedule>();
            if (schedule != null)
            {
                schedule.DragDropCanvas.Children.Clear();
                if(schedule.editpopup != null)
                schedule.editpopup.IsOpen = false;
                if (schedule.addnewpopup != null)
                schedule.addnewpopup.IsOpen = false;
#if !WINRT
                schedule.contextmenupopup.IsOpen = false;
                schedule.AddnewContextmenuPopup.IsOpen = false;
#endif
            }

        }
        #endregion

        #region DayViewColumnCount
        public int DayViewColumnCount
        {
            get { return (int)GetValue(DayViewColumnCountProperty); }
            internal set { SetValue(DayViewColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DayViewColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DayViewColumnCountProperty =
            DependencyProperty.Register("DayViewColumnCount", typeof(int), typeof(ScheduleDaysHeaderViewItemsControl), new PropertyMetadata(1, OnDayViewColumnCountChanged));

        private static void OnDayViewColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is ScheduleDaysHeaderViewItemsControl)
            {
                ScheduleDaysHeaderViewItemsControl scheduleDaysHeaderViewItemsControl = d as ScheduleDaysHeaderViewItemsControl;
                var schedule = scheduleDaysHeaderViewItemsControl.FindParentElementOfType<SfSchedule>();
                if(schedule != null && schedule.ScheduleType == ScheduleType.Day)
                    scheduleDaysHeaderViewItemsControl.GenerateItems();
            }
        }
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
            DependencyProperty.Register("DayViewVerticaLineStroke", typeof(Brush), typeof(ScheduleDaysHeaderViewItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #endregion

        #region Methods

        #region GenerateItems with DefaultArgument

        internal void GenerateItems()
        {
            var currentType = CurrentScheduleType;
            switch (currentType)
            {
                case ScheduleType.Day:
                case ScheduleType.WorkWeek:
                case ScheduleType.Week:
                    if (SelectedDates != null)
                        GenerateItems(SelectedDates.Count);
                    break;
            }
        }

        #endregion

        #region CalculateLeafChildCount

        int CalculateLeafChildCount(ResourceType restype)
        {
            int leafchild = 1;
            leafchild = leafchild * restype.ResourceCollection.Count;
            ResourceType tempresotype = restype.SubResourceType;
            while (tempresotype != null)
            {
                leafchild = leafchild * tempresotype.ResourceCollection.Count;
                tempresotype = tempresotype.SubResourceType;
            }
            return leafchild;
        }

        #endregion

        #region GenerateItems with Single Argument

        private void GenerateItems(int count)
        {
            var backgroundBinding = new Binding { Source = this, Path = new PropertyPath("CurrentDateBackground") };
            if (Items != null) Items.Clear();
            double totalresource = 1;
            var schedule = this.FindParentElementOfType<SfSchedule>();
            if (schedule == null)
            {
                return;
            }
            if (schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0 && schedule.DayHeaderOrder == DayHeaderOrder.OrderByResource)
            {
                totalresource = CalculateLeafChildCount(schedule.ScheduleResourceType);
            }
            var formatbinding = new Binding { Source = schedule, Path = new PropertyPath("HeaderFormat") };
            var borderStrokeBinding = new Binding { Source = schedule, Path = new PropertyPath("DayViewVerticaLineStroke") };
            var selecteddates = new ObservableCollection<DateTime>(SelectedDates.OrderBy(s => s));
            for (int j = 0; j < totalresource; j++)
            {
                for (int i = 0; i < count; i++)
                {
                    var dateTime = selecteddates[i];
                    var item = (ScheduleDaysHeaderViewControl)GetContainerForItemOverride();
                    item.DateTime = dateTime.Date;
                    item.SetBinding(ScheduleDaysHeaderViewControl.FormatProperty, formatbinding);
                    item.DayText = dateTime.ToString();
                    item.TextForeground = Foreground;
                    item.IsCurrentDate = (dateTime.Date == DateTime.Now.Date);
                    if (item.IsCurrentDate)
                    {
                        item.SetBinding(ScheduleDaysHeaderViewControl.HeaderBrushProperty, backgroundBinding);
                        item.TextForeground = new SolidColorBrush(Colors.White);
                    }
                    item.SetBinding(ScheduleDaysHeaderViewControl.DayViewVerticaLineStrokeProperty, borderStrokeBinding);
                    item.BorderThickness = i == count - 1 ? new Thickness(3, 0, 3, 2) : new Thickness(3, 0, 0, 2);
                    if (Items != null) Items.Add(item);
                }
            }
        }

        #endregion

        #region Loaded

        void ScheduleDaysHeaderViewItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateItems();
        }

        #endregion

        #endregion

        #region Overrides

        #region GetContainerForItemOverride

        protected override DependencyObject GetContainerForItemOverride()
        {
            var daysHeaderControl = new ScheduleDaysHeaderViewControl();
#if WINRT
            if (ItemContainerStyle != null)
            {
                daysHeaderControl.Style = ItemContainerStyle;
            }
#endif
            return daysHeaderControl;
        }

        #endregion

        #region IsItemItsOwnContainerOverride

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ScheduleDaysHeaderViewControl);
        }

        #endregion

        #endregion
    }
}
