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
    public class Ranges
    {
        #region Fields

        //String Values
        private String backgroundColor = "Magenta";
        private String borderColor = "Red";
        //Interger Values
        private int endWidth = 10;
        private int startWidth = 10;
        private int distanceFromScale = 50;
        private int endValue = 60;
        private int startValue = 20;
        //Double values
        private double borderWidth = 1.5;
        private double opacity = 1;
        //Enum Values
        private RangePositions rangePosition = RangePositions.Center;
        //Object values
        private List<RangeGradient> rangeGradient = new List<RangeGradient>();
        #endregion

        #region Properties

        //Object values
        [JsonProperty("rangeGradient")]
        public List<RangeGradient> RangeGradient
        {
            get { return this.rangeGradient; }
            set { this.rangeGradient = value; }
        }
        //String Values
        [JsonProperty("backgroundColor")]
        [DefaultValue("Magenta")]
        public String RangeBackgroundColor
        {
            get { return this.backgroundColor; }
            set { this.backgroundColor = value; }
        }
        [JsonProperty("borderColor")]
        [DefaultValue("Red")]
        public String RangeBorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }
        //Enumeration Values
        [JsonProperty("rangePosition")]
        [DefaultValue(RangePositions.Center)]
        [JsonConverter(typeof(StringEnumConverter))]
        public RangePositions RangePosition
        {
            get { return this.rangePosition; }
            set { this.rangePosition = value; }
        }
        //Integer values
        [JsonProperty("endWidth")]
        [DefaultValue(10)]
        public int EndWidth
        {
            get { return this.endWidth; }
            set { this.endWidth = value; }
        }
        [JsonProperty("startWidth")]
        [DefaultValue(10)]
        public int StartWidth
        {
            get { return this.startWidth; }
            set { this.startWidth = value; }
        }
        [JsonProperty("distanceFromScale")]
        [DefaultValue(50)]
        public int DistanceFromScale
        {
            get { return this.distanceFromScale; }
            set { this.distanceFromScale = value; }
        }
        [JsonProperty("endValue")]
        [DefaultValue(60)]
        public int EndValue
        {
            get { return this.endValue; }
            set { this.endValue = value; }
        }
        [JsonProperty("startValue")]
        [DefaultValue(20)]
        public int StartValue
        {
            get { return this.startValue; }
            set { this.startValue = value; }
        }
        //Double Values
        [JsonProperty("borderWidth")]
        [DefaultValue(1.5)]
        public double BorderWidth
        {
            get { return this.borderWidth; }
            set { this.borderWidth = value; }
        }
        [JsonProperty("opacity")]
        [DefaultValue(1)]
        public double RangeOpacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeRangeGradient()
        {
            if (RangeGradient.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
    public class RangesBuilder
    {
        private List<Ranges> range = new List<Ranges>();
        Ranges ranges = new Ranges();
        Scales scales;
        public RangesBuilder(Scales ranges)
        {
            this.scales = ranges;
            this.range = ranges.Ranges;
        }
        //String Values
        public RangesBuilder RangeBackgroundColor(String backgroundColor)
        {
            ranges.RangeBackgroundColor = backgroundColor;
            return this;
        }
        public RangesBuilder RangeBorderColor(String borderColor)
        {
            ranges.RangeBorderColor = borderColor;
            return this;
        }
        //EnumValues
        public RangesBuilder RangePosition(RangePositions rangePosition)
        {
            ranges.RangePosition = rangePosition;
            return this;
        }
        //Integers
        public RangesBuilder EndWidth(int endWidth)
        {
            ranges.EndWidth = endWidth;
            return this;
        }
        public RangesBuilder StartWidth(int startWidth)
        {
            ranges.StartWidth = startWidth;
            return this;
        }
        public RangesBuilder DistanceFromScale(int distanceFromScale)
        {
            ranges.DistanceFromScale = distanceFromScale;
            return this;
        }
        public RangesBuilder EndValue(int endValue)
        {
            ranges.EndValue = endValue;
            return this;
        }
        public RangesBuilder StartValue(int startValue)
        {
            ranges.StartValue = startValue;
            return this;
        }

        //DoubleValues
        public RangesBuilder BorderWidth(double borderWidth)
        {
            ranges.BorderWidth = borderWidth;
            return this;
        }
        public RangesBuilder RangeOpacity(double opacity)
        {
            ranges.RangeOpacity = opacity;
            return this;
        }
        //RangeGradient values
        public RangesBuilder RangeGradient(Action<RangeGradientBuilder> rangegradient)
        {
            var rangeGradient = new List<RangeGradient>();
            ranges.RangeGradient = rangeGradient;
            var builder = new RangeGradientBuilder(this.ranges);
            if (rangegradient != null)
                rangegradient.Invoke(builder);
            return this;
        }

        public void Add()
        {
            scales.Ranges.Add(ranges);
            ranges = new Ranges();
        }
    }
}
