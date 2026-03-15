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
    public class ScrollOptionsBuilder<T> where T : class
    {
        
        private ScrollOptions<T> scrollOptions=new ScrollOptions<T>();
        public ScrollOptionsBuilder(ScrollOptions<T> scroll)
        {
            scrollOptions = scroll;
        }
        public ScrollOptionsBuilder<T> Width(int width)
        {
            scrollOptions.Width = width;
            return this;
        }
        public ScrollOptionsBuilder<T> FrozenColumns(int ColumnCount)
        {
            scrollOptions.FrozenColumns = ColumnCount;
            return this;
        }
        public ScrollOptionsBuilder<T> FrozenRows(int RowCount)
        {
            scrollOptions.FrozenRows = RowCount;
            return this;
        }
        public ScrollOptionsBuilder<T> Height(int height)
        {
            scrollOptions.Height = height;
            return this;
        }
        public ScrollOptionsBuilder<T> AllowVirtualScrolling()
        {
            scrollOptions.AllowVirtualScrolling = true;
            return this;
        }
        public ScrollOptionsBuilder<T> AllowVirtualScrolling(bool allowVirtualScrolling)
        {
            scrollOptions.AllowVirtualScrolling = allowVirtualScrolling;
            return this;
        }
    }
}
