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
    public class Pointers
    {

        #region fields

        //Integer values
        private int distanceFromScale= 0;        
        private int backNeedleLength= 10;
        private int pointerLength= 150;       
        private int pointerWidth= 7;                
        private int value= 0;

        //Double values
        private double  opacity = 1;
        private double borderWidth= 1.5;

        //Enumeration values
        private PointerPositions pointerPosition = PointerPositions.Near;
        private PointerTypes pointerType = PointerTypes.Needle;
        private NeedleStyles needleStyle = NeedleStyles.Triangle;
        private MarkerStyles markerStyle = MarkerStyles.Rectangle;

        //String values        
        private string borderColor= null;
        private string backgroundColor= null;

        //Boolean values
        private bool showBackNeedle= false;

        //Object values
        private List<CircularPointerGradient> pointerGradient = new List<CircularPointerGradient>();

        #endregion

        #region Properties
        //Boolean values
        [JsonProperty("showBackNeedle")]
        [DefaultValue(false)]
        public bool ShowBackNeedle
        {
            get { return this.showBackNeedle; }
            set { this.showBackNeedle = value; }
        }
        
        //String Values        
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
        //Enumeration Values
        [JsonProperty("pointerPosition")]
        [DefaultValue(PointerPositions.Near)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PointerPositions PointerPosition
        {
            get { return this.pointerPosition; }
            set { this.pointerPosition = value; }
        }
        [JsonProperty("needleStyle")]
        [DefaultValue(NeedleStyles.Triangle)]
        [JsonConverter(typeof(StringEnumConverter))]
        public NeedleStyles NeedleStyle
        {
            get { return this.needleStyle; }
            set { this.needleStyle = value; }
        }
        [JsonProperty("pointerType")]
        [DefaultValue(PointerTypes.Needle)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PointerTypes PointerType
        {
            get { return this.pointerType; }
            set { this.pointerType = value; }
        }
        [JsonProperty("markerStyle")]
        [DefaultValue(MarkerStyles.Rectangle)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MarkerStyles MarkerStyle
        {
            get { return this.markerStyle; }
            set { this.markerStyle = value; }
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
        //Integer values               
        [JsonProperty("distanceFromScale")]
        [DefaultValue(0)]
        public int DistanceFromScale
        {
            get { return this.distanceFromScale; }
            set { this.distanceFromScale = value; }
        }
        [JsonProperty("backNeedleLength")]
        [DefaultValue(10)]
        public int BackNeedleLength
        {
            get { return this.backNeedleLength; }
            set { this.backNeedleLength = value; }
        }
        [JsonProperty("pointerLength")]
        [DefaultValue(150)]
        public int PointerLength
        {
            get { return this.pointerLength; }
            set { this.pointerLength = value; }
        }
        [JsonProperty("pointerWidth")]
        [DefaultValue(7)]
        public int PointerWidth
        {
            get { return this.pointerWidth; }
            set { this.pointerWidth = value; }
        }        
        [JsonProperty("value")]
        [DefaultValue(0)]
        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        //Object values
        [JsonProperty("pointerGradient")]
        public List<CircularPointerGradient> PointerGradient
        {
            get { return this.pointerGradient; }
            set { this.pointerGradient = value; }
        }

        #endregion

        #region ShouldSerialize Methods
        public bool ShouldSerializePointerGradient()
        {
            if (PointerGradient.Count != 0)
                return true;
            else
                return false;
        }       
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization
{
        public class PointersBuilder
        {
            private List<Pointers> pointer = new List<Pointers>();
            Pointers pointers = new Pointers();  
            CircularScales scales;                            
            public PointersBuilder(CircularScales pointers)
            {
                this.scales = pointers;
                this.pointer = pointers.Pointers;
            }
            //String Values           
            public PointersBuilder BackgroundColor(String backgroundColor)
            {
                pointers.BackgroundColor = backgroundColor;
                return this;
            }
            public PointersBuilder BorderColor(String borderColor)
            {
                pointers.BorderColor = borderColor;
                return this;
            }
            //Double values
            public PointersBuilder Opacity(double opacity)
            {
                pointers.Opacity = opacity;
                return this;
            }
            public PointersBuilder BorderWidth(double borderWidth)
            {
                pointers.BorderWidth = borderWidth;
                return this;
            }
            //Integers
            public PointersBuilder PointerLength(int pointerLength)
            {
                pointers.PointerLength = pointerLength;
                return this;
            }
            public PointersBuilder DistanceFromScale(int distanceFromScale)
            {
                pointers.DistanceFromScale = distanceFromScale;
                return this;
            }
            public PointersBuilder PointerWidth(int pointerWidth)
            {
                pointers.PointerWidth = pointerWidth;
                return this;
            }
            public PointersBuilder Value(int value)
            {
                pointers.Value = value;
                return this;
            }            
            public PointersBuilder BackNeedleLength(int backNeedleLength)
            {
                pointers.BackNeedleLength = backNeedleLength;
                return this;
            }                        
            //BoolValues
            public PointersBuilder ShowBackNeedle()
            {
                pointers.ShowBackNeedle = true;
                return this;
            }
            public PointersBuilder ShowBackNeedle(bool showBackNeedle)
            {
                pointers.ShowBackNeedle = showBackNeedle;
                return this;
            }
            //EnumValues
            public PointersBuilder MarkerStyle(MarkerStyles markerStyle)
            {
                pointers.MarkerStyle = markerStyle;
                return this;
            }
            public PointersBuilder PointerType(PointerTypes pointerType)
            {
                pointers.PointerType = pointerType;
                return this;
            }
            public PointersBuilder NeedleStyle(NeedleStyles needleStyle)
            {
                pointers.NeedleStyle = needleStyle;
                return this;
            }
            public PointersBuilder PointerPosition(PointerPositions pointerPosition)
            {
                pointers.PointerPosition = pointerPosition;
                return this;
            }
            // PointerGradient Values           
            public PointersBuilder PointerGradient(Action<CircularPointerGradientBuilder> pointerGradient)
            {
                var pointerGradients = new List<CircularPointerGradient>();
                pointers.PointerGradient = pointerGradients;
                var builder = new CircularPointerGradientBuilder(this.pointers);
                if (pointerGradient != null)
                    pointerGradient.Invoke(builder);
                return this;
            }
            public void Add()
            {
                scales.Pointers.Add(pointers);                
                pointers = new Pointers();
            }
        
    }
}
