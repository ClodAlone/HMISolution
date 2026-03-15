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
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Data;
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
    /// Represents chart HiLoOpenClose segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="HiLoOpenCloseSeries"/>
    [ClassReference(IsReviewed = false)]
    public class HiLoOpenCloseSegment : ChartSegment
    {
        #region fields
        
        private Canvas canvas;
        private Point hipoint;
        private Point lowpoint;
        private Point sopoint;
        private Point eopoint;
        private Point scpoint;
        private Point ecpoint;
        
        private HiLoOpenCloseSeries parentSeries;

        private Line hiLoline;

        private Line closeLine;

        private Line openLine;

        private bool isbull;

        private Brush bullFillColor, bearFillColor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the actual color used to paint the interior of the segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush ActualInterior
        {
            get
            {
                return isbull
                    ? BullFillColor : BearFillColor;
            }
        }

        /// <summary>
        /// Gets the high value interior
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush BearFillColor
        {
            get
            {
                return bearFillColor == null
                    ? this.Interior : bearFillColor;
            }
            set
            {
                if (bearFillColor != value)
                {
                    bearFillColor = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        /// <summary>
        /// Gets the low value interior
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush BullFillColor
        {
            get
            {
                return bullFillColor == null
                    ? this.Interior : bullFillColor;
            }
            set
            {
                if (bullFillColor != value)
                {
                    bullFillColor = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        public double High { get; set; }

        public double Low { get; set; }

        public double Open { get; set; }

        public double Close { get; set; }

        #endregion
        #region constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public HiLoOpenCloseSegment()
        {

        }

        /// <summary>
        /// Called when instance created for HiLoOpenCloseSegment
        /// </summary>
        /// <param name="hghpoint"></param>
        /// <param name="lowpoint"></param>
        /// <param name="sopoint"></param>
        /// <param name="eopoint"></param>
        /// <param name="scpoint"></param>
        /// <param name="ecpoint"></param>
        /// <param name="isbull"></param>
        /// <param name="series"></param>
        public HiLoOpenCloseSegment(Point hghpoint, Point lowpoint,Point sopoint,Point eopoint, Point scpoint, Point ecpoint,bool isbull, HiLoOpenCloseSeries series, object item)
        {
            base.Series = series;
            parentSeries = series;
            BullFillColor = series.BullFillColor;
            BearFillColor = series.BearFillColor;
            base.Item = item;
            SetData(hghpoint, lowpoint, sopoint, eopoint, scpoint, ecpoint, isbull);
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="hipoint"></param>
        /// <param name="lopoint"></param>
        /// <param name="sopoint"></param>
        /// <param name="eopoint"></param>
        /// <param name="scpoint"></param>
        /// <param name="ecpoint"></param>
        /// <param name="isbull"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(Point hipoint, Point lopoint,Point sopoint,Point eopoint, Point scpoint, Point ecpoint,bool isbull)
        {
            this.hipoint = hipoint;
            this.lowpoint = lopoint;
            this.sopoint = sopoint;
            this.eopoint = eopoint;
            this.scpoint = scpoint;
            this.ecpoint = ecpoint;
            this.isbull = isbull;
            XRange = new DoubleRange(ChartMath.Min(scpoint.X, ecpoint.X, sopoint.X, eopoint.X), ChartMath.Max(scpoint.X, ecpoint.X, sopoint.X, eopoint.X));
            YRange = new DoubleRange(lopoint.Y, hipoint.Y);
        }
        #endregion

        #region methods

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
            canvas = new Canvas();
            hiLoline = new Line();

            SetVisualBindings(hiLoline);
            canvas.Children.Add(hiLoline);

            openLine = new Line();
            SetVisualBindings(openLine);
            canvas.Children.Add(openLine);

            closeLine = new Line();
            SetVisualBindings(closeLine);
            canvas.Children.Add(closeLine);
            hiLoline.Tag = openLine.Tag = closeLine.Tag = this;
            return canvas;
        }

        /// <summary>
        /// Method Implementation for set  Binding to CgartSegments properties
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ActualInterior");
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return canvas;
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
            if (transformer != null && hipoint != null && lowpoint != null)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double soPointX = xIsLogarithmic ? Math.Log(sopoint.X, xBase) : sopoint.X;
                double scPointX = xIsLogarithmic ? Math.Log(scpoint.X, xBase) : scpoint.X;
                double ecPointX = xIsLogarithmic ? Math.Log(ecpoint.X, xBase) : ecpoint.X;
                if ((soPointX >= xStart && scPointX <= xEnd ||
                      ecPointX >= xStart && ecPointX <= xEnd)
                    && ((!double.IsNaN(hipoint.Y) && !double.IsNaN(lowpoint.Y) && !double.IsNaN(sopoint.Y) && !double.IsNaN(eopoint.Y)) || Series.ShowEmptyPoints)
                    )
                {
                    Point hiPoint = transformer.TransformToVisible(hipoint.X, hipoint.Y);
                    Point loPoint = transformer.TransformToVisible(lowpoint.X, lowpoint.Y);
                    Point startopenpoint = transformer.TransformToVisible(sopoint.X, sopoint.Y);
                    Point endopenpoint = transformer.TransformToVisible(eopoint.X, eopoint.Y);
                    Point startclosepoint = transformer.TransformToVisible(scpoint.X, scpoint.Y);
                    Point endclosepoint = transformer.TransformToVisible(ecpoint.X, ecpoint.Y);

                    hiLoline.X1 = hiPoint.X;
                    hiLoline.Y1 = hiPoint.Y;
                    hiLoline.X2 = loPoint.X;
                    hiLoline.Y2 = loPoint.Y;

                    this.openLine.X1 = startopenpoint.X;
                    this.openLine.Y1 = startopenpoint.Y;
                    this.openLine.X2 = endopenpoint.X;
                    this.openLine.Y2 = endopenpoint.Y;

                    this.closeLine.X1 = startclosepoint.X;
                    this.closeLine.Y1 = startclosepoint.Y;
                    this.closeLine.X2 = endclosepoint.X;
                    this.closeLine.Y2 = endclosepoint.Y;
                }
                else
                {
                    hiLoline.ClearUIValues();
                    openLine.ClearUIValues();
                    closeLine.ClearUIValues();
                }

            }
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

        /// <summary>
        /// Called when Property changed 
        /// </summary>
        /// <param name="name"></param>
        protected override void OnPropertyChanged(string name)
        {
            if (name == "Interior")
                name = "ActualInterior";
            base.OnPropertyChanged(name);
        }
        #endregion

    }
}
