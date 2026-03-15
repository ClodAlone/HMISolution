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
using Syncfusion.JavaScript.Models;
namespace Syncfusion.JavaScript
{
    public class SortOptionsBuilder<T> where T : class
    {
      
        private SortOptions<T> sortOptions = new SortOptions<T>();
        public SortOptionsBuilder(SortOptions<T> sort)
        {
            sortOptions = sort;
        }
        public SortOptionsBuilder<T> SortedColumns(Action<SortedColumnBuilder<T>> sortedColumns)
        {
            var builder = new SortedColumnBuilder<T>(sortOptions);
            if (builder != null)
                sortedColumns.Invoke(builder);
            return this;
        }
        public SortOptionsBuilder<T> SortedColumns(List<SortedColumn<T>> sortedColumns)
        {
            sortOptions.SortedColumn = sortedColumns;
            return this;
        }
    }
}
