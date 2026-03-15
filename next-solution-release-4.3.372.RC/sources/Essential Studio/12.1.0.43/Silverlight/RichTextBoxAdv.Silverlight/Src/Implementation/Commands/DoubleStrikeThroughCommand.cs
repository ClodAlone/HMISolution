#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    public class DoubleStrikeThroughCommand : CommandBase
    {
        public DoubleStrikeThroughCommand(RichTextBoxAdv rich):base(rich)
        {

        }

        protected override void ExecuteCommand(object parameter)
        {
            AssociatedRichEditor.Selection.ChangeDoubleStrikeThrough();
            AssociatedRichEditor.Focus();
            base.ExecuteCommand(parameter);           
        }
    }
}
