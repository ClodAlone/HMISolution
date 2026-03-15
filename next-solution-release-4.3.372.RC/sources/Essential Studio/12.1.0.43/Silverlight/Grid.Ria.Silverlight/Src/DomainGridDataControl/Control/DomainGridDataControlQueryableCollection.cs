#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid.Ria
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
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.Windows.Data;

    public class DomainQueryableCollectionView : CollectionViewAdv, IExcelLikeFilterExt
    {
        public DomainQueryableCollectionView(ICollectionView source, GridDataTableModel tableModel, Func<object, RecordEntry> createRecordFunc)
            : base(source)
        {
            this.EnsureSourceList();
            this.Collection = (ICollectionView)source;
            this.TableModel = (DomainGridDataTableModel)tableModel;
            this.createRecordFunc = createRecordFunc;
            this.TableSummaryRows.Clear();
            foreach (var tsummaryrow in this.TableModel.TableProperties.TableSummaryRows)
            {
                this.TableSummaryRows.Add(tsummaryrow);
            }

            this.MoveCurrentToFirst();
        }

        private Func<object, RecordEntry> createRecordFunc;
        public override RecordEntry CreateRecordEntry(object data)
        {
            return this.createRecordFunc(data);
        }

        private ICollectionView Collection
        {
            get;
            set;
        }


        public DomainGridDataTableModel TableModel
        {
            get;
            set;
        }

        protected override IEnumerable<string> GetMappingNames()
        {
            return this.TableModel.TableProperties.VisibleColumns.GetMappingNames();
        }

        public override PropertyInfoCollection GetItemProperties()
        {
            var properties = base.GetItemProperties();

            if (this.TableModel != null && this.TableModel.TableProperties.AutoPopulateColumns)
            {
                for (int i = 0; i < properties.Values.Count; i++)
                {
                    string key = properties.Keys.ToList()[i];
                    string propDet = properties[key].ToString();

                    if (propDet.Equals("System.ServiceModel.DomainServices.Client.EntityConflict EntityConflict") ||
                   propDet.Equals("System.Collections.Generic.ICollection`1[System.ComponentModel.DataAnnotations.ValidationResult] ValidationErrors") ||
                   propDet.Equals("Boolean HasValidationErrors") ||
                   propDet.Equals("System.ServiceModel.DomainServices.Client.EntityState EntityState") ||
                   propDet.Equals("Boolean HasChanges") ||
                   propDet.Equals("Boolean IsReadOnly") ||
                   propDet.Equals("System.Collections.Generic.IEnumerable`1[System.ServiceModel.DomainServices.Client.EntityAction] EntityActions"))
                    {
                        properties.Remove(key);
                        i--;
                    }
                }
            }
            return properties;
        }

        protected override IRecordsList CreateRecords()
        {
            var recordsList = EnumerableRecordsWrapper.CreateNew(this.Collection.SourceCollection.AsQueryable(), this);
            return (IRecordsList)recordsList;
        }

        protected override TopLevelGroup CreateTopLevelGroup()
        {
            var topLevelgroup = new GridDataTopLevelGroup(this.TableModel, this);
            return topLevelgroup;
        }

        protected override void OnTopLevelGroupPopulated(TopLevelGroup topLevelGroup)
        {
            var gridDataTopLevelGroup = topLevelGroup as GridDataTopLevelGroup;
            this.CaptionSummaryRow = this.TableModel.TableProperties.CaptionSummaryRow;
            this.SummaryRows.Clear();

            foreach (var row in this.TableModel.TableProperties.SummaryRows)
            {
                this.SummaryRows.Add((ISummaryRow)row);
            }
        }

        public override System.Linq.Expressions.Expression<Func<string, object, object>> GetExpressionFunc(string propertyName)
        {
            Func<string, object, object> recordFunc = (columnName, record) =>
            {
                PropertyInfo property = record.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    return property.GetValue(record);
                }

                return null;
            };

            Expression<Func<string, object, object>> expFunc = (columnName, record) => recordFunc(columnName, record);
            var unboundExpFunc = this.TableModel.GetUnboundExpressionFunc(propertyName);
            if (unboundExpFunc != null)
            {
                expFunc = unboundExpFunc;
            }

            return expFunc;

        }

       

      
        Dictionary<object, object> filteredRecord, unFilteredRecord;

         public Dictionary<object, object> FilteredRecords
        {
            get
            {
                if (filteredRecord == null)
                    filteredRecord = new Dictionary<object, object>();

                return filteredRecord;
            }
            set
            {
                filteredRecord = value;
            }
        }

         public Dictionary<object, object> UnFilteredRecords
        {
            get
            {
                if (unFilteredRecord == null)
                    UnFilteredRecords = new Dictionary<object, object>();

                return unFilteredRecord;
            }
            set
            {
                unFilteredRecord = value;
            }
        }

         public bool IsExcelLikeFilter { get; set; }

        public bool? IsSelectAllFiltered { get; set; }

        public override void RefreshFilters()
        {
            if (!this.IsExcelLikeFilter)
            {
                this.RefreshDomainFilters();
            }
            else if (this.FilteredRecords != null && this.UnFilteredRecords != null)
                {
                    var filterColumns = this.FilterPredicates.Where(v => v.Filters != null && v.Filters.Count > 0).ToList();

                    if (filterColumns.Count == 0 && ((this.IsSelectAllFiltered != null && this.IsSelectAllFiltered.Value == true) || this.IsSelectAllFiltered == null))
                    {
                        this.Filter = null;
                    }
                    else if (filterColumns.Count > 0 && this.IsSelectAllFiltered != null && this.IsSelectAllFiltered.Value == false)
                    {
                        this.Filter = (o) =>
                            {
                                return false;
                            };
                    }
                    else
                    {
                        var source = this.SourceCollection.AsQueryable();
                        Expression predicate;
                        ParameterExpression paramExpression;
                        this.ExcelFilterPredicates(source, out predicate, out paramExpression, null);
                        if (paramExpression != null && predicate != null)
                        {
                            var lambda = Expression.Lambda(predicate, paramExpression);
                            var delg = lambda.Compile();
                            this.Filter = (o) =>
                            {
                                var result = (bool)delg.DynamicInvoke(o);
                                return result;
                            };
                        }
                    }
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
            }
        }
        protected override IEnumerable<GroupResult> GetGroupResult(string[] groupBy)
        {
            IQueryable queryable = this.SourceCollection.AsQueryable();
            var result = queryable.GroupByMany(queryable.ElementType, (property) => this.GetExpressionFunc(property), groupBy);
            return result;
        }


        protected void RefreshSortingOrderWithFiltersForBottomLevel(List<Group> groups)
        {
            foreach (var group in groups)
            {
                if (group.IsBottomLevel)
                {
                    var groupRecordsEntry = group.Details as GroupRecordEntry;
                    if (this.SortDescriptions.Count == 0)
                    {
                        groupRecordsEntry.PopulateRecords(groupRecordsEntry.UnfilteredRecords, this.Filter);
                        return;
                    }
                    //var recordcount = groupRecordsEntry.GetRecordsCount();
                    //if (recordcount > 0)
                    //{
                    if (groupRecordsEntry.UnfilteredRecords != null)
                    {
                        IQueryable source = groupRecordsEntry.UnfilteredRecords.OfQueryable().AsQueryable();
#if !SILVERLIGHT && SyncfusionFramework4_0
                        if (this.ItemPropertiesSet && this.UsePLINQ)
                        {
                            //only when ItemProperties is set use PLINQ otherwise the SourceType is not equal to the actual source collection
                            var parallelQuery = EnumerableExtensions.GetParallelQuery(groupRecordsEntry.UnfilteredRecords.OfQueryable(), this.SourceType);
                            source = parallelQuery.AsQueryable();
                        }
#endif
                        if (source.Count() > 0)
                        {
                            IQueryable sortedSource = this.SortQueryable(source);
                            if (this.CanFilter)
                            {

                                //This is for poupulate the group record for Paging support.
                                if (this.EnablePaging)
                                {
                                    groupRecordsEntry.PopulateRecords(sortedSource, this.Filter, this.IsViewLevelPaging);
                                }
                                else

                                    groupRecordsEntry.PopulateRecords(sortedSource, this.Filter);
                            }
                            else
                            {

                                //This is for poupulate the group record for Paging support.
                                if (this.EnablePaging)
                                {
                                    groupRecordsEntry.PopulateRecords(sortedSource, null, this.IsViewLevelPaging);
                                }
                                else

                                    groupRecordsEntry.PopulateRecords(sortedSource, null);
                            }
                        }
                    }
                    //}

                    this.TopLevelGroup.UpdateSummaries(group);
                }
                else
                {
                    // sort the records first
                    this.RefreshSortingOrderWithFiltersForBottomLevel(group.Groups);
                }
            }

            if (this.Filter != null)
            {
                // call refresh to sync the RecordEntry collection from TopLevelGroup and View.Records collection. This also ensures that the uniqueIdentifiers are same for both the cases.
                this.Refresh();
            }
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
                Delegate typeLambda = null;
                if (expressionFunc != null)
                {
                    expressionLambda = expressionFunc.Compile();
                }
                if (typeFunc != null)
                {
                    typeLambda = typeFunc.Compile();
                }
                Type memberType = null;
                if (typeFunc != null)
                {
                    var enumerator = source.GetEnumerator();
                    if (!enumerator.MoveNext())
                    {
                        continue;
                    }
                    var checkDelg = typeFunc.Compile();
                    // invoking this delegate will return a value, that determines the type of method to be called in the Queryable class.
                    memberType = (Type)checkDelg.DynamicInvoke(new object[] { column.MappingName, enumerator.Current });
                }
                Expression ColumnPredicate = null;      // this calcilates the previcate for pirticulat column and then added to predicate 

                for (int i = 0; i < column.Filters.Count; i++)
                {
                    var fp = column.Filters[i];
                    bool Firstpredicate = false;
                    // Update the Exact Predicate type .between columns from the first predicate type. 
                    if (i != 0)
                        column.Filters[i].FilterBehavior = column.Filters[0].FilterBehavior;
                    if (!firstLoop)
                    {
                        // initially we only want the predicate to build WHERE expressions based on AND/OR symbols
                        //if (fp.FilterValue.ToString() != "" && source.Count() > 0)
                        if (source.Count() > 0)
                        {
                            if (fp.FilterValue != null && fp.FilterValue.ToString() != string.Empty)
                            {
                                if (expressionFunc == null)
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, source.ElementType);
                                }
                                else
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, memberType, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, source.ElementType, expressionLambda, typeLambda);
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
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, source.ElementType);
                                }
                                else
                                {
                                    ColumnPredicate = ColumnPredicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, fp.FilterValue, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, source.ElementType));
                                }

                            }
                            else
                            {

                                if (ColumnPredicate == null)
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, memberType, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, source.ElementType, expressionLambda, typeLambda);
                                }
                                else
                                {
                                    ColumnPredicate = ColumnPredicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, fp.FilterValue, memberType, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, source.ElementType, expressionLambda, typeLambda));
                                }

                            }
                        }
                        else if (fp.PredicateType == PredicateType.Or)
                        {
                            if (expressionFunc == null)
                            {
                                if (ColumnPredicate == null)
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, source.ElementType);
                                }
                                else
                                {
                                    ColumnPredicate = ColumnPredicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, fp.FilterValue, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, source.ElementType));
                                }

                            }
                            else
                            {

                                if (ColumnPredicate == null)
                                {
                                    ColumnPredicate = source.Predicate(paramExpression, column.MappingName, fp.FilterValue, memberType, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, source.ElementType, expressionLambda, typeLambda);
                                }
                                else
                                {
                                    ColumnPredicate = ColumnPredicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, fp.FilterValue, memberType, fp.FilterType, fp.FilterBehavior, fp.IsCaseSensitive, source.ElementType, expressionLambda, typeLambda));
                                }
                            }
                        }
                    }
                    if (i == column.Filters.Count - 1 && !Firstpredicate)
                    {
                        if (column != filterColumns.FirstOrDefault())               // Since First Column does not need any Predicate Type to Add with it we just assign the ColumnPredicate to it in else part
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


        private void RefreshDomainFilters()
        {
            var editableView = this as Syncfusion.Windows.Data.IEditableCollectionView;
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
                this.ExtractQueryableFilterPredicates(source, out predicate, out paramExpression);
                if (paramExpression != null && predicate != null)
                {
                    var lambda = Expression.Lambda(predicate, paramExpression);
                    var delg = lambda.Compile();

                    this.Filter = (o) =>
                    {
                        var result = (bool)delg.DynamicInvoke(o);
                        return result;
                    };


                    //This is for add the filter in the paged source
                    if (this.PagedSource != null && !this.IsViewLevelPaging)
                        this.PagedSource.Filter = this.Filter;

                }
            }
            else
            {
                this.Filter = null;


                if (this.PagedSource != null && !this.IsViewLevelPaging)
                    this.PagedSource.Filter = null;


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
            }
        }

        private IQueryable SortQueryable(IQueryable source)
        {
            for (int i = 0; i < this.SortDescriptions.Count; i++)
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
                                source = source.OrderBy(s.PropertyName, expressionFunc);
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
                                    source = source.ThenBy(s.PropertyName, this.SourceType);
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
                                source = source.ThenBy(s.PropertyName, expressionFunc);
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
                                source = source.OrderByDescending(s.PropertyName, expressionFunc);
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
                                source = source.ThenByDescending(s.PropertyName, this.SourceType);
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
                                source = source.ThenByDescending(s.PropertyName, expressionFunc);
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


        public void ExcelFilterPredicates(IQueryable source, out Expression predicate, out ParameterExpression paramExpression, string columnName)
        {
            predicate = null;
            paramExpression = source.ElementType.Parameter();
            var filterColumns = this.FilterPredicates.Where(v => v.Filters != null && v.Filters.Count > 0).ToList();
            if (filterColumns.Count == 0)
            {
                this.Filter = null;
            }
            
            foreach (var column in filterColumns)
            {

                if (columnName != null && column.MappingName.Equals(columnName))
                    continue;

                if (this.IsSelectAllFiltered != null && !(bool)this.IsSelectAllFiltered)
                {
                    this.Filter = (o) =>
                    {
                        return false;
                    };
                    break;

                }
                System.Linq.Expressions.Expression columnPredicate = null;
                int loopCount = column.Filters.Count;

                var srcItems = column.Filters;

                var expressionFunc = this.GetExpressionFunc(column.MappingName);
                var typeFunc = this.GetTypeExpressionFunc(column.MappingName);
                Delegate expressionLambda = null;
                Delegate typeLambda = null;
                if (expressionFunc != null)
                {
                    expressionLambda = expressionFunc.Compile();
                }
                if (typeFunc != null)
                {
                    typeLambda = typeFunc.Compile();
                }
                Type memberType = null;
                if (typeFunc != null)
                {
                    var enumerator = source.GetEnumerator();
                    if (!enumerator.MoveNext())
                    {
                        continue;
                    }
                    var checkDelg = typeFunc.Compile();
                    // invoking this delegate will return a value, that determines the type of method to be called in the Queryable class.
                    memberType = (Type)checkDelg.DynamicInvoke(new object[] { column.MappingName, enumerator.Current });
                }

                for (int i = 0; i < loopCount; i++)
                {

                    var item = srcItems[i];

                    if (expressionFunc == null)
                    {
                        if (columnPredicate == null)
                            columnPredicate = source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType);
                        else
                        {
                            if (item.FilterType == FilterType.Equals)
                                columnPredicate = columnPredicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType));
                            else
                                columnPredicate = columnPredicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType));
                        }
                    }
                    else
                    {
                        if (columnPredicate == null)
                            columnPredicate = source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType, expressionLambda, typeLambda);
                        else
                        {
                            if (item.FilterType == FilterType.Equals)
                                columnPredicate = columnPredicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType, expressionLambda, typeLambda));
                            else
                                columnPredicate = columnPredicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType, expressionLambda, typeLambda));
                        }
                    }
                }

                if (predicate == null)
                    predicate = columnPredicate;
                else
                    predicate = predicate.AndAlsoPredicate(columnPredicate);
            }
        }

       public void GetColumnPredicateExpression(IQueryable source, out Expression predicate, out ParameterExpression paramExpression, GridDataVisibleColumn column)
        {
            predicate = null;
            paramExpression = source.ElementType.Parameter();


            int loopCount = column.Filters.Count;

            var srcItems = column.Filters;

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
            if (typeFunc != null)
            {
                var enumerator = source.GetEnumerator();
                enumerator.MoveNext();
                var checkDelg = typeFunc.Compile();
                // invoking this delegate will return a value, that determines the type of method to be called in the Queryable class.
                memberType = (Type)checkDelg.DynamicInvoke(new object[] { column.MappingName, enumerator.Current });
            }

            for (int i = 0; i < loopCount; i++)
            {

                var item = srcItems[i];

                if (expressionFunc == null)
                {
                    if (predicate == null)
                        predicate = source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType);
                    else
                    {
                        if (item.FilterType == FilterType.Equals)
                            predicate = predicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType));
                        else
                            predicate = predicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType));
                    }
                }
                else
                {
                    if (predicate == null)
                        predicate = source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType, expressionLambda, typeLamda);
                    else
                    {
                        if (item.FilterType == FilterType.Equals)
                            predicate = predicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType, expressionLambda, typeLamda));
                        else
                            predicate = predicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType, expressionLambda, typeLamda));
                    }
                }
            }
        }
    }

}