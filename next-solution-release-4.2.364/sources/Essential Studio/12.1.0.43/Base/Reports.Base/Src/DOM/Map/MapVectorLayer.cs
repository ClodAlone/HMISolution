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

    public abstract class MapVectorLayer : MapLayer
    {
        public string MapDataRegionName { get; set; }
        public MapBindingFieldPairs MapBindingFieldPairs { get; set; }
        public MapFieldDefinitions MapFieldDefinitions { get; set; }

        [XmlElementAttribute("MapSpatialDataRegion", typeof(MapSpatialDataRegion))]
        [XmlElementAttribute("MapSpatialDataSet", typeof(MapSpatialDataSet))]
        [XmlElementAttribute("MapShapefile", typeof(MapShapefile))]
        public MapSpatialData MapSpatialData { get; set; }

        public string DataElementName { get; set; }
        public DataElementOutputs DataElementOutput { get; set; }
    }
}
