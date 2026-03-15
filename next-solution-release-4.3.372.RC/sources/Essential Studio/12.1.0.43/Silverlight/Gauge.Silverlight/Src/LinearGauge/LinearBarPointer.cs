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
    using Syncfusion.Windows.Controls.Theming;

    /// <summary>
    /// Represents the linear pointer visual element.
    /// </summary>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <term>C# :</term></listheader>
    /// <item>
    /// <description>LinearBarPointer pointer = new LinearBarPointer(); 
    /// <para></para>
    /// <para>                 pointer.Background = new SolidColorBrush( Colors.Blue ); </para>
    /// <para></para>
    /// <para>                 pointer.PointerWidth = 6; </para>
    /// <para></para>
    /// <para>                 pointer.Value = 50; </para>
    /// <para></para>
    /// <para>                 scale.Pointers.Add( pointer );</para></description></item></list>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>&lt;syncfusion:LinearScale.Pointers&gt; 
    /// <para>      &lt;syncfusion:LinearBarPointer Background=&quot;Blue&quot; </para>
    /// <para>             PointerWidth=&quot;5&quot;  Value=&quot;50&quot;/&gt; </para>
    /// <para>  &lt;/syncfusion:LinearScale.Pointers&gt;   </para></description></item></list>
    /// </example>
    public class LinearBarPointer : LinearPointer
    {
        #region Private members

        /// <summary>
        /// The geometry used to draw the pointer.
        /// </summary>
        private RectangleGeometry mpointerGeometry;

        /// <summary>
        /// The geometry used to draw the pointer.
        /// </summary>
        internal Path mpointerPath;

        /// <summary>
        /// The geometry used to draw the selection pointer.
        /// </summary>
        internal Path mselecctionpointerPath;

        /// <summary>
        /// Temp variable for storing clicked event. 
        /// </summary>
        private bool mclick = false;

        private double tempval = 0;

        /// <summary>
        /// The path used to draw the scale.
        /// </summary>
        private Ellipse mGlassscalePath, mThermoscalePath;
        /// <summary>
        /// The key used to draw the scale.
        /// </summary>
        public  Key increaseKeyVal;
        /// <summary>
        /// The key used to draw the scale.
        /// </summary>
        public  Key decreaseKeyVal;


        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.LinearBarPointer">LinearBarPointer</see> class.
        /// </summary>
        public LinearBarPointer()
        {
            DefaultStyleKey = typeof(LinearBarPointer);
            this.Loaded += new RoutedEventHandler(this.LinearBarPointerLoaded);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mselecctionpointerPath = this.GetTemplateChild("PART_SelectionPointerPath") as Path;
            this.mpointerPath = this.GetTemplateChild("PART_PointerPath") as Path;
            this.mThermoscalePath = this.GetTemplateChild("PART_ThermoPointer") as Ellipse;
            this.mGlassscalePath = this.GetTemplateChild("PART_ThermoPointerGlass") as Ellipse;
            this.UpdateVisualStyle();
            this.RefreshPointerPath();
            if (this.EnablePointerInteraction)
            {
                EnableMouseEvents(this.mpointerPath);
            }
            else
            {
                DisableMouseEvents(this.mpointerPath);
            }
            this.TabIndex = 1;
           
        }

        #endregion

        #region Implementation

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
                        if ((scale.ScaleBarLength / 2) + poit >= scale.Offset && (scale.ScaleBarLength / 2) + poit + 2 * tempval <= scale.ScaleBarLength)
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
                        poit = pt.Y;
                    }
                    else
                    {
                        poit = -pt.X;
                    }
                    if ((scale.ScaleBarLength / 2) + poit >= scale.Offset && (scale.ScaleBarLength / 2) + poit + 2 * tempval <= scale.ScaleBarLength)
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
        /// Method to diable the MouseMove and Key board events
        /// </summary>
        /// <param name="path">path to which mouse events are added</param>
        internal void DisableMouseEvents(Path path)
        {
            this.Cursor = Cursors.Arrow;
            if (path != null)
            {
                path.MouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(LinearPointer_MouseLeftButtonDown);
                path.MouseLeftButtonUp -= new System.Windows.Input.MouseButtonEventHandler(LinearPointer_MouseLeftButtonUp);
                path.MouseMove -= new System.Windows.Input.MouseEventHandler(LinearPointer_MouseMove);
                this.KeyDown -= new KeyEventHandler(LinearBarPointer_KeyDown);
            }
        }

        /// <summary>
        /// Method to enable Mousemove and Key board Events events.
        /// </summary>
        /// <param name="path">path to which mouse events are removed</param>
        internal void EnableMouseEvents(Path path)
        {
            this.Cursor = Cursors.Hand;
            path.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(LinearPointer_MouseLeftButtonDown);
            path.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(LinearPointer_MouseLeftButtonUp);
            path.MouseMove += new System.Windows.Input.MouseEventHandler(LinearPointer_MouseMove);
            this.KeyDown += new KeyEventHandler(LinearBarPointer_KeyDown);
            this.GotFocus += new RoutedEventHandler(LinearBarPointer_GotFocus);
            this.LostFocus += new RoutedEventHandler(LinearBarPointer_LostFocus);
        }


        /// <summary>
        /// Method to do when pointer lost the focus
        /// </summary>
        void LinearBarPointer_LostFocus(object sender, RoutedEventArgs e)
        {
            LinearBarPointer pointer = (LinearBarPointer)sender;
            pointer.mselecctionpointerPath.Visibility = Visibility.Collapsed;
                       
        }


        /// <summary>
        /// Method to do when pointer got the focus
        /// </summary>
        void LinearBarPointer_GotFocus(object sender, RoutedEventArgs e)
        {
            LinearBarPointer pointer = (LinearBarPointer)this;
            pointer.mselecctionpointerPath.Visibility = Visibility.Visible;           
            pointer.Focus();

        }


        /// <summary>
        /// Method to Chane the value of the pointer on key press
        /// </summary>
        void LinearBarPointer_KeyDown(object sender, KeyEventArgs e)
        {
            
            this.Focus();
            LinearBarPointer  pointer;
            pointer = this as LinearBarPointer;
            if ((e.Key == Key.Tab) || ((e.Key == Key.Shift) && (e.Key == Key.Tab)))
            {
            }
            else
            {

                LinearScale scale = pointer.GaugeElementParent as LinearScale;
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
        /// Updates the position of the pointer.
        /// </summary>
        protected internal override void RefreshPointerPosition()
        {
            LinearScale scale = this.GaugeElementParent as LinearScale;
            if (scale != null)
            {               
                this.RefreshPointerPath();
                if (scale.Orientation == GaugeOrientation.Horizontal)
                {
                    TransformGroup gro = new TransformGroup();
                    TranslateTransform trans = new TranslateTransform();
                    trans.X = 0;
                    trans.Y = 0;
                    //(scale.ScaleBarLength / 2) - this.Position;
                    RotateTransform rot = new RotateTransform() { Angle = 90 };
                    gro.Children.Add(trans);
                    gro.Children.Add(rot);
                    this.RenderTransform = gro;
                }
            }
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
        // /// Raised when <see cref="PointerWidth"/> property is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnPointerWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPointerWidthChanged(e);
            this.RefreshPointerPosition();
        }

        /// <summary>
        /// Invoked when the Linear Bar Pointer is loaded
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void LinearBarPointerLoaded(object sender, RoutedEventArgs e)
        {
            LinearScale scale = this.GaugeElementParent as LinearScale;
            if (scale != null)
            {
                if (scale.ScaleType == ScaleTypes.Thermometer && (!scale.IsReversed))
                    tempval = 0;
            }
            this.RefreshPointerPosition();
        }

        /// <summary>
        /// Gets the geometry to draw the bar pointer.
        /// </summary>
        /// <returns>The geometry object.</returns>
        private RectangleGeometry GetBarPath(int val)
        {
            double tem = 0;
            if (val == 1)
                tem = -4;
            else
                tem = -2;

            this.mpointerGeometry = new RectangleGeometry();
            LinearScale scale = this.GaugeElementParent as LinearScale;
            if (scale != null)
            {
                double height = 0;
                if (scale.IsReversed)
                {
                    this.Position = this.Position < 0 ? 0 : this.Position;
                    this.mpointerGeometry.Rect = new Rect( -1 , scale.FrameOffset/2, this.PointerWidth + tem, this.Position + height + tem);

                    this.mpointerGeometry.RadiusX = scale.RadiusX;
                    this.mpointerGeometry.RadiusY = scale.RadiusY;
                    TranslateTransform transform = new TranslateTransform();
                    transform.X = 0;
                    transform.Y = -scale.ScaleBarLength / 2 + this.Position / 2 - this.PointerWidth / 2;
                    TranslateTransform transform1 = new TranslateTransform();
                    transform1.X = 1;
                    transform1.Y = -scale.ScaleBarLength / 2 + this.Position / 2 - this.PointerWidth / 2;
                    this.mpointerPath.RenderTransform = transform;
                    this.mselecctionpointerPath.RenderTransform = transform1;
                }
                else
                {
                    height = this.Position > 1 ? this.Position-1 : 1;
                    if (tempval < height)
                    {
                       // this.mpointerGeometry.Rect = new Rect( ? 0 : (-this.PointerWidth + 1), 0, this.PointerWidth + tem, this.Position + height + tem + 1);

                        this.mpointerGeometry.Rect = new Rect(((scale.GaugeElementParent as LinearGauge).Orientation == GaugeOrientation.Vertical) ? -tem / 2 : (this.PointerWidth + 1), scale.ScaleBarLength - height + (scale.ScaleType == ScaleTypes.Thermometer ? 2 * scale.Offset : 0), this.PointerWidth + tem, height + (scale.ScaleType == ScaleTypes.Thermometer ? 2 * scale.Offset : 0) + tem);
                        this.mpointerGeometry.RadiusX = scale.RadiusX;
                        this.mpointerGeometry.RadiusY = scale.RadiusY;
                    }

                }
            }

            return this.mpointerGeometry;
        }

        /// <summary>
        /// Refreshes the Pointer path
        /// </summary>
        private void RefreshPointerPath()
        {
            if (this.mpointerPath != null)
            {
                LinearScale scale = this.GaugeElementParent as LinearScale;
                if (scale != null)
                {
                    if (scale.ScaleType == ScaleTypes.Thermometer && (!scale.IsReversed))
                    {
                        tempval = 0;
                        this.mGlassscalePath.Visibility = Visibility.Visible;
                        this.mThermoscalePath.Visibility = Visibility.Visible;
                        this.mGlassscalePath.Height = this.PointerWidth;
                        this.mGlassscalePath.Width = this.PointerWidth;
                        TranslateTransform transform = new TranslateTransform();
                        transform.X = scale.Offset / 2;
                        transform.Y = scale.ScaleBarLength / 2 + scale.ScaleBarSize - this.PointerWidth + 2 * scale.Offset;
                        this.mThermoscalePath.RenderTransform = transform;
                        TranslateTransform transform1 = new TranslateTransform();
                        transform1.X = -1 * this.PointerWidth / 4;
                        transform1.Y = scale.ScaleBarLength / 2 + scale.ScaleBarSize - this.PointerWidth;
                        this.mGlassscalePath.RenderTransform = transform1;
                        this.mThermoscalePath.Height = 2 * (scale.ScaleBarSize - 2 * scale.Offset);
                        this.mThermoscalePath.Width = 2 * (scale.ScaleBarSize - 2 * scale.Offset);

                    }
                    else
                    {
                        this.mGlassscalePath.Visibility = Visibility.Collapsed;
                        this.mThermoscalePath.Visibility = Visibility.Collapsed;
                    }
                    double diff=scale.IsReversed ? -scale.FrameOffset / 2 : scale.FrameOffset / 2;
                    TranslateTransform trans = new TranslateTransform();
                    trans.X = scale.Orientation == GaugeOrientation.Vertical ? 0 : -this.PointerWidth/2-1;
                    trans.Y = scale.Orientation == GaugeOrientation.Vertical ? -diff : (-this.PointerWidth / 2 + scale.Offset / 2) - diff+1;
                    this.mpointerPath.RenderTransform = trans;
                    TranslateTransform strans = new TranslateTransform();
                    strans.X = (scale.ScaleType == ScaleTypes.Thermometer ? scale.Offset / 2 : scale.Orientation == GaugeOrientation.Vertical ? 0 : -this.PointerWidth/2);
                    strans.Y = scale.Orientation == GaugeOrientation.Vertical ? -diff : (-this.PointerWidth / 2 + scale.Offset / 2) - diff;
                    this.mselecctionpointerPath.RenderTransform = strans;
                    this.mpointerPath.Data = this.GetBarPath(1);
                    this.mselecctionpointerPath.Data = this.GetBarPath(2);
                    this.mselecctionpointerPath.Fill = this.PointerSelectionBrush;
                    LinearGauge gauge = scale.GaugeElementParent as LinearGauge;
                    if (SkinManager.GetVisualStyle(gauge).ToString() == "VS2010" && gauge.Orientation == GaugeOrientation.Vertical)
                    {
                        ResourceDictionary r = new ResourceDictionary();
                        r.Source = new Uri("/Syncfusion.Theming.VS2010;component/Gauge.xaml", UriKind.RelativeOrAbsolute);
                        this.Background = r["VerticalLinearBarBackgroundBrush"] as Brush;
                    }

                }
            }
        }

       
        #endregion
    }
}
