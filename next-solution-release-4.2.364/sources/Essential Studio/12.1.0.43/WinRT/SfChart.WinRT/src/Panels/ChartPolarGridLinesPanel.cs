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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
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
    /// Represents ChartPolarGridLinesPanel
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartPolarGridLinesPanel: ILayoutCalculator
    {
        #region fields

        private Size desiredSize;
        private Panel panel;
        private UIElementsRecycler<Ellipse> ellipseRecycler;
        private UIElementsRecycler<Line> linesRecycler;

        #endregion

        #region properties
        /// <summary>
        /// Checks whether the Series is Radar/Polar Series type.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool IsRadar
        {
            get
            {
                if (Area != null)
                {
                    return Area.VisibleSeries[0] is RadarSeries;
                }
                return false;
            }
        }

        public Panel Panel
        {
            get { return panel; }
        }

        internal SfChart Area
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets the x-axis of the chart.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartAxis XAxis
        {
            get
            {
                return Area.InternalPrimaryAxis;
            }
        }

        /// <summary>
        ///Gets the y-axis of the chart.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartAxis YAxis
        {
            get
            {
                return Area.InternalSecondaryAxis;
            }
        }

        /// <summary>
        /// Gets the desired position of the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Size DesiredSize
        {
            get { return desiredSize; }
        }

        /// <summary>
        /// Gets the children count in the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public List<UIElement> Children
        {
            get 
            {
                if (panel != null)
                {
                    return panel.Children.Cast<UIElement>().ToList();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public double Left
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public double Top
        {
            get;
            set;
        }
        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartPolargridLinesPanel
        /// </summary>
        /// <param name="panel"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ChartPolarGridLinesPanel(Panel panel)
        {
            if (panel == null)
                throw new ArgumentNullException();

            this.panel = panel;
            ellipseRecycler = new UIElementsRecycler<Ellipse>(panel);
            linesRecycler = new UIElementsRecycler<Line>(panel);
        }

        #endregion

        #region methods
        /// <summary>
        /// Measures the elements of a panel.
        /// </summary>
        /// <param name="availableSize">available size of the panel.</param>
        /// <returns>returns Size.</returns>
        [ClassReference(IsReviewed = false)]
        public Size Measure(Size availableSize)
        {
            desiredSize = new Size(Area.SeriesClipRect.Width, Area.SeriesClipRect.Height);

            if(!IsRadar)
                RenderCircles();

            return availableSize;
        }

        /// <summary>
        /// Arranges the elements of a panel.
        /// </summary>
        /// <param name="finalSize">final size of the panel.</param>
        /// <returns>returns Size</returns>
        [ClassReference(IsReviewed = false)]
        public Size Arrange(Size finalSize)
        {
            RenderGridLines();
            return finalSize;
        }

        public void DetachElements()
        {
            if (ellipseRecycler != null)
                ellipseRecycler.Clear();

            if(linesRecycler != null)
                linesRecycler.Clear();
            panel = null;
        }

        /// <summary>
        /// Adds the elements to the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public void UpdateElements()
        {
            int count = 0;
            if (this.YAxis!=null)
            count=this.YAxis.VisibleLabels.Count;

            int totalLinesCount = 0;
            if (!linesRecycler.BindingProvider.Keys.Contains(Line.StyleProperty) && this.Area.InternalPrimaryAxis!=null)
            {
                Binding binding = new Binding();
                binding.Path = new PropertyPath("MajorGridLineStyle");
                binding.Source = this.Area.InternalPrimaryAxis;
                linesRecycler.BindingProvider.Add(Line.StyleProperty, binding);
            }

            if (!IsRadar)
            {
                ellipseRecycler.GenerateElements(count);

                foreach (Ellipse ellipse in ellipseRecycler)
                {
                    ellipse.Stroke = new SolidColorBrush(Colors.Gray);
                    ellipse.StrokeThickness = 1;
                }
            }
            else if (this.XAxis != null)
            {
                ellipseRecycler.Clear();
                totalLinesCount = count * this.XAxis.VisibleLabels.Count;
                linesRecycler.GenerateElements(totalLinesCount);
            }

            if(this.XAxis!=null)
            count = this.XAxis.VisibleLabels.Count;

            linesRecycler.GenerateElements(totalLinesCount + count);
        }

        internal void RenderCircles()
        {
            ChartAxis xAxis = this.XAxis;
            ChartAxis yAxis = this.YAxis;

            double bigRadius = Math.Min(this.DesiredSize.Width, this.DesiredSize.Height) / 2;
            Point center = new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);

            if (yAxis != null && yAxis.ShowGridLines 
                && ellipseRecycler.Count > 0)
            {
                int pos = 0;
                foreach (ChartAxisLabel label in yAxis.VisibleLabels)
                {
                    double radius = bigRadius * yAxis.ValueToCoefficientCalc(label.Position);
                    Ellipse ellipse = ellipseRecycler[pos];
                    ellipse.Width = radius * 2;
                    ellipse.Height = radius * 2;

                    Canvas.SetLeft(ellipse, center.X - radius);
                    Canvas.SetTop(ellipse, center.Y - radius);

                    pos++;
                }
            }
        }

        void RenderGridLines()
        {
            ChartAxis xAxis = this.XAxis;
            ChartAxis yAxis = this.YAxis;

            double bigRadius = Math.Min(this.DesiredSize.Width, this.DesiredSize.Height) / 2;
            Point center = new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);
            int pos = 0;
            if (IsRadar && yAxis != null && yAxis.ShowGridLines)
            {
                foreach (ChartAxisLabel label in yAxis.VisibleLabels)
                {
                    double radius = bigRadius * yAxis.ValueToCoefficientCalc(label.Position);
                    for (int i = 0; i < xAxis.VisibleLabels.Count; i++)
                    {
                        Point vector = ChartTransform.ValueToVector(xAxis, xAxis.VisibleLabels[i].Position);
                        Point vector2 = new Point();
                        if ((i + 1) < xAxis.VisibleLabels.Count)
                        {
                            vector2 = ChartTransform.ValueToVector(xAxis, xAxis.VisibleLabels[i + 1].Position);
                        }
                        else
                        {
                            vector2 = ChartTransform.ValueToVector(xAxis, xAxis.VisibleLabels[0].Position);
                        }
                        Point connectPoint = new Point(center.X + radius * vector.X, center.Y + radius * vector.Y);
                        Point endPoint = new Point(center.X + radius * vector2.X, center.Y + radius * vector2.Y);

                        Line line = linesRecycler[pos];
                        line.X1 = connectPoint.X;
                        line.Y1 = connectPoint.Y;
                        line.X2 = endPoint.X;
                        line.Y2 = endPoint.Y;
                        pos++;
                    }
                }
            }

            if (xAxis != null && xAxis.ShowGridLines)
            {
                //Pen pen = ChartArea.GetGridLineStroke(xAxis);
                
                foreach (ChartAxisLabel label in xAxis.VisibleLabels)
                {
                    Point vector = ChartTransform.ValueToVector(xAxis, label.Position);
                    Line line = linesRecycler[pos];
                    line.X1 = center.X;
                    line.Y1 = center.Y;
                    line.X2 = center.X + bigRadius * vector.X;
                    line.Y2 = center.Y + bigRadius * vector.Y;
                    pos++;
                }
            }
        }


        #endregion
    }
}
