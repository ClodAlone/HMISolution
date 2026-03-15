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
    public class QueryParameters : List<QueryParameter>
    {
    }

    public class QueryParameter
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public string Value { get; set; }

        public object Clone()
        {
            QueryParameter queryParameter = new QueryParameter();
            queryParameter.Name = this.Name;
            queryParameter.Value = this.Value;
            return queryParameter;
        }
    }
}
