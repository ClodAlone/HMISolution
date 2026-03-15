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
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class FilterValues : List<FilterValue>
    {
    }

    public class FilterValue
    {
        [XmlText()]
        public string Value { get; set; }
        
        [XmlAttribute("DataType")]
        public DataTypes DataType { get; set; }

        public object Clone()
        {
            FilterValue filterValue = new FilterValue();
            filterValue.DataType = this.DataType;
            filterValue.Value = this.Value;
            return filterValue;
        }
    }
}
