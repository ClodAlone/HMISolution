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

    /// <summary>
    /// Represents Schedule's HorizontalDaysHeaderViewControl
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleHorizontalDaysHeaderViewControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalDaysHeaderViewControl"/>
        /// class.
        /// </summary>
        public ScheduleHorizontalDaysHeaderViewControl()
        {
            this.DefaultStyleKey = typeof(ScheduleHorizontalDaysHeaderViewControl);
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DateText.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DateTextProperty = DependencyProperty.Register("DateText", typeof(string), typeof(ScheduleHorizontalDaysHeaderViewControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the date text.
        /// </summary>
        /// <value>The date text.</value>
        public string DateText
        {
            get
            {
                return (string)this.GetValue(ScheduleHorizontalDaysHeaderViewControl.DateTextProperty);
            }

            set
            {
                this.SetValue(ScheduleHorizontalDaysHeaderViewControl.DateTextProperty, value);
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
            DependencyProperty.Register("DateTime", typeof(DateTime), typeof(ScheduleHorizontalDaysHeaderViewControl),
              new PropertyMetadata(null));

        #endregion

        private Border mainBorder;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mainBorder = this.GetTemplateChild("border") as Border;           
        }
    }
}
