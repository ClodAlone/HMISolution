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
    public class MarkerPointers
    {
        #region Fields

        //String Values
        private String backgroundColor = null;
        private String borderColor = null;
        //Interger Values
        private int pointerLength = 30;
        private int distanceFromScale = 0;
        private int pointerWidth = 30;
        private int value = 0;        
        //Double Values
        private double borderWidth = 1.5;
        private double opacity = 1;
        //Enumeration Values
        private MarkerStyles markerStyle = MarkerStyles.Triangle;
        private MarkerPointerPlacements pointerPlacement = MarkerPointerPlacements.Far;
        //Object values
        private List<PointerGradient> pointerGradient = new List<PointerGradient>();
        #endregion

        #region Properties

        //String Values
        [JsonProperty("backgroundColor")]
        [DefaultValue(null)]
        public String MarkerBackgroundColor
        {
            get { return this.backgroundColor; }
            set { this.backgroundColor = value; }
        }
        [JsonProperty("borderColor")]
        [DefaultValue(null)]
        public String MarkerBorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }
        //Integer values
        [JsonProperty("pointerLength")]
        [DefaultValue(30)]
        public int PointerLength
        {
            get { return this.pointerLength; }
            set { this.pointerLength = value; }
        }
        [JsonProperty("distanceFromScale")]
        [DefaultValue(0)]
        public int MarkerdistanceFromScale
        {
            get { return this.distanceFromScale; }
            set { this.distanceFromScale = value; }
        }
        [JsonProperty("value")]
        [DefaultValue(0)]
        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("pointerWidth")]
        [DefaultValue(30)]
        public int PointerWidth
        {
            get { return this.pointerWidth; }
            set { this.pointerWidth = value; }
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
        [DefaultValue(1.0)]
        public double MarkerOpacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
        }
        //Enumeration Values
        [JsonProperty("markerStyle")]
        [DefaultValue(MarkerStyles.Triangle)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MarkerStyles MarkerStyle
        {
            get { return this.markerStyle; }
            set { this.markerStyle = value; }
        }
        [JsonProperty("pointerPlacement")]
        [DefaultValue(MarkerPointerPlacements.Far)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MarkerPointerPlacements MarkerPointerPlacement
        {
            get { return this.pointerPlacement; }
            set { this.pointerPlacement = value; }
        }
        //Object values
        [JsonProperty("pointerGradient")]
        public List<PointerGradient> PointerGradient
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
    public class MarkerPointerBuilder
    {
        private List<MarkerPointers> markerPointers = new List<MarkerPointers>();
        MarkerPointers markerPointer = new MarkerPointers();
        Scales scales;
        public MarkerPointerBuilder(Scales markerPointer)
        {
            this.scales = markerPointer;
            this.markerPointers = markerPointer.MarkerPointers;
        }
        //String Values
        public MarkerPointerBuilder MarkerBackgroundColor(String backgroundColor)
        {
            markerPointer.MarkerBackgroundColor = backgroundColor;
            return this;
        }
        public MarkerPointerBuilder MarkerBorderColor(String borderColor)
        {
            markerPointer.MarkerBorderColor = borderColor;
            return this;
        }
        //Integers
        public MarkerPointerBuilder PointerLength(int pointerLength)
        {
            markerPointer.PointerLength = pointerLength;
            return this;
        }
        public MarkerPointerBuilder MarkerDistanceFromScale(int distanceFromScale)
        {
            markerPointer.MarkerdistanceFromScale = distanceFromScale;
            return this;
        }
        public MarkerPointerBuilder PointerWidth(int pointerWidth)
        {
            markerPointer.PointerWidth = pointerWidth;
            return this;
        }
        public MarkerPointerBuilder Value(int value)
        {
            markerPointer.Value = value;
            return this;
        }
        //Double Values
        public MarkerPointerBuilder BorderWidth(double borderWidth)
        {
            markerPointer.BorderWidth = borderWidth;
            return this;
        }
        public MarkerPointerBuilder MarkerOpacity(double opacity)
        {
            markerPointer.MarkerOpacity = opacity;
            return this;
        }
        //EnumValues
        public MarkerPointerBuilder MarkerStyle(MarkerStyles markerStyle)
        {
            markerPointer.MarkerStyle = markerStyle;
            return this;
        }
        public MarkerPointerBuilder MarkerPointerPlacement(MarkerPointerPlacements pointerPlacement)
        {
            markerPointer.MarkerPointerPlacement = pointerPlacement;
            return this;
        }
        //PointerGradient values
        public MarkerPointerBuilder PointerGradient(Action<PointerGradientBuilder> pointergradient)
        {
            var pointerGradient = new List<PointerGradient>();
            markerPointer.PointerGradient = pointerGradient;
            var builder = new PointerGradientBuilder(this.markerPointer);
            if (pointergradient != null)
                pointergradient.Invoke(builder);
            return this;
        }
        public void Add()
        {
            scales.MarkerPointers.Add(markerPointer);
            markerPointer = new MarkerPointers();
        }
    }
}
