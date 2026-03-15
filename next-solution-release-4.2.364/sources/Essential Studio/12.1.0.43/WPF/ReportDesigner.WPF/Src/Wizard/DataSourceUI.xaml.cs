//-------------------------------------------------------------------------------------------------
// <copyright file="DataSourceUI.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
using Syncfusion.RDL.DOM;
using Syncfusion.Windows.Reports.Sql;
using System.ComponentModel;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.Windows.Reports.Designer.Wizard;
using Microsoft.SqlServer.ReportingServices2005;
using System.Web.Services.Protocols;
using Syncfusion.RDL.Data;
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
    internal partial class DataSourceUI
        : ChromelessWindow, IDisposable
    {
        #region Members
        private ConnectionProperties connectionProperties;
        private ConnectionPropertiesOracle connectionPropertiesOracle;
        private ConnectionPropertiesOLEDB connectionPropertiesOLEDB;
        private ConnectionPropertiesODBC connectionPropertiesODBC;
        #endregion       

        #region Public Properties

        public DesignPanel DesignPanel { get; set; }

        public string SharedDatasourcePath { get; set; }

        public string ReportServerPath { get; set; }

        public bool IsSharedDatasource { get; set; }

        public string SharedDatsourceName { get; set; }

        internal System.Net.ICredentials ReportServerCredential { get; private set; }

        public string LoginUsername { get; set; }

        public string LoginPassword { get; set; }

        public string DatasourcelistValue { get; set; }

        public RDL.DOM.DataSource DataSource { get; set; }

        public DataSources DataSources { get; set; }

        public RDL.DOM.ConnectionProperties ConnectionProperties { get; set; }

        public List<string> DataSourceProviders;

        #endregion

        #region Private Properties

        private bool isModifyDataset = false;  
        private Microsoft.SqlServer.ReportingServices2010.ReportingService2010 ServiceProxy { get; set; }

        #endregion

        #region Constructors

        public DataSourceUI(DataSources dataSources,DesignPanel designPanel)
        {
            InitializeComponent();
            this.DesignPanel = designPanel;
            this.UpdateProviders();            
            int availableCount = 1;
            this.DataSources = dataSources;

            string[] availableDataSourceNames = (from dsrc in this.DataSources
                                                 select dsrc.Name).ToArray<string>();

            foreach (string strName in availableDataSourceNames)
            {
                if (strName.Equals("DataSource" + (availableCount)))
                {
                    availableCount++;
                }
            }

            this.DataSource = new RDL.DOM.DataSource();
            this.DataSource.Name = "DataSource" + availableCount.ToString();
            this.DataSource.ConnectionProperties = new RDL.DOM.ConnectionProperties();
            this.DataSource.ConnectionProperties.DataProvider = "SQL";
            this.DataSource.ConnectionProperties.IntegratedSecurity = true;
            this.IntializeDataSourceUI();
            isModifyDataset = false;
        }

        public DataSourceUI(RDL.DOM.DataSource reportDataSource, DataSources dataSource,DesignPanel designPanel)
        {
            InitializeComponent();            
            this.DesignPanel = designPanel;
            this.UpdateProviders();
            this.DataSources = dataSource;
            this.DataSource = reportDataSource;
            this.IntializeDataSourceUI();
            isModifyDataset = true;
        }

        #endregion        

        #region Helper Methods

        void UpdateProviders()
        {
            this.DataSourceProviders = new List<string>();
            this.DataSourceProviders.Add("Microsoft SQL Server");
            this.DataSourceProviders.Add("SQL CE");
            this.DataSourceProviders.Add("Oracle");
            this.DataSourceProviders.Add("OLE DB");
            this.DataSourceProviders.Add("ODBC");
            this.DataSourceProviders.Add("XML");

            if (this.DesignPanel.RDLType == RDLType.RDL2010)
            {
                this.DataSourceProviders.Add("Microsoft SQL Azure");
            }

            this.DataSourceProviders.Sort();
            this.cmb_ConnectionType.ItemsSource = this.DataSourceProviders;
        }

        void IntializeDataSourceUI()
        {
            UpdateConnectionProperties();

            this.txt_DataSourceName.Text = this.DataSource.Name;
            this.cmb_ConnectionType.Text = "Microsoft SQL Server";

            if (this.ConnectionProperties.DataProvider.Equals(DataProviders.ORACLE.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                this.cmb_ConnectionType.Text = "Oracle";
            }
            else if (this.ConnectionProperties.DataProvider.Equals(DataProviders.SQLAzure.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                this.cmb_ConnectionType.Text = "Microsoft SQL Azure";
            }
            else if (this.ConnectionProperties.DataProvider.Equals(DataProviders.OLEDB.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                this.cmb_ConnectionType.Text = "OLE DB";
            }
            else if (this.ConnectionProperties.DataProvider.Equals(DataProviders.ODBC.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                this.cmb_ConnectionType.Text = "ODBC";
            }
            else if (this.ConnectionProperties.DataProvider.Equals(DataProviders.XML.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                this.cmb_ConnectionType.Text = "XML";
            }

            else if (this.ConnectionProperties.DataProvider.Equals(DataProviders.SQLServerCe.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                this.cmb_ConnectionType.Text = "SQL CE";
            }

            this.txt_ConnectionString.Text = this.ConnectionProperties.ConnectString;

            UpdateButtonStatus();
            WireEvents();
        }

        void UpdateConnectionProperties()
        {
            this.ConnectionProperties = new RDL.DOM.ConnectionProperties();
            this.ConnectionProperties.ConnectString = string.Empty;
            this.ConnectionProperties.IntegratedSecurity = true;
            this.ConnectionProperties.Prompt = null;
            this.ConnectionProperties.UserName = null;
            this.ConnectionProperties.PassWord = null;

            if (this.DataSource.ConnectionProperties != null)
            {
                this.ConnectionProperties.ConnectString = this.DataSource.ConnectionProperties.ConnectString;
                this.ConnectionProperties.DataProvider = this.DataSource.ConnectionProperties.DataProvider;
                this.ConnectionProperties.IntegratedSecurity = this.DataSource.ConnectionProperties.IntegratedSecurity;
                this.ConnectionProperties.Prompt = this.DataSource.ConnectionProperties.Prompt;
                this.ConnectionProperties.UserName = this.DataSource.ConnectionProperties.UserName;
                this.ConnectionProperties.PassWord = this.DataSource.ConnectionProperties.PassWord;
            }

            if (this.ConnectionProperties.DataProvider == null)
            {
                this.ConnectionProperties.DataProvider = "SQL";
            }
        }

        private void UpdateButtonStatus()
        {
            if (cmb_ConnectionType.SelectedItem.ToString() == "XML" || cmb_ConnectionType.SelectedItem.ToString() == "SQL CE")
            {
                this.btn_Build.IsEnabled = false;
            }
            else
            {
                this.btn_Build.IsEnabled = true;
            }
        }

        void WireEvents()
        {
            this.sharedconnection.Visibility = System.Windows.Visibility.Collapsed;
            this.btn_Build.Click += new RoutedEventHandler(btn_Build_Click);
            this.browse.Click+=new RoutedEventHandler(browse_Click);
            this.rbtn_embeddedconnection.Checked+=new RoutedEventHandler(rbtn_embeddedconnection_Checked);
            this.rbtn_sharedconnection.Checked+=new RoutedEventHandler(rbtn_sharedconnection_Checked);
            this.cmb_ConnectionType.SelectionChanged+=new SelectionChangedEventHandler(cmb_ConnectionType_SelectionChanged);
        }

        private void cmb_ConnectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateButtonStatus();
        }

        void rbtn_sharedconnection_Checked(object sender, RoutedEventArgs e)
        {
            this.embedded_connection.Visibility = System.Windows.Visibility.Collapsed;
            this.sharedconnection.Visibility = System.Windows.Visibility.Visible;
        }
        void rbtn_embeddedconnection_Checked(object sender, RoutedEventArgs e)
        {
            this.embedded_connection.Visibility = System.Windows.Visibility.Visible;
            this.sharedconnection.Visibility = System.Windows.Visibility.Collapsed;
        }
        void btn_Build_Click(object sender, RoutedEventArgs e)
        {
            this.BuildConnection();
        }
        void browse_Click(object sender, RoutedEventArgs e)
        {
            Wizard.ReportOpenDialog reportopen = new Wizard.ReportOpenDialog();
            SkinStorage.SetVisualStyle(reportopen, SkinStorage.GetVisualStyle(this.Owner));
            reportopen.FileType = DialogFileType.DataSource;
            reportopen.DialogMode = DialogMode.Server;
            reportopen.ShowDialog();
            if (reportopen.DialogResult == true)
            {
                datasourcelist.Items.Clear();
                int i = reportopen.FilePath.LastIndexOf("/");
                int j = reportopen.FilePath.Length;
                string delim = "*;|@/";
                string datasource = reportopen.FilePath.Substring(i, j - i).TrimStart(delim.ToCharArray());
                this.DatasourcelistValue = datasource + "\n" + reportopen.ReportServerURL.ToString() + reportopen.FilePath.ToString();
                datasourcelist.Items.Add(this.DatasourcelistValue);
                datasourcelist.SelectedIndex = 0;
                this.SharedDatsourceName = datasource.ToString();
                this.SharedDatasourcePath = reportopen.FilePath.ToString();
                this.ReportServerPath = reportopen.ReportServerURL.ToString();
                this.ReportServerCredential = reportopen.ReportServerCredential;
                this.LoginUsername = reportopen.LoginUser;
                this.LoginPassword = reportopen.LoginPassword;
            }
        }
        private string GetDataProvider()
        {
          switch (this.cmb_ConnectionType.Text)
            {
                case "Microsoft SQL Azure":
                    return DataProviders.SQLAzure;
                case "Oracle":
                    return DataProviders.ORACLE;
                case "OLE DB":
                    return DataProviders.OLEDB;
                case "ODBC":
                    return DataProviders.ODBC;
                case "XML":
                    return DataProviders.XML;
                case "SQL CE":
                    return DataProviders.SQLServerCe;
            }

            return DataProviders.SQLServer;
        }

        private void BuildConnection()
        {
            this.ConnectionProperties.ConnectString = this.txt_ConnectionString.Text;

            switch (this.cmb_ConnectionType.Text)
            {
                case "Microsoft SQL Server":
                case "Microsoft SQL Azure":
                    {
                        connectionProperties = new ConnectionProperties(this.ConnectionProperties);
                        connectionProperties.Owner = Window.GetWindow(this.btn_Build);
                        SkinStorage.SetVisualStyle(connectionProperties, SkinStorage.GetVisualStyle(connectionProperties.Owner));

                        if (connectionProperties.ShowDialog() == true)
                        {
                            this.txt_ConnectionString.Text = SqlUtil.TruncateConnectionSring(connectionProperties.ConnProperties.ConnectString);
                            this.ConnectionProperties = connectionProperties.ConnProperties;
                        }
                    }
                    break;
                case "Oracle":
                    {
                        connectionPropertiesOracle = new ConnectionPropertiesOracle(this.ConnectionProperties);
                        connectionPropertiesOracle.Owner = Window.GetWindow(this.btn_Build);
                        SkinStorage.SetVisualStyle(connectionPropertiesOracle, SkinStorage.GetVisualStyle(connectionPropertiesOracle.Owner));

                        if (connectionPropertiesOracle.ShowDialog() == true)
                        {
                            this.ConnectionProperties = connectionPropertiesOracle.ConnProperties;
                            this.txt_ConnectionString.Text = connectionPropertiesOracle.ConnProperties.ConnectString;
                        }
                    }
                    break;
                case "OLE DB":
                    {
                        connectionPropertiesOLEDB = new ConnectionPropertiesOLEDB(this.ConnectionProperties);
                        connectionPropertiesOLEDB.Owner = Window.GetWindow(this.btn_Build);
                        SkinStorage.SetVisualStyle(connectionPropertiesOLEDB, SkinStorage.GetVisualStyle(connectionPropertiesOLEDB.Owner));

                        if (connectionPropertiesOLEDB.ShowDialog() == true)
                        {
                            this.ConnectionProperties = connectionPropertiesOLEDB.ConnProperties;
                            this.txt_ConnectionString.Text = connectionPropertiesOLEDB.ConnProperties.ConnectString;
                        }
                    }
                    break;
                case "ODBC":
                    {
                        connectionPropertiesODBC = new ConnectionPropertiesODBC(this.ConnectionProperties);
                        connectionPropertiesODBC.Owner = Window.GetWindow(this.btn_Build);
                        SkinStorage.SetVisualStyle(connectionPropertiesODBC, SkinStorage.GetVisualStyle(connectionPropertiesODBC.Owner));
                        if (connectionPropertiesODBC.ShowDialog() == true)
                        {
                            this.ConnectionProperties = connectionPropertiesODBC.ConnProperties;
                            this.txt_ConnectionString.Text = connectionPropertiesODBC.ConnProperties.ConnectString;
                        }
                    }
                    break;
            }
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            if (rbtn_embeddedconnection.IsChecked == true)
            {
                if (this.txt_ConnectionString.Text != null && this.txt_ConnectionString.Text.ToString() != string.Empty || this.cmb_ConnectionType.Text == "XML")
                {
                    if (Common.Util.CheckNameWithRE(this.txt_DataSourceName.Text.ToString()))
                    {
                        string[] availableDataSourceNames = null;

                        if (this.DataSources != null && this.DataSources.Count > 0)
                        {
                            //availableDataSourceNames = (from dsrc in this.DataSources
                            //                            where dsrc.Name != this.txt_DataSourceName.Text.ToString()
                            //                            select dsrc.Name).ToArray<string>();
                            availableDataSourceNames = (from dsrc in this.DataSources
                                                        select dsrc.Name).ToArray<string>();
                        }

                        if (Common.Util.CheckNameWithPreviousCollection(this.txt_DataSourceName.Text.ToString(), availableDataSourceNames) || isModifyDataset)
                        {
                            string datasourceName = this.DataSource.Name;
                            this.DataSource.Name = this.txt_DataSourceName.Text;
                            this.DataSource.ConnectionProperties = this.ConnectionProperties;

                            if (this.DataSource.ConnectionProperties.UserName != null)
                            {
                                this.DataSource.ConnectionProperties.Prompt = "Enter UserName and Password to loginto Database";
                            }

                            this.ConnectionProperties.DataProvider = this.GetDataProvider();
                            this.DataSource.ConnectionProperties.ConnectString = this.txt_ConnectionString.Text;
                            this.DialogResult = true;
                            this.Close();
                            this.UpdateDependentDataSets(datasourceName, this.txt_DataSourceName.Text);
                        }
                        else
                        {
                            MessageBox.Show("<Name:>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxDataSourceExist"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("<Name:>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSpecifyValidName"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxBuildDataSource"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"));
                }
            }
            else if(rbtn_sharedconnection.IsChecked==true)
            {
                try
                {
                    if (datasourcelist.SelectedItem.ToString() ==DatasourcelistValue)
                    {
                        List<Microsoft.SqlServer.ReportingServices2010.DataSource> datasourceList = new List<Microsoft.SqlServer.ReportingServices2010.DataSource>();
                        ServiceProxy = new Microsoft.SqlServer.ReportingServices2010.ReportingService2010();
                        ServiceProxy.Url = this.ReportServerPath + "/reportservice2010.asmx";
                        ServiceProxy.Credentials = this.ReportServerCredential;
                        this.DataSource.DataSourceReference = this.SharedDatasourcePath;
                        Microsoft.SqlServer.ReportingServices2010.DataSourceDefinition dataSourceDefinition = null;
                        dataSourceDefinition = ServiceProxy.GetDataSourceContents(SharedDatasourcePath);
                        this.ConnectionProperties.DataProvider = dataSourceDefinition.Extension.ToString();
                        this.DataSource.ConnectionProperties.ConnectString = dataSourceDefinition.ConnectString.ToString();

                        if (this.LoginUsername == null && this.LoginPassword == null)
                        {
                            this.DataSource.ConnectionProperties.IntegratedSecurity = true;
                        }
                        else
                        {
                            this.DataSource.ConnectionProperties.IntegratedSecurity = false;
                            this.DataSource.ConnectionProperties.UserName = this.LoginUsername;
                            this.DataSource.ConnectionProperties.PassWord = this.LoginPassword;
                        }
                        this.DialogResult = true;
                        this.Close();
                    }
                    else if (datasourcelist.SelectedItem.ToString() != DatasourcelistValue)
                    {
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch (SoapException ex)
                {
                    MessageBox.Show(ex.Message, "soap");
                }
                catch (System.Net.WebException ex)
                {
                    MessageBox.Show(ex.Message, "web");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        private void UpdateDependentDataSets(string datasourceName, string modifiedName)
        {
            if (!string.IsNullOrEmpty(datasourceName) && this.DesignPanel.DataSets != null && this.DesignPanel.DataSets.Count > 0)
            {
                var dependentDataSets = from dataset in this.DesignPanel.DataSets
                                        where dataset.Query != null && dataset.Query.DataSourceName.Equals(datasourceName)
                                        select dataset;
                if (dependentDataSets != null)
                {
                    foreach (var dataset in dependentDataSets)
                    {
                        dataset.Query.DataSourceName = modifiedName;
                    }
                }
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposing the DataSource UI Dialog
        /// </summary>
        public void Dispose()
        {
            if (this.connectionProperties != null)
            {
                this.connectionProperties.Dispose();
                this.connectionPropertiesOracle.Dispose();
            }
        }

        #endregion

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = SR.GetString(CultureInfo.CurrentUICulture, "titleDataSourceProperties");
            this.tabItemGeneral.Focus();
            if (!string.IsNullOrEmpty(this.DataSource.DataSourceReference))
            {
                this.rbtn_sharedconnection.IsChecked = true;
                this.datasourcelist.Items.Add(this.DataSource.DataSourceReference);
                this.datasourcelist.SelectedIndex = 0;
            }
        }

        private void ChromelessWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

    }
}
