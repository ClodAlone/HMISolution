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
using Syncfusion.RDL.DOM;
using System.Collections;
using System.ComponentModel;
using Syncfusion.RDL.Data;
using System.Reflection;

#if !WINRT
using Syncfusion.Windows.Data;
#endif

namespace Syncfusion.RDL.Internal
{
    class ReportingAggEngine
    {
        private IEnumerable dataSourceList = null;

        private object dataSource = null;

        public System.Type ItemType { get; set; }

        public List<DataField> Fields { get; set; }

        public ReportModel Model { get; set; }

        public List<ReportingEngineFieldValue> Values { get; set; }

        public bool IsEmptyDataSource { get; set; }

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
                return ((Syncfusion.RDL.Data.ReportData)o).Data[field];
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
            return this.ItemProperties[field].GetValue(o);
#endif
        }

        public object GetValue()
        {
            IEnumerable list = DataSourceList;

            SummaryBase summary = null;
            var expressFields = this.Model.ExpressionEngine.FieldValues;

            if (this.Fields.Count > 0)
            {
                summary = this.Fields.Last().GetSummaryInstance();

                foreach (object o in list)
                {
                    IsEmptyDataSource = false;

                    this.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();

                    foreach (var field in this.Fields.Where(f => f.Expression == null))
                    {
                        object value = this.GetValue(field.FieldName, o);
                        this.Model.ExpressionEngine.FieldValues.Add(field.Name, value);
                    }

                    foreach (var field in this.Fields.Where(f => f.Expression != null))
                    {
                        object value = this.Model.ExpressionEngine.GetEvalExpression(field.Expression);
                        this.Model.ExpressionEngine.FieldValues.Add(field.Name, value);
                    }

                    summary.Combine(this.Model.ExpressionEngine.FieldValues.Last().Value);
                }
            }

            this.Model.ExpressionEngine.FieldValues = expressFields;

            if (summary != null)
            {
                return summary.GetResult();
            }

            return null;
        }

        public void PopulateValue()
        {
            IEnumerable list = DataSourceList;
            var expressFields = this.Model.ExpressionEngine.FieldValues;

            if (this.Fields != null && this.Fields.Count > 0)
            {
                foreach (var field in this.Fields)
                {
                    field.Value = new ReportingEngineFieldValue() { Name = field.Name };
                    field.Value.Value = field.GetSummaryInstance();
                }

                foreach (object o in list)
                {
                    IsEmptyDataSource = false;

                    this.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();

                    foreach (var field in this.Fields.Where(f => f.Expression == null))
                    {
                        object value = this.GetValue(field.FieldName, o);
                        this.Model.ExpressionEngine.FieldValues.Add(field.Name, value);
                        field.Value.Value.Combine(value);
                    }

                    foreach (var field in this.Fields.Where(f => f.Expression != null))
                    {
                        object value = this.Model.ExpressionEngine.GetEvalExpression(field.Expression);
                        this.Model.ExpressionEngine.FieldValues.Add(field.Name, value);
                        field.Value.Value.Combine(value);
                    }

                    this.Model.ExpressionEngine.FieldValues.Clear();
                }
            }

            this.Model.ExpressionEngine.FieldValues = expressFields;
        }

        public void DisposeEngine()
        {
            this.DataSource = null;
            this.Model = null;
            this.DataSourceList = null;
            this.ItemProperties = null;

            if (this.Fields != null)
            {
                foreach (var field in this.Fields)
                {
                    if (field.Value != null)
                    {
                        field.Value.Value = null;
                    }

                    field.Value = null;
                }

                this.Fields.Clear();
                this.Fields = null;
            }

            if (this.Values != null)
            {
                this.Values.Clear();
            }
        }
    }

    internal class ReportingEngineFieldValue
    {
        public string Name { get; set; }

        public SummaryBase Value { get; set; }
    }

}
