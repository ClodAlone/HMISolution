#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows.Data;
    using Syncfusion.Linq;
    using Syncfusion.Linq.Data;
    using Syncfusion.Windows.Collections.Generic;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Data;
    using System.Linq.Expressions;
#if !SILVERLIGHT
    using System.Data;
#endif

#if SyncfusionFramework4_0
    using System.Dynamic;
    using Syncfusion.Dynamic;
    
#endif

#if SyncfusionFramework4_0
    public class GridDataModelDynamicPropertiesProvider : DynamicPropertiesProvider
    {
        public GridDataModelDynamicPropertiesProvider(GridDataQueryableCollectionViewWrapper view)
            : base(view)
        {
        }

        public GridDataTableModel TableModel
        {
            get;
            internal set;
        }

        public override object GetValue(object record, string propName)
        {
            var model = this.TableModel;
            if (model != null && model.TableProperties != null && model.TableProperties.VisibleColumns != null)
            {
                var column = model.TableProperties.VisibleColumns.Where(d => d.MappingName == propName).FirstOrDefault();

                if (column != null && column.Binding != null && column.ColumnWrapper != null)
                {
                    column.ColumnWrapper.DataContext = record;
                    var val = column.ColumnWrapper.Value;

                    if (val != null && !string.IsNullOrEmpty(val.ToString()))
                    {
                        var expression = column.ColumnWrapper.GetBindingExpression(GridDataVisibleColumnWrapper.ValueProperty);
                        if (expression != null)
                            return val;
                        else
                        {
                            column.ColumnWrapper.SetValueBinding(column.Binding); //Adding this code to refresh the binding in case of value is null
                            return column.ColumnWrapper.Value;
                        }
                    }
                    else
                    {
                        column.ColumnWrapper.SetValueBinding(column.Binding); //Adding this code to refresh the binding in case of value is null
                        return column.ColumnWrapper.Value;
                    }
                }
                if (column != null && column.ValueConverter != null)
                {
                    return column.ValueConverter.Convert(record, null, column.ValueConverterParameter == null ? column.MappingName : column.ValueConverterParameter, System.Globalization.CultureInfo.CurrentCulture);
                }
            }

            return base.GetValue(record, propName);
        }

        public override bool SetValue(object record, string propName, object value)
        {
            var model = this.TableModel;
            if (model != null)
            {
                var column = model.TableProperties.VisibleColumns.Where(d => d.MappingName == propName).FirstOrDefault();

                if (column != null && column.Binding != null)
                {
                    column.ColumnWrapper.DataContext = record;
                    if (column.ColumnWrapper.Value != value)
                        column.ColumnWrapper.Value = value;
                    return true;
                }

                if (column != null && column.ValueConverter != null)
                {
                    value = column.ValueConverter.ConvertBack(value, null, column.ValueConverterParameter == null ? column.MappingName : column.ValueConverterParameter, System.Globalization.CultureInfo.CurrentCulture);
                }
            }

            return base.SetValue(record, propName, value);
        }
    }
