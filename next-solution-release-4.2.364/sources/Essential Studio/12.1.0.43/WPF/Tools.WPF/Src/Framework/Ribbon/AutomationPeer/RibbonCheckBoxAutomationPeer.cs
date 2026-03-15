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
using System.Windows.Automation;

namespace Syncfusion.Windows.Tools.Controls
{
    public class RibbonCheckBoxAutomationPeer : CheckBoxAutomationPeer
    {
        public RibbonCheckBoxAutomationPeer(RibbonCheckBox owner)
            : base(owner)
        {

        }

        protected override string GetNameCore()
        {
            return "RibbonCheckBox";
        }

        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Toggle)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }
       
    }

}
