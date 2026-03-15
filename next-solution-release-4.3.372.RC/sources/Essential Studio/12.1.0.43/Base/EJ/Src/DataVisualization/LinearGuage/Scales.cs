#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class Scales
    {
        #region Fields

        //Boolean Values
        private bool showRanges = false;
        private bool showIndicators = false;
        private bool showCustomLabel = false;
        private bool showLabels = true;
        private bool showTicks = true;
        private bool showBarPointers = true;
        private bool showMarkerPointers = true;
        //Enumeration Values
        private ScaleDirections scaleDirection = ScaleDirections.CounterClockwise;
        private ScaleStyles scaleStyle = ScaleStyles.Rectangle;
        //String Values
        private String backgroundColor = null;
        private String borderColor = null;
        //Interger Values
        private int minimum = 0;
        private int maximum = 100;
        private int majorIntervalValue = 10;
        private int minorIntervalValue = 2;
        private int scaleBarSize = 30;
        private int shadowOffset = 0;
        private int scaleBarLength = 290;
        //Double Values
        private double borderWidth = 1.5;
        private double opacity = 0.4;
        //Object Values
        private LinearLocation location = new LinearLocation();
        private List<LinearTicks> ticks = new List<LinearTicks>();
        private List<Ranges> ranges = new List<Ranges>();
        private List<Labels> labels = new List<Labels>();
        private List<MarkerPointers> markerPointers = new List<MarkerPointers>();
        private List<BarPointers> barPointers = new List<BarPointers>();
        private List<Indicators> indicators = new List<Indicators>();
        private List<CustomLabel> customLabel = new List<CustomLabel>();
        #endregion

        #region Properties
        //Boolean values
        [JsonProperty("showRanges")]
        [DefaultValue(false)]
        public bool ShowRanges 
        {
            get { return this.showRanges; }
            set { this.showRanges = value; }
        }
        [JsonProperty("showIndicators")]
        [DefaultValue(false)]
        public bool ShowIndicators
        {
            get { return this.showIndicators; }
            set { this.showIndicators = value; }
        }
        [JsonProperty("showCustomLabel")]
        [DefaultValue(false)]
        public bool ShowCustomLabel
        {
            get { return this.showCustomLabel; }
            set { this.showCustomLabel = value; }
        }
        [JsonProperty("showLabels")]
        [DefaultValue(true)]
        public bool ShowLabels
        {
            get { return this.showLabels; }
            set { this.showLabels = value; }
        }
        [JsonProperty("showTicks")]
        [DefaultValue(true)]
        public bool ShowTicks
        {
            get { return this.showTicks; }
            set { this.showTicks = value; }
        }
        [JsonProperty("showBarPointers")]
        [DefaultValue(true)]
        public bool ShowBarPointers
        {
            get { return this.showBarPointers; }
            set { this.showBarPointers = value; }
        }
        [JsonProperty("showMarkerPointers")]
        [DefaultValue(true)]
        public bool ShowMarkerPointers
        {
            get { return this.showMarkerPointers; }
            set { this.showMarkerPointers = value; }
        }
        //Enumeration Values
        [JsonProperty("scaleDirection")]
        [DefaultValue(ScaleDirections.CounterClockwise)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ScaleDirections ScaleDirection
        {
            get { return this.scaleDirection; }
            set { this.scaleDirection = value; }
        }
        [JsonProperty("scaleStyle")]
        [DefaultValue(ScaleStyles.Rectangle)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ScaleStyles ScaleStyle
        {
            get { return this.scaleStyle; }
            set { this.scaleStyle = value; }
        }
        //String Values
        [JsonProperty("backgroundColor")]
        [DefaultValue(null)]
        public String BackgroundColor
        {
            get { return this.backgroundColor; }
            set { this.backgroundColor = value; }
        }
        [JsonProperty("borderColor")]
        [DefaultValue(null)]
        public String ScaleBorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }
        //Integer values
        [JsonProperty("minimum")]
        [DefaultValue(0)]
        public int Minimum
        {
            get { return this.minimum; }
            set { this.minimum = value; }
        }
        [JsonProperty("maximum")]
        [DefaultValue(100)]
        public int Maximum
        {
            get { return this.maximum; }
            set { this.maximum = value; }
        }
        [JsonProperty("majorIntervalValue")]
        [DefaultValue(10)]
        public int MajorIntervalValue
        {
            get { return this.majorIntervalValue; }
            set { this.majorIntervalValue = value; }
        }
        [JsonProperty("minorIntervalValue")]
        [DefaultValue(2)]
        public int MinorIntervalValue
        {
            get { return this.minorIntervalValue; }
            set { this.minorIntervalValue = value; }
        }
        [JsonProperty("scaleBarSize")]
        [DefaultValue(30)]
        public int ScaleBarSize
        {
            get { return this.scaleBarSize; }
            set { this.scaleBarSize = value; }
        }
        [JsonProperty("shadowOffset")]
        [DefaultValue(0)]
        public int ShadowOffset
        {
            get { return this.shadowOffset; }
            set { this.shadowOffset = value; }
        }
        [JsonProperty("scaleBarLength")]
        [DefaultValue(290)]
        public int ScaleBarLength
        {
            get { return this.scaleBarLength; }
            set { this.scaleBarLength = value; }
        }
        //Object values
        [JsonProperty("location")]
        public LinearLocation ScaleLocation
        {
            get { return this.location; }
            set { this.location = value; }
        }
        [JsonProperty("ticks")]
        public List<LinearTicks> Ticks
        {
            get { return this.ticks; }
            set { this.ticks = value; }
        }
        [JsonProperty("ranges")]
        public List<Ranges> Ranges
        {
            get { return this.ranges; }
            set { this.ranges = value; }
        }
        [JsonProperty("labels")]
        public List<Labels> Labels
        {
            get { return this.labels; }
            set { this.labels = value; }
        }
        [JsonProperty("markerPointers")]
        public List<MarkerPointers> MarkerPointers
        {
            get { return this.markerPointers; }
            set { this.markerPointers = value; }
        }
        [JsonProperty("barPointers")]
        public List<BarPointers> BarPointers
        {
            get { return this.barPointers; }
            set { this.barPointers = value; }
        }
        [JsonProperty("indicators")]
        public List<Indicators> Indicators
        {
            get { return this.indicators; }
            set { this.indicators = value; }
        }
        [JsonProperty("customLabel")]
        public List<CustomLabel> CustomLabel
        {
            get { return this.customLabel; }
            set { this.customLabel = value; }
        }
        //Double values
        [JsonProperty("borderWidth")]
        [DefaultValue(1.5)]
        public double BorderWidth
        {
            get { return this.borderWidth; }
            set { this.borderWidth = value; }
        }
        [JsonProperty("opacity")]
        [DefaultValue(0.4)]
        public double ScaleOpacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeLocation()
        {
            if (Utils.PropertyCompare(ScaleLocation, new LinearLocation()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeTicks()
        {
            if (Ticks.Count!=0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeRanges()
        {
            if (Ranges.Count!=0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeLabels()
        {
            if (Labels.Count!=0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeMarkerPointers()
        {
            if (MarkerPointers.Count!=0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeBarPointers()
        {
            if (BarPointers.Count!=0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeIndicators()
        {
            if (Indicators.Count!=0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeCustomLabel()
        {
            if (CustomLabel.Count!=0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