#endif

    public class GridDataModelItemPropertiesProvider : ItemPropertiesProvider
    {
        public GridDataModelItemPropertiesProvider(GridDataQueryableCollectionViewWrapper view)
            : base(view)
        {
        }

        public GridDataTableModel TableModel
        {
            get;
            internal set;
        }

        public override object GetValue(object record, string propName)
        {
            var model = this.TableModel;
            if (model != null)
            {
                var column = model.TableProperties.VisibleColumns.Where(d => d.MappingName == propName).FirstOrDefault();

                if (column != null && column.Binding != null)
                {
                    column.ColumnWrapper.DataContext = record;
                    var val = column.ColumnWrapper.Value;
#if !SILVERLIGHT
                    if (val != null && !string.IsNullOrEmpty(val.ToString()))
                        return val;
                    else
#endif
                    {
                        column.ColumnWrapper.SetValueBinding(column.Binding); //Adding this code to refresh the binding in case of value is null
                        return column.ColumnWrapper.Value;
                    }
                }
                if (column != null && column.ValueConverter != null)
                {
                    return column.ValueConverter.Convert(record, null, column.ValueConverterParameter == null ? column.MappingName : column.ValueConverterParameter, System.Globalization.CultureInfo.CurrentCulture);
                }
            }

            return base.GetValue(record, propName);
        }

        public override bool SetValue(object record, string propName, object value)
        {
            var model = this.TableModel;
            if (model != null)
            {
                var column = model.TableProperties.VisibleColumns.Where(d => d.MappingName == propName).FirstOrDefault();

                if (column != null && column.Binding != null)
                {
                    column.ColumnWrapper.DataContext = record;
                    if ((value != null && column.ColumnWrapper.Value!= null && !column.ColumnWrapper.Value.Equals(value))
                        ||(column.ColumnWrapper.Value == null && value != null)
                        ||(column.ColumnWrapper.Value != null && value == null))
                        column.ColumnWrapper.Value = value;
                    return true;
                }
                
                if (column != null && column.ValueConverter != null)
                {
                    value = column.ValueConverter.ConvertBack(value, null, column.ValueConverterParameter == null ? column.MappingName : column.ValueConverterParameter, System.Globalization.CultureInfo.CurrentCulture);
                }
            }

            return base.SetValue(record, propName, value);
        }
    }

    public class GridDataQueryableCollectionViewWrapper : QueryableCollectionView, IExcelLikeFilterExt
    {
        public GridDataQueryableCollectionViewWrapper(IEnumerable source, GridDataTableModel tableModel,
                                                      Func<object, ICollectionViewAdv, RecordEntry> createRecordFunc)
            : base(source, tableModel.TableProperties.SourceType)
        {
            this.IsInSuspend = true;
            this.TableModel = tableModel;
            this.createRecordFunc = createRecordFunc;
            this.TableSummaryRows.Clear();
            foreach (var tsummaryrow in this.TableModel.TableProperties.TableSummaryRows)
            {
                this.TableSummaryRows.Add(tsummaryrow);
            }

            if (tableModel.TableProperties.SelectFirstRowOnLoad)
            {
                this.MoveCurrentToFirst();
            }

            this.IsInSuspend = false;
            //this.IsExcelLikeFilter = false;
#if SyncfusionFramework4_0
            this.InitDynamicExpressionFunc();
        }

        private Func<string, object, object> dynamicFunc = null;
        private System.Linq.Expressions.Expression<Func<string, object, object>> InitDynamicExpressionFunc()
        {
            if (this.dynamicFunc == null)
            {
                this.dynamicFunc = (propertyName, record) =>
                {
                    var dynamicProvider = this.GetPropertyAccessProvider() as DynamicPropertiesProvider;
                    if (dynamicProvider != null)
                    {
                        return dynamicProvider.GetValue(record, propertyName);
                    }

                    return null;
                };
            }

            return (propertyName, record) => this.dynamicFunc(propertyName, record);
        }

        private Func<string, object, object> dynamicTypeFunc = null;
        private System.Linq.Expressions.Expression<Func<string, object, object>> InitDynamicTypeExpressionFunc()
        {
            if (this.dynamicTypeFunc == null)
            {
                this.dynamicTypeFunc = (propertyName, record) =>
                {
                    var dynamicProvider = this.GetPropertyAccessProvider() as DynamicPropertiesProvider;
                    if (dynamicProvider != null)
                    {
                        var Value= dynamicProvider.GetValue(record, propertyName);
                        if (Value == null || Value == DBNull.Value)
                            return null;
                        else
                            return Value.GetType();
                    }

                    return null;
                };
            }

            return (propertyName, record) => this.dynamicTypeFunc(propertyName, record);
        }
#else
        }
