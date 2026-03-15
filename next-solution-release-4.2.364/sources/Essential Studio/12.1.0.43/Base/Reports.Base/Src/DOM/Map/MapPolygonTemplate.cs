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
    public class MapPolygonTemplate : MapSpatialElementTemplate
    {
        public float ScaleFactor { get; set; }
        public float CenterPointOffsetX { get; set; }
        public float CenterPointOffsetY { get; set; }
        public BooleanOptions ShowLabel { get; set; }
        public LabelPlacement LabelPlacement { get; set; }
    }
}
