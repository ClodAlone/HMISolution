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
    public class FilterOptionsBuilder<T> where T : class
    {
       
        private FilterOptions<T> filterOption=new FilterOptions<T>();
        public FilterOptionsBuilder(FilterOptions<T> filter)
        {
            filterOption = filter;
        }
        public FilterOptionsBuilder<T> FilterType(FilterType filterType)
        {
            filterOption.FilterType =filterType;
            return this;
        }
        public FilterOptionsBuilder<T> FilterBarMode(FilterBarMode filterMode)
        {
            filterOption.FilterBarMode = filterMode;
            return this;
        }
        public FilterOptionsBuilder<T> StatusBarWidth(int width)
        {
            filterOption.StatusBarWidth = width;
            return this;
        }
        public FilterOptionsBuilder<T> ShowPredicate()
        {
            filterOption.ShowPredicate = true;
            return this;
        }
        public FilterOptionsBuilder<T> ShowPredicate(bool showPredicate)
        {
            filterOption.ShowPredicate = showPredicate;
            return this;
        }
        public FilterOptionsBuilder<T> ShowFilterBarMessage()
        {
            filterOption.ShowPredicate = true;
            return this;
        }
        public FilterOptionsBuilder<T> ShowFilterBarMessage(bool showFilterBarMessage)
        {
            filterOption.ShowFilterBarMessage = showFilterBarMessage;
            return this;
        }
    }
}
