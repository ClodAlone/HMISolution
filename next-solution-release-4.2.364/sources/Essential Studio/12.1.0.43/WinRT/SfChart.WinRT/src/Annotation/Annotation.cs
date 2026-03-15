#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
#else
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract class Annotation : FrameworkElement, ICloneable
    {        
        protected double x1;

        protected double y1;

        protected bool IsUiCleared;

        internal bool IsAddedToVisualTree;

        protected Grid AnnotationElement;

        protected Canvas TextElementCanvas;

        protected ContentControl TextElement;

        private Matrix _transformation;

        private Rect _transformedDesiredElement;

        private Size _transformedDesiredSize;

        internal Rect RotatedRect { get; set; }

        protected Rect RotatedTextRect { get; set; }

        internal bool IsSelected { get; set; }

        internal bool IsResizing { get; set; }

        internal bool IsVisbilityChanged { get; set; }

        SfChart chart;

        /// <summary>
        /// Initializes a new instance of the <see cref="Annotation"/> class.
        /// </summary>
        public Annotation()
        {
            this.AnnotationElement = new Grid();
            this.TextElementCanvas = new Canvas();
            this.TextElement = new ContentControl();
        }
        

        /// <summary>
        /// Gets or Sets the HorizontalAlignment for Annotation
        /// </summary>
        internal HorizontalAlignment InternalHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(InternalHorizontalAlignmentProperty); }
            set { SetValue(InternalHorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// The horizontal alignment property
        /// </summary>
        internal static readonly DependencyProperty InternalHorizontalAlignmentProperty =
            DependencyProperty.Register("InternalHorizontalAlignment", typeof(HorizontalAlignment), typeof(Annotation), new PropertyMetadata(HorizontalAlignment.Right, OnUpdatePropertyChanged));


        /// <summary>
        /// Gets or Sets the VerticalAlignment for the Annotation
        /// </summary>
        internal VerticalAlignment InternalVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(InternalVerticalAlignmentProperty); }
            set { SetValue(InternalVerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// The vertical alignment property
        /// </summary>
        internal static readonly DependencyProperty InternalVerticalAlignmentProperty =
            DependencyProperty.Register("InternalVerticalAlignment", typeof(VerticalAlignment), typeof(Annotation), new PropertyMetadata(VerticalAlignment.Bottom, OnUpdatePropertyChanged));

        /// <summary>
        /// Gets or Sets the VerticalAlignment for the Annotation
        /// </summary>
        internal Visibility InternalVisibility
        {
            get { return (Visibility)GetValue(InternalVisibilityProperty); }
            set { SetValue(InternalVisibilityProperty, value); }
        }

        /// <summary>
        /// The vertical alignment property
        /// </summary>
        internal static readonly DependencyProperty InternalVisibilityProperty =
            DependencyProperty.Register("InternalVisibility", typeof(Visibility), typeof(Annotation), new PropertyMetadata(Visibility.Visible, OnVisibiltyChanged));

        private static void OnVisibiltyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(!e.NewValue.ToString().Equals(e.OldValue.ToString()))
                (d as Annotation).OnVisibilityChanged(e.NewValue);
        }

        internal virtual void OnVisibilityChanged(object visibility)
        {
            if (this.chart != null && this.chart.AnnotationManager !=null)
            {
                bool isResizer = (this is ShapeAnnotation && chart.AnnotationManager.SelectedAnnotation == this
                    && (this as ShapeAnnotation).CanResize);
                IsVisbilityChanged = true;
                
                if (visibility.Equals(Visibility.Collapsed))
                {
                    this.chart.AnnotationManager.AddOrRemoveAnnotations(this, true);
                    if (isResizer && this.chart.AnnotationManager.AnnotationResizer!=null)
                    {
                        this.chart.AnnotationManager.AnnotationResizer.IsVisbilityChanged = true;
                        this.chart.AnnotationManager.AddOrRemoveAnnotations(this.chart.AnnotationManager.AnnotationResizer, true);
                        
                    }
                }
                else
                {
                    this.chart.AnnotationManager.AddOrRemoveAnnotations(this, false);
                    if (isResizer && this.chart.AnnotationManager.AnnotationResizer != null)
                    {
                        this.chart.AnnotationManager.AnnotationResizer.IsVisbilityChanged = true;
                        this.chart.AnnotationManager.AddOrRemoveAnnotations(this.chart.AnnotationManager.AnnotationResizer, false);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets the description text for Annotation
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// The text property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Annotation), new PropertyMetadata(string.Empty));



        /// <summary>
        /// Gets or sets the text template.
        /// </summary>
        /// <value>
        /// The text template.
        /// </value>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// The text template property
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(Annotation), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ShowToolTip value to enable or disable the tooltip view in annotation.
        /// </summary>
        public bool ShowToolTip
        {
            get { return (bool)GetValue(ShowToolTipProperty); }
            set { SetValue(ShowToolTipProperty, value); }
        }

       
        /// <summary>
        /// The ShowToolTip property
        /// </summary>
        public static readonly DependencyProperty ShowToolTipProperty =
            DependencyProperty.Register("ShowToolTip", typeof(bool), typeof(Annotation), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the ToolTipContent to view in annoatation tooltip.
        /// </summary>
        public object ToolTipContent
        {
            get { return (object)GetValue(ToolTipContentProperty); }
            set { SetValue(ToolTipContentProperty, value); }
        }

        /// <summary>
        /// The ToolTipContent property
        /// </summary>
        public static readonly DependencyProperty ToolTipContentProperty =
            DependencyProperty.Register("ToolTipContent", typeof(object), typeof(Annotation), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ToolTipShowDuration(MilliSeconds). It's to mention the duration of annotation tooltip viewing.  
        /// </summary>
        public double ToolTipShowDuration
        {
            get { return (double)GetValue(ToolTipShowDurationProperty); }
            set { SetValue(ToolTipShowDurationProperty, value); }
        }

        /// <summary>
        /// The ToolTipContent property
        /// </summary>
        public static readonly DependencyProperty ToolTipShowDurationProperty =
            DependencyProperty.Register("ToolTipShowDuration", typeof(double), typeof(Annotation), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the ToolTipTemplate for the annotation tooltip.
        /// </summary>
        /// <value>
        /// The data template.
        /// </value>
        public DataTemplate ToolTipTemplate
        {
            get { return (DataTemplate)GetValue(ToolTipTemplateProperty); }
            set { SetValue(ToolTipTemplateProperty, value); }
        }

        /// <summary>
        /// The ToolTipTemplate property
        /// </summary>
        public static readonly DependencyProperty ToolTipTemplateProperty =
            DependencyProperty.Register("ToolTipTemplate", typeof(DataTemplate), typeof(Annotation), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ToolTipPlacement value to place the tooltip in different position.
        /// </summary>
        public ToolTipLabelPlacement ToolTipPlacement
        {
            get { return (ToolTipLabelPlacement)GetValue(ToolTipPlacementProperty); }
            set { SetValue(ToolTipPlacementProperty, value); }
        }

        /// <summary>
        /// The ToolTipPlacement property
        /// </summary>
        public static readonly DependencyProperty ToolTipPlacementProperty =
            DependencyProperty.Register("ToolTipPlacement", typeof(ToolTipLabelPlacement), typeof(Annotation), new PropertyMetadata(ToolTipLabelPlacement.Right));

        /// <summary>
        /// Gets or Sets the Annotation Drawing Mode
        /// </summary>
        public CoordinateUnit CoordinateUnit
        {
            get { return (CoordinateUnit)GetValue(CoordinateUnitProperty); }
            set { SetValue(CoordinateUnitProperty, value); }
        }

        /// <summary>
        /// The coordinate unit property
        /// </summary>
        public static readonly DependencyProperty CoordinateUnitProperty =
            DependencyProperty.Register("CoordinateUnit", typeof(CoordinateUnit), typeof(Annotation), new PropertyMetadata(CoordinateUnit.Axis, OnUpdatePropertyChanged));


        internal ChartAxis XAxis { get; set; }

        internal ChartAxis YAxis { get; set; }
     
        /// <summary>
        /// Gets or Sets the Which axis attached to XAxis of Annoation when Annotation CoordinateUnit is Axis
        /// </summary>
        public string XAxisName
        {
            get { return (string)GetValue(XAxisNameProperty); }
            set { SetValue(XAxisNameProperty, value); }
        }

        /// <summary>
        /// The X axis name property
        /// </summary>
        public static readonly DependencyProperty XAxisNameProperty =
            DependencyProperty.Register("XAxisName", typeof(string), typeof(Annotation), new PropertyMetadata(string.Empty));


        /// <summary>
        /// Gets or Sets the Which axis attached to YAxis of Annoation when Annotation CoordinateUnit is Axis
        /// </summary>
        public string YAxisName
        {
            get { return (string)GetValue(YAxisNameProperty); }
            set { SetValue(YAxisNameProperty, value); }
        }

        /// <summary>
        /// The Y axis name property
        /// </summary>
        public static readonly DependencyProperty YAxisNameProperty =
            DependencyProperty.Register("YAxisName", typeof(string), typeof(Annotation), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or Sets the X1 position of Annoation
        /// </summary>
        public object X1
        {
            get { return (object)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        /// <summary>
        /// The x1 property
        /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(object), typeof(Annotation), new PropertyMetadata(null,OnUpdatePropertyChanged));


        /// <summary>
        /// Gets or Sets the Y1 position of Annoation
        /// </summary>
        public object Y1
        {
            get { return (object)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        /// <summary>
        /// The font size property
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(Annotation), new PropertyMetadata(TextBlock.FontSizeProperty.GetMetadata(typeof(TextBlock)).DefaultValue));

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// The font family property
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(Annotation), new PropertyMetadata(TextBlock.FontFamilyProperty.GetMetadata(typeof(TextBlock)).DefaultValue));

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// The font stretch property
        /// </summary>
        public static readonly DependencyProperty FontStretchProperty =
            DependencyProperty.Register("FontStretch", typeof(FontStretch), typeof(Annotation), new PropertyMetadata(TextBlock.FontStretchProperty.GetMetadata(typeof(TextBlock)).DefaultValue));

        /// <summary>
        /// Gets or sets the font stretch.
        /// </summary>
        /// <value>
        /// The font stretch.
        /// </value>
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        /// <summary>
        /// The font style property
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(Annotation), new PropertyMetadata(TextBlock.FontStyleProperty.GetMetadata(typeof(TextBlock)).DefaultValue));

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>
        /// The font style.
        /// </value>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// The font weight property
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(Annotation), new PropertyMetadata(TextBlock.FontWeightProperty.GetMetadata(typeof(TextBlock)).DefaultValue));

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>
        /// The font weight.
        /// </value>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// The foreground property
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(Annotation), new PropertyMetadata(TextBlock.ForegroundProperty.GetMetadata(typeof(TextBlock)).DefaultValue));

        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>
        /// The foreground.
        /// </value>
        public Brush Foreground
        {
            get { return (Brush) GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// The y1 property
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(object), typeof(Annotation), new PropertyMetadata(null, OnUpdatePropertyChanged));
        
        private static void OnUpdatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var annotation = sender as Annotation;
            if (annotation != null) annotation.UpdatePropertyChanged(args);
        }

        internal void UpdatePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Chart != null && Chart.AnnotationManager!=null && Chart.AnnotationManager.SelectedAnnotation != null && this is AnnotationResizer)
            {
                Chart.AnnotationManager.SelectedAnnotation.Y1 = this.Y1;
                Chart.AnnotationManager.SelectedAnnotation.X1 = this.X1;
            }
            UpdateAnnotation();
        }


        internal virtual SfChart Chart
        {
            get { return chart; }
            set
            {
                chart = value;
                SetAxisFromName();
            }
        }
        

        internal virtual UIElement CreateAnnotation()
        {
            return AnnotationElement;
        }

        /// <summary>
        /// Gets the rendered annotation.
        /// </summary>
        /// <returns></returns>
        public virtual UIElement GetRenderedAnnotation()
        {
            return AnnotationElement;
        }

        internal void SetAxisFromName()
        {
            if (Chart != null)
            {
                this.XAxis = Chart.Axes[this.XAxisName] ?? Chart.InternalPrimaryAxis;                
                this.YAxis = Chart.Axes[this.YAxisName] ?? Chart.InternalSecondaryAxis;                                         
            }
        }


        protected Point GetElementPosition(Size desiredSize,Point originalPosition)
        {
            Point point = originalPosition;
            HorizontalAlignment horizontalAlignment = this.InternalHorizontalAlignment;
            VerticalAlignment verticalAlignment = this.InternalVerticalAlignment;    

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    point.X -= (desiredSize.Width / 2);
                    break;
                case HorizontalAlignment.Left:
                    point.X -= (desiredSize.Width);
                    break;
                case HorizontalAlignment.Right:                   
                    break;   
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    break;
                case VerticalAlignment.Center:
                    point.Y -= (desiredSize.Height / 2);
                    break;               
                case VerticalAlignment.Top:
                    point.Y -= (desiredSize.Height);
                    break;
            }
            return point;
        }

        protected Point GetElementPosition(FrameworkElement element, Point originalPosition)
        {
            Point point = originalPosition;
            HorizontalAlignment horizontalAlignment = this.InternalHorizontalAlignment;
            VerticalAlignment verticalAlignment = this.InternalVerticalAlignment;
            Size desiredSize = new Size(element.ActualWidth, element.ActualHeight);

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    point.X -= (desiredSize.Width / 2);
                    break;
                case HorizontalAlignment.Left:
                    point.X -= (desiredSize.Width);
                    break;
                case HorizontalAlignment.Right:
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    break;
                case VerticalAlignment.Center:
                    point.Y -= (desiredSize.Height / 2);
                    break;
                case VerticalAlignment.Top:
                    point.Y -= (desiredSize.Height);
                    break;
            }
            return point;
        }

        

        protected bool IntersectsWith(Rect r1, Rect r2)
        {
            return !(r2.Left > r1.Right ||
                     r2.Right < r1.Left ||
                     r2.Top > r1.Bottom ||
                     r2.Bottom < r1.Top);
        }



        /// <summary>
        /// Updates the annotation.
        /// </summary>
        public virtual void UpdateAnnotation()
        {          
            SetData();
        }

        protected  void SetData()
        {
            if (XAxis != null && YAxis !=null)
            {
                x1 = ConvertData(X1,XAxis);
                y1 = ConvertData(Y1,YAxis);                
            }
        }

        protected virtual void SetBindings()
        {            
            Binding horizontalAlignBinding= new Binding{ Path= new PropertyPath("HorizontalAlignment"), Source=this };
            this.SetBinding(Annotation.InternalHorizontalAlignmentProperty, horizontalAlignBinding);
            Binding verticalAlignBinding = new Binding { Path = new PropertyPath("VerticalAlignment"), Source = this };
            this.SetBinding(Annotation.InternalVerticalAlignmentProperty, verticalAlignBinding);
            Binding visibilityBinding = new Binding { Path = new PropertyPath("Visibility"), Source = this };
            this.SetBinding(Annotation.InternalVisibilityProperty, visibilityBinding);
            if (TextElement != null)
            {
                Binding textBinding = new Binding { Path = new PropertyPath("Text"), Source = this };
                TextElement.SetBinding(ContentControl.ContentProperty, textBinding);
                Binding fontSizeBinding = new Binding { Source = this, Path = new PropertyPath("FontSize") };
                TextElement.SetBinding(ContentControl.FontSizeProperty, fontSizeBinding);
                Binding fontStyleBinding = new Binding { Source = this, Path = new PropertyPath("FontStyle") };
                TextElement.SetBinding(ContentControl.FontStyleProperty, fontStyleBinding);
                Binding fontStretchBinding = new Binding { Source = this, Path = new PropertyPath("FontStretch") };
                TextElement.SetBinding(ContentControl.FontStretchProperty, fontStretchBinding);
                Binding fontFamilyBinding = new Binding { Source = this, Path = new PropertyPath("FontFamily") };
                TextElement.SetBinding(ContentControl.FontFamilyProperty, fontFamilyBinding);
                Binding fontWeightBinding = new Binding { Source = this, Path = new PropertyPath("FontWeight") };
                TextElement.SetBinding(ContentControl.FontWeightProperty, fontWeightBinding);
                Binding foregroundBinding = new Binding { Source = this, Path = new PropertyPath("Foreground") };
                TextElement.SetBinding(ContentControl.ForegroundProperty, foregroundBinding);
                Binding templateBinding = new Binding { Source = this, Path = new PropertyPath("ContentTemplate") };
                TextElement.SetBinding(ContentControl.ContentTemplateProperty, templateBinding);
            }
        }

        internal double ConvertData(object data, ChartAxis axis)
        {
            if (axis is NumericalAxis)
                return Convert.ToDouble(data);
            if (axis is DateTimeAxis)
            {
                if (data is DateTime)
                    return ((DateTime)data).ToOADate();
                else if (data is string)
                    return (Convert.ToDateTime(data.ToString())).ToOADate();
                else
                    return Convert.ToDouble(data);
            }
            if (axis is TimeSpanAxis)
            {
                if (data is TimeSpan)
                    return ((TimeSpan)data).TotalMilliseconds;
                else if (data is string)
                    return (TimeSpan.Parse(data.ToString())).TotalMilliseconds;
                else
                    return Convert.ToDouble(data);
            }
            if (axis is LogarithmicAxis)
                return Math.Log(Convert.ToDouble(data), (axis as LogarithmicAxis).LogarithmicBase);
            return Convert.ToDouble(data);
        }

        internal object ConvertToObject(double data, ChartAxis axis)
        {
            if (axis is DateTimeAxis)
                return (data).FromOADate();
            if (axis is TimeSpanAxis)
                return TimeSpan.FromMilliseconds(data);
            if (axis is LogarithmicAxis)
                return Math.Pow((axis as LogarithmicAxis).LogarithmicBase, data);
            return Convert.ToDouble(data);
        }

        protected Rect RotateElement(double angle, FrameworkElement item)
        {            
            double angleRadians = (2 * Math.PI * angle) / 360;
            double cos = Math.Cos(angleRadians);
            double sin = Math.Sin(angleRadians);
            var trfmGroup = new TransformGroup();
            var matrix = new MatrixTransform();
            _transformation = new Matrix(cos, sin, -sin, cos, 0, 0);
            double offsety = item.ActualHeight/ 2;
            _transformedDesiredElement = ElementTransform(new Rect(0, 0, item.ActualWidth, item.ActualHeight), _transformation);
            _transformedDesiredSize = new Size(_transformedDesiredElement.Width, _transformedDesiredElement.Height);
            matrix.Matrix = new Matrix(cos, sin, -sin, cos, 0, (_transformedDesiredElement.Height / 2) - offsety);
           
            return _transformedDesiredElement;
        }
        
        
        protected Point GetRotatePoint(double angle, FrameworkElement item,Point originalPoint)
        {
            double angleRadians = (2 * Math.PI * angle) / 360;
            double cos = Math.Cos(angleRadians);
            double sin = Math.Sin(angleRadians);
            var trfmGroup = new TransformGroup();
            var matrix = new MatrixTransform();
            _transformation = new Matrix(cos, sin, -sin, cos, 0, 0);
            double offsety = item.ActualHeight / 2;
            _transformedDesiredElement = ElementTransform(new Rect(0, 0, item.ActualWidth, item.ActualHeight), _transformation);
            _transformedDesiredSize = new Size(_transformedDesiredElement.Width, _transformedDesiredElement.Height);
            matrix.Matrix = new Matrix(cos, sin, -sin, cos, 0, (_transformedDesiredElement.Height / 2) - offsety);
            item.RenderTransformOrigin = new Point(0.5, 0.5);
            trfmGroup.Children.Add(matrix);
            item.RenderTransform = trfmGroup;
#if !NETFX_CORE
            return matrix.Transform(originalPoint);
#else
            return matrix.Matrix.Transform(originalPoint);
#endif
        }

        protected Rect RotateElement(double angle,FrameworkElement item, Size itemSize)
        {
            double angleRadians = (2 * Math.PI * angle) / 360;
            double cos = Math.Cos(angleRadians);
            double sin = Math.Sin(angleRadians);
            var trfmGroup = new TransformGroup();
            var matrix = new MatrixTransform();
            _transformation = new Matrix(cos, sin, -sin, cos, 0, 0);
            double offsety = itemSize.Height / 2;
            _transformedDesiredElement = ElementTransform(new Rect(0, 0, itemSize.Width, itemSize.Height), _transformation);
            _transformedDesiredSize = new Size(_transformedDesiredElement.Width, _transformedDesiredElement.Height);
            matrix.Matrix = new Matrix(cos, sin, -sin, cos, 0, (_transformedDesiredElement.Height / 2) - offsety);            
            trfmGroup.Children.Add(matrix);
          
            return _transformedDesiredElement;
        }

        private Rect ElementTransform(Rect rect, Matrix matrix)
        {
            Point leftTop = matrix.Transform(new Point(rect.Left, rect.Top));
            Point rightTop = matrix.Transform(new Point(rect.Right, rect.Top));
            Point leftBottom = matrix.Transform(new Point(rect.Left, rect.Bottom));
            Point rightBottom = matrix.Transform(new Point(rect.Right, rect.Bottom));
            double left = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
            double top = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
            double right = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
            double bottom = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));
            return new Rect(left, top, right - left, bottom - top);
        }

        protected Point EnsurePoint(Point point1, Point point2)
        {
            double x = point1.X;
            double y = point1.Y;            
            x = Math.Min(x, point2.X);
            y = Math.Min(y, point2.Y);

            return new Point(x, y);
        }

        protected virtual DependencyObject CloneAnnotation(Annotation annotation)
        {
            annotation.ContentTemplate = this.ContentTemplate;
            annotation.CoordinateUnit = this.CoordinateUnit;
            annotation.FontFamily = this.FontFamily;
            annotation.FontSize = this.FontSize;
            annotation.FontStyle = this.FontStyle;
            annotation.FontWeight = this.FontWeight;
            annotation.Foreground = this.Foreground;
            annotation.InternalHorizontalAlignment = this.InternalHorizontalAlignment;
            annotation.Text = this.Text;
            annotation.InternalVerticalAlignment = this.InternalVerticalAlignment;
            annotation.X1 = this.X1;
            annotation.Y1 = this.Y1;
            annotation.XAxisName = this.XAxisName;
            annotation.YAxisName = this.YAxisName;
            return annotation;
        }        

        public DependencyObject Clone()
        {
            return CloneAnnotation(null);
        }
    }
}
