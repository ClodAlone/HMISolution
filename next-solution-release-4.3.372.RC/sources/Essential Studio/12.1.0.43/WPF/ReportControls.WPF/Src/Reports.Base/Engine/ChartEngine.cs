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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Syncfusion.RDL.ItemModel;
using Syncfusion.RDL.Data;

#if !WINRT
using Syncfusion.Windows.Data;
using Syncfusion.RDL.Internal;
#endif

namespace Syncfusion.RDL.Internal
{
    #region ChartEngine class

    /// <summary>
    /// This class encapsulates Charting calculation support. To use it, you first populate the <see cref="CategoryFields"/> and 
    /// <see cref="SeriesFields"/> collections to define the properties being Charted. You then populate the <see cref="ChartDataFields"/>
    /// collection to define the values you would like to see populated.
    /// </summary>
    internal class ChartEngine: IDisposable
    {
        #region properties

        private bool emptyChart = false;

        private object dataSource = null;

        private IEnumerable dataSourceList = null;
        private List<ChartItem> seriesFields = null;
        private List<ChartItem> categoryFields = null;
        private List<ChartComputationInfo> chartDataFields = null;
        private List<ChartComputationInfo> seriesValueFields = null;

        BinaryList seriesKeyValues;
        BinaryList categoryKeyValues;
        BinaryList chartDataKeysCalcValues;

        /// <summary>
        /// Gets or set a collection of drill through Chart properties.
        /// </summary>
        public List<ChartItem> SeriesActionInfoFields
        {
            get
            {
                if (seriesFields == null)
                    seriesFields = new List<ChartItem>();
                return seriesFields;
            }
            set { seriesFields = value; }
        }


        /// <summary>
        /// Gets or set a collection of row Chart properties.
        /// </summary>
        public List<ChartItem> SeriesFields
        {
            get
            {
                if (seriesFields == null)
                    seriesFields = new List<ChartItem>();
                return seriesFields;
            }
            set { seriesFields = value; }
        }

        /// <summary>
        /// Gets or sets a collection of column Chart properties.
        /// </summary>
        public List<ChartItem> CategoryFields
        {
            get
            {
                if (categoryFields == null)
                    categoryFields = new List<ChartItem>();
                return categoryFields;
            }
            set { categoryFields = value; }
        }

        /// <summary>
        /// Gets or sets a collection of Chart calculations.
        /// </summary>
        public List<ChartComputationInfo> ChartDataFields
        {
            get
            {
                if (chartDataFields == null)
                    chartDataFields = new List<ChartComputationInfo>();
                return chartDataFields;
            }
            set { chartDataFields = value; }
        }

        /// <summary>
        /// Gets or sets a collection of Chart calculations.
        /// </summary>
        public List<ChartComputationInfo> SeriesValueFields
        {
            get
            {
                if (seriesValueFields == null)
                    seriesValueFields = new List<ChartComputationInfo>();
                return seriesValueFields;
            }
            set { seriesValueFields = value; }
        }

        /// <summary>
        /// Gets whether a Chart results contains any items.
        /// <remarks>For example, if you apply a filter which filters
        /// out all items in the underlying data source, then this
        /// EmptyChart property will be set true. The RowCount and ColumnCount will
        /// be set to one, and the Engine[0, 0] will hold the value
        /// <see cref="EmptyChart"/>.</remarks>
        /// </summary>
        public bool EmptyChart
        {
            get { return emptyChart; }
            internal set { emptyChart = value; }
        }


        /// <summary>
        /// Gets or sets source of data for this Chart table. This object should be either 
        /// an IEnumerable list, or a DataTable.
        /// </summary>
        public object DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;

                    //force list to be reset
                    dataSourceList = null;
                    ItemType = null;
                    itemProperties = null;
                }
            }
        }

        /// <summary>
        /// Used internally. Gets or sets an IEnumerable list that is used as the data for the
        /// Chart table. The default behavior is to initailize this list from DataSource.
        /// </summary>
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
#if !SILVERLIGHT
                    else if (DataSource is System.Data.DataTable)
                    {
                        dataSourceList = ((System.Data.DataTable)DataSource).DefaultView as IEnumerable;
                    }
