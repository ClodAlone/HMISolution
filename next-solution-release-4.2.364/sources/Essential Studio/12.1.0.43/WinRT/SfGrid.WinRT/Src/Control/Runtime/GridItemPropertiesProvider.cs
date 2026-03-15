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
    public class GridItemPropertiesProvider : ItemPropertiesProvider
    {
        private SfDataGrid dataGrid;
        public GridItemPropertiesProvider(ICollectionViewAdv view, SfDataGrid _dataGrid)
            : base(view)
        {
            dataGrid = _dataGrid;
        }

        private GridColumn cachedcolumn;

        public override object GetValue(object record, string propName)
        {
            if (dataGrid != null)
            {
                if (cachedcolumn == null || !cachedcolumn.MappingName.Equals(propName))
                    cachedcolumn = dataGrid.Columns.FirstOrDefault(col => col.MappingName == propName);
                if (cachedcolumn != null && cachedcolumn.ValueBinding != null && cachedcolumn.UseBindingValue)
                {
                    cachedcolumn.ColumnWrapper.DataContext = record;
                    return cachedcolumn.ColumnWrapper.Value;
                }
                if (cachedcolumn is GridUnBoundColumn)
                    return dataGrid.GetUnBoundCellValue(cachedcolumn, record);
            }
            return base.GetValue(record, propName);
        }

        public override object GetValue(object record, string propName, bool useBindingValue)
        {
            if (dataGrid != null)
            {
                if (cachedcolumn == null || !cachedcolumn.MappingName.Equals(propName))
                    cachedcolumn = dataGrid.Columns.FirstOrDefault(col => col.MappingName == propName);
                if (cachedcolumn != null && cachedcolumn.ValueBinding != null && useBindingValue)
                {
                    cachedcolumn.ColumnWrapper.DataContext = record;
                    return cachedcolumn.ColumnWrapper.Value;
                }
            }
            return base.GetValue(record, propName, useBindingValue);
        }

        public override bool SetValue(object record, string propName, object value)
        {
            return base.SetValue(record, propName, value);
        }

        public override object GetFormattedValue(object record, string propName)
        {
            if (dataGrid != null)
            {
                if (cachedcolumn == null || !cachedcolumn.MappingName.Equals(propName))
                    cachedcolumn = dataGrid.Columns.FirstOrDefault(col => col.MappingName == propName);

                if (cachedcolumn != null && cachedcolumn.DisplayBinding != null)
                {
                    cachedcolumn.ColumnWrapper.DataContext = record;
                    cachedcolumn.ColumnWrapper.SetDisplayBinding(cachedcolumn.DisplayBinding);
                    return cachedcolumn.ColumnWrapper.FormattedValue;
                }
            }
            return base.GetFormattedValue(record, propName);
        }

        internal void SetDataGrid(SfDataGrid dataGrid)
        {
            this.dataGrid = dataGrid;
        }
    }
#if !WP

    public class GridDynamicPropertiesProvider : DynamicPropertiesProvider
    {
        private SfDataGrid dataGrid;
        public GridDynamicPropertiesProvider(ICollectionViewAdv view, SfDataGrid _dataGrid)
            : base(view)
        {
            dataGrid = _dataGrid;
        }

        private GridColumn cachedcolumn;

        public override object GetValue(object record, string propName)
        {
            if (dataGrid != null)
            {
                if (cachedcolumn == null || !cachedcolumn.MappingName.Equals(propName))
                    cachedcolumn = dataGrid.Columns.FirstOrDefault(col => col.MappingName == propName);

                if (cachedcolumn != null && cachedcolumn.ValueBinding != null && cachedcolumn.UseBindingValue)
                {
                    cachedcolumn.ColumnWrapper.DataContext = record;
                    return cachedcolumn.ColumnWrapper.Value;
                }
                if (cachedcolumn is GridUnBoundColumn)
                    return dataGrid.GetUnBoundCellValue(cachedcolumn, record);
            }
            return base.GetValue(record, propName);
        }

        public override object GetValue(object record, string propName, bool useBindingValue)
        {
            if (dataGrid != null)
            {
                if (cachedcolumn == null || !cachedcolumn.MappingName.Equals(propName))
                    cachedcolumn = dataGrid.Columns.FirstOrDefault(col => col.MappingName == propName);
                if (cachedcolumn != null && cachedcolumn.ValueBinding != null && useBindingValue)
                {
                    cachedcolumn.ColumnWrapper.DataContext = record;
                    return cachedcolumn.ColumnWrapper.Value;
                }
            }
            return base.GetValue(record, propName, useBindingValue);
        }

        public override bool SetValue(object record, string propName, object value)
        {
            return base.SetValue(record, propName, value);
        }

        public override object GetFormattedValue(object record, string propName)
        {
            if (dataGrid != null)
            {
                if (cachedcolumn == null || !cachedcolumn.MappingName.Equals(propName))
                    cachedcolumn = dataGrid.Columns.FirstOrDefault(col => col.MappingName == propName);

                if (cachedcolumn != null && cachedcolumn.DisplayBinding != null)
                {
                    cachedcolumn.ColumnWrapper.DataContext = record;
                    cachedcolumn.ColumnWrapper.SetDisplayBinding(cachedcolumn.DisplayBinding);
                    return cachedcolumn.ColumnWrapper.FormattedValue;
                }
            }
            return base.GetFormattedValue(record, propName);
        }

        internal void SetDataGrid(SfDataGrid dataGrid)
        {
            this.dataGrid = dataGrid;
        }
    }

#endif

}
