//-------------------------------------------------------------------------------------------------
// <copyright file="DataSetUI.cs" company="syncfusion">
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
using Syncfusion.RDL.DOM;
using Syncfusion.Windows.Reports.Sql;
using System.ComponentModel;
using System.Data.Common;
using System.Text.RegularExpressions;
using Syncfusion.Windows.Reports.Relational.Sql;
using System.Data.SqlClient;
using System.Data;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.RDL.Data;
using System.Globalization;
using Syncfusion.Windows.ReportDesigner.Resources;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    #region delegates

    /// <summary>
    /// Occurs when DatasSourceChange event occurs.
    /// </summary>
    internal delegate void DataSourceChangedEventHandler(object sender, DataSourceChangedEventArgs e);
    #endregion

    #region Event Class Declaration
    /// <summary>
    /// Delegate class for DataSourceChaned Event
    /// </summary>
    internal class DataSourceChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceChangedEventArgs"/> class.
        /// </summary>
        /// <param name="reportDataSource">The report data source.</param>
        public DataSourceChangedEventArgs(Syncfusion.RDL.DOM.DataSource reportDataSource)
        {
            this.DataSource = reportDataSource;
        }

        /// <summary>
        /// Gets or sets the report data source.
        /// </summary>
        /// <value>The report data source.</value>
        public Syncfusion.RDL.DOM.DataSource DataSource { get; private set; }
    }
    #endregion

    /// <summary>
    /// Interaction logic for Essential WPF DataSetUI.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif

        [ObsoleteAttribute("OracleCommand has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    internal partial class DataSetUI
        : ChromelessWindow
    {
        #region Private Properties

        private string ConnectionString { get; set; }

        private DesignPanel Designpanel { get; set; }

        private bool isModifyDataset = false;

        private string OldQueryString { get; set; }

        private string OldProcedure { get; set; }

        private Credentials Credentials { get; set; }

        private List<string> StoredProcedures { get; set; }

        private Dictionary<string, List<ProcedureParameter>> ListofProcedures { get; set; }

        private List<ProcedureParameter> ListofParameters { get; set; }

        private string old_Query = string.Empty;

        private string old_datasource = string.Empty;

        private string old_datasetname = string.Empty;

        private DataTable dataSetTables { get; set; }

        private static Dictionary<string, string> Old_ProcedureParameters
        {
            get;
            set;
        }

        private string error_title;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the report data source.
        /// </summary>
        /// <value>The report data source.</value>
        public RDL.DOM.DataSource DataSource { get; set; }

        public DataSources DataSources { get; set; }

        /// <summary>
        /// Gets or sets the report data set.
        /// </summary>
        /// <value>The report data set.</value>
        public RDL.DOM.DataSet DataSet { get; set; }

        public DataSets DataSets { get; set; }

        public RDL.DOM.Query Query { get; set; }

        public RDL.DOM.Fields Fields { get; set; }

        public ReportParameters ReportParameters { get; set; }

        /// <summary>
        /// Gets or sets the DB connection.
        /// </summary>
        /// <value>The DB connection.</value>
        public DbConnection DBConnection { get; set; }

        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        /// <value>The query string.</value>
        public string QueryString { get; set; }

    
        #endregion

        #region Contsructor

        public DataSetUI(DataSets dataSets,DataSources dataSources,DesignPanel panel )
        {
            InitializeComponent();
            this.Title = SR.GetString(CultureInfo.CurrentUICulture, "titleDataSetProperties");
            this.error_title = SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner");
            this.DataSets = dataSets;
            this.DataSources = dataSources;
            this.DataSet = new RDL.DOM.DataSet();
            this.Designpanel = panel;
            if (this.DataSources.Count > 0)
            {
                if (!string.IsNullOrEmpty(panel.CurrentDataSource))
                {
                    this.DataSource = (from datasource in this.DataSources
                                       where datasource.Name.Equals(panel.CurrentDataSource)
                                       select datasource).FirstOrDefault();
                }
                else
                {
                    this.DataSource = this.DataSources.First();
                }
            }
            else
            {
                this.DataSource = null;
            }

            int availableDataSetsNamesCount = 1;

            string[] availableDataSetNames = (from dset in this.DataSets
                                              select dset.Name).ToArray<string>();

            foreach (string dsName in availableDataSetNames)
            {
                if (dsName.Equals("DataSet" + (availableDataSetsNamesCount)))
                {
                    availableDataSetsNamesCount++;
                }
            }

            this.DataSet.Name =this.old_datasetname ="DataSet" + (availableDataSetsNamesCount).ToString();
            this.txt_DataSetName.Text= this.DataSet.Name;

            this.DataSet.Query = new RDL.DOM.Query();
            this.IntializeDataSetUI();
            this.WireEvents();
            this.UpdateQueryDesignerButtonStatus();
        }

        public DataSetUI(RDL.DOM.DataSet reportDataSet,DataSets dataSets,DataSources dataSources,DesignPanel panel)
        {
            this.InitializeComponent();
            this.Title = SR.GetString(CultureInfo.CurrentUICulture, "titleDataSetProperties");
            this.error_title = SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner");
            this.Designpanel = panel;
            this.DataSet = reportDataSet;
            this.txt_DataSetName.Text =this.old_datasetname=this.DataSet.Name;
            this.DataSources = dataSources;
            this.DataSets = dataSets;
            this.isModifyDataset = true;

            this.DataSource = (from dataSource in this.DataSources
                               where dataSource.Name == this.DataSet.Query.DataSourceName
                               select dataSource).First();

            this.IntializeDataSetUI();
            this.WireEvents();
            this.UpdateQueryDesignerButtonStatus();

            if (this.DataSet != null && this.DataSet.Query != null && this.DataSet.Query.CommandType == RDL.DOM.CommandType.StoredProcedure)
            {
                try
                {
                    this.param_query.ItemsSource = null;
                    this.ListofProcedures = null;
                    this.Storedprocedure.IsChecked = true;
                    this.param_query.Text = this.DataSet.Query.CommandText;
                    this.param_query.SelectedItem = ListofProcedures[this.DataSet.Query.CommandText];
                }
                catch { }
            }
        }

        #endregion

        #region Helper Methods

        void UpdateQueryDesignerButtonStatus()
        {
            this.btn_QueryDesigner.ToolTip = null;

            if (this.DataSource != null &&
                (this.DataSource.ConnectionProperties.DataProvider == DataProviders.SQLServer ||
                this.DataSource.ConnectionProperties.DataProvider == DataProviders.SQLAzure))
            {
                this.btn_QueryDesigner.IsEnabled = true;
                this.Storedprocedure.Visibility = System.Windows.Visibility.Visible;
            }

            else if (this.DataSource != null && this.DataSource.ConnectionProperties.DataProvider == DataProviders.SQLServerCe)
            {
                this.btn_QueryDesigner.IsEnabled = true;
                this.Storedprocedure.Visibility = System.Windows.Visibility.Hidden;
            }

            else
            {
                this.btn_QueryDesigner.IsEnabled = false;
                this.btn_QueryDesigner.ToolTip = "This feature not available for selected DataSource connection type.";
                this.Storedprocedure.Visibility = System.Windows.Visibility.Hidden;
            }

            if (this.Designpanel != null)
            {
                if (!this.Designpanel.ShowHelp)
                {
                    this.btn_HelponDataSet.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        void IntializeDataSetUI()
        {
            this.Query = new RDL.DOM.Query();
            foreach (RDL.DOM.DataSource reportDataSource in this.DataSources)
            {
                this.cmb_DataSources.Items.Add(reportDataSource.Name);
            }

            if (this.cmb_DataSources.Items.Count > 0)
            {
                this.cmb_DataSources.SelectedItem =this.old_datasource =this.DataSource.Name;
                this.Query.DataSourceName = this.DataSource.Name;
                this.Query.CommandText = this.DataSet.Query.CommandText;

                if (this.DataSet.Query.CommandType == RDL.DOM.CommandType.StoredProcedure)
                {
                    this.OldProcedure = this.DataSet.Query.CommandText;
                }
                else
                {
                    this.OldQueryString = this.old_Query = this.txt_Query.Text = this.Query.CommandText;
                }
            }
            if (this.DataSet != null)
            {
                this.tbl_filters.PopulateFilterWindow(this.DataSet, this.DataSet.Filters);
            }
        }

        void WireEvents()
        {
            this.btn_NewDataSource.Click += new RoutedEventHandler(btn_NewDataSource_Click);
            this.btn_QueryDesigner.Click += new RoutedEventHandler(btn_QueryDesigner_Click);
            this.btn_RefreshFields.Click += new RoutedEventHandler(btn_RefreshFields_Click);
            this.btn_DataSetOk.Click += new RoutedEventHandler(btn_DataSetOk_Click);
            this.btn_DataSetCancel.Click += new RoutedEventHandler(btn_DataSetCancel_Click);
            this.btn_HelponDataSet.Click += new RoutedEventHandler(btn_HelponDataSet_Click);
            this.Storedprocedure.Checked += new RoutedEventHandler(Storedprocedure_Checked);
            this.Storedprocedure.Unchecked += new RoutedEventHandler(Storedprocedure_Unchecked);
            this.param_query.SelectionChanged += new SelectionChangedEventHandler(param_query_SelectionChanged);
            this.Loaded += new RoutedEventHandler(DataSetUI_Loaded);
        }

        void param_query_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.param_query.SelectedItem != null && this.param_query.SelectedItem.ToString() != string.Empty)
            {
                ListofParameters = ListofProcedures[this.param_query.SelectedItem.ToString()];
                this.Query.CommandText = this.param_query.SelectedItem.ToString();
            }
        }

        void Storedprocedure_Unchecked(object sender, RoutedEventArgs e)
        {
            this.param_query.Visibility = System.Windows.Visibility.Collapsed;
            this.txt_Query.Visibility = System.Windows.Visibility.Visible;
        }

        void Storedprocedure_Checked(object sender, RoutedEventArgs e)
        {
            this.param_query.Visibility = System.Windows.Visibility.Visible;
            this.txt_Query.Visibility = System.Windows.Visibility.Collapsed;

            if (this.param_query.ItemsSource == null)
            {
                UpdateStoredProcedure();
            }
        }

        void DataSetUI_Loaded(object sender, RoutedEventArgs e)
        {
            this.PreviewKeyDown += new KeyEventHandler(DataSetUI_PreviewKeyDown);
            this.txt_DataSetName.Focus();
        }

        void DataSetUI_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        void btn_HelponDataSet_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://help.syncfusion.com/Reporting/Report Designer/WPF/DataSet");
        }

        private Syncfusion.RDL.DOM.DataSource FindDataSource(string name)
        {
            foreach (Syncfusion.RDL.DOM.DataSource reportDataSource in this.DataSources)
            {
                if (reportDataSource.Name == name)
                {
                    return reportDataSource;
                }
            }
            return null;
        }

        void btn_RefreshFields_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.cmb_DataSources.SelectedValue != null)
                {
                    if (this.Storedprocedure.IsChecked == true)
                    {
                        string selected = this.param_query.Text;
                        this.param_query.ItemsSource = null;
                        this.ListofProcedures = null;
                        UpdateStoredProcedure();
                        this.param_query.Text = selected;
                        this.OldProcedure = this.param_query.Text;
                        this.param_query.SelectedItem = ListofProcedures[selected];
                        string connstr = this.ConnectionString;

                        if (this.DataSource.ConnectionProperties.IntegratedSecurity)
                        {
                            connstr = this.ConnectionString + ";Trusted_Connection=yes;";
                        }

                        GetStoredProcedureTable(connstr, false);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(this.txt_Query.Text))
                        {
                            if (this.Text.IsChecked == true)
                            {
                                this.Query.CommandText = this.txt_Query.Text;
                                this.QueryString = this.txt_Query.Text;
                            }
                            
                            this.ConnectionString = this.DataSource.ConnectionProperties.ConnectString;
                            ValidateDataSetQuery();
                        }
                        else
                        {
                            MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxGenerateQuery"), this.error_title);
                        }
                    }

                    this.UpdateFieldsCollection();
                }
                else
                {
                    MessageBox.Show("<Name:>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSpecifyValidName"), this.error_title, MessageBoxButton.OK);
                }
            }
            catch { }
        }

        private void UpdateFieldsCollection()
        {
            if (dataSetTables != null)
            {
                RDL.DOM.Fields fields = new RDL.DOM.Fields();
                for (int i = 0; i < dataSetTables.Columns.Count; i++)
                {
                    Field dataSetField = new Field();
                    dataSetField.Name = dataSetTables.Columns[i].ColumnName;
                    dataSetField.DataField = dataSetTables.Columns[i].ColumnName;
                    dataSetField.TypeName = dataSetTables.Columns[i].DataType.ToString();
                    fields.Add(dataSetField);
                }
                if (fields.Count > 0)
                {
                    tbl_filters.ModifyDataSetFields(fields);
                }
            }
        }

        void btn_QueryDesigner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmb_DataSources.Items.Count > 0)
                {
                    if (cmb_DataSources.SelectedItem == null)
                    {
                        MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxChooseDataSource"), this.error_title);
                    }
                    else
                    {
                        QueryDesigner queryDesigner = new QueryDesigner(this.Query.CommandText, this.DataSource, (bool)this.Storedprocedure.IsChecked, this.Designpanel.VisualStyle);
                        queryDesigner.Owner = Window.GetWindow(btn_QueryDesigner);
                        SkinStorage.SetVisualStyle(queryDesigner, SkinStorage.GetVisualStyle(queryDesigner.Owner));
                        if (queryDesigner.ShowDialog() == true)
                        {
                            this.txt_Query.Text = queryDesigner.QueryString;
                            this.Query.CommandText = queryDesigner.QueryString;
                            this.dataSetTables = queryDesigner.queryResult;
                            this.UpdateFieldsCollection();
                        }
                    }
                }
                else
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxCreateDataSource"), this.error_title);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, this.error_title);
            }
        }

        private DataTable GetStoredProcedureTable(string connectionstring, bool populateParams)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionstring);
                SqlCommand cmd = new SqlCommand(this.param_query.Text, conn);
                DataTable datatable = null;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                this.Query = new RDL.DOM.Query();
                ListofParameters = ListofProcedures[this.param_query.SelectedItem.ToString()];
                if (this.ListofParameters.Count > 0)
                {
                    this.Query.QueryParameters = new QueryParameters();
                }
                foreach (ProcedureParameter param in this.ListofParameters)
                {
                    if (!string.IsNullOrEmpty(param.ParameterName))
                    {
                        Syncfusion.RDL.DOM.ReportParameter reportParameter = new RDL.DOM.ReportParameter();
                        reportParameter.Name = param.ParameterName.Remove(0, 1);
                        reportParameter.Prompt = param.ParameterName.Remove(0, 1);
                        cmd.Parameters.Add(param.ParameterName, param.ParameterType);
                        reportParameter.DataType = param.ParameterType;

                        if (populateParams)
                        {
                            if (this.ReportParameters == null)
                            {
                                this.ReportParameters = new RDL.DOM.ReportParameters();
                            }

                            var parameters = from parameter in this.ReportParameters
                                             where parameter.Name.Equals(reportParameter.Name)
                                             select parameter;

                            if (this.Designpanel.ReportParameters != null)
                            {
                                parameters = from parameter in this.Designpanel.ReportParameters
                                             where parameter.Name.Equals(reportParameter.Name)
                                             select parameter;
                            }
                            if (parameters.Count() == 0)
                            {
                                this.ReportParameters.Add(reportParameter);
                            }
                        }
                        QueryParameter queryParameter = new QueryParameter();
                        queryParameter.Name = param.ParameterName;
                        queryParameter.Value = "=Parameters!" + reportParameter.Name + ".Value";
                        this.Query.QueryParameters.Add(queryParameter);
                    }
                }

                this.Query.CommandText = this.param_query.Text;
                this.Query.CommandType = RDL.DOM.CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    datatable = new DataTable();
                    datatable.Load(reader);
                }
                if (datatable.Rows.Count == 0 && this.Query.QueryParameters != null && this.Query.QueryParameters.Count > 0)
                {
                    bool success = false;
                    bool validatePrevious = string.Equals(this.param_query.Text, this.OldProcedure);
                    if (validatePrevious && populateParams)
                    {
                        try
                        {
                            if (Old_ProcedureParameters.Count != this.Query.QueryParameters.Count)
                            {
                                validatePrevious = false;
                            }
                            else
                            {
                                validatePrevious = (from param in this.Query.QueryParameters
                                                    where !Old_ProcedureParameters.ContainsKey(param.Name)
                                                    select param).Count() == 0;
                            }
                        }
                        catch
                        {
                            validatePrevious = false;
                        }
                        if (validatePrevious)
                        {
                            cmd.Parameters.Clear();
                            for (int i = 0; i < this.Query.QueryParameters.Count; i++)
                            {
                                cmd.Parameters.AddWithValue(this.Query.QueryParameters[i].Name, Old_ProcedureParameters[this.Query.QueryParameters[i].Name]);
                            }
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                datatable.Load(reader);
                                if (datatable.Rows.Count == 0)
                                {
                                    validatePrevious = false;
                                }
                            }
                        }
                        if (validatePrevious)
                        {
                            this.dataSetTables = datatable;
                            return datatable;
                        }
                    }
                    do
                    {
                        ParameterQuery queryParams = new ParameterQuery();
                        queryParams.Owner = this;
                        SkinStorage.SetVisualStyle(queryParams, SkinStorage.GetVisualStyle(this));
                        int row = 1;

                        foreach (var parameter in this.Query.QueryParameters)
                        {
                            StackPanel panel = new StackPanel();
                            panel.Height = 25;
                            panel.Width = 300;
                            panel.Orientation = System.Windows.Controls.Orientation.Horizontal;
                            TextBlock block = new TextBlock();
                            block.Height = 20;
                            block.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            block.Width = 150;
                            block.Text = parameter.Name;
                            block.TextAlignment = System.Windows.TextAlignment.Center;
                            panel.Children.Add(block);

                            ComboBox box = new ComboBox();
                            box.Width = 130;
                            box.Height = 20;
                            box.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            box.Name = "combobox" + row;
                            box.Items.Add("null");
                            box.IsEditable = true;
                            panel.Children.Add(box);

                            try
                            {
                                if (Old_ProcedureParameters != null)
                                {
                                    box.Text = Old_ProcedureParameters[parameter.Name];
                                }
                            }
                            catch { }

                            queryParams.Mainpanel.Children.Add(panel);
                            row++;
                        }
                        if (queryParams.ShowDialog() == true)
                        {
                            cmd.Parameters.Clear();
                            for (int i = 0; i < this.Query.QueryParameters.Count; i++)
                            {
                                cmd.Parameters.AddWithValue(this.Query.QueryParameters[i].Name, queryParams.ListText[i]);
                            }
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                datatable.Load(reader);

                                if (datatable.Rows.Count != 0)
                                {
                                    success = true;
                                    Old_ProcedureParameters = new Dictionary<string, string>();

                                    for (int i = 0; i < this.Query.QueryParameters.Count; i++)
                                    {
                                        Old_ProcedureParameters.Add(this.Query.QueryParameters[i].Name, queryParams.ListText[i]);
                                    }
                                }
                                else
                                {
                                    MessageBoxResult result = MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxFieldNotPopulated"), this.error_title, MessageBoxButton.YesNo);
                                    if (result == MessageBoxResult.Yes)
                                    {
                                        success = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            success = true;
                        }
                        //try
                        //{
                        //    string command = string.Format("SELECT OBJECT_NAME(object_id), OBJECT_DEFINITION(object_id) FROM sys.procedures WHERE OBJECT_DEFINITION(object_id) LIKE '%{0}%'", this.param_query.Text);
                        //    SqlCommand procCmd = new SqlCommand(command, conn);
                        //    SqlDataReader reader = procCmd.ExecuteReader();
                        //    datatable.Load(reader);

                        //    if (datatable.Rows.Count > 0 && string.Equals(datatable.Rows[0][0].ToString(), this.param_query.Text))
                        //    {
                        //        datatable = GetProcedureQueryTable(datatable.Rows[0][1].ToString(), procCmd);
                        //    }
                        //}
                        //catch { }
                    } while (!success);
                }
                if (populateParams && this.ReportParameters != null && this.ReportParameters.Count > 0)
                {
                    this.Designpanel.RaiseParameterCollectionModifiedEvent();
                }

                this.dataSetTables = datatable;
                return populateParams ? datatable : null;
            }
            catch(Exception e)
            {
                this.ReportParameters = null;
                return null;
                throw e;
            }
        }

        private DataTable GetProcedureQueryTable(string query, SqlCommand command)
        {
            Regex pattern = new Regex(@"(?i)(begin|select|return|if|else|else if|end|case)");
            DataTable table = new DataTable();
            SqlDataReader reader;
            Match match = pattern.Match(query);
            bool success = false;
            string subquery = "";

            do
            {
                //subquery = query;
                subquery = query.Substring(match.Index + match.Length);
                //query = subquery;

                while (match.Success && !success)
                {
                    match = pattern.Match(subquery);

                    switch (match.Value.ToLower())
                    {
                        case "if":
                        case "else if":
                        case "begin":
                            subquery = subquery.Substring(match.Index + match.Length);
                            break;
                        case "select":
                            subquery = subquery.Substring(match.Index);
                            subquery = subquery.TrimStart();
                            Match sub = Regex.Match(subquery, "(?i)(;|end|\n|\r)");

                            if (sub.Success)
                            {
                                subquery = subquery.Substring(0, sub.Index);
                                success = true;
                            }
                            break;
                        case "case":
                            {
                                Match caseMatch = Regex.Match(subquery, "(?i)(then)");
                                subquery = subquery.Substring(caseMatch.Index + caseMatch.Length);
                            }
                            break;
                    }
                }
                if (success)
                {
                    try
                    {
                        command.CommandText = subquery;
                        reader = command.ExecuteReader();
                        table.Load(reader);
                    }
                    catch
                    {
                    }
                }
            } while (!success);

            return table;
        }

        private void UpdateStoredProcedure()
        {
            try
            {
                this.ConnectionString = this.DataSource.ConnectionProperties.ConnectString;
                if (!this.DataSource.ConnectionProperties.IntegratedSecurity)
                {
                    if (this.DataSource.ConnectionProperties.UserName == null)
                    {
                        this.Credentials = new Credentials(this.DataSource);
                        this.Designpanel.UpdateOwnerWindow(this.Credentials);
                        SkinStorage.SetVisualStyle(this.Credentials, SkinStorage.GetVisualStyle(this.Designpanel));

                        if (this.Credentials.ShowDialog() == true)
                        {
                            this.ConnectionString += ";" + ConnectionConstants.UserID + "=" + this.DataSource.ConnectionProperties.UserName;
                            this.ConnectionString += ";" + ConnectionConstants.Password + "=" + this.DataSource.ConnectionProperties.PassWord;
                        }
                    }
                    else
                    {
                        this.ConnectionString += ";" + ConnectionConstants.UserID + "=" + this.DataSource.ConnectionProperties.UserName;
                        this.ConnectionString += ";" + ConnectionConstants.Password + "=" + this.DataSource.ConnectionProperties.PassWord;
                    }

                    this.param_query.ItemsSource = this.LoadProcedure(this.ConnectionString);
                }
                else
                {
                    this.param_query.ItemsSource = this.LoadProcedure(this.ConnectionString + "; Trusted_Connection=true");
                }
            }
            catch { }
        }

        public List<string> LoadProcedure(String Con)
        {
            List<string> procedures = new List<string>();
            ListofProcedures = new Dictionary<string, List<ProcedureParameter>>();

            try
            {
                SqlConnection connection = new SqlConnection(Con);
                connection.Open();

                DataTable procedureDataTable = connection.GetSchema("Procedures");
                DataColumn procedureDataColumn = procedureDataTable.Columns["ROUTINE_NAME"];

                if (procedureDataColumn != null)
                {
                    foreach (System.Data.DataRow row in procedureDataTable.Rows)
                    {
                        String procedureName = row[procedureDataColumn].ToString();
                        procedures.Add(procedureName);
                        string parmName = null;
                        string parmType = null;
                        List<ProcedureParameter> parameters = new List<ProcedureParameter>();
                        DataTable parmsDataTable = connection.GetSchema("ProcedureParameters", new string[] { null, null, procedureName });

                        DataColumn parmNameDataColumn = parmsDataTable.Columns["PARAMETER_NAME"];
                        DataColumn parmTypeDataColumn = parmsDataTable.Columns["DATA_TYPE"];

                        foreach (System.Data.DataRow parmRow in parmsDataTable.Rows)
                        {
                            parmName = parmRow[parmNameDataColumn].ToString();
                            parmType = parmRow[parmTypeDataColumn].ToString();
                            DataTypes datatype;
                            switch (parmType.ToLower())
                            {
                                case "int":
                                    {
                                        datatype = DataTypes.Integer;
                                        break;
                                    }
                                case "datetime":
                                    {
                                        datatype = DataTypes.DateTime;
                                        break;
                                    }
                                default:
                                    {
                                        datatype = DataTypes.String;
                                        break;
                                    }
                            }
                            ProcedureParameter param = new ProcedureParameter(parmName, datatype);
                            parameters.Add(param);
                        }
                        ListofProcedures.Add(procedureName, parameters);
                    }
                }
            }
            catch { }

            return procedures;
        }



        void btn_DataSetCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.txt_Query.ToString() != string.Empty)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        void btn_DataSetOk_Click(object sender, RoutedEventArgs e)
        {
            if (!this.UpdateDataSetFilters())
            {
                return;
            }
            if (((this.Text.IsChecked == true && this.old_Query != this.txt_Query.Text) || (this.Storedprocedure.IsChecked == true)
                || this.old_datasource != this.cmb_DataSources.Text || this.txt_DataSetName.Text != this.old_datasetname) || this.txt_DataSetName.Text != this.old_datasetname)
            {
                if (this.cmb_DataSources.SelectedItem != null)
                {
                    if (Common.Util.CheckNameWithRE(this.txt_DataSetName.Text.ToString()))
                    {
                        string[] availableDatasetNames = null;

                        availableDatasetNames = (from dset in this.DataSets
                                                 select dset.Name).ToArray<string>();

                        if (Common.Util.CheckNameWithPreviousCollection(this.txt_DataSetName.Text.ToString(), availableDatasetNames) || isModifyDataset)
                        {
                            this.DataSet.Name = this.txt_DataSetName.Text;
                            if (!string.IsNullOrEmpty(this.txt_Query.Text) || this.Storedprocedure.IsChecked == true)
                            {
                                if (this.Text.IsChecked == true)
                                {
                                    this.Query.CommandText = this.txt_Query.Text.ToString();
                                    this.QueryString = this.txt_Query.Text.ToString();
                                }
                                this.ConnectionString = this.DataSource.ConnectionProperties.ConnectString;

                                DataTable dataTable = new DataTable();

                                try
                                {
                                    if (this.Query.QueryParameters == null)
                                    {
                                        this.Query.QueryParameters = new QueryParameters();
                                    }

                                    switch (this.DataSource.ConnectionProperties.DataProvider)
                                    {
                                        case DataProviders.SQLServer:
                                        case DataProviders.SQLAzure:
                                            {
                                                if (!this.DataSource.ConnectionProperties.IntegratedSecurity && this.OldQueryString != this.txt_Query.Text && this.DataSource.ConnectionProperties.UserName == null)
                                                {
                                                    this.Credentials = new Credentials(this.DataSource);
                                                    this.Credentials.Owner = this;
                                                    SkinStorage.SetVisualStyle(this.Credentials, SkinStorage.GetVisualStyle(this));
                                                    if (this.Credentials.ShowDialog() == true)
                                                    {
                                                        this.ConnectionString += ";" + ConnectionConstants.UserID + "=" + this.DataSource.ConnectionProperties.UserName;
                                                        this.ConnectionString += ";" + ConnectionConstants.Password + "=" + this.DataSource.ConnectionProperties.PassWord;
                                                    }

                                                    if (this.Credentials.isCancel)
                                                    {
                                                        this.Credentials.isCancel = false;
                                                        MessageBoxResult result = MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxInvalidQuery"), this.error_title, MessageBoxButton.OKCancel);
                                                        if (result == MessageBoxResult.OK)
                                                        {
                                                            this.DataSet.Query = this.Query;
                                                            this.DialogResult = false;
                                                            this.isModifyDataset = false;
                                                            this.Close();
                                                            return;
                                                        }
                                                        else
                                                        {
                                                            return;
                                                        }
                                                    }
                                                }
                                                else if (!this.DataSource.ConnectionProperties.IntegratedSecurity && this.OldQueryString == this.txt_Query.Text)
                                                {
                                                    this.Close();
                                                    return;
                                                }

                                                if ((!this.DataSource.ConnectionProperties.IntegratedSecurity && this.OldQueryString != this.txt_Query.Text && this.DataSource.ConnectionProperties.UserName != null))
                                                {
                                                    this.ConnectionString += ";" + ConnectionConstants.UserID + "=" + this.DataSource.ConnectionProperties.UserName.ToString();
                                                    this.ConnectionString += ";" + ConnectionConstants.Password + "=" + this.DataSource.ConnectionProperties.PassWord.ToString();
                                                }

                                                if (this.DataSource.ConnectionProperties.IntegratedSecurity)
                                                {
                                                    this.ConnectionString += "; Trusted_Connection=true";
                                                }

                                                try
                                                {
                                                    dataTable = GetSqlTable();
                                                }
                                                catch (Exception excep)
                                                {
                                                    MessageBoxResult result = MessageBox.Show(excep.Message + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxInvalidQuery"), this.error_title, MessageBoxButton.OKCancel);
                                                    if (result == MessageBoxResult.OK)
                                                    {
                                                        this.DataSet.Query = this.Query;
                                                        this.DialogResult = false;
                                                        this.isModifyDataset = false;
                                                        this.Close();
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        return;
                                                    }
                                                }
                                            }
                                            break;
                                        case DataProviders.ORACLE:
                                            {
                                                dataTable = GetOracleTable();
                                            }
                                            break;

                                        case DataProviders.ODBC:
                                            {
                                                if (this.DataSource.ConnectionProperties.UserName != string.Empty && this.DataSource.ConnectionProperties.PassWord != string.Empty)
                                                {
                                                    this.ConnectionString += "Uid=" + this.DataSource.ConnectionProperties.UserName + ";Pwd=" + this.DataSource.ConnectionProperties.PassWord;
                                                }

                                                dataTable = GetOdbcTable();
                                            }
                                            break;
                                        case DataProviders.OLEDB:
                                            {
                                                if (this.DataSource.ConnectionProperties.IntegratedSecurity == true)
                                                {
                                                    this.ConnectionString += ";Trusted_Connection=yes";
                                                }
                                                if (!this.DataSource.ConnectionProperties.IntegratedSecurity && this.DataSource.ConnectionProperties.UserName != string.Empty && this.DataSource.ConnectionProperties.PassWord != string.Empty)
                                                {
                                                    this.ConnectionString += ";Uid=" + this.DataSource.ConnectionProperties.UserName + "; Pwd=" + this.DataSource.ConnectionProperties.PassWord;
                                                }
                                                if (!this.DataSource.ConnectionProperties.IntegratedSecurity)
                                                {
                                                    this.ConnectionString = this.DataSource.ConnectionProperties.ConnectString;
                                                }

                                                dataTable = GetOledbTable();
                                            }
                                            break;
                                        case DataProviders.XML:
                                            {
                                                dataTable = GetXmlTable();
                                            }
                                            break;
                                        case DataProviders.SQLServerCe:
                                            {
                                                dataTable = GetSqlceTable();
                                            }
                                            break;
                                    }


                                    this.Query.DataSourceName = this.DataSource.Name;

                                    if (this.Fields == null)
                                    {
                                        this.Fields = new RDL.DOM.Fields();
                                    }

                                    this.Fields.Clear();

                                    for (int i = 0; i < dataTable.Columns.Count; i++)
                                    {
                                        Field dataSetField = new Field();
                                        dataSetField.Name = dataTable.Columns[i].ColumnName;
                                        dataSetField.DataField = dataTable.Columns[i].ColumnName;
                                        dataSetField.TypeName = dataTable.Columns[i].DataType.ToString();
                                        this.Fields.Add(dataSetField);
                                    }

                                    this.DataSet.Query = this.Query;
                                    this.DataSet.Fields = this.Fields;
                                    this.DialogResult = true;
                                    this.isModifyDataset = false;
                                    this.Close();
                                }

                                catch (Exception excep)
                                {
                                    MessageBox.Show(excep.Message.ToString(), this.error_title);
                                }
                            }
                            else
                            {
                                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture,"msgBoxGenerateQuery"), this.error_title);
                            }
                        }
                        else
                        {
                            MessageBox.Show("<Name:>\n" + SR.GetString(CultureInfo.CurrentUICulture,"msgBoxDataSetExist"), this.error_title, MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("<Name:>\n" + SR.GetString(CultureInfo.CurrentUICulture,"msgBoxSpecifyValidName"), this.error_title, MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(this.cmb_DataSources.Text))
                {
                    MessageBox.Show("<DataSource:>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectDataSource"), this.error_title, MessageBoxButton.OK);
                }
                else
                {
                    this.Close();
                }
            }
        }

        private bool UpdateDataSetFilters()
        {
            List<Grid> Panels = tbl_filters.lbx_Filters.Items.OfType<Grid>().ToList();
            RDL.DOM.Filters Filters = new RDL.DOM.Filters();
            bool isValidValue = true;

            foreach (var panel in Panels)
            {
                List<ComboBox> expression = panel.Children.OfType<ComboBox>().ToList();
                System.Windows.Controls.TextBox txt_value = panel.Children.OfType<System.Windows.Controls.TextBox>().ToList().FirstOrDefault();

                if (!string.IsNullOrEmpty(expression.First().Text))
                {
                    if (!string.IsNullOrEmpty(txt_value.Text))
                    {
                        string temp = txt_value.Text;

                        switch (expression.ElementAt(1).Text)
                        {
                            case "Integer":
                                isValidValue = (System.Text.RegularExpressions.Regex.IsMatch(temp, "^[-+]?[0-9]+$") == true) ? true : false;
                                temp = "Value is not an Integer.";
                                break;

                            case "Float":
                                isValidValue = (System.Text.RegularExpressions.Regex.IsMatch(temp, @"^[-+]?[0-9]*\.?[0-9]+$") == true) ? true : false;
                                temp = "Value is not Float.";
                                break;

                            case "DateTime":
                                isValidValue = (System.Text.RegularExpressions.Regex.IsMatch(temp,
                                                     @"^((0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|2\d)\d\d)\s*((0?[0-9]|1[0-9]|2[0-3])[:|.][0-5]?[\d])?\s*(([Aa]|[Pp])[Mm])?\s*$") == true) ? true : false;
                                temp = "Value is not Data Time.";
                                break;

                            case "Boolean":
                                if (temp.ToLower().Equals("true") || temp.ToLower().Equals("false"))
                                {
                                    isValidValue = true;
                                }
                                else
                                {
                                    isValidValue = false;
                                    temp = "Value is not Boolean.";
                                }
                                break;

                            case "String":
                                isValidValue = true;
                                break;
                        }
                        if (isValidValue)
                        {
                            RDL.DOM.FilterOperators opr = new RDL.DOM.FilterOperators();

                            switch (expression.Last().Text)
                            {
                                case ">":
                                    opr = RDL.DOM.FilterOperators.GreaterThan;
                                    break;
                                case "<":
                                    opr = RDL.DOM.FilterOperators.LessThan;
                                    break;
                                case "<=":
                                    opr = RDL.DOM.FilterOperators.LessThanOrEqual;
                                    break;
                                case ">=":
                                    opr = RDL.DOM.FilterOperators.GreaterThanOrEqual;
                                    break;
                                case "In":
                                    opr = RDL.DOM.FilterOperators.In;
                                    break;
                                case "Like":
                                    opr = RDL.DOM.FilterOperators.Like;
                                    break;
                                case "<>":
                                    opr = RDL.DOM.FilterOperators.NotEqual;
                                    break;
                                case "Top N":
                                    opr = RDL.DOM.FilterOperators.TopN;
                                    break;
                                case "Bottom N":
                                    opr = RDL.DOM.FilterOperators.BottomN;
                                    break;
                                case "Between":
                                    opr = RDL.DOM.FilterOperators.Between;
                                    break;
                                case "Bottom %":
                                    opr = RDL.DOM.FilterOperators.BottomPercent;
                                    break;
                                case "Top %":
                                    opr = RDL.DOM.FilterOperators.TopPercent;
                                    break;
                            }
                            
                            FilterValues values = new FilterValues();
                            values.Add(new FilterValue() { DataType = (RDL.DOM.DataTypes)expression.ElementAt(1).SelectedItem, Value = txt_value.Text });
                            Filters.Add(new Filter() { FilterExpression = ControlProperties.ConvertFieldToExpression(expression.First().Text), FilterValues = values, Operator = opr });
                        }
                        else
                        {
                            MessageBox.Show("<Value> \n" + temp, this.error_title, MessageBoxButton.OK);
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("<Value> \n " + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.error_title, MessageBoxButton.OK);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("<Expression> \n " + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxEnterValue"), this.error_title, MessageBoxButton.OK);
                    return false;
                }
            }
            if (isValidValue)
            {
                this.DataSet.Filters = Filters;
            }
            return true;
        }

        private void ValidateDataSetQuery()
        {
            try
            {
                if (this.DataSource.ConnectionProperties != null)
                {
                    switch (this.DataSource.ConnectionProperties.DataProvider)
                    {
                        case DataProviders.SQLServer:
                        case DataProviders.SQLAzure:
                            {
                                if (!this.DataSource.ConnectionProperties.IntegratedSecurity && this.OldQueryString != this.txt_Query.Text && this.DataSource.ConnectionProperties.UserName == null)
                                {
                                    this.Credentials = new Credentials(this.DataSource);
                                    this.Credentials.Owner = this;
                                    SkinStorage.SetVisualStyle(this.Credentials, SkinStorage.GetVisualStyle(this));
                                    if (this.Credentials.ShowDialog() == true)
                                    {
                                        this.ConnectionString += ";" + ConnectionConstants.UserID + "=" + this.DataSource.ConnectionProperties.UserName;
                                        this.ConnectionString += ";" + ConnectionConstants.Password + "=" + this.DataSource.ConnectionProperties.PassWord;
                                    }

                                    if (this.Credentials.isCancel)
                                    {
                                        this.Credentials.isCancel = false;
                                        MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxInvalidQuery"), this.error_title, MessageBoxButton.OK);
                                        return;
                                    }
                                }
                                else if (!this.DataSource.ConnectionProperties.IntegratedSecurity && this.OldQueryString == this.txt_Query.Text)
                                {
                                    return;
                                }

                                if ((!this.DataSource.ConnectionProperties.IntegratedSecurity && this.OldQueryString != this.txt_Query.Text && this.DataSource.ConnectionProperties.UserName != null))
                                {
                                    this.ConnectionString += ";" + ConnectionConstants.UserID + "=" + this.DataSource.ConnectionProperties.UserName.ToString();
                                    this.ConnectionString += ";" + ConnectionConstants.Password + "=" + this.DataSource.ConnectionProperties.PassWord.ToString();
                                }

                                if (this.DataSource.ConnectionProperties.IntegratedSecurity)
                                {
                                    this.ConnectionString += "; Trusted_Connection=true";
                                }

                                this.dataSetTables = GetSqlTable();
                            }
                            break;
                        case DataProviders.ORACLE:
                            {
                                this.dataSetTables = GetOracleTable();
                            }
                            break;

                        case DataProviders.ODBC:
                            {
                                if (this.DataSource.ConnectionProperties.UserName != string.Empty && this.DataSource.ConnectionProperties.PassWord != string.Empty)
                                {
                                    this.ConnectionString += "Uid=" + this.DataSource.ConnectionProperties.UserName + ";Pwd=" + this.DataSource.ConnectionProperties.PassWord;
                                }

                                this.dataSetTables = GetOdbcTable();
                            }
                            break;
                        case DataProviders.OLEDB:
                            {
                                if (this.DataSource.ConnectionProperties.IntegratedSecurity == true)
                                {
                                    this.ConnectionString += ";Trusted_Connection=yes";
                                }
                                if (!this.DataSource.ConnectionProperties.IntegratedSecurity && this.DataSource.ConnectionProperties.UserName != string.Empty && this.DataSource.ConnectionProperties.PassWord != string.Empty)
                                {
                                    this.ConnectionString += ";Uid=" + this.DataSource.ConnectionProperties.UserName + "; Pwd=" + this.DataSource.ConnectionProperties.PassWord;
                                }
                                if (!this.DataSource.ConnectionProperties.IntegratedSecurity)
                                {
                                    this.ConnectionString = this.DataSource.ConnectionProperties.ConnectString;
                                }

                                this.dataSetTables = GetOledbTable();
                            }
                            break;
                        case DataProviders.XML:
                            {
                                this.dataSetTables = GetXmlTable();
                            }
                            break;
                        case DataProviders.SQLServerCe:
                            {
                                this.dataSetTables = GetSqlceTable();
                            }
                            break;
                    }
                }
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxInvalidQuery"), this.error_title, MessageBoxButton.OK);
            }
        }

        private DataTable GetSqlceTable()
        {
            using (System.Data.SqlServerCe.SqlCeConnection connection = new System.Data.SqlServerCe.SqlCeConnection(this.ConnectionString))
            {
                connection.Open();
                DataTable dataTable = this.GetsqlceTable(this.QueryString, connection);
                return dataTable;
            }
        }

        private DataTable GetXmlTable()
        {
            XMLDataProvider xml = new XMLDataProvider();
            DataTable dataTable = xml.GetTable(this.ConnectionString, this.QueryString, "table");
            return dataTable;
        }

        private DataTable GetOledbTable()
        {
            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(this.ConnectionString))
            {
                conn.Open();
                DataTable dataTable = new DataTable();
                string con = this.QueryString;
                System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(con, conn);
                System.Data.OleDb.OleDbDataReader dr = cmd.ExecuteReader();
                dataTable.Load(dr);
                return dataTable;
            }
        }

        private DataTable GetOdbcTable()
        {
            using (System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection(this.ConnectionString))
            {
                conn.Open();
                DataTable dataTable = new DataTable();
                string con = this.QueryString;
                System.Data.Odbc.OdbcCommand cmd = new System.Data.Odbc.OdbcCommand(con, conn);
                System.Data.Odbc.OdbcDataReader dr = cmd.ExecuteReader();
                dataTable.Load(dr);
                return dataTable;
            }
        }

        private DataTable GetSqlTable()
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                DataTable dataTable = new DataTable();

                connection.Open();
                SqlSchemaProvider sqlSchemaProvider = new SqlSchemaProvider(connection);

                #region Parameterized Query for SQLServer
                if (this.Text.IsChecked == true && this.QueryString.Contains("@"))
                {
                    dataTable = GetParameterizedTable(this.Query);
                }
                #endregion

                #region Plain Query for SQLServer
                else if (this.Text.IsChecked == true)
                {
                    dataTable = sqlSchemaProvider.GetQueryTables(this.QueryString);
                }
                else
                {
                    dataTable = this.GetStoredProcedureTable(this.ConnectionString, true);
                }
                #endregion

                dataTable.TableName = "QueryResults"; //Setting TableName as QueryResults

                return dataTable;
            }
        }

        private DataTable GetOracleTable()
        {
            using (System.Data.OracleClient.OracleConnection connection = new System.Data.OracleClient.OracleConnection(this.ConnectionString))
            {
                connection.Open();
                DataTable dataTable = new DataTable();
                OracleSchemaProvider sqlSchemaProvider = new OracleSchemaProvider(connection);

                #region Parameterized Query for Oracle
                if (this.QueryString.Contains("@"))
                {
                    dataTable = GetParameterizedTable(this.Query);
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
        }


        /// <summary>
        /// Get Parameterized Fileds from query
        /// </summary>
        /// <param name="query">Represents the Query from RDL Specification</param>
        /// <returns>Containig as DataTable</returns>
        private DataTable GetParameterizedTable(Query query)
        {
            // find any Token for Parameterized Query followed by '@'  Or ':' Symbol
            Regex theReg = new Regex(@"(\@)([a-zA-Z0-9]+)", RegexOptions.IgnoreCase);

            switch (this.DataSource.ConnectionProperties.DataProvider)
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

            //Check whether the parameters in base already

            if (query.QueryParameters != null && query.QueryParameters.Count >0 )
            {
                //If exsist, list the parameters
                foreach (QueryParameter qp in query.QueryParameters)
                {
                    try
                    {
                        _eliminateDuplicatesIgnoreCase.Add(qp.Name.ToLower(),qp.Value);
                    }
                    catch
                    {
                    }
                }
            }

            // iterate through the collection
            foreach (Match theMatch in theMatches)
            {
                if (theMatch.Length != 0)
                {
                    char[] delimetersToTrim = {' ', '@', ':'};
                    try
                    {
                        _eliminateDuplicatesIgnoreCase.Add(theMatch.ToString().ToLower(), "");
                        QueryParameter queryParameter = new QueryParameter();
                        queryParameter.Name = theMatch.ToString();
                        queryParameter.Value = "=Parameters!" + theMatch.ToString().Trim(delimetersToTrim) + ".Value";
                        query.QueryParameters.Add(queryParameter);

                        Syncfusion.RDL.DOM.ReportParameter reportParameter = new RDL.DOM.ReportParameter();
                        reportParameter.Name = theMatch.ToString().Trim(delimetersToTrim);
                        reportParameter.Prompt = theMatch.ToString().Trim(delimetersToTrim);

                        if (this.ReportParameters == null)
                        {
                            this.ReportParameters = new RDL.DOM.ReportParameters();
                        }

                        var parameters = from parameter in this.ReportParameters
                                         where parameter.Name.Equals(reportParameter.Name)
                                         select parameter;

                        if (this.Designpanel.ReportParameters != null)
                        {
                            parameters = from parameter in this.Designpanel.ReportParameters
                                         where parameter.Name.Equals(reportParameter.Name)
                                         select parameter;
                        }
                        if (parameters.Count() == 0)
                        {
                            this.ReportParameters.Add(reportParameter);
                            this.Designpanel.RaiseParameterCollectionModifiedEvent();
                        }
                    }
                    catch

                    {
                    }
                }

            }
            switch (this.DataSource.ConnectionProperties.DataProvider)
            {
                case DataProviders.SQLServer:
                case DataProviders.SQLAzure:
                    {
                        return GetSqlCommeandResult(query);
                    }
                case DataProviders.ORACLE:
                    {
                        return GetOracleCommandResult(query);
                    }
            } 

            return null;
        }

        private DataTable GetOracleCommandResult(RDL.DOM.Query query)
        {
            System.Data.OracleClient.OracleCommand orclCmd = new System.Data.OracleClient.OracleCommand(query.CommandText);

            if (query.QueryParameters.Count > 0)
            {
                foreach (QueryParameter qP in query.QueryParameters)
                {
                    orclCmd.Parameters.AddWithValue(qP.Name, string.Empty);
                }
            }
            return new OracleDataProvider().GetTable(this.DBConnection, orclCmd);
        }

        private DataTable GetSqlCommeandResult(Query query)
        {
            SqlCommand sqlCmd = new SqlCommand(query.CommandText);
            if (query.QueryParameters.Count > 0)
            {
                foreach (QueryParameter qP in query.QueryParameters)
                {
                    sqlCmd.Parameters.AddWithValue(qP.Name, string.Empty);
                }
            }
            return new SqlDataProvider().GetTable(this.DBConnection, sqlCmd);
        }

        public DataTable GetsqlceTable(string query, DbConnection connection)
        {
            using (System.Data.SqlServerCe.SqlCeCommand command = new System.Data.SqlServerCe.SqlCeCommand(query, (System.Data.SqlServerCe.SqlCeConnection)connection))
            {
                using (System.Data.SqlServerCe.SqlCeDataReader sqlDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(sqlDataReader);
                    return table;
                }
            }
        }


        void btn_NewDataSource_Click(object sender, RoutedEventArgs e)
        {
            this.Designpanel.AddDataSource();

            if (this.DataSource == null && this.DataSources.Count() > 0)
            {
                this.DataSource = this.DataSources.First();
            }
            else if (this.cmb_DataSources.Items.Count != this.DataSources.Count())
            {
                this.DataSource = this.DataSources.Last();
            }

            this.cmb_DataSources.Items.Clear();
            this.IntializeDataSetUI();
        }

        private void cmb_DataSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmb_DataSources.SelectedItem != null)
            {
                if (FindDataSource(this.cmb_DataSources.SelectedItem.ToString()) != null)
                {
                    this.DataSource = FindDataSource(this.cmb_DataSources.SelectedItem.ToString());
                }

                this.UpdateQueryDesignerButtonStatus();

                if (this.Storedprocedure.IsChecked == true)
                {
                    try
                    {
                        string selected = this.param_query.Text;
                        this.param_query.ItemsSource = null;
                        this.ListofProcedures = null;
                        UpdateStoredProcedure();
                        this.param_query.Text = selected;
                        this.param_query.SelectedItem = selected;
                    }
                    catch { }
                }
            }
        }

        #endregion
    }

   class ProcedureParameter
   {
       public ProcedureParameter(String Name,DataTypes Type)
       {
           this.ParameterName = Name;
           this.ParameterType = Type;
       }

       public string ParameterName
       {
           get;
           set;
       }

       public DataTypes ParameterType
       {
           get;
           set;
       }
   }
}
