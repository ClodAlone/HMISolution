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
    public class LinearGaugeProperties
    {
        #region Fields

        //Boolean Values
        private bool readOnly = true;
        private bool animate = true;
        private bool canResize = false;
        //Enumeration Values
        private Themes theme = Themes.FlatLight;
        private Orientation orientation = Orientation.Vertical;
        //String Values
        private String backgroundColor = null;
        private String borderColor = null;
        private String labelColor = null;
        private String tickColor = null;
        private String frameBackgroundImageUrl = null;
        //Object Values
        private List<Scales> scales = new List<Scales>();
        private List<PointerGradient1> pointerGradient1 = new List<PointerGradient1>();
        private List<PointerGradient2> pointerGradient2 = new List<PointerGradient2>();
        //Interger Values
        private int value = 0;
        private int minimum = 0;
        private int maximum = 100;
        private int height = 400;
        private int width = 150;
        private int frameOuterWidth = 12;
        private int frameInnerWidth = 8;
        private int animationSpeed = 500;
        //Events 
        private String create = null;
        private String drawTicks = null;
        private String drawLabels = null;
        private String drawBarPointers = null;
        private String drawMarkerPointers = null;
        private String drawRange = null;
        private String drawCustomLabel = null;
        private String drawIndicators = null;
        private String load = null;
        private String init = null;
        private String renderComplete = null;
        private String destroy = null;
        //LinearGauge 
        private LinearGauge linearGauge = new LinearGauge();

        #endregion

        #region Properties
        //Boolean values
        [JsonProperty("readOnly")]
        [DefaultValue(true)]
        public bool ReadOnly
        {
            get { return this.readOnly; }
            set { this.readOnly = value; }
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
        //Enumeration Values
        [JsonProperty("theme")]
        [DefaultValue(Themes.FlatLight)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Themes Theme
        {
            get { return this.theme; }
            set { this.theme = value; }
        }
        [JsonProperty("orientation")]
        [DefaultValue(Orientation.Vertical)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Orientation Orientation
        {
            get { return this.orientation; }
            set { this.orientation = value; }
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
        public String BorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }
        [JsonProperty("labelColor")]
        [DefaultValue(null)]
        public String LabelColor
        {
            get { return this.labelColor; }
            set { this.labelColor = value; }
        }
        [JsonProperty("tickColor")]
        [DefaultValue(null)]
        public String TickColor
        {
            get { return this.tickColor; }
            set { this.tickColor = value; }
        }
        [JsonProperty("frameBackgroundImageUrl")]
        [DefaultValue(null)]
        public String FrameBackgroundImageUrl
        {
            get { return this.frameBackgroundImageUrl; }
            set { this.frameBackgroundImageUrl = value; }
        }
        //Integer values
        [JsonProperty("height")]
        [DefaultValue(400)]
        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        [JsonProperty("width")]
        [DefaultValue(150)]
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        [JsonProperty("frameOuterWidth")]
        [DefaultValue(12)]
        public int FrameOuterWidth
        {
            get { return this.frameOuterWidth; }
            set { this.frameOuterWidth = value; }
        }
        [JsonProperty("frameInnerWidth")]
        [DefaultValue(8)]
        public int FrameInnerWidth
        {
            get { return this.frameInnerWidth; }
            set { this.frameInnerWidth = value; }
        }
        [JsonProperty("animationSpeed ")]
        [DefaultValue(500)]
        public int AnimationSpeed
        {
            get { return this.animationSpeed; }
            set { this.animationSpeed = value; }
        }
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
        //Events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
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
        [JsonProperty("drawBarPointers")]
        [DefaultValue(null)]
        public String DrawBarPointers
        {
            get { return this.drawBarPointers; }
            set { this.drawBarPointers = value; }
        }
        [JsonProperty("drawMarkerPointers")]
        [DefaultValue(null)]
        public String DrawMarkerPointers
        {
            get { return this.drawMarkerPointers; }
            set { this.drawMarkerPointers = value; }
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
        [JsonProperty("load")]
        [DefaultValue(null)]
        public String Load
        {
            get { return this.load; }
            set { this.load = value; }
        }
        [JsonProperty("init")]
        [DefaultValue(null)]
        public String Init
        {
            get { return this.init; }
            set { this.init = value; }
        }
        [JsonProperty("renderComplete")]
        [DefaultValue(null)]
        public String RenderComplete
        {
            get { return this.renderComplete; }
            set { this.renderComplete = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        //Object values
        [JsonProperty("scales")]
        public List<Scales> Scales
        {
            get { return this.scales; }
            set { this.scales = value; }
        }
        [JsonProperty("pointerGradient1")]
        public List<PointerGradient1> PointerGradient1
        {
            get { return this.pointerGradient1; }
            set { this.pointerGradient1 = value; }
        }
        [JsonProperty("pointerGradient2")]
        public List<PointerGradient2> PointerGradient2
        {
            get { return this.pointerGradient2; }
            set { this.pointerGradient2 = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeScales()
        {
            if (Scales.Count!=0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializePoinerGradient1()
        {
            if (PointerGradient1.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializePoinerGradient2()
        {
            if (PointerGradient2.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
