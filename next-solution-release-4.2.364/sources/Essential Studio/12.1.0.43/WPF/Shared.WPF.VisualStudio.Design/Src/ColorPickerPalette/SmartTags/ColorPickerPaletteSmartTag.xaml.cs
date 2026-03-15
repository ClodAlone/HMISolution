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
using System.Windows.Shapes;
using Syncfusion.Windows.Design;
using Syncfusion.Windows.Shared;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;


namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for ColorPickerPaletteSmartTag.xaml
    /// </summary>
    public partial class ColorPickerPaletteSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the ColorPickerPaletteSmartTag class.
        /// </summary>
        public ColorPickerPaletteSmartTag()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This method is called when the ColorPickerPaletteSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(this.NameTextBox, "Name");
            BindTextBox(this.CustomHeaderText, "CustomHeaderText");
            BindSelectorWithEnum(AutomaticColorVisibility, "AutomaticColorVisibility", typeof(Visibility));
            BindSelectorWithEnum(BlackWhiteVisibility, "BlackWhiteVisibility", typeof(BlackWhiteVisible));
            BindSelectorWithEnum(ThemePanelVisibility, "ThemePanelVisibility", typeof(Visibility));
            BindSelectorWithEnum(CustomHeaderVisibility, "CustomHeaderVisibility", typeof(Visibility));
            BindSelectorWithEnum(MoreColorOptionVisibility, "MoreColorOptionVisibility", typeof(Visibility));
            BindSelectorWithEnum(IsCustomTabVisible, "IsCustomTabVisible", typeof(Visibility));
            BindSelectorWithEnum(IsStandardTabVisible, "IsStandardTabVisible", typeof(Visibility));
            BindSelectorWithEnum(StandardPanelVisibility, "StandardPanelVisibility", typeof(Visibility));
            BindSelectorWithEnum(RecentlyUsedPanelVisibility, "RecentlyUsedPanelVisibility", typeof(Visibility));
            BindCheckBox(IsExpanded, "IsExpanded");
            BindTextBox(PopupHeight, "PopupHeight");
            BindTextBox(PopupWidth, "PopupWidth");
        }

    }
}
