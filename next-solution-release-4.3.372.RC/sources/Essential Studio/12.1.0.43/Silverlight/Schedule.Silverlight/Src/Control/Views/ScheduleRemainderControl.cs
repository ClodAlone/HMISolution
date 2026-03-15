#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Controls.Schedule;

namespace Syncfusion.Windows.Controls.Schedule
{
    /// <summary>
    /// Represents Schedule's Reminder Control.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
  
    public class ScheduleReminderControl : Control
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleReminderControl"/> class.
        /// </summary>
        public ScheduleReminderControl()
        {
            this.DefaultStyleKey = typeof(ScheduleReminderControl);
        }
#if SILVERLIGHT
        //private Popup pop;
        DataGrid list;       
        //private ScheduleCalendarViewModel model;
#else
        ListView list;
#endif
        private Button closeButton;
        private Button dismissButton;
        private Button dismissAllButton;
        private Button snoozeButton;
        private ComboBox cbox;
        /// <summary>
        ///  new instance for ScheduleReminderWrapper
        /// </summary>
        public ScheduleReminderWrapper wrap = new ScheduleReminderWrapper();
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.closeButton = this.GetTemplateChild("CloseButton") as Button;           
            this.snoozeButton = this.GetTemplateChild("Snooze") as Button;
            this.dismissAllButton = this.GetTemplateChild("DismissAll") as Button;
            this.dismissButton = this.GetTemplateChild("Dismiss") as Button;
            this.cbox = this.GetTemplateChild("SnoozeCombo") as ComboBox;
#if SILVERLIGHT
            this.list = this.GetTemplateChild("List") as DataGrid;
            this.list.SelectedIndex = 0;
#else
            this.list = this.GetTemplateChild("List") as ListView;
            this.list.SelectedIndex = 0;
#endif

            this.closeButton.Click += new RoutedEventHandler(closeButton_Click);
            this.dismissButton.Click += new RoutedEventHandler(dismissButton_Click);
            this.dismissAllButton.Click += new RoutedEventHandler(dismissAllButton_Click);
            this.snoozeButton.Click += new RoutedEventHandler(snoozeButton_Click);

            this.wrap = this.DataContext as ScheduleReminderWrapper;
            this.cbox.SelectedIndex = 0;
           
        }

        /// <summary>
        /// Handles the Click event of the snoozeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void snoozeButton_Click(object sender, RoutedEventArgs e)
        {
            wrap.SnoozeSelectedItem();                   
        }


        /// <summary>
        /// Handles the Click event of the dismissAllButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void dismissAllButton_Click(object sender, RoutedEventArgs e)
        {
            wrap.DismissAllReminders();
        }

        /// <summary>
        /// Handles the Click event of the dismissButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void dismissButton_Click(object sender, RoutedEventArgs e)
        {
            wrap.DismissSelectedItem();              
        }


        /// <summary>
        /// Handles the Click event of the closeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void closeButton_Click(object sender, RoutedEventArgs e)
        {
            wrap.SnoozeAllReminders();
        }     
    }
}
