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
using System.Drawing;
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;


namespace Syncfusion.JavaScript.DataVisualization
{
    public class NavigatorSeriesBuilder  
    {
        private Series m_Series = new Series ();
           
        private RangeNavigatorProperties m_model;
       
        private List<Series> seriesCollection; 
         public NavigatorSeriesBuilder(Series  options)
        {
            this.m_Series = options;
        }
         public NavigatorSeriesBuilder(Series options, RangeNavigator chart)
         {
             this.m_Series = options;
             this.m_model = chart.RangeNavigatorModel;

             seriesCollection = new List<Series>();
             this.m_model.Series = new List<Series>();
         }
         
         public void Add()
         {
             this.m_model.Series.Add(m_Series);
             this.seriesCollection.Add(m_Series);
             m_Series = new Series();

         }
          
         public NavigatorSeriesBuilder Type(SeriesType m_type)
         {
             this.m_Series.Type = m_type;
             return this;
         }
         public NavigatorSeriesBuilder Marker(Action<NavigatorMarkerBuilder> marker)
         {
             var obj = new Marker();
             this.m_Series.Marker = obj;
             var builder = new NavigatorMarkerBuilder(obj);
             if (marker != null)
                 marker.Invoke(builder);
             return this;
         }
         public NavigatorSeriesBuilder DataSource(Action<ChartDataSourceBuilder> dataSource)
        {
            var obj = new ChartDataSource();
            this.m_Series.DataSource = obj;
            var builder = new ChartDataSourceBuilder(obj);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
         public NavigatorSeriesBuilder Points(Action<NavigatorPointsBuilder> points)
        {
            var obj = new Points();
            
            var builder = new NavigatorPointsBuilder(obj,m_Series);
            if (points != null)
                points.Invoke(builder);
            return this;
        }
         public NavigatorSeriesBuilder Series(List<Points> points)
         {
             m_Series.Points = points;
             return this;
         }

         public NavigatorSeriesBuilder BearFillColor(string bearFillColor)
         {
             this.m_Series.BearFillColor = bearFillColor;
             return this;
         }
         public NavigatorSeriesBuilder BullFillColor(string bullFillColor)
         {
             this.m_Series.BullFillColor = bullFillColor;
             return this;
         }
         public NavigatorSeriesBuilder DrawMode(SeriesDrawMode drawMode)
         {
             this.m_Series.DrawMode = drawMode;
             return this;
         }
         public NavigatorSeriesBuilder LineJoin(ChartLineJoin lineJoin)
         {
             this.m_Series.LineJoin = lineJoin;
             return this;
         }
         public NavigatorSeriesBuilder LineCap(ChartLineCap lineCap)
         {
             this.m_Series.LineCap = lineCap;
             return this;
         }
         public NavigatorSeriesBuilder Border(Action<ChartBorderBuilder> font)
         {
             var obj = new ChartBorder();
             this.m_Series.Border = obj;
             var builder = new ChartBorderBuilder(obj);
             if (font != null)
                 font.Invoke(builder);
             return this;
         }
         public NavigatorSeriesBuilder Fill(string fill)
         {
             this.m_Series.Fill = fill;
             return this;
         }
         public NavigatorSeriesBuilder Opacity(double opacity)
         {
             this.m_Series.Opacity = opacity;
             return this;
         }
         public NavigatorSeriesBuilder Width(double width)
         {
             this.m_Series.Width = width;
             return this;
         }
         public NavigatorSeriesBuilder DashArray(string dashArray)
         {
             this.m_Series.DashArray = dashArray;
             return this;
         }
         public NavigatorSeriesBuilder Font(Action<ChartFontBuilder> font)
        {
            var obj = new ChartFont();
            this.m_Series.Font = obj;
            var builder = new ChartFontBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
         
       
          
         public NavigatorSeriesBuilder Animation(bool animation)
        {
            this.m_Series.Animation = animation;
            return this;
        }
         
         public NavigatorSeriesBuilder Name(string name)
        {
            this.m_Series.Name = name;
            return this;
        }
         
         
    }

    
    
    public class NavigatorMarkerBuilder
    {
        private Marker marker;
        public NavigatorMarkerBuilder(Marker marker)
        {
            this.marker = marker;
        }
        
       
        public NavigatorMarkerBuilder Visible(bool Visible)
        {
            this.marker.Visible = Visible;
            return this;
        }
        
        public NavigatorMarkerBuilder Shape(ChartShape Shape)
        {
            this.marker.Shape = Shape;
            return this;
        }

        public NavigatorMarkerBuilder Size(Action<SizeBuilder> size)
        {
            var obj = new ChartSize();
            this.marker.Size = obj;
            var builder = new SizeBuilder(obj);
            if (size != null)
                size.Invoke(builder);
            return this;
        }


    }


    public class NavigatorGridLinesBuilder
    {
        private NavigatorGridLines m_gridLine = null;
        public NavigatorGridLinesBuilder(NavigatorGridLines gridLine)
        {
            this.m_gridLine = gridLine;
        }

        public NavigatorGridLinesBuilder Color(string color)
        {
            this.m_gridLine.Color = color;
            return this;
        }
        public NavigatorGridLinesBuilder Opacity(double opacity)
        {
            this.m_gridLine.Opacity = opacity;
            return this;
        }

        public NavigatorGridLinesBuilder DashArray(string dasharray)
        {
            this.m_gridLine.DashArray = dasharray;
            return this;
        }


        public NavigatorGridLinesBuilder Width(double width)
        {
            this.m_gridLine.Width = width;
            return this;
        }


        public NavigatorGridLinesBuilder Visible(bool visible)
        {
            this.m_gridLine.Visible = visible;
            return this;
        }

        public NavigatorGridLinesBuilder Offset(int offset)
        {
            this.m_gridLine.Offset = offset;
            return this;
        }
    }
   
    public class ValueAxisSettingsBuilder
    {
        private ValueAxisSettings m_Axis;
        public ValueAxisSettingsBuilder(ValueAxisSettings marker)
        {
            this.m_Axis = marker;
        }

         public ValueAxisSettingsBuilder RangePadding(string rangePadding)
         {
             this.m_Axis.RangePadding = rangePadding;
             return this;
         }
         public ValueAxisSettingsBuilder Range(Action<RangeBuilder> range)
         {
             var obj = new Range();
             this.m_Axis.Range = obj;
             var builder = new RangeBuilder(obj);
             if (range != null)
                 range.Invoke(builder);
             return this;
         }
         public ValueAxisSettingsBuilder MajorTickLines(Action<MajorTicksBuilder> majorticklines)
         {

             var obj = new MajorTicks();
             this.m_Axis.MajorTickLines = obj;
             var builder = new MajorTicksBuilder(obj);
             if (majorticklines != null)
                 majorticklines.Invoke(builder);
             return this;
         }
         public ValueAxisSettingsBuilder MajorGridLines(Action<NavigatorGridLinesBuilder> gridLine)
         {
             var obj = new NavigatorGridLines();
             this.m_Axis.MajorGridLines = obj;
             var builder = new NavigatorGridLinesBuilder(obj);
             if (gridLine != null)
                 gridLine.Invoke(builder);
             return this;

         }
         public ValueAxisSettingsBuilder AxisLine(Action<NavigatorGridLinesBuilder> axisline)
         {
             var obj = new NavigatorGridLines();
             this.m_Axis.AxisLine = obj;
             var builder = new NavigatorGridLinesBuilder(obj);
             if (axisline != null)
                 axisline.Invoke(builder);
             return this;
         }
         public ValueAxisSettingsBuilder Font(Action<ChartFontBuilder> font)
         {
             var obj = new ChartFont();
             this.m_Axis.Font = obj;
             var builder = new ChartFontBuilder(obj);
             if (font != null)
                 font.Invoke(builder);
             return this;
         }


    }
     
   
    public class NavigatorPointsBuilder 
    {
        private Points Points=new Points();
        private Series m_series;
        private List<Points> pointsCollection; 
        public NavigatorPointsBuilder(Points points)
        {
            this.Points = points;
        }
        public NavigatorPointsBuilder(Points options, Series series)
         {
             this.Points = options;
             this.m_series = series;

             pointsCollection = new List<Points>();
             this.m_series.Points = new List<Points>();
         }
         public void Add()
         {
             this.m_series.Points.Add(Points);
             this.pointsCollection.Add(Points);
             Points = new Points();

         }
         public void Add(object x,object y)
         {
             this.m_series.Points.Add(new Points(x,y));
             this.pointsCollection.Add(new Points(x, y));
         }
         public void Add(object x, object high,object low)
         {
             this.m_series.Points.Add(new Points(x, high,low));
             this.pointsCollection.Add(new Points(x, high,low));
         }
         public void Add(object x, object high,object low,object open,object close)
         {
             this.m_series.Points.Add(new Points(x, high,low,open,close));
             this.pointsCollection.Add(new Points(x, high, low, open, close));
         }
        public NavigatorPointsBuilder X(object X)
        {
             this.Points.X = X;
            return this;
        }
        public NavigatorPointsBuilder High(object high)
        {
            this.Points.High = high;
            return this;
        }
        public NavigatorPointsBuilder Low(object low)
        {
            this.Points.Low = low;
            return this;
        }
        public NavigatorPointsBuilder Close(object close)
        {
            this.Points.Close = close;
            return this;
        }
        public NavigatorPointsBuilder Open(object open)
        {
            this.Points.Open = open;
            return this;
        }
        
		  public NavigatorPointsBuilder Size(int size)
        {
            this.Points.Size = size;
            return this;
        }
        public NavigatorPointsBuilder Y(object Y)
        {
            this.Points.Y = Y;
            return this;
        }
          
        
        public NavigatorPointsBuilder Text(string Text)
        {
            this.Points.Text = Text;
            return this;
        }
        
        public NavigatorPointsBuilder Font(object font)
        {
            this.Points.Font = font;
            return this;
        }
       

    }
}