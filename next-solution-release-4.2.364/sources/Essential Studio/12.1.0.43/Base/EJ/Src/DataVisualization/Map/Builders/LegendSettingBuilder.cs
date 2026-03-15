#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.JavaScript.DataVisualization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class LegendSettingBuilder
    {
        public MapLegendSetting legendSetting;

        public ShapeFileLayer shapeFileLayer;

        public LegendSettingBuilder(MapLegendSetting legendSetting, ShapeFileLayer layer)
        {
            this.legendSetting = legendSetting;
            this.shapeFileLayer = layer;
            this.shapeFileLayer.LegendSetting = this.legendSetting;
        }

        public LegendSettingBuilder ShowLegend(bool legendVisibility)
        {
            this.legendSetting.ShowLegend = legendVisibility;
            return this;
        }

        public LegendSettingBuilder LegendPositionX(double legendPositionX)
        {
            this.legendSetting.LegendPositionX = legendPositionX;
            return this;
        }

        public LegendSettingBuilder LegendWidth(double legendWidth)
        {
            this.legendSetting.LegendWidth = legendWidth;
            return this;
        }

        public LegendSettingBuilder LegendHeight(double legendHeight)
        {
            this.legendSetting.LegendHeight = legendHeight;
            return this;
        }

        public LegendSettingBuilder LegendPositionY(double legendPositionY)
        {
            this.legendSetting.LegendPositionY = legendPositionY;
            return this;
        }

        public LegendSettingBuilder LegendIcon(LegendIcons legendIcon)
        {
            this.legendSetting.LegendIcon = legendIcon;
            return this;
        }

        public LegendSettingBuilder LegendType(string legendType)
        {
            this.legendSetting.LegendType = legendType;
            return this;
        }

        public LegendSettingBuilder LegendTitle(string legendTitle)
        {
            this.legendSetting.LegendTitle = legendTitle;
            return this;
        }

        public LegendSettingBuilder LegendMode(LegendMode legendMode)
        {
            this.legendSetting.LegendMode = legendMode;
            return this;
        }

        public LegendSettingBuilder LegendLeftLabel(string legendLeftLabel)
        {
            this.legendSetting.LegendLeftLabel = legendLeftLabel;
            return this;
        }

        public LegendSettingBuilder LegendRightLabel(string legendRightLabel)
        {
            this.legendSetting.LegendRightLabel = legendRightLabel;
            return this;
        }

        public LegendSettingBuilder LegendPosition(DockPosition LegendPosition)
        {
            this.legendSetting.LegendPosition = LegendPosition;
            return this;
        }
    }
}
