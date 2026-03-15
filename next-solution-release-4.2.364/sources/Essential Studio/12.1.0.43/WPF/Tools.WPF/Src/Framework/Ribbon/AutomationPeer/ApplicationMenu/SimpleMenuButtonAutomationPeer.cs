#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class that provides UI Automation support.
    /// </summary>
    public class SimpleMenuButtonAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMenuButtonAutomationPeer"/> class.
        /// </summary>
        /// <param name="control">The control.</param>
        public SimpleMenuButtonAutomationPeer(SimpleMenuButton control)
            : base(control)
        {
        }

        /// <summary>
        /// Returns SimpleMenuButton class name.
        /// </summary>
        /// <returns>It returns simple menu button</returns>
        protected override string GetClassNameCore()
        {
            return "SimpleMenuButton";
        }

        /// <summary>
        /// When overridden in a derived class, is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetLocalizedControlType"/>.
        /// </summary>
        /// <returns>
        /// The string that contains the type of control.
        /// </returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return "button";
        }

        /// <summary>
        /// Gets the control type for the <see cref="T:System.Windows.UIElement"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer"/>. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Custom"/> enumeration value.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        /// <summary>
        /// Gets the control pattern for the <see cref="T:System.Windows.UIElement"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface">A value from the enumeration.</param>
        /// <returns>It returns null.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets ribbon button object.
        /// </summary>
        private SimpleMenuButton MyOwner
        {
            get
            {
                return (SimpleMenuButton)base.Owner;
            }
        }

        #region IInvokeProvider Members
        /// <summary>
        /// Invokes SimpleMenuButton Click event.
        /// </summary>
        public void Invoke()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
            MyOwner.RaiseEvent(newEventArgs);

            MyOwner.CheckClick();
        }
        #endregion
    }
}
