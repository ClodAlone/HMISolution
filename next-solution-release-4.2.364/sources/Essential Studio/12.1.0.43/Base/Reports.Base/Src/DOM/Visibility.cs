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
    public class Visibility
    {
        public string Hidden { get; set; }
        public string ToggleItem { get; set; }

        public object Clone()
        {
            Visibility visibility = new Visibility();
            visibility.Hidden = this.Hidden;
            visibility.ToggleItem = this.ToggleItem;
            return visibility;
        }
    }
}
