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

    [XmlInclude(typeof(MapPolygonLayer))]
    [XmlInclude(typeof(MapPointLayer))]
    [XmlInclude(typeof(MapLineLayer))]
    [XmlInclude(typeof(MapTileLayer))]
    public class MapLayers : List<MapLayer>
    {
    }

    public abstract class MapLayer
    {
        public string Name { get; set; }
        public VisibilityMode VisibilityMode { get; set; }
        public float MinimumZoom { get; set; }
        public float MaximumZoom { get; set; }
        public float Transparency { get; set; }
    }
}
