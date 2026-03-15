#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Microsoft.Win32;
using Syncfusion.Olap.Manager;
using Syncfusion.Windows.Shared;
using Syncfusion.Olap.DataProvider;

namespace Syncfusion.Windows.Client.Olap
{
    /// <summary>
    /// Interaction logic for ConnectOptions.xaml
    /// </summary>
    /// 
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public partial class ConnectOptions : ChromelessWindow
    {
        #region Variables

        static string dBName;
        static string serverName;
        static string dataSource = "OfflineCube";
        static Stack<string> serverNameStack = new Stack<string>();
        static Stack<string> databaseNameStack = new Stack<string>();

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectOptions"/> class.
        /// </summary>
        public ConnectOptions()
        {
            InitializeComponent();
            try
            {
                this.ConnectionString = string.Empty;
                switch (dataSource)
                {
                    case "OfflineCube":
                        chkCustomOfflineCube.IsChecked = true;
                        chkCutomServer.IsChecked = false;
                        cmbServerName.IsEnabled = false;
                        cmbDatabaseName.IsEnabled = false;
                        break;
                    case "CustomServer":
                        chkCutomServer.IsChecked = true;
                        chkCustomOfflineCube.IsChecked = false;
                        cmbServerName.IsEnabled = true;
                        cmbDatabaseName.IsEnabled = true;
                        cmbServerName.Text = serverName;
                        cmbDatabaseName.Text = dBName;
                        break;
                    default:
                        chkCustomOfflineCube.IsChecked = true;
                        break;
                }
                LoadServerComboBox();
                LoadDBComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectOptions"/> class.
        /// </summary>
        /// <param name="connection">The connection String</param>
        public ConnectOptions(string connection)
            : this()
        {
            if (!String.IsNullOrEmpty(connection))
            {
                this.txtconnectionString.Text = connection; 
            }
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="ConnectOptions"/> class.
        /// </summary>
        /// <param name="connection">The connection string</param>
        /// <param name="providerName">A name of the service provider</param>
        public ConnectOptions(string connection, Providers providerName)
            : this()
        {
            if (!String.IsNullOrEmpty(connection))
            {
                this.txtconnectionString.Text = connection;
                switch (providerName)
                {
                    case Providers.SSAS:
                        this.providerSSAS.IsChecked = true;
                        break;
                    case Providers.Mondrian:
                        this.providerMondrian.IsChecked = true;
                        break;
                    case Providers.ActivePivot:
                        this.providerActivePivot.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Gets or sets the name of the service provider.
        /// </summary>
        public Providers ProviderName { get; set; }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        bool Validate()
        {
            if (this.chkCustomOfflineCube.IsChecked == true)
            {
                if (File.Exists(this.txtOfflineCubeFilePath.Text))
                {
                    this.ConnectionString = Common.GetLocalCubeConnectionString(this.txtOfflineCubeFilePath.Text);
                    if (this.providerSSAS.IsChecked.HasValue && this.providerSSAS.IsChecked.Value)
                        this.ProviderName = Providers.SSAS;
                    else if (this.providerMondrian.IsChecked.HasValue && this.providerMondrian.IsChecked.Value)
                        this.ProviderName = Providers.Mondrian;
                    else if (this.providerActivePivot.IsChecked.HasValue && this.providerActivePivot.IsChecked.Value)
                        this.ProviderName = Providers.ActivePivot;
                    return true;
                }
                else
                {
                    MessageBox.Show("Specified file not found", "File not found");
                    return false;
                }
            }
            else if (this.chkCutomServer.IsChecked == true)
            {
                if (this.chkCutomServer.IsChecked == true)
                {

                    if (this.cmbServerName.Text != null && this.cmbServerName.Text.Trim() != string.Empty)
                    {

                        if (this.cmbDatabaseName.Text != null && this.cmbDatabaseName.Text.Trim() != string.Empty)
                        {
                            if (this.txtUsername.Text != string.Empty && this.txtPassword.Password != string.Empty)
                            {
                                this.ConnectionString = Common.GetServerConnectionString(cmbServerName.Text, cmbDatabaseName.Text, this.txtUsername.Text, this.txtPassword.Password);
                            }
                            else
                            {
                                this.ConnectionString = Common.GetServerConnectionString(cmbServerName.Text, cmbDatabaseName.Text);
                            }
                            OlapDataManager cubeModel = new OlapDataManager(this.ConnectionString);
                            if (cubeModel.DataProvider.ValidateConnectionString())
                            {
                                if (this.providerSSAS.IsChecked.HasValue && this.providerSSAS.IsChecked.Value)
                                    this.ProviderName = Providers.SSAS;
                                else if (this.providerMondrian.IsChecked.HasValue && this.providerMondrian.IsChecked.Value)
                                    this.ProviderName = Providers.Mondrian;
                                else if (this.providerActivePivot.IsChecked.HasValue && this.providerActivePivot.IsChecked.Value)
                                    this.ProviderName = Providers.ActivePivot;
                                return true;
                            }

                        }
                        else
                        {
                            MessageBox.Show("Please enter the database name", "Error");
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter the server name", "Error");
                        return false;
                    }
                }

                dataSource = "CustomServer";
            }
            else if (this.chkCustomConnectionString.IsChecked == true)
            {
                if (this.txtconnectionString.Text != string.Empty)
                {
                    this.ConnectionString = this.txtconnectionString.Text;
                    OlapDataManager cubeModel = new OlapDataManager(this.ConnectionString);
                    if (cubeModel.DataProvider.ValidateConnectionString())
                    {
                        if (this.providerSSAS.IsChecked.HasValue && this.providerSSAS.IsChecked.Value)
                            this.ProviderName = Providers.SSAS;
                        else if (this.providerMondrian.IsChecked.HasValue && this.providerMondrian.IsChecked.Value)
                            this.ProviderName = Providers.Mondrian;
                        else if (this.providerActivePivot.IsChecked.HasValue && this.providerActivePivot.IsChecked.Value)
                            this.ProviderName = Providers.ActivePivot;
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter the valid Connection String", "ConnectionString Error");
                    return false;
                }
            }

            return false;
        }

        //Loading the values for the Server name Combobox
        private void LoadServerComboBox()
        {
            RegistryKey HKLM = Registry.LocalMachine;
            try
            {
                HKLM = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Syncfusion\\Essential Suite\\InstalledVersions\\OLAP\\ACList\\ServerName", RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);
                if (HKLM == null)
                {
                    HKLM = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Syncfusion\\Essential Suite\\InstalledVersions\\OLAP\\ACList\\ServerName", RegistryKeyPermissionCheck.ReadWriteSubTree);
                    HKLM.SetValue("1", "localhost");
                }
                else
                {
                    string[] serverNameArray = HKLM.GetValueNames();
                    int count = HKLM.ValueCount;
                    foreach (string sKey in serverNameArray)
                    {
                        serverNameStack.Push(HKLM.GetValue(sKey).ToString());
                    }
                    for (int j = 0; j < count; j++)
                    {
                        cmbServerName.Items.Add(serverNameStack.ElementAt(j));
                    }
                }
                ArrayList list = ArrayList.Adapter(cmbServerName.Items);
                list.Sort();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        // Loading the values for Database Name Combobox
        private void LoadDBComboBox()
        {
            RegistryKey HKLM = Registry.LocalMachine;
            try
            {
                HKLM = HKLM.OpenSubKey("SOFTWARE\\Syncfusion\\Essential Suite\\InstalledVersions\\OLAP\\ACList\\DBName", true);
                if (HKLM == null)
                {
                    HKLM = HKLM.CreateSubKey("SOFTWARE\\Syncfusion\\Essential Suite\\InstalledVersions\\OLAP\\ACList\\DBName");
                    HKLM.SetValue("1", "Adventure Works DW");
                }
                else
                {
                    string[] databaseNameArray = HKLM.GetValueNames();
                    int count = HKLM.ValueCount;
                    foreach (string sKey in databaseNameArray)
                    {
                        databaseNameStack.Push(HKLM.GetValue(sKey).ToString());
                    }
                    for (int j = 0; j < count; j++)
                    {
                        cmbDatabaseName.Items.Add(databaseNameStack.ElementAt(j).ToString());
                    }
                }
                ArrayList list = ArrayList.Adapter(cmbDatabaseName.Items);
                list.Sort();
            }
            catch (Exception e)
            {
            }
        }

        //For writing the Server Details into the registry
        void writeServerRegistry(string sName)
        {
            RegistryKey HKLMS = Registry.LocalMachine;
            HKLMS = HKLMS.CreateSubKey("SOFTWARE\\Syncfusion\\Essential Suite\\InstalledVersions\\OLAP\\ACList\\ServerName");
            HKLMS.SetValue((HKLMS.ValueCount + 1).ToString(), sName);
            HKLMS.Close();
        }

        //For writing the Database details into the registry
        void writeDBRegistry(string dName)
        {
            RegistryKey HKLMR = Registry.LocalMachine;
            HKLMR = HKLMR.CreateSubKey("SOFTWARE\\Syncfusion\\Essential Suite\\InstalledVersions\\OLAP\\ACList\\DBName");
            HKLMR.SetValue((HKLMR.ValueCount + 1).ToString(), dName);
            HKLMR.Close();
        }

        // For avoiding replicated data entering into the Server Registry Key
        private bool CheckServer(string sName)
        {
            return !serverNameStack.Contains(sName);
        }

        // For avoiding replicated data entering into the Database Registry Key
        private bool CheckDB(string dName)
        {
            return !databaseNameStack.Contains(dName);
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the btnOkay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isValid = false;
                try
                {
                    isValid = this.Validate();
                    if (isValid == true)
                    {
                        this.DialogResult = isValid;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                    isValid = false;
                    this.cmbServerName.Text = string.Empty;
                    this.cmbDatabaseName.Text = string.Empty;
                }
                if (chkCutomServer.IsChecked == true && isValid== true)
                {
                    if (!String.IsNullOrEmpty(this.cmbServerName.Text) && !string.IsNullOrEmpty(this.cmbDatabaseName.Text))
                    {
                        if (CheckServer(this.cmbServerName.Text.ToString()))
                            writeServerRegistry(this.cmbServerName.Text.ToString());
                        if (CheckDB(this.cmbDatabaseName.Text.ToString()))
                            writeDBRegistry(this.cmbDatabaseName.Text.ToString());
                        serverName = this.cmbServerName.Text.ToString();
                        dBName = this.cmbDatabaseName.Text.ToString();
                        dataSource = "CustomServer";
                    }
                }
                else
                {
                    dataSource = "OfflineCube";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnBrowser_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.AddExtension = true;
            openFileDialog.DefaultExt = "cub";
            openFileDialog.Filter = "Cube (.cub)|*.cub";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                this.txtOfflineCubeFilePath.Text = fileName;
            }
        }

        /// <summary>
        /// Handles the Checked event of the chkCutomServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkCutomServer_Checked(object sender, RoutedEventArgs e)
        {
            this.stpCutomServer.IsEnabled = true;
            this.grdCustomOfflineCube.IsEnabled = false;
            this.chkCustomOfflineCube.Unchecked -= new RoutedEventHandler(chkCustomOfflineCube_Unchecked);
            this.chkCustomOfflineCube.IsChecked = false;
            this.chkCustomOfflineCube.Unchecked += new RoutedEventHandler(chkCustomOfflineCube_Unchecked);

            this.chkCustomConnectionString.Unchecked -= new RoutedEventHandler(chkCustomConnectionString_Unchecked);
            this.chkCustomConnectionString.IsChecked = false;
            this.chkCustomConnectionString.Unchecked += new RoutedEventHandler(chkCustomConnectionString_Unchecked);

            chkCutomServer.IsChecked = true;
            chkCustomOfflineCube.IsChecked = false;
            cmbServerName.IsEnabled = true;
            cmbDatabaseName.IsEnabled = true;
            txtUsername.IsEnabled = false;
            txtPassword.IsEnabled = false;
            this.txtconnectionString.IsEnabled = false;
            cmbServerName.Text = serverName;
            cmbDatabaseName.Text = dBName;
            dataSource = "CustomServer";
        }

        /// <summary>
        /// Handles the Unchecked event of the chkCutomServer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkCutomServer_Unchecked(object sender, RoutedEventArgs e)
        {
            this.chkCustomOfflineCube.Unchecked -= new RoutedEventHandler(chkCutomServer_Unchecked);
            this.chkCutomServer.IsChecked = true;
            this.chkCustomOfflineCube.Unchecked -= new RoutedEventHandler(chkCustomOfflineCube_Unchecked);
            this.chkCustomOfflineCube.Unchecked += new RoutedEventHandler(chkCustomOfflineCube_Unchecked);

            this.chkCustomConnectionString.Unchecked -= new RoutedEventHandler(chkCustomConnectionString_Unchecked);
            this.chkCustomConnectionString.IsChecked = true;
            this.chkCustomConnectionString.Unchecked -= new RoutedEventHandler(chkCustomConnectionString_Unchecked);
            this.chkCustomConnectionString.Unchecked += new RoutedEventHandler(chkCustomConnectionString_Unchecked);
        }

        /// <summary>
        /// Handles the Checked event of the chkCustomOfflineCube control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkCustomOfflineCube_Checked(object sender, RoutedEventArgs e)
        {
            this.grdCustomOfflineCube.IsEnabled = true;
            this.stpCutomServer.IsEnabled = false;
            this.chkCutomServer.Unchecked -= new RoutedEventHandler(chkCutomServer_Unchecked);
            this.chkCutomServer.IsChecked = false;
            this.chkCutomServer.Unchecked += new RoutedEventHandler(chkCutomServer_Unchecked);
            this.chkCustomConnectionString.Unchecked -= new RoutedEventHandler(chkCustomConnectionString_Unchecked);
            this.chkCustomConnectionString.IsChecked = false;
            this.chkCustomConnectionString.Unchecked += new RoutedEventHandler(chkCustomConnectionString_Unchecked);
            chkCustomOfflineCube.IsChecked = true;
            chkCutomServer.IsChecked = false;
            cmbServerName.IsEnabled = false;
            cmbDatabaseName.IsEnabled = false;
            this.txtconnectionString.IsEnabled = false;
            dataSource = "OfflineCube";
        }

        /// <summary>
        /// Handles the Unchecked event of the chkCustomOfflineCube control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkCustomOfflineCube_Unchecked(object sender, RoutedEventArgs e)
        {
            this.chkCutomServer.Unchecked -= new RoutedEventHandler(chkCustomOfflineCube_Unchecked);
            this.chkCustomOfflineCube.IsChecked = true;
            this.chkCutomServer.Unchecked -= new RoutedEventHandler(chkCutomServer_Unchecked);
            this.chkCutomServer.Unchecked += new RoutedEventHandler(chkCutomServer_Unchecked);

            this.chkCustomConnectionString.Unchecked -= new RoutedEventHandler(chkCustomConnectionString_Unchecked);
            this.chkCustomOfflineCube.IsChecked = true;
            this.chkCustomConnectionString.Unchecked -= new RoutedEventHandler(chkCustomConnectionString_Unchecked);
            this.chkCustomConnectionString.Unchecked += new RoutedEventHandler(chkCustomConnectionString_Unchecked);
        }

        /// <summary>
        /// Handles the Checked event of the chkCustomConnectionString control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkCustomConnectionString_Checked(object sender, RoutedEventArgs e)
        {
            this.grdconnectionString.IsEnabled = true;
            this.stpCutomServer.IsEnabled = false;
            this.chkCutomServer.Unchecked -= new RoutedEventHandler(chkCutomServer_Unchecked);
            this.chkCutomServer.IsChecked = false;
            this.chkCutomServer.Unchecked += new RoutedEventHandler(chkCutomServer_Unchecked);
            this.chkCustomOfflineCube.Unchecked -= new RoutedEventHandler(chkCustomOfflineCube_Unchecked);
            this.chkCustomOfflineCube.IsChecked = false;
            this.chkCustomOfflineCube.Unchecked += new RoutedEventHandler(chkCustomOfflineCube_Unchecked);
            chkCustomConnectionString.IsChecked = true;
            txtconnectionString.IsEnabled = true;
            chkCutomServer.IsChecked = false;
            cmbServerName.IsEnabled = false;
            cmbDatabaseName.IsEnabled = false;
        }

        /// <summary>
        /// Handles the Unchecked event of the chkCustomConnectionString control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void chkCustomConnectionString_Unchecked(object sender, RoutedEventArgs e)
        {
            this.chkCutomServer.Unchecked -= new RoutedEventHandler(chkCustomOfflineCube_Unchecked);
            this.chkCustomConnectionString.IsChecked = true;
            this.chkCutomServer.Unchecked -= new RoutedEventHandler(chkCutomServer_Unchecked);
            this.chkCutomServer.Unchecked += new RoutedEventHandler(chkCutomServer_Unchecked);

            this.chkCustomOfflineCube.Unchecked -= new RoutedEventHandler(chkCustomOfflineCube_Unchecked);
            this.chkCustomConnectionString.IsChecked = true;
            this.chkCustomOfflineCube.Unchecked -= new RoutedEventHandler(chkCustomOfflineCube_Unchecked);
            this.chkCustomOfflineCube.Unchecked += new RoutedEventHandler(chkCustomOfflineCube_Unchecked);
        }

        /// <summary>
        /// Handles the Expanded event of the Expander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            this.Height = 505;
            this.txtUsername.IsEnabled = true;
            this.txtPassword.IsEnabled = true;
        }

        /// <summary>
        /// Handles the Collapsed event of the CredentialExpander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CredentialExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Height = 420;
            this.txtUsername.IsEnabled = false;
            this.txtPassword.IsEnabled = false;
        }
        #endregion
    }
}
