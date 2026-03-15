#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// DoughnutSeries displays data as a proportion of the whole.DoughnutSeries are most commonly used to make comparisons among a set of given data.
    /// </summary>
    /// <remarks>
    /// DoughnutSeries does not have any axis. The segments in DoughnutSeries can be exploded to a certain distance from the center using <see>
    ///                                                                                                                                       <cref>DoughnutSeries.ExplodeIndex</cref>
    ///                                                                                                                                   </see>
    ///     or <see>
    ///            <cref>DoughnutSeries.ExplodeAll</cref>
    ///        </see>
    ///     property.
    /// The segments can be filled with a custom set of colors using <see cref="ChartColorModel.CustomBrushes"/> property.
    /// </remarks>
    /// <seealso cref="DoughnutSegment"/>
    /// <seealso cref="PieSeries"/>
    /// <seealso cref="PieSegment"/>
    public class DoughnutSeries : CircularSeriesBase,ISegmentSelectable
    {
        #region Fields

        private const double ARCLENGTH = Math.PI * 2;

        internal const double DOUGHNUTSIZE = 0.8d;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets coefficient, which determines the radius of doughnut series.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double DoughnutCoefficient
        {
            get { return (double)GetValue(DoughnutCoefficientProperty); }
            set { SetValue(DoughnutCoefficientProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for DoughnutCoefficient.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DoughnutCoefficientProperty =
            DependencyProperty.Register("DoughnutCoefficient", typeof(double), typeof(DoughnutSeries), new PropertyMetadata(0.4d, new PropertyChangedCallback(OnDoughnutCoefficientChanged)));

         private static void OnDoughnutCoefficientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DoughnutSeries).SetDoughnutCoefficient((double)e.NewValue);
        }

        private void SetDoughnutCoefficient(double n)
        {
            if (Area != null)
                Area.ScheduleUpdate();
        }

#if NETFX_CORE
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public DoughnutSegment Segment { get { return null; } }
#endif

        #endregion

        #region constructor

        /// <summary>
        /// Called when instance created for DoughnutSeries
        /// </summary>
        public DoughnutSeries()
        {
            DefaultStyleKey = typeof(DoughnutSeries);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the doughnut series count.
        /// </summary>
        /// <returns></returns>
        internal int GetDoughnutSeriesCount()
        {
            return (from series in Area.VisibleSeries where series is DoughnutSeries select series).ToList().Count;
        }

        /// <summary>
        /// Creates the doughnut segments.
        /// </summary>
        [ClassReference (IsReviewed=false)]
        public override void CreateSegments()
        {
            Segments.Clear();
            if (Stroke == null)
                StrokeThickness = 0d;

            double arcEndAngle, arcStartAngle = DegreeToRadianConverter(StartAngle);
            ClearUnUsedAdornments(this.DataCount);
            int explodedIndex = ExplodeIndex;
            bool explodedAll = ExplodeAll;
            double explodeRadius = ExplodeRadius;
            List<double> xValues = GetXValues();

            if (xValues != null)
            {
                var grandTotal = (from val in YValues
                                  select (val) > 0 ? val : Math.Abs(val)).Sum();
                for (int i = 0; i < DataCount; i++)
                {
                    arcEndAngle =Math.Abs(YValues[i]) * (ARCLENGTH / grandTotal);
                    DoughnutSegment doughnutSegment = new DoughnutSegment(arcStartAngle, arcStartAngle + arcEndAngle, this);
                    doughnutSegment.XData = xValues[i];
                    doughnutSegment.YData = Math.Abs(YValues[i]);
                    doughnutSegment.AngleOfSlice = (2 * arcStartAngle + arcEndAngle) / 2;
                    doughnutSegment.IsExploded = explodedAll || (explodedIndex == i);
                    doughnutSegment.Item = ActualData[i];
                    Segments.Add(doughnutSegment);
                    if (AdornmentsInfo != null)
                        AddDoughnutAdornments(xValues[i], YValues[i], arcStartAngle, arcStartAngle + arcEndAngle, i);
                    arcStartAngle += arcEndAngle;
                }
                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues);
            }
        }

        internal override void UpdateEmptyPointSegments(List<double> xValues)
        {
            var emptyPointBrush = this.EmptyPointInterior;
            if (EmptyPointIndexes != null)
            foreach (var item in EmptyPointIndexes[0])
            {
                DoughnutSegment segment = Segments[item] as DoughnutSegment;
                bool explode = segment.IsExploded;
                Segments[item].IsEmptySegmentInterior = true;
                (Segments[item] as DoughnutSegment).AngleOfSlice = segment.AngleOfSlice;
                (Segments[item] as DoughnutSegment).IsExploded = explode;
            }
        }

        /// <summary>
        /// Method implementation for Create Adornments
        /// </summary>
        /// <param name="series"></param>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        protected override ChartAdornment CreateAdornment(AdornmentSeries series, double xVal, double yVal, double angle, double radius)
        {
            return new ChartPieAdornment(xVal, yVal, angle, radius, series);
        }

        private void AddDoughnutAdornments(double x, double y, double startAngle, double endAngle, int i)
        {
            double angle = (startAngle + endAngle) / 2;
            double radius =((DOUGHNUTSIZE)/2);  

            if (i < Adornments.Count)
            {
                Adornments[i].SetData(x, y, angle, radius);
            }
            else
            {
                Adornments.Add(this.CreateAdornment(this, x, y, angle, radius));
            }
            Adornments[i].Item = ActualData[i];
        }
        /// <summary>
        /// Return IChartTransformer value from the given size
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
        /// Method implementation for ExplodeIndex
        /// </summary>
        /// <param name="i"></param>
        protected override void SetExplodeIndex(int i)
        {
            if (Segments.Count > 0)
            {
                foreach (DoughnutSegment segment in Segments)
                {
                    int index = Segments.IndexOf(segment);
                    if (i == index)
                    {
                        segment.IsExploded = !segment.IsExploded;
                        var tempSegment = segment;
                        UpdateSegments(i, NotifyCollectionChangedAction.Remove);
                    }
                    else if (i == -1)
                    {
                        segment.IsExploded = !segment.IsExploded;
                        var tempSegment = segment;
                        UpdateSegments(i, NotifyCollectionChangedAction.Remove);
                    }

                }

            }
        }

        private Storyboard sb;
        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

        /// <summary>
        /// Virtual Method for Animate
        /// </summary>
        internal override void Animate()
        {
             if (this.Segments.Count > 0)
            {
               
                int i = 0;
                if (sb != null)
                    sb.Stop();
                sb = new Storyboard();
                   
                foreach (DoughnutSegment segment in this.Segments)
                {
                    double segStartAngle = segment.StartAngle;
                    double segEndAngle = segment.EndAngle;
                    
                    DoubleAnimationUsingKeyFrames keyFrames = new DoubleAnimationUsingKeyFrames();
                    SplineDoubleKeyFrame keyFrame = new SplineDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                        Value = 0
                    };
                    keyFrames.KeyFrames.Add(keyFrame);

                    keyFrame = new SplineDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(AnimationDuration),
                        Value = segStartAngle
                    };
                    var keySpline = new KeySpline();
                    keySpline.ControlPoint1 = new Point(0.64, 0.84);
                    keySpline.ControlPoint2 = new Point(0.67, 0.95);
                    keyFrame.KeySpline = keySpline;

                    keyFrames.KeyFrames.Add(keyFrame);
#if !WINDOWS_PHONE
                    Storyboard.SetTargetProperty(keyFrames, "DoughnutSegment.ActualStartAngle");
                    keyFrames.EnableDependentAnimation = true;
#else
                    Storyboard.SetTargetProperty(keyFrames, new PropertyPath(DoughnutSegment.ActualStartAngleProperty));
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
                        Value = segEndAngle
                    };
                    keySpline = new KeySpline
                    {
                        ControlPoint1 = new Point(0.64, 0.84),
                        ControlPoint2 = new Point(0.67, 0.95)
                    };
                    keyFrame.KeySpline = keySpline;

                    keyFrames.KeyFrames.Add(keyFrame);
#if !WINDOWS_PHONE
                    Storyboard.SetTargetProperty(keyFrames, "DoughnutSegment.ActualEndAngle");
                    keyFrames.EnableDependentAnimation = true;
#else
                    Storyboard.SetTargetProperty(keyFrames, new PropertyPath(DoughnutSegment.ActualEndAngleProperty));
#endif
                    Storyboard.SetTarget(keyFrames, segment);
                    sb.Children.Add(keyFrames);                   

                    if (AdornmentsInfo != null && AdornmentsInfo.ShowLabel)
                    {
                        Point adornPoint = new Point(Adornments[i].X, Adornments[i].Y);
                        UIElement label = this.AdornmentsInfo.LabelPresenters[i];
                        label.Opacity = 0;
                        keyFrames = new DoubleAnimationUsingKeyFrames();
                        keyFrame = new SplineDoubleKeyFrame();
                        keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
                        keyFrame.Value = 0;
                        keyFrames.KeyFrames.Add(keyFrame);
                        keyFrame = new SplineDoubleKeyFrame();

                        keyFrame.Value = 1;
                        keyFrames.KeyFrames.Add(keyFrame);
#if !WINDOWS_PHONE
                        Storyboard.SetTargetProperty(keyFrames, "ContentControl.Opacity");
#else
                                            Storyboard.SetTargetProperty(keyFrames, new PropertyPath(UIElement.OpacityProperty));
#endif
                        Storyboard.SetTarget(keyFrames, label);
                        sb.Children.Add(keyFrames);

                        if (AdornmentsInfo.ShowConnectorLine || LabelPosition != CircularSeriesLabelPosition.Inside)
                        {
                            keyFrames.BeginTime = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 20) / 100);
                            keyFrame.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);

                        }
                        else
                        {
                            keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 40) / 100));
                            keyFrames.BeginTime = TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100);

                            label.RenderTransform = new TranslateTransform() { };
                            keyFrames = new DoubleAnimationUsingKeyFrames();
                            keyFrame = new SplineDoubleKeyFrame();
                            keyFrame.KeyTime =
                                KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100));
                            keyFrame.Value = ((segment.startPoint.X - adornPoint.X) * 20) / 100;
                            keyFrames.KeyFrames.Add(keyFrame);
                            keyFrame = new SplineDoubleKeyFrame();
                            keyFrame.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);

                            keySpline = new KeySpline();
                            keySpline.ControlPoint1 = new Point(0.64, 0.84);
                            keySpline.ControlPoint2 = new Point(0.67, 0.95);
                            keyFrame.KeySpline = keySpline;
                            keyFrames.KeyFrames.Add(keyFrame);
                            keyFrame.Value = 0;
