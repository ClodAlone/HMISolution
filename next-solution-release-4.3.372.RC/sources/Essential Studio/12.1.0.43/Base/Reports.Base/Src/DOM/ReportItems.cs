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
using System.ComponentModel;
namespace Syncfusion.RDL.DOM
{

#if !SILVERLIGHT
    [Serializable]
#endif
    public class ClipboardData
    {
        internal ClipboardData()
        {   
        }

        public string Data { get; set; }
    }

#if !SILVERLIGHT
    [Serializable]
#endif

    [XmlInclude(typeof(TextBox))]
    [XmlInclude(typeof(Tablix))]
    [XmlInclude(typeof(GaugePanel))]
    [XmlInclude(typeof(Rectangle))]
    [XmlInclude(typeof(SubReport))]
    [XmlInclude(typeof(Line))]
    [XmlInclude(typeof(SubReport))]
    [XmlInclude(typeof(Chart))]
    [XmlInclude(typeof(Map))]
    [XmlInclude(typeof(SubReport))]
    public class ReportItems : List<ReportItem>
    {
    }

    public abstract class ReportItem
    {
        private Size m_left = "0in";
        private Size m_top = "0in";

        public Size Left
        {
            get { return m_left; }
            set { m_left = value; }
        }

        public Size Top
        {
            get { return m_top; }
            set { m_top = value; }
        }

        [XmlAttribute()]
        public string Name { get; set; }
        public ActionInfo ActionInfo { get; set; }
        public Size Height { get; set; }
        public Size Width { get; set; }
        public int ZIndex { get; set; }
        public Visibility Visibility { get; set; }
        public string ToolTip { get; set; }
        public string DocumentMapLabel { get; set; }
        public string Bookmark { get; set; }
        public string RepeatWith { get; set; }
        public CustomProperties CustomProperties { get; set; }
        public string DataElementName { get; set; }

        [DefaultValue(DataElementOutputs.Auto)]
        public DataElementOutputs DataElementOutput { get; set; }
        public Style Style { get; set; }

        public bool ShouldSerializeZIndex()
        {
            return this.ZIndex > 0;
        }

        public void ResetZIndex()
        {
            this.ZIndex = 0;
        }

        public bool ShouldSerializeCustomProperties()
        {
            return CustomProperties != null && CustomProperties.Count > 0;
        }

        public void ResetCustomProperties()
        {
            this.CustomProperties = new CustomProperties();
        }

        public bool ShouldSerializeDataElementOutput()
        {
            return DataElementOutput != DataElementOutputs.Auto;
        }

        public void ResetDataElementOutput()
        {
            this.DataElementOutput = DataElementOutputs.Auto;
        }
    }
}
