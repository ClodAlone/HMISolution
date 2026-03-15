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
using System.Windows.Controls;
#else
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// FastBarBitmapSeries is another version of BarSeries which uses different technology for rendering line in order to boost performance.
    /// </summary>
    /// <remarks>
    /// It uses WriteableBitmap for rendering; Its advantage is that it will render the series with large quantity of data in a fraction of milliseconds.
    ///</remarks>
    ///<seealso cref="FastLineBitmapSeries"/>
    ///<seealso cref="FastHiLoBitmapSeries"/>
    public class FastBarBitmapSeries : XyDataSeries
    {
        #region fields

        private ChartSegment Segment { get; set; }

        #endregion

        #region Properties

        

        protected internal override bool IsSideBySide
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region ctor

        public FastBarBitmapSeries()
        {
            IsActualTransposed = true;
        }

        #endregion

        #region methods

        internal override void OnTransposeChanged(bool val)
        {
            IsActualTransposed = !val;
        }

        /// <summary>
        /// Creates the segments of FastBarBitmapSeries
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            List<double> xValues = GetXValues();
            IList<double> x1Values, x2Values, y1Values, y2Values;
            x1Values = new List<double>();
            x2Values = new List<double>();
            y1Values = new List<double>();
            y2Values = new List<double>();
            
            if (xValues != null)
            {
                ClearUnUsedAdornments(this.DataCount);
                DoubleRange sbsInfo = this.GetSideBySideInfo(this);
                if (!this.IsIndexed)
                {
                    for (int i = 0; i < this.DataCount; i++)
                    {

                        x1Values.Add(xValues[i] + sbsInfo.Start);
                        x2Values.Add(xValues[i] + sbsInfo.End);
                        y1Values.Add(YValues[i]);
                        y2Values.Add(0);
                    }
                }
                else
                {
                    for (int i = 0; i < this.DataCount; i++)
                    {
                        x1Values.Add(i + sbsInfo.Start);
                        x2Values.Add(i + sbsInfo.End);
                        y1Values.Add(YValues[i]);
                        y2Values.Add(0);
                    }
                }
                if (Segment != null && (!IsActualTransposed && Segment is FastBarBitmapSegment)
                         || (IsActualTransposed && Segment is FastColumnBitmapSegment))
                    Segments.Clear();
                if (Segment == null || Segments.Count == 0)
                {
                    if (IsActualTransposed)
                        Segment = new FastBarBitmapSegment(x1Values, y1Values, x2Values, y2Values, this);
                    else
                       Segment = new FastColumnBitmapSegment(x1Values, y1Values, x2Values, y2Values, this);
                    this.Segments.Add(Segment);
                }
                else if (xValues != null)
                {
                    if(Segment is FastBarBitmapSegment)
                        (Segment as FastBarBitmapSegment).SetData(x1Values, y1Values, x2Values, y2Values);
                    else
                        (Segment as FastColumnBitmapSegment).SetData(x1Values, y1Values, x2Values, y2Values);
                }

                if (AdornmentsInfo != null)
                {
                    for (int i = 0; i < this.DataCount; i++)
                    {
                        if (i < this.DataCount)
                        {
                            if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                AddColumnAdornments(xValues[i], YValues[i], x1Values[i], y1Values[i], i, sbsInfo.Delta / 2);
                            else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                AddColumnAdornments(xValues[i], YValues[i], x1Values[i], y2Values[i], i, sbsInfo.Delta / 2);
                            else
                                AddColumnAdornments(xValues[i], YValues[i], x1Values[i], y1Values[i] + (y2Values[i] - y1Values[i]) / 2, i, sbsInfo.Delta / 2);
                        }
                    }
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastBarBitmapSeries());
        }

        #endregion
    }
}
