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
    public class ChartLegends : List<ChartLegend>
    {
    }

    public class ChartLegend
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public bool Hidden { get; set; }
        public Style Style { get; set; }
        public Positions Position { get; set; }
        [DefaultValue(Layout.AutoTable)]
        public Layout Layout { get; set; }
        public string DockToChartArea { get; set; }
        [DefaultValue(false)]
        public bool DockOutsideChartArea { get; set; }
        public ChartElementPosition ChartElementPosition { get; set; }
        public ChartLegendTitle ChartLegendTitle { get; set; }
        [DefaultValue(false)]
        public bool AutoFitTextDisabled { get; set; }
        public Size MinFontSize { get; set; }
        public Separator HeaderSeparator { get; set; }
        public string HeaderSeparatorColor { get; set; }
        public Separator ColumnSeparator { get; set; }
        public string ColumnSeparatorColor { get; set; }
        public int ColumnSpacing { get; set; }
        public bool InterlacedRows { get; set; }
        public string InterlacedRowsColor { get; set; }
        public bool EquallySpacedItems { get; set; }
        public BooleanOptions Reversed { get; set; }
        [DefaultValue(0)]
        public int MaxAutoSize { get; set; }
        public int TextWrapThreshold { get; set; }
    }
}
