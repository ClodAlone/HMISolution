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
    public class CircularLabels
    {
        #region Fields

        //String Values
        private string labelColor = null;
        private string unitText = null;
        private string unitTextPosition = "Back";

        //Interger Values
        private int distanceFromScale = 0;        
        private int angle = 0; 
       
        //Double values
        private double opacity = 1;

        //object values
        private CircularFont font = new CircularFont();

        //Enumeration Values
        private GaugeStyles labelStyle = GaugeStyles.Major;
        private LabelPositions labelPosition = LabelPositions.Near;

        //Boolean Value
        private bool includeFirstValue = true;

        #endregion

        #region Properties

        //String Values
        [JsonProperty("labelColor")]
        [DefaultValue(null)]
        public String LabelColor
        {
            get { return this.labelColor; }
            set { this.labelColor = value; }
        }
        [JsonProperty("unitText")]
        [DefaultValue(null)]
        public String UnitText
        {
            get { return this.unitText; }
            set { this.unitText = value; }
        }
        [JsonProperty("unitTextPosition")]
        [DefaultValue("Back")]
        public String UnitTextPosition
        {
            get { return this.unitTextPosition; }
            set { this.unitTextPosition = value; }
        }
        //Double values
        [JsonProperty("opacity")]
        [DefaultValue(1)]
        public double Opacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
        }
        //Integer values
        [JsonProperty("distanceFromScale")]
        [DefaultValue(0)]
        public int DistanceFromScale
        {
            get { return this.distanceFromScale; }
            set { this.distanceFromScale = value; }
        }       
        [JsonProperty("angle")]
        [DefaultValue(0)]
        public int Angle
        {
            get { return this.angle; }
            set { this.angle = value; }
        }               
        //Enumeration Values
        [JsonProperty("labelStyle")]
        [DefaultValue(GaugeStyles.Major)]
        [JsonConverter(typeof(StringEnumConverter))]
        public GaugeStyles LabelStyle
        {
            get { return this.labelStyle; }
            set { this.labelStyle = value; }
        }
        [JsonProperty("labelPosition")]
        [DefaultValue(LabelPositions.Near)]
        [JsonConverter(typeof(StringEnumConverter))]
        public LabelPositions LabelPosition
        {
            get { return this.labelPosition; }
            set { this.labelPosition = value; }
        }
        //Boolean Values
        [JsonProperty("includeFirstValue")]
        [DefaultValue(true)]
        public bool IncludeFirstValue
        {
            get { return this.includeFirstValue; }
            set { this.includeFirstValue = value; }
        }
        //Object values
        [JsonProperty("font")]
        public CircularFont Font
        {
            get { return this.font; }
            set { this.font = value; }
        }
        #endregion
        #region ShouldSerialize Methods        
        public bool ShouldSerializeFont()
        {
            if (Utils.PropertyCompare(Font, new CircularFont()))
                return true;
            else
                return false;
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class CircularLabelsBuilder
    {
        CircularScales scales;
        private List<CircularLabels> label = new List<CircularLabels>();
        CircularLabels labels = new CircularLabels();
        public CircularLabelsBuilder(CircularScales labels)
        {
            this.scales=labels;
            this.label = labels.Labels;
        }
        //String Values
        public CircularLabelsBuilder LabelColor(String labelColor)
        {
            labels.LabelColor = labelColor;
            return this;
        }
        public CircularLabelsBuilder UnitText(String unitText)
        {
            labels.UnitText = unitText;
            return this;
        }
        public CircularLabelsBuilder UnitTextPosition(String unitTextPosition)
        {
            labels.UnitTextPosition = unitTextPosition;
            return this;
        }
        public CircularLabelsBuilder Opacity(double opacity)
        {
            labels.Opacity = opacity;
            return this;
        }
        //Integers
        public CircularLabelsBuilder DistanceFromScale(int distanceFromScale)
        {
            labels.DistanceFromScale = distanceFromScale;
            return this;
        }       
        public CircularLabelsBuilder Angle(int angle)
        {
            labels.Angle = angle;
            return this;
        }      
        //EnumValues
        public CircularLabelsBuilder LabelStyle(GaugeStyles labelStyle)
        {
            labels.LabelStyle = labelStyle;
            return this;
        }
        public CircularLabelsBuilder LabelPosition(LabelPositions labelPosition)
        {
            labels.LabelPosition = labelPosition;
            return this;
        }
        //BoolValues
        public CircularLabelsBuilder IncludeFirstValue()
        {
            labels.IncludeFirstValue = true;
            return this;
        }
        public CircularLabelsBuilder IncludeFirstValue(bool includeFirstValue)
        {
            labels.IncludeFirstValue = includeFirstValue;
            return this;
        }
        // Font Values
        public CircularLabelsBuilder Font(Action<CircularFontBuilder> font)
        {            
            var builder = new CircularFontBuilder(this.labels);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public void Add()
        {
            scales.Labels.Add(labels);
            labels = new CircularLabels();            
        }
    }
}
