#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for ChartAxisBase3D
    /// </summary>
    public abstract class ChartAxisBase3D : ChartAxis
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisBase3D"/> class.
        /// </summary>
        public ChartAxisBase3D()
        {
#if !NETFX_CORE
            DefaultStyleKey = typeof(ChartAxisBase3D);
#endif
        }

        #endregion

        #region fields

        ChartCartesianAxisPanel3D axisPanel;

        #endregion

        #region methods
        /// <summary>
        /// Raises the <see cref="E:AxisBoundsChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ChartAxisBoundsEventArgs"/> instance containing the event data.</param>
        protected internal override void OnAxisBoundsChanged(ChartAxisBoundsEventArgs args)
        {
            base.OnAxisBoundsChanged(args);
            if (axisPanel != null)
                axisPanel.ArrangeElements(new Size(ArrangeRect.Width, ArrangeRect.Height));
        }

        internal override void CreateLineRecycler()
        {
            if (Area == null) return;
            GridLinesRecycler = new UIElementsRecycler<Line>(null);
            MinorGridLinesRecycler = new UIElementsRecycler<Line>(null);
        }

        internal override void ComputeDesiredSize(Size size)
        {
            AvailableSize = size;
            CalculateRangeAndInterval(size);
            if (Visibility != Visibility.Collapsed)
            {
                UpdatePanels();
                UpdateLabels();
                ComputedDesiredSize = axisPanel.ComputeSize(size);
            }
            else
            {
                ActualPlotOffset = PlotOffset;
                InsidePadding = 0;
                UpdateLabels();
                ComputedDesiredSize = Orientation == Orientation.Horizontal
                                          ? new Size(size.Width, 0)
                                          : new Size(0, size.Height);
            }
        }

        void UpdatePanels()
        {
            if (axisLabelsPanel != null)
            {
                axisLabelsPanel.DetachElements();
            }
            if (axisElementsPanel != null)
            {
                axisElementsPanel.DetachElements();
            }
            if (axisPanel != null)
            {
                axisPanel.LayoutCalc.Clear();
            }
            else
            {
                axisPanel = new ChartCartesianAxisPanel3D {Axis = this};
            }

            axisLabelsPanel = new ChartCartesianAxisLabelsPanel(null)
            {
                Axis = this
            };
            axisElementsPanel = new ChartCartesianAxisElementsPanel(null)
            {
                Axis = this
            };
            if (headerContent == null)
                headerContent = new ContentControl { Content = Header, ContentTemplate = HeaderTemplate, RenderTransformOrigin = new Point(0.5,0.5) };
            axisPanel.HeaderContent = headerContent;
            axisPanel.LayoutCalc.Add(axisLabelsPanel);
            axisPanel.LayoutCalc.Add(axisElementsPanel);
        }

        #endregion

    }
}
