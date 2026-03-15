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
    public class ChangePageLayoutCommand : CommandBase
    {
        public ChangePageLayoutCommand(RichTextBoxAdv rich)
            : base(rich)
        {

        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (AssociatedRichEditor != null)
            {
                if (parameter != null)
                {
                    PageLayout layout;
#if WPF
                    if(parameter != null)
                    {
                       layout = (PageLayout)Enum.Parse(typeof(PageLayout), parameter.ToString()); 
#else
                    if (Enum.TryParse(parameter.ToString(), out layout))
                    {
#endif
                        return AssociatedRichEditor.PageLayout != layout;
                    }
                }
            }
            return base.CanExcuteCommand(parameter);
        }

       protected override void ExecuteCommand(object parameter)
       {
           PageLayout layout;
#if WPF
           if (parameter != null)
           {
           layout = (PageLayout)Enum.Parse(typeof(PageLayout), parameter.ToString()); 
#else
           if (Enum.TryParse(parameter.ToString(), out layout))
           { 
#endif       
            AssociatedRichEditor.ChangePageLayout(layout);
           }
           AssociatedRichEditor.Focus();
     	   base.ExecuteCommand(parameter);
       }
    }
}
