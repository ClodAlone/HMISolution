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
using Syncfusion.Windows.Tools.Controls;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for RibbonSmartTag.xaml
    /// </summary>
    public partial class RibbonGallerySmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the RibbonGallerySmartTag class.
        /// </summary>
        public RibbonGallerySmartTag()
        {
            InitializeComponent( );
        }
        /// <summary>
        /// This method is called when the RibbonGallerySmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindTextBox(NameTextBox, "Name");
            BindTextBox(CaptionTextBox, "Label");



           
            
            BindCheckBox(isEnabled, "IsEnabled");
            BindCheckBox(menuicon, "MenuIconBarEnabled");

            

            BindDoubleTextBox(expandHeight, "ExpandHeight");
            BindDoubleTextBox(expandWidth, "ExpandHeight");
            BindDoubleTextBox(itemWidth, "ExpandHeight");
            BindDoubleTextBox(itemHeight, "ExpandHeight");

            BindSelectorWithEnum(VisualModeSelector, "VisualMode", typeof(RibbonGalleryVisualMode));
            BindSelectorWithEnum(resizeDirection, "ResizeDirection", typeof(ResizeDirection));
            BindSelectorWithEnum(sizeForm, "SizeForm", typeof(SizeForm));
            BindSelectorWithEnum(HorizontalAlignmentSelector, "HorizontalAlignment", typeof(HorizontalAlignment));
            BindSelectorWithEnum(VerticalAlignmentSelector, "VerticalAlignment", typeof(VerticalAlignment));
            BindSelectorWithEnum(HorizontalContentAlignmentSelector, "HorizontalContentAlignment", typeof(HorizontalAlignment));
            BindSelectorWithEnum(VerticalContentAlignmentSelector, "VerticalContentAlignment", typeof(VerticalAlignment));
            
         //   BindCheckBox(AllowDrop, "AllowDrop");
          //  BindCheckBox(IsEnabled, "IsEnabled");
           // BindCheckBox(IsChecked, "IsChecked");
           // BindCheckBox(SelectorIsSelected, "Selector.IsSelected");

           

        }

        
    }
}
