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
    public class NavigatorFontBuilder
    {
        private NavigatorFont m_font;
        public NavigatorFontBuilder(NavigatorFont options)
        {
            this.m_font = options;
        }

        public NavigatorFontBuilder Size(string size)
        {
            this.m_font.FontSize = size;
            return this;
        }


        public NavigatorFontBuilder Color(string color)
        {
            this.m_font.Color = color;
            return this;
        }

        public NavigatorFontBuilder FontStyle(RangeNavigatorFontStyle style)
        {
            this.m_font.FontStyle = style;
            return this;
        }

        public NavigatorFontBuilder FontFamily(string family)
        {
            this.m_font.FontFamily = family;
            return this;
        }

        public NavigatorFontBuilder FontWeight(RangeNavigatorFontWeight fontweight)
        {
            this.m_font.FontWeight = fontweight;
            return this;
        }

        public NavigatorFontBuilder Opacity(double opacity)
        {
            this.m_font.Opacity = opacity;
            return this;
        }
    }
    public class NavigatorSizeBuilder
    {
        private NavigatorSize size;
        public NavigatorSizeBuilder(NavigatorSize size)
        {
            this.size = size;
        }
        public NavigatorSizeBuilder Width(string Width)
        {
            this.size.Width = Width;
            return this;
        }
        public NavigatorSizeBuilder Height(string Height)
        {
            this.size.Height = Height;
            return this;
        }

    }
}
