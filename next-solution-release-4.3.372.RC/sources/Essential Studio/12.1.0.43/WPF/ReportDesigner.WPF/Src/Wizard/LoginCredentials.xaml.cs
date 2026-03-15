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
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Wizard
{
    /// <summary>
    /// Interaction logic for LoginCredentials.xaml
    /// </summary>
    internal partial class LoginCredentials : ChromelessWindow
    {
        public string Username { get; set; }

        public string Password { get; set; }

        private string serverurl = string.Empty;

        public LoginCredentials(string serverurl)
        {
            InitializeComponent();
            this.serverurl = serverurl;
        }
   
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxAuthentication"), SR.GetString(CultureInfo.CurrentUICulture, "titleWarning"), MessageBoxButton.OK);
            }
            else
            {                
                this.Username = txtUsername.Text;
                this.Password = txtPassword.Password;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txt_serverName.Text = "Connecting to " + this.serverurl + "...";
        }
    }
}
