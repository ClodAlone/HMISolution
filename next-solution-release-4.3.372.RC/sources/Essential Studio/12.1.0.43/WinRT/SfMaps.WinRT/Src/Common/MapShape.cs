#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Path = Windows.UI.Xaml.Shapes.Path;
#else
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#endif


namespace Syncfusion.UI.Xaml.Maps
{
    /// <summary>
    /// Represents the MapShape class in the SfMap.Inherited from <see cref="Control"/> class.
    /// </summary>
    /// <remarks>
    /// MapShape are core element of the SfMap. MapShape is generate from a record in the ShapeFile.
    /// </remarks>

    [ClassReference(IsReviewed = false)]
    public class MapShape : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.MapShape">MapShape</see> class. 
        /// </summary>
        /// <remarks>
        /// Intialize the instance of the <see cref="MapShape"/>class and attribute values.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public MapShape()
        {
            DefaultStyleKey = typeof(MapShape);
            popupTimer.Interval = new TimeSpan(0, 0, 3);
            popupTimer.Tick += CollapsePopup;
        }

        #endregion

        #region Internal Fields

        internal bool isSelected = false;
        internal Brush tempFill;
        internal object value;
        internal Int32 shapeValueIndex;
        internal bool isKmlPolygon;

        #endregion

        #region PrivateFileds
      
        private ShapeFileLayer shapeLayer;
        private readonly DispatcherTimer popupTimer = new DispatcherTimer();

        #endregion

        #region Dependency Properties

