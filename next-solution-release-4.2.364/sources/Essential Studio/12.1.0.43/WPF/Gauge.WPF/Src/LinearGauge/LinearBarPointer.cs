// <copyright file="LinearBarPointer.cs" company="Syncfusion Software">
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
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the linear bar pointer visual element, which starts from the scale's minimum and goes up to
    /// the current value to be pointed out by the pointer.
    /// </summary>
    /// <remarks>
    /// The LinearBarPointer is animated using <see cref="DoubleAnimation"/>. Both LinearBarPointer and 
    /// <see cref="LinearMarkerPointer"/> can be used hand-in-hand or independently, it depends on one's
    /// personal preference.
    /// </remarks>
    /// <seealso cref="LinearMarkerPointer"/>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="LinearBarPointerSample.Window1" Title="LinearBarPointerSample" Height="400"
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
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace LinearBarPointerSample
    /// {
    ///         public partial class Window1 : Window
    ///         {
    ///             private LinearGauge linearGauge1;            
    ///             public Window1()
    ///             {                
    ///                 InitializeComponent();<para/>
    ///                 linearGauge1 = new LinearGauge();
    ///                 this.linearGauge1.CenterFrameFillColor = Colors.Brown;
    ///                 LinearScale scale = new LinearScale();
    ///                 scale.Minimum = 0;
    ///                 scale.Maximum = 100;
    ///                 scale.MinorIntervalValue = 2;
    ///                 scale.MajorIntervalValue = 10;
    ///                 scale.ScaleBarSize = 20;
    ///                 scale.ScaleBarLength = 260;
    ///                 linearGauge1.Scales.Add(scale);
    ///                 LinearBarPointer pointer = new LinearBarPointer();
    ///                 pointer.BackgroundBrush = Brushes.Red;
    ///                 pointer.BorderBrush = new SolidColorBrush(Colors.Red);
    ///                 pointer.PointerWidth = 8;
    ///                 pointer.Value = 45;
    ///                 scale.Pointers.Add(pointer);
    ///                 this.Content = linearGauge1;
    ///             }
    ///         }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LinearBarPointer : LinearPointer
    {
        #region Private Members
        /// <summary>
        /// The geometry used to draw the pointer.
        /// </summary>
        private Geometry pointerPath;

        private bool mclick = false;
        #endregion Private Members

        #region Events
        /// <summary>
        /// Event that will raise when <see cref="BarCustomGeometry"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback BarCustomGeometryChanged;

        /// <summary>
        /// Event that is raised when <see cref="BarStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback BarStyleChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="BarCustomGeometry"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BarCustomGeometryProperty =
            DependencyProperty.Register("BarCustomGeometry", typeof(Geometry), typeof(LinearBarPointer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnBarCustomGeometryChanged)));

        /// <summary>
        /// Identifies the <see cref="MarkerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BarStyleProperty =
            DependencyProperty.Register("BarStyle", typeof(BarStyle), typeof(LinearBarPointer), new FrameworkPropertyMetadata(BarStyle.Rectangle, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnBarStyleChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the custom geometry value for Bar pointer.
        /// </summary>
        /// <value>
        /// Type : <see cref="Geometry"/>
        /// Default Value is Null.
        /// </value>
        public Geometry BarCustomGeometry
        {
            get
            {
                return (Geometry)GetValue(BarCustomGeometryProperty);
            }

            set
            {
                SetValue(BarCustomGeometryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the different shapes for the bar pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="BarStyle"/>
        /// Default value is BarStyle.Rectangle.
        /// </value>
        public BarStyle BarStyle
        {
            get
            {
                return (BarStyle)GetValue(BarStyleProperty);
            }

            set
            {
                SetValue(BarStyleProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="LinearBarPointer"/> class.
        /// Overrides the meta data for default template.
        /// </summary>
        static LinearBarPointer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinearBarPointer), new FrameworkPropertyMetadata(typeof(LinearBarPointer)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearBarPointer"/> class.
        /// </summary>
        public LinearBarPointer()
        {
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
            return new Size(1, this.PointerWidth);
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            bool isThermometer = false;
            double scaleBarSize = 0;

            if (this.VisualParent is LinearScale)
            {
                LinearScale scale = this.VisualParent as LinearScale;
                if (scale.ScaleStyle == LinearScaleStyle.Thermometer)
                {
                    isThermometer = true;                    
                }

                scaleBarSize = scale.ScaleBarSize;
            }

            if (this.BarStyle == BarStyle.Thermometer || isThermometer)
            {
                this.pointerPath = new PathGeometry();
                double bpointerWidth = this.PointerWidth;
                double bpointerHeight = this.Position;
                if (scaleBarSize == 0)
                {
                    scaleBarSize = bpointerWidth;
                }

                Point topLeft = new Point(0, 0);
                Point topRight = new Point(bpointerWidth, 0);
                Point bottomRight = new Point(bpointerWidth, bpointerHeight);
                Point bottomCenter1 = new Point(bpointerWidth, bpointerHeight);
                Point bottomCenter2 = new Point(0, bpointerHeight);
                Point bottomLeft = new Point(0, bpointerHeight);

                LineSegment tl = new LineSegment(topLeft, true);
                LineSegment tr = new LineSegment(topRight, true);
                LineSegment br = new LineSegment(bottomRight, true);
                LineSegment bc1 = new LineSegment(bottomCenter1, true);
                ArcSegment abc2 = new ArcSegment(bottomCenter2, new Size(scaleBarSize - 1, scaleBarSize - 1), 0, true, SweepDirection.Clockwise, true);
                LineSegment bc2 = new LineSegment(bottomCenter2, true);
                LineSegment bl = new LineSegment(bottomLeft, true);

                PathFigure figure1 = new PathFigure(
                    new Point(0, 0),
                    new PathSegment[] { tr, br, bc1, abc2, bl, tl },
                    false);
                (this.pointerPath as PathGeometry).Figures.Add(figure1);
            }
            else if (this.BarStyle == BarStyle.Custom)
            {
                if (this.BarCustomGeometry != null)
                {
                    this.pointerPath = this.BarCustomGeometry;
                }
                else
                {
                    this.pointerPath = this.GetBarPath();
                }
            }
            else
            {
                this.pointerPath = this.GetBarPath();
            }

            return finalSize;
        }        

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        /// <remarks>Draws the rectangular bar pointer</remarks>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.pointerPath != null)
            {                
                drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), this.pointerPath);
                if (this.IsKeyboardFocused)
                {
                    drawingContext.DrawGeometry(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(Colors.Black), 0.5), this.pointerPath);
                }
                else
                {
                    drawingContext.DrawGeometry(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(Colors.Transparent), 0.5), this.pointerPath);
                }
            }

        }

        /// <summary>
        /// Updates the position of the pointer by transforming it to the correct position.
        /// The pointer is created to the appropriate height <para/>and width at the center point of the
        /// Gauge and then transformed.
        /// </summary>
        protected internal override void RefreshPointerPosition()
        {
            LinearScale scale = this.VisualParent as LinearScale;
            if (scale != null)
            {
                if (scale.Orientation == GaugeOrientation.Vertical)
                {
                    ResourceDictionary r = new ResourceDictionary();
                    if (SkinStorage.GetVisualStyle(this).ToString() == "VS2010")
                    {
                        r.Source = new Uri("/Syncfusion.Gauge.WPF;component/Themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
                        this.BackgroundBrush = r["VerticalLinearBarPointerBackgroundBrush"] as Brush;
                    }        
                    this.RenderTransform = new TranslateTransform(-this.PointerWidth / 2, scale.ScaleDirection == ScaleDirection.CounterClockwise ? (scale.ScaleBarLength / 2) - this.Position : -(scale.ScaleBarLength / 2));
                }
                else
                {
                    this.RenderTransform = new TranslateTransform(scale.ScaleDirection == ScaleDirection.CounterClockwise ? (scale.ScaleBarLength / 2) - this.Position : -(scale.ScaleBarLength / 2), -this.PointerWidth / 2);
                }
            }

        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Gets the geometry to draw the bar pointer.
        /// </summary>
        /// <returns>The geometry object.</returns>
        private Geometry GetBarPath()
        {
              this.pointerPath = new RectangleGeometry();
              if (this.VisualParent is LinearScale)
              {
                  LinearScale scale = this.VisualParent as LinearScale;
                  //(this.pointerPath as RectangleGeometry).Rect = new Rect(0, 0, this.PointerWidth, this.Position!=0?this.Position:1);
                  (this.pointerPath as RectangleGeometry).Rect = new Rect(0, 0, scale.Orientation == GaugeOrientation.Vertical ? this.PointerWidth :(this.Position!=0?this.Position:1), scale.Orientation == GaugeOrientation.Vertical ? (this.Position!=0?this.Position:1) : this.PointerWidth);
              }
               
              return this.pointerPath;
        }

        /// <summary>
        /// Method to diable the MouseMove events
        /// </summary>
        /// <param name="path">path to which mouse events are added</param>
        internal void DisableMouseEvents(LinearBarPointer path)
        {
            this.Cursor = Cursors.Arrow;
            path.MouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(LinearBarPointer_MouseLeftButtonDown);
            path.MouseLeftButtonUp -= new System.Windows.Input.MouseButtonEventHandler(LinearBarPointer_MouseLeftButtonUp);
            path.MouseMove -= new System.Windows.Input.MouseEventHandler(LinearBarPointer_MouseMove);
            path.Focusable = false;
            path.KeyDown -= new KeyEventHandler(LinearPointer_KeyDown);
        }

        /// <summary>
        /// Method to enable Mousemove events.
        /// </summary>
        /// <param name="path">path to which mouse events are removed</param>
        internal void EnableMouseEvents(LinearBarPointer path)
        {
            this.Cursor = Cursors.Hand;
            path.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(LinearBarPointer_MouseLeftButtonDown);
            path.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(LinearBarPointer_MouseLeftButtonUp);
            path.MouseMove += new System.Windows.Input.MouseEventHandler(LinearBarPointer_MouseMove);
            path.Focusable = true;
            path.KeyDown += new KeyEventHandler(LinearPointer_KeyDown);
        }

        /// <summary>
        /// Raised when <see cref="LinearPointer.PointerWidthProperty"/> is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnPointerWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPointerWidthChanged(e);
            this.RefreshPointerPosition();
        }

        /// <summary>
        /// Raises <see cref="LinearBarPointer.BarCustomGeometryChanged"/> event
        /// </summary>
        /// <param name="e">Contains data related to the event</param>
        private void OnBarCustomGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.BarCustomGeometryChanged != null)
            {
                this.OnBarCustomGeometryChanged(e);
            }
        }

        /// <summary>
        /// Calls OnBarCustomGeometryChanged method of the instance, notifies of the 
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBarCustomGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearBarPointer instance = (LinearBarPointer)d;
            instance.OnBarCustomGeometryChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="BarStyleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnBarStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.BarStyleChanged != null)
            {
                this.BarStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnBarStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBarStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearBarPointer instance = (LinearBarPointer)d;
            instance.OnBarStyleChanged(e);
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
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that
        /// contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.FocusVisualStyle = null;
            this.InvalidateVisual();
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
                if (e.Key == Key.Down || e.Key == Key.PageDown ||e.Key==Key.Left || e.Key==DecrementKey)
                {
                    e.Handled = true;
                    if (this.Value > scale.Minimum)
                        this.Value--;                    
                }
                else if (e.Key == Key.Up || e.Key == Key.PageUp ||e.Key==Key.Right || e.Key==IncrementKey)
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
        /// <param name="sender">LinearBarPointer</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        void LinearBarPointer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.EnablePointerInteraction == true)
            {
                e.Handled = true;
                this.Cursor = Cursors.Hand;
                if (this.mclick == true)
                {
                    LinearBarPointer el = sender as LinearBarPointer;
                    if (this.VisualParent is ScaleBase)
                    {
                        LinearScale scale = this.VisualParent as LinearScale;
                        Point pt = e.GetPosition(scale);
                        double point = 0, length = 0;
                        bool istrue;
                        if (scale.Orientation == GaugeOrientation.Horizontal)
                        {
                            point = pt.X;
                            length = -scale.ScaleBarLength / 2+7;
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
                            //if (istrue)
                            //    this.Position = scale.ScaleDirection == ScaleDirection.CounterClockwise ? (length) - point : point;


                        }
                        if (istrue)
                        {
                            double tempvalue = this.GetValueByPosition(point);
                            this.Value = tempvalue;
                            this.RefreshPointerPosition();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the MouseLeftButtonUp event when the mouse Left button is up on the pointer.
        /// </summary>
        /// <param name="sender">LinearBarPointer</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        void LinearBarPointer_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(LinearBarPointer))
            {
                e.Handled = true;
                LinearBarPointer el = sender as LinearBarPointer;
                el.ReleaseMouseCapture();
                this.mclick = false;
            }
        }

        /// <summary>
        /// Raises the  MouseLeftButtonDown event when the mouse Left button is down on the pointer.
        /// </summary>
        /// <param name="sender">LinearBarPointer</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        void LinearBarPointer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.EnablePointerInteraction)
            {
                Keyboard.Focus(this);
            }
            if (sender.GetType() == typeof(LinearBarPointer))
            {
                e.Handled = true;
                LinearBarPointer el = sender as LinearBarPointer;
                el.CaptureMouse();
                if (this.VisualParent is ScaleBase)
                {
                    LinearScale scale = this.VisualParent as LinearScale;
                    Point pt = e.GetPosition(scale);
                      double point = 0 ,length=0;
                        bool istrue;
                        if (scale.Orientation == GaugeOrientation.Horizontal)
                        {
                            point = pt.X;
                            length = -scale.ScaleBarLength / 2+7;
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
                            double tempvalue = this.GetValueByPosition(point);
                            this.Value = tempvalue;
                            this.RefreshPointerPosition();
                            this.mclick = true;
                        }
                }
            }
        }

        /// <summary>
        /// Gets the value with respect to the given position. 
        /// This method is invoked during pointer interaction.
        /// </summary>
        /// <param name="pos">The <see cref="LinearGauge"/> that contains the reference to the Gauge.</param>
        internal double GetValueByPosition(double pos)
        {
            LinearScale scale = this.VisualParent as LinearScale;
            double ratio = (scale.Maximum - scale.Minimum) / scale.ScaleBarLength;
            if (pos == 0)
            {
                if (scale.ScaleDirection == ScaleDirection.Clockwise)
                {
                    return scale.Minimum;
                }
                else
                {
                    return scale.Maximum;
                }
            }
            else
            {
                if (scale.ScaleDirection==ScaleDirection.Clockwise)
                {
                    return (ratio * pos) + scale.Minimum;
                }
                else
                {
                return scale.Maximum-(ratio * pos) + scale.Minimum;
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
            this.CalculateScope(gauge);
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
