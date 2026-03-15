// <copyright file="LinearMarkerPointer.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the pointer visual element which may assist the Bar pointer in denoting values.    
    /// </summary>
    /// <remarks>
    /// The LinearMarkerPointer and <see cref="LinearBarPointer"/> can be used hand-in-hand or
    /// independently to denote values on the Scale, it depends on one's personal preference.
    /// </remarks>
    /// <seealso cref="LinearBarPointer"/>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="LinearMarkerPointerSample.Window1" Title="LinearMarkerPointerSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:LinearGauge CenterFrameFillColor="Brown" Name="linearGauge1">
    ///             <syncfusion:LinearGauge.Scales>
    ///                 <syncfusion:LinearScale Name="LinearScale" Minimum="0" Maximum="100" 
    ///                                         MinorIntervalValue="2" MajorIntervalValue="10" 
    ///                                         ScaleBarSize="20" ScaleBarLength="260">
    ///                     <syncfusion:LinearScale.Pointers>
    ///                         <syncfusion:LinearBarPointer Name="barpointer" BackgroundBrush="Red" 
    ///                                                      BorderBrush="Black" PointerWidth="5" 
    ///                                                      Value="45" /> 
    ///                         <syncfusion:LinearMarkerPointer Value="{Binding Value,ElementName=barpointer}" 
    ///                                                         PointerLength="10" PointerWidth="10"
    ///                                                         PointerPlacement="Outside" 
    ///                                                         MarkerStyle="Triangle" 
    ///                                                         BackgroundBrush="Black" />
    ///                     </syncfusion:LinearScale.Pointers> 
    ///                 </syncfusion:LinearScale>
    ///             </syncfusion:LinearGauge.Scales>
    ///         </syncfusion:LinearGauge>
    ///     </Grid>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Media;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace LinearMarkerPointerSample
    /// {
    ///         public partial class Window1 : Window
    ///         {
    ///             private LinearGauge linearGauge1;<para/>
    ///             public Window1()
    ///             {                
    ///                 InitializeComponent();<para/>
    ///                 linearGauge1 = new LinearGauge();
    ///                 this.linearGauge1.CenterFrameFillColor = Colors.Brown;<para/>
    ///                 LinearScale scale = new LinearScale();
    ///                 scale.Minimum = 0;
    ///                 scale.Maximum = 100;
    ///                 scale.MinorIntervalValue = 2;
    ///                 scale.MajorIntervalValue = 10;
    ///                 scale.ScaleBarSize = 20;
    ///                 scale.ScaleBarLength = 260;
    ///                 linearGauge1.Scales.Add(scale);<para/>       
    ///                 LinearBarPointer pointer = new LinearBarPointer();
    ///                 pointer.BackgroundBrush = Brushes.Red;
    ///                 pointer.BorderBrush = new SolidColorBrush(Colors.Red);
    ///                 pointer.PointerWidth = 8;
    ///                 pointer.Value = 45;
    ///                 scale.Pointers.Add(pointer);<para/>
    ///                 LinearMarkerPointer markpointer = new LinearMarkerPointer();
    ///                 markpointer.BackgroundBrush = Brushes.Black;
    ///                 markpointer.PointerLength = 10;
    ///                 markpointer.PointerWidth = 10;
    ///                 markpointer.MarkerStyle = MarkerStyle.Triangle;
    ///                 markpointer.PointerPlacement = ScalePlacement.Outside;
    ///                 Binding bind = new Binding("Value");                
    ///                 bind.Source = pointer;
    ///                 markpointer.SetBinding(LinearMarkerPointer.ValueProperty, bind);
    ///                 scale.Pointers.Add(markpointer);<para/>
    ///                 this.Content = linearGauge1;
    ///             }
    ///         }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LinearMarkerPointer : LinearPointer
    {
        #region Private Members
        /// <summary>
        /// Stores the geometry used to draw the pointer.
        /// </summary>
        private Geometry m_pointerPath;

        private bool mclick = false;

        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double m_lengthsizeRatio;
        #endregion Private Members

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="MarkerStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MarkerStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="PointerLength"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerLengthChanged;

        /// <summary>
        /// Event that is raised when <see cref="PointerPlacement"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerPlacementChanged;

        /// <summary>
        /// Event that will raise when <see cref="MarkerCustomGeometry"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MarkerCustomGeometryChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="MarkerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerStyleProperty =
            DependencyProperty.Register("MarkerStyle", typeof(MarkerStyle), typeof(LinearMarkerPointer), new FrameworkPropertyMetadata(MarkerStyle.Rectangle, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnMarkerStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="PointerLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerLengthProperty =
            DependencyProperty.Register("PointerLength", typeof(double), typeof(LinearMarkerPointer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPointerLengthChanged), CoercePointerLength));

        /// <summary>
        /// Identifies the <see cref="PointerPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerPlacementProperty =
            DependencyProperty.Register("PointerPlacement", typeof(ScalePlacement), typeof(LinearMarkerPointer), new FrameworkPropertyMetadata(ScalePlacement.Inside, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnPointerPlacementChanged)));

        /// <summary>
        /// Identifies the <see cref="MarkerCustomGeometry"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerCustomGeometryProperty =
            DependencyProperty.Register("MarkerCustomGeometry", typeof(Geometry), typeof(LinearMarkerPointer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnMarkerCustomGeometryChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the different shapes for the marker pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="MarkerStyle"/>
        /// Default value is MarkerStyle.Rectangle.
        /// </value>
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
        /// Gets or sets the length of the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="LinearPointer.PointerWidth"/>
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
        /// Gets or sets the placement of the marker pointer
        /// relative to the scale. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ScalePlacement"/>
        /// Default value is ScalePlacement.Inside.
        /// </value>
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

        /// <summary>
        /// Gets or sets the custom geometry value for Marker.
        /// </summary>
        /// <value>
        /// Type : <see cref="Geometry"/>
        /// Default Value is Null.
        /// </value>
        public Geometry MarkerCustomGeometry
        {
            get
            {
                return (Geometry)GetValue(MarkerCustomGeometryProperty);
            }

            set
            {
                SetValue(MarkerCustomGeometryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ratio of ScaleBarSize to that of LinearGauge's Width.
        /// </summary>
        internal double LengthSizeRatio
        {
            get
            {
                return m_lengthsizeRatio;
            }

            set
            {
                m_lengthsizeRatio = value;
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="LinearMarkerPointer"/> class.
        /// Overrides meta data for default template.
        /// </summary>
        static LinearMarkerPointer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinearMarkerPointer), new FrameworkPropertyMetadata(typeof(LinearMarkerPointer)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearMarkerPointer"/> class.
        /// </summary>
        public LinearMarkerPointer()
        {
            this.Loaded += new RoutedEventHandler(LinearMarkerPointerLoaded);

        }
        #endregion Initialization

        #region Overrides
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
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.RefreshPointerPosition();
            m_pointerPath = this.GetMarkerPath();
            return finalSize;
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        /// <remarks>For Triangle-Inside placement, for Pentagon and for Trapezoid, we have
        /// to Rotate the geometry by 180 degree</remarks>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (m_pointerPath != null)
            {
                RotateTransform transform1;
                if (this.PointerPlacement == ScalePlacement.Inside)// && (this.MarkerStyle == MarkerStyle.Triangle || this.MarkerStyle == MarkerStyle.Pentagon || this.MarkerStyle == MarkerStyle.Trapezoid))
                {
                    transform1 = new RotateTransform(180, this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);
                    drawingContext.PushTransform(transform1);

                    drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_pointerPath);
                    drawingContext.Pop();
                }
                else
                {
                    transform1 = new RotateTransform(0, this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);
                    drawingContext.PushTransform(transform1);
                    drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_pointerPath);

                }
                if (this.IsKeyboardFocused)
                {
                    drawingContext.PushTransform(transform1);
                    drawingContext.DrawGeometry(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(Colors.Black), 0.5), m_pointerPath);
                }
                else
                {
                    drawingContext.PushTransform(transform1);
                    drawingContext.DrawGeometry(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(Colors.Transparent), 0.5), m_pointerPath);
                }


            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.UIElement.LostFocus"/> routed event by
        /// using the event data that is provided.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.RoutedEventArgs"/> that contains
        /// event data. This event data must contain the identifier for the <see cref="E:System.Windows.UIElement.LostFocus"/> event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.InvalidateVisual();
        }


        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.UIElement.GotFocus"/>
        /// event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains
        /// the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.FocusVisualStyle = null;
            this.InvalidateVisual();
        }
        /// <summary>
        /// Updates the position of the pointer.
        /// </summary>
        /// <remarks>The pointer which is drawn in the Center by OnRender method is transformed
        /// to the appropriate location</remarks>
        protected internal override void RefreshPointerPosition()
        {
            LinearScale scale = this.VisualParent as LinearScale;
            if (scale != null)
            {
                double posX = 0;
                double posY = 0;
                if (this.PointerPlacement == ScalePlacement.Cross)
                {
                    posX = -this.PointerLength / 2;
                    posY = (this.PointerLength > this.PointerWidth ? -(this.PointerLength - this.PointerWidth) : (this.PointerWidth - this.PointerLength));
                    posY = -this.PointerLength - (this.PointerLength / 2) + scale.ScaleBarSize / 32 - (posY / 2) + this.PointerWidth;
                }
                else if (this.PointerPlacement == ScalePlacement.Inside)
                {
                    posX = (-scale.ScaleBarSize / 2) - this.PointerLength;
                    posY = (this.PointerLength > this.PointerWidth ? -(this.PointerLength - this.PointerWidth) : (this.PointerWidth - this.PointerLength));
                    posY = -this.PointerLength - (this.PointerLength / 2) - scale.ScaleBarSize / 2 - (posY * 2) + this.PointerWidth / 2;
                }
                else if (this.PointerPlacement == ScalePlacement.Outside)
                {
                    posX = scale.ScaleBarSize / 2;
                    posY = (this.PointerLength > this.PointerWidth ? -(this.PointerLength - this.PointerWidth) : (this.PointerWidth - this.PointerLength));
                    posY = -this.PointerLength - (this.PointerLength / 2) + scale.ScaleBarSize / 2 - (posY) + this.PointerWidth + this.PointerWidth/2; 
                }
                if (scale.Orientation == GaugeOrientation.Vertical)
                {
                    if (this.MarkerStyle == MarkerStyle.Custom)
                    {
                        this.RenderTransform = new TranslateTransform(posX, scale.ScaleDirection == ScaleDirection.CounterClockwise ? ((scale.ScaleBarLength / 2) - this.Position - (this.PointerWidth / 20)) : -(scale.ScaleBarLength / 2) + this.Position - (this.PointerWidth / 25));
                    }
                    else
                    {
                        this.RenderTransform = new TranslateTransform(posX, scale.ScaleDirection == ScaleDirection.CounterClockwise ? ((scale.ScaleBarLength / 2) - this.Position - (this.PointerWidth / 2)) : -(scale.ScaleBarLength / 2) + this.Position - (this.PointerWidth / 2));
                    }
                }
                else
                {
                    if (this.MarkerStyle == MarkerStyle.Custom)
                    {
                        this.RenderTransform = new TranslateTransform(scale.ScaleDirection == ScaleDirection.CounterClockwise ? ((scale.ScaleBarLength / 2) - this.Position - (this.PointerLength / 20)) : -(scale.ScaleBarLength / 2) + this.Position - (this.PointerWidth / 20), posY);
                    }
                    else
                    {
                        this.RenderTransform = new TranslateTransform(scale.ScaleDirection == ScaleDirection.CounterClockwise ? ((scale.ScaleBarLength / 2) - this.Position - (this.PointerWidth / 2)) : -(scale.ScaleBarLength / 2) + this.Position - (this.PointerLength / 2), posY);
                    }
                }

            }
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Coerces the value of the <see cref="PointerLength"/> property.
        /// </summary>
        /// <param name="d">The <see cref="LinearMarkerPointer"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoercePointerLength(DependencyObject d, object value)
        {
            LinearMarkerPointer owner = d as LinearMarkerPointer;
            return owner.CoercePointerLength(value);
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
        /// Gets the geometry to draw the marker pointer.
        /// </summary>
        /// <returns>The geometry object.</returns>
        private Geometry GetMarkerPath()
        {
            PathGeometry path = new PathGeometry();

            if (this.MarkerStyle == MarkerStyle.Rectangle)
            {
                path.AddGeometry(new RectangleGeometry(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height)));
            }
            else if (this.MarkerStyle == MarkerStyle.Ellipse)
            {
                path.AddGeometry(new EllipseGeometry(
                    new Point(this.PointerLength / 2, this.PointerWidth / 2), this.PointerLength / 2, this.PointerWidth / 2));
            }
            else if (this.MarkerStyle == MarkerStyle.Triangle)
            {
                LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth / 2), true);
                LineSegment l2 = new LineSegment(new Point(this.PointerLength, 0), true);
                LineSegment l3 = new LineSegment(new Point(this.PointerLength, this.PointerWidth), true);
                LineSegment l4 = new LineSegment(new Point(0, this.PointerWidth / 2), true);
                PathFigure figure1 = new PathFigure(
                    new Point(0, this.PointerWidth / 2),
                    new PathSegment[] { l1, l2, l3, l4 },
                    true);
                path.Figures.Add(figure1);
            }
            else if (this.MarkerStyle == MarkerStyle.Diamond)
            {
                LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth / 2), true);
                LineSegment l2 = new LineSegment(new Point(this.PointerLength / 2, 0), true);
                LineSegment l3 = new LineSegment(new Point(this.PointerLength, this.PointerWidth / 2), true);
                LineSegment l4 = new LineSegment(new Point(this.PointerLength / 2, this.PointerWidth), true);
                LineSegment l5 = new LineSegment(new Point(0, this.PointerWidth / 2), true);
                PathFigure figure1 = new PathFigure(
                    new Point(0, this.PointerWidth / 2),
                    new PathSegment[] { l1, l2, l3, l4, l5 },
                    true);
                path.Figures.Add(figure1);
            }
            else if (this.MarkerStyle == MarkerStyle.Pentagon)
            {
                LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth / 2), true);
                LineSegment l2 = new LineSegment(new Point(this.PointerLength / 2, 0), true);
                LineSegment l3 = new LineSegment(new Point(this.PointerLength, this.PointerWidth / 4), true);
                LineSegment l4 = new LineSegment(new Point(this.PointerLength, 3 * this.PointerWidth / 4), true);
                LineSegment l5 = new LineSegment(new Point(this.PointerLength / 2, this.PointerWidth), true);
                LineSegment l6 = new LineSegment(new Point(0, this.PointerWidth / 2), true);
                PathFigure figure1 = new PathFigure(
                    new Point(0, this.PointerWidth / 2),
                    new PathSegment[] { l1, l2, l3, l4, l5, l6 },
                    true);
                path.Figures.Add(figure1);
            }
            else if (this.MarkerStyle == MarkerStyle.Trapezoid)
            {
                LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth / 4), true);
                LineSegment l2 = new LineSegment(new Point(this.PointerLength, 0), true);
                LineSegment l3 = new LineSegment(new Point(this.PointerLength, this.PointerWidth), true);
                LineSegment l4 = new LineSegment(new Point(0, 3 * this.PointerWidth / 4), true);
                PathFigure figure1 = new PathFigure(
                    new Point(0, this.PointerWidth / 2),
                    new PathSegment[] { l1, l2, l3, l4 },
                    true);
                path.Figures.Add(figure1);
            }
            else if (this.MarkerStyle == MarkerStyle.Custom)
            {
                if (MarkerCustomGeometry != null)
                {
                    return MarkerCustomGeometry;
                }
                else
                {
                    this.MarkerStyle = MarkerStyle.Triangle;
                    LineSegment l1 = new LineSegment(new Point(0, this.PointerWidth / 2), true);
                    LineSegment l2 = new LineSegment(new Point(this.PointerLength, 0), true);
                    LineSegment l3 = new LineSegment(new Point(this.PointerLength, this.PointerWidth), true);
                    LineSegment l4 = new LineSegment(new Point(0, this.PointerWidth / 2), true);
                    PathFigure figure1 = new PathFigure(
                        new Point(0, this.PointerWidth / 2),
                        new PathSegment[] { l1, l2, l3, l4 },
                        true);
                    path.Figures.Add(figure1);
                }
            }
            if (this.VisualParent is LinearScale)
            {
                LinearScale scale = this.VisualParent as LinearScale;
                if (scale.Orientation == GaugeOrientation.Horizontal)
                {
                    path.Transform = new RotateTransform(90, this.PointerWidth / 2, this.PointerLength / 2);
                }
            }


            return path;
        }

        /// <summary>
        /// Method to diable the MouseMove events
        /// </summary>
        /// <param name="path">path to which mouse events are added</param>
        internal void DisableMouseEvents(LinearMarkerPointer path)
        {
            this.Cursor = Cursors.Arrow;
            path.MouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(LinearMarkerPointer_MouseLeftButtonDown);
            path.MouseLeftButtonUp -= new System.Windows.Input.MouseButtonEventHandler(LinearMarkerPointer_MouseLeftButtonUp);
            path.MouseMove -= new System.Windows.Input.MouseEventHandler(LinearMarkerPointer_MouseMove);
            path.Focusable = false;
            path.KeyDown -= new KeyEventHandler(LinearPointer_KeyDown);
        }

        /// <summary>
        /// Method to enable Mousemove events.
        /// </summary>
        /// <param name="path">path to which mouse events are removed</param>
        internal void EnableMouseEvents(LinearMarkerPointer path)
        {
            this.Cursor = Cursors.Hand;
            path.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(LinearMarkerPointer_MouseLeftButtonDown);
            path.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(LinearMarkerPointer_MouseLeftButtonUp);
            path.MouseMove += new System.Windows.Input.MouseEventHandler(LinearMarkerPointer_MouseMove);
            path.Focusable = true;
            path.KeyDown += new KeyEventHandler(LinearPointer_KeyDown);
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
        /// Updates property value cache and raises <see cref="MarkerStyleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMarkerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MarkerStyleChanged != null)
            {
                this.MarkerStyleChanged(this, e);
            }
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
        /// Updates property value cache and raises <see cref="PointerLengthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerLengthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PointerLengthChanged != null)
            {
                this.PointerLengthChanged(this, e);
            }
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
        /// Updates property value cache and raises <see cref="PointerPlacementChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerPlacementChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PointerPlacementChanged != null)
            {
                this.PointerPlacementChanged(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="LinearMarkerPointer.MarkerCustomGeometryChanged"/> event
        /// </summary>
        /// <param name="e">Contains data related to the event</param>
        private void OnMarkerCustomGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MarkerCustomGeometryChanged != null)
            {
                this.OnMarkerCustomGeometryChanged(e);
            }
        }

        /// <summary>
        /// Calls OnMarkerCustomGeometryChanged method of the instance, notifies of the 
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMarkerCustomGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearMarkerPointer instance = (LinearMarkerPointer)d;
            instance.OnMarkerCustomGeometryChanged(e);
        }
        #endregion Implementation

        #region Added Code
        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="System.Windows.FrameworkElement.IsInitialized"/> property 
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            NameScope.SetNameScope(this, null);
            if (this.EnablePointerInteraction)
                this.EnableMouseEvents(this);
            else
                this.DisableMouseEvents(this);

        }

        /// <summary>
        /// Raises the KeyDown event when the Key is pressed during the pointer is in focus.
        /// </summary>
        /// <param name="sender">LinearPointer</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        void LinearPointer_KeyDown(object sender, KeyEventArgs e)
        {
            Keyboard.Focus(this);

            LinearPointer pointer = this as LinearPointer;

            if (this.VisualParent is ScaleBase)
            {
                LinearScale scale = this.VisualParent as LinearScale;
                if (e.Key == Key.Down || e.Key == Key.PageDown || e.Key == Key.Left || e.Key == DecrementKey)
                {
                    e.Handled = true;
                    if (this.Value > scale.Minimum)
                        this.Value--;

                }
                else if (e.Key == Key.Up || e.Key == Key.PageUp || e.Key == Key.Right || e.Key == IncrementKey)
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
        /// Raises the MouseMove event when the mouse cursor is moved on the pointer.
        /// </summary>
        /// <param name="sender">LinearMarkerPointer</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        void LinearMarkerPointer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.EnablePointerInteraction == true)
            {
                e.Handled = true;
                this.Cursor = Cursors.Hand;
                if (this.mclick == true)
                {
                    LinearMarkerPointer el = sender as LinearMarkerPointer;
                    if (this.VisualParent is ScaleBase)
                    {
                        LinearScale scale = this.VisualParent as LinearScale;
                        Point pt = e.GetPosition(scale);
                        double point = 0, length = 0;                        
                        if (scale.Orientation == GaugeOrientation.Horizontal)
                        {
                            point = pt.X;
                            length = -scale.ScaleBarLength / 2 + 7;                            
                            point = -(length) + point;
                        }
                        else
                        {
                            point = pt.Y;
                            length = scale.ScaleBarLength;                            

                        }                        
                            double tempvalue = this.GetValueByPosition(point);
                            this.Value = tempvalue;
                            this.RefreshPointerPosition();                       
                    }
                }
            }
        }

        /// <summary>
        /// Raises the MouseLeftButtonUp event when the mouse Left button is Up on the pointer.
        /// </summary>
        /// <param name="sender">LinearMarkerPointer</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        void LinearMarkerPointer_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(LinearMarkerPointer))
            {
                e.Handled = true;
                LinearMarkerPointer el = sender as LinearMarkerPointer;
                el.ReleaseMouseCapture();
                this.mclick = false;
            }
        }

        /// <summary>
        /// Raises the MouseLeftButtonDown event when the mouse Left button is down on the pointer.
        /// </summary>
        /// <param name="sender">LinearMarkerPointer</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        void LinearMarkerPointer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.EnablePointerInteraction)
            {
                Keyboard.Focus(this);


            }
            if (sender.GetType() == typeof(LinearMarkerPointer))
            {
                e.Handled = true;
                this.mclick = true;
                LinearMarkerPointer el = sender as LinearMarkerPointer;
                el.CaptureMouse();
                if (this.VisualParent is ScaleBase)
                {
                    LinearScale scale = this.VisualParent as LinearScale;
                    Point pt = e.GetPosition(scale);
                    double point = 0, length = 0;
                    bool istrue;
                    if (scale.Orientation == GaugeOrientation.Horizontal)
                    {
                        point = pt.X;
                        length = -scale.ScaleBarLength / 2 + 7;
                        if (length <= point && length + scale.ScaleBarLength >= point)
                            istrue = true;
                        else
                            istrue = false;
                        point = -(length) + point;

                    }
                    else
                    {
                        point = pt.Y;
                        length = scale.ScaleBarLength;
                        if (scale.ScaleBarLength + point >= -1 && point >= -1)
                            istrue = true;
                        else
                            istrue = false;
                    }
                    if (istrue)
                    {

                        this.Value = this.GetValueByPosition(point);
                        
                        this.RefreshPointerPosition();                        
                    }
                }
            }

        }


        /// <summary>
        /// Gets the corresponding value for Pointer position passed.
        /// </summary>
        /// <param name="pos">Pointer position.</param>
        /// <returns>Value.</returns>
        public double GetValueByPosition(double pos)
        {
            LinearScale scale = this.VisualParent as LinearScale;
            double ratio = (scale.Maximum - scale.Minimum) / (scale.ScaleBarLength - 0);
            if (pos == 0)
            {
                if (scale.ScaleDirection == ScaleDirection.Clockwise)
                {
                    return scale.Maximum;
                }
                else
                {
                    return scale.Minimum;
                }

            }
            else
            {
                if (scale.ScaleDirection == ScaleDirection.Clockwise)
                {
                    return (ratio * pos) + scale.Minimum;
                }
                else
                {
                    return scale.Maximum - (ratio * pos) + scale.Minimum;
                }
            }
        }

        /// <summary>
        /// Sets the scope to <see cref="CircularGauge"/>to facilitate Binding. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="gauge">The <see cref="LinearGauge"/> that contains the reference to the Gauge.</param>
        internal void SetScope(LinearGauge gauge)
        {
            CalculateScope(gauge);
        }

        /// <summary>
        /// Calculates the scope. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> which contains the element to set the scope for.</param>
        internal void CalculateScope(DependencyObject obj)
        {
            DependencyObject ele = obj;
            while (ele != null)
            {
                INameScope ns = NameScope.GetNameScope(ele);
                if (ns != null)
                {
                    if (!(ns is System.Windows.NameScope))
                    {
                        break;
                    }

                    NameScope.SetNameScope(this, ns);
                    break;
                }

                ele = LogicalTreeHelper.GetParent(ele) ?? VisualTreeHelper.GetParent(ele);
            }
        }
        #endregion Added Code
    }
}
