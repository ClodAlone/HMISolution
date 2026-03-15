#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents layout panel for chart axis labels.
    /// </summary>
    /// <remarks>
    /// The elements inside the panel comprises of <see cref="ChartAxis"/> labels.You can customize the label elements appearance using  
    /// <see cref="ChartAxis.LabelTemplate"/> property.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ChartCartesianAxisLabelsPanel : ILayoutCalculator
    {
        #region fields

        private Panel labelsPanels;

        private Size desiredSize;

        UIElementsRecycler<TextBlock> textBlockRecycler;

        UIElementsRecycler<ContentControl> contentControlRecycler;

        #endregion

        #region properties

        public Panel Panel
        {
            get { return labelsPanels; }
        }

        /// <summary>
        /// Gets the desired size of the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Size DesiredSize
        {
            get
            {
                return desiredSize;
            }
        }

        /// <summary>
        /// Gets or Sets the chart axis of the panel./>
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartAxis Axis { get; set; }

        private List<UIElement> children;

        /// <summary>
        /// Gets the children count in the panel.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public List<UIElement> Children
        {
            get
            {
                children = textBlockRecycler.generatedElements.Cast<UIElement>().ToList();
                if (children.Count < 1)
                    children = contentControlRecycler.generatedElements.Cast<UIElement>().ToList();
                return children;
            }
        }

        private AxisLabelLayout labelLayout = null;

        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartCartessianAxisLabelsPanel
        /// </summary>
        /// <param name="panel"></param>
        public ChartCartesianAxisLabelsPanel(Panel panel)
        {
            labelsPanels = panel;
            textBlockRecycler = new UIElementsRecycler<TextBlock>(panel);
            contentControlRecycler = new UIElementsRecycler<ContentControl>(panel);
        }

        #endregion

        #region methods

        /// <summary>
        /// Method declaration for Measure
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public Size Measure(Size availableSize)
        {
            labelLayout = AxisLabelLayout.CreateAxisLayout(Axis, Children);
            desiredSize = labelLayout.Measure(availableSize);
            return desiredSize;
        }

        /// <summary>
        /// Method declaration for Arrange
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        [ClassReference(IsReviewed = false)]
        public Size Arrange(Size finalSize)
        {
            if (labelLayout != null)
            {
                labelLayout.Left = Left;
                labelLayout.Top = Top;
                labelLayout.Arrange(DesiredSize);
                labelLayout = null;
            }
            return finalSize;
        }

        public void DetachElements()
        {
            labelsPanels = null;
            if(textBlockRecycler != null)
                textBlockRecycler.Clear();
            if (contentControlRecycler != null)
                contentControlRecycler.Clear();
        }

        internal void GenerateContainers()
        {
            int pos = 0;

            ObservableCollection<ChartAxisLabel> visibleLabels = Axis.VisibleLabels;
            var prefixLabeltemplate = this.Axis.PrefixLabelTemplate;
            var postfixLabelTemplate = this.Axis.PostfixLabelTemplate;
            if (Axis.LabelTemplate == null && Axis.PrefixLabelTemplate == null && Axis.PostfixLabelTemplate == null)
            {
                contentControlRecycler.Clear();
                textBlockRecycler.GenerateElements(visibleLabels.Count);
                foreach (var item in visibleLabels)
                {
                    if (item.LabelContent != null)
                    {
                        var textblock = textBlockRecycler[pos];
                        textblock.Text = item.LabelContent.ToString();
                    }
                    pos++;
                }
            }
            else if (this.Axis.LabelTemplate == null)
            {
                textBlockRecycler.Clear();
                contentControlRecycler.GenerateElements(visibleLabels.Count);
                foreach (var item in visibleLabels)
                {
                    ContentControl control = contentControlRecycler[pos];
                    item.PrefixLabelTemplate = prefixLabeltemplate;
                    item.PostfixLabelTemplate = postfixLabelTemplate;
                    control.Content = item;
                    control.ContentTemplate = ChartDictionaries.GenericCommonDictionary["AxisLabelsCustomTemplate"] as DataTemplate;
                    control.ApplyTemplate();                    
                    pos++;
                }
            }
            else
            {
                textBlockRecycler.Clear();
                contentControlRecycler.GenerateElements(visibleLabels.Count);
                foreach (var item in visibleLabels)
                {
                    ContentControl control = contentControlRecycler[pos];
                    control.ContentTemplate = this.Axis.LabelTemplate;
                    control.ApplyTemplate();
                    control.Content = item;
                    pos++;
                }
            }
        }

        #endregion

        /// <summary>
        /// Method declaration for UpdateElements
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public void UpdateElements()
        {
            GenerateContainers();
        }


        public double Left
        {
            get;
            set;
        }

        public double Top
        {
            get;
            set;
        }
    }

    public abstract class AxisLabelLayout
    {
        internal double Left { get; set; }
        internal double Top { get; set; }

        protected List<Dictionary<int, Rect>> RectssByRowsAndCols { get; set; }

        /// <summary>
        /// Holds the width and height of the element after rotating.
        /// </summary>
        protected List<Size> ComputedSizes { get; set; }

        /// <summary>
        /// Holds the width and height of the element without rotating.
        /// </summary>
        protected List<Size> DesiredSizes { get; set; }

        private readonly List<UIElement> children;

        protected Thickness Margin = new Thickness(2, 2, 2, 2);

        protected ChartAxis Axis { get; set; }

        protected List<UIElement> Children
        {
            get { return children; }
        }

        protected AxisLabelLayout(ChartAxis axis, List<UIElement> elements)
        {
            Axis = axis;
            children = elements;
            DesiredSizes = new List<Size>();
        }

        protected bool IntersectsWith(Rect r1, Rect r2, int prevIndex, int currentIndex)
        {
            if (Axis.LabelRotationAngle != 0d)
            {
                var shape1Points = GetRotatedPoints(r1, prevIndex);
                var shape2Points = GetRotatedPoints(r2, currentIndex);
                return IntersectsWith(shape1Points, shape2Points);
            }

            return !(r2.Left > r1.Right ||
                     r2.Right < r1.Left ||
                     r2.Top > r1.Bottom ||
                     r2.Bottom < r1.Top);
        }

        public static AxisLabelLayout CreateAxisLayout(ChartAxis chartAxis, List<UIElement> elements)
        {
            if (chartAxis.Orientation == Orientation.Horizontal)
            {
                return new HorizontalLabelLayout(chartAxis, elements);
            }
            return new VerticalLabelLayout(chartAxis, elements);
        }

        /// <summary>
        /// Checks whether two line segments are intersecting
        /// </summary>
        /// <param name="point11"></param>
        /// <param name="point12"></param>
        /// <param name="point21"></param>
        /// <param name="point22"></param>
        /// <returns></returns>
        private bool DoLinesIntersect(Point point11, Point point12, Point point21, Point point22)
        {
            double d = (point22.Y - point21.Y)*(point12.X - point11.X) - 
                (point22.X - point21.X)*(point12.Y - point11.Y);
            double na = (point22.X - point21.X)*(point11.Y - point21.Y) -
                         (point22.Y - point21.Y)*(point11.X - point21.X);
            double nb = (point12.X - point11.X)*(point11.Y - point21.Y) -
                         (point12.Y - point11.Y)*(point11.X - point21.X);

            if (d == 0)
                return false;

            double ua = na/d;
            double ub = nb/d;

            return (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d);
        }

        Size GetRotatedSize(double angle, Size size)
        {
            var angleRadians = (2 * Math.PI * angle) / 360;
            var sine = Math.Sin(angleRadians);
            var cosine = Math.Cos(angleRadians);
            var matrix = new Matrix(cosine, sine, -sine, cosine, 0, 0);

            var leftTop = matrix.Transform(new Point(0, 0));
            var rightTop = matrix.Transform(new Point(size.Width, 0));
            var leftBottom = matrix.Transform(new Point(0, size.Height));
            var rightBottom = matrix.Transform(new Point(size.Width, size.Height));
            var left = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
            var top = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
            var right = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
            var bottom = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));

            return new Size(right - left, bottom - top);
        }

        /// <summary>
        /// Returns the points after rotating a rectangle.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        List<Point> GetRotatedPoints(Rect rect, int index)
        {
            //Get actual left and actual top of the element without rotating
            var left = rect.Left + (ComputedSizes[index].Width - DesiredSizes[index].Width) / 2;
            var top = rect.Top + (ComputedSizes[index].Height - DesiredSizes[index].Height) / 2;

            //Rotating the points about the origin (0,0) and translating it to actual left and top
            var offsetX = DesiredSizes[index].Width / 2;
            var offsetY = DesiredSizes[index].Height / 2;
            rect = new Rect(-offsetX, -offsetY, DesiredSizes[index].Width, DesiredSizes[index].Height);
            var translateX = left + offsetX;
            var translateY = top + offsetY;

            return GetRotatedPoints(Axis.LabelRotationAngle, rect, translateX, translateY);
        }

        /// <summary>
        /// Returns the points after translating the rect about (0,0) and then translating it by some x and y.
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="rect">Rect</param>
        /// <param name="translateX">Offset x to be translated after rotating</param>
        /// <param name="translateY">Offset y to be translated after rotating</param>
        /// <returns></returns>
        List<Point> GetRotatedPoints(double angle, Rect rect, double translateX, double translateY)
        {
            var angleRadians = (2 * Math.PI * angle) / 360;
            var sine = Math.Sin(angleRadians);
            var cosine = Math.Cos(angleRadians);
            var matrix = new Matrix(cosine, sine, -sine, cosine, translateX, translateY);
            var transformedPoints= new List<Point>();
            transformedPoints.Add(matrix.Transform(new Point(rect.Left, rect.Top)));
            transformedPoints.Add(matrix.Transform(new Point(rect.Right, rect.Top)));
            transformedPoints.Add(matrix.Transform(new Point(rect.Right, rect.Bottom)));
            transformedPoints.Add(matrix.Transform(new Point(rect.Left, rect.Bottom)));
            return transformedPoints;
        }

        /// <summary>
        /// Checks whether two polygons intersects.
        /// </summary>
        /// <param name="shape1Points">Polygon</param>
        /// <param name="shape2Points">Polygon</param>
        /// <returns></returns>
        private bool IntersectsWith(List<Point> shape1Points, List<Point> shape2Points)
        {
            //Checks whether two lines from both the shapes intersects. 
            //If it intersects, it means both the shapes are intersecting.
            for (int i = 0; i < shape1Points.Count; i++)
            {
                var point11 = shape1Points[i];
                var nextIndex = i == shape1Points.Count - 1 ? 0 : i + 1;
                var point12 = shape1Points[nextIndex];
                for (int j = 0; j < shape2Points.Count; j++)
                {
                    var point21 = shape2Points[j];
                    nextIndex = j == shape2Points.Count - 1 ? 0 : j + 1;
                    var point22 = shape2Points[nextIndex];

                    if (DoLinesIntersect(point11, point12, point21, point22))
                        return true;
                }
            }

            return false;
        }

        public virtual Size Measure(Size availableSize)
        {
            if (Axis != null && Children.Count > 0)
            {
                bool needToRotate = !double.IsNaN(Axis.LabelRotationAngle) && Axis.LabelRotationAngle != 0.0;
                ComputedSizes = DesiredSizes;
                double angle = Axis.LabelRotationAngle;
                if (needToRotate) ComputedSizes = new List<Size>();

                foreach (FrameworkElement element in Children)
                {
                    element.Visibility = Visibility.Visible;
                    element.Measure(availableSize);

#if SILVERLIGHT
                if(element is TextBlock)
                {
                     DesiredSizes.Add(new Size(element.ActualWidth, element.ActualHeight));
                }
                else
#endif
                    DesiredSizes.Add(element.DesiredSize);

                    if (needToRotate)
                    {
                        element.RenderTransformOrigin = new Point(0.5, 0.5);
                        element.RenderTransform = new RotateTransform() {Angle = angle};
                        ComputedSizes.Add(GetRotatedSize(angle, DesiredSizes.Last()));
                    }
                    else
                    {
                        element.RenderTransform = null;
                    }
                }
                CalculateActualPlotOffset(availableSize);
            }
            return new Size();
        }

        protected void InsertToRowOrColumn(int rowOrColIndex, int itemIndex, Rect rect)
        {
            if (RectssByRowsAndCols.Count <= rowOrColIndex)
            {
                RectssByRowsAndCols.Add(new Dictionary<int, Rect>());
                RectssByRowsAndCols[rowOrColIndex].Add(itemIndex, rect);
            }
            else
            {
                var rowOrColumn = RectssByRowsAndCols[rowOrColIndex].Last();
                Rect prevRect = rowOrColumn.Value;

                if (IntersectsWith(prevRect, rect, rowOrColumn.Key, itemIndex))
                {
                    InsertToRowOrColumn(++rowOrColIndex, itemIndex, rect);
                }
                else
                {
                    RectssByRowsAndCols[rowOrColIndex].Add(itemIndex, rect);
                }
            }
        }

        protected virtual void CalcBounds(double size)
        {
            
        }

        protected bool IsOpposed()
        {
            if (Axis != null)
            {
                return (Axis.OpposedPosition && Axis.LabelsPosition == AxisElementPosition.Outside)
                       || (!Axis.OpposedPosition && Axis.LabelsPosition == AxisElementPosition.Inside);
            }

            return false;
        }

        public virtual void Arrange(Size finalSize)
        {
            
        }

        /// <summary>
        /// Returns desired height
        /// </summary>
        /// <returns></returns>
        protected virtual double LayoutElements()
        {
            int i = 1;
            int prevIndex = 0;

            if (Axis.LabelsIntersectAction == AxisLabelsIntersectAction.Hide)
            {
                for (; i < Children.Count; i++)
                {
                    if (IntersectsWith(RectssByRowsAndCols[0][prevIndex], RectssByRowsAndCols[0][i], prevIndex, i))
                    {
                        Children[i].Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        prevIndex = i;
                    }
                }
            }
            else if (Axis.LabelsIntersectAction == AxisLabelsIntersectAction.MultipleRows)
            {
                i = 1;
                prevIndex = 0;

                for (; i < Children.Count; i++)
                {
                    if (IntersectsWith(RectssByRowsAndCols[0][prevIndex], RectssByRowsAndCols[0][i], prevIndex, i))
                    {
                        Rect rect = RectssByRowsAndCols[0][i];
                        RectssByRowsAndCols[0].Remove(i);
                        InsertToRowOrColumn(1, i, rect);
                    }
                    else
                    {
                        prevIndex = i;
                    }
                }
            }

            return 0;
        }

        protected virtual void CalculateActualPlotOffset(Size availableSize)
        {
            Axis.ActualPlotOffset = Axis.PlotOffset;
        }
    }

    public class HorizontalLabelLayout:AxisLabelLayout
    {
        public HorizontalLabelLayout(ChartAxis axis, List<UIElement> elements)
            :base(axis, elements)
        {
            
        }

        /// <summary>
        /// Returns desired height
        /// </summary>
        /// <returns></returns>
        protected override double LayoutElements()
        {
            base.LayoutElements();

            return RectssByRowsAndCols.Sum(dictionary => dictionary.Values.Max(rect => rect.Height));
        }

        protected override void CalcBounds(double availableWidth)
        {
            RectssByRowsAndCols = new List<Dictionary<int, Rect>>();
            RectssByRowsAndCols.Add(new Dictionary<int, Rect>());

            for (int j = 0; j < Children.Count; j++)
            {
                double coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[j].Position);
                double position = (coeff*availableWidth) - (ComputedSizes[j].Width/2);

                RectssByRowsAndCols[0].Add(j, new Rect(new Point(position, 0), ComputedSizes[j]));
            }

            if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Shift)
            {
                if (RectssByRowsAndCols[0][0].Left < 0)
                {
                    RectssByRowsAndCols[0][0] = new Rect(0, 0, ComputedSizes[0].Width, ComputedSizes[0].Height);
                }
                int index = Children.Count - 1;
                if (RectssByRowsAndCols[0][index].Right > availableWidth)
                {
                    double position = availableWidth - ComputedSizes[Children.Count - 1].Width;
                    RectssByRowsAndCols[0][index] = new Rect(position, 0, ComputedSizes[index].Width,
                                                             ComputedSizes[index].Height);
                }
            }
            else if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Hide)
            {
                if (RectssByRowsAndCols[0][0].Left < 0)
                {
                    RectssByRowsAndCols[0][0] = new Rect(0, 0, 0, 0);
                    Children[0].Visibility = Visibility.Collapsed;
                }
                int index = Children.Count - 1;
                if (RectssByRowsAndCols[0][index].Right > availableWidth)
                {
                    RectssByRowsAndCols[0][index] = new Rect(0, 0, 0, 0);
                    Children[index].Visibility = Visibility.Collapsed;
                }
            }
        }

        public override Size Measure(Size availableSize)
        {
            if (Axis != null && Children.Count > 0)
            {
                base.Measure(availableSize);
                CalcBounds(availableSize.Width - Axis.ActualPlotOffset * 2);
                double desiredHeight = Math.Max(LayoutElements(), Axis.LabelExtent);
                desiredHeight += ((Margin.Top + Margin.Bottom)*RectssByRowsAndCols.Count);
                return new Size(availableSize.Width, desiredHeight);
            }
            return new Size(availableSize.Width, 0);
        }

        protected override void CalculateActualPlotOffset(Size availableSize)
        {
            if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Fit)
            {
                double coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[0].Position);
                double position = (coeff * availableSize.Width) - (ComputedSizes[0].Width / 2);
                double firstElementWidth = 0;
                double lastElementWidth = 0;

                if ((position - ComputedSizes[0].Width / 2) + Axis.PlotOffset < 0)
                {
                    firstElementWidth = ComputedSizes[0].Width;
                }

                int index = Children.Count - 1;
                if ((position + ComputedSizes[index].Width/2) - Axis.PlotOffset < availableSize.Width)
                {
                    lastElementWidth = ComputedSizes[index].Width;
                }

                double offset = Math.Max(firstElementWidth/2, lastElementWidth/2);
                Axis.ActualPlotOffset = Math.Max(offset, Axis.PlotOffset);
            }
            else
            {
                base.CalculateActualPlotOffset(availableSize);
            }
        }

        public override void Arrange(Size finalSize)
        {
            if (RectssByRowsAndCols == null)
                return;

            bool isOpposed = IsOpposed();
            bool needToRotate = !double.IsNaN(Axis.LabelRotationAngle) && Axis.LabelRotationAngle != 0.0;

            if (Axis.Area is SfChart3D)
            {
                var top = Axis.ArrangeRect.Top;
                var left = Axis.ArrangeRect.Left;
                var g3 = (Axis.Area as SfChart3D).Graphics3D;
                foreach (var dictionary in RectssByRowsAndCols)
                {
                    foreach (var keyValue in dictionary)
                    {
                        var element = Children[keyValue.Key];
                        var actualTop = isOpposed ? top : top + Top;
                        var actualLeft = keyValue.Value.Left + Axis.ActualPlotOffset;

                        if (needToRotate)
                        {
                            actualTop += (ComputedSizes[keyValue.Key].Height - DesiredSizes[keyValue.Key].Height) / 2;
                            actualLeft += (ComputedSizes[keyValue.Key].Width - DesiredSizes[keyValue.Key].Width) / 2;
                        }

#if !WPF
                        if (element is TextBlock)
                        {
                            actualLeft += (element as FrameworkElement).ActualWidth / 2;
                            actualTop += (element as FrameworkElement).ActualHeight / 2;
                        }
                        else
                        {
                            actualLeft += (element as FrameworkElement).DesiredSize.Width / 2;
                            actualTop += (element as FrameworkElement).DesiredSize.Height / 2;
                        }
#else
                        actualTop += element.DesiredSize.Height / 2;
                        actualLeft += element.DesiredSize.Width / 2;

#endif
                        actualLeft += left;
                        g3.AddVisual(Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, 0), element, 10, 10));
                    }

                    if (isOpposed)
                    {
                        top -= (dictionary.Values.Max(rect => rect.Height) + Margin.Bottom);
                    }
                    else
                    {
                        top += (dictionary.Values.Max(rect => rect.Height) + Margin.Top);
                    }
                }
            }
            else
            {
                double top = isOpposed ? finalSize.Height - Margin.Bottom : Margin.Top;

                foreach (Dictionary<int, Rect> dictionary in RectssByRowsAndCols)
                {
                    foreach (KeyValuePair<int, Rect> keyValue in dictionary)
                    {
                        UIElement element = Children[keyValue.Key];
                        double actualTop = isOpposed ? top - ComputedSizes[keyValue.Key].Height : top;
                        double actualLeft = keyValue.Value.Left + Axis.ActualPlotOffset;

                        if (needToRotate)
                        {
                            actualTop += (ComputedSizes[keyValue.Key].Height - DesiredSizes[keyValue.Key].Height) / 2;
                            actualLeft += (ComputedSizes[keyValue.Key].Width - DesiredSizes[keyValue.Key].Width) / 2;
                        }

                        Canvas.SetLeft(element, actualLeft);
                        Canvas.SetTop(element, actualTop);
                    }

                    if (isOpposed)
                    {
                        top -= (dictionary.Values.Max(rect => rect.Height) + Margin.Bottom);
                    }
                    else
                    {
                        top += (dictionary.Values.Max(rect => rect.Height) + Margin.Top);
                    }
                }
            }
        }
    }

    public class VerticalLabelLayout:AxisLabelLayout
    {
        public VerticalLabelLayout(ChartAxis axis, List<UIElement> elements)
            :base(axis, elements)
        {
                
        }

        /// <summary>
        /// Returns desired width
        /// </summary>
        /// <returns></returns>
        protected override double LayoutElements()
        {
            base.LayoutElements();

            return RectssByRowsAndCols.Sum(dictionary => dictionary.Values.Max(rect => rect.Width));
        }

        protected override void CalcBounds(double availableHeight)
        {
            RectssByRowsAndCols = new List<Dictionary<int, Rect>>();
            RectssByRowsAndCols.Add(new Dictionary<int, Rect>());

            for (int j = 0; j < Children.Count; j++)
            {
                double coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[j].Position);
                double position = ((1 - coeff)*availableHeight) - (ComputedSizes[j].Height/2);

                RectssByRowsAndCols[0].Add(j, new Rect(new Point(0, position), ComputedSizes[j]));
            }

            if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Shift)
            {
                if (RectssByRowsAndCols[0][0].Bottom > availableHeight)
                {
                    double position = availableHeight - ComputedSizes[0].Height;
                    RectssByRowsAndCols[0][0] = new Rect(0, position, ComputedSizes[0].Width, ComputedSizes[0].Height);
                }

                int index = Children.Count - 1;
                if (RectssByRowsAndCols[0][index].Top < 0)
                {
                    RectssByRowsAndCols[0][index] = new Rect(0, 0, ComputedSizes[index].Width,
                                                             ComputedSizes[index].Height);
                }
            }
            else if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Hide)
            {
                if (RectssByRowsAndCols[0][0].Bottom > availableHeight)
                {
                    RectssByRowsAndCols[0][0] = new Rect(0, 0, 0, 0);
                    Children[0].Visibility = Visibility.Collapsed;
                }

                int index = Children.Count - 1;
                if (RectssByRowsAndCols[0][index].Top < 0)
                {
                    RectssByRowsAndCols[0][index] = new Rect(0, 0, 0, 0);
                    Children[index].Visibility = Visibility.Collapsed;
                }
            }
        }

        public override Size Measure(Size availableSize)
        {
            if (Axis != null && Children.Count > 0)
            {
                base.Measure(availableSize);
                CalcBounds(availableSize.Height - Axis.ActualPlotOffset * 2);
                double desiredWidth = Math.Max(LayoutElements(), Axis.LabelExtent);
                desiredWidth += ((Margin.Left + Margin.Right)*RectssByRowsAndCols.Count);
                return new Size(desiredWidth, availableSize.Height);
            }
            return new Size(0, availableSize.Height);
        }

        protected override void CalculateActualPlotOffset(Size availableSize)
        {
            if (Axis.EdgeLabelsDrawingMode == EdgeLabelsDrawingMode.Fit)
            {
                double coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[0].Position);
                double position = ((1 - coeff) * availableSize.Height) - (ComputedSizes[0].Height / 2);
                double firstElementHeight = 0;
                double lastElementHeight = 0;
                if ((position + ComputedSizes[0].Height/2) - Axis.PlotOffset > availableSize.Height)
                {
                    firstElementHeight = ComputedSizes[0].Height;
                }

                int index = Children.Count - 1;
                coeff = Axis.ValueToCoefficientCalc(Axis.VisibleLabels[index].Position);
                position = ((1 - coeff) * availableSize.Height) - (ComputedSizes[index].Height / 2);

                if ((position - ComputedSizes[index].Height/2) + Axis.PlotOffset > availableSize.Height)
                {
                    lastElementHeight = ComputedSizes[index].Height;
                }

                double offset = Math.Max(firstElementHeight/2, lastElementHeight/2);
                Axis.ActualPlotOffset = Math.Max(offset, Axis.PlotOffset);
            }
            else
            {
                base.CalculateActualPlotOffset(availableSize);
            }
        }

        public override void Arrange(Size finalSize)
        {
            if (RectssByRowsAndCols == null)
                return;
            bool isOpposed = IsOpposed();
            bool needToRotate = !double.IsNaN(Axis.LabelRotationAngle) && Axis.LabelRotationAngle != 0.0;
            double left = isOpposed ? Margin.Left : finalSize.Width - Margin.Right;

            if (Axis.Area is SfChart3D)
            {
                var g3 = (Axis.Area as SfChart3D).Graphics3D;
                foreach (var dictionary in RectssByRowsAndCols)
                {
                    foreach (var keyValue in dictionary)
                    {
                        var elemSize = Size.Empty;
                        var element = Children[keyValue.Key];

#if !WPF
                        if (element is TextBlock)
                        {
                            elemSize = new Size((element as FrameworkElement).ActualWidth,(element as FrameworkElement).ActualHeight);
                        }
                        else
                        {
                            elemSize = (element as FrameworkElement).DesiredSize; ;
                        }
#else
                        elemSize = (element as FrameworkElement).DesiredSize;

#endif

                        var actualLeft = isOpposed ? left : left - elemSize.Width;
                        actualLeft += Axis.ArrangeRect.Left + Left + DesiredSizes[keyValue.Key].Width / 2;
                        var actualTop = keyValue.Value.Top + Axis.ActualPlotOffset + Axis.ArrangeRect.Top;

                        if (needToRotate)
                        {
                            actualLeft += (elemSize.Width - DesiredSizes[keyValue.Key].Width) / 2;
                            actualTop += (elemSize.Height - DesiredSizes[keyValue.Key].Height) / 2;
                        }

                        actualTop += DesiredSizes[keyValue.Key].Height / 2;

                        g3.AddVisual(Polygon3D.CreateUIElement(new Vector3D(actualLeft, actualTop, 0), element, 10, 10));
                    }

                    if (isOpposed)
                    {
                        left += (dictionary.Values.Max(rect => rect.Width) + Margin.Left);
                    }
                    else
                    {
                        left -= (dictionary.Values.Max(rect => rect.Width) + Margin.Right);
                    }
                }
            }
            else
            {
                foreach (Dictionary<int, Rect> dictionary in RectssByRowsAndCols)
                {
                    foreach (KeyValuePair<int, Rect> keyValue in dictionary)
                    {
                        UIElement element = Children[keyValue.Key];
                        
                        double actualLeft = isOpposed ? left : left - ComputedSizes[keyValue.Key].Width;
                        double actualTop = keyValue.Value.Top + Axis.ActualPlotOffset;

                        if (needToRotate)
                        {
                            actualLeft += (ComputedSizes[keyValue.Key].Width - DesiredSizes[keyValue.Key].Width) / 2;
                            actualTop += (ComputedSizes[keyValue.Key].Height - DesiredSizes[keyValue.Key].Height) / 2;
                        }

                        Canvas.SetLeft(element, actualLeft);
                        Canvas.SetTop(element, actualTop);

                    }

                    if (isOpposed)
                    {
                        left += (dictionary.Values.Max(rect => rect.Width) + Margin.Left);
                    }
                    else
                    {
                        left -= (dictionary.Values.Max(rect => rect.Width) + Margin.Right);
                    }
                }
            }
        }
    }
}
