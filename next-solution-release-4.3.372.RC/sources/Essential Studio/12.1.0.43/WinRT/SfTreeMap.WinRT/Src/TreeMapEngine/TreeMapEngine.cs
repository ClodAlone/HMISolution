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
using System.Collections;
using System.Reflection;
using System.Collections.ObjectModel;
#if WPF
using System.Data;
using System.ComponentModel;
#endif

namespace Syncfusion.UI.Xaml.TreeMap
{
    internal class TreeMapEngine
    {
        private IEnumerable dataSourceList;

        private object dataSource;

        public Type ItemType { get; set; }

        public bool IsEmptyDataSource { get; set; }

        public bool HasNestedData { get; internal set; }

        public object DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;

                    //force list to be reset
                    if (DataSource is IEnumerable)
                    {
                        dataSourceList = DataSource as IEnumerable;
                        if (ItemProperties != null)
                        {
#if WPF
                            for (int i = 0; i < ItemProperties.Count; i++)
                            {
                                if (ItemProperties[i].PropertyType.IsGenericType)
#else
                            foreach (PropertyInfo propInfo in ItemProperties)
                            {
#if WINRT
                                if (propInfo.PropertyType.IsConstructedGenericType)
#else
                                if (propInfo.PropertyType.IsGenericType)
#endif
#endif
                                {
                                    HasNestedData = true;
                                    break;
                                }
                            }
                        }
                    }
#if WPF
                    else if (DataSource is DataTable)
                    {
                        dataSourceList = ((DataTable)DataSource).DefaultView as IEnumerable;
                        if ((DataSource as DataTable).ChildRelations.Count > 0)
                            HasNestedData = true;
                    }
                    else
                    {
                        dataSourceList = null;
                    }
#endif
                    if (dataSourceList != null)
                    {
                        foreach (object o in DataSourceList)
                        {
                            ItemType = o.GetType();
                            break;
                        }
                    }
#if !WPF
                    if (ItemType != null)
                    {
#if WINRT
                        itemProperties = ItemType.GetTypeInfo().DeclaredProperties;
#else
                        itemProperties = ItemType.GetProperties();
#endif
                    }
#endif
                }
            }
        }

        public virtual IEnumerable DataSourceList
        {
            get
            {
                return dataSourceList;
            }
            set
            {
                dataSourceList = value;
            }
        }

#if WPF
        private PropertyDescriptorCollection itemProperties;

        /// <summary>
        /// Gets a PropertyDescriptorCollection describing the type of the underlying data item.
        /// </summary>
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
#else
        private IEnumerable<PropertyInfo> itemProperties;
        /// <summary>
        /// Gets a PropertyInfo collection describing the type of the underlying data item.
        /// </summary>
        public IEnumerable<PropertyInfo> ItemProperties
        {
            get
            {
                return itemProperties;
            }
        }