#endif

        internal void InternalSetSourceType(Type sourceType)
        {
            this.SetSourceType(sourceType);
        }


        internal void SetViewSource(IEnumerable source)
        {
            SetSource(source);
            SetSource(source.AsQueryable());
        }


        private Func<object, ICollectionViewAdv, RecordEntry> createRecordFunc;
        public override RecordEntry CreateRecordEntry(object data)
        {
            if (this.createRecordFunc != null)
                return this.createRecordFunc(data, this);
            else
                return null;
        }

        private GridDataTableModel model;
        public GridDataTableModel TableModel
        {
            get
            {
                return this.model;
            }
            private set
            {
                if (this.model != value)
                {
                    this.model = value;
                    var propertiesProvider = this.GetPropertyAccessProvider();
                    // if the underlying provider is a dynamic source
                    if (propertiesProvider != null)
                    {
                        var itempropertypropertiesProvider = propertiesProvider as GridDataModelItemPropertiesProvider;
                        if (itempropertypropertiesProvider != null && itempropertypropertiesProvider.TableModel == null)
                        {
                            itempropertypropertiesProvider.TableModel = value;
                        }
#if SyncfusionFramework4_0
                        var dynamicpropertiesProvider = propertiesProvider as GridDataModelDynamicPropertiesProvider;
                        if (dynamicpropertiesProvider != null && dynamicpropertiesProvider.TableModel == null)
                        {
                            dynamicpropertiesProvider.TableModel = value;
                        }
#endif
                    }
                }
            }
        }

        protected override IEnumerable<string> GetMappingNames()
        {
            if (this.TableModel != null && this.TableModel.TableProperties != null && this.TableModel.TableProperties.VisibleColumns != null)
                return this.TableModel.TableProperties.VisibleColumns.GetMappingNames();
            else
                return null;
        }

        protected override IPropertyAccessProvider CreateItemPropertiesProvider()
        {
#if SyncfusionFramework4_0
            if (this.IsDynamicBound)
            {
                return new GridDataModelDynamicPropertiesProvider(this);
                //return base.CreateItemPropertiesProvider();
            }
#endif
            if (this.IsInterfaceBound)
            {
                return base.CreateItemPropertiesProvider();
            }

            return new GridDataModelItemPropertiesProvider(this);
        }

        protected override void Dispose(bool disposing)
        {
            //if (this.TableModel != null && this.TableModel.IsSourceReset)
            //{
            //    disposing = false;
            //}

            base.Dispose(disposing);
            if (disposing)
            {
                this.UnwireEvents();
                this.createRecordFunc = null;
#if SyncfusionFramework4_0
                this.dynamicFunc = null;
#endif

                this.model = null;
                this.TableModel = null;
            }
        }

        protected override IRecordsList CreateRecords()
        {
            var recordsList = EnumerableRecordsWrapper.CreateNew(this.ViewSource, this);
            return (IRecordsList)recordsList;
        }

        protected override TopLevelGroup CreateTopLevelGroup()
        {
            var topLevelgroup = new GridDataTopLevelGroup(this.TableModel, this);
            return topLevelgroup;
        }

        protected override void OnTopLevelGroupPopulated(TopLevelGroup topLevelGroup)
        {
            // var gridDataTopLevelGroup = topLevelGroup as GridDataTopLevelGroup; Unused local variable
            this.CaptionSummaryRow = this.TableModel.TableProperties.CaptionSummaryRow;
            this.SummaryRows.Clear();

            foreach (var row in this.TableModel.TableProperties.SummaryRows)
            {
                this.SummaryRows.Add((ISummaryRow)row);
            }

            //gridDataTopLevelGroup.UpdateCaptionSummaries();
        }

        internal void UpdateViewCurrentItem()
        {
            this.UpdateCurrentItem();
        }

        internal void UpdateViewCurrentPosition()
        {
            this.UpdateCurrentPosition();
        }

        public override Func<string, object, object> GetFunc(string propertyName)
        {
            if (!this.TableModel.IsInitialized)
            {
                return null;
            }

            Func<string, object, object> func = base.GetFunc(propertyName);
            if (func != null)
            {
                return func;
            }

#if SyncfusionFramework4_0
            if (this.TableModel.Table.IsDynamicBound)
            {
                var column = this.TableModel.TableProperties.VisibleColumns.OfType<GridDataUnboundVisibleColumn>().FirstOrDefault(v => v.MappingName == propertyName);
                if (column == null)
                {
                    return this.dynamicFunc;
                }
            }
#endif

            var valueConverter = this.TableModel.GetConverterFunc(propertyName);
            if (valueConverter != null)
            {
                return valueConverter;
            }

#if !SILVERLIGHT
            if (!this.TableModel.IsLegacyDataTable)
#endif
            {
                var unboundExpFunc = this.TableModel.GetUnboundFunc(propertyName);
                if (unboundExpFunc != null)
                {
                    return unboundExpFunc;
                }
            }


            return null;
        }

        public override Func<string, object, object> GetTypeFunc(string propertyName)
        {
            if (!this.TableModel.IsInitialized)
            {
                return null;
            }

            Func<string, object, object> func = base.GetTypeFunc(propertyName);
            if (func != null)
            {
                return func;
            }

#if SyncfusionFramework4_0
            if (this.TableModel.Table.IsDynamicBound)
            {
                var column = this.TableModel.TableProperties.VisibleColumns.OfType<GridDataUnboundVisibleColumn>().FirstOrDefault(v => v.MappingName == propertyName);
                if (column == null)
                {
                    return this.dynamicTypeFunc;
                }
            }
#endif

            var valueConverter = this.TableModel.GetConverterFunc(propertyName);
            if (valueConverter != null)
            {
                return valueConverter;
            }

#if !SILVERLIGHT
            if (!this.TableModel.IsLegacyDataTable)
#endif
            {
                var unboundExpFunc = this.TableModel.GetUnboundTypeFunc(propertyName);
                if (unboundExpFunc != null)
                {
                    return unboundExpFunc;
                }
            }


            return null;
        }

        #region IExcelLikeFilterExt Members

        public bool IsExcelLikeFilter { get; set; }

        public bool? IsSelectAllFiltered { get; set; }

        #endregion

        /// <summary>
        /// To handle the Excel Like Check box filtering.
        /// </summary>
        public override void RefreshFilters()
        {
            if (!this.IsExcelLikeFilter)
            {
                base.RefreshFilters();
            }
            else if (this.IsExcelLikeFilter)
            {
                var filterColumns = this.FilterPredicates.Where(v => v.Filters != null && v.Filters.Count > 0).ToList();

                if (filterColumns.Count == 0 && ((this.IsSelectAllFiltered != null && this.IsSelectAllFiltered.Value == true) || this.IsSelectAllFiltered == null))
                {
                    this.RowFilter = null;
                }
                else if (filterColumns.Count > 0 && this.IsSelectAllFiltered != null && this.IsSelectAllFiltered.Value == false)
                {
                    this.RowFilter = (o) =>
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
                        this.RowFilter = (o) =>
                        {
                            var result = (bool)delg.DynamicInvoke(o);
                            return result;
                        };
                    }
                }

                if (this.IsGrouping && this.TopLevelGroup != null)
                {
                    // no need to check for EndDefer,
                    //if (!this.IsInEndDefer)
                    {
                        this.RefreshSortingOrderWithFiltersForBottomLevel(this.TopLevelGroup.Groups);
                    }
                    this.TopLevelGroup.SetDirty();
                    var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                    grpRefresh.RefreshFilters();
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// To Create the predicate for Excel Like Check box filter
        /// </summary>
        /// <param name="source">Item Source</param>
        /// <param name="predicate">Expression</param>
        /// <param name="paramExpression">ParameterExpression</param>
        public void ExcelFilterPredicates(IQueryable source, out Expression predicate, out ParameterExpression paramExpression, string columnName)
        {
            predicate = null;
            paramExpression = source.ElementType.Parameter();
            var filterColumns = this.FilterPredicates.Where(v => v.Filters != null && v.Filters.Count > 0).ToList();
            if (filterColumns.Count == 0)
            {
                this.RowFilter = null;
            }
            //bool firstLoop = false;
            //bool firstColumnLoop = false;
            //bool changeOfColumn = false; Unused local variable
            foreach (var column in filterColumns)
            {
                if (columnName != null && column.MappingName.Equals(columnName))
                    continue;

                if (this.IsSelectAllFiltered != null && !(bool)this.IsSelectAllFiltered)
                {
                    this.RowFilter = (o) =>
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
                    // invoking this delegate will return the type of the Column which determines the method to be called in the Queryable class.
                    memberType = (Type)checkDelg.DynamicInvoke(new object[] { column.MappingName, enumerator.Current });
                }

                for (int i = 0; i < loopCount; i++)
                {
                    var item = srcItems[i];

                    if (expressionFunc == null)
                    {
                        if (columnPredicate == null)
                            columnPredicate = source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, this.SourceType);
                        else if (item.PredicateType == PredicateType.Or)
                            columnPredicate = columnPredicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, this.SourceType));
                        else
                            columnPredicate = columnPredicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, this.SourceType));                        
                    }
                    else
                    {
                        if (columnPredicate == null)
                            columnPredicate = source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, this.SourceType, expressionLambda, typeLamda);
                        else if (item.PredicateType == PredicateType.Or)
                            columnPredicate = columnPredicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, this.SourceType, expressionLambda, typeLamda));
                        else
                            columnPredicate = columnPredicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, this.SourceType, expressionLambda, typeLamda));                        
                    }
                }

                if (predicate == null)
                    predicate = columnPredicate;
                else
                    predicate = predicate.AndAlsoPredicate(columnPredicate);
            }
        }

        public override System.Linq.Expressions.Expression<Func<string, object, object>> GetExpressionFunc(string propertyName)
        {
            if (!this.TableModel.IsInitialized)
            {
                return null;
            }

            System.Linq.Expressions.Expression<Func<string, object, object>> expFunc = base.GetExpressionFunc(propertyName);
            if (expFunc != null)
            {
                return expFunc;
            }

#if SyncfusionFramework4_0
            if (this.TableModel.Table.IsDynamicBound)
            {
                var column = this.TableModel.TableProperties.VisibleColumns.OfType<GridDataUnboundVisibleColumn>().FirstOrDefault(v => v.MappingName == propertyName);
                if (column == null)
                {
                    return this.InitDynamicExpressionFunc();
                }
            }
#endif

            var valueConverter = this.TableModel.GetConverterExpressionFunc(propertyName);
            if (valueConverter != null)
            {
                return valueConverter;
            }

#if !SILVERLIGHT
            if (!this.TableModel.IsLegacyDataTable)
            {
#endif
                var visiblecolumn = this.TableModel.TableProperties.VisibleColumns.OfType<GridDataVisibleColumn>().FirstOrDefault(v => v.MappingName == propertyName);
                if (visiblecolumn != null && visiblecolumn.Binding != null)
                {
                    var boundExpFunc = this.TableModel.GetboundExpressionFunc(propertyName);
                    if (boundExpFunc != null)
                        expFunc = boundExpFunc;
                }
                else
                {
                    var unboundExpFunc = this.TableModel.GetUnboundExpressionFunc(propertyName);
                    if (unboundExpFunc != null)
                        expFunc = unboundExpFunc;
                }
#if !SILVERLIGHT
            }
#endif
            return expFunc;
        }

        public override System.Linq.Expressions.Expression<Func<string, object, object>> GetTypeExpressionFunc(string propertyName)
        {
            if (!this.TableModel.IsInitialized)
            {
                return null;
            }

            System.Linq.Expressions.Expression<Func<string, object, object>> expFunc = base.GetTypeExpressionFunc(propertyName);
            if (expFunc != null)
            {
                return expFunc;
            }

#if SyncfusionFramework4_0
            if (this.TableModel.Table.IsDynamicBound)
            {
                var column = this.TableModel.TableProperties.VisibleColumns.OfType<GridDataUnboundVisibleColumn>().FirstOrDefault(v => v.MappingName == propertyName);
                if (column == null)
                {
                    return this.InitDynamicTypeExpressionFunc();
                }
            }
#endif

            var valueConverter = this.TableModel.GetConverterExpressionFunc(propertyName);
            if (valueConverter != null)
            {
                return valueConverter;
            }

#if !SILVERLIGHT
            if (!this.TableModel.IsLegacyDataTable)
            {
#endif
                var visiblecolumn = this.TableModel.TableProperties.VisibleColumns.OfType<GridDataVisibleColumn>().FirstOrDefault(v => v.MappingName == propertyName);
                if (visiblecolumn != null && visiblecolumn.Binding != null)
                {
                    var boundExpFunc = this.TableModel.GetboundTypeExpressionFunc(propertyName);
                    if (boundExpFunc != null)
                        expFunc = boundExpFunc;
                }
                else
                {
                    var unboundExpFunc = this.TableModel.GetUnboundTypeExpressionFunc(propertyName);
                    if (unboundExpFunc != null)
                        expFunc = unboundExpFunc;
                }
#if !SILVERLIGHT
            }
#endif
            return expFunc;
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
            if (expressionFunc != null)
            {
                var enumerator = source.GetEnumerator();
                enumerator.MoveNext();
                var checkDelg = typeFunc.Compile();
                // invoking this delegate will return the type of the column which determines the method to be called in the Queryable class.
                memberType = (Type)checkDelg.DynamicInvoke(new object[] { column.MappingName, enumerator.Current });
            }
            for (int i = 0; i < loopCount; i++)
            {

                var item = srcItems[i];

                if (expressionFunc == null)
                {
                    if (predicate == null)
                        predicate = source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, this.SourceType);
                    else if (item.PredicateType == PredicateType.Or)
                        predicate = predicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType));
                    else
                        predicate = predicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType));
                }
                else
                {
                    if (predicate == null)
                        predicate = source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, this.SourceType, expressionLambda, typeLambda);
                    else if (item.PredicateType == PredicateType.Or)
                        predicate = predicate.OrElsePredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType, expressionLambda, typeLambda));
                    else
                        predicate = predicate.AndAlsoPredicate(source.Predicate(paramExpression, column.MappingName, item.FilterValue, item.FilterType, item.FilterBehavior, true, source.ElementType, expressionLambda, typeLambda));                    
                }
            }
        }
    }

