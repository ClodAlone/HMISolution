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
using System.Web;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class SortSettingsOptionsBuilder
    {
        private SortSettingsOptions sortSettingsOptions = new SortSettingsOptions();
        public SortSettingsOptionsBuilder(SortSettingsOptions sort)
        {
            sortSettingsOptions = sort;
        }
        public SortSettingsOptionsBuilder SortedColumns(Action<GanttSortedColumnBuilder> sortedColumns)
        {
            var builder = new GanttSortedColumnBuilder(sortSettingsOptions);
            if (builder != null)
                sortedColumns.Invoke(builder);
            return this;
        }
        public SortSettingsOptionsBuilder SortedColumns(List<GanttSortedColumn> sortedColumns)
        {
            sortSettingsOptions.SortedColumn = sortedColumns;
            return this;
        }
    }
}