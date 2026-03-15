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
using System.ComponentModel;
using Syncfusion.Windows.Shared;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{

    /// <summary>
    /// 
    /// </summary>
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "RibbonButtonStates", Name = "Disabled")]
    public class RibbonDropDownButton : DropDownButtonAdv, IRibbonControl, IRibbonItem
    {

        /// <summary>
        /// 
        /// </summary>
        public RibbonDropDownButton()
        {
            DefaultStyleKey = typeof(RibbonDropDownButton);
        }

        #region IRibbonItem Members

        /// <summary>
        /// 
        /// </summary>
        public bool IsAutoSizeFormEnabled
        {
            get { return (bool)GetValue(IsAutoSizeFormEnabledProperty); }
            set { SetValue(IsAutoSizeFormEnabledProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsAutoSizeFormEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAutoSizeFormEnabledProperty =
            DependencyProperty.Register("IsAutoSizeFormEnabled", typeof(bool), typeof(RibbonDropDownButton), new PropertyMetadata(false));   

        #endregion
    }
}
