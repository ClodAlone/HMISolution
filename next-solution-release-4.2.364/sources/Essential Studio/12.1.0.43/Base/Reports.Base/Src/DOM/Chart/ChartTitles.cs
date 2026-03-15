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
    public class ChartTitles : List<ChartTitle>
    {
    }

    public class ChartTitle
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public string Caption { get; set; }
        public bool Hidden { get; set; }
        public Style Style { get; set; }
        public Positions Position { get; set; }
        public string DockToChartArea { get; set; }
        [DefaultValue(false)]
        public bool DockOutChartArea { get; set; }
        public int DockOffset { get; set; }
        public ChartElementPosition ChartElementPosition { get; set; }
        public string ToolTip { get; set; }
        public ActionInfo ActionInfo { get; set; }
        public TextOrientation TextOrientation { get; set; }
    }

    public class ChartNoDataMessage : ChartTitle
    {

    }
}
