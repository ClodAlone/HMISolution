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
using System.ComponentModel;
using System.Collections;
using System.Data;
using Syncfusion.PivotAnalysis.Base;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    public class FilterHelper
    {
        PivotGridControlBase tableControl;
        public FilterHelper()
        {
        }

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="grid"></param>
        public FilterHelper(PivotGridControlBase grid)
        {
            this.tableControl = grid;
        }

        /// <summary>
        /// Helper function which used to return only the filtered items of the field
        /// </summary>
        internal FilterItemsCollection GetFilterItem(PropertyDescriptor propertyDescriptor)
        {
            FilterItemsCollection filterItemsCollection = new FilterItemsCollection();
            IEnumerable itemSource = null;
            if (this.tableControl.ItemSource is System.Data.DataTable)
                itemSource = (this.tableControl.ItemSource as System.Data.DataTable).DefaultView;
            else
                itemSource = this.tableControl.ItemSource as IEnumerable;
            if (itemSource != null && !(itemSource is DataView))
            {
                foreach (var item in itemSource)
                {
                    filterItemsCollection.FilterProperty = propertyDescriptor;
                    if (propertyDescriptor.PropertyType == typeof(int) ||
                        propertyDescriptor.PropertyType == typeof(double) ||
                        propertyDescriptor.PropertyType == typeof(float) ||
                        propertyDescriptor.PropertyType == typeof(decimal) ||
                        propertyDescriptor.PropertyType == typeof(short) ||
                        propertyDescriptor.PropertyType == typeof(long) ||
                        propertyDescriptor.PropertyType == typeof(DateTime))
                    {
                        bool allow = this.tableControl.PivotEngine.Filters.Count == 0 ? true : false;
                        for (int i = 0; i < this.tableControl.PivotEngine.Filters.Count; i++)
                        {
                            allow = bool.Parse(this.tableControl.PivotEngine.Filters[i].Evaluator.DynamicInvoke(item).ToString());
                        }
                        if (allow)
                            filterItemsCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item).ToString() });
                    }

                    else
                    {
                        bool allow = this.tableControl.PivotEngine.Filters.Count == 0 ? true : false;
                        for (int i = 0; i < this.tableControl.PivotEngine.Filters.Count; i++)
                        {
                            allow = bool.Parse(this.tableControl.PivotEngine.Filters[i].Evaluator.DynamicInvoke(item).ToString());
                        }


                        if (allow)
                        {
                            if (propertyDescriptor is ExpressionPropertyDescriptor)
                            {
                                string s = propertyDescriptor.GetValue(item).ToString();
                                filterItemsCollection.AddIfUnique(new FilterItemElement { Key = s });
                            }
                            else
                                filterItemsCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item) as string });
                        }
                    }

                }

            }
            else
            {
                if (this.tableControl.ItemSource is DataTable || this.tableControl.ItemSource is DataView)
                {
                    DataView source = this.tableControl.ItemSource is DataView ? ((DataView)this.tableControl.ItemSource) : ((DataTable)this.tableControl.ItemSource).DefaultView;

                    string s = "";
                    if (!string.IsNullOrEmpty(source.RowFilter))
                        s = source.RowFilter;
                    foreach (FilterExpression exp in this.tableControl.PivotEngine.Filters)
                    {
                        if (s.Length > 0)
                        {
                            s += " AND " + "(" + exp.Expression + ")";
                        }
                        else
                        {
                            s = "(" + exp.Expression + ")";
                        }
                    }
                    using (DataView dv = new DataView(source.Table, s, "", DataViewRowState.CurrentRows))
                    {
                        foreach (var item in dv)
                        {
                            filterItemsCollection.FilterProperty = propertyDescriptor;
                            filterItemsCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item).ToString() });

                        }
                    }


                }

            }
            filterItemsCollection.Reverse();
            return filterItemsCollection;
        }

        /// <summary>
        /// A helper function which used to return all the items bounded with the item source
        /// </summary>
        internal FilterItemsCollection RequireFilterItem(PropertyDescriptor propertyDescriptor)
        {
            FilterItemsCollection filterItemsCollection = new FilterItemsCollection();
            IEnumerable itemSource = null;
            if (tableControl.ItemSource is System.Data.DataTable)
                itemSource = (tableControl.ItemSource as System.Data.DataTable).DefaultView;
            else
                itemSource = tableControl.ItemSource as IEnumerable;
            if (itemSource != null && !(itemSource is DataView))
            {
                foreach (var item in itemSource)
                {
                    filterItemsCollection.FilterProperty = propertyDescriptor;
                    if (propertyDescriptor.PropertyType == typeof(int) ||
                        propertyDescriptor.PropertyType == typeof(double) ||
                        propertyDescriptor.PropertyType == typeof(float) ||
                        propertyDescriptor.PropertyType == typeof(decimal) ||
                        propertyDescriptor.PropertyType == typeof(short) ||
                        propertyDescriptor.PropertyType == typeof(long) ||
                        propertyDescriptor.PropertyType == typeof(DateTime))
                    {
                        bool allow = tableControl.PivotEngine.Filters.Count == 0 ? true : false;
                        for (int i = 0; i < tableControl.PivotEngine.Filters.Count; i++)
                        {
                            allow = bool.Parse(tableControl.PivotEngine.Filters[i].Evaluator.DynamicInvoke(item).ToString());
                        }
                        if (allow)
                            filterItemsCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item).ToString() });
                    }

                    else
                    {
                        if (propertyDescriptor is ExpressionPropertyDescriptor)
                        {
                            string s = propertyDescriptor.GetValue(item).ToString();
                            filterItemsCollection.AddIfUnique(new FilterItemElement { Key = s });
                        }
                        else
                            filterItemsCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item) as string });
                    }

                }

            }
            else
            {
                if (tableControl.ItemSource is DataTable || tableControl.ItemSource is DataView)
                {
                    DataView source = tableControl.ItemSource is DataView ? ((DataView)tableControl.ItemSource) : ((DataTable)tableControl.ItemSource).DefaultView;

                    string s = "";
                    if (!string.IsNullOrEmpty(source.RowFilter))
                        s = source.RowFilter;
                    foreach (FilterExpression exp in tableControl.PivotEngine.Filters)
                    {
                        if (s.Length > 0)
                        {
                            s += " AND " + "(" + exp.Expression + ")";
                        }
                        else
                        {
                            s = "(" + exp.Expression + ")";
                        }
                    }
                    using (DataView dv = new DataView(source.Table, s, "", DataViewRowState.CurrentRows))
                    {
                        foreach (var item in dv)
                        {
                            filterItemsCollection.FilterProperty = propertyDescriptor;
                            filterItemsCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item).ToString() });

                        }
                    }
                }
            }
            filterItemsCollection.Reverse();
            return filterItemsCollection;
        }

        /// <summary>
        /// To get the property descriptor of a field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        internal PropertyDescriptor GetPropertyDescriptor(string fieldName)
        {

            if (this.tableControl.ItemSource != null && this.tableControl.PivotEngine != null && this.tableControl.PivotEngine.ItemProperties != null)
            {
                PivotItem item;
                this.tableControl.fieldNameCollectionTablelist.TryGetValue(fieldName,out item);
                if (!this.tableControl.fieldNameCollectionTablelist.TryGetValue(fieldName, out item))
                {
                    if (!this.tableControl.collectionTablelist.TryGetValue(fieldName, out item))
                    {
                        PivotComputationInfo compute;
                        this.tableControl.pivotComputationCollection.TryGetValue(fieldName, out compute);
                        if (compute == null)
                        {
                            PivotItem newItem = new PivotItem();
                            string passedValue = null;
                            this.tableControl.completeTablelist.TryGetValue(fieldName, out newItem);
                            if (String.IsNullOrEmpty(newItem.FieldHeader))
                            {
                                newItem.FieldHeader = newItem.FieldMappingName;
                                passedValue = newItem.FieldMappingName;
                            }
                            else
                                passedValue = newItem.FieldHeader;
                            return this.tableControl.PivotEngine.ItemProperties[passedValue];
                        }
                        return this.tableControl.PivotEngine.ItemProperties[compute.FieldName];
                    }
                }
                return this.tableControl.PivotEngine.ItemProperties[item.FieldMappingName];
            }
            else
                return null;

        }
    }
}
