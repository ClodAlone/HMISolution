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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{

    /// <summary>
    /// Class implementation for ChartFastHiLoOpenCloseType
    /// </summary>
   #if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
  #endif
  public  class ChartFastHiLoOpenCloseType : ChartType
    {
        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.SideBySide | ChartTypeFlags.Indexed;
            }
        }

        #region Properties

        /// <summary>
        /// Gets the requirement for data count.
        /// </summary>
        /// <value>The require data count.</value>
        public override int RequiresDataCount
        {
            get
            {
                return 4;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Converts ChartHiLoOpenCloseType to string 
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            return "FastHiLoOpenClose";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);

            double center = sbsInfo.Median;
            double left = sbsInfo.Start;
            double right = sbsInfo.End;

            series.Segments.Add(new ChartFastHiLoOpenCloseSegment(points, series));
            if (series.AdornmentsInfo.Visible == true)
            {
                series.Adornments.Clear();
                int index = -1;
                for (int i = 0; i < points.Length; i++)
                {
                    double x = points[i].DataPoint.X;
                    double y1 = points[i].DataPoint.Values[0];
                    double y2 = points[i].DataPoint.Values[1];
                    double y3 = points[i].DataPoint.Values[2];
                    double y4 = points[i].DataPoint.Values[3];

                    series.Adornments.Add(this.CreateAdornment(series, x + center, y1, y1, y1, y2, points[i], ++index));
                    series.Adornments.Add(this.CreateAdornment(series, x + center, y2, y2, y1, y2, points[i], ++index));
                    series.Adornments.Add(this.CreateAdornment(series, x + left, y3, y3, y1, y2, points[i], ++index));
                    series.Adornments.Add(this.CreateAdornment(series, x + right, y4, y4, y1, y2, points[i], ++index));
                }
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The ChartSeries</param>
        /// <param name="points">The indexed data points</param>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            this.CalculateSegments(series, points);
        }
        #endregion
    }


  /// <summary>
  /// Class implementation for ChartFastHiLoOpenCloseSegment
  /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartFastHiLoOpenCloseSegment : ChartSegment
    {
        #region Members
        /// <summary>
        /// Initializes m_sOpen
        /// </summary>
        private List<IChartDataPoint> m_sopen;

        /// <summary>
        /// Initializes m_eOpen
        /// </summary>
        private List<IChartDataPoint> m_eopen;

        /// <summary>
        /// Initializes m_sClose
        /// </summary>
        private List<IChartDataPoint> m_sclose;

        /// <summary>
        /// Initializes m_eClose
        /// </summary>
        private List<IChartDataPoint> m_eclose;
        private List<IChartDataPoint> m_point1;
        private List<IChartDataPoint> m_point2;

        internal bool affectRender = false;

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (this.m_point2 != null)
            {
                this.m_point2.Clear();
                this.m_point2 = null;
            }
            if (this.m_point1 != null)
            {
                this.m_point1.Clear();
                this.m_point1 = null;
            }
            if (this.m_eclose != null)
            {
                this.m_eclose.Clear();
                this.m_eclose = null;
            }
            if (this.m_sopen != null)
            {
                this.m_sopen.Clear();
                this.m_sopen = null;
            }
            if (this.m_sclose != null)
            {
                this.m_sclose.Clear();
                this.m_sclose = null;
            }
            if (this.m_eopen != null)
            {
                this.m_eopen.Clear();
                this.m_eopen = null;
            }
            if (this.Points != null)
            {
                this.Points.Clear();
                this.Points = null;
            }
            this.seriesCorrespondingPoints = null;
            this.Item = null;
        }

        /// <summary>
        /// Get or Set PointsProperty
        /// </summary>
        public HiLoOpenCloseChartDrawingValuesCollection Points
        {
            get { return (HiLoOpenCloseChartDrawingValuesCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        /// <summary>
        /// Identifies the points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(HiLoOpenCloseChartDrawingValuesCollection), typeof(ChartFastHiLoOpenCloseSegment), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="ChartHiLoOpenCloseSegment"/> class.
        /// </summary>
        /// <remarks>Default template is being assigned automatically.</remarks>
        static ChartFastHiLoOpenCloseSegment()
        {
            Type type = typeof(ChartFastHiLoOpenCloseSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.ChartHiLoOpenCloseSegment">ChartHiLoOpenCloseSegment</see> class. 
        /// </summary>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        /// <remarks></remarks>
        internal ChartFastHiLoOpenCloseSegment(ChartIndexedDataPoint[] correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {
            Points = new HiLoOpenCloseChartDrawingValuesCollection();
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
            double center = sbsInfo.Median;
            double left = sbsInfo.Start;
            double right = sbsInfo.End;
            m_sopen = new List<IChartDataPoint>();
            m_eopen = new List<IChartDataPoint>();
            m_sclose = new List<IChartDataPoint>();
            m_eclose = new List<IChartDataPoint>();
            m_point1 = new List<IChartDataPoint>();
            m_point2 = new List<IChartDataPoint>();
            
            for (int i = 0; i < correspondingPoints.Length; i++)
            {
                if (!correspondingPoints[i].DataPoint.EmptyPoint)
                {
                    ChartPoint hipoint = new ChartPoint(correspondingPoints[i].DataPoint.X + center, correspondingPoints[i].DataPoint.Values[1]);
                    ChartPoint lopoint = new ChartPoint(correspondingPoints[i].DataPoint.X + center, correspondingPoints[i].DataPoint.Values[0]);

                    ChartPoint sopoint = new ChartPoint(correspondingPoints[i].DataPoint.X + left, correspondingPoints[i].DataPoint.Values[2]);
                    ChartPoint eopoint = new ChartPoint(correspondingPoints[i].DataPoint.X + center, correspondingPoints[i].DataPoint.Values[2]);

                    ChartPoint scpoint = new ChartPoint(correspondingPoints[i].DataPoint.X + right, correspondingPoints[i].DataPoint.Values[3]);
                    ChartPoint ecpoint = new ChartPoint(correspondingPoints[i].DataPoint.X + center, correspondingPoints[i].DataPoint.Values[3]);

                    m_sopen.Add(sopoint);
                    m_eopen.Add(eopoint);
                    m_sclose.Add(scpoint);
                    m_eclose.Add(ecpoint);

                    m_point1.Add(hipoint);
                    m_point2.Add(lopoint);

                    xRange += new DoubleRange(ChartMath.Min(scpoint.X, ecpoint.X, sopoint.X, eopoint.X), ChartMath.Max(scpoint.X, ecpoint.X, sopoint.X, eopoint.X));
                    yRange += new DoubleRange(ChartMath.Min(scpoint.Y, ecpoint.Y, sopoint.Y, eopoint.Y), ChartMath.Max(scpoint.Y, ecpoint.Y, sopoint.Y, eopoint.Y));
                    //series.Segments.Add(new ChartHiLoOpenCloseSegment(hipoint, lopoint, sopoint, eopoint, scpoint, ecpoint, points[i], series));
                }
            }
            SetRange(series);

            correspondingPoints = null;
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            //if (noOfPoint == m_point1.Count && tranformWidth == transformer.Viewport.Width && tranformHeight == transformer.Viewport.Height)
            //{
            //    return;
            //}

    if (this.Interior.CanFreeze)
            {
                this.Interior.Freeze();
            }

            if (this.Stroke.CanFreeze)
            {
                this.Stroke.Freeze();
            }
            base.Update(transformer);
            if (Points != null)
            {
                Points.Clear();
                AffectRender = affectRender;
                affectRender = true;
                AffectRender = true;
            }
            for (int i = 0; i < m_point1.Count; i++)
            {
                Point sopoint = transformer.TransformToVisible(m_sopen[i].X, m_sopen[i].Y);
                Point eopoint = transformer.TransformToVisible(m_eopen[i].X, m_eopen[i].Y);
                Point scpoint = transformer.TransformToVisible(m_sclose[i].X, m_sclose[i].Y);
                Point ecpoint = transformer.TransformToVisible(m_eclose[i].X, m_eclose[i].Y);

                Point point1 = transformer.TransformToVisible(m_point1[i].X, m_point1[i].Y);
                Point point2 = transformer.TransformToVisible(m_point2[i].X, m_point2[i].Y);

                this.Points.Add(new HiLoOpenCloseDrawingValues()
                {
                    StartOpenPoint = sopoint,
                    EndOpenPoint = eopoint,
                    StartClosePoint = scpoint,
                    EndClosePoint = ecpoint,
                    HighPoint=point1,
                    LowPoint=point2
                });
            }

           
            //tranformHeight = transformer.Viewport.Height;
            //tranformWidth = transformer.Viewport.Width;
            //noOfPoint = m_point1.Count;
        }


        internal static readonly DependencyProperty AffectRenderProperty =
DependencyProperty.RegisterAttached("AffectRender", typeof(bool), typeof(ChartFastHiLoOpenCloseSegment), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Get or Set AffectRenderProperty
        /// </summary>
        public bool AffectRender
        {
            get { return (bool)GetValue(AffectRenderProperty); }
            set { SetValue(AffectRenderProperty, value); }
        }

        #endregion
    }
}
