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
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Shared;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class SortedColumnBuilder<T> where T : class
    {
        private SortedColumn<T> sortedColumns;
        public SortOptions<T> sortOptions = new SortOptions<T>();
        public SortedColumnBuilder(SortOptions<T> sortOptions)
        {
            this.sortOptions = sortOptions;
            sortedColumns = new SortedColumn<T>();
        }
        public SortedColumnBuilder<T> Field(String field)
        {
            sortedColumns.Field = field;
            return this;
        }
        public SortedColumnBuilder<T> Direction(SortOrder direction)
        {
            sortedColumns.Direction = (Direction)direction;
            return this;
        }
        
        public void Add()
        {
            this.sortOptions.SortedColumn.Add(sortedColumns);
            sortedColumns = new SortedColumn<T>();
            //return this; 
        }
    }
}
