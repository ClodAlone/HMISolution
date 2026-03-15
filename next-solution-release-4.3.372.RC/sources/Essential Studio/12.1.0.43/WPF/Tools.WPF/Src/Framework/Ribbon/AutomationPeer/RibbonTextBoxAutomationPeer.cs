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
    public class RibbonTextBoxAutomationPeer : TextBoxAutomationPeer
    {
        public RibbonTextBoxAutomationPeer(RibbonTextBox owner)
            : base(owner)
        {

        }

        protected override string GetNameCore()
        {
            return "RibbonTextBox";
        }
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }
    }
}
