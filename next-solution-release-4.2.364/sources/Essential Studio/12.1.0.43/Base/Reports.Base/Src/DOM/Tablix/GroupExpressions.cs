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
using System.Xml.Serialization;


namespace Syncfusion.RDL.DOM
{
    public class GroupExpressions:List<GroupExpression>
    {
    }

    public class GroupExpression
    {
        [XmlText()]
        public string Value{get;set;}

        public object Clone()
        {
            GroupExpression groupExpression = new GroupExpression();
            groupExpression.Value = this.Value;
            return groupExpression;
        }
    }
}
