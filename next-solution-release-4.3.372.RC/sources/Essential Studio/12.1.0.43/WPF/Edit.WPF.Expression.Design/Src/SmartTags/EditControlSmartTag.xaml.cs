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
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Edit;
using System.Diagnostics;
using Syncfusion.Windows.Design;

namespace Syncfusion.Edit.Wpf.Expression.Design
{
	/// <summary>
	/// Interaction logic for EditSmartTag.xaml
	/// </summary>
	public partial class EditControlSmartTag : SmartTagBase
	{
        /// <summary>
        /// Initializes a new instance of the EditControlSmartTag class.
        /// </summary>
        public EditControlSmartTag()
		{
			InitializeComponent();
		}
        /// <summary>
        /// This method is called when the EditControlSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindTextBox(txtName, "Name");
            BindTextBox(txtText, "Text");
            BindSelectorWithBrushes(cmbBackground, "Background");
            BindSelectorWithBrushes(cmbForeground, "Foreground");
            BindSelectorWithBrushes(cmbBorderBrush, "BorderBrush");
            BindThickness(txtBorderThickness, "BorderThickness");
           // BindSelectorWithEnum(cmbLanguage, "DocumentLanguage", typeof(Languages));
            BindCheckBox(chkline, "ShowLineNumber");
            BindCheckBox(chkreadonly, "IsReadOnly");
            BindCheckBox(chkshwcm, "ShowDefaultContextMenu");
            
        }
	}
}
