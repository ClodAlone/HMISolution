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
    public class BarPointers
    {
        #region Fields

        //String Values
        private String backgroundColor = null;
        private String borderColor = null;
        //Interger Values
        private int distanceFromScale = 0;
        private int pointerWidth = 30;
        private int value = 0;
        //Double Values
        private double borderWidth = 1.5;
        private double opacity = 1;
        //Object values
        private List<ScaleBarGradient> scaleBarGradient = new List<ScaleBarGradient>();
        #endregion

        #region Properties

        //String Values
        [JsonProperty("backgroundColor")]
        [DefaultValue(null)]
        public String BarPointerBackgroundColor
        {
            get { return this.backgroundColor; }
            set { this.backgroundColor = value; }
        }
        [JsonProperty("borderColor")]
        [DefaultValue(null)]
        public String BarPointerBorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }
        //Integer values
        [JsonProperty("value")]
        [DefaultValue(0)]
        public int BarPointerValue
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("distanceFromScale")]
        [DefaultValue(0)]
        public int BarPointerdistanceFromScale
        {
            get { return this.distanceFromScale; }
            set { this.distanceFromScale = value; }
        }
        [JsonProperty("pointerWidth")]
        [DefaultValue(30)]
        public int BarPointerWidth
        {
            get { return this.pointerWidth; }
            set { this.pointerWidth = value; }
        }
        //Double Values
        [JsonProperty("borderWidth")]
        [DefaultValue(1.5)]
        public double BarPointerBorderWidth
        {
            get { return this.borderWidth; }
            set { this.borderWidth = value; }
        }
        [JsonProperty("opacity")]
        [DefaultValue(1.0)]
        public double BarPointerOpacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
        }
        //Object values
        [JsonProperty("scaleBarGradient")]
        public List<ScaleBarGradient> ScaleBarGradient
        {
            get { return this.scaleBarGradient; }
            set { this.scaleBarGradient = value; }
        }
        #endregion
        #region ShouldSerialize Methods
        public bool ShouldSerializeScaleBarGradient()
        {
            if (ScaleBarGradient.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
namespace Syncfusion.JavaScript.DataVisualization{
    public class BarPointerBuilder
    {
        private List<BarPointers> barPointers = new List<BarPointers>();
        BarPointers barPointer = new BarPointers();
        Scales scales;
        public BarPointerBuilder(Scales barPointer)
        {
            this.scales = barPointer;
            this.barPointers = barPointer.BarPointers;
        }
        //String Values
        public BarPointerBuilder BarPointerBackgroundColor(String backgroundColor)
        {
            barPointer.BarPointerBackgroundColor = backgroundColor;
            return this;
        }
        public BarPointerBuilder BarPointerBorderColor(String borderColor)
        {
            barPointer.BarPointerBorderColor = borderColor;
            return this;
        }
        //Integers
        public BarPointerBuilder BarPointerDistanceFromScale(int distanceFromScale)
        {
            barPointer.BarPointerdistanceFromScale = distanceFromScale;
            return this;
        }
        public BarPointerBuilder BarPointerWidth(int pointerWidth)
        {
            barPointer.BarPointerWidth = pointerWidth;
            return this;
        }
        public BarPointerBuilder BarPointerValue(int value)
        {
            barPointer.BarPointerValue = value;
            return this;
        }
        //Double Values
        public BarPointerBuilder BarPointerBorderWidth(double borderWidth)
        {
            barPointer.BarPointerBorderWidth = borderWidth;
            return this;
        }
        public BarPointerBuilder BarPointerOpacity(double opacity)
        {
            barPointer.BarPointerOpacity = opacity;
            return this;
        }
        //ScaleBarGradient values
        public BarPointerBuilder ScaleBarGradient(Action<ScaleBarGradientBuilder> scalebargradient)
        {
            var scalebarGradient = new List<ScaleBarGradient>();
            barPointer.ScaleBarGradient = scalebarGradient;
            var builder = new ScaleBarGradientBuilder(this.barPointer);
            if (scalebargradient != null)
                scalebargradient.Invoke(builder);
            return this;
        }
        public void Add()
        {
            scales.BarPointers.Add(barPointer);
            barPointer= new BarPointers();
        }
    }
}