#if !SILVERLIGHT
    public class GridDataTableCollectionViewWrapper : DataTableCollectionView, IExcelLikeFilterExt
    {
        public GridDataTableCollectionViewWrapper(IEnumerable source, GridDataTableModel tableModel, Func<object, ICollectionViewAdv, RecordEntry> createRecordFunc)
            : base(source)
        {
            this.TableModel = tableModel;
            this.createRecordFunc = createRecordFunc;
            this.TableSummaryRows.Clear();
            foreach (var tsummaryrow in this.TableModel.TableProperties.TableSummaryRows)
            {
                this.TableSummaryRows.Add(tsummaryrow);
            }

            if (tableModel.TableProperties.SelectFirstRowOnLoad)
            {
                this.MoveCurrentToFirst();
            }
        }

        internal void InternalSetSourceType(Type sourceType)
        {
            this.SetSourceType(sourceType);
        }

        Func<object, ICollectionViewAdv, RecordEntry> createRecordFunc = null;
        public override RecordEntry CreateRecordEntry(object data)
        {
            return this.createRecordFunc(data, this);
        }

        public GridDataTableModel TableModel
        {
            get;
            private set;
        }

        private DataView FilteredSource = null;

        protected override IRecordsList CreateRecords()
        {
            var recordsList = EnumerableRecordsWrapper.CreateNew(this.ViewSource, this);
            return (IRecordsList)recordsList;
        }

        protected override TopLevelGroup CreateTopLevelGroup()
        {
            var topLevelgroup = new GridDataTopLevelGroup(this.TableModel, this);
            return topLevelgroup;
        }

        protected override void OnTopLevelGroupPopulated(TopLevelGroup topLevelGroup)
        {
            // var gridDataTopLevelGroup = topLevelGroup as GridDataTopLevelGroup; Unused local variable
            this.CaptionSummaryRow = this.TableModel.TableProperties.CaptionSummaryRow;
            this.SummaryRows.Clear();

            foreach (var row in this.TableModel.TableProperties.SummaryRows)
            {
                this.SummaryRows.Add((ISummaryRow)row);
            }

            //gridDataTopLevelGroup.UpdateCaptionSummaries();
        }

        internal void EnsureRecordsInitialized()
        {
            if (this.Records == null)
            {
                this.EnsureInitialized();
            }
        }

        protected override void EnsureInitialized()
        {
            base.EnsureInitialized();
        }

        public DataView GetClonedSource(out PropertyDescriptorCollection clonedItemsProperties)
        {
            clonedItemsProperties = ((ITypedList)(FilteredSource)).GetItemProperties(null);
            return FilteredSource;
        }

        private DataTable CloneSource(DataView dv)
        {
            var filterString = dv.RowFilter;
            var canSuspend = !string.IsNullOrEmpty(filterString);
            if (canSuspend)
            {
                this.IsInSuspend = true;
                dv.RowFilter = string.Empty;
            }
            DataTable dt = dv.Table.Clone();
            foreach (DataRowView dvr in dv)
            {
                dt.ImportRow(dvr.Row);
            }
            dt.AcceptChanges();
            if (canSuspend)
            {
                dv.RowFilter = filterString;
                this.IsInSuspend = false;
            }
            return dt;
        }

        #region IExcelLikeFilterExt Members

        public bool IsExcelLikeFilter { get; set; }

        public bool? IsSelectAllFiltered { get; set; }

        #endregion

        public override void RefreshFilters()
        {
            if (!this.IsExcelLikeFilter)
            {

                var filterColumns1 =
                    this.FilterPredicates.Where(v => v.Filters != null && v.Filters.Count > 0)
                        .ToList<GridDataVisibleColumn>()
                        .Where(f => f.IsCaseSensitiveFilter)
                        .ToList();

                if (this.IsLegacyDataTable && this.ViewSource != null)
                {
                    this.ViewSource.Table.CaseSensitive = filterColumns1.Count > 0;
                }

                base.RefreshFilters();
            }
            else if (this.IsExcelLikeFilter)
            {

                var filterColumns = this.FilterPredicates.Where(v => v.Filters != null && v.Filters.Count > 0).ToList();

                if (filterColumns.Count == 0 && ((this.IsSelectAllFiltered != null && this.IsSelectAllFiltered.Value == true) || this.IsSelectAllFiltered == null))
                {
                    if (this.TableModel is GridDataChildTableModel)
                        this.ViewSource.RowFilter = (this.TableModel as GridDataChildTableModel).RowFilter;
                    else
                        this.ViewSource.RowFilter = string.Empty;
                }
                else if (filterColumns.Count > 0 && this.IsSelectAllFiltered != null && this.IsSelectAllFiltered.Value == false)
                {
                        this.ViewSource.RowFilter = string.Empty;
                }
                else
                {
                    FilteredSource = CloneSource(this.SourceCollection as DataView).DefaultView;
                    var filterString = GetFilterString(null);
                    var needsRefresh = false;
                    if (this.ViewSource.RowFilter != string.Empty && filterString == string.Empty)
                    {
                        // when the rowfilter is set as string.empty we need to manuall refresh the state
                        needsRefresh = true;
                    }
                    if (this.TableModel is GridDataChildTableModel)
                    {
                        var tempFilterString = (this.TableModel as GridDataChildTableModel).RowFilter;
                        if (filterString != string.Empty)
                        {
                            tempFilterString = tempFilterString.AndPredicate();
                            tempFilterString += " " + filterString;
                        }
                        this.ViewSource.RowFilter = tempFilterString;
                    }
                    else
                        this.ViewSource.RowFilter = filterString;
                    if (needsRefresh)
                    {
                        this.EnsureInitialized();
                    }
                }
            }

            if (this.IsGrouping && this.TopLevelGroup != null)
            {
                // no need to check for EndDefer,
                // if (!this.IsInEndDefer)
                {
                    this.ViewSource.RowFilter = this.GetFilterString(null);
                    //this.RefreshSortingOrderWithFiltersForBottomLevel(this.TopLevelGroup.Groups, string.Empty, 0);
                }

                var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                grpRefresh.RefreshFilters();
            }

        }


        internal String GetFilterString(string columnName)
        {
            var filterString = string.Empty;
            string tempfilterstring = string.Empty;
            bool IsChanged = false;
            var firstLoop = false;
            bool changeOfColumn = false;
            var filterColumns = this.FilterPredicates.Where(v => v.Filters != null && v.Filters.Count > 0).ToList();
            if (filterColumns.Count == 0)
            {
                this.ViewSource.RowFilter = string.Empty;
            }
           
            foreach (var column in filterColumns)
            {
                tempfilterstring = string.Empty;
                if (columnName != null && column.MappingName.Equals(columnName))
                    continue;

                int loopCount = column.Filters.Count;
                var srcItems = column.Filters;

                if (this.IsSelectAllFiltered != null && !(bool)this.IsSelectAllFiltered)
                {

                    this.ViewSource.RowFilter = string.Empty;
                    break;

                }

                for (int i = 0; i < loopCount; i++)
                {
                    var fp = srcItems[i];
                    // For null value filtering we need to check null values.
                    if (fp.FilterValue == DBNull.Value)
                    {
                        var visibleColumn = this.TableModel.TableProperties.VisibleColumns[column.MappingName];
                        if (visibleColumn.ColumnType == typeof(string))
                        {
                            columnName = EscapeSpecialChars(column.MappingName);
                            if (!firstLoop)
                            {
                                if (fp.FilterType == FilterType.Equals)
                                    tempfilterstring = "IsNull(" + columnName + ", 'Null Column')='Null Column'";
                                if (fp.FilterType == FilterType.NotEquals)
                                    tempfilterstring = "IsNull(" + columnName + ", 'Null Column')<>'Null Column'";
                                firstLoop = true;
                            }
                            else if (changeOfColumn)
                            {
                                IsChanged = true;
                                if (fp.FilterType == FilterType.Equals)
                                    tempfilterstring += "IsNull(" + columnName + ", 'Null Column')='Null Column'";
                                if (fp.FilterType == FilterType.NotEquals)
                                    tempfilterstring += "IsNull(" + columnName + ", 'Null Column')<>'Null Column'";
                                changeOfColumn = false;
                            }
                            else
                            {
                                if (fp.FilterType == FilterType.Equals)
                                {
                                    tempfilterstring = tempfilterstring.OrPredicate();
                                    tempfilterstring += "IsNull(" + columnName + ", 'Null Column')='Null Column'";
                                }
                                else
                                {
                                    tempfilterstring = tempfilterstring.AndPredicate();
                                    tempfilterstring += "IsNull(" + columnName + ", 'Null Column')<>'Null Column'";
                                }
                            }
                        }
                        else
                        {
                            columnName = EscapeSpecialChars(column.MappingName);
                            if (!firstLoop)
                            {
                                if (fp.FilterType == FilterType.Equals)
                                    tempfilterstring = columnName + " is NULL";
                                if (fp.FilterType == FilterType.NotEquals)
                                    tempfilterstring = columnName + " is NOT NULL";
                                firstLoop = true;
                            }
                            else if (changeOfColumn)
                            {
                                IsChanged = true;
                                if (fp.FilterType == FilterType.Equals)
                                    tempfilterstring += columnName + " is NULL";
                                if (fp.FilterType == FilterType.NotEquals)
                                    tempfilterstring += columnName + " is NOT NULL";
                                changeOfColumn = false;
                            }
                            else
                            {
                                if (fp.FilterType == FilterType.Equals)
                                {
                                    tempfilterstring = tempfilterstring.OrPredicate();
                                    tempfilterstring += columnName + " is NULL";
                                }
                                else
                                {
                                    tempfilterstring = tempfilterstring.AndPredicate();
                                    tempfilterstring += columnName + " is NOT NULL";
                                }
                            }
                        }
                    }
                    else if (fp.FilterValue != null)
                    {
                        if (!firstLoop)
                        {
                            tempfilterstring = tempfilterstring.Predicate(column.MappingName, fp.FilterValue, fp.FilterType);
                            firstLoop = true;
                        }
                        else if (changeOfColumn)
                        {
                            IsChanged = true;
                            tempfilterstring = tempfilterstring.Predicate(column.MappingName, fp.FilterValue, fp.FilterType);

                            changeOfColumn = false;
                        }
                        else
                        {
                            if (fp.PredicateType == PredicateType.Or)
                            {
                                tempfilterstring = tempfilterstring.OrPredicate();
                                tempfilterstring = tempfilterstring.Predicate(column.MappingName, fp.FilterValue, fp.FilterType);
                            }
                            else
                            {
                                tempfilterstring = tempfilterstring.AndPredicate();
                                tempfilterstring = tempfilterstring.Predicate(column.MappingName, fp.FilterValue, fp.FilterType);
                            }
                        }
                    }
                }
                changeOfColumn = true;
                if (IsChanged)
                {
                    filterString = filterString.AndPredicate() + "(" + tempfilterstring + ")";
                    IsChanged = false;
                }
                else
                {
                    filterString += "(" + tempfilterstring + ")";
                }
            }
            return filterString;
        }

        public void ExcelFilterPredicates(IQueryable source, out Expression predicate, out ParameterExpression paramExpression, string columnName)
        {
            throw new NotImplementedException();
        }

        public void GetColumnPredicateExpression(IQueryable source, out Expression predicate, out ParameterExpression paramExpression, GridDataVisibleColumn column)
        {
            throw new NotImplementedException();
        }
    }