        #region Shape
        /// <summary>
        /// Get the shape of the MapShape.
        /// </summary>
        /// <remarks>
        /// Shape is the read only property. Value for the Shape is internally set.
        /// </remarks>
        /// <value>
        /// Type :<see cref="Path"/>
        /// </value>
        [ClassReference(IsReviewed = false)]
        public Path Shape
        {
            get { return (Path)GetValue(ShapeProperty); }
            internal set { SetValue(ShapeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Shape.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register("Shape", typeof(Path), typeof(MapShape), new PropertyMetadata(null));
        #endregion

        #region Placemark
        internal KmlPlacemark Placemark
        {
            get { return (KmlPlacemark)GetValue(PlacemarkProperty); }
            set { SetValue(PlacemarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placemark.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PlacemarkProperty =
            DependencyProperty.Register("Placemark", typeof(KmlPlacemark), typeof(MapShape), new PropertyMetadata(null));
        #endregion

        #region ShapeValue
        /// <summary>
        /// Get the Under bound object value of the shape.
        /// </summary>
        /// <value>
        /// Type :<see cref="object"/>
        /// </value>
        /// <remarks>
        /// ShapeValue is the read only property. It contains under bound object value of the corresponding shape. Value for this property is internally set.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public object ShapeValue
        {
            get { return GetValue(ShapeValueProperty); }
            internal set { SetValue(ShapeValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeValueProperty =
            DependencyProperty.Register("ShapeValue", typeof(object), typeof(MapShape), new PropertyMetadata(null));
        #endregion

        #region ColorValue
        [ClassReference(IsReviewed = false)]
        public object ColorValue
        {
            get { return GetValue(ColorValueProperty); }
            internal set { SetValue(ColorValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorValueProperty =
            DependencyProperty.Register("ColorValue", typeof(object), typeof(MapShape), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Events      

#if WINRT
        void MapShape_PointerEntered(object sender, PointerRoutedEventArgs e)
#else
        void MapShape_MouseEnter(object sender, MouseEventArgs e)
#endif
        {
            KmlStyle kmlStyle = Placemark.HighlightStyle;
            if (kmlStyle != null)
            {
                SetShapePathWithKmlStyle(kmlStyle);
                if (shapeLayer == null)
                {
                    shapeLayer = SfMap.FindParent<ShapeFileLayer>(this);
                }
                foreach (MapAnnotations annotation in shapeLayer.Annotations)
                {
                    if (Shape.Data.Bounds.Contains(annotation.midpoint))
                        annotation.SetAnnotationSymbolWithKmlIcon(kmlStyle);
                }
            }
        }

#if WINRT
        void MapShape_PointerExited(object sender, PointerRoutedEventArgs e)
#else
        void MapShape_MouseLeave(object sender, MouseEventArgs e)
#endif
        {
            SfMap map = SfMap.FindParent<SfMap>(this);
            if (map == null || sender is MapShape && (e.OriginalSource.Equals(map.popup.Child) ||
                e.OriginalSource.Equals((map.popup.Child as Border).Child)))
            {
                return;
            }
            if (shapeLayer == null)
            {
                shapeLayer = SfMap.FindParent<ShapeFileLayer>(this);
            }
            if (Placemark != null && shapeLayer != null)
            {
                var kmlStyle = Placemark.NormalStyle;
                if (kmlStyle != null)
                {
                    SetShapePathWithKmlStyle(kmlStyle);
                    foreach (MapAnnotations annotation in shapeLayer.Annotations)
                    {
                        if (Shape.Data.Bounds.Contains(annotation.midpoint))
                            annotation.SetAnnotationSymbolWithKmlIcon(kmlStyle);
                    }
                }
            }
        }

        void CollapsePopup(object sender, object e)
        {
            if (shapeLayer.mapPopup.Visibility == Visibility.Visible)
                shapeLayer.mapPopup.Visibility = Visibility.Collapsed;
            popupTimer.Stop();
        } 

        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()
        {
#else
        public override void OnApplyTemplate()
        {
#endif          
            if (isKmlPolygon)
            {
#if WINRT
                PointerEntered += MapShape_PointerEntered;
                PointerExited += MapShape_PointerExited;
#else
                MouseEnter += MapShape_MouseEnter;
                MouseLeave += MapShape_MouseLeave;
#endif
            }

            base.OnApplyTemplate();
        }

#if WINRT
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (shapeLayer == null)
            {
                shapeLayer = SfMap.FindParent<ShapeFileLayer>(this);
            }
            var count = shapeLayer.SelectedMapShapes.Count;

            for (int i = 0; i < count; i++)
            {
                shapeLayer.SelectedMapShapes.RemoveAt((count - 1) - i);
            }
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            SfMap map = SfMap.FindParent<SfMap>(this);
            if (map != null &&  Placemark != null && Placemark.BalloonStyle != null)
            {
                map.popupGrid.Children.Clear();
                (map.popup.Child as Border).Background = new SolidColorBrush(Placemark.BalloonStyle.Background);
                FrameworkElement child = Placemark.BalloonStyle.GetContent(Placemark);
                map.popupGrid.Children.Add(child);
                map.popup.IsOpen = true;
                map.popup.HorizontalOffset = e.GetCurrentPoint(this).Position.X;
                map.popup.VerticalOffset = e.GetCurrentPoint(this).Position.Y;
            }
            base.OnPointerReleased(e);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (shapeLayer == null)
            {
                shapeLayer = SfMap.FindParent<ShapeFileLayer>(this);
            }
            if (shapeLayer.PopupCustomTemplate != null)
            {
                shapeLayer.MapPopupObject = DataContext;
            }
            if (ShapeValue == null)
            {
                shapeLayer.MapPopupObject = null;
                shapeLayer.PopupVisibility = Visibility.Collapsed;
            }
            shapeLayer.MapPopupMargin = new Thickness(e.GetPosition(shapeLayer).X, e.GetPosition(shapeLayer).Y, 0, 0);
            if (shapeLayer.EnableSelection)
            {

                if (isSelected)
                {
                    shapeLayer.SelectedMapShapes.Remove(this);
                }
                else
                {
                    shapeLayer.SelectedMapShapes.Add(this);
                }
            }

            base.OnTapped(e);
        }
#else
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.shapeLayer == null)
            {
                this.shapeLayer = SfMap.FindParent<ShapeFileLayer>(this) as ShapeFileLayer;
            }
            var count = this.shapeLayer.SelectedMapShapes.Count;

            for (int i = 0; i < count; i++)
            {
                this.shapeLayer.SelectedMapShapes.RemoveAt((count - 1) - i);
            }
            if (this.shapeLayer.PopupCustomTemplate != null)
            {
                this.shapeLayer.MapPopupObject = this.DataContext;
            }
            else
            {
                this.shapeLayer.MapPopupObject = null;
            }
            this.shapeLayer.MapPopupMargin = new Thickness(e.GetPosition(this.shapeLayer).X, e.GetPosition(this.shapeLayer).Y, 0, 0);
            this.shapeLayer.mapPopup.Visibility = Visibility.Visible;
            popupTimer.Start();
            if (this.shapeLayer.EnableSelection)
            {

                if (this.isSelected)
                {
                    this.shapeLayer.SelectedMapShapes.Remove(this);
                }
                else
                {
                    this.shapeLayer.SelectedMapShapes.Add(this);
                }
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            SfMap map = SfMap.FindParent<SfMap>(this);
            if (map != null && Placemark!=null && Placemark.BalloonStyle != null)
            {
                (map.popup.Child as Border).Background = new SolidColorBrush(Placemark.BalloonStyle.Background);
                FrameworkElement child = Placemark.BalloonStyle.GetContent(Placemark);
                map.popupGrid.Children.Clear();
                map.popupGrid.Children.Add(child);
                map.popup.IsOpen = true;
#if WPF
                map.popup.PlacementTarget = this;
                map.popup.PlacementRectangle = new Rect(e.GetPosition(this).X, e.GetPosition(this).Y, 0, 0);
#elif SILVERLIGHT
                map.popup.HorizontalOffset = e.GetPosition(null).X;
                map.popup.VerticalOffset = e.GetPosition(null).Y;
#else
                map.popup.HorizontalOffset = e.GetPosition(this).X;
                map.popup.VerticalOffset = e.GetPosition(this).Y;
#endif
            }
            base.OnMouseLeftButtonUp(e);
        }
#if !WPF
        protected override void OnHold(System.Windows.Input.GestureEventArgs e)
        {

            if (this.shapeLayer == null)
            {
                this.shapeLayer = SfMap.FindParent<ShapeFileLayer>(this) as ShapeFileLayer;
            }
            if (this.shapeLayer.PopupCustomTemplate != null)
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 3);
                timer.Tick += timer_Tick;
                timer.Start();
                this.shapeLayer.MapPopupVisibility = Visibility.Visible;
                this.shapeLayer.MapPopupObject = this.DataContext;
            }
            else
            {
                if (this.shapeLayer.MapPopupObject != null)
                {
                    this.shapeLayer.MapPopupObject = this;
                }
            }
            this.shapeLayer.MapPopupMargin = new Thickness(e.GetPosition(this.shapeLayer).X, e.GetPosition(this.shapeLayer).Y, 0, 0);

            base.OnHold(e);
        }        
        protected override void OnTap(System.Windows.Input.GestureEventArgs e)
        {
           

            if (this.shapeLayer.EnableSelection)
            {

                if (this.isSelected)
                {
                    this.shapeLayer.SelectedMapShapes.Remove(this);
                }
                else
                {
                    this.shapeLayer.SelectedMapShapes.Add(this);
                }
            }

            base.OnTap(e);
        }
#endif
        void timer_Tick(object sender, EventArgs e)
        {
            this.shapeLayer.MapPopupVisibility = Visibility.Collapsed;
        }
#endif

        #endregion

        #region Implementation

        internal void SetShapePathWithKmlStyle(KmlStyle kmlStyle)
        {
            Shape.Stroke = new SolidColorBrush(kmlStyle.LineStyle.LineColor);
            Shape.StrokeThickness = kmlStyle.PolyStyle.IsOutlined ? kmlStyle.LineStyle.LineThickness : 0;
            Shape.Fill = kmlStyle.PolyStyle.IsFilled
                ? new SolidColorBrush(kmlStyle.PolyStyle.FillColor)
                : new SolidColorBrush(Colors.Transparent);
        }

        #endregion
    }
}
