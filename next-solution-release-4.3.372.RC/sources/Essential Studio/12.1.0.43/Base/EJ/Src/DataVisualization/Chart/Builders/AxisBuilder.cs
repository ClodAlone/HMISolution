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
using Syncfusion.JavaScript.Shared;

namespace Syncfusion.JavaScript.DataVisualization 
{
    public class AxisBuilder
    {
         
        
         private Axis m_Axis = new Axis();
        private ChartProperties m_model;
        private List<Axis> m_axes; 
         public AxisBuilder(Axis options)
        {
            this.m_Axis = options;
        }
         public AxisBuilder(Axis options,Chart chart)
         {
             this.m_Axis = options;
             this.m_model = chart.ChartModel;

             m_axes = new List<Axis>();
             this.m_model.Axes = new List<Axis>();
         }
         public void Add()
         {
             this.m_model.Axes.Add(m_Axis);
             this.m_axes.Add(m_Axis);
             m_Axis = new Axis();

         }
         public AxisBuilder Orientation(Orientation orientation)
         {
             this.m_Axis.Orientation = orientation;
             return this;
         }
         
         public AxisBuilder ZoomPosition(double zoomposition)
         {
             this.m_Axis.ZoomPosition = zoomposition;
             return this;
         }
         public AxisBuilder RowIndex(int rowIndex)
         {
             this.m_Axis.RowIndex = rowIndex;
             return this;
         }
         public AxisBuilder ColumnIndex(int columnIndex)
         {
             this.m_Axis.ColumnIndex = columnIndex;
             return this;
         }
         public AxisBuilder RowSpan(int rowSpan)
         {
             this.m_Axis.RowSpan = rowSpan;
             return this;
         }
         public AxisBuilder ColumnSpan(int columnSpan)
         {
             this.m_Axis.ColumnSpan = columnSpan;
             return this;
         }
         public AxisBuilder LogBase(int logBase)
         {
             this.m_Axis.LogBase = logBase;
             return this;
         }

         public AxisBuilder CategoryInterval(int categoryInterval)
         {
             this.m_Axis.CategoryInterval = categoryInterval;
             return this;
         }
         public AxisBuilder PlotOffset(int plotOffset)
         {
             this.m_Axis.PlotOffset = plotOffset;
             return this;
         }
         public AxisBuilder ZoomFactor(double zoomfactor)
         {
             this.m_Axis.ZoomFactor = zoomfactor;
             return this;
         }
       
         public AxisBuilder LabelFormat(string labelformat)
         {
             this.m_Axis.LabelFormat = labelformat;
             return this;
         }
         public AxisBuilder LabelRotation(int labelRotation)
         {
             this.m_Axis.LabelRotation= labelRotation;
             return this;
         }
         public AxisBuilder MinorGridLines(Action<MinorGridLinesBuilder> gridLine)
         {
             var obj = new MinorGridLines();
             this.m_Axis.MinorGridLines = obj;
             var builder = new MinorGridLinesBuilder(obj);
             if (gridLine != null)
                 gridLine.Invoke(builder);
             return this;
         }
        
         //public AxisBuilder Range(Action<RangeBuilder> range)
         //{
         //    RangeBuilder builder = new RangeBuilder(this.m_Axis.Range);
         //    range.Invoke(builder);
         //    return this;
         //}

          
         public AxisBuilder MinorTicksPerInterval(int minorTicksPerInterval)
         {
             this.m_Axis.MinorTicksPerInterval = minorTicksPerInterval;
             return this;
         }


         public AxisBuilder MajorGridLines(Action<MajorGridLinesBuilder> gridLine)
         {
             var obj = new MajorGridLines();
             this.m_Axis.MajorGridLines = obj;
             var builder = new MajorGridLinesBuilder(obj);
             if (gridLine != null)
                 gridLine.Invoke(builder);
             return this;
            
         }

         public AxisBuilder MajorTickLines(Action<MajorTicksBuilder> majorticklines)
         {

             var obj = new MajorTicks();
             this.m_Axis.MajorTickLines = obj;
             var builder = new MajorTicksBuilder(obj);
             if (majorticklines != null)
                 majorticklines.Invoke(builder);
             return this;
         }

         public AxisBuilder MinorTickLines(Action<MinorTicksBuilder> minorticklines)
         {
             var obj = new MinorTicks();
             this.m_Axis.MinorTickLines = obj;
             var builder = new MinorTicksBuilder(obj);
             if (minorticklines != null)
                 minorticklines.Invoke(builder);
             return this;
         }

