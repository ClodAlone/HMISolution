#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.JavaScript
{

    public class Sort {
        public string Name { get; set; }
        public string Direction { get; set; }
    }
    public class SearchFilter
    {
        public List<string> Fields { get; set; }
        public string Key { get; set; }
        public string Operator { get; set; }
    }
    public class WhereFilter
    {
        public string Field { get; set; }
        public Boolean IgnoreCase { get; set; }
        public Boolean IsComplex { get; set; }
        public string Operator { get; set; }
        public string Condition { get; set; }
        public object value { get; set; }
        public List<WhereFilter> predicates { get; set; }
    }
    public class CRUDModel
    {
        public string Action { get; set; }
        public string Table { get; set; }
        public string keyColumn { get; set; }
        public object key { get; set; }
    }
    public class DataManager
    {
        [DefaultValue(null)]
        public int Skip { get; set; }
        [DefaultValue(null)]
        public int Take { get; set; }
        [DefaultValue(null)]
        public Boolean RequiresCounts { get; set; }
        public string Table { get; set; }
        public List<string> Group { get; set; }
        public List<string> Select { get; set; }
        public List<string> Expand { get; set; }
        public List<Sort> Sorted { get; set; }
        public List<SearchFilter> Search { get; set; }
        public List<WhereFilter> Where { get; set; }
    } 
}