#endif

    public class GridDataTopLevelGroup : TopLevelGroup /*, IGroupList , IGroupingModel*/
    {
        public GridDataTopLevelGroup(GridDataTableModel model, CollectionViewAdv collectionView)
            : base(collectionView)
        {
            this.TableModel = model;
        }

        public override void Invalidate(int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var resolvedindex = this.TableModel.ResolveGroupPositionToIndex(index + i);
                this.TableModel.InvalidateCell(GridRangeInfo.Row(resolvedindex));
            }
            //InvalidateCell itself call the InvalidateVisual. So, it is not required to call InvalidateVisual here.
            ////this.TableModel.InvalidateVisual(false);
        }

        #region TableModel Properties

        public GridDataTableModel TableModel
        {
            get;
            private set;
        }

        public GridDataTableProperties TableProperties
        {
            get
            {
                return this.TableModel.TableProperties;
            }
        }

        public GridDataTable Table
        {
            get
            {
                return this.TableModel.Table;
            }
        }

        public override int RelationsCount
        {
            get
            {
                /// Groupped grid's row count will be calculated by the linq, linq will get 
                /// the relation count from here to add additional row for nested table, to add
                /// the row for details view additional value is added to relation count.
                return (this.Table.HasNestedTables ? this.TableProperties.Relations.Count : 0) + (this.Table.HasDetailsView ? 1 : 0);
            }
            set
            {
                base.RelationsCount = value;
            }
        }

        #endregion

        #region Expand/Collapse Group

        public void ComputeCount(Group group, ref int itemcount)
        {
            if (group.IsExpanded)
            {
                if (!group.IsBottomLevel)
                {
                    itemcount += group.GetGroupsCount();
                    foreach (var childGroup in group.Groups)
                    {
                        ComputeCount(childGroup, ref itemcount);
                    }
                }
                else
                {
                    /// Calculating the row count of a expanded group
                    if (this.TableProperties.ShowRecordPlusMinus || this.Table.HasDetailsView)
                    {
                        int rCount = group.GetRelationsCount();
                        itemcount += (group.Records.Count * (rCount + 1)) + ((GroupRecordEntry)group.Details).Summaries.Count;
                    }
                    else
                    {
                        itemcount += group.Records.Count + ((GroupRecordEntry)group.Details).Summaries.Count;
                    }
                }
            }
        }

        public virtual int ExpandGroup(Group group)
        {
            this.ResetCache = true;
            int itemcount = 0;
            if (!group.IsExpanded)
            {
                group.IsExpanded = true;
                this.ComputeCount(group, ref itemcount);
            }

            return itemcount;
        }

        public virtual int CollapseGroup(Group group)
        {
            this.ResetCache = true;
            int itemcount = 0;
            if (group.IsExpanded)
            {
                ComputeCount(group, ref itemcount);
                group.IsExpanded = false;
            }
            return itemcount;
        }

        /// <summary>
        /// Determines whether [has details view].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has details view]; otherwise, <c>false</c>.
        /// </returns>
        public override bool HasDetailsView()
        {
            /// passing the existance of details view to linq from here.
            return this.TableModel.Table.HasDetailsView;
        }

        #endregion

