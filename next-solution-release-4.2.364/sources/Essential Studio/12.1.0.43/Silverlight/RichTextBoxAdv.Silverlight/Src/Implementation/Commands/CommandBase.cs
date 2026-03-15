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
    public class CommandBase : ICommand 
    {
        public RichTextBoxAdv AssociatedRichEditor;

        public CommandBase(RichTextBoxAdv richTextBox)
        {
            AssociatedRichEditor = richTextBox;
        }


        public bool CanExecute(object parameter)
        {
            return CanExcuteCommand(parameter);
        }

        public virtual bool CanExcuteCommand(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void ExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public void Execute(object parameter)
        {
            ExecuteCommand(parameter);
        }

        protected virtual void ExecuteCommand(object parameter)
        {

        }
    }
}
