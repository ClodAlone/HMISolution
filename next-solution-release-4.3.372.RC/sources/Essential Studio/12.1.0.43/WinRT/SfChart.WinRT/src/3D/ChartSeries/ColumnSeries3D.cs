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
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ColumnSeries displays its data points using a set of vertical bars.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ColumnSeries3D : XyDataSeries3D
    {
        #region Properties

        internal override void OnTransposeChanged(bool val)
        {
            IsActualTransposed = val;
        }

        /// <summary>
        /// Gets a value indicating whether [is side by side].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is side by side]; otherwise, <c>false</c>.
        /// </value>
        protected internal override bool IsSideBySide
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Creates the segments of ColumnSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            var xValues = GetXValues();
            double median;
            if (xValues == null) return;
            ClearUnUsedSegments(DataCount);
            ClearUnUsedAdornments(DataCount);
            var depthInfo = GetSegmentDepth();
            var sbsInfo = GetSideBySideInfo(this);
            median = sbsInfo.Delta / 2;
            for (var i = 0; i < DataCount; i++)
            {
                if (i >= DataCount) continue;
                var x1 = xValues[i] + sbsInfo.Start;
                var x2 = xValues[i] + sbsInfo.End;
                var y1 = YValues[i];
                const double y2 = 0;
                if (i < Segments.Count)
                {
                    (Segments[i]).SetData(x1, y1, x2, y2, depthInfo.Start, depthInfo.End);
                    ((ColumnSegment3D) Segments[i]).Plans = null;
                }
                else
                {
                    Segments.Add(new ColumnSegment3D(x1, y1, x2, y2, depthInfo.Start, depthInfo.End, this)
                    {
                        XData = xValues[i],
                        YData = YValues[i],
                        Item = ActualData[i]
                    });
                }

                if (AdornmentsInfo == null) continue;
                switch (AdornmentsInfo.AdornmentsPosition)
                {
                    case AdornmentsPosition.Top:
                        AddColumnAdornments(xValues[i], YValues[i], x1, y1, i, median, depthInfo.Start + (depthInfo.Delta/2) );
                        break;
                    case AdornmentsPosition.Bottom:
                        AddColumnAdornments(xValues[i], YValues[i], x1, y2, i, median, depthInfo.Start + (depthInfo.End - depthInfo.Start) / 2);
                        break;
                    default:
                        AddColumnAdornments(xValues[i], YValues[i], x1, y1 + (y2 - y1) / 2, i, median, depthInfo.Start);
                        break;
                }

                Adornments[i].Item = ActualData[i];
            }

            if (ShowEmptyPoints)
                UpdateEmptyPointSegments(xValues);
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

        internal override void Animate()
        {
            if (Segments.Count <= 0) return;
            if (sb != null)
                sb.Stop();
            sb = new Storyboard();
            foreach (ColumnSegment3D segment in Segments)
            {
                var segmentTop = segment.Top;

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
            }
            sb.Begin();
        }

#if NETFX_CORE
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public ColumnSegment3D Segment { get; set; }
#endif

        #endregion
    }
}
