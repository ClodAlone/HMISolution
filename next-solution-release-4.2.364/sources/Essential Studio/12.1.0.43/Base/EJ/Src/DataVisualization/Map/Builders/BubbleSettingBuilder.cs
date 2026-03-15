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
    public class BubbleSettingBuilder
    {
        public BubbleSetting bubbleSetting;

        public ShapeFileLayer shapeFileLayer;
        
        public BubbleSettingBuilder(BubbleSetting bubbleSetting, ShapeFileLayer layer)
        {
            this.bubbleSetting = bubbleSetting;
            this.shapeFileLayer = layer;
            this.shapeFileLayer.BubbleSetting = this.bubbleSetting;
        }

        public BubbleSettingBuilder MinSize(double minSize)
        {
            this.bubbleSetting.MinSize = minSize;
            return this;
        }

        public BubbleSettingBuilder MaxSize(double maxSize)
        {
            this.bubbleSetting.MaxSize = maxSize;
            return this;
        }

        public BubbleSettingBuilder BubbleColor(string bubbleColor)
        {
            this.bubbleSetting.BubbleColor = bubbleColor;
            return this;
        }

        public BubbleSettingBuilder BubbleColorValuepath(string bubbleColorValuepath)
        {
            this.bubbleSetting.BubbleColorValuepath = bubbleColorValuepath;
            return this;
        }

        public BubbleSettingBuilder BubbleValuepath(string bubbleValuepath)
        {
            this.bubbleSetting.BubbleValuepath = bubbleValuepath;
            return this;
        }

        public BubbleSettingBuilder TooltipTemplate(string tooltipTemplate)
        {
            this.bubbleSetting.TooltipTemplate = tooltipTemplate;
            return this;
        }

        public BubbleSettingBuilder EqualColorMappings(Action<EqualColorMappingBuilder> colorMapping)
        {
            var obj = this.bubbleSetting.ColorMappings;

            if (obj == null)
            {
                this.bubbleSetting.ColorMappings = new ColorMapping();
            }

            var builder = new EqualColorMappingBuilder(this.bubbleSetting.ColorMappings);

            if (colorMapping != null)
                colorMapping.Invoke(builder);
            return this;
        }

        public BubbleSettingBuilder RangeColorMappings(Action<RangeColorMappingBuilder> colorMapping)
        {
            var obj = this.bubbleSetting.ColorMappings;

            if (obj == null)
            {
                this.bubbleSetting.ColorMappings = new ColorMapping();
            }

            var builder = new RangeColorMappingBuilder(this.bubbleSetting.ColorMappings);

            if (colorMapping != null)
                colorMapping.Invoke(builder);
            return this;
        }

        public BubbleSettingBuilder ShowToolTip(bool tooltipVisibility)
        {
            this.bubbleSetting.ShowToolTip = tooltipVisibility;
            return this;
        }
    }
}
