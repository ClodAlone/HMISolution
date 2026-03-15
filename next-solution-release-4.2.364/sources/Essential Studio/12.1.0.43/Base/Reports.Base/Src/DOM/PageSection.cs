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
    public abstract class PageSection : ReportElement
    {
        public Size Height { get; set; }
        public bool PrintOnFirstPage { get; set; }
        public bool PrintOnLastPage { get; set; }

        [XmlArrayItem("Tablix", typeof(Tablix))]
        [XmlArrayItem("Line", typeof(Line))]
        [XmlArrayItem("Textbox", typeof(TextBox))]
        [XmlArrayItem("Chart", typeof(Chart))]
        [XmlArrayItem("Rectangle", typeof(Rectangle))]
        [XmlArrayItem("Image", typeof(Image))]
        [XmlArrayItem("Subreport", typeof(SubReport))]
        public ReportItems ReportItems { get; set; }

        public bool ShouldSerializeReportItems()
        {
            return ReportItems != null && ReportItems.Count > 0;
        }

        public void ResetReportItems()
        {
            this.ReportItems = new ReportItems();
        }

        public bool ShouldSerializePrintOnFirstPage()
        {
            return this.PrintOnFirstPage != false;
        }

        public void ResetPrintOnFirstPage()
        {
            this.PrintOnFirstPage = false;
        }

        public bool ShouldSerializePrintOnLastPage()
        {
            return this.PrintOnLastPage != false;
        }

        public void ResetPrintOnLastPage()
        {
            this.PrintOnLastPage = false;
        }
    }

    public class PageHeader : PageSection
    {

    }

    public class PageFooter : PageSection
    {

    }
}
