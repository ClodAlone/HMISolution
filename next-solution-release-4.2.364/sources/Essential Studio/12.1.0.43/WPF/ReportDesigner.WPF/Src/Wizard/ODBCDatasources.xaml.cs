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
using Syncfusion.Windows.Reports.Designer.Wizard;
using Syncfusion.Windows.Shared;
using System.Data;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ODBCDatasources.xaml
    /// </summary>
    internal partial class ODBCDatasources : ChromelessWindow
    {
        public ODBCDatasources()
        {
            InitializeComponent();
            InitializeUI();
        }
        public string ConnectString { get; set; }

        public string DSNType { get; set; }

        public string FileName { get; set; }

        public string DirectoryName { get; set; }

        private void InitializeUI()
        {
            datasourcelst.Items.Clear();
            datasourcelst.ItemsSource = GetDrivers();
        }

        private IEnumerable<string> GetDrivers()
        {
            List<string> names = new List<string>();
            // get system dsn's
            Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.LocalMachine).OpenSubKey("Software");
            if (reg != null)
            {
                reg = reg.OpenSubKey("ODBC");
                if (reg != null)
                {
                    reg = reg.OpenSubKey("ODBCINST.INI");
                    if (reg != null)
                    {

                        reg = reg.OpenSubKey("ODBC Drivers");
                        if (reg != null)
                        {
                            // Get all DSN entries defined in DSN_LOC_IN_REGISTRY.
                            foreach (string sName in reg.GetValueNames())
                            {
                                if (sName == "PostgreSQL Unicode" || sName == "PostgreSQL Unicode(x64)" || sName == "PostgreSQL ODBC Driver(UNICODE)"
                                    || sName == "PostgreSQL ANSI" || sName == "PostgreSQL ANSI(x64)" || sName == "PostgreSQL ODBC Driver(ANSI)")
                                    names.Add(sName);
                            }
                        }
                        try
                        {
                            reg.Close();
                        }
                        catch { /* ignore this exception if we couldn't close */ }
                    }
                }
            }

            names.Add("Microsoft Excel Driver (*.xlsx)");
            names.Add("Microsoft Access Driver (*.mdb)");
            return names;
        }

        private void btn_browse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(openFileDialog_FileOk);
            this.FileName = openFileDialog.FileName;
            this.DSNType = this.txt_dsn.Text.ToString();
            
            bool isAccesSource = (this.datasourcelst.SelectedItem.ToString() == "MS Access Database") ? true : false;

            if (this.datasourcelst.SelectedItem.ToString() == "Microsoft Access Driver (*.mdb)")
            {
                openFileDialog.Filter = "Microsoft Access Database files (*.mdb)|*.mdb";
            }
            else if (this.datasourcelst.SelectedItem.ToString() == "Microsoft Excel Driver (*.xlsx)")
            {
                openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";          
            }

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(fileName);

                if (!string.IsNullOrEmpty(this.DSNType) && (isAccesSource || extension == ".mdb" || extension == ".accdb"))
                {
                    this.ConnectString = "Dsn=" + this.DSNType + ";" + "Driver={Microsoft Access Driver (*.mdb,*.accdb)};" + @"Dbq=" + fileName + ";";
                }
                else if (!string.IsNullOrEmpty(this.DSNType) && (extension == ".xls" || extension == ".xlsx"))
                {
                    string dir = System.IO.Path.GetDirectoryName(fileName);
                    this.ConnectString = "Dsn=" + this.DSNType + ";" ;
                }
            }
        }
        void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

            System.Windows.Forms.OpenFileDialog openFileDialog = sender as System.Windows.Forms.OpenFileDialog;
            this.FileName = openFileDialog.FileName;
            string extension = System.IO.Path.GetExtension(this.FileName);

            if (openFileDialog.Filter == "Microsoft Access Database files (*.mdb)|*.mdb" || extension == ".mdb" || extension == ".accdb")
            {
                this.ConnectString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=" + this.FileName.ToString() + ";Uid=" + this.txt_username.ToString()
                    + ";Pwd=" + this.txt_pwd.Password.ToString() + ";";
                this.txt_db.Text = this.FileName;
            }
            else if (openFileDialog.Filter == "Excel files (*.xlsx)|*.xlsx" || extension == ".xls" || extension == ".xlsx")
            {
                this.DirectoryName = System.IO.Path.GetDirectoryName(this.FileName);
                this.txt_db.Text = this.FileName;
             }
        }

        private void datasourcelst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.datasourcelst.SelectedItem.ToString() == "Microsoft Excel Driver (*.xlsx)")
            {
                this.Postgre_grid.IsEnabled = false;
                this.btn_browse.IsEnabled = true;
            }
            else if (this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ANSI(x64)" || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL Unicode(x64)"
                || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ANSI" || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL Unicode" ||
                this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ODBC Driver(UNICODE)" || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ODBC Driver(ANSI)")
            {
                this.Postgre_grid.IsEnabled = true;
                this.btn_browse.IsEnabled = false;
                this.txt_servername.IsEnabled = true;
                this.txt_portname.IsEnabled = true;
            }
            else if (this.datasourcelst.SelectedItem.ToString() == "Microsoft Access Driver (*.mdb)")
            {
                this.Postgre_grid.IsEnabled = true;
                this.btn_browse.IsEnabled = true;
                this.txt_servername.IsEnabled = false;
                this.txt_portname.IsEnabled = false;               
            }
            else
            {
                this.Postgre_grid.IsEnabled = false;
                this.btn_browse.IsEnabled = false;
            }
        }

        private void btn_testconnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.datasourcelst.SelectedItem.ToString() == "Microsoft Access Driver (*.mdb)")
                {
                    this.ConnectString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=" + this.FileName.ToString() + ";Uid=" + this.txt_username.Text.ToString() + ";Pwd=" + this.txt_pwd.Password.ToString() + ";";
                }

                if (this.datasourcelst.SelectedItem.ToString() == "Microsoft Excel Driver (*.xlsx)")
                {
                    this.ConnectString = "Driver={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)};DriverId=790;Dbq=" + this.FileName.ToString()
                        + ";DefaultDir=" + this.DirectoryName + ";HDR=YES;";
                }

                else if (this.datasourcelst.SelectedItem.ToString() == "PostgreSQL Unicode" || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ODBC Driver(UNICODE)"
                    || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL Unicode(x64)")
                {
                    try
                    {
                        this.ConnectString = "Driver={PostgreSQL UNICODE};Server=" + this.txt_servername.Text.ToString()
                            + ";Port=5432;Database=" + txt_db.Text.ToString() + ";Uid=" + this.txt_username.Text.ToString()
                            + ";Pwd=" + this.txt_pwd.Password.ToString();
                        System.Data.Odbc.OdbcConnection connection = new System.Data.Odbc.OdbcConnection(this.ConnectString);
                        connection.Open();
                    }
                    catch
                    {
                        this.ConnectString = "Driver={PostgreSQL UNICODE(x64)};Server=" + this.txt_servername.Text.ToString()
                            + ";Port=" + this.txt_portname.Text.ToString() + ";Database=" + this.txt_db.Text.ToString() + ";Uid=" + this.txt_username.Text.ToString()
                            + ";Pwd=" + this.txt_pwd.Password.ToString();
                    }
                }

                else if (this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ANSI" || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ODBC Driver(ANSI)"
                    || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ANSI(x64)")
                {
                    try
                    {
                        this.ConnectString = " Driver={PostgreSQL ANSI};Server=" + this.txt_servername.Text.ToString()
                            + ";Port=" + this.txt_portname.Text.ToString() + ";Database=" + this.txt_db.Text.ToString()
                            + ";Uid=" + this.txt_username.Text.ToString() + ";Pwd=" + this.txt_pwd.Password.ToString();
                        System.Data.Odbc.OdbcConnection connection = new System.Data.Odbc.OdbcConnection(this.ConnectString);
                        connection.Open();
                    }
                    catch
                    {
                        this.ConnectString = "Driver={PostgreSQL ANSI(x64)};Server=" + this.txt_servername.Text.ToString()
                            + ";Port=" + this.txt_portname.Text.ToString() + ";Database=" + this.txt_db.Text.ToString()
                            + ";Uid=" + this.txt_username.Text.ToString() + ";Pwd=" + this.txt_pwd.Password.ToString();
                    }
                }

                System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection(this.ConnectString);
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

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            if (this.ConnectString == string.Empty)
            {
                try
                {
                    if (this.datasourcelst.SelectedItem.ToString() == "Microsoft Access Driver (*.mdb)")
                    {
                        this.ConnectString = "Driver={Microsoft Access Driver (*.mdb)};Dbq=" + this.FileName.ToString() + ";Uid=" + this.txt_username.Text.ToString() + ";Pwd=" + this.txt_pwd.Password.ToString() + ";";
                    }

                    if (this.datasourcelst.SelectedItem.ToString() == "Microsoft Excel Driver (*.xlsx)")
                    {
                        this.ConnectString = "Driver={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)};DriverId=790;Dbq=" + this.FileName.ToString()
                            + ";DefaultDir=" + this.DirectoryName + ";HDR=YES;";
                    }

                    else if (this.datasourcelst.SelectedItem.ToString() == "PostgreSQL Unicode" || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ODBC Driver(UNICODE)"
                        || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL Unicode(x64)")
                    {
                        try
                        {
                            this.ConnectString = "Driver={PostgreSQL UNICODE};Server=" + this.txt_servername.Text.ToString()
                                + ";Port=5432;Database=" + txt_db.Text.ToString() + ";Uid=" + this.txt_username.Text.ToString()
                                + ";Pwd=" + this.txt_pwd.Password.ToString();
                            System.Data.Odbc.OdbcConnection connection = new System.Data.Odbc.OdbcConnection(this.ConnectString);
                            connection.Open();
                        }
                        catch
                        {
                            this.ConnectString = "Driver={PostgreSQL UNICODE(x64)};Server=" + this.txt_servername.Text.ToString()
                                + ";Port=" + this.txt_portname.Text.ToString() + ";Database=" + this.txt_db.Text.ToString() + ";Uid=" + this.txt_username.Text.ToString()
                                + ";Pwd=" + this.txt_pwd.Password.ToString();
                        }
                    }

                    else if (this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ANSI" || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ODBC Driver(ANSI)"
                        || this.datasourcelst.SelectedItem.ToString() == "PostgreSQL ANSI(x64)")
                    {
                        try
                        {
                            this.ConnectString = " Driver={PostgreSQL ANSI};Server=" + this.txt_servername.Text.ToString()
                                + ";Port=" + this.txt_portname.Text.ToString() + ";Database=" + this.txt_db.Text.ToString()
                                + ";Uid=" + this.txt_username.Text.ToString() + ";Pwd=" + this.txt_pwd.Password.ToString();
                            System.Data.Odbc.OdbcConnection connection = new System.Data.Odbc.OdbcConnection(this.ConnectString);
                            connection.Open();
                        }
                        catch
                        {
                            this.ConnectString = "Driver={PostgreSQL ANSI(x64)};Server=" + this.txt_servername.Text.ToString()
                                + ";Port=" + this.txt_portname.Text.ToString() + ";Database=" + this.txt_db.Text.ToString()
                                + ";Uid=" + this.txt_username.Text.ToString() + ";Pwd=" + this.txt_pwd.Password.ToString();
                        }
                    }
                }
                catch { }
            }
            this.DialogResult = true;
            this.Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        } 
    }
}
