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
using Windows.Foundation;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// RadarSeries displays data points as a circular line or area.    
    /// </summary>
    /// <remarks>
    /// Unlike the <see cref="PolarSeries"/>, RadarSeries does not display data in terms of polar coordinates.
    /// RadarSeries is useful for comparisons between multiple series of category data.
    /// </remarks>
    /// <seealso cref="PolarSeries"/>
    [ClassReference(IsReviewed = false)]
    public class RadarSeries : PolarRadarSeriesBase
    {
        #region ctor

      

        #endregion

        #region methods

        /// <summary>
        /// Return IChartTranform value based upon the given size
        /// </summary>
        /// <param name="size"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        protected internal override IChartTransformer CreateTransformer(Size size, bool create)
        {
            if (create || ChartTransformer == null)
            {
                ChartTransformer = ChartTransform.CreatePolar(new Rect(new Point(), size), this);
            }

            return ChartTransformer;
        }

        /// <summary>
        /// Creates the Segments of RadarSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            Segments.Clear(); Segment=null;
            if (DrawType == ChartSeriesDrawType.Area)
            {
                double Origin = this.ActualXAxis!=null ?this.ActualXAxis.Origin:0;
                List<double> xValues = GetXValues().ToList();
                List<double> tempYValues = new List<double>();
                tempYValues = (from val in YValues select val).ToList();

                if (xValues != null)
                {
                    if (!IsClosed)
                    {
                        xValues.Insert((int)DataCount - 1, xValues[(int)DataCount - 1]);
                        xValues.Insert(0, xValues[0]);
                        tempYValues.Insert(0, Origin);
                        tempYValues.Insert(tempYValues.Count, Origin);
                    }
                    else
                    {
                        xValues.Insert(0, xValues[0]);
                        tempYValues.Insert(0, YValues[0]);
                        xValues.Insert(0, xValues[(int)DataCount]);
                        tempYValues.Insert(0, YValues[(int)DataCount - 1]);
                    }

                    if (Segment == null)
                    {
                        Segment = new AreaSegment(xValues, tempYValues, this, null);
                        Segments.Add(Segment);
                    }
                    else
                        Segment.SetData(xValues, tempYValues);
                    if (AdornmentsInfo != null)
                        AddAreaAdornments(YValues);
                }
            }
            else if (DrawType == ChartSeriesDrawType.Line)
            {
                int index = -1;
                int i = 0;
                double xIndexValues = 0d;
                List<double> xValues = ActualXValues as List<double>;

                if (IsIndexed || xValues == null)
                {
                    xValues = xValues != null ? (from val in (xValues) select (xIndexValues++)).ToList()
                          : (from val in (ActualXValues as List<string>) select (xIndexValues++)).ToList();

                }
                if (xValues != null)
                {
                    for (i = 0; i < this.DataCount; i++)
                    {
                        index = i + 1;
                        if (index < this.DataCount)
                        {
                            if (i < Segments.Count)
                            {
                                (Segments[i]).SetData(xValues[i], YValues[i], xValues[index], YValues[index]);
                            }
                            else
                            {
                                LineSegment line = new LineSegment(xValues[i], YValues[i], xValues[index], YValues[index],this, this);
                                Segments.Add(line);
                            }
                        }
                        if (AdornmentsInfo != null)
                        {
                            if (i < Adornments.Count)
                            {
                                Adornments[i].SetData(xValues[i], YValues[i], xValues[i], YValues[i]);
                            }
                            else
                            {
                                Adornments.Add(this.CreateAdornment(this, xValues[i], YValues[i], xValues[i], YValues[i]));
                            }
                            Adornments[i].Item = ActualData[i];
                        }
                    }
                    if (IsClosed)
                    {
                        LineSegment line = new LineSegment(xValues[0], YValues[0], xValues[i - 1], YValues[i - 1],this, this);
                        Segments.Add(line);
                    }
                    if (ShowEmptyPoints)
                        UpdateEmptyPointSegments(xValues);
                   
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new RadarSeries());
        }

        #endregion
    }
}
