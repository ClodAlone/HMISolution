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
using System.ComponentModel;
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for TablixGroupAdvanced.xaml
    /// </summary>

    internal partial class TablixGroupAdvanced : UserControl
    {
        ExpressionDialog dialog;

        public TablixGroupAdvanced()
        {
            InitializeComponent();
        }

        private void RecursiveButton_Click(object sender, RoutedEventArgs e)
        {
            dialog = new ExpressionDialog();
            this.dialog.Owner = Window.GetWindow(this);
            Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(this.dialog, Syncfusion.Windows.Shared.SkinStorage.GetVisualStyle(this.dialog.Owner));

            if (dialog.ShowDialog() == true)
            {

            }
        }

        private void DocumentMapButton_Click(object sender, RoutedEventArgs e)
        {
            dialog = new ExpressionDialog();
            this.dialog.Owner = Window.GetWindow(this);
            Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(this.dialog, Syncfusion.Windows.Shared.SkinStorage.GetVisualStyle(this.dialog.Owner));
            
            if (dialog.ShowDialog() == true)
            {
            }
        }
    }
}
