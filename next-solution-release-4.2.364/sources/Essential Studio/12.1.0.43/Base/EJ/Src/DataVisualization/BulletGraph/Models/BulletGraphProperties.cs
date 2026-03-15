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
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class BulletGraphProperties
    {
        #region Fields
        private double value = 0;
        private double comparativeMeasureValue = 0;
        private int height = 200;
        private int width = 610;
        
        private String theme = "flatlight";
        private Orientation orientation = Orientation.Horizontal;
        private FlowDirection flowDirection = FlowDirection.Forward;
        private int qualitativeRangeSize = 50;
        private int quantitativeScaleLength = 475;
        private bool showTooltip = true;
        private String tooltipTemplateId = "";
        private bool enableAnimation = true;
        private bool bindRangeStrokeToTicks = false;
        private bool bindRangeStrokeToLabels = false;

        //objects
        private object fields = new Fields();
        private Object quantitativeScale = new QuantitativeScale();
        private List<QualitativeRanges> qualitativeRanges = new List<QualitativeRanges>();
        private object caption = new Caption();


        //Events
        private String drawTicks = String.Empty;
        private String drawLabels = String.Empty;
        private String drawCaption = String.Empty;
        private String drawQualitativeRanges = String.Empty;
        private String drawFeatureMeasureBar = String.Empty;
        private String drawCategory = String.Empty;
        private String drawComparativeMeasureSymbol = String.Empty;

        private BulletGraph bullet = new BulletGraph();
        #endregion

        public BulletGraphProperties() { }
       
        #region Properties
        // objects
        [JsonProperty("fields")]
        public object Fields
        {
            get { return this.fields; }
            set { this.fields = value; }
        }

        [JsonProperty("quantitativeScale")]
        
        public Object QuantitativeScale
        {
            get { return this.quantitativeScale; }
            set { this.quantitativeScale = value; }
        }

        [JsonProperty("qualitativeRanges")]
        public List<QualitativeRanges> QualitativeRanges
        {
            get { return this.qualitativeRanges; }
            set { this.qualitativeRanges = value; }
        }

        [JsonProperty("caption")]
        public object Caption
        {
            get { return this.caption; }
            set { this.caption = value; }
        }

       // //

        [JsonProperty("height")]
        [DefaultValue("200")]
        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        [JsonProperty("value")]
        [DefaultValue(0)]
        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        [JsonProperty("comparativeMeasureValue")]
        [DefaultValue(0)]
        public double ComparativeMeasureValue
        {
            get { return this.comparativeMeasureValue; }
            set { this.comparativeMeasureValue = value; }
        }

        [JsonProperty("width")]
        [DefaultValue("610")]
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        [JsonProperty("theme")]
        [DefaultValue("flatlight")]
        public String Theme
        {
            get { return this.theme; }
            set { this.theme = value; }
        }

        [JsonProperty("orientation")]
        [DefaultValue(Orientation.Horizontal)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Orientation Orientation
        {
            get { return this.orientation; }
            set { this.orientation = value; }
        }

        [JsonProperty("flowDirection")]
        [DefaultValue(FlowDirection.Forward)]
        [JsonConverter(typeof(StringEnumConverter))]
        public FlowDirection FlowDirection
        {
            get { return this.flowDirection; }
            set { this.flowDirection = value; }
        }

        [JsonProperty("qualitativeRangeSize")]
        [DefaultValue(50)]
        public int QualitativeRangeSize
        {
            get { return this.qualitativeRangeSize; }
            set { this.qualitativeRangeSize = value; }
        }

        [JsonProperty("quantitativeScaleLength")]
        [DefaultValue(475)]
        public int QuantitativeScaleLength
        {
            get { return this.quantitativeScaleLength; }
            set { this.quantitativeScaleLength = value; }
        }

        [JsonProperty("showTooltip")]
        [DefaultValue(true)]
        public bool ShowTooltip
        {
            get { return this.showTooltip; }
            set { this.showTooltip = value; }
        }

        [JsonProperty("tooltipTemplateId")]
        [DefaultValue("")]
        public String TooltipTemplateId
        {
            get { return this.tooltipTemplateId; }
            set { this.tooltipTemplateId = value; }
        }

        [JsonProperty("enableAnimation")]
        [DefaultValue(true)]
        public bool EnableAnimation
        {
            get { return this.enableAnimation; }
            set { this.enableAnimation = value; }
        }

        [JsonProperty("bindRangeStrokeToTicks")]
        [DefaultValue(false)]
        public bool BindRangeStrokeToTicks
        {
            get { return this.bindRangeStrokeToTicks; }
            set { this.bindRangeStrokeToTicks = value; }
        }

        [JsonProperty("bindRangeStrokeToLabels")]
        [DefaultValue(false)]
        public bool BindRangeStrokeToLabels
        {
            get { return this.bindRangeStrokeToLabels; }
            set { this.bindRangeStrokeToLabels = value; }
        }

        //Events
        [JsonProperty("drawTicks")]
        //[DefaultValue(null)]
        public String DrawTicks
        {
            get { return this.drawTicks; }
            set { this.drawTicks = value; }
        }

        [JsonProperty("drawLabels")]
        //[DefaultValue(null)]
        public String DrawLabels
        {
            get { return this.drawLabels; }
            set { this.drawLabels = value; }
        }

        [JsonProperty("drawCaption")]
        //[DefaultValue(null)]
        public String DrawCaption
        {
            get { return this.drawCaption; }
            set { this.drawCaption = value; }
        }

        [JsonProperty("drawQualitativeRanges")]
        //[DefaultValue(null)]
        public String DrawQualitativeRanges
        {
            get { return this.drawQualitativeRanges; }
            set { this.drawQualitativeRanges = value; }
        }

        [JsonProperty("drawFeatureMeasureBar")]
        //[DefaultValue(null)]
        public String DrawFeatureMeasureBar
        {
            get { return this.drawFeatureMeasureBar; }
            set { this.drawFeatureMeasureBar = value; }
        }

        [JsonProperty("drawCategory")]
        //[DefaultValue(null)]
        public String DrawCategory
        {
            get { return this.drawCategory; }
            set { this.drawCategory = value; }
        }

        [JsonProperty("drawComparativeMeasureSymbol")]
        //[DefaultValue(null)]
        public String DrawComparativeMeasureSymbol
        {
            get { return this.drawComparativeMeasureSymbol; }
            set { this.drawComparativeMeasureSymbol = value; }
        }

        #endregion

        #region ShouldSerialize Methods
        public bool ShouldSerializeFields()
        {
            if (Utils.PropertyCompare(Fields, new Fields()))
                return true;
            else
                return false;
        }


        public bool ShouldSerializeQuantitativeScale()
        {
            if (Utils.PropertyCompare(QuantitativeScale, new QuantitativeScale()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeQualitativeRanges()
        {
            if (QualitativeRanges.Count != 0)
                return true;
            else
                return false;
        }
        public bool ShouldSerializeCaption()
        {
            if (Utils.PropertyCompare(Caption, new Caption()))
                return true;
            else
                return false;
        }
        #endregion

    }
}