#if !WINDOWS_PHONE
                            keyFrames.EnableDependentAnimation = true;
                            Storyboard.SetTargetProperty(keyFrames, "(UIElement.RenderTransform).(TranslateTransform.X)");
#else
                                                Storyboard.SetTargetProperty(keyFrames,
                                                                             new PropertyPath(
                                                                                 "(UIElement.RenderTransform).(TranslateTransform.X)"));
#endif
                            Storyboard.SetTarget(keyFrames, label);
                            sb.Children.Add(keyFrames);
                            keyFrames = new DoubleAnimationUsingKeyFrames();
                            keyFrame = new SplineDoubleKeyFrame();
                            keyFrame.KeyTime =
                                KeyTime.FromTimeSpan(TimeSpan.FromSeconds((AnimationDuration.TotalSeconds * 80) / 100));
                            keyFrame.Value = ((segment.startPoint.Y - adornPoint.Y) * 20) / 100;
                            keyFrames.KeyFrames.Add(keyFrame);
                            keyFrame = new SplineDoubleKeyFrame();
                            keyFrame.KeyTime = KeyTime.FromTimeSpan(AnimationDuration);
                            keySpline = new KeySpline();
                            keySpline.ControlPoint1 = new Point(0.64, 0.84);
                            keySpline.ControlPoint2 = new Point(0.67, 0.95);
                            keyFrame.KeySpline = keySpline;
                            keyFrames.KeyFrames.Add(keyFrame);
                            keyFrame.Value = 0;
