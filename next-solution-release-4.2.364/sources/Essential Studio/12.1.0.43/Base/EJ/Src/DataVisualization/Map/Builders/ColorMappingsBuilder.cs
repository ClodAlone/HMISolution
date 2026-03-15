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
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class RangeColorMappingBuilder
    {
        public ColorMapping colorMapping;

        public RangeColorMapping rangeColorMapping;

        public RangeColorMappingBuilder(ColorMapping colorMapping)
        {
            this.colorMapping = colorMapping;

            if (colorMapping.ColorMappings == null)
            {
                colorMapping.ColorMappings = new List<object>();
            }

            this.rangeColorMapping = new RangeColorMapping();
        }

        public void Add()
        {
            colorMapping.ColorMappings.Add(this.rangeColorMapping);
            this.rangeColorMapping = new RangeColorMapping();
        }

        public RangeColorMappingBuilder From(double value)
        {
            this.rangeColorMapping.From = value;
            return this;
        }

        public RangeColorMappingBuilder To(double value)
        {
            this.rangeColorMapping.To = value;
            return this;
        }

        public RangeColorMappingBuilder Color(string color)
        {
            this.rangeColorMapping.Color = color;
            return this;
        }

        public RangeColorMappingBuilder GradientColors(List<string> colors)
        {
            this.rangeColorMapping.GradientColors = colors;
            return this;
        }
    }

    public class EqualColorMappingBuilder
    {
        public ColorMapping colorMapping;

        public EqualColorMapping equalColorMapping;

        public EqualColorMappingBuilder(ColorMapping colorMapping)
        {
            this.colorMapping = colorMapping;

            if (colorMapping.ColorMappings == null)
            {
                colorMapping.ColorMappings = new List<object>();
            }

            this.equalColorMapping = new EqualColorMapping();
        }

        public void Add()
        {
            colorMapping.ColorMappings.Add(this.equalColorMapping);
            this.equalColorMapping = new EqualColorMapping();
        }

        public EqualColorMappingBuilder Value(object value)
        {
            this.equalColorMapping.Value = value;
            return this;
        }

        public EqualColorMappingBuilder Color(string color)
        {
            this.equalColorMapping.Color = color;
            return this;
        }
    }
}
