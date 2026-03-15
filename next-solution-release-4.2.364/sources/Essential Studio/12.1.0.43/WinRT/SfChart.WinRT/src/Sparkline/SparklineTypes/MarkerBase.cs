#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINDOWS_PHONE
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract class MarkerBase : SparklineBase
    {

        #region fields


        Line trackLine, axisLine;

        Ellipse trackBall;

        SparklinePointsInfo sparklineInfo;

        Canvas trackBallCanvas;

        internal Canvas MarkerPresenter { get; set; }

        #endregion

        #region ctor

        public MarkerBase()
        {
            this.DefaultStyleKey = typeof(MarkerBase);
        }

        #endregion

        #region methods

        protected override void SetIndividualPoints(int index, object obj, bool replace, string xPath)
        {
            base.SetIndividualPoints(index, obj, replace, XBindingPath);
        }

        protected override void GeneratePoints(string xPath)
        {
            base.GeneratePoints(XBindingPath);
        }

        protected void RemoveAxis()
        {
            if (RootPanel.Children.Contains(axisLine))
                RootPanel.Children.Remove(axisLine);
            axisLine = null;
        }

        protected virtual void UpdateHorizontalAxis()
        {
            if (RootPanel != null && ShowAxis)
            {
                if (axisLine == null)
                {
                    axisLine = new Line();
                    StyleBinding(axisLine, AxisStyle, "AxisStyle");
                    RootPanel.Children.Add(axisLine);
                }
                Point pos = TransformToVisible(0, AxisOrigin);
                axisLine.X1 = 0;
                axisLine.X2 = availableWidth;
                axisLine.Y1 = pos.Y;
                axisLine.Y2 = pos.Y;
            }
        }

        private static void OnShowAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
                (d as MarkerBase).RemoveAxis();
            else
                (d as MarkerBase).UpdateHorizontalAxis();
        }

        private static void OnAxisOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MarkerBase).UpdateHorizontalAxis();
        }

        protected override void AnimateSegments(UIElementCollection elements)
        {
            var seriesRect = new Rect(0, 0, availableWidth, availableHeight);
#if WPF
            RectangleGeometry geometry = new RectangleGeometry();
            RootPanel.Clip = geometry;
            System.Windows.Media.Animation.RectAnimation animation = new System.Windows.Media.Animation.RectAnimation()
#else
            var animation = new RectAnimation()
#endif
            {
                From = new Rect(0, seriesRect.Y, 0, seriesRect.Height),
                To = new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height),
                Duration = TimeSpan.FromSeconds(0.7)

            };
#if WPF
            RectAnimationUsingKeyFrames keyFrames = new RectAnimationUsingKeyFrames();
            SplineRectKeyFrame keyFrame = new SplineRectKeyFrame(new Rect(0, seriesRect.Y, 0, seriesRect.Height), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
            keyFrames.KeyFrames.Add(keyFrame);
            keyFrame = new SplineRectKeyFrame(new Rect(0, seriesRect.Y, seriesRect.Width, seriesRect.Height), KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)));
            keyFrames.KeyFrames.Add(keyFrame);
            keyFrame.KeySpline = new KeySpline(0.65, 0.84, 0.67, 0.95);

            geometry.BeginAnimation(RectangleGeometry.RectProperty, keyFrames);
#else
            animation.SetTarget(RootPanel);
            animation.Begin();
