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
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
#if !SILVERLIGHT
    [Serializable]
#endif

    public class MapViewport : MapSubItem
    {
        public MapCoordinateSystem MapCoordinateSystem { get; set; }
        public MapProjection MapProjection { get; set; }
        public float ProjectionCenterX { get; set; }
        public float ProjectionCenterY { get; set; }
        public MapLimits MapLimits { get; set; }
        public float MaximumZoom { get; set; }
        public float MinimumZoom { get; set; }

        [XmlElementAttribute("MapDataBoundView", typeof(MapDataBoundView))]
        [XmlElementAttribute("MapCustomView", typeof(MapCustomView))]
        [XmlElementAttribute("MapElementView", typeof(MapElementView))]
        public MapView MapView { get; set; }

        public Size ContentMargin { get; set; }
        public MapMeridians MapMeridians { get; set; }
        public MapParallels MapParallels { get; set; }
        public bool GridUnderContent { get; set; }
        public float SimplificationResolution { get; set; }
    }
}
