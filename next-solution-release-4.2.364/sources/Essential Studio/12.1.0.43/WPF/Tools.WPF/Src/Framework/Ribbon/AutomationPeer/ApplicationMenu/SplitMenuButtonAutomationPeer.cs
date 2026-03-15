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
    /// Class that provides UI Automation support for SplitMenuButton.
    /// </summary>
    public class SplitMenuButtonAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitMenuButtonAutomationPeer"/> class.
        /// </summary>
        /// <param name="control">The control.</param>
        public SplitMenuButtonAutomationPeer(SplitMenuButton control)
            : base(control)
        {
        }

        /// <summary>
        /// Gets the name of the <see cref="T:System.Windows.UIElement"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer"/>. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="F:System.String.Empty"/> string.
        /// </returns>
        protected override string GetClassNameCore()
        {
            return "SplitMenuButton";
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the localized version of the control type for the
        /// SplitMenuButton.
        /// </summary>
        /// <returns>Returns the button.</returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return "button";
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the control type for the ribbon button that is
        /// associated with this SplitMenuButtonAutomationPeer.
        /// </summary>
        /// <returns>AutomationControlType Button</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        /// <summary>
        /// Gets the control pattern for the <see cref="T:System.Windows.UIElement"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface">A value from the enumeration.</param>
        /// <returns>returns pattern Interface.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets ribbon button object.
        /// </summary>
        private SplitMenuButton MyOwner
        {
            get
            {
                return (SplitMenuButton)base.Owner;
            }
        }

        #region IInvokeProvider Members

        /// <summary>
        /// Sends a request to activate a control and initiate its single, unambiguous action.
        /// </summary>
        /// <exception cref="T:System.Windows.Automation.ElementNotEnabledException">
        /// If the control is not enabled.
        /// </exception>
        public void Invoke()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
            MyOwner.RaiseEvent(newEventArgs);
            if (MyOwner.Command != null)
            {
                MyOwner.Command.Execute(MyOwner.CommandParameter);
            }
        }
        #endregion
    }
}
