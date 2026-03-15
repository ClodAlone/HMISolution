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
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Collections;
#else
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Specialized;
using Windows.UI.Xaml.Input;
using System.Collections;
#endif


namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the Chart control which is used to visualize the data graphically in 3 dimensional.
    /// </summary>
    /// <remarks>
    /// The Chart is often used to make it easier to
    /// understand large amount of data and the relationship between different parts
    /// of the data. Chart can usually be read more quickly than the raw data that they
    /// come from. <para> Certain <see cref="ChartSeries3D" /> are more useful for
    /// presenting a given data set than others. For example, data that presents
    /// percentages in different groups (such as "satisfied, not satisfied, unsure") are
    /// often displayed in a <see cref="PieSeries3D" /> chart, but are more easily
    /// understood when presented in a horizontal <see cref="BarSeries3D" /> chart.
    /// </remarks>
    /// <seealso cref="ChartSeries3D"/>
    /// <seealso cref="ChartLegend"/>
    /// <seealso cref="ChartAxis3D"/>
#if WINDOWS_PHONE
    [ContentProperty("Series")]
#else
    [ContentProperty(Name = "Series")]
#endif
    [ClassReference(IsReviewed = false)]
    public class SfChart3D : ChartBase
    {

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfChart3D()
        {
#if WPF
            EnvironmentTest.ValidateLicense(typeof(SfChart));
#endif
            DefaultStyleKey = typeof (SfChart3D);
            UpdateAction = UpdateAction.Invalidate;
            Series = new ChartSeries3DCollection();
            VisibleSeries = new ChartVisibleSeriesCollection();
            Axes = new ChartAxisCollection();
#if NETFX_CORE || SILVERLIGHT_UNCOMMON || WPF
            Printing = new Printing(this);
#endif
#if !WINDOWS_PHONE
            ManipulationMode = ManipulationModes.Scale
                               | ManipulationModes.TranslateRailsX
                               | ManipulationModes.TranslateRailsY
                               | ManipulationModes.TranslateX
                               | ManipulationModes.TranslateY
                               | ManipulationModes.TranslateInertia
                               | ManipulationModes.Rotate;
#endif
            ColorModel = new ChartColorModel(Palette);
        }


        #endregion

        #region fields

        private Point previousChartPosition;

        private bool rotationActivated;

#if WINDOWS_PHONE
       internal bool isRenderSeriesDispatched = false;
#else
        internal IAsyncAction renderSeriesAction;
#endif

        internal bool IsAutoDepth
        {
            get;
            set;
        }

        private double previousAutoDepth;

        private Dictionary<int, double> sumByIndex = new Dictionary<int, double>();

        internal Canvas RootPanel { get; set; }

        private Panel controlsPresenter;

        internal Graphics3D Graphics3D = new Graphics3D();

        private bool is3DUpdateScheduled;

        #endregion

        #region methods

        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize"></param>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                SizeChanged -= OnSizeChanged;
                SizeChanged += OnSizeChanged;
                AvailableSize = new Size(ActualWidth == 0d ? 500d : ActualWidth, ActualHeight == 0d ? 500d : ActualHeight);
            }
            else
                AvailableSize = availableSize;

            return base.MeasureOverride(AvailableSize);
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize != AvailableSize)
                InvalidateMeasure();
        }

        private void RotateChart(Point updatedPosition)
        {
            if (rotationActivated)
            {
                foreach (var series in VisibleSeries)
                {
                    series.RemoveTooltip();
                }
                var difY = (previousChartPosition.Y - updatedPosition.Y);
                var difX = (previousChartPosition.X - updatedPosition.X);
                Tilt -= difY;
                Rotation += difX;
                previousChartPosition = updatedPosition;
            }
        }

        /// <summary>
        /// Clone the entire chart
        /// </summary>
        /// <returns></returns>
        internal override DependencyObject CloneChart()
        {
            var chart = new SfChart3D();
            ChartCloning.CloneControl(this, chart);
            chart.Height = double.IsNaN(Height) ? ActualHeight : Height;
            chart.Width = double.IsNaN(Width) ? ActualWidth : Width;
            chart.Header = Header;
            chart.Palette = Palette;
            chart.SideBySideSeriesPlacement = SideBySideSeriesPlacement;
            chart.PrimaryAxis = (ChartAxisBase3D)(PrimaryAxis as ICloneable).Clone();
            chart.SecondaryAxis = (RangeAxisBase3D)(SecondaryAxis as ICloneable).Clone();
            if (Legend != null)
                chart.Legend = (ChartLegend)(Legend as ICloneable).Clone();
            foreach (ChartSeriesBase series in Series)
            {
                chart.Series.Add((ChartSeries3D)(series as ICloneable).Clone());
            }
            foreach (var rowDefinition in RowDefinitions)
            {
                chart.RowDefinitions.Add((ChartRowDefinition)(rowDefinition as ICloneable).Clone());
            }
            foreach (var columnDefinition in ColumnDefinitions)
            {
                chart.ColumnDefinitions.Add((ChartColumnDefinition)(columnDefinition as ICloneable).Clone());
            }
            chart.UpdateArea(true);
            return chart;
        }


        /// <summary>
        /// Sets the axis for chart series.
        /// </summary>
        /// <param name="series">The series.</param>
        internal void SetAxisForChartSeries(ISupportAxes3D series)
        {
            if (series == null) return;
            var xAxis = series.XAxis;
            ChartAxis yAxis = series.YAxis;
            if (xAxis == null && InternalPrimaryAxis != null)
            {
                series.XAxis = InternalPrimaryAxis as ChartAxisBase3D;
            }
            else if (xAxis != null && xAxis != InternalPrimaryAxis)
            {
                if (!Axes.Contains(xAxis))
                {
                    Axes.Add(xAxis);
                }
            }

            if (yAxis == null && InternalSecondaryAxis != null)
            {
                series.YAxis = InternalSecondaryAxis as RangeAxisBase3D;
            }
            else if (yAxis != null && yAxis != InternalSecondaryAxis)
            {
                if (!Axes.Contains(yAxis))
                {
                    Axes.Add(yAxis);
                }
            }
            if (series.XAxis != null)
                series.XAxis.Area = this;
            if (series.YAxis != null)
                series.YAxis.Area = this;
        }

        internal double GetPercentByIndex(List<StackingSeriesBase3D> series, int index, double value, bool reCalculation)
        {
            if (sumByIndex.Keys.Contains(index) && !reCalculation) return value / sumByIndex[index] * 100;
            var result = series.Sum(item => item.YValues.Count != 0d ? Math.Abs(item.YValues[index]) : 0d);
            sumByIndex[index] = result;
            return value / sumByIndex[index] * 100;
        }

        /// <summary>
        /// Renders the series.
        /// </summary>
        internal void RenderSeries()
        {
            if (RootPanelDesiredSize != null)
            {
                Update3DWall();

                var size = RootPanelDesiredSize.Value;

                if (VisibleSeries != null)
                {
                    foreach (ChartSeries3D series in VisibleSeries)
                    {
                        series.UpdateOnSeriesBoundChanged(size);
                        if (series.AdornmentsInfo != null)
                            series.AdornmentsInfo.Arrange(size);
                        foreach (ChartSegment3D segment in series.Segments.OfType<ChartSegment3D>())
                        {
                            segment.Polygons.Clear();
                        }
                    }
                }

                Graphics3D.PrepareView(PerspectiveAngle, Depth, Rotation, Tilt, size);
                Graphics3D.View(RootPanel);

                foreach (var item in VisibleSeries.Where(item => item.CanAnimate && item.Segments.Count > 0))
                {
                    item.Animate();
                    item.CanAnimate = false;
                }
            }

#if WINDOWS_PHONE
            isRenderSeriesDispatched = false;
#else
            renderSeriesAction = null;
#endif
            StackedValues = null;
        }

        /// <summary>
        /// Converts point to value.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        /// The double point to value
        /// </returns>
        public override double PointToValue(ChartAxis axis, Point point)
        {
            var frontPlane = new Polygon3D(new Vector3D(0, 0, 1), 0);
            var transform = Graphics3D.Transform;
            frontPlane.Transform(transform.View);
            var actualPosition = transform.ToPlane(point, frontPlane);
            return base.PointToValue(axis, new Point(actualPosition.X, actualPosition.Y));
        }

        /// <summary>
        /// Converts Value to point.
        /// </summary>
        /// <param name="axis">The Chart axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The double value to point
        /// </returns>
        public override double ValueToPoint(ChartAxis axis, double value)
        {
            var actualValue = base.ValueToPoint(axis, value);
            if (axis.Orientation == Orientation.Horizontal)
                return Graphics3D.Transform.ToScreen(new Vector3D(actualValue, 0, 0)).X;
            else
                return Graphics3D.Transform.ToScreen(new Vector3D(0, actualValue, 0)).Y;
        }

        /// <summary>
        /// Gets the wall.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="z">The z.</param>
        /// <returns></returns>
        internal Polygon3D GetWall(Rect bounds, double z, Brush brush)
        {
            var vs = new[]{
        new Vector3D( bounds.Left, bounds.Top, z ),
        new Vector3D( bounds.Right, bounds.Top, z ),
        new Vector3D( bounds.Right, bounds.Bottom, z ),
        new Vector3D( bounds.Left, bounds.Bottom, z ) };

            return new Polygon3D(vs, null, 0, null, 0, brush);
        }

        /// <summary>
        /// Update3D the wall.
        /// </summary>
        void Update3DWall()
        {
            if (AreaType != ChartAreaType.CartesianAxes) return;
            var actualSeriesRect = SeriesClipRect;
            Graphics3D.AddVisual(GetWall(actualSeriesRect, Depth, BackWallBrush));
            Polygon3D bottomWall = null;
            Polygon3D topWall = null;
            Polygon3D leftSideWall = null;
            Polygon3D rightSideWall = null;
            foreach (var item in Axes)
            {
                if (item.Orientation == Orientation.Vertical)
                {
                    if (item.OpposedPosition && rightSideWall == null)
                    {
                        rightSideWall = GetWall(new Rect(-(Depth), SeriesClipRect.Top, Depth, SeriesClipRect.Height), SeriesClipRect.Left + SeriesClipRect.Width + 1, RightWallBrush);
                        if (rightSideWall == null) continue;
                        rightSideWall.Transform(Matrix3D.Turn((float)(-Math.PI / 2)));
                        Graphics3D.AddVisual(rightSideWall);
                    }
                    else if (leftSideWall == null)
                    {
                        leftSideWall = GetWall(new Rect(-(Depth), SeriesClipRect.Top, Depth, SeriesClipRect.Height), SeriesClipRect.Left, LeftWallBrush);
                        if (leftSideWall == null) continue;
                        leftSideWall.Transform(Matrix3D.Turn((float)(-Math.PI / 2)));
                        Graphics3D.AddVisual(leftSideWall);
                    }
                }
                else
                {
                    if (item.OpposedPosition && topWall == null)
                    {
                        topWall = GetWall(new Rect(SeriesClipRect.Left, -(int)Depth, actualSeriesRect.Width, (int)Depth), SeriesClipRect.Top, TopWallBrush );
                        if (topWall == null) continue;
                        topWall.Transform(Matrix3D.Tilt((float)(Math.PI / 2)));
                        Graphics3D.AddVisual(topWall);
                    }
                    else if (bottomWall == null)
                    {
                        bottomWall = GetWall(new Rect(SeriesClipRect.Left, -(int)Depth, actualSeriesRect.Width, (int)Depth), SeriesClipRect.Top + SeriesClipRect.Height + 1, BottomWallBrush);
                        if (bottomWall == null) continue;
                        bottomWall.Transform(Matrix3D.Tilt((float)(Math.PI / 2)));
                        Graphics3D.AddVisual(bottomWall);
                    }
                }
            }
        }




        /// <summary>
        /// Update the 3D view.
        /// </summary>
        void Update3DView()
        {
            is3DUpdateScheduled = false;
            if (RootPanelDesiredSize != null)
            {
                foreach (var segment in Series.SelectMany(series => series.Segments.OfType<ChartSegment3D>()))
                {
                    ((ChartSegment3D)segment).Polygons.Clear();
                }
                
                if (RootPanel == null) return;
                if (IsAutoDepth)
                {
                    if (AutoDepthAdjust())
                        Graphics3D.PrepareView(PerspectiveAngle, Depth, Rotation, Tilt, RootPanelDesiredSize.Value);
                }
                Graphics3D.View(RootPanel, Rotation, Tilt, RootPanelDesiredSize.Value, PerspectiveAngle, Depth);
            }
        }

        internal bool IsChartRotated()
        {
            var actualTiltView = Math.Abs(Tilt % 360);
            var actualRotateView = Math.Abs(Rotation % 360);
            if ((actualTiltView > 90 && actualTiltView < 270) ^ (actualRotateView > 90 && actualRotateView < 270))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Automatics the depth adjust.
        /// </summary>
        /// <returns></returns>
        private bool AutoDepthAdjust()
        {
            const double depthSpacing = 5d;
            var depth = 0d;

            var actualTiltView = Math.Abs(Tilt % 360);
            var actualRotateView = Math.Abs(Rotation % 360);
            if (IsChartRotated())
            {
                depth = Depth + depthSpacing;
            }
            if (previousAutoDepth == depth)
                return false;

            foreach (var item in Graphics3D.GetVisual())
            {
                if (!(item is UIElement3D) && !(item is PolyLine3D)) continue;
                var count = 0;
                var updatedVector = new Vector3D[item.VectorPoints.Length];
                foreach (var vectorPoint in item.VectorPoints)
                {
                    updatedVector[count] = new Vector3D(vectorPoint.X, vectorPoint.Y, depth);
                    count++;
                }

                item.VectorPoints = updatedVector;
            }
            previousAutoDepth = depth;
            return true;
        }

        private void ChartMouseMove(object source, Point position)
        {
            var element = source as FrameworkElement;
            var segment = element != null ? element.Tag as ChartSegment : null;
            if (segment != null)
            {
                ((ChartSeries3D)segment.Series).OnSeriesMouseMove(source, position);
            }
            RotateChart(position);
        }

        private void ChartMouseDown(object source, Point position, object pointer)
        {
            previousChartPosition = position;
            rotationActivated = EnableRotation;
            var element = source as FrameworkElement;
#if WINDOWS_PHONE
            element.CaptureMouse();
#else
            CapturePointer(pointer as Pointer);
#endif
            var segment = element != null ? element.Tag as ChartSegment : null;
            if (segment == null) return;

            ((ChartSeries3D)segment.Series).OnSeriesMouseDown(source, position);
            
        }

        private void ChartMouseUp(object source, Point position, object pointer)
        {
            rotationActivated = false;
            var element = source as FrameworkElement;
#if WINDOWS_PHONE
            if (element != null)
                element.ReleaseMouseCapture();
#else
            ReleasePointerCapture(pointer as Pointer);
#endif
            var segment = element != null ? element.Tag as ChartSegment : null;
            if (segment == null) return;
            ((ChartSeries3D)segment.Series).OnSeriesMouseUp(source, position);

        }

#if WINDOWS_PHONE

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            ChartMouseMove(e.OriginalSource, e.GetPosition(this));
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            ChartMouseDown(e.OriginalSource, e.GetPosition(this), null);
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            ChartMouseUp(e.OriginalSource, e.GetPosition(this), null);
        }

