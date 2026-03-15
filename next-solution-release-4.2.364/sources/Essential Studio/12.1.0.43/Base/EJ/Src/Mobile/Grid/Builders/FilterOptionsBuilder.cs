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
    public class MobileFilterOptionsBuilder<T> where T : class
    {
       
        private MobileFilterOptions<T> filterOption=new MobileFilterOptions<T>();
        public MobileFilterOptionsBuilder(MobileFilterOptions<T> filter)
        {
            filterOption = filter;
        }
        
        public MobileFilterOptionsBuilder<T> FilterBarMode(FilterBarMode filterMode)
        {
            filterOption.FilterBarMode = filterMode;
            return this;
        }

        public MobileFilterOptionsBuilder<T> Interval(int interval)
        {
            filterOption.Interval = interval;
            return this;
        }
       
    }
}
