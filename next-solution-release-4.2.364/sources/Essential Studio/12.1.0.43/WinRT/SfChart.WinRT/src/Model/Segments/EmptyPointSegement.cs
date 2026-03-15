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
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;
#else
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
    /// Represents chart empty point segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <see cref="ChartSeriesBase.ShowEmptyPoints"/>
    [ClassReference(IsReviewed = false)]
    public class EmptyPointSegment : ScatterSegment
    {
        private double ypos;

        private double xpos;

        private double emptyPointSymbolHeight = 20;

        private double emptyPointSymbolWidth = 20;

        /// <summary>
        /// Gets or sets empty point symbol height.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double EmptyPointSymbolHeight
        {
            get
            {
                return emptyPointSymbolHeight;
            }
            set
            {
                emptyPointSymbolHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets empty point symbol width.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double EmptyPointSymbolWidth
        {
            get
            {
                return emptyPointSymbolWidth;
            }
            set
            {
                emptyPointSymbolWidth = value;
            }
        }

        /// <summary>
        /// Gets or Sets the x coordinate of this segment
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double X
        {
            get
            {

                return xpos;
            }
            set
            {
                xpos = value;
                OnPropertyChanged("X");
            }
        }

        /// <summary>
        /// Gets or Sets the y coordinate of this segment
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Y
        {

            get
            {

                return ypos;
            }
            set
            {
                ypos = value;
                OnPropertyChanged("Y");
            }
        }
        /// <summary>
        /// Called when instance created for EmptyPointSegment with following arguments
        /// </summary>
        /// <param name="xData"></param>
        /// <param name="yData"></param>
        /// <param name="series"></param>
        /// <param name="isEmptyPointInterior"></param>
        public EmptyPointSegment(double xData, double yData, ChartSeriesBase series, bool isEmptyPointInterior)
        {
            base.Series = series;
            ScatterWidth = EmptyPointSymbolWidth;
            ScatterHeight = EmptyPointSymbolHeight;
            this.IsEmptySegmentInterior = isEmptyPointInterior;
            strokeThickness = 1d;
            this.XData = xData;
            this.YData = yData;
            base.SetData(xData, yData);
        }


        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="BottomLeft"></param>
        /// <param name="RightTop"></param>
        /// <param name="hipoint"></param>
        /// <param name="loPoint"></param>
        /// <param name="isBull"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(Point BottomLeft, Point RightTop, Point hipoint, Point loPoint, bool isBull)
        {

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
        /// <param name="isBull"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(Point hipoint, Point lopoint, Point sopoint, Point eopoint, Point scpoint, Point ecpoint, bool isBull)
        {

        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(Point point1, Point point2, Point point3, Point point4)
        {

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
            if (Series.EmptyPointSymbolTemplate == null)
            {
                Ellipse ellipse = base.CreateVisual(size) as Ellipse;
                ellipse.Fill = ellipse.Stroke = this.Interior;
                return ellipse;
            }
            else
            {
                ContentControl control = new ContentControl();
                control.Content = this;
                control.Width = EmptyPointSymbolWidth;
                control.Height = EmptyPointSymbolHeight;
                control.ContentTemplate = Series.EmptyPointSymbolTemplate;
                return control;
            }
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
            if (transformer != null)
            {
                Point position = transformer.TransformToVisible(XData, YData);
                this.X = position.X - (EmptyPointSymbolWidth / 2);
                this.Y = position.Y - (EmptyPointSymbolHeight / 2);
            }
            if (Series.EmptyPointSymbolTemplate == null)
                base.Update(transformer);
        }
    }
}
