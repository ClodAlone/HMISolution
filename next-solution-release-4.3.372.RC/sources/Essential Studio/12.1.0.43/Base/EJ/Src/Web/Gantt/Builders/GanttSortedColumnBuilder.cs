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
    public class GanttSortedColumnBuilder
    {
        private GanttSortedColumn sortedColumns;
        public SortSettingsOptions sortOptions = new SortSettingsOptions();
        public GanttSortedColumnBuilder(SortSettingsOptions sortOptions)
        {
            this.sortOptions = sortOptions;
            sortedColumns = new GanttSortedColumn();
        }
        public GanttSortedColumnBuilder Field(String field)
        {
            sortedColumns.Field = field;
            return this;
        }
        public GanttSortedColumnBuilder Direction(SortOrder direction)
        {
            sortedColumns.Direction = (Direction)direction;
            return this;
        }

        public void Add()
        {
            this.sortOptions.SortedColumn.Add(sortedColumns);
            sortedColumns = new GanttSortedColumn();
            //return this; 
        }
    }
}