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
    public class CircularGaugeProperties
    {
        #region Fields
         //Integer values
         private int value = 0;
         private int minimum = 0;
         private int maximum = 100;
         private int radius= 180;
         private int width= 360;
         private int height = 360; 
         private int halfCircleFrameStartAngle= 180;
         private int halfCircleFrameEndAngle = 360;
         private int animationSpeed= 500;

         //Boolean values
         private bool readOnly= true;
         private bool isRadialGradient= false;
         private bool animate = true;
         private bool canResize = false;

         //String values         
         private string backgroundColor = null; 
        
         //Enumeration values
         private Themes theme = Themes.FlatLight;
         private FrameTypes frameType = FrameTypes.fullCircle;

         //Events
         private string drawTicks= null;
         private string drawLabels= null;
         private string drawPointers= null;
         private string drawRange= null;
         private string drawCustomLabel= null;
         private string drawIndicators= null;
         private string drawPointerCap= null;
         private string renderComplete= null;
         private string mouseClick= null;
         private string mouseClickMove= null;
         private string mouseClickUp= null;

         //Object values
         private List<InteriorGradients> interiorGradient = new List<InteriorGradients>();
         private List<CircularScales> scales = new List<CircularScales>();

         //CircularGauge
         private CircularGauge circulargauge = new CircularGauge();
        #endregion 

         //public CircularGaugeProperties() { }

        #region Properties
         //Integer values
         [JsonProperty("value")]
         [DefaultValue(0)]
         public int Value
         {
             get { return this.value; }
             set { this.value = value; }
         }
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
         [JsonProperty("radius")]
         [DefaultValue(180)]
         public int Radius
         {
             get { return this.radius; }
             set { this.radius = value; }
         }
         [JsonProperty("width")]
         [DefaultValue(360)]
         public int Width
         {
             get { return this.width; }
             set { this.width = value; }
         }
         [JsonProperty("height")]
         [DefaultValue(360)]
         public int Height
         {
             get { return this.height; }
             set { this.height = value; }
         }
         [JsonProperty("halfCircleFrameStartAngle")]
         [DefaultValue(180)]
         public int HalfCircleFrameStartAngle
         {
             get { return this.halfCircleFrameStartAngle; }
             set { this.halfCircleFrameStartAngle = value; }
         }
         [JsonProperty("halfCircleFrameEndAngle")]
         [DefaultValue(360)]
         public int HalfCircleFrameEndAngle
         {
             get { return this.halfCircleFrameEndAngle; }
             set { this.halfCircleFrameEndAngle = value; }
         }
         [JsonProperty("animationSpeed")]
         [DefaultValue(500)]
         public int AnimationSpeed
         {
             get { return this.animationSpeed; }
             set { this.animationSpeed = value; }
         }

        //Boolean values
         [JsonProperty("readOnly")]
         [DefaultValue(true)]
         public bool ReadOnly
         {
             get { return this.readOnly; }
             set { this.readOnly = value; }
         }
         [JsonProperty("isRadialGradient")]
         [DefaultValue(false)]
         public bool IsRadialGradient
         {
             get { return this.isRadialGradient; }
             set { this.isRadialGradient = value; }
         }
         [JsonProperty("animate")]
         [DefaultValue(true)]
         public bool Animate
         {
             get { return this.animate; }
             set { this.animate = value; }
         }
         [JsonProperty("canResize")]
         [DefaultValue(false)]
         public bool CanResize
         {
             get { return this.canResize; }
             set { this.canResize = value; }
         }
         //String values         
         [JsonProperty("backgroundColor")]
         [DefaultValue(null)]
         public String BackgroundColor
         {
             get { return this.backgroundColor; }
             set { this.backgroundColor = value; }
         }         
         //Enumeration Values
         [JsonProperty("theme")]
         [DefaultValue(Themes.FlatLight)]
         [JsonConverter(typeof(StringEnumConverter))]
         public Themes Theme
         {
             get { return this.theme; }
             set { this.theme = value; }
         }
         [JsonProperty("frameType")]
         [DefaultValue(FrameTypes.fullCircle)]
         [JsonConverter(typeof(StringEnumConverter))]
         public FrameTypes Frametype
         {
             get { return this.frameType; }
             set { this.frameType = value; }
         }
         //Events
         [JsonProperty("drawTicks")]
         [DefaultValue(null)]
         public String DrawTicks
         {
             get { return this.drawTicks; }
             set { this.drawTicks = value; }
         }
         [JsonProperty("drawLabels")]
         [DefaultValue(null)]
         public String DrawLabels
         {
             get { return this.drawLabels; }
             set { this.drawLabels = value; }
         }
         [JsonProperty("drawPointers")]
         [DefaultValue(null)]
         public String DrawPointers
         {
             get { return this.drawPointers; }
             set { this.drawPointers = value; }
         }
         [JsonProperty("drawRange")]
         [DefaultValue(null)]
         public String DrawRange
         {
             get { return this.drawRange; }
             set { this.drawRange = value; }
         }
         [JsonProperty("drawCustomLabel")]
         [DefaultValue(null)]
         public String DrawCustomLabel
         {
             get { return this.drawCustomLabel; }
             set { this.drawCustomLabel = value; }
         }
         [JsonProperty("drawIndicators")]
         [DefaultValue(null)]
         public String DrawIndicators
         {
             get { return this.drawIndicators; }
             set { this.drawIndicators = value; }
         }
         [JsonProperty("drawPointerCap")]
         [DefaultValue(null)]
         public String DrawPointerCap
         {
             get { return this.drawPointerCap; }
             set { this.drawPointerCap = value; }
         }
         [JsonProperty("renderComplete")]
         [DefaultValue(null)]
         public String RenderComplete
         {
             get { return this.renderComplete; }
             set { this.renderComplete = value; }
         }
         [JsonProperty("mouseClick")]
         [DefaultValue(null)]
         public String MouseClick
         {
             get { return this.mouseClick; }
             set { this.mouseClick = value; }
         }
         [JsonProperty("mouseClickMove")]
         [DefaultValue(null)]
         public String MouseClickMove
         {
             get { return this.mouseClickMove; }
             set { this.mouseClickMove = value; }
         }
         [JsonProperty("mouseClickUp")]
         [DefaultValue(null)]
         public String MouseClickUp
         {
             get { return this.mouseClickUp; }
             set { this.mouseClickUp = value; }
         }
         //Object values
         [JsonProperty("interiorGradient")]
         public List<InteriorGradients> InteriorGradient
         {
             get { return this.interiorGradient; }
             set { this.interiorGradient = value; }
         }
         //Object values
         [JsonProperty("scales")]
         public List<CircularScales> Scales
         {
             get { return this.scales; }
             set { this.scales = value; }
         }
        #endregion
        #region ShouldSerialize Methods
         public bool ShouldSerializeInteriorGradient()
         {
             if (InteriorGradient.Count != 0)
                 return true;
             else
                 return false;
         }
         public bool ShouldSerializeScales()
         {
             if (Scales.Count != 0)
                 return true;
             else
                 return false;
         }           
        #endregion
    }
}
