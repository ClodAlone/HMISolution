#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WindowsLineSegment = System.Windows.Media.LineSegment;
#else
using Windows.UI.Xaml;
using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for PieSeries3D
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class PieSeries3D : CircularSeriesBase3D
    {
        #region fields

        bool allowExplode;

        ChartSegment mouseUnderSegment;

        protected internal double InsideRadius = 0;

        protected double actualWidth, actualHeight;

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="PieSeries3D"/> class.
        /// </summary>
        public PieSeries3D()
        {
            DefaultStyleKey = typeof(PieSeries3D);
        }

        #endregion

        #region properties

        /// <summary>
        /// Return IChartTranform value based upon the given size
        /// </summary>
        /// <param name="size"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        protected internal override IChartTransformer CreateTransformer(Size size, bool create)
        {
            if (create || ChartTransformer == null)
            {
                ChartTransformer = ChartTransform.CreateSimple(size);
            }

            return ChartTransformer;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [explode on mouse click].
        /// </summary>
        /// <value>
        /// <c>true</c> if [explode on mouse click]; otherwise, <c>false</c>.
        /// </value>
        public bool ExplodeOnMouseClick
        {
            get { return (bool)GetValue(ExplodeOnMouseClickProperty); }
            set { SetValue(ExplodeOnMouseClickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExplodeOnMouseClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExplodeOnMouseClickProperty =
            DependencyProperty.Register("ExplodeOnMouseClick", typeof(bool), typeof(PieSeries3D), new PropertyMetadata(false));

        #endregion

        #region methods

        internal override void UpdateOnSeriesBoundChanged(Size size)
        {
            if (AdornmentsInfo != null)
            {
                AdornmentsInfo.UpdateElements();
                AdornmentsInfo.Measure(size, null);
            }

            var canUpdate = !(this is ISupportAxes) || this is ISupportAxes && ActualXAxis != null && ActualYAxis != null;

            if (!canUpdate) return;
            var chartTransformer = CreateTransformer(size, true);

            List<Polygon3D>[] poligons = new List<Polygon3D>[]{ new List<Polygon3D>(), new List<Polygon3D>(),
                                              new List<Polygon3D>(), new List<Polygon3D>() };

            foreach (PieSegment3D segment in Segments)
            {
                segment.CreateSegmentVisual(size);
                var plgs = segment.CreateSector();
                if(plgs!= null)
                    for (int ai = 0; ai < plgs.Length; ai++)
                    {
                        if (plgs[ai] != null)
                        {
                            for (int pi = 0; pi < plgs[ai].Length; pi++)
                            {
                                poligons[ai].Add(plgs[ai][pi]);

                            }
                        }
                    }
            }

            for (int ai = 0; ai < poligons.Length; ai++)
            {
                foreach (var item in poligons[ai])
                {
                    Area.Graphics3D.AddVisual(item);
                }
            }
        }

        /// <summary>
        /// An abstract method which will be called over to create segments.
        /// </summary>
        public override void CreateSegments()
        {
            InsideRadius = 0d;
            if (Area != null)
                CreatePoints();
            if (ShowEmptyPoints)
                UpdateEmptyPointSegments(GetXValues());
        }

        private void AddPieAdornments(double x, double y, double startAngle, double endAngle, int i, double radius, double startDepth)
        {
            var angle = (startAngle + endAngle) / 2;
         
            if (i < Adornments.Count)
            {
                Adornments[i].SetData(x, y, angle, radius);
            }
            else
            {
                Adornments.Add(CreateAdornment(this, x, y, angle, radius, startDepth));
            }
            Adornments[i].Item = ActualData[i];
        }

        /// <summary>
        /// Method implementation for Create Adornments
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="xVal">The x value.</param>
        /// <param name="yVal">The y value.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        protected ChartAdornment CreateAdornment(ChartSeries3D series, double xVal, double yVal, double angle, double radius, double startDepth)
        {
            return new ChartPieAdornment3D(startDepth, xVal, yVal, angle, radius, series);
        }

        private double CalculateSegmentRadius()
        {
            var pieSeries = (from series in Area.VisibleSeries where series is PieSeries3D select series).ToList();
            var pieSeriesCount = pieSeries.Count();
            var pieIndex = pieSeries.IndexOf(this);
            if (pieIndex == 0)
                Area.CircularSegmentRadius[pieIndex] = Math.Pow(2, pieSeriesCount);
            else
                Area.CircularSegmentRadius[pieIndex] = Area.CircularSegmentRadius[pieIndex - 1] / 2;

            return Area.CircularSegmentRadius[pieIndex];
        }
        /// <summary>
        /// Creates the points.
        /// </summary>
        protected virtual void CreatePoints()
        {
            var pieIndex = GetPieSeriesIndex();
            ClearUnUsedSegments(DataCount);
            Adornments.Clear();
            if (Area.RootPanelDesiredSize != null)
            {
                actualWidth = Area.RootPanelDesiredSize.Value.Width;
                actualHeight = Area.RootPanelDesiredSize.Value.Height;
            }
            var all = YValues.Select(item => Math.Abs(item)).Sum();
            var coef = 360d / all;
            var count = YValues.Count;
            var radius = (CircleCoefficient * Math.Min(actualWidth, actualHeight)) / CalculateSegmentRadius();
            if (pieIndex > 0)
            {
                InsideRadius = radius * CircleCoefficient;
            }
            var pieHeight = Area.Depth;
            if (ExplodeIndex >= 0 && ExplodeIndex < count || ExplodeAll)
            {
                radius -= ExplodeRadius;
            }
            double arcStartAngle = 0, arcEndAngle;
            double current = 0;
            for (var i = 0; i < count; i++)
            {
                var val = Math.Abs(YValues[i]);
                arcEndAngle = Math.Abs(val) * ((Math.PI * 2) / all);
                var rect = new Rect(0, 0, actualWidth, actualHeight);
                if (val != 0)
                {
                    if (ExplodeIndex == i || ExplodeAll)
                    {
                        var offset = new Point(
                            (float)(Math.Cos(2 * Math.PI * (current + val / 2) / all)),
                            (float)(Math.Sin(2 * Math.PI * (current + val / 2) / all)));
#if !WPF
                    rect = rect.Offset(0.01f * radius * offset.X * ExplodeRadius,
                        0.01f * radius * offset.Y * ExplodeRadius);
#else
                        rect.Offset(0.01f * radius * offset.X * ExplodeRadius,
                            0.01f * radius * offset.Y * ExplodeRadius);
#endif
                    }

                    var center = new Vector3D(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, 0);
                    if (i < Segments.Count)
                        Segments[i].SetData((float)(coef * current), (float)(coef * val), pieHeight, radius, val, center.X, center.Y, center.Z, InsideRadius);
                    else
                        Segments.Add(new PieSegment3D(this, center, (float)(coef * current), (float)(coef * val), pieHeight, radius, i, val, InsideRadius));
                }
                if (AdornmentsInfo != null)
                    AddPieAdornments(i, YValues[i], arcStartAngle, arcStartAngle + arcEndAngle, i, radius, Area.IsChartRotated() ? Area.Depth + 5d : 0d);

                arcStartAngle += arcEndAngle;
                current += val;
            }
        }

        private Storyboard sb;
        /// <summary>
        /// Gets the animation is active.
        /// </summary>
        /// <returns></returns>
        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

#if NETFX_CORE
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public PieSegment3D Segment { get; set; }
#endif

        /// <summary>
        /// Animates this instance.
        /// </summary>
        internal override void Animate()
        {
            sb = new Storyboard();
            foreach (PieSegment3D segment in Segments)
            {
                var segmentStartAngle = segment.StartValue;
                var segmentEndAngle = segment.EndValue;
                var keyFrames = new DoubleAnimationUsingKeyFrames();
                var keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = 0
                };
                keyFrames.KeyFrames.Add(keyFrame);

                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(AnimationDuration),
                    Value = segmentStartAngle
                };
                var keySpline = new KeySpline
                {
                    ControlPoint1 = new Point(0.64, 0.84),
                    ControlPoint2 = new Point(0.67, 0.95)
                };
                keyFrame.KeySpline = keySpline;

                keyFrames.KeyFrames.Add(keyFrame);
#if !WINDOWS_PHONE
                Storyboard.SetTargetProperty(keyFrames, "PieSegment3D.ActualStartValue");
                keyFrames.EnableDependentAnimation = true;
#else
                Storyboard.SetTargetProperty(keyFrames, new PropertyPath(PieSegment3D.ActualStartValueProperty));
#endif
                Storyboard.SetTarget(keyFrames, segment);
                sb.Children.Add(keyFrames);

                keyFrames = new DoubleAnimationUsingKeyFrames();
                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = 0
                };
                keyFrames.KeyFrames.Add(keyFrame);

                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(AnimationDuration),
                    Value = segmentEndAngle
                };
                keySpline = new KeySpline
                {
                    ControlPoint1 = new Point(0.64, 0.84),
                    ControlPoint2 = new Point(0.67, 0.95)
                };
                keyFrame.KeySpline = keySpline;

                keyFrames.KeyFrames.Add(keyFrame);
#if !WINDOWS_PHONE
                Storyboard.SetTargetProperty(keyFrames, "PieSegment3D.ActualEndValue");
                keyFrames.EnableDependentAnimation = true;
#else
                Storyboard.SetTargetProperty(keyFrames, new PropertyPath(PieSegment3D.ActualEndValueProperty));
#endif
                Storyboard.SetTarget(keyFrames, segment);
                sb.Children.Add(keyFrames);
            }
            sb.Begin();
        }

        /// <summary>
        /// Called when [series mouse move].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pos">The position.</param>
        protected internal override void OnSeriesMouseMove(object source, Point pos)
        {
            allowExplode = false;
            base.OnSeriesMouseMove(source, pos);
        }

        /// <summary>
        /// Called when [series mouse up].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pos">The position.</param>
        protected internal override void OnSeriesMouseUp(object source, Point pos)
        {
            var element = source as FrameworkElement;
            var segment = element != null ? element.Tag as ChartSegment : null;
            if (segment is PieSegment3D && ExplodeOnMouseClick && mouseUnderSegment == segment && allowExplode)
            {
                var newIndex = Segments.IndexOf(segment);
                var oldIndex = ExplodeIndex;
                if (newIndex != oldIndex)
                    ExplodeIndex = newIndex;
                else if (ExplodeIndex >= 0)
                    ExplodeIndex = -1;
                allowExplode = false;
            }
            base.OnSeriesMouseUp(source, pos);
        }

        /// <summary>
        /// Called when [series mouse down].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pos">The position.</param>
        protected internal override void OnSeriesMouseDown(object source, Point pos)
        {
            allowExplode = true;
            var element = source as FrameworkElement;
            mouseUnderSegment = element != null ? element.Tag as ChartSegment : null;
            base.OnSeriesMouseDown(source, pos);
        }

        #endregion
    }
}
