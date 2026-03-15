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
    public class ChangeFontSizeCommand :CommandBase 
    {
        public ChangeFontSizeCommand(RichTextBoxAdv rich):base(rich)
        {

        }
        protected override void ExecuteCommand(object parameter)
        {
            double fontSize=0.0;

            if (parameter is double)
            {
                fontSize = Convert.ToDouble(parameter);
            }
            else if (parameter is string)
            {
                fontSize = double.Parse(parameter.ToString());
            }
            if (fontSize > 0.0)
            {
                AssociatedRichEditor.Selection.ChangeFontSize(fontSize);
            }
            AssociatedRichEditor.Focus();
            base.ExecuteCommand(parameter);
        }       
    }
}
