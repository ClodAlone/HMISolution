#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class represents the LineSegments implemetation
    /// </summary>
    public class LinePresenter : SparkLinePresenter 
    {

        /// <summary>
        /// Get and Set the PointsProperty
        /// </summary>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        /// <summary>
        ///  Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(LinePresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        /// <summary>
        /// Get and Set SparkLineProperty
        /// </summary>
        public SparkLine SparkLine
        {
            get { return (SparkLine)GetValue(SparkLineProperty); }
            set { SetValue(SparkLineProperty, value); }
        }

        /// <summary>
        ///  Identifies the SparkLine dependency property.
        /// </summary>
        public static readonly DependencyProperty SparkLineProperty =
            DependencyProperty.Register("SparkLine", typeof(SparkLine), typeof(LinePresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        //DoubleRange sbsInfo = new DoubleRange(-1, 1);

        //double count = 0d;
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. 
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Points != null)
            {
                VisualCollection.Clear();
                if (SparkLine.Data.Count > 0)
                {
                    double mindata = SparkLine.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? SparkLine.Data.Min() : SparkLine.VerticalAxisMinimumValue;
                    double maxdata = SparkLine.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? SparkLine.Data.Max() : SparkLine.VerticalAxisMaximumValue;
                    double deltaY = maxdata - mindata;
                    double yMinValue = mindata;
                    deltaY = (deltaY == 0 ? 1 : deltaY);
                    var visual = new SyncDrawingVisual();
                    visual.Index = 0;
                    double val;
                    double val1;
                    if (this.SparkLine.IsEnableRangeBand)
                    {
                        if (this.SparkLine.BandRange.Start < this.SparkLine.BandRange.End)
                        {
                             val = 1 - (this.SparkLine.BandRange.Start - yMinValue) / deltaY;
                             val1 = 1 - (this.SparkLine.BandRange.End - yMinValue) / deltaY;
                        }
                        else
                        {
                             val = 1 - (this.SparkLine.BandRange.End - yMinValue) / deltaY;
                             val1 = 1 - (this.SparkLine.BandRange.Start - yMinValue) / deltaY;
                        }
                        
                        using (DrawingContext context = visual.RenderOpen())
                        {
                            
                            context.PushOpacity(0.5);
                            context.DrawRectangle(SparkLine.RangeBandInterior, null,
                                        new Rect(0, this.ActualHeight*val1, this.ActualWidth, this.ActualHeight*(val-val1)));
                        }
                    }
                    VisualCollection.Add(visual);
                    if (SparkLine.SparkLineType == SparkLineTypes.Line)
                    {
                        DrawLine();
                    }
                    else if (SparkLine.SparkLineType == SparkLineTypes.Column)
                    {
                        DrawColumn();
                    }
                    else if (SparkLine.SparkLineType == SparkLineTypes.WinLoss)
                    {
                        DrawWinLoss();
                    }
                }
            }
            base.OnRender(drawingContext);
        }

        private void DrawWinLoss()
        {


            double deltaX = ActualWidth / Points.Count;
            double mindata = SparkLine.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? SparkLine.Data.Min() : SparkLine.VerticalAxisMinimumValue;
            double maxdata = SparkLine.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? SparkLine.Data.Max() : SparkLine.VerticalAxisMaximumValue;
            double deltaY = maxdata - mindata;
            deltaY = (deltaY == 0 ? 1 : deltaY);
            double delta = ActualWidth / Points.Count * 0.6;
            double xPos = (deltaX - delta) / 2;
            double yMin = mindata;
            double actualHeight = this.ActualHeight;
            double midpoint = actualHeight / 2;
            Pen normalpen = new Pen(SparkLine.Interior, SparkLine.StrokeThickness);
            Pen negetivepen = new Pen(SparkLine.NegativePointsHighlightBrush, SparkLine.StrokeThickness);
            double origin = SparkLine.ValueToPoint(0, 1).Y;
            for (var i = 0; i < Points.Count; i++)
            {
                var visual = new SyncDrawingVisual();
                visual.Index = i;
                using (DrawingContext context = visual.RenderOpen())
                {
                    var val = 1 - (Points[i].Y - yMin) / deltaY;
                    if (Points[i].Y > 0)
                    {
                        context.DrawRectangle(SparkLine.Interior, null,
                            new Rect(new Point(xPos, 0), new Point(xPos + delta, midpoint)));
                    }
                    else
                    {
                        if (SparkLine.IsNegativePointsHighlighted)
                        {
                            context.DrawRectangle(SparkLine.NegativePointsHighlightBrush, null,
                                 new Rect(new Point(xPos, midpoint), new Point(xPos + delta, actualHeight)));
                        }
                        else
                            context.DrawRectangle(SparkLine.Interior, null,
                                 new Rect(new Point(xPos, midpoint), new Point(xPos + delta, actualHeight)));
                    }
                    xPos += deltaX;

                }
                VisualCollection.Add(visual);
            }


            if (SparkLine.IsLastPointHighlighted)
            {
                xPos -= deltaX;

                var visual2 = new SyncDrawingVisual();
                visual2.Index = 0;
                using (DrawingContext context = visual2.RenderOpen())
                {
                    if (Points.LastOrDefault().Y > 0)
                    {
                        var val = 1 - (Points.LastOrDefault().Y - yMin) / deltaY;
                        context.DrawRectangle(SparkLine.LastPointHighlightBrush, null,
                                    new Rect(new Point(xPos, 0), new Point(xPos + delta, midpoint)));
                    }
                    else
                    {
                        var val = 1 - (Points.LastOrDefault().Y - yMin) / deltaY;
                        context.DrawRectangle(SparkLine.LastPointHighlightBrush, null,
                                    new Rect(new Point(xPos, midpoint), new Point(xPos + delta, actualHeight)));
                    }
                }
                VisualCollection.Add(visual2);
            }
            if (SparkLine.IsFirstPointHighlighted)
            {
                xPos = (deltaX - delta) / 2;
                var visual1 = new SyncDrawingVisual();
                visual1.Index = 0;
                using (DrawingContext context = visual1.RenderOpen())
                {
                    if (Points.Count != 0 && Points[0].Y > 0)
                    {
                        var val = 1 - (Points[0].Y - yMin) / deltaY;
                        context.DrawRectangle(SparkLine.FirstPointHighlightBrush, null,
                                    new Rect(new Point(xPos, 0), new Point(xPos + delta, midpoint)));
                    }
                    else if (Points.Count != 0)
                    {
                        var val = 1 - (Points[0].Y - yMin) / deltaY;
                        context.DrawRectangle(SparkLine.FirstPointHighlightBrush, null,
                                    new Rect(new Point(xPos, midpoint), new Point(xPos + delta, actualHeight)));
                    }
                }
                VisualCollection.Add(visual1);
            }
            if (SparkLine.IsHighPointHighlighted)
            {

                Point minP = Points.OrderByDescending(p => p.Y).FirstOrDefault();
                xPos = (minP.X * deltaX) + (deltaX - delta) / 2;

                var visual3 = new SyncDrawingVisual();
                visual3.Index = 0;
                using (DrawingContext context = visual3.RenderOpen())
                {
                    if (minP.Y > 0)
                    {
                        var val = 1 - (minP.Y - yMin) / deltaY;
                        context.DrawRectangle(SparkLine.HighPointHighlightBrush, null,
                                    new Rect(new Point(xPos, 0), new Point(xPos + delta, midpoint)));
                    }
                    else
                    {
                        var val = 1 - (minP.Y - yMin) / deltaY;
                        context.DrawRectangle(SparkLine.HighPointHighlightBrush, null,
                                    new Rect(new Point(xPos, midpoint), new Point(xPos + delta, actualHeight)));
                    }
                }
                VisualCollection.Add(visual3);
            }

            if (SparkLine.IsLowPointHighlighted)
            {
                Point maxP = Points.OrderBy(p => p.Y).FirstOrDefault();
                xPos = maxP.X * deltaX + (deltaX - delta) / 2;

                var visual4 = new SyncDrawingVisual();
                visual4.Index = 0;
                using (DrawingContext context = visual4.RenderOpen())
                {
                    if (maxP.Y > 0)
                    {
                        var val = 1 - (maxP.Y - yMin) / deltaY;
                        context.DrawRectangle(SparkLine.LowPointHighlightBrush, null,
                                    new Rect(new Point(xPos, 0), new Point(xPos + delta, midpoint)));
                    }
                    else
                    {
                        var val = 1 - (maxP.Y - yMin) / deltaY;
                        context.DrawRectangle(SparkLine.LowPointHighlightBrush, null,
                                    new Rect(new Point(xPos, midpoint), new Point(xPos + delta, actualHeight)));
                    }
                }
                VisualCollection.Add(visual4);
            }
            if (SparkLine.ShowAxis)
            {
                var visual5 = new SyncDrawingVisual();
                visual5.Index = 0;
                using (DrawingContext context = visual5.RenderOpen())
                {
                    context.DrawLine(SparkLine.OriginLineStroke, new Point(0, midpoint), new Point(ActualWidth, midpoint));
                }
                VisualCollection.Add(visual5);
            }
        }

        private void DrawColumn()
        {
            double deltaX = ActualWidth / Points.Count;
            double mindata = SparkLine.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? SparkLine.Data.Min() : SparkLine.VerticalAxisMinimumValue;
            double maxdata = SparkLine.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? SparkLine.Data.Max() : SparkLine.VerticalAxisMaximumValue;
            double deltaY = maxdata - mindata;
            deltaY = (deltaY == 0 ? 1 : deltaY);
            double delta = ActualWidth / Points.Count * 0.6;
            double xPos = (deltaX - delta) / 2;
            double yMin = mindata;
            double actualHeight = this.ActualHeight;
            Pen normalpen = new Pen(SparkLine.Interior, SparkLine.StrokeThickness);
            Pen negetivepen = new Pen(SparkLine.NegativePointsHighlightBrush, SparkLine.StrokeThickness);
            double origin = SparkLine.Origin;

            origin = actualHeight *(1 - (origin - yMin) / deltaY);
            for (var i = 0; i < Points.Count; i++)
            {
                var visual = new SyncDrawingVisual();
                visual.Index = i;
                using (DrawingContext context = visual.RenderOpen())
                {
                    var val = 1 - (Points[i].Y - yMin) / deltaY;
                    if (Points[i].Y > 0 || !SparkLine.IsNegativePointsHighlighted)
                    {
                        context.DrawRectangle(SparkLine.Interior,null,
                            new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)));
                    }
                    else
                    {
                        context.DrawRectangle(SparkLine.NegativePointsHighlightBrush, null,
                             new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)));
                    }
                    xPos += deltaX;

                }
                VisualCollection.Add(visual);
            }
          

            xPos -= deltaX;
            if (SparkLine.IsLastPointHighlighted)
            {
                var visual2 = new SyncDrawingVisual();
                visual2.Index = 0;
                using (DrawingContext context = visual2.RenderOpen())
                {
                    var val = 1 - (Points.LastOrDefault().Y - yMin) / deltaY;
                    context.DrawRectangle(SparkLine.LastPointHighlightBrush, null,
                                new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)));
                }
                VisualCollection.Add(visual2);
            }
            if (SparkLine.IsFirstPointHighlighted)
            {
                xPos = (deltaX - delta) / 2;
                var visual1 = new SyncDrawingVisual();
                visual1.Index = 0;
                using (DrawingContext context = visual1.RenderOpen())
                {
                    if (Points.Count > 0)
                    {
                        var val = 1 - (Points[0].Y - yMin) / deltaY;
                        context.DrawRectangle(SparkLine.FirstPointHighlightBrush, null,
                                    new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)));
                    }
                }
                VisualCollection.Add(visual1);
            }
            if (SparkLine.IsHighPointHighlighted)
            {
                Point minP = Points.OrderByDescending(p => p.Y).FirstOrDefault();
                xPos = (minP.X * deltaX) + (deltaX - delta) / 2;

                var visual3 = new SyncDrawingVisual();
                visual3.Index = 0;
                using (DrawingContext context = visual3.RenderOpen())
                {
                    var val = 1 - (minP.Y - yMin) / deltaY;
                    context.DrawRectangle(SparkLine.HighPointHighlightBrush, null,
                                new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)));
                }
                VisualCollection.Add(visual3);
            }
            if (SparkLine.IsLowPointHighlighted)
            {
                Point maxP = Points.OrderBy(p => p.Y).FirstOrDefault();
                xPos = maxP.X * deltaX + (deltaX - delta) / 2;

                var visual4 = new SyncDrawingVisual();
                visual4.Index = 0;
                using (DrawingContext context = visual4.RenderOpen())
                {
                    var val = 1 - (maxP.Y - yMin) / deltaY;
                    context.DrawRectangle(SparkLine.LowPointHighlightBrush, null,
                                new Rect(new Point(xPos, actualHeight * val), new Point(xPos + delta, origin)));
                }
                VisualCollection.Add(visual4);
            }

            if (SparkLine.ShowAxis)
            {
                var visual5 = new SyncDrawingVisual();
                visual5.Index = 0;
                using (DrawingContext context = visual5.RenderOpen())
                {
                    context.DrawLine(SparkLine.OriginLineStroke, new Point(0, origin), new Point(ActualWidth, origin));
                }
                VisualCollection.Add(visual5);
            }
        }

        private void DrawLine()
        {
            double deltaX = ActualWidth / Points.Count;
            double mindata = SparkLine.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? SparkLine.Data.Min() : SparkLine.VerticalAxisMinimumValue;
            double maxdata = SparkLine.VerticalAxisEndPointMode == AxisEndPointMode.Auto ? SparkLine.Data.Max() : SparkLine.VerticalAxisMaximumValue;
            double deltaY = maxdata - mindata;
            deltaY = (deltaY == 0 ? 0.5 : deltaY);
            double xPos = deltaX / 2;
            double yMin = mindata;
            double actualHeight = this.ActualHeight;
            Pen normalpen = new Pen(SparkLine.Interior, SparkLine.StrokeThickness);
            Pen negetivepen = new Pen(SparkLine.NegativePointsHighlightBrush, SparkLine.StrokeThickness);
            double origin = SparkLine.Origin;

            origin = actualHeight *(1 - ((origin - yMin) / deltaY));

            for (var i = 0; i < Points.Count - 1; i++)
            {
                var visual = new SyncDrawingVisual();
                visual.Index = i;
                using (DrawingContext context = visual.RenderOpen())
                {
                    var val = 1 - (Points[i].Y - yMin) / deltaY;
                    var val2 = 1 - (Points[i+1].Y - yMin) / deltaY;
                    context.DrawLine(normalpen,
                                    new Point(xPos, actualHeight * val),
                                    new Point(xPos = (xPos + deltaX), actualHeight * val2));                    
                    
                }
                VisualCollection.Add(visual);
            }
            if (SparkLine.IsMarkerEnabled || SparkLine.IsNegativePointsHighlighted)
            {
                xPos = deltaX / 2;
                for (var i = 0; i < Points.Count; i++)
                {
                    var visual5 = new SyncDrawingVisual();
                    visual5.Index = i;
                    using (DrawingContext context = visual5.RenderOpen())
                    {
                        var val = 1 - (Points[i].Y - yMin) / deltaY;
                                            
                        if (SparkLine.IsNegativePointsHighlighted && Points[i].Y < 0)
                        {
                            if (SparkLine.LineMarkerType == LineMarkerTypes.Square)
                            {
                                context.DrawRectangle(SparkLine.NegativePointsHighlightBrush, null,
                                        new Rect(new Point(xPos - 2.5, actualHeight * val - 2.5), new Size(5, 5)));
                            }
                            else 
                            {
                                context.DrawEllipse(SparkLine.NegativePointsHighlightBrush, null,new Point(xPos, actualHeight * val),2.5,2.5);
                            }
                        }
                        
                        else if (SparkLine.IsMarkerEnabled)
                        {
                            if (SparkLine.LineMarkerType == LineMarkerTypes.Square)
                            {
                                context.DrawRectangle(SparkLine.MarkerColor, null,
                                    new Rect(new Point(xPos - 2.5, actualHeight * val - 2.5), new Size(5, 5)));
                            }
                            else
                            {
                                context.DrawEllipse(SparkLine.MarkerColor, null, new Point(xPos, actualHeight * val), 2.5, 2.5);
                            }
                        }   
                    }
                    xPos = xPos + deltaX;
                    VisualCollection.Add(visual5);
                }
                xPos = xPos - deltaX;
            }
            if (SparkLine.IsFirstPointHighlighted)
            {
                var visual1 = new SyncDrawingVisual();
                visual1.Index = 0;
                using (DrawingContext context = visual1.RenderOpen())
                {
                    if (Points.Count > 0)
                    {
                        var val = 1 - (Points[0].Y - yMin) / deltaY;
                        if (SparkLine.LineMarkerType == LineMarkerTypes.Square)
                        {
                            context.DrawRectangle(SparkLine.FirstPointHighlightBrush, null,
                                       new Rect(new Point(deltaX / 2 - 2.5, actualHeight * val - 2.5), new Size(5, 5)));
                        }
                        else
                        {
                            context.DrawEllipse(SparkLine.FirstPointHighlightBrush, null,new Point(deltaX / 2, actualHeight * val), 2.5, 2.5);
                        }
                    }
                }
                VisualCollection.Add(visual1);
            }
            if (SparkLine.IsLastPointHighlighted)
            {
                
                var visual2 = new SyncDrawingVisual();
                visual2.Index = 0;
                using (DrawingContext context = visual2.RenderOpen())
                {
                    var val = 1 - (Points.LastOrDefault().Y - yMin) / deltaY;
                    if (SparkLine.LineMarkerType == LineMarkerTypes.Square)
                    {
                        context.DrawRectangle(SparkLine.LastPointHighlightBrush, null,
                                   new Rect(new Point(xPos - 2.5, actualHeight * val - 2.5), new Size(5, 5)));
                    }
                    else
                    {
                        context.DrawEllipse(SparkLine.LastPointHighlightBrush, null, new Point(xPos, actualHeight * val), 2.5, 2.5);
                    }
                }
                VisualCollection.Add(visual2);
            }

            if (SparkLine.IsHighPointHighlighted)
            {

                Point maxp = Points.OrderByDescending(p => p.Y).FirstOrDefault();
                var visual3 = new SyncDrawingVisual();
                visual3.Index = 0;
                using (DrawingContext context = visual3.RenderOpen())
                {
                    var val = 1 - (maxp.Y - yMin) / deltaY;
                    if (SparkLine.LineMarkerType == LineMarkerTypes.Square)
                    {
                        context.DrawRectangle(SparkLine.HighPointHighlightBrush, null,
                                   new Rect(new Point(maxp.X * deltaX + (deltaX / 2) -2.5, actualHeight * val-2.5), new Size(5, 5)));
                    }
                    else
                    {
                        context.DrawEllipse(SparkLine.HighPointHighlightBrush, null, new Point(maxp.X * deltaX + (deltaX / 2), actualHeight * val ), 2.5, 2.5);
                    }
                }
                VisualCollection.Add(visual3);
            }

            if (SparkLine.IsLowPointHighlighted)
            {
                Point minp = Points.OrderBy(p => p.Y).FirstOrDefault();
                var visual4 = new SyncDrawingVisual();
                visual4.Index = 0;
                using (DrawingContext context = visual4.RenderOpen())
                {
                    var val = 1 - (minp.Y - yMin) / deltaY;
                    if (SparkLine.LineMarkerType == LineMarkerTypes.Square)
                    {
                        context.DrawRectangle(SparkLine.LowPointHighlightBrush, null,
                                   new Rect(new Point(minp.X * deltaX + (deltaX / 2) - 2.5, actualHeight * val - 2.5), new Size(5, 5)));
                    }

                    else
                    {
                        context.DrawEllipse(SparkLine.LowPointHighlightBrush, null, new Point(minp.X * deltaX + (deltaX / 2) , actualHeight * val), 2.5, 2.5);
                    }
                }
                VisualCollection.Add(visual4);
            }
            if (SparkLine.ShowAxis)
            {
                var visual6 = new SyncDrawingVisual();
                visual6.Index = 0;
                using (DrawingContext context = visual6.RenderOpen())
                {
                    context.DrawLine(SparkLine.OriginLineStroke, new Point(0, origin), new Point(ActualWidth, origin));
                }
                VisualCollection.Add(visual6);
            }
        }
    }
}
