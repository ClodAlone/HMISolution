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
using Syncfusion.Windows.Shared;
using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "RibbonMenuItemStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "RibbonMenuItemStates", Name = "DropDownOpened")]
    [TemplateVisualState(GroupName = "RibbonMenuItemStates", Name = "DropDownClosed")]
    [TemplateVisualState(GroupName = "RibbonMenuItemStates", Name = "Checked")]
    [TemplateVisualState(GroupName = "RibbonMenuItemStates", Name = "UnChecked")]
    public class RibbonMenuItem : DropDownMenuItem,IRibbonControl
    {
        /// <summary>
        /// 
        /// </summary>
        public RibbonMenuItem()
        {

        }

        internal RibbonCommandProvider _provider;

        /// <summary>
        /// Called when [click].
        /// </summary>
        protected override void OnClick()
        {
            FrameworkElement parent = this.Parent as FrameworkElement;
            do
            {
                if (parent == null || parent is RibbonBar)
                {
                    if (parent is RibbonBar)
                    {
                        (parent as RibbonBar).IsDropDownOpen = false;
                    }
                    break;
                }
                   
                parent = parent.Parent as FrameworkElement;
            } while (parent != null);
            var ribbonmenugroup = VisualUtils.FindAncestor(this, typeof(RibbonMenuGroup));
            if (ribbonmenugroup != null)
            {
                DropDownButtonAdv dropDown = ((RibbonMenuGroup)ribbonmenugroup).Parent as DropDownButtonAdv;
                if (dropDown != null)
                {
                    dropDown.IsDropDownOpen = false;
                }
            }

            base.OnClick();
        }
    }
}
