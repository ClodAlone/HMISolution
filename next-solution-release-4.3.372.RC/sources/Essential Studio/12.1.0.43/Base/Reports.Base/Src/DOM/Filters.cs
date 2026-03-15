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
using System.Xml;

namespace Syncfusion.RDL.DOM
{
    public class Filters : List<Filter>
    {
    }

    public class Filter
    {
        public string FilterExpression { get; set; }
        public FilterOperators Operator { get; set; }
        public FilterValues FilterValues { get; set; }

        public object Clone()
        {
            Filter filter = new Filter();
            filter.FilterExpression = this.FilterExpression;
            filter.FilterValues = new FilterValues();
            filter.FilterValues.AddRange(this.FilterValues.Clone());
            filter.Operator = this.Operator;
            return filter;
        }
    }
}
