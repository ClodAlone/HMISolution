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

namespace Syncfusion.Windows.Tools.Controls
{
    #region UI Automation support

    /// <property name="flag" value="Finished" />
    /// <summary>
    /// Class that provides UI Automation support.
    /// </summary>
    /// <exclude/>
    public class ApplicationMenuAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationMenuAutomationPeer"/> class.
        /// </summary>
        /// <param name="control">The control.</param>
        public ApplicationMenuAutomationPeer(ApplicationMenu control)
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
            return "ApplicationMenu";
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the localized version of the control type for the
        /// RibbonButton.
        /// </summary>
        /// <returns>It returns the Menu</returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return "Menu";
        }

        /// <summary>
        /// Gets the control type for the ribbon button that is associated with this ApplicationMenuAutomationPeer.
        /// </summary>
        /// <returns>
        /// The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Custom"/> enumeration value.
        /// <returns>Returns AutomationControlType</returns></returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Menu;
        }

        /// <summary>
        /// Gets the control pattern for the <see cref="T:System.Windows.UIElement"/> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface">A value from the enumeration.</param>
        /// <returns>pattern interface</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets ribbon button object.
        /// </summary>
        private ApplicationMenu MyOwner
        {
            get
            {
                return (ApplicationMenu)base.Owner;
            }
        }
        #region IExpandCollapseProvider Members

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Implements Collapse method for IExpandCollapseProvider.
        /// </summary>
        public void Collapse()
        {
            MyOwner.IsPopupOpen = false;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Implements Expand method for IExpandCollapseProvider. 
        /// </summary>
        public void Expand()
        {
            MyOwner.IsPopupOpen = true;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets Collapse state for current object.
        /// </summary>
        public System.Windows.Automation.ExpandCollapseState ExpandCollapseState
        {
            get
            {
                if (MyOwner.IsPopupOpen)
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
    #endregion
}
