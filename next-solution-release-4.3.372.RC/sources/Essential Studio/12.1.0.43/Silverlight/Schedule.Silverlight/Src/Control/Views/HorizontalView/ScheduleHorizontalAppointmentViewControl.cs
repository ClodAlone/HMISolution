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
    using System.Linq;
    using System.Collections.Generic;
    /// <summary>
    /// Represents Schedule's HorizontalAppointmentViewControl
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
 
    public class ScheduleHorizontalAppointmentViewControl : Control
    {
        /// <summary>
        ///  Sets double value 4
        /// </summary>
        public const double ShadowDepth = 4.0;

        /// <summary>
        /// Sets double value 25
        /// </summary>
        public const double AddAppointmentPadding = 25d;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleHorizontalAppointmentViewControl"/> class.
        /// </summary>
        public ScheduleHorizontalAppointmentViewControl()
        {
            this.DefaultStyleKey = typeof(ScheduleHorizontalAppointmentViewControl);
        }

        private Border PART_RightEdge;
        private Border PART_LeftEdge;
        /// <summary>
        ///  reference for mouse pointer type
        /// </summary>
        public MousePointerType DragStatus;
        /// <summary>
        ///  reference for mouse resize position
        /// </summary>
        public ResizePosition MousePosition;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.PART_LeftEdge = this.GetTemplateChild("PART_LeftEdge") as Border;
            this.PART_RightEdge = this.GetTemplateChild("PART_RightEdge") as Border;
            this.SetUpEvents();
            base.OnApplyTemplate();
            //this.SetVisualStates();
        }

        private void SetUpEvents()
        {
#if SILVERLIGHT
            this.PART_LeftEdge.MouseEnter += new MouseEventHandler(ScheduleDaysAppointmentViewControl_MouseEnter);
            this.PART_LeftEdge.MouseLeave += new MouseEventHandler(ScheduleDaysAppointmentViewControl_MouseLeave);
            this.PART_RightEdge.MouseEnter += new MouseEventHandler(ScheduleDaysAppointmentViewControl_MouseEnter);
            this.PART_RightEdge.MouseLeave += new MouseEventHandler(ScheduleDaysAppointmentViewControl_MouseLeave);
#endif
        }

        private void ScheduleDaysAppointmentViewControl_MouseLeave(object sender, MouseEventArgs e)
        {
            DragStatus = MousePointerType.None;
            MousePosition = ResizePosition.None;
        }

        void ScheduleDaysAppointmentViewControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((sender as FrameworkElement).Name == "PART_LeftEdge")
            {
                MousePosition = ResizePosition.Left;
            }
            else if ((sender as FrameworkElement).Name == "PART_RightEdge")
            {
                MousePosition = ResizePosition.Right;
            }
            DragStatus = MousePointerType.Resize;
        }

        //private void SetVisualStates()
        //{
        //    if (this.ScheduleAppointment == null) return;
        //    switch (this.ScheduleAppointment.Priority)
        //    {
        //        case AppointmentPriority.Busy:
        //            VisualStateManager.GoToState(this, "Busy", false);
        //            break;
        //        case AppointmentPriority.Free:
        //            VisualStateManager.GoToState(this, "Free", false);
        //            break;
        //        case AppointmentPriority.OutOfOffice:
        //            VisualStateManager.GoToState(this, "OutOfOffice", false);
        //            break;
        //        case AppointmentPriority.Tentative:
        //            VisualStateManager.GoToState(this, "Tentative", false);
        //            break;
        //    }
        //}


        /// <summary>
        /// Gets or sets the schedule appointment.
        /// </summary>
        /// <value>The schedule appointment.</value>
        public ScheduleAppointment ScheduleAppointment
        {
            get
            {
                return this.DataContext as ScheduleAppointment;
            }
            set
            {
                if (this.DataContext != value)
                {
                    this.DataContext = value;
                    //this.SetVisualStates();
                }
            }
        }

        #region IsSelected (DependencyProperty)

        /// <summary>
        /// Gets / Sets the IsSelected property.
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
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ScheduleHorizontalAppointmentViewControl),
              new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var appView = dpo as ScheduleHorizontalAppointmentViewControl;
            appView.GotoIsSelected();
        }

        private void GotoIsSelected()
        {
            if (this.IsSelected)
            {
                VisualStateManager.GoToState(this, "Selected", false); 
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        #endregion
    }
}
