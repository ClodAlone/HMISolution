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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents the Class SparkLinePresenter for draw SparkLine
    /// </summary>
    public abstract class SparkLinePresenter : FrameworkElement
    {
        #region Members

        ListCollectionView pointsView;

        #endregion

        #region Properties
        /// <summary>
        /// CLR property PointView defiinition
        /// </summary>
        protected CollectionView PointView
        {
            get
            {
                return pointsView;
            }
        }

        /// <summary>
        /// Get and Set VisualCollectioProeprty
        /// </summary>
        protected VisualCollection VisualCollection { get; private set; }

        #endregion

        #region Dependency Properties

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.RegisterAttached("Index", typeof(int), typeof(SparkLinePresenter), new UIPropertyMetadata(0));

        /// <summary>
        ///  Identifies the Pen dependency property.
        /// </summary>
        public static readonly DependencyProperty PenProperty =
      DependencyProperty.RegisterAttached("Pen", typeof(Pen), typeof(SparkLinePresenter), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPenChanged)));

        private static void OnPenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //ChartSeries series = d as ChartSeries;
            //if (series != null)
            //{
            //    series.Invalidate();
            //}
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for DrawingPoints.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrawingPointsProperty =
            DependencyProperty.Register("DrawingPoints", typeof(IEnumerable), typeof(SparkLinePresenter),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDrawingPointsChanged)));

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(SparkLinePresenter), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(SparkLinePresenter), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Interior.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InteriorProperty =
            DependencyProperty.Register("Interior", typeof(Brush), typeof(SparkLinePresenter), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

        internal static readonly DependencyProperty AffectRenderProperty =
          DependencyProperty.RegisterAttached("AffectRender", typeof(bool), typeof(SparkLinePresenter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        #endregion

        #region Dependency Properties Members

        /// <summary>
        /// Method implemetation for getindex from DependencyObject
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
        /// method implementation for setindex from the given integer value
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
        /// Method implementation for return pen value from given DependencyObject
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
        /// Method implemetation for Set pen value to given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetPen(DependencyObject obj, Pen value)
        {
            if (obj != null)
            {
                obj.SetValue(PenProperty, value);
            }
        }
        /// <summary>
        /// Get and Set DrawingPointsProperty
        /// </summary>
        public IEnumerable DrawingPoints
        {
            get { return (IEnumerable)GetValue(DrawingPointsProperty); }
            set { SetValue(DrawingPointsProperty, value); }
        }

        /// <summary>
        /// Get and Set InteriorProperty
        /// </summary>
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }

        /// <summary>
        /// Get and Set AffectRenderProperty
        /// </summary>
        public bool AffectRender
        {
            get { return (bool)GetValue(AffectRenderProperty); }
            set { SetValue(AffectRenderProperty, value); }
        }

        /// <summary>
        /// Get and Set StrokeProperty
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Get and Set StrokeThicknessProperty
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }



        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for SparkLinePresenter
        /// </summary>
        public SparkLinePresenter()
        {
            VisualCollection = new VisualCollection(this);
        }



        #endregion

        #region PropertyChangedCallbacks

        private static void OnDrawingPointsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SparkLinePresenter presenter = obj as SparkLinePresenter;
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
                if (VisualCollection != null)
                    return VisualCollection.Count;
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
            if (index < 0 || index >= VisualCollection.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return VisualCollection[index];
        }


        #endregion

    }
  

    /// <summary>
    /// Enum values for SparkLineTypes
    /// </summary>
    public enum SparkLineTypes
    {
        /// <summary>
        /// Enum value for Line type
        /// </summary>
        Line,
        /// <summary>
        /// Enum value for column type
        /// </summary>
        Column,
        /// <summary>
        /// Enum value for WinLoss type
        /// </summary>
        WinLoss
    }

    /// <summary>
    /// Enum values for LineMarker types
    /// </summary>
    public enum LineMarkerTypes
    { 
        /// <summary>
        /// Enum value for square type
        /// </summary>
        Square,
        /// <summary>
        /// Enum value for Circle type
        /// </summary>
        Circle
    }

    /// <summary>
    /// Enum values for AxisEndPointMode
    /// </summary>
    public enum AxisEndPointMode
    { 
        /// <summary>
        /// Enum value for Auto Mode
        /// </summary>
        Auto,
        /// <summary>
        /// Enum value for Custom Mode
        /// </summary>
        Custom
    }
}