#endif
                }
                return dataSourceList;
            }
            set { dataSourceList = value; }
        }

        internal ChartEngineType EngineType
        {
            get;
            set;
        }

        public System.Type ItemType
        {
            get;
            set;
        }

        public List<SeriesDataSource> SeriesDataSources
        {
            get;
            set;
        }

        internal Dictionary<int, ChartDrillAction> ActionInfo
        {
            get;
            set;
        }

        internal Dictionary<int, Dictionary<string,TextboxActionInfoExpVal>> DrillActions
        {
            get;
            set;
        }

#if WINRT
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
                    itemProperties = ItemType.GetTypeInfo().DeclaredProperties.ToList();
                }
                return itemProperties;
            }
        }

#elif SILVERLIGHT
        private PropertyInfoCollection itemProperties;

        /// <summary>
        /// Gets a PropertyDescriptorCollection describing the type of the underlying data item.
        /// </summary>
        public PropertyInfoCollection ItemProperties
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
                    itemProperties = new PropertyInfoCollection(ItemType);
                }
                return itemProperties;
            }
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
        #endregion

        public void Populate()
        {
            PopulateChartTable();
            PopulateChartDataSource();
        }

        public void Reset()
        {
            this.DataSource = null;
            this.DataSourceList = null;
            this.ChartDataFields.Clear();
            this.CategoryFields.Clear();
            this.SeriesFields.Clear();
            this.DrillActions = null;
            this.ActionInfo = null;
        }

        #region private implementation methods

        object GetExpVal(string fieldName, object o)
        {
            try
            {
                return this.GetValue(fieldName, o);
            }
            catch
            {
                return fieldName;
            }
        }

