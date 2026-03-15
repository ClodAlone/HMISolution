#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization 
{
    public class TooltipBuilder
    {
        private Tooltip  m_tooltip;
        public TooltipBuilder(Tooltip tooltip)
        {
            this.m_tooltip = tooltip;
        }
        public TooltipBuilder BackgroundColor(string backgroundColor)
        {
            this.m_tooltip.BackgroundColor = backgroundColor;
            return this;
        }
        public TooltipBuilder Visible(bool visible)
        {
            this.m_tooltip.Visible = visible;
            return this;
        }
        public TooltipBuilder TooltipDisplayMode(string tooltipDisplayMode)
        {
            this.m_tooltip.TooltipDisplayMode = tooltipDisplayMode;
            return this;
        }
        public TooltipBuilder TooltipPosition(string labelFormat)
        {
            this.m_tooltip.TooltipPosition = labelFormat;
            return this;
        }
        public TooltipBuilder LabelFormat(string labelFormat)
        {
            this.m_tooltip.LabelFormat = labelFormat;
            return this;
        }
        public TooltipBuilder LabelStyles(Action<LabelStylesBuilder> lineStyle)
        {
            var obj = new LabelStyles();
            this.m_tooltip.Labelstyles = obj;
            var builder = new LabelStylesBuilder(obj);
            if (lineStyle != null)
                lineStyle.Invoke(builder);
            return this;

        }
    }
}
