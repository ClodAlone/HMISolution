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
namespace Syncfusion.JavaScript
{
    public class CustomAttributeBuilder<T> where T:class
    {
        Column<T> column = new Column<T>();
        public CustomAttributeBuilder(Column<T> column)
        {
            this.column = column;
        }
        public CustomAttributeBuilder<T> AddAttribute(String key, object value)
        {
            if (!this.column.CustomAttributes.ContainsKey(key))
                this.column.CustomAttributes.Add(key, value);
            return this;
        }
    }
}
