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
    public class ChartAreas : List<ChartArea>
    {

    }

    public class ChartArea
    {
        [XmlAttribute()]
        public string Name { get;set; }
        public bool Hidden { get; set; }
        public ChartCategoryAxes ChartCategoryAxes { get; set; }
        public ChartValueAxes ChartValueAxes { get; set; }
        public ChartThreeDProperties ChartThreeDProperties { get; set; }
        public Style Style { get; set; }
        public AlignOrientation AlignOrientation { get; set; }
        public ChartAlignType ChartAlignType { get; set; }
        public string AlignWithChartArea { get; set; }
        public ChartElementPosition ChartElementPosition { get; set; }
        public ChartInnerPlotPosition ChartInnerPlotPosition { get; set; }
        public bool EquallySizedAxesFont { get; set; }

        public bool ShouldSerializeAlignOrientation()
        {
            return this.AlignOrientation != DOM.AlignOrientation.None;
        }

        public void ResetAlignOrientation()
        {
            this.AlignOrientation = DOM.AlignOrientation.None;
        }


        public bool ShouldSerializeEquallySizedAxesFont()
        {
            return this.EquallySizedAxesFont != false;
        }

        public void ResetEquallySizedAxesFont()
        {
            this.EquallySizedAxesFont = false;
        }

        public bool ShouldSerializeHidden()
        {
            return this.Hidden != false;
        }

        public void ResetHidden()
        {
            this.Hidden = false;
        }
    }
}
