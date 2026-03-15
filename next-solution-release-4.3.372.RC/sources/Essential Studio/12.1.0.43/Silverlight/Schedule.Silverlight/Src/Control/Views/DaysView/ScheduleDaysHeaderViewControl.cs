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
    /// Represents Schedule's DaysHeader 
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
  
    public class ScheduleDaysHeaderViewControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDaysHeaderViewControl"/> class.
        /// </summary>
        public ScheduleDaysHeaderViewControl()
        {
            this.DefaultStyleKey = typeof(ScheduleDaysHeaderViewControl);
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DayText.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayTextProperty = DependencyProperty.Register("DayText", typeof(string), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the day text.
        /// </summary>
        /// <value>The day text.</value>
        public string DayText
        {
            get
            {
                return (string)this.GetValue(ScheduleDaysHeaderViewControl.DayTextProperty);
            }

            set
            {
                this.SetValue(ScheduleDaysHeaderViewControl.DayTextProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DateText.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateTextProperty = DependencyProperty.Register("DateText", typeof(string), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the date text.
        /// </summary>
        /// <value>The date text.</value>
        public string DateText
        {
            get
            {
                return (string)this.GetValue(ScheduleDaysHeaderViewControl.DateTextProperty);
            }

            set
            {
                this.SetValue(ScheduleDaysHeaderViewControl.DateTextProperty, value);
            }
        }


        #region DateTime (DependencyProperty)

        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>The date time.</value>
        public DateTime DateTime
        {
            get { return (DateTime)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DateTime.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register("DateTime", typeof(DateTime), typeof(ScheduleDaysHeaderViewControl),
              new PropertyMetadata(null));

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsCurrentDateProperty = DependencyProperty.Register("IsCurrentDate", typeof(bool), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(false, OnIsCurrentDateChanged));

        private static void OnIsCurrentDateChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var daysHeader = dpo as ScheduleDaysHeaderViewControl;
            daysHeader.GotoCurrentState();
        }

        private bool isCurrentChangedBeforeTemplateApplied = false;
        private void GotoCurrentState()
        {
            if (this.mainBorder == null)
            {
                this.isCurrentChangedBeforeTemplateApplied = true;
                return;
            }

            if (this.IsCurrentDate && this.IsSelected)
            {
                VisualStateManager.GoToState(this, "CurrentAndSelected", false);
            }
            else
            {
                if (this.IsCurrentDate)
                {
                    VisualStateManager.GoToState(this, "Current", false);
                }
                else if (this.IsSelected)
                {
                    VisualStateManager.GoToState(this, "Selected", false);
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
                return (bool)this.GetValue(ScheduleDaysHeaderViewControl.IsCurrentDateProperty);
            }

            set
            {
                this.SetValue(ScheduleDaysHeaderViewControl.IsCurrentDateProperty, value);
            }
        }

        #region IsSelected (DependencyProperty)

        /// <summary>
        /// Gets / Sets the IsSelected property. Also updates the Selected state.
        /// </summary>
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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ScheduleDaysHeaderViewControl),
              new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var headerView = dpo as ScheduleDaysHeaderViewControl;
            headerView.GotoSelectedState();
        }

        private void GotoSelectedState()
        {
            if (this.IsSelected && this.IsCurrentDate)
            {
                VisualStateManager.GoToState(this, "CurrentAndSelected", false);
            }
            else
            {
                if (this.IsSelected)
                {
                    VisualStateManager.GoToState(this, "Selected", false);
                }
                else if (this.IsCurrentDate)
                {
                    VisualStateManager.GoToState(this, "Current", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", false);
                }
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
            typeof(Brush), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(null));



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
            typeof(Brush), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(null));



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
            typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(null));



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
            typeof(Brush), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(null));



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
            typeof(Thickness), typeof(ScheduleDaysHeaderViewControl), new PropertyMetadata(null));


        #endregion


        #region DaysTextConverter (DependencyProperty)

        /// <summary>
        /// Gets / sets the Text property.
        /// </summary>
        public IValueConverter DaysHeaderTextConverter
        {
            get { return (IValueConverter)GetValue(DaysHeaderTextConverterProperty); }
            set { SetValue(DaysHeaderTextConverterProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DaysHeaderTextConverter.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DaysHeaderTextConverterProperty =
            DependencyProperty.Register("DaysHeaderTextConverter", typeof(IValueConverter), typeof(ScheduleDaysHeaderViewControl),
              new PropertyMetadata(DaysHeaderTextConverterChanged));

        private static void DaysHeaderTextConverterChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var scheduleDaysHeaderViewControl = dpo as ScheduleDaysHeaderViewControl;
            scheduleDaysHeaderViewControl.UpdateConverter((IValueConverter)args.NewValue);
        }

        private void UpdateConverter(IValueConverter newValue)
        {            
            if (this.DaysTextBlock != null)
            {
                Binding TextBinding = new Binding() { Source = this.DayText, Converter = newValue };            
                BindingOperations.SetBinding(this.DaysTextBlock, TextBlock.TextProperty, TextBinding);
            }
        }

        #endregion 

        private Border mainBorder;
        private TextBlock DaysTextBlock;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mainBorder = this.GetTemplateChild("border") as Border;
            if (this.isCurrentChangedBeforeTemplateApplied)
            {
                this.GotoCurrentState();
            }
            DaysTextBlock = this.GetTemplateChild("DaysTextBlock") as TextBlock;
            if (this.DaysHeaderTextConverter!=null && DaysTextBlock != null)
            {
                this.UpdateConverter(this.DaysHeaderTextConverter);
            }
        }
    }
}
