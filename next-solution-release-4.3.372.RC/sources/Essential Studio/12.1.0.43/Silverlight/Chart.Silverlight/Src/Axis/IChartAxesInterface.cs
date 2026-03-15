#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Interface implementation for IChartAxes
    /// </summary>
    public interface IChartAxes
    {
        /// <summary>
        /// Get or Set X1 property 
        /// </summary>
        double X1 { get; set; }
        /// <summary>
        /// Get or Set Y1 property
        /// </summary>
        double Y1 { get; set; }
        /// <summary>
        /// Method Declaration for GetPoints 
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="m_visibleInterval"></param>
        /// <param name="Orientation"></param>
        /// <param name="OpposedPosition"></param>
        /// <param name="TotalSize"></param>
        /// <param name="anglePoints"></param>
        /// <param name="IsLogarithmic"></param>
        /// <param name="LogarithmicBase"></param>
        /// <param name="firstinterval"></param>
        /// <param name="enableBreaks"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        IEnumerable<ChartAxisPoints> GetPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval, bool enableBreaks, ChartAxis axis);
        /// <summary>
        /// Method declaration for GetPoints
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="m_visibleInterval"></param>
        /// <param name="Orientation"></param>
        /// <param name="OpposedPosition"></param>
        /// <param name="TotalSize"></param>
        /// <param name="anglePoints"></param>
        /// <param name="IsLogarithmic"></param>
        /// <param name="LogarithmicBase"></param>
        /// <param name="firstinterval"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        IEnumerable<ChartAxisPoints> GetPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval,ChartAxis axis);
        /// <summary>
        /// Method declaration for GetSortedPoints
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="m_visibleInterval"></param>
        /// <param name="Orientation"></param>
        /// <param name="OpposedPosition"></param>
        /// <param name="TotalSize"></param>
        /// <param name="anglePoints"></param>
        /// <param name="IsLogarithmic"></param>
        /// <param name="LogarithmicBase"></param>
        /// <param name="firstinterval"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        IEnumerable<ChartAxisPoints> GetSortedPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval, ChartAxis axis);

    }

    /// <summary>
    /// Interface implementation for IChartCartesianAxes
    /// </summary>
    public interface IChartCartesianAxes : IChartAxes
    {
        /// <summary>
        /// Get or Set X2 property
        /// </summary>
        double X2 { get; set; }
        /// <summary>
        /// Get or Set Y2 property
        /// </summary>
        double Y2 { get; set; }
    }

    /// <summary>
    /// Interface implementation for IChartPolaraxes
    /// </summary>
    public interface IChartPolarAxes : IChartAxes
    {
        /// <summary>
        /// Get or Set Radius property
        /// </summary>
        double Radius { get; set; }
        /// <summary>
        /// Get or Set LabelRadius
        /// </summary>
        double LabelRadius { get; set; }
        /// <summary>
        /// Get or Set Angle property
        /// </summary>
        double Angle { get; set; }
        /// <summary>
        /// Get or Set Centerpoint property
        /// </summary>
        Point CenterPoint { get; set; }
    }
    /// <summary>
    /// Interface implementation for IChartRadarAxes 
    /// </summary>
    public interface IChartRadarAxes : IChartAxes
    {
        /// <summary>
        /// Get or Set Radius property
        /// </summary>
        double Radius { get; set; }
        /// <summary>
        /// Get or Set LabelRadius property
        /// </summary>
        double LabelRadius { get; set; }
        /// <summary>
        /// Get or Set Angle property
        /// </summary>
        double Angle { get; set; }
        /// <summary>
        /// Get or Set Centet point
        /// </summary>
        Point CenterPoint { get; set; }
    }

    /// <summary>
    /// Class implementation for ChartAxisPoints
    /// </summary>
    public class ChartAxisPoints
    {
        /// <summary>
        /// Called when instancecreated for ChartAxisPoints
        /// </summary>
        public ChartAxisPoints()
        {
            this.PolyPoints = new PointCollection();
            this.CenterPoint = new Point(0, 0);
        }

        /// <summary>
        /// Get or Set Actualvalue
        /// </summary>
        public double ActualValue
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set X1 property
        /// </summary>
        public double X1
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set X2 property 
        /// </summary>
        public double X2
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Y1 property
        /// </summary>
        public double Y1
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Y2 property
        /// </summary>
        public double Y2
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Orientation property
        /// </summary>
        public Orientation Orientation
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Angle value
        /// </summary>
        public double Angle
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set PolyPoints property
        /// </summary>
        public PointCollection PolyPoints { get; set; }

        /// <summary>
        /// Get or Set LabelXPosition
        /// </summary>
        public double LabelXPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set LabelYPositionProperty
        /// </summary>
        public double LabelYPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Radius property
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Get or Set CenterPoint property
        /// </summary>
        public Point CenterPoint { get; set; }

        /// <summary>
        /// Get or Set MaxRadius property
        /// </summary>
        public double MaxRadius { get; set; }

    }

    #region Cartesian Axis

    /// <summary>
    /// Class implementation for ChartCartessianAxesGenerator
    /// </summary>
    public class ChartCartesianAxesGenerator : IChartCartesianAxes
    {
        /// <summary>
        /// Get or Set X1 property
        /// </summary>
        public double X1
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set X2 property
        /// </summary>
        public double X2
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Y property
        /// </summary>
        public double Y1
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Y2 property
        /// </summary>
        public double Y2
        {
            get;
            set;
        }

        /// <summary>
        /// Return Ienumerable chartPoint collections.
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="m_visibleInterval"></param>
        /// <param name="Orientation"></param>
        /// <param name="OpposedPosition"></param>
        /// <param name="TotalSize"></param>
        /// <param name="anglePoints"></param>
        /// <param name="IsLogarithmic"></param>
        /// <param name="LogarithmicBase"></param>
        /// <param name="firstinterval"></param>
        /// <param name="enableBreaks"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public IEnumerable<ChartAxisPoints> GetPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval, bool enableBreaks, ChartAxis axis)
        {
            double SegmentMinValue = Start + (Start * (-1));
            double SegmentMaxValue = End - Start;
            SegmentMaxValue = (SegmentMaxValue == 0) ? 1 : SegmentMaxValue;
            Dictionary<double, ChartAxisPoints> axesPoints = new Dictionary<double, ChartAxisPoints>();

            double interval = m_visibleInterval;
            if (double.IsNaN(firstinterval) == false)
            {
                interval = firstinterval;
            }

            if (axis.m_enableBreaks && axis.BreaksMode == ScaleBreaksModes.Auto)
            {
                ChartScaleBreak scaleBreak = new ChartScaleBreak();
                if (axis.m_autoScaleBreak != null)
                {
                    scaleBreak = axis.m_autoScaleBreak;
                }
                else if(axis.Area != null && axis.Area.Series != null)
                {
                    scaleBreak = new ChartScaleBreak() { m_axis = axis };
                    scaleBreak.Compute(axis.Area.Series);
                }
                double pos1 = 0;
                for (int i = 0, ci = scaleBreak.m_breakSegments.Count; i < ci; i++)
                {
                    DoubleRange breakSegment = scaleBreak.m_breakSegments[i];
                    pos1 = breakSegment.Start;

                    while (pos1 <= breakSegment.End)
                    {
                        double segmentvalue = pos1 + (Start * (-1));
                        double Value = segmentvalue;
                        double rangeDiff = (SegmentMaxValue - SegmentMinValue);
                        ChartAxisPoints linePoint = new ChartAxisPoints();

                        if (!axesPoints.ContainsKey(axis.ValueToCoefficient(pos1)))
                        {
                            linePoint.X1 = Orientation == Orientation.Horizontal ? (enableBreaks ? Math.Round(TotalSize.Width * axis.ValueToCoefficient(Value)) : Math.Round(TotalSize.Width / (rangeDiff) * Value)) : 0;
                            linePoint.Y1 = Orientation == Orientation.Horizontal ? 0 : (enableBreaks ? Math.Round(TotalSize.Height * (1 - axis.ValueToCoefficient(Value))) : Math.Round((TotalSize.Height * (1 - ((1 / (rangeDiff)) * Value)))));
                            linePoint.X2 = Orientation == Orientation.Horizontal ? linePoint.X1 : TotalSize.Width;
                            linePoint.Y2 = Orientation == Orientation.Horizontal ? TotalSize.Height : linePoint.Y1;
                            linePoint.ActualValue = pos1;
                            axesPoints.Add(axis.ValueToCoefficient(pos1), linePoint);
                            yield return linePoint;
                        }
                        pos1 += m_visibleInterval;
                    }
                }
            }
            else
            {
                for (double i = Start; i <= End; i = ((i + interval) > End && i != End) ? End : i + interval)
                {
                    if (i != Start)
                    {
                        interval = m_visibleInterval;
                    }

                    double segmentvalue = i + (Start * (-1));
                    double Value = segmentvalue;
                    double rangeDiff = (SegmentMaxValue - SegmentMinValue);

                    ChartAxisPoints linePoint = new ChartAxisPoints();
                    linePoint.X1 = Orientation == Orientation.Horizontal ? (enableBreaks ? Math.Round(TotalSize.Width * axis.ValueToCoefficient(Value)) : Math.Round(TotalSize.Width / (rangeDiff) * Value)) : 0;
                    linePoint.Y1 = Orientation == Orientation.Horizontal ? 0 : (enableBreaks ? Math.Round(TotalSize.Height * (1 - axis.ValueToCoefficient(Value))) : Math.Round((TotalSize.Height * (1 - ((1 / (rangeDiff)) * Value)))));
                    linePoint.X2 = Orientation == Orientation.Horizontal ? linePoint.X1 : TotalSize.Width;
                    linePoint.Y2 = Orientation == Orientation.Horizontal ? TotalSize.Height : linePoint.Y1;
                    linePoint.ActualValue = i;

                    yield return linePoint;
                }

            }
        }

		/// <summary>
		/// Return Ienumerable Chart points
		/// </summary>
		/// <param name="Start"></param>
		/// <param name="End"></param>
		/// <param name="m_visibleInterval"></param>
		/// <param name="Orientation"></param>
		/// <param name="OpposedPosition"></param>
		/// <param name="TotalSize"></param>
		/// <param name="anglePoints"></param>
		/// <param name="IsLogarithmic"></param>
		/// <param name="LogarithmicBase"></param>
		/// <param name="firstinterval"></param>
		/// <param name="axis"></param>
		/// <returns></returns>
		public IEnumerable<ChartAxisPoints> GetSortedPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval, ChartAxis axis)
        {
            double SegmentMinValue = Start + (Start * (-1));
            double SegmentMaxValue = End - Start;
            SegmentMaxValue = (SegmentMaxValue == 0) ? 1 : SegmentMaxValue;
            ObservableCollection<ChartAxisPoints> axesPoints = new ObservableCollection<ChartAxisPoints>();

            double interval = m_visibleInterval;
            if (double.IsNaN(firstinterval) == false)
            {
                interval = firstinterval;
            }
            double SStart = SegmentMinValue;
            double SEnd = SegmentMaxValue;
            if (axis.Isindexedseries)
            {
                for (double i = SStart; i <= SEnd; i = ((i + interval) > SEnd && i != SEnd) ? SEnd : i + interval)
                {
                    if (i != SStart)
                    {
                        interval = m_visibleInterval;
                    }

                    double segmentvalue = i + (SStart * (-1));
                    double Value = segmentvalue;
                    double rangeDiff = (SegmentMaxValue - SegmentMinValue);

                    ChartAxisPoints linePoint = new ChartAxisPoints();
                    linePoint.X1 = Orientation == Orientation.Horizontal ? (TotalSize.Width / (rangeDiff) * Value) : 0;
                    linePoint.Y1 = Orientation == Orientation.Horizontal ? 0 : (TotalSize.Height * (1 - ((1 / (rangeDiff)) * Value)));
                    linePoint.X2 = Orientation == Orientation.Horizontal ? linePoint.X1 : TotalSize.Width;
                    linePoint.Y2 = Orientation == Orientation.Horizontal ? TotalSize.Height : linePoint.Y1;
                    linePoint.ActualValue = i;

                    yield return linePoint;
                }
            }
            else
            {
                for (double i = Start; i <= End; i = ((i + interval) > End && i != End) ? End : i + interval)
                {
                    if (i != Start)
                    {
                        interval = m_visibleInterval;
                    }

                    double segmentvalue = i + (Start * (-1));
                    double Value = segmentvalue;
                    double rangeDiff = (SegmentMaxValue - SegmentMinValue);

                    ChartAxisPoints linePoint = new ChartAxisPoints();
                    linePoint.X1 = Orientation == Orientation.Horizontal ? (TotalSize.Width / (rangeDiff) * Value) : 0;
                    linePoint.Y1 = Orientation == Orientation.Horizontal ? 0 : (TotalSize.Height * (1 - ((1 / (rangeDiff)) * Value)));
                    linePoint.X2 = Orientation == Orientation.Horizontal ? linePoint.X1 : TotalSize.Width;
                    linePoint.Y2 = Orientation == Orientation.Horizontal ? TotalSize.Height : linePoint.Y1;
                    linePoint.ActualValue = i;

                    yield return linePoint;
                }
            }
        }

        /// <summary>
        /// Return Ienumerable ChartPoints collections
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="m_visibleInterval"></param>
        /// <param name="Orientation"></param>
        /// <param name="OpposedPosition"></param>
        /// <param name="TotalSize"></param>
        /// <param name="anglePoints"></param>
        /// <param name="IsLogarithmic"></param>
        /// <param name="LogarithmicBase"></param>
        /// <param name="firstinterval"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public IEnumerable<ChartAxisPoints> GetPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval, ChartAxis axis)
        {
            double ratio = End, t = 0;
            do
            {
                ratio = ratio / 10;
                t++;
            } while (ratio > 1);
            double incrementer = 1;
            for (double i = Start; i <= End; i = (i + incrementer))
            {
                if (i /10 == incrementer)
                {
                    incrementer = 10 * (i / 10);
                }
                double segmentvalue = axis.ActualVisibleInterval / axis.SmallTicksPerInterval;
                double Value = Math.Log(i, axis.LogarithmicBase) ; 
                var val =  TotalSize.Width / (axis.SmallTicksPerInterval * axis.DesiredIntervalsCount);
                ChartAxisPoints linePoint = new ChartAxisPoints();
                linePoint.X1 = Orientation == Orientation.Horizontal ? axis.IsInversed ? TotalSize.Width - ((TotalSize.Width / t) * Value) : ((TotalSize.Width / t) * Value) : 0;
                linePoint.Y1 = Orientation == Orientation.Horizontal ? 0 : axis.IsInversed ? (TotalSize.Height/t) * (Value) : (TotalSize.Height - ( TotalSize.Height/t) * (Value));
                linePoint.X2 = Orientation == Orientation.Horizontal ? linePoint.X1 : TotalSize.Width;
                linePoint.Y2 = Orientation == Orientation.Horizontal ? TotalSize.Height : linePoint.Y1;
                linePoint.ActualValue = i;

                yield return linePoint;
            }
        }
    }
    #endregion

    #region RadarAxis
    /// <summary>
    /// Class implementation for ChartradarAxesGenerartor
    /// </summary>
    public class ChartRadarAxesGenerator : IChartRadarAxes
    {
        /// <summary>
        /// Get or Set X1 property
        /// </summary>
        public double X1
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Y1 property
        /// </summary>
        public double Y1
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set X2 property
        /// </summary>
        public double X2
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Y2 property
        /// </summary>
        public double Y2
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Radius property
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// Get or Set LabelRadius property
        /// </summary>
        public double LabelRadius { get; set; }
        /// <summary>
        /// Get or Set Angle property
        /// </summary>
        public double Angle { get; set; }
        /// <summary>
        /// Get or Set CenterPoints property
        /// </summary>
        public Point CenterPoint { get; set; }

        /// <summary>
        /// Get or Set AnglePoints property
        /// </summary>
        public List<double> AnglePoints { get; set; }

        /// <summary>
        /// Return IEnumerable points in the given values
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="m_visibleInterval"></param>
        /// <param name="Orientation"></param>
        /// <param name="OpposedPosition"></param>
        /// <param name="TotalSize"></param>
        /// <param name="anglePoints"></param>
        /// <param name="IsLogarithmic"></param>
        /// <param name="LogarithmicBase"></param>
        /// <param name="firstinterval"></param>
        /// <param name="enableBreaks"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public IEnumerable<ChartAxisPoints> GetPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval, bool enableBreaks, ChartAxis axis)
        {
            this.Radius = (Math.Min(TotalSize.Width, TotalSize.Height) / 2d) * 0.8d;
            this.CenterPoint = new Point(TotalSize.Width / 2d, TotalSize.Height / 2d);
            LabelRadius = (Math.Min(TotalSize.Width, TotalSize.Height) / 2d) * 0.9d;
            this.AnglePoints = anglePoints;

            double SegmentMinValue = Start + (Start * (-1));
            double SegmentMaxValue = End - Start;
            SegmentMaxValue = (SegmentMaxValue == 0) ? 1 : SegmentMaxValue;
            ObservableCollection<ChartAxisPoints> axesPoints = new ObservableCollection<ChartAxisPoints>();
            double sumangle = 0d;
            double isclockwise = OpposedPosition ? 1 : -1;

            for (double i = Start; i <= End; i = ((i + m_visibleInterval) > End && i != End && Orientation == Orientation.Vertical) ? End : i + m_visibleInterval)
            {
                double segmentvalue = i + (Start * (-1));
                double Value = segmentvalue;

                ChartAxisPoints point = new ChartAxisPoints();
                point.CenterPoint = this.CenterPoint;
                point.MaxRadius = this.Radius;
                point.Orientation = Orientation;
                point.ActualValue = i;
                Point origin = new Point(this.CenterPoint.X, this.CenterPoint.Y);
                if (Orientation == Orientation.Horizontal)
                {

                    Point endpoint = GeneralPointRotation(origin, new Point(this.CenterPoint.X, this.CenterPoint.Y - this.Radius), (sumangle * isclockwise));
                    Point labelendpoint = GeneralPointRotation(origin, new Point(this.CenterPoint.X, this.CenterPoint.Y - this.LabelRadius), (sumangle * isclockwise));
                    this.Angle = (360d / (SegmentMaxValue - SegmentMinValue + 1)) * m_visibleInterval;
                    point.Angle = this.Angle;
                    sumangle += this.Angle;

                    point.X1 = endpoint.X;
                    point.Y1 = endpoint.Y;
                    point.X2 = origin.X;
                    point.Y2 = origin.Y;
                    point.LabelXPosition = labelendpoint.X;
                    point.LabelYPosition = labelendpoint.Y;// -(TotalSize.Height / 2);
                }
                else
                {
                    double sum = 0;
                    double y = this.CenterPoint.Y - (this.Radius * (1 - ((1 / (SegmentMaxValue - SegmentMinValue)) * segmentvalue)));
                    double y1 = this.CenterPoint.Y - (this.Radius * (1 - ((1 / (SegmentMaxValue - SegmentMinValue)) * (SegmentMaxValue - segmentvalue))));
                    PointCollection collection = new PointCollection();

                    int j = 0;
                    while (sum < 360 && j < AnglePoints.Count)
                    {
                        
                        double angle = AnglePoints[j++];
                        Point endpoint = GeneralPointRotation(origin, new Point(this.CenterPoint.X, y1), sum);
                        collection.Add(endpoint);
                        sum += angle;
                    }

                    collection.Add(GeneralPointRotation(origin, new Point(this.CenterPoint.X, y1), 0));

                    point.PolyPoints = collection;
                    point.LabelXPosition = this.CenterPoint.X;// -(TotalSize.Width / 2);
                    point.LabelYPosition = this.CenterPoint.Y + (this.CenterPoint.Y - y - this.Radius);
                }

                yield return point;
            }

        }

        Point GeneralPointRotation(Point originpoint, Point endpoint, double angle)
        {
            double ang = angle * Math.PI / 180;
            Point displacement = new Point(endpoint.X - originpoint.X, endpoint.Y - originpoint.Y);
            endpoint.X = (displacement.X * Math.Cos(ang)) - (displacement.Y * Math.Sin(ang));
            endpoint.Y = (displacement.Y * Math.Cos(ang)) + (displacement.X * Math.Sin(ang));
            endpoint.X += originpoint.X;
            endpoint.Y += originpoint.Y;
            return endpoint;
        }




        /// <summary>
        /// Return IEnumerable values form the given inputs.
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="m_visibleInterval"></param>
        /// <param name="Orientation"></param>
        /// <param name="OpposedPosition"></param>
        /// <param name="TotalSize"></param>
        /// <param name="anglePoints"></param>
        /// <param name="IsLogarithmic"></param>
        /// <param name="LogarithmicBase"></param>
        /// <param name="firstinterval"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ChartAxisPoints> GetPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval, ChartAxis axis)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Return IEnumerable points from the given values
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="m_visibleInterval"></param>
        /// <param name="Orientation"></param>
        /// <param name="OpposedPosition"></param>
        /// <param name="TotalSize"></param>
        /// <param name="anglePoints"></param>
        /// <param name="IsLogarithmic"></param>
        /// <param name="LogarithmicBase"></param>
        /// <param name="firstinterval"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ChartAxisPoints> GetSortedPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval, ChartAxis axis)
        {
            throw new NotImplementedException();
        }
    }
    #endregion


    #region PolarAxis
    /// <summary>
    /// Get or Set ChartpolarAxesGenerator property
    /// </summary>
    public class ChartPolarAxesGenerator : IChartPolarAxes
    {
        /// <summary>
        /// Get or Set X1 property 
        /// </summary>
        public double X1
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set X2 property
        /// </summary>
        public double X2
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Y1 property
        /// </summary>
        public double Y1
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Y2 property
        /// </summary>
        public double Y2
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Radius property
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Get or Set LabelRadius
        /// </summary>
        public double LabelRadius { get; set; }

        /// <summary>
        /// Get or Set Angle property
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Get or Set Centerpoint property
        /// </summary>
        public Point CenterPoint { get; set; }

        /// <summary>
        /// Return IEnumerable collection values from the given values
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="m_visibleInterval"></param>
        /// <param name="Orientation"></param>
        /// <param name="OpposedPosition"></param>
        /// <param name="TotalSize"></param>
        /// <param name="anglePoints"></param>
        /// <param name="IsLogarithmic"></param>
        /// <param name="LogarithmicBase"></param>
        /// <param name="firstinterval"></param>
        /// <param name="enableBreaks"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public IEnumerable<ChartAxisPoints> GetPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval, bool enableBreaks, ChartAxis axis)
        {
            this.Radius = (Math.Min(TotalSize.Width, TotalSize.Height) / 2d) * 0.8d;
            this.CenterPoint = new Point(TotalSize.Width / 2d, TotalSize.Height / 2d);
            LabelRadius = (Math.Min(TotalSize.Width, TotalSize.Height) / 2d) * 0.9d;

            double SegmentMinValue = Start + (Start * (-1));
            double SegmentMaxValue = End - Start;
            SegmentMaxValue = (SegmentMaxValue == 0) ? 1 : SegmentMaxValue;
            ObservableCollection<ChartAxisPoints> axesPoints = new ObservableCollection<ChartAxisPoints>();
            double sumangle = 0d;
            double isclockwise = OpposedPosition ? 1 : -1;

            for (double i = Start; i <= End; i = ((i + m_visibleInterval) > End && i != End && Orientation == Orientation.Vertical) ? End : i + m_visibleInterval)
            {
                double segmentvalue = i + (Start * (-1));
                double Value = segmentvalue;

                ChartAxisPoints point = new ChartAxisPoints();
                point.CenterPoint = this.CenterPoint;
                point.MaxRadius = this.Radius;
                point.Orientation = Orientation;
                Point origin = new Point(this.CenterPoint.X, this.CenterPoint.Y);
                point.ActualValue = i;

                if (Orientation == Orientation.Horizontal)
                {

                    Point endpoint = GeneralPointRotation(origin, new Point(this.CenterPoint.X, this.CenterPoint.Y - this.Radius), (sumangle * isclockwise));
                    Point labelendpoint = GeneralPointRotation(origin, new Point(this.CenterPoint.X, this.CenterPoint.Y - this.LabelRadius), (sumangle * isclockwise));
                    this.Angle = (360d / (SegmentMaxValue - SegmentMinValue + 1)) * m_visibleInterval;
                    sumangle += this.Angle;

                    //point.Angle = sumangle;
                    point.Angle = this.Angle;
                    point.X1 = endpoint.X;
                    point.Y1 = endpoint.Y;
                    point.X2 = origin.X;
                    point.Y2 = origin.Y;
                    point.LabelXPosition = labelendpoint.X;
                    point.LabelYPosition = labelendpoint.Y;// -(TotalSize.Height / 2);
                }
                else
                {
                    double y = this.CenterPoint.Y - (this.Radius * (1 - ((1 / (SegmentMaxValue - SegmentMinValue)) * segmentvalue)));
                    double y1 = this.CenterPoint.Y - (this.Radius * (1 - ((1 / (SegmentMaxValue - SegmentMinValue)) * (SegmentMaxValue - segmentvalue))));
                    point.Radius = (this.CenterPoint.Y - y1) * 2;

                    point.LabelXPosition = this.CenterPoint.X;// -(TotalSize.Width / 2);
                    point.LabelYPosition = this.CenterPoint.Y + (this.CenterPoint.Y -y - this.Radius);
                }

                yield return point;
            }

        }

        Point GeneralPointRotation(Point originpoint, Point endpoint, double angle)
        {
            double ang = angle * Math.PI / 180;
            Point displacement = new Point(endpoint.X - originpoint.X, endpoint.Y - originpoint.Y);
            endpoint.X = (displacement.X * Math.Cos(ang)) - (displacement.Y * Math.Sin(ang));
            endpoint.Y = (displacement.Y * Math.Cos(ang)) + (displacement.X * Math.Sin(ang));
            endpoint.X += originpoint.X;
            endpoint.Y += originpoint.Y;
            return endpoint;
        }




        /// <summary>
        /// Return Ienumerable collection values from the given values
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="m_visibleInterval"></param>
        /// <param name="Orientation"></param>
        /// <param name="OpposedPosition"></param>
        /// <param name="TotalSize"></param>
        /// <param name="anglePoints"></param>
        /// <param name="IsLogarithmic"></param>
        /// <param name="LogarithmicBase"></param>
        /// <param name="firstinterval"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ChartAxisPoints> GetPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval, ChartAxis axis)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Return IEnumerable Collection values 
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="m_visibleInterval"></param>
        /// <param name="Orientation"></param>
        /// <param name="OpposedPosition"></param>
        /// <param name="TotalSize"></param>
        /// <param name="anglePoints"></param>
        /// <param name="IsLogarithmic"></param>
        /// <param name="LogarithmicBase"></param>
        /// <param name="firstinterval"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ChartAxisPoints> GetSortedPoints(double Start, double End, double m_visibleInterval, Orientation Orientation, bool OpposedPosition, Size TotalSize, List<double> anglePoints, bool IsLogarithmic, double LogarithmicBase, double firstinterval, ChartAxis axis)
        {
            throw new NotImplementedException();
        }
    }
    #endregion


}
