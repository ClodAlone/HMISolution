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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
#if !WinRT
using System.ComponentModel;
using System.Windows.Data;
#endif

namespace Syncfusion.Data
{

#if !SILVERLIGHT && !SyncfusionFramework3_5
    /// <summary>
    /// Implement this interface to instruct the QueryableCollectionView derived view for generating PLINQ query expression trees.
    /// </summary>
    public interface IParallelizableView
    {
        bool UsePLINQ { get; set; }
    }
#endif

    public class QueryableCollectionView : CollectionViewAdv, IFilterExt
#if !SILVERLIGHT && !SyncfusionFramework3_5
, IParallelizableView
#endif
    {
        public IQueryable ViewSource { get; private set; }

#if !SILVERLIGHT && !SyncfusionFramework3_5
        private bool usePLINQ = false;
        /// <summary>
        /// Gets or sets a value indicating whether [use PLINQ]. 
        /// </summary>
        /// <value><c>true</c> if [use PLINQ]; otherwise, <c>false</c>.</value>
        public bool UsePLINQ
        {
            get
            {
                //only when ItemProperties is set use PLINQ otherwise the SourceType is not equal to the actual source collection
                return this.usePLINQ;
            }
            set
            {
                if (this.usePLINQ != value)
                {
                    this.usePLINQ = value;
                }
            }
        }
#endif

        public QueryableCollectionView(IEnumerable source)
            : base(source)
        {
            this.ViewSource = source.AsQueryable();
            this.EnsureSourceList();
        }

        public QueryableCollectionView(IEnumerable source, Type sourceType)
            : base(source, sourceType)
        {
            this.ViewSource = source.AsQueryable();
            this.EnsureSourceList();
        }

        protected override IRecordsList CreateRecords()
        {
            return EnumerableRecordsWrapper.CreateNew(this.ViewSource, this);
        }

        protected override void OnSortDescriptionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.RefreshSort();
            base.OnSortDescriptionChanged(e);
        }

        protected override void RefreshSort()
        {
            if (!IsGrouping)
            {
                var source = this.SourceCollection.AsQueryable();
                if (this.SortDescriptions.Count == 0 || source.Count() == 0)
                {
                    this.ViewSource = this.SourceCollection.AsQueryable();
                    return;
                }

#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP
                if (this.ItemPropertiesSet && this.UsePLINQ)
                {
                    //only when ItemProperties is set use PLINQ otherwise the SourceType is not equal to the actual source collection    
                    var parallelQuery = EnumerableExtensions.GetParallelQuery(this.SourceCollection, this.SourceType);
                    source = parallelQuery.AsQueryable();
                }
#endif
                this.ViewSource = SortQueryable(source);
            }
            else
            {
                if (this.SortDescriptions.Count > 0)
                {
                    this.TopLevelGroup.SuspendEvents();
                    var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                    grpRefresh.RefreshSortingOrder();
                    this.TopLevelGroup.ResumeEvents();
                }
            }
        }

