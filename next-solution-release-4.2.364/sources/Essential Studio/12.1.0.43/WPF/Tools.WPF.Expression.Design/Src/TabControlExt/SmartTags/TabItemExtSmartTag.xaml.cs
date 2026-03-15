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
using Microsoft.Win32;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.Expression.Design
{
	/// <summary>
	/// Interaction logic for GroupBarItemSmartTag.xaml
	/// </summary>
	public partial class TabItemExtSmartTag : SmartTagBase
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="TabItemExtSmartTag"/> class.
        /// </summary>
        public TabItemExtSmartTag()
		{
			InitializeComponent();
		}

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
		public override void OnApplyTemplate( )
		{
			base.OnApplyTemplate( );

            BindTextBox(HeaderTextBox, "Header");
            BindCheckBox(IsSelectedCheckBox, "IsSelected");
            BindNumeric(heighttextbox, "Height");
            BindNumeric(widthtextbox, "Width");
        }
	}
}