#else
        /// <summary>
        /// Called before the PointerMoved event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            ChartMouseMove(e.OriginalSource, e.GetCurrentPoint(this).Position);
            base.OnPointerMoved(e);
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            ChartMouseUp(e.OriginalSource, e.GetCurrentPoint(this).Position, e.Pointer);
            ReleasePointerCapture(e.Pointer);
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            ChartMouseDown(e.OriginalSource, e.GetCurrentPoint(this).Position, e.Pointer);
            CapturePointer(e.Pointer);
        }

#endif


#if WINDOWS_PHONE
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
#else
        /// <summary>
        /// Invoke to render sfchart3D
        /// </summary>
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            RootPanel = GetTemplateChild("PART_3DPanel") as Canvas;
            AdorningCanvas = GetTemplateChild("PART_adorningCanvas") as Canvas;
            ChartDockPanel = GetTemplateChild("Part_DockPanel") as ChartDockPanel;
            var layout = GetTemplateChild("Part_LayoutRoot") as ChartRootPanel;
            layout.Area = this;
            controlsPresenter = GetTemplateChild("Part_ControlsPanel") as Panel;
            UpdateAxisLayoutPanels();
            foreach (var visibleSeries in VisibleSeries.Where(visibleSeries => controlsPresenter != null))
            {
                controlsPresenter.Children.Add(visibleSeries);
            }
            UpdateLegend(Legend, true);
            IsTemplateApplied = true;
        }

        /// <summary>
        /// Schedule the 3d update.
        /// </summary>
        void Schedule3DUpdate()
        {
            if (is3DUpdateScheduled) return;
#if NETFX_CORE
            IAsyncAction updateView;
            if (DesignMode.DesignModeEnabled)
                Update3DView();
            else
                updateView=Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Update3DView);
