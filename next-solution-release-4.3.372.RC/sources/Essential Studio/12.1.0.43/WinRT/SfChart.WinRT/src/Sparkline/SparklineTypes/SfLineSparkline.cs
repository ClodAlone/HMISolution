#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class SfLineSparkline : MarkerBase
    {
        #region members

        Polyline segmentLine;

        #endregion

        #region ctor

        public SfLineSparkline()
        {

        }

        #endregion

        #region methods

        protected override void RenderSegments()
        {
            if (yValues != null)
            {
                base.RenderSegments();
                double xTolerance = Math.Abs((deltaX * 5) / availableWidth);
                double yTolerance = Math.Abs((deltaY * 5) / availableHeight);
                double prevXValue = 1, prevYValue = 0, yVal = 0, xVal = 0;
                Point pointToScreen;
                int index = 0;
                int segCount = EmptyPointValue == EmptyPointValues.None ? EmptyPointIndexes.Count : 1;
                for (int j = 0; j < segCount; j++)
                {
                    if (SegmentPresenter.Children.Count > j)
                    {
                        segmentLine = SegmentPresenter.Children[j] as Polyline;
                        segmentLine.Points.Clear();
                    }
                    else
                    {
                        segmentLine = new Polyline();
                        SetBinding(segmentLine);
                        SegmentPresenter.Children.Add(segmentLine);
                    }
                    segmentLine.Clip = null;
                    for (double i = EmptyPointIndexes[j]; i < yValues.Count; i++)
                    {
                        if (yValues.Count > index)
                        {
                            yVal = yValues[index];
                            xVal = xValues[index];

                            if (!double.IsNaN(yVal))
                            {
                                if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                                {
                                    pointToScreen = TransformToVisible(xVal, yVal);
                                    segmentLine.Points.Add(pointToScreen);
                                    prevXValue = xVal;
                                    prevYValue = yVal;
                                    if (MarkerVisibility == Visibility.Visible)
                                    {
                                        AddMarker(pointToScreen, xVal, yVal, index);
                                    }
                                }
                            }
                            else
                            {
                                index++;
                                break;
                            }
                        }
                        index++;
                    }
                }
            }
        }

        #endregion
    }
}
