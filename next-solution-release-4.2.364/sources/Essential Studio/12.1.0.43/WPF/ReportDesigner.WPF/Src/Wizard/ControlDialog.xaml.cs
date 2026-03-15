//-------------------------------------------------------------------------------------------------
// <copyright file="ControlDialog.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using Syncfusion.RDL.DOM;
using Syncfusion.Windows.Reports.Relational.Sql;
using Syncfusion.Windows.Reports.Sql;
using Syncfusion.Windows.Reports.Common;
using System.ComponentModel;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.RDL.Data;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
   
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///  Interaction logic for ControlDialog.xaml
    /// </summary>
    internal partial class ControlDialog : ChromelessWindow
    {
        #region Members

        private DataSourceUI dataSource;
        private DataSetClient dataSetClient;

        private DataSources dataSources;
        private DataSets dataSets;

        private DesignPanel designPanel;

        private List<string> m_valueList;
        private List<string> m_rowList;
        private bool textChanged = false;

        private List<string> m_columnList;
        DbConnection dbConnection;
        DataTable dataTableRelations;
        List<SchemaInfo> listOfSchema;
        public object FolderImage { get; set; }
        private string old_DataSetName;
        string error_title;
        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDialog"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="requireFieldFilter">if set to <c>true</c> [require field filter].</param>
        /// <param name="reportSettings">The report settings.</param>
        public ControlDialog(string title, RDL.DOM.DataSources dataSources,RDL.DOM.DataSets datasets,DesignPanel designPanel)
        {
            this.designPanel = designPanel;
            this.ControlDialogSetUp(dataSources, datasets);

            this.Title = title;
            this.CachedDataSources = new DataSources();
            this.CachedDataSets = new DataSets();
            this.error_title = SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner");

            if (title == RESX.titleNewChart)
            {
                this.lvw_ColumnGroups.AllowDrop = true;
                this.lvw_RowGroups.AllowDrop = true;
                this.lvw_MeasureGroups.AllowDrop = true;
                this.TopLabel.Text = "Series";
                this.LeftLabel.Text = "Categories";
                this.txt_wizardInfo.Visibility = System.Windows.Visibility.Collapsed;
                this.Title = SR.GetString(CultureInfo.CurrentUICulture, "titleNewChart");
            }

            if (title == RESX.titleNewTable)
            {
                this.lvw_ColumnGroups.AllowDrop = true;
                this.lvw_RowGroups.AllowDrop = true;
                this.lvw_MeasureGroups.AllowDrop = true;
                this.txt_wizardInfo.Visibility = System.Windows.Visibility.Visible;
                this.Title = SR.GetString(CultureInfo.CurrentUICulture, "titleNewTable");
            }
            
            this.wiz_QueryDesigner.NextEnabled = false;

            lvw_ColGroupMenuItem.Click += new RoutedEventHandler(lvw_ColGroupMenuItem_Click);
            lvw_RowGroupMenuItem.Click += new RoutedEventHandler(lvw_RowGroupsMenuItem_Click);
            this.lvw_MeasureGroupsMenuItem.Click += new RoutedEventHandler(lvw_MeasureGroupsMenuItem_Click);
        }

        #endregion

        #region Public Variables

        public RDL.DOM.DataSource reportDataSource;
        public RDL.DOM.DataSet reportDataSet;

        public string ConnectionString;

        public string QueryString;
        public string QueryEditString;

        public List<SchemaInfo> selectedTables;
        public ArrayList arrayList;
        //public string[] DataSets;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the row list.
        /// </summary>
        /// <value>The row list.</value>
        public List<string> RowList
        {
            get { return m_rowList; }
            set { m_rowList = value; }
        }


        /// <summary>
        /// Gets or sets the column list.
        /// </summary>
        /// <value>The column list.</value>
        public List<string> ColumnList
        {
            get { return m_columnList; }
            set { m_columnList = value; }
        }

        /// <summary>
        /// Gets or sets the values list.
        /// </summary>
        /// <value>The values list.</value>
        public List<string> ValuesList
        {
            get { return m_valueList; }
            set { m_valueList = value; }
        }

        public DataSources CachedDataSources
        { 
            get;
            set; 
        }

        public DataSets CachedDataSets
        {
            get;
            set;
        }

        #endregion

        #region Static Variables
        /// <summary>
        /// Represents the Drag Type
        /// </summary>
        static public System.Type DragType;
        #endregion

        #region Event Declaration
        /// <summary>
        /// Occurs when [data source changed].
        /// </summary>
        public event DataSourceChangedEventHandler DataSourceChanged;
        #endregion

        #region Helper Methods

        private void ControlDialogSetUp(RDL.DOM.DataSources dataSources,RDL.DOM.DataSets datasets)
        {
            InitializeComponent();

            this.ConnectionString = string.Empty;
            this.QueryString = string.Empty;
            this.QueryEditString = string.Empty;
            this.btn_New.Click += new RoutedEventHandler(btn_New_Click);
            this.lvw_DataSources.SelectionChanged += new SelectionChangedEventHandler(lvw_DataSources_SelectionChanged);
            this.lvw_DataSets.SelectionChanged += new SelectionChangedEventHandler(lvw_DataSets_SelectionChanged);
            this.optionChooseDataSet.Checked += new RoutedEventHandler(optionChooseDataSet_Checked);
            this.optionCreateDataSet.Checked += new RoutedEventHandler(optionCreateDataSet_Checked);
            this.optionChooseDataSet.IsChecked = true;
            this.Wiz_ControlDataSetBinder.NextEnabled = true;
            this.UpdateHelpButtonVisibility();
            this.dataSources = dataSources;
            this.dataSets = datasets;

            if (this.dataSources != null && this.dataSources.Count > 0)
            {
                foreach (RDL.DOM.DataSource reportDataSource in this.dataSources)
                {
                    System.Windows.Controls.ListBoxItem lst_ViewItemDataSource = new System.Windows.Controls.ListBoxItem();
                    lst_ViewItemDataSource.Height = 40;
                    lst_ViewItemDataSource.FontSize = 20;
                    lst_ViewItemDataSource.Content = reportDataSource.Name;
                    this.lvw_DataSources.Items.Add(lst_ViewItemDataSource);
                    lst_ViewItemDataSource.Selected += new RoutedEventHandler(lst_ViewItemDataSource_Selected);
                }

                if (this.dataSets != null && this.dataSets.Count > 0)
                {
                    foreach (RDL.DOM.DataSet reportDataSet in this.dataSets)
                    {
                        System.Windows.Controls.ListBoxItem lst_ViewItemDataSet = new ListBoxItem();
                        lst_ViewItemDataSet.Height = 40;
                        lst_ViewItemDataSet.FontSize = 20;
                        lst_ViewItemDataSet.Content = reportDataSet.Name;
                        this.lvw_DataSets.Items.Add(lst_ViewItemDataSet);
                        lst_ViewItemDataSet.Selected += new RoutedEventHandler(lst_ViewItemDataSet_Selected);
                    }
                    this.lvw_DataSets.SelectedIndex = 0;
                }
                else
                {
                    this.optionCreateDataSet.IsChecked = true;
                    this.optionChooseDataSet.IsEnabled = false;
                    this.wiz_ControlDataSourceBinder.BackVisible = false;
                }

            }
            else
            {
                this.optionCreateDataSet.IsChecked = true;
                this.optionChooseDataSet.IsEnabled = false;
                this.wiz_ControlDataSourceBinder.BackVisible = false;
            }
        }

        private void UpdateHelpButtonVisibility()
        {
            if(!designPanel.ShowHelp)
            {
                this.wizardControl.HelpVisible = false;
                this.Wiz_ControlDataSetBinder.HelpVisible = false;
                this.wiz_ControlDataSourceBinder.HelpVisible = false;
                this.wiz_QueryDesigner.HelpVisible = false;
                this.wiz_ChartTypes.HelpVisible = false;
                this.wiz_ArrangeFields.HelpVisible = false;
                this.wiz_ArrangeChartFields.HelpVisible = false;
                this.wiz_ChooseLayout.HelpVisible = false;
                this.wiz_ChooseStyle.HelpVisible = false;
            }

            if (this.designPanel.VisualStyle == "Metro")
            {
                this.FolderImage = this.Resources["MetroFolderClose"];
            }
            else if (this.designPanel.VisualStyle == "Blend")
            {
                this.FolderImage = this.Resources["BlendFolderClose"];
            }
            else
            {
                this.FolderImage = this.Resources["FolderClose"];
            }
        }

        void optionCreateDataSet_Checked(object sender, RoutedEventArgs e)
        {
            this.Wiz_ControlDataSetBinder.NextPage = this.wiz_ControlDataSourceBinder;
            this.wiz_ControlDataSourceBinder.BackEnabled = true;
            this.wiz_ControlDataSourceBinder.BackVisible = true;
            this.wiz_ControlDataSourceBinder.IsEnabled = true;
            this.wiz_ControlDataSourceBinder.Visibility = System.Windows.Visibility.Visible;
            this.wiz_QueryDesigner.IsEnabled = true;
            this.wiz_QueryDesigner.Visibility = System.Windows.Visibility.Visible;
        }

        void optionChooseDataSet_Checked(object sender, RoutedEventArgs e)
        {
            this.Wiz_ControlDataSetBinder.NextPage = this.wiz_ArrangeFields;
            this.wiz_ControlDataSourceBinder.IsEnabled = false;
            this.wiz_ControlDataSourceBinder.Visibility = System.Windows.Visibility.Collapsed;
            this.wiz_QueryDesigner.IsEnabled = false;
            this.wiz_QueryDesigner.Visibility = System.Windows.Visibility.Collapsed;
        }

        void lvw_DataSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem listItemAsDataSet = (ListBoxItem)this.lvw_DataSets.SelectedItem;
            this.reportDataSet = this.FindDataSetByName(listItemAsDataSet.Content.ToString());

            if (string.IsNullOrEmpty(this.old_DataSetName))
            {
                this.old_DataSetName = this.reportDataSet.Name;
            }
        }

        private RDL.DOM.DataSet FindDataSetByName(string name)
        {
            if (this.dataSets != null && this.dataSets.Count > 0)
            {
                RDL.DOM.DataSet dSet = (from dset in this.dataSets
                                                   where dset.Name == name
                                                   select dset).SingleOrDefault();
                return dSet;
            }
            else return null;
        }

        void lst_ViewItemDataSet_Selected(object sender, RoutedEventArgs e)
        {
            ListBoxItem listItemAsDataSet = (ListBoxItem)this.lvw_DataSets.Items[this.lvw_DataSets.SelectedIndex];
            this.reportDataSet = this.FindDataSetByName(listItemAsDataSet.Content.ToString());
            this.optionChooseDataSet.IsChecked = true;
        }

        private void lst_ViewItemDataSource_Selected(object sender, RoutedEventArgs e)
        {
            if (this.btn_New.Content.ToString().Equals(RESX.btnEdit.ToString()))
            {
                this.btn_New.IsEnabled = false;
            }
        }

        private void lvw_ColumnGroups_Drop(object sender, System.Windows.DragEventArgs e)
        {
            object data = e.Data.GetData(DragType);
            ListBoxItem listItem = (ListBoxItem)data;
            ((System.Windows.Controls.ListBox)sender).Items.Add(new ListBoxItem().Content = listItem.Content);
        }

        private void lvw_DataSetFields_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.ListBox DragSource = (System.Windows.Controls.ListBox)sender;
            object data = (object)GetObjectDataFromPoint(DragSource, e.GetPosition(DragSource));
            if (data != null)
            {
                DragType = data.GetType();
                DragDrop.DoDragDrop(DragSource, data, System.Windows.DragDropEffects.Copy);
            }
        }

        private void lvw_RowGroups_Drop(object sender, System.Windows.DragEventArgs e)
        {
            object data = e.Data.GetData(DragType);
            ListBoxItem listItem = (ListBoxItem)data;
            ((System.Windows.Controls.ListBox)sender).Items.Add(new ListBoxItem().Content = listItem.Content);
        }

        private void lvw_MeasureGroups_Drop(object sender, System.Windows.DragEventArgs e)
        {
            object data = e.Data.GetData(DragType);
            //ListBoxItem listItem = (ListBoxItem)data;
            ((System.Windows.Controls.ListBox)sender).Items.Add(GetListBoxItem(data));
        }

        private object GetListBoxItem(object data)
        {            
            ListBoxItem lstBoxItem = new ListBoxItem();            
            try
            {
                ListBoxItem listItem = (ListBoxItem)data;         
                lstBoxItem.Content = listItem.Content;
                lstBoxItem.ContextMenu = this.AddContextMenu(lstBoxItem);
                lstBoxItem.Tag = listItem;
            }
            catch
            {
            }
            return lstBoxItem;
        }
        
        private ContextMenu AddContextMenu(ListBoxItem lable)
        {
            ContextMenu cmenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = RESX.headerSum.ToString();
            menuItem.Tag = lable;
            menuItem.Click += new RoutedEventHandler(menuItem_Click);
            cmenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = RESX.headerCount.ToString();
            menuItem.Tag = lable;
            menuItem.Click+=new RoutedEventHandler(menuItem_Click);
            cmenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = RESX.headerAvg.ToString();
            menuItem.Tag = lable;
            menuItem.Click+=new RoutedEventHandler(menuItem_Click);
            cmenu.Items.Add(menuItem);

            System.Windows.Controls.Separator sep = new System.Windows.Controls.Separator();
            cmenu.Items.Add(sep);

            menuItem = new MenuItem();
            menuItem.Header = RESX.headerRemove.ToString();
            menuItem.Tag = lable;
            menuItem.Click+=new RoutedEventHandler(menuItem_Click);
            cmenu.Items.Add(menuItem);

            return cmenu;
           
        }

        void menuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mItem = (MenuItem)sender;            
            ListBoxItem lstItem = mItem.Tag as ListBoxItem;
            if (mItem.Header.ToString().Equals(RESX.headerRemove.ToString()) && this.lvw_MeasureGroups.Items.Count > 0)
            {
                this.lvw_MeasureGroups.Items.Remove(lstItem);
            }
            //foreach (MenuItem mitem in lstItem.ContextMenu.Items)
            //{
            //    mitem.IsCheckable = false;
            //}  
            ListBoxItem lstItemInitial = lstItem.Tag as ListBoxItem;
            lstItem.Content = mItem.Header + "(" + lstItemInitial.Content + ")";
            //mItem.IsCheckable = true;
        }

        private void lvw_RowGroupsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.lvw_RowGroups.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectField"), this.error_title);
            }
            else
            {
                this.lvw_RowGroups.Items.Remove(this.lvw_RowGroups.SelectedItem);
            }
        }

        private void lvw_ColGroupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.lvw_ColumnGroups.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectField"), this.error_title);
            }
            else
            {
                this.lvw_ColumnGroups.Items.Remove(this.lvw_ColumnGroups.SelectedItem);
            }
        }

        private void lvw_MeasureGroupsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.lvw_MeasureGroups.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectField"), this.error_title);
            }
            else
            {
                this.lvw_MeasureGroups.Items.Remove(this.lvw_MeasureGroups.SelectedItem);
            }
        }

        private void lvw_DataSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.dataSources.Count > 0)
            {
                try
                {
                    ListBoxItem listViewItem = (ListBoxItem)lvw_DataSources.SelectedValue;
                    this.reportDataSource = FindDataSourceByName(listViewItem.Content.ToString());
                    this.wiz_ControlDataSourceBinder.NextEnabled = true;
                    this.ConnectionString = this.reportDataSource.ConnectionProperties.ConnectString;
                    BindTreeViewControl();
                }
                catch (Exception excep)
                {
                    MessageBox.Show(excep.Message, this.error_title);
                }
            }
        }

        private void btn_New_Click(object sender, RoutedEventArgs e)
        {
            if (this.btn_New.Content.ToString().Equals(RESX.btnNew.ToString()))
            {
                if (DesignMode.RDL == this.designPanel.reportDesignView.DesignMode)
                {
                    System.Windows.Controls.ListBoxItem lst_View_NowAdded = null;

                    dataSource = new DataSourceUI(this.dataSources, this.designPanel);
                    dataSource.Owner = Window.GetWindow(this.btn_New);
                    this.designPanel.UpdateOwnerWindow(dataSource);
                    if (dataSource.ShowDialog() == true)
                    {
                        try
                        {
                            this.ConnectionString = dataSource.ConnectionProperties.ConnectString;
                            reportDataSource = dataSource.DataSource;
                            BindTreeViewControl();
                            this.btn_New.Content = RESX.btnEdit.ToString();
                            lst_View_NowAdded = new System.Windows.Controls.ListBoxItem();
                            lst_View_NowAdded.Selected += new RoutedEventHandler(lst_View_NowAdded_Selected);
                            lst_View_NowAdded.Height = 40;
                            lst_View_NowAdded.FontSize = 20;
                            lst_View_NowAdded.Content = reportDataSource.Name;
                            this.wiz_ControlDataSourceBinder.NextEnabled = true;
                            this.lvw_DataSources.Items.Add(lst_View_NowAdded);
                            this.reportDataSource.Name = dataSource.txt_DataSourceName.Text;
                            this.CachedDataSources.Add(reportDataSource);
                            this.designPanel.AddDataSource(reportDataSource);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, this.error_title);
                        }
                    }
                }
                else
                {
                    dataSetClient = new DataSetClient(this.designPanel.DataSets, this.designPanel.DataSources, this.designPanel.reportDesignView.AssemblyInfos, this.designPanel.reportDesignView.ObjectInfos, this.designPanel.reportDesignView.Assemblies);
                    dataSetClient.Owner = Window.GetWindow(this.btn_New);
                    designPanel.UpdateOwnerWindow(dataSetClient);
                    System.Windows.Controls.ListBoxItem lst_View_NowAdded = null;

                    if (dataSetClient.ShowDialog() == true)
                    {
                        if (dataSetClient.CreatedDataSource != null)
                        {
                            reportDataSource = dataSetClient.CreatedDataSource;
                            reportDataSet = dataSetClient.DataSet;
                            this.btn_New.Content = RESX.btnEdit.ToString();
                            lst_View_NowAdded = new System.Windows.Controls.ListBoxItem();
                            lst_View_NowAdded.Selected += new RoutedEventHandler(lst_View_NowAdded_Selected);
                            lst_View_NowAdded.Height = 40;
                            lst_View_NowAdded.FontSize = 25;
                            lst_View_NowAdded.Content = reportDataSource.Name;

                            this.lvw_DataSources.Items.Add(lst_View_NowAdded);
                            this.CachedDataSources.Add(reportDataSource);
                            this.designPanel.AddDataSource(dataSetClient.CreatedDataSource);
                            this.designPanel.AddDataSet(dataSetClient.DataSet);
                            this.designPanel.RaiseDataSetCollectionModifiedEvent();

                            wiz_ControlDataSourceBinder.NextPage = Wiz_ControlDataSetBinder;
                            if (dataSetClient.DataSets != null)
                            {
                                System.Windows.Controls.ListBoxItem lst_ViewItemDataSet = new ListBoxItem();
                                lst_ViewItemDataSet.Height = 40;
                                lst_ViewItemDataSet.FontSize = 20;
                                lst_ViewItemDataSet.Content = dataSetClient.DataSets.First().Name;
                                this.lvw_DataSets.Items.Add(lst_ViewItemDataSet);
                                lst_ViewItemDataSet.Selected += new RoutedEventHandler(lst_ViewItemDataSet_Selected);
                                this.lvw_DataSets.SelectedIndex = 0;
                                this.optionCreateDataSet.IsChecked = false;
                                this.optionChooseDataSet.IsEnabled = true;
                                this.wiz_ControlDataSourceBinder.BackVisible = false;
                            }
                        }
                    }
                }
            }
            else if (this.btn_New.Content.ToString().Equals(RESX.btnEdit.ToString()))
            {
                if (DesignMode.RDL == this.designPanel.reportDesignView.DesignMode)
                {
                    if (dataSource != null)
                    {
                        dataSource = new DataSourceUI(reportDataSource, this.dataSources, this.designPanel);
                        dataSource.Owner = Window.GetWindow(this.btn_New);
                        this.designPanel.UpdateOwnerWindow(dataSource);
                        if (dataSource.ShowDialog() == true)
                        {
                            this.reportDataSource.Name = reportDataSource.Name;
                            this.ConnectionString = this.reportDataSource.ConnectionProperties.ConnectString;
                            try
                            {
                                BindTreeViewControl();
                                this.wiz_ControlDataSourceBinder.NextEnabled = true;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, this.error_title);
                            }
                        }
                    }
                }
                else
                {
                    var dataSets = (from dataSet in this.designPanel.DataSets
                                    where dataSet.Name.Equals(reportDataSet.Name)
                                    select dataSet).First();

                    dataSetClient = new DataSetClient(dataSets, this.designPanel.DataSets, this.designPanel.DataSources, this.designPanel.reportDesignView.AssemblyInfos, this.designPanel.reportDesignView.ObjectInfos, this.designPanel.reportDesignView.Assemblies);
                    this.designPanel.UpdateOwnerWindow(dataSetClient);
                    if (dataSetClient.ShowDialog() == true)
                    {
                        if (dataSetClient.CreatedDataSource != null)
                        {
                            this.reportDataSource.Name = reportDataSource.Name;
                        }
                        this.designPanel.RaiseDataSetCollectionModifiedEvent();
                    }
                }
            }
        }

        void lst_View_NowAdded_Selected(object sender, RoutedEventArgs e)
        {
            this.btn_New.IsEnabled = true;
        }

        /// <summary>
        /// Handles the Help event of the wizardControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void wizardControl_Help(object sender, RoutedEventArgs e)
        {
            if (this.Title == RESX.titleNewChart)
            {
                System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/ChartWizard");
            }
            else
            {
                System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/TablixWizard");
            }
        }

        private void wiz_QueryDesigner_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ConnectionString != string.Empty)
            {
                string query = "select case when xtype = 'V' then 'View' else 'Table' end, name from sys.sysobjects where type='v' or type='U' order by 1";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, this.ConnectionString);
                DataTable dataTable = new DataTable("Table");
                sqlDataAdapter.Fill(dataTable);
                this.trvw_Schemas.ItemsSource = (IEnumerable)dataTable;
            }
        }

        private void btn_RunQuery_Click(object sender, RoutedEventArgs e)
        {
            this.dgrd_QueryResults.IsHitTestVisible = true;
            this.dgrd_QueryResults.Model.TableStyle.Foreground = ((Syncfusion.Windows.Controls.Grid.IGridDataVisualStyle)(this.dgrd_QueryResults.Model.GridVisualStyle)).ValueForegroundBrush;
            if ((bool)this.tgbtn_EditText.IsChecked)
            {
                if (this.reportDataSource.ConnectionProperties.DataProvider == DataProviders.XML ||
                        this.reportDataSource.ConnectionProperties.DataProvider == DataProviders.ODBC ||
                        this.reportDataSource.ConnectionProperties.DataProvider == DataProviders.OLEDB)
                {
                    reportDataSet = new RDL.DOM.DataSet();
                    reportDataSet.Query = new Query();

                    if (this.dataSets.Count > 0)
                    {
                        reportDataSet.Name = "Dataset" + (this.dataSets.Count + 1).ToString();
                    }
                    else
                    {
                        reportDataSet.Name = "Dataset1";
                    }
                    reportDataSet.Query.DataSourceName = this.reportDataSource.Name;
                }

                DataTable dataTable = new DataTable();
                this.QueryString = this.rtbox_TextView.Text;
                reportDataSet.Query.CommandText = this.QueryString;

                if (this.QueryString != string.Empty || this.reportDataSource.ConnectionProperties.DataProvider == DataProviders.XML)
                {
                    switch (this.reportDataSource.ConnectionProperties.DataProvider)
                    {
                        case DataProviders.SQLServer:
                        case DataProviders.SQLAzure:
                            {
                                try
                                {
                                    UpdateSqlQueryResult();
                                }
                                catch (Exception ex)
                                {
                                    System.Windows.MessageBox.Show(ex.Message, this.error_title);
                                }
                            }
                            break;
                        case DataProviders.ORACLE:
                            {
                                try
                                {
                                    UpdateOracleQueryResult();
                                }
                                catch (Exception ex)
                                {
                                    System.Windows.MessageBox.Show(ex.Message, this.error_title);
                                }
                            }
                            break;
                             case DataProviders.ODBC:
                            {
                                UpdateOdbcQueryResult();
                            }
                            break;
                        case DataProviders.OLEDB:
                            {
                                UpdateOledbQueryResult();
                            }
                            break;
                        case DataProviders.XML:
                            {
                                UpdateXmlQueryResult();
                            }
                            break;
                    }

                    if (this.reportDataSource.ConnectionProperties.DataProvider == DataProviders.XML ||
                        this.reportDataSource.ConnectionProperties.DataProvider == DataProviders.ODBC ||
                        this.reportDataSource.ConnectionProperties.DataProvider == DataProviders.OLEDB)
                    {
                        wiz_QueryDesigner.NextVisible = true;
                        wiz_QueryDesigner.NextEnabled = true;
                        wiz_QueryDesigner.NextPage = wiz_ArrangeFields;
                        wiz_ArrangeFields.Visibility = System.Windows.Visibility.Visible;
                    }

                    this.wiz_QueryDesigner.NextEnabled = true;
                }
                else
                {
                    System.Windows.MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSpecifyQuery"), this.error_title);
                    this.wiz_QueryDesigner.NextEnabled = false;
                }
            }
            else
            {
                this.textChanged = false;
                if (this.ConnectionString != null)
                {
                    List<SchemaInfo> tempSchemaInfos = new List<SchemaInfo>();
                    tempSchemaInfos = this.GetSelectedNode(this.listOfSchema);

                    if (tempSchemaInfos.Count > 0)
                    {
                        switch (this.reportDataSource.ConnectionProperties.DataProvider)
                        {
                            case DataProviders.SQLServer:
                            case DataProviders.SQLAzure:
                                {
                                    PopulateSqlSchemaResult(tempSchemaInfos);
                                }
                                break;
                            case DataProviders.ORACLE:
                                {
                                    PopulateOracleSchemaResult(tempSchemaInfos);
                                }
                                break;
                        } 

                        this.wiz_QueryDesigner.NextEnabled = true;
                    }

                    else
                    {
                        System.Windows.MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectQueryToRun"), this.error_title);
                        this.wiz_QueryDesigner.NextEnabled = false;
                    }

                }
            }
        }

        private void PopulateOracleSchemaResult(List<SchemaInfo> tempSchemaInfos)
        {
            System.Data.OracleClient.OracleConnection dbConnection = new System.Data.OracleClient.OracleConnection(this.ConnectionString);
            dbConnection.Open();
            OracleSchemaProvider oracleSchemaProvider = new OracleSchemaProvider(dbConnection);
            DataTable dataTable = new DataTable();

            oracleSchemaProvider.GetQueryResults(tempSchemaInfos, this.dataTableRelations);
            this.QueryString = oracleSchemaProvider.QueryString;
            reportDataSet.Query.CommandText = this.QueryString;
            dataTable = oracleSchemaProvider.GetQueryTables(this.QueryString);
            dataTable.TableName = "QueryResults";
            dgrd_QueryResults.ItemsSource = dataTable;
        }

        private void PopulateSqlSchemaResult(List<SchemaInfo> tempSchemaInfos)
        {
            SqlConnection dbConnection = new SqlConnection(this.ConnectionString);
            dbConnection.Open();
            SqlSchemaProvider sqlSchemaProvider = new SqlSchemaProvider(dbConnection);
            DataTable dataTable = new DataTable();

            sqlSchemaProvider.GetQueryResults(tempSchemaInfos, this.dataTableRelations);
            this.QueryString = sqlSchemaProvider.QueryString;
            reportDataSet.Query.CommandText = this.QueryString;
            dataTable = sqlSchemaProvider.GetQueryTables(Util.ReturnSQLTop(this.QueryString));
            dataTable.TableName = "QueryResults";
            dgrd_QueryResults.ItemsSource = dataTable;
        }

        private void UpdateXmlQueryResult()
        {
            XMLDataProvider xml = new XMLDataProvider();
            DataTable dataTable = xml.GetTable(this.ConnectionString, this.QueryString, "Table");
            dataTable.TableName = "QueryResults";
            dgrd_QueryResults.ItemsSource = dataTable;
        }

        private void UpdateOledbQueryResult()
        {
            System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(this.ConnectionString);
            connection.Open();
            System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(this.QueryString, connection);
            System.Data.OleDb.OleDbDataReader dr = cmd.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(dr);
            dataTable.TableName = "QueryResults";
            dgrd_QueryResults.ItemsSource = dataTable;
        }

        private void UpdateOdbcQueryResult()
        {
            System.Data.Odbc.OdbcConnection connection = new System.Data.Odbc.OdbcConnection(this.ConnectionString);
            connection.Open();
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(this.QueryString, connection);
            System.Data.Odbc.OdbcDataReader dr = cmd.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(dr);
            dataTable.TableName = "QueryResults";
            dgrd_QueryResults.ItemsSource = dataTable;
        }

        private void UpdateOracleQueryResult()
        {
            OracleSchemaProvider sqlSchemaProvider = new OracleSchemaProvider(dbConnection);
            DataTable dataTable = sqlSchemaProvider.GetQueryTables(this.QueryString);
            dataTable.TableName = "QueryResults";
            dgrd_QueryResults.ItemsSource = dataTable;
        }

        private void UpdateSqlQueryResult()
        {
            SqlSchemaProvider sqlSchemaProvider = new SqlSchemaProvider(dbConnection);
            DataTable dataTable = sqlSchemaProvider.GetQueryTables(Util.ReturnSQLTop(this.QueryString));
            dataTable.TableName = "QueryResults";
            dgrd_QueryResults.ItemsSource = dataTable;
        }

        private void AddListRelations(DataTable dataTable)
        {
            //Create the ListView
            System.Windows.Controls.ListView listview = new System.Windows.Controls.ListView();

            //Create the GridView and create GridViewColumns
            GridView gridview = new GridView();

            GridViewColumn gvcolumn = new GridViewColumn();
            gvcolumn.Width = 170;
            gvcolumn.Header = RESX.headerLeftTable.ToString();
            gvcolumn.DisplayMemberBinding = new System.Windows.Data.Binding(RESX.headerLeftTable.ToString());

            gridview.Columns.Add(gvcolumn);

            gvcolumn = new GridViewColumn();
            gvcolumn.Width = 150;
            gvcolumn.Header = RESX.headerRelationships.ToString() ;
            gvcolumn.DisplayMemberBinding = new System.Windows.Data.Binding(RESX.headerRelationships.ToString());

            gridview.Columns.Add(gvcolumn);

            gvcolumn = new GridViewColumn();
            gvcolumn.Width = 170;
            gvcolumn.Header = RESX.headerRightTable.ToString();
            gvcolumn.DisplayMemberBinding = new System.Windows.Data.Binding(RESX.headerRightTable.ToString());

            gridview.Columns.Add(gvcolumn);

            //Set the ListView's View to GridView
            listview.View = gridview;

            //Set the DataContext property of the ListView to the DataTable and bind the ItemsSourceProperty to {Binding}
            System.Windows.Data.Binding bind = new System.Windows.Data.Binding();
            listview.DataContext = dataTable;

            listview.SetBinding(System.Windows.Controls.ListView.ItemsSourceProperty, bind);
            //this.expnd_RelationShips.Content = listview;
        }

        private string GetFields(List<SchemaInfo> list)
        {
            StringBuilder tempString = new StringBuilder();
            int i = 0;
            foreach (SchemaInfo schemaInfo in list)
            {
                if (i > 0)
                {
                    tempString.Append(",");
                }
                tempString.Append("'" + schemaInfo.Key + "'");
                i++;
            }
            return tempString.ToString();
        }

        private void wizardControl_Cancel(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCancelWizard"), this.error_title, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void wizardControl_Finish(object sender, RoutedEventArgs e)
        {
            if (this.lvw_MeasureGroups.Items.Count > 0)
            {
                if (this.Title != RESX.titleNewChart)
                {
                    this.DialogResult = true;
                }
                else
                {
                    int isAggregate = 0;
                    foreach (ListBoxItem value in lvw_MeasureGroups.Items)
                    {
                        string setText = value.Content.ToString();
                        if (setText.StartsWith(RESX.headerSum, true, null) || setText.StartsWith(RESX.headerCount, true, null) || setText.StartsWith(RESX.headerFirst, true, null) ||
                            setText.StartsWith(RESX.headerAvg, true, null))
                        {
                        }
                        else
                        {
                            isAggregate--;
                            break;
                        }
                    }
                    if (isAggregate >= 0)
                    {
                        this.DialogResult = true;
                    }
                }
                this.arrayList = new ArrayList();
                this.ValuesList = new List<string>();
                this.RowList = new List<string>();
                this.ColumnList = new List<string>();

                if (this.FindDataSetByName(reportDataSet.Name) == null)
                {
                    this.designPanel.AddDataSet(reportDataSet);
                    this.CachedDataSets.Add(reportDataSet);
                }

                foreach (ListBoxItem item in lvw_DataSetFields.Items)
                {
                    this.arrayList.Add(item.Content.ToString());
                }
                
                foreach (ListBoxItem value in lvw_MeasureGroups.Items)
                {
                    string setText = value.Content.ToString();
                    if (setText.StartsWith(RESX.headerSum, true, null) || setText.StartsWith(RESX.headerCount, true, null) || setText.StartsWith(RESX.headerFirst, true, null) ||
                        setText.StartsWith(RESX.headerAvg, true, null))
                    {
                        this.ValuesList.Add("=" + setText);
                    }
                    else
                    {
                        this.ValuesList.Add(setText);
                    }
                }
                
                foreach (string value in lvw_RowGroups.Items)
                {
                    this.RowList.Add(value);
                }

                foreach (string value in lvw_ColumnGroups.Items)
                {
                    this.ColumnList.Add(value);
                }

                if (this.Title == RESX.titleNewChart)
                {
                    if (this.DialogResult == true)
                    {
                        this.Close();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotAggregateFunction"), this.error_title, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                else
                {
                    this.Close();
                }
            }
            else
            {
                System.Windows.MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxValueFieldList"), this.error_title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void chck_TableNode_Click(object sender, RoutedEventArgs e)
        {
            bool isTableSelected = false;
            reportDataSet = new RDL.DOM.DataSet();
            reportDataSet.Query = new Query();
            
            if (this.dataSets.Count > 0)
            {
                reportDataSet.Name = "Dataset" + (this.dataSets.Count + 1).ToString();
            }
            else
            {
                reportDataSet.Name = "Dataset1";
            }

            reportDataSet.Query.DataSourceName = this.reportDataSource.Name;
            selectedTables = new List<SchemaInfo>();
            lvw_SelectedFields.Items.Clear();
            lvw_DataSetFields.Items.Clear();
            selectedTables = GetSelectedNode(listOfSchema);
            if (reportDataSet.Fields == null)
            {
                reportDataSet.Fields = new Fields();
            }

            switch (this.reportDataSource.ConnectionProperties.DataProvider)
            {
                case DataProviders.SQLServer:
                case DataProviders.SQLAzure:
                    {
                        OpenSqlConnection();
                    }
                    break;
                case DataProviders.ORACLE:
                    {
                        OpenOracleConnection();
                    }
                    break;
            } 

            try
            {
                if (selectedTables.Count > 0)
                {
                    switch (this.reportDataSource.ConnectionProperties.DataProvider)
                    {
                        case DataProviders.SQLServer:
                        case DataProviders.SQLAzure:
                            {
                                UpdateSqlQueryText();
                            }
                            break;
                        case DataProviders.ORACLE:
                            {
                                UpdateOracleQueryText();
                            }
                            break;
                    } 

                    reportDataSet.Query.CommandText = this.QueryString;
                }

                foreach (var item in selectedTables)
                {
                    if (item.IsSelected)
                    {
                        isTableSelected = true;
                        foreach (var tableColumns in item.SchemaInfos)
                        {
                            if (tableColumns.IsSelected)
                            {
                                Field dataSetField = new Field();
                                dataSetField.Name = tableColumns.Key;
                                dataSetField.DataField = tableColumns.Key;
                                dataSetField.TypeName = tableColumns.DataType;
                                if (CheckDuplicateField(tableColumns.Key))
                                {
                                    dataSetField.Name = item.Key + "_" + tableColumns.Key;
                                    dataSetField.DataField = dataSetField.Name;
                                    ListBoxItem listItem = new ListBoxItem();
                                    listItem.Content = dataSetField.Name;
                                    lvw_DataSetFields.Items.Add(listItem);
                                    lvw_SelectedFields.Items.Add(dataSetField.Name);
                                    reportDataSet.Fields.Add(dataSetField);
                                }
                                else
                                {
                                    dataSetField.Name = tableColumns.Key;
                                    dataSetField.DataField = tableColumns.Key;
                                    ListBoxItem listItem = new ListBoxItem();
                                    listItem.Content = dataSetField.Name;
                                    lvw_DataSetFields.Items.Add(listItem);
                                    lvw_SelectedFields.Items.Add(tableColumns.Key);
                                    reportDataSet.Fields.Add(dataSetField);
                                }
                            }
                        }
                    }
                }

                dataTableRelations = new DataTable();

                switch (this.reportDataSource.ConnectionProperties.DataProvider)
                {
                    case DataProviders.SQLServer:
                    case DataProviders.SQLAzure:
                        {
                            try
                            {
                                if (this.selectedTables.Count > 0)
                                {
                                    dataTableRelations = GetSqlTableRelation();
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Windows.MessageBox.Show(ex.Message, this.error_title);
                            }
                            if (dataTableRelations.Rows.Count > 0)
                            {
                                this.ListViewRelationships.DataContext = dataTableRelations;
                            }
                            else
                            {
                                DataTable dataTableNoRelations = new DataTable(RESX.headerRelationships.ToString());

                                dataTableNoRelations.Columns.Add(RESX.headerLeftTable.ToString(), typeof(string));
                                dataTableNoRelations.Columns.Add(RESX.headerRelationship.ToString(), typeof(string));
                                dataTableNoRelations.Columns.Add(RESX.headerRightTable.ToString(), typeof(string));

                                foreach (var item in this.selectedTables)
                                {
                                    dataTableNoRelations.Rows.Add(item.Key, RESX.headerUnRelated.ToString(), string.Empty);
                                }
                                this.ListViewRelationships.DataContext = dataTableNoRelations;
                            }
                        }
                        break;
                    case DataProviders.ORACLE:
                        {
                            /*OracleSchemaProvider sqlSchemaProvider = new OracleSchemaProvider(dbConnection);

                            try
                            {
                                if (this.selectedTables.Count > 0)
                                {
                                    dataTableRelations = sqlSchemaProvider.GetTableRelations(GetFields(this.selectedTables));
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Syncfusion.QueryDesigner");
                            }*/
                        }
                        break;
                } 

                if (isTableSelected)
                {
                    this.wiz_QueryDesigner.NextEnabled = true;
                }
                else
                {
                    this.wiz_QueryDesigner.NextEnabled = false;
                }
                dbConnection.Close();
                dbConnection.Dispose();
            }
            catch (Exception dataBaseException)
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxConnectionFailed") + this.reportDataSource.Name + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxDueTo") + "\n" + dataBaseException.Message, this.error_title, MessageBoxButton.OK);
            }
        }

        private void OpenOracleConnection()
        {
            try
            {
                dbConnection = new System.Data.OracleClient.OracleConnection(this.ConnectionString);
                dbConnection.Open();
            }
            catch { }
        }

        private void OpenSqlConnection()
        {
            try
            {
                dbConnection = new SqlConnection(this.ConnectionString);
                dbConnection.Open();
            }
            catch { }
        }

        private DataTable GetSqlTableRelation()
        {
            SqlSchemaProvider sqlSchemaProvider = new SqlSchemaProvider(dbConnection);
            return sqlSchemaProvider.GetTableRelations(GetFields(this.selectedTables));
        }

        private void UpdateOracleQueryText()
        {
            OracleSchemaProvider MySchemaProvider = new OracleSchemaProvider(dbConnection);
            MySchemaProvider.GetQueryResults(selectedTables);
            this.QueryString = MySchemaProvider.QueryString;
        }

        private void UpdateSqlQueryText()
        {
            SqlSchemaProvider MySchemaProviderquery = new SqlSchemaProvider(dbConnection);
            MySchemaProviderquery.GetQueryResults(selectedTables);
            this.QueryString = MySchemaProviderquery.QueryString;
        }

        private bool CheckDuplicateField(string fieldName)
        {
            if (this.reportDataSet.Fields != null)
            {
                foreach (Field dataSetField in this.reportDataSet.Fields)
                {
                    if (dataSetField.Name.ToLower() == fieldName.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void UnCheckSelectedNodes()
        {
            if (this.listOfSchema != null)
            {
                foreach (var item in this.listOfSchema)
                {
                    foreach (var childItem in item.SchemaInfos)
                    {
                        foreach (var tableItem in childItem.SchemaInfos)
                        {
                            if (tableItem.TreeNodeType == NodeType.Table
                                || tableItem.TreeNodeType == NodeType.View)
                            {
                                if (tableItem.IsSelected)
                                {
                                    tableItem.IsSelected = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void tgbtn_EditText_Click(object sender, RoutedEventArgs e)
        {
            if (this.ConnectionString != null)
            {
                List<SchemaInfo> tempSchemaInfos = new List<SchemaInfo>();
                tempSchemaInfos = this.GetSelectedNode(this.listOfSchema);

                if (tempSchemaInfos.Count > 0)
                {
                    switch (this.reportDataSource.ConnectionProperties.DataProvider)
                    {
                        case DataProviders.SQLServer:
                        case DataProviders.SQLAzure:
                            {
                                UpdateSqlQuerySchema(tempSchemaInfos);
                            }
                            break;
                        case DataProviders.ORACLE:
                            {
                                UpdateOracleQuerySchema(tempSchemaInfos);
                            }
                            break;
                    } 
                }
            }

            if (QueryEditString == string.Empty || QueryEditString == null)
            {
                QueryEditString = this.QueryString;
            }
            rtbox_TextView.Text = this.QueryEditString;
            if ((bool)tgbtn_EditText.IsChecked)
            {
                this.grd_TreeView.Visibility = System.Windows.Visibility.Collapsed;
                this.grd_TextView.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                if (textChanged)
                {
                    if (System.Windows.MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxQueryDesignerSupport"), this.error_title, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        if (this.rtbox_TextView.Text.ToString() == string.Empty)
                        {
                            UnCheckSelectedNodes();
                            this.trvw_Schemas.ItemsSource = null;
                            this.trvw_Schemas.ItemsSource = this.listOfSchema;
                            this.lvw_SelectedFields.Items.Clear();
                            this.QueryString = string.Empty;
                        }
                        this.grd_TreeView.Visibility = System.Windows.Visibility.Visible;
                        this.grd_TextView.Visibility = System.Windows.Visibility.Collapsed;
                        this.textChanged = false;
                    }
                    else
                    {
                        tgbtn_EditText.IsChecked = true;
                    }
                }
                else
                {
                    if (this.rtbox_TextView.Text.ToString() == string.Empty)
                    {
                        UnCheckSelectedNodes();
                        this.trvw_Schemas.ItemsSource = null;
                        this.trvw_Schemas.ItemsSource = this.listOfSchema;
                        this.lvw_SelectedFields.Items.Clear();
                        this.QueryString = string.Empty;
                    }
                    this.grd_TreeView.Visibility = System.Windows.Visibility.Visible;
                    this.grd_TextView.Visibility = System.Windows.Visibility.Collapsed;
                    this.textChanged = false;
                }

            }
        }

        private void UpdateOracleQuerySchema(List<SchemaInfo> tempSchemaInfos)
        {
            dbConnection = new System.Data.OracleClient.OracleConnection(this.ConnectionString);
            dbConnection.Open();
            OracleSchemaProvider sqlSchemaProvider = new OracleSchemaProvider(dbConnection);
            sqlSchemaProvider.GetQueryResults(tempSchemaInfos, this.dataTableRelations);
            this.QueryString = string.Empty;
            this.QueryString = sqlSchemaProvider.QueryString;
        }

        private void UpdateSqlQuerySchema(List<SchemaInfo> tempSchemaInfos)
        {
            dbConnection = new SqlConnection(this.ConnectionString);
            dbConnection.Open();
            SqlSchemaProvider sqlSchemaProvider = new SqlSchemaProvider(dbConnection);
            sqlSchemaProvider.GetQueryResults(tempSchemaInfos, this.dataTableRelations);
            this.QueryString = string.Empty;
            this.QueryString = sqlSchemaProvider.QueryString;
        }

        private RDL.DOM.DataSource FindDataSourceByName(string name)
        {
            foreach (RDL.DOM.DataSource reportDataSource in this.dataSources)
            {
                if (reportDataSource.Name == name)
                {
                    return reportDataSource;
                }
            }
            return null;
        }

        //This method returns all the key collection containing the name of tables
        private List<SchemaInfo> GetSelectedNode(List<SchemaInfo> schemaInfos)
        {
            List<SchemaInfo> tempSchemaInfo = new List<SchemaInfo>();
            if (schemaInfos != null)
            {
                foreach (var item in schemaInfos)
                {
                    foreach (var tableItem in item.SchemaInfos)
                    {
                        foreach (var tableitems in tableItem.SchemaInfos)
                        {
                            if (tableitems.IsSelected && (tableitems.TreeNodeType == NodeType.Table || tableitems.TreeNodeType == NodeType.View))
                            {
                                tempSchemaInfo.Add(tableitems);
                            }
                        }
                    }
                }
            }
            return tempSchemaInfo;
        }

        /// <summary>
        /// Occurs When Page #3(Wizard_ArrangeFileds) selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wiz_ArrangeFields_Selected(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.old_DataSetName) && this.reportDataSet != null && !string.Equals(this.old_DataSetName, this.reportDataSet.Name))
            {
                this.old_DataSetName = this.reportDataSet.Name;
                this.lvw_RowGroups.Items.Clear();
                this.lvw_ColumnGroups.Items.Clear();
                this.lvw_MeasureGroups.Items.Clear();
            }

            if (this.optionChooseDataSet.IsChecked == true)
            {
                this.lvw_DataSetFields.Items.Clear();
                if (this.reportDataSet != null && this.reportDataSet.Fields!=null)
                {
                    foreach (RDL.DOM.Field dataSetField in this.reportDataSet.Fields)
                    {
                        ListBoxItem listItem = new ListBoxItem();
                        listItem.Content = dataSetField.Name;
                        lvw_DataSetFields.Items.Add(listItem);
                    }
                }
                
            }

            else if ((bool)this.tgbtn_EditText.IsChecked)
            {
                DataTable dataTable = new DataTable();
                this.QueryString = this.rtbox_TextView.Text;
                reportDataSet.Query.CommandText = this.QueryString;
                try
                {
                    if (reportDataSet.Query.QueryParameters == null)
                    {
                        reportDataSet.Query.QueryParameters = new QueryParameters();
                    }

                    switch (this.reportDataSource.ConnectionProperties.DataProvider)
                    {
                        case DataProviders.SQLServer:
                        case DataProviders.SQLAzure:
                            {
                                dataTable = GetSqlTableFields();
                            }
                            break;
                        case DataProviders.ORACLE:
                            {
                                dataTable = GetOracleTableFields();
                            }
                            break;
                        case DataProviders.ODBC:
                            {
                                dataTable = GetOdbcTableFields();
                            }
                            break;
                        case DataProviders.OLEDB:
                            {
                                dataTable = GetOledbTableFields();
                            }
                            break;
                        case DataProviders.XML:
                            {
                                dataTable = GetXmlTableFields();
                            }
                            break;
                    } 
                }

                catch (Exception excep)
                {
                    MessageBox.Show(excep.Message, this.error_title);
                }

                reportDataSet.Query.DataSourceName = this.reportDataSource.Name;
                if (reportDataSet.Fields == null)
                {
                    reportDataSet.Fields = new Fields();
                }

                reportDataSet.Fields.Clear();
                lvw_DataSetFields.Items.Clear();
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    Field dataSetField = new Field();
                    dataSetField.Name = dataTable.Columns[i].ColumnName;
                    dataSetField.DataField = dataTable.Columns[i].ColumnName;
                    dataSetField.TypeName = dataTable.Columns[i].DataType.ToString();
                    reportDataSet.Fields.Add(dataSetField);
                    ListBoxItem listItem = new ListBoxItem();
                    listItem.Content = dataSetField.Name;
                    lvw_DataSetFields.Items.Add(listItem);                    
                }

            }
        }

        private DataTable GetXmlTableFields()
        {
            XMLDataProvider xml = new XMLDataProvider();
            DataTable dataTable = new DataTable();
            dataTable = xml.GetTable(this.ConnectionString, this.QueryString, "Table");
            return dataTable;
        }

        private DataTable GetOledbTableFields()
        {
            System.Data.OleDb.OleDbConnection connection = new System.Data.OleDb.OleDbConnection(this.ConnectionString);
            connection.Open();
            this.dbConnection = connection;
            DataTable dataTable = new DataTable();
            System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(this.QueryString, connection);
            System.Data.OleDb.OleDbDataReader dr = cmd.ExecuteReader();
            dataTable.Load(dr);
            return dataTable;
        }

        private DataTable GetOdbcTableFields()
        {
            System.Data.Odbc.OdbcConnection connection = new System.Data.Odbc.OdbcConnection(this.ConnectionString);
            connection.Open();
            DataTable dataTable = new DataTable();
            System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(this.QueryString, connection);
            System.Data.Odbc.OdbcDataReader dr = cmd.ExecuteReader();
            dataTable.Load(dr);
            return dataTable;
        }

        private DataTable GetOracleTableFields()
        {
            dbConnection = new System.Data.OracleClient.OracleConnection(this.ConnectionString);
            this.dbConnection.Open();
            //dataTable = new DataTable();
            OracleSchemaProvider sqlSchemaProvider = new OracleSchemaProvider(dbConnection);
            DataTable dataTable = new DataTable();
            #region Parameterized Query for Oracle
            if (this.QueryString.Contains(":"))
            {
                dataTable = GetParameterizedTable(this.reportDataSet.Query);
            }
            #endregion

            #region Plain Query for Oracle
            else
            {
                dataTable = sqlSchemaProvider.GetQueryTables(this.QueryString);
            }
            #endregion

            dataTable.TableName = "QueryResults"; //Setting TableName as QueryResults

            return dataTable;
        }

        private DataTable GetSqlTableFields()
        {
            dbConnection = new SqlConnection(this.ConnectionString);
            this.dbConnection.Open();
            DataTable dataTable = new DataTable();
            SqlSchemaProvider sqlSchemaProvider = new SqlSchemaProvider(this.dbConnection);

            #region Parameterized Query for SQLServer
            if (this.QueryString.Contains("@"))
            {
                dataTable = GetParameterizedTable(this.reportDataSet.Query);
            }
            #endregion

            #region Plain Query for SQLServer
            else
            {
                dataTable = sqlSchemaProvider.GetQueryTables(this.QueryString);
            }
            #endregion

            dataTable.TableName = "QueryResults"; //Setting TableName as QueryResults

            return dataTable;
        }

        /// <summary>
        /// To execute the Parameterized query
        /// </summary>
        /// <param name="query">RDL Specification Query</param>
        /// <returns>Contailing as DataTable</returns>
        private DataTable GetParameterizedTable(Query query)
        {

            // find any Token for Parameterized Query followed by '@' Symbol Or ':' Symbol
            Regex theReg = new Regex("");

            switch (this.reportDataSource.ConnectionProperties.DataProvider)
            {
                case DataProviders.SQLServer:
                case DataProviders.SQLAzure:
                    {
                        theReg = new Regex(@"(\@)([a-zA-Z0-9]+)", RegexOptions.IgnoreCase);
                    }
                    break;
                case DataProviders.ORACLE:
                    {
                        theReg = new Regex(@"([:])([a-zA-Z0-9]+)", RegexOptions.IgnoreCase);
                    }
                    break;
            } 

            // get the collection of matches
            MatchCollection theMatches = theReg.Matches(query.CommandText);

            // For Avoiding duplicate Parameterized Tokens
            Dictionary<string, string> _eliminateDuplicatesIgnoreCase = new Dictionary<string, string>();

            // iterate through the collection
            foreach (Match theMatch in theMatches)
            {
                if (theMatch.Length != 0)
                {
                    try
                    {
                        _eliminateDuplicatesIgnoreCase.Add(theMatch.ToString().ToUpper(), "");

                        QueryParameter queryParameter = new QueryParameter();
                        queryParameter.Name = theMatch.ToString().ToUpper();
                        queryParameter.Value = " =Parameters!" + theMatch.ToString() + ".Value";
                        query.QueryParameters.Add(queryParameter);
                    }
                    catch
                    {
                    }
                }

            }

            switch (this.reportDataSource.ConnectionProperties.DataProvider)
            {
                case DataProviders.SQLServer:
                case DataProviders.SQLAzure:
                    {
                        return GetSqlTable(query);
                    }
                case DataProviders.ORACLE:
                    {
                        return GetOracleTable(query);
                    }
            } 

            return null;
        }

        private DataTable GetSqlTable(Query query)
        {
            SqlCommand sqlCmd = new SqlCommand(query.CommandText);
            if (query.QueryParameters.Count > 0)
            {
                foreach (QueryParameter qP in query.QueryParameters)
                {
                    sqlCmd.Parameters.AddWithValue(qP.Name, string.Empty);
                }
            }

            return new SqlDataProvider().GetTable(this.dbConnection, sqlCmd);
        }

        private DataTable GetOracleTable(Query query)
        {
            System.Data.OracleClient.OracleCommand orclCmd = new System.Data.OracleClient.OracleCommand(query.CommandText);
            if (query.QueryParameters.Count > 0)
            {
                foreach (QueryParameter qP in query.QueryParameters)
                {
                    orclCmd.Parameters.AddWithValue(qP.Name, string.Empty);
                }
            }
            return new OracleDataProvider().GetTable(this.dbConnection, orclCmd);
        }

        #endregion

        #region Static Methods

        private static object GetObjectDataFromPoint(System.Windows.Controls.ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                        element = VisualTreeHelper.GetParent(element) as UIElement;

                    if (element == source)
                        return null;
                }
                if (data != DependencyProperty.UnsetValue)
                    return data;
            }
            return null;
        }

        #endregion

        #region TreeView Populating methods

        private void BindTreeViewControl()
        {
            try
            {
                if (this.ConnectionString != "")
                {
                    switch (this.reportDataSource.ConnectionProperties.DataProvider)
                    {
                        case DataProviders.SQLServer:
                        case DataProviders.SQLAzure:
                            {
                                if (!string.IsNullOrEmpty(this.reportDataSource.ConnectionProperties.UserName) && !string.IsNullOrEmpty(this.reportDataSource.ConnectionProperties.PassWord))
                                {
                                    this.ConnectionString += ";" + ConnectionConstants.UserID + "=" + this.reportDataSource.ConnectionProperties.UserName.ToString();
                                    this.ConnectionString += ";" + ConnectionConstants.Password + "=" + this.reportDataSource.ConnectionProperties.PassWord.ToString();
                                }

                                if (this.reportDataSource.ConnectionProperties.IntegratedSecurity)
                                {
                                    this.ConnectionString += ";Trusted_Connection=" + (bool)this.reportDataSource.ConnectionProperties.IntegratedSecurity;
                                }

                                OpenSqlConnection();
                            }
                            break;
                        case DataProviders.ORACLE:
                            {
                                OpenOracleConnection();
                            }
                            break;
                        case DataProviders.ODBC:
                            {
                                if (this.reportDataSource.ConnectionProperties.UserName != string.Empty && this.reportDataSource.ConnectionProperties.PassWord != string.Empty)
                                {
                                    this.ConnectionString += "Uid=Admin";
                                }

                                OpenOdbcConnection();
                            }
                            break;
                        case DataProviders.OLEDB:
                            {
                                if (this.reportDataSource.ConnectionProperties.IntegratedSecurity == true)
                                {
                                    this.ConnectionString += ";Trusted_Connection=yes";
                                }
                                if (!this.reportDataSource.ConnectionProperties.IntegratedSecurity && this.reportDataSource.ConnectionProperties.UserName != string.Empty &&
                                    this.reportDataSource.ConnectionProperties.PassWord != string.Empty)
                                {
                                    this.ConnectionString += ";Uid=" + this.reportDataSource.ConnectionProperties.UserName + "; Pwd=" + this.reportDataSource.ConnectionProperties.PassWord;
                                }
                                if (!this.reportDataSource.ConnectionProperties.IntegratedSecurity)
                                {
                                    this.ConnectionString = this.reportDataSource.ConnectionProperties.ConnectString;
                                }

                                OpenOledbConnection();
                            }
                            break;
                        case DataProviders.XML:
                            {
                            }
                            break;
                    }

                    this.rtbox_TextView.Text = string.Empty;
                    this.QueryString = string.Empty;
                    this.dgrd_QueryResults.ItemsSource = null;
                    this.wiz_QueryDesigner.NextEnabled = false;

                    if (this.reportDataSource.ConnectionProperties.DataProvider == DataProviders.SQLServer ||
                        this.reportDataSource.ConnectionProperties.DataProvider == DataProviders.SQLAzure ||
                        this.reportDataSource.ConnectionProperties.DataProvider == DataProviders.ORACLE)
                    {
                        //dbConnection.Open();
                        this.lvw_SelectedFields.Items.Clear();
                        this.trvw_Schemas.ItemsSource = null;
                        this.trvw_Schemas.ItemsSource = this.listOfSchema;

                        this.tgbtn_EditText.IsChecked = false;
                        this.tgbtn_EditText.IsEnabled = true;
                        this.grd_TreeView.Visibility = System.Windows.Visibility.Visible;
                        this.grd_TextView.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        this.tgbtn_EditText.IsChecked = true;
                        this.tgbtn_EditText.IsEnabled = false;
                        this.grd_TreeView.Visibility = System.Windows.Visibility.Collapsed;
                        this.grd_TextView.Visibility = System.Windows.Visibility.Visible;
                        this.wiz_ControlDataSourceBinder.NextPage = this.wiz_QueryDesigner;
                    }
                }

                PopulateSchemas();
                this.trvw_Schemas.ItemsSource = listOfSchema;
            }
            catch (Exception conException)
            {
                throw conException;
            }
        }

        private void OpenOledbConnection()
        {
            try
            {
                dbConnection = new System.Data.OleDb.OleDbConnection(this.ConnectionString);
                dbConnection.Open();
            }
            catch { }
        }

        private void OpenOdbcConnection()
        {
            try
            {
                dbConnection = new System.Data.Odbc.OdbcConnection(this.ConnectionString);
                dbConnection.Open();
            }
            catch { }
        }

        private void PopulateSchemas()
        {
            switch (this.reportDataSource.ConnectionProperties.DataProvider)
            {
                case DataProviders.SQLServer:
                case DataProviders.SQLAzure:
                    {
                        try
                        {
                            PopulateSqlSchemas();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, this.error_title);
                        }
                    }
                    break;
                case DataProviders.ORACLE:
                    {
                        try
                        {
                            PopulateOracleSchemas();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, this.error_title);
                        }
                    }
                    break;
            }

            ArrangeViewOrder(this.listOfSchema);
        }

        private List<SchemaInfo> ArrangeViewOrder(List<SchemaInfo> list)
        {
            if (list != null && list.Count > 0)
            {
                foreach (SchemaInfo schemaInfo in list.ToList())
                {
                    if (schemaInfo.SchemaInfos.Count > 0)
                    {
                        if (schemaInfo.TreeNodeType == NodeType.Folder)
                        {
                            schemaInfo.SchemaInfos = ArrangeViewOrder(schemaInfo.SchemaInfos);
                        }
                        else if (schemaInfo.TreeNodeType == NodeType.Table)
                        {
                            return list.OrderBy(schema => schema.Key).ToList();
                        }
                    }
                }
            }

            return list;
        }

        private void PopulateOracleSchemas()
        {
            string UserID = GetUserID(this.reportDataSource.ConnectionProperties.ConnectString).ToUpper();

            OracleSchemaProvider MySchemaProvider = new OracleSchemaProvider(dbConnection, UserID);
            DataTable dataTableSchemas = new DataTable();
            dataTableSchemas = MySchemaProvider.GetSchemas();
            dataTableSchemas.TableName = "Schemas";
            DataTable dataTable = new DataTable();
            dataTable = MySchemaProvider.GetTables();
            dataTable.TableName = "Tables";
            DataTable dataTableViews = new DataTable();
            dataTableViews = MySchemaProvider.GetViews();
            dataTableViews.TableName = "Views";
            DataTable dataTableStoredProc = new DataTable();
            //dataTableStoredProc = MySchemaProvider.GetStoredProcedures();
            dataTableStoredProc.TableName = "Stored Procedures";
            DataTable dataTableTableFunc = new DataTable();
            //dataTableTableFunc = MySchemaProvider.GetTableValueFunctions();
            dataTableTableFunc.TableName = "Table Valued Functions";
            DataTable dataTableTableColumns = new DataTable();
            dataTableTableColumns = MySchemaProvider.GetTableColumns();
            dataTableTableColumns.TableName = "Table Columns";
            DataTable dataTableViewColumns = new DataTable();
            dataTableViewColumns = MySchemaProvider.GetViewColumns();
            dataTableViewColumns.TableName = "View Columns";
            dbConnection.Close();
            listOfSchema = GetSchemaInfo(dataTableSchemas);
            UpdateSchemaDefaultChild(listOfSchema);
            //iterating the child nodes and updating the tables, stored procedures, views and functions
            foreach (var item in listOfSchema)
            {
                GetTableInfo("Table", dataTable, item, NodeType.Table);
                GetTableInfo("Views", dataTableViews, item, NodeType.View);
                //GetTableInfo("Stored Procedures", dataTableStoredProc, item, NodeType.StoredProcedure);
                //GetTableInfo("Table Value Functions", dataTableTableFunc, item, NodeType.TableValuedFunction);
                GetTableColumnInfo(item.Key, "Table", dataTableTableColumns, item, NodeType.TableColumn);
                GetTableColumnInfo(item.Key, "Views", dataTableViewColumns, item, NodeType.TableColumn);
            }
            RemoveEmptyChildNodes(this.listOfSchema);
        }

        private void PopulateSqlSchemas()
        {
            SqlSchemaProvider MySchemaProvider = new SqlSchemaProvider(dbConnection);
            DataTable dataTableSchemas = new DataTable();
            dataTableSchemas = MySchemaProvider.GetSchemas();
            dataTableSchemas.TableName = "Schemas";
            DataTable dataTable = new DataTable();
            dataTable = MySchemaProvider.GetTables();
            dataTable.TableName = "Tables";
            DataTable dataTableViews = new DataTable();
            dataTableViews = MySchemaProvider.GetViews();
            dataTableViews.TableName = "Views";
            DataTable dataTableStoredProc = new DataTable();
            //dataTableStoredProc = MySchemaProvider.GetStoredProcedures();
            dataTableStoredProc.TableName = "Stored Procedures";
            DataTable dataTableTableFunc = new DataTable();
            //dataTableTableFunc = MySchemaProvider.GetTableValueFunctions();
            dataTableTableFunc.TableName = "Table Valued Functions";
            DataTable dataTableTableColumns = new DataTable();
            dataTableTableColumns = MySchemaProvider.GetTableColumns();
            dataTableTableColumns.TableName = "Table Columns";
            DataTable dataTableViewColumns = new DataTable();
            dataTableViewColumns = MySchemaProvider.GetViewColumns();
            dataTableViewColumns.TableName = "View Columns";
            dbConnection.Close();
            listOfSchema = GetSchemaInfo(dataTableSchemas);
            UpdateSchemaDefaultChild(listOfSchema);
            //iterating the child nodes and updating the tables, stored procedures, views and functions
            foreach (var item in listOfSchema)
            {
                GetTableInfo("Table", dataTable, item, NodeType.Table);
                GetTableInfo("Views", dataTableViews, item, NodeType.View);
                //GetTableInfo("Stored Procedures", dataTableStoredProc, item, NodeType.StoredProcedure);
                //GetTableInfo("Table Value Functions", dataTableTableFunc, item, NodeType.TableValuedFunction);
                GetTableColumnInfo(item.Key, "Table", dataTableTableColumns, item, NodeType.TableColumn);
                GetTableColumnInfo(item.Key, "Views", dataTableViewColumns, item, NodeType.TableColumn);
            }
            RemoveEmptyChildNodes(this.listOfSchema);
        }

        private string GetUserID(string fieldNameContainer)
        {
            string parsedFieldName = "";
            string startString = "User ID=";
            string endString = ";Password";
            if (fieldNameContainer.Contains(startString) && fieldNameContainer.Contains(endString))
            {
                int indexOfStarting = fieldNameContainer.IndexOf(startString);
                int endingIndex = fieldNameContainer.IndexOf(endString);
                int startingIndex = indexOfStarting + startString.Length;
                int stringLength = (endingIndex - startingIndex);
                parsedFieldName = fieldNameContainer.Substring(startingIndex, stringLength);
            }
            return parsedFieldName;

        }

        private void RemoveEmptyChildNodes(List<SchemaInfo> schemaInfos)
        {
            SchemaInfo tempSchemaInfo = new SchemaInfo();
            for (int i = (schemaInfos.Count - 1); i >= 0; i--)
            {
                SchemaInfo item = schemaInfos[i];
                for (int j = (item.SchemaInfos.Count - 1); j >= 0; j--)
                {
                    SchemaInfo objectItem = item.SchemaInfos[j];
                    if (objectItem.SchemaInfos.Count <= 0 && objectItem.TreeNodeType == NodeType.Folder)
                    {
                        item.SchemaInfos.Remove(objectItem);
                    }
                }
            }
        }

        List<SchemaInfo> GetSchemaInfo(DataTable schemaTable)
        {
            List<SchemaInfo> schemaInfo = new List<SchemaInfo>();
            foreach (System.Data.DataRow item in schemaTable.Rows)
            {
                schemaInfo.Add(new SchemaInfo { Key = item.ItemArray[0].ToString(), Parent = null, ImageSource = this.FolderImage });
            }
            return schemaInfo;
        }

        private void GetTableInfo(string key, DataTable tableInfo, SchemaInfo schemaInfo, NodeType nodeType)
        {
            if (schemaInfo != null)
            {
                foreach (var item in schemaInfo.SchemaInfos)
                {
                    if (item.Key == key)
                    {
                        //Perform population of tables
                        if (item.SchemaInfos == null)
                        {
                            item.SchemaInfos = new List<SchemaInfo>();
                        }
                        foreach (System.Data.DataRow tableRow in tableInfo.Rows)
                        {
                            if (item.Parent.Key == tableRow.ItemArray[0].ToString())
                            {
                                item.SchemaInfos.Add(new SchemaInfo { Key = tableRow.ItemArray[1].ToString(), Parent = item, TreeNodeType = nodeType, ImageSource = this.FolderImage });
                            }
                        }
                        return;
                    }
                    else
                    {
                        //pass it to the child nodes update function and retrieve the childs
                        GetTableInfo(key, tableInfo, UpdateChildNodes(key, item), nodeType);
                        //List<SchemaInfo> schemaTableInfo = UpdateChildNodes("Table", item.SchemaInfos);
                    }
                }
                return;
            }
            else
            {
                return;
            }
        }

        private void GetTableColumnInfo(string schemaKey, string keyType, DataTable tableColumnInfo, SchemaInfo schemaInfo, NodeType nodeType)
        {
            if (schemaInfo != null)
            {
                foreach (var item in schemaInfo.SchemaInfos)
                {
                    if (item.Key == keyType)
                    {
                        foreach (var objectItem in item.SchemaInfos)
                        {
                            foreach (System.Data.DataRow tableRow in tableColumnInfo.Rows)
                            {
                                if (item.Parent.Key == tableRow.ItemArray[0].ToString())
                                {
                                    if (objectItem.Key == tableRow.ItemArray[1].ToString())
                                    {
                                        objectItem.SchemaInfos.Add(new SchemaInfo { Key = tableRow.ItemArray[2].ToString(), Parent = objectItem, TreeNodeType = nodeType, DataType = tableRow.ItemArray[3].ToString(), ImageSource = this.FolderImage });
                                    }
                                }
                            }
                        }
                        return;
                    }
                }
                return;
            }
            else
            {
                return;
            }
        }

        private void UpdateSchemaDefaultChild(List<SchemaInfo> listOfSchema)
        {
            foreach (var item in listOfSchema)
            {
                item.SchemaInfos.Add(new SchemaInfo { Key = "Table", Parent = item, ImageSource = this.FolderImage });
                item.SchemaInfos.Add(new SchemaInfo { Key = "Views", Parent = item, ImageSource = this.FolderImage });
                //item.SchemaInfos.Add(new SchemaInfo { Key = "Stored Procedures", Parent = item });
                //item.SchemaInfos.Add(new SchemaInfo { Key = "Table Value Functions", Parent = item });
            }
        }

        private SchemaInfo UpdateChildNodes(string key, SchemaInfo schemaInfo)
        {
            if (schemaInfo != null)
            {
                foreach (var item in schemaInfo.SchemaInfos)
                {
                    if (item.Key == key)
                    {
                        return item;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises the data source changed.
        /// </summary>
        internal void RaiseDataSourceChanged()
        {
            if (this.DataSourceChanged != null)
            {
                this.OnDataSourceChanged(this, new DataSourceChangedEventArgs(this.reportDataSource));
            }
        }

        protected virtual void OnDataSourceChanged(object sender, DataSourceChangedEventArgs e)
        {
            if (this.DataSourceChanged != null)
            {
                this.DataSourceChanged(this, e);
            }
        }
        #endregion

        private void wiz_QueryDesigner_Selected(object sender, RoutedEventArgs e)
        {
            this.wiz_QueryDesigner.NextPage = this.wiz_ArrangeFields;
            this.Wiz_ControlDataSetBinder.NextPage = this.wiz_ControlDataSourceBinder;
        }

        private void wnd_ControlDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.PreviewKeyDown += new KeyEventHandler(ControlDialog_PreviewKeyDown);
            this.lvw_DataSets.Focus();
        }

        void ControlDialog_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
            else if (e.Key == Key.Enter)
            {
                this.wizardControl.NextFocused = true;
                if (this.wizardControl.FinishVisible == true)
                {
                    this.wizardControl.FinishFocused = true;
                }
            }
        }
    }

}