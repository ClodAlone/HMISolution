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

namespace Syncfusion.OLAP.MDXQueryBuilder
{
    public class ElementConvertor
    {
        public static string DimensionElementToString(DimensionElement dimensionElement)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(dimensionElement.Name);
            sb.Append(":");
            sb.Append(dimensionElement.HierarchyName);
            return sb.ToString();
        }

        public static DimensionElement DimensionElementFromString(string dimensionElementString)
        {
            string[] dimensionDetails = dimensionElementString.Split(':');
            if (dimensionDetails.Length == 2)
            {
                DimensionElement dimensionElement = new DimensionElement();
                dimensionElement.Name = dimensionDetails[0];
                dimensionElement.HierarchyName= dimensionDetails[1];
                return dimensionElement;
            }
            return null;
        }
    }
}
