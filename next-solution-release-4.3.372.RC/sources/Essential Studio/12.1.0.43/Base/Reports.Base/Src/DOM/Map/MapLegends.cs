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
    public class MapLegends : List<MapLegend>
    {

    }

    public class MapLegend : MapDockableSubItem
    {
        [XmlAttribute()]
        public string Name { get; set; }
        public Layout Layout { get; set; }
        public MapLegendTitle MapLegendTitle { get; set; }
        public bool AutoFitTextDisabled { get; set; }
        public Size MinFontSize { get; set; }
        public bool InterlacedRows { get; set; }
        public string InterlacedRowsColor { get; set; }
        public bool EquallySpacedItems { get; set; }
        public int TextWrapThreshold { get; set; }
    }
}
