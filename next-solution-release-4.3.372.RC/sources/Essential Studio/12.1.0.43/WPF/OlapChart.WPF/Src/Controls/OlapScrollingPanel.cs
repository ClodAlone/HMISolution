#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart.Olap
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Syncfusion.Olap.Engine;

    /// <summary>
    /// Representing OlapScrollingPanel
    /// </summary>

#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class OlapScrollingPanel : Panel
    {
        public static RoutedUICommand ExpandElement = new RoutedUICommand("ExpandElement", "ExpandElement", typeof(OlapScrollingPanel));

        public OlapScrollingPanel()
        {
            CommandBindings.Add(new CommandBinding(OlapScrollingPanel.ExpandElement, ExpandElementExecuted, ExpandElementCanExecute));
        }

        public void ExpandElementExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            (Axis as OlapChartAxis).RaiseLabelClick(new OlapLabelClickEvenArgs(e.Parameter as PivotCellDescriptor));
        }

        public void ExpandElementCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }


        #region Members
        ///<summary>
        /// Identifies the Axis dependency property.
        ///</summary>
        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register("Axis", typeof(OlapChartAxis), typeof(OlapScrollingPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));
        private const double m_AxisPaddingCoefficient = 0.5d;
        private const double m_labelMinWidth = 10;
        private object m_mouseDownSource;
        #endregion

        #region DependencyProperties
        ///<summary>
        /// Identifies the Orientation dependency property.
        ///</summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(OlapScrollingPanel), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.Inherits));

        ///<summary>
        /// Identifies the ZoomFactor dependency property.
        ///</summary>
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(OlapScrollingPanel), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsArrange));

        ///<summary>
        /// Identifies the ZoomPosition dependency property.
        ///</summary>
        public static readonly DependencyProperty ZoomPositionProperty =
            DependencyProperty.Register("ZoomPosition", typeof(double), typeof(OlapScrollingPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Axis. This is a dependency property.
        /// </summary>
        /// <value>The Axis.</value>
        public OlapChartAxis Axis
        {
            get { return (OlapChartAxis)GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Orientation. This is a dependency property.
        /// </summary>
        /// <value>The Orientation.</value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ZoomFactor. This is a dependency property.
        /// </summary>
        /// <value>The ZoomFactor.</value>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ZoomPosition. This is a dependency property.
        /// </summary>
        /// <value>The ZoomPosition.</value>
        public double ZoomPosition
        {
            get { return (double)GetValue(ZoomPositionProperty); }
            set { SetValue(ZoomPositionProperty, value); }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size retValue;
            Rect rect = new Rect(finalSize);
            double shift = 0;// m_AxisPaddingCoefficient / Axis.VisibleRange.Delta;

            if (Orientation == Orientation.Horizontal)
            {
                //shift *=  finalSize.Width;
                rect.Width /= this.ZoomFactor;
                rect.X -= rect.Width * this.ZoomPosition;
                retValue = rect.Size;
                rect.Width -= 2 * shift;
                rect.X += shift;
            }
            else
            {
                shift *= finalSize.Height;
                rect.Y += rect.Height;
                rect.Height /= this.ZoomFactor;
                rect.Y -= (1 - this.ZoomPosition) * rect.Height;
                retValue = rect.Size;
                rect.Height -= 2 * shift;
                rect.Y += shift;
            }
            foreach (UIElement element in Children)
            {
                element.Arrange(rect);
            }
            return retValue;
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {

            Size retValue = new Size();

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                retValue = child.DesiredSize;
            }
            return retValue;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    m_mouseDownSource = e.Source as OlapLabelPresenter;
        //    base.OnMouseLeftButtonDown(e);
        //}

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    if (m_mouseDownSource == e.Source && Axis != null)
        //    {
        //        (Axis as OlapChartAxis).RaiseLabelClick(new OlapLabelClickEvenArgs((e.Source as OlapLabelPresenter).Content as PivotCellDescriptor));
        //    }
        //    m_mouseDownSource = null;
        //    base.OnMouseLeftButtonUp(e);
        //}

        /// <summary>
        /// Tells panel to set passed zoom factor on corresponding axis.
        /// </summary>
        /// <param name="zoomFactor">The zoom factor.</param>
        internal void RequestZoomFactor(double zoomFactor)
        {
            if (this.Axis.Area.PivotEngine!=null && this.Axis.Area.PivotEngine.ItemSource != null)
            {
                Axis.ProcessingLabelsState = true;
            }
            
            if (this.Axis.Area.ChartControl.DisplayMode == DisplayMode.CompressedMode)
            {
                Axis.ZoomFactor = this.Axis.MaximalZoomFactor;
            }
            else
            {
                Axis.ZoomFactor = zoomFactor;
            }
        }
        #endregion

    }
}
