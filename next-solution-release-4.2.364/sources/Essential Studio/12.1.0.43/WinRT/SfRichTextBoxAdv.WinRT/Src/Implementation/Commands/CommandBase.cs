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
using System.Windows.Input;

namespace Syncfusion.UI.Xaml.RichTextBoxAdv
{
    public class CommandBase : ICommand
    {
        public SfRichTextBoxAdv OwnerControl;

        public CommandBase(SfRichTextBoxAdv richTextBoxAdv)
        {
            OwnerControl = richTextBoxAdv;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteCommand(parameter);
        }

        public virtual bool CanExecuteCommand(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        internal void ExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
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
