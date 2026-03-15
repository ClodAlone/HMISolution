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
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Controls.Schedule;

    /// <summary>
    /// Represents Schedule's TimeLine region.
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ScheduleTimeLineHourControl))]
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleTimeLineItemsControl : ItemsControl, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleTimeLineItemsControl"/> class.
        /// </summary>
        public ScheduleTimeLineItemsControl()
        {
            this.DefaultStyleKey = typeof(ScheduleTimeLineItemsControl);
        }

        private ScheduleCalendarViewModel model;
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public ScheduleCalendarViewModel Model
        {
            get
            {
                return this.model;
            }
        }

        internal void SetCalendarViewModel(ScheduleCalendarViewModel model)
        {
            this.model = model;
            this.ClearValue(ScheduleTimeLineItemsControl.TimeIntervalProperty);
            var bindTimeInterval = new Binding("CurrentTimeInterval") { Source = this.model, Mode = BindingMode.TwoWay };
            this.SetBinding(ScheduleTimeLineItemsControl.TimeIntervalProperty, bindTimeInterval);

            this.ClearValue(ScheduleTimeLineItemsControl.IntervalHeightProperty);
            var bindIntervalHeight = new Binding("IntervalHeight") { Source = this.model };
            this.SetBinding(ScheduleTimeLineItemsControl.IntervalHeightProperty, bindIntervalHeight);
        }

        void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.Model.CurrentScheduleType != ScheduleType.Day && this.Model.CurrentScheduleType != ScheduleType.Week
                && this.Model.CurrentScheduleType != ScheduleType.WorkWeek) return;

            if (e.PropertyName == "CurrentTimeInterval")
            {
                this.SetupHours();
            }
            else if (e.PropertyName == "IsAmPmTimeMode")
            {
                this.IsAmPmTimeMode = this.Model.IsAmPmTimeMode;
            }
            else if (e.PropertyName == "IntervalHeight")
            {
                this.Height = this.Model.GetTimeSlotHeight();
            }
        }

        #region StartWorkHour
        /// <summary>
        ///  Using a DependencyProperty as the backing store for StartWorkHour.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartWorkHourProperty = DependencyProperty.Register("StartWorkHour", typeof(int), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(8, new PropertyChangedCallback(OnStartWorkHourChanged)));

        private static void OnStartWorkHourChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeLineItemsControl = dpo as ScheduleTimeLineItemsControl;
            if (timeLineItemsControl.isTemplateApplied)
            {
                timeLineItemsControl.SetupHours();
            }
        }

        /// <summary>
        /// Gets or sets work day start hour.
        /// </summary>
        public int StartWorkHour
        {
            get
            {
                return (int)GetValue(StartWorkHourProperty);
            }

            set
            {
                SetValue(StartWorkHourProperty, value);
            }
        }
        #endregion

        #region EndWorkHour
        /// <summary>
        ///  Using a DependencyProperty as the backing store for EndWorkHour.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndWorkHourProperty = DependencyProperty.Register("EndWorkHour", typeof(int), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(17, new PropertyChangedCallback(OnEndWorkHourChanged)));

        private static void OnEndWorkHourChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeLineItemsControl = dpo as ScheduleTimeLineItemsControl;
            if (timeLineItemsControl.isTemplateApplied)
            {
                timeLineItemsControl.SetupHours();
            }
        }

        /// <summary>
        /// Gets or sets work day end hour.
        /// </summary>
        public int EndWorkHour
        {
            get
            {
                return (int)GetValue(EndWorkHourProperty);
            }

            set
            {
                SetValue(EndWorkHourProperty, value);
            }
        }

        #endregion

        #region IsAmPmTimeMode

        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsAmPmTimeMode.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAmPmTimeModeProperty = DependencyProperty.Register("IsAmPmTimeMode", typeof(bool), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether hours are showed in 24 hours or AM/PM time format.
        /// </summary>
        public bool IsAmPmTimeMode
        {
            get
            {
                return (bool)GetValue(IsAmPmTimeModeProperty);
            }

            set
            {
                SetValue(IsAmPmTimeModeProperty, value);
            }
        }

        #endregion

        #region TimelineHourDivisionVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTimelineHourDivisionVisibility.
        /// </summary>
        public Visibility TimelineHourDivisionVisibility
        {
            get { return (Visibility)GetValue(TimelineHourDivisionVisibilityProperty); }
            set { SetValue(TimelineHourDivisionVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TimelineHourDivisionVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimelineHourDivisionVisibilityProperty = DependencyProperty.Register("TimelineHourDivisionVisibility", typeof(Visibility), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region ItemContainerStyle
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ItemContainerStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
#if SILVERLIGHT
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
           typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(null));
#else
        public static readonly new DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
            typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(null));
#endif
        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        /// <value>The item container style.</value>
#if SILVERLIGHT
        public Style ItemContainerStyle
#else
         public new Style ItemContainerStyle
#endif
        {
            get
            {
                return (Style)base.GetValue(ItemContainerStyleProperty);
            }
            set
            {
                base.SetValue(ItemContainerStyleProperty, value);
            }
        }

        #endregion

        #region LinesStroke

        /// <summary>
        ///  Using a DependencyProperty as the backing store for LinesStroke.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LinesStrokeProperty = DependencyProperty.Register("LinesStroke", typeof(Brush), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the lines' stroke.
        /// </summary>
        /// <value>The lines stroke.</value>
        public Brush LinesStroke
        {
            get
            {
                return (Brush)GetValue(ScheduleTimeLineItemsControl.LinesStrokeProperty);
            }

            set
            {
                SetValue(ScheduleTimeLineItemsControl.LinesStrokeProperty, value);
            }
        }

        #endregion

        #region TimeInterval
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TimeInterval.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeIntervalProperty = DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(TimeInterval.OneHour, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeLineItemsControl = dpo as ScheduleTimeLineItemsControl;
            if (timeLineItemsControl.isTemplateApplied)
            {
                timeLineItemsControl.SetupHours();
            }
        }

        /// <summary>
        /// Gets or sets TimeInterval.
        /// </summary>
        public TimeInterval TimeInterval
        {
            get
            {
                return (TimeInterval)GetValue(ScheduleTimeLineItemsControl.TimeIntervalProperty);
            }

            set
            {
                SetValue(ScheduleTimeLineItemsControl.TimeIntervalProperty, value);
            }
        }

        #endregion

         #region TimelineVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTimelineVisibility.
        /// </summary>
        

        public static readonly DependencyProperty TimelineVisibilityProperty = DependencyProperty.Register("TimelineVisibility", typeof(Visibility), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets visibility value for TimeLineVisibilty.
        /// </summary>
      
        public Visibility TimelineVisibility
        {
            get { return (Visibility)GetValue(TimelineVisibilityProperty); }
            set { SetValue(TimelineVisibilityProperty, value); }
        }
        #endregion


        

        #region IntervalHeight

        private void UpdateHeight()
        {
            if (this.Model == null)
            {
                return;
            }

            this.Height = this.Model.GetTimeSlotHeight();
        }

        /// <summary>
        ///  set double value as 24
        /// </summary>
        public const double DefaultIntervalHeight = 24d;
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IntervalHeight.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalHeightProperty = DependencyProperty.Register("IntervalHeight", typeof(double), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(DefaultIntervalHeight, new PropertyChangedCallback(OnIntervalHeightChanged)));

        private static void OnIntervalHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleTimeLineItemsControl instance = (ScheduleTimeLineItemsControl)d;
            instance.OnIntervalHeightChanged(e);
        }

        private void OnIntervalHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateHeight();
        }

        /// <summary>
        /// Gets or sets the height of the inner hour interval.
        /// </summary>
        public double IntervalHeight
        {
            get
            {
                return (double)GetValue(IntervalHeightProperty);
            }

            set
            {
                SetValue(IntervalHeightProperty, value);
            }
        }

        #endregion

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var timeHourControl = new ScheduleTimeLineHourControl();
            timeHourControl.Background = this.Background;
            timeHourControl.BorderBrush = this.BorderBrush;
            timeHourControl.BorderThickness = this.BorderThickness;
            timeHourControl.TimelineHourDivisionVisibility = this.TimelineHourDivisionVisibility;
            timeHourControl.TimelineVisibility = this.TimelineVisibility;
            return timeHourControl;
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ScheduleTimeLineHourControl);
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var control = item as ScheduleTimeLineHourControl;
            var controlStyle = this.ItemContainerStyle;
            if (controlStyle != null && control.Style == null)
            {
                control.Style = controlStyle;
            }
        }

        private bool isTemplateApplied = false;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.isTemplateApplied = true;
            if (this.Model != null)
            {
                this.IsAmPmTimeMode = this.Model.IsAmPmTimeMode;
            }
            this.SetupHours();
            this.Model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
        }

        private void SetupHours()
        {
            if (this.model == null)
            {
                return;
            }

            foreach (ScheduleTimeLineHourControl item in this.Items)
            {
                item.ClearValue(ScheduleTimeLineHourControl.IsAmPmTimeModeProperty);
                item.ClearValue(ScheduleTimeLineHourControl.LinesStrokeProperty);
                item.ClearValue(ScheduleTimeLineHourControl.TimeIntervalProperty);
            }

            this.Items.Clear();
            Binding bindIsAMPM = new Binding("IsAmPmTimeMode") { Source = this };
            var bindLinesStroke = new Binding("LinesStroke") { Source = this };
            var bindTimeInterval = new Binding("TimeInterval") { Source = this };
            for (int i = ScheduleTimeLineHourControl.MinValue;i <= ScheduleTimeLineHourControl.MaxValue;i++)
            {
                var timeLineHour = (ScheduleTimeLineHourControl)this.GetContainerForItemOverride();
                timeLineHour.Hour = i;
                timeLineHour.SetBinding(ScheduleTimeLineHourControl.IsAmPmTimeModeProperty, bindIsAMPM);
                timeLineHour.SetBinding(ScheduleTimeLineHourControl.LinesStrokeProperty, bindLinesStroke);
                timeLineHour.SetBinding(ScheduleTimeLineHourControl.TimeIntervalProperty, bindTimeInterval);
                this.Items.Add(timeLineHour);
            }

            this.UpdateHeight();
        }
    }
}
