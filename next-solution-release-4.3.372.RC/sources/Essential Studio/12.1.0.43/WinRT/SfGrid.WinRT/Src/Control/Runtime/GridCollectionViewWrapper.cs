#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
#if WinRT
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Controls;
#if WPF
using System.Data;
#endif
#endif


namespace Syncfusion.UI.Xaml.Grid
{

    public class GridQueryableCollectionViewWrapper : QueryableCollectionView
    {
        #region Fields
        SfDataGrid datagrid;
        #endregion

        #region ctor
        public GridQueryableCollectionViewWrapper(IEnumerable source, SfDataGrid grid)
            : base(source, grid.SourceType)
        {
            datagrid = grid;
            propertyAccessProvider = this.CreateItemPropertiesProvider();
        }
        #endregion

        #region private methods
        private object ConvertToType(object value, Type type)
        {
#if WinRT
            var method = type.GetTypeInfo().DeclaredMethods.Where(x => x.Name.Equals("TryParse"));
#else
            var method = type.GetMethods().Where(x => x.Name.Equals("TryParse"));
#endif
            if (method.Any())
            {
                var methodinfo = method.FirstOrDefault();
                object[] args = { value.ToString(), null };
                if ((bool)methodinfo.Invoke(null, args))
                    return args[1];
                return null;
            }
            return null;
        }
        #endregion

        #region Override Functions

        public override Func<string, object, object> GetFunc(string propertyName)
        {
            var column = this.datagrid.Columns.FirstOrDefault(col => col.MappingName == propertyName);

            if (column != null && column.IsUnbound && column is GridUnBoundColumn)
            {
                var unboundcolumn = column as GridUnBoundColumn;
                unboundcolumn.UnBoundFunc = this.GetUnboundFunc(propertyName);
                if (unboundcolumn.UnBoundFunc != null)
                    return (columnName, record) => unboundcolumn.UnBoundFunc(columnName, record);
                return null;
            }

            if (column != null && column.ValueBinding != null && column.UseBindingValue)
                return this.GetBindingFunc();

            var func = base.GetFunc(propertyName);
            if (func != null)
                return func;

#if !WP
            if (this.IsDynamicBound)
            {
                return this.GetDynamicFunc();
            }
#endif
            return null;
        }

        /// <summary>
        /// Returns Expression Func for UnBoundColumns for Sorting and Grouping.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override System.Linq.Expressions.Expression<Func<string, object, object>> GetExpressionFunc(string propertyName)
        {
            var column = this.datagrid.Columns.FirstOrDefault(col => col.MappingName == propertyName);

            if (column != null && column.IsUnbound && column is GridUnBoundColumn)
            {
                var unboundcolumn = column as GridUnBoundColumn;
                unboundcolumn.UnBoundFunc = this.GetUnboundFunc(propertyName);
                if (unboundcolumn.UnBoundFunc != null)
                    return (columnName, record) => unboundcolumn.UnBoundFunc(columnName, record);
                return null;
            }

            if (column != null && column.ValueBinding != null && column.UseBindingValue)
                return this.GetBindingExpressionFunc();

            var expFunc = base.GetExpressionFunc(propertyName);
            if (expFunc != null)
                return expFunc;

#if !WP
            if (this.IsDynamicBound)
            {
                return this.GetDynamicExpressionFunc();
            }
#endif
            return null;
        }

#if !WP
        private Func<string, object, object> dynamicFuncCache = null;
        private Func<string, object, object> GetDynamicFunc()
        {
            if (this.dynamicFuncCache == null)
            {
                this.dynamicFuncCache = (propertyName, record) =>
                {
                    var dynamicProvider = this.GetPropertyAccessProvider() as DynamicPropertiesProvider;
                    return dynamicProvider != null ? dynamicProvider.GetValue(record, propertyName) : null;
                };
            }
            return dynamicFuncCache;
        }

        private System.Linq.Expressions.Expression<Func<string, object, object>> GetDynamicExpressionFunc()
        {
            if (this.dynamicFuncCache == null)
            {
                this.GetDynamicFunc();
            }
            return (propertyName, record) => this.dynamicFuncCache(propertyName, record);
        }

        public override Func<string, object, object> GetDisplayValueFunc(string propertyName)
        {
            var column = this.datagrid.Columns.FirstOrDefault(col => col.MappingName == propertyName);

            if (column != null && column.IsUnbound && column is GridUnBoundColumn)
            {
                var unboundcolumn = column as GridUnBoundColumn;
                unboundcolumn.UnBoundFunc = this.GetUnboundFunc(propertyName);
                if (unboundcolumn.UnBoundFunc != null)
                    return (columnName, record) => unboundcolumn.UnBoundFunc(columnName, record);
                return null;
            }
            return base.GetDisplayValueFunc(propertyName);
        }

