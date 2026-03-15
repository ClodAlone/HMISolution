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
    class ChartModelPropertiesBuilder
    {

    }

    public class LayoutBorderBuilder
    {

        private LayoutBorder m_Border = new LayoutBorder();
        public LayoutBorderBuilder(LayoutBorder options)
        {
            this.m_Border = options;
        }

     public LayoutBorderBuilder Color(string color)
     {
         this.m_Border.Color = color;
         return this;
     }

     public LayoutBorderBuilder Width(double width)
     {
         this.m_Border.Width = width;
         return this;
     }
     public LayoutBorderBuilder Opacity(double opacity)
     {
         this.m_Border.Opacity = opacity;
         return this;
     }

    }
    
    public class ChartAreaBuilder
    {

        private ChartArea m_ChartAreaBorder = new ChartArea();
        public ChartAreaBuilder(ChartArea options)
        {
            this.m_ChartAreaBorder = options;
        }
        public ChartAreaBuilder Border(Action<LayoutBorderBuilder> border)
        {
            var obj = new LayoutBorder();
            this.m_ChartAreaBorder.Border = obj;
            var builder = new LayoutBorderBuilder(obj);
            if (border != null)
                border.Invoke(builder);
            return this;
        }
       
         public ChartAreaBuilder Background(string color)
        {
            this.m_ChartAreaBorder.Background = color;
            return this;
        }
        
    }

    public class ZoomingBuilder
    {

        private Zooming m_ZoomingBuilder = new Zooming();
        public ZoomingBuilder(Zooming options)
        {
            this.m_ZoomingBuilder = options;
        }

        public ZoomingBuilder Enable(bool enable)
        {
            this.m_ZoomingBuilder.Enable = enable;
            return this;
        }
        public ZoomingBuilder EnableMouseWheel(bool enableMouseWheel)
        {
            this.m_ZoomingBuilder.EnableMouseWheel = enableMouseWheel;
            return this;
        }
        public ZoomingBuilder Type(string type)
        {
            this.m_ZoomingBuilder.Type = type;
            return this;
        }
    }
    public class CrossHairBuilder
    {
        private CrossHair m_CrossHairBuilder = new CrossHair();
        public CrossHairBuilder(CrossHair options)
        {
            this.m_CrossHairBuilder = options;
        }

        public CrossHairBuilder Marker(Action<MarkerBuilder> marker)
        {
            var obj = new Marker();
            this.m_CrossHairBuilder.Marker = obj;
            var builder = new MarkerBuilder(obj);
            if (marker != null)
                marker.Invoke(builder);
            return this;
        }
        public CrossHairBuilder Line(Action<LineBuilder> line)
        {
            var obj = new Line();
            this.m_CrossHairBuilder.Line = obj;
            var builder = new LineBuilder(obj);
            if (line != null)
                line.Invoke(builder);
            return this;
        }
        public CrossHairBuilder Type(CrosshairType type)
        {
            this.m_CrossHairBuilder.Type = type;
            return this;
        }
        public CrossHairBuilder Visible(bool visible)
        {
            this.m_CrossHairBuilder.Visible = visible;
            return this;
        }
    }
     public class LineBuilder
     {
         private Line line=new Line();
         public LineBuilder(Line options)
        {
            this.line = options;
        }
         public LineBuilder Width(double width)
         {
             this.line.Width = width;
             return this;
         }
         public LineBuilder Color(string color)
         {
             this.line.Color = color;
             return this;
         }
     }
    public class MarginBuilder
    {
        private Margin m_Margin = new Margin();
        public MarginBuilder(Margin options)
        {
            this.m_Margin = options;
        }
        public MarginBuilder Left(double left)
        {
            this.m_Margin.Left = left;
            return this;
        }
        public MarginBuilder Right(double right)
        {
            this.m_Margin.Right = right;
            return this;
        }
        public MarginBuilder Top(double top)
        {
            this.m_Margin.Top = top;
            return this;
        }
        public MarginBuilder Bottom(double bottom)
        {
            this.m_Margin.Bottom = bottom;
            return this;
        }
    }

    public class CommonSeriesOptionsBuilder
    {
        private CommonSeriesOptions m_CommonSeriesOptionsBuilder = new CommonSeriesOptions();
        public CommonSeriesOptionsBuilder(CommonSeriesOptions options)
        {
            this.m_CommonSeriesOptionsBuilder = options;
        }

        public CommonSeriesOptionsBuilder Type(SeriesType type)
        {
            this.m_CommonSeriesOptionsBuilder.Type = type;
            return this;
        }

        public CommonSeriesOptionsBuilder Animation(bool animation)
        {
            this.m_CommonSeriesOptionsBuilder.Animation = animation;
            return this;
        }

        public CommonSeriesOptionsBuilder DoughnutCoefficient(double doughnutCoefficient)
        {
            this.m_CommonSeriesOptionsBuilder.DoughnutCoefficient = doughnutCoefficient;
            return this;
        }

        public CommonSeriesOptionsBuilder PieCoefficient(double pieCoefficient)
        {
            this.m_CommonSeriesOptionsBuilder.PieCoefficient = pieCoefficient;
            return this;
        }
        public CommonSeriesOptionsBuilder DoughnutSize(double doughnutSize)
        {
            this.m_CommonSeriesOptionsBuilder.DoughnutSize = doughnutSize;
            return this;
        }

        public CommonSeriesOptionsBuilder ExplodeOffset(double explodeOffset)
        {
            this.m_CommonSeriesOptionsBuilder.ExplodeOffset = explodeOffset;
            return this;
        }

        public CommonSeriesOptionsBuilder GapRatio(double gapRatio)
        {
            this.m_CommonSeriesOptionsBuilder.GapRatio = gapRatio;
            return this;
        }

        public CommonSeriesOptionsBuilder LabelPosition(ChartLabelPosition labelPosition)
        {
            this.m_CommonSeriesOptionsBuilder.LabelPosition = labelPosition;
            return this;
        }
        public CommonSeriesOptionsBuilder PyramidMode(PyramidMode pyramidMode)
        {
            this.m_CommonSeriesOptionsBuilder.PyramidMode = pyramidMode;
            return this;
        }
        public CommonSeriesOptionsBuilder StartAngle(int startAngle)
        {
            this.m_CommonSeriesOptionsBuilder.StartAngle = startAngle;
            return this;
        }
        public CommonSeriesOptionsBuilder Explode(bool explode)
        {
            this.m_CommonSeriesOptionsBuilder.Explode = explode;
            return this;
        }
        public CommonSeriesOptionsBuilder ExplodeAll(bool explodeAll)
        {
            this.m_CommonSeriesOptionsBuilder.ExplodeAll = explodeAll;
            return this;
        }
        public CommonSeriesOptionsBuilder ExplodeIndex(int explodeIndex)
        {
            this.m_CommonSeriesOptionsBuilder.ExplodeIndex = explodeIndex;
            return this;
        }
        public CommonSeriesOptionsBuilder BearFillColor(string bearFillColor)
        {
            this.m_CommonSeriesOptionsBuilder.BearFillColor = bearFillColor;
            return this;
        }
        public CommonSeriesOptionsBuilder BullFillColor(string bullFillColor)
        {
            this.m_CommonSeriesOptionsBuilder.BullFillColor = bullFillColor;
            return this;
        }
        public CommonSeriesOptionsBuilder DrawMode(SeriesDrawMode drawMode)
        {
            this.m_CommonSeriesOptionsBuilder.DrawMode = drawMode;
            return this;
        }
        public CommonSeriesOptionsBuilder Tooltip(Action<NewTooltipBuilder> tooltip)
        {
            var obj = new NewTooltip();
            this.m_CommonSeriesOptionsBuilder.Tooltip = obj;
            var builder = new NewTooltipBuilder(obj);
            if (tooltip != null)
                tooltip.Invoke(builder);
            return this;
        }
        public CommonSeriesOptionsBuilder Marker(Action<MarkerBuilder> marker)
        {
            var obj = new Marker();
            this.m_CommonSeriesOptionsBuilder.Marker = obj;
            var builder = new MarkerBuilder(obj);
            if (marker != null)
                marker.Invoke(builder);
            return this;
        }
        public CommonSeriesOptionsBuilder Font(Action<ChartFontBuilder> font)
        {
            var obj = new ChartFont();
            this.m_CommonSeriesOptionsBuilder.Font = obj;
            var builder = new ChartFontBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public CommonSeriesOptionsBuilder DataSource(Action<ChartDataSourceBuilder> dataSource)
        {
            var obj = new ChartDataSource();
            this.m_CommonSeriesOptionsBuilder.DataSource = obj;
            var builder = new ChartDataSourceBuilder(obj);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public CommonSeriesOptionsBuilder Border(Action<ChartBorderBuilder> font)
        {
            var obj = new ChartBorder();
            this.m_CommonSeriesOptionsBuilder.Border = obj;
            var builder = new ChartBorderBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public CommonSeriesOptionsBuilder LineJoin(ChartLineJoin lineJoin)
        {
            this.m_CommonSeriesOptionsBuilder.LineJoin = lineJoin;
            return this;
        }
        public CommonSeriesOptionsBuilder LineCap(ChartLineCap lineCap)
        {
            this.m_CommonSeriesOptionsBuilder.LineCap = lineCap;
            return this;
        }
        public CommonSeriesOptionsBuilder Fill(string fill)
        {
            this.m_CommonSeriesOptionsBuilder.Fill = fill;
            return this;
        }
        public CommonSeriesOptionsBuilder Opacity(double Opacity)
        {
            this.m_CommonSeriesOptionsBuilder.Opacity = Opacity;
            return this;
        }
        public CommonSeriesOptionsBuilder Width(double Width)
        {
            this.m_CommonSeriesOptionsBuilder.Width = Width;
            return this;
        }
        public CommonSeriesOptionsBuilder DashArray(string dashArray)
        {
            this.m_CommonSeriesOptionsBuilder.DashArray = dashArray;
            return this;
        }



    }

    public class RowDefinitionsBuilder
    {
        private ChartProperties m_model;
        private List<RowDefinitions> RowDefinitions; 
        private RowDefinitions m_RowDefinitions = new RowDefinitions();
        public RowDefinitionsBuilder(RowDefinitions options)
        {
            this.m_RowDefinitions = options;
        }
        public RowDefinitionsBuilder(RowDefinitions options, Chart chart)
         {
             this.m_RowDefinitions = options;
             this.m_model = chart.ChartModel;

             RowDefinitions = new List<RowDefinitions>();
             this.m_model.RowDefinitions = new List<RowDefinitions>();
         }

        public void Add()
        {
            this.m_model.RowDefinitions.Add(m_RowDefinitions);
            this.RowDefinitions.Add(m_RowDefinitions);
            m_RowDefinitions = new RowDefinitions();

        }

        public RowDefinitionsBuilder RowHeight(double rowHeight)
        {
            this.m_RowDefinitions.RowHeight = rowHeight;
            return this;
        }

        public RowDefinitionsBuilder LineWidth(double lineWidth)
        {
            this.m_RowDefinitions.LineWidth = lineWidth;
            return this;
        }
        public RowDefinitionsBuilder LineColor(string lineColor)
        {
            this.m_RowDefinitions.LineColor = lineColor;
            return this;
        }
        public RowDefinitionsBuilder Unit(string unit)
        {
            this.m_RowDefinitions.Unit = unit;
            return this;
        }

    }

    public class ColumnDefinitionsBuilder
    {
        private ChartProperties m_model;
        private List<ColumnDefinitions> ColumnDefinitions;
        private ColumnDefinitions m_ColumnDefinitions = new ColumnDefinitions();
        public ColumnDefinitionsBuilder(ColumnDefinitions options)
        {
            this.m_ColumnDefinitions = options;
        }
        public ColumnDefinitionsBuilder(ColumnDefinitions options, Chart chart)
        {
            this.m_ColumnDefinitions = options;
            this.m_model = chart.ChartModel;

            ColumnDefinitions = new List<ColumnDefinitions>();
            this.m_model.ColumnDefinitions = new List<ColumnDefinitions>();
        }

        public void Add()
        {
            this.m_model.ColumnDefinitions.Add(m_ColumnDefinitions);
            this.ColumnDefinitions.Add(m_ColumnDefinitions);
            m_ColumnDefinitions = new ColumnDefinitions();

        }

        public ColumnDefinitionsBuilder ColumnWidth(double columnWidth)
        {
            this.m_ColumnDefinitions.ColumnWidth = columnWidth;
            return this;
        }


        public ColumnDefinitionsBuilder Unit(string unit)
        {
            this.m_ColumnDefinitions.Unit = unit;
            return this;
        }

    }
}
