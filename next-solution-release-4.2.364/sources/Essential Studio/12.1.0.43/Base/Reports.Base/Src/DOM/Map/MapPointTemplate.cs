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
    public abstract class MapPointTemplate : MapSpatialElementTemplate
    {
        public Size Size { get; set; }
        public LabelPlacement LabelPlacement { get; set; }
    }

    public class MapCenterPointTemplate : MapPointTemplate
    {
    }

    public class MapMarkerTemplate : MapPointTemplate
    {
        public MapMarker MapMarker { get; set; }
    }
}