        private IQueryable SortQueryable(IQueryable source)
        {
            for (int i = 0; i < this.SortDescriptions.Count; i++)
            {
                var s = this.SortDescriptions[i];
                var expressionFunc = this.GetExpressionFunc(s.PropertyName);
                var customComparer = this.SortComparers[s.PropertyName];
                if (s.Direction == ListSortDirection.Ascending)
                {
                    if (i == 0)
                    {
                        if (expressionFunc == null)
                        {
                            if (customComparer == null)
                                source = source.OrderBy(s.PropertyName, this.SourceType);
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.OrderBy(s.PropertyName, customComparer, this.SourceType);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                        else
                        {
                            if (customComparer == null)
                                source = source.OrderBy(s.PropertyName, expressionFunc);
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.OrderBy(s.PropertyName, customComparer, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                    }
                    else
                    {
                        if (expressionFunc == null)
                        {
                            if (customComparer == null)
                                source = source.ThenBy(s.PropertyName, this.SourceType);
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.ThenBy(s.PropertyName, customComparer, this.SourceType);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                        else
                        {
                            if (customComparer == null)
                            {
                                source = source.ThenBy(s.PropertyName, expressionFunc);
                            }
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.ThenBy(s.PropertyName, customComparer, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        if (expressionFunc == null)
                        {
                            if (customComparer == null)
                                source = source.OrderByDescending(s.PropertyName, this.SourceType);
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.OrderByDescending(s.PropertyName, customComparer, this.SourceType);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                        else
                        {
                            if (customComparer == null)
                            {
                                source = source.OrderByDescending(s.PropertyName, expressionFunc);
                            }
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.OrderByDescending(s.PropertyName, customComparer, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                    }
                    else
                    {
                        if (expressionFunc == null)
                        {
                            if (customComparer == null)
                                source = source.ThenByDescending(s.PropertyName, this.SourceType);
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.ThenByDescending(s.PropertyName, customComparer, this.SourceType);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                        else
                        {
                            if (customComparer == null)
                            {
                                source = source.ThenByDescending(s.PropertyName, expressionFunc);
                            }
                            else
                            {
#if !SyncfusionFramework3_5
                                source = source.ThenByDescending(s.PropertyName, customComparer, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                    }
                }
                
            }
            return source;
        }

        #region Filtering

        private Predicate<object> rowFilter;
        public Predicate<object> RowFilter
        {
            get { return rowFilter; }
            set { rowFilter = value; }
        }

        public override bool FilterRecord(object record)
        {
            return (this.Filter == null || this.Filter(record)) && (this.RowFilter == null || this.RowFilter(record));
        }
        
        public override void RefreshFilter()
        {
            var filterPresent = this.FilterPredicates.Any(v => v.FilterPredicates != null && v.FilterPredicates.Count > 0);
            if (filterPresent)
            {
                var source = this.SourceCollection.AsQueryable();
                ParameterExpression paramExpression;
                Expression predicate = this.GetPredicateExpression(source, out paramExpression);
                if (paramExpression != null && predicate != null)
                {
                    var lambda = Expression.Lambda(predicate, paramExpression);
                    var delg = lambda.Compile();
                    this.RowFilter = (o) =>
                        {
                            var result = (bool) delg.DynamicInvoke(o);
                            return result;
                        };
                }
                else
                    this.RowFilter = null;
            }
            else
            {
                this.RowFilter = null;
            }

            if (!this.IsInEndeferal)
            {
                if (this.IsGrouping)
                {
                    this.RefreshSortingOrderWithFiltersForBottomLevel(this.TopLevelGroup.Groups);
                    this.TopLevelGroup.SetDirty();
                    var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                    grpRefresh.RefreshFilters();
                }
                this.Refresh();
                if (this.TopLevelGroup != null)
                    this.TopLevelGroup.ResetCache = true;
            }
        }

        public virtual Expression GetPredicateExpression(IQueryable source, out ParameterExpression paramExpression)
        {
            return this.GetPredicateExpressionExt(source, out paramExpression);
        }

        public Expression GetPredicateExpression(IQueryable source, out ParameterExpression paramExpression, string columnName, bool returncolExpression)
        {
            return this.GetPredicateExpressionExt(source, out paramExpression, columnName, returncolExpression);
        }

        protected virtual void RefreshSortingOrderWithFiltersForBottomLevel(List<Group> groups)
        {
            foreach (var group in groups)
            {
                if (group.IsBottomLevel)
                {
                    var groupRecordsEntry = group.Details as GroupRecordEntry;
                    if (this.SortDescriptions.Count == 0)
                    {
                        groupRecordsEntry.PopulateRecords(groupRecordsEntry.UnfilteredRecords, this.FilterRecord);
                        this.TopLevelGroup.UpdateSummaries(group);
                        group.SetDirty();
                        return;
                    }

                    if (groupRecordsEntry.UnfilteredRecords != null)
                    {
                        IQueryable source = groupRecordsEntry.UnfilteredRecords.OfQueryable(this.SourceType).AsQueryable();
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP
                        if (this.ItemPropertiesSet && this.UsePLINQ)
                        {
                            //only when ItemProperties is set use PLINQ otherwise the SourceType is not equal to the actual source collection
                            var parallelQuery = EnumerableExtensions.GetParallelQuery(groupRecordsEntry.UnfilteredRecords.OfQueryable(this.SourceType), this.SourceType);
                            source = parallelQuery.AsQueryable();
                        }
#endif
                        if (source.Count() > 0)
                        {
                            IQueryable sortedSource = this.SortQueryable(source);
                            if (this.CanFilter)
                            {
                                groupRecordsEntry.PopulateRecords(sortedSource, this.FilterRecord);
                            }
                            else
                            {
                                groupRecordsEntry.PopulateRecords(sortedSource, null);
                            }
                        }
                    }
                    this.TopLevelGroup.UpdateSummaries(group);
                    group.SetDirty();
                }
                else
                {
                    this.RefreshSortingOrderWithFiltersForBottomLevel(group.Groups);
                }
            }
            //this.Refresh();
        }

        #endregion

        protected override IEnumerable<GroupResult> GetGroupResult(string[] groupBy)
        {
            var unsortedGroups = this.GetSortWithoutGroups();

            var sortComparer = new QueryableSortFieldComparer(unsortedGroups, this.GetItemProperties(), propertyAccessProvider, this.SortComparers, this.Culture, this.GetFunc(string.Empty));
            IQueryable queryable = this.ViewSource.AsQueryable();
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP
            if (this.ItemPropertiesSet && this.UsePLINQ) 
            {
                //only when ItemProperties is set use PLINQ otherwise the SourceType is not equal to the actual source collection
                var parallelQuery = EnumerableExtensions.GetParallelQuery(this.SourceCollection, this.SourceType);
                queryable = parallelQuery.AsQueryable();
            }
#endif
            Func<string, Expression> groupConverterFunc = (property) => this.GetGroupConverterExpressionFunc(property);
            var hasGroupConverter = this.GroupDescriptions.OfType<PropertyGroupDescription>().FirstOrDefault(g => g.Converter != null) != null;
            var expressionFunc = hasGroupConverter ? groupConverterFunc : (property) => this.GetExpressionFunc(property);
            var result = queryable.GroupByMany(this.SourceType, expressionFunc, groupBy).ToList();
            
            if (unsortedGroups.Count > 0)
            {
                foreach (var groupResult in result)
                {
                    if (groupResult.SubGroups == null)
                    {
#if !SILVERLIGHT && !SyncfusionFramework3_5
                        if (this.UsePLINQ)
                            groupResult.Items = this.SortQueryable(groupResult.Items.AsQueryable());
                        else
#endif
                            groupResult.Items = SortItemsForBottomLevel(groupResult.Items, sortComparer);
                    }
                    else
                    {
                        groupResult.SubGroups = this.SetSortOrderForInnerGroup(groupResult.SubGroups, sortComparer);
                    }
                }
            }
            return result;
        }

        private ObservableCollection<SortDescription> GetSortWithoutGroups()
        {
            var sortFields = new ObservableCollection<SortDescription>();
            foreach (var sortDesc in this.SortDescriptions)
            {
                var unsortedGroup = this.GroupDescriptions.OfType<PropertyGroupDescription>().FirstOrDefault(s => s.PropertyName == sortDesc.PropertyName);
                if (unsortedGroup == null)
                {
                    sortFields.Add(new SortDescription(sortDesc.PropertyName, sortDesc.Direction));
                }
            }

            return sortFields;
        }

        private IEnumerable<object> SortItemsForBottomLevel(IEnumerable items, IComparer<object> sortComparer)
        {
            var provider = this.GetPropertyAccessProvider();
            var comparer = new SortFieldComparer(this.SortDescriptions, this.SortComparers, this.Culture, (rec, propName) =>
            {
                if (provider != null)
                {
                    var value = provider.GetValue(rec, propName);
                    if (value != null)
                    {
                        return value;
                    }
                }

                var func = this.GetFunc(propName);
                if (func != null)
                {
                    var value = func(propName, rec);
                    return value;
                }

                return null;
            });

            List<object> list = items.Cast<object>().ToList();

            list.Sort(comparer);
            return list;
        }

        private IEnumerable<GroupResult> SetSortOrderForInnerGroup(IEnumerable<GroupResult> groups, IComparer<object> sortComparer)
        {
            IEnumerable<GroupResult> groupResults = groups.ToList();
            foreach (var groupResult in groupResults)
            {
                if (groupResult.SubGroups == null)
                {
                    if (this.SortDescriptions.Count > 0)
                    {
#if !SILVERLIGHT && !SyncfusionFramework3_5
                        if (this.UsePLINQ)
                            groupResult.Items = this.SortQueryable(groupResult.Items.AsQueryable());
                        else
#endif
                            groupResult.Items = this.SortItemsForBottomLevel(groupResult.Items, sortComparer);
                    }
                }
                else
                {
                    groupResult.SubGroups = this.SetSortOrderForInnerGroup(groupResult.SubGroups, sortComparer);
                }
            }
            return groupResults;
        }
    }

    public static class QueryableCollectionViewExtensions
    {
        public static Expression GetPredicateExpressionExt(this CollectionViewAdv view, IQueryable source,
                                                           out ParameterExpression paramExpression)
        {
            return GetPredicateExpressionExt(view, source, out paramExpression, null, false);
        }

        public static Expression GetPredicateExpressionExt(this CollectionViewAdv view, IQueryable source, out ParameterExpression paramExpression, string columnName, bool returncolExpression)
        {
            Expression predicate = null;
            paramExpression = source.ElementType.Parameter();
            var filterColumns = view.FilterPredicates.Where(v => v.FilterPredicates != null && v.FilterPredicates.Count > 0).ToList();
            if (filterColumns.Count == 0)
                return null;

            for (int colIndex = 0; colIndex < filterColumns.Count; colIndex++)
            {
                var column = filterColumns[colIndex];

                if (columnName != null)
                {
                    if (returncolExpression)
                    {
                        if (!column.MappingName.Equals(columnName))
                            continue;
                    }
                    else
                    {
                        if (column.MappingName.Equals(columnName))
                            continue;
                    }
                }

                Expression columnPredicate = null;
                Delegate valueLambda = null;
                Delegate displayLambda = null;

                var displayexpressionFunc = view.GetDisplayValueExpressionFunc(column.MappingName);
                var valueexpressionFunc = view.GetExpressionFunc(column.MappingName);

                if (valueexpressionFunc != null)
                    valueLambda = valueexpressionFunc.Compile();
                if (displayexpressionFunc != null)
                    displayLambda = displayexpressionFunc.Compile();

              
                for (int i = 0; i < column.FilterPredicates.Count; i++)
                { 
                    var item = column.FilterPredicates[i];
                    var expressionLambda = item.FilterBehavior == FilterBehavior.StringTyped
                                               ? displayLambda
                                               : valueLambda;

                    if (columnPredicate == null)
                    {
                        if (expressionLambda == null)
                            columnPredicate = source.Predicate(paramExpression, column.MappingName, item.FilterValue,item.FilterType, item.FilterBehavior,item.IsCaseSensitive, view.SourceType);
                        else
                            columnPredicate = source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, item.IsCaseSensitive, view.SourceType, expressionLambda);
                    }
                    else
                    {
                        if (expressionLambda == null)
                        {
                            if (item.PredicateType == PredicateType.Or)
                                columnPredicate = columnPredicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, item.IsCaseSensitive, view.SourceType));
                            else
                                columnPredicate = columnPredicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, item.IsCaseSensitive, view.SourceType));
                        }
                        else
                        {
                            if (item.PredicateType == PredicateType.Or)
                                columnPredicate = columnPredicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, item.IsCaseSensitive, view.SourceType, expressionLambda));
                            else
                                columnPredicate = columnPredicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, item.IsCaseSensitive, view.SourceType, expressionLambda));
                        }
                    }
                }

                if (columnPredicate == null)
                    continue;

                if (predicate == null)
                    predicate = columnPredicate;
                else
                {
                    if (column.FilterPredicates[0].PredicateType == PredicateType.Or)
                        predicate = predicate.OrPredicate(columnPredicate);
                    else
                        predicate = predicate.AndAlsoPredicate(columnPredicate);
                }
            }
            return predicate;
        }
    }

    internal class QueryableSortFieldComparer : IComparer<object>
    {
        #region Private Members

        private Comparer _comparer;
        private SortPropertyInfo[] _fields;
        private ObservableCollection<SortDescription> _sortFields;
        private Func<object, string, object> propertyInfoFunc;
        private SortComparers comparers;
#if WPF
        private PropertyDescriptorCollection _pdc;
#else
        private PropertyInfoCollection _pdc;
#endif
        private bool hasCustomComparers = false;
        private bool hasExtendedFuncExpressions = false;
        private IPropertyAccessProvider PropertyAccessProvider;

        #endregion

        #region Ctor
#if WPF
        internal QueryableSortFieldComparer(ObservableCollection<SortDescription> sortFields, PropertyDescriptorCollection pdc, IPropertyAccessProvider PropertyAccessProvider, SortComparers comparers, CultureInfo culture, Func<string, object, object> func)
#else
        internal QueryableSortFieldComparer(ObservableCollection<SortDescription> sortFields, PropertyInfoCollection pdc, IPropertyAccessProvider PropertyAccessProvider, SortComparers comparers, CultureInfo culture, Func<string, object, object> func)
#endif
        {
            this.PropertyAccessProvider = PropertyAccessProvider;
            this._sortFields = sortFields;
            this._pdc = pdc;
            this._fields = this.CreatePropertyInfo(this._sortFields);

            this._comparer = ((culture == null) || (culture == CultureInfo.InvariantCulture)) ? Comparer.Default : ((culture == CultureInfo.CurrentCulture) ? Comparer.Default : new Comparer(culture));
            this.comparers = comparers;
            if (comparers != null)
            {
                this.hasCustomComparers = comparers.Count > 0;
            }

            this.hasExtendedFuncExpressions = func != null;
            if (this.hasExtendedFuncExpressions)
            {
                this.propertyInfoFunc = (rec, propertyName) =>
                {
                    if (func != null)
                    {
                        var value = func(propertyName, rec);
                        return value;
                    }

                    return null;
                };
            }
        }

        #endregion

        public int Compare(object x, object y)
        {
            var o1 = x as RecordEntry;
            var o2 = y as RecordEntry;
            var data1 = o1 != null ? o1.Data : x;
            var data2 = o2 != null ? o2.Data : y;
            int num = 0;
            for (int i = 0; i < this._fields.Length; i++)
            {
                object a = null;
                object b = null;

                if (!this.hasExtendedFuncExpressions)
                {
                    a = this._fields[i].GetValue(data1);
                    b = this._fields[i].GetValue(data2);
                }
                else
                {
                    a = this.propertyInfoFunc(data1, this._fields[i].propName);
                    b = this.propertyInfoFunc(data2, this._fields[i].propName);
                }

                if (hasCustomComparers)
                {
                    //IComparer<object> customComparer = null;
                    var customComparer = this.comparers[this._fields[i].propName];//.TryGetValue(this._fields[i].propName, out customComparer);
                    num = customComparer != null ? customComparer.Compare(a, b) : this._comparer.Compare(a, b);
                }
                else
                {
                    num = this._comparer.Compare(a, b);
                }

                if (this._fields[i].descending)
                {
                    num = -num;
                }

                if (num != 0)
                    break;
                else
                    continue;
            }
            return num;
        }

        private SortPropertyInfo[] CreatePropertyInfo(ObservableCollection<SortDescription> sortFields)
        {
            var infoArray = new SortPropertyInfo[sortFields.Count];
            for (int i = 0; i < sortFields.Count; i++)
            {
                var description = sortFields[i];
#if WPF
                PropertyDescriptor pd = null;
#else
                PropertyInfo pd = null;
#endif
                if (string.IsNullOrEmpty(description.PropertyName))
                {
                    pd = null;
                }
                else if (this._pdc != null)
                {
                    pd = this._pdc.GetPropertyDescriptor(description.PropertyName);//.Find(description2.PropertyName, false); //new PropertyPath(description2.PropertyName, new object[0]);
                }

                infoArray[i].propName = description.PropertyName;
                infoArray[i].descending = description.Direction == ListSortDirection.Descending;
                infoArray[i].PropertyAccessProvider = PropertyAccessProvider;
            }
            return infoArray;
        }

        //[System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
        private struct SortPropertyInfo
        {
            internal string propName;
            internal bool descending;

            internal IPropertyAccessProvider PropertyAccessProvider;
            internal object GetValue(object o)
            {
                if (this.PropertyAccessProvider == null)
                {
                    return o;
                }
                return PropertyAccessProvider.GetValue(o, propName);
            }
        }
    }
}
