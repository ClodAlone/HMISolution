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
    public class ChartSmartLabel
    {
        public bool Disabled { get; set; }
        public AllowOutSidePlotArea AllowOutSidePlotArea { get; set; }
        public string CalloutBackColor { get; set; }
        public CalloutLineAnchor CalloutLineAnchor { get; set; }
        public string CalloutLineColor { get; set; }
        public LineStyle CalloutLineStyle { get; set; }
        public Size CalloutLineWidth { get; set; }
        public CalloutStyle CalloutStyle { get; set; }
        public bool ShowOverlapped { get; set; }
        public bool MarkerOverlapping { get; set; }
        public Size MaxMovingDistance { get; set; }
        public Size MinMovingDistance { get; set; }
        public ChartNoMoveDirection ChartNoMoveDirection { get; set; }
    }
}
