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
using Syncfusion.Windows.Shared;
using Microsoft.Windows.Design.Model;

namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for CarouselSmartTag.xaml
    /// </summary>
    public partial class CarouselSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the CarouselSmartTag class.
        /// </summary>
        public CarouselSmartTag()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This method is called when the CarouselSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(this.NameTextBox, "Name");
            BindTextBox(this.ItemsTextBox, "ItemsPerPage");
            BindTextBox(this.OpacityFraction, "OpacityFraction");
            BindTextBox(this.RadiusXTextBox, "RadiusX");
            BindTextBox(this.RadiusYTextBox, "RadiusY");
            BindTextBox(this.RotationAngleTextBox, "RotationAngle");
            BindTextBox(this.RotationSpeedTextBox, "RotationSpeed");
            BindCheckBox(this.OpacityEnabled, "OpacityEnabled");
            BindCheckBox(this.ScalingEnabled, "ScalingEnabled");
            BindCheckBox(this.SkewAngleXEnabled, "SkewAngleXEnabled");
            BindCheckBox(this.SkewAngleYEnabled, "SkewAngleYEnabled");
            BindCheckBox(this.EnableTouch, "EnableTouch");
            BindSelectorWithEnum(VisualMode, "VisualMode", typeof(VisualMode));

            
        }
    }
}
