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
    public class ShapeSettingBuilder
    {
        public ShapeSetting shapeSetting;

        public ShapeFileLayer shapeFileLayer;
        
        public ShapeSettingBuilder(ShapeSetting shapeSetting, ShapeFileLayer shapeFileLayer)
        {
            this.shapeSetting = shapeSetting;
            this.shapeFileLayer = shapeFileLayer;
            this.shapeFileLayer.ShapeSetting = this.shapeSetting;
        }

        public ShapeSettingBuilder MouseHoverColor(string mouseHoverColor)
        {
            this.shapeSetting.MouseHoverColor = mouseHoverColor;
            return this;
        }

        public ShapeSettingBuilder MouseHoverWidth(double mouseHoverWidth)
        {
            this.shapeSetting.MouseHoverWidth = mouseHoverWidth;
            return this;
        }

        public ShapeSettingBuilder ShapeSelectionColor(string shapeSelectionColor)
        {
            this.shapeSetting.ShapeSelectionColor = shapeSelectionColor;
            return this;
        }

        public ShapeSettingBuilder ShapeFill(string shapeFill)
        {
            this.shapeSetting.ShapeFill = shapeFill;
            return this;
        }

        public ShapeSettingBuilder ShapeStrokeThickness(double shapeStrokeThickness)
        {
            this.shapeSetting.ShapeStrokeThickness = shapeStrokeThickness;
            return this;
        }

        public ShapeSettingBuilder ShapeSelectionStrokeWidth(double shapeSelectionStrokeWidth)
        {
            this.shapeSetting.ShapeSelectionStrokeWidth = shapeSelectionStrokeWidth;
            return this;
        }

        public ShapeSettingBuilder ShapeStroke(string shapeStroke)
        {
            this.shapeSetting.ShapeStroke = shapeStroke;
            return this;
        }

        public ShapeSettingBuilder ShapeSelectionStroke(string shapeSelectionStroke)
        {
            this.shapeSetting.ShapeSelectionStroke = shapeSelectionStroke;
            return this;
        }

        public ShapeSettingBuilder MouseHoverStroke(string mouseHoverStroke)
        {
            this.shapeSetting.MouseHoverStroke = mouseHoverStroke;
            return this;
        }

        public ShapeSettingBuilder ShapeColorValuepath(string shapeColorValuepath)
        {
            this.shapeSetting.ShapeColorValuepath = shapeColorValuepath;
            return this;
        }

        public ShapeSettingBuilder ShapeValuepath(string shapeValuepath)
        {
            this.shapeSetting.ShapeValuepath = shapeValuepath;
            return this;
        }

        public ShapeSettingBuilder EqualColorMappings(Action<EqualColorMappingBuilder> colorMapping)
        {
            var obj = this.shapeSetting.ColorMappings;

            if (obj == null)
            {
                this.shapeSetting.ColorMappings = new ColorMapping();
                obj = this.shapeSetting.ColorMappings;
            }

            var builder = new EqualColorMappingBuilder(this.shapeSetting.ColorMappings);
            if (colorMapping != null)
                colorMapping.Invoke(builder);
            return this;
        }

        public ShapeSettingBuilder RangeColorMappings(Action<RangeColorMappingBuilder> colorMapping)
        {
            var obj = this.shapeSetting.ColorMappings;

            if (obj == null)
            {
                this.shapeSetting.ColorMappings = new ColorMapping();
                obj = this.shapeSetting.ColorMappings;
            }

            var builder = new RangeColorMappingBuilder(this.shapeSetting.ColorMappings);

            if (colorMapping != null)
                colorMapping.Invoke(builder);
            return this;
        }

        public ShapeSettingBuilder AutoFill(bool autoFill)
        {
            this.shapeSetting.AutoFill = autoFill;
            return this;
        }

        public ShapeSettingBuilder EnableGradient(bool enableGradient)
        {
            this.shapeSetting.EnableGradient = enableGradient;
            return this;
        }

        public ShapeSettingBuilder ShapeColorPalette(ColorPalette shapeColorPalette)
        {
            this.shapeSetting.ShapeColorPalette = shapeColorPalette;
            return this;
        }

        public ShapeSettingBuilder CustomPalette(List<string> customPalette)
        {
            this.shapeSetting.CustomPalette = customPalette;
            return this;
        }
    }
}
