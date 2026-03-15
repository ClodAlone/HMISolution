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
    using System.Collections.Generic;
    /// <summary>
    /// Represents Schedule's DaysView Grid lines
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class ScheduleTimeSlotControl : Control, IScheduleCalendarViewModelHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleTimeSlotControl"/> class.
        /// </summary>
        public ScheduleTimeSlotControl()
        {
            this.DefaultStyleKey = typeof(ScheduleTimeSlotControl);

#if SILVERLIGHT
            if (DesignerProperties.IsInDesignTool)
            {
                var calendarViewModel = new ScheduleCalendarViewModel();
                var dates = new System.Collections.ObjectModel.ObservableCollection<DateTime>();
                dates.Add(DateTime.Now);
                dates.Add(DateTime.Now.Date.AddDays(1));
                calendarViewModel.SelectedDates = dates;
                calendarViewModel.CurrentTimeInterval = TimeInterval.ThirtyMin;
                this.SetCalendarViewModel(calendarViewModel);
            }
#endif
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

        void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.Model.CurrentScheduleType != ScheduleType.Day && this.Model.CurrentScheduleType != ScheduleType.Week
                && this.Model.CurrentScheduleType != ScheduleType.WorkWeek) return;

            if (e.PropertyName == "CurrentTimeInterval")
            {
                this.GenerateTimeSlots();
            }
        }

        internal void SetCalendarViewModel(ScheduleCalendarViewModel model)
        {
            if (this.model != null)
            {
                this.model.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
            }
            this.model = model;
            if (this.isTemplateApplied)
            {
                this.GenerateTimeSlots();
            }
            this.model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
        }

        #region IsCurrentDate
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsCurrentDate.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static DependencyProperty IsCurrentDateProperty = DependencyProperty.Register("IsCurrentDate", typeof(bool), typeof(ScheduleTimeSlotControl), new PropertyMetadata(OnIsCurrentDateChanged));

        private bool isCurrentChangedBeforeTemplateApplied = false;
        private static void OnIsCurrentDateChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeSlot = dpo as ScheduleTimeSlotControl;
            if (!timeSlot.isTemplateApplied)
            {
                timeSlot.isCurrentChangedBeforeTemplateApplied = true;
            }
            else
            {
                timeSlot.GotoIsCurrentDateState();
            }
        }

        private void GotoIsCurrentDateState()
        {
            if (this.IsCurrentDate && this.IsShaded)
            {
                VisualStateManager.GoToState(this, "IsCurrentAndShaded", false);
            }
            else
            {
                if (this.IsCurrentDate)
                {
                    VisualStateManager.GoToState(this, "IsCurrent", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", false);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is current date.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is current date; otherwise, <c>false</c>.
        /// </value>
        public bool IsCurrentDate
        {
            get
            {
                return (bool)this.GetValue(ScheduleTimeSlotControl.IsCurrentDateProperty);
            }

            set
            {
                this.SetValue(ScheduleTimeSlotControl.IsCurrentDateProperty, value);
            }
        }

        #endregion

        #region LinesStroke

        /// <summary>
        ///  Using a DependencyProperty as the backing store for LinesStroke.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LinesStrokeProperty = DependencyProperty.Register("LinesStroke",
            typeof(Brush), typeof(ScheduleTimeSlotControl), new PropertyMetadata(OnLineStrokeChanged));
        private static void OnLineStrokeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeslot = dpo as ScheduleTimeSlotControl;
            timeslot.GenerateTimeSlots();
        }
        /// <summary>
        /// Gets or sets the lines' stroke.
        /// </summary>
        /// <value>The lines stroke.</value>
        public Brush LinesStroke
        {
            get
            {
                return (Brush)GetValue(ScheduleTimeSlotControl.LinesStrokeProperty);
            }

            set
            {
                SetValue(ScheduleTimeSlotControl.LinesStrokeProperty, value);
            }
        }

        #endregion

        #region ScheduleBackground (DependencyProperty)

        /// <summary>
        /// Gets / Sets the SelectionBackground property.
        /// </summary>
        public Brush ScheduleBackground
        {
            get { return (Brush)GetValue(ScheduleBackgroundProperty); }
            set { SetValue(ScheduleBackgroundProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ScheduleBackground.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScheduleBackgroundProperty = DependencyProperty.Register("ScheduleBackground", 
            typeof(Brush), typeof(ScheduleTimeSlotControl), new PropertyMetadata(null));       

        #endregion

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
            typeof(Brush), typeof(ScheduleTimeSlotControl), new PropertyMetadata(null));      

        #endregion

        #region ShadedBackground (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ShadedBackground property.
        /// </summary>
        public Brush ShadedBackground
        {
            get { return (Brush)GetValue(ShadedBackgroundProperty); }
            set { SetValue(ShadedBackgroundProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ShadedBackground.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShadedBackgroundProperty = DependencyProperty.Register("ShadedBackground",
            typeof(Brush), typeof(ScheduleTimeSlotControl), new PropertyMetadata(null));

         #endregion

        #region StrokeLine (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ShadedBackground property.
        /// </summary>
        public Brush StrokeLine
        {
            get { return (Brush)GetValue(StrokeLineProperty); }
            set { SetValue(StrokeLineProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for StrokeLine.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeLineProperty = DependencyProperty.Register("StrokeLine",
            typeof(Brush), typeof(ScheduleTimeSlotControl), new PropertyMetadata(null));


        #endregion

        #region StrokeThickness (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ShadedBackground property.
        /// </summary>
        public Thickness StrokeThickness
        {
            get { return (Thickness)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for StrokeThickness.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness",
            typeof(Thickness), typeof(ScheduleTimeSlotControl), new PropertyMetadata(null));
      
        #endregion

        #region IsShaded
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsShaded.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static DependencyProperty IsShadedProperty = DependencyProperty.Register("IsShaded", typeof(bool), typeof(ScheduleTimeSlotControl), new PropertyMetadata(OnIsShadedChanged));

        private bool isShadedAppliedBeforeTemplateLoaded = false;
        private static void OnIsShadedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeSlot = dpo as ScheduleTimeSlotControl;
            if (!timeSlot.isTemplateApplied)
            {
                timeSlot.isShadedAppliedBeforeTemplateLoaded = true;
            }
            else
            {
                timeSlot.GotoShadedState();
            }
        }

        private void GotoShadedState()
        {
            if (this.IsShaded && this.IsCurrentDate)
            {
                VisualStateManager.GoToState(this, "IsCurrentAndShaded", false);
            }
            else
            {
                if (this.IsShaded)
                {
                    VisualStateManager.GoToState(this, "Shaded", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", false);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is shaded.
        /// </summary>
        /// <value><c>true</c> if this instance is shaded; otherwise, <c>false</c>.</value>
        public bool IsShaded
        {
            get
            {
                return (bool)this.GetValue(ScheduleTimeSlotControl.IsShadedProperty);
            }

            set
            {
                this.SetValue(ScheduleTimeSlotControl.IsShadedProperty, value);
            }
        }

        #endregion

        #region Hour

        /// <summary>
        ///  Using a DependencyProperty as the backing store for Hour.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HourProperty = DependencyProperty.Register("Hour", typeof(int), typeof(ScheduleTimeSlotControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the hour.
        /// </summary>
        /// <value>The hour.</value>
        public int Hour
        {
            get
            {
                return (int)this.GetValue(ScheduleTimeSlotControl.HourProperty);
            }

            set
            {
                this.SetValue(ScheduleTimeSlotControl.HourProperty, value);
            }
        }
        #endregion

        #region DateTime
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DateTime.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateTimeProperty = DependencyProperty.Register("DateTime", typeof(DateTime), typeof(ScheduleTimeSlotControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>The date time.</value>
        public DateTime DateTime
        {
            get
            {
                return (DateTime)this.GetValue(ScheduleTimeSlotControl.DateTimeProperty);
            }

            set
            {
                this.SetValue(ScheduleTimeSlotControl.DateTimeProperty, value);
            }
        }
        #endregion
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsFirstItem.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsFirstItemProperty = DependencyProperty.Register("IsFirstItem", typeof(bool), typeof(ScheduleTimeSlotControl), new PropertyMetadata(false, OnIsFirstItemChanged));

        private bool isFirstItemChangedBeforeLoaded = false;
        private static void OnIsFirstItemChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeSlot = dpo as ScheduleTimeSlotControl;
            if (!timeSlot.isTemplateApplied)
            {
                timeSlot.isFirstItemChangedBeforeLoaded = true;
            }
            else
            {
                timeSlot.AdjustBorderForFirstItem();
            }
        }

        private void AdjustBorderForFirstItem()
        {
            if (this.mainBorder == null)
            {
                return;
            }

            if (this.IsFirstItem)
            {
                this.mainBorder.BorderThickness = new Thickness(1, 0, 1, 0);
            }
            else
            {
                this.mainBorder.BorderThickness = new Thickness(1, 1, 1, 0);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is first item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first item; otherwise, <c>false</c>.
        /// </value>
        public bool IsFirstItem
        {
            get
            {
                return (bool)this.GetValue(ScheduleTimeSlotControl.IsFirstItemProperty);
            }

            set
            {
                this.SetValue(ScheduleTimeSlotControl.IsFirstItemProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsLastItem.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLastItemProperty = DependencyProperty.Register("IsLastItem", typeof(bool), typeof(ScheduleTimeSlotControl), new PropertyMetadata(false, OnIsLastItemChanged));

        private bool isLastItemChangedBeforeLoaded = false;
        private static void OnIsLastItemChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeSlot = dpo as ScheduleTimeSlotControl;
            if (!timeSlot.isTemplateApplied)
            {
                timeSlot.isLastItemChangedBeforeLoaded = true;
            }
            else
            {
                timeSlot.AdjustBorderForLastItem();
            }
        }

        private void AdjustBorderForLastItem()
        {
            if (this.mainBorder == null)
            {
                return;
            }

            if (this.IsLastItem)
            {
                this.mainBorder.BorderThickness = new Thickness(1, 1, 1, 1);
            }
            else
            {
                this.mainBorder.BorderThickness = new Thickness(1, 0, 1, 0);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is last item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is last item; otherwise, <c>false</c>.
        /// </value>
        public bool IsLastItem
        {
            get
            {
                return (bool)this.GetValue(ScheduleTimeSlotControl.IsLastItemProperty);
            }

            set
            {
                this.SetValue(ScheduleTimeSlotControl.IsLastItemProperty, value);
            }
        }

        private bool isTemplateApplied = false;
#if Test
        internal UniformStackPanel timeSlotsPanel;
#else
        private UniformStackPanel timeSlotsPanel;
#endif
        private ScheduleRectangleBorder mainBorder;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see
        /// cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.timeSlotsPanel = this.GetTemplateChild("PART_TimeSlotPanel") as UniformStackPanel;
            this.mainBorder = this.GetTemplateChild("PART_Border") as ScheduleRectangleBorder;
            this.isTemplateApplied = true;
            if (this.isShadedAppliedBeforeTemplateLoaded)
            {
                this.GotoShadedState();
            }

            if (this.isCurrentChangedBeforeTemplateApplied)
            {
                this.GotoIsCurrentDateState();
            }

            if (this.isFirstItemChangedBeforeLoaded)
            {
                this.AdjustBorderForFirstItem();
            }

            if (this.isLastItemChangedBeforeLoaded)
            {
                this.AdjustBorderForLastItem();
            }
            this.GenerateTimeSlots();
        }

        /// <summary>
        /// Occurs when [time slots generated].
        /// </summary>
        public event EventHandler TimeSlotsGenerated;
        private void GenerateTimeSlots()
        {
            if (this.Model == null)
            {
                return;
            }
            if (this.timeSlotsPanel == null)
                return;
            this.timeSlotsPanel.Children.Clear();
            var intervalsCount = this.Model.GetTimeSlotIntervals();
            int totalIntervals = intervalsCount;            
            for (int i = 0;i < totalIntervals;i++)
            {
                var isVisible = i % intervalsCount != 0;
                var rectangle = new ScheduleRectangleBorderExt()
                {
                    TopBrush = isVisible ? this.LinesStroke : null,
                    BorderThickness = new Thickness(0, 0.35, 0, 0),
                    SelectionBackground = this.model.SelectionBackground
                };
                this.timeSlotsPanel.Children.Add(rectangle);
            }

            var handler = this.TimeSlotsGenerated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