         public AxisBuilder Title(Action<AxesTitleBuilder> title)
         {
             var obj = new Title();
             this.m_Axis.Title = obj;
             var builder = new AxesTitleBuilder(obj);
             if (title != null)
                 title.Invoke(builder);
             return this;
         }

         public AxisBuilder AxisLine(Action<MajorGridLinesBuilder> axisline)
         {
             var obj = new MajorGridLines();
             this.m_Axis.AxisLine = obj;
             var builder = new MajorGridLinesBuilder(obj);
             if (axisline != null)
                 axisline.Invoke(builder);
             return this;
         }

         public AxisBuilder AxisName(string axisname)
         {
             this.m_Axis.AxisName = axisname;
             return this;
         }

         public AxisBuilder IsInversed(bool inversed)
         {
             this.m_Axis.IsInversed = inversed;
             return this;
         }
         public AxisBuilder RangePadding(ChartRangePadding rangePadding)
         {
             this.m_Axis.RangePadding = rangePadding;
             return this;
         }
     
         public AxisBuilder OpposedPosition(bool opposedPosition)
         {
             this.m_Axis.OpposedPosition = opposedPosition;
             return this;
         }
        
         public AxisBuilder HidePartialLabels(bool hidePartialLabels)
         {
             this.m_Axis.HidePartialLabels = hidePartialLabels;
             return this;
         }

         public AxisBuilder CrosshairLabel(Action<CrosshairLabelBuilder> crosshairlabel)
         {
             var obj = new CrosshairLabel();
             this.m_Axis.CrosshairLabel = obj;
             var builder = new CrosshairLabelBuilder(obj);
             if (crosshairlabel != null)
                 crosshairlabel.Invoke(builder);
             return this;
         }

         public AxisBuilder Stripline(Action<StriplineBuilder> stripline)
         {
             var obj = new Stripline();
             var builder = new StriplineBuilder(obj, m_Axis);
             if (stripline != null)
                 stripline.Invoke(builder);
             return this;
         }
       public AxisBuilder RoundingPlaces(int roundingPlaces)
       {
           this.m_Axis.RoundingPlaces = roundingPlaces;
           return this;
       }
         public AxisBuilder Stripline(List<Stripline> stripline)
         {
             this.m_Axis.Stripline = stripline;
             return this;
         }
         public AxisBuilder IntervalType(ChartIntervalType interval)
         {
             this.m_Axis.IntervalType = interval;
             return this;
         }
                       
         public AxisBuilder DesiredIntervals(int desiredIntervals)
         {
             this.m_Axis.DesiredIntervals = desiredIntervals;
             return this;
         }
                             
         public AxisBuilder ValueType(AxisValueType valueType)
         {
             this.m_Axis.ValueType = valueType;
             return this;
         }
         public AxisBuilder LabelIntersectAction(LabelIntersectAction action)
         {
             this.m_Axis.LabelIntersectAction = action;
             return this;
         }
         public AxisBuilder LabelPlacement(string placement)
         {
             this.m_Axis.LabelPlacement = placement;
             return this;
         }
        public AxisBuilder Font(Action<ChartFontBuilder> font)
         {
             var obj = new ChartFont();
             this.m_Axis.Font = obj;
             var builder = new ChartFontBuilder(obj);
             if (font != null)
                 font.Invoke(builder);
             return this;
         }
        public AxisBuilder Range(Action<RangeBuilder> range)
        {
            var obj = new Range();
            this.m_Axis.Range = obj;
            var builder = new RangeBuilder(obj);
            if (range != null)
                range.Invoke(builder);
            return this;
        }

    }

    public class StriplineBuilder
     {
       private Stripline Stripline=new Stripline();
        private Axis m_axis;
        private List<Stripline> striplineCollection; 
        public StriplineBuilder(Stripline stripline)
        {
            this.Stripline = stripline;
        }
        public StriplineBuilder(Stripline options, Axis axis)
         {
             this.Stripline = options;
             this.m_axis = axis;

             striplineCollection = new List<Stripline>();
             this.m_axis.Stripline = new List<Stripline>();
         }
         public void Add()
         {
             this.m_axis.Stripline.Add(Stripline);
             this.striplineCollection.Add(Stripline);
             Stripline = new Stripline();

         }

         public StriplineBuilder Visible(bool visible)
         {

             this.Stripline.Visible = visible;
             return this;
         }

