// <copyright file="ChartPolarType.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
   using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using System.Collections;
    using System.Globalization;
    using System.Collections.ObjectModel;
    //using System.Windows.Media;

    //public class Symbol : ChartSegment
    //{
    //    #region Depdency Properties
    //    /// <summary>
    //    /// Identifies the X depency property.
    //    /// </summary>
    //    public static readonly DependencyProperty XProperty =
    //      DependencyProperty.Register("X", typeof(double), typeof(Symbol), new PropertyMetadata(0d));

    //    /// <summary>
    //    /// Identifies the Y depency property.
    //    /// </summary>
    //    public static readonly DependencyProperty YProperty =
    //      DependencyProperty.Register("Y", typeof(double), typeof(Symbol), new PropertyMetadata(0d));

    //    //// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty TemplateProperty =
    //        DependencyProperty.Register("Template", typeof(DataTemplate), typeof(Symbol), new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(typeof(Symbol))));

    //    /// <summary>
    //    /// Identifies the Points property.
    //    /// </summary>
    //    public static readonly DependencyProperty PointsProperty =
    //      DependencyProperty.Register("Points", typeof(PointCollection), typeof(Symbol), new PropertyMetadata(new PointCollection()));

    //    public static readonly DependencyProperty FillColorProperty =
    //        DependencyProperty.Register("FillColor", typeof(Brush), typeof(Symbol), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
    //    #endregion

    //    #region variables
    //    protected PointCollection m_points = new PointCollection();
    //    protected ChartPoint m_point;
    //    protected bool m_iswholeSegment = true;
    //    #endregion

    //    #region Properties
    //    /// <summary>
    //    /// Gets or sets the Template. This is a depency property.
    //    /// </summary>
    //    /// <value>The DataTemplate value.</value>
    //    public DataTemplate Template
    //    {
    //        get
    //        {
    //            return (DataTemplate)GetValue(TemplateProperty);
    //        }

    //        set
    //        {
    //            SetValue(TemplateProperty, value);
    //        }
    //    }

    //    public Brush FillColor
    //    {
    //        get
    //        {
    //            return (Brush)GetValue(FillColorProperty);
    //        }

    //        set
    //        {
    //            SetValue(FillColorProperty, value);
    //        }
    //    }

    //    /// <summary>
    //    /// Gets or sets the Points co-ordinate of segment. This is a depency property.
    //    /// </summary>
    //    /// <value>The Points value.</value>
    //    public PointCollection Points
    //    {
    //        get
    //        {
    //            return (PointCollection)GetValue(PointsProperty);
    //        }

    //        set
    //        {
    //            SetValue(PointsProperty, value);
    //        }
    //    }

    //    /// <summary>
    //    /// Gets or sets the X co-ordinate of segment. This is a depency property.
    //    /// </summary>
    //    /// <value>The X value.</value>
    //    public double X
    //    {
    //        get
    //        {
    //            return (double)GetValue(XProperty);
    //        }

    //        set
    //        {
    //            SetValue(XProperty, value);
    //        }
    //    }

    //    /// <summary>
    //    /// Gets or sets the Y co-ordinate of segment. This is a depency property.
    //    /// </summary>
    //    /// <value>The Y value.</value>
    //    public double Y
    //    {
    //        get
    //        {
    //            return (double)GetValue(YProperty);
    //        }

    //        set
    //        {
    //            SetValue(YProperty, value);
    //        }
    //    }
    //    #endregion

    //    #region Constructor
    //    internal Symbol(PointCollection points, ChartPoint point, PointCollection correspondingPoint, ChartSeries series)
    //        : base(series, correspondingPoint)
    //    {
    //        if (series.Template == null)
    //        {
    //            series.Template =   this.Template;
    //            //this.SegmentTemplate = r.GetAdornmentsTemplate(typeof(ChartPolarType), "Symbol") : this.Template;
    //        }
    //        else
    //        {
    //            series.ActualSegmentTemplate = series.SegmentTemplate;
    //            this.SegmentTemplate = series.SegmentTemplate;
    //        }

    //        m_point = point;
    //        m_points = points;
    //        m_iswholeSegment = iswholesegment;
    //    }
    //    #endregion

    //    #region Implementations
    //    /// <summary>
    //    /// Updates the real coordinates of segment.
    //    /// </summary>
    //    /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
    //    public override void Update(IChartTransformer transformer)
    //    {
    //        base.Update(transformer);
    //        this.Points.Clear();
    //        this.Points = new PointCollection();
    //        if (m_iswholeSegment)
    //        {
    //            foreach (ChartPoint data in m_points)
    //            {
    //                Point pt = transformer.TransformToVisible(data.X, data.Y, series);
    //                this.Points.Add(pt);
    //            }
    //        }
    //        else
    //        {
    //            Point pt = transformer.TransformToVisible(m_point.X, m_point.Y, series);
    //            this.X = pt.X;
    //            this.Y = pt.Y;
    //        }
    //    }
    //    #endregion

    //    public override void Dispose()
    //    {
    //        base.Dispose();
    //        if (this.m_points != null)
    //        {
    //            for (int temp = 0; temp < this.m_points.Count; temp++)
    //                this.m_points[temp] = null;
    //            this.m_points.Clear();
    //            this.m_points.series = null;
    //            this.m_points = null;
    //        }
    //        if (this.m_point != null)
    //            this.m_point = null;
    //        if (this.Points != null)
    //            this.Points.Clear();
    //        this.Points = null;
    //    }
    //    ~PolarRadarSegment()
    //    {
    //    }
    //}

    /// <summary>
    /// Represents ChartPolarType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartPolarType : ChartAreaType
    {
        #region Properties
        /// <summary>
        /// Gets chart type flags.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.None;
            }
        }
        #endregion

        #region Dependency property
        /// <summary>
        ///  Identifies the IsClosed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsClosedProperty =
       DependencyProperty.RegisterAttached("IsClosed", typeof(bool), typeof(ChartPolarType), new PropertyMetadata(false, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        ///  Identifies the DrawType dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawTypeProperty =
        DependencyProperty.RegisterAttached("DrawType", typeof(ChartPolarDrawType), typeof(ChartPolarType), new PropertyMetadata(ChartPolarDrawType.Area ,new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        ///  Identifies the PolarSymbol dependency property.
        /// </summary>
        public static readonly DependencyProperty PolarSymbolProperty =
     DependencyProperty.RegisterAttached("PolarSymbol", typeof(DataTemplate), typeof(ChartPolarType), new PropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));

        #endregion

        #region Static method
        /// <summary>
        /// Gets the IsClosed property value.
        /// </summary>
        /// <param name="area">The Chartarea.</param>
        /// <returns>The IsClosed</returns>
        public static bool GetIsClosed(ChartArea area)
        {
            return (bool)area.GetValue(IsClosedProperty);
        }

        /// <summary>
        /// Sets the IsClosed property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <param name="value">The value.</param>
        public static void SetIsClosed(ChartArea area, bool value)
        {
            area.SetValue(IsClosedProperty, value);
        }


        /// <summary>
        /// Gets the IsClosed property value.
        /// </summary>
        /// <param name="series">The ChartSeries.</param>
        /// <returns>The IsClosed</returns>
        public static DataTemplate GetPolarSymbol(ChartSeries series)
        {
            return (DataTemplate)series.GetValue(PolarSymbolProperty);
        }

        /// <summary>
        /// Sets the IsClosed property value.
        /// </summary>
        /// <param name="series">The ChartSeries.</param>
        /// <param name="value">The value.</param>
        public static void SetPolarSymbol(ChartSeries series, DataTemplate value)
        {
            series.SetValue(PolarSymbolProperty, value);
        }
        /// <summary>
        /// Gets the DrawType property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <returns>The Drawtype</returns>
        public static ChartPolarDrawType GetDrawType(ChartArea area)
        {
            return (ChartPolarDrawType)area.GetValue(DrawTypeProperty);
        }

        /// <summary>
        /// Sets the DrawType property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <param name="value">The value.</param>
        public static void SetDrawType(ChartArea area, ChartPolarDrawType value)
        {
            area.SetValue(DrawTypeProperty, value);          
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                area.m_visibleSeriesSegmentsRecountRequired = true;
                area.UpdateArea();
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets axes type that are required for chart to be built.
        /// </summary>
        /// <value>The type of the axes.</value>
        public override ChartAxesType AxesType
        {
            get
            {
                return ChartAxesType.PolarAxes;
            }
        }

        /// <summary>
        /// Converts ChartAreaType to string
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartPolarType"/>
        public override string ToString()
        {
            return "Polar";
        }

        //public override void Dispose()
        //{
        //    base.Dispose();
        //}
        #endregion
    }
}