        public override Expression<Func<string, object, object>> GetDisplayValueExpressionFunc(string propertyName)
        {
            var func = this.GetDisplayValueFunc(propertyName);
            return (columnName, record) => func(propertyName, record);
        }
#endif

        private Func<string, object, object> bindingFuncCache = null;
        private Func<string, object, object> GetBindingFunc()
        {
            if (this.bindingFuncCache == null)
            {
                this.bindingFuncCache = (propertyName, record) =>
                {
                    var provider = this.GetPropertyAccessProvider();
                    return provider.GetValue(record, propertyName);
                };
            }
            return bindingFuncCache;
        }

        private System.Linq.Expressions.Expression<Func<string, object, object>> GetBindingExpressionFunc()
        {
            if (this.bindingFuncCache == null)
            {
                this.GetBindingFunc();
            }
            return (propertyName, record) => this.bindingFuncCache(propertyName, record);
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Gets UnBound Func Value for sorting and grouping
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal Func<string, object, object> GetUnboundFunc(string propertyName)
        {
            var column = this.datagrid.Columns.FirstOrDefault(col => col.MappingName == propertyName);
            if (column != null && column.IsUnbound)
            {
                var col = column as GridUnBoundColumn;
                col.UnBoundFunc = (columnName, record) =>
                {
//#if !SILVERLIGHT && !WP
//                    if (col.CellTemplate != null || col.CellTemplateSelector != null || col.EditTemplate != null || col.EditTemplateSelector != null)
//#elif  !WP
//                    if (col.CellTemplate != null || col.EditTemplate != null)
//#else
//                    if(col.CellTemplate != null)
//#endif
//                        return null;

                    if (col.Format != string.Empty)
                    {
                        var result = col.Format.FormatByName(null, (key) =>
                        {
                            var itemProperties = this.datagrid.View.GetItemProperties();
                            var pd = itemProperties.GetPropertyDescriptor(key);
                            return pd != null ? pd.GetValue(record) : null;
                        });
                        result = result.ToString().Substring(1, result.ToString().Length - 2);
                        if (result != null)
                        {
                            var handledResult = this.datagrid.GetUnBoundCellValue(column, record);
                            if (result.GetType() != handledResult.GetType())
                                return ConvertToType(handledResult, result.GetType());
                            return handledResult;
                        }
                        return string.Empty;
                    }
                    else if (col.Expression != string.Empty)
                    {
                        var result = col.ComputedValue(record);
                        if (result != null)
                        {
                            var handledResult = this.datagrid.GetUnBoundCellValue(column, record);
                            if (result.GetType() != handledResult.GetType())
                                return ConvertToType(handledResult, result.GetType());
                            return handledResult;
                        }
                    }
                    return string.Empty;
                };
                return col.UnBoundFunc ?? null;
            }
            return null;
        }

        #endregion

        protected override IPropertyAccessProvider CreateItemPropertiesProvider()
        {
#if !WP
            if (this.IsDynamicBound)
            {
                return new GridDynamicPropertiesProvider(this, datagrid);
            }
#endif
            return new GridItemPropertiesProvider(this, datagrid);
        }

        protected override TopLevelGroup CreateTopLevelGroup()
        {
            var topLevelgroup = new GridDataTopLevelGroup(this.datagrid, this);
            return topLevelgroup;
        }

        protected override void RemoveRecord(object record)
        {
            base.RemoveRecord(record);
            this.datagrid.SelectionController.HandleCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, record, 0), CollectionChangedReason.DataReorder);
        }
    }

#if WPF
    public class GridDataTableCollectionViewWrapper : DataTableCollectionView
    {
        SfDataGrid datagrid;

        #region ctor
        public GridDataTableCollectionViewWrapper(IEnumerable source, SfDataGrid grid)
            : base(source)
        {
            datagrid = grid;
        }
        #endregion

        protected override TopLevelGroup CreateTopLevelGroup()
        {
            var topLevelgroup = new GridDataTopLevelGroup(this.datagrid, this);
            return topLevelgroup;
        }
    }
#endif

