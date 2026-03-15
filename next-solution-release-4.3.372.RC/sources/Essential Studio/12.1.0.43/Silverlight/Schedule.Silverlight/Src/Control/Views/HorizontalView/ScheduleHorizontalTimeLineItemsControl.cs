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
    using System.Windows.Data;
    /// <summary>
    /// Represents Schedule's HorizontalTimeLineItemsControl
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
   
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ScheduleHorizontalTimeLineHourControl))]
    public class ScheduleHorizontalTimeLineItemsControl : ItemsControl, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleHorizontalTimeLineItemsControl"/> class.
        /// </summary>
        public ScheduleHorizontalTimeLineItemsControl()
        {
            this.DefaultStyleKey = typeof(ScheduleHorizontalTimeLineItemsControl);
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

        #region TimeInterval

        /// <summary>
        ///  Using a DependencyProperty as the backing store for TimeInterval.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeIntervalProperty = DependencyProperty.Register("TimeInterval",
            typeof(TimeInterval), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(TimeInterval.OneHour, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeLineItemsControl = dpo as ScheduleHorizontalTimeLineItemsControl;
            if (timeLineItemsControl.isTemplateApplied)
            {
                timeLineItemsControl.SetupHours(timeLineItemsControl.Model.SelectedDates.Count);
            }
        }

        #region StartWorkHour
        /// <summary>
        ///  Using a DependencyProperty as the backing store for StartWorkHour.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartWorkHourProperty = DependencyProperty.Register("StartWorkHour", typeof(int),
            typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(8, new PropertyChangedCallback(OnStartWorkHourChanged)));

        private static void OnStartWorkHourChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeLineItemsControl = dpo as ScheduleHorizontalTimeLineItemsControl;
            if (timeLineItemsControl.isTemplateApplied)
            {
                timeLineItemsControl.SetupHours(timeLineItemsControl.Model.SelectedDates.Count);
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
        public static readonly DependencyProperty EndWorkHourProperty = DependencyProperty.Register("EndWorkHour", typeof(int),
            typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(17, new PropertyChangedCallback(OnEndWorkHourChanged)));

        private static void OnEndWorkHourChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeLineItemsControl = dpo as ScheduleHorizontalTimeLineItemsControl;
            if (timeLineItemsControl.isTemplateApplied)
            {
                timeLineItemsControl.SetupHours(timeLineItemsControl.Model.SelectedDates.Count);
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
        public static readonly DependencyProperty IsAmPmTimeModeProperty = DependencyProperty.Register("IsAmPmTimeMode", typeof(bool),
            typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(true));

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

        #region ItemContainerStyle

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ItemContainerStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
#if SILVERLIGHT
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
           typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(null));
#else
        public static readonly new DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style),
            typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(null));
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
        public static readonly DependencyProperty LinesStrokeProperty = DependencyProperty.Register("LinesStroke", typeof(Brush),
            typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the lines' stroke.
        /// </summary>
        /// <value>The lines stroke.</value>
        public Brush LinesStroke
        {
            get
            {
                return (Brush)GetValue(ScheduleHorizontalTimeLineItemsControl.LinesStrokeProperty);
            }

            set
            {
                SetValue(ScheduleHorizontalTimeLineItemsControl.LinesStrokeProperty, value);
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets TimeInterval.
        /// </summary>
        public TimeInterval TimeInterval
        {
            get
            {
                return (TimeInterval)GetValue(ScheduleHorizontalTimeLineItemsControl.TimeIntervalProperty);
            }

            set
            {
                SetValue(ScheduleHorizontalTimeLineItemsControl.TimeIntervalProperty, value);
            }
        }

        #endregion

        #region TimelineVisibility (DependencyProperty)
        /// <summary>
        /// Initialize DefaultTimeLineVisibility as Visible
        /// </summary>
        public const Visibility DefaultTimeLineVisibility= Visibility.Visible;
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TimelineVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimelineVisibilityProperty = DependencyProperty.Register("TimelineVisibility",
            typeof(Visibility), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(DefaultTimeLineVisibility));

        /// <summary>
        /// Gets / Sets the ScheduleTimelineVisibility.
        /// </summary>
        public Visibility TimelineVisibility
        {
            get { return (Visibility)GetValue(TimelineVisibilityProperty); }
            set { SetValue(TimelineVisibilityProperty, value); }
        }
        #endregion

        #region IntervalWidth

        private void UpdateWidth()
        {
            if (this.Model == null)
            {
                return;
            }

            this.Width = this.Model.GetTimeSlotWidth();
        }

        /// <summary>
        /// Initialize constant double as 24
        /// </summary>
        public const double DefaultIntervalWidth = 24d;
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IntervalWidth.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalWidthProperty = DependencyProperty.Register("IntervalWidth", typeof(double),
            typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(DefaultIntervalWidth, new PropertyChangedCallback(OnIntervalWidthChanged)));

        private static void OnIntervalWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleHorizontalTimeLineItemsControl instance = (ScheduleHorizontalTimeLineItemsControl)d;
            instance.OnIntervalWidthChanged(e);
        }

        private void OnIntervalWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateWidth();
        }

        /// <summary>
        /// Gets or sets the height of the inner hour interval.
        /// </summary>
        public double IntervalWidth
        {
            get
            {
                return (double)GetValue(IntervalWidthProperty);
            }

            set
            {
                SetValue(IntervalWidthProperty, value);
            }
        }

        #endregion

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
                this.SetupHours(this.Model.SelectedDates.Count);
            }
        }


        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var horizontalTimeHourControl = new ScheduleHorizontalTimeLineHourControl();
            horizontalTimeHourControl.BorderBrush = this.BorderBrush;
            horizontalTimeHourControl.Background = this.Background;
            horizontalTimeHourControl.BorderThickness = this.BorderThickness;
            horizontalTimeHourControl.TimelineVisibility = this.TimelineVisibility;
            return horizontalTimeHourControl;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item. 
        /// </summary>
        /// <param name="item">Specified item.</param>
        /// <param name="element">Element used to display the specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var control = item as ScheduleHorizontalTimeLineHourControl;
            var controlStyle = this.ItemContainerStyle;
            if (controlStyle != null && control.Style == null)
            {
                control.Style = controlStyle;
            }
        }

        internal void SetCalendarViewModel(ScheduleCalendarViewModel model)
        {
            if (this.model != null)
            {
                this.model.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
            }

            this.model = model;
            this.model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
            
            this.ClearValue(ScheduleHorizontalTimeLineItemsControl.TimeIntervalProperty);
            var bindTimeInterval = new Binding("CurrentTimeInterval") { Source = this.model, Mode = BindingMode.TwoWay };
            this.SetBinding(ScheduleHorizontalTimeLineItemsControl.TimeIntervalProperty, bindTimeInterval);

            this.ClearValue(ScheduleHorizontalTimeLineItemsControl.IntervalWidthProperty);
            var bindIntervalWidth = new Binding("IntervalWidth") { Source = this.model, Mode = BindingMode.TwoWay };
            this.SetBinding(ScheduleHorizontalTimeLineItemsControl.IntervalWidthProperty, bindIntervalWidth);
        }

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.Model.CurrentScheduleType != ScheduleType.ScheduleView) return;

            if (e.PropertyName == "SelectedDates" || e.PropertyName == "CurrentTimeInterval")
            {
                this.GenerateTimeLineItems(this.Model.SelectedDates.Count);
            }
            else if (e.PropertyName == "IsAmPmTimeMode")
            {
                this.IsAmPmTimeMode = this.Model.IsAmPmTimeMode;
            }
            else if (e.PropertyName == "IntervalHeight")
            {
                this.IntervalWidth = this.Model.IntervalHeight;
            }
        }

        private void GenerateTimeLineItems(int cntVal)
        {
            this.SetupHours(cntVal);
        }

        private void SetupHours(int selectedDatesCount)
        {
            if (this.model == null)
            {
                return;
            }

            foreach (ScheduleHorizontalTimeLineHourControl item in this.Items)
            {
                item.ClearValue(ScheduleHorizontalTimeLineHourControl.IsAmPmTimeModeProperty);
                item.ClearValue(ScheduleHorizontalTimeLineHourControl.LinesStrokeProperty);
                item.ClearValue(ScheduleHorizontalTimeLineHourControl.TimeIntervalProperty);
            }

            this.Items.Clear();

            for (int cnt = 0; cnt < selectedDatesCount; cnt++)
            {
                Binding bindIsAMPM = new Binding("IsAmPmTimeMode") { Source = this };
                var bindLinesStroke = new Binding("LinesStroke") { Source = this };
                var bindTimeInterval = new Binding("TimeInterval") { Source = this };
                for (int i = ScheduleHorizontalTimeLineHourControl.MinValue; i <= ScheduleHorizontalTimeLineHourControl.MaxValue; i++)
                {
                    var timeLineHour = (ScheduleHorizontalTimeLineHourControl)this.GetContainerForItemOverride();
                    timeLineHour.Hour = i;
                    timeLineHour.SetBinding(ScheduleHorizontalTimeLineHourControl.IsAmPmTimeModeProperty, bindIsAMPM);
                    timeLineHour.SetBinding(ScheduleHorizontalTimeLineHourControl.LinesStrokeProperty, bindLinesStroke);
                    timeLineHour.SetBinding(ScheduleHorizontalTimeLineHourControl.TimeIntervalProperty, bindTimeInterval);
                    this.Items.Add(timeLineHour);
                }

                if (cnt == selectedDatesCount - 1)
                {
                    this.BorderThickness = new Thickness(0, 0, 1, 0);
                    this.BorderBrush = this.LinesStroke;
                }
            }
            this.UpdateWidth();
        }
    }
}
