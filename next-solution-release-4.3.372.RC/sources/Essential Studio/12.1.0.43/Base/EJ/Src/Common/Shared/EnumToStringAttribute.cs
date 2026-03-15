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
using System.Reflection;

namespace Syncfusion.JavaScript
{
    public class EnumToStringAttribute : Attribute
    {
        private String propertyname;
        public EnumToStringAttribute(String name)
        {
            this.propertyname = name;
        }
        public String PropertyName
        {
            get { return this.propertyname; }
            set { this.propertyname = value; }
        }
    }
    public static class EnumToString
    {
        public static string StringValue(Enum enumvalue)
        {
            String finalvalue = null;
            Type EnumType = enumvalue.GetType();
            FieldInfo fieldinfo = EnumType.GetField(enumvalue.ToString());
            EnumToStringAttribute[] attrs =
           fieldinfo.GetCustomAttributes(typeof(EnumToStringAttribute),
                                   false) as EnumToStringAttribute[];
            if (attrs.Length > 0)
            {
                finalvalue = attrs[0].PropertyName;
            }
            return finalvalue;

        }
    }
}
