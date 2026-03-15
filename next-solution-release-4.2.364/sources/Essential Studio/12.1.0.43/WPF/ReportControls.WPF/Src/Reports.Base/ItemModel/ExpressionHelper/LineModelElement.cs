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
using Syncfusion.RDL.DOM;

namespace Syncfusion.RDL.ItemModel
{
    internal class LinePropertiesExpVal
    {
        public string BookMark { get; set; }
        public string DocumentMapLabel { get; set; }
        public int ZIndex { get; set; }
        public string LineColor { get; set; }
        public LineStyle LineStyle { get; set; }
        public double LineWidth { get; set; }
        public bool Hidden { get; set; }
    }
}
