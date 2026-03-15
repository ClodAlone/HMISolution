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
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    public class InsertTableCommand :CommandBase
    {
        public InsertTableCommand(RichTextBoxAdv rich)
            : base(rich)
        {

        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (AssociatedRichEditor != null)
            {
                return !AssociatedRichEditor.Viewer.IsSelected;
            }
            return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (parameter is string)
            {
                string[] collection = parameter.ToString().Split(new string[] { ",", " " }, StringSplitOptions.None);
                if (AssociatedRichEditor != null)
                {
                    AssociatedRichEditor.InsertTableInBlocks(int.Parse(collection[0]),int.Parse(collection[1]));
                    AssociatedRichEditor.Focus();
                }
            }
            else if (parameter is int[])
            {
                int[] rowcolumn = (int[])parameter;
                if (AssociatedRichEditor != null)
                {
                    AssociatedRichEditor.InsertTableInBlocks(rowcolumn[0], rowcolumn[1]);
                    AssociatedRichEditor.Focus();
                }
            }
        }
    }
}
