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
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media.Animation;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Data;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class SfWinLossSparkline : ColumnBase
    {
        #region properties

        /// <summary>
        /// Gets or Sets the brush to paint the interior of the negative segment(s).
        /// </summary>
        public Brush NegativePointBrush
        {
            get { return (Brush)GetValue(NegativePointBrushProperty); }
            set { SetValue(NegativePointBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NegativePointBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NegativePointBrushProperty =
            DependencyProperty.Register("NegativePointBrush", typeof(Brush), typeof(SfWinLossSparkline), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        /// Gets or Sets the brush to paint the interior of the neutral segment(s).
        /// </summary>
        public Brush NeutralBrush
        {
            get { return (Brush)GetValue(NeutralBrushProperty); }
            set { SetValue(NeutralBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NeutralBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NeutralBrushProperty =
            DependencyProperty.Register("NeutralBrush", typeof(Brush), typeof(SfWinLossSparkline), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        #endregion

        #region fields

        Rectangle rectSegment;

        #endregion

        #region ctor

        public SfWinLossSparkline()
        {

        }

        #endregion

        #region methods

        protected override void UpdateMinMaxValues()
        {
            base.UpdateMinMaxValues();
            maxXValue += 0.8;
            deltaX = maxXValue - minXValue;
        }

        protected override void RenderSegments()
        {
            base.RenderSegments();
            double yVal = 0, xVal = 0;
            
            for (int i = 0; i < yValues.Count; i++)
            {
                xVal = xValues[i];
                yVal = yValues[i];
                if (!double.IsNaN(yVal))
                {
                    if (SegmentPresenter.Children.Count > i)
                    {
                        rectSegment = SegmentPresenter.Children[i] as Rectangle;
                    }
                    else
                    {
                        rectSegment = new Rectangle();
                        SegmentPresenter.Children.Add(rectSegment);
                        rectSegment.Tag = new object[] { "Selectable", xVal, yVal };
                    }
                    double X1 = Math.Round(availableWidth * (((xVal) - minXValue) / deltaX));
                    double X2 = Math.Round(availableWidth * (((xVal + 0.8) - minXValue) / deltaX));
                    double Y2 = availableHeight / 2;
                    double Y1 = 0d;
                    if (yVal > 0)
                    {
                        BindFillProperty(rectSegment, "Interior");
                    }
                    else if (yVal < 0)
                    {
                        BindFillProperty(rectSegment, "NegativePointBrush");
                        Y1 = Y2;
                    }
                    else
                    {
                        BindFillProperty(rectSegment, "NeutralBrush");
                        Y1 = Y2;
                        Y2 = Y2 / 10;
                        Y1 = Y1 - Y2 / 2;
                    }
                    rectSegment.SetValue(Canvas.LeftProperty, X1);
                    rectSegment.SetValue(Canvas.TopProperty, Y1);
                    rectSegment.Height = Y2;
                    rectSegment.Width = X2 - X1;
                }
            }
        }

        #endregion
    }
}
