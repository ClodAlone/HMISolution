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
    public class ChartFontBuilder
    {
        private ChartFont m_font;
       
        public ChartFontBuilder(ChartFont style)
        {
            this.m_font = style;
        }

        public ChartFontBuilder Size(string size)
        {
            this.m_font.FontSize = size;
            return this;
        }
        
            
        public ChartFontBuilder Color(string color)
        {
            this.m_font.Color = color;
            return this;
        }
        
        public ChartFontBuilder Style(ChartFontStyle style)
        {
            this.m_font.FontStyle = style;
            return this;
        }
       
        public ChartFontBuilder FontFamily(string family)
        {
            this.m_font.FontFamily = family;
            return this;
        }

        public ChartFontBuilder FontWeight(ChartFontWeight fontweight)
        {
            this.m_font.FontWeight = fontweight;
            return this;
        }
      
        public ChartFontBuilder Opacity(double opacity)
        {
            this.m_font.Opacity = opacity;
            return this;
        }
    }

    public class ChartStyleBuilder
    {
        private ChartStyle m_style=null;

        public ChartStyleBuilder(ChartStyle style)
        {
            this.m_style = style;
        }

        public ChartStyleBuilder Opacity(double opacity)
        {
            this.m_style.Opacity = opacity;
            return this;
        }
        public ChartStyleBuilder BorderColor(string bordercolor)
        {
            this.m_style.BorderColor = bordercolor;
            return this;
        }
        public ChartStyleBuilder BorderWidth(double borderwidth)
        {
            this.m_style.BorderWidth = borderwidth;
            return this;
        }
        public ChartStyleBuilder Interior(string interior)
        {
            this.m_style.Fill = interior;
            return this;
        }
        public ChartStyleBuilder DashArray(string dashArray)
        {
            this.m_style.DashArray = dashArray;
            return this;
        }
    }
    public class ChartBorderBuilder
    {
        private ChartBorder m_border = null;
        public ChartBorderBuilder(ChartBorder border)
        {
            this.m_border = border;
        }
        public ChartBorderBuilder Color(string interior)
        {
            this.m_border.Color = interior;
            return this;
        }
        public ChartBorderBuilder Width(double width)
        {
            this.m_border.Width = width;
            return this;
        }
        
    }
}