#if !WP
    public class GridPagedCollectionViewWrapper : PagedCollectionView
    {
        #region Fields
        SfDataGrid dataGrid;
        new IPropertyAccessProvider propertyAccessProvider;
        #endregion

        #region Ctor

        public GridPagedCollectionViewWrapper()
            : base()
        {

        }

        public GridPagedCollectionViewWrapper(IEnumerable sender)
            : base(sender)
        {
        }

        #endregion

        #region private methods
        private object ConvertToType(object value, Type type)
        {
#if WinRT
            var method = type.GetTypeInfo().DeclaredMethods.Where(x => x.Name.Equals("TryParse"));
#else
            var method = type.GetMethods().Where(x => x.Name.Equals("TryParse"));
#endif
            if (method.Any())
            {
                var methodinfo = method.FirstOrDefault();
                object[] args = { value.ToString(), null };
                if ((bool)methodinfo.Invoke(null, args))
                    return args[1];
                return null;
            }
            return null;
        }
        #endregion

        #region Override Functions

        public override Func<string, object, object> GetFunc(string propertyName)
        {
            var column = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == propertyName);

            if (column != null && column.IsUnbound && column is GridUnBoundColumn)
            {
                var unboundcolumn = column as GridUnBoundColumn;
                unboundcolumn.UnBoundFunc = this.GetUnboundFunc(propertyName);
                if (unboundcolumn.UnBoundFunc != null)
                    return (columnName, record) => unboundcolumn.UnBoundFunc(columnName, record);
                return null;
            }

            if (column != null && column.ValueBinding != null && column.UseBindingValue)
                return this.GetBindingFunc();

            var func = base.GetFunc(propertyName);
            if (func != null)
                return func;

#if !WP
            if (this.IsDynamicBound)
            {
                return this.GetDynamicFunc();
            }
#endif
            return null;
        }

        /// <summary>
        /// Returns Expression Func for UnBoundColumns for Sorting and Grouping.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override System.Linq.Expressions.Expression<Func<string, object, object>> GetExpressionFunc(string propertyName)
        {
            var column = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == propertyName);

            if (column != null && column.IsUnbound && column is GridUnBoundColumn)
            {
                var unboundcolumn = column as GridUnBoundColumn;
                unboundcolumn.UnBoundFunc = this.GetUnboundFunc(propertyName);
                if (unboundcolumn.UnBoundFunc != null)
                    return (columnName, record) => unboundcolumn.UnBoundFunc(columnName, record);
                return null;
            }

            if (column != null && column.ValueBinding != null && column.UseBindingValue)
                return this.GetBindingExpressionFunc();

            var expFunc = base.GetExpressionFunc(propertyName);
            if (expFunc != null)
                return expFunc;

#if !WP
            if (this.IsDynamicBound)
            {
                return this.GetDynamicExpressionFunc();
            }
#endif
            return null;
        }

#if !WP
        private Func<string, object, object> dynamicFuncCache = null;
        private Func<string, object, object> GetDynamicFunc()
        {
            if (this.dynamicFuncCache == null)
            {
                this.dynamicFuncCache = (propertyName, record) =>
                {
                    var dynamicProvider = this.GetPropertyAccessProvider() as DynamicPropertiesProvider;
                    return dynamicProvider != null ? dynamicProvider.GetValue(record, propertyName) : null;
                };
            }
            return dynamicFuncCache;
        }

        private System.Linq.Expressions.Expression<Func<string, object, object>> GetDynamicExpressionFunc()
        {
            if (this.dynamicFuncCache == null)
            {
                this.GetDynamicFunc();
            }
            return (propertyName, record) => this.dynamicFuncCache(propertyName, record);
        }
