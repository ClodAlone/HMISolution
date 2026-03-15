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
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript;

namespace Syncfusion.JavaScript.Models
{
    public class SliderProperties
    {
        #region Fields
        //Int Values
        private int animationSpeed = 500;
        private int startValue = 0;
        private int endValue = 100;
        private int step = 1;
        private int largeStep = 10;
        private int smallStep = 1;


        //Boolean Values
        private bool animate = true;
        private bool readOnly = false;
        private bool enabled = true;
        private bool showTooltip = true;
        private bool persist = false;
        private bool showSmallTicks = true;
        private bool showScale = false;
        private bool roundedCorner = false;
        private bool rtl = false;

        //Enumeration Values
        private Orientation orientation = Orientation.Horizontal;
        private SlideType sliderType = SlideType.Default;

        //String Values
        private String height = null;
        private String width = null;
        private String cssClass = "";
        private String value = null;
        private String values = null;

        //List
        //private List<SliderValues> values = new List<SliderValues>();
        //int array
        

        //Events 
        private String create = null;
        private String start = null;
        private String stop = null;
        private String slide = null;
        private String change = null;
        private String destroy = null;

         //Slider
        private Slider slider = new Slider();
        #endregion

        public SliderProperties() { }
        //public SliderProperties(String id, Slider slider)
        //{
        //    slider.ID = id;
        //    this.slider = slider;
        //}

        #region Properties
        //int values
        [JsonProperty("animationSpeed")]
        [DefaultValue(500)]
        public int AnimationSpeed
        {
            get { return this.animationSpeed; }
            set { this.animationSpeed = value; }
        }
        [JsonProperty("startValue")]
        [DefaultValue(0)]
        public int StartValue
        {
            get { return this.startValue; }
            set { this.startValue = value; }
        }
        [JsonProperty("endValue")]
        [DefaultValue(100)]
        public int EndValue
        {
            get { return this.endValue; }
            set { this.endValue = value; }
        }
        [JsonProperty("step")]
        [DefaultValue(1)]
        public int Step
        {
            get { return this.step; }
            set { this.step = value; }
        }
        [JsonProperty("largeStep")]
        [DefaultValue(10)]
        public int LargeStep
        {
            get { return this.largeStep; }
            set { this.largeStep = value; }
        }
        [JsonProperty("smallStep")]
        [DefaultValue(1)]
        public int SmallStep
        {
            get { return this.smallStep; }
            set { this.smallStep = value; }
        }
        //Boolean values
        [JsonProperty("animate")]
        [DefaultValue(true)]
        public bool Animate
        {
            get { return this.animate; }
            set { this.animate = value; }
        }
        [JsonProperty("roundedCorner")]
        [DefaultValue(false)]
        public bool RoundedCorner
        {
            get { return this.roundedCorner; }
            set { this.roundedCorner = value; }
        }
        [JsonProperty("readOnly")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return this.readOnly; }
            set { this.readOnly = value; }
        }
        [JsonProperty("rtl")]
        [DefaultValue(false)]
        public bool Rtl
        {
            get { return this.rtl; }
            set { this.rtl = value; }
        }
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("showScale")]
        [DefaultValue(false)]
        public bool ShowScale
        {
            get { return this.showScale; }
            set { this.showScale = value; }
        }
        [JsonProperty("showSmallTicks")]
        [DefaultValue(true)]
        public bool ShowSmallTicks
        {
            get { return this.showSmallTicks; }
            set { this.showSmallTicks = value; }
        }
        [JsonProperty("showTooltip")]
        [DefaultValue(true)]
        public bool ShowTooltip
        {
            get { return this.showTooltip; }
            set { this.showTooltip = value; }
        }
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist
        {
            get { return this.persist; }
            set { this.persist = value; }
        }
        //Enum values
        [JsonProperty("orientation")]
        [DefaultValue(Orientation.Horizontal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Orientation Orientation
        {
            get { return this.orientation; }
            set { this.orientation = value; }
        }
        [JsonProperty("sliderType")]
        [DefaultValue(SlideType.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SlideType SliderType
        {
            get { return this.sliderType; }
            set { this.sliderType = value; }
        }
        //string values
        [JsonProperty("height")]
        [DefaultValue(null)]
        public String Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        [JsonProperty("width")]
        [DefaultValue(null)]
        public String Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("value")]
        [DefaultValue(null)]
        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }       
        [JsonProperty("values")]
        [DefaultValue(null)]
        public String Values
        {
            get { return this.values; }
            set { this.values = value; }
        }
        
        //Events 
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("start")]
        [DefaultValue(null)]
        public String Start
        {
            get { return this.start; }
            set { this.start = value; }
        }
        [JsonProperty("stop")]
        [DefaultValue(null)]
        public String Stop
        {
            get { return this.stop; }
            set { this.stop = value; }
        }
        [JsonProperty("slide")]
        [DefaultValue(null)]
        public String Slide
        {
            get { return this.slide; }
            set { this.slide = value; }
        }
        [JsonProperty("change")]
        [DefaultValue(null)]
        public String Change
        {
            get { return this.change; }
            set { this.change = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        #endregion       
    }  
}
