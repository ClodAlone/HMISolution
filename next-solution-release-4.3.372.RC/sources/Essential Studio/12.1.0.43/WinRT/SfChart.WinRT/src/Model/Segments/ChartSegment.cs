#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Shapes;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using Windows.UI;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// An abstract base class for all type of chart segments.
    /// </summary>
    /// <remarks>
    /// You can create a custom chart segment by inheriting from <see cref="ChartSegment"/>. You can also customize the appearance of a chart segment,
    /// by specifying values for <see cref="ChartSegment.Interior"/>,<see cref="ChartSegment.Stroke"/> and <see cref="ChartSegment.StrokeThickness"/> properties.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public abstract class ChartSegment : DependencyObject, INotifyPropertyChanged
    {
        #region fields

        internal bool IsAddedToVisualTree = false;

        private bool isEmptySegmentInterior = false;

        /// <summary>
        /// Contains the x-value range for the segment.
        /// </summary>
        public DoubleRange XRange
        { get; set; }

        /// <summary>
        /// Contains the y-value range for the segment.
        /// </summary>
        public DoubleRange YRange
        { get; set; }
        /// <summary>
        /// Get or Set IsEmptySegmentinterior property
        /// </summary>
        protected internal bool IsEmptySegmentInterior 
        {
            get
            {
                return isEmptySegmentInterior;
            }
            internal set
            {
                if (isEmptySegmentInterior != value)
                {
                    isEmptySegmentInterior = value;
                    BindProperties();
                }
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or Sets the data object that this segment belongs to.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public object Item
        {
            get { return (object)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Item.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(object), typeof(ChartSegment), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the brush to paint the interior of the segment.
        /// </summary>
        ///<remarks>
        ///By default,the interior value for a chart segment will be calculated and set automatically based on the <see cref="ChartSeriesBase.Palette"/> set.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Interior.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InteriorProperty =
            DependencyProperty.Register("Interior", typeof(Brush), typeof(ChartSegment), new PropertyMetadata(null, OnInteriorChanged));

        private static void OnInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartSegment).OnPropertyChanged("Interior");
        }


        /// <summary>
        /// Gets or sets the stroke thickness value of the segment.
        /// </summary>
        /// <remarks>
        /// By default, this property inherits its value from <see cref="ChartSeriesBase.StrokeThickness"/> property.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ChartSegment), new PropertyMetadata(1d, onStrokeThicknessChanged));



        /// <summary>
        /// Gets or sets a collection of Double values that indicates the pattern of
        /// dashes and gaps that is used to outline shapes.</summary>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(ChartSegment), new PropertyMetadata(null,OnStrokeDashArrayChanged));

        private static void OnStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ChartSegment;
            if (e.NewValue != null && instance !=null)
            {
                    var collection = (DoubleCollection)e.NewValue;
                    if (collection != null && collection.Count > 0)
                    {
                        var doubleCollection = new DoubleCollection();
                        foreach (var value in collection)
                        {
                            doubleCollection.Add(value);
                        }
                        var shape = instance.GetRenderedVisual() as Shape;
                        if (shape != null)
                            shape.StrokeDashArray = doubleCollection;
                    }
            }
        }


        ///<summary>
        /// Gets or Sets the stroke value of the segment.
        ///</summary>
        /// <remarks>
        /// By default, this property inherits its value from <see cref="ChartSeriesBase.Stroke"/> property.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ChartSegment), new PropertyMetadata(null,OnStrokeValueChanged));

        private static void OnStrokeValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartSegment).OnPropertyChanged("Stroke");
        }

        private static void onStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartSegment).OnPropertyChanged("StrokeThickness");
        }

        /// <summary>
        /// ReadOnly property to get the value of underlying series of a chart segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartSeriesBase Series
        {
            get;
            protected internal set;
        }

        #endregion

        #region ctor

        #endregion       

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        [ClassReference(IsReviewed = false)]
        public virtual void SetData(IList<double> xVals, IList<double> yVals)
        {

        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        [ClassReference(IsReviewed = false)]
        public virtual void SetData(params double[] Values)
        {

        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="AreaPoints"></param>
        [ClassReference(IsReviewed = false)]
        public virtual void SetData(List<Point> AreaPoints)
        {

        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        [ClassReference(IsReviewed = false)]
        public virtual void SetData(Point point1, Point point2, Point point3, Point point4)
        {

        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yHiValues"></param>
        /// <param name="yLowValues"></param>
        [ClassReference(IsReviewed = false)]
        public virtual void SetData(IList<double> xValues, IList<double> yHiValues, IList<double> yLowValues)
        {

        }

        ///// <summary>
        ///// Sets the values for this segment. This method is not
        ///// intended to be called explicitly outside the Chart but it can be overriden by
        ///// any derived class.
        ///// </summary>
        ///// <param name="xValues"></param>
        ///// <param name="yHiValues"></param>
        ///// <param name="yLowValues"></param>
        ///// /// <param name="yOpenValues"></param>
        ///// /// <param name="yCloseValues"></param>
        //[ClassReference(IsReviewed = false)]
        //public virtual void SetData(List<double> xValues, List<double> yHiValues, List<double> yLowValues, List<double> yOpenValues, List<double> yCloseValues)
        //{

        //}

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yHiValues"></param>
        /// <param name="yLowValues"></param>
        /// /// <param name="yOpenValues"></param>
        /// /// <param name="yCloseValues"></param>
        [ClassReference(IsReviewed = false)]
        public virtual void SetData(IList<double> xValues, IList<double> yHiValues, IList<double> yLowValues, IList<double> yOpenValues, IList<double> yCloseValues)
        {

        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="x1Values"></param>
        /// <param name="y1Values"></param>
        /// <param name="x2Values"></param>
        /// /// <param name="y2Values"></param>
        public virtual void SetData(IList<double> x1Values, IList<double> y1Values, IList<double> x2Values, IList<double> y2Values)
        {

        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="leftpoint"></param>
        /// <param name="rightpoint"></param>
        /// <param name="toppoint"></param>
        /// <param name="bottompoint"></param>
        /// <param name="vercappoint"></param>
        /// <param name="horcappoint"></param>
        /// <param name="series"></param>
        [ClassReference(IsReviewed = false)]
        public virtual void SetData(Point leftpoint, Point rightpoint, Point toppoint, Point bottompoint, Point vercappoint, Point horcappoint)
        {

        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="BottomLeft"></param>
        /// <param name="RightTop"></param>
        /// <param name="hipoint"></param>
        /// <param name="loPoint"></param>
        /// <param name="isBull"></param>
        [ClassReference(IsReviewed = false)]
        public virtual void SetData(Point BottomLeft, Point RightTop, Point hipoint, Point loPoint, bool isBull)
        {
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="hipoint"></param>
        /// <param name="lopoint"></param>
        /// <param name="sopoint"></param>
        /// <param name="eopoint"></param>
        /// <param name="scpoint"></param>
        /// <param name="ecpoint"></param>
        /// <param name="isBull"></param>
        [ClassReference(IsReviewed = false)]
        public virtual void SetData(Point hipoint, Point lopoint, Point sopoint, Point eopoint, Point scpoint, Point ecpoint,bool isBull)
        {

        }

        #region methods

        internal void BindProperties()
        {
            if (!this.IsEmptySegmentInterior)
            {
                Binding binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("Interior");
                binding.Converter = new InteriorConverter(Series);
                binding.ConverterParameter = (Series is FunnelSeries) ? (Series.DataCount - 1) - Series.Segments.IndexOf(this) : Series.Segments.IndexOf(this);
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
          
        }
        /// <summary>
        /// Method Implementation for set  Binding to CgartSegments properties
        /// </summary>
        /// <param name="element"></param>
        protected virtual void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Interior");
            element.SetBinding(Shape.FillProperty, binding);           
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
            DoubleCollection collection = this.StrokeDashArray;
            if (collection != null && collection.Count > 0)
            {
                DoubleCollection doubleCollection = new DoubleCollection();
                foreach (double value in collection)
                {
                    doubleCollection.Add(value);
                }
                element.StrokeDashArray = doubleCollection;
            }

        }

        internal virtual UIElement CreateSegmentVisual(Size size)
        {
            BindProperties();
            return CreateVisual(size);
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
        public abstract UIElement CreateVisual(Size size);

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public abstract UIElement GetRenderedVisual();

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public abstract void Update(IChartTransformer transformer);

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        [ClassReference(IsReviewed = false)]
        public abstract void OnSizeChanged(Size size);

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Called when Property changed 
        /// </summary>
        /// <param name="name"></param>
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }

    /// <summary>
    /// An abstract base class for 3D type of chart segments.
    /// </summary>
    public abstract class ChartSegment3D : ChartSegment
    {
        internal List<Polygon3D> Polygons = new List<Polygon3D>();

        internal override UIElement CreateSegmentVisual(Size size)
        {
            if ((Series as ChartSeries3D).PrevSelectedIndex != Series.Segments.IndexOf(this))
                BindProperties();
            return CreateVisual(size);
        }
    }
}
