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
using System.Net;
using System.IO;
using Microsoft.Win32;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Microsoft.SqlServer.ReportingServices2005;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ServerAuthentication.xaml
    /// </summary>
    internal partial class ServerAuthentication : ChromelessWindow
    {
        public ServerAuthentication()
        {
            InitializeComponent();
        }

        #region Public Properties
        public string ServerAddress { get; set; }

        public TextBox UserName
        {
            get
            {
                return this.txBx_Username;
            }
            set
            {
                this.txBx_Username = value;
            }
        }

        public PasswordBox Password
        {
            get
            {
                return this.pwBx_Password;
            }
            set
            {
                this.pwBx_Password = value;
            }
        }

        public CatalogItem[] Items
        {
            get
            {
                return this.items;
            }
            set
            {
                this.items = value;
            }
        }

        #endregion

        public ServerAuthentication(string serverName, SelectReport selectReport)
        {
            InitializeComponent();
            this.Owner = Window.GetWindow(selectReport);
            this.ServerAddress = serverName;
            this.txt_serverName.Text = "Connecting to " + this.ServerAddress + "...";
        }

        ReportingService2005 rs = new ReportingService2005();
        CatalogItem[] items;

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            Uri uriObj;

            try
            {
                uriObj = new Uri(this.ServerAddress);
                request = (HttpWebRequest)WebRequest.CreateDefault(uriObj);
                request.Credentials = new NetworkCredential(this.txBx_Username.Text, this.pwBx_Password.Password);
                response = (HttpWebResponse)request.GetResponse();
                rs.Url = ServerAddress + "/reportservice2005.asmx?wsdl";
                rs.Credentials = new System.Net.NetworkCredential(this.txBx_Username.Text, this.pwBx_Password.Password);
                items = rs.ListChildren("/", true);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex1)
            {
                if (ex1.Message == "The remote server returned an error: (401) Unauthorized.")
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxAuthentication"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Error);
                    this.pwBx_Password.Password = string.Empty;
                    this.Show();
                }
                else
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxUnableToConnect") + ServerAddress + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxConnectingServerInfo"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Error);
                    this.pwBx_Password.Password = string.Empty;
                }
            }
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
