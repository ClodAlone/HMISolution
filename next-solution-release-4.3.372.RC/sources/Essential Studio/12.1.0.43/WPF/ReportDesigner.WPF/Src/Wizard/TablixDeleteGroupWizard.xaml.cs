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
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for TablixDeleteGroupWizard.xaml
    /// </summary>
    internal partial class TablixDeleteGroupWizard : ChromelessWindow
    {
        public TablixDeleteGroupWizard()
        {
            InitializeComponent();
            this.Title = Syncfusion.Windows.ReportDesigner.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "titleTablixDeleteGroupWizard");
        }

        public bool isCancel
        {
            get;
            set;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.isCancel = true;
            this.Close();
        }
    }
}
