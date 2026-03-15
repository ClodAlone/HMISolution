//-------------------------------------------------------------------------------------------------
// <copyright file="Reports.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.RDL.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Syncfusion.RDL.DOM;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Collections;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Syncfusion.RDL.Internal;
    using Syncfusion.RDL.Data;

#if !WINRT
    using Syncfusion.Linq;
    using Syncfusion.Windows.Data;
    using System.Windows.Data;
#endif

#if WINRT
    using Syncfusion.UI.Xaml.Reports;
    using Syncfusion.Reports.Server;
#elif SILVERLIGHT
    using Syncfusion.Windows.Reports;
    using Syncfusion.Reports.Server;
#elif MVC
    using System.Data.SqlClient;
    using System.Data.OracleClient;
    using System.Data.OleDb;
    using System.Data.Odbc;
    using System.Data.SqlServerCe;
    using System.Data;
    using System.Data.Common;
    using System.Xml.Linq;
    using Syncfusion.Reports.Mvc;
#else
    using Syncfusion.Windows.Reports;
    using System.Data.SqlClient;
    using System.Data.OracleClient;
    using System.Data.OleDb;
    using System.Data.Odbc;
    using System.Data.SqlServerCe;
    using System.Data;
    using System.Data.Common;
    using System.Xml.Linq;
