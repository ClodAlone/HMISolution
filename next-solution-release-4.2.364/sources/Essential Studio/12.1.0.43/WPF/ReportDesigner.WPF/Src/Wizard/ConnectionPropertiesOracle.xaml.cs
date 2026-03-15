//-------------------------------------------------------------------------------------------------
// <copyright file="ConnectionPropertiesOracle.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Data.OracleClient;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Reports.Sql;
using Syncfusion.Windows.Shared;
using System.ComponentModel;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for Essential WPF RibbonWindow1.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif

        [ObsoleteAttribute("OracleCommand has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    internal partial class ConnectionPropertiesOracle : ChromelessWindow, IDisposable
    {
        #region Members
        private OracleConnection con;
        string error_title;
        #endregion

        #region Public Properties

        public RDL.DOM.ConnectionProperties ConnProperties { get; set; }

        #endregion

        # region Constructors

        public ConnectionPropertiesOracle(RDL.DOM.ConnectionProperties connectionProperties)
        {
            InitializeComponent();
            this.ConnProperties = connectionProperties;
            UpdateConnectionProperties();
            this.service_text.Focus();
            this.error_title = SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner");
        }

        # endregion

        #region Helper Methods

        void UpdateConnectionProperties()
        {
            this.pwd_Password.Password = string.Empty;
            this.txt_UserName.Text = string.Empty;
            this.service_text.Text = string.Empty;

            if ( this.ConnProperties != null && !string.IsNullOrEmpty(this.ConnProperties.ConnectString))
            {
                PopulateControlsFromConnectionString();
            }
        }

        private string GetTestConnectionString()
        {
            string connString = this.GetConnectionString();

            if (txt_UserName.Text != string.Empty && pwd_Password.Password != string.Empty)
            {
                connString += ConnectionConstants.UserID + "=" + txt_UserName.Text.ToString() + ";";
                connString += ConnectionConstants.Password + "=" + pwd_Password.Password.ToString() + ";";
            }

            return connString;
        }

        private string GetConnectionString()
        {
            return ConnectionConstants.DataSource + "=" + this.service_text.Text + ";";
        }

        /// <summary>
        /// Populates the controls from connection string.
        /// </summary>
        private void PopulateControlsFromConnectionString()
        {
            try
            {
                string tempString = this.ConnProperties.ConnectString;
                string[] strArray = tempString.Split(';');
                for (int i = 0; i < strArray.Length; i++)
                {
                    string[] subArray = strArray[i].Split('=');
                    if (ConnectionConstants.DataSource.ToLower() == subArray[0].ToLower())
                    {
                        this.service_text.Text = subArray[1];
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Testing the connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void btn_TestConnection_Click(object sender, RoutedEventArgs e)
        {
            this.CheckConnection();
        }

        /// <summary>
        /// Checking the Connection whether valid or invalid
        /// </summary>
        private void CheckConnection()
        {
            using (this.con = new OracleConnection(this.GetTestConnectionString()))
            {
                try
                {
                    con.Open();
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxConnectionSucceed"), this.error_title, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception sqlhandler)
                {
                    MessageBox.Show(sqlhandler.Message, this.error_title, MessageBoxButton.OK, MessageBoxImage.Warning);
                }                
            }
        }

        /// <summary>
        /// Reject the connection settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_TestConnectionCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Accept these connection settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_TestConnectionOk_Click(object sender, RoutedEventArgs e)
        {
            this.ConnectionDone();
        }

        /// <summary>
        /// Event activation when Window is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.PreviewKeyDown += new KeyEventHandler(ConnectionPropertiesOracle_PreviewKeyDown);
            this.service_text.Focus();
        }

        /// <summary>
        /// Event activation when KeyUp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConnectionPropertiesOracle_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ///Finding whether Escape is key is pressed
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }           
        }
        
        /// <summary>
        /// Checking the Server Connection valid or invalid
        /// </summary>
        private void ConnectionDone()
        {
            if (this.service_text.Text.ToString() != null && this.service_text.Text.ToString() != string.Empty)
            {
                if (this.ConnProperties == null)
                {
                    this.ConnProperties = new RDL.DOM.ConnectionProperties();
                }

                this.ConnProperties.ConnectString = this.GetTestConnectionString();
                this.ConnProperties.DataProvider = "ORACLE";
                this.ConnProperties.UserName = this.txt_UserName.Text;
                this.ConnProperties.PassWord = this.pwd_Password.Password;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxProvideServerName"), this.error_title);
                this.service_text.Focus();
            }
        }
       
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }
        #endregion

    }
}
