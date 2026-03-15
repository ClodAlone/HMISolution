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

namespace Syncfusion.Windows.Forms.TreeMap
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
                    }
                    if (dataSourceList != null)
                    {
                        foreach (object o in DataSourceList)
                        {
                            ItemType = o.GetType();
                            break;
                        }
                    }
#if WINRT
                    itemProperties = ItemType.GetTypeInfo().DeclaredProperties;
#else
                    itemProperties = ItemType.GetProperties();
#endif

                    foreach (PropertyInfo prop in itemProperties)
                    {
#if WINRT
                        if (prop.PropertyType.IsConstructedGenericType)
#else
                        if (prop.PropertyType.IsGenericType)
#endif
                        {
                            HasNestedData = true;
                            break;
                        }
                    }
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

        private IEnumerable<PropertyInfo> itemProperties;

        /// <summary>
        /// Gets a PropertyDescriptorCollection describing the type of the underlying data item.
        /// </summary>
        public IEnumerable<PropertyInfo> ItemProperties
        {
            get
            {
                return itemProperties;
            }
        }

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
            if (ItemProperties.Any(info => info.Name == field))
                return ItemProperties.First(info => info.Name == field).GetValue(o, null);

            throw new ArgumentException("Specified Path (" + field + " ) does not exist in given DataSource");
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
                                                       SubItemsList = new List<object>(t.DataSources),
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
                                      Label = labelData!= null ? GetValue(labelData, sequenceEnum.Current).ToString() : null,
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