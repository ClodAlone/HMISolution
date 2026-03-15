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
using System.ComponentModel;
using System.Xml.Serialization;
namespace Syncfusion.RDL.DOM
{
    public class Chart : DataRegion
    {
        public ChartSeriesHierarchy ChartSeriesHierarchy { get; set; }
        public ChartCategoryHierarchy ChartCategoryHierarchy { get; set; }
        public ChartData ChartData { get; set; }
        public ChartAreas ChartAreas { get; set; }
        public ChartLegends ChartLegends { get; set; }
        public ChartTitles ChartTitles { get; set; }
        public string Palette { get; set; }
        public ChartCustomPaletteColors ChartCustomPaletteColors { get; set; }
        [DefaultValue(PaletteHatchBehavior.Default)]
        public PaletteHatchBehavior PaletteHatchBehavior { get; set; }
        public Size DynamicHeight { get; set; }
        public Size DynamicWidth { get; set; }
        public ChartBorderSkin ChartBorderSkin { get; set; }
        public ChartNoDataMessage ChartNoDataMessage { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
        public string DesignerMode { get; set; }

        public bool ShouldSerializeChartCustomPaletteColors()
        {
            return ChartCustomPaletteColors != null && ChartCustomPaletteColors.Count > 0;
        }

        public void ResetChartCustomPaletteColors()
        {
            this.ChartCustomPaletteColors = new ChartCustomPaletteColors();
        }

        public bool ShouldSerializeDesignerMode()
        {
            return !string.IsNullOrEmpty(DesignerMode);
        }

        public void ResetDesignerMode()
        {
            this.DesignerMode = null;
        }
    }
        
}
