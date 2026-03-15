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
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// HiLoOpenCloseSeries is used primarily to analyze price movements of a stock market over a period of time.
    /// </summary>
    /// <remarks>
    /// Each data point contains two values namely high, low. Typically, the high and low values are connected using a vertical straight line.
    /// </remarks>
    /// <seealso cref="HiLoOpenCloseSegment"/>
    /// <seealso cref="HiLoSeries"/>
    /// <seealso cref="CandleSeries"/>
    [ClassReference(IsReviewed = false)]
    public class HiLoOpenCloseSeries : FinancialSeriesBase
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

        #endregion

        #region methods

        /// <summary>
        /// Creates the segments of HiLoOpenCloseSeries
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            List<double> xValues = GetXValues();
            bool isBull = false; double y1, y2 = 0d;
            if (xValues != null)
            {
                if (Segments.Count > this.DataCount)
                {
                    ClearUnUsedSegments(this.DataCount);
                }

                if (AdornmentsInfo != null)
                {
                    if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        ClearUnUsedAdornments(this.DataCount * 4);
                    else
                        ClearUnUsedAdornments(this.DataCount * 2);
                }

                DoubleRange sbsInfo = this.GetSideBySideInfo(this);
                double median = sbsInfo.Delta / 2;
                double center = sbsInfo.Median;
                double Left = sbsInfo.Start;
                double Right = sbsInfo.End;
                for (int i = 0; i < this.DataCount; i++)
                {
                    
                    Point highPt = new Point(xValues[i]+center, HighValues[i]);
                    Point lowPt = new Point(xValues[i] + center, LowValues[i]);
                    Point startOpenPt = new Point(xValues[i] + Left, OpenValues[i]);
                    Point endOpenPt = new Point(xValues[i] + center, OpenValues[i]);
                    Point startClosePt = new Point(xValues[i] + Right, CloseValues[i]);
                    Point endClosePt = new Point(xValues[i] + center, CloseValues[i]);

                    y1 = OpenValues[i];
                    y2 = CloseValues[i];
                    if (y1 < y2)
                    {
                        isBull = true;
                    }
                    else
                    {
                        isBull = false;
                    }

                    if (i < Segments.Count)
                    {
                        (Segments[i]).SetData(highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt,isBull);
                    }
                    else
                    {
                        HiLoOpenCloseSegment hiloOpenClose = new HiLoOpenCloseSegment(highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt,isBull, this, ActualData[i]);
                        hiloOpenClose.High = HighValues[i];
                        hiloOpenClose.Low = LowValues[i];
                        hiloOpenClose.Open = OpenValues[i];
                        hiloOpenClose.Close = CloseValues[i];
                        Segments.Add(hiloOpenClose);
                    }

                    if (AdornmentsInfo != null)
                        AddAdornments(xValues[i],highPt, lowPt, startOpenPt, endOpenPt, startClosePt, endClosePt, i, median);

                }

                if (ShowEmptyPoints)
                    UpdateEmptyPointSegments(xValues);
            }
        }

        private void AddAdornments(double xVal,Point highpt,Point lowpt,Point StartOpenpt,Point endOpenPt,Point StartClosept,Point endClosePt,int i,double Median)
        {
            double adornX1, adornX2, adornX3, adornX4, adornMax, adornMin, adornOpen, adornClose;

            adornMax = highpt.Y;
            adornMin = lowpt.Y;

            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
            {
                adornX1 = highpt.X;

                if (StartOpenpt.Y > StartClosept.Y)
                {
                    adornOpen = StartOpenpt.Y;
                    adornX2 = highpt.X - Median;
                }
                else
                {
                    adornOpen = StartClosept.Y;
                    adornX2 = highpt.X + Median;
                }

                if (i < Adornments.Count / 2)
                {
                    int j = 2 * i;
                    Adornments[j++].SetData(xVal, adornMax,adornX1, adornMax);
                    Adornments[j].SetData(xVal, adornOpen,adornX2, adornOpen);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this,xVal, adornMax, adornX1, adornMax));
                    Adornments.Add(this.CreateAdornment(this, xVal, adornOpen, adornX2, adornOpen));
                }
                int k = 2 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];

            }
            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
            {
                adornX1 = lowpt.X;
               

                if (StartClosept.Y < StartOpenpt.Y)
                {
                    adornClose = endClosePt.Y;
                    adornX2 = lowpt.X + Median;
                }
                else
                {
                    adornClose = StartOpenpt.Y;
                    adornX2 = lowpt.X - Median;                    
                }
                if (i < Adornments.Count / 2)
                {
                    int j = 2 * i;
                    Adornments[j++].SetData(xVal, adornMin, adornX1, adornMin);
                    Adornments[j].SetData(xVal, adornClose,adornX2, adornClose);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this, xVal, adornMin, adornX1, adornMin));
                    Adornments.Add(this.CreateAdornment(this, xVal, adornClose, adornX2, adornClose));
                }
                int k = 2 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
            else
            {
                adornX1 = highpt.X;
                adornX2 = highpt.X + Median;
                adornX3 = lowpt.X;
                adornX4 = lowpt.X - Median;
                adornOpen = StartOpenpt.Y;
                adornClose = endClosePt.Y;
                if (i < Adornments.Count / 4)
                {
                    int j = 4 * i;
                    Adornments[j++].SetData(xVal, adornMax,adornX1, adornMax);
                    Adornments[j++].SetData(xVal, adornOpen, adornX4, adornOpen);
                    Adornments[j++].SetData(xVal, adornMin, adornX3, adornMin);
                    Adornments[j].SetData(xVal, adornClose, adornX2, adornClose);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this,xVal, adornMax, adornX1, adornMax));
                    Adornments.Add(this.CreateAdornment(this, xVal, adornOpen, adornX4, adornOpen));
                    Adornments.Add(this.CreateAdornment(this, xVal, adornMin, adornX3, adornMin));
                    Adornments.Add(this.CreateAdornment(this, xVal, adornClose, adornX2, adornClose));
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
            return base.CloneSeries(new HiLoOpenCloseSeries());
        }

        #endregion
    }
}