#if SILVERLIGHT

        object GetValue(string field, object o)
        {
            if (o is ReportData && !string.IsNullOrEmpty(field))
            {
                return (o as ReportData).Data[field];
            }
            else if (o is IDictionary<string, object> && o is System.Dynamic.ExpandoObject)
            {
                try
                {
                    return (o as IDictionary<string, object>)[field];
                }
                catch
                {
                    return null;
                }
            }

            return o;
        }

        IComparable GetComparableValue(string field, object o)
        {
            if (o is ReportData && !string.IsNullOrEmpty(field))
            {
                return (o as ReportData).Data[field] as IComparable;
            }
            else if (o is IDictionary<string, object> && o is System.Dynamic.ExpandoObject)
            {
                try
                {
                    return (o as IDictionary<string, object>)[field] as IComparable;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
#else
        object GetValue(string field, object o)
        {
            if (o is ReportData && !string.IsNullOrEmpty(field))
            {
                return (o as ReportData).Data[field];
            }
#if !SILVERLIGHT && !WINRT
            else if (o is System.Data.DataRowView)
            {
                try
                {
                    return (o as System.Data.DataRowView)[field];
                }
                catch
                {
                    return null;
                }
            }
#endif
            else if (!string.IsNullOrEmpty(field) && (o.GetType().FullName == "Syncfusion.Reports.Server.ReportData"))
            {
                System.Reflection.PropertyInfo pi = o.GetType().GetProperty("Data");

                if (pi.GetValue(o, null).GetType() == typeof(Dictionary<string, object>))
                {
                    Dictionary<string, object> objectName = (Dictionary<string, object>)((object)(pi.GetValue(o, null)));
                    return objectName[field];
                }
            }
#if !SyncfusionFramework3_5
            else if (o is IDictionary<string, object> && o is System.Dynamic.ExpandoObject)
            {
                try
                {
                    return (o as IDictionary<string, object>)[field];
                }
                catch
                {
                    return null;
                }
            }
#endif
            return o;
        }

        IComparable GetComparableValue(string field, object o)
        {
            if (o is ReportData && !string.IsNullOrEmpty(field))
            {
                return (o as ReportData).Data[field] as IComparable;
            }
#if !SILVERLIGHT && !WINRT
            else if (o is System.Data.DataRowView)
            {
                try
                {
                    return (o as System.Data.DataRowView)[field] as IComparable;
                }
                catch
                {
                    return null;
                }
            }
#endif
            else if (!string.IsNullOrEmpty(field) && (o.GetType().FullName == "Syncfusion.Reports.Server.ReportData"))
            {
                System.Reflection.PropertyInfo pi = o.GetType().GetProperty("Data");

                if (pi.GetValue(o, null).GetType() == typeof(Dictionary<string, object>))
                {
                    Dictionary<string, object> objectName = (Dictionary<string, object>)((object)(pi.GetValue(o, null)));
                    return objectName[field] as IComparable;
                }
            }
#if !SyncfusionFramework3_5
            else if (o is IDictionary<string, object> && o is System.Dynamic.ExpandoObject)
            {
                try
                {
                    return (o as IDictionary<string, object>)[field] as IComparable;
                }
                catch
                {
                    return null;
                }
            }
#endif

            return null;
        }
#endif

        private void PopulateChartTable()
        {
            emptyChart = true;

            if (DataSourceList != null)
            {
                foreach (var o in dataSourceList)
                {
                    emptyChart = false;
                    break;
                }

                if (!emptyChart)
                {

                    seriesKeyValues = new BinaryList();
                    categoryKeyValues = new BinaryList();
                    chartDataKeysCalcValues = new BinaryList();

#if !SILVERLIGHT
                    PropertyDescriptor[] calcValuesPDs = GetCalcValuesPDs();
                    PropertyDescriptor[] seriesDataSourceFields = ProcessList(SeriesFields);
                    PropertyDescriptor[] categoryField = ProcessList(CategoryFields);
                    PropertyDescriptor[] seriesValueFields = GetSeriesValueFieldsPDs();
#else
                    PropertyInfo[] calcValuesPDs = GetCalcValuesPDs();
                    PropertyInfo[] seriesDataSourceFields = ProcessList(SeriesFields);
                    PropertyInfo[] categoryField = ProcessList(CategoryFields);
                    PropertyInfo[] seriesValueFields = GetSeriesValueFieldsPDs();
#endif

                    IComparer[] colComparers = GetComparers(CategoryFields);
                    IComparer[] rowComparers = GetComparers(SeriesFields);

                    List<IComparable> tableKeys;
                    List<IComparable> seriesKeys;
                    List<IComparable> categoryKeys;
                    List<object> values;
                    SummaryBase sb;

                    KeysCalculationValues tempValue = null;
                    int loc = -1;
                    IEnumerable list = DataSourceList;
                    int dataCount = 1;

                    foreach (object o in list)
                    {
                        tableKeys = new List<IComparable>();
                        seriesKeys = new List<IComparable>();
                        categoryKeys = new List<IComparable>();
                        values = new List<object>();
#if SILVERLIGHT
                        for (int i = 0; i < calcValuesPDs.Count(); i++)
                        {
                            PropertyInfo pd = calcValuesPDs[i];

                            if (pd == null)
                            {
                                string fieldName = this.ChartDataFields[i].FieldMappingName;
                                values.Add(this.GetValue(fieldName, o));
                            }
                            else
                            {
                                values.Add(pd.GetValue(o));
                            }
                        }

                        for (int i = 0; i < seriesValueFields.Count(); i++)
                        {
                            PropertyInfo pd = seriesValueFields[i];

                            if (pd == null)
                            {
                                string fieldName = this.SeriesValueFields[i].FieldMappingName;
                                values.Add(this.GetValue(fieldName, o));
                            }
                            else
                            {
                                values.Add(pd.GetValue(o));
                            }
                        }
#else
                        for (int i = 0; i < calcValuesPDs.Count(); i++)
                        {
                            PropertyDescriptor pd = calcValuesPDs[i];

                            if (pd == null)
                            {
                                string fieldName = this.ChartDataFields[i].FieldMappingName;
                                values.Add(this.GetValue(fieldName, o));
                            }
                            else
                            {
                                values.Add(pd.GetValue(o));
                            }
                        }

                        for (int i = 0; i < seriesValueFields.Count(); i++)
                        {
                            PropertyDescriptor pd = seriesValueFields[i];

                            if (pd == null)
                            {
                                string fieldName = this.SeriesValueFields[i].FieldMappingName;
                                values.Add(this.GetValue(fieldName, o));
                            }
                            else
                            {
                                values.Add(pd.GetValue(o));
                            }
                        }
#endif

                        if (SeriesFields.Count > 0)
                        {
                            for (int k = 0; k < SeriesFields.Count; ++k)// (PropertyDescriptor pd in rowPDs)
                            {
#if SILVERLIGHT
                                IComparable val = null;

                                if (seriesDataSourceFields[k] == null)
                                {
                                    string fieldName = this.SeriesFields[k].FieldMappingName;
                                    val = this.GetComparableValue(fieldName, o);
                                }
                                else
                                {
                                    val = seriesDataSourceFields[k].GetValue(o) as IComparable;
                                }
#else
                                IComparable val = null;

                                if (seriesDataSourceFields[k] == null)
                                {
                                    string fieldName = this.SeriesFields[k].FieldMappingName;
                                    val = this.GetComparableValue(fieldName, o);
                                }
                                else
                                {
                                    val = seriesDataSourceFields[k].GetValue(o) as IComparable;
                                }
#endif

                                if (val != null)
                                {
                                    if (SeriesFields[k].Format != null && SeriesFields[k].Format.Length > 0)
                                    {
                                        val = string.Format(SeriesFields[k].Format, val);
                                    }
                                    else
                                    {
                                        val = val.ToString();
                                    }
                                }
                                seriesKeys.Add(val);
                                tableKeys.Add(val);//"(null)");
                            }
                        }
                        else
                        {
                            seriesKeys.Add("Group");
                            tableKeys.Add("Group");//"(null)");
                        }

                        if (CategoryFields.Count > 0)
                        {
                            for (int k = 0; k < CategoryFields.Count; ++k)// (PropertyDescriptor pd in rowPDs)
                            {
#if SILVERLIGHT
                                IComparable val = null;
                                string fieldName="";

                                if (categoryField[k] == null)
                                {
                                    fieldName = this.CategoryFields[k].FieldMappingName;
                                    val = this.GetComparableValue(fieldName, o);
                                }
                                else
                                {
                                    val = categoryField[k].GetValue(o) as IComparable;
                                }
#else
                                IComparable val = null;
                                string fieldName = "";

                                if (categoryField[k] == null)
                                {
                                    fieldName = this.CategoryFields[k].FieldMappingName;
                                    val = this.GetComparableValue(fieldName, o);
                                }
                                else
                                {
                                    val = val = categoryField[k].GetValue(o) as IComparable;
                                }
#endif
                                if (val != null)
                                {
                                    if (CategoryFields[k].Format != null && CategoryFields[k].Format.Length > 0)
                                    {
                                        val = string.Format(SeriesFields[k].Format, val);
                                    }
                                    else
                                        val = val.ToString();
                                }
                                else if (string.IsNullOrEmpty(fieldName))
                                {
                                    val = dataCount;
                                    dataCount++;
                                }

                                categoryKeys.Add(val);
                                tableKeys.Add(val);//"(null)");
                            }
                        }
                        else
                        {
                            categoryKeys.Add(this.ChartDataFields.First().FieldMappingName);
                            tableKeys.Add(this.ChartDataFields.First().FieldMappingName);//"(null)");
                        }

                        if (this.ActionInfo != null && this.ActionInfo.Count > 0)
                        {
                            foreach (var item in this.ActionInfo)
                            {
                                TextboxActionInfoExpVal actionInfo = new TextboxActionInfoExpVal();
                                if (item.Value.BookmarkLink != null && !string.IsNullOrEmpty(item.Value.BookmarkLink.FieldMappingName))
                                {
                                    var val = this.GetExpVal(item.Value.Hyperlink.FieldMappingName, o);
                                    actionInfo.BookmarkLink = val == null ? null : val.ToString();
                                }
                                else if (item.Value.Hyperlink != null && !string.IsNullOrEmpty(item.Value.Hyperlink.FieldMappingName))
                                {
                                    var val = this.GetExpVal(item.Value.Hyperlink.FieldMappingName, o);
                                    actionInfo.Hyperlink = val == null ? null : val.ToString();
                                }
                                else if (item.Value.DrillThrough != null && item.Value.DrillThrough.ReportName != null && !string.IsNullOrEmpty(item.Value.DrillThrough.ReportName.FieldMappingName))
                                {
                                    var val = this.GetExpVal(item.Value.DrillThrough.ReportName.FieldMappingName, o);
                                    actionInfo.ReportName = val == null ? null : val.ToString();

                                    if (item.Value.DrillThrough.Parameters != null && item.Value.DrillThrough.Parameters.Count > 0)
                                    {
                                        actionInfo.Parameters = new List<TextboxParameterExpVal>();
                                        foreach (var par in item.Value.DrillThrough.Parameters)
                                        {
                                            var vname = this.GetExpVal(par.Name.FieldMappingName, o);
                                            var value = this.GetExpVal(par.Value.FieldMappingName, o);

                                            TextboxParameterExpVal param = new TextboxParameterExpVal();
                                            param.Name = vname == null ? null : vname.ToString();
                                            param.Value = value == null ? null : value.ToString();
                                            actionInfo.Parameters.Add(param);
                                        }
                                    }
                                }
                                if (DrillActions == null)
                                {
                                    DrillActions = new Dictionary<int, Dictionary<string, TextboxActionInfoExpVal>>();
                                }
                                if (DrillActions.ContainsKey(item.Key))
                                {
                                    if (!DrillActions[item.Key].ContainsKey(categoryKeys[0].ToString()))
                                    {
                                        Dictionary<string, List<TextboxActionInfoExpVal>> tempDt = new Dictionary<string, List<TextboxActionInfoExpVal>>();
                                        DrillActions[item.Key].Add(categoryKeys[0].ToString(), actionInfo);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, TextboxActionInfoExpVal> tempDt = new Dictionary<string, TextboxActionInfoExpVal>();
                                    tempDt.Add(categoryKeys[0].ToString(), actionInfo);
                                    DrillActions.Add(item.Key, tempDt);
                                }
                            }
                        }

                        this.categoryKeyValues.AddIfUnique(tempValue = new KeysCalculationValues { Keys = categoryKeys });
                        this.seriesKeyValues.AddIfUnique(tempValue = new KeysCalculationValues { Keys = seriesKeys });

                        if ((loc = chartDataKeysCalcValues.AddIfUnique(tempValue = new KeysCalculationValues() { Keys = tableKeys })) < 0)
                        {
                            int k = 0;
                            tempValue.Values = new List<SummaryBase>();
                            foreach (ChartComputationInfo info in ChartDataFields)
                            {
                                sb = info.Summary.GetInstance();
                                //sb.Reset();
                                sb.Combine(values[k++]);
                                tempValue.Values.Add(sb);
                            }
                            foreach (ChartComputationInfo info in this.SeriesValueFields)
                            {
                                sb = info.Summary.GetInstance();
                                //sb.Reset();
                                sb.Combine(values[k++]);
                                tempValue.Values.Add(sb);
                            }
                        }
                        else
                        {
                            tempValue = chartDataKeysCalcValues[loc] as KeysCalculationValues;
                            for (int k = 0; k < ChartDataFields.Count; ++k)
                            {
                                tempValue.Values[k].Combine(values[k]);
                            }
                            for (int k = ChartDataFields.Count; k < ChartDataFields.Count + SeriesValueFields.Count; ++k)
                            {
                                tempValue.Values[k].Combine(values[k]);
                            }
                        }
                    }
                }
                else
                {
                    emptyChart = true;
                    return;
                }
            }
        }

        void PopulateChartDataSource()
        {
            SeriesDataSources = new List<SeriesDataSource>();

            if (this.seriesKeyValues != null)
            {
                foreach (var groupKey in this.seriesKeyValues)
                {
                    KeysCalculationValues groupKeys = groupKey as KeysCalculationValues;
                    foreach (var dataField in this.ChartDataFields)
                    {
                        int fieldIndex = this.ChartDataFields.IndexOf(dataField);
                        Dictionary<string,TextboxActionInfoExpVal> actionVals = null;
                        if(this.DrillActions != null && this.DrillActions.ContainsKey(fieldIndex))
                        {
                            actionVals = this.DrillActions[fieldIndex];
                        }
                        List<ReportingChartData> datas = new List<ReportingChartData>();
                        foreach (var category in this.categoryKeyValues)
                        {
                            KeysCalculationValues categoryKeys = category as KeysCalculationValues;
                            List<IComparable> keyValue = new List<IComparable>();
                            keyValue.AddRange(groupKeys.Keys);
                            keyValue.AddRange(categoryKeys.Keys);
                            ReportingChartData dataValue = new ReportingChartData();
                            dataValue.X = categoryKeys.ToString();
                            dataValue.Y = 0;
                            var value = from chartValue in this.chartDataKeysCalcValues where CompareList(((KeysCalculationValues)chartValue).Keys, keyValue) select chartValue;
                            if (value.Count() > 0)
                            {
                                dataValue.Y = ((KeysCalculationValues)value.First()).Values[fieldIndex].GetResult();
                            }
                            if (actionVals != null && actionVals.Count > 0 && actionVals.ContainsKey(dataValue.X))
                            {
                                dataValue.ActionInfo = actionVals[dataValue.X];
                            }
                            datas.Add(dataValue);
                        }

                        List<IComparable> keys = new List<IComparable>();
                        keys.AddRange(groupKeys.Keys);
                        keys.AddRange(((KeysCalculationValues)this.categoryKeyValues.First()).Keys);
                        var seriedFieldDataValue = from chartValue in this.chartDataKeysCalcValues where CompareList(((KeysCalculationValues)chartValue).Keys, keys) select chartValue;

                        SeriesDataSource dataSource = new SeriesDataSource();
                        dataSource.SeriesFieldValues = new Dictionary<string, object>();
                        string labelString = String.Empty;

                        foreach (var seriesValueField in this.SeriesValueFields)
                        {
                            int index = this.SeriesValueFields.IndexOf(seriesValueField);

                            if (!dataSource.SeriesFieldValues.Keys.Contains(seriesValueField.FieldName))
                            {
                                dataSource.SeriesFieldValues.Add(seriesValueField.FieldName, ((KeysCalculationValues)seriedFieldDataValue.First()).Values[index + this.ChartDataFields.Count].GetResult());
                            }
                        }

                        if (groupKeys.Keys.Count == 1 && !groupKeys.Keys[0].Equals("Group"))
                        {
                            labelString += groupKey.ToString();
                        }

                        if (this.chartDataFields.Count > 1 || this.seriesFields.Count == 0)
                        {
                            labelString += this.chartDataFields[fieldIndex].FieldName;
                        }

                        dataSource.Label = labelString;
                        dataSource.DataSource = datas;
                        dataSource.Name = dataField.FieldName;
                        SeriesDataSources.Add(dataSource);
                    }
                }
            }
        }

        bool CompareList(List<IComparable> obj1, List<IComparable> obj2)
        {
            if (obj1.Count != obj2.Count)
                return false;
            for (int i = 0; i < obj1.Count; i++)
            {
                if (obj1[i] == null && obj2[i] != null)
                    return false;
                else if (obj2[i] == null && obj1[i] != null)
                    return false;
                else if (obj1[i] != null && !obj1[i].Equals(obj2[i]))
                    return false;
            }
            return true;
        }

        private IComparer[] GetComparers(List<ChartItem> ChartItems)
        {
            int count = ChartItems.Count;

            IComparer[] pds = new IComparer[count];
            for (int i = 0; i < count; i++)
            {
                pds[i] = ChartItems[i].Comparer;
            }

            return pds;
        }

#if SILVERLIGHT

        private PropertyInfo[] ProcessList(List<ChartItem> ChartItems)
        {
            int count = ChartItems.Count;
            PropertyInfo[] pds = new PropertyInfo[count];
            for (int i = 0; i < count; i++)
            {
                if (dataSource.GetType().Name.Contains("ReportData"))
                {
                    pds[i] = null;
                }
                else
                {
                    try
                    {

#if WINRT
                        pds[i] = this.ItemProperties.Where(info => info.Name == ChartItems[i].FieldMappingName).First();
                  
#else
                        pds[i] = ItemProperties[ChartItems[i].FieldMappingName];
#endif
                    }
                    catch
                    {
                        pds[i] = null;
                    }
                }
            }

            return pds;
        }

        private PropertyInfo[] GetSeriesValueFieldsPDs()
        {
            PropertyInfo[] pds = new PropertyInfo[this.SeriesValueFields.Count];

            for (int i = 0; i < SeriesValueFields.Count; i++)
            {
                if (dataSource.GetType().Name.Contains("ReportData"))
                {
                    pds[i] = null;
                }
                else
                {
                    try
                    {

#if WINRT
                        pds[i] = this.ItemProperties.Where(info => info.Name == SeriesValueFields[i].FieldMappingName).First();
         
#else
                        pds[i] = ItemProperties[SeriesValueFields[i].FieldMappingName];
#endif
                    }
                    catch
                    {
                        pds[i] = null;
                    }
                }
            }

            return pds;
        }

        private PropertyInfo[] GetCalcValuesPDs()
        {
            PropertyInfo[] pds = new PropertyInfo[this.ChartDataFields.Count];
            if (ItemProperties != null)
            {
                for (int i = 0; i < ChartDataFields.Count; i++)
                {
                    if (dataSource.GetType().Name.Contains("ReportData"))
                    {
                        pds[i] = null;
                    }
                    else
                    {
                        try
                        {
#if WINRT
                            pds[i] = this.ItemProperties.Where(info => info.Name == ChartDataFields[i].FieldMappingName).First();
#else
                            pds[i] = ItemProperties[ChartDataFields[i].FieldMappingName];
#endif
                        }
                        catch 
                        {
                            pds[i] = null;
                        }

                    }
                }
            }

            return pds;
        }

#else
        private PropertyDescriptor[] ProcessList(List<ChartItem> ChartItems)
        {
            int count = ChartItems.Count;
            PropertyDescriptor[] pds = new PropertyDescriptor[count];

            for (int i = 0; i < count; i++)
            {
                if (dataSource.GetType().Name.Contains("ReportData"))
                {
                    pds[i] = null;
                }
                else
                {
                    pds[i] = ItemProperties[ChartItems[i].FieldMappingName];
                }
            }

            return pds;
        }

        private PropertyDescriptor[] GetSeriesValueFieldsPDs()
        {
            PropertyDescriptor[] pds = new PropertyDescriptor[this.SeriesValueFields.Count];

            for (int i = 0; i < SeriesValueFields.Count; i++)
            {
                if (dataSource.GetType().Name.Contains("ReportData"))
                {
                    pds[i] = null;
                }
                else
                {
                    pds[i] = ItemProperties[SeriesValueFields[i].FieldMappingName];
                }
            }

            return pds;
        }

        private PropertyDescriptor[] GetCalcValuesPDs()
        {
            PropertyDescriptor[] pds = new PropertyDescriptor[ChartDataFields.Count];

            if (ItemProperties != null)
            {
                for (int i = 0; i < ChartDataFields.Count; i++)
                {
                    if (dataSource.GetType().Name.Contains("ReportData"))
                    {
                        pds[i] = null;
                    }
                    else
                    {
                        pds[i] = ItemProperties[ChartDataFields[i].FieldMappingName];
                    }
                }
            }

            return pds;
        }
#endif

        #endregion

        public void Dispose()
        {
            this.DrillActions = null;
            this.ActionInfo = null;
        }
    }

    #endregion

    internal class SeriesDataSource
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public Dictionary<string, object> SeriesFieldValues { get; set; }
        public IEnumerable DataSource { get; set; }
    }

    public class ReportingChartData
    {
        public string X { get; set; }
        public object Y { get; set; }
        public object ActionInfo { get; set; }
    }

    #region ChartItem class

    /// <summary>
    /// Enacapulates the information needed to define a Chart item, for either a row or column Chart.
    /// </summary>
    /// <remarks>
    /// A Chart item is a property in the underlying data objects 
    /// that is used to grouped the data in a Chart table. You can add Chart items to both the 
    /// ChartColumns and ChartRows collection in a mutually exclusive manner.</remarks>
    internal class ChartItem
    {
        /// <summary>
        /// Gets or sets the property's mappingname.
        /// </summary>
        public string FieldMappingName { get; set; }
        /// <summary>
        /// Gets or sets the title you want to see in the header for this Chart item.
        /// </summary>
        public string FieldHeader { get; set; }

        /// <summary>
        /// Gets or sets the string you want appended to the Chart item's summary cells.
        /// </summary>
        public string TotalHeader { get; set; }

        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the IComparer object used for sorting. If this value is null, then sorting is done assuming this field is IComparable.
        /// </summary>
        public IComparer Comparer { get; set; }
    }


    internal class ChartDrillAction
    {
        public ChartItem BookmarkLink { get; set; }

        public ChartItem Hyperlink { get; set; }

        public ChartDrillThrough DrillThrough { get; set; }
    }

    internal class ChartDrillThrough
    {
        public ChartItem ReportName { get; set; }

        public List<ChartDrillParameter> Parameters { get; set; }
    }

    internal class ChartDrillParameter
    {
        public ChartItem Name { get; set; }

        public ChartItem Value { get; set; }
    }

    #endregion
}
