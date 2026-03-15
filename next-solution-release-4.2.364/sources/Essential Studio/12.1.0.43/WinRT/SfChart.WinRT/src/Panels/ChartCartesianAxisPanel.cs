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
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// This interfaces defines the memebers and methods to create and arrange the child elements in a panel.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public interface ILayoutCalculator
    {
        /// <summary>
        /// Get Children property
        /// </summary>
        List<UIElement> Children { get; }

        /// <summary>
        /// Gets the panel.
        /// </summary>
        /// <value>
        /// The panel.
        /// </value>
        Panel Panel { get; }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        double Left { get; set; }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        double Top { get; set; }

        /// <summary>
        /// Get desiredSize property
        /// </summary>
        Size DesiredSize { get; }

        /// <summary>
        /// Method declaration for Measure
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        Size Measure(Size availableSize);

        /// <summary>
        /// Method declaration for Arrange
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        Size Arrange(Size finalSize);

        /// <summary>
        /// Method declaration for UpdateElements
        /// </summary>
        void UpdateElements();
        
        /// <summary>
        /// Detachs elements from the panel
        /// </summary>
        void DetachElements();
    }

    /// <summary>
    /// Represents ChartCartesianAxisPanel.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartCartesianAxisPanel : Canvas
    {
        #region fields

        internal ChartAxisBase2D Axis;

        internal List<ILayoutCalculator> LayoutCalc;

        #endregion

        #region ctor

        /// <summary>
        /// called when instance created for ChartCarttesianAxisPanel
        /// </summary>
        public ChartCartesianAxisPanel()
        {
            LayoutCalc = new List<ILayoutCalculator>();
        }

        #endregion

        #region methods

        internal Size ComputeSize(Size availableSize)
        {
            Size size = Size.Empty;

            if (Axis.AxisLayoutPanel is ChartPolarAxisLayoutPanel)
            {
                foreach (UIElement element in this.Children)
                {
                    element.Measure(availableSize);
                }

                foreach (ILayoutCalculator element in this.LayoutCalc)
                {
                    element.Measure(availableSize);
                    size = element.DesiredSize;

                    ChartPolarAxisLayoutPanel axisLayoutPanel = Axis.AxisLayoutPanel as ChartPolarAxisLayoutPanel;
                    ChartCircularAxisPanel circularAxisPanel = element as ChartCircularAxisPanel;
                    axisLayoutPanel.Radius = circularAxisPanel.Radius;
                }
            }
            else
            {
                double width = 0;
                double height = 0;
                double angle = 0d; //double.IsNaN(this.Axis.HeaderRotationAngle) ? 0d : this.Axis.HeaderRotationAngle;
                double direction = 1;
                FrameworkElement headerContent = this.Children[0] as FrameworkElement;
                headerContent.HorizontalAlignment = HorizontalAlignment.Center;
                headerContent.VerticalAlignment = VerticalAlignment.Center;

                if (Axis.Orientation == Orientation.Vertical)
                {
                    direction = this.Axis.OpposedPosition ? 1 : -1;
                    angle = direction*90;
                    var transform = new RotateTransform {Angle = angle};
                    headerContent.RenderTransform = transform;
                }
                else
                    headerContent.RenderTransform = null;

                foreach (UIElement element in this.Children)
                {
                    if (headerContent == element)
                        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    else
                        element.Measure(availableSize);

#if WPF || NETFX_CORE ||SILVERLIGHT_UNCOMMON
                    if (element is SfChartResizableBar && Axis.EnableTouchMode)
                        continue;
#endif

                    bool isHeader = headerContent == element
                                    && this.Axis.Orientation == Orientation.Vertical;

                    width += isHeader ? element.DesiredSize.Height : element.DesiredSize.Width;
                    height += isHeader ? element.DesiredSize.Width : element.DesiredSize.Height;
                }

                double horizontalPadding = 0;
                double verticalPadding = 0;

                foreach (ILayoutCalculator element in LayoutCalc)
                {
                    element.Measure(availableSize);
                    if ((element is ChartCartesianAxisLabelsPanel
                         && Axis.LabelsPosition == AxisElementPosition.Inside)
                        || (element is ChartCartesianAxisElementsPanel
                            && Axis.TickLinesPosition == AxisElementPosition.Inside))
                    {
                        horizontalPadding += element.DesiredSize.Width;
                        verticalPadding += element.DesiredSize.Height;
                    }
                    width += element.DesiredSize.Width;
                    height += element.DesiredSize.Height;
                }

                if (Axis.Orientation == Orientation.Vertical)
                {
                    Axis.InsidePadding = horizontalPadding;
                    size = new Size(width, availableSize.Height);
                }
                else
                {
                    Axis.InsidePadding = verticalPadding;
                    size = new Size(availableSize.Width, height);
                }
            }

            return ChartLayoutUtils.CheckSize(size);
        }

        internal void ArrangeElements(Size finalSize)
        {
            Size originalSize = Size.Empty;
            if (Axis.AxisLayoutPanel is ChartPolarAxisLayoutPanel)
            {
                foreach (UIElement element in this.Children)
                {
                    element.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                }

                foreach (ILayoutCalculator layout in this.LayoutCalc)
                {
                    layout.Arrange(finalSize);

                    ChartPolarAxisLayoutPanel axisLayoutPanel = Axis.AxisLayoutPanel as ChartPolarAxisLayoutPanel;
                    ChartCircularAxisPanel circularAxisPanel = layout as ChartCircularAxisPanel;
                    axisLayoutPanel.Radius = circularAxisPanel.Radius;
                }
            }
            else
            {
                ArrangeCartesianElements(finalSize);
            }
        }

        private void ArrangeCartesianElements(Size finalSize)
        {
            if (Axis == null)
                return;

            foreach (UIElement element in Children)
            {
                SetLeft(element, 0);
                SetTop(element, 0);
            }

            var headerContent = Children[0];
            var labelsPanel = LayoutCalc[0];
            var elementsPanel = LayoutCalc[1];
            var scrollBar = Children.OfType<SfChartResizableBar>().FirstOrDefault();

            var elements = new List<UIElement>();
            var sizes = new List<Size>();
            var isVertical = Axis.Orientation == Orientation.Vertical;
            var isInversed = Axis.OpposedPosition ^ isVertical;

            if (scrollBar != null)
            {
                elements.Add(scrollBar);
                sizes.Add(scrollBar.DesiredSize);
            }
            if (Axis.TickLinesPosition == AxisElementPosition.Inside)
            {
                elements.Insert(0, elementsPanel.Panel);
                sizes.Insert(0, elementsPanel.DesiredSize);
            }
            else
            {
                elements.Add(elementsPanel.Panel);
                sizes.Add(elementsPanel.DesiredSize);
            }

            if (Axis.LabelsPosition == AxisElementPosition.Inside)
            {
                elements.Insert(0, labelsPanel.Panel);
                sizes.Insert(0, labelsPanel.DesiredSize);
            }
            else
            {
                elements.Add(labelsPanel.Panel);
                sizes.Add(labelsPanel.DesiredSize);
            }

            elements.Add(headerContent);
            Size headerSize = headerContent.DesiredSize;
            if (isVertical)
            {
                headerSize = new Size(headerSize.Height, headerSize.Width);
            }
            sizes.Add(headerSize);
            
            if (isInversed)
            {
                elements.Reverse();
                sizes.Reverse();
            }

            double currentPos = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                UIElement element = elements[i];
                
                if (isVertical)
                {
                    if (element == headerContent)
                    {
                        SetTop(element, (finalSize.Height - element.DesiredSize.Height)/2);
                        SetLeft(element, currentPos - ((element.DesiredSize.Width - sizes[i].Width) / 2));
                    }
                    else
                    {
#if WPF || NETFX_CORE ||SILVERLIGHT_UNCOMMON
                        if (element is SfChartResizableBar && Axis.EnableTouchMode)
                        {
                            SetLeft(element, currentPos - sizes[i].Width/2);
                            continue;
                        }
#endif
                        SetLeft(element, currentPos);
                    }
                    currentPos += sizes[i].Width;
                }
                else
                {
                    if (element == headerContent)
                    {
                        SetLeft(element,(finalSize.Width - sizes[i].Width)/2);
                    }
#if WPF || NETFX_CORE ||SILVERLIGHT_UNCOMMON
                    if (element is SfChartResizableBar && Axis.EnableTouchMode)
                    {
                        SetTop(element, currentPos - sizes[i].Height / 2);
                        continue;
                    }
#endif
                    SetTop(element,currentPos);
                    currentPos += sizes[i].Height;
                }
            }

            foreach (ILayoutCalculator layout in this.LayoutCalc)
            {
                layout.Arrange(layout.DesiredSize);
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents ChartCartesianAxisPanel3D.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartCartesianAxisPanel3D
    {
        #region fields

        internal ChartAxisBase3D Axis;

        internal FrameworkElement HeaderContent { get; set; }

        internal List<ILayoutCalculator> LayoutCalc;

        #endregion

        #region ctor

        /// <summary>
        /// called when instance created for ChartCartesianAxisPanel3D
        /// </summary>
        public ChartCartesianAxisPanel3D()
        {
            LayoutCalc = new List<ILayoutCalculator>();
        }

        #endregion

        #region methods

        internal Size ComputeSize(Size availableSize)
        {
            Size size;

            double width = 0;
            double height = 0;
            HeaderContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            HeaderContent.HorizontalAlignment = HorizontalAlignment.Center;
            HeaderContent.VerticalAlignment = VerticalAlignment.Center;
            if (Axis.Orientation == Orientation.Vertical)
            {
                double direction = Axis.OpposedPosition ? 1 : -1;
                var angle = direction * 90;
                var transform = new RotateTransform { Angle = angle };
                HeaderContent.RenderTransform = transform;
            }
            else
                HeaderContent.RenderTransform = null;

            double horizontalPadding = 0;
            double verticalPadding = 0;

            foreach (var element in LayoutCalc)
            {
                element.Measure(availableSize);
                if ((element is ChartCartesianAxisLabelsPanel
                     && Axis.LabelsPosition == AxisElementPosition.Inside)
                    || (element is ChartCartesianAxisElementsPanel
                        && Axis.TickLinesPosition == AxisElementPosition.Inside))
                {
                    horizontalPadding += element.DesiredSize.Width;
                    verticalPadding += element.DesiredSize.Height;
                }
                width += element.DesiredSize.Width;
                height += element.DesiredSize.Height;
            }
            var isVertical = Axis.Orientation == Orientation.Vertical;

            width += isVertical ? HeaderContent.DesiredSize.Height : HeaderContent.DesiredSize.Width;
            height += isVertical ? HeaderContent.DesiredSize.Width : HeaderContent.DesiredSize.Height;

            if (Axis.Orientation == Orientation.Vertical)
            {
                Axis.InsidePadding = horizontalPadding;
                size = new Size(width, availableSize.Height);
            }
            else
            {
                Axis.InsidePadding = verticalPadding;
                size = new Size(availableSize.Width, height);
            }

            return ChartLayoutUtils.CheckSize(size);
        }

        internal void ArrangeElements(Size finalSize)
        {
            ArrangeCartesianElements(finalSize);
        }

        private void ArrangeCartesianElements(Size finalSize)
        {
            if (Axis == null)
                return;
            var labelsPanel = LayoutCalc[0];
            var elementsPanel = LayoutCalc[1];

            var elements = new List<object>();
            var sizes = new List<Size>();
            var isVertical = Axis.Orientation == Orientation.Vertical;
            var isInversed = Axis.OpposedPosition ^ isVertical;

            if (Axis.TickLinesPosition == AxisElementPosition.Inside)
            {
                elements.Insert(0, elementsPanel);
                sizes.Insert(0, elementsPanel.DesiredSize);
            }
            else
            {
                elements.Add(elementsPanel);
                sizes.Add(elementsPanel.DesiredSize);
            }

            if (Axis.LabelsPosition == AxisElementPosition.Inside)
            {
                elements.Insert(0, labelsPanel);
                sizes.Insert(0, labelsPanel.DesiredSize);
            }
            else
            {
                elements.Add(labelsPanel);
                sizes.Add(labelsPanel.DesiredSize);
            }

            elements.Add(HeaderContent);
            var headerSize = HeaderContent.DesiredSize;
            if (isVertical)
            {
                headerSize = new Size(headerSize.Height, headerSize.Width);
            }
            sizes.Add(headerSize);

            if (isInversed)
            {
                elements.Reverse();
                sizes.Reverse();
            }

            double currentPosition = 0;
            double headerX = 0, headerY = 0;
            for (var i = 0; i < elements.Count; i++)
            {
                var element = elements[i];

                if (isVertical)
                {
                    if (element == HeaderContent)
                    {
                        headerY = (finalSize.Height - ((Control)element).DesiredSize.Height) / 2 + Axis.ArrangeRect.Top;
                        if (isInversed)
                            headerX = currentPosition +  Axis.ArrangeRect.Left;
                        else
                            headerX = currentPosition + sizes[i].Width + Axis.ArrangeRect.Left;
                    }
                    else
                    {
                        var layout = element as ILayoutCalculator;
                        layout.Left = currentPosition;
                    }
                    currentPosition += sizes[i].Width;
                }
                else
                {
                    if (element == HeaderContent)
                    {
                        headerX = (finalSize.Width - sizes[i].Width) / 2 + Axis.ArrangeRect.Left;
                        if (isInversed)
                            headerY = currentPosition + Axis.ArrangeRect.Top;
                        else
                            headerY = currentPosition + sizes[i].Height + Axis.ArrangeRect.Top;
                    }
                    else
                    {
                        var layout = element as ILayoutCalculator;
                        layout.Top = currentPosition;
                    }
                    currentPosition += sizes[i].Height;
                }
            }

            foreach (var layout in LayoutCalc)
            {
                layout.Arrange(layout.DesiredSize);
            }

            var vectorColl = new Vector3D[3];

            vectorColl[0] = new Vector3D(headerX, headerY, 0);
            vectorColl[1] = new Vector3D(headerX, headerY + 10, 0);
            vectorColl[2] = new Vector3D(headerX + 10, headerY + 10, 0);

            ((SfChart3D) Axis.Area).Graphics3D.AddVisual(new UIElement3D(HeaderContent, vectorColl));

        }

        #endregion
    }
}
