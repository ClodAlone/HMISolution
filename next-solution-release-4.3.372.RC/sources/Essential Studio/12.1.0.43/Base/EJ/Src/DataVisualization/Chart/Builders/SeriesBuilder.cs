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
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class SeriesBuilder  
    {
        private Series m_Series = new Series ();
           
        private ChartProperties m_model;
        private List<Series> seriesCollection; 
         public SeriesBuilder(Series  options)
        {
            this.m_Series = options;
        }
        public SeriesBuilder(Series options,Chart chart)
         {
             this.m_Series = options;
             this.m_model = chart.ChartModel;

             seriesCollection = new List<Series>();
             this.m_model.Series = new List<Series>();
         }
         public void Add()
         {
             this.m_model.Series.Add(m_Series);
             this.seriesCollection.Add(m_Series);
             m_Series = new Series();

         }

         public SeriesBuilder Tooltip(Action<NewTooltipBuilder> tooltip)
        {
             var obj = new NewTooltip();
             this.m_Series.Tooltip = obj;
             var builder = new NewTooltipBuilder(obj);
             if (tooltip != null)
                 tooltip.Invoke(builder);
            return this;
        }
         public SeriesBuilder Visible(bool mvisibility)
         {
             this.m_Series.Visibility = mvisibility;
             return this;
         }
         public SeriesBuilder DataSource(Action<ChartDataSourceBuilder> dataSource)
        {
            var obj = new ChartDataSource();
            this.m_Series.DataSource = obj;
            var builder = new ChartDataSourceBuilder(obj);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
         public SeriesBuilder Points(Action<PointsBuilder> points)
        {
            var obj = new Points();
            
            var builder = new PointsBuilder(obj,m_Series);
            if (points != null)
                points.Invoke(builder);
            return this;
        }
         public SeriesBuilder Points(List<Points> points)
         {
             m_Series.Points = points;
             return this;
         }
         public SeriesBuilder Marker(Action<MarkerBuilder> marker)
        {
            var obj = new Marker();
            this.m_Series.Marker = obj;
            var builder = new MarkerBuilder(obj);
            if (marker != null)
                marker.Invoke(builder);
            return this;
        }
       
         
         
         
         public SeriesBuilder Font(Action<ChartFontBuilder> font)
        {
            var obj = new ChartFont();
            this.m_Series.Font = obj;
            var builder = new ChartFontBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        
         public SeriesBuilder BearFillColor(string bearFillColor)
         {
             this.m_Series.BearFillColor = bearFillColor;
             return this;
         }
         public SeriesBuilder BullFillColor(string bullFillColor)
         {
             this.m_Series.BullFillColor = bullFillColor;
             return this;
         }
         public SeriesBuilder DrawMode(SeriesDrawMode drawMode)
         {
             this.m_Series.DrawMode = drawMode;
             return this;
         }
         public SeriesBuilder LabelPosition(ChartLabelPosition labelPosition)
        {
            this.m_Series.LabelPosition = labelPosition;
            return this;
        }
         public SeriesBuilder SeriesType(SeriesType type)
        {
            this.m_Series.Type = type;
            return this;
        }
         public SeriesBuilder LineJoin(ChartLineJoin lineJoin)
        {
            this.m_Series.LineJoin = lineJoin;
            return this;
        }
         public SeriesBuilder LineCap(ChartLineCap lineCap)
         {
             this.m_Series.LineCap = lineCap;
             return this;
         }
         public SeriesBuilder PyramidMode(PyramidMode pyramidMode)
         {
             this.m_Series.PyramidMode = pyramidMode;
             return this;
         }
         public SeriesBuilder ExplodeIndex(int explodeIndex)
        {
            this.m_Series.ExplodeIndex = explodeIndex;
            return this;
        }
         public SeriesBuilder Border(Action<ChartBorderBuilder> font)
         {
             var obj = new ChartBorder();
             this.m_Series.Border = obj;
             var builder = new ChartBorderBuilder(obj);
             if (font != null)
                 font.Invoke(builder);
             return this;
         }
         public SeriesBuilder Fill(string fill)
         {
             this.m_Series.Fill = fill;
             return this;
         }
         public SeriesBuilder Opacity(double Opacity)
         {
             this.m_Series.Opacity = Opacity;
             return this;
         }
         public SeriesBuilder Width(double Width)
         {
             this.m_Series.Width = Width;
             return this;
         }
         public SeriesBuilder DashArray(string dashArray)
         {
             this.m_Series.DashArray = dashArray;
             return this;
         }
         public SeriesBuilder StartAngle(int startAngle)
        {
            this.m_Series.StartAngle = startAngle;
            return this;
        }
         public SeriesBuilder ExplodeOffset(float explodeOffset)
        {
            this.m_Series.ExplodeOffset = explodeOffset;
            return this;
        }
         public SeriesBuilder DoughnutCoefficient(float doughnutCoefficient)
        {
            this.m_Series.DoughnutCoefficient = doughnutCoefficient;
            return this;
        }
         public SeriesBuilder PieCoefficient(float pieCoefficient)
        {
            this.m_Series.PieCoefficient = pieCoefficient;
            return this;
        }
         public SeriesBuilder DoughnutSize(float doughnutSize)
        {
            this.m_Series.DoughnutSize = doughnutSize;
            return this;
        }
         public SeriesBuilder GapRatio(float gapRatio)
        {
            this.m_Series.GapRatio = gapRatio;
            return this;
        }
         public SeriesBuilder Explode(bool explode)
        {
            this.m_Series.Explode = explode;
            return this;
        }
         public SeriesBuilder ExplodeAll(bool explodeAll)
        {
            this.m_Series.ExplodeAll = explodeAll;
            return this;
        }
         public SeriesBuilder EnableSmartLabels(bool smartLabelEnabled)
         {
             this.m_Series.EnableSmartLabels = smartLabelEnabled;
             return this;
         }
         public SeriesBuilder Animation(bool animation)
        {
            this.m_Series.Animation = animation;
            return this;
        }
         public SeriesBuilder XAxisName(string xAxisName)
        {
            this.m_Series.XAxisName = xAxisName;
            return this;
        }
         public SeriesBuilder YAxisName(string yAxisName)
        {
            this.m_Series.YAxisName = yAxisName;
            return this;
        }
         public SeriesBuilder Name(string name)
        {
            this.m_Series.Name = name;
            return this;
        }
    }

    public class SizeBuilder
    {
        private ChartSize size;
        public SizeBuilder(ChartSize size)
        {
            this.size = size;
        }
        public SizeBuilder Width(int Width)
        {
            this.size.Width = Width;
            return this;
        }
        public SizeBuilder Height(int Height)
        {
            this.size.Height = Height;
            return this;
        }

    }

    public class NewTooltipBuilder
    {
        private NewTooltip tooltip = new NewTooltip();
        public NewTooltipBuilder(NewTooltip tooltip)
        {
            this.tooltip = tooltip;
        }
        public NewTooltipBuilder Visible(bool visible)
        {
            this.tooltip.Visible = visible;
            return this;
        }
        public NewTooltipBuilder Animation(bool Animation)
        {
            this.tooltip.Animation = Animation;
            return this;
        }
        public NewTooltipBuilder Duration(string Duration)
        {
            this.tooltip.Duration = Duration;
            return this;
        }
        public NewTooltipBuilder Format(string format)
        {
            this.tooltip.Format = format;
            return this;
        }
        public NewTooltipBuilder Template(string template)
        {
            this.tooltip.Template = template;
            return this;
        }
        public NewTooltipBuilder RX(int rx)
        {
            this.tooltip.RX = rx;
            return this;
        }
        public NewTooltipBuilder RY(int ry)
        {
            this.tooltip.RY = ry;
            return this;
        }
        public NewTooltipBuilder Fill(string fill)
        {
            this.tooltip.Fill = fill;
            return this;
        }
        public NewTooltipBuilder Font(Action<ChartFontBuilder> font)
        {
            var obj = new ChartFont();
            this.tooltip.Font = obj;
            var builder = new ChartFontBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public NewTooltipBuilder Border(Action<ChartBorderBuilder> font)
        {
            var obj = new ChartBorder();
            this.tooltip.Border = obj;
            var builder = new ChartBorderBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
    }
    public class DataLabelBuilder
    {
        private DataLabel datalabel = new DataLabel();
        public DataLabelBuilder(DataLabel datalabel)
        {
            this.datalabel = datalabel;
        }
        public DataLabelBuilder Offset(int Offset)
        {
            this.datalabel.Offset = Offset;
            return this;
        }
        public DataLabelBuilder Template(string Template)
        {
            this.datalabel.Template= Template;
            return this;
        }
        public DataLabelBuilder Font(Action<ChartFontBuilder> font)
        {
            var obj = new ChartFont();
            this.datalabel.Font = obj;
            var builder = new ChartFontBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public DataLabelBuilder Margin(Action<MarginBuilder> margin)
        {
            var obj = new Margin();
            this.datalabel.Margin = obj;
            var builder = new MarginBuilder(obj);
            if (margin != null)
                margin.Invoke(builder);
            return this;
        }
        public DataLabelBuilder Fill(string fill)
        {
            this.datalabel.Fill = fill;
            return this;
        }
        public DataLabelBuilder Opacity(double Opacity)
        {
            this.datalabel.Opacity = Opacity;
            return this;
        }

        public DataLabelBuilder Border(Action<ChartBorderBuilder> border)
        {
            var obj = new ChartBorder();
            this.datalabel.Border = obj;
            var builder = new ChartBorderBuilder(obj);
            if (border != null)
                border.Invoke(builder);
            return this;
        }
       
        public DataLabelBuilder ConnectorLine(Action<ConnectorLineBuilder> connectorLine)
        {
            var obj = new ConnectorLine();
            this.datalabel.ConnectorLine = obj;
            var builder = new ConnectorLineBuilder(obj);
            if (connectorLine != null)
                connectorLine.Invoke(builder);
            return this;
        }
        public DataLabelBuilder Shape(ChartShape Shape)
        {
            this.datalabel.Shape = Shape;
            return this;
        }
        public DataLabelBuilder Visible(bool visible)
        {
            this.datalabel.Visible = visible;
            return this;
        }
      
        public DataLabelBuilder TextPosition(TextPosition TextPosition)
        {
            this.datalabel.TextPosition = TextPosition;
            return this;
        }
        public DataLabelBuilder VerticalTextAlignment(TextAlignment vertextAlignment)
        {
            this.datalabel.VerticalTextAlignment = vertextAlignment;
            return this;
        }
        public DataLabelBuilder HorizontalTextAlignment(TextAlignment textAlignment)
        {
            this.datalabel.HorizontalTextAlignment = textAlignment;
            return this;
        }

    }
   
    public class MarkerBuilder
    {
        private Marker marker = new Marker();
        public MarkerBuilder(Marker marker)
        {
            this.marker = marker;
        }
       
        public MarkerBuilder Visible(bool Visible)
        {
            this.marker.Visible= Visible;
            return this;
        }
        public MarkerBuilder Shape(ChartShape Shape)
        {
            this.marker.Shape = Shape;
            return this;
        }
        public MarkerBuilder Fill(string fill)
        {
            this.marker.Fill = fill;
            return this;
        }
        public MarkerBuilder Opacity(double Opacity)
        {
            this.marker.Opacity = Opacity;
            return this;
        }
        public MarkerBuilder Size(Action<SizeBuilder> size)
        {
            var obj = new ChartSize();
            this.marker.Size = obj;
            var builder = new SizeBuilder(obj);
            if (size != null)
                size.Invoke(builder);
            return this;
        }
        public MarkerBuilder Border(Action<ChartBorderBuilder> border)
        {
            var obj = new ChartBorder();
            this.marker.Border = obj;
            var builder = new ChartBorderBuilder(obj);
            if (border != null)
                border.Invoke(builder);
            return this;
        }
        public MarkerBuilder DataLabel(Action<DataLabelBuilder> dataLabel)
        {
            var obj = new DataLabel();
            this.marker.DataLabel = obj;
            var builder = new DataLabelBuilder(obj);
            if (dataLabel != null)
                dataLabel.Invoke(builder);
            return this;
        }


    }

    public class ConnectorLineBuilder
    {
        private ConnectorLine connectorLine = new ConnectorLine();
        public ConnectorLineBuilder(ConnectorLine connectorLine)
        {
            this.connectorLine = connectorLine;
        }
        public ConnectorLineBuilder Width(double width)
        {
            this.connectorLine.Width = width;
            return this;
        }
        public ConnectorLineBuilder Color(string color)
        {
            this.connectorLine.Color = color;
            return this;
        }
        public ConnectorLineBuilder Height(double height)
        {
            this.connectorLine.Height = height;
            return this;
        }
        public ConnectorLineBuilder Type(ConnectorType type)
        {
            this.connectorLine.Type = type;
            return this;
        }

    }
   
    public class PointsBuilder 
    {
        private Points Points=new Points();
        private Series m_series;
        private List<Points> pointsCollection; 
        public PointsBuilder(Points points)
        {
            this.Points = points;
        }
        public PointsBuilder(Points options, Series series)
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
        public PointsBuilder X(object X)
        {
             this.Points.X = X;
            return this;
        }
        public PointsBuilder High(object high)
        {
            this.Points.High = high;
            return this;
        }
        public PointsBuilder Low(object low)
        {
            this.Points.Low = low;
            return this;
        }
        public PointsBuilder Close(object close)
        {
            this.Points.Close = close;
            return this;
        }
        public PointsBuilder Open(object open)
        {
            this.Points.Open = open;
            return this;
        }
        
		  public PointsBuilder Size(double size)
        {
            this.Points.Size = size;
            return this;
        }
        public PointsBuilder Y(object Y)
        {
            this.Points.Y = Y;
            return this;
        }
        public PointsBuilder Visible(bool visible)
        {
            this.Points.Visible = visible;
            return this;
        }
        public PointsBuilder IsEmpty(bool isEmpty)
        {
            this.Points.IsEmpty = isEmpty;
            return this;
        }
        
        public PointsBuilder Text(string text)
        {
            this.Points.Text = text;
            return this;
        }

        public PointsBuilder Border(Action<ChartBorderBuilder> font)
        {
            var obj = new ChartBorder();
            this.Points.Border = obj;
            var builder = new ChartBorderBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public PointsBuilder Fill(string fill)
        {
            this.Points.Fill = fill;
            return this;
        }
        public PointsBuilder Opacity(double opacity)
        {
            this.Points.Opacity = opacity;
            return this;
        }
        public PointsBuilder Width(double width)
        {
            this.Points.Width = width;
            return this;
        }

        public PointsBuilder Font(object font)
        {
            this.Points.Font = font;
            return this;
        }
        public PointsBuilder Shape(ChartShape shape)
        {
            this.Points.Shape = shape;
            return this;
        }
       
        public PointsBuilder Marker(Action<MarkerBuilder> marker)
        {
            var obj = new Marker();
            this.Points.Marker = obj;
            var builder = new MarkerBuilder(obj);
            if (marker != null)
                marker.Invoke(builder);
            return this;
        }
       
    }
}