#endif
        }

        public override void Reset()
        {
            MarkerPresenter.Children.Clear();
            base.Reset();
        }

        private void UpdateMarker(double screenPointX, double screenPointY, double x, double y, FrameworkElement element)
        {
            if (MarkerTemplateSelector != null)
            {
                if (element is ContentPresenter)
                {
                    ContentPresenter marker = element as ContentPresenter;
                    marker.ContentTemplate = MarkerTemplateSelector.SelectTemplate(x, y);
                    marker.Measure(new Size(availableWidth, availableHeight));
                    marker.SetValue(Canvas.LeftProperty, screenPointX - marker.DesiredSize.Width / 2);
                    marker.SetValue(Canvas.TopProperty, screenPointY - marker.DesiredSize.Height / 2);
                }
                else
                {
                    Shape marker = element as Shape;
                    (MarkerTemplateSelector as MarkerTemplateSelector).BindVisual(x, y, marker);
                    PlaceMarker(marker, new Point(screenPointX, screenPointY));
                }
            }
            else
                PlaceMarker(element, new Point(screenPointX, screenPointY));
        }

        private void PlaceMarker(FrameworkElement marker, Point screenPoint)
        {
            marker.SetValue(Canvas.LeftProperty, screenPoint.X - (marker.Width / 2));
            marker.SetValue(Canvas.TopProperty, screenPoint.Y - (marker.Height / 2));
        }

        public void AddMarker(double screenPointX, double screenPointY, double x, double y)
        {
            Ellipse ellips;
            if (MarkerTemplateSelector != null)
            {
                ContentPresenter marker;
                DataTemplate markerTemplate = MarkerTemplateSelector.SelectTemplate(x, y);
                if (markerTemplate == null)
                {
                    ellips = new Ellipse();
                    (MarkerTemplateSelector as MarkerTemplateSelector).BindVisual(x, y, ellips);
                    PlaceMarker(ellips, new Point(screenPointX, screenPointY));
                    MarkerPresenter.Children.Add(ellips);
                }
                else
                {
                    marker = new ContentPresenter();
                    marker.ContentTemplate = markerTemplate;
                    marker.Measure(new Size(availableWidth, availableHeight));
                    marker.SetValue(Canvas.LeftProperty, screenPointX - marker.DesiredSize.Width / 2);
                    marker.SetValue(Canvas.TopProperty, screenPointY - marker.DesiredSize.Height / 2);
                    MarkerPresenter.Children.Add(marker);
                }
            }
            else
            {
                ellips = new Ellipse();
                Binding defaultBinding = new Binding();
                defaultBinding.Path = new PropertyPath("Interior");
                defaultBinding.Source = this;
                ellips.SetBinding(Shape.FillProperty, defaultBinding);
                ellips.Height = 10;
                ellips.Width = 10;
                PlaceMarker(ellips, new Point(screenPointX, screenPointY));
                MarkerPresenter.Children.Add(ellips);
            }
        }

        protected override void RenderSegments()
        {
            if (ShowAxis)
                UpdateHorizontalAxis();
            if (MarkerTemplateSelector != null)
                MarkerTemplateSelector.SetData(this, DataCount);
            if (MarkerPresenter != null)
                ClearUnUsedMarkers(xValues.Count - (EmptyPointIndexes.Count - 1));
            ClearUnUsedSegments(EmptyPointIndexes.Count);
        }

        internal virtual void ClearUnUsedMarkers(int dataCount)
        {
            if (MarkerVisibility == Visibility.Visible)
            {
                int count = MarkerPresenter.Children.Count;
                if (count > dataCount)
                {
                    for (int i = dataCount; i < count; i++)
                    {
                        MarkerPresenter.Children.RemoveAt(dataCount);
                    }
                }
            }
            else
                MarkerPresenter.Children.Clear();
        }

        private void CreateTrackBall()
        {
            trackBallCanvas = new Canvas();
            trackLine = new Line();
            trackBall = new Ellipse();
            StyleBinding(trackBall, TrackBallStyle, "TrackBallStyle");
            StyleBinding(trackLine, TrackBallStyle, "LineStyle");
            trackBallCanvas.Children.Add(trackLine);
            trackBallCanvas.Children.Add(trackBall);
            this.RootPanel.Children.Add(trackBallCanvas);
        }

        private static void OnShowTrackBallChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
                (d as MarkerBase).ResetTrackBallCanvas();
        }

        private static void OnMarkerVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MarkerBase).OnMarkerVisibilityChanged(e);
        }

        private void OnMarkerVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((Visibility)e.NewValue == Visibility.Visible)
            {
                if (MarkerPresenter == null && RootPanel != null)
                {
                    MarkerPresenter = new Canvas();
                    RootPanel.Children.Add(MarkerPresenter);
                }
                UpdateArea();
            }
            else
            {
                RootPanel.Children.Remove(MarkerPresenter);
                MarkerPresenter.Children.Clear();
                MarkerPresenter = null;
            }
        }

        private void ResetTrackBallCanvas()
        {
            if (trackBallCanvas != null)
            {
                this.trackBallCanvas.Children.Clear();
                trackLine = null;
                trackBall = null;
                this.RootPanel.Children.Remove(trackBallCanvas);
            }
        }

        protected void AddMarker(Point pointToScreen, double x, double y, int index)
        {
            if (MarkerPresenter != null)
            {
                if (MarkerPresenter.Children.Count > index)
                    UpdateMarker(pointToScreen.X, pointToScreen.Y, x, y, (FrameworkElement)MarkerPresenter.Children[index]);
                else
                    AddMarker(pointToScreen.X, pointToScreen.Y, x, y);
            }
        }

#if WINDOWS_PHONE
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            if (MarkerVisibility == Visibility.Visible)
            {
                if (MarkerPresenter == null)
                {
                    MarkerPresenter = new Canvas();
                    RootPanel.Children.Add(MarkerPresenter);
                }
            }
        }