#endif

        private Func<string, object, object> bindingFuncCache = null;
        private Func<string, object, object> GetBindingFunc()
        {
            if (this.bindingFuncCache == null)
            {
                this.bindingFuncCache = (propertyName, record) =>
                {
                    var provider = this.GetPropertyAccessProvider();
                    return provider.GetValue(record, propertyName);
                };
            }
            return bindingFuncCache;
        }

        private System.Linq.Expressions.Expression<Func<string, object, object>> GetBindingExpressionFunc()
        {
            if (this.bindingFuncCache == null)
            {
                this.GetBindingFunc();
            }
            return (propertyName, record) => this.bindingFuncCache(propertyName, record);
        }

        public override Func<string, object, object> GetDisplayValueFunc(string propertyName)
        {
            var column = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == propertyName);

            if (column != null && column.IsUnbound && column is GridUnBoundColumn)
            {
                var unboundcolumn = column as GridUnBoundColumn;
                unboundcolumn.UnBoundFunc = this.GetUnboundFunc(propertyName);
                if (unboundcolumn.UnBoundFunc != null)
                    return (columnName, record) => unboundcolumn.UnBoundFunc(columnName, record);
                return null;
            }
            return base.GetDisplayValueFunc(propertyName);
        }

        public override Expression<Func<string, object, object>> GetDisplayValueExpressionFunc(string propertyName)
        {
            var func = this.GetDisplayValueFunc(propertyName);
            return (columnName, record) => func(propertyName, record);
        }

        /// <summary>
        /// To handle the Excel Like Check box filtering.
        /// </summary>
        public override void RefreshFilter()
        {
            var filterPresent = this.FilterPredicates.Any(v => v.FilterPredicates != null && v.FilterPredicates.Count > 0);
            if (filterPresent)
            {
                var source = this.SourceCollection.AsQueryable();
                ParameterExpression paramExpression;
                System.Linq.Expressions.Expression predicate = this.GetPredicateExpression(source, out paramExpression);
                if (paramExpression != null && predicate != null)
                {
                    var lambda = System.Linq.Expressions.Expression.Lambda(predicate, paramExpression);
                    var delg = lambda.Compile();
                    this.RowFilter = (o) =>
                    {
                        var result = (bool)delg.DynamicInvoke(o);
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

            this.UpdateItems();
            if (this.IsGrouping && !UseOnDemandPaging && !IsInEndeferal)
            {
                this.RefreshTopLevelGroup();

                var hasSort = this.SortDescriptions.Count > 0;
                if (hasSort)
                {
                    this.RefreshSort();
                }
            }
            if (!this.IsInEndeferal)
                this.Refresh();
            this.OnPropertyChanged("ItemsCount");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets UnBound Func Value for sorting and grouping
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal Func<string, object, object> GetUnboundFunc(string propertyName)
        {
            var column = this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == propertyName);
            if (column != null && column.IsUnbound)
            {
                var col = column as GridUnBoundColumn;
                col.UnBoundFunc = (columnName, record) =>
                {
#if !SILVERLIGHT && !WP
                    if (col.CellTemplate != null || col.CellTemplateSelector != null || col.EditTemplate != null || col.EditTemplateSelector != null)
#elif  !WP
                    if (col.CellTemplate != null || col.EditTemplate != null)
#else
                    if(col.CellTemplate != null)
#endif
                        return null;

                    if (col.Format != string.Empty)
                    {
                        var result = col.Format.FormatByName(null, (key) =>
                        {
                            var itemProperties = this.dataGrid.View.GetItemProperties();
                            var pd = itemProperties.GetPropertyDescriptor(key);
                            return pd != null ? pd.GetValue(record) : null;
                        });
                        result = result.ToString().Substring(1, result.ToString().Length - 2);
                        if (result != null)
                        {
                            var handledResult = this.dataGrid.GetUnBoundCellValue(column, record);
                            if (result.GetType() != handledResult.GetType())
                                return ConvertToType(handledResult, result.GetType());
                            return handledResult;
                        }
                        return string.Empty;
                    }
                    else if (col.Expression != string.Empty)
                    {
                        var result = col.ComputedValue(record);
                        if (result != null)
                        {
                            var handledResult = this.dataGrid.GetUnBoundCellValue(column, record);
                            if (result.GetType() != handledResult.GetType())
                                return ConvertToType(handledResult, result.GetType());
                            return handledResult;
                        }
                    }
                    return string.Empty;
                };
                return col.UnBoundFunc ?? null;
            }
            return null;
        }

        #endregion

        protected override IPropertyAccessProvider CreateItemPropertiesProvider()
        {
#if !WP
            if (this.IsDynamicBound)
            {
                propertyAccessProvider = new GridDynamicPropertiesProvider(this, dataGrid);
                return propertyAccessProvider;
            }
#endif
            propertyAccessProvider = new GridItemPropertiesProvider(this, dataGrid);
            return propertyAccessProvider;
        }


        protected override TopLevelGroup CreateTopLevelGroup()
        {
            return new GridDataTopLevelGroup(dataGrid, this);
        }

        protected override void RemoveRecord(object record)
        {
            if (this.dataGrid != null)
            {
                this.dataGrid.SelectionController.HandleCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, record, 0), CollectionChangedReason.DataReorder);
            }
        }

        #region Public Method

        public void SetDataGrid(SfDataGrid grid)
        {
            this.dataGrid = grid;
            if (propertyAccessProvider is GridItemPropertiesProvider)
                (propertyAccessProvider as GridItemPropertiesProvider).SetDataGrid(grid);
#if !WP
            else if (propertyAccessProvider is GridDynamicPropertiesProvider)
                (propertyAccessProvider as GridDynamicPropertiesProvider).SetDataGrid(grid);
#endif
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            this.dataGrid = null;
        }

    }
#endif

}
