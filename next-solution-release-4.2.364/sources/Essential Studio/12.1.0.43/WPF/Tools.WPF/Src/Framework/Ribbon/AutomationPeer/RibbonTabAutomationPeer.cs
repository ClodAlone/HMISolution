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
using System.Windows.Controls;
using System.Windows.Automation.Provider;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    public class RibbonTabAutomationPeer : ItemsControlAutomationPeer, ISelectionItemProvider
    {
        public RibbonTabAutomationPeer(ItemsControl ribbonTab)
            : base(ribbonTab)
        {

        }

        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        protected override string GetNameCore()
        {
            return "RibbonTab";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new RibbonBarDataAutomationPeer(item, this);
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.SelectionItem)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }


        #region ISelectionItemProvider Members

        public void AddToSelection()
        {
            throw new NotImplementedException();
        }

        public bool IsSelected
        {
            get
            {
                if (this.Owner is RibbonTab && (this.Owner as RibbonTab).IsChecked)
                    return true;
                else
                    return false;

            }
        }

        public void RemoveFromSelection()
        {

        }

        public void Select()
        {
            if (this.Owner is RibbonTab)
            {
                (this.Owner as RibbonTab).IsChecked = true;
            }
        }

        public IRawElementProviderSimple SelectionContainer
        {
            get { throw new NotImplementedException(); }
        }

        #endregion        
    }

    public class RibbonTabDataAutomationPeer : ItemAutomationPeer
    {
        public RibbonTabDataAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer)
            : base(item, itemsControlAutomationPeer)
        {

        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return "RibbonTab";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return null;
        }
       
    }
}
