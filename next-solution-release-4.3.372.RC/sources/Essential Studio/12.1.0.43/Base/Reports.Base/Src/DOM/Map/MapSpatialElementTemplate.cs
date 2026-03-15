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

namespace Syncfusion.RDL.DOM
{
    public abstract class MapSpatialElementTemplate
    {
        public bool Hidden { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public Style Style { get; set; }
        public string Label { get; set; }
        public string ToolTip { get; set; }
        public ActionInfo ActionInfo { get; set; }
        public string DataElementName { get; set; }
        public DataElementOutputs DataElementOutput { get; set; }
        public string DataElementLabel { get; set; }
    }
}
