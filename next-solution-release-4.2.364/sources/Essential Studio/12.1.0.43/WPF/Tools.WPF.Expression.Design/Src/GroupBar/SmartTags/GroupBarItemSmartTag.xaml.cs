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
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.Expression.Design
{
	/// <summary>
	/// Interaction logic for GroupBarItemSmartTag.xaml
	/// </summary>
	public partial class GroupBarItemSmartTag : SmartTagBase
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBarItemSmartTag"/> class.
        /// </summary>
		public GroupBarItemSmartTag()
		{
			InitializeComponent();
		}

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
		public override void OnApplyTemplate( )
		{
			base.OnApplyTemplate( );

            BindTextBox(NameTextBox, "Name");
       
    		BindTextBox( HeaderTextBox, "HeaderText" );
            BindCheckBox(IsExpandedCheckBox, "IsExpanded");
            BindCheckBox(ShowInGroupBarCheckBox, "ShowInGroupBar");
            BindCheckBox(IsAnimatingCheckBox, "IsAnimating");
            BindCheckBox(IsInEditModeCheckBox, "IsInEditMode");
            BindCheckBox(IsDragOverTopCheckBox, "IsDragOverTop");
            BindCheckBox(IsLastItemCheckBox, "IsLastItem");
            BindCheckBox(IsSelectedCheckBox, "IsSelected");

		}

        /// <summary>
        /// Sets the content of the group view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		public void SetGroupViewContent( object sender, RoutedEventArgs e )
		{
			Hyperlink hyperlink = sender as Hyperlink;
			AddChildControl( this.ModelItem.Properties[ "Content" ], hyperlink.Tag as Type );
			GroupViewItem groupViewItem = new GroupViewItem();
			groupViewItem.Text = "New GroupViewItem";
			this.ModelItem.Properties[ "Content" ].Value.Properties[ "Items" ].Collection.Add( groupViewItem );
		}
	}
}
