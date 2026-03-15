#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// CandleSeries is used primarily to analyze price movements of a stcock market over a period of time.
    /// </summary>
    /// <remarks>
    /// Each data point contains four values namely open, high, low, close. Typically, the high and low values are connected using a vertical straight line, 
    /// whereas the region between open and close values are connected using a vertical column segment.
    /// </remarks>
    /// <seealso cref="CandleSegment"/>
    /// <seealso cref="HiLoSeries"/>
    /// <seealso cref="HiLoOpenCloseSeries"/>
    [ClassReference(IsReviewed = false)]
    public class CandleSeries:FinancialSeriesBase
    {
        #region properties

        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

        protected internal override bool IsSideBySide
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region constructor

        public CandleSeries()
        {
            DefaultStyleKey = typeof(CandleSeries);
        }

        #endregion

        #region methods

        /// <summary>
        /// Creates the segments of CandleSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            DoubleRange sbsInfo = this.GetSideBySideInfo(this);
            double center = sbsInfo.Median;
            List<double> xValues = GetXValues();
            if (xValues != null)
            {
                ClearUnUsedSegments(this.DataCount);

                if (AdornmentsInfo != null)
                {
                    if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        ClearUnUsedAdornments(this.DataCount * 4);
                    else
                        ClearUnUsedAdornments(this.DataCount * 2);
                }

                for (int i = 0; i < DataCount; i++)
                {
                    double x1 =xValues[i] + sbsInfo.Start;
                    double x2 = xValues[i] + sbsInfo.End;
                    double y1 = OpenValues[i];
                    double y2 = CloseValues[i];
                    bool isbull = false;
                    if (y1 < y2)
                    {
                        isbull = true;
                    }
                    else
                    {
                        isbull = false;
                    }

                    Point cdpBottomLeft = new Point(x1, y1);
                    Point cdpRightTop = new  Point(x2, y2);

                    Point hipoint = new Point(xValues[i] + center,HighValues[i]);
                    Point lopoint = new Point(xValues[i] + center, LowValues[i]);
                 
                    if (i < Segments.Count)
                        {
                            (Segments[i]).SetData(cdpBottomLeft, cdpRightTop, hipoint, lopoint,isbull);
                        }
                        else
                        {
                        var segment = new CandleSegment(cdpBottomLeft, cdpRightTop, hipoint, lopoint, isbull, this,
                                                        ActualData[i]);
                        segment.High = HighValues[i];
                        segment.Low = LowValues[i];
                        segment.Open = OpenValues[i];
                        segment.Close = CloseValues[i];
                        Segments.Add(segment);
                        }
                    if (AdornmentsInfo != null)
                        AddAdornments(xValues[i],hipoint, lopoint, cdpBottomLeft, cdpRightTop,i);
                }

                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues);
            }
        }

        private void AddAdornments(double xVal,Point highpt, Point lowpt, Point Openpt, Point ClosePt, int i)
        {
            double adornX1,adornMax, adornMin, adornOpen, adornClose;

            adornMax = highpt.Y;
            adornMin = lowpt.Y;

            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
            {
                adornX1 = highpt.X;

                if (Openpt.Y > ClosePt.Y)
                {
                    adornOpen = Openpt.Y;
                }
                else
                {
                    adornOpen = ClosePt.Y;
                }

                if (i < Adornments.Count / 2)
                {
                    int j = 2 * i;
                    Adornments[j++].SetData(xVal,adornMax,adornX1, adornMax);
                    Adornments[j].SetData(xVal,adornOpen,adornX1, adornOpen);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this,xVal,adornMax, adornX1, adornMax));
                    Adornments.Add(this.CreateAdornment(this,xVal, adornOpen, adornX1, adornOpen));
                }
                int k = 2 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
            {
                adornX1 = lowpt.X;


                if (ClosePt.Y < Openpt.Y)
                {
                    adornClose = ClosePt.Y;
                }
                else
                {
                    adornClose = Openpt.Y;
                }
                if (i < Adornments.Count / 2)
                {
                    int j = 2 * i;
                    Adornments[j++].SetData(xVal, adornMin, adornX1, adornMin);
                    Adornments[j].SetData(xVal, adornClose,adornX1, adornClose);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this,xVal, adornMin, adornX1, adornMin));
                    Adornments.Add(this.CreateAdornment(this,xVal, adornClose, adornX1, adornClose));
                }
                int k = 2 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
            else
            {
                adornX1 = highpt.X;
                adornOpen = Openpt.Y;
                adornClose = ClosePt.Y;
                if (i < Adornments.Count / 4)
                {
                    int j = 4 * i;
                    Adornments[j++].SetData(xVal,adornMax, adornX1, adornMax);
                    Adornments[j++].SetData(xVal,adornOpen,adornX1, adornOpen);
                    Adornments[j++].SetData(xVal,adornMin,adornX1, adornMin);
                    Adornments[j].SetData(xVal, adornClose, adornX1, adornClose);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this,xVal,adornMax, adornX1, adornMax));
                    Adornments.Add(this.CreateAdornment(this,xVal,adornOpen, adornX1, adornOpen));
                    Adornments.Add(this.CreateAdornment(this, xVal, adornMin, adornX1, adornMin));
                    Adornments.Add(this.CreateAdornment(this, xVal, adornClose, adornX1, adornClose));
                }
                int k = 4 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k++].Item = ActualData[i];
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new CandleSeries());
        }

        #endregion
    }
}
