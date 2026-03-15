//-------------------------------------------------------------------------------------------------
// <copyright file="Credentials.xaml.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------
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
using System.ComponentModel;
using Syncfusion.RDL.DOM;
using Syncfusion.Windows.Tools.Controls;
using System.Data.SqlClient;
using Syncfusion.Windows.Reports.Sql;
using System.Globalization;
using Syncfusion.Windows.ReportDesigner.Resources;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Represents the Credential window.
    /// </summary>

    public partial class Credentials 
        : ChromelessWindow
    {
  
        private RDL.DOM.DataSource DataSource { get; set; }
        public bool isCancel = false;
        private int tryCount;
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Credentials"/> class.
        /// </summary>
        public Credentials(RDL.DOM.DataSource datasource)
        {
            InitializeComponent();
            this.DataSource=datasource;
            tryCount = 0;
            this.KeyDown += new KeyEventHandler(Credentials_KeyDown);
            this.btn_OK.Click += new RoutedEventHandler(btn_OK_Click);
            this.btn_Cancel.Click += new RoutedEventHandler(btn_Cancel_Click);
        }

        #endregion

        private string GetTestConnectionString()
        {
            string connString= this.DataSource.ConnectionProperties.ConnectString;

            if (!this.DataSource.ConnectionProperties.IntegratedSecurity &&  this.textBox_UserName.Text.ToString() != string.Empty && this.pass_Password.Password != string.Empty)
            {
                connString += ";" + ConnectionConstants.UserID + "=" + this.textBox_UserName.Text.ToString();
                connString += ";" + ConnectionConstants.Password + "=" + this.pass_Password.Password.ToString();
            }

            else if (this.DataSource.ConnectionProperties.IntegratedSecurity)
            {
                connString += "Trusted_Connection=" + true;
            }

            return connString;
        }

        void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DataSource.ConnectionProperties.UserName = null;
            this.DataSource.ConnectionProperties.PassWord = null;
            this.isCancel = true;
            this.Close();
        }

        void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection dbConnection = new SqlConnection(GetTestConnectionString());

            try
            {
                tryCount++;
                dbConnection.Open();
                dbConnection.Close();
                this.DataSource.ConnectionProperties.UserName = this.textBox_UserName.Text;
                this.DataSource.ConnectionProperties.PassWord = this.pass_Password.Password;
                this.DialogResult = true;
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxConnectFaild") + "\"" + this.DataSource.Name + "\" \n" + ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK);
                this.DataSource.ConnectionProperties.UserName = this.textBox_UserName.Text = null;
                this.DataSource.ConnectionProperties.PassWord = this.pass_Password.Password = null;

                if (tryCount > 2)
                {
                    this.DialogResult = false;
                    this.Close();
                }
            }
        }

        void Credentials_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}