#if !SILVERLIGHT

        #region GroupCaptionText
        public override string GetGroupCaptionText(Group group, string groupSpecifierText, string columnHeaderName)
        {
            var kvp = base.GetGroupCaptionTextList(group, groupSpecifierText, columnHeaderName);

            var visibleColumn = this.TableProperties.VisibleColumns.Where(d => d.HeaderText == columnHeaderName).FirstOrDefault();
            if (visibleColumn != null && visibleColumn.TableModel != null && visibleColumn.TableModel.IsLegacyDataTable)
            {
                Type type = visibleColumn.ColumnType;
                if (type.IsEnum)
                {
                    //Set the Enum integer value into respect Enum property.
                    object value = kvp.Value[1];
                    value = Enum.ToObject(type, Convert.ToInt32(value));
                    kvp.Value[1] = value.ToString();
                }
            }
            if (visibleColumn != null && visibleColumn.ColumnStyle != null)
            {
                if (visibleColumn.ColumnStyle != null && visibleColumn.ColumnStyle.HasDateTimeEdit)
                {
                    var array = new string[kvp.Value.Count];
                    int i = 0;
                    foreach (var val in kvp.Value)
                    {
                        DateTime dateTime = DateTime.MinValue;
                        if (DateTime.TryParse(val, visibleColumn.ColumnStyle.CultureInfo, System.Globalization.DateTimeStyles.None, out dateTime))
                        {
                            //Debug.WriteLine(dateTime.ToString());
                            array[i] = GridCellDateTimeEditCellModel.GetDateTimePatternString(dateTime, visibleColumn.ColumnStyle.DateTimeEdit, visibleColumn.ColumnStyle.DateTimeEdit.DateTimePattern, visibleColumn.ColumnStyle.CultureInfo != null ? visibleColumn.ColumnStyle.CultureInfo : System.Globalization.CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            array[i] = val;
                        }

                        i++;
                    }

                    return String.Format(kvp.Key, array);
                }
            }

            return String.Format(kvp.Key, kvp.Value.ToArray());
        }

        public string GetGroupCaptionText(Group group, string groupSpecifierText, string columnHeaderName, IValueConverter converter)
        {
            var kvp = base.GetGroupCaptionTextList(group, groupSpecifierText, columnHeaderName);

            var visibleColumn = this.TableProperties.VisibleColumns.Where(d => d.HeaderText == columnHeaderName).FirstOrDefault();
            if (visibleColumn != null && visibleColumn.TableModel != null && visibleColumn.TableModel.IsLegacyDataTable)
            {
                Type type = visibleColumn.ColumnType;
                if (type.IsEnum)
                {
                    //Set the Enum integer value into respect Enum property.
                    object value = kvp.Value[1];
                    value = Enum.ToObject(type, Convert.ToInt32(value));
                    kvp.Value[1] = value.ToString();
                }
            }
            if (visibleColumn != null && visibleColumn.ColumnStyle != null)
            {
                if (visibleColumn.ColumnStyle != null && visibleColumn.ColumnStyle.HasDateTimeEdit)
                {
                    // var array = new string[kvp.Value.Count]; Unused local variable
                    int i = 0;
                    //foreach (var val in kvp.Value)
                    for(int j=0;j<kvp.Value.Count;j++)
                    {
                        var val = kvp.Value[j];
                        DateTime dateTime = DateTime.MinValue;
                        if (DateTime.TryParse(val, visibleColumn.ColumnStyle.CultureInfo, System.Globalization.DateTimeStyles.None, out dateTime))
                        {
                            //Debug.WriteLine(dateTime.ToString());
                            //array[i] = GridCellDateTimeEditCellModel.GetDateTimePatternString(dateTime, visibleColumn.ColumnStyle.DateTimeEdit, visibleColumn.ColumnStyle.DateTimeEdit.DateTimePattern, visibleColumn.ColumnStyle.CultureInfo != null ? visibleColumn.ColumnStyle.CultureInfo : System.Globalization.CultureInfo.CurrentCulture);
                            kvp.Value[i] = GridCellDateTimeEditCellModel.GetDateTimePatternString(dateTime, visibleColumn.ColumnStyle.DateTimeEdit, visibleColumn.ColumnStyle.DateTimeEdit.DateTimePattern, visibleColumn.ColumnStyle.CultureInfo != null ? visibleColumn.ColumnStyle.CultureInfo : System.Globalization.CultureInfo.CurrentCulture);
                        }
                        else
                        {
                            //array[i] = val;
                            kvp.Value[i] = val;
                        }

                        i++;
                    }

                    //return String.Format(kvp.Key, array);
                }
            }
            return converter.Convert(kvp, typeof(object), columnHeaderName, System.Globalization.CultureInfo.CurrentCulture).ToString();
            //return String.Format(kvp.Key, kvp.Value.ToArray());
        }
        #endregion
#endif

    }

    #region Excel Like Filter Extension

    /// <summary>
    /// Excel Like Check Box Filter Extension interface
    /// </summary>
    public interface IExcelLikeFilterExt
    {
        bool IsExcelLikeFilter { get; set; }
        bool? IsSelectAllFiltered { get; set; }
        void ExcelFilterPredicates(IQueryable source, out System.Linq.Expressions.Expression predicate, out ParameterExpression paramExpression, string columnName);
        void GetColumnPredicateExpression(IQueryable source, out System.Linq.Expressions.Expression predicate, out ParameterExpression paramExpression, GridDataVisibleColumn column);
    }

    #endregion

}
