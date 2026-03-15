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

    public class Map : DataRegion
    {
        [XmlArrayItem("MapPolygonLayer", typeof(MapPolygonLayer))]
        [XmlArrayItem("MapPointLayer", typeof(MapPointLayer))]
        [XmlArrayItem("MapLineLayer", typeof(MapLineLayer))]
        [XmlArrayItem("MapTileLayer", typeof(MapTileLayer))]
        public MapLayers MapLayers { get; set; }

        public MapDataRegions MapDataRegions { get; set; }
        public MapViewport MapViewport { get; set; }
        public MapLegends MapLegends { get; set; }
        public MapTitles MapTitles { get; set; }
        public MapDistanceScale MapDistanceScale { get; set; }
        public MapColorScale MapColorScale { get; set; }
        public MapBorderSkin MapBorderSkin { get; set; }
        public AntiAliasing AntiAliasing { get; set; }
        public TextAntiAliasingQuality TextAntiAliasingQuality { get; set; }
        public float ShadowIntensity { get; set; }
        public int MaximumSpatialElementCount { get; set; }
        public int MaximumTotalPointCount { get; set; }
        public string TileLanguage { get; set; }
    }
}
