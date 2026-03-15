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
using System.Linq;
using System.Reflection;
using System.Text;
using Syncfusion.RDL.ItemModel;

namespace Syncfusion.RDL.Internal
{
    internal class MapEngine : IDisposable
    {
        private IEnumerable dataSourceList = null;

        private object dataSource = null;

        public System.Type ItemType { get; set; }

        public MapModel Model { get; set; }

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

        private object GetValue(string field, object o)
        {
#if !SILVERLIGHT && !WINRT
            if (o is System.Data.DataRowView)
            {
                var row = (o as System.Data.DataRowView).Row;
                return row[field];
            }
            else
#endif
            {
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
                else if (o.GetType().FullName == "Syncfusion.Reports.Server.ReportData")
                {
#if WINRT
                    System.Reflection.PropertyInfo pi = o.GetType().GetRuntimeProperty("Data");
#else
                    System.Reflection.PropertyInfo pi = o.GetType().GetProperty("Data");
#endif
                    if (pi.GetValue(o, null).GetType() == typeof(Dictionary<string, object>))
                    {
                        Dictionary<string, object> objectName = (Dictionary<string, object>)((object)(pi.GetValue(o, null)));
                        return objectName[field];
                    }
                    else
                    {
                        return null;
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
                else
                {
                    try
                    {
#if SILVERLIGHT
                     return this.ItemProperties.Where(t=>t.Name == field).First().GetValue(o,null);
#else
                     return this.ItemProperties[field].GetValue(o);
#endif

                    }
                    catch (Exception)
                    {
                        return null;
                    }
                    
                }
            }
        }

        Dictionary<string, object> GetRecords(object o)
        {
            Dictionary<string,object> val = new Dictionary<string, object>();
#if !SILVERLIGHT && !WINRT
            if (o is System.Data.DataRowView)
            {
                var items = (o as System.Data.DataRowView).Row.ItemArray;
                var row = (o as System.Data.DataRowView).Row;
                for (int pos = 0; pos < items.Length; pos++)
                {
                    val.Add(row.Table.Columns[pos].ColumnName, items[pos]);
                }
                return val;
            }
            else
#endif
            {
                if (o is Syncfusion.RDL.Data.ReportData)
                {
                    try
                    {
                        return ((Syncfusion.RDL.Data.ReportData)o).Data;
                    }
                    catch
                    {
                        return val;
                    }
                }
                else if (o.GetType().FullName == "Syncfusion.Reports.Server.ReportData")
                {
#if WINRT
                    System.Reflection.PropertyInfo pi = o.GetType().GetRuntimeProperty("Data");
#else
                    System.Reflection.PropertyInfo pi = o.GetType().GetProperty("Data");
#endif
                    if (pi.GetValue(o, null).GetType() == typeof(Dictionary<string, object>))
                    {
                        Dictionary<string, object> objectName = (Dictionary<string, object>)((object)(pi.GetValue(o, null)));
                        return objectName;
                    }
                    else
                    {
                        return val;
                    }
                }
#if !SyncfusionFramework3_5
                else if (o is IDictionary<string, object> && o is System.Dynamic.ExpandoObject)
                {
                    return new Dictionary<string, object>((o as IDictionary<string, object>));
                }
#endif
                else
                {
                    try
                    {
                        for (int i = 0; i < this.ItemProperties.Count; i++)
                        {
#if SILVERLIGHT 
                            val.Add(this.ItemProperties[i].Name, this.ItemProperties[i].GetValue(o,null));
#else
                            val.Add(this.ItemProperties[i].Name, this.ItemProperties[i].GetValue(o));
#endif
                        }
                    }
                    catch (Exception)
                    {
                        return val;
                    }
                    
                }
            }
            return val;
        }

        internal List<ReportingMapData> PopulateMapValues()
        {
            List<ReportingMapData> reportingMapDatas = new List<ReportingMapData>();
            IEnumerable list = DataSourceList;
            var mapInfo = this.Model.MapEngineInfo;

            var engineOldValue = this.Model.Model.ExpressionEngine.FieldValues;

            this.Model.Model.ExpressionEngine.FieldValues = new Dictionary<string, object>();
            if (list != null)
            {
                foreach (object o in list)
                {
                    ReportingMapData mapData = new ReportingMapData();

                    this.Model.Model.ExpressionEngine.FieldValues = this.GetRecords(o);

                    if (mapInfo.MapFieldValues != null)
                    {
                        var field = mapInfo.MapFieldValues.First();
                        mapData.Value = this.Model.Model.ExpressionEngine.GetEvalExpression(field);
                    }

                    if (mapInfo.MapDisplayLables != null)
                    {
                        var field = mapInfo.MapDisplayLables.First();
                        mapData.DisplayLabel = this.Model.Model.ExpressionEngine.GetEvalExpression(field);
                    }

                    if (mapInfo.MapBubbleLables != null)
                    {
                        var field = mapInfo.MapBubbleLables.First();
                        mapData.BubbleValuePath = this.Model.Model.ExpressionEngine.GetEvalExpression(field);
                    }

                    if (mapInfo.MapColorLables != null)
                    {
                        var field = mapInfo.MapColorLables.First();
                        mapData.ColorValuePath = this.Model.Model.ExpressionEngine.GetEvalExpression(field);
                    }

                    if (mapInfo.MapColorRule != null)
                    {
                        var field = mapInfo.MapColorRule.First();
                        mapData.ColorRangeValue = this.Model.Model.ExpressionEngine.GetEvalExpression(field);
                    }

                    if (mapInfo.MapShapeActions != null)
                    {
                        TextboxActionInfoExpVal actionInfo = new TextboxActionInfoExpVal();
                        var field = mapInfo.MapShapeActions;
                        if (!string.IsNullOrEmpty(field.Hyperlink))
                        {
                            actionInfo.Hyperlink =
                                this.Model.Model.ExpressionEngine.GetEvalExpressionString(field.Hyperlink);
                        }
                        else if (!string.IsNullOrEmpty(field.BookmarkLink))
                        {
                            actionInfo.BookmarkLink =
                                this.Model.Model.ExpressionEngine.GetEvalExpressionString(field.BookmarkLink);
                        }
                        else if (!string.IsNullOrEmpty(field.ReportName))
                        {
                            actionInfo.ReportName =
                                this.Model.Model.ExpressionEngine.GetEvalExpressionString(field.ReportName);

                            if (field.Parameters != null && field.Parameters.Count > 0)
                            {
                                List<TextboxParameterExpVal> parameters = new List<TextboxParameterExpVal>();
                                foreach (var parameter in field.Parameters)
                                {
                                    TextboxParameterExpVal para = new TextboxParameterExpVal();
                                    para.Name = this.Model.Model.ExpressionEngine.GetEvalExpressionString(parameter.Name);
                                    para.Omit = this.Model.Model.ExpressionEngine.GetEvalExpressionString(parameter.Omit);
                                    para.Value =
                                        this.Model.Model.ExpressionEngine.GetEvalExpressionString(parameter.Value);
                                    parameters.Add(para);
                                }
                                actionInfo.Parameters = parameters;
                            }
                        }
                        mapData.ShapeActionInfo = actionInfo;
                    }

                    reportingMapDatas.Add(mapData);
                }
            }

            this.Model.Model.ExpressionEngine.FieldValues = engineOldValue;

            return reportingMapDatas;
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

    internal class MapInfo
    {
        public List<string> MapFieldValues
        {
            get; 
            set;
        }

        public List<string> MapDisplayLables
        {
            get;
            set;
        }

        public List<string> MapBubbleLables
        {
            get;
            set;
        }

        public List<string> MapColorLables
        {
            get;
            set;
        }

        public List<string> MapColorRule
        {
            get; 
            set;
        }

        public TextboxActionInfoExp MapShapeActions
        {
            get;
            set;
        }
    }
}
