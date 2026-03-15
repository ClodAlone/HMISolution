#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    /// xmlns:MyNamespace="clr-namespace:CustomColorChooser"
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    /// xmlns:MyNamespace="clr-namespace:CustomColorChooser;assembly=CustomColorChooser"
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    /// Right click on the target project in the Solution Explorer and
    /// "Add Reference"-&gt;"Projects"-&gt;[Browse to and select this project]
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ColorPickerDockPanel : Control
    {
        /// <summary>
        /// Initializes the <see cref="ColorPickerDockPanel"/> class.
        /// </summary>
        static ColorPickerDockPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPickerDockPanel), new FrameworkPropertyMetadata(typeof(ColorPickerDockPanel)));
        }
    }
}
