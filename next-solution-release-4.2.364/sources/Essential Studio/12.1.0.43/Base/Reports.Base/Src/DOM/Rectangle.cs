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
using System.Xml;
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class Rectangle : ReportItem
    {
        [XmlArrayItem("Tablix", typeof(Tablix))]
        [XmlArrayItem("Line", typeof(Line))]
        [XmlArrayItem("Textbox", typeof(TextBox))]
        [XmlArrayItem("Chart", typeof(Chart))]
        [XmlArrayItem("Rectangle", typeof(Rectangle))]
        [XmlArrayItem("Image", typeof(Image))]
        [XmlArrayItem("GaugePanel", typeof(GaugePanel))]
        [XmlArrayItem("Subreport", typeof(SubReport))]
        public ReportItems ReportItems { get; set; }
        public PageBreak PageBreak { get; set; }
        public bool KeepTogether { get; set; }
        public bool OmitBorderOnPageBreak { get; set; }
        public string LinkToChild { get; set; }

        public bool ShouldSerializeReportItems()
        {
            return ReportItems != null && ReportItems.Count > 0;
        }

        public void ResetReportItems()
        {
            this.ReportItems = new ReportItems();
        }

        public bool ShouldSerializeKeepTogether()
        {
            return this.KeepTogether != false;
        }

        public void ResetKeepTogether()
        {
            this.KeepTogether = false;
        }

        public bool ShouldSerializeOmitBorderOnPageBreak()
        {
            return this.OmitBorderOnPageBreak != false;
        }

        public void ResetOmitBorderOnPageBreak()
        {
            this.OmitBorderOnPageBreak = false;
        }
    }
}
