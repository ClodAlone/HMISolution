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
    using System.Windows.Media.Imaging;
    using System.Collections.ObjectModel;
    /// <summary>
    /// Represents Schedule's Month DateContainer
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
   
    public class ScheduleMonthDateContentControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleMonthDateContentControl"/> class.
        /// </summary>
        public ScheduleMonthDateContentControl()
        {
            this.DefaultStyleKey = typeof(ScheduleMonthDateContentControl);
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DateText.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateTextProperty = DependencyProperty.Register("DateText", typeof(string), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the date text.
        /// </summary>
        /// <value>The date text.</value>
        public string DateText
        {
            get
            {
                return (string)this.GetValue(ScheduleMonthDateContentControl.DateTextProperty);
            }

            set
            {
                this.SetValue(ScheduleMonthDateContentControl.DateTextProperty, value);
            }
        }



        /// <summary>
        /// Gets or sets the Navigator button opacity in the ScheduleMonthDateContentControl 
        /// </summary>
        public double NavigatorButtonOpacity
        {
            get
            {
                return (double)this.GetValue(ScheduleMonthDateContentControl.NavigatorButtonOpacityProperty);
            }

            set
            {
                this.SetValue(ScheduleMonthDateContentControl.NavigatorButtonOpacityProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for NavigatorButtonOpacity.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NavigatorButtonOpacityProperty = DependencyProperty.Register("NavigatorButtonOpacity", typeof(double), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(0d, OnNavigatorOverFlowingChanged));

        /// <summary>
        ///  Using a DependencyProperty as the backing store for Date.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(DateTime), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(DateTime.Now.Date));

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime Date
        {
            get
            {
                return (DateTime)this.GetValue(ScheduleMonthDateContentControl.DateProperty);
            }

            set
            {
                this.SetValue(ScheduleMonthDateContentControl.DateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsSelected.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ScheduleMonthDateContentControl),
              new PropertyMetadata(false, OnIsSelectedChanged));


        private static void OnIsSelectedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var content = dpo as ScheduleMonthDateContentControl;
            content.GotoVisualState();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is current month.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is current month; otherwise, <c>false</c>.
        /// </value>
        public bool IsCurrentMonth
        {
            get { return (bool)GetValue(IsCurrentMonthProperty); }
            set { SetValue(IsCurrentMonthProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsCurrentMonth.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCurrentMonthProperty =
            DependencyProperty.Register("IsCurrentMonth", typeof(bool), typeof(ScheduleMonthDateContentControl),
              new PropertyMetadata(true, OnIsCurrentMonthChanged));

        private static void OnIsCurrentMonthChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var content = dpo as ScheduleMonthDateContentControl;
            content.GotoCurrentMonthState();
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is having appointment more than its capacity.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is current month; otherwise, <c>false</c>.
        /// </value>
        public bool IsAppointmentOverFlowing
        {
            get { return (bool)GetValue(IsAppointmentOverFlowingProperty); }
            set { SetValue(IsAppointmentOverFlowingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsAppointmentOverFlowing.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAppointmentOverFlowingProperty =
            DependencyProperty.Register("IsAppointmentOverFlowing", typeof(bool), typeof(ScheduleMonthDateContentControl),
              new PropertyMetadata(false, OnIsAppointmentOverFlowingChanged));

        private static void OnIsAppointmentOverFlowingChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var content = dpo as ScheduleMonthDateContentControl;
            content.GotoAppointmentOverFlowingState();
        }

        private static void OnNavigatorOverFlowingChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var content = dpo as ScheduleMonthDateContentControl;
            content.NavigatorButtonOpacity = (double)args.NewValue;
        }


        private void GotoAppointmentOverFlowingState()
        {
            if (!this.isTemplateApplied)
            {
                return;
            }
            if (IsAppointmentOverFlowing)
                VisualStateManager.GoToState(this, "OverFlowing", false);
            else
                VisualStateManager.GoToState(this, "NonOverFlowing", false); 
            
        }

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
            typeof(Brush), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(null));

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
            typeof(Brush), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(null));



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
            typeof(Brush), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(null));

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
            typeof(Thickness), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(null));

        #endregion

        #region HeaderBrush (DependencyProperty)

        /// <summary>
        /// Gets / Sets the HeaderBrush property.
        /// </summary>
        public Brush HeaderBrush
        {
            get { return (Brush)GetValue(HeaderBrushProperty); }
            set { SetValue(HeaderBrushProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for HeaderBrush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderBrushProperty = DependencyProperty.Register("HeaderBrush", typeof(Brush),
            typeof(ScheduleMonthDateContentControl), new PropertyMetadata(null));



        #endregion

        private void GotoCurrentMonthState()
        {
            if (!this.isTemplateApplied)
            {
                return;
            }

            if (!IsCurrentMonth)
            {
                VisualStateManager.GoToState(this, "NotCurrentMonth", false);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsCurrentDate.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCurrentDateProperty = DependencyProperty.Register("IsCurrentDate", typeof(bool), typeof(ScheduleMonthDateContentControl), new PropertyMetadata(false, OnIsCurrentDateChanged));

        private static void OnIsCurrentDateChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var current = dpo as ScheduleMonthDateContentControl;
            current.GotoVisualState();
        }

        private void GotoVisualState()
        {
            if (!this.isTemplateApplied)
            {
                return;
            }
           
                if (this.IsCurrentDate && this.IsSelected)
                {
                    VisualStateManager.GoToState(this, "CurrentDateSelected", false);
                }
                else if (!this.IsSelected && this.IsCurrentDate)
                {
                    VisualStateManager.GoToState(this, "CurrentDateNormal", false);
                }
                else if (!this.IsCurrentDate && this.IsSelected)
                {
                    VisualStateManager.GoToState(this, "ContentSelected", false);
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
                return (bool)this.GetValue(ScheduleMonthDateContentControl.IsCurrentDateProperty);
            }

            set
            {
                this.SetValue(ScheduleMonthDateContentControl.IsCurrentDateProperty, value);
            }
        }

       
        private Button navigatorButton;
        private bool isTemplateApplied = false;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.isTemplateApplied = true;
            this.GotoVisualState();
            this.GotoCurrentMonthState();
            this.navigatorButton = this.GetTemplateChild("Navigation") as Button;
            this.navigatorButton.Click += new RoutedEventHandler(navigatorButton_Click);
        }

        void navigatorButton_Click(object sender, RoutedEventArgs e)
        {
            var content = this.FindParentElementOfType<ScheduleMonthView>();
            ObservableCollection<DateTime> dates = new ObservableCollection<DateTime>();
            dates.Add(this.Date);
            content.Model.SelectedDates = dates;
        }
    }
}
