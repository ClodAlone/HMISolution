#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
#else
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastCandleBitmapSeries is another version of CandleSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    ///</remarks>
    ///<seealso cref="FastLineBitmapSeries"/>
    ///<seealso cref="FastHiLoBitmapSeries"/>
    public class FastCandleBitmapSeries : FinancialSeriesBase
    {
        #region Properties

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

        #region ctor

        public FastCandleBitmapSeries()
        {

        }

        #endregion

        #region methods

        /// <summary>
        /// Creates the segments of FastCandleBitmapSeries
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
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

                    double center = this.GetSideBySideInfo(this).Median;
                    for (int i = 0; i < this.DataCount; i++)
                    {
                        xValues[i] += center;
                        AddAdornments(xValues[i], HighValues[i], LowValues[i], OpenValues[i], CloseValues[i], i);
                    }
                }
                if (Segment == null || Segments.Count == 0)
                {
                    Segment = new FastCandleBitmapSegment(xValues, OpenValues, CloseValues, HighValues, LowValues, this);
                    this.Segments.Add(Segment);
                }
                else if (xValues != null)
                {
                    (Segment as FastCandleBitmapSegment).SetData(xValues, OpenValues, CloseValues, HighValues, LowValues);
                }
            }
        }

        private void AddAdornments(double x, double high, double low, double open, double close, int i)
        {
            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
            {
                if (i < Adornments.Count / 2)
                {
                    int j = 2 * i;
                    Adornments[j++].SetData(x, high, x, high);
                    Adornments[j].SetData(x, open, x, open);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this, x, high, x, high));
                    Adornments.Add(this.CreateAdornment(this, x, open, x, open));
                }
                int k = 2 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
            {
                if (i < Adornments.Count / 2)
                {
                    int j = 2 * i;
                    Adornments[j++].SetData(x, low, x, low);
                    Adornments[j].SetData(x, close, x, close);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this, x, low, x, low));
                    Adornments.Add(this.CreateAdornment(this, x, close, x, close));
                }
                int k = 2 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
            else
            {
                if (i < Adornments.Count / 4)
                {
                    int j = 4 * i;
                    Adornments[j++].SetData(x, high, x, high);
                    Adornments[j++].SetData(x, low, x, low);
                    Adornments[j++].SetData(x, open, x, open);
                    Adornments[j].SetData(x, open, x, close);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this, x, high, x, high));
                    Adornments.Add(this.CreateAdornment(this, x, low, x, low));
                    Adornments.Add(this.CreateAdornment(this, x, open, x, open));
                    Adornments.Add(this.CreateAdornment(this, x, close, x, close));
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
           return base.CloneSeries(new FastCandleBitmapSeries());
        }

        #endregion
    }
}
