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
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript
{
    public class MobileSortedColumnBuilder<T> where T : class
    {
        private MobileSortedColumn<T> sortedColumns;
        public MobileSortOptions<T> sortOptions = new MobileSortOptions<T>();
        public MobileSortedColumnBuilder(MobileSortOptions<T> sortOptions)
        {
            this.sortOptions = sortOptions;
            sortedColumns = new MobileSortedColumn<T>();
        }
        public MobileSortedColumnBuilder<T> Field(String field)
        {
            sortedColumns.Field = field;
            return this;
        }
        public MobileSortedColumnBuilder<T> Direction(SortType direction)
        {
            sortedColumns.Direction = (SortType)direction;
            return this;
        }
        
        public void Add()
        {
            this.sortOptions.SortedColumn.Add(sortedColumns);
            sortedColumns = new MobileSortedColumn<T>();
            //return this; 
        }
    }
}
