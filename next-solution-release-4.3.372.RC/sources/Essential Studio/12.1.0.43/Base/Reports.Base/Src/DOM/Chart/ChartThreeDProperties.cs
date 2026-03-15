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
    public class ChartThreeDProperties
    {
        public bool Enabled { get; set; }
        public ProjectionMode ProjectionMode { get; set; }
        public int Perspective { get; set; }
        public int Rotation { get; set; }
        public int Inclination { get; set; }
        public int DepthRatio { get; set; }
        public Shading Shading { get; set; }
        public int GapDepth { get; set; }
        public int WallThickness { get; set; }
        public bool Clustered { get; set; }
    }
}