         public StriplineBuilder StartFromAxis(bool startFromAxis)
         {

             this.Stripline.StartFromAxis = startFromAxis;
             return this;
         }

         public StriplineBuilder Text(string text)
         {
             this.Stripline.Text = text;
             return this;
         }
   
         public StriplineBuilder TextAlignment(StriplineTextAlignment textAlignment)
         {
             this.Stripline.TextAlignment = textAlignment;
             return this;
         }

         public StriplineBuilder Font(Action<ChartFontBuilder> font)
         {
             var obj = new ChartFont();
             this.Stripline.Font = obj;
             var builder = new ChartFontBuilder(obj);
             if (font != null)
                 font.Invoke(builder);
             return this;
         }

         public StriplineBuilder StriplineColor(string color)
         {
             this.Stripline.StriplineColor = color;
             return this;
         }
         public StriplineBuilder BorderColor(string borderColor)
         {
             this.Stripline.BorderColor = borderColor;
             return this;
         }
         public StriplineBuilder ZOrder(ChartZOrder zOrder)
         {
             this.Stripline.ZOrder = zOrder;
             return this;
         }
         public StriplineBuilder BorderWidth(double borderWidth)
         {
             this.Stripline.BorderWidth = borderWidth;
             return this;
         }
         public StriplineBuilder Start(double start)
         {
             this.Stripline.Start = start;
             return this;
         }
         public StriplineBuilder End(double end)
         {
             this.Stripline.End = end;
             return this;
         }

     }



    public class CrosshairLabelBuilder
    {
        private CrosshairLabel m_crossHairLabelStyle = null;
        public CrosshairLabelBuilder(CrosshairLabel crossHairLabelStyle)
        {
            this.m_crossHairLabelStyle = crossHairLabelStyle;
        }
        public CrosshairLabelBuilder Font(Action<ChartFontBuilder> font)
        {

            var obj = new ChartFont();
            this.m_crossHairLabelStyle.Font = obj;
            var builder = new ChartFontBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;

        }
        public CrosshairLabelBuilder Border(Action<ChartBorderBuilder> font)
        {
            var obj = new ChartBorder();
            this.m_crossHairLabelStyle.Border = obj;
            var builder = new ChartBorderBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public CrosshairLabelBuilder RX(int rx)
        {
            this.m_crossHairLabelStyle.RX = rx;
            return this;
        }
        public CrosshairLabelBuilder RY(int ry)
        {
            this.m_crossHairLabelStyle.RY = ry;
            return this;
        }
        public CrosshairLabelBuilder Fill(string fill)
        {
            this.m_crossHairLabelStyle.Fill = fill;
            return this;
        }
        public CrosshairLabelBuilder Visible(bool visible)
        {
            this.m_crossHairLabelStyle.Visible = visible;
            return this;
        }

    }

    public class TitleBuilder
    {
        private Title m_title = null;
        public TitleBuilder(Title title)
        {
            this.m_title = title;
        }
         
        public TitleBuilder Text(string title)
        {
            this.m_title.Text = title;
            return this;
        }

        public TitleBuilder TextAlignment(TextAlignment alignment)
        {
            this.m_title.TextAlignment = alignment;
            return this;
        }
        
        public TitleBuilder Font(Action<ChartFontBuilder> font)
        {
            var obj = new ChartFont();
            this.m_title.Font = obj;
            var builder = new ChartFontBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }


    }
    public class AxesTitleBuilder
    {
        private Title m_title = null;
        public AxesTitleBuilder(Title title)
        {
            this.m_title = title;
        }

        public AxesTitleBuilder Text(string title)
        {
            this.m_title.Text = title;
            return this;
        }

         
        public AxesTitleBuilder Font(Action<ChartFontBuilder> font)
        {
            var obj = new ChartFont();
            this.m_title.Font = obj;
            var builder = new ChartFontBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }


    }
    /// create a RangeBuilder class.
    /// </summary>
    public class RangeBuilder
    {
        private Range m_Range;
        public RangeBuilder(Range range)
        {
            this.m_Range = range;
        }

      
        
        public RangeBuilder Min(object minimum)
        {
            this.m_Range.Min = minimum;
            return this;
        }
       
        public RangeBuilder Max(object maximum)
        {
            this.m_Range.Max = maximum;
            return this;
        }
       
        public RangeBuilder Interval(double interval)
        {
            this.m_Range.Interval = interval;
            return this;
        }
    }
}