#else
#if WPF
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(Update3DView));
#else
            Dispatcher.BeginInvoke(Update3DView);
#endif
#endif
            is3DUpdateScheduled = true;
        }

        /// <summary>
        /// Raises the <see>
        /// <cref>E:AxisChanged</cref>
        /// </see>
        /// event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void OnAxisChanged(DependencyPropertyChangedEventArgs e)
        {
            var chartAxis = e.NewValue as ChartAxis;

            var oldAxis = e.OldValue as ChartAxis;

            if (Series != null)
                foreach (var series in Series)
                {
                    SetAxisForChartSeries(series as ISupportAxes3D);// Set XAxis and YAxis for each series
                }
            if (oldAxis != null && Axes.Contains(oldAxis))
            {
                Axes.Remove(oldAxis);

                if (oldAxis.RegisteredSeries.Count > 0)
                {
                    var registeredSeriesCol = oldAxis.RegisteredSeries.Cast<ChartSeriesBase>().ToList();
                    foreach (var series in registeredSeriesCol.OfType<ISupportAxes>())
                    {
                        if (((ISupportAxes3D)series).XAxis == oldAxis)
                        {
                            ((ISupportAxes3D)series).XAxis = null;
                        }
                        else if (((ISupportAxes3D)series).YAxis == oldAxis)
                        {
                            (series as ISupportAxes3D).YAxis = null;
                        }
                    }
                }
            }
            if (Axes !=null && chartAxis != null && !Axes.Contains(chartAxis))
            {
                chartAxis.Area = this;
                Axes.Insert(0,chartAxis);
            }
            ScheduleUpdate();
        }

        /// <summary>
        /// Updates the entire chart series and axis
        /// </summary>
        internal override void UpdateAxisLayoutPanels()
        {
            if (AreaType == ChartAreaType.CartesianAxes)
            {
                ChartAxisLayoutPanel = new ChartCartesianAxisLayoutPanel(controlsPresenter)
                {
                    Area = this
                };

                GridLinesLayout = new ChartCartesianGridLinesPanel(null)
                {
                    Area = this
                };
            }
            else
            {
                ChartAxisLayoutPanel = null;
                GridLinesLayout = null;
            }
        }

        /// <summary>
        /// Layouts the axis.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        private void LayoutAxis(Size availableSize)
        {
            if (ChartAxisLayoutPanel != null)
            {
                ChartAxisLayoutPanel.UpdateElements();
                ChartAxisLayoutPanel.Measure(availableSize);
                ChartAxisLayoutPanel.Arrange(availableSize);
            }

            if (GridLinesLayout == null) return;
            GridLinesLayout.UpdateElements();
            GridLinesLayout.Measure(availableSize);
            ((ChartCartesianGridLinesPanel)GridLinesLayout).Arrange3D(availableSize);
        }

        /// <summary>
        /// Update the chart area
        /// </summary>
        /// <param name="forceUpdate"></param>
        internal override void UpdateArea(bool forceUpdate)
        {
#if WINDOWS_PHONE
            if (isUpdateDispatched || forceUpdate)
#else
            if (updateAreaAction != null || forceUpdate)
#endif
            {
                sumByIndex.Clear();
                Graphics3D.ClearVisual();
                if (AreaType == ChartAreaType.CartesianAxes)
                {
                    if (ColumnDefinitions.Count == 0)
                        ColumnDefinitions.Add(new ChartColumnDefinition());
                    if (RowDefinitions.Count == 0)
                        RowDefinitions.Add(new ChartRowDefinition());
                }
                if (VisibleSeries == null)
                    return;
                if (AreaType == ChartAreaType.None)
                {
                    if (VisibleSeries.Count > 0)
                    {
                        CircularSegmentRadius = new double[VisibleSeries.Count];
                    }
                }

                if ((UpdateAction & UpdateAction.Create) == UpdateAction.Create)
                {
                    foreach (var series in VisibleSeries)
                    {
                        if (!series.IsPointGenerated)
                            series.GeneratePoints();
                        if (series.ShowTooltip)
                            ShowTooltip = true;
                    }
                    //Initialize default axes for SfChart when PrimaryAxis or SecondayAxis is not set
                    InitializeDefaultAxes();

                    foreach (var series in VisibleSeries)
                    {
                        series.Invalidate();
                    }
                    if (ShowTooltip)
                        Tooltip = new ChartTooltip();

                }

                if (IsUpdateLegend && (ChartDockPanel != null))
                {
                    UpdateLegend(Legend, false);
                    IsUpdateLegend = false;
                }
                if ((UpdateAction & UpdateAction.UpdateRange) == UpdateAction.UpdateRange)
                {
                    foreach (var series in VisibleSeries)
                    {
                        series.UpdateRange();
                    }
                }

                if (RootPanelDesiredSize != null)
                {
                    if ((UpdateAction & UpdateAction.Layout) == UpdateAction.Layout)
                        LayoutAxis(RootPanelDesiredSize.Value);
                    UpdateLegendArrangeRect();
                    if ((UpdateAction & UpdateAction.Render) == UpdateAction.Render)
                    {
                        if (!isLoaded)
                        {
                            ScheduleRenderSeries();
                            isLoaded = true;
                        }
#if WINDOWS_PHONE
                        else if (!isRenderSeriesDispatched)
#else
                        else if (renderSeriesAction == null)
#endif
                        {
                            RenderSeries();
                        }
                    }
                }

                UpdateAction = UpdateAction.Invalidate;

#if WINDOWS_PHONE
                isUpdateDispatched = false;
#else
                updateAreaAction = null;
#endif
            }
        }

        /// <summary>
        ///  Set default axes for SfChart
        /// </summary>
        internal void InitializeDefaultAxes()
        {
            if (PrimaryAxis == null)
            {
                if (Series.Count == 0)
                    PrimaryAxis = new NumericalAxis3D();
#if WPF || Silverlight
                else if (Series[0] is CartesianSeries3D && isLoaded)
#else
                else  if (Series[0] is CartesianSeries3D && RootPanelDesiredSize!=null)
#endif
                {
                    //get the XAxisValueType from the each series in Series collection which are having ActualXAxis as null
                    var valueTypes = (from series in Series
                                      where (series.ActualXAxis == null)
                                      select series.XAxisValueType).ToList();

                    if (valueTypes.Count > 0)
                        SetPrimaryAxis(valueTypes[0]); //Set PrimaryAxis for SfChart based on XAxisValueType
                    else
                        InternalPrimaryAxis = Series[0].ActualXAxis;
                }
            }

            if (SecondaryAxis == null) SecondaryAxis = new NumericalAxis3D();
        }

        
        /// <summary>
        ///Set PrimaryAxis for SfChart
        /// </summary>
        internal void SetPrimaryAxis(ChartValueType type)
        {
            switch (type)
            {
                case ChartValueType.Double:
                    PrimaryAxis = new NumericalAxis3D();
                    break;
                case ChartValueType.DateTime:
                    PrimaryAxis = new DateTimeAxis3D();
                    break;
                case ChartValueType.String:
                    PrimaryAxis = new CategoryAxis3D();
                    break;
                case ChartValueType.TimeSpan:
                    PrimaryAxis = new TimeSpanAxis3D();
                    break;
            }
        }

        /// <summary>
        /// Schedules the render series.
        /// </summary>
        private void ScheduleRenderSeries()
        {
#if WINDOWS_PHONE
            if (!isRenderSeriesDispatched)
            {
#if WPF 
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(RenderSeries));
#else
                Dispatcher.BeginInvoke(RenderSeries);
#endif
                isRenderSeriesDispatched = true;
            }
