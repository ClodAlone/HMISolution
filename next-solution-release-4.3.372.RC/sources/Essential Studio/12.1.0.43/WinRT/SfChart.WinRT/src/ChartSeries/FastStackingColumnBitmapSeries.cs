#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.UI.Xaml;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    public class FastStackingColumnBitmapSeries : StackingSeriesBase
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

        protected override bool IsStacked
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
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            Segment = null;
            base.OnDataSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Creates the segments of FastStackingColumnBitmapSeries
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            List<double> xValues =(ActualXValues is List<double>)? ActualXValues as List<double>: GetXValues();
            IList<double> x1Values, x2Values, y1Values, y2Values;
            x1Values = new List<double>();
            x2Values = new List<double>();
            y1Values = new List<double>();
            y2Values = new List<double>();
            var stackingValues = GetCumulativeStackValues(this);
            if (stackingValues != null)
            {
                YRangeStartValues = stackingValues.StartValues;
                YRangeEndValues = stackingValues.EndValues;
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
                            y1Values.Add(YRangeEndValues[i]);
                            y2Values.Add(YRangeStartValues[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < this.DataCount; i++)
                        {
                            x1Values.Add(i + sbsInfo.Start);
                            x2Values.Add(i + sbsInfo.End);
                            y1Values.Add(YRangeEndValues[i]);
                            y2Values.Add(YRangeStartValues[i]);
                        }
                    }
                    if (Segment != null && (IsActualTransposed && Segment is FastStackingColumnSegment)
                           || (!IsActualTransposed && Segment is FastBarBitmapSegment))
                        Segments.Clear();
                    if (Segment == null || Segments.Count == 0)
                    {
                        if (this.IsActualTransposed)
                            Segment = new FastBarBitmapSegment (x1Values, y1Values, x2Values, y2Values, this);
                        else
                            Segment = new FastStackingColumnSegment(x1Values, y1Values, x2Values, y2Values, this);
                        this.Segments.Add(Segment);
                    }
                    else if (xValues != null)
                    {
                        if (Segment is FastBarBitmapSegment)
                            (Segment as FastBarBitmapSegment).SetData(x1Values, y1Values, x2Values, y2Values);
                        else
                            (Segment as FastStackingColumnSegment).SetData(x1Values, y1Values, x2Values, y2Values);
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
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new FastStackingColumnBitmapSeries());
        }

        #endregion
    }
}
