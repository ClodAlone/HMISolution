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
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
	/// <summary>
	/// Interaction logic for GroupViewSmartTag.xaml
	/// </summary>
	public partial class GroupViewSmartTag : SmartTagBase
	{
        /// <summary>
        /// Initializes a new instance of the GroupViewSmartTag class.
        /// </summary>
		public GroupViewSmartTag()
		{
			InitializeComponent();
		}
        /// <summary>
        /// This method is called when the GroupViewSmartTag Item template is initialized.
        /// </summary>
		public override void OnApplyTemplate( )
		{
			base.OnApplyTemplate( );

            BindTextBox(NameTextBox, "Name");
       

			BindSelectorWithEnum( OrientationSelector, "Orientation", typeof( Orientation ) );
            BindSelectorWithEnum(TextAlignmentSelector, "TextAlignment", typeof(HorizontalAlignment));
            BindSelectorWithEnum(GroupViewItemCursorType, "GroupViewItemCursorType", typeof(ItemCursorType));


            BindCheckBox(IsShowText, "IsShowText");
            BindCheckBox(IsListViewMode, "IsListViewMode");
            BindCheckBox(ShowToolTip, "ShowToolTip");
            
		}

        
	}
}