#if !WINDOWS_PHONE
                            keyFrames.EnableDependentAnimation = true;
                            Storyboard.SetTargetProperty(keyFrames, "(UIElement.RenderTransform).(TranslateTransform.Y)");
#else
                                                Storyboard.SetTargetProperty(keyFrames,
                                                                             new PropertyPath(
                                                                                 "(UIElement.RenderTransform).(TranslateTransform.Y)"));
#endif
                            Storyboard.SetTarget(keyFrames, label);
                            sb.Children.Add(keyFrames);
                        }
                    }
                    i++;
                   
                }
                sb.Begin();
            }
           
        }

        /// <summary>
        /// Virtual Method for ExplodeRadius
        /// </summary>
        protected override void SetExplodeRadius()
        {
            if (Segments.Count > 0)
            {
                foreach (DoughnutSegment segment in Segments)
                {
                    int index = Segments.IndexOf(segment);
                    UpdateSegments(index, NotifyCollectionChangedAction.Replace);
                }
            }
        }

        /// <summary>
        /// Virtual method for ExplodeAll
        /// </summary>
        protected override void SetExplodeAll()
        {
            if (Segments.Count > 0)
            {
                foreach (DoughnutSegment segment in Segments)
                {
                    int index = Segments.IndexOf(segment);
                    segment.IsExploded = true;
                    UpdateSegments(index, NotifyCollectionChangedAction.Replace);
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new DoughnutSeries() { DoughnutCoefficient = this.DoughnutCoefficient });
        }

        #endregion
    }
}
