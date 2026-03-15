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
    public class ActionInfo 
    {
        public Actions Actions { get; set; }
    }

    public class Actions : List<Action>
    {

    }

    public class Action
    {
        public string Hyperlink { get; set; }
        public Drillthrough Drillthrough { get; set; }
        public string BookmarkLink { get; set; }
    }
}