#endif

    internal class ReportData
    {
        public Dictionary<string, object> Data { get; set; }
    }

    /// <summary>
    /// It holds the processed data.
    /// </summary>
    [ObsoleteAttribute("OracleCommand has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    internal class ProcessedData
    {
        #region Members

        private ReportDefinition reports;
        private ReportModel model;
        private List<DataSetInformations> dataSetCollection;
        private List<DataSetInformations> parameterDataSetCollection;
        private List<DataSetInformations> reportDataSets;
        private int reportDatasetIndex = -1;

        #endregion

        #region Public Properties

        public ExpressionEngine ExpressionEngine
        {
            get;
            set;
        }

        public Exception Exception
        {
            get;
            set;
        }

        public bool HasException
        {
            get
            {
                return this.Exception != null;
            }
        }

        /// <summary>
        /// Gets or sets CollectionViewObjects as Dictionary of CollectionViewAdv where Key represent name as strig and Value represents the ICollectionViewAdv
        /// </summary>
        public Dictionary<string, IEnumerable> DataSourceObjects
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ReportSetting type
        /// </summary>
        public ReportDefinition Report
        {
            get { return this.reports; }
            set { this.reports = value; }
        }

        #endregion

        internal Dictionary<string, List<ExpFilter>> ExpFilters { get; set; }

        internal event DataSourceUpdatedEventHandler DataSourceUpdated;

        internal static IEnumerable<DataSourceCredentials> dataSourceCredentials;
#if !WINRT
        internal DataSourceCredentials[] dataSourceCredential = null;
#endif

        protected virtual void RaiseDataSourceUpdatedEvent(EventArgs e)
        {
            if (DataSourceUpdated != null)
            {
                DataSourceUpdated(this, e);
            }
        }

        #region Constructor

        public ProcessedData(ReportModel model)
        {
            this.model = model;
            this.Report = model.Report;
            this.ExpressionEngine = model.ExpressionEngine;
            List<string> dataSetCollection = new List<string>();
            List<string> parameterDataSetCollection = new List<string>();
            List<string> paramDataSetCollection = new List<string>();

            foreach (var dataSet in model.DataSetDetails)
            {
                dataSetCollection.Add(dataSet.Name);
            }

            foreach (var parameter in model.ParameterDetails)
            {
                foreach (var parDataSet in parameter.DependentDataSets)
                {
                    if (!paramDataSetCollection.Contains(parDataSet))
                    {
                        paramDataSetCollection.Add(parDataSet);
                    }
                }
            }

            foreach (var dataSet in dataSetCollection)
            {
                if (paramDataSetCollection.Contains(dataSet))
                {
                    parameterDataSetCollection.Add(dataSet);
                }
            }

            this.parameterDataSetCollection = this.GetDataSets(parameterDataSetCollection);
            this.dataSetCollection = this.GetDataSets(dataSetCollection);

            this.ResetProceesedData();
            this.ResetDocumentModel();
        }

        #endregion

        #region Helper Methods

        public void ResetProceesedData()
        {
            this.Exception = null;
            this.DataSourceObjects = new Dictionary<string, IEnumerable>();
        }

        public void ResetDocumentModel()
        {
            if (this.model.MapModel != null && this.model.MapModel.NodeData.Count > 0)
            {
                this.model.MapModel.NodeData.Clear();
                this.model.MapModel = null;
            }
            this.model.MapModel = null;
        }

        public void UpdateData(object itemSource)
        {
            this.InitilizeParameterValues();

            try
            {
                this.UpdateDataset(itemSource, this.dataSetCollection);
            }
            catch (Exception ex)
            {
                this.Exception = ex;
            }
        }

        internal void UpdateParameterDatasets(object itemSource)
        {
            this.ResetProceesedData();
            try
            {
                if (this.parameterDataSetCollection.Count > 0)
                {
                    this.UpdateDataset(itemSource, this.parameterDataSetCollection);
                }
            }
            catch { }
        }

        internal void InitilizeDatasetParameterValues(IEnumerable<string> parameters)
        {
            var reportParamsCollection = from reportParam in this.Report.ReportParameters
                                         where (reportParam.ValidValues != null && reportParam.ValidValues.DataSetReference != null)
                                         select reportParam;

            foreach (var parameter in reportParamsCollection)
            {
                string valueMemberPath = parameter.ValidValues.DataSetReference.ValueField;
                string displayMemberPath = valueMemberPath;

                if (parameter.ValidValues.DataSetReference.LabelField != null)
                {
                    displayMemberPath = parameter.ValidValues.DataSetReference.LabelField;
                }

                var param = (from modelParamter in this.model.ParameterDetails
                             where (modelParamter.Name.Equals(parameter.Name))
                             select modelParamter).FirstOrDefault();

                var datas = from data in this.model.ProcessedData.DataSourceObjects
                            where (data.Key == parameter.ValidValues.DataSetReference.DataSetName)
                            select data;

                param.Label = new List<object>();

                if (datas.Count() > 0 && param.Value != null)
                {
                    IEnumerable m_itemSource = datas.First().Value;
                    List<string> values = new List<string>();

                    foreach (var value in param.Value)
                    {
                        values.Add(value.ToString());
                    }

                    if (values.Count == 0)
                    {
                        var defaultparam = (from modelParamter in this.model.ReportParameters
                                            where (modelParamter.Name.Equals(parameter.Name) && modelParamter.DefaultValue != null && modelParamter.DefaultValue.DataSetReference != null)
                                            select modelParamter).FirstOrDefault();
                        if (defaultparam != null)
                        {
                            foreach (ReportData data in m_itemSource)
                            {
                                object value = data.Data[valueMemberPath];
                                defaultparam.DefaultValue.Values.Add(value.ToString());
                                param.Value.Add(value);
                            }
                            values = defaultparam.DefaultValue.Values;
                        }

                    }

                    if (!this.model.IsRDLC)
                    {
                        foreach (ReportData data in m_itemSource)
                        {
                            object value = data.Data[valueMemberPath];
                            object labelValue = data.Data[displayMemberPath];

                            if (value != null && values.Contains(value.ToString()))
                            {
                                string label = (labelValue != null) ? labelValue.ToString() : value.ToString();
                                param.Label.Add(label);
                            }
                        }
                    }
#if SILVERLIGHT
                    else
                    {
                        System.Reflection.PropertyInfo valueProperty = null;
                        System.Reflection.PropertyInfo displayProperty = null;
                        System.Type objectType = null;

                        foreach (var data in m_itemSource)
                        {
                            if (objectType == null)
                            {
                                objectType = data.GetType();
#if WINRT
                                valueProperty = objectType.GetTypeInfo().DeclaredProperties.Where(info => info.Name == valueMemberPath).First();
                                displayProperty = objectType.GetTypeInfo().DeclaredProperties.Where(info => info.Name == displayMemberPath).First();
#else
                                valueProperty = objectType.GetProperty(valueMemberPath);
                                displayProperty = objectType.GetProperty(displayMemberPath);
#endif
                            }

                            object value = valueProperty.GetValue(data, null);
                            object labelValue = displayProperty.GetValue(data, null);

                            if (value != null && values.Contains(value.ToString()))
                            {
                                string label = (labelValue != null) ? labelValue.ToString() : value.ToString();
                                param.Label.Add(label);
                            }
                        }
                    }
#else
                    else
                    {
                        PropertyDescriptor valueProperty = null;
                        PropertyDescriptor displayProperty = null;

                        System.Type objectType = null;

                        if (objectType == null)
                        {
                            foreach (object o in m_itemSource)
                            {
                                objectType = o.GetType();
                                break;
                            }
                        }

                        if (objectType != null)
                        {
                            PropertyDescriptorCollection itemProperties = TypeDescriptor.GetProperties(objectType);
                            valueProperty = (from property in itemProperties.ToList<PropertyDescriptor>() where property.Name.Equals(valueMemberPath) select property).FirstOrDefault();
                            displayProperty = (from property in itemProperties.ToList<PropertyDescriptor>() where property.Name.Equals(displayMemberPath) select property).FirstOrDefault();
                        }


                        foreach (var data in m_itemSource)
                        {
                            object value = null;
                            object labelValue = null;

                            if (data is ReportData)
                            {
                                value = (data as ReportData).Data[valueMemberPath];
                                labelValue = (data as ReportData).Data[displayMemberPath];

                            }

                            else
                            {
                                value = valueProperty.GetValue(data);
                                labelValue = displayProperty.GetValue(data);
                            }

                            if (value != null && values.Contains(value.ToString()))
                            {
                                string label = (labelValue != null) ? labelValue.ToString() : value.ToString();
                                param.Label.Add(label);
                            }
                        }
                    }
#endif
                    if (param.Label.Count == 0)
                    {
                        param.Value = new List<object>();
                    }
                }
            }
        }

        internal void InitilizeDatasetDefaultParameterValues(IEnumerable<string> parameters)
        {
            var reportParamsCollection = from reportParam in this.Report.ReportParameters
                                         where (reportParam.DefaultValue != null && reportParam.DefaultValue.DataSetReference != null && (reportParam.DefaultValue.Values == null || (reportParam.DefaultValue.Values != null && reportParam.DefaultValue.Values.Count == 0)))
                                         select reportParam;

            foreach (var parameter in reportParamsCollection)
            {
                string valueMemberPath = parameter.DefaultValue.DataSetReference.ValueField;
                string displayMemberPath = valueMemberPath;

                var param = (from modelParamter in this.model.ParameterDetails
                             where (modelParamter.Name.Equals(parameter.Name))
                             select modelParamter).FirstOrDefault();

                var datas = from data in this.model.ProcessedData.DataSourceObjects
                            where (data.Key == parameter.DefaultValue.DataSetReference.DataSetName)
                            select data;

                param.Label = new List<object>();

                if (datas.Count() > 0 && param.Value != null)
                {
                    IEnumerable m_itemSource = datas.First().Value;
                    List<string> values = new List<string>();

                    foreach (var value in param.Value)
                    {
                        values.Add(value.ToString());
                    }

                    if (values.Count == 0)
                    {
                        var defaultparam = (from modelParamter in this.model.ReportParameters
                                            where (modelParamter.Name.Equals(parameter.Name) && modelParamter.DefaultValue != null && modelParamter.DefaultValue.DataSetReference != null)
                                            select modelParamter).FirstOrDefault();
                        if (defaultparam != null)
                        {
                            if (defaultparam.DefaultValue.Values == null)
                            {
                                defaultparam.DefaultValue.Values = new Values();
                            }
                            foreach (ReportData data in m_itemSource)
                            {
                                object value = data.Data[valueMemberPath];
                                defaultparam.DefaultValue.Values.Add(value.ToString());
                                param.Value.Add(value);
                            }
                            values = defaultparam.DefaultValue.Values;
                        }

                    }

                    if (!this.model.IsRDLC)
                    {
                        foreach (ReportData data in m_itemSource)
                        {
                            object value = data.Data[valueMemberPath];
                            object labelValue = data.Data[displayMemberPath];

                            if (value != null && values.Contains(value.ToString()))
                            {
                                string label = (labelValue != null) ? labelValue.ToString() : value.ToString();
                                param.Label.Add(label);
                            }
                        }
                    }
#if SILVERLIGHT
                    else
                    {
                        System.Reflection.PropertyInfo valueProperty = null;
                        System.Reflection.PropertyInfo displayProperty = null;
                        System.Type objectType = null;

                        foreach (var data in m_itemSource)
                        {
                            if (objectType == null)
                            {
                                objectType = data.GetType();
#if WINRT
                                valueProperty = objectType.GetTypeInfo().DeclaredProperties.Where(info => info.Name == valueMemberPath).First();
                                displayProperty = objectType.GetTypeInfo().DeclaredProperties.Where(info => info.Name == displayMemberPath).First();
#else
                                valueProperty = objectType.GetProperty(valueMemberPath);
                                displayProperty = objectType.GetProperty(displayMemberPath);
#endif
                            }

                            object value = valueProperty.GetValue(data, null);
                            object labelValue = displayProperty.GetValue(data, null);

                            if (value != null && values.Contains(value.ToString()))
                            {
                                string label = (labelValue != null) ? labelValue.ToString() : value.ToString();
                                param.Label.Add(label);
                            }
                        }
                    }
#else
                    else
                    {
                        PropertyDescriptor valueProperty = null;
                        PropertyDescriptor displayProperty = null;

                        System.Type objectType = null;

                        if (objectType == null)
                        {
                            foreach (object o in m_itemSource)
                            {
                                objectType = o.GetType();
                                break;
                            }
                        }

                        if (objectType != null)
                        {
                            PropertyDescriptorCollection itemProperties = TypeDescriptor.GetProperties(objectType);
                            valueProperty = (from property in itemProperties.ToList<PropertyDescriptor>() where property.Name.Equals(valueMemberPath) select property).FirstOrDefault();
                            displayProperty = (from property in itemProperties.ToList<PropertyDescriptor>() where property.Name.Equals(displayMemberPath) select property).FirstOrDefault();
                        }


                        foreach (var data in m_itemSource)
                        {
                            object value = null;
                            object labelValue = null;

                            if (data is ReportData)
                            {
                                value = (data as ReportData).Data[valueMemberPath];
                                labelValue = (data as ReportData).Data[displayMemberPath];

                            }

                            else
                            {
                                value = valueProperty.GetValue(data);
                                labelValue = displayProperty.GetValue(data);
                            }

                            if (value != null && values.Contains(value.ToString()))
                            {
                                string label = (labelValue != null) ? labelValue.ToString() : value.ToString();
                                param.Label.Add(label);
                            }
                        }
                    }
#endif
                    if (param.Label.Count == 0)
                    {
                        param.Value = new List<object>();
                    }
                }
            }
        }


        private void InitilizeParameterValues()
        {
            if (this.Report.ReportParameters == null)
            {
                return;
            }

            var reportParamsCollection = from reportParam in this.Report.ReportParameters
                                         where (reportParam.ValidValues != null && reportParam.ValidValues.DataSetReference == null)
                                         select reportParam;

            foreach (var reportParam in reportParamsCollection)
            {
                var param = (from parameter in this.model.ParameterDetails
                             where (parameter.Name.Equals(reportParam.Name))
                             select parameter).FirstOrDefault();

                if (param.Value != null)
                {
#if WINRT
                    IEnumerable<object> values = param.Value.ToList();
#else
                    IEnumerable<string> values = param.Value.ToList<string>();
#endif
                    var validValueCollection = from paramValue in reportParam.ValidValues.ParameterValues
                                               where (values.Contains(paramValue.Value))
                                               select paramValue;
                    param.Label.Clear();
                    foreach (var val in validValueCollection)
                    {
                        string label = (val.Label != null) ? val.Label : val.Value;
                        param.Label.Add(label);
                    }
                }
            }
        }

        internal void UpdateDataset(object itemsSource, IEnumerable<DataSetInformations> dataSets)
        {
            InitilizeParameterValues();
            this.DataSourceObjects = new Dictionary<string, IEnumerable>();

            ////Identifying the type and convert into ICollectionViewAdv type.
            if (itemsSource is ReportDataSourceCollection)
            {
                foreach (var dataSet in dataSets)
                {
                    var dataSources = from dataSrc in (itemsSource as ReportDataSourceCollection)
                                      where dataSrc.Name.Equals(dataSet.Name)
                                      select dataSrc;

                    if (dataSources.Count() > 0)
                    {
                        var dataSource = dataSources.First();
                        string dataSetName = dataSource.Name;
                        var dataSetInfo = this.model.DataSetDetails.Where(set => set.Name.Equals(dataSetName)).FirstOrDefault();
                        var paramSets = this.model.ParameterDetails.Where(param => dataSetInfo.DependentParameters.Contains(param.Name)).Select(param => param.Name);

                        if (paramSets.Count() > 0)
                        {
                            InitilizeDatasetParameterValues(paramSets);
                        }

                        object value = dataSource.Value;

                        if (value is IEnumerable)
                        {
                            this.DataSourceObjects.Add(dataSource.Name, this.ExpFilters.ContainsKey(dataSource.Name) ? this.FilterItemSoruce(value as IEnumerable, this.ExpFilters[dataSource.Name]) : value as IEnumerable);
                        }
#if !SILVERLIGHT && !WINRT
                        else if (value is DataTable)
                        {
                            this.DataSourceObjects.Add(dataSource.Name, this.ExpFilters.ContainsKey(dataSource.Name) ? this.FilterItemSoruce((value as DataTable).DefaultView as IEnumerable, this.ExpFilters[dataSource.Name]) : (value as DataTable).DefaultView);
                        }
                        else if (value is System.Data.DataSet)
                        {
                            this.DataSourceObjects.Add(dataSource.Name, this.ExpFilters.ContainsKey(dataSource.Name) ? this.FilterItemSoruce(((value as System.Data.DataSet).Tables[0]).DefaultView as IEnumerable, this.ExpFilters[dataSource.Name]) : ((value as System.Data.DataSet).Tables[0]).DefaultView);
                        }
#endif
                        else
                        {
                            throw new Exception("Collection class or datatable Input is needed");
                        }
                    }
                    else
                    {
                        throw new Exception("Provide Dataset inputs for Report");
                    }
                }
            }
        }

        List<DataSetInformations> GetDataSets(List<string> dataSets)
        {
            List<DataSetInformations> dataSetInfos = new List<DataSetInformations>();

            foreach (Syncfusion.RDL.DOM.DataSource dataSource in Report.DataSources)
            {
                foreach (var dataSetName in dataSets)
                {
                    Syncfusion.RDL.DOM.DataSet dataSet = this.Report.DataSets.Where(set => set.Name.Equals(dataSetName)).FirstOrDefault();

                    if (dataSet.Query.DataSourceName.Equals(dataSource.Name))
                    {
                        DataSetInformations info = new DataSetInformations() { Name = dataSetName, DataSet = dataSet, DataSource = dataSource };
                        dataSetInfos.Add(info);
                    }
                }
            }

            return dataSetInfos;
        }

        public void UpdateData()
        {
            if (this.Report.ReportParameters != null)
            {
                this.InitilizeParameterValues();
            }

            try
            {
                this.reportDataSets = this.dataSetCollection;
                this.reportDatasetIndex = -1;
                this.UpdateDataSetValue();
            }
            catch (Exception ex)
            {
                this.Exception = ex;
            }
        }

        public void UpdateParameterDatasets()
        {
            this.ResetProceesedData();

            try
            {
                if (this.parameterDataSetCollection.Count > 0)
                {
                    this.reportDataSets = this.parameterDataSetCollection;
                    this.reportDatasetIndex = -1;
                    this.UpdateDataSetValue();
                }
                else
                {
                    this.RaiseDataSourceUpdatedEvent(new EventArgs());
                }
            }
            catch { }
        }

#if SILVERLIGHT
        void UpdateDataSetValue()
        {
            this.reportDatasetIndex++;
            DataSetInformations dataSet = this.reportDataSets[reportDatasetIndex];

            var dataSetInfo = this.model.DataSetDetails
                                .Where(set => set.Name.Equals(dataSet.Name))
                                .FirstOrDefault();

            var paramSets = this.model.ParameterDetails
                            .Where(param => dataSetInfo.DependentParameters.Contains(param.Name))
                            .Select(param => param.Name);
            
            if (paramSets.Count() > 0)
            {
                this.InitilizeDatasetParameterValues(paramSets);
            }
            try
            {
                Syncfusion.Reports.Server.ReportDataInfo info = new Syncfusion.Reports.Server.ReportDataInfo();

                info.DataSource = new Syncfusion.Reports.Server.DataSource();
             
                info.DataSource.Name = dataSet.DataSource.Name;
                info.DataSource.ConnectionProperties = new Syncfusion.Reports.Server.ConnectionProperties();
                string connectionString = this.ExpressionEngine.GetEvalExpressionString(dataSet.DataSource.ConnectionProperties.ConnectString);
                ReportSetting setting = new ReportSetting();
             
                if (dataSet.DataSource.ConnectionProperties.UserName != null)
                {
                    connectionString += "; User ID=" + dataSet.DataSource.ConnectionProperties.UserName + "; Password=" + dataSet.DataSource.ConnectionProperties.PassWord;
                }

                else if (!dataSet.DataSource.ConnectionProperties.IntegratedSecurity)
                {
                    this.model.SetDataSourceCredentials(dataSourceCredentials);
                    connectionString += "; User ID=" + dataSet.DataSource.ConnectionProperties.UserName + "; Password=" + dataSet.DataSource.ConnectionProperties.PassWord;
                }

                info.DataSource.ConnectionProperties.ConnectionString = connectionString;
                info.DataSource.ConnectionProperties.DataProvider = dataSet.DataSource.ConnectionProperties.DataProvider;
                info.DataSource.ConnectionProperties.IntegratedSecurity = dataSet.DataSource.ConnectionProperties.IntegratedSecurity;
                info.DataSource.CommandText = dataSet.DataSet.Query.CommandText;

                List<String> shareddatasource = new List<string>();

                foreach (DOM.DataSource datasource in this.Report.DataSources.Where(d => d.DataSourceReference != null))
                {
                    shareddatasource.Add(datasource.Name);
                }

                if ((shareddatasource.Count > 0 || this.Report.DataSets.Where(d => d.SharedDataSet != null).Count()>0) && this.model.LoadInformationFromServer)
                {

#if WINRT
                    info.DataSource.DataSourceReference = new ObservableCollection<string>();

                    foreach (string data in shareddatasource)
                    {
                        info.DataSource.DataSourceReference.Add(data);
                    }
#elif SILVERLIGHT
                    info.DataSource.DataSourceReference = shareddatasource.ToArray();
#endif

                    if (this.Report.DataSets.Where(d => d.SharedDataSet != null).Count() > 0)
                    {
                        info.DataSource.DataSetRefernce = dataSet.Name;
                    }

                    setting.ReportPath = this.model.ReportPath;
                    setting.ReportServerURL = this.model.ReportServerUrl;
                    setting.ReportServerCredential = this.model.GetReportServerCredential();
                }


#if WINRT
                ObservableCollection<Syncfusion.Reports.Server.QueryReportParameter> queryParameters = new ObservableCollection<QueryReportParameter>();
#else
                List<Syncfusion.Reports.Server.QueryReportParameter> queryParameters = new List<QueryReportParameter>();
#endif

                foreach (QueryParameter QP in dataSet.DataSet.Query.QueryParameters)
                {
                    object value = this.ExpressionEngine.GetEvalExpression(QP.Value);
                    List<object> values = value as List<object>;

                    if (values == null)
                    {
                        queryParameters.Add(new QueryReportParameter() { Name = QP.Name, Value = value.ToString() });
                    }
                    else if (values.Count > 0)
                    {
                        info.DataSource.CommandText = info.DataSource.CommandText.Replace(QP.Name, "{0}");
                        string format = QP.Name + "{0}";
                        var parameters = new string[values.Count];

                        for (int i = 0; i < values.Count; i++)
                        {
                            parameters[i] = string.Format(format, i);
                            queryParameters.Add(new QueryReportParameter() { Name = parameters[i], Value = values[i] });
                        }

                        info.DataSource.CommandText = string.Format(info.DataSource.CommandText, string.Join(", ", parameters));
                    }
                }              

#if WINRT
                info.DataSource.QueryParameters = queryParameters;
#else
                info.DataSource.QueryParameters = queryParameters.ToArray();
#endif
                this.model.ReportingServer.GetDataCompleted += new ServerProcessor.GetDataEventHandler(ReportingServer_GetDataCompleted);
                this.model.ReportingServer.GetData(setting ,info);
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                this.model.ReportingServer.GetDataCompleted -= new ServerProcessor.GetDataEventHandler(ReportingServer_GetDataCompleted);

                if (this.reportDatasetIndex == this.reportDataSets.Count - 1)
                {
                    this.RaiseDataSourceUpdatedEvent(new EventArgs());
                }
                else
                {
                    this.UpdateDataSetValue();
                }
            }           
        }

        void ReportingServer_GetDataCompleted(object sender, ServerProcessor.GetDataEventArgs e)
        {
            this.model.ReportingServer.GetDataCompleted -= new ServerProcessor.GetDataEventHandler(ReportingServer_GetDataCompleted);

            if (e.Result != null)
            {
                DataSetInformations dataSet = this.reportDataSets[reportDatasetIndex];
                this.DataSourceObjects.Add(dataSet.Name, this.ExpFilters.ContainsKey(dataSet.Name) ? this.GetWrapperDataTable(e.Result, dataSet.DataSet.Fields, this.ExpFilters[dataSet.Name]) : this.GetWrapperDataTable(e.Result, dataSet.DataSet.Fields, null));
            }

            if (this.reportDatasetIndex == this.reportDataSets.Count - 1)
            {
                RaiseDataSourceUpdatedEvent(new EventArgs());
            }
            else
            {
                UpdateDataSetValue();
            }
        }

        List<Syncfusion.RDL.Data.ReportData> GetWrapperDataTable(RecordInfo Recordinfos,RDL.DOM.Fields fields,List<ExpFilter> filters)
        {
            List<Syncfusion.RDL.Data.ReportData> retrivedDataSource = new List<Syncfusion.RDL.Data.ReportData>();

            var engineOldValue = this.ExpressionEngine.FieldValues;

            foreach (ReportDatas rd in Recordinfos.ReportItems)
            {
                int col = 0;
                Syncfusion.RDL.Data.ReportData data = new Syncfusion.RDL.Data.ReportData();
                data.Data = new Dictionary<string, object>();
                this.ExpressionEngine.FieldValues = new Dictionary<string, object>();

                foreach (Syncfusion.Reports.Server.DataField df in rd.Data)
                {
                    object value = null;
                    if (df.Value != null)
                    {
                        value = this.TypeConversion(Recordinfos.FieldType[col], df.Value);
                    }
                  
                    this.ExpressionEngine.FieldValues.Add(df.FieldName, value);
                    data.Data.Add(df.FieldName, value);
                    col++;
                }

                foreach (var field in fields.Where(f=>f.DataField==null))
                {
                    object value = this.ExpressionEngine.GetEvalExpression(field.Value);
                    this.ExpressionEngine.FieldValues.Add(field.Name, value);
                    data.Data.Add(field.Name, value);
                }

                this.ExpressionEngine.FieldValues.Clear();
                this.ExpressionEngine.FieldValues = null;
                retrivedDataSource.Add(data);
            }

            this.ExpressionEngine.FieldValues = engineOldValue;
            return this.FilterItemSoruce(retrivedDataSource as IEnumerable, filters).Cast<ReportData>().ToList();
        }

        object TypeConversion(string datatype, string value)
        {
            object o = value;
            switch (datatype)
            {
                case "System.Int":
                    try
                    {
                        o = int.Parse(value);
                    }
                    catch
                    {
                        o = null;
                    }
                    break;
                case "System.Double":
                    try
                    {
                        o = double.Parse(value);
                    }
                    catch
                    {
                        o = null;
                    }
                    break;
                case "System.Float":
                    try
                    {
                        o = float.Parse(value);
                    }
                    catch
                    {
                        o = null;
                    }
                    break;

                case "System.Single":
                    try
                    {
                        o = Single.Parse(value);
                    }
                    catch
                    {
                        o = null;
                    }
                    break;
                case "System.DateTime":
                    try
                    {
                        o = System.DateTime.Parse(value);
                    }
                    catch
                    {
                        o = null;
                    }
                    break;
                case "System.Guid":
                    try
                    {
                        o = Guid.Parse(value);
                    }
                    catch
                    {
                        o = null;
                    }
                    break;
                case "System.Boolean":
                    try
                    {
                        o = bool.Parse(value);
                    }
                    catch
                    {
                        o = null;
                    }
                    break;
                case "System.Decimal":
                    try
                    {
                        o = decimal.Parse(value);
                    }
                    catch
                    {
                        o = null;
                    }
                    break;
                case "System.Byte[]":
                    try
                    {
                        o = System.Byte.Parse(value);
                    }
                    catch
                    {
                        o = null;
                    }
                    break;
                default:
                    o = value;
                    break;
            }
            return o;

        }
#else
        void UpdateDataSetValue()
        {
            this.reportDatasetIndex++;
            DataSetInformations dataSet = this.reportDataSets[reportDatasetIndex];

            var dataSetInfo = this.model.DataSetDetails
                                .Where(set => set.Name.Equals(dataSet.Name))
                                .FirstOrDefault();

            var paramSets = this.model.ParameterDetails
                            .Where(param => dataSetInfo.DependentParameters.Contains(param.Name))
                            .Select(param => param.Name);


            if (paramSets.Count() > 0)
            {
                this.InitilizeDatasetParameterValues(paramSets);
            }

            this.UpdateTable(dataSet);
        }

        void UpdateQueryParameters(DbCommand command, Syncfusion.RDL.DOM.DataSet dataSet, string provider)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();

            if (dataSet.Query.QueryParameters != null)
            {
                foreach (QueryParameter queryParameter in dataSet.Query.QueryParameters)
                {
                    object value = this.ExpressionEngine.GetEvalExpression(queryParameter.Value);
                    List<object> values = value as List<object>;

                    if (value == null)
                    {
                        throw new Exception("Please provide the parameter value for parameter " + queryParameter.Name);
                    }

                    if (values == null)
                    {
                        queryParameters.Add(queryParameter.Name, value);
                    }
                    else if (values.Count > 0)
                    {
                        command.CommandText = command.CommandText.Replace(queryParameter.Name, "{0}");

                        string format = queryParameter.Name + "{0}";
                        var parameters = new string[values.Count];

                        for (int i = 0; i < values.Count; i++)
                        {
                            parameters[i] = string.Format(format, i);
                            queryParameters.Add(parameters[i], values[i]);
                        }

                        command.CommandText = string.Format(command.CommandText, string.Join(",", parameters));
                    }
                }
            }

            if (provider.Equals(DataProviders.SQLServer, StringComparison.InvariantCultureIgnoreCase)
                || provider.Equals(DataProviders.SQLAzure, StringComparison.InvariantCultureIgnoreCase))
            {
                this.UpdateSQLParameters(command, queryParameters);
            }
            else if (provider.Equals(DataProviders.ORACLE, StringComparison.InvariantCultureIgnoreCase))
            {
                this.UpdateOracleParameters(command, queryParameters);
            }
            else if (provider.Equals(DataProviders.OLEDB, StringComparison.InvariantCultureIgnoreCase))
            {
                this.UpdateOLEDBParameters(command, queryParameters);
            }
            else if (provider.Equals(DataProviders.SQLServerCe, StringComparison.InvariantCultureIgnoreCase))
            {
                this.UpdateSQLCeParameters(command, queryParameters);
            }
            else if (provider.Equals(DataProviders.ODBC, StringComparison.InvariantCultureIgnoreCase))
            {
                this.UpdateODBCParameters(command, queryParameters);
            }
        }

        void UpdateSQLParameters(DbCommand command, Dictionary<string, object> queryParameters)
        {
            foreach (string queryParameter in queryParameters.Keys)
            {
                (command as SqlCommand).Parameters.AddWithValue(queryParameter, queryParameters[queryParameter]);
            }
        }

        void UpdateSQLCeParameters(DbCommand command, Dictionary<string, object> queryParameters)
        {
            foreach (string queryParameter in queryParameters.Keys)
            {
                (command as SqlCeCommand).Parameters.AddWithValue(queryParameter, queryParameters[queryParameter]);
            }
        }

        void UpdateOracleParameters(DbCommand command, Dictionary<string, object> queryParameters)
        {
            foreach (string queryParameter in queryParameters.Keys)
            {
                (command as OracleCommand).Parameters.AddWithValue(queryParameter, queryParameters[queryParameter]);
            }
        }

        void UpdateOLEDBParameters(DbCommand command, Dictionary<string, object> queryParameters)
        {
            foreach (string queryParameter in queryParameters.Keys)
            {
                (command as OleDbCommand).Parameters.AddWithValue(queryParameter, queryParameters[queryParameter]);
            }
        }

        void UpdateODBCParameters(DbCommand command, Dictionary<string, object> queryParameters)
        {
            foreach (string queryParameter in queryParameters.Keys)
            {
                (command as OdbcCommand).Parameters.AddWithValue(queryParameter, queryParameters[queryParameter]);
            }
        }

        void UpdateTable(DataSetInformations dataset)
        {
            Syncfusion.RDL.DOM.DataSource dataSource = dataset.DataSource;
            Syncfusion.RDL.DOM.DataSet dataSet = dataset.DataSet.Clone() as Syncfusion.RDL.DOM.DataSet;
            List<ReportData> data = null;

            try
            {
                string connectionString = this.ExpressionEngine.GetEvalExpressionString(dataSource.ConnectionProperties.ConnectString);
                dataSet.Query.CommandText = this.ExpressionEngine.GetEvalExpressionString(dataSet.Query.CommandText);

                if (dataSource.ConnectionProperties.UserName != null)
                {
                    connectionString += "; User ID=" + dataSource.ConnectionProperties.UserName + "; Password=" + dataSource.ConnectionProperties.PassWord;
                }

                else if (!dataSource.ConnectionProperties.IntegratedSecurity)
                {
                    this.model.SetDataSourceCredentials(ReportModel.dataSourceCredential);
                    if (dataSource.ConnectionProperties.UserName != null)
                    {
                        connectionString += "; User ID=" + dataSource.ConnectionProperties.UserName + "; Password=" + dataSource.ConnectionProperties.PassWord;
                    }
                }

                if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.SQLServer, StringComparison.InvariantCultureIgnoreCase)
                    || dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.SQLAzure, StringComparison.InvariantCultureIgnoreCase))
                {
                    data = this.GetSQLData(dataSource, dataSet, connectionString);
                }
                else if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.ORACLE, StringComparison.InvariantCultureIgnoreCase))
                {
                    data = this.GetOracleData(dataSource, dataSet, connectionString);
                }
                else if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.OLEDB, StringComparison.InvariantCultureIgnoreCase))
                {
                    data = this.GetOLEDBData(dataSource, dataSet, connectionString);
                }
                else if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.SQLServerCe, StringComparison.InvariantCultureIgnoreCase))
                {
                    data = this.GetSQLCeData(dataSource, dataSet, connectionString);
                }
                else if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.ODBC, StringComparison.InvariantCultureIgnoreCase))
                {
                    data = this.GetODBCData(dataSource, dataSet, connectionString);
                }
                else if (dataSource.ConnectionProperties.DataProvider.Equals(DataProviders.XML, StringComparison.InvariantCultureIgnoreCase))
                {
                    data = new XMLDataProvider(this.model).GetData(connectionString, dataSet.Query.CommandText);
                }

                List<string> expressionDependentColumns = new List<string>();
                List<System.Data.DataColumn> expressionColumns = new List<System.Data.DataColumn>();

                var fileds = from filed in dataSet.Fields where filed.DataField == null && filed.Value != null select filed;

                if (data != null)
                {
                    if (!this.DataSourceObjects.ContainsKey(dataSet.Name))
                    {
                        this.DataSourceObjects.Add(dataSet.Name, this.ExpFilters != null && this.ExpFilters.ContainsKey(dataSet.Name) ? UpdateCalulatedFields(data, dataSet.Fields, this.ExpFilters[dataSet.Name]) : UpdateCalulatedFields(data, dataSet.Fields, null));
                    }
                }
            }
            catch (Exception ex)
            {
                this.Exception = ex;
            }

            if (reportDatasetIndex == reportDataSets.Count - 1)
            {
                RaiseDataSourceUpdatedEvent(new EventArgs());
            }
            else
            {
                UpdateDataSetValue();
            }
        }

        List<ReportData> GetSQLData(DOM.DataSource dataSource, DOM.DataSet dataSet, string connectionString)
        {
            try
            {
                if (dataSource.ConnectionProperties.IntegratedSecurity == true)
                {
                    connectionString += ";Trusted_Connection=true;";
                }


                SqlCommand command = new SqlCommand(dataSet.Query.CommandText);

                if (dataSet.Query.CommandType == Syncfusion.RDL.DOM.CommandType.StoredProcedure)
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                }

                command.CommandTimeout = dataSet.Query.Timeout;

                UpdateQueryParameters(command, dataSet, dataSource.ConnectionProperties.DataProvider);
                return new SqlDataProvider().GetTableData(connectionString, command, dataSet.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        List<ReportData> GetSQLCeData(DOM.DataSource dataSource, DOM.DataSet dataSet, string connectionString)
        {
            try
            {
                connectionString = dataSource.ConnectionProperties.ConnectString;
                SqlCeCommand command = new SqlCeCommand(dataSet.Query.CommandText);
                UpdateQueryParameters(command, dataSet, dataSource.ConnectionProperties.DataProvider);

                try
                {
                    var path = connectionString.Remove(0, 12).Trim();

                    if (!path.Contains("Data Source") && !System.IO.Path.IsPathRooted(path) && !string.IsNullOrEmpty(this.model.ReportPath))
                    {
                        if (!System.IO.File.Exists(path))
                        {
                            string rootpath = System.IO.Path.GetDirectoryName(this.model.ReportPath);
                            string dataPath = System.IO.Path.Combine(rootpath, path);
                            string dir = System.IO.Path.GetDirectoryName(dataPath);
                            string fileName = System.IO.Path.GetFileName(dataPath);
                            connectionString = "Data Source=" + System.IO.Path.Combine(dir, fileName);
                        }
                    }
                }
                catch
                {
                }

                return new SqlServerCeDataProvider().GetTableData(connectionString, command, dataSet.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        List<ReportData> GetOracleData(DOM.DataSource dataSource, DOM.DataSet dataSet, string connectionString)
        {
            try
            {
                connectionString = dataSource.ConnectionProperties.ConnectString;
                OracleCommand command = new OracleCommand(dataSet.Query.CommandText);
                command.CommandTimeout = dataSet.Query.Timeout;
                UpdateQueryParameters(command, dataSet, dataSource.ConnectionProperties.DataProvider);
                return new OracleDataProvider().GetTableData(connectionString, command, dataSet.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        List<ReportData> GetOLEDBData(DOM.DataSource dataSource, DOM.DataSet dataSet, string connectionString)
        {
            try
            {
                connectionString = dataSource.ConnectionProperties.ConnectString;
                OleDbCommand command = new OleDbCommand(dataSet.Query.CommandText);
                command.CommandTimeout = dataSet.Query.Timeout;
                UpdateQueryParameters(command, dataSet, dataSource.ConnectionProperties.DataProvider);
                return new OleDbDataProvider().GetTableData(connectionString, command, dataSet.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        List<ReportData> GetODBCData(DOM.DataSource dataSource, DOM.DataSet dataSet, string connectionString)
        {
            try
            {
                connectionString = dataSource.ConnectionProperties.ConnectString;
                OdbcCommand command = new OdbcCommand(dataSet.Query.CommandText);
                command.CommandTimeout = dataSet.Query.Timeout;
                UpdateQueryParameters(command, dataSet, dataSource.ConnectionProperties.DataProvider);
                return new OdbcDataProvider().GetTableData(connectionString, command, dataSet.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        IList<ReportData> UpdateCalulatedFields(List<ReportData> dataSource, Fields fields, List<ExpFilter> filters)
        {
            var engineOldValue = this.ExpressionEngine.FieldValues;

            foreach (var field in fields.Where(f => f.DataField == null))
            {
                foreach (var data in dataSource)
                {
                    this.ExpressionEngine.FieldValues = data.Data;
                    object value = this.ExpressionEngine.GetEvalExpression(field.Value);
                    data.Data[field.Name] = value;
                }
            }

            this.ExpressionEngine.FieldValues = engineOldValue;

            return FilterItemSoruce(dataSource as IEnumerable, filters).Cast<ReportData>().ToList();
        }
#endif
        internal IEnumerable FilterItemSoruce(IEnumerable datasource, List<ExpFilter> filters)
        {
            using (FilterEngine fe = new FilterEngine())
            {
                return fe.FilterItemSoruce(datasource, filters, this.ExpressionEngine);
            }
        }

        #endregion
    }

    internal class DataSetInformations
    {
        public string Name { get; set; }
        public Syncfusion.RDL.DOM.DataSet DataSet { get; set; }
        public Syncfusion.RDL.DOM.DataSource DataSource { get; set; }
    }

    internal class ExpFilter
    {
        public string FilterExp { get; set; }
        public object FieldValue { get; set; }
        public bool IsSelectType { get; set; }
        public FilterOperators OperatorType { get; set; }
    }

    /// <summary>
    /// A Class Provides the DataProviders List
    /// </summary>
    class DataProviders
    {
        /// <summary>
        /// Constants stands for Providers List
        /// </summary>
        #region Constants

        /// <summary>
        /// Constant string determines Microsoft SQL Server Database
        /// </summary>
        public const string SQLServer = "SQL";

        /// <summary>
        /// Constant string determines Microsoft SQL Server Database
        /// </summary>
        public const string SQLAzure = "SQLAZURE";

        /// <summary>
        /// Constant string determines Oracle Database
        /// </summary>
        public const string ORACLE = "ORACLE";

        /// <summary>
        /// Constant string determines MySql Database
        /// </summary>
        public const string MySQL = "MySQL";

        /// <summary>
        /// Constant string determines OLEDB Connection
        /// </summary>
        public const string OLEDB = "OLEDB";

        /// <summary>
        /// Constant string determines SQLServer Service Analysis
        /// </summary>
        public const string SQLServerAnalysisService = "OLEDB-MD";

        /// <summary>
        /// Constant string determines ODBC Connection
        /// </summary>
        public const string ODBC = "ODBC";

        /// <summary>
        /// Constant string determines XML files
        /// </summary>
        public const string XML = "XML";

        /// <summary>
        /// Constant string determines SAP NetWeaverBI
        /// </summary>
        public const string SAPNetWeaverBI = "SAP NetWeaverBI";

        /// <summary>
        /// Constant string determines Hypersion Essbase Database
        /// </summary>
        public const string HyperionEssbase = "Hyperion Essbase";

        /// <summary>
        /// Constant string determines TERADATA
        /// </summary>
        public const string TERADATA = "TERADATA";

        /// <summary>
        /// Constant string determines SQLServerCe Compact Edition Database.
        /// </summary>
        public const string SQLServerCe = "SQLCe";

        #endregion
    }

    internal class FilterEngine : IDisposable
    {
        private IEnumerable dataSourceList = null;

        private object dataSource = null;

        public System.Type ItemType { get; set; }

        public object DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    dataSourceList = null;
                    ItemType = null;
                    itemProperties = null;
                }
            }
        }

        public virtual IEnumerable DataSourceList
        {
            get
            {
                if (dataSourceList == null)
                {
                    if (DataSource is IEnumerable)
                    {
                        dataSourceList = DataSource as IEnumerable;
                    }
                }
                return dataSourceList;
            }
            set { dataSourceList = value; }
        }

#if WINRT
        private List<PropertyInfo> itemProperties;

        /// <summary>
        /// Gets a PropertyDescriptorCollection describing the type of the underlying data item.
        /// </summary>
        public List<PropertyInfo> ItemProperties
        {
            set { itemProperties = value; }
            get
            {
                if (itemProperties == null && (DataSourceList != null || ItemType != null))
                {

                    if (ItemType == null)
                    {
                        foreach (object o in DataSourceList)
                        {
                            ItemType = o.GetType();
                            break;
                        }
                    }
                }

                if (ItemType != null && itemProperties == null)
                {
                    itemProperties = ItemType.GetTypeInfo().DeclaredProperties.ToList();
                }
                return itemProperties;
            }
        }

#elif SILVERLIGHT
        private List<PropertyInfo> itemProperties;

        /// <summary>
        /// Gets a PropertyDescriptorCollection describing the type of the underlying data item.
        /// </summary>
        public List<PropertyInfo> ItemProperties
        {
            get
            {
                if (itemProperties == null && (DataSourceList != null || ItemType != null))
                {

                    if (ItemType == null)
                    {
                        foreach (object o in DataSourceList)
                        {
                            ItemType = o.GetType();
                            break;
                        }
                    }
                }

                if (ItemType != null && itemProperties == null)
                {
                    itemProperties = ItemType.GetProperties().ToList();
                }
                return itemProperties;
            }
            set { itemProperties = value; }
        }

#else
        private PropertyDescriptorCollection itemProperties = null;

        public PropertyDescriptorCollection ItemProperties
        {
            get
            {
                if (itemProperties == null && (DataSourceList != null || ItemType != null))
                {
                    if (dataSourceList is ITypedList)
                    {
                        itemProperties = ((ITypedList)dataSourceList).GetItemProperties(null);
                    }
                    else
                    {
                        if (ItemType == null)
                        {
                            foreach (object o in DataSourceList)
                            {
                                ItemType = o.GetType();
                                break;
                            }
                        }
                        if (ItemType != null)
                        {
                            itemProperties = TypeDescriptor.GetProperties(ItemType);
                        }
                    }
                }
                return itemProperties;
            }
            set { itemProperties = value; }
        }
#endif

        Dictionary<string, object> GetRecords(object o)
        {
            if (o is ReportData)
            {
                return (o as ReportData).Data;
            }
#if !SyncfusionFramework3_5
            else if (o is IDictionary<string, object> && o is System.Dynamic.ExpandoObject)
            {
                try
                {
                    return new Dictionary<string, object>((o as IDictionary<string, object>));
                }
                catch
                {
                    return null;
                }
            }
#endif
#if !SILVERLIGHT && !WINRT
            else if (o is System.Data.DataRowView)
            {
                Dictionary<string, object> fieldValues = new Dictionary<string, object>();
                var items = (o as System.Data.DataRowView).Row.ItemArray;
                var row = (o as System.Data.DataRowView).Row;
                for (int pos = 0; pos < items.Count(); pos++)
                {
                    fieldValues.Add(row.Table.Columns[pos].ColumnName, items[pos]);
                }
                return fieldValues;
            }
#endif
            else
            {

                Dictionary<string, object> fieldValues = new Dictionary<string, object>();
                for (int i = 0; i < this.ItemProperties.Count; i++)
                {
                    fieldValues.Add(this.ItemProperties[i].Name, this.ItemProperties[i].GetValue(o));
                }
                return fieldValues;
            }
        }

        internal IEnumerable FilterItemSoruce(IEnumerable datasource, List<ExpFilter> filters, ExpressionEngine expressionEngine)
        {
            if (filters != null && filters.Count > 0)
            {
                var engineOldValue = expressionEngine.FieldValues;
                List<object> dataList;
                Dictionary<int, object> dataDic;
                Dictionary<object, int> duplicateDic;
                //int index = 0;
                foreach (var filter in filters)
                {
                    int i = 0;
                    int limit = 0;
                    dataList = new List<object>();
                    duplicateDic = new Dictionary<object, int>();
                    dataDic = new Dictionary<int, object>();
                    this.DataSource = datasource;
                    IEnumerable list = DataSourceList;
                    foreach (object o in list)
                    {
                        expressionEngine.FieldValues = new Dictionary<string, object>();
                        expressionEngine.FieldValues = this.GetRecords(o);
                        if (!filter.IsSelectType)
                        {
                            if ((bool)expressionEngine.GetEvalExpression(filter.FilterExp))
                            {
                                dataList.Add(o);
                            }
                        }
                        else
                        {
                            if (expressionEngine.FieldValues.ContainsKey(expressionEngine.FieldInformations[filter.FilterExp].First().FieldName))
                            {
                                dataList.Add(o);
                                var filterValue = expressionEngine.GetEvalExpression(filter.FilterExp);
                                dataDic.Add(i++, filterValue);
                                if (duplicateDic.ContainsKey(filterValue))
                                {
                                    duplicateDic[filterValue] += 1;
                                }
                                else
                                {
                                    duplicateDic.Add(filterValue, 1);
                                }
                                if (filter.OperatorType != FilterOperators.In)
                                    limit = int.Parse(expressionEngine.GetEvalExpression(filter.FieldValue.ToString()).ToString());
                            }
                        }
                    }
                    if (filter.IsSelectType)
                    {
                        var count = dataDic.Count;
                        var dic = dataDic.OrderBy(t => t.Value);
                        if (filter.OperatorType == FilterOperators.TopN)
                        {
                            var duplicate = duplicateDic.OrderByDescending(t => t.Key);
                            limit = GetRangingLimit(duplicate, limit);
                            dataList = FilterList(dic.Take(count - limit), dataList);
                        }
                        else if (filter.OperatorType == FilterOperators.BottomN)
                        {
                            var duplicate = duplicateDic.OrderBy(t => t.Key);
                            limit = GetRangingLimit(duplicate, limit);
                            dataList = FilterList(dic.Reverse().Take(count - limit), dataList);
                        }
                        else if (filter.OperatorType == FilterOperators.TopPercent)
                        {
                            var duplicate = duplicateDic.OrderByDescending(t => t.Key);
                            limit = (int)(((decimal)limit / 100) * count);
                            limit = GetRangingLimit(duplicate, limit);
                            dataList = FilterList(dic.Take(count - limit), dataList);
                        }
                        else if (filter.OperatorType == FilterOperators.BottomPercent)
                        {
                            var duplicate = duplicateDic.OrderBy(t => t.Key);
                            limit = (int)(((decimal)limit / 100) * count);
                            limit = GetRangingLimit(duplicate, limit);
                            dataList = FilterList(dic.Reverse().Take(count - limit), dataList);
                        }
                        else if (filter.OperatorType == FilterOperators.In)
                        {
                            var valColl = expressionEngine.GetEvalExpression(filter.FieldValue.ToString());
                            if (valColl != null)
                            {
                                List<object> items = new List<object>();
                                try
                                {
                                    items = (List<object>)valColl;
                                    var obj = from pair in dic
                                              where items != null && !items.Contains(pair.Value)
                                              select pair;
                                    dataList = FilterList(obj, dataList);
                                }
                                catch
                                {
                                    items.Add(valColl);
                                    if (items != null && items.Count() == 1)
                                    {
                                        var elements = items[0].ToString().Split(',');
                                        var obj = from pair in dic
                                                  where elements != null && !elements.Contains(pair.Value)
                                                  select pair;
                                        dataList = FilterList(obj, dataList);
                                    }
                                }
                            }
                        }
                    }
                    datasource = new List<object>(dataList);
                }
                expressionEngine.FieldValues = engineOldValue;
            }
            return datasource;
        }

        List<object> FilterList(IEnumerable<KeyValuePair<int, object>> dic, List<object> lt)
        {
            foreach (KeyValuePair<int, object> pair in dic)
            {
                lt[pair.Key] = null;
            }
            return new List<object>(lt.Where(t => t != null));
        }

        int GetRangingLimit(IEnumerable<KeyValuePair<object, int>> dic, int limit)
        {
            int ranking = 0;
            foreach (KeyValuePair<object, int> pair in dic)
            {
                ranking += pair.Value;
                if (ranking < limit)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            return ranking;
        }

        public void Dispose()
        {
            this.ItemProperties = null;
            this.DataSource = null;
            this.ItemType = null;
            this.dataSource = null;
            this.dataSourceList = null;
            this.DataSourceList = null;
            GC.SuppressFinalize(this);
        }
    }
}

