#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
    namespace Syncfusion.Windows.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows.Data;
    using Syncfusion.Linq;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Globalization;

    /// <summary>
    /// Extends CollectionViewAdv to implement LINQ based operations on strongly-typed
    /// data sources.
    /// </summary>
    public class QueryableCollectionView : CollectionViewAdv
#if !SILVERLIGHT && SyncfusionFramework4_0
, IParallelizableView
#endif
    {
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
#if !SILVERLIGHT && SyncfusionFramework4_0
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

        /// <summary>
        /// Gets the underlying view source that holds the current collection in a separate IQueryable view.
        /// </summary>
        /// <value>The view source.</value>
        protected IQueryable ViewSource
        {
            get;
            set;
        }

        /// <summary>
        /// Raises the <see cref="E:SortDescriptionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnSortDescriptionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.RefreshSort();
            base.OnSortDescriptionChanged(e);
        }

        protected void SetSource(IQueryable source)
        {
            this.ViewSource = source;
        }

        /// <summary>
        /// Refreshes the sort.
        /// </summary>
        protected override void RefreshSort()
        {
            if (!this.IsGrouping)
            {
                var source = this.SourceCollection.AsQueryable();
                // if sort is changed / removed, we need to re-evaluate fully
                if (this.SortDescriptions.Count == 0 || source.Count() == 0)
                {
                    this.ViewSource = this.SourceCollection.AsQueryable();
                    return;
                }

#if !SILVERLIGHT && SyncfusionFramework4_0
                if (this.ItemPropertiesSet && this.UsePLINQ)
                {
                    //only when ItemProperties is set use PLINQ otherwise the SourceType is not equal to the actual source collection    
                    var parallelQuery = EnumerableExtensions.GetParallelQuery(this.SourceCollection, this.SourceType);
                    source = parallelQuery.AsQueryable();
                }
#endif
                this.ViewSource = SortQueryable(source);
                //This method invoked from RefreshSort method. To avoid no of hits commenting the invoking of this method here.
                //this.EnsureInitialized();
            }
            else
            {
                if (this.SortDescriptions.Count > 0)
                {
                    this.TopLevelGroup.SuspendEvents();
                    var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                    grpRefresh.RefreshSortingOrder();
                    // no need to call the below method, sorting is handled in GetGroupResult()
                    // RefreshSortingOrderWithFiltersForBottomLevel(this.TopLevelGroup.Groups);
                    this.TopLevelGroup.ResumeEvents();
                }
            }
        }

        /// <summary>
        /// Refreshes the sorting order for bottom level.
        /// </summary>
        /// <param name="groups">The groups.</param>
        protected virtual void RefreshSortingOrderWithFiltersForBottomLevel(List<Group> groups)
        {
            foreach (var group in groups)
            {
                if (group.IsBottomLevel)
                {
                    var groupRecordsEntry = group.Details as GroupRecordEntry;
                    if (this.SortDescriptions.Count == 0)
                    {
                        groupRecordsEntry.PopulateRecords(groupRecordsEntry.UnfilteredRecords, this.PassesFilter);
                        this.TopLevelGroup.UpdateSummaries(group);
                        continue;
                    }
                    
                    if (groupRecordsEntry.UnfilteredRecords != null)
                    {
                        IQueryable source = groupRecordsEntry.UnfilteredRecords.OfQueryable(this.SourceType).AsQueryable();
#if !SILVERLIGHT && SyncfusionFramework4_0
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
                                if (this.EnablePaging)
                                    groupRecordsEntry.PopulateRecords(sortedSource, this.PassesFilter, this.IsViewLevelPaging);
                                else
                                    groupRecordsEntry.PopulateRecords(sortedSource, this.PassesFilter);
                            }
                            else
                            {
                                if (this.EnablePaging)
                                    groupRecordsEntry.PopulateRecords(sortedSource, null, this.IsViewLevelPaging);
                                else
                                    groupRecordsEntry.PopulateRecords(sortedSource,this.PassesFilter);
                            }
                        }
                    }
                    this.TopLevelGroup.UpdateSummaries(group);
                }
                else
                {
                    this.RefreshSortingOrderWithFiltersForBottomLevel(group.Groups);
                }
            }
        }

        private IQueryable SortQueryable(IQueryable source)
        {
            for (int i = 0;i < this.SortDescriptions.Count;i++)
            {
                var s = this.SortDescriptions[i];
                var expressionFunc = this.GetExpressionFunc(s.PropertyName);
                IComparer<object> customComparer = null;
                this.SortComparers.TryGetValue(s.PropertyName, out customComparer);
                if (s.Direction == ListSortDirection.Ascending)
                {
                    if (i == 0)
                    {
                        if (expressionFunc == null)
                        {
                            if (customComparer == null)
                            {
//#if !SyncfusionFramework4_0
//                                if (this.IsGrouping && source.Count() > 0 && this.SourceType.IsInterface)
//                                {
//                                    var unsortedGroups = this.GetSortWithoutGroups();
//                                    var sortComparer = new QueryableSortFieldComparer(unsortedGroups, this.GetItemProperties(), propertyAccessProvider, this.SortComparers, this.Culture, this.GetFunc(string.Empty));
//                                    this.SortItemsForBottomLevel(source, sortComparer);
//                                }
//                                else
//#endif
                                    source = source.OrderBy(s.PropertyName, this.SourceType);
                            }
                            else
                            {
#if SyncfusionFramework4_0
                                source = source.OrderBy(s.PropertyName, customComparer, this.SourceType);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                        }
                        else
                        {
                            if (customComparer == null)
#if SyncfusionFramework4_0
                                source = source.OrderBy(s.PropertyName, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            else
                            {
#if SyncfusionFramework4_0
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
                            {
//#if !SyncfusionFramework4_0
//                                if (this.IsGrouping && source.Count() > 0 && this.SourceType.IsInterface)
//                                {
//                                    var unsortedGroups = this.GetSortWithoutGroups();
//                                    var sortComparer = new QueryableSortFieldComparer(unsortedGroups, this.GetItemProperties(), propertyAccessProvider, this.SortComparers, this.Culture, this.GetFunc(string.Empty));
//                                    this.SortItemsForBottomLevel(source, sortComparer);
//                                }
//                                else

//#endif
                                {
#if SyncfusionFramework4_0
                                    source = source.ThenBy(s.PropertyName, this.SourceType);
#else
                                    source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                                }
                            }
                            else
                            {
#if SyncfusionFramework4_0
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
#if SyncfusionFramework4_0
                                source = source.ThenBy(s.PropertyName, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                            else
                            {
#if SyncfusionFramework4_0
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
                            {
//#if !SyncfusionFramework4_0
//                                if (this.IsGrouping && source.Count() > 0 && this.SourceType.IsInterface)
//                                {
//                                    var unsortedGroups = this.GetSortWithoutGroups();
//                                    var sortComparer = new QueryableSortFieldComparer(unsortedGroups, this.GetItemProperties(),propertyAccessProvider, this.SortComparers, this.Culture, this.GetFunc(string.Empty));
//                                    this.SortItemsForBottomLevel(source, sortComparer);
//                                }
//                                else
//#endif
                                {
                                    source = source.OrderByDescending(s.PropertyName, this.SourceType);
                                }
                            }
                            else
                            {
#if SyncfusionFramework4_0
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
#if SyncfusionFramework4_0
                                source = source.OrderByDescending(s.PropertyName, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                            else
                            {
#if SyncfusionFramework4_0
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
                            {
//#if !SyncfusionFramework4_0
//                                if (this.IsGrouping && source.Count() > 0 && this.SourceType.IsInterface)
//                                {
//                                    var unsortedGroups = this.GetSortWithoutGroups();
//                                    var sortComparer = new QueryableSortFieldComparer(unsortedGroups, this.GetItemProperties(), this.SortComparers, this.Culture, this.GetFunc(string.Empty));
//                                    this.SortItemsForBottomLevel(source, sortComparer);
//                                }
//                                else
//#endif
#if SyncfusionFramework4_0
                                    source = source.ThenByDescending(s.PropertyName, this.SourceType);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                            else
                            {
#if SyncfusionFramework4_0
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
#if SyncfusionFramework4_0
                                source = source.ThenByDescending(s.PropertyName, expressionFunc);
#else
                                source = this.SortItemsForBottomLevel(source, customComparer).AsQueryable().OfType(this.SourceType);
#endif
                            }
                            else
                            {
#if SyncfusionFramework4_0
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


        /// <summary>
        /// Creates the records.
        /// </summary>
        /// <returns></returns>
        protected override IRecordsList CreateRecords()
        {
            IEnumerable source = this.ViewSource;
            //if (this.IsGrouping)
            //{
            //    List<object> recordList = new List<object>();
            //    foreach (var record in this.TopLevelGroup)
            //    {
            //        if (record is RecordEntry)
            //        {
            //            recordList.Add(((RecordEntry)record).Data);
            //        }
            //    }
            //    source = recordList;
            //}
            return EnumerableRecordsWrapper.CreateNew(source, this);
        }

        private Predicate<object> rowFilter;
        public Predicate<object> RowFilter
        {
            get { return rowFilter; }
            set
            {
                rowFilter = value;
                if (!this.IsGrouping)
                {
                    this.Refresh();
                }
            }
       }
 
        protected override bool CanFilterRecord()
        {
            return this.Filter != null || this.RowFilter != null;
        }

        public override bool PassesFilter(object record)
        {
            return (this.Filter == null || this.Filter(record)) && (this.RowFilter == null || this.RowFilter(record));
        }
        /// <summary>
        /// Refreshes the filters.
        /// </summary>
        public override void RefreshFilters()
        {
            try
            {
                var editableView = this as IEditableCollectionView;
                if (editableView.IsAddingNew || editableView.IsEditingItem)
                {
                    editableView.CommitEdit();
                }

                var filterPresent = this.FilterPredicates.Where(v => v.Filters != null && v.Filters.Count > 0).Count() > 0;
                if (filterPresent)
                {
                    var source = this.SourceCollection.AsQueryable();
                    Expression predicate;
                    ParameterExpression paramExpression;
                    if (source.Count() > 0)
                    {
                        this.ExtractQueryableFilterPredicates(source, out predicate, out paramExpression);
                        if (paramExpression != null && predicate != null)
                        {
                            var lambda = Expression.Lambda(predicate, paramExpression);
                            var delg = lambda.Compile();
                            this.RowFilter = (o) =>
                            {
                                var result = (bool)delg.DynamicInvoke(o);
                                return result;
                            };
                            //This is for add the filter in the paged source
                            if (this.PagedSource != null && !this.IsViewLevelPaging)
                                this.PagedSource.Filter = this.RowFilter;
                        }
                    }
                }
                else
                {
                    this.RowFilter = null;
                }

                if (this.IsGrouping && this.TopLevelGroup != null)
                {
                    // no need to check for EndDefer,
                    // if (!this.IsInEndDefer)
                    {
                        this.RefreshSortingOrderWithFiltersForBottomLevel(this.TopLevelGroup.Groups);
                    }
                    this.TopLevelGroup.SetDirty();
                    var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                    grpRefresh.RefreshFilters();
                    this.Refresh();
                }
            }
            catch
            { }
        }

        private void ExtractQueryableFilterPredicates(IQueryable source, out Expression predicate, out ParameterExpression paramExpression)
        {
            var filterColumns = this.FilterPredicates.Where(v => v.Filters != null && v.Filters.Count > 0).ToList();
            var firstLoop = false;
            predicate = null;
            paramExpression = source.ElementType.Parameter();
            foreach (var column in filterColumns)
            {
                var expressionFunc = this.GetExpressionFunc(column.MappingName);
                var typeFunc = this.GetTypeExpressionFunc(column.MappingName);
                Delegate expressionLambda = null;
                Delegate typeLamda = null;
                if (expressionFunc != null)
                {
                    expressionLambda = expressionFunc.Compile();
                }
                if (typeFunc != null)
                {
                    typeLamda = typeFunc.Compile();
                }
                Type memberType = null;
                if (expressionFunc != null)
                {
                    var enumerator = source.GetEnumerator();
                    if (!enumerator.MoveNext())
                    {
                        continue;
                    }
                    var checkDelg = typeFunc.Compile();
                    // invoking this delegate will return a value, that determines the type of method to be called in the Queryable class.
                    while (enumerator != null)
                    {
                        if (checkDelg.DynamicInvoke(new object[] { column.MappingName, enumerator.Current }) != null)
                            break;
                        else
                            enumerator.MoveNext();
                    }
                    var returnValue = (Type)checkDelg.DynamicInvoke(new object[] { column.MappingName, enumerator.Current });
                    memberType = (returnValue == typeof(int)) ? typeof(int?) :(returnValue == typeof(double))?typeof(double?): returnValue;
                }
                Expression ColumnPredicate = null;
                
                for (int i = 0;i < column.Filters.Count;i++)
                {
                    var fp = column.Filters[i];
                    bool Firstpredicate = false;       
				   // Update the Exact Predicate type .between columns from the first predicate type. 
                    if (i != 0)
                       column.Filters[i].FilterBehavior = column.Filters[0].FilterBehavior;
                    if (!firstLoop)
                    {
                        //Initially we only want the predicate to build WHERE expressions based on AND/OR symbols
                        //if (fp.FilterValue.ToString() != "" && source.Count() > 0)
                        if (source.Count() > 0)
                        {
                            if (fp.FilterValue == null)
                            {
                                if (expressionFunc == null)
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, this.SourceType);
                                }
                                else
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, memberType, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, this.SourceType, expressionLambda, typeLamda);
                                }
                            }
                            if (fp.FilterValue != null && fp.FilterValue.ToString() != string.Empty)
                            {
                                if (expressionFunc == null)
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, this.SourceType);
                                }
                                else
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, memberType, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, this.SourceType, expressionLambda, typeLamda);
                                }
                            }
                            firstLoop = true;
                            Firstpredicate = true;
                        }
                    }
                    else
                    {
                        if (fp.PredicateType == PredicateType.And)
                        {
                            if (expressionFunc == null)
                            {
                                if (ColumnPredicate == null)
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, this.SourceType);
                                }
                                else
                                {
                                    ColumnPredicate = ColumnPredicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, fp.FilterValue, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, this.SourceType));
                                }
                            }
                            else
                            {
                                if (ColumnPredicate == null)
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, memberType, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, this.SourceType, expressionLambda, typeLamda);
                                }
                                else
                                {
                                    ColumnPredicate = ColumnPredicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, fp.FilterValue, memberType, fp.FilterType, fp.FilterBehavior,
                                                                                                                                     fp.IsCaseSensitive, this.SourceType, expressionLambda, typeLamda));
                                }
                            }
                        }
                        else if (fp.PredicateType == PredicateType.Or)
                        {
                            if (expressionFunc == null)
                            {
                                if (ColumnPredicate == null)
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, this.SourceType);
                                }
                                else
                                {
                                    ColumnPredicate = ColumnPredicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, fp.FilterValue, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, this.SourceType));
                                }
                            }
                            else
                            {
                                if (ColumnPredicate == null)
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, memberType, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive,
                                                                                                                                            this.SourceType, expressionLambda, typeLamda);
                                }
                                else
                                {
                                    ColumnPredicate = ColumnPredicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, fp.FilterValue, memberType, fp.FilterType, fp.FilterBehavior,
                                                                                                                                   fp.IsCaseSensitive, this.SourceType, expressionLambda, typeLamda));
                                }
                            }
                        }
                    }
                    if (i == column.Filters.Count - 1 && !Firstpredicate)
                    {
                        // Since First Column does not need any Predicate Type to Add with it we just assign the ColumnPredicate to it in else part
                        if (column != filterColumns.FirstOrDefault())
                        {
                            PredicateType columnpredicatetype = column.Filters[0].PredicateType;
                            if (columnpredicatetype == PredicateType.And)
                            {
                                predicate = predicate.AndPredicate(ColumnPredicate);
                            }
                            else if (columnpredicatetype == PredicateType.Or)
                            {
                                predicate = predicate.OrPredicate(ColumnPredicate);
                            }
                        }
                        else
                            predicate = ColumnPredicate;
                    }
                    if (predicate == null && ColumnPredicate != null)
                    {
                        predicate = ColumnPredicate;
                    }
                }
            }
        }

        private SortDescriptionCollection GetSortWithoutGroups()
        {
            var sortFields = new SortDescriptionCollection();
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

        /// <summary>
        /// Gets the group result.
        /// </summary>
        /// <param name="groupBy">The group by.</param>
        /// <returns></returns>
        protected override IEnumerable<GroupResult> GetGroupResult(string[] groupBy)
        {
            var unsortedGroups = this.GetSortWithoutGroups();
                               
            var sortComparer = new QueryableSortFieldComparer(unsortedGroups, this.GetItemProperties(),propertyAccessProvider, this.SortComparers, this.Culture, this.GetFunc(string.Empty));
            IQueryable queryable = this.ViewSource.AsQueryable();
#if !SILVERLIGHT && SyncfusionFramework4_0
            if (this.ItemPropertiesSet && this.UsePLINQ)
            {
                //only when ItemProperties is set use PLINQ otherwise the SourceType is not equal to the actual source collection
                var parallelQuery = EnumerableExtensions.GetParallelQuery(this.SourceCollection, this.SourceType);
                queryable = parallelQuery.AsQueryable();
            }
#endif

            Func<string, Expression> groupConverterFunc = (property) => this.GetGroupConverterExpressionFunc(property);
            var hasGroupConverter = this.GroupDescriptions.OfType<PropertyGroupDescription>().FirstOrDefault(g => g.Converter != null) != null;
            //This function will be called to sort the data before Group. 
            //var result = queryable.GroupByMany(this.SourceType, this.SortDescriptions.ToList(), this.SortComparers, hasGroupConverter ? groupConverterFunc : (property) => this.GetExpressionFunc(property), groupBy).ToList();
            if (this.SourceType == null)
                return null;
            var result = queryable.GroupByMany(this.SourceType, hasGroupConverter ? groupConverterFunc : (property) => this.GetExpressionFunc(property), groupBy).ToList();
            //This function will sort the data without Converters
            //var result = queryable.GroupByMany(this.SourceType, this.SortDescriptions.ToList(), (property) => this.GetExpressionFunc(property), groupBy);

            if (unsortedGroups.Count > 0)
            {
                foreach (var groupResult in result)
                {
                    if (groupResult.SubGroups == null)
                    {
#if !SILVERLIGHT && SyncfusionFramework4_0
                        if (this.UsePLINQ)
                        {
                            groupResult.Items = this.SortQueryable(groupResult.Items.AsQueryable());
                        }
                        else
                        {
#endif
                        // LINQ queries are a bit slow when sorting nested levels
                        List<object> list = SortItemsForBottomLevel(groupResult.Items, sortComparer);
                        groupResult.Items = list;
#if !SILVERLIGHT && SyncfusionFramework4_0
                        }
#endif
                    }
                    else
                    {
                        groupResult.SubGroups = this.SetSortOrderForInnerGroup(groupResult.SubGroups, sortComparer);
                    }
                }

                // yield return groupResult;
            }

            return result;
        }

        private List<object> SortItemsForBottomLevel(IEnumerable items, IComparer<object> sortComparer)
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

            List<object> list = new List<object>();
            foreach (var item in items)
            {
                list.Add(item);
            }
            list.Sort(comparer);
            return list;
        }

        private IEnumerable<GroupResult> SetSortOrderForInnerGroup(IEnumerable<GroupResult> groups, IComparer<object> sortComparer)
        {
            // do a ToList() here, since it is an IEnumerable<> using LINQ creates immutable lists
            IEnumerable<GroupResult> groupResults = groups.ToList();
            foreach (var groupResult in groupResults)
            {
                if (groupResult.SubGroups == null)
                {
                    if (this.SortDescriptions.Count > 0)
                    {
#if !SILVERLIGHT && SyncfusionFramework4_0
                        if (this.UsePLINQ)
                        {
                            groupResult.Items = this.SortQueryable(groupResult.Items.AsQueryable());
                        }
                        else
                        {
#endif
                        groupResult.Items = this.SortItemsForBottomLevel(groupResult.Items, sortComparer);
#if !SILVERLIGHT && SyncfusionFramework4_0
                        }
#endif
                    }
                }
                else
                {
                    groupResult.SubGroups = this.SetSortOrderForInnerGroup(groupResult.SubGroups, sortComparer);
                }
            }

            return groupResults;
        }

        protected override void Dispose(bool disposing)
        {
            ViewSource = null;
            base.Dispose(disposing);
        }
    }

    internal class QueryableSortFieldComparer : IComparer<object>
    {
        // Fields
        private Comparer _comparer;
        private SortPropertyInfo[] _fields;
        private SortDescriptionCollection _sortFields;
        private Func<object, string, object> propertyInfoFunc;
        private Dictionary<string, IComparer<object>> comparers;
#if !SILVERLIGHT
        private PropertyDescriptorCollection _pdc;
#else
        private PropertyInfoCollection _pdc;
#endif
        private bool hasCustomComparers = false;
        private bool hasExtendedFuncExpressions = false;
        private IPropertyAccessProvider PropertyAccessProvider;
#if !SILVERLIGHT
        internal QueryableSortFieldComparer(SortDescriptionCollection sortFields, PropertyDescriptorCollection pdc, IPropertyAccessProvider PropertyAccessProvider, Dictionary<string, IComparer<object>> comparers, CultureInfo culture, Func<string, object, object> func)
#else
        internal QueryableSortFieldComparer(SortDescriptionCollection sortFields, PropertyInfoCollection pdc, IPropertyAccessProvider PropertyAccessProvider, Dictionary<string, IComparer<object>> comparers, CultureInfo culture, Func<string, object, object> func)
#endif
        {
            //this.collectionView = cView;
            this.PropertyAccessProvider = PropertyAccessProvider;
            this._sortFields = sortFields;
            this._pdc = pdc;
            this._fields = this.CreatePropertyInfo(this._sortFields);

            this._comparer = ((culture == null) || (culture == CultureInfo.InvariantCulture)) ? Comparer.DefaultInvariant : ((culture == CultureInfo.CurrentCulture) ? Comparer.Default : new Comparer(culture));
            this.comparers = comparers;
            this.hasCustomComparers = comparers.Count > 0;

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

        public int Compare(object x, object y)
        {
            var o1 = x as RecordEntry;
            var o2 = y as RecordEntry;
            var data1 = o1 != null ? o1.Data : x;
            var data2 = o2 != null ? o2.Data : y;
            int num = 0;
            for (int i = 0;i < this._fields.Length;i++)
            {
                object a = null;// this.propertyInfoFunc(data1, this._fields[i].propName);
                object b = null;// this.propertyInfoFunc(data2, this._fields[i].propName);

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
                    IComparer<object> customComparer = null;
                    this.comparers.TryGetValue(this._fields[i].propName, out customComparer);
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

                //if (data1 == data2)
                //{
                //    return -1;
                //}
                if (num != 0)
                    break;
                else
                    continue;

                //if (num != 0)
                //{
                //    return num;
                //}
            }
            return num;
        }

        private SortPropertyInfo[] CreatePropertyInfo(SortDescriptionCollection sortFields)
        {
            SortPropertyInfo[] infoArray = new SortPropertyInfo[sortFields.Count];
            for (int i = 0;i < sortFields.Count;i++)
            {
                var description = sortFields[i];
#if !SILVERLIGHT
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

        // Nested Types
        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
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
