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
using Syncfusion.JavaScript.Mobile.Models;
namespace Syncfusion.JavaScript
{
    public class MobileSortOptionsBuilder<T> where T : class
    {
      
        private MobileSortOptions<T> sortOptions = new MobileSortOptions<T>();
        public MobileSortOptionsBuilder(MobileSortOptions<T> sort)
        {
            sortOptions = sort;
        }
        public MobileSortOptionsBuilder<T> SortedColumns(Action<MobileSortedColumnBuilder<T>> sortedColumns)
        {
            var builder = new MobileSortedColumnBuilder<T>(sortOptions);
            if (builder != null)
                sortedColumns.Invoke(builder);
            return this;
        }
        public MobileSortOptionsBuilder<T> SortedColumns(List<MobileSortedColumn<T>> sortedColumns)
        {
            sortOptions.SortedColumn = sortedColumns;
            return this;
        }
    }
}
