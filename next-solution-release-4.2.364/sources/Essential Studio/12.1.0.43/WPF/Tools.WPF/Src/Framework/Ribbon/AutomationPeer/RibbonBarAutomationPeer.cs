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
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    public class RibbonBarAutomationPeer : ItemsControlAutomationPeer
    {
        public RibbonBarAutomationPeer(ItemsControl owner)
            : base(owner)
        {

        }

        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        protected override string GetNameCore()
        {
            return "RibbonBar";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new RibbonBarDataAutomationPeer(item, this);
        }
    }

    public class RibbonBarDataAutomationPeer : ItemAutomationPeer 
    {
        public RibbonBarDataAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer)
                : base(item, itemsControlAutomationPeer)
            {

            }
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return "RibbonBar";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return null;
        }
    }
}
