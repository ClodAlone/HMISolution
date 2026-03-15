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
    public class RibbonMenuGroupAutomationPeer : ItemsControlAutomationPeer
    {

        public RibbonMenuGroupAutomationPeer(RibbonMenuGroup owner)
            : base(owner)
        {

        }

        protected override string GetNameCore()
        {
            return "RibbonMenuGroup";
        }

        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new RibbonMenuGroupItemAutomationPeer(item, this);
        }
    }

    public class RibbonMenuGroupItemAutomationPeer:ItemAutomationPeer
    {
        public RibbonMenuGroupItemAutomationPeer(object item, ItemsControlAutomationPeer owner)
            : base(item, owner)
        {

        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return "RibbonMenuGroupItem";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return null;
        }
    }
}
