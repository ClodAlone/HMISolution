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
using System.Data;
using Syncfusion.Windows.Reports.Designer.Wizard;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ConnectionPropertiesODBC.xaml
    /// </summary>
    internal partial class ConnectionPropertiesODBC
    {
        public RDL.DOM.ConnectionProperties ConnProperties { get; set; }

        public string FileName { get; set; }
        public string DSNType { get; set; }

        public ConnectionPropertiesODBC(RDL.DOM.ConnectionProperties connectionProperties)
        {
            // TODO: Complete member initialization
            this.ConnProperties = connectionProperties;
            InitializeComponent();
            InitializeUI();
            this.InitializeConnProperties();
        }

        public void InitializeUI()
        {
            txt_username.Text = string.Empty;
            pwd_userpassword.Password = string.Empty;
            btn_testok.IsEnabled = false;
            rbtn_sourcename.IsChecked = true;
            src_grid.IsEnabled = true;
            txt_connectiostring.IsEnabled = false;
            btn_buildconnectionstring.IsEnabled = false;
            btn_testok.IsEnabled = true;
            cmb_sourcename.Items.Clear();
            cmb_sourcename.ItemsSource = EnumDsn(Microsoft.Win32.Registry.CurrentUser);
            cmb_sourcename.SelectedIndex = 0;
        }

        private IEnumerable<string> EnumDsn(Microsoft.Win32.RegistryKey rootKey)
        {
            Microsoft.Win32.RegistryKey regKey = rootKey.OpenSubKey(@"Software\ODBC\ODBC.INI\ODBC Data Sources");
            if (regKey != null)
            {
                foreach (string name in regKey.GetValueNames())
                {
                    if (name != "dBASE Files")
                    {
                        string value = regKey.GetValue(name, "").ToString();
                        yield return name;
                    }
                }
            }
        }
       void InitializeConnProperties()
        {
            if (this.ConnProperties.ConnectString != string.Empty)
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
                   if (subArray[0].ToString() == "Dsn")
                   {
                       this.txt_connectiostring.Text = this.ConnProperties.ConnectString;
                       this.cmb_sourcename.SelectedItem = subArray[1].ToString();
                   }
                   else
                   {
                       this.txt_connectiostring.Text = this.ConnProperties.ConnectString;
                   }
               }
           }
           catch
           { }
       }

        void Wireevents()
        {
            this.btn_buildconnectionstring.Click += new RoutedEventHandler(btn_buildconnectionstring_Click);
        }

        private void rbtn_sourcename_Checked(object sender, RoutedEventArgs e)
        {
            src_grid.IsEnabled = true;
            txt_connectiostring.IsEnabled = true;
            btn_buildconnectionstring.IsEnabled = false;
            if (cmb_sourcename.SelectionBoxItem.ToString() != null)
            {
                txt_connectiostring.Text = "Dsn=" + cmb_sourcename.SelectionBoxItem.ToString();
            }
        }

        private void con_str_radbtn_Checked(object sender, RoutedEventArgs e)
        {
            constr_grid.IsEnabled = true;
            txt_connectiostring.IsEnabled = true;
            btn_buildconnectionstring.IsEnabled = true;
            src_grid.IsEnabled = false;

            if (cmb_sourcename.SelectionBoxItem.ToString() != null)
            {
                txt_connectiostring.Text = "Dsn=" + cmb_sourcename.SelectionBoxItem.ToString();
            }
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_testconnection_Click(object sender, RoutedEventArgs e)
        {
            this.txt_connectiostring.Text = string.Empty;
            if (cmb_sourcename.SelectedItem.ToString()=="MS Access Database")
            {
                this.txt_connectiostring.Text = this.ConnProperties.ConnectString+";Uid=Admin";
            }
            else
            {
                this.txt_connectiostring.Text = this.ConnProperties.ConnectString;
            }
            if (this.ConnProperties.ConnectString == string.Empty)
            {
                this.txt_connectiostring.Text = "DSN="+this.cmb_sourcename.SelectedItem.ToString()+";";
                this.ConnProperties.ConnectString = "DSN=" + this.cmb_sourcename.SelectedItem.ToString()+";";
                this.ConnProperties.UserName = string.Empty;
                this.ConnProperties.PassWord = string.Empty;
            }
            System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection(this.txt_connectiostring.Text);

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

        public void btn_testok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
            if (this.cmb_sourcename.SelectedItem.ToString() == "MS Access Database")
            {
                this.ConnProperties.UserName = this.txt_username.Text;
                this.pwd_userpassword.Password = this.pwd_userpassword.Password;
            }
        }

        void btn_refreshdsn_Click(object sender, RoutedEventArgs e)
        {
            cmb_sourcename.ItemsSource = EnumDsn(Microsoft.Win32.Registry.CurrentUser);
            cmb_sourcename.SelectedIndex = 0;
        }

        void btn_buildconnectionstring_Click(object sender, RoutedEventArgs e)
        {
            ODBCDatasources datasources = new ODBCDatasources();
            datasources.Owner = Window.GetWindow(this.btn_buildconnectionstring);
            SkinStorage.SetVisualStyle(datasources, SkinStorage.GetVisualStyle(datasources.Owner));
            if (datasources.ShowDialog() == true)
            {
                try
                {
                    this.ConnProperties.ConnectString = datasources.ConnectString;
                    this.txt_connectiostring.Text = datasources.ConnectString.ToString();
                    this.ConnProperties.UserName = datasources.txt_username.Text.ToString();
                    this.ConnProperties.PassWord = datasources.txt_pwd.Password.ToString();
                }
                catch { }
            }
            //bool isAccessSource = (this.cmb_sourcename.SelectedItem.ToString() == "MS Access Database") ? true : false;
            //if (isAccessSource == true && this.txt_username.Text != string.Empty && this.pwd_userpassword.Password != string.Empty)
            //{
            //    this.BuildConnection();
            //}
            //else if (isAccessSource == true && this.txt_username.Text == string.Empty && this.pwd_userpassword.Password == string.Empty)
            //{
            //    MessageBox.Show("credentials needed", "credentials");
            //}
            //else
            //{
            //    this.BuildConnection();
            //}
        }

        private void BuildConnection()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(openFileDialog_FileOk);
            this.FileName = openFileDialog.FileName;
            this.DSNType = this.cmb_sourcename.SelectedItem.ToString();
            bool isAccesSource = (this.cmb_sourcename.SelectedItem.ToString() == "MS Access Database") ? true : false;

            if (this.cmb_sourcename.SelectedItem.ToString()=="MS Access Database")
            {
                   openFileDialog.Filter = "Microsoft Access Database files (*.mdb)|*.mdb";             
            }
            else if (this.cmb_sourcename.SelectedItem.ToString() == "Excel Files")
            {
                openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            }
            else
            {
                openFileDialog.Filter = "All files (*.*)|*.*|Excel files (*.xlsx)|*.xlsx|Microsoft Access Database files (*.mdb)|*.mdb";
            }

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(fileName);

                if (!string.IsNullOrEmpty(this.DSNType) && (isAccesSource || extension == ".mdb" || extension == ".accdb"))
                {
                    this.ConnProperties.ConnectString = "Dsn="+this.DSNType+";" + "Driver={Microsoft Access Driver (*.mdb,*.accdb)};" + @"Dbq=" + fileName + ";";
                }
                else if (!string.IsNullOrEmpty(this.DSNType) && (extension == ".xls" || extension == ".xlsx"))
                {
                    string dir = System.IO.Path.GetDirectoryName(fileName);
                    this.ConnProperties.ConnectString = "Dsn="+this.DSNType+";" + "Driver={Microsoft Excel Driver (*.xls)};"
                        + "Driverid=790;" + @"Dbq=" + fileName + ";"
                        + @"DefaultDir=" + dir + ";"
                      + "HDR=YES;";        
                }
            }
        }

        void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

            System.Windows.Forms.OpenFileDialog openFileDialog = sender as System.Windows.Forms.OpenFileDialog;
            this.FileName = openFileDialog.FileName;
            string extension = System.IO.Path.GetExtension(this.FileName);

            if (!string.IsNullOrEmpty(this.DSNType) && (openFileDialog.Filter == "Microsoft Access Database files (*.mdb)|*.mdb" || extension == ".mdb" || extension == ".accdb"))
            {
                this.ConnProperties.ConnectString = "Dsn="+this.DSNType+";" + "Driver={Microsoft Access Driver (*.mdb,*.accdb)};" + @"Dbq=" + this.FileName + ";";
            }
            else if (!string.IsNullOrEmpty(this.DSNType) && (openFileDialog.Filter == "Excel files (*.xlsx)|*.xlsx" || extension == ".xls" || extension == ".xlsx"))
            {
                string dir = System.IO.Path.GetDirectoryName(this.FileName);
                this.ConnProperties.ConnectString = "Dsn="+this.DSNType+";" + "Driver={Microsoft Excel Driver (*.xls)};"
                    + "Driverid=790;" + @"Dbq=" + this.FileName + ";"
                    + @"DefaultDir=" + dir + ";"
                  + "HDR=YES;";
            }
            this.txt_connectiostring.Text = this.ConnProperties.ConnectString;
        }
    }
}

