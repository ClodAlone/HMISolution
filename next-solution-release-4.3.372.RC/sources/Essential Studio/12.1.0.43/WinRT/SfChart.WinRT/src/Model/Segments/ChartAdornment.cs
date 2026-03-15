#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart adornment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="ChartAdornmentInfo"/>
    [ClassReference(IsReviewed = false)]
    public class ChartAdornment : ChartSegment
    {
        #region field

        internal ChartAdornmentContainer adormentContainer;

        double x;

        double y;

        double xpos, ypos;

        double xData;

        double yData;

        /// <summary>
        /// Get or Set series property
        /// </summary>
        public new ChartSeriesBase Series
        {
            get;
            protected internal set;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the connector rotation angle.
        /// </summary>
        /// <value>
        /// The connector rotation angle.
        /// </value>
        public double ConnectorRotationAngle
        {
            get { return (double)GetValue(ConnectorRotationAngleProperty); }
            set { SetValue(ConnectorRotationAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConnectorRotationAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorRotationAngleProperty =
            DependencyProperty.Register("ConnectorRotationAngle", typeof(double), typeof(ChartAdornment), new PropertyMetadata(0d));

        public double ConnectorHeight
        {
            get { return (double)GetValue(ConnectorHeightProperty); }
            set { SetValue(ConnectorHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConnectorHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorHeightProperty =
            DependencyProperty.Register("ConnectorHeight", typeof(double), typeof(ChartAdornment), new PropertyMetadata(0d));

        /// <summary>
        /// Gets the actual content displayed visually. Actual content is resolved based on
        /// <see cref="ChartAdornmentInfo.SegmentLabelContent"/>
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public object ActualContent
        {
            get
            {
                if (Series.adornmentInfo == null) return null;
                string labelFormat = Series.adornmentInfo.SegmentLabelFormat;
                var content = Series.adornmentInfo.SegmentLabelContent;
                if (Series.adornmentInfo.UseSeriesPalette)
                {
                    content = LabelContent.LabelContentPath;
                }
                switch (content)
                {
                    case LabelContent.XValue:
                        return this.XData.ToString(labelFormat, CultureInfo.CurrentCulture);
                    case LabelContent.YValue:
                        if (!double.IsNaN(YData))
                            return this.YData.ToString(labelFormat, CultureInfo.CurrentCulture);
                        else
                            return "";
                    case LabelContent.Percentage:
                        double grandPercentage = this.Series.GetGrandTotal(this.Series.ActualSeriesYValues[0]);
                        return (this.YData / grandPercentage * 100).ToString(labelFormat, CultureInfo.CurrentCulture) + "%";
                    case LabelContent.YofTot:
                        double grandTotal = this.Series.GetGrandTotal(this.Series.ActualSeriesYValues[0]);
                        return this.YData.ToString(labelFormat, CultureInfo.CurrentCulture) + " of " + grandTotal.ToString(labelFormat, CultureInfo.CurrentCulture);
                    case LabelContent.DateTime:
                        object datetimeContent = null;
                        if (this.Series.IsIndexed)
                        {
                            List<double> dateValues = this.Series.ActualXValues as List<double>;
                            if (dateValues != null)
                                datetimeContent = dateValues[(int)this.XData].FromOADate().ToString(this.Series.adornmentInfo.SegmentLabelFormat, CultureInfo.CurrentCulture);
                            else
                            {
                                List<string> stringValues = this.Series.ActualXValues as List<string>;
                                DateTime date=DateTime.MinValue;
                                DateTime.TryParse(stringValues[(int)this.XData], out date);
                                datetimeContent = date.ToString(this.Series.adornmentInfo.SegmentLabelFormat, CultureInfo.CurrentCulture);
                            }

                        }
                        else
                        {
                            datetimeContent = this.XData.FromOADate().ToString(this.Series.adornmentInfo.SegmentLabelFormat, CultureInfo.CurrentCulture);
                        }
                        return datetimeContent;
                    case LabelContent.LabelContentPath:
                        return this;
                    default:
                        if (!double.IsNaN(YData))
                            return this.YData.ToString(labelFormat, CultureInfo.CurrentCulture);
                        else
                            return "";
                }
            }
        }

        internal double XPos
        {
            get
            {
                return xpos;
            }
            set
            {
                xpos = value;
                OnPropertyChanged("XPos");
            }
        }


        internal double YPos
        {
            get
            {
                return ypos;
            }
            set
            {
                ypos = value;
                OnPropertyChanged("YPos");
            }
        }

        /// <summary>
        /// Gets or sets the x-value to be displayed in ChartAdornment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double XData
        {
            get
            {
                return xData;
            }
            set
            {
                xData = value;
                OnPropertyChanged("XData");
                OnPropertyChanged("ActualContent");
            }
        }

        /// <summary>
        /// Gets or sets the Y-value to be displayed in ChartAdornment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double YData
        {
            get
            {
                return yData;
            }
            set
            {
                yData = value;
                OnPropertyChanged("YData");
                OnPropertyChanged("ActualContent");
            }
        }

        /// <summary>
        /// Gets or sets the x screen coordinate relative to series
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double X
        {
            get
            {
                return x;
            }
            internal set
            {
                x = value;
            }
        }

        /// <summary>
        /// Gets or sets the y screen coordinate relative to series
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Y
        {
            get
            {
                return y;
            }
            internal set
            {
                y = value;
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public ChartAdornment()
        {

        }
        /// <summary>
        /// Called when instance created for ChartAdornment
        /// </summary>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="series"></param>
        public ChartAdornment(double xVal, double yVal, double x, double y, ChartSeriesBase series)
        {
            XData = xVal;
            YData = yVal;
            XPos = x;
            YPos = y;
            Series = series;
        }

        #endregion

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        public override void SetData(params double[] Values)
        {
            XData = Values[0];
            YData = Values[1];
            XPos = Values[2];
            YPos = Values[3];
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// retuns UIElement
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement CreateVisual(Size size)
        {
            if (adormentContainer == null)
                adormentContainer = new ChartAdornmentContainer(this);

            return adormentContainer;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return adormentContainer;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public override void Update(IChartTransformer transformer)
        {
            Point point = transformer.TransformToVisible(XPos, YPos);
            this.X = point.X;
            this.Y = point.Y;
        }

        internal override UIElement CreateSegmentVisual(Size size)
        {
            if (!this.IsEmptySegmentInterior)
            {
                Binding binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("Interior");
                binding.Converter = new InteriorConverter(Series);
                binding.ConverterParameter = (Series is FunnelSeries) ? (Series.DataCount - 1) - Series.Adornments.IndexOf(this) : Series.Adornments.IndexOf(this);
                BindingOperations.SetBinding(this, ChartSegment.InteriorProperty, binding);

                binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("Stroke");
                BindingOperations.SetBinding(this, ChartSegment.StrokeProperty, binding);
            }
            else
            {
                Binding binding = new Binding();
                binding.Source = Series;
                binding.ConverterParameter = Series.Interior;
                binding.Path = new PropertyPath("EmptyPointInterior");
                binding.Converter = new MultiInteriorConverter();
                BindingOperations.SetBinding(this, ChartSegment.InteriorProperty, binding);

                binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("Stroke");
                BindingOperations.SetBinding(this, ChartSegment.StrokeProperty, binding);
            }

            Binding binding2 = new Binding();
            binding2.Source = Series;
            binding2.Path = new PropertyPath("StrokeThickness");
            BindingOperations.SetBinding(this, ChartSegment.StrokeThicknessProperty, binding2);
           
            return CreateVisual(size);
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        [ClassReference(IsReviewed = false)]
        public override void OnSizeChanged(Size size)
        {

        }

        internal virtual ChartAdornmentContainer GetAdornmentContainer()
        {
            return adormentContainer;
        }

        #endregion
    }

    public class ChartAdornment3D : ChartAdornment
    {
        public ChartAdornment3D()
        {

        }

        /// <summary>
        /// Called when instance created for ChartAdornment
        /// </summary>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="startDepth"></param>
        /// <param name="series"></param>
        public ChartAdornment3D(double xVal, double yVal, double x, double y, double startDepth, ChartSeriesBase series)
        {
            StartDepth = startDepth;
            XData = xVal;
            YData = yVal;
            XPos = x;
            YPos = y;
            Series = series;
            if (series.ActualData.Count > x)
                Item = series.ActualData[(int) x];
        }
        internal double StartDepth { get; set; }
    }

    /// <summary>
    /// Represents chart adornment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    [ClassReference(IsReviewed = false)]
    public class ChartPieAdornment : ChartAdornment
    {
        #region field

        double angle;

        double radius;

        int pieIndex;

        #endregion

        #region properties

        /// <summary>
        /// Get or Set Angle property 
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Angle
        {
            get
            {
                return angle;
            }
            internal set
            {
                angle = value;
                OnPropertyChanged("Angle");
            }
        }
        /// <summary>
        /// Get or Set Radius property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Radius
        {
            get
            {
                return radius;
            }
            internal set
            {
                radius = value;
                OnPropertyChanged("Radius");
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartPieAdornment
        /// </summary>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <param name="series"></param>
        public ChartPieAdornment(double xVal, double yVal, double angle, double radius, AdornmentSeries series)
        {
            XPos = XData = xVal;
            YPos = YData = yVal;
            Radius = radius;
            Angle = angle;
            Series = base.Series = series;
            pieIndex = (from pieSeries in Series.ActualArea.VisibleSeries where pieSeries is CircularSeriesBase select pieSeries).ToList().IndexOf(series);
        }

        #endregion

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        public override void SetData(params double[] Values)
        {
            XPos = XData = Values[0];
            YPos = YData = Values[1];
            Angle = Values[2];
            Radius = Values[3];
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public override void Update(IChartTransformer transformer)
        {
            double radius = 0d;
            if (Series is PieSeries)
            {
                var hostSeries = Series as PieSeries;
                double width = Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / 2;
                Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);
                radius = Radius * width * (pieIndex + 1);
                if (hostSeries != null && hostSeries.LabelPosition != CircularSeriesLabelPosition.Inside)
                {
                    radius = hostSeries.PieCoefficient * width; ;
                }
                else if (hostSeries != null && hostSeries.GetPieSeriesCount() > 1)
                {

                    if (pieIndex > 0)
                    {
                        radius = radius - (radius - hostSeries.PieCoefficient * radius) / 2;
                    }
                    else
                    {
                        radius = radius / 2;
                    }
                }

                this.X = center.X + radius * Math.Cos(Angle);
                this.Y = center.Y + radius * Math.Sin(Angle);            
            }
            else if (Series is DoughnutSeries)
            {
                var hostSeries = Series as DoughnutSeries;

                double width = Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / 2;
                Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);
                radius = Radius * width * (pieIndex + 1);
                double dradius = radius * hostSeries.DoughnutCoefficient;
                radius += dradius;
                if (hostSeries != null && hostSeries.LabelPosition != CircularSeriesLabelPosition.Inside)
                {
                    radius = DoughnutSeries.DOUGHNUTSIZE * width;;
                }
                else if (hostSeries != null && hostSeries.GetDoughnutSeriesCount() > 1)
                {
                    if (pieIndex > 0)
                    {
                        radius = radius - (radius - hostSeries.DoughnutCoefficient * radius) / 2;
                    }
                    else
                    {
                        radius = radius / 2;
                    }
                }
                this.X = center.X + radius * Math.Cos(Angle);
                this.Y = center.Y + radius * Math.Sin(Angle);  
            }
        }
        #endregion
    }

    /// <summary>
    /// Class implementation for triangularAdornments
    /// </summary>
    public class TriangularAdornment : ChartAdornment
    {
        #region field

        double CurrY, Height;

        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for TriangularAdornment
        /// </summary>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="currY"></param>
        /// <param name="height"></param>
        /// <param name="series"></param>
        [ClassReference(IsReviewed = false)]
        public TriangularAdornment(double xVal, double yVal, double currY, double height, AdornmentSeries series)
        {
            XPos = XData = xVal;
            YPos = YData = yVal;
            CurrY = currY;
            Height = height;
            Series = base.Series = series;
        }

        #endregion

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(params double[] Values)
        {
            XPos = XData = Values[0];
            YPos = YData = Values[1];
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public override void Update(IChartTransformer transformer)
        {
            double bottom;
            Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);
            bottom = center.Y;
            center.Y = 0;
            center.Y += ((CurrY * bottom) * 2) - (Height / 2) * 4;
            this.X = center.X;
            this.Y = center.Y;
        }
        #endregion
    }

    public class ChartPieAdornment3D : ChartAdornment3D
    {
        #region field

        double angle;

        double radius;

        readonly int pieIndex;

        #endregion

        #region properties

        /// <summary>
        /// Get or Set Angle property 
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Angle
        {
            get
            {
                return angle;
            }
            internal set
            {
                angle = value;
                OnPropertyChanged("Angle");
            }
        }
        /// <summary>
        /// Get or Set Radius property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Radius
        {
            get
            {
                return radius;
            }
            internal set
            {
                radius = value;
                OnPropertyChanged("Radius");
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartPieAdornment
        /// </summary>
        /// <param name="xVal"></param>
        /// <param name="yVal"></param>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <param name="series"></param>
        public ChartPieAdornment3D(double startDepth, double xVal, double yVal, double angle, double radius, ChartSeries3D series)
        {
            XPos = XData = xVal;
            YPos = YData = yVal;
            Radius = radius;
            Angle = angle;
            Series = series;
            StartDepth = startDepth;
            pieIndex = (from pieSeries in Series.ActualArea.VisibleSeries where pieSeries is PieSeries3D select pieSeries).ToList().IndexOf(series);
        }

        public ChartPieAdornment3D()
        {

        }
        #endregion

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overridden by
        /// any derived class.
        /// </summary>
        /// <param name="values"></param>
        public override void SetData(params double[] values)
        {
            XPos = XData = values[0];
            YPos = YData = values[1];
            Angle = values[2];
            Radius = values[3];
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Represents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public override void Update(IChartTransformer transformer)
        {
            double radius = 0d;
            if (Series is PieSeries3D)
            {
                var hostSeries = Series as PieSeries3D;
                double width = Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / 2;
                Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);
                radius = Radius;// *width * (pieIndex + 1);
                if (hostSeries != null && hostSeries.LabelPosition != CircularSeriesLabelPosition.Inside)
                {
                    radius = hostSeries.CircleCoefficient * width;
                }
                else if (hostSeries != null)
                {
                    if (pieIndex > 0)
                    {
                        radius = radius - (radius - hostSeries.CircleCoefficient * radius) / 2;
                    }
                    else
                    {
                        radius = radius / 2;
                    }
                }

                this.X = center.X + radius * Math.Cos(Angle);
                this.Y = center.Y + radius * Math.Sin(Angle);
            }
            else if (Series is DoughnutSeries3D)
            {
                var hostSeries = Series as DoughnutSeries3D;

                double width = Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / 2;
                Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);
                radius = Radius * width * (pieIndex + 1);
                double dradius = radius * hostSeries.DoughnutCoefficient;
                radius += dradius;
                if (hostSeries != null && hostSeries.LabelPosition != CircularSeriesLabelPosition.Inside)
                {
                    radius = hostSeries.CircleCoefficient * width;
                }
                else if (hostSeries != null && hostSeries.GetCircularSeriesCount() > 1)
                {
                    if (pieIndex > 0)
                    {
                        radius = radius - (radius - hostSeries.DoughnutCoefficient * radius) / 2;
                    }
                    else
                    {
                        radius = radius / 2;
                    }
                }
                this.X = center.X + radius * Math.Cos(Angle);
                this.Y = center.Y + radius * Math.Sin(Angle);
            }
        }
        #endregion
    }
}
