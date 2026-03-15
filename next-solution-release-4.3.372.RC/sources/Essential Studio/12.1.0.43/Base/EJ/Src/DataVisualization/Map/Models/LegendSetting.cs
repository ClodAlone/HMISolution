#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.JavaScript.Shared.Serializer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class MapLegendSetting
    {

        #region Fields

        private bool showLegend = false;
        private double legendPositionX = 0;
        private double legendWidth = 20;
        private double legendHeight = 20;
        private double legendPositionY = 0;
        private LegendIcons legendIcon = LegendIcons.Rectangle;
        private string legendType = null;
        private string legendTitle = "";
        private LegendMode legendMode=LegendMode.Default;
        private string legendLeftLabel = "";
        private string legendRightLabel = "";
        private DockPosition legendPosition = DockPosition.BottomRight;

        #endregion

        #region Properties

        [JsonProperty("legendPosition")]
        [DefaultValue(DockPosition.BottomRight)]
        [JsonConverter(typeof(StringEnumConverter))]
        public DockPosition LegendPosition
        {
            get { return this.legendPosition; }
            set { this.legendPosition = value; }
        }

        [JsonProperty("legendMode")]
        [DefaultValue(LegendMode.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public LegendMode LegendMode
        {
            get { return this.legendMode; }
            set { this.legendMode = value; }
        }

        [JsonProperty("legendTitle")]
        [DefaultValue("")]
        public string LegendTitle
        {
            get { return this.legendTitle; }
            set { this.legendTitle = value; }
        }

        [JsonProperty("legendLeftLabel")]
        [DefaultValue("")]
        public string LegendLeftLabel
        {
            get { return this.legendLeftLabel; }
            set { this.legendLeftLabel = value; }
        }

        [JsonProperty("legendRightLabel")]
        [DefaultValue("")]
        public string LegendRightLabel
        {
            get { return this.legendRightLabel; }
            set { this.legendRightLabel = value; }
        }

        [JsonProperty("showLegend")]
        [DefaultValue(false)]
        public bool ShowLegend
        {
            get { return this.showLegend; }
            set { this.showLegend = value; }
        }

        [JsonProperty("legendPositionX")]
        [DefaultValue(0)]
        public double LegendPositionX
        {
            get { return this.legendPositionX; }
            set { this.legendPositionX = value; }
        }

        [JsonProperty("legendPositionY")]
        [DefaultValue(0)]
        public double LegendPositionY
        {
            get { return this.legendPositionY; }
            set { this.legendPositionY = value; }
        }

        [JsonProperty("legendWidth")]
        [DefaultValue(20)]
        public double LegendWidth
        {
            get { return this.legendWidth; }
            set { this.legendWidth = value; }
        }

        [JsonProperty("legendHeight")]
        [DefaultValue(20)]
        public double LegendHeight
        {
            get { return this.legendHeight; }
            set { this.legendHeight = value; }
        }

        [JsonProperty("legendIcon")]
        [DefaultValue(LegendIcons.Rectangle)]
        [JsonConverter(typeof(StringEnumConverter))]
        public LegendIcons LegendIcon
        {
            get { return this.legendIcon; }
            set { this.legendIcon = value; }
        }

        [JsonProperty("legendType")]
        [DefaultValue(null)]
        public string LegendType
        {
            get { return this.legendType; }
            set { this.legendType = value; }
        }

        #endregion       

    }
}
