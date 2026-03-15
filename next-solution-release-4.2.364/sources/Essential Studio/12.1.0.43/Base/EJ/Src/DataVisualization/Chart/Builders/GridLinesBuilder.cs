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
    public class MajorGridLinesBuilder
    {
        private MajorGridLines m_gridLine = null;
        public MajorGridLinesBuilder(MajorGridLines gridLine)
        {
            this.m_gridLine = gridLine;
        }

        public MajorGridLinesBuilder Color(string color)
        {
            this.m_gridLine.Color = color;
            return this;
        }
        public MajorGridLinesBuilder Opacity(double opacity)
        {
            this.m_gridLine.Opacity = opacity;
            return this;
        }

        public MajorGridLinesBuilder DashArray(string dasharray)
        {
            this.m_gridLine.DashArray = dasharray;
            return this;
        }


        public MajorGridLinesBuilder Width(double width)
        {
            this.m_gridLine.Width = width;
            return this;
        }


        public MajorGridLinesBuilder Visible(bool visible)
        {
            this.m_gridLine.Visible = visible;
            return this;
        }

        public MajorGridLinesBuilder Offset(int offset)
        {
            this.m_gridLine.Offset = offset;
            return this;
        }
    }
    public class MinorGridLinesBuilder
    {
        private MinorGridLines m_gridLine = null;
        public MinorGridLinesBuilder(MinorGridLines gridLine)
        {
            this.m_gridLine = gridLine;
        }

        public MinorGridLinesBuilder Color(string color)
        {
            this.m_gridLine.Color = color;
            return this;
        }
        public MinorGridLinesBuilder Opacity(double opacity)
        {
            this.m_gridLine.Opacity = opacity;
            return this;
        }

        public MinorGridLinesBuilder DashArray(string dasharray)
        {
            this.m_gridLine.DashArray = dasharray;
            return this;
        }


        public MinorGridLinesBuilder Width(double width)
        {
            this.m_gridLine.Width = width;
            return this;
        }


        public MinorGridLinesBuilder Visible(bool visible)
        {
            this.m_gridLine.Visible = visible;
            return this;
        }
    }
    public class MajorTicksBuilder
    {
        private MajorTicks m_Tick = null;
        public MajorTicksBuilder(MajorTicks ticks)
        {
            this.m_Tick = ticks;
        }

        public MajorTicksBuilder Visible(bool visible)
        {
            this.m_Tick.Visible = visible;
            return this;
        }


        public MajorTicksBuilder Width(double width)
        {
            this.m_Tick.Width = width;
            return this;
        }


        public MajorTicksBuilder Size(int size)
        {
            this.m_Tick.Size = size;
            return this;
        }


        public MajorTicksBuilder Color(string color)
        {
            this.m_Tick.Color = color;
            return this;
        }
    }
    public class MinorTicksBuilder
    {
        private MinorTicks m_Tick = null;
        public MinorTicksBuilder(MinorTicks ticks)
        {
            this.m_Tick = ticks;
        }

        public MinorTicksBuilder Visible(bool visible)
        {
            this.m_Tick.Visible = visible;
            return this;
        }


        public MinorTicksBuilder Width(double width)
        {
            this.m_Tick.Width = width;
            return this;
        }


        public MinorTicksBuilder Size(int size)
        {
            this.m_Tick.Size = size;
            return this;
        }


        public MinorTicksBuilder Color(string color)
        {
            this.m_Tick.Color = color;
            return this;
        }
    }
}
