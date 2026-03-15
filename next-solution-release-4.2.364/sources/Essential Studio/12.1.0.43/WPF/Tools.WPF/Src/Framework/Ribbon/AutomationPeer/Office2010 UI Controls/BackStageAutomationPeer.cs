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
    public class BackStageAutomationPeer : SelectorAutomationPeer
    {
        public BackStageAutomationPeer(Backstage owner)
            : base(owner)
        {

        }
        protected override string GetNameCore()
        {
            return "BackStage";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new BackStageItemAutomationPeer(item, this);
        }
    }

    public class BackStageItemAutomationPeer : ItemAutomationPeer
    {

        public BackStageItemAutomationPeer(object item, ItemsControlAutomationPeer itemsControlPeer)
            : base(item, itemsControlPeer)
        {

        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return "BackStageItem";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return null;
        }
    }
}
