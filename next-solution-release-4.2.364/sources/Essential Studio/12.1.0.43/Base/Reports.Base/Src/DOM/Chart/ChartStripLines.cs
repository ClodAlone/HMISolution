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
    public class ChartStripLines : List<ChartStripLine>
    {

    }

    public class ChartStripLine
    {
        public Style Style { get; set; }
        public string Title { get; set; }
        public TextOrientation TextOrientation { get; set; }
        public ActionInfo ActionInfo { get; set; }
        public string ToolTip { get; set; }
        public float Interval { get; set; }
        public IntervalType IntervalType { get; set; }
        public float IntervalOffset { get; set; }
        public IntervalType IntervalOffsetType { get; set; }
        public float StripWidth { get; set; }
        public IntervalType StripWidthType { get; set; }
    }
}
