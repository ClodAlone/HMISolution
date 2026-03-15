//-------------------------------------------------------------------------------------------------
// <copyright file="PassThroughGroupingResult.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// An interface for pass through grouping support in the grouping engine. If you
    /// assign a datasource which implements this interface as datasource for the
    /// grouping engine, the engine will interact with the datasource through this
    /// interface and query groups details and nested tables on demand only.
    /// </summary>
    public interface IPassThroughGroupingResult : IEnumerable, ITypedList
    {
        /// <summary>
        /// Returns the fields the query is grouped by. A query can be grouped by multiple levels.
        /// </summary>
        string[] GroupByColumns { get; }

        /// <summary>
        /// Returns the data type of the items in the data source. This information is used for instantiating and
        /// adding new items to the data source.
        /// </summary>
        /// <returns>The data type of the items in the datasource.</returns>
        Type GetListItemType();

        /// <summary>
        /// Returns the items for a given group. The items can either be nested groups or records.
        /// </summary>
        /// <param name="group">The group in the grouping engine.</param>
        /// <param name="item">The item the group is associated with.</param>
        /// <returns>The items for a given group.</returns>
        IEnumerable GetItems(Group group, object item);

        /// <summary>
        /// Returns the child count for a given group. This is the count that is displayed in the caption.
        /// </summary>
        /// <param name="group">The group in the grouping engine.</param>
        /// <param name="item">The item the group is associated with.</param>
        /// <returns>The child count for a given group.</returns>
        int GetItemCount(Group group, object item);

        /// <summary>
        /// Returns the category of a new group that will be associated with the given item.
        /// </summary>
        /// <param name="column">A name that matches one of the strings return by <see cref="GroupByColumns"/></param>
        /// <param name="item">The item the group is associated with.</param>
        /// <returns>The group category for the given item.</returns>
        object GetGroupByKey(string column, object item);

        /// <summary>
        /// Returns totals for the whole table. It can be an integer that represents the count that is displayed 
        /// in the caption for the TopLevelGroup.
        /// You can also return a object which contains summaries for the whole table.
        /// </summary>
        /// <returns>returns Totals.</returns>
        object GetTotals();

        /// <summary>
        /// Returns the nested items for a parent record when it is expanded. In the implementation
        /// for this method you should execute a query on the related table and match the records
        /// to the criteria specified using the keys argument and the RelationDescriptor information
        /// specified with the rd argument.
        /// </summary>
        /// <param name="rd">The RelationDescriptor that describes the relation</param>
        /// <param name="childTable">The child table where the new items will be added after this method returns.</param>
        /// <param name="keys">The identifier for the nested records</param>
        /// <param name="totals">The number of child elements in the nested table or an object with summaries.</param>
        /// <returns>The results of the query for the nested records.</returns>
        IEnumerable GetNestedItems(RelationDescriptor rd, ChildTable childTable, object[] keys, out object totals);

        /// <summary>
        /// Returns the first few items of this result. The GridGroupingControl
        /// will loop through this items to determine the optimum width of columns.
        /// </summary>
        /// <returns>First few items.</returns>
        IEnumerable GetSampleItems();
    }

    /// <summary>
    /// A delegate that allows the <see cref="PassThroughGroupingResult"/> to call back and
    /// retrieve nested tables with totals on demand.
    /// </summary>
    /// <param name="keys">The object array</param>
    /// <param name="totals">The totals</param>
    /// <returns>returns IEnumerable</returns>
    public delegate IEnumerable NestedQueryResultsDelegate(object[] keys, out object totals);

    /// <summary>
    /// A delegate that allows the <see cref="PassThroughGroupingResult"/> to call back and
    /// retrieve nested groups details on demand.
    /// </summary>
    /// <param name="group">The group value</param>
    /// <returns>returns IEnumerable</returns>
    public delegate IEnumerable QueryGroupsDetailsDelegate(Group group);

    /// <summary>
    /// A default implementation of the <see cref="IPassThroughGroupingResult"/> with
    /// support for Linq queries. See the LinqGroupBy example.
    /// </summary>
    public class PassThroughGroupingResult : IPassThroughGroupingResult
    {
        #region Fields
        IEnumerable results;

        IRecordUpdateHelper updateHelper;
        PropertyDescriptorCollection properties;
        string name;
        string[] groupByColumns;
        object totals;
        NestedQueryResultsDelegate nestedQueryHandler;
        QueryGroupsDetailsDelegate groupsDetailsHandler;
        IEnumerable sampleQuery;
        string keyProperty = "Key";
        string captionProperty = "Caption";
        string countProperty = "Count";
        string detailsProperty = "Details";
        Table table;
        Type listItemType;
        #endregion

        #region Ctor
        /// <summary>
        /// Constructor for PassThroughGroupingResult.
        /// </summary>
        /// <param name="name">A name for this collection.</param>
        /// <param name="results">The collection that stores the grouping results.</param>
        /// <param name="itemType">The data type of the items in this collection.</param>
        /// <param name="totals">The Totals.</param>
        /// <param name="groupByColumns">The group by columns.</param>
        public PassThroughGroupingResult(string name, IEnumerable results, Type itemType, object totals, params string[] groupByColumns)
        {
            this.name = name;
            this.results = results;
            this.listItemType = itemType;
            this.properties = TypeDescriptor.GetProperties(itemType);
            this.groupByColumns = groupByColumns;
            this.totals = totals;
        }

        /// <summary>
        /// Constructor for PassThroughGroupingResult.
        /// </summary>
        /// <param name="name">A name for this collection.</param>
        /// <param name="nestedQueryHandler">A <see cref="NestedQueryResultsDelegate"/> to retrieve the nested tables with totals on demand.</param>
        /// <param name="itemType">The data type of the items in this collection.</param>
        /// <param name="groupByColumns">The group by columns.</param>
        public PassThroughGroupingResult(string name, NestedQueryResultsDelegate nestedQueryHandler, Type itemType, params string[] groupByColumns)
        {
            this.name = name;
            this.nestedQueryHandler = nestedQueryHandler;
            this.listItemType = itemType;
            this.properties = TypeDescriptor.GetProperties(itemType);
            this.groupByColumns = groupByColumns;
            this.totals = null;
        }

        /// <summary>
        /// Constructor for PassThroughGroupingResult.
        /// </summary>
        /// <param name="name">A name for this collection.</param>
        /// <param name="results">The collection that stores the grouping results.</param>
        /// <param name="itemType">The data type of the items in this collection.</param>
        /// <param name="totals">The Totals.</param>
        /// <param name="sampleResults">Returns first few items of the results.</param>
        /// <param name="groupByColumns">The group by columns.</param>
        public PassThroughGroupingResult(string name, IEnumerable results, Type itemType, object totals, IEnumerable sampleResults, params string[] groupByColumns)
            : this(name, results, itemType, totals, groupByColumns)
        {
            this.sampleQuery = sampleResults;
        }

        /// <summary>
        /// Constructor for PassThroughGroupingResult.
        /// </summary>
        /// <param name="name">A name for this collection.</param>
        /// <param name="results">The collection that stores the grouping results.</param>
        /// <param name="itemType">The data type of the items in this collection.</param>
        /// <param name="totals">The Totals.</param>
        /// <param name="groupsDetailsHandler">The <see cref="QueryGroupsDetailsDelegate"/> that retrieves the nested group details on demand.</param>
        /// <param name="sampleResults">Returns first few items of the results.</param>
        /// <param name="groupByColumns">The group by columns.</param>
        public PassThroughGroupingResult(string name, IEnumerable results, Type itemType, object totals, QueryGroupsDetailsDelegate groupsDetailsHandler, IEnumerable sampleResults, params string[] groupByColumns)
            : this(name, results, itemType, totals, groupByColumns)
        {
            this.sampleQuery = sampleResults;
            this.groupsDetailsHandler = groupsDetailsHandler;
        }

        /// <summary>
        /// Constructor for PassThroughGroupingResult.
        /// </summary>
        /// <param name="name">A name for this collection.</param>
        /// <param name="nestedQueryHandler">A <see cref="NestedQueryResultsDelegate"/> to retrieve the nested tables with totals on demand.</param>
        /// <param name="itemType">The data type of the items in this collection.</param>
        /// <param name="sampleResults">Returns first few items of the results.</param>
        /// <param name="groupByColumns">The group by columns.</param>
        public PassThroughGroupingResult(string name, NestedQueryResultsDelegate nestedQueryHandler, Type itemType, IEnumerable sampleResults, params string[] groupByColumns)
            : this(name, nestedQueryHandler, itemType, groupByColumns)
        {
            this.sampleQuery = sampleResults;
        }

        /// <summary>
        /// Constructor for PassThroughGroupingResult.
        /// </summary>
        /// <param name="name">A name for this collection.</param>
        /// <param name="nestedQueryHandler">A <see cref="NestedQueryResultsDelegate"/> to retrieve the nested tables with totals on demand.</param>
        /// <param name="itemType">The data type of the items in this collection.</param>
        /// <param name="groupsDetailsHandler">The <see cref="QueryGroupsDetailsDelegate"/> that retrieves the nested group details on demand.</param>
        /// <param name="sampleResults">Returns first few items of the results.</param>
        /// <param name="groupByColumns">The group by columns.</param>
        public PassThroughGroupingResult(string name, NestedQueryResultsDelegate nestedQueryHandler, Type itemType, QueryGroupsDetailsDelegate groupsDetailsHandler, IEnumerable sampleResults, params string[] groupByColumns)
            : this(name, nestedQueryHandler, itemType, groupByColumns)
        {
            this.sampleQuery = sampleResults;
            this.groupsDetailsHandler = groupsDetailsHandler;
        }

        /// <summary>
        /// Constructor for PassThroughGroupingResult.
        /// </summary>
        /// <param name="name">A name for this collection.</param>
        /// <param name="results">The collection that stores the grouping results.</param>
        /// <param name="itemType">The data type of the items in this collection.</param>
        /// <param name="totals">The Totals.</param>
        /// <param name="sampleResults">Returns first few items of the results.</param>
        /// <param name="updateHelper">A <see cref="IRecordUpdateHelper"/>.</param>
        /// <param name="groupByColumns">The group by columns.</param>
        public PassThroughGroupingResult(string name, IEnumerable results, Type itemType, object totals, IEnumerable sampleResults, IRecordUpdateHelper updateHelper, params string[] groupByColumns)
            : this(name, results, itemType, totals, groupByColumns)
        {
            this.sampleQuery = sampleResults;
            this.updateHelper = updateHelper;
        }

        /// <summary>
        /// Constructor for PassThroughGroupingResult.
        /// </summary>
        /// <param name="name">A name for this collection.</param>
        /// <param name="results">The collection that stores the grouping results.</param>
        /// <param name="itemType">The data type of the items in this collection.</param>
        /// <param name="totals">The Totals.</param>
        /// <param name="sampleResults">Returns first few items of the results.</param>
        /// <param name="groupsDetailsHandler">The <see cref="QueryGroupsDetailsDelegate"/> that retrieves the nested group details on demand.</param>
        /// <param name="updateHelper">A <see cref="IRecordUpdateHelper"/>.</param>
        /// <param name="groupByColumns">The group by columns.</param>
        public PassThroughGroupingResult(string name, IEnumerable results, Type itemType, object totals, IEnumerable sampleResults, QueryGroupsDetailsDelegate groupsDetailsHandler, IRecordUpdateHelper updateHelper, params string[] groupByColumns)
            : this(name, results, itemType, totals, groupByColumns)
        {
            this.sampleQuery = sampleResults;
            this.updateHelper = updateHelper;
            this.groupsDetailsHandler = groupsDetailsHandler;
        }

        /// <summary>
        /// Constructor for PassThroughGroupingResult.
        /// </summary>
        /// <param name="name">A name for this collection.</param>
        /// <param name="nestedQueryHandler">A <see cref="NestedQueryResultsDelegate"/> to retrieve the nested tables with totals on demand.</param>
        /// <param name="itemType">The data type of the items in this collection.</param>
        /// <param name="sampleResults">Returns first few items of the results.</param>
        /// <param name="updateHelper">A <see cref="IRecordUpdateHelper"/>.</param>
        /// <param name="groupByColumns">The group by columns.</param>
        public PassThroughGroupingResult(string name, NestedQueryResultsDelegate nestedQueryHandler, Type itemType, IEnumerable sampleResults, IRecordUpdateHelper updateHelper, params string[] groupByColumns)
            : this(name, nestedQueryHandler, itemType, groupByColumns)
        {
            this.sampleQuery = sampleResults;
            this.updateHelper = updateHelper;
        }

        /// <summary>
        /// Constructor for PassThroughGroupingResult.
        /// </summary>
        /// <param name="name">A name for this collection.</param>
        /// <param name="nestedQueryHandler">A <see cref="NestedQueryResultsDelegate"/> to retrieve the nested tables with totals on demand.</param>
        /// <param name="itemType">The data type of the items in this collection.</param>
        /// <param name="groupsDetailsHandler">The <see cref="QueryGroupsDetailsDelegate"/> that retrieves the nested group details on demand.</param>
        /// <param name="sampleResults">Returns first few items of the results.</param>
        /// <param name="updateHelper">A <see cref="IRecordUpdateHelper"/>.</param>
        /// <param name="groupByColumns">The group by columns.</param>
        public PassThroughGroupingResult(string name, NestedQueryResultsDelegate nestedQueryHandler, Type itemType, QueryGroupsDetailsDelegate groupsDetailsHandler, IEnumerable sampleResults, IRecordUpdateHelper updateHelper, params string[] groupByColumns)
            : this(name, nestedQueryHandler, itemType, groupByColumns)
        {
            this.sampleQuery = sampleResults;
            this.updateHelper = updateHelper;
            this.groupsDetailsHandler = groupsDetailsHandler;
        }
        #endregion

        /// <summary>
        /// Applies the given settings to the current object. 
        /// </summary>
        /// <param name="results">Pass through grouping results.</param>
        /// <param name="totals">The Totals.</param>
        /// <param name="groupByColumns">The group by columns.</param>
        public void ChangeResults(IEnumerable results, object totals, params string[] groupByColumns)
        {
            bool changed = false;
            if (!Object.ReferenceEquals(results, this.results))
            {
                changed = true;
                this.results = results;
            }

            if (!Object.ReferenceEquals(totals, this.totals))
            {
                if (totals != null && this.totals != null && totals.GetType().IsPrimitive
                    && totals.Equals(this.totals))
                {
                }
                else
                {
                    changed = true;
                    this.totals = totals;
                }
            }

            if (!Object.ReferenceEquals(groupByColumns, this.groupByColumns))
            {
                changed = true;
                this.groupByColumns = groupByColumns;
            }

            if (changed)
            {
                Refresh();
            }
        }

        private void EnsureTable(Group group)
        {
            if (table == null)
            {
                table = group.ParentTable;
                if (updateHelper != null)
                {
                    table.CurrentRecordManager.UpdateHelper = updateHelper;
                }
            }
        }

        public IRecordUpdateHelper UpdateHelper
        {
            get 
            {
                return this.updateHelper; 
            }
          
            set
            {
                this.updateHelper = value;
                if (table != null && updateHelper != null)
                {
                    table.CurrentRecordManager.UpdateHelper = updateHelper;
                }
            }
        }

        /// <summary>
        /// Refreshes the control.
        /// </summary>
        public void Refresh()
        {
            if (table != null)
            {
                table.EngineTable.TableDirty = true;
                table.TableDirty = true;
            }
        }

        #region Properties
        public string KeyProperty
        {
            get { return keyProperty; }
            set { keyProperty = value; }
        }

        public string CaptionProperty
        {
            get { return captionProperty; }
            set { captionProperty = value; }
        }

        public string CountProperty
        {
            get { return countProperty; }
            set { countProperty = value; }
        }

        public string DetailsProperty
        {
            get { return detailsProperty; }
            set { detailsProperty = value; }
        }

        public NestedQueryResultsDelegate NestedQueryHandler
        {
            get { return nestedQueryHandler; }
            set { nestedQueryHandler = value; }
        }
        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns the enumerator for the entire collection.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return results.GetEnumerator();
        }

        #endregion

        #region ITypedList Members

        /// <summary>
        /// Returns the <see cref="PropertyDescriptorCollection"/> that represents the properties on each item used to bind data. 
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="PropertyDescriptor"/> objects to find in the collection as bindable.</param>
        /// <returns>The <see cref="PropertyDescriptorCollection"/>.</returns>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return properties;
        }

        /// <summary>
        /// Gets the name of the list.
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="PropertyDescriptor"/> objects, for which the list name is returned.</param>
        /// <returns>Name of the list.</returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return name;
        }

        #endregion

        #region IPassThroughGroupingResult Members

        /// <summary>
        /// Returns the fields the query is grouped by. A query can be grouped by multiple levels.
        /// </summary>
        public string[] GroupByColumns
        {
            get { return groupByColumns; }
        }

        /// <summary>
        /// Returns the items for a given group. The items can either be nested groups or records.
        /// </summary>
        /// <param name="group">The group in the grouping engine.</param>
        /// <param name="item">The item the group is associated with.</param>
        /// <returns>The items for a given group.</returns>
        public IEnumerable GetItems(Group group, object item)
        {
            IEnumerable items;

            EnsureTable(group);

            if (groupsDetailsHandler != null)
            {
                items = groupsDetailsHandler(group);
                return items;
            }

            items = GetValue(item, DetailsProperty) as IEnumerable;
            if (items != null)
            {
                return items;
            }

            //// Try known Fields and Methods for IGrouping<T,K> type.

            FieldInfo fi = item.GetType().GetField("items", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {
                items = fi.GetValue(item) as IEnumerable;
                if (items != null)
                {
                    return items;
                }
            }

            if (item is IEnumerable)
            {
                return (IEnumerable)item;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the child count for a given group. This is the count that is displayed in the caption.
        /// </summary>
        /// <param name="group">The group in the grouping engine.</param>
        /// <param name="item">The item the group is associated with.</param>
        /// <returns>The child count for a given group.</returns>
        public int GetItemCount(Group group, object item)
        {
            EnsureTable(group);

            if (item == null)
            {
                return 0;
            }

            // when GetNestedItems returned Totals as out parameter, the Tables RaiseRecordExpanding
            // method sets the Count as item in the child table. This is the count that gets displayed
            // in the caption bar.
            if (item is int)
            {
                return (int)item;
            }

            object p;

            if (GetValue(item, CountProperty, out p))
            {
                return (int)p;
            }

            //// Try known Fields and Methods for IGrouping<T,K> type.

            MethodInfo mi = item.GetType().GetMethod("Count", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi != null)
            {
                return (int)mi.Invoke(item, null);
            }

            FieldInfo fi = item.GetType().GetField("items", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {
                IList items = fi.GetValue(item) as IList;
                if (items != null)
                {
                    return items.Count;
                }
            }

            throw new InvalidOperationException("Could not find Count property");
        }

        /// <summary>
        /// Returns the category of a new group that will be associated with the given item.
        /// </summary>
        /// <param name="column">A name that matches one of the strings return by <see cref="GroupByColumns"/></param>
        /// <param name="item">The item the group is associated with.</param>
        /// <returns>The group category for the given item.</returns>
        public object GetGroupByKey(string column, object item)
        {
            object key;

            if (GetValue(item, CaptionProperty, out key))
            {
                return key;
            }

            if (GetValue(item, KeyProperty, out key))
            {
                return key;
            }

            //// Try known Fields and Methods for IGrouping<T,K> type.

            FieldInfo fi = item.GetType().GetField("key", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)           
            {   
                return fi.GetValue(item); 
            }

            throw new InvalidOperationException("Could not find Key property");
        }

        /// <summary>
        /// Gets the current value of the given property.
        /// </summary>
        /// <param name="item">The item whose value needs to be retrieved.</param>
        /// <param name="name">The property name.</param>
        /// <returns>Property value.</returns>
        public static object GetValue(object item, string name)
        {
            string[] parts = name.Split('.');

            for (int n = 0; item != null && n < parts.Length; n++)
            {
                string part = parts[n];

                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(item);
                PropertyDescriptor pd = pdc[part];
                if (pd != null)
                {
                    item = pd.GetValue(item);
                }
                else
                {
                    item = null;
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the current value of the specified property.
        /// </summary>
        /// <param name="item">The item whose value needs to be retrieved.</param>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        /// <returns>True if this operation is successful.</returns>
        public static bool GetValue(object item, string name, out object value)
        {
            string[] parts = name.Split('.');

            for (int n = 0; item != null && n < parts.Length; n++)
            {
                string part = parts[n];

                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(item);
                PropertyDescriptor pd = pdc[part];
                if (pd != null)
                {
                    item = pd.GetValue(item);
                }
                else
                {
                    value = null;
                    return false;
                }
            }

            value = item;
            return true;
        }

        /// <summary>
        /// Returns totals for the whole table. It can be an integer that represents the count that is displayed 
        /// in the caption for the TopLevelGroup.
        /// You can also return a object which contains summaries for the whole table.
        /// </summary>
        /// <returns>returns Totals.</returns>
        public object GetTotals()
        {
            return totals;
        }

        /// <summary>
        /// Returns the nested items for a parent record when it is expanded. In the implementation
        /// for this method you should execute a query on the related table and match the records
        /// to the criteria specified using the keys argument and the RelationDescriptor information
        /// specified with the rd argument.
        /// </summary>
        /// <param name="rd">The RelationDescriptor that describes the relation</param>
        /// <param name="childTable">The child table where the new items will be added after this method returns.</param>
        /// <param name="keys">The identifier for the nested records</param>
        /// <param name="totals">The number of child elements in the nested table or an object with summaries.</param>
        /// <returns>The results of the query for the nested records.</returns>
        public IEnumerable GetNestedItems(RelationDescriptor rd, ChildTable childTable, object[] keys, out object totals)
        {
            EnsureTable(childTable);

            if (NestedQueryHandler == null)
            {
                totals = null;
                return null;
            }

            return NestedQueryHandler(keys, out totals);
        }

        /// <summary>
        /// Returns the first few items of this result. The GridGroupingControl
        /// will loop through this items to determine the optimum width of columns.
        /// </summary>
        /// <returns>First few items.</returns>
        public IEnumerable GetSampleItems()
        {
            return sampleQuery;
        }
        
        #endregion

        #region IPassThroughGroupingResult Members

        /// <summary>
        /// Returns the data type of the items in the data source. This information is used for instantiating and
        /// adding new items to the data source.
        /// </summary>
        /// <returns>The data type of the items in the datasource.</returns>
        public Type GetListItemType()
        {
            return listItemType;
        }
        #endregion
    }
}