#if !WINDOWS_PHONE
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
#else
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
#endif
        {
#if !WINDOWS_PHONE
            base.OnPointerMoved(e);
#else
            base.OnMouseMove(e);
#endif
            if (ShowTrackBall)
            {
                if (trackLine == null)
                    CreateTrackBall();
#if WINDOWS_PHONE
                Point point = e.GetPosition(this);
                sparklineInfo = FindPoints(point.X, point.Y);
#else
                PointerPoint point = e.GetCurrentPoint(this);
                sparklineInfo = FindPoints(point.Position.X, point.Position.Y);
#endif
                double x = sparklineInfo.Coordinate.X;
                double y = sparklineInfo.Coordinate.Y;
                if (!double.IsNaN(x) && !double.IsNaN(y))
                {
                    trackLine.X1 = x;
                    trackLine.X2 = x;
                    trackLine.Y2 = availableHeight;
                    trackBall.SetValue(Canvas.LeftProperty, x - trackBall.Width / 2);
                    trackBall.SetValue(Canvas.TopProperty, y - trackBall.Height / 2);
                }
            }
        }

        internal override void SetBinding(Shape element)
        {
            base.SetBinding(element);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Interior");
            element.SetBinding(Shape.StrokeProperty, binding);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or Sets the property path of the x data in ItemsSource.
        /// </summary>
        public string XBindingPath
        {
            get { return (string)GetValue(XBindingPathProperty); }
            set { SetValue(XBindingPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XBindingPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XBindingPathProperty =
            DependencyProperty.Register("XBindingPath", typeof(string), typeof(MarkerBase), new PropertyMetadata(String.Empty));

        /// <summary>
        /// Gets or sets the axis line style.
        /// </summary>
        public Style AxisStyle
        {
            get { return (Style)GetValue(AxisStyleProperty); }
            set { SetValue(AxisStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisStyleProperty =
            DependencyProperty.Register("AxisStyle", typeof(Style), typeof(MarkerBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets a value whether to show/hide axis
        /// </summary>
        public bool ShowAxis
        {
            get { return (bool)GetValue(ShowAxisProperty); }
            set { SetValue(ShowAxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowAxisProperty =
            DependencyProperty.Register("ShowAxis", typeof(bool), typeof(MarkerBase), new PropertyMetadata(false, OnShowAxisChanged));

        /// <summary>
        /// Gets or Sets axis origin
        /// </summary>
        public double AxisOrigin
        {
            get { return (double)GetValue(AxisOriginProperty); }
            set { SetValue(AxisOriginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisOrigin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisOriginProperty =
            DependencyProperty.Register("AxisOrigin", typeof(double), typeof(MarkerBase), new PropertyMetadata(0d, OnAxisOriginChanged));

        /// <summary>
        /// Gets or sets the track ball style.
        /// </summary>
        public Style TrackBallStyle
        {
            get { return (Style)GetValue(TrackBallStyleProperty); }
            set { SetValue(TrackBallStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrackBallStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackBallStyleProperty =
            DependencyProperty.Register("TrackBallStyle", typeof(Style), typeof(SparklineBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets line style for track ball.
        /// </summary>
        public Style LineStyle
        {
            get { return (Style)GetValue(LineStyleProperty); }
            set { SetValue(LineStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register("LineStyle", typeof(Style), typeof(SparklineBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value for show track ball.
        /// </summary>
        public bool ShowTrackBall
        {
            get { return (bool)GetValue(ShowTrackBallProperty); }
            set { SetValue(ShowTrackBallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowTrackBall.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowTrackBallProperty =
            DependencyProperty.Register("ShowTrackBall", typeof(bool), typeof(MarkerBase), new PropertyMetadata(false, OnShowTrackBallChanged));

        /// <summary>
        /// Gets or sets the marker visibility.
        /// </summary>
        public Visibility MarkerVisibility
        {
            get { return (Visibility)GetValue(MarkerVisibilityProperty); }
            set { SetValue(MarkerVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerVisibilityProperty =
            DependencyProperty.Register("MarkerVisibility", typeof(Visibility), typeof(MarkerBase), new PropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnMarkerVisibilityChanged)));

        /// <summary>
        /// Gets or Sets marker template selector to customize the each markers
        /// </summary>
        public TemplateSelector MarkerTemplateSelector
        {
            get { return (TemplateSelector)GetValue(MarkerTemplateSeletorProperty); }
            set { SetValue(MarkerTemplateSeletorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerTemplateSeletor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerTemplateSeletorProperty =
            DependencyProperty.Register("MarkerTemplateSeletor", typeof(TemplateSelector), typeof(MarkerBase), new PropertyMetadata(null));

        #endregion

    }
}
