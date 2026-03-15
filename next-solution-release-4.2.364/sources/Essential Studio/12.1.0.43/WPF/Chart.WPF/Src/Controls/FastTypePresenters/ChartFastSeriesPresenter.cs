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
using System.Collections;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// ChartFastSeriesPresenter abstract class acts a base renderer for Fast Chart types
    /// </summary>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class ChartFastSeriesPresenter : FrameworkElement
    {
        #region Members

        ListCollectionView pointsView;
        VisualCollection collection = null;



        #endregion

        #region Properties
            /// <summary>
            /// Get PointView CLR property value
            /// </summary>
        protected CollectionView PointView
        {
            get
            {
                return pointsView;
            }
        }
/// <summary>
/// Get Visualcollection CLR property value
/// </summary>
        protected VisualCollection VisualCollection
        {
            get
            {
                return collection;
            }
        }

        #endregion

        #region Dependency Properties

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.RegisterAttached("Index", typeof(int), typeof(ChartFastSeriesPresenter), new UIPropertyMetadata(0));

        /// <summary>
        /// Identifies the Pen dependency property.
        /// </summary>
        public static readonly DependencyProperty PenProperty =
      DependencyProperty.RegisterAttached("Pen", typeof(Pen), typeof(FastLinePresenter), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPenChanged)));

        private static void OnPenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null)
            {
                series.Invalidate();
            }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for DrawingPoints.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrawingPointsProperty =
            DependencyProperty.Register("DrawingPoints", typeof(IEnumerable), typeof(ChartFastSeriesPresenter),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDrawingPointsChanged)));

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ChartFastSeriesPresenter), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ChartFastSeriesPresenter), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Interior.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InteriorProperty =
            DependencyProperty.Register("Interior", typeof(Brush), typeof(ChartFastSeriesPresenter), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

        internal static readonly DependencyProperty AffectRenderProperty =
          DependencyProperty.RegisterAttached("AffectRender", typeof(bool), typeof(ChartFastSeriesPresenter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender |FrameworkPropertyMetadataOptions.BindsTwoWayByDefault ));


        /// <summary>
        /// Identifies the SegmentInteriorList dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentInteriorListProperty =
    DependencyProperty.Register("SegmentInteriorList", typeof(FastSegmnetPropertiesCollection), typeof(ChartFastSeriesPresenter), new FrameworkPropertyMetadata(new FastSegmnetPropertiesCollection()));

        #endregion

        #region Dependency Properties Members

        /// <summary>
        /// Get or Set SegmentInteriorList property
        /// </summary>
        public FastSegmnetPropertiesCollection SegmentInteriorList
        {
            get
            {
                return (FastSegmnetPropertiesCollection)GetValue(SegmentInteriorListProperty);
            }

            set
            {
                SetValue(SegmentInteriorListProperty, value);
            }
        }

        /// <summary>
        /// Return int value based on the given dependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetIndex(DependencyObject obj)
        {
            if (obj == null)
                return -1;
            return (int)obj.GetValue(IndexProperty);
        }

        /// <summary>
        /// Set Index to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIndex(DependencyObject obj, int value)
        {
            if (obj != null)
            {
                obj.SetValue(IndexProperty, value);
            }
        }

        /// <summary>
        /// return pen value based on given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Pen GetPen(DependencyObject obj)
        {
            if (obj == null)
                return new Pen(Brushes.Black, 0);
            return (Pen)obj.GetValue(PenProperty);
        }
            /// <summary>
        /// Set Pen value to the Corresponding DependencyObject from the Given value.
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="value"></param>
        public static void SetPen(DependencyObject obj, Pen value)
        {
            if (obj != null)
            {
                
                obj.SetValue(PenProperty, value);
                ChartSeries ser = (ChartSeries)obj;
                ser.FastTypePen = value;
                if (ser.Type == ChartTypes.FastLine)
                {
                    //ser.Interior = value.Brush;
                }
            }
        }
        /// <summary>
        /// Get or Set DrawingPointsProperty
        /// </summary>
        public IEnumerable DrawingPoints
        {
            get { return (IEnumerable)GetValue(DrawingPointsProperty); }
            set { SetValue(DrawingPointsProperty, value); }
        }

        /// <summary>
        /// Get or Set InteriorProperty
        /// </summary>
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }

        /// <summary>
        /// Get or Set AffectRenderProperty
        /// </summary>
        public bool AffectRender
        {
            get { return (bool)GetValue(AffectRenderProperty); }
            set { SetValue(AffectRenderProperty, value); }
        }

        /// <summary>
        /// Get or Set StrokeProperty
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Get or Set StrokeThicknessProperty
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }



        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for ChartFastSeriesPresenter
        /// </summary>
        public ChartFastSeriesPresenter()
        {
            collection = new VisualCollection(this);

            //this.Unloaded += delegate
            //{
            //    if (this.VisualCollection != null)
            //    {
            //        this.VisualCollection.Clear();
            //    }
            //    this.collection = null;
            //    this.pointsView = null;
            //    this.DrawingPoints = null;
            //};
            
        }

       

        #endregion

        #region PropertyChangedCallbacks

        private static void OnDrawingPointsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartFastSeriesPresenter presenter = obj as ChartFastSeriesPresenter;
            if (presenter.DrawingPoints != null)
            {
                presenter.pointsView = new ListCollectionView(presenter.DrawingPoints as IList);
            }
            else
            {
                presenter.pointsView = null;
            }
            presenter.OnDrawingPointsChanged(args);
        }

        #endregion

        #region Methods
            /// <summary>
        /// Virtual method created for OnDrawingPointsChanged
            /// </summary>
            /// <param name="args"></param>
        protected virtual void OnDrawingPointsChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the number of visual child elements.
        /// </summary>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        /// <remarks>VisualChildrenCount is the Override method used to get the count of the visual child from the visual collection.</remarks>
        protected override int VisualChildrenCount
        {
            get
            {
                if (collection != null)
                    return collection.Count;
                return -1;
            }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        /// <remarks>GetVisualChild is the Override method used to get the visual child from the visual collection based on the index value.</remarks>
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= collection.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return collection[index];
        }


        #endregion

    }
    /// <summary>
    /// 
    /// </summary>
    public class FastSegmnetProperties : DependencyObject
    {

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(FastSegmnetProperties), new PropertyMetadata(double.NaN));

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(FastSegmnetProperties), new PropertyMetadata(null));

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Interior.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InteriorProperty =
            DependencyProperty.Register("Interior", typeof(Brush), typeof(FastSegmnetProperties), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the interior.
        /// </summary>
        /// <value>The interior.</value>
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>The stroke.</value>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>The stroke thickness.</value>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastSegmnetProperties"/> class.
        /// </summary>
        public FastSegmnetProperties()
        {
            Stroke = null;
            StrokeThickness = double.NaN;
            Interior = null;
        }
    }


    internal class FastSegmentsPropertiesConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Converts a value. The data binding engine calls this method when it propagates a value from the binding target to the binding source.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that to the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
        /// </returns>
        /// <seealso cref="FastSegmentsPropertiesConverter"/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FastSegmnetPropertiesCollection collection = value as FastSegmnetPropertiesCollection;
            collection.DataCount = collection.Count;
            collection.IsUpdated = true;
            return collection;
        }


        /// <summary>
        /// Converts a value. The data binding engine calls this method when it propagates a value from the binding target to the binding source.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that to the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
        /// </returns>
        /// <seealso cref="FastSegmentsPropertiesConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
