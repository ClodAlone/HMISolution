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
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
    public class CellContents
    {
        [XmlElement("Tablix", typeof(Tablix))]
        [XmlElement("Line", typeof(Line))]
        [XmlElement("Textbox", typeof(TextBox))]
        [XmlElement("Chart", typeof(Chart))]
        [XmlElement("Rectangle", typeof(Rectangle))]
        [XmlElement("Image", typeof(Image))]
        [XmlElement("Map", typeof(Map))]
        [XmlElement("Subreport", typeof(SubReport))]
        [XmlElement("GaugePanel", typeof(GaugePanel))]
        public ReportItem ReportItem { get; set; }

        [DefaultValue(0)]
        public int ColSpan { get; set; }

        [DefaultValue(0)]
        public int RowSpan { get; set; }

        public object Clone()
        {
            CellContents cellContents = new CellContents();
            cellContents.ColSpan = this.ColSpan;
            cellContents.RowSpan = this.RowSpan;
            cellContents.ReportItem = this.ReportItem;
            return cellContents;
        }
    }
}
