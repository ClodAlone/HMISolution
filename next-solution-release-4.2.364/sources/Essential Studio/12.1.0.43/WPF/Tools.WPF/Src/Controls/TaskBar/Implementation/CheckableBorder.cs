// <copyright file="CheckableBorder.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class represents extended <see cref="Border"/> control.
    /// It is control with internal purpose.
    /// It catches <see cref="ToggleButton.Checked"/> and <see cref="ToggleButton.Unchecked"/> routed events
    /// from <see cref="ToggleButton"/> control in which it is contained.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CheckableBorder : Border
    {
        #region RoutedEvent

        /// <summary>
        /// Checked event which is used for border checking. This event
        /// sets instance of its TemplateParent.
        /// </summary>
        public static readonly RoutedEvent CheckedEvent;

        /// <summary>
        /// Unchecked event which is used for border checking. This event
        /// sets instance of its TemplateParent.
        /// </summary>
        public static readonly RoutedEvent UncheckedEvent;

        #endregion RoutedEvent

        #region Initialize/Finalize Methods

        /// <summary>
        /// Initializes static members of the <see cref="CheckableBorder"/> class.
        /// </summary>
        static CheckableBorder()
        {
            CheckedEvent = EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(CheckableBorder));
            UncheckedEvent = EventManager.RegisterRoutedEvent("Unchecked", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(CheckableBorder));
        }

        #endregion Initialize/Finalize Methods

        #region Implementation

        /// <summary>
        /// This method initializes check/uncheck event.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            ToggleButton parent = TemplatedParent as ToggleButton;

            if (null != parent)
            {
                parent.AddHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(ProcessChecked), true);
                parent.AddHandler(ToggleButton.UncheckedEvent, new RoutedEventHandler(ProcessUnchecked), true);

                if (parent.IsChecked.HasValue && parent.IsChecked.Value)
                {
                    FireChecked();
                }
                else
                {
                    FireUnchecked();
                }
            }
        }

        /// <summary>
        /// Fires <see cref="CheckedEvent"/> routed event.
        /// </summary>
        protected void FireChecked()
        {
            RoutedEventArgs args = new RoutedEventArgs(CheckedEvent);
            try
            {
                RaiseEvent(args);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Fires <see cref="UncheckedEvent"/> routed event.
        /// </summary>
        protected void FireUnchecked()
        {
            RoutedEventArgs args = new RoutedEventArgs(UncheckedEvent);
            try
            {
                RaiseEvent(args);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Method which is used for check processing.
        /// </summary>
        /// <param name="sender">Object which raises event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void ProcessChecked(object sender, RoutedEventArgs e)
        {
            TaskBarItem Item = VisualUtils.FindAncestor(this, typeof(TaskBarItem)) as TaskBarItem;
            if (Item != null)
            {
                if (Item.AllowExpand == true)
                {
                    FireChecked();
                }
            }
        }

        /// <summary>
        /// Method which is used for uncheck processing.
        /// </summary>
        /// <param name="sender">Object which raises event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void ProcessUnchecked(object sender, RoutedEventArgs e)
        {
            TaskBarItem item = VisualUtils.FindAncestor(this, typeof(TaskBarItem)) as TaskBarItem;
            if (item != null)
            {
                if (item.AllowCollapse == true)
                {
                    FireUnchecked();
                }
            }
        }

        #endregion Implementation
    }
}