#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Syncfusion.Olap.Data;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.MDXQueryBuilder;
using Syncfusion.Olap.Reports;

namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// Interaction logic for ConnectOptions.xaml
    /// </summary>
    public partial class DesignerWizard : Window
    {
        #region Variables
        static string dBName;
        static string serverName;
        bool isdefaultLoaded = false;
        bool isCubeLoaded = false;
        public OlapDataManager model;
        string dataSource = "OfflineCube";
        String OlapProduct = "";
        bool isValidConnection = false;
        #endregion

        #region Initialization

        public DesignerSettings designerSettings
        {
            get;
            set;
        }

        public DesignerWizard(String ControlName)
        {
            InitializeComponent();
            try
            {
                this.ConnectionString = string.Empty;
                //enableCustomizeReports.Visibility = Visibility.Visible;
                chkCutomServer.IsChecked = true;
                chkCustomOfflineCube.IsChecked = false;
                cmbServerName.IsEnabled = true;
                cmbDatabaseName.IsEnabled = true;
                cmbServerName.Text = serverName;
                cmbDatabaseName.Text = dBName;

                if (ControlName == "OlapClient")
                {
                    this.OlapProduct = ControlName;
                    //enableCustomizeReports.IsEnabled = true;
                    this.wizPage1.NextEnabled = false;
                    this.wizPage1.NextVisible = false;
                }
                //else
                //{
                //    this.OlapProduct = "OlapGrid";
                //    //enableCustomizeReports.IsEnabled = true;
                //    this.wizPage1.NextEnabled = false;
                //    this.wizPage1.NextVisible = false;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion

        #region Properties

        public string ConnectionString { get; set; }

        #endregion

        #region Helper Methods

        bool Validate()
        {
            if (this.chkCustomOfflineCube.IsChecked == true)
            {
                if (File.Exists(this.txtOfflineCubeFilePath.Text))
                {
                    this.ConnectionString = GetLocalCubeConnectionString(this.txtOfflineCubeFilePath.Text);
                    designerSettings = new DesignerSettings();
                    designerSettings.ConnectionString = this.ConnectionString;
                    this.designerSettings = designerSettings;
                    return true;
                }
                else
                {
                    MessageBox.Show("Specified file not found", "File not found");
                    this.wizPage2.NextEnabled = false;
                    this.wizPage2.NextEnabled = false;
                  
                    return false;
                }
            }
            else if (this.chkCutomServer.IsChecked == true)
            {
                if (this.chkCutomServer.IsChecked == true)
                {
                    if (this.cmbServerName.Text != null)
                    {
                        if (this.cmbServerName.Text.Trim() != string.Empty)
                        {
                            if (this.cmbDatabaseName.Text != null)
                            {
                                if (this.cmbDatabaseName.Text.Trim() != string.Empty)
                                {
                                    if (this.txtUsername.Text != string.Empty && this.txtPassword.Password != string.Empty)
                                    {
                                        this.ConnectionString = GetServerConnectionString(cmbServerName.Text, cmbDatabaseName.Text, this.txtUsername.Text, this.txtPassword.Password);
                                        this.wizPage1.NextEnabled = true;
                                    }
                                    else
                                    {
                                        this.ConnectionString = GetServerConnectionString(cmbServerName.Text, cmbDatabaseName.Text);
                                        this.wizPage1.NextEnabled = true;
                                    }

                                    designerSettings = new DesignerSettings();
                                    designerSettings.ConnectionString = this.ConnectionString;
                                    this.designerSettings = designerSettings;
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please enter the Database name", "Error");
                            this.wizPage2.NextEnabled = false;
                            this.wizPage2.NextEnabled = false;
                          
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter the Server name", "Error");
                        this.wizPage2.NextEnabled = false;
                        this.wizPage2.NextEnabled = false;
                       

                        return false;
                    }
                    dataSource = "CustomServer";
                }
            }
            else if (this.chkCustomConnectionString.IsChecked == true)
            {
                if (this.txtconnectionString.Text != string.Empty)
                {
                    this.wizPage1.NextEnabled = true;
                    this.ConnectionString = this.txtconnectionString.Text;
                    this.designerSettings = new DesignerSettings();
                    designerSettings.ConnectionString = this.ConnectionString;
                    this.designerSettings = designerSettings;
                    return true;
                }
                else
                {
                    MessageBox.Show("Please enter a valid Connection String", "Error");
                    this.wizPage2.NextEnabled = false;
                    return false;
                }
            }
            return false;
        }


        #endregion

        #region Events

        private void wizPage2_Loaded(object sender, RoutedEventArgs e)
        {
            if (isValidConnection)
            {
                this.wizPage2.FinishEnabled = true;
                this.wizPage2.FinishVisible = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

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
            isCubeLoaded = false;
        }

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
            isCubeLoaded = false;
        }

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
            isCubeLoaded = false;
            dataSource = "OfflineCube";
        }

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
            isCubeLoaded = false;
        }

        #endregion

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
            isCubeLoaded = false;
        }

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
            isCubeLoaded = false;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            this.Height = 480;
            this.txtUsername.IsEnabled = true;
            this.txtPassword.IsEnabled = true;
        }

        private void CredentialExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Height = 410;
            this.txtUsername.IsEnabled = false;
            this.txtPassword.IsEnabled = false;
        }

        private void BindModelToControls()
        {
            try
            {
                if (this.ConnectionString != null && this.ConnectionString != string.Empty)
                {
                    try
                    {                        
                        if (this.model != null)
                        {
                            this.model.DataProvider.CloseConnection();
                        }
                        this.model = new Syncfusion.Olap.Manager.OlapDataManager(this.ConnectionString);
                        this.cubeDimensionBrowser.OlapDataManager = this.model;
                        this.cubeSelector.OlapDataManager = this.model;
                        this.axisBuilderColumn.OlapDataManager = this.model;
                        this.axisBuilderRow.OlapDataManager = this.model;
                        this.axisBuilderSlicer.OlapDataManager = this.model;
                        isCubeLoaded = true;
                        //this.LoadDefaultData();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in binding the model");
            }
        }

        private void cubeSelector_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
        public static string GetServerConnectionString(string serverName, string databaseName)
        {
            if (serverName != "" && databaseName != "")
            {
                return string.Format("Data Source={0}; Initial Catalog={1};", serverName, databaseName);
            }
            return null;
        }

        public static string GetServerConnectionString(string serverName, string databaseName, string userName, string PassWord)
        {
            if (serverName != string.Empty && databaseName != string.Empty && userName != string.Empty && PassWord != string.Empty)
            {
                return string.Format("Data Source={0}; Initial Catalog={1};User Id={2};Password={3};", serverName, databaseName, userName, PassWord);
            }
            return null;
        }

        public static string GetLocalCubeConnectionString(string cubeLocation)
        {
            if (cubeLocation != "")
            {
                return string.Format(@"Datasource='{0}'; Provider=msolap;", cubeLocation);
            }
            return null;
        }

        private void UpdateDesignerSettings(Syncfusion.Windows.Shared.Olap.DesignerSettings designerSettings)
        {
            if (this.ConnectionString != "" && model.CurrentCubeName != "")
            {
                designerSettings.ConnectionString = model.ConnectionString;
                designerSettings.CurrentCubeName = model.CurrentCubeName;
                AddItems(designerSettings.CategoricalElements, model.CurrentReport.CategoricalElements);
                AddItems(designerSettings.SeriesElements, model.CurrentReport.SeriesElements);
                AddItems(designerSettings.SlicerElements, model.CurrentReport.SlicerElements);
            }
        }
        private void AddItems(Items designerSettings, Items items)
        {
            foreach (Item item in items)
            {
                ///Dimension element

                if (item.ElementValue is DimensionElement)
                {
                    DimensionElement dimensionElement = new DimensionElement { Name = item.ElementValue.Name };
                    designerSettings.Add(new Item { ElementValue = dimensionElement });
                }
                else if (item.ElementValue is MeasureElements)
                {
                    MeasureElements measureElement = (MeasureElements)item.ElementValue;
                    designerSettings.Add(new Item { ElementValue = measureElement });
                }
                else if (item.ElementValue is NamedSetElement)
                {
                    NamedSetElement namedsetElement = new NamedSetElement { Name = item.ElementValue.Name };
                    namedsetElement.Name = item.ElementValue.Name;
                    designerSettings.Add(new Item { ElementValue = namedsetElement });
                }
                else if (item.ElementValue is KpiElements)
                {
                    KpiElements kpiElement = new KpiElements { Name = item.ElementValue.ToString() };
                    kpiElement.Elements.Add(new KpiElement { Name = item.ElementValue.Name });
                    designerSettings.Add(new Item { ElementValue = kpiElement });

                }

            }
        }

        //private void LoadDefaultData()
        //{
        //    ///// Loading the Default data
        //    CubeSchema schema = this.model.DataProvider.GetCubeSchema(this.model.CurrentCubeName);
        //    Dimension timeDimension = schema.GetTimeDimension();
        //    if (timeDimension != null)
        //    {
        //        Hierarchy defaultHierarchy = schema.GetHierarchyByUniqueName(timeDimension.DefaultHierarchyName);
        //        Level defaultLevel = schema.GetLevelByUniqueName(defaultHierarchy.DefaultLevelUniqueName);
        //        /// Creating Elements
        //        DimensionElement dimensionElement = new DimensionElement();
        //        dimensionElement.Name = timeDimension.Name;
        //        dimensionElement.AddLevel(defaultHierarchy.Name, defaultLevel.Name);

        //        this.model.CurrentReport.SeriesElements.Add(new Item { ElementValue = dimensionElement, Axis = AxisPosition.Series });
        //    }
        //    Member defaultMeasure = schema.GetDefaultMeasure();
        //    Item seriesItem = this.GetDefaultMeasureElements(schema, defaultMeasure);
        //    this.model.CurrentReport.CategoricalElements.Add(seriesItem);
        //    this.model.NotifyDrillDown();
        //    this.model.NotifyElementModified();
        //}

        private Item GetDefaultMeasureElements(CubeSchema schema, Member memberMeasureObj)
        {
            try
            {
                MeasureElements measureElements = new MeasureElements();
                measureElements.Add(memberMeasureObj.Caption);
                return new Item { ElementValue = measureElements };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Report Builder");
            }
            return null;
        }
        
        private void ConnectOptionWizard_Finish(object sender, RoutedEventArgs e)
        {
            if (this.ConnectionString != "" && this.designerSettings != null)
            {
                this.DialogResult = true;
                UpdateDesignerSettings(designerSettings);
                this.Close();
            }
            else
            {
                this.Close();
            }
        }
        void cubeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cubeSelector.SelectedItem != null)
            {
                this.model.Reports.Clear();

                axisBuilderColumn.MetaTreeNodes.Clear();
                axisBuilderRow.MetaTreeNodes.Clear();
                axisBuilderSlicer.MetaTreeNodes.Clear();

                model.NotifyDrillDown();
                model.NotifyElementModified();

                if (!isdefaultLoaded)
                {

                    //this.LoadDefaultData();
                }
                isdefaultLoaded = false;
            }
        }
        private void ConnectOptionWizard_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void wizPage1_Loaded(object sender, RoutedEventArgs e)
        {
            if (OlapProduct != "" && OlapProduct == "OlapClient")
            {
                //enableCustomizeReports.IsEnabled = true;
                this.wizPage1.NextEnabled = false;
                this.wizPage1.NextVisible = false;
                this.wizPage1.FinishVisible = true;
                this.wizPage1.FinishEnabled = true;
            }
            // this.wizPage1.NextEnabled = true;
        }
        /// <summary>
        /// To Customize the reports
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCustomizeReports(object sender, RoutedEventArgs e)
        {
            if (isValidConnection)
            {
                this.wizPage1.NextEnabled = true;
                this.wizPage1.NextVisible = true;
                MessageBox.Show("Valid Connection");
            }
            else
            {
                MessageBox.Show("Invalid Connection");
            }
        }
        /// <summary>
        /// To validate the Connection
        /// </summary>
        private bool ValidateConnection()
        {           
            try
            {
                try
                {
                    isValidConnection = this.Validate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
                if (chkCutomServer.IsChecked == true)
                {
                    if ((this.cmbServerName.Text != null) & (this.cmbDatabaseName.Text != null))
                    {
                        if ((this.cmbServerName.Text != "") & (this.cmbDatabaseName.Text != ""))
                        {
                            this.wizPage2.NextEnabled = true;
                            this.wizPage2.FinishEnabled = true;

                            serverName = this.cmbServerName.Text.ToString();
                            dBName = this.cmbDatabaseName.Text.ToString();
                            dataSource = "CustomServer";
                        }
                    }
                }
                else
                {
                    dataSource = "OfflineCube";
                }
                BindModelToControls();
                this.wizPage1.NextVisible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }            
            return isValidConnection;
        }

        private void btnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            this.ValidateConnection(); 
            if (isValidConnection)
            {
                this.wizPage1.NextEnabled = true;
                this.wizPage1.NextVisible = true;
                MessageBox.Show("Test Connection successfull!","Status");
            }
            else
            {
                this.wizPage1.NextEnabled = false;
            }
        }
    }
}
