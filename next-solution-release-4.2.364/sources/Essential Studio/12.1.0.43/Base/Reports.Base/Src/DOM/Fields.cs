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
    public class Fields :List<Field>
    {
    } 

    public class Field
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public string DataField { get; set; }
        public string Value { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
        public string TypeName { get; set; }

        public object Clone()
        {
            Field field = new Field();
            field.Name = this.Name;
            field.TypeName = this.TypeName;
            field.Value = this.Value;
            field.DataField = this.DataField;
            return field;
        }
    }
}
