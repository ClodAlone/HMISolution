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
#else
using Windows.UI.Xaml;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// HiLoSeries is used primarily to analyze price movements of a stock market over a period of time.
    /// </summary>
    /// <seealso cref="HiLoOpenCloseSeries"/>
    /// <seealso cref="HiLoSegment"/>
    /// <seealso cref="CandleSeries"/>
    [ClassReference(IsReviewed = false)]
    public class HiLoSeries : RangeSeriesBase
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
        /// Creates the segments of HiLoSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            List<double> xValues = GetXValues();
            
            if (xValues != null)
            {
                if (Segments.Count > this.DataCount)
                {
                    ClearUnUsedSegments(this.DataCount);
                }

                if (AdornmentsInfo != null)
                {
                    if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        ClearUnUsedAdornments(this.DataCount * 2);
                    else
                        ClearUnUsedAdornments(this.DataCount);
                }

                double center = this.GetSideBySideInfo(this).Median;
                for (int i = 0; i < this.DataCount; i++)
                {
                    if (i < Segments.Count)
                    {
                         xValues[i] += center;
                        (Segments[i]).SetData(xValues[i], HighValues[i], LowValues[i]);
                        (Segments[i]).Item = ActualData[i];
                    }
                    else
                    {
                        xValues[i] += center;
                        HiLoSegment line = new HiLoSegment(xValues[i], HighValues[i], LowValues[i], this, ActualData[i]);
                        line.High = HighValues[i];
                        line.Low = LowValues[i];
                        line.XValue = xValues[i];
                        Segments.Add(line);
                    }
                    if (AdornmentsInfo != null)
                        AddAdornments(xValues[i], HighValues[i], LowValues[i], i);
                }

                if(ShowEmptyPoints)
                  UpdateEmptyPointSegments(xValues);
                
            }
        }

        private void AddAdornments(double x, double high, double low, int i)
        {
            double adornX = 0d, adornHigh = 0d, adornLow = 0d;
            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
            {
                adornX = x;
                adornHigh = high;
                if (i < Adornments.Count)
                {
                    Adornments[i].SetData(adornX, adornHigh,adornX, adornHigh);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this,adornX, adornHigh, adornX, adornHigh));
                }
                Adornments[i].Item = ActualData[i];
            }
            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
            {
                adornX = x ;
                adornLow = low;
                if (i < Adornments.Count)
                {
                    Adornments[i].SetData(adornX, adornLow, adornX, adornLow);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this, adornX, adornLow, adornX, adornLow));
                }
                Adornments[i].Item = ActualData[i];
            }
            else
            {
                adornX = x;
                adornHigh = high;
                adornLow = low;
                if (i < Adornments.Count / 2)
                {
                    int j = 2 * i;
                    Adornments[j++].SetData(adornX, adornHigh, adornX, adornHigh);
                    Adornments[j].SetData(adornX, adornLow, adornX, adornLow);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this,adornX, adornHigh,adornX, adornHigh));
                    Adornments.Add(this.CreateAdornment(this,adornX, adornLow, adornX, adornLow));
                }
                int k = 2 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new HiLoSeries());
        }

        #endregion

    }
}
