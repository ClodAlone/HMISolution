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
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart scatter segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="ScatterSeries"/>
    [ClassReference(IsReviewed = false)]
    public class ScatterSegment:ChartSegment
    {
        private double scatterWidth;
        private double scatterHeight;
        private ScatterSeries containerSeries;

        private double xPos, YPos = 0;
       
        /// <summary>
        /// EllipseSegment property declarations
        /// </summary>
        protected Ellipse EllipseSegment;
        /// <summary>
        /// strokeThickness property declarations
        /// </summary>
        protected double strokeThickness;

        /// <summary>
        /// Gets the x data of this segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double XData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the y data of this segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double YData
        {

            get;
            internal set;
        }

        /// <summary>
        /// Gets or Sets the width of this segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double ScatterWidth
        {
            get
            {
                return scatterWidth;
            }
            set
            {
                if (scatterWidth != value)
                {
                    scatterWidth = value;
                    OnPropertyChanged("ScatterWidth");
                }
            }
        }

        /// <summary>
        /// Gets or Sets the height of this segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double ScatterHeight
        {
            get
            {
                return scatterHeight;
            }
            set
            {
                if (scatterHeight != value)
                {
                    scatterHeight = value;
                    OnPropertyChanged("ScatterHeight");
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ScatterSegment()
        {

        }
        /// <summary>
        /// Called when instance created for Scattersegment
        /// </summary>
        /// <param name="xpos"></param>
        /// <param name="ypos"></param>
        /// <param name="series"></param>
        public ScatterSegment(double xpos,double ypos, ScatterSeries series)
        {
            base.Series = series;
            this.ScatterWidth = series.ScatterWidth;
            this.ScatterHeight = series.ScatterHeight;
            containerSeries = series;
            strokeThickness = series.StrokeThickness;
            SetData(xpos, ypos);
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(params double[] Values)
        {
            XData = Values[0];
            YData = Values[1];
            xPos = Values[0];
            YPos = Values[1];
            if (!double.IsNaN(xPos))
                XRange = DoubleRange.Union(xPos);
            else
                XRange = DoubleRange.Empty;
            if (!double.IsNaN(YPos))
                YRange = DoubleRange.Union(YPos);
            else
                YRange = DoubleRange.Empty;     
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// retuns UIElement
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement CreateVisual(Size size)
        {
            EllipseSegment = new Ellipse();
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ScatterWidth");
            EllipseSegment.Tag = this;
            EllipseSegment.SetBinding(Ellipse.WidthProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ScatterHeight");
            EllipseSegment.SetBinding(Ellipse.HeightProperty, binding);
            EllipseSegment.Tag = this;
            SetVisualBindings(EllipseSegment);
            return EllipseSegment;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return EllipseSegment;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public override void Update(IChartTransformer transformer)
        {
            ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
            if (cartesianTransformer != null)
            {
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double edgeValue = xIsLogarithmic ? Math.Log(xPos, xBase) : xPos;
                if (edgeValue >= xStart && edgeValue <= xEnd && (!double.IsNaN(YData) || Series.ShowEmptyPoints))
                {
                    if (this is EmptyPointSegment)
                    {
                        ScatterHeight = (this as EmptyPointSegment).EmptyPointSymbolHeight;
                        ScatterWidth = (this as EmptyPointSegment).EmptyPointSymbolWidth;
                    }
                    else
                    {
                        ScatterHeight = (Series as ScatterSeries).ScatterHeight;
                        ScatterWidth = (Series as ScatterSeries).ScatterWidth;
                    }
                    Point point1 = transformer.TransformToVisible(xPos, YPos);
                    EllipseSegment.SetValue(Canvas.LeftProperty, point1.X - ScatterWidth / 2);
                    EllipseSegment.SetValue(Canvas.TopProperty, point1.Y - ScatterHeight / 2);
                }
                else
                {
                    ScatterHeight = 0;
                    ScatterWidth = 0;
                }
            }
            else
            {
                ScatterWidth = 0;
                ScatterHeight = 0;
            }
        }

        /// <summary>
        /// Method Implementation for set  Binding to CgartSegments properties
        /// </summary>
        /// <param name="element"></param>
        [ClassReference(IsReviewed = false)]
        protected override void SetVisualBindings(Shape element)
        {
            base.SetVisualBindings(element);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty, binding);
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        [ClassReference(IsReviewed = false)]
        public override void OnSizeChanged(Size size)
        {
                        
        }
    }
}
