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

namespace Syncfusion.Windows.Tools.Controls
{
    public class RibbonComboBoxItemAutomationPeer : ItemAutomationPeer
    {
        public RibbonComboBoxItemAutomationPeer(object item, ItemsControlAutomationPeer owner)
            : base(item, owner)
        {

        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return "RibbonComboBoxItem";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            object pattern = null;

            if (patternInterface == PatternInterface.Selection)
            {
                pattern = this;
            }
            return pattern;
        }
    }
}
