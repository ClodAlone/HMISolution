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
    public class QuickAccessToolBarAutomationPeer : ItemsControlAutomationPeer
    {
        public QuickAccessToolBarAutomationPeer(QuickAccessToolBar owner)
            : base(owner)
        {

        }

        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        protected override string GetNameCore()
        {
            return "QuickAccessToolBar";
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new QucikAccessToolBarItemAutomationPeer(item, this);
        }
    }

    public class QucikAccessToolBarItemAutomationPeer:ItemAutomationPeer
    {

        public QucikAccessToolBarItemAutomationPeer(object item, ItemsControlAutomationPeer itemsControlAutomationPeer)
            : base(item, itemsControlAutomationPeer)
        {

        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return "QuickAccessToolBarItem";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return null;
        }
    }


}