#else

            if (DesignMode.DesignModeEnabled)
                RenderSeries();
            else if (renderSeriesAction == null)
                renderSeriesAction = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, RenderSeries);
#endif
        }

        private static void On3DValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SfChart3D)d).Schedule3DUpdate();
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets a value indicating whether [enable rotation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable rotation]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRotation
        {
            get { return (bool)GetValue(EnableRotationProperty); }
            set { SetValue(EnableRotationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableRotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableRotationProperty =
            DependencyProperty.Register("EnableRotation", typeof(bool), typeof(SfChart3D), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the top wall brush.
        /// </summary>
        /// <value>
        /// The top wall brush.
        /// </value>
        public Brush TopWallBrush
        {
            get { return (Brush)GetValue(TopWallBrushProperty); }
            set { SetValue(TopWallBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopWallBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopWallBrushProperty =
            DependencyProperty.Register("TopWallBrush", typeof(Brush), typeof(SfChart3D), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(130, 201, 201, 201))));

        /// <summary>
        /// Gets or sets the bottom wall brush.
        /// </summary>
        /// <value>
        /// The bottom wall brush.
        /// </value>
        public Brush BottomWallBrush
        {
            get { return (Brush)GetValue(BottomWallBrushProperty); }
            set { SetValue(BottomWallBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BottomWallBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomWallBrushProperty =
            DependencyProperty.Register("BottomWallBrush", typeof(Brush), typeof(SfChart3D), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(130, 201, 201, 201))));

        /// <summary>
        /// Gets or sets the right wall brush.
        /// </summary>
        /// <value>
        /// The right wall brush.
        /// </value>
        public Brush RightWallBrush
        {
            get { return (Brush)GetValue(RightWallBrushProperty); }
            set { SetValue(RightWallBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightWallBrush.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The right wall brush property
        /// </summary>
        public static readonly DependencyProperty RightWallBrushProperty =
            DependencyProperty.Register("RightWallBrush", typeof(Brush), typeof(SfChart3D), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(130, 201, 201, 201))));

        /// <summary>
        /// Gets or sets the left wall brush.
        /// </summary>
        /// <value>
        /// The left wall brush.
        /// </value>
        public Brush LeftWallBrush
        {
            get { return (Brush)GetValue(LeftWallBrushProperty); }
            set { SetValue(LeftWallBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftWallBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftWallBrushProperty =
            DependencyProperty.Register("LeftWallBrush", typeof(Brush), typeof(SfChart3D), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(130, 201, 201, 201))));

        /// <summary>
        /// Gets or sets the back wall brush.
        /// </summary>
        /// <value>
        /// The back wall brush.
        /// </value>
        public Brush BackWallBrush
        {
            get { return (Brush)GetValue(BackWallBrushProperty); }
            set { SetValue(BackWallBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackWallBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackWallBrushProperty =
            DependencyProperty.Register("BackWallBrush", typeof(Brush), typeof(SfChart3D), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(130, 201, 201, 201))));

        

        /// <summary>
        /// Gets or sets the perspective angle.
        /// </summary>
        /// <value>
        /// The perspective angle.
        /// </value>
        public double PerspectiveAngle
        {
            get { return (double)GetValue(PerspectiveAngleProperty); }
            set { SetValue(PerspectiveAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PerspectiveAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PerspectiveAngleProperty =
            DependencyProperty.Register("PerspectiveAngle", typeof(double), typeof(SfChart3D), new PropertyMetadata(90d, OnPerspectiveAngleChanged));

        /// <summary>
        /// Gets or Sets primary axis.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartAxisBase3D PrimaryAxis
        {
            get { return (ChartAxisBase3D)GetValue(PrimaryAxisProperty); }
            set { SetValue(PrimaryAxisProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for PrimaryAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PrimaryAxisProperty =
            DependencyProperty.Register("PrimaryAxis", typeof(ChartAxisBase3D), typeof(SfChart3D), new PropertyMetadata(null, OnPrimaryAxisChanged));

        private static void OnPrimaryAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((SfChart3D)d).InternalPrimaryAxis = (ChartAxis)e.NewValue;
                ((ChartAxis)e.NewValue).Orientation = Orientation.Horizontal;
            }
            ((SfChart3D)d).OnAxisChanged(e);

        }

        /// <summary>
        /// Gets or Sets secondary axis.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public RangeAxisBase3D SecondaryAxis
        {
            get { return (RangeAxisBase3D)GetValue(SecondaryAxisProperty); }
            set { SetValue(SecondaryAxisProperty, value); }
        }


        /// <summary>
        ///  Using a DependencyProperty as the backing store for SecondaryAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SecondaryAxisProperty =
            DependencyProperty.Register("SecondaryAxis", typeof(RangeAxisBase3D), typeof(SfChart3D), new PropertyMetadata(null, OnSecondaryAxisChanged));

        private static void OnSecondaryAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((SfChart3D)d).InternalSecondaryAxis = (ChartAxis)e.NewValue;
                ((ChartAxis)e.NewValue).Orientation = Orientation.Vertical;
            }

            ((SfChart3D)d).OnAxisChanged(e);

        }

        /// <summary>
        /// Gets or sets the series.
        /// </summary>
        /// <value>
        /// The series.
        /// </value>
        public ChartSeries3DCollection Series
        {
            get { return (ChartSeries3DCollection)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(ChartSeries3DCollection), typeof(SfChart3D), new PropertyMetadata(null, OnSeriesPropertyCollectionChanged));

        private static void OnSeriesPropertyCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SfChart3D)d).OnSeriesPropertyCollectionChanged(e);
        }

        /// <summary>
        /// Called when [series collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    {
                        if (controlsPresenter != null)
                        {
                            for (var i = controlsPresenter.Children.Count - 1; i >= 0; i--)
                            {
                                if (!(controlsPresenter.Children[i] is ChartSeries3D)) continue;
                                var series = controlsPresenter.Children[i] as ISupportAxes;
                                if (series != null)
                                {
                                    var cartesianSeries = series as CartesianSeries3D;
                                    if (cartesianSeries != null)
                                    {
                                        cartesianSeries.YAxis = null;
                                        cartesianSeries.XAxis = null;
                                    }
                                }
                                controlsPresenter.Children.RemoveAt(i);
                            }
                        }
                        ActualSeries.Clear();
                        VisibleSeries.Clear();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var series = e.OldItems[0] as ChartSeriesBase;

                        if (series is ISupportAxes && series.ActualYAxis.RegisteredSeries != null &&
                            series.ActualYAxis.RegisteredSeries.Contains(series as ISupportAxes))
                        {
                            var cartesianSeries = series as CartesianSeries3D;
                            if (cartesianSeries != null)
                            {
                                cartesianSeries.YAxis = null;
                                cartesianSeries.XAxis = null;
                            }
                        }

                        if (VisibleSeries.Contains(series))
                            VisibleSeries.Remove(series);
                        if (ActualSeries.Contains(series))
                            ActualSeries.Remove(series);
                        controlsPresenter.Children.Remove(series);
                        series.RemoveTooltip();
                        if (VisibleSeries.Count == 0 && Series.Count > 0)
                        {
                            if (Series[0] is CircularSeriesBase3D)
                                AreaType = ChartAreaType.None;
                            else
                                AreaType = ChartAreaType.CartesianAxes;
                            UpdateVisibleSeries(Series);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex == 0)
                    {
                        if (Series[0] is CircularSeriesBase3D)
                            AreaType = ChartAreaType.None;
                        else
                            AreaType = ChartAreaType.CartesianAxes;
                    }
                    UpdateVisibleSeries(e.NewItems);
                    break;
            }
            var canvas = GetAdorningCanvas();
            if (canvas != null)
            {
                foreach (var item in canvas.Children.OfType<ChartTooltip>())
                {
                    canvas.Children.Remove(item);
                }
            }
            IsUpdateLegend = true;
            ScheduleUpdate();
            SBSInfoCalculated = false;
        }


        private void UpdateVisibleSeries(IList seriesColl)
        {
            foreach (ChartSeries3D series in seriesColl)
            {
                series.UpdateLegendIconTemplate(false);
                SetAxisForChartSeries(series as ISupportAxes3D);
                series.Area = this;
                if (series.ActualXAxis != null && !this.Axes.Contains(series.ActualXAxis))
                {
                    series.ActualXAxis.Area = this;
                    Axes.Add(series.ActualXAxis);
                }
                if (series.ActualYAxis != null && !this.Axes.Contains(series.ActualYAxis))
                {
                    series.ActualYAxis.Area = this;
                    Axes.Add(series.ActualYAxis);
                }
                if (controlsPresenter != null && !this.controlsPresenter.Children.Contains(series))
                {
                    controlsPresenter.Children.Add(series);
                }
                if (series.IsSeriesVisible)
                {
                    if (AreaType == ChartAreaType.None && series is CircularSeriesBase3D)
                    {
                        VisibleSeries.Add(series);
                    }
                    else if (AreaType == ChartAreaType.CartesianAxes && series is CartesianSeries3D)
                    {
                        VisibleSeries.Add(series);
                    }
                }
                base.ActualSeries.Add(series);
            }
        }

        /// <summary>
        /// Raises the <see>
        ///     <cref>E:SeriesPropertyCollectionChanged</cref>
        /// </see>
        ///     event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnSeriesPropertyCollectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((ChartSeries3DCollection)e.OldValue).Clear();
                ((ChartSeries3DCollection)e.OldValue).CollectionChanged -= OnSeriesCollectionChanged;
                VisibleSeries.Clear();
                ActualSeries.Clear();
            }
            if (Series == null) return;
                Series.CollectionChanged += OnSeriesCollectionChanged;
            if(Series.Count <= 0) return;
            if (Series[0] is CircularSeriesBase3D)
                AreaType = ChartAreaType.None;
            else
                AreaType = ChartAreaType.CartesianAxes;
            UpdateVisibleSeries(Series);
            UpdateLegend(Legend, false);
            ScheduleUpdate();
        }

        private static void OnPerspectiveAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var chart = d as SfChart3D;
            if (chart != null) chart.OnPerspectiveAngleChanged();
        }

        private void OnPerspectiveAngleChanged()
        {
            if (RootPanel == null || RootPanelDesiredSize == null) return;
            Graphics3D.View(RootPanel, Rotation, Tilt, RootPanelDesiredSize.Value, PerspectiveAngle, Depth);
        }

        /// <summary>
        /// Gets or sets the tilt.
        /// </summary>
        /// <value>
        /// The tilt.
        /// </value>
        public double Tilt
        {
            get { return (double)GetValue(TiltProperty); }
            set { SetValue(TiltProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tilt.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TiltProperty =
            DependencyProperty.Register("Tilt", typeof(double), typeof(SfChart3D), new PropertyMetadata(0d, On3DValuesChanged));

        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public double Depth
        {
            get { return (double)GetValue(DepthProperty); }
            set { SetValue(DepthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DepthProperty =
            DependencyProperty.Register("Depth", typeof(double), typeof(SfChart3D), new PropertyMetadata(30d, OnDepthPropertyChanged));

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        public double Rotation
        {
            get { return (double)GetValue(RotationProperty); }
            set { SetValue(RotationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register("Rotation", typeof(double), typeof(SfChart3D), new PropertyMetadata(0d, On3DValuesChanged));

        private static void OnDepthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((SfChart3D)d).ScheduleUpdate();
        }

        #endregion

    }
}
