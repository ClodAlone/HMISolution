// <copyright file="RibbonComboBoxItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the ComboBox item
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonComboBoxItem : ComboBoxItem
    {
        /// <summary>
        /// Initializes static members of the <see cref="RibbonComboBoxItem"/> class.
        /// </summary>
        static RibbonComboBoxItem()
        {
                      
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonComboBoxItem), new FrameworkPropertyMetadata(typeof(RibbonComboBoxItem)));
        }
    }
}
