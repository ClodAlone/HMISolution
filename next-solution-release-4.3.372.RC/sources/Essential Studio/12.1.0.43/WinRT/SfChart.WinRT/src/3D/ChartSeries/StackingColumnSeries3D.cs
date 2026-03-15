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
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// StackingColumnSeries3D is typically preferred in cases of multiple series of type <see cref="ColumnSeries3D" />.
    /// Each series is then stacked vertically one above the other.
    /// If there exists only single series, it will resemble like a simple <see cref="ColumnSeries3D" /> chart.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class StackingColumnSeries3D : StackingSeriesBase3D
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether [is stacked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is stacked]; otherwise, <c>false</c>.
        /// </value>
        protected override bool IsStacked
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [is side by side].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is side by side]; otherwise, <c>false</c>.
        /// </value>
        protected internal override bool IsSideBySide
        {
            get { return true; }
        }

        #endregion

        #region methods

        /// <summary>
        /// creates the segments of StackingColumnSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            var sbsInfo = GetSideBySideInfo(this);
            var origin = ActualXAxis.Origin;
            var median = sbsInfo.Delta / 2;
            var xValues = GetXValues();
            var stackingValues = GetCumulativeStackValues(this);
            if (stackingValues == null) return;
            var space = Area.Depth / 4;
            var start = space;
            var end = space * 3;
            YRangeStartValues = stackingValues.StartValues;
            YRangeEndValues = stackingValues.EndValues;

            if (YRangeStartValues == null)
            {
                YRangeStartValues = (from val in xValues select origin).ToList();
            }

            if (xValues == null) return;
            ClearUnUsedSegments(DataCount);
            ClearUnUsedAdornments(DataCount);

            for (var i = 0; i < DataCount; i++)
            {
                var x1 = xValues[i] + sbsInfo.Start;
                var x2 = xValues[i] + sbsInfo.End;
                var y2 = YRangeStartValues[i];
                var y1 = YRangeEndValues[i];

                if (i < Segments.Count)
                {
                    (Segments[i]).SetData(x1, y1, x2, y2, start, end);
                }
                else
                {
                    Segments.Add(new StackingColumnSegment3D(x1, y1, x2, y2, start, end, this)
                    {
                        XData = xValues[i],
                        YData = ActualSeriesYValues[0][i],
                        Item = ActualData[i]
                    });
                }
                if (AdornmentsInfo == null) continue;
                switch (AdornmentsInfo.AdornmentsPosition)
                {
                    case AdornmentsPosition.Top:
                        AddColumnAdornments(xValues[i], YValues[i], x1, y1, i, median, start - 2);
                        break;
                    case AdornmentsPosition.Bottom:
                        AddColumnAdornments(xValues[i], YValues[i], x1, y2, i, median, start);
                        break;
                    default:
                        AddColumnAdornments(xValues[i], YValues[i], x1, y1 + (y2 - y1) / 2, i, median, start);
                        break;
                }
            }
            if (ShowEmptyPoints)
                UpdateEmptyPointSegments(xValues);
        }

#if NETFX_CORE
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public StackingColumnSegment3D Segment { get; set; }
#endif

        internal override bool GetAnimationIsActive()
        {
            return sb != null && sb.GetCurrentState() == ClockState.Active;
        }

        private Storyboard sb;
        internal override void Animate()
        {
            canAnimate = false;
            if (Segments.Count <= 0) return;
            if (sb != null)
                sb.Stop();
            sb = new Storyboard();

            var stackingSeries = Area.VisibleSeries.OfType<StackingSeriesBase3D>().ToList();
            int index = stackingSeries.IndexOf(this);
            int count = 0;
            var stackedValues = Area.StackedValues;
            double prevValue = 0;
            var durationInSeconds  = AnimationDuration.TotalSeconds;///stackingSeries.Count;
            foreach (StackingColumnSegment3D segment in Segments)
            {
                
                if (index > 0)
                {
                    prevValue = stackedValues[stackingSeries[index - 1]].EndValues[count];
                    //beginTime = durationInSeconds / (double)index;
                }
                //segment.Top = prevValue;
                var segmentTop = segment.InternalTop;
                var segmentBottom = segment.InternalBottom;

                var keyFrames = new DoubleAnimationUsingKeyFrames
                {
                    //BeginTime = TimeSpan.FromSeconds(beginTime)
                };
                var keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = 0d
                };
                keyFrames.KeyFrames.Add(keyFrame);

                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(durationInSeconds)),
                    Value = segmentTop
                };
                var keySpline = new KeySpline
                {
                    ControlPoint1 = new Point(0.64, 0.84),
                    ControlPoint2 = new Point(0.67, 0.95)
                };
                keyFrame.KeySpline = keySpline;

                keyFrames.KeyFrames.Add(keyFrame);
#if !WINDOWS_PHONE
                Storyboard.SetTargetProperty(keyFrames, "ColumnSegment3D.Top");
                keyFrames.EnableDependentAnimation = true;
#else
                Storyboard.SetTargetProperty(keyFrames, new PropertyPath(ColumnSegment3D.TopProperty));
#endif
                Storyboard.SetTarget(keyFrames, segment);

                sb.Children.Add(keyFrames);

                keyFrames = new DoubleAnimationUsingKeyFrames
                {
                    //BeginTime = TimeSpan.FromSeconds(beginTime)
                };
                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = 0d
                };
                keyFrames.KeyFrames.Add(keyFrame);

                keyFrame = new SplineDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(durationInSeconds)),
                    Value = segmentBottom
                };
                keySpline = new KeySpline
                {
                    ControlPoint1 = new Point(0.64, 0.84),
                    ControlPoint2 = new Point(0.67, 0.95)
                };
                keyFrame.KeySpline = keySpline;

                keyFrames.KeyFrames.Add(keyFrame);
#if !WINDOWS_PHONE
                Storyboard.SetTargetProperty(keyFrames, "ColumnSegment3D.Bottom");
                keyFrames.EnableDependentAnimation = true;
#else
                Storyboard.SetTargetProperty(keyFrames, new PropertyPath(ColumnSegment3D.BottomProperty));
#endif
                Storyboard.SetTarget(keyFrames, segment);


                sb.Children.Add(keyFrames);
                count++;
            }
            sb.Begin();
        }

#if WPF

        void sb_Completed(object sender, EventArgs e)
        {
            Storyboard storyBoard = (Storyboard)(sender as ClockGroup).Timeline;
            foreach (Timeline timeline in storyBoard.Children)
            {
                FrameworkElement element = Storyboard.GetTarget(timeline) as FrameworkElement;
                element.BeginAnimation(Canvas.TopProperty, null);
                element.BeginAnimation(Canvas.LeftProperty, null);
            }

            //storyBoard.Completed -= sb_Completed;
        }

#endif
        #endregion
    }
}
