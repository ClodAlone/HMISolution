#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Gauge
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Input;

    /// <summary>
    /// Represents the pointer visual element.
    /// </summary>
    /// <example>
    /// C# : 
    /// <para>LinearMarkerPointer pointer = new LinearMarkerPointer(); </para>
    /// <para></para>
    /// <para>                 pointer.Background = new SolidColorBrush( Colors.Blue ); </para>
    /// <para></para>
    /// <para>                 pointer.PointerWidth = 6; </para>
    /// <para></para>
    /// <para>                 pointer.Value = 50; </para>
    /// <para></para>
    /// <para>                 scale.Pointers.Add( pointer ); </para>
    /// <para></para>
    /// <para>Xaml :</para>
    /// <para> &lt;syncfusion:LinearScale.Pointers&gt; </para>
    /// <para>      &lt;syncfusion:LinearMarkerPointer Background=&quot;Blue&quot; </para>
    /// <para>             PointerWidth=&quot;5&quot;  Value=&quot;50&quot;/&gt; </para>
    /// <para>  &lt;/syncfusion:LinearScale.Pointers&gt; </para>
    /// </example>
    public class LinearMarkerPointer : LinearPointer
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearMarkerPointer.MarkerStyle">MarkerStyle</see> dependency property.
        /// </summary>
        /// <remarks>
        /// <para>There are many Marker Style for the linear Gauge like <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Diamond">Diamond</see>, <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Rectangle">Rectangle</see>, <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Ellipse">Ellipse</see> ,<see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Trapezoid">Trapizoid</see> , <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Pentagon">Pentagon</see> and <see
        /// cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Triangle">Triangle</see>.The defaut Style is <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Rectangle">Rectangle</see></para>
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:Syncfusion.Windows.Gauge.MarkerStyle">MarkerStyle</see></para>
        /// </returns>
        public static readonly DependencyProperty MarkerStyleProperty =
            DependencyProperty.Register("MarkerStyle", typeof(MarkerStyle), typeof(LinearMarkerPointer), new PropertyMetadata(MarkerStyle.Rectangle, new PropertyChangedCallback(OnMarkerStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearMarkerPointer.PointerLength">PointerLength</see> dependency property.
        /// </summary>
        /// <remarks>
        /// The default value for PointerLength is 0d.
        /// </remarks>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty PointerLengthProperty =
            DependencyProperty.Register("PointerLength", typeof(double), typeof(LinearMarkerPointer), new PropertyMetadata(0d, new PropertyChangedCallback(OnPointerLengthChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearMarkerPointer.PointerPlacement">PointerPlacement</see> dependency property.
        /// </summary>
        /// <remarks>
        /// The default placement for marker pointer is <see cref="F:Syncfusion.Windows.Gauge.ScalePlacement.Inside">ScalePlacement.Inside</see>
        /// </remarks>
        /// <returns>
        /// Type : <see cref="T:Syncfusion.Windows.Gauge.ScalePlacement">ScalePlacement</see>
        /// </returns>
        public static readonly DependencyProperty PointerPlacementProperty =
            DependencyProperty.Register("PointerPlacement", typeof(ScalePlacement), typeof(LinearMarkerPointer), new PropertyMetadata(ScalePlacement.Inside, new PropertyChangedCallback(OnPointerPlacementChanged)));


    
        #endregion

        #region Private members

        /// <summary>
        /// The geometry used to draw the pointer.
        /// </summary>
        private Geometry mpointerGeometry;

        /// <summary>
        /// The geometry used to draw the pointer.
        /// </summary>
        internal Path mpointerPath;

        /// <summary>
        /// Temp variable for storing clicked event. 
        /// </summary>
        private bool mclick = false;

        private Brush pointerBrush = null;

        private Thickness pointerThickness;

        public Key increaseKeyVal;

        public Key decreaseKeyVal;

        
        
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.LinearMarkerPointer">LinearMarkerPointer</see> class.
        /// </summary>
        public LinearMarkerPointer()
        {
            DefaultStyleKey = typeof(LinearMarkerPointer);
            this.Loaded += new RoutedEventHandler(this.LinearMarkerPointerLoaded);
        }

        #endregion
               
        #region Events

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearMarkerPointer.MarkerStyle">MarkerStyle</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback MarkerStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearMarkerPointer.PointerLength">PointerLength</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerLengthChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearMarkerPointer.PointerPlacement">PointerPlacement</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerPlacementChanged;


       


        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets different shapes for the marker pointer. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is <see cref="F:Syncfusion.Windows.Gauge.MarkerStyle.Rectangle">MarkerStyle.Rectangle</see>.</para>
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:Syncfusion.Windows.Gauge.MarkerStyle">MarkertStyle</see>
        /// </value>
        /// <seealso cref="MarkerStyle">MarkerStyle</seealso>
        public MarkerStyle MarkerStyle
        {
            get
            {
                return (MarkerStyle)GetValue(MarkerStyleProperty);
            }

            set
            {
                SetValue(MarkerStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the pointer. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type :<see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double PointerLength
        {
            get
            {
                return (double)GetValue(PointerLengthProperty);
            }

            set
            {
                SetValue(PointerLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the placement of the marker pointer relatively to the scale. This is a dependency property.
        /// </summary>
        /// <remarks>
        ///  Default value is <see cref="F:Syncfusion.Windows.Gauge.ScalePlacement.Inside">ScalePlacement.Inside</see>.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:Syncfusion.Windows.Gauge.ScalePlacement">ScalePlacement</see>
        /// </value>
        /// <seealso cref="ScalePlacement">ScalePlacement</seealso>
        public ScalePlacement PointerPlacement
        {
            get
            {
                return (ScalePlacement)GetValue(PointerPlacementProperty);
            }

            set
            {
                SetValue(PointerPlacementProperty, value);
            }
        }

        #endregion

        void LinearPointer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.EnablePointerInteraction == true)
            {
                if (this.mclick == true)
                {
                    Path el = sender as Path;
                    if (this.GaugeElementParent is ScaleBase)
                    {
                        LinearScale scale = this.GaugeElementParent as LinearScale;
                        Point pt = e.GetPosition(scale);
                        double poit = 0;
                        if (scale.Orientation == GaugeOrientation.Vertical)
                        {
                            poit = pt.Y;
                        }
                        else
                        {
                            poit = -pt.X;
                        }
                        if ((scale.ScaleBarLength / 2) + poit >= scale.Offset && (scale.ScaleBarLength / 2) + poit <= scale.ScaleBarLength)
                        {
                            this.Position = (scale.ScaleBarLength / 2) - poit;

                            this.Value = this.GetValueByPosition(this.Position);
                            this.RefreshPointerPosition();
                        }
                    }
                }
            }
        }

        void LinearPointer_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(Path))
            {
                Path el = sender as Path;
                el.ReleaseMouseCapture();
                this.mclick = false;
            }
        }

        void LinearPointer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Focus();

            if (sender.GetType() == typeof(Path))
            {
                Path el = sender as Path;
                el.CaptureMouse();
                if (this.GaugeElementParent is ScaleBase)
                {
                    LinearScale scale = this.GaugeElementParent as LinearScale;
                    Point pt = e.GetPosition(scale);
                    double poit = 0;
                    if (scale.Orientation == GaugeOrientation.Vertical)
                    {
                        poit = pt.Y ;
                    }
                    else
                    {
                        poit = -pt.X;
                    }
                    if ((scale.ScaleBarLength / 2) + poit >= scale.Offset && (scale.ScaleBarLength / 2) + poit <= scale.ScaleBarLength)
                    {
                        this.Position = (scale.ScaleBarLength / 2) - poit;
                        this.Value = this.GetValueByPosition(this.Position);

                        this.RefreshPointerPosition();
                        this.mclick = true;
                    }
                }
            }
        }

        /// <summary>
        /// Coerces the value of the  property.
        /// </summary>
        /// <param name="d">The <see cref="T:Syncfusion.Windows.Gauge.LinearMarkerPointer">LinearMarkerPointer</see> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoercePointerLength(DependencyObject d, object value)
        {
            LinearMarkerPointer owner = d as LinearMarkerPointer;
            return owner.CoercePointerLength(value);
        }

        public double GetValueByPosition(double pos)
        {
            LinearScale scale = this.GaugeElementParent as LinearScale;
            double ratio = (scale.Maximum - scale.Minimum) / (scale.ScaleBarLength-scale.FrameOffset);
            if (pos == 0)
            {
                return scale.Minimum;
            }
            else 
            {
                if (scale.IsReversed)
                {
                    return scale.Maximum - (ratio * pos);
                }
                else
                {
                    return (ratio * pos) + scale.Minimum;
                }
            }            
        }

        #region Overrides

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.mpointerPath = this.GetTemplateChild("PART_PointerPath") as Path;
            this.UpdateVisualStyle();
           this.RefreshPointerPosition();

            base.OnApplyTemplate();
            if (this.EnablePointerInteraction)
            {
                EnableMouseEvents(this.mpointerPath);
            }
            else
            {
                DisableMouseEvents(this.mpointerPath);
            }
            this.TabIndex = 2;
           
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Method to diable the MouseMove and Keyb board events
        /// </summary>
        /// <param name="path">path to which mouse key board events are added</param>
        internal void DisableMouseEvents(Path path)
        {
            this.Cursor = Cursors.Arrow;
            if (path != null)
            {
                path.MouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(LinearPointer_MouseLeftButtonDown);
                path.MouseLeftButtonUp -= new System.Windows.Input.MouseButtonEventHandler(LinearPointer_MouseLeftButtonUp);
                path.MouseMove -= new System.Windows.Input.MouseEventHandler(LinearPointer_MouseMove);
            }
        }

        /// <summary>
        /// Method to enable Mousemove and Key board events.
        /// </summary>
        /// <param name="path">path to which mouse and key board events are removed</param>
        internal void EnableMouseEvents(Path path)
        {
            this.Cursor = Cursors.Hand;
            path.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(LinearPointer_MouseLeftButtonDown);
            path.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(LinearPointer_MouseLeftButtonUp);
            path.MouseMove += new System.Windows.Input.MouseEventHandler(LinearPointer_MouseMove);
            this.KeyDown += new KeyEventHandler(LinearMarkerPointer_KeyDown);
            this.GotFocus += new RoutedEventHandler(LinearMarkerPointer_GotFocus);
            this.LostFocus += new RoutedEventHandler(LinearMarkerPointer_LostFocus);
        }


        /// <summary>
        /// Method to do when pointer got focus
        /// </summary>
        void LinearMarkerPointer_LostFocus(object sender, RoutedEventArgs e)
        {

            LinearMarkerPointer pointer = (LinearMarkerPointer)sender;
            if (pointer.pointerBrush != null)
                pointer.BorderBrush = pointer.pointerBrush;

            if (pointer.pointerThickness != null)
                pointer.BorderThickness = pointer.pointerThickness;
           
        }


        /// <summary>
        /// Method to do when pointer lost the focus
        /// </summary>
        void LinearMarkerPointer_GotFocus(object sender, RoutedEventArgs e)
        {
            LinearMarkerPointer pointer = (LinearMarkerPointer)this;
            pointer.pointerBrush = pointer.BorderBrush;
            pointer.pointerThickness = pointer.BorderThickness;

            pointer.BorderThickness = new Thickness(1);

            pointer.pointerBrush = pointer.BorderBrush;
            pointer.BorderBrush = pointer.PointerSelectionBrush;
            pointer.Focus();
        }

        /// <summary>
        /// Method to change the value with Key Pressed
        /// </summary>
        void LinearMarkerPointer_KeyDown(object sender, KeyEventArgs e)
        {
            this.Focus();
            LinearMarkerPointer pointer;
            pointer = this as LinearMarkerPointer;
            if ((e.Key == Key.Tab) || ((e.Key == Key.Shift) && (e.Key == Key.Tab)))
            {
            }
            else
            {

                LinearScale scale = pointer.GaugeElementParent as LinearScale;
                if (scale != null)
                    if (scale != null)
                        if ((e.Key == Key.Left) || (e.Key == Key.Down) || (e.Key == pointer.decreaseKeyVal) || (e.Key == Key.PageDown))
                        {
                            e.Handled = true;
                            if (this.Value > scale.Minimum)
                                this.Value--;
                        }
                        else if ((e.Key == Key.Right) || (e.Key == Key.Up) || (e.Key == pointer.increaseKeyVal) || (e.Key == Key.PageUp))
                        {
                            e.Handled = true;
                            if (this.Value < scale.Maximum)
                                this.Value++;
                        }
                        else if (e.Key == Key.End)
                        {
                            e.Handled = true;
                            this.Value = scale.Maximum;
                        }
                        else if (e.Key == Key.Home)
                        {
                            e.Handled = true;
                            this.Value = scale.Minimum;
                        }
                    
            }
        }


        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="PointerLength"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoercePointerLength(object value)
        {
            double val = (double)value;
            if (val < 0)
            {
                val = 0;
            }

            return val;
        }

        /// <summary>
        /// Updates the position of the pointer.
        /// </summary>
        protected internal override void RefreshPointerPosition()
        {
            LinearScale scale = this.GaugeElementParent as LinearScale;
            if (scale != null)
            {
                double posX = 0;
                if (this.PointerPlacement == ScalePlacement.Cross)
                {
                    posX = -this.PointerLength / 2;
                }
                else if (this.PointerPlacement == ScalePlacement.Inside)
                {
                    posX = (-scale.scaleBarSize / 2) - this.PointerLength;
                }
                else if (this.PointerPlacement == ScalePlacement.Outside)
                {
                    posX = scale.scaleBarSize / 2;
                }

                TransformGroup transformGroup = new TransformGroup();
                RotateTransform rotateTransform = new RotateTransform();
                rotateTransform.CenterX = this.PointerLength / 2;
                rotateTransform.CenterY = this.PointerWidth / 2;
                if (this.PointerPlacement == ScalePlacement.Inside)
                {
                    rotateTransform.Angle = 180;
                }
                else
                {
                    rotateTransform.Angle = 0;
                }

                transformGroup.Children.Add(rotateTransform);
                TranslateTransform transform = new TranslateTransform();
                transform.X = posX;
                if (scale.scaleBarLength - this.Position >= 0)
                {
                    transform.Y = (scale.scaleBarLength / 2) - this.Position - (this.PointerWidth / 2);
                }

                transformGroup.Children.Add(transform);
                this.RenderTransform = transformGroup;
                this.RefreshPointerPath();

                if (scale.Orientation == GaugeOrientation.Horizontal)
                {
                    if (this.PointerPlacement == ScalePlacement.Cross)
                    {
                        posX = -this.PointerLength / 2;
                    }
                    else if (this.PointerPlacement == ScalePlacement.Inside)
                    {
                        posX = (-scale.ScaleBarSize / 2) - this.PointerLength;
                    }
                    else if (this.PointerPlacement == ScalePlacement.Outside)
                    {
                        posX = scale.ScaleBarSize / 2 + this.PointerLength/2;
                    }                    

                    TransformGroup gro = new TransformGroup();
                    TranslateTransform trans = new TranslateTransform();
                    trans.X = posX;
                    trans.Y = -this.Position + scale.ScaleBarLength / 2 - (this.PointerWidth / 2);
                    RotateTransform rot = new RotateTransform() { Angle = scale.Orientation==GaugeOrientation.Vertical?90:90 };
                    gro.Children.Add(trans);
                    gro.Children.Add(rot);
                    this.RenderTransform = gro;
                }
            }
        }

        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(this.PointerLength, this.PointerWidth);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MarkerStyleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMarkerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshPointerPath();
            if (this.MarkerStyleChanged != null)
            {
                this.MarkerStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerLengthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerLengthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is LinearScale)
            {
                LinearScale scale = this.GaugeElementParent as LinearScale;
                scale.RefreshScale();
                this.RefreshPointerPath();
                this.RefreshPointerPosition();
            }

            if (this.PointerLengthChanged != null)
            {
                this.PointerLengthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerPlacementChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerPlacementChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is LinearScale)
            {
                LinearScale scale = this.GaugeElementParent as LinearScale;
                scale.RefreshScale();
                this.RefreshPointerPosition();
            }

            if (this.PointerPlacementChanged != null)
            {
                this.PointerPlacementChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPointerWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnPointerWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPointerWidthChanged(e);
            this.RefreshPointerPath();
        }

        /// <summary>
        /// Calls OnMarkerStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMarkerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearMarkerPointer instance = (LinearMarkerPointer)d;
            instance.OnMarkerStyleChanged(e);
        }

        /// <summary>
        /// Calls OnPointerLengthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearMarkerPointer instance = (LinearMarkerPointer)d;
            instance.OnPointerLengthChanged(e);
        }

        /// <summary>
        /// Calls OnPointerPlacementChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearMarkerPointer instance = (LinearMarkerPointer)d;
            instance.OnPointerPlacementChanged(e);
        }

        /// <summary>
        /// Gets the geometry to draw the marker pointer.
        /// </summary>
        /// <returns>The geometry object.</returns>
        private Geometry GetMarkerPath()
        {
            Geometry value = null;

            if (this.MarkerStyle == MarkerStyle.Rectangle)
            {
                RectangleGeometry rect = new RectangleGeometry();
                rect.Rect = new Rect(0, 0, this.PointerLength, this.PointerWidth );
                value = rect;
            }
            else if (this.MarkerStyle == MarkerStyle.Ellipse)
            {
                EllipseGeometry ellipse = new EllipseGeometry();
                ellipse.Center = new Point(this.PointerLength / 2, this.PointerWidth / 2);
                ellipse.RadiusX = this.PointerLength / 2;
                ellipse.RadiusY = this.PointerWidth / 2;
                value = ellipse;
            }
            else if (this.MarkerStyle == MarkerStyle.Triangle)
            {
                PathGeometry pathGeom = new PathGeometry();

                PathFigure figure1 = new PathFigure();
                figure1.StartPoint = new Point(0, this.PointerWidth / 2);
                LineSegment line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(this.PointerLength, 0);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(this.PointerLength, this.PointerWidth);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);

                pathGeom.Figures.Add(figure1);

                value = pathGeom;
            }
            else if (this.MarkerStyle == MarkerStyle.Diamond)
            {
                PathGeometry pathGeom = new PathGeometry();

                PathFigure figure1 = new PathFigure();
                figure1.StartPoint = new Point(0, this.PointerWidth / 2);
                LineSegment line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(this.PointerLength / 2, 0);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(this.PointerLength, this.PointerWidth / 2);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(this.PointerLength / 2, this.PointerWidth);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);

                pathGeom.Figures.Add(figure1);

                value = pathGeom;
            }
            else if (this.MarkerStyle == MarkerStyle.Pentagon)
            {
                PathGeometry pathGeom = new PathGeometry();

                PathFigure figure1 = new PathFigure();
                figure1.StartPoint = new Point(0, this.PointerWidth / 2);
                LineSegment line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(this.PointerLength / 2, 0);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(this.PointerLength, this.PointerWidth / 4);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(this.PointerLength, 3 * this.PointerWidth / 4);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(this.PointerLength / 2, this.PointerWidth);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 2);
                figure1.Segments.Add(line);

                pathGeom.Figures.Add(figure1);

                value = pathGeom;
            }
            else if (this.MarkerStyle == MarkerStyle.Trapezoid)
            {
                PathGeometry pathGeom = new PathGeometry();

                PathFigure figure1 = new PathFigure();
                figure1.StartPoint = new Point(0, this.PointerWidth / 2);
                LineSegment line = new LineSegment();
                line.Point = new Point(0, this.PointerWidth / 4);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(this.PointerLength, 0);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(this.PointerLength, this.PointerWidth);
                figure1.Segments.Add(line);

                line = new LineSegment();
                line.Point = new Point(0, 3 * this.PointerWidth / 4);
                figure1.Segments.Add(line);

                pathGeom.Figures.Add(figure1);

                value = pathGeom;
            }

            return value;
        }

        /// <summary>
        /// Invoked when the control is ready for presentation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        private void LinearMarkerPointerLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshPointerPosition();
        }

        /// <summary>
        /// Refreshes the pointer path
        /// </summary>
        private void RefreshPointerPath()
        {
            if (this.mpointerPath != null)
            {
                this.mpointerGeometry = this.GetMarkerPath();

                this.mpointerPath.Data = this.mpointerGeometry;
            }
        }

        #endregion
    }
}
