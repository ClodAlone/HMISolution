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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System.Reflection;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for SeriesGeneral1.xaml
    /// </summary>
    internal partial class SeriesGeneral1 : UserControl
    {
        public SeriesGeneral1()
        {
            InitializeComponent();
        }

        private void chk_ShowDataLabels_Click(object sender, RoutedEventArgs e)
        {
            if (this.chk_ShowDataLabels.IsChecked == true)
            {
                this.cmb_DatalabelPosition.IsEnabled = true;
            }
            else
            {
                this.cmb_DatalabelPosition.IsEnabled = false;
            }
        }

   
    }
}
