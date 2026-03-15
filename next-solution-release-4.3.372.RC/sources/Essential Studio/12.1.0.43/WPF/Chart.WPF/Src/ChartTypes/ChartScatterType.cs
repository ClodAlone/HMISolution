// <copyright file="ChartScatterType.cs" company="Syncfusion">
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
    using System.Windows.Shapes;
    using System.Windows.Media;
    using System.Windows.Data;
    using System.Windows.Controls;
    using System.Windows.Media.Media3D;
    using System.Windows.Documents;

    /// <summary>
    /// Represents the symbol that is a part of <see cref="ChartAdornment"/>.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartSymbolSegment : ChartSegment
    {
        #region Constants
        /// <summary>
        /// Initializes C_symbolSize
        /// </summary>
        private const double C_symbolSize = 10d;
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_point
        /// </summary>
        private IChartDataPoint m_point = null;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ChartSymbolSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ChartSymbolSegment), new PropertyMetadata(0d));

        #region Dependency Properties
        /// <summary>
        /// Identifies the scatterheight dependency property.
        /// </summary>
        public static readonly DependencyProperty scatterHeightProperty =
         DependencyProperty.Register("scatterHeight", typeof(double), typeof(ChartSymbolSegment), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the ScatterHeight property
        /// </summary>
        public double scatterHeight
        {
            get
            {
                return (double)GetValue(scatterHeightProperty);
            }

            set
            {
                SetValue(scatterHeightProperty, value);
            }
        }


        /// <summary>
        /// Identifies the scatterwidth dependency property.
        /// </summary>
        public static readonly DependencyProperty scatterWidthProperty =
              DependencyProperty.Register("scatterWidth", typeof(double), typeof(ChartSymbolSegment), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the scatterwidth
        /// </summary>
        public double scatterWidth
        {
            get
            {
                return (double)GetValue(scatterWidthProperty);
            }

            set
            {
                SetValue(scatterWidthProperty, value);
            }
        }

        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X symbol's co-ordinate. This is a dependency property.
        /// </summary>
        /// <remarks>Represents X co-ordinate of segment.</remarks>
        /// <value>The X <see cref="Double"/> value..</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y symbol's co-ordinate. This is a dependency property.
        /// </summary>
        /// <remarks>Represents Y co-ordinate of segment.</remarks>
        /// <value>The Y <see cref="Double"/> value.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartSymbolSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Chart symbol's segment default template is created automatically.
        /// </remarks>
        static ChartSymbolSegment()
        {
            Type type = typeof(ChartSymbolSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSymbolSegment"/> class.
        /// </summary>
        /// <param name="point">The point to represent.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        internal ChartSymbolSegment(IChartDataPoint point, ChartIndexedDataPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint })
        {
            m_point = point;
            this.SetXRange(m_point.X, m_point.X);
            this.SetYRange(m_point.Y, m_point.Y);
            if (this.Series.Area.EnableDepthAxis && m_point.Values.Length > 1)
                this.SetZRange(m_point.Values[1], m_point.Values[1]);
           
            Binding binding1 = new Binding();
            binding1.Path = new PropertyPath(ChartScatterType.ScatterHeightProperty);
            binding1.Source = this.Series;
            BindingOperations.SetBinding(this, ChartSymbolSegment.scatterHeightProperty, binding1);


            Binding binding2 = new Binding();
            binding2.Path = new PropertyPath(ChartScatterType.ScatterWidthProperty);
            binding2.Source = this.Series;
            BindingOperations.SetBinding(this, ChartSymbolSegment.scatterWidthProperty, binding2);


        }
        private void SetRange(ref ChartIndexedDataPoint correspondingPoint, ref DoubleRange sbsinfo)
        {
            if (!this.Series.IsIndexed)
            {
                xRange += xRange.Start - Math.Floor(this.Series.ActualXAxis.VisibleInterval * 0.5);
                xRange += xRange.End + Math.Ceiling(this.Series.ActualXAxis.VisibleInterval * 0.5);
               
            }
            else
            {
                xRange += xRange.Start - 0.5 + sbsinfo.Start;
                xRange += xRange.End + 0.5 + sbsinfo.End;
                
            }
            yRange += yRange.Start - Math.Floor(this.Series.ActualYAxis.VisibleInterval * 0.5);
            yRange += yRange.End + Math.Ceiling(this.Series.ActualYAxis.VisibleInterval * 0.5);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {

            if (this.Interior.CanFreeze && this.Series.ShowEmptyPoints == false)
            {
                this.Interior.Freeze();
            }

            if (this.Stroke.CanFreeze && this.Series.ShowEmptyPoints == false)
            {
                this.Stroke.Freeze();
            }

            Point pt = transformer.TransformToVisible(m_point.X, m_point.Y);

            if (this.Series.EnableAnimation)
            {
                this.X = pt.X - scatterWidth / 2;
                this.Y = pt.Y - scatterHeight / 2;
            }
            else
            {
                this.X = pt.X ;
                this.Y = pt.Y ;
            }
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public override void Dispose()
        {
            if (this.m_point != null)
            {
                this.m_point.Item = null;
                this.m_point = null;
            }
            this.Item = null;

            this.seriesCorrespondingPoints = null;

        }

        /// <summary>
        /// Draw3s the D segment.
        /// </summary>
        /// <param name="transformer">The transformer.</param>
        /// <seealso cref="ChartSymbolSegment"/>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            DoubleRange axisRange = Series.XAxis.VisibleRange;
            ////Skipping the segment if it falls out of axis' range.
            if (xRange.Start >= axisRange.Start && xRange.End <= axisRange.End)
            {
                Point3D pt = transformer.TransformToVisible(m_point.X, m_point.Y, m_point.Values.Length > 1 ? m_point.Values[1] : 0.06);
                pt=new Point3D(pt.X,pt.Y,this.Series.Area.EnableDepthAxis==true?(1-pt.Z):0.2);
                ////pt.X = 1 - pt.X;
                pt.Y = 1 - pt.Y;
                Geometry3D.Geometry = MeshGenerator.Sphere(0.02, 25);

                MaterialGroup materialGroup = new MaterialGroup();
                materialGroup.Children.Add(new EmissiveMaterial(Brushes.Green));

                DiffuseMaterial difuseMaterial = new DiffuseMaterial();
                Binding binding = new Binding("Interior");
                binding.Source = Series;
                bool colorEachValue = (this.Series.ColorEach == null ? false : (bool)this.Series.ColorEach);
                if (!colorEachValue)
                    BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
                else
                    this.Series.UpdateColorEachSegments(this.Series, this, this.Series.Segments.IndexOf(this), difuseMaterial);
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(difuseMaterial);

                Geometry3D.Material = materialGroup;

                Geometry3D.Transform = new TranslateTransform3D(pt.X - 0.5, pt.Y - 0.5, pt.Z);
            }
        }


        #endregion
    }

    /// <summary>
    /// Represents ChartScatterType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartScatterType : ChartType
    {
        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.None;
            }
        }
        #endregion
        #region Dependency Properties

        /// <summary>
        /// Return the double Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetScatterHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(ScatterHeightProperty);
        }

        /// <summary>
        /// Sets the value of the FastScatterHeight dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetScatterHeight(DependencyObject obj, double value)
        {
            obj.SetValue(ScatterHeightProperty, value);
        }

        /// <summary>
        /// Indicates the FastScatterHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty ScatterHeightProperty =
                DependencyProperty.RegisterAttached("ScatterHeight", typeof(double), typeof(ChartScatterType), new FrameworkPropertyMetadata(10d));




        /// <summary>
        /// Return double value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetScatterWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(ScatterWidthProperty);
        }

        /// <summary>
        /// Sets the value of the FastScatterWidth dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetScatterWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ScatterWidthProperty, value);
        }

        /// <summary>
        /// Indicates the FastScatterWidth Dependency Property
        /// </summary>
        public static readonly DependencyProperty ScatterWidthProperty =
                DependencyProperty.RegisterAttached("ScatterWidth", typeof(double), typeof(ChartScatterType), new FrameworkPropertyMetadata(10d));


        #endregion

        #region Public methods
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].DataPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            series.Segments.Add(new ChartSymbolSegment(points[i].DataPoint, points[i], series));
                            series.Segments[i].Interior = series.EmptyPointInterior;
                        }
                        else
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                        }
                    }
                    else
                    {
                        series.Segments.Add(new ChartSymbolSegment(points[i].DataPoint, points[i], series));
                        series.Segments[i].Interior = Brushes.Transparent;
                        series.Segments[i].Stroke = Brushes.Transparent;
                    }
                }
                else
                {
                    series.Segments.Add(new ChartSymbolSegment(points[i].DataPoint, points[i], series));
                }
            }

            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                for (int i = 0; i < points.Length; i++)
                {
                    //Add for SD11357
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                            series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                    }
                    else
                    {
                        series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartScatterType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            ////Removing last segments.
            while (series.Segments.Count > points.Length)
            {
                series.Segments.RemoveAt(series.Segments.Count - 1);
            }
            ////Synchronizing existing segments.
            //series.Segments.Clear();
            for (int i = 0; i < series.Segments.Count; i++)
            {
                IChartDataPoint segmentPoint = series.Segments[i].CorrespondingPoints[0].DataPoint;
                if (segmentPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            series.Segments[i] = new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate);
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            series.Segments[i] = new ChartSymbolSegment(points[i].DataPoint, points[i], series);
                            series.Segments[i].Interior = series.EmptyPointInterior;
                        }
                        else
                        {
                            series.Segments[i] = new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate);
                        }
                        //series.Segments[i].Interior = series.EmptyPointInterior;
                    }
                    else
                    {
                        series.Segments[i] = new ChartSymbolSegment(points[i].DataPoint, points[i], series);
                        series.Segments[i].Interior = Brushes.Transparent;
                        series.Segments[i].Stroke = Brushes.Transparent;
                    }
                }

                else if (segmentPoint.X != points[i].DataPoint.X || segmentPoint.Y != points[i].DataPoint.Y)
                {
                    series.Segments[i] = new ChartSymbolSegment(points[i].DataPoint, points[i], series);
                }
            }

            ////Adding missing points.
            int counter = 0;
            int segmentsPosition = series.Segments.Count;
            while (series.Segments.Count < points.Length)
            {
                series.Segments.Add(new ChartSymbolSegment(points[segmentsPosition + counter].DataPoint, points[segmentsPosition + counter], series));
                counter++;
            }

            series.Adornments.Clear();
            if (series.AdornmentsInfo.Visible)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                            series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                    }
                    else
                    {
                        series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                    }
                }
            }
        }
        #endregion
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <seealso cref="ChartScatterType"/>
        public override string ToString()
        {
            return "Scatter";
        }
    }

    /// <summary>
    /// Return double value for height proeprty
    /// </summary>
    public class HeightConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double size = -((double)(value) / 2);
            return size;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Return double value for Width property
    /// </summary>
    public class WidthConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double size = -((double)(value) / 2);
            return size;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Represents the symbol that is a part of <see cref="ChartAdornment"/>.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartEmptySymbolSegment : ChartSegment
    {
        #region Constants
        /// <summary>
        /// Initializes C_symbolSize
        /// </summary>
        private const double C_symbolSize = 10d;
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_point
        /// </summary>
        private IChartDataPoint m_point = null;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ChartEmptySymbolSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ChartEmptySymbolSegment), new PropertyMetadata(0d));

        //public static ResourceDictionary rd = new ResourceDictionary()
        //   {
        //       Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
        //   };
        //public static DataTemplate dt = new DataTemplate { DataType = typeof(ContentControl) };


        /// <summary>
        ///Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
        DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartEmptySymbolSegment), new PropertyMetadata(null));


        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X symbol's co-ordinate. This is a dependency property.
        /// </summary>
        /// <remarks>Represents X co-ordinate of segment.</remarks>
        /// <value>The X <see cref="Double"/> value..</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Get or set TemplateProperty
        /// </summary>
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y symbol's co-ordinate. This is a dependency property.
        /// </summary>
        /// <remarks>Represents Y co-ordinate of segment.</remarks>
        /// <value>The Y <see cref="Double"/> value.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartEmptySymbolSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Chart symbol's segment default template is created automatically.
        /// </remarks>
        /// 

        static ChartEmptySymbolSegment()
        {

            Type type = typeof(ChartEmptySymbolSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartEmptySymbolSegment"/> class.
        /// </summary>
        /// <param name="point">The point to represent.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        /// <param name="dt"></param>
        internal ChartEmptySymbolSegment(IChartDataPoint point, ChartIndexedDataPoint correspondingPoint, ChartSeries series, DataTemplate dt)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint })
        {

            m_point = point;
            double value = 10;
            DataTemplate DT = new DataTemplate();
            FrameworkElementFactory canvas = new FrameworkElementFactory(typeof(Canvas));
            FrameworkElementFactory ellipse = new FrameworkElementFactory(typeof(Ellipse));
            ellipse.SetValue(Ellipse.WidthProperty, value);
            ellipse.SetValue(Ellipse.HeightProperty, value);
            ellipse.SetValue(Ellipse.StrokeProperty, Brushes.Black);
            if (this.Series.EmptyPointStyle != EmptyPointStyle.SymbolAndInterior)
            {
                ellipse.SetValue(Ellipse.FillProperty, this.Interior);
            }
            else
            {
                ellipse.SetValue(Ellipse.FillProperty, series.EmptyPointInterior);
            }
            canvas.AppendChild(ellipse);
            DT.VisualTree = canvas;
            this.SetXRange(m_point.X, m_point.X);
            this.SetYRange(m_point.Y, m_point.Y);
            if (series.Type != ChartTypes.Polar && series.Type != ChartTypes.Radar)
            {
                if ((dt != null) && (series.EmptyPointStyle != EmptyPointStyle.SymbolAndInterior))
                {

                    this.Template = dt;
                }
                else
                {
                    this.Template = DT;

                }
            }
            else
            {
                if (dt != null)
                {

                    this.Template = dt;
                }
                else
                {
                    this.Template = DT;

                }
            }
        }


        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {

            if (this.Interior.CanFreeze && this.Series.ShowEmptyPoints == false)
            {
                this.Interior.Freeze();
            }

            if (this.Stroke.CanFreeze && this.Series.ShowEmptyPoints == false)
            {
                this.Stroke.Freeze();
            }

            Point pt = transformer.TransformToVisible(m_point.X, m_point.Y);

            this.X = pt.X;
            this.Y = pt.Y;
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public override void Dispose()
        {
            if (this.m_point != null)
            {
                this.m_point.Item = null;
                this.m_point = null;
            }
            this.Item = null;

            this.seriesCorrespondingPoints = null;

        }

        /// <summary>
        /// Draw3s the D segment.
        /// </summary>
        /// <param name="transformer">The transformer.</param>
        /// <seealso cref="ChartEmptySymbolSegment"/>
        public override void Draw3DSegment(IChartTransformer transformer)
        {
            DoubleRange axisRange = Series.XAxis.VisibleRange;
            ////Skipping the segment if it falls out of axis' range.
            if (xRange.Start >= axisRange.Start && xRange.End <= axisRange.End)
            {
                Point pt = transformer.TransformToVisible(m_point.X, m_point.Y);

                ////pt.X = 1 - pt.X;
                pt.Y = 1 - pt.Y;
                Geometry3D.Geometry = MeshGenerator.Sphere(0.02, 25);

                MaterialGroup materialGroup = new MaterialGroup();
                materialGroup.Children.Add(new EmissiveMaterial(Brushes.Green));

                DiffuseMaterial difuseMaterial = new DiffuseMaterial();
                Binding binding = new Binding("Interior");
                binding.Source = Series;
                bool colorEachValue = (this.Series.ColorEach == null ? false : (bool)this.Series.ColorEach);
                if (!colorEachValue)
                    BindingOperations.SetBinding(difuseMaterial, DiffuseMaterial.BrushProperty, binding);
                else
                    this.Series.UpdateColorEachSegments(this.Series, this, this.Series.Segments.IndexOf(this), difuseMaterial);
                materialGroup = new MaterialGroup();
                materialGroup.Children.Add(difuseMaterial);

                Geometry3D.Material = materialGroup;

                Geometry3D.Transform = new TranslateTransform3D(pt.X - 0.5, pt.Y - 0.5, 0.2);
            }
        }


        #endregion
    }


}
