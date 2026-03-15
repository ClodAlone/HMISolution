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
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.RichTextBoxAdv
{
    #region LayoutType Command
    public class LayoutTypeCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutTypeCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public LayoutTypeCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Determines whether this instance can execute command with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute command with the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecuteCommand(object parameter)
        {
            if (OwnerControl != null)
            {
                LayoutType layoutType;
                if (Enum.IsDefined(typeof(LayoutType), parameter))
                    return OwnerControl.LayoutType != (LayoutType)parameter;
                else if (parameter is string && Enum.TryParse(parameter.ToString(), true, out layoutType))
                    return OwnerControl.LayoutType != layoutType;
            }
            return base.CanExecuteCommand(parameter);
        }
        /// <summary>
        /// Executes the layout type command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            LayoutType layoutType;
            if (Enum.IsDefined(typeof(LayoutType), parameter))
                OwnerControl.LayoutType = (LayoutType)parameter;
            else if (parameter is string && Enum.TryParse(parameter.ToString(), true, out layoutType))
                OwnerControl.LayoutType = layoutType;
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion
}
