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
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    public class BackStageButtonAutoamtionPeer : FrameworkElementAutomationPeer,IInvokeProvider
    {
        public BackStageButtonAutoamtionPeer(BackStageButton owner)
            : base(owner)
        {

        }

        protected override string GetNameCore()
        {
            return "BackStageButton";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }


        #region IInvokeProvider Members

        public void Invoke()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
            MyOwner.RaiseEvent(newEventArgs);
            if (MyOwner.Command != null)
            {

                if (MyOwner.Command is RoutedCommand)
                    MyOwner.Command.Execute(MyOwner.CommandParameter);
                MyOwner.CheckClick();
            }
        }

        private BackStageButton MyOwner
        {
            get
            {
                return (BackStageButton)base.Owner;
            }
        }


        #endregion
    }
}
