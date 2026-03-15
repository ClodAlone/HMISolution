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
using Syncfusion.RDL.DOM;
using Syncfusion.RDL.Data;

#if !WINRT
using Syncfusion.Windows.Data;
#endif

namespace Syncfusion.RDL.Internal
{
    internal class TablixEngine
    {
        private IEnumerable dataSourceList = null;

        private object dataSource = null;

        public GroupInfoCollection GroupCollection { get; set; }

        public System.Type ItemType { get; set; }

        public TablixModel Model { get; set; }

        public bool IsEmptyDataSource { get; set; }

        public bool EnableVirtulization { get; set; }

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

        object GetValue(string field, object o)
        {
#if WINRT
            if (o is Syncfusion.RDL.Data.ReportData)
            {
                return ((Syncfusion.RDL.Data.ReportData)o).Data[field];
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

            return this.ItemProperties.Where(info=> info.Name == field).First().GetValue(o);
#elif SILVERLIGHT
            if (o is Syncfusion.RDL.Data.ReportData)
            {
                return ((Syncfusion.RDL.Data.ReportData)o).Data[field];
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

            return this.ItemProperties[field].GetValue(o);
#else
            if (o is Syncfusion.RDL.Data.ReportData)
            {
                try
                {
                    return ((Syncfusion.RDL.Data.ReportData)o).Data[field];
                }
                catch
                {
                    return null;
                }
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
            else if (o.GetType().FullName == "Syncfusion.Reports.Server.ReportData")
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
            try
            {
                return this.ItemProperties[field].GetValue(o);
            }
            catch
            {
                return null;
            }
#endif
        }

        public void PopulateTable()
        {
            IsEmptyDataSource = true;

            KeysCalculationValues emptKey = new KeysCalculationValues();
            try
            {
                IEnumerable list = DataSourceList;
                Dictionary<string, KeysCalculationValues> rowKeyValues = new Dictionary<string, KeysCalculationValues>();
                Dictionary<string, KeysCalculationValues> colKeyValues = new Dictionary<string, KeysCalculationValues>();

                foreach (object o in list)
                {
                    IsEmptyDataSource = false;

                    foreach (var info in this.GroupCollection)
                    {
                        KeysCalculationValues sortOrderKey = emptKey;
                        KeysCalculationValues recursiveOrderKey = emptKey;

                        if (info.RecursiveParent != null)
                        {
                            recursiveOrderKey = new KeysCalculationValues();
                            List<IComparer> recursiveComparers = new List<IComparer>();
                            recursiveComparers.Add(new AscendingComparerHelper());

                            recursiveOrderKey.Keys = new List<IComparable>();
                            recursiveOrderKey.Comparers = recursiveComparers.ToArray();

                            this.Model.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();

                            foreach (var field in info.RecursiveParentFields)
                            {
                                object value = null;

                                if (field.Expression == null)
                                {
                                    value = this.GetValue(field.FieldName, o);
                                }
                                else
                                {
                                    value = this.Model.Model.ExpressionEngine.GetEvalExpression(field.Expression);
                                }

                                this.Model.Model.ExpressionEngine.FieldValues.Add(field.Name, value);
                            }

                            object recValue = this.Model.Model.ExpressionEngine.GetEvalExpression(info.RecursiveParent);
                            recursiveOrderKey.Keys.Add(recValue as IComparable);
                            this.Model.Model.ExpressionEngine.FieldValues.Clear();
                        }

                        if (info.SortExpressions != null)
                        {
                            List<IComparable> groupKeys = new List<IComparable>();
                            List<IComparer> comparers = new List<IComparer>();

                            this.Model.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();

                            foreach (var field in info.SortFields)
                            {
                                object value = null;

                                if (field.Expression == null)
                                {
                                    value = this.GetValue(field.FieldName, o);
                                }
                                else
                                {
                                    value = this.Model.Model.ExpressionEngine.GetEvalExpression(field.Expression);
                                }

                                this.Model.Model.ExpressionEngine.FieldValues.Add(field.Name, value);
                            }

                            foreach (var sort in info.SortExpressions)
                            {
                                groupKeys.Add(this.Model.Model.ExpressionEngine.GetEvalExpression(sort.Expression) as IComparable);

                                if (sort.SortOrder == SortDirection.Descending)
                                {
                                    comparers.Add(new DescendingComparerHelper());
                                }
                                else
                                {
                                    comparers.Add(new AscendingComparerHelper());
                                }
                            }

                            this.Model.Model.ExpressionEngine.FieldValues.Clear();

                            sortOrderKey = new KeysCalculationValues() { Keys = groupKeys, Comparers = comparers.ToArray() };
                        }

                        KeysCalculationValues groupKey = emptKey;

                        if (info.IsRow && info.ParentGroup != null && rowKeyValues.ContainsKey(info.ParentGroup.Name))
                        {
                            groupKey = rowKeyValues[info.ParentGroup.Name];
                        }
                        else if (info.ParentGroup != null && colKeyValues.ContainsKey(info.ParentGroup.Name))
                        {
                            groupKey = colKeyValues[info.ParentGroup.Name];
                        }

                        if (info.GroupExpressions != null)
                        {
                            List<IComparable> groupKeys = new List<IComparable>();

                            if (groupKey.Keys != null && groupKey.Keys.Count > 0)
                            {
                                groupKeys.AddRange(groupKey.Keys);
                            }

                            this.Model.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();

                            foreach (var field in info.GroupFields)
                            {
                                object value = null;

                                if (field.Expression == null)
                                {
                                    value = this.GetValue(field.FieldName, o);
                                }
                                else
                                {
                                    value = this.Model.Model.ExpressionEngine.GetEvalExpression(field.Expression);
                                }

                                this.Model.Model.ExpressionEngine.FieldValues.Add(field.Name, value);
                            }

                            foreach (var groupExpression in info.GroupExpressions)
                            {
                                groupKeys.Add(this.Model.Model.ExpressionEngine.GetEvalExpression(groupExpression) as IComparable);
                            }

                            this.Model.Model.ExpressionEngine.FieldValues.Clear();

                            groupKey = new KeysCalculationValues() { Keys = groupKeys };
                        }

                        info.GroupRecursiveParentOrderKey.AddIfUnique(recursiveOrderKey);

                        if (info.IsRow)
                        {
                            if (!rowKeyValues.ContainsKey(info.Name))
                            {
                                rowKeyValues.Add(info.Name, groupKey);
                            }
                        }
                        else
                        {
                            if (!colKeyValues.ContainsKey(info.Name))
                            {
                                colKeyValues.Add(info.Name, groupKey);
                            }
                        }

                        GroupValueInfo valueInfo = null;

                        int index = info.GroupValueOrderKeys.GetKey(groupKey);

                        if (index < 0)
                        {
                            valueInfo = new GroupValueInfo();
                            valueInfo.GroupKey = groupKey;
                            valueInfo.RecursiveParentGroupKey = recursiveOrderKey;
                            valueInfo.ParentGroupKey = emptKey;

                            if (info.IsRow && info.ParentGroup != null && rowKeyValues.ContainsKey(info.ParentGroup.Name))
                            {
                                valueInfo.ParentGroupKey = rowKeyValues[info.ParentGroup.Name];
                            }
                            else if (info.ParentGroup != null && colKeyValues.ContainsKey(info.ParentGroup.Name))
                            {
                                valueInfo.ParentGroupKey = colKeyValues[info.ParentGroup.Name];
                            }

                            int sortOrderLoc = emptKey.Equals(sortOrderKey) ? info.GroupValuesIndexes.Count() : info.GroupSortOrderKey.AddIfUniqueKey(sortOrderKey);

                            int orderKey = info.GroupValueOrderKeys.AddIfUniqueKey(groupKey);
                            info.GroupValuesIndexes.Insert(orderKey, valueInfo);

                            if (info.Type == TablixGroupType.Group)
                            {                                
                                info.GroupValues.Insert(sortOrderLoc, valueInfo);
                            }
                            else
                            {
                                info.GroupValues.Add(valueInfo);
                            }

                            if ((!string.IsNullOrEmpty(info.ToggleItem) || !string.IsNullOrEmpty(info.IsHidden)) && info.Type != TablixGroupType.None)
                            {
                                valueInfo.ToggleGroups = info.ToggleGroups;
                                valueInfo.ToggleItem = info.ToggleItem;
                                valueInfo.IsHidden = info.IsHidden;
                                valueInfo.HiddenFields = info.HiddenFields;
                            }
                        }
                        else
                        {
                            valueInfo = info.GroupValuesIndexes[index];
                        }

                        if (info.DocumentMapFields != null)
                        {
                            valueInfo.DocumentKey = info.DocumentKey;
                            valueInfo.DocumentMapFields = info.DocumentMapFields;
                        }

                        TablixEngineFieldValue fieldValueInfo = null;

                        if (info.Type == TablixGroupType.Detail)
                        {
                            int sortOrderLoc = emptKey.Equals(sortOrderKey) ? -1 : valueInfo.FieldSortOrderKey.AddKeyValue(sortOrderKey);

                            if (sortOrderLoc < 0)
                            {
                                sortOrderLoc = valueInfo.FieldSortOrderKey.Count();
                                valueInfo.FieldSortOrderKey.Insert(sortOrderLoc, sortOrderKey);
                            }

                            valueInfo.DataSources.Insert(sortOrderLoc, o);
                        }
                        else if (valueInfo.Values.Count == 0)
                        {
                            fieldValueInfo = new TablixEngineFieldValue();
                            fieldValueInfo.Value = new List<FieldValue>();
                            valueInfo.Values.Add(fieldValueInfo);
                        }
                        else
                        {
                            fieldValueInfo = valueInfo.Values.First();
                        }

                        if (info.Type != TablixGroupType.Detail)
                        {
                            valueInfo.DataSources.Add(o);
                        }

                        this.Model.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();

                        foreach (var field in info.Fields)
                        {
                            KeysCalculationValues rowKey = rowKeyValues.Keys.Contains(field.RowGroupName) ? rowKeyValues[field.RowGroupName] : emptKey;
                            KeysCalculationValues columnKey = colKeyValues.Keys.Contains(field.ColumnGroupName) ? colKeyValues[field.ColumnGroupName] : emptKey;

                            List<IComparable> groupKeys = new List<IComparable>();

                            if (rowKey.Keys != null && rowKey.Keys.Count > 0)
                            {
                                groupKeys.AddRange(rowKey.Keys);
                            }

                            if (columnKey.Keys != null && columnKey.Keys.Count > 0)
                            {
                                groupKeys.AddRange(columnKey.Keys);
                            }

                            KeysCalculationValues fieldGroupKey = emptKey;

                            if (groupKeys.Count > 0)
                            {
                                fieldGroupKey = new KeysCalculationValues();
                                fieldGroupKey.Keys = groupKeys;
                            }

                            valueInfo.GroupFieldKeys.AddIfUnique(fieldGroupKey);

                            if (info.Type != TablixGroupType.Detail)
                            {
                                object value = null;

                                if (!field.IsDataSetField)
                                {
                                    if (field.Expression == null)
                                    {
                                        value = this.GetValue(field.FieldName, o);
                                    }
                                    else
                                    {
                                        value = this.Model.Model.ExpressionEngine.GetEvalExpression(field.Expression);
                                    }

                                    if (this.Model.Model.ExpressionEngine.FieldValues.Keys.Contains(field.Name))
                                    {
                                        this.Model.Model.ExpressionEngine.FieldValues[field.Name] = value;
                                    }
                                    else
                                    {
                                        this.Model.Model.ExpressionEngine.FieldValues.Add(field.Name, value);
                                    }

                                    SummaryBase summaryBase = null;

                                    var fieldValues = from fieldValue in fieldValueInfo.Value
                                                      where (fieldValue.Name.Equals(field.Name)
                                                          //&& fieldValue.RowGroupName.Equals(field.RowGroupName)
                                                          //&& fieldValue.ColumnGroupName.Equals(field.ColumnGroupName)
                                                      && fieldValue.GroupKey.Equals(fieldGroupKey))
                                                      select fieldValue;

                                    if (fieldValues.Count() > 0)
                                    {
                                        summaryBase = fieldValues.First().Value;
                                    }
                                    else
                                    {
                                        summaryBase = field.GetSummaryInstance();
                                        FieldValue fieldValue = new FieldValue() { Name = field.Name, Value = summaryBase };
                                        //fieldValue.RowGroupName = field.RowGroupName;
                                        //fieldValue.ColumnGroupName = field.ColumnGroupName;
                                        fieldValue.GroupKey = fieldGroupKey;
                                        fieldValueInfo.Value.Add(fieldValue);
                                    }

                                    summaryBase.Combine(value);
                                }
                            }
                        }

                        this.Model.Model.ExpressionEngine.FieldValues.Clear();
                    }

                    rowKeyValues.Clear();
                    colKeyValues.Clear();
                }

                foreach (var info in this.GroupCollection)
                {
                    foreach (var values in info.GroupValues)
                    {
                        values.FieldSortOrderKey.Clear();
                        values.FieldSortOrderKey = null;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void DisposeEngine()
        {
            this.DataSource = null;
            this.GroupCollection = null;
            this.ItemProperties = null;
            this.ItemType = null;
            this.Model = null;
            this.DataSourceList = null;
        }
    }
}
