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
using Syncfusion.Windows.Design;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for RangeSliderSmartTag.xaml
    /// </summary>
    public partial class RangeSliderSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the RangeSliderSmartTag class.
        /// </summary>
        public RangeSliderSmartTag()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This method is called when the RangeSliderSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(name, "Name");

            BindSelectorWithBrushes(Backgroundselector, "Background");
            BindSelectorWithBrushes(Foregroundselector, "Foreground");
            BindDoubleTextBox(Tickfrequency, "TickFrequency");
            BindSelectorWithEnum(Tickplacement, "TickPlacement", typeof(Syncfusion.Windows.Tools.Controls.Tickplacement));
        }
    }
}
