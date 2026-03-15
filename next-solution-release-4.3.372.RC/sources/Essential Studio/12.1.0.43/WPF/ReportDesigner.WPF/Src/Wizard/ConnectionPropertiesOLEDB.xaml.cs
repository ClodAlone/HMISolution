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
using System.ComponentModel;
using System.Data.OleDb;
using System.Data;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using System.Data.SqlClient;
using Syncfusion.Windows.Reports.Relational.Sql;
using Syncfusion.Windows.Reports.Sql;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ConnectionPropertiesOLEDB.xaml
    /// </summary>

#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class ConnectionPropertiesOLEDB : ChromelessWindow, IDisposable
    {
        List<string> OLEDBProviders;

        public RDL.DOM.ConnectionProperties ConnProperties { get; set; }
       
        public ConnectionPropertiesOLEDB(RDL.DOM.ConnectionProperties connectionProperties)
        {
            this.ConnProperties = connectionProperties;
            InitializeComponent();
            this.updateProvider();
            this.cmb_OLEDBProvider.SelectionChanged += new SelectionChangedEventHandler(cmb_OLEDBProvider_SelectionChanged);
            this.InitializeConnProperties();
         
        }
       
        void InitializeConnProperties()
        {
            this.ConnProperties.UserName = string.Empty;
            this.ConnProperties.PassWord = string.Empty;
            this.ConnProperties.IntegratedSecurity = false;
            rbtn_WindowsAuthentication.IsChecked = true;
            this.cmb_OLEDBProvider.IsEditable = true;
            this.cmb_catalog.IsEditable = true;
            if (this.ConnProperties.ConnectString!=string.Empty)
            {
                PopulateControlsFromConnectionString();
            }
        }

        void PopulateControlsFromConnectionString()
        {
            try
            {
                string tempString = this.ConnProperties.ConnectString;
                string[] strArray = tempString.Split(';');

                for (int i = 0; i < strArray.Length; i++)
                {
                    string[] subArray = strArray[i].Split('=');

                    if (subArray[0].ToString()=="Provider")
                    {
                        if (subArray[1].ToString() == "SQLNCLI10")
                        {
                            this.cmb_OLEDBProvider.Text = "SQL Server Native Client 10.0";
                        }
                        else
                        {
                            this.cmb_OLEDBProvider.Text = subArray[1];
                        }
                    }
                    else if (subArray[0].ToString() == "Data Source")
                    {
                        this.txt_FileName.Text = subArray[1];
                    }
                    else if (subArray[0].ToString() == " Server")
                    {
                        this.txt_FileName.Text = subArray[1];
                    }
                    else if (subArray[0].ToString() == "Database")
                    {
                        this.cmb_catalog.Text =subArray[1];
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void cmb_OLEDBProvider_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_OLEDBProvider.SelectedIndex == 0)
            {
                this.rbtn_SqlAuthentication.IsEnabled = false;
                this.disableInitialCatalog();
                this.btn_browse.IsEnabled = true;
            }

            if (cmb_OLEDBProvider.SelectedIndex == 1)
            {
                this.btn_browse.IsEnabled = false;
                this.enableInitialCatalog();
            }
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //this.cmb_ServerName.Focus();
        }

        void updateProvider()
        {
            this.OLEDBProviders = new List<string>();
            this.OLEDBProviders.Add("Microsof Jet 4.0 OLE DB Provider");
            //this.OLEDBProvider.Add("Microsoft OLE DB Provider for Analysis Services 10.0");
            //this.OLEDBProvider.Add("Microsoft OLE DB Provider for Data Mining Services");
            //this.OLEDBProvider.Add("Microsoft OLE DB Provider for Indexing Service");
            //this.OLEDBProvider.Add("Microsoft OLE DB Provider for OLAP Services 8.0");
            //this.OLEDBProvider.Add("Microsoft OLE DB Provider for Oracle");
            //this.OLEDBProvider.Add("Microsoft OLE DB Provider for Search");
            //this.OLEDBProvider.Add("Microsoft OLE DB Provider for SQL Server");
            //this.OLEDBProvider.Add("Microsoft OLE DB Simple Provider");
            //this.OLEDBProvider.Add("MSDataShape");
            //this.OLEDBProvider.Add("OLE DB Provider for Microsoft Directory Services");
            this.OLEDBProviders.Add("SQL Server Native Client 10.0");
            this.cmb_OLEDBProvider.ItemsSource = this.OLEDBProviders;
            //this.cmb_OLEDBProvider.SelectedIndex = this.OLEDBProvider.Count - 1;
        }

        void disableInitialCatalog()
        {
            grpbx_catalog.IsEnabled = false;
        }

        void enableInitialCatalog()
        {
            grpbx_catalog.IsEnabled = true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Close();
        }

        #endregion

        public string getconnectionstring()
        {
            string connectionString = string.Empty;

            if (cmb_OLEDBProvider.SelectedIndex == 0 && txt_FileName.Text != string.Empty)
            {
                connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + txt_FileName.Text;
            }
            if (cmb_OLEDBProvider.SelectedIndex == 1 && txt_FileName.Text != string.Empty)
            {
                connectionString = "Provider=SQLNCLI10; Server=" + txt_FileName.Text + ";Database=" + cmb_catalog.SelectedValue.ToString();
            }
          
            return connectionString;
        }
        
        private void btn_TestConnection_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_OLEDBProvider.SelectedIndex == 0 && txt_FileName.Text != string.Empty)
            {
                string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + txt_FileName.Text;
                System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connectionString);

                try
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxConnectionSucceed"), SR.GetString(CultureInfo.CurrentUICulture, "titleTestResult"), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "titleTestResult"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }

            if (cmb_OLEDBProvider.SelectedIndex == 1 && txt_FileName.Text != string.Empty)
            {
               
                if (rbtn_SqlAuthentication.IsChecked == true)
                {
                    string connectionstring = "Provider=SQLNCLI10; Server=" + txt_FileName.Text + ";Database=" + cmb_catalog.SelectionBoxItem.ToString() + ";Uid=" + txt_UserName.Text + "; Pwd=" + pwd_Password.Password;
                    System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connectionstring);
                    try
                    {
                        conn.Open();
                        if (conn.State == ConnectionState.Open)
                        {
                            MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxConnectionSucceed"), SR.GetString(CultureInfo.CurrentUICulture, "titleTestResult"), MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "titleTestResult"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else if (rbtn_WindowsAuthentication.IsChecked == true)
                {
                    
                    try
                    {
                        string connectionstring = "Provider=SQLNCLI10; Server=" + txt_FileName.Text + ";Database=" + cmb_catalog.SelectionBoxItem.ToString() + ";Trusted_Connection=yes";
                        System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(connectionstring);

                        conn.Open();
                        if (conn.State == ConnectionState.Open)
                        {
                            MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxConnectionSucceed"), SR.GetString(CultureInfo.CurrentUICulture, "titleTestResult"), MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "titleTestResult"), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void btn_TestConnectionOK_Click(object sender, RoutedEventArgs e)
        {
            string connetionString = getconnectionstring();
            ConnProperties.ConnectString = connetionString;
            if (cmb_OLEDBProvider.SelectedIndex == 1)
            {
                ConnProperties.IntegratedSecurity = (bool)rbtn_WindowsAuthentication.IsChecked;
                ConnProperties.UserName = this.txt_UserName.Text;
                ConnProperties.PassWord = this.pwd_Password.Password;
            }
                this.DialogResult = true;
                this.Close();
           
        }


        private void btn_browse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Microsoft Access Database files (*.mdb)|*.mdb";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txt_FileName.Text = openFileDialog.FileName;
            }
        }

        public void PopulateDataBase()
        {
            if ((bool)rbtn_WindowsAuthentication.IsChecked)
            {
                using (SqlConnection connection = new SqlConnection("Data Source=" + txt_FileName.Text + ";Trusted_Connection=Yes"))
                {
                    connection.Open();
                    DataTable myData = connection.GetSchema(SqlClientMetaDataCollectionNames.Databases);
                    foreach (DataRow row in myData.Rows)
                        cmb_catalog.Items.Add(row[0]);
                    connection.Close();
                }
            }
            else
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection("Data Source=" + txt_FileName.Text + ";User Id=" + this.txt_UserName.Text + ";Password=" + this.pwd_Password.Password))
                    {
                        connection.Open();
                        DataTable myData = connection.GetSchema(SqlClientMetaDataCollectionNames.Databases);
                        foreach (DataRow row in myData.Rows)
                            cmb_catalog.Items.Add(row[0]);
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void cmb_catalog_GotFocus(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrEmpty(this.txt_FileName.Text))
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxChooseServer"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"));
                return;
            }
            else
            {
                if (this.cmb_catalog.Items.Count <= 0)
                {
                    PopulateDataBase();
                }
            }
        }

        private void rbtn_WindowsAuthentication_Checked(object sender, RoutedEventArgs e)
        {
            this.grd_SqlServerAuthentication.IsEnabled = false;
            this.cmb_catalog.Items.Clear();
        }

        private void rbtn_SqlAuthentication_Checked(object sender, RoutedEventArgs e)
        {
            this.grd_SqlServerAuthentication.IsEnabled = true;
            this.cmb_catalog.Items.Clear();
        }

        private void btn_TestConnectionCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}