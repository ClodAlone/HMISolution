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
    public class ParameterValues : List<ParameterValue>
    {
    }

    public class ParameterValue
    {
        public string Value { get; set; }
        public string Label { get; set; }

        public object Clone()
        {
            ParameterValue parameterValue = new ParameterValue();
            parameterValue.Label = this.Label;
            parameterValue.Value =this.Value;
            return parameterValue;
        }
    }
}
