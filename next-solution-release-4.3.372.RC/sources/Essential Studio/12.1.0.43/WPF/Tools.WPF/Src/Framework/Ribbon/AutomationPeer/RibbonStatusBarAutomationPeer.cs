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

namespace Syncfusion.Windows.Tools.Controls
{
    public class RibbonStatusBarAutomationPeer : ItemsControlAutomationPeer
    {
        public RibbonStatusBarAutomationPeer(RibbonStatusBar owner)
            : base(owner)
        {

        }

        protected override string GetNameCore()
        {
            return "RibbonStatusBar";
        }

        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new RibbonStatusBarItemAutomationPeer(item, this);
        }
    }

    public class RibbonStatusBarItemAutomationPeer:ItemAutomationPeer
    {
        public RibbonStatusBarItemAutomationPeer(object item, ItemsControlAutomationPeer owner)
            : base(item, owner)
        {

        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return "RibbonStatusBarItem";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return null;
        }
    }
}
