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
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// Class that provides UI Automation support.
    /// </summary>
    public class RibbonButtonAutomationPeer : FrameworkElementAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonButtonAutomationPeer"/> class.
        /// </summary>
        /// <param name="control">The control.</param>
        public RibbonButtonAutomationPeer(RibbonButton control)
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
            return "RibbonButton";
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
        /// <returns>returns the pattern</returns>
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
        private RibbonButton MyOwner
        {
            get
            {
                return (RibbonButton)base.Owner;
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

                if (MyOwner.Command is RoutedCommand && ((System.Windows.Input.RoutedCommand)(MyOwner.Command)).Name.Contains("Toggle"))
                    MyOwner.Command.Execute(MyOwner.CommandParameter);
                MyOwner.CheckClick();
            }
        }

        #endregion
    }
}