#endif

        public TreeMapValueField ValueField
        {
            get;
            set;
        }

        public TreeMapValueField ColorField
        {
            get;
            set;
        }

        public object GetValue(string field, object o)
        {
#if WPF
            if (o is DataRow)
            {
                if (HasNestedData)
                {
                    //If datatable is relational datatable, then data are retrieved using relation defined for that datatable.
                    DataRelationCollection relations = (o as DataRow).Table.ChildRelations;
                    foreach (DataRelation relation in relations)
                    {
                        Dictionary<string, DataColumn> childColumns = relation.ChildColumns.ToDictionary(x => x.ColumnName, x => x);
                        //Child rows are retrieved based on relation which must be matched with field name.
                        if (childColumns.Keys.Contains(field))
                        {
                            childColumns.Clear();
                            childColumns = null;
                            return (o as DataRow).GetChildRows(relation);
                        }
                    }
                }
                else
                {
                    return (o as DataRow)[field];
                }
            }
            else if (o is DataRowView && (o as DataRowView)[field] != null)
                return (o as DataRowView)[field];
            //else if (ItemProperties.Count > 0 && ItemProperties[field] != null)
            //    return ItemProperties[field].GetValue(o);
            else if (ItemType.GetProperties().Any(info => info.Name == field))
#if SyncfusionFramework4_0 || SyncfusionFramework3_5
                return ItemType.GetProperties().First(info => info.Name == field).GetValue(o, null); 
#else
                return ItemType.GetProperties().First(info => info.Name == field).GetValue(o); 
#endif
#else
            if (ItemProperties.Any(info => info.Name == field))

#if WINRT
                return ItemProperties.First(info => info.Name == field).GetValue(o);
#else
                return ItemProperties.First(info => info.Name == field).GetValue(o, null);
#endif
#endif
            throw new ArgumentException("Specified Path (" + field + ") does not match with any fields in given DataSource");
        }

        public List<TreeMapItem> GetGroupItem(string field)
        {
            IsEmptyDataSource = true;
            var groupValues = new List<GroupValueInfo>();
            var groupInfoKeys = new BinaryList();

            IEnumerator iterator = DataSourceList.GetEnumerator();
            while (iterator.MoveNext())
            {
                IsEmptyDataSource = false;

                var groupKey = new KeysCalculationValues { Keys = new List<IComparable> { GetValue(field, iterator.Current) as IComparable } };

                int index = -1;
                var findKey = from grKey in groupInfoKeys
                              where ((KeysCalculationValues)grKey).Equals(groupKey)
                              select grKey;
                GroupValueInfo valueInfo;

                IEnumerable<IComparable> comparables = findKey as IComparable[] ?? findKey.ToArray();
                if (comparables.Any())
                {
                    index = groupInfoKeys.IndexOf(comparables.First());
                }

                if (index == -1)
                {
                    valueInfo = new GroupValueInfo();
                    groupInfoKeys.Add(groupKey);
                    valueInfo.GroupName = groupKey.ToString();
                    groupValues.Add(valueInfo);
                }
                else
                {
                    valueInfo = groupValues[index];
                }

                valueInfo.DataSources.Add(iterator.Current);

                SummaryBase valuesummaryBase, colorsummaryBase;

                if (valueInfo.Value == null)
                {
                    valuesummaryBase = valueInfo.Value = ValueField.GetSummaryInstance();
                    colorsummaryBase = valueInfo.ColorValue = ColorField.GetSummaryInstance();
                }
                else
                {
                    valuesummaryBase = valueInfo.Value;
                    colorsummaryBase = valueInfo.ColorValue;
                }

                if (ValueField.FieldName != null)
                    valuesummaryBase.Combine(GetValue(ValueField.FieldName, iterator.Current));
                if (ColorField.FieldName != null)
                    colorsummaryBase.Combine(GetValue(ColorField.FieldName, iterator.Current));
            }

            if (!IsEmptyDataSource)
            {
                return groupValues.Select(t => new TreeMapItem
                                                   {
                                                       Weight = (double)t.Value.GetResult(),
                                                       ColorWeight = (double)t.ColorValue.GetResult(),
                                                       SubItemsList = new ObservableCollection<object>(t.DataSources),
                                                       Data = t.DataSources,
                                                       Header = t.GroupName,
                                                       Label = t.GroupName
                                                   }).ToList();
            }
            return null;
        }

        public List<TreeMapItem> GetTreeMapItems(string valueData, string colorData, string labelData, string headerData)
        {
            var items = new List<TreeMapItem>();
            IsEmptyDataSource = true;

            if (DataSourceList != null)
            {
                var sequenceEnum = DataSourceList.GetEnumerator();
                while (sequenceEnum.MoveNext())
                {
                    IsEmptyDataSource = false;
                    items.Add(new TreeMapItem
                                  {
                                      Weight = Convert.ToDouble(GetValue(valueData, sequenceEnum.Current)),
                                      ColorWeight = Convert.ToDouble(GetValue(colorData, sequenceEnum.Current)),
                                      Label = labelData != null ? GetValue(labelData, sequenceEnum.Current).ToString() : null,
                                      Header = headerData != null ? GetValue(headerData, sequenceEnum.Current).ToString() : null,
                                      Data = sequenceEnum.Current,
                                  });
                }
            }
            return items;
        }

        public List<TreeMapLeafNode> GetTreeMapLeafNodes(string valueData, string colorData, string labelData)
        {
            var items = new List<TreeMapLeafNode>();
            IsEmptyDataSource = true;

            var sequenceEnum = DataSourceList.GetEnumerator();
            while (sequenceEnum.MoveNext())
            {
                IsEmptyDataSource = false;
                items.Add(new TreeMapLeafNode
                {
                    Data = sequenceEnum.Current,
                    Label = labelData != null ? Convert.ToString(GetValue(labelData, sequenceEnum.Current)) : null,
                    Weight = valueData != null ? Convert.ToDouble(GetValue(valueData, sequenceEnum.Current)) : 0,
                    ColorWeight = colorData != null ? Convert.ToDouble(GetValue(colorData, sequenceEnum.Current)) : 0
                });
            }
            return items;
        }
    }
}