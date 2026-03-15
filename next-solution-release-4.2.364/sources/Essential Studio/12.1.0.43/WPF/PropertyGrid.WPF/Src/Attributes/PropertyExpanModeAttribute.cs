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

namespace Syncfusion.Windows.PropertyGrid
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=true)]
    public class PropertyExpandModeAttribute:Attribute
    {
        private string expandMode="FlatMode";

        public string ExpandMode
        {
            get { return expandMode; }
            set { expandMode = value; }
        }


        public PropertyExpandModeAttribute(string ExpandMode)
        {
            this.ExpandMode = ExpandMode;
        }
    }
}
