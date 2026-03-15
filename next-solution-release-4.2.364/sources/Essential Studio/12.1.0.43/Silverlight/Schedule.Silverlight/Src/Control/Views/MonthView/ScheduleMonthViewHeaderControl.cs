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
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Threading;
    using System.Windows.Data;
    /// <summary>
    /// Represents Schedule's MonthView Header Collecttion
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
  
    public class ScheduleMonthViewHeaderControl : Control
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleMonthViewHeaderControl"/> class.
        /// </summary>
		public ScheduleMonthViewHeaderControl()
		{
            this.DefaultStyleKey = typeof(ScheduleMonthViewHeaderControl);
		}
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DayText.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayTextProperty = DependencyProperty.Register("DayText", typeof(string), typeof(ScheduleMonthViewHeaderControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the day text.
        /// </summary>
        /// <value>The day text.</value>
        public string DayText
        {
            get
            {
                return (string)this.GetValue(ScheduleMonthViewHeaderControl.DayTextProperty);
            }

            set
            {
                this.SetValue(ScheduleMonthViewHeaderControl.DayTextProperty, value);
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DayOfWeek.  This
        /// enables animation, styling, binding, etc...
        /// </summary>

        public static readonly DependencyProperty DayOfWeekProperty = DependencyProperty.Register("DayOfWeek", typeof(string), typeof(ScheduleMonthViewHeaderControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the day text.
        /// </summary>
        /// <value>The day text.</value>
        public string DayOfWeek
        {
            get
            {
                return (string)this.GetValue(ScheduleMonthViewHeaderControl.DayOfWeekProperty);
            }

            set
            {
                this.SetValue(ScheduleMonthViewHeaderControl.DayOfWeekProperty, value);
            }
        }


        #region MonthHeaderTextConverter (DependencyProperty)

        /// <summary>
        /// Gets / sets the Text property.
        /// </summary>
        public IValueConverter MonthHeaderTextConverter
        {
            get { return (IValueConverter)GetValue(MonthHeaderTextConverterProperty); }
            set { SetValue(MonthHeaderTextConverterProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for MonthHeaderTextConverter.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthHeaderTextConverterProperty =
            DependencyProperty.Register("MonthHeaderTextConverter", typeof(IValueConverter), typeof(ScheduleMonthViewHeaderControl),
              new PropertyMetadata(MonthHeaderTextConverterChanged));

        private static void MonthHeaderTextConverterChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var scheduleMonthViewHeaderControl = dpo as ScheduleMonthViewHeaderControl;
            scheduleMonthViewHeaderControl.UpdateConverter((IValueConverter)args.NewValue);
        }

        private void UpdateConverter(IValueConverter newValue)
        {
            if (this.MonthHeaderTextBlock != null)
            {
                Binding TextBinding = new Binding() { Source = this.DayText, Converter = newValue };
                BindingOperations.SetBinding(this.MonthHeaderTextBlock, TextBlock.TextProperty, TextBinding);
            }
        }

        #endregion 

        private TextBlock MonthHeaderTextBlock;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            MonthHeaderTextBlock = this.GetTemplateChild("DayText") as TextBlock;

            if (this.MonthHeaderTextConverter != null && MonthHeaderTextBlock != null)
            {
                this.UpdateConverter(this.MonthHeaderTextConverter);
            }
        }
	}     
      
}