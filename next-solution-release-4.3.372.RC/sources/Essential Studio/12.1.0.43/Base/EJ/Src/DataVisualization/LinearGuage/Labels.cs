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
using Syncfusion.JavaScript.DataVisualization.Models;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class Labels
    {
        #region Fields

        //String Values
        private String textColor = null;
        private string unitText = null;
        private string unitTextPosition = "Back";
        //Interger Values
        private int yDistanceFromScale = 0;
        private int xDistanceFromScale = -10;
        private int angle = 0;
        //Double Values
        private double opacity = 0;
        //object values
        private Font font = new Font();
        //Enumeration Values
        private GaugeStyles labelStyle = GaugeStyles.Major;
        private LabelPlacements labelPlacement = LabelPlacements.Near;
        //Boolean Value
        private bool includeFirstValue = true;
        #endregion
        #region Properties

        //String Values
        [JsonProperty("textColor")]
        [DefaultValue(null)]
        public String TextColor
        {
            get { return this.textColor; }
            set { this.textColor = value; }
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
        //Integer values
        [JsonProperty("xDistanceFromScale")]
        [DefaultValue(-10)]
        public int XDistanceFromScale
        {
            get { return this.xDistanceFromScale; }
            set { this.xDistanceFromScale = value; }
        }
        [JsonProperty("yDistanceFromScale")]
        [DefaultValue(0)]
        public int YDistanceFromScale
        {
            get { return this.yDistanceFromScale; }
            set { this.yDistanceFromScale = value; }
        }
        [JsonProperty("angle")]
        [DefaultValue(0)]
        public int Angle
        {
            get { return this.angle; }
            set { this.angle = value; }
        }
        //Double Values
        [JsonProperty("opacity")]
        [DefaultValue(0)]
        public double Opacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
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
        [JsonProperty("labelPlacement")]
        [DefaultValue(LabelPlacements.Near)]
        [JsonConverter(typeof(StringEnumConverter))]
        public LabelPlacements LabelPlacement
        {
            get { return this.labelPlacement; }
            set { this.labelPlacement = value; }
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
        public Font Font
        {
            get { return this.font; }
            set { this.font = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeFont()
        {
            if (Utils.PropertyCompare(Font, new Font()))
                return true;
            else
                return false;
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class LabelsBuilder
    {
        private List<Labels> label = new List<Labels>();
        Labels labels = new Labels();
        Scales scales;
        public LabelsBuilder(Scales labels)
        {
            this.scales = labels;
            this.label = labels.Labels;
        }
        //String Values
        public LabelsBuilder TextColor(String textColor)
        {
            labels.TextColor = textColor;
            return this;
        }
        public LabelsBuilder UnitText(String unitText)
        {
            labels.UnitText = unitText;
            return this;
        }
        public LabelsBuilder UnitTextPosition(String unitTextPosition)
        {
            labels.UnitTextPosition = unitTextPosition;
            return this;
        }
        //Integers
        public LabelsBuilder XDistanceFromScale(int xDistanceFromScale)
        {
            labels.XDistanceFromScale = xDistanceFromScale;
            return this;
        }
        public LabelsBuilder YDistanceFromScale(int yDistanceFromScale)
        {
            labels.YDistanceFromScale = yDistanceFromScale;
            return this;
        }
        public LabelsBuilder Angle(int angle)
        {
            labels.Angle = angle;
            return this;
        }
        //Double Values
        public LabelsBuilder Opacity(double opacity)
        {
            labels.Opacity = opacity;
            return this;
        }
        //EnumValues
        public LabelsBuilder LabelStyle(GaugeStyles labelStyle)
        {
            labels.LabelStyle = labelStyle;
            return this;
        }
        public LabelsBuilder LabelPlacement(LabelPlacements labelPlacement)
        {
            labels.LabelPlacement = labelPlacement;
            return this;
        }
        //BoolValues
        public LabelsBuilder IncludeFirstValue()
        {
            labels.IncludeFirstValue = true;
            return this;
        }
        public LabelsBuilder IncludeFirstValue(bool includeFirstValue)
        {
            labels.IncludeFirstValue = includeFirstValue;
            return this;
        }
        // Font Values
        public LabelsBuilder Font(Action<FontBuilder> font)
        {
            var fonts = new Font();
            labels.Font = fonts;
            var builder = new FontBuilder(this.labels);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public void Add()
        {
            scales.Labels.Add(labels);
            labels = new Labels();
        }
    }
}
