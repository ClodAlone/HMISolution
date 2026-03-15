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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the TaskBarAutomationPeer Class.
    /// </summary>
    public class TaskBarAutomationPeer : ItemsControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBarAutomationPeer"/> class.
        /// </summary>
        /// <param name="taskbar">The taskbar.</param>
        public TaskBarAutomationPeer(TaskBar taskbar)
            : base(taskbar)
        {
        }

        /// <summary>
        /// Returns the control type for the <see cref="T:System.Windows.UIElement"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer"/>. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>A value of the enumeration.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Tab;
        }

        /// <summary>
        /// Returns the name of the <see cref="T:System.Windows.UIElement"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer"/>. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The name of the owner type that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer"/>. See Remarks.
        /// </returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <summary>
        /// Gets a control pattern for the <see cref="T:System.Windows.Controls.ItemsControl"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.ItemsControlAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface">One of the enumeration values that indicates the control pattern.</param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the specified pattern interface is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Toggle)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Returns a <see cref="T:System.Windows.Point"/> that represents the clickable space that is on the <see cref="T:System.Windows.UIElement"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer"/>. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClickablePoint"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Windows.Point"/> on the element that allows a click.
        /// </returns>
        protected override Point GetClickablePointCore()
        {
            return new Point(double.NaN, double.NaN);
        }

        /// <summary>
        /// Returns a human-readable string that contains the item type that the <see cref="T:System.Windows.UIElement"/> for this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer"/> represents. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetItemType"/>.
        /// </summary>
        /// <returns>
        /// The string that contains the <see cref="P:System.Windows.Automation.AutomationProperties.ItemType"/> that is returned by <see cref="M:System.Windows.Automation.AutomationProperties.GetItemType(System.Windows.DependencyObject)"/>.
        /// </returns>
        protected override string GetItemTypeCore()
        {
            return typeof(TaskBarItemAutomationPeer).Name;
        }
    }

    /// <summary>
    /// Represents the TaskBarItemAutomationPeer Class.
    /// </summary>
    public class TaskBarItemAutomationPeer : ItemAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBarItemAutomationPeer"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        [CLSCompliant(false)]
        public TaskBarItemAutomationPeer(TaskBarItem item)
            : base(item)
        {
        }

        /// <summary>
        /// Returns the control type for the item that is associated with this <see cref="T:System.Windows.Automation.Peers.ItemAutomationPeer"/>. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>A value of the enumeration.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.TabItem;
        }

        /// <summary>
        /// Returns the class name of the item that is associated with this <see cref="T:System.Windows.Automation.Peers.ItemAutomationPeer"/>. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The name of the owner type that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer"/>. See Remarks.
        /// </returns>
        protected override string GetClassNameCore()
        {
            return "TaskBarItem";
        }

        /// <summary>
        /// Returns the control pattern for the item that is associated with this <see cref="T:System.Windows.Automation.Peers.ItemAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface">One of the enumeration values.</param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the specified pattern interface is not implemented.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        [CLSCompliant(false)]
        public TaskBarItem Instance
        {
            get
            {
                return (TaskBarItem)base.Owner;
            }
        }

        /// <summary>
        /// Gets the parent task bar.
        /// </summary>
        /// <value>The parent task bar.</value>
        public TaskBar ParentTaskBar
        {
            get
            {
                if (ItemsControlAutomationPeer != null)
                {
                    return (TaskBar)ItemsControlAutomationPeer.Owner;
                }

                return null;
            }
        }

        #region IExpandCollapseProvider Members

        /// <summary>
        /// Hides all nodes, controls, or content that are descendants of the control.
        /// </summary>
        public void Collapse()
        {
            if (Instance.IsEnabled)
            {
                if (Instance.headgrid.ActualWidth < Instance.contGrid.ActualWidth)
                {
                    Instance.headgrid.Width = Instance.contGrid.ActualWidth;
                }

                Instance.contGrid.Visibility = Visibility.Collapsed;
                Instance.contentborder.Visibility = Visibility.Collapsed;
                Instance.tbtn.IsChecked = true;
                Instance.IsExpanded = false;
            }
        }

        /// <summary>
        /// Displays all child nodes, controls, or content of the control.
        /// </summary>
        public void Expand()
        {
            if (Instance.IsEnabled && Instance.headgrid!= null && Instance.contGrid!= null && Instance.tbtn != null && Instance.contentborder != null)
            {
                Instance.headgrid.Width = double.NaN;
                Instance.contGrid.Visibility = Visibility.Visible;
                Instance.contentborder.Visibility = Visibility.Visible;
                Instance.tbtn.IsChecked = false;
                Instance.IsExpanded = true;
            }
        }

        /// <summary>
        /// Gets the state (expanded or collapsed) of the control.
        /// </summary>
        /// <value></value>
        /// <returns>The state (expanded or collapsed) of the control.</returns>
        public System.Windows.Automation.ExpandCollapseState ExpandCollapseState
        {
            get
            {
                if (Instance.IsExpanded)
                {
                    return System.Windows.Automation.ExpandCollapseState.Expanded;
                }
                else
                {
                    return System.Windows.Automation.ExpandCollapseState.Collapsed;
                }
            }
        }

        #endregion
    }
}
