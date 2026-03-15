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
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastHiLoSeries is another version of HiLoSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    ///</remarks>
    ///<seealso cref="FastLineBitmapSeries"/>
    ///<seealso cref="FastHiLoOpenCloseBitmapSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FastHiLoBitmapSeries : RangeSeriesBase
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

        #region ctor

        #endregion

        #region methods

        /// <summary>
        /// Creates the segments of FastHiLoSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            List<double> xValues = GetXValues();
            
            if (xValues != null)
            {
                if (Segment == null || Segments.Count == 0)
                {
                    FastHiLoSegment segment = new FastHiLoSegment(xValues as IList<double>, HighValues, LowValues, this);
                    Segment = segment;
                    this.Segments.Add(segment);
                }
                else if (xValues != null)
                {
                    (Segment as FastHiLoSegment).SetData(xValues as IList<double>, HighValues, LowValues);
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
                    xValues[i] += center; 
                    if (AdornmentsInfo != null)
                        AddAdornments(xValues[i], HighValues[i], LowValues[i], i);
                }
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
                    Adornments[i].SetData(adornX, adornHigh, adornX, adornHigh);
                }
                else
                {
                    Adornments.Add(this.CreateAdornment(this, adornX, adornHigh, adornX, adornHigh));
                }
                Adornments[i].Item = ActualData[i];
            }
            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
            {
                adornX = x;
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
                    Adornments.Add(this.CreateAdornment(this, adornX, adornHigh, adornX, adornHigh));
                    Adornments.Add(this.CreateAdornment(this, adornX, adornLow, adornX, adornLow));
                }
                int k = 2 * i;
                Adornments[k++].Item = ActualData[i];
                Adornments[k].Item = ActualData[i];
            }
        }


        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastHiLoBitmapSeries());
        }

        #endregion
    }
}
