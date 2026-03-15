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
   public class DigitalGaugeProperties
    {
#region Fields
       // int values
       private int height=150;
       private int width=400;
       private int frameInnerWidth=6;
       private int frameOuterWidth=10;
      
       //Object values
       private List<DigitalGaugeItems> items = new List<DigitalGaugeItems>();
                
       //Enum values
       private Themes themes = Themes.FlatLight;

       //string values
       private String value = "text";
       private String frameBackgroundImageUrl=null;
       private String segmentColor="#836B33";
                 
       //Events
       private String create=null;
       private String init=null;
       private String load=null;
       private String renderComplete=null;
       private String destroy=null;
        
            //Auto Complete
        private DigitalGauge digitalgauge = new DigitalGauge();
        #endregion
        public DigitalGaugeProperties() { }

        #region Properties
        //Boolean values
        private bool canResize = false;

        [JsonProperty("height")]
        [DefaultValue(150)]
        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        [JsonProperty("width")]
        [DefaultValue(400)]
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        [JsonProperty("frameInnerWidth")]
        [DefaultValue(6)]
        public int FrameInnerWidth
        {
            get { return this.frameInnerWidth; }
            set { this.frameInnerWidth = value; }
        }
        [JsonProperty("frameOuterWidth")]
        [DefaultValue(10)]
        public int FrameOuterWidth
        {
            get { return this.frameOuterWidth; }
            set { this.frameOuterWidth = value; }
        }
       //Objects
        [JsonProperty("items")]
        public List<DigitalGaugeItems> DigitalGaugeItems
        {
            get { return this.items; }
            set { this.items = value; }
        }
       //String values
        [JsonProperty("frameBackgroundImageUrl")]
        [DefaultValue(null)]
        public String FrameBackgroundImageUrl
        {
            get { return this.frameBackgroundImageUrl; }
            set { this.frameBackgroundImageUrl = value; }
        }
        [JsonProperty("segmentColor")]
        [DefaultValue("#836B33")]
        public String SegmentColor
        {
            get { return this.segmentColor; }
            set { this.segmentColor = value; }
        }
        [JsonProperty("themes")]
        [DefaultValue(Themes.FlatLight)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Themes Themes
        {
            get { return this.themes; }
            set { this.themes = value; }
        }
        [JsonProperty("value")]
        [DefaultValue("text")]
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
       //Events
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("init")]
        [DefaultValue(null)]
        public String Init
        {
            get { return this.init; }
            set { this.init = value; }
        }
        [JsonProperty("load")]
        [DefaultValue(null)]
        public String Load
        {
            get { return this.load; }
            set { this.load = value; }
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
        [JsonProperty("canResize")]
        [DefaultValue(false)]
        public bool CanResize
        {
            get { return this.canResize; }
            set { this.canResize = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeDigitalGaugeItems()
        {
            if (DigitalGaugeItems.Count!=0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
