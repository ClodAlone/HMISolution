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
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataVisualization.Models;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class CircularScales
    {
        #region fields

        //Integer values   
                private int scaleBarSize= 6;                               
                private int sweepAngle= 310;
                private int scaleRadius= 170;
                private int startAngle= 115;
                private double majorIntervalValue = 10;
                private double minorIntervalValue = 2;
                private int maximum= 100;
                private int minimum = 0;
                private int pointerCapRadius= 7;
                private int pointerCapBorderWidth= 3;
          
        //Double values

                private double scaleBorderWidth = 1.5;

        //Enumeration values

                private ScaleDirections scaleDirection = ScaleDirections.Clockwise;

        //Object values

                private List<Pointers> pointers = new List<Pointers>();
                private List<CircularRanges> ranges = new List<CircularRanges>();
                private List<CircularTicks> ticks = new List<CircularTicks>();
                private List<CircularLabels> labels = new List<CircularLabels>();
                private List<CircularIndicators> indicators = new List<CircularIndicators>();
                private List<SubGauge> subGauge = new List<SubGauge>();
                private List<CapInteriorGradient> capInteriorGradient = new List<CapInteriorGradient>();
                private List<CircularCustomLabel> customLabel = new List<CircularCustomLabel>();

        //Boolean values

                private bool showScaleBar= false;
                private bool labelAutoAngle= false;
                private bool showPointers= true;
                private bool showRanges= false;
                private bool showTicks= true;
                private bool showLabels= true;
                private bool showIndicators= false;

        //String values

                private string capBorderColor= null;
                private string capBackgroundColor= null;
                private string borderColor= null;
                private string backgroundColor= null;

       #endregion

       #region Properties
                //Boolean values
                [JsonProperty("showScaleBar")]
                [DefaultValue(false)]
                public bool ShowScaleBar
                {
                    get { return this.showScaleBar; }
                    set { this.showScaleBar = value; }
                }
                [JsonProperty("labelAutoAngle")]
                [DefaultValue(false)]
                public bool LabelAutoAngle
                {
                    get { return this.labelAutoAngle; }
                    set { this.labelAutoAngle = value; }
                }
                [JsonProperty("showPointers")]
                [DefaultValue(true)]
                public bool ShowPointers
                {
                    get { return this.showPointers; }
                    set { this.showPointers = value; }
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
                //Enumeration Values
                [JsonProperty("scaleDirection")]
                [DefaultValue(ScaleDirections.Clockwise)]
                [JsonConverter(typeof(StringEnumConverter))]
                public ScaleDirections ScaleDirection
                {
                    get { return this.scaleDirection; }
                    set { this.scaleDirection = value; }
                }
                //String Values
               [JsonProperty("capBorderColor")]
               [DefaultValue(null)]
                public String CapBorderColor
               {
                   get { return this.capBorderColor; }
                   set { this.capBorderColor = value; }
               }
               [JsonProperty("capBackgroundColor")]
               [DefaultValue(null)]
               public String CapBackgroundColor
               {
                   get { return this.capBackgroundColor; }
                   set { this.capBackgroundColor = value; }
               }
               [JsonProperty("borderColor")]
               [DefaultValue(null)]
               public String BorderColor
               {
                   get { return this.borderColor; }
                   set { this.borderColor = value; }
               }
               [JsonProperty("backgroundColor")]
               [DefaultValue(null)]
               public String BackgroundColor
               {
                   get { return this.backgroundColor; }
                   set { this.backgroundColor = value; }
               }
               //Object values               
               [JsonProperty("ticks")]
               public List<CircularTicks> Ticks
               {
                   get { return this.ticks; }
                   set { this.ticks = value; }
               }
               [JsonProperty("ranges")]
               public List<CircularRanges> Ranges
               {
                   get { return this.ranges; }
                   set { this.ranges = value; }
               }
               [JsonProperty("labels")]
               public List<CircularLabels> Labels
               {
                   get { return this.labels; }
                   set { this.labels = value; }
               }
               [JsonProperty("pointers")]
               public List<Pointers> Pointers
               {
                   get { return this.pointers; }
                   set { this.pointers = value; }
               }
               [JsonProperty("subGauge")]
               public List<SubGauge> SubGauge
               {
                   get { return this.subGauge; }
                   set { this.subGauge = value; }
               }
               [JsonProperty("indicators")]
               public List<CircularIndicators> Indicators
               {
                   get { return this.indicators; }
                   set { this.indicators = value; }
               }
               [JsonProperty("capInteriorGradient")]
               public List<CapInteriorGradient> CapInteriorGradient
               {
                   get { return this.capInteriorGradient; }
                   set { this.capInteriorGradient = value; }
               }
               [JsonProperty("customLabel")]
               public List<CircularCustomLabel> CustomLabel
               {
                   get { return this.customLabel; }
                   set { this.customLabel = value; }
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
               [JsonProperty("scaleBarSize")]
               [DefaultValue(6)]
               public int ScaleBarSize
               {
                   get { return this.scaleBarSize; }
                   set { this.scaleBarSize = value; }
               }
               [JsonProperty("sweepAngle")]
               [DefaultValue(310)]
               public int SweepAngle
               {
                   get { return this.sweepAngle; }
                   set { this.sweepAngle = value; }
               }
               [JsonProperty("scaleRadius")]
               [DefaultValue(170)]
               public int ScaleRadius
               {
                   get { return this.scaleRadius; }
                   set { this.scaleRadius = value; }
               }
               [JsonProperty("startAngle")]
               [DefaultValue(115)]
               public int StartAngle
               {
                   get { return this.startAngle; }
                   set { this.startAngle = value; }
               }
               [JsonProperty("majorIntervalValue")]
               [DefaultValue(10)]
               public double MajorIntervalValue
               {
                   get { return this.majorIntervalValue; }
                   set { this.majorIntervalValue = value; }
               }
               [JsonProperty("minorIntervalValue")]
               [DefaultValue(2)]
               public double MinorIntervalValue
               {
                   get { return this.minorIntervalValue; }
                   set { this.minorIntervalValue = value; }
               }
               [JsonProperty("pointerCapRadius")]
               [DefaultValue(7)]
               public int PointerCapRadius
               {
                   get { return this.pointerCapRadius; }
                   set { this.pointerCapRadius = value; }
               }
               [JsonProperty("pointerCapBorderWidth")]
               [DefaultValue(7)]
               public int PointerCapBorderWidth
               {
                   get { return this.pointerCapBorderWidth; }
                   set { this.pointerCapBorderWidth = value; }
               }
               //Double values               
               [JsonProperty("scaleBorderWidth")]
               [DefaultValue(1.5)]
               public double ScaleBorderWidth
               {
                   get { return this.scaleBorderWidth; }
                   set { this.scaleBorderWidth = value; }
               }

       #endregion

               #region ShouldSerialize Methods
               public bool ShouldSerializePointers()
               {
                   if (Pointers.Count != 0)
                       return true;
                   else
                       return false;
               }
               public bool ShouldSerializeTicks()
               {
                   if (Ticks.Count != 0)
                       return true;
                   else
                       return false;
               }
               public bool ShouldSerializeRanges()
               {
                   if (Ranges.Count != 0)
                       return true;
                   else
                       return false;
               }
               public bool ShouldSerializeLabels()
               {
                   if (Labels.Count != 0)
                       return true;
                   else
                       return false;
               }
               public bool ShouldSerializeCustomLabel()
               {
                   if (CustomLabel.Count != 0)
                       return true;
                   else
                       return false;
               }
               public bool ShouldSerializeSubGauge()
               {
                   if (SubGauge.Count!= 0)
                       return true;
                   else
                       return false;
               }   
              public bool ShouldSerializeIndicators()
               {
                   if (Indicators.Count != 0)
                       return true;
                   else
                       return false;
               }
              public bool ShouldSerializeCapInteriorGradient()
              {
                  if (CapInteriorGradient.Count != 0)
                      return true;
                  else
                      return false;
              }               
               #endregion
    }


}