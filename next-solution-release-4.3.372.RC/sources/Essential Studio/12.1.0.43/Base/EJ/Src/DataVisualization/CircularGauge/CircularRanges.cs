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
    public class CircularRanges
    {

        #region fields

        //Integer values
        private int distanceFromScale = 25;
        private int size = 5;
        private int startWidth = 10;
        private int endWidth = 10;
        
        //Double values
        private double borderWidth = 1.5;
        private double opacity = 1;

        //Object values
        private List<CircularRangeGradient> rangeGradient = new List<CircularRangeGradient>();

        //String values            
        private string backgroundColor = "#32b3c6";
        private string borderColor = "#32b3c6";
        private string startValue = null;
        private string endValue = null; 

        //Enumeration values
        private RangePositions rangePosition = RangePositions.Near;

        #endregion

        #region Properties
        //Integer values
        [JsonProperty("distanceFromScale")]
        [DefaultValue(25)]
        public int DistanceFromScale
        {
            get { return this.distanceFromScale; }
            set { this.distanceFromScale = value; }
        }
        [JsonProperty("size")]
        [DefaultValue(5)]
        public int Size
        {
            get { return this.size; }
            set { this.size = value; }
        }
        [JsonProperty("startWidth")]
        [DefaultValue(10)]
        public int StartWidth
        {
            get { return this.startWidth; }
            set { this.startWidth = value; }
        }
        [JsonProperty("endWidth")]
        [DefaultValue(10)]
        public int EndWidth
        {
            get { return this.endWidth; }
            set { this.endWidth = value; }
        }
        [JsonProperty("startValue")]
        [DefaultValue(null)]
        public string StartValue
        {
            get { return this.startValue; }
            set { this.startValue = value; }
        }
        [JsonProperty("endValue")]
        [DefaultValue(null)]
        public string EndValue
        {
            get { return this.endValue; }
            set { this.endValue = value; }
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
        [DefaultValue(1)]
        public double Opacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
        }
        //Object values
        [JsonProperty("rangeGradient")]
        public List<CircularRangeGradient> RangeGradient
        {
            get { return this.rangeGradient; }
            set { this.rangeGradient = value; }
        }

        //String Values       
        [JsonProperty("borderColor")]
        [DefaultValue("#32b3c6")]
        public String BorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }
        [JsonProperty("backgroundColor")]
        [DefaultValue("#32b3c6")]
        public String BackgroundColor
        {
            get { return this.backgroundColor; }
            set { this.backgroundColor = value; }
        }

        //Enumeration Values
        [JsonProperty("rangePosition")]
        [DefaultValue(RangePositions.Near)]
        [JsonConverter(typeof(StringEnumConverter))]
        public RangePositions RangePosition
        {
            get { return this.rangePosition; }
            set { this.rangePosition = value; }
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
        public class CircularRangesBuilder
        {

            CircularScales scales;
            private List<CircularRanges> range = new List<CircularRanges>();
            private CircularRanges ranges = new CircularRanges();
            public CircularRangesBuilder(CircularScales ranges)
            {
                this.scales = ranges;
                this.range = ranges.Ranges;
            }
            //String Values
            public CircularRangesBuilder BorderColor(String borderColor)
            {
                ranges.BorderColor = borderColor;
                return this;
            }            
            public CircularRangesBuilder BackgroundColor(String BackgroundColor)
            {
                ranges.BackgroundColor = BackgroundColor;
                return this;
            }            
            //EnumValues
            public CircularRangesBuilder RangePosition(RangePositions rangePosition)
            {
                ranges.RangePosition = rangePosition;
                return this;
            }
            //Integers
            public CircularRangesBuilder EndWidth(int endWidth)
            {
                ranges.EndWidth = endWidth;
                return this;
            }
            public CircularRangesBuilder EndValue(String endValue)
            {
                ranges.EndValue = endValue;
                return this;
            }
            public CircularRangesBuilder StartValue(String startValue)
            {
                ranges.StartValue = startValue;
                return this;
            }
            public CircularRangesBuilder Size(int size)
            {
                ranges.Size = size;
                return this;
            }
            public CircularRangesBuilder StartWidth(int startWidth)
            {
                ranges.StartWidth = startWidth;
                return this;
            }
            public CircularRangesBuilder DistanceFromScale(int distanceFromScale)
            {
                ranges.DistanceFromScale = distanceFromScale;
                return this;
            }

            //DoubleValues
            public CircularRangesBuilder BorderWidth(double borderWidth)
            {
                ranges.BorderWidth = borderWidth;
                return this;
            }
            public CircularRangesBuilder Opacity(double opacity)
            {                
                ranges.Opacity = opacity;
                return this;
            }
            //RangeGradient values
            public CircularRangesBuilder RangeGradient(Action<CircularRangeGradientBuilder> rangegradient)
            {
                var rangeGradient = new List<CircularRangeGradient>();
                ranges.RangeGradient= rangeGradient;
                var builder = new CircularRangeGradientBuilder(this.ranges);
                if (rangegradient != null)
                    rangegradient.Invoke(builder);
                return this;
            }
            public void Add()
            {
                scales.Ranges.Add(ranges);
                ranges = new CircularRanges();                
            }

        
    }
}
