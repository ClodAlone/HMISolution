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
    public class NavigatorStyleBuilder
    {
        private NavigatorStyle m_navigatorStyle;
        public NavigatorStyleBuilder(NavigatorStyle range)
        {
            this.m_navigatorStyle = range;
        }
        public NavigatorStyleBuilder SelectedRegionColor(string selectedRegionColor)
        {
            this.m_navigatorStyle.SelectedRegionColor = selectedRegionColor;
            return this;
        }
        public NavigatorStyleBuilder UnselectedRegionColor(string unselectedRegionColor)
        {
            this.m_navigatorStyle.UnselectedRegionColor = unselectedRegionColor;
            return this;
        }
        public NavigatorStyleBuilder ThumbColor(string thumbColor)
        {
            this.m_navigatorStyle.ThumbColor = thumbColor;
            return this;
        }
        public NavigatorStyleBuilder ThumbRadius(int thumbRadius)
        {
            this.m_navigatorStyle.ThumbRadius = thumbRadius;
            return this;
        }
        public NavigatorStyleBuilder Background(string background)
        {
            this.m_navigatorStyle.Background = background;
            return this;
        }
        public NavigatorStyleBuilder Border(Action<LineStyleBuilder> label)
        {
            var obj = new LineStyle();
            this.m_navigatorStyle.Border = obj;
            var builder = new LineStyleBuilder(obj);
            if (label != null)
                label.Invoke(builder);
            return this;

        }
        public NavigatorStyleBuilder MajorGridLineStyle(Action<GridLineStyleBuilder> label)
        {
            var obj = new GridLineStyle();
            this.m_navigatorStyle.MajorGridLineStyle = obj;
            var builder = new GridLineStyleBuilder(obj);
            if (label != null)
                label.Invoke(builder);
            return this;

        }
        public NavigatorStyleBuilder MinorGridLineStyle(Action<GridLineStyleBuilder> label)
        {
            var obj = new GridLineStyle();
            this.m_navigatorStyle.MinorGridLineStyle = obj;
            var builder = new GridLineStyleBuilder(obj);
            if (label != null)
                label.Invoke(builder);
            return this;

        }
    }
    public class GridLineStyleBuilder
    {
        private GridLineStyle mLineStyle;
        public GridLineStyleBuilder(GridLineStyle options)
        {
            this.mLineStyle = options;
        }
        public GridLineStyleBuilder Color(string color)
        {
            this.mLineStyle.Color = color;
            return this;
        }
        public GridLineStyleBuilder Visible(bool visible)
        {
            this.mLineStyle.Visible = visible;
            return this;
        }
    }
}
