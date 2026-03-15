//-------------------------------------------------------------------------------------------------
// <copyright file="ConnectionProperties.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.Windows.Reports.Relational.Sql;
using Syncfusion.Windows.Reports.Sql;
using Syncfusion.Windows.Shared;
using System.ComponentModel;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using System.Collections.Generic;
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
    internal partial class ConnectionProperties
        : ChromelessWindow, IDisposable
    {
        #region Public Variables

        public RDL.DOM.ConnectionProperties ConnProperties { get; set; }

        #endregion

        #region Private Variables
        private SqlConnection sqlConnection;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnProperties"/> class.
        /// </summary>
        public ConnectionProperties(RDL.DOM.ConnectionProperties connectionProperties)
        {
            InitializeComponent();
            this.ConnProperties = connectionProperties;
            this.Title = SR.GetString(CultureInfo.CurrentUICulture, "titleConnectionProperties");
            UpdateConnectionProperties();

            if (string.IsNullOrEmpty(this.cmb_ServerName.Text))
            {
                this.DisableDataBaseBehaviours();
            }
            else
            {
                this.EnableDataBaseBehaviours();
            }

            this.cmb_Databases.IsEditable = true;
            EnableDisableSqlAuthentication();
            EventInitialization();
        }

        #endregion

        #region EventInitialization

        private void EventInitialization()
        {
            this.btn_TestConnectionOK.Click += new RoutedEventHandler(btn_TestConnectionOK_Click);
            this.btn_TestConnectionCancel.Click += new RoutedEventHandler(btn_TestConnectionCancel_Click);
            this.cmb_Databases.DropDownOpened += new EventHandler(cmb_Databases_DropDownOpened);
            this.btn_TestConnection.Click += new RoutedEventHandler(btn_TestConnection_Click);
            this.btn_BrowseFile.Click += new RoutedEventHandler(btn_BrowseFile_Click);
            this.rbtn_SelectDataBase.Click += new RoutedEventHandler(rbtn_SelectDataBase_Click);
            this.rbtn_AttachDataBaseFile.Click += new RoutedEventHandler(rbtn_AttachDataBaseFile_Click);
            this.rbtn_SqlAuthentication.Click += new RoutedEventHandler(rbtn_SqlAuthentication_Click);
            this.rbtn_WindowsAuthentication.Click += new RoutedEventHandler(rbtn_WindowsAuthentication_Click);
            this.rbtn_WindowsAuthentication.Checked += new RoutedEventHandler(chk_IntegratedSecurity_Checked);
            this.rbtn_WindowsAuthentication.Unchecked += new RoutedEventHandler(chk_IntegratedSecurity_Unchecked);
            this.PreviewKeyUp += new KeyEventHandler(ConnectionProperties_PreviewKeyUp);
            this.cmb_ServerName.DropDownOpened += new EventHandler(cmb_ServerName_DropDownOpened);
            this.cmb_ServerName.LostFocus += new RoutedEventHandler(cmb_ServerName_LostFocus);
            this.cmb_ServerName.KeyUp += new KeyEventHandler(cmb_ServerName_KeyUp);
            this.cmb_ServerName.DropDownClosed += new EventHandler(cmb_ServerName_DropDownClosed);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.UnWireEvents();
            base.OnClosing(e);
        }

        private void UnWireEvents()
        {
            this.btn_TestConnectionOK.Click -= new RoutedEventHandler(btn_TestConnectionOK_Click);
            this.btn_TestConnectionCancel.Click -= new RoutedEventHandler(btn_TestConnectionCancel_Click);
            this.cmb_Databases.DropDownOpened -= new EventHandler(cmb_Databases_DropDownOpened);
            this.btn_TestConnection.Click -= new RoutedEventHandler(btn_TestConnection_Click);
            this.btn_BrowseFile.Click -= new RoutedEventHandler(btn_BrowseFile_Click);
            this.rbtn_SelectDataBase.Click -= new RoutedEventHandler(rbtn_SelectDataBase_Click);
            this.rbtn_AttachDataBaseFile.Click -= new RoutedEventHandler(rbtn_AttachDataBaseFile_Click);
            this.rbtn_SqlAuthentication.Click -= new RoutedEventHandler(rbtn_SqlAuthentication_Click);
            this.rbtn_WindowsAuthentication.Click -= new RoutedEventHandler(rbtn_WindowsAuthentication_Click);
            this.rbtn_WindowsAuthentication.Checked -= new RoutedEventHandler(chk_IntegratedSecurity_Checked);
            this.rbtn_WindowsAuthentication.Unchecked -= new RoutedEventHandler(chk_IntegratedSecurity_Unchecked);
            this.PreviewKeyUp -= new KeyEventHandler(ConnectionProperties_PreviewKeyUp);
            this.cmb_ServerName.DropDownOpened -= new EventHandler(cmb_ServerName_DropDownOpened);
            this.cmb_ServerName.LostFocus -= new RoutedEventHandler(cmb_ServerName_LostFocus);
            this.cmb_ServerName.KeyUp -= new KeyEventHandler(cmb_ServerName_KeyUp);
            this.cmb_ServerName.DropDownClosed -= new EventHandler(cmb_ServerName_DropDownClosed);
        }

        void cmb_ServerName_DropDownClosed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.cmb_ServerName.Text))
            {
                this.DisableDataBaseBehaviours();
            }
            else
            {
                this.EnableDataBaseBehaviours();
            }
        }

        void UpdateConnectionProperties()
        {
            if (this.ConnProperties.IntegratedSecurity)
            {
                this.rbtn_WindowsAuthentication.IsChecked = true;
            }
            else
            {
                this.rbtn_SqlAuthentication.IsChecked = true;
            }

            this.cmb_ServerName.Text = String.Empty;
            this.txt_DataBaseFilePath.Text = String.Empty;
            this.cmb_Databases.Text = String.Empty;
            this.txt_LogicalName.Text = String.Empty;

            if (!string.IsNullOrEmpty(this.ConnProperties.ConnectString))
            {
                PopulateControlsFromConnectionString();
            }
        }

        void ConnectionProperties_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        
        #endregion

        #region Event Declarations

        private void btn_TestConnectionOK_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.cmb_ServerName.Text))
            {
                this.ConnProperties.ConnectString = this.GetConnectionString();
                this.ConnProperties.DataProvider = "SQL";
                this.ConnProperties.IntegratedSecurity = (bool)this.rbtn_WindowsAuthentication.IsChecked;

                this.ConnProperties.UserName = null;
                this.ConnProperties.PassWord = null;

                if ((bool)this.rbtn_SqlAuthentication.IsChecked && txt_UserName.Text.ToString() != string.Empty && pwd_Password.Password != string.Empty)
                {
                    this.ConnProperties.UserName = this.txt_UserName.Text;
                    this.ConnProperties.PassWord = this.pwd_Password.Password;
                }

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxProvideServerName"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"));
                this.cmb_ServerName.Focus();
            }
        }

        private void btn_BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Microsoft SQL Server files (*.mdf)|*.mdf|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txt_DataBaseFilePath.Text = openFileDialog.FileName;
            }
        }

        void cmb_Databases_DropDownOpened(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.cmb_ServerName.Text))
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxChooseServer"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"));
                return;
            }
            else
            {
                if (this.cmb_Databases.ItemsSource == null)
                {
                    PopulateDataBase();
                }
            }
        }

        private void chk_IntegratedSecurity_Checked(object sender, RoutedEventArgs e)
        {
            this.rbtn_SqlAuthentication.IsChecked = false;
            EnableDisableSqlAuthentication();
            this.cmb_Databases.ItemsSource = null;
            this.cmb_Databases.Text = string.Empty;
            PopulateDataBase();
            this.rbtn_SelectDataBase.IsEnabled = true;
            this.cmb_Databases.IsEnabled = true;

            this.rbtn_AttachDataBaseFile.IsEnabled = true;
            this.grd_AttachDataBaseFile.IsEnabled = true;
        }

        private void chk_IntegratedSecurity_Unchecked(object sender, RoutedEventArgs e)
        {
            this.rbtn_SqlAuthentication.IsChecked = true;
            EnableDisableSqlAuthentication();
            this.cmb_Databases.ItemsSource = null;
            this.cmb_Databases.Text = string.Empty;

            this.rbtn_SelectDataBase.IsEnabled = false;
            this.rbtn_AttachDataBaseFile.IsEnabled = false;
            this.cmb_Databases.IsEnabled = false;
            this.grd_AttachDataBaseFile.IsEnabled = false;
        }

        private void txt_UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.cmb_ServerName.Text) || string.IsNullOrEmpty(this.txt_UserName.Text) )
            {
                this.rbtn_SelectDataBase.IsEnabled = false;
                this.rbtn_AttachDataBaseFile.IsEnabled = false;
                this.cmb_Databases.IsEnabled = false;
                this.grd_AttachDataBaseFile.IsEnabled = false;
            }
            else
            {
                this.rbtn_SelectDataBase.IsEnabled = true;
                this.rbtn_AttachDataBaseFile.IsEnabled = true;
                this.cmb_Databases.IsEnabled = true;
                this.grd_AttachDataBaseFile.IsEnabled = true;
            }
        }

        private void btn_TestConnectionCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btn_TestConnection_Click(object sender, RoutedEventArgs e)
        {
            sqlConnection = new SqlConnection(this.GetTestConnectionString());
            try
            {
                sqlConnection.Open();
                if (sqlConnection.State == ConnectionState.Open)
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxConnectionSucceed"), SR.GetString(CultureInfo.CurrentUICulture, "titleTestResult"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,  SR.GetString(CultureInfo.CurrentUICulture, "titleTestResult"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void rbtn_AttachDataBaseFile_Click(object sender, RoutedEventArgs e)
        {
            this.cmb_Databases.ItemsSource = null;
            EnableDisableDataBases();
        }

        private void rbtn_SelectDataBase_Click(object sender, RoutedEventArgs e)
        {
            PopulateDataBase();
            EnableDisableDataBases();
        }

        private void EnableDisableDataBases()
        {
            foreach (UIElement element in this.grd_AttachDataBaseFile.Children)
            {
                element.IsEnabled = (bool)this.rbtn_AttachDataBaseFile.IsChecked;
            }

            this.cmb_Databases.IsEnabled = !(bool)this.rbtn_AttachDataBaseFile.IsChecked;
        }

        private void rbtn_WindowsAuthentication_Click(object sender, RoutedEventArgs e)
        {
            EnableDisableSqlAuthentication();
        }

        private void rbtn_SqlAuthentication_Click(object sender, RoutedEventArgs e)
        {
            EnableDisableSqlAuthentication();
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.cmb_ServerName.Focus();
        }

        #endregion

        #region Private Methods

        void UpdateDataSources()
        {
            cmb_Databases.ItemsSource = null;
        }

        void cmb_ServerName_LostFocus(object sender, RoutedEventArgs e)
        {
            this.UpdateDataSources();
            this.PopulateDataBase();
        }

        void cmb_ServerName_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(this.cmb_ServerName.Text))
            {
                this.DisableDataBaseBehaviours();
            }
            else
            {
                this.EnableDataBaseBehaviours();
            }

            this.UpdateDataSources();
        }

        void cmb_ServerName_DropDownOpened(object sender, EventArgs e)
        {
            if (this.cmb_ServerName.ItemsSource == null)
            {
                cmb_ServerName.IsDropDownOpen = false;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                this.busyIndicator.IsBusy = true;
                worker.RunWorkerAsync();
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.cmb_ServerName.ItemsSource = e.Result as List<string>;
            this.UpdateDataSources();
            this.busyIndicator.IsBusy = false;
            this.cmb_ServerName.IsDropDownOpen = true;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Data.Sql.SqlDataSourceEnumerator sqlInstanceEnum = System.Data.Sql.SqlDataSourceEnumerator.Instance;
            DataTable sqlInstanceTable = sqlInstanceEnum.GetDataSources();
            List<string> serverNames = new List<string>();

            foreach (DataRow row in sqlInstanceTable.Rows)
            {
                serverNames.Add(row[0].ToString());
            }

            serverNames.Sort();

            e.Result = serverNames;
        }

        private void DisableDataBaseBehaviours()
        {
            this.btn_TestConnectionOK.IsEnabled = false;
            foreach (UIElement element in this.stck_DataBase.Children)
            {
                if (element.IsEnabled)
                {
                    element.IsEnabled = false;
                }
            }

            foreach (UIElement element in this.grd_AttachDataBaseFile.Children)
            {
                element.IsEnabled = false;
            }
        }

        private void EnableDataBaseBehaviours()
        {
            foreach (UIElement element in this.stck_DataBase.Children)
            {
                if (!element.IsEnabled)
                {
                    element.IsEnabled = true;
                }
            }

            this.btn_TestConnectionOK.IsEnabled = true;
            EnableDisableDataBases();
        }

        private void EnableDisableSqlAuthentication()
        {
            foreach (UIElement uiElement in grd_SqlServerAuthentication.Children)
            {
                uiElement.IsEnabled = (bool)this.rbtn_SqlAuthentication.IsChecked;
            }
        }

        private void PopulateDataBase()
        {
          
                this.cmb_Databases.ItemsSource = null;

                BackgroundWorker worker = new BackgroundWorker();
                List<string> databases = new List<string>();
                string connectionString = GetTestConnectionString();

                worker.DoWork += (sen,arg) =>
                    {
                        try
                        {
                            SqlConnection dbConnection = new SqlConnection(connectionString);
                            dbConnection.Open();
                            SqlSchemaProvider sqlSchemaProvider = new SqlSchemaProvider(dbConnection);
                            DataTable dataTable = new DataTable();
                            dataTable = sqlSchemaProvider.GetDatabases();

                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                databases.Add(dataTable.Rows[i].ItemArray[0].ToString());
                            }
                        }
                        catch (Exception)
                        {
                            //MessageBox.Show(ex.Message, "Synfusion Essential ReportDefinition Designer Control");
                        }
                    };

                worker.RunWorkerCompleted += (sen, arg) =>
                    {
                        this.cmb_Databases.ItemsSource = databases;
                    };

                worker.RunWorkerAsync();

        }

        private string GetTestConnectionString()
        {
            string connString= this.GetConnectionString();

            if ((bool)this.rbtn_SqlAuthentication.IsChecked && txt_UserName.Text.ToString() != string.Empty && pwd_Password.Password != string.Empty)
            {
                connString += ConnectionConstants.UserID + "=" + txt_UserName.Text.ToString() + ";";
                connString += ConnectionConstants.Password + "=" + pwd_Password.Password.ToString() + ";";
            }

            if ((this.rbtn_WindowsAuthentication.IsChecked != null) && (this.rbtn_WindowsAuthentication.IsChecked == true))
            {
                connString += "Trusted_Connection=" + (bool)this.rbtn_WindowsAuthentication.IsChecked;
            }

            return connString;
        }

        private string GetConnectionString()
        {
            string tempConnectionString = string.Empty;
            tempConnectionString = ConnectionConstants.DataSource + "=" + this.cmb_ServerName.Text.ToString() + ";";
           
            if ((bool)this.rbtn_SelectDataBase.IsChecked && this.cmb_Databases.Text.ToString() != string.Empty)
            {
                tempConnectionString += ConnectionConstants.InitialCatalog + "=" + this.cmb_Databases.Text.ToString() + ";";
            }
            else if ((bool)this.rbtn_AttachDataBaseFile.IsChecked)
            {
                tempConnectionString += ConnectionConstants.AttachDbFileName + "=" + this.txt_DataBaseFilePath.Text.ToString() + ";";
                
                if (this.txt_LogicalName.Text.ToString() != string.Empty)
                {
                    tempConnectionString += ConnectionConstants.InitialCatalog + "=" + this.txt_LogicalName.Text.ToString() + ";";
                }
            }           

            return tempConnectionString;
        }

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
                        this.cmb_ServerName.Text = subArray[1];
                    }
                    else if (ConnectionConstants.InitialCatalog.ToLower() == subArray[0].ToLower())
                    {                        
                        this.cmb_Databases.Text = subArray[1];
                        this.rbtn_SelectDataBase.IsChecked = true;
                    }
                    else if (ConnectionConstants.AttachDbFileName.ToLower() == subArray[0].ToLower())
                    {
                        this.txt_DataBaseFilePath.Text = subArray[1];
                        this.rbtn_AttachDataBaseFile.IsChecked = true;
                    }
                }
            }
            catch
            { }
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